using EmailTriageAgent.Application;
using EmailTriageAgent.Application.LLM;
using EmailTriageAgent.Application.Services;
using EmailTriageAgent.Domain;
using EmailTriageAgent.ML;
using EmailTriageAgent.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EmailTriageAgent.Web.Controllers;

[ApiController]
[Route("api/llm")]
public sealed class LlmController : ControllerBase
{
    private readonly ILlmAnalyzer _analyzer;
    private readonly SettingsService _settingsService;
    private readonly DecisionRules _decisionRules;
    private readonly IEmailClassifier _classifier;

    public LlmController(
        ILlmAnalyzer analyzer,
        SettingsService settingsService,
        DecisionRules decisionRules,
        IEmailClassifier classifier)
    {
        _analyzer = analyzer;
        _settingsService = settingsService;
        _decisionRules = decisionRules;
        _classifier = classifier;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<LlmAnalyzeResponseDto>> AnalyzeAsync(
        [FromBody] LlmAnalyzeRequestDto dto,
        CancellationToken ct)
    {
        var message = new EmailMessage
        {
            Id = Guid.NewGuid(),
            Subject = dto.Subject,
            Body = dto.Body
        };

        var settings = await _settingsService.GetAsync(ct);
        var score = await _classifier.PredictAsync(message, settings, ct);
        var decision = _decisionRules.Decide(score, settings);

        var result = await _analyzer.AnalyzeAsync(
            message,
            score,
            decision.ToString(),
            settings.SpamKeywordsCsv,
            ct);
        var rationalePrefix =
            $"Agent decision uses thresholds (Review {settings.ReviewThreshold:0.00}, Block {settings.BlockThreshold:0.00}).";
        var combinedRationale = result.Rationale == null
            ? rationalePrefix
            : $"{rationalePrefix} LLM notes: {result.Rationale}";

        return Ok(new LlmAnalyzeResponseDto
        {
            Status = result.Status,
            SpamScore = score,
            Decision = decision.ToString(),
            Rationale = combinedRationale,
            RedFlags = result.RedFlags,
            Raw = result.Raw
        });
    }
}
