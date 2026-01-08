using EmailTriageAgent.Application.Runners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmailTriageAgent.Web.Workers;

public sealed class ScoringWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ScoringWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<ScoringAgentRunner>();
            var result = await runner.StepAsync(ct);

            var delay = result == null ? TimeSpan.FromSeconds(2) : TimeSpan.FromMilliseconds(250);
            await Task.Delay(delay, ct);
        }
    }
}
