using EmailTriageAgent.Domain;

namespace EmailTriageAgent.ML;

public sealed class KeywordEmailClassifier : IEmailClassifier
{
    public Task<double> PredictAsync(EmailMessage message, SystemSettings settings, CancellationToken ct)
    {
        var keywords = ParseKeywords(settings.SpamKeywordsCsv);
        if (keywords.Count == 0)
        {
            return Task.FromResult(0.0);
        }

        var text = $"{message.Subject} {message.Body}";
        var hits = keywords.Count(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        var score = Math.Min(1.0, (double)hits / keywords.Count);
        return Task.FromResult(score);
    }

    public Task TrainAsync(IEnumerable<TrainingExample> examples, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    private static List<string> ParseKeywords(string csv)
    {
        return csv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(word => word.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
