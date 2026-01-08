namespace EmailTriageAgent.Domain;

public sealed class Prediction
{
    public Guid Id { get; set; }
    public Guid EmailMessageId { get; set; }
    public EmailMessage? EmailMessage { get; set; }
    public double SpamScore { get; set; }
    public TriageDecision Decision { get; set; }
    public Guid? ModelVersionId { get; set; }
    public ModelVersion? ModelVersion { get; set; }
    public DateTime CreatedAt { get; set; }
}
