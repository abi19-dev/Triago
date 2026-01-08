using EmailTriageAgent.Domain;

namespace EmailTriageAgent.Web.Contracts;

public sealed class ReviewCreateDto
{
    public Guid EmailId { get; set; }
    public ReviewLabel Label { get; set; }
    public string Reviewer { get; set; } = string.Empty;
}
