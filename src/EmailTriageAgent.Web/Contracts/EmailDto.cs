using EmailTriageAgent.Domain;

namespace EmailTriageAgent.Web.Contracts;

public sealed class EmailDto
{
    public Guid Id { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public EmailStatus Status { get; set; }
    public TriageDecision? Decision { get; set; }
    public double? SpamScore { get; set; }
}
