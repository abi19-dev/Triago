using EmailTriageAgent.Domain;

namespace EmailTriageAgent.Application.LLM;

public interface ILlmAnalyzer
{
    Task<LlmAnalysisResult> AnalyzeAsync(
        EmailMessage message,
        double agentScore,
        string agentDecision,
        string keywordsCsv,
        CancellationToken ct);
}
