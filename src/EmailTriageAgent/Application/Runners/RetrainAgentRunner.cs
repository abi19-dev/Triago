using EmailTriageAgent.Application.Results;
using EmailTriageAgent.Application.Services;

namespace EmailTriageAgent.Application.Runners;

public sealed class RetrainAgentRunner
{
    private readonly SettingsService _settingsService;
    private readonly TrainingService _trainingService;

    public RetrainAgentRunner(SettingsService settingsService, TrainingService trainingService)
    {
        _settingsService = settingsService;
        _trainingService = trainingService;
    }

    public async Task<RetrainTickResult?> StepAsync(CancellationToken ct)
    {
        // SENSE
        var settings = await _settingsService.GetAsync(ct);
        if (!settings.RetrainEnabled || settings.NewGoldSinceLastTrain <= 0)
        {
            return null;
        }

        // THINK
        if (settings.NewGoldSinceLastTrain < settings.GoldThreshold)
        {
            return null;
        }

        // ACT + LEARN
        var modelVersion = await _trainingService.TrainModelAsync(ct);
        return new RetrainTickResult
        {
            ModelVersionId = modelVersion.Id,
            Version = modelVersion.Version,
            TrainedAt = modelVersion.TrainedAt
        };
    }
}
