using Module_02_Multi_Agent_Systems.Models;
using Module_02_Multi_Agent_Systems.Services;

namespace Module_02_Multi_Agent_Systems.HostedServices;

public sealed class CoordinationWorker : BackgroundService
{
    private readonly TaskDispatchQueueService _queue;
    private readonly CoordinationEngineService _engine;
    private readonly ILogger<CoordinationWorker> _logger;

    public CoordinationWorker(
        TaskDispatchQueueService queue,
        CoordinationEngineService engine,
        ILogger<CoordinationWorker> logger)
    {
        _queue = queue;
        _engine = engine;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Multi-agent coordination worker started");

        await foreach (var task in _queue.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation("Coordinating task {TaskId} for team {Team}", task.TaskId, task.TeamName);
            await _engine.CoordinateAsync(task, stoppingToken);
        }
    }
}
