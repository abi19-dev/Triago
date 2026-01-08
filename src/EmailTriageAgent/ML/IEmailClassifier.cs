using EmailTriageAgent.Domain;

namespace EmailTriageAgent.ML;

public interface IEmailClassifier
{
    Task<double> PredictAsync(EmailMessage message, SystemSettings settings, CancellationToken ct);
    Task TrainAsync(IEnumerable<TrainingExample> examples, CancellationToken ct);
}
