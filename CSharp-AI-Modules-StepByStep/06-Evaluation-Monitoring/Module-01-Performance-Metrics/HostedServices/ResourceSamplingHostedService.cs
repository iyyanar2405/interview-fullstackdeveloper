using Module_01_Performance_Metrics.Models;
using Module_01_Performance_Metrics.Services;
using Microsoft.Extensions.Options;

namespace Module_01_Performance_Metrics.HostedServices;

public sealed class ResourceSamplingHostedService : BackgroundService
{
    private readonly ILogger<ResourceSamplingHostedService> _logger;
    private readonly ResourceUtilizationService _resourceUtilizationService;
    private readonly RequestMetricsService _requestMetricsService;
    private readonly ResourceSamplingOptions _options;
    private readonly Process _process;
    private TimeSpan _lastTotalProcessorTime;
    private DateTimeOffset _lastSampleTime;

    public ResourceSamplingHostedService(
        ILogger<ResourceSamplingHostedService> logger,
        ResourceUtilizationService resourceUtilizationService,
        RequestMetricsService requestMetricsService,
        IOptions<ResourceSamplingOptions> options)
    {
        _logger = logger;
        _resourceUtilizationService = resourceUtilizationService;
        _requestMetricsService = requestMetricsService;
        _options = options.Value;
        _process = Process.GetCurrentProcess();
        _lastTotalProcessorTime = _process.TotalProcessorTime;
        _lastSampleTime = DateTimeOffset.UtcNow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(Math.Max(1, _options.IntervalSeconds));
        using var timer = new PeriodicTimer(interval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                CaptureSample();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to capture resource sample");
            }
        }
    }

    private void CaptureSample()
    {
        _process.Refresh();
        var now = DateTimeOffset.UtcNow;
        var totalProcessorTime = _process.TotalProcessorTime;
        var elapsed = (now - _lastSampleTime).TotalMilliseconds;
        var cpuTimeDelta = (totalProcessorTime - _lastTotalProcessorTime).TotalMilliseconds;
        var cpuUsage = elapsed <= 0 ? 0 : cpuTimeDelta / elapsed / Environment.ProcessorCount * 100.0;
        cpuUsage = Math.Clamp(cpuUsage, 0, 100);

        var snapshot = new ResourceSnapshot
        {
            Timestamp = now,
            CpuUsagePercentage = cpuUsage,
            MemoryUsageMb = _process.WorkingSet64 / 1024d / 1024d,
            ManagedMemoryMb = GC.GetTotalMemory(false) / 1024d / 1024d,
            ActiveThreads = _process.Threads.Count,
            ActiveRequests = _requestMetricsService.GetActiveRequests(),
            HandleCount = _process.HandleCount
        };

        _resourceUtilizationService.RecordSample(snapshot);
        _lastSampleTime = now;
        _lastTotalProcessorTime = totalProcessorTime;
    }
}
