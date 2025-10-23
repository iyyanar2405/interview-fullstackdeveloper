using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using Module_06_Production_Monitoring.Models;

namespace Module_06_Production_Monitoring.Services;

public sealed class TelemetryStoreService
{
    private readonly ConcurrentQueue<TelemetryEvent> _events = new();
    private readonly ConcurrentDictionary<string, TelemetryMetric> _metrics = new();
    private readonly ConcurrentDictionary<string, TelemetryLog> _logs = new();
    private readonly ConcurrentDictionary<string, TelemetryTrace> _traces = new();
    private readonly ILogger<TelemetryStoreService> _logger;

    public TelemetryStoreService(ILogger<TelemetryStoreService> logger)
    {
        _logger = logger;
    }

    public void IngestEvent(TelemetryEvent evt)
    {
        _events.Enqueue(Clone(evt));
        TrimQueue(_events, 10_000);
        _logger.LogDebug("Ingested telemetry event {Id} for {Entity}", evt.Id, evt.EntityId);
    }

    public void IngestMetric(TelemetryMetric metric)
    {
        var copy = Clone(metric);
        _metrics[copy.Id] = copy;
        _logger.LogDebug("Recorded telemetry metric {Metric}", metric.Id);
    }

    public void IngestLog(TelemetryLog log)
    {
        var copy = Clone(log);
        _logs[copy.Id] = copy;
        _logger.LogDebug("Recorded telemetry log {LogId}", log.Id);
    }

    public void IngestTrace(TelemetryTrace trace)
    {
        var copy = Clone(trace);
        _traces[copy.Id] = copy;
        _logger.LogDebug("Recorded telemetry trace {TraceId}", trace.Id);
    }

    public IReadOnlyCollection<TelemetryEvent> GetRecentEvents(int count = 100)
    {
        return _events.Take(count).Select(Clone).ToArray();
    }

    public IReadOnlyCollection<TelemetryMetric> FilterMetrics(Func<TelemetryMetric, bool> predicate)
    {
        return _metrics.Values.Where(predicate).Select(Clone).ToArray();
    }

    public IReadOnlyCollection<TelemetryLog> FilterLogs(Func<TelemetryLog, bool> predicate)
    {
        return _logs.Values.Where(predicate).Select(Clone).ToArray();
    }

    public IReadOnlyCollection<TelemetryTrace> FilterTraces(Func<TelemetryTrace, bool> predicate)
    {
        return _traces.Values.Where(predicate).Select(Clone).ToArray();
    }

    public MonitoringSnapshot CaptureSnapshot()
    {
        return new MonitoringSnapshot
        {
            CapturedAt = DateTimeOffset.UtcNow,
            ActiveMetrics = _metrics.Values.Select(Clone).ToList(),
            Alerts = new List<AlertInstance>(),
            AlertsTriggered = new List<AlertInstance>(),
            CurrentIncidents = new List<ActiveIncident>(),
            RecentEvents = _events.Take(200).Select(Clone).ToList(),
            Slos = new List<SloStatus>(),
            ServiceHealth = CalculateServiceHealth(_metrics.Values)
        };
    }

    private static ServiceHealthScore CalculateServiceHealth(IEnumerable<TelemetryMetric> metrics)
    {
        var availability = metrics.Where(m => m.Name.Equals("availability", StringComparison.OrdinalIgnoreCase)).ToList();
        if (availability.Count == 0)
        {
            return new ServiceHealthScore
            {
                ServiceName = "unknown",
                CurrentScore = 0.95,
                Trend = "steady"
            };
        }

        var average = availability.Average(m => m.Value / 100.0);
        return new ServiceHealthScore
        {
            ServiceName = availability[0].EntityId,
            CurrentScore = Math.Round(average, 3),
            Trend = average switch
            {
                >= 0.98 => "improving",
                <= 0.9 => "declining",
                _ => "steady"
            }
        };
    }

    private static void TrimQueue<T>(ConcurrentQueue<T> queue, int maxSize)
    {
        while (queue.Count > maxSize && queue.TryDequeue(out _))
        {
        }
    }

    private static TelemetryEvent Clone(TelemetryEvent evt)
    {
        return new TelemetryEvent
        {
            Id = evt.Id,
            Timestamp = evt.Timestamp,
            Type = evt.Type,
            Severity = evt.Severity,
            Message = evt.Message,
            Entity = evt.Entity,
            EntityId = evt.EntityId,
            Metadata = new Dictionary<string, string>(evt.Metadata, StringComparer.OrdinalIgnoreCase)
        };
    }

    private static TelemetryMetric Clone(TelemetryMetric metric)
    {
        return new TelemetryMetric
        {
            Id = metric.Id,
            Timestamp = metric.Timestamp,
            Name = metric.Name,
            Value = metric.Value,
            Unit = metric.Unit,
            Entity = metric.Entity,
            EntityId = metric.EntityId,
            Dimensions = new Dictionary<string, string>(metric.Dimensions, StringComparer.OrdinalIgnoreCase)
        };
    }

    private static TelemetryLog Clone(TelemetryLog log)
    {
        return new TelemetryLog
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            Level = log.Level,
            Message = log.Message,
            Source = log.Source,
            Entity = log.Entity,
            EntityId = log.EntityId,
            Context = new Dictionary<string, string>(log.Context, StringComparer.OrdinalIgnoreCase)
        };
    }

    private static TelemetryTrace Clone(TelemetryTrace trace)
    {
        return new TelemetryTrace
        {
            Id = trace.Id,
            StartTimestamp = trace.StartTimestamp,
            Duration = trace.Duration,
            OperationName = trace.OperationName,
            RootService = trace.RootService,
            RootServiceId = trace.RootServiceId,
            Spans = trace.Spans.Select(span => new TelemetrySpan
            {
                SpanId = span.SpanId,
                ParentSpanId = span.ParentSpanId,
                Service = span.Service,
                ServiceId = span.ServiceId,
                Operation = span.Operation,
                Start = span.Start,
                Duration = span.Duration,
                Tags = new Dictionary<string, string>(span.Tags, StringComparer.OrdinalIgnoreCase)
            }).ToList()
        };
    }
}
