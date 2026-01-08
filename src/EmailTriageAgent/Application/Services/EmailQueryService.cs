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
}
