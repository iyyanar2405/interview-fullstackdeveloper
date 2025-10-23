using Microsoft.Extensions.Options;
using Module_05_Memory_Systems.Models;
using Module_05_Memory_Systems.Services;

namespace Module_05_Memory_Systems.HostedServices;

public sealed class MemoryConsolidationWorker : BackgroundService
{
    private readonly MemoryConsolidationService _consolidationService;
    private readonly MemorySystemsOptions _options;
    private readonly ILogger<MemoryConsolidationWorker> _logger;

    public MemoryConsolidationWorker(
        MemoryConsolidationService consolidationService,
        IOptions<MemorySystemsOptions> options,
        ILogger<MemoryConsolidationWorker> logger)
    {
        _consolidationService = consolidationService;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Memory consolidation worker starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consolidationService.RunCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running memory consolidation cycle");
            }

            var delay = Math.Max(250, _options.Simulation.ConsolidationIntervalMilliseconds);
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("Memory consolidation worker stopping.");
    }
}
