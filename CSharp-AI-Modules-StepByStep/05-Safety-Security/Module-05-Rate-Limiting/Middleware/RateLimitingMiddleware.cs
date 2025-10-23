using Module_05_Rate_Limiting.Services;
using System.Net;

namespace Module_05_Rate_Limiting.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IRateLimitingService rateLimitingService,
        IQuotaManagementService quotaService,
        IDDoSProtectionService ddosService,
        IThrottlingService throttlingService)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var endpoint = context.Request.Path.Value ?? "/";
        var clientId = context.Request.Headers["X-Client-Id"].FirstOrDefault() ?? ipAddress;
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();

        // 1. Check if IP is blocked (DDoS protection)
        if (await ddosService.IsIpBlockedAsync(ipAddress))
        {
            _logger.LogWarning("Blocked IP attempted access: {IP}", ipAddress);
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Access denied",
                message = "Your IP has been temporarily blocked due to suspicious activity"
            });
            return;
        }

        // 2. Analyze for DDoS patterns
        var ddosResult = await ddosService.AnalyzeRequestAsync(ipAddress, endpoint, userAgent);
        if (ddosResult.ThreatLevel >= Models.ThreatLevel.High)
        {
            _logger.LogWarning("High threat detected from {IP}: {Reasons}", 
                ipAddress, string.Join(", ", ddosResult.DetectionReasons));

            if (ddosResult.RecommendedAction == Models.DDoSAction.Block)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests",
                    message = "Your IP has been temporarily blocked due to excessive requests",
                    threat_level = ddosResult.ThreatLevel.ToString()
                });
                return;
            }
        }

        // 3. Check quota
        var quotaCheck = await quotaService.CheckQuotaAsync(clientId, endpoint);
        if (!quotaCheck.HasQuota)
        {
            _logger.LogInformation("Quota exceeded for client: {ClientId}", clientId);
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers["X-RateLimit-Limit"] = "0";
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            context.Response.Headers["X-RateLimit-Reset"] = quotaCheck.QuotaResetTime.ToString("o");
            
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Quota exceeded",
                message = quotaCheck.Message,
                reset_time = quotaCheck.QuotaResetTime
            });
            return;
        }

        // 4. Check rate limit
        var rateLimitResult = await rateLimitingService.CheckRateLimitAsync(clientId, endpoint, ipAddress);
        if (!rateLimitResult.IsAllowed)
        {
            _logger.LogInformation("Rate limit exceeded for {ClientId} on {Endpoint}", clientId, endpoint);
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers["X-RateLimit-Limit"] = rateLimitResult.RemainingRequests.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            context.Response.Headers["X-RateLimit-Reset"] = rateLimitResult.ResetTime?.ToString("o") ?? "";
            context.Response.Headers["Retry-After"] = ((int)(rateLimitResult.RetryAfter?.TotalSeconds ?? 60)).ToString();
            
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = rateLimitResult.ReasonDenied,
                retry_after_seconds = rateLimitResult.RetryAfter?.TotalSeconds,
                reset_time = rateLimitResult.ResetTime
            });
            return;
        }

        // 5. Check throttling
        var throttleResult = await throttlingService.CheckThrottleAsync(clientId, "api");
        if (throttleResult.IsThrottled)
        {
            _logger.LogInformation("Request throttled for {ClientId}. Queue position: {Position}", 
                clientId, throttleResult.QueuePosition);
            
            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            context.Response.Headers["Retry-After"] = throttleResult.EstimatedWaitSeconds.ToString();
            
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Service throttled",
                message = throttleResult.Reason,
                queue_position = throttleResult.QueuePosition,
                estimated_wait_seconds = throttleResult.EstimatedWaitSeconds,
                retry_at = throttleResult.RetryAt
            });
            return;
        }

        // Add rate limit headers to successful responses
        context.Response.Headers["X-RateLimit-Limit"] = rateLimitResult.RemainingRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = rateLimitResult.RemainingRequests.ToString();
        context.Response.Headers["X-RateLimit-Reset"] = rateLimitResult.ResetTime?.ToString("o") ?? "";
        context.Response.Headers["X-Quota-Remaining-Daily"] = quotaCheck.RemainingDaily.ToString();
        context.Response.Headers["X-Quota-Remaining-Monthly"] = quotaCheck.RemainingMonthly.ToString();

        try
        {
            // Process request
            await _next(context);
        }
        finally
        {
            // Release throttling resources
            await throttlingService.ReleaseResourceAsync(clientId, "api");
        }
    }
}

public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}
