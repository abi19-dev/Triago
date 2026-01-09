using AiAgents.Core;
using EmailTriageAgent.Application.Results;
using EmailTriageAgent.Application.Services;
using EmailTriageAgent.Domain;
using EmailTriageAgent.ML;

namespace EmailTriageAgent.Application.Runners;

public sealed class ScoringAgentRunner
    : SoftwareAgent<object, object, ScoringTickResult, object>
{
    private readonly QueueService _queueService;
    private readonly ScoringService _scoringService;
    private readonly DecisionRules _decisionRules;
    private readonly SettingsService _settingsService;
    private readonly IEmailClassifier _classifier;

    public ScoringAgentRunner(
        QueueService queueService,
        ScoringService scoringService,
        DecisionRules decisionRules,
        SettingsService settingsService,
        IEmailClassifier classifier)
    {
        _queueService = queueService;
        _scoringService = scoringService;
        _decisionRules = decisionRules;
        _settingsService = settingsService;
        _classifier = classifier;
    }

    public override async Task<ScoringTickResult?> StepAsync(CancellationToken ct)
    {
        // SENSE
        var message = await _queueService.DequeueNextAsync(ct);
        if (message == null)
        {
            return null;
        }

        var settings = await _settingsService.GetAsync(ct);

        // THINK
        var spamScore = await _classifier.PredictAsync(message, settings, ct);
        var decision = _decisionRules.Decide(spamScore, settings);

        // ACT
        await _scoringService.ApplyDecisionAsync(message, spamScore, decision, settings, ct);

        return new ScoringTickResult
        {
            EmailId = message.Id,
            Status = message.Status,
            Decision = decision,
            SpamScore = spamScore,
            ModelVersionId = settings.ActiveModelVersionId
        };
    }
}
