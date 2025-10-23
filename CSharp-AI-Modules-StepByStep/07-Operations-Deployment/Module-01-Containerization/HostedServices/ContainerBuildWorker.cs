using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Module_01_Containerization.Models;
using Module_01_Containerization.Services;

namespace Module_01_Containerization.HostedServices;

public sealed class ContainerBuildWorker : BackgroundService
{
    private readonly ContainerBuildQueueService _queue;
    private readonly ContainerBuildPlannerService _planner;
    private readonly ContainerBuildHistoryService _history;
    private readonly ILogger<ContainerBuildWorker> _logger;
    private readonly SimulationOptions _options;

    public ContainerBuildWorker(
        ContainerBuildQueueService queue,
        ContainerBuildPlannerService planner,
        ContainerBuildHistoryService history,
        IOptions<ContainerizationOptions> options,
        ILogger<ContainerBuildWorker> logger)
    {
        _queue = queue;
        _planner = planner;
        _history = history;
        _logger = logger;
        _options = options.Value.Simulation;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Container build worker started with concurrency {Concurrency}", _options.Concurrency);

        await foreach (var item in _queue.ReadAllAsync(stoppingToken))
        {
            var registry = SelectRegistry();
            var record = _history.AddRecord(item.Request, item.RequestedBy, registry, item.TemplateName);

            try
            {
                var plan = _planner.CreatePlan(item.TemplateName, item.Request, item.ComposeProfile);
                await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _options.ProcessingDelaySeconds)), stoppingToken);
                _history.CompleteRecord(record.BuildId, plan);
                _logger.LogInformation("Build {BuildId} completed for {Project}", record.BuildId, item.Request.ProjectName);
            }
            catch (Exception ex)
            {
                _history.FailRecord(record.BuildId, ex.Message);
                _logger.LogError(ex, "Build {BuildId} failed: {Message}", record.BuildId, ex.Message);
            }
        }
    }

    private string SelectRegistry()
    {
        if (_options.SampleRegistries.Count == 0)
        {
            return "registry.local/ai";
        }

        var index = Random.Shared.Next(_options.SampleRegistries.Count);
        return _options.SampleRegistries[index];
    }
}
