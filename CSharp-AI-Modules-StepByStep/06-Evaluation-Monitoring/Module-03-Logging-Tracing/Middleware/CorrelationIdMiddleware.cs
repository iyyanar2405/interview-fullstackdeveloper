namespace Module_03_Logging_Tracing.Middleware;

/// <summary>
/// Ensures every incoming request has a correlation identifier that flows through logs and downstream calls.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private const string CorrelationHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationHeader, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
            context.Request.Headers[CorrelationHeader] = correlationId;
        }

        context.TraceIdentifier = correlationId;
        context.Response.Headers.TryAdd(CorrelationHeader, correlationId);

        using var activity = System.Diagnostics.Activity.Current ?? new System.Diagnostics.Activity("Correlation");
        if (activity.Id is null)
        {
            activity.SetIdFormat(System.Diagnostics.ActivityIdFormat.W3C);
            activity.Start();
        }

        activity.SetTag("correlation.id", correlationId.ToString());
        activity.SetBaggage("correlation.id", correlationId.ToString());

        await _next(context);
    }
}
