using EmailTriageAgent.Domain;

namespace EmailTriageAgent.Application;

public sealed class DecisionRules
{
    public TriageDecision Decide(double spamScore, SystemSettings settings)
    {
        if (spamScore >= settings.BlockThreshold)
        {
            return TriageDecision.Block;
        }

        if (spamScore >= settings.ReviewThreshold)
        {
            return TriageDecision.Review;
        }

        return TriageDecision.Allow;
    }
}
