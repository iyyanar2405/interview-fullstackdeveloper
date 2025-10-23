using Microsoft.Extensions.Options;
using Module_06_Production_Monitoring.Models;
using Module_06_Production_Monitoring.Services;

namespace Module_06_Production_Monitoring.HostedServices;

public sealed class MonitoringSimulationWorker : BackgroundService
{
    private readonly TelemetryStoreService _telemetry;
    private readonly AlertingService _alerting;
    private readonly IncidentService _incidents;
    private readonly SloEvaluationService _slos;
    private readonly ILogger<MonitoringSimulationWorker> _logger;
    private readonly SimulationSettings _settings;
    private readonly Random _random;

    public MonitoringSimulationWorker(
        TelemetryStoreService telemetry,
        AlertingService alerting,
        IncidentService incidents,
        SloEvaluationService slos,
        IOptions<MonitoringOptions> options,
        ILogger<MonitoringSimulationWorker> logger)
    {
        _telemetry = telemetry;
        _alerting = alerting;
        _incidents = incidents;
        _slos = slos;
        _logger = logger;
        _settings = options.Value.Simulation;
        _random = _settings.Seed.HasValue ? new Random(_settings.Seed.Value) : new Random();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Monitoring simulation worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                SimulateTelemetry();
                _slos.Evaluate();
                var alerts = _alerting.EvaluateAlerts();
                MaybeOpenIncidents(alerts);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Monitoring simulation loop failed");
            }

            var interval = Math.Max(1, _settings.MetricIntervalSeconds);
            await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
        }
    }

    private void SimulateTelemetry()
    {
        var timestamp = DateTimeOffset.UtcNow;
        foreach (var service in _settings.Services)
        {
            foreach (var region in _settings.Regions)
            {
                EmitMetric(service, region, "cpu", NextBetween(30, 95), "%", timestamp);
                EmitMetric(service, region, "latency", NextBetween(20, 350), "ms", timestamp);
                EmitMetric(service, region, "errors", NextBetween(0, 10), "count", timestamp);
                EmitMetric(service, region, "availability", NextBetween(96.5, 100), "%", timestamp);

                if (_random.NextDouble() < 0.25)
                {
                    EmitLog(service, region, timestamp);
                }

                if (_random.NextDouble() < 0.15)
                {
                    EmitTrace(service, region, timestamp);
                }

                if (_random.NextDouble() < 0.1)
                {
                    EmitEvent(service, region, timestamp);
                }
            }
        }
    }

    private void EmitMetric(string service, string region, string metric, double value, string unit, DateTimeOffset timestamp)
    {
        var sample = new TelemetryMetric
        {
            Timestamp = timestamp,
            Name = metric,
            Value = Math.Round(value, 2),
            Unit = unit,
            Entity = "service",
            EntityId = service,
            Dimensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "region", region }
            }
        };

        _telemetry.IngestMetric(sample);
    }

    private void EmitLog(string service, string region, DateTimeOffset timestamp)
    {
        var log = new TelemetryLog
        {
            Timestamp = timestamp,
            Level = _random.NextDouble() < 0.1 ? "Warning" : "Information",
            Message = $"{service} handled request",
            Source = "simulation",
            Entity = "service",
            EntityId = service,
            Context = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "region", region },
                { "requestId", Guid.NewGuid().ToString("N") }
            }
        };

        _telemetry.IngestLog(log);
    }

    private void EmitTrace(string service, string region, DateTimeOffset timestamp)
    {
        var spanCount = _random.Next(2, 6);
        var trace = new TelemetryTrace
        {
            StartTimestamp = timestamp,
            Duration = TimeSpan.FromMilliseconds(NextBetween(50, 400)),
            OperationName = "HTTP GET /api/resource",
            RootService = service,
            RootServiceId = service,
            Spans = new List<TelemetrySpan>()
        };

        for (var i = 0; i < spanCount; i++)
        {
            trace.Spans.Add(new TelemetrySpan
            {
                Service = service,
                ServiceId = service,
                Operation = $"dependency-{i}",
                Start = timestamp.AddMilliseconds(i * 10),
                Duration = TimeSpan.FromMilliseconds(NextBetween(5, 100)),
                Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "region", region },
                    { "status", "OK" }
                }
            });
        }

        _telemetry.IngestTrace(trace);
    }

    private void EmitEvent(string service, string region, DateTimeOffset timestamp)
    {
        var evt = new TelemetryEvent
        {
            Timestamp = timestamp,
            Type = "deployment",
            Severity = _random.NextDouble() < 0.05 ? "warning" : "info",
            Message = $"Deployment event for {service}",
            Entity = "service",
            EntityId = service,
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "region", region },
                { "deploymentSlot", _random.NextDouble() < 0.5 ? "blue" : "green" }
            }
        };

        _telemetry.IngestEvent(evt);
    }

    private void MaybeOpenIncidents(IReadOnlyCollection<AlertInstance> alerts)
    {
        if (alerts.Count == 0)
        {
            return;
        }

        foreach (var alert in alerts)
        {
            if (_random.NextDouble() <= _settings.IncidentLikelihood)
            {
                _logger.LogWarning("Opening simulated incident for alert {Alert}", alert.Name);
                _incidents.DeclareIncident(alert.Id, $"Simulated incident triggered by alert {alert.Name}", MapIncidentSeverity(alert.Severity));
            }
        }
    }

    private IncidentSeverity MapIncidentSeverity(AlertSeverity severity)
    {
        return severity switch
        {
            AlertSeverity.Low => IncidentSeverity.Low,
            AlertSeverity.Medium => IncidentSeverity.Medium,
            AlertSeverity.High => IncidentSeverity.High,
            AlertSeverity.Critical => IncidentSeverity.Critical,
            _ => IncidentSeverity.Medium
        };
    }

    private double NextBetween(double min, double max)
    {
        return min + (_random.NextDouble() * (max - min));
    }
}
