namespace EmailTriageAgent.Domain;

public enum EmailStatus
{
    Queued = 0,
    Processing = 1,
    PendingReview = 2,
    Completed = 3,
    Blocked = 4,
    Reviewed = 5
}
