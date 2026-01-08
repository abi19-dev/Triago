namespace EmailTriageAgent.Domain;

public sealed class Review
{
    public Guid Id { get; set; }
    public Guid EmailMessageId { get; set; }
    public EmailMessage? EmailMessage { get; set; }
    public ReviewLabel Label { get; set; }
    public string Reviewer { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
}
