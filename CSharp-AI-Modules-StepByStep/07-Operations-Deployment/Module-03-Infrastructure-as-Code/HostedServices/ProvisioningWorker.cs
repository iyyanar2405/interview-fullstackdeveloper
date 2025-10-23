using Microsoft.Extensions.Options;
using Module_03_Infrastructure_as_Code.Models;
using Module_03_Infrastructure_as_Code.Services;

namespace Module_03_Infrastructure_as_Code.HostedServices;

public sealed class ProvisioningWorker : BackgroundService
{
    private readonly ProvisioningQueueService _queue;
    private readonly ProvisioningHistoryService _history;
    private readonly DriftDetectionService _drift;
    private readonly ILogger<ProvisioningWorker> _logger;
    private readonly SimulationOptions _simulation;

    public ProvisioningWorker(
        ProvisioningQueueService queue,
        ProvisioningHistoryService history,
        DriftDetectionService drift,
        IOptions<InfrastructureOptions> options,
        ILogger<ProvisioningWorker> logger)
    {
        _queue = queue;
        _history = history;
        _drift = drift;
        _logger = logger;
        _simulation = options.Value.Simulation;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Provisioning worker started");
        await foreach (var item in _queue.ReadAllAsync(stoppingToken))
        {
            _history.Add(item);
            _history.Start(item.RunId);

            try
            {
                _history.Log(item.RunId, "Validating infrastructure plan");
                await DelayAsync(stoppingToken);

                _history.Log(item.RunId, "Applying infrastructure changes");
                await DelayAsync(stoppingToken);

                var drift = _drift.Estimate(item.Plan);
                _history.Log(item.RunId, "Finalizing run and checking drift");

                var compliance = item.Plan.Compliance.Count > 0
                    ? item.Plan.Compliance
                    : new List<ComplianceResult>();

                _history.Complete(item.RunId, item.Plan, drift, compliance);
                _logger.LogInformation("Provisioning run {RunId} for {Project} completed", item.RunId, item.Request.ProjectName);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _history.Fail(item.RunId, ex.Message);
                _logger.LogError(ex, "Provisioning run {RunId} failed", item.RunId);
            }
        }
    }

    private async Task DelayAsync(CancellationToken token)
    {
        var interval = Math.Max(1, _simulation.WorkerIntervalSeconds);
        await Task.Delay(TimeSpan.FromSeconds(interval), token);
    }
}
