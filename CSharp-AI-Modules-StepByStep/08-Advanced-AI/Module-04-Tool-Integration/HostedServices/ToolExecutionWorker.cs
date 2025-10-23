using Microsoft.Extensions.Options;
using Module_04_Tool_Integration.Models;
using Module_04_Tool_Integration.Services;

namespace Module_04_Tool_Integration.HostedServices;

public sealed class ToolExecutionWorker : BackgroundService
{
    private readonly ToolExecutionQueueService _queue;
    private readonly ToolExecutionOrchestrator _orchestrator;
    private readonly ToolIntegrationOptions _options;
    private readonly ILogger<ToolExecutionWorker> _logger;

    public ToolExecutionWorker(
        ToolExecutionQueueService queue,
        ToolExecutionOrchestrator orchestrator,
        IOptions<ToolIntegrationOptions> options,
        ILogger<ToolExecutionWorker> logger)
    {
        _queue = queue;
        _orchestrator = orchestrator;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Tool execution worker starting.");

        await foreach (var workItem in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _orchestrator.ExecuteAsync(workItem, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing tool {ToolId}", workItem.Request.ToolId);
            }

            await Task.Delay(_options.Simulation.WorkerIntervalMilliseconds, stoppingToken);
        }

        _logger.LogInformation("Tool execution worker stopping.");
    }
}
