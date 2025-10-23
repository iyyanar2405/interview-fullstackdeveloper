using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Module_04_Application_Insights.Telemetry;

/// <summary>
/// Filters out noisy telemetry (e.g., health checks) to reduce ingestion costs.
/// </summary>
public sealed class TelemetryFilterProcessor : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;
    private readonly HashSet<string> _ignoredPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health",
        "/metrics",
        "/swagger"
    };

    public TelemetryFilterProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is Microsoft.ApplicationInsights.DataContracts.RequestTelemetry request)
        {
            if (_ignoredPaths.Contains(request.Url.AbsolutePath))
            {
                return;
            }
        }

        _next.Process(item);
    }
}
