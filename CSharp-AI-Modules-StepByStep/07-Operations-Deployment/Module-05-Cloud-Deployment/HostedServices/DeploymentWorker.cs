using Microsoft.Extensions.Options;
using Module_05_Cloud_Deployment.Models;
using Module_05_Cloud_Deployment.Services;

namespace Module_05_Cloud_Deployment.HostedServices;

public sealed class DeploymentWorker : BackgroundService
{
    private readonly DeploymentQueueService _queue;
    private readonly DeploymentHistoryService _history;
    private readonly DeploymentSimulationService _simulation;
    private readonly ILogger<DeploymentWorker> _logger;
    private readonly SimulationSettings _settings;

    public DeploymentWorker(
        DeploymentQueueService queue,
        DeploymentHistoryService history,
        DeploymentSimulationService simulation,
        IOptions<CloudDeploymentOptions> options,
        ILogger<DeploymentWorker> logger)
    {
        _queue = queue;
        _history = history;
        _simulation = simulation;
        _logger = logger;
        _settings = options.Value.Simulation;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Deployment worker started");
        await foreach (var item in _queue.ReadAllAsync(stoppingToken))
        {
            _history.Add(item);
            _history.Start(item.RunId);

            try
            {
                _history.Log(item.RunId, "Validating release guardrails");
                await DelayAsync(stoppingToken);

                _history.Log(item.RunId, "Deploying infrastructure");
                await DelayAsync(stoppingToken);

                var timeline = _simulation.Simulate(item.Plan);
                _history.Log(item.RunId, "Recording deployment timeline");

                _history.Complete(item.RunId, timeline);
                _logger.LogInformation("Deployment run {RunId} completed for {Blueprint}", item.RunId, item.Plan.BlueprintName);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _history.Fail(item.RunId, ex.Message);
                _logger.LogError(ex, "Deployment run {RunId} failed", item.RunId);
            }
        }
    }

    private async Task DelayAsync(CancellationToken token)
    {
        var interval = Math.Max(1, _settings.WorkerIntervalSeconds);
        await Task.Delay(TimeSpan.FromSeconds(interval), token);
    }
}
