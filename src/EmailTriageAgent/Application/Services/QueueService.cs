using EmailTriageAgent.Domain;
using EmailTriageAgent.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EmailTriageAgent.Application.Services;

public sealed class QueueService
{
    private readonly EmailTriageDbContext _dbContext;

    public QueueService(EmailTriageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EmailMessage> EnqueueAsync(EmailMessage message, CancellationToken ct)
    {
        message.Status = EmailStatus.Queued;
        message.ReceivedAt = DateTime.UtcNow;
        _dbContext.EmailMessages.Add(message);
        await _dbContext.SaveChangesAsync(ct);
        return message;
    }

    public async Task<EmailMessage?> DequeueNextAsync(CancellationToken ct)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);

        var message = await _dbContext.EmailMessages
            .Where(m => m.Status == EmailStatus.Queued)
            .OrderBy(m => m.ReceivedAt)
            .FirstOrDefaultAsync(ct);

        if (message == null)
        {
            return null;
        }

        message.Status = EmailStatus.Processing;
        message.ProcessingStartedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return message;
    }
}
