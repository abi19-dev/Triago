namespace EmailTriageAgent.ML;

public sealed class TrainingExample
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsSpam { get; set; }
}
