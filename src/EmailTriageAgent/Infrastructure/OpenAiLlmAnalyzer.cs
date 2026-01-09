using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EmailTriageAgent.Application.LLM;
using EmailTriageAgent.Domain;
using Microsoft.Extensions.Configuration;

namespace EmailTriageAgent.Infrastructure;

public sealed class OpenAiLlmAnalyzer : ILlmAnalyzer
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenAiLlmAnalyzer(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<LlmAnalysisResult> AnalyzeAsync(
        EmailMessage message,
        double agentScore,
        string agentDecision,
        string keywordsCsv,
        CancellationToken ct)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new LlmAnalysisResult
            {
                Status = "not_configured",
                Rationale = "OpenAI API key is missing. Add OpenAI:ApiKey in appsettings.json."
            };
        }

        var baseUrl = _configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com/v1";
        var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";

        var payload = new
        {
            model,
            response_format = new { type = "json_object" },
            messages = new[]
            {
                new
                {
                    role = "system",
                    content =
                        "You are an email security assistant. Explain the AGENT decision only. Return JSON only."
                },
                new
                {
                    role = "user",
                    content =
                        "Return JSON with fields: rationale (string) and red_flags (array). " +
                        $"Agent score: {agentScore:0.00}. Agent decision: {agentDecision}. " +
                        $"Agent keywords: {keywordsCsv}. " +
                        "Explain why the agent decided that outcome given the keywords and thresholds. " +
                        $"Subject: {message.Subject}\nBody: {message.Body}"
                }
            },
            temperature = 0.2
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            return new LlmAnalysisResult
            {
                Status = "error",
                Rationale = $"OpenAI error: {response.StatusCode}",
                Raw = responseBody
            };
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var content = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
            {
                return new LlmAnalysisResult { Status = "error", Rationale = "Empty response." };
            }

            if (!content.TrimStart().StartsWith("{", StringComparison.Ordinal))
            {
                return new LlmAnalysisResult
                {
                    Status = "error",
                    Rationale = "Model response was not JSON.",
                    Raw = content
                };
            }

            using var resultDoc = JsonDocument.Parse(content);
            var result = new LlmAnalysisResult
            {
                Rationale = resultDoc.RootElement.TryGetProperty("rationale", out var rationale)
                    ? rationale.GetString()
                    : null,
                RedFlags = resultDoc.RootElement.TryGetProperty("red_flags", out var flags)
                    ? flags.EnumerateArray().Select(x => x.GetString() ?? string.Empty).Where(x => x.Length > 0).ToList()
                    : new List<string>(),
                Raw = content
            };

            return result;
        }
        catch (JsonException)
        {
            return new LlmAnalysisResult
            {
                Status = "error",
                Rationale = "Failed to parse model response.",
                Raw = responseBody
            };
        }
    }
}
