using EmailTriageAgent.Domain;
using EmailTriageAgent.Infrastructure;
using EmailTriageAgent.ML;
using Microsoft.EntityFrameworkCore;

namespace EmailTriageAgent.Application.Services;

public sealed class TrainingService
{
    private readonly EmailTriageDbContext _dbContext;
    private readonly IEmailClassifier _classifier;

    public TrainingService(EmailTriageDbContext dbContext, IEmailClassifier classifier)
    {
        _dbContext = dbContext;
        _classifier = classifier;
    }

    public async Task<ModelVersion> TrainModelAsync(CancellationToken ct)
    {
        var reviews = await _dbContext.Reviews
            .Include(r => r.EmailMessage)
            .ToListAsync(ct);

        var examples = reviews
            .Where(r => r.EmailMessage != null)
            .Select(r => new TrainingExample
            {
                Subject = r.EmailMessage!.Subject,
                Body = r.EmailMessage!.Body,
                IsSpam = r.Label == ReviewLabel.Spam
            })
            .ToList();

        await _classifier.TrainAsync(examples, ct);

        var modelVersion = new ModelVersion
        {
            Id = Guid.NewGuid(),
            Version = $"v{DateTime.UtcNow:yyyyMMddHHmmss}",
            TrainedAt = DateTime.UtcNow,
            IsActive = true,
            Notes = "Keyword classifier refresh"
        };

        foreach (var existing in _dbContext.ModelVersions)
        {
            existing.IsActive = false;
        }

        _dbContext.ModelVersions.Add(modelVersion);

        var settings = await _dbContext.SystemSettings.FirstAsync(ct);
        settings.ActiveModelVersionId = modelVersion.Id;
        settings.LastTrainedAt = modelVersion.TrainedAt;
        settings.NewGoldSinceLastTrain = 0;

        await _dbContext.SaveChangesAsync(ct);
        return modelVersion;
    }
}
