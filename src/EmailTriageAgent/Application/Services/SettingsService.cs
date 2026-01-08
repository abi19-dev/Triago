using EmailTriageAgent.Domain;
using EmailTriageAgent.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EmailTriageAgent.Application.Services;

public sealed class SettingsService
{
    private readonly EmailTriageDbContext _dbContext;

    public SettingsService(EmailTriageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<SystemSettings> GetAsync(CancellationToken ct)
    {
        return _dbContext.SystemSettings.FirstAsync(ct);
    }

    public async Task<SystemSettings> UpdateAsync(SystemSettings update, CancellationToken ct)
    {
        var settings = await _dbContext.SystemSettings.FirstAsync(ct);
        settings.ReviewThreshold = update.ReviewThreshold;
        settings.BlockThreshold = update.BlockThreshold;
        settings.RetrainEnabled = update.RetrainEnabled;
        settings.GoldThreshold = update.GoldThreshold;
        settings.SpamKeywordsCsv = update.SpamKeywordsCsv;

        await _dbContext.SaveChangesAsync(ct);
        return settings;
    }
}
