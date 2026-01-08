using EmailTriageAgent.Domain;
using Microsoft.EntityFrameworkCore;

namespace EmailTriageAgent.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(EmailTriageDbContext dbContext, CancellationToken ct)
    {
        await dbContext.Database.EnsureCreatedAsync(ct);

        if (!await dbContext.SystemSettings.AnyAsync(ct))
        {
            dbContext.SystemSettings.Add(new SystemSettings
            {
                ReviewThreshold = 0.5,
                BlockThreshold = 0.8,
                RetrainEnabled = true,
                NewGoldSinceLastTrain = 0,
                GoldThreshold = 5,
                SpamKeywordsCsv = "free,discount,offer,win,urgent,click"
            });
            await dbContext.SaveChangesAsync(ct);
        }

        var settings = await dbContext.SystemSettings.FirstAsync(ct);
        if (!await dbContext.ModelVersions.AnyAsync(ct))
        {
            var model = new ModelVersion
            {
                Id = Guid.NewGuid(),
                Version = "v1",
                TrainedAt = DateTime.UtcNow,
                IsActive = true,
                Notes = "Initial keyword model"
            };

            dbContext.ModelVersions.Add(model);
            settings.ActiveModelVersionId = model.Id;
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
