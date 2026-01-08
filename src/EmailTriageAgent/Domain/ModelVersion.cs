namespace EmailTriageAgent.Domain;

public sealed class ModelVersion
{
    public Guid Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime TrainedAt { get; set; }
    public bool IsActive { get; set; }
    public string Notes { get; set; } = string.Empty;
}
