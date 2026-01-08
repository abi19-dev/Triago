namespace EmailTriageAgent.Web.Contracts;

public sealed class EmailCreateDto
{
    public string Sender { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
