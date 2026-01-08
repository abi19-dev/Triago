using EmailTriageAgent.Domain;

namespace EmailTriageAgent.Application.Results;

public sealed class ScoringTickResult
{
    public Guid EmailId { get; set; }
    public EmailStatus Status { get; set; }
    public TriageDecision Decision { get; set; }
    public double SpamScore { get; set; }
    public Guid? ModelVersionId { get; set; }
}
