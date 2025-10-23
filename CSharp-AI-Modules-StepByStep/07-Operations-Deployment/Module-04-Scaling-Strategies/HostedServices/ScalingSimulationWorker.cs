using Microsoft.Extensions.Options;
using Module_04_Scaling_Strategies.Models;
using Module_04_Scaling_Strategies.Services;

namespace Module_04_Scaling_Strategies.HostedServices;

public sealed class ScalingSimulationWorker : BackgroundService
{
    private readonly ScalingQueueService _queue;
    private readonly ScalingHistoryService _history;
    private readonly ScalingSimulationService _simulation;
    private readonly ILogger<ScalingSimulationWorker> _logger;
    private readonly SimulationSettings _settings;

    public ScalingSimulationWorker(
        ScalingQueueService queue,
        ScalingHistoryService history,
        ScalingSimulationService simulation,
        IOptions<ScalingOptions> options,
        ILogger<ScalingSimulationWorker> logger)
    {
        _queue = queue;
        _history = history;
        _simulation = simulation;
        _logger = logger;
        _settings = options.Value.Simulation;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scaling simulation worker started");
        await foreach (var item in _queue.ReadAllAsync(stoppingToken))
        {
            _history.Add(item);
            _history.Start(item.RunId);

            try
            {
                _history.Log(item.RunId, "Evaluating scaling plan");
                await DelayAsync(stoppingToken);

                _history.Log(item.RunId, "Applying scaling decisions");
                await DelayAsync(stoppingToken);

                var timeline = _simulation.Simulate(item.Plan, item.Request);
                _history.Log(item.RunId, "Recording simulation timeline");
                _history.Complete(item.RunId, timeline);

                _logger.LogInformation(
                    "Scaling simulation {RunId} for profile {Profile} finished with {Steps} steps",
                    item.RunId,
                    item.Plan.ProfileName,
                    timeline.Count);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _history.Fail(item.RunId, ex.Message);
                _logger.LogError(ex, "Scaling simulation {RunId} failed", item.RunId);
            }
        }
    }

    private async Task DelayAsync(CancellationToken token)
    {
        var interval = Math.Max(1, _settings.WorkerIntervalSeconds);
        await Task.Delay(TimeSpan.FromSeconds(interval), token);
    }
}
