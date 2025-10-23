using OpenTelemetry.Trace;

namespace Module_03_Logging_Tracing.Services;

/// <summary>
/// Helper methods to annotate OpenTelemetry spans with business-specific attributes.
/// </summary>
public sealed class RequestTracingService
{
    private readonly Tracer _tracer;

    public RequestTracingService(TracerProvider provider)
    {
        _tracer = provider.GetTracer("Module-03-Logging-Tracing");
    }

    public IDisposable StartSpan(string name, IDictionary<string, object>? attributes = null)
    {
        var span = _tracer.StartActiveSpan(name);
        if (attributes != null)
        {
            foreach (var (key, value) in attributes)
            {
                span.SetAttribute(key, value?.ToString());
            }
        }

        return span;
    }

    public void RecordEvent(string name, IDictionary<string, object>? attributes = null)
    {
        if (Activity.Current is { } activity)
        {
            activity.AddEvent(new ActivityEvent(name, tags: attributes is null ? default : new ActivityTagsCollection(attributes)));
        }
    }
}
