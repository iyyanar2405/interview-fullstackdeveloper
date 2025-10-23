using Module_03_Logging_Tracing.Services;

namespace Module_03_Logging_Tracing.Middleware;

/// <summary>
/// Captures inbound request metrics, emits structured log entries, and writes trace annotations.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestLogFormatter _formatter;
    private readonly RequestTracingService _tracing;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        RequestLogFormatter formatter,
        RequestTracingService tracing)
    {
        _next = next;
        _logger = logger;
        _formatter = formatter;
        _tracing = tracing;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = Stopwatch.GetTimestamp();
        using var span = _tracing.StartSpan("http.request", new Dictionary<string, object>
        {
            ["http.method"] = context.Request.Method,
            ["http.route"] = context.Request.Path.ToString()
        });

        try
        {
            await _next(context);
            var elapsed = Stopwatch.GetElapsedTime(start);
            span.SetAttribute("http.status_code", context.Response.StatusCode);
            span.SetAttribute("http.elapsed_ms", elapsed.TotalMilliseconds);

            var envelope = _formatter.BuildRequestEnvelope(context, elapsed);
            _logger.LogInformation("Request completed: {Envelope}", envelope);
        }
        catch (Exception ex)
        {
            span.RecordException(ex);
            span.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
