using Serilog.Core;
using Serilog.Events;

namespace Module_03_Logging_Tracing.Infrastructure;

/// <summary>
/// Adds correlation identifiers and additional context into log events when present in the current scope.
/// </summary>
public sealed class LoggingEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (Activity.Current is { } activity)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
            if (!string.IsNullOrWhiteSpace(activity.ParentId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentId", activity.ParentId));
            }
        }

        if (logEvent.Properties.ContainsKey("RequestId"))
        {
            return;
        }

        if (System.Diagnostics.Activity.Current?.Id is { } requestId)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestId", requestId));
        }
    }
}
