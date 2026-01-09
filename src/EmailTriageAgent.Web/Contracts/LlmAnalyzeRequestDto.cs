namespace EmailTriageAgent.Web.Contracts;

public sealed class LlmAnalyzeRequestDto
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
