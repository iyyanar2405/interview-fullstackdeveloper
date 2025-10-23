using Module_01_Agentic_AI.Models;
using Module_01_Agentic_AI.Services;

namespace Module_01_Agentic_AI.HostedServices;

public sealed class AgentTaskWorker : BackgroundService
{
    private readonly AgentTaskQueueService _queue;
    private readonly AgentExecutionService _executor;
    private readonly ILogger<AgentTaskWorker> _logger;

    public AgentTaskWorker(
        AgentTaskQueueService queue,
        AgentExecutionService executor,
        ILogger<AgentTaskWorker> logger)
    {
        _queue = queue;
        _executor = executor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Agent task worker started");

        await foreach (var task in _queue.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation("Executing agent task {TaskId} for agent {Agent}", task.TaskId, task.AgentId);
            await _executor.ExecuteAsync(task, stoppingToken);
        }
    }
}
