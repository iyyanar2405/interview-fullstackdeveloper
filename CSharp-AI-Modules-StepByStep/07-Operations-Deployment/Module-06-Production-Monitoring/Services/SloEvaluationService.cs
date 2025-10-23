using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Module_06_Production_Monitoring.Models;
using System.Linq;

namespace Module_06_Production_Monitoring.Services;

public sealed class SloEvaluationService
{
    private readonly ILogger<SloEvaluationService> _logger;
    private readonly TelemetryStoreService _telemetryStore;
    private readonly MonitoringOptions _options;

    public SloEvaluationService(
        ILogger<SloEvaluationService> logger,
        TelemetryStoreService telemetryStore,
        IOptions<MonitoringOptions> options)
    {
        _logger = logger;
        _telemetryStore = telemetryStore;
        _options = options.Value;
    }

    public IReadOnlyCollection<SloStatus> Evaluate()
    {
        var statuses = new List<SloStatus>();
        foreach (var slo in _options.ServiceLevelObjectives)
        {
            var status = EvaluateSlo(slo);
            statuses.Add(status);
        }

        return statuses;
    }

    private SloStatus EvaluateSlo(ServiceLevelObjective slo)
    {
        var metrics = _telemetryStore.FilterMetrics(metric =>
            metric.EntityId.Equals(slo.Service, StringComparison.OrdinalIgnoreCase) &&
            metric.Name.Equals(slo.Metric, StringComparison.OrdinalIgnoreCase));

        if (metrics.Count == 0)
        {
            _logger.LogWarning("No metrics available for SLO {Slo}", slo.Name);
            return new SloStatus
            {
                Name = slo.Name,
                Target = slo.Target,
                Period = slo.Period,
                RollingWindow = TimeSpan.FromMinutes(5),
                CurrentValue = 0,
                Status = SloState.Unknown,
                StatusReason = "No telemetry available"
            };
        }

        var recent = metrics.OrderByDescending(m => m.Timestamp).Take(100).ToList();
        var value = slo.Aggregation switch
        {
            "avg" => recent.Average(m => m.Value),
            "p95" => Percentile(recent, 0.95),
            "max" => recent.Max(m => m.Value),
            _ => recent.Last().Value
        };

        var status = slo.Direction switch
        {
            SloDirection.GreaterThanOrEqual => value >= slo.Target,
            SloDirection.LessThanOrEqual => value <= slo.Target,
            _ => false
        };

        var state = status ? SloState.Healthy : SloState.Breaching;
        var reason = status ? "Within target" : "Outside target";

        return new SloStatus
        {
            Name = slo.Name,
            Target = slo.Target,
            Period = slo.Period,
            RollingWindow = slo.Window,
            CurrentValue = value,
            Status = state,
            StatusReason = reason
        };
    }

    private static double Percentile(List<TelemetryMetric> metrics, double percentile)
    {
        var ordered = metrics.Select(m => m.Value).OrderBy(v => v).ToList();
        if (ordered.Count == 0)
        {
            return 0;
        }

        var index = (int)Math.Floor(percentile * ordered.Count);
        index = Math.Clamp(index, 0, ordered.Count - 1);
        return ordered[index];
    }
}
