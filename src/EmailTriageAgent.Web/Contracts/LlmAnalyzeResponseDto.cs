namespace EmailTriageAgent.Web.Contracts;

public sealed class LlmAnalyzeResponseDto
{
    public string Status { get; set; } = "ok";
    public double? SpamScore { get; set; }
    public string? Decision { get; set; }
    public string? Rationale { get; set; }
    public List<string> RedFlags { get; set; } = new();
    public string? Raw { get; set; }
}
