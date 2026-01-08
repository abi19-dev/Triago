namespace EmailTriageAgent.Domain;

public sealed class EmailMessage
{
    public Guid Id { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public EmailStatus Status { get; set; }
    public TriageDecision? Decision { get; set; }
    public DateTime? ProcessingStartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Prediction? Prediction { get; set; }
    public List<Review> Reviews { get; set; } = new();
}
