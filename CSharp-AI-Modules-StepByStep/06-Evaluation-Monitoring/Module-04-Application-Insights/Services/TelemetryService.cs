using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Module_04_Application_Insights.Services;

/// <summary>
/// Provides convenience methods for emitting custom events, metrics, and dependencies to Application Insights.
/// </summary>
public sealed class TelemetryService
{
    private readonly TelemetryClient _client;
    private readonly ILogger<TelemetryService> _logger;

    public TelemetryService(TelemetryClient client, ILogger<TelemetryService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public void TrackCustomEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
        _client.TrackEvent(eventName, properties, metrics);
        _logger.LogInformation("Tracked custom event {EventName} with properties {Properties}", eventName, properties);
    }

    public void TrackBusinessMetric(string metricName, double value, IDictionary<string, string>? properties = null)
    {
        var metric = new MetricTelemetry(metricName, value);
        if (properties != null)
        {
            foreach (var kvp in properties)
            {
                metric.Properties[kvp.Key] = kvp.Value;
            }
        }

        _client.TrackMetric(metric);
        _logger.LogInformation("Tracked metric {Metric}={Value}", metricName, value);
    }

    public IDisposable StartDependency(string dependencyType, string target, string command)
    {
        var dependency = new DependencyTelemetry
        {
            Type = dependencyType,
            Target = target,
            Data = command
        };
        var operation = _client.StartOperation(dependency);
        return new DependencyScope(operation, _logger);
    }

    public void TrackException(Exception exception, IDictionary<string, string>? properties = null)
    {
        _client.TrackException(exception, properties);
        _logger.LogError(exception, "Exception tracked in Application Insights");
    }

    public void Flush() => _client.Flush();

    private sealed class DependencyScope : IDisposable
    {
        private readonly IOperationHolder<DependencyTelemetry> _operation;
        private readonly ILogger _logger;
        private bool _disposed;

        public DependencyScope(IOperationHolder<DependencyTelemetry> operation, ILogger logger)
        {
            _operation = operation;
            _logger = logger;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _operation.Telemetry.Success = !_operation.Telemetry.Success.HasValue || _operation.Telemetry.Success.Value;
            _operation.Dispose();
            _logger.LogDebug("Dependency telemetry flushed: {Dependency}", _operation.Telemetry.Data);
            _disposed = true;
        }
    }
}
