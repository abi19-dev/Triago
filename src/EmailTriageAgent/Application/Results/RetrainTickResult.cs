namespace EmailTriageAgent.Application.Results;

public sealed class RetrainTickResult
{
    public Guid ModelVersionId { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime TrainedAt { get; set; }
}
