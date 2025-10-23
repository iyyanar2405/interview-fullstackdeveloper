using System.Diagnostics;

namespace AI.Foundation.API.Middleware;

/// <summary>
/// Request/Response logging middleware
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        // Log request
        _logger.LogInformation(
            "HTTP {Method} {Path} started. RequestId: {RequestId}",
            context.Request.Method,
            context.Request.Path,
            requestId);

        // Log request headers (excluding sensitive information)
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var headers = context.Request.Headers
                .Where(h => !IsSensitiveHeader(h.Key))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            _logger.LogDebug(
                "Request headers for {RequestId}: {@Headers}",
                requestId,
                headers);
        }

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log response
            _logger.LogInformation(
                "HTTP {Method} {Path} completed in {ElapsedMs}ms with status {StatusCode}. RequestId: {RequestId}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode,
                requestId);

            // Log slow requests
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {ElapsedMs}ms. RequestId: {RequestId}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    requestId);
            }

            // Log error responses
            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning(
                    "Error response: {Method} {Path} returned {StatusCode}. RequestId: {RequestId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    requestId);
            }
        }
    }

    private static bool IsSensitiveHeader(string headerName)
    {
        var sensitiveHeaders = new[]
        {
            "authorization",
            "cookie",
            "x-api-key",
            "x-auth-token"
        };

        return sensitiveHeaders.Contains(headerName.ToLowerInvariant());
    }
}