using EmailTriageAgent.Domain;
using EmailTriageAgent.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EmailTriageAgent.Application.Services;

public sealed class ReviewService
{
    private readonly EmailTriageDbContext _dbContext;

    public ReviewService(EmailTriageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Review> SubmitReviewAsync(Guid emailId, ReviewLabel label, string reviewer, CancellationToken ct)
    {
        var message = await _dbContext.EmailMessages.FirstOrDefaultAsync(m => m.Id == emailId, ct);
        if (message == null)
        {
            throw new InvalidOperationException("Email not found.");
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),
            EmailMessageId = emailId,
            Label = label,
            Reviewer = reviewer,
            ReviewedAt = DateTime.UtcNow
        };

        message.Status = EmailStatus.Reviewed;
        message.Decision = label == ReviewLabel.Spam ? TriageDecision.Block : TriageDecision.Allow;
        message.CompletedAt = DateTime.UtcNow;

        var settings = await _dbContext.SystemSettings.FirstAsync(ct);
        settings.NewGoldSinceLastTrain += 1;

        _dbContext.Reviews.Add(review);
        await _dbContext.SaveChangesAsync(ct);
        return review;
    }
}
