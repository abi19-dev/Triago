namespace AiAgents.Core;

public abstract class SoftwareAgent<TPercept, TAction, TResult, TExperience>
{
    public abstract Task<TResult?> StepAsync(CancellationToken ct);

    public virtual Task LearnAsync(TExperience experience, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}

public interface IPerceptionSource<TPercept>
{
    Task<TPercept?> SenseAsync(CancellationToken ct);
}

public interface IPolicy<TPercept, TAction>
{
    Task<TAction> DecideAsync(TPercept percept, CancellationToken ct);
}

public interface IActuator<TAction, TResult>
{
    Task<TResult> ActAsync(TAction action, CancellationToken ct);
}

public interface ILearningComponent<TExperience>
{
    Task LearnAsync(TExperience experience, CancellationToken ct);
}
