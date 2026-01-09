using EmailTriageAgent.Domain;
using EmailTriageAgent.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EmailTriageAgent.Application.Services;

public sealed class EmailQueryService
{
    private readonly EmailTriageDbContext _dbContext;

    public EmailQueryService(EmailTriageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<EmailMessage>> GetRecentAsync(int take, CancellationToken ct)
    {
        return _dbContext.EmailMessages
            .Include(m => m.Prediction)
            .OrderByDescending(m => m.ReceivedAt)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var message = await _dbContext.EmailMessages
            .Include(m => m.Prediction)
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id, ct);
        if (message == null)
        {
            return false;
        }

        if (message.Prediction != null)
        {
            _dbContext.Predictions.Remove(message.Prediction);
        }

        if (message.Reviews.Count > 0)
        {
            _dbContext.Reviews.RemoveRange(message.Reviews);
        }

        _dbContext.EmailMessages.Remove(message);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}
