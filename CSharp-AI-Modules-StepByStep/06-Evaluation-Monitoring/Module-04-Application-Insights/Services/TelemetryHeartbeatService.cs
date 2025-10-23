using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Module_04_Application_Insights.Services;

/// <summary>
/// Periodically emits heartbeat metrics and availability pings to Application Insights.
/// </summary>
public sealed class TelemetryHeartbeatService : IHostedService, IDisposable
{
    private readonly TelemetryClient _client;
    private readonly ILogger<TelemetryHeartbeatService> _logger;
    private Timer? _timer;
    private int _sequence;

    public TelemetryHeartbeatService(TelemetryClient client, ILogger<TelemetryHeartbeatService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Telemetry heartbeat service starting");
        _timer = new Timer(EmitHeartbeat, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    private void EmitHeartbeat(object? state)
    {
        var sequence = Interlocked.Increment(ref _sequence);
        var metric = new MetricTelemetry("heartbeat.uptime.minutes", sequence)
        {
            Timestamp = DateTimeOffset.UtcNow
        };
        metric.Properties["environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        _client.TrackMetric(metric);

        var availability = new AvailabilityTelemetry
        {
            Name = "HeartbeatAvailability",
            Success = true,
            Duration = TimeSpan.FromMilliseconds(10),
            RunLocation = Environment.MachineName,
            Message = "Heartbeat check"
        };
        _client.TrackAvailability(availability);
        _logger.LogDebug("Heartbeat #{Sequence} emitted", sequence);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Telemetry heartbeat service stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
