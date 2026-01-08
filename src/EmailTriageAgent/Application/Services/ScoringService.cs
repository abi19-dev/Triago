using EmailTriageAgent.Domain;
using EmailTriageAgent.Infrastructure;

namespace EmailTriageAgent.Application.Services;

public sealed class ScoringService
{
    private readonly EmailTriageDbContext _dbContext;

    public ScoringService(EmailTriageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Prediction> ApplyDecisionAsync(
        EmailMessage message,
        double spamScore,
        TriageDecision decision,
        SystemSettings settings,
        CancellationToken ct)
    {
        var prediction = new Prediction
        {
            Id = Guid.NewGuid(),
            EmailMessageId = message.Id,
            SpamScore = spamScore,
            Decision = decision,
            ModelVersionId = settings.ActiveModelVersionId,
            CreatedAt = DateTime.UtcNow
        };

        message.Decision = decision;
        message.Status = decision switch
        {
            TriageDecision.Allow => EmailStatus.Completed,
            TriageDecision.Block => EmailStatus.Blocked,
            _ => EmailStatus.PendingReview
        };
        message.CompletedAt = message.Status == EmailStatus.PendingReview ? null : DateTime.UtcNow;

        _dbContext.Predictions.Add(prediction);
        await _dbContext.SaveChangesAsync(ct);
        return prediction;
    }
}
