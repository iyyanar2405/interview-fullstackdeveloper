using System.Diagnostics;
using Module_01_Performance_Metrics.Models;
using Module_01_Performance_Metrics.Services;

namespace Module_01_Performance_Metrics.Middleware;

public sealed class RequestMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestMetricsMiddleware> _logger;

    public RequestMetricsMiddleware(RequestDelegate next, ILogger<RequestMetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        RequestMetricsService requestMetricsService,
        CostAnalysisService costAnalysisService)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestStart = DateTimeOffset.UtcNow;
        var path = context.Request.Path.HasValue ? context.Request.Path.Value! : string.Empty;
        requestMetricsService.IncrementActiveRequests();

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Path}", path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        finally
        {
            stopwatch.Stop();
            requestMetricsService.DecrementActiveRequests();
            var metric = BuildMetric(context, stopwatch.Elapsed, requestStart, path);
            requestMetricsService.RecordRequestMetric(metric);
            costAnalysisService.RecordCost(metric);
        }
    }

    private static RequestMetric BuildMetric(HttpContext context, TimeSpan duration, DateTimeOffset startedAt, string path)
    {
        var statusCode = context.Response.StatusCode;
        context.Request.Headers.TryGetValue("X-Token-Usage", out var tokenHeader);
        double? tokensUsed = null;
        if (double.TryParse(tokenHeader.ToString(), out var parsedTokens))
        {
            tokensUsed = parsedTokens;
        }

        return new RequestMetric
        {
            Method = context.Request.Method,
            Path = path,
            StatusCode = statusCode,
            IsSuccess = statusCode < 500,
            StartedAt = startedAt,
            Duration = duration,
            RequestBytes = context.Request.ContentLength ?? 0,
            ResponseBytes = context.Response.ContentLength,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            TraceId = context.TraceIdentifier,
            TokensUsed = tokensUsed
        };
    }
}
