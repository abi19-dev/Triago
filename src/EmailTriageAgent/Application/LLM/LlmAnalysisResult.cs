namespace EmailTriageAgent.Application.LLM;

public sealed class LlmAnalysisResult
{
    public string Status { get; set; } = "ok";
    public double? SpamScore { get; set; }
    public string? Decision { get; set; }
    public string? Rationale { get; set; }
    public List<string> RedFlags { get; set; } = new();
    public string? Raw { get; set; }
}
