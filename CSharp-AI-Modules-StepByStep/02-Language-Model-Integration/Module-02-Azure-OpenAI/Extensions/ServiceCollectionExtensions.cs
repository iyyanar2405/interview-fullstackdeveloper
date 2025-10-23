using AI.AzureOpenAI.Models;
using AI.AzureOpenAI.Services;

namespace AI.AzureOpenAI.Extensions;

/// <summary>
/// Service collection extensions for Azure OpenAI
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Azure OpenAI services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddAzureOpenAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure settings
        services.Configure<AzureOpenAISettings>(
            configuration.GetSection(AzureOpenAISettings.SectionName));

        // Validate settings
        services.AddSingleton<IValidateOptions<AzureOpenAISettings>, AzureOpenAISettingsValidator>();

        // Register services
        services.AddScoped<IAzureOpenAIService, AzureOpenAIService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IContentFilterService, ContentFilterService>();

        // Add HTTP client for potential REST API calls
        services.AddHttpClient("AzureOpenAI", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(120);
            client.DefaultRequestHeaders.Add("User-Agent", "AzureOpenAI-CSharp-Client/1.0");
        });

        return services;
    }

    /// <summary>
    /// Add Azure OpenAI services with custom settings
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureSettings">Settings configuration action</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddAzureOpenAI(
        this IServiceCollection services,
        Action<AzureOpenAISettings> configureSettings)
    {
        var settings = new AzureOpenAISettings
        {
            Endpoint = "",
            DeploymentName = ""
        };
        configureSettings(settings);

        services.Configure<AzureOpenAISettings>(opts =>
        {
            opts.Endpoint = settings.Endpoint;
            opts.ApiKey = settings.ApiKey;
            opts.UseManagedIdentity = settings.UseManagedIdentity;
            opts.DeploymentName = settings.DeploymentName;
            opts.EmbeddingDeploymentName = settings.EmbeddingDeploymentName;
            opts.ModelName = settings.ModelName;
            opts.TimeoutSeconds = settings.TimeoutSeconds;
            opts.MaxRetryAttempts = settings.MaxRetryAttempts;
            opts.ContentFilter = settings.ContentFilter;
            opts.RateLimit = settings.RateLimit;
            opts.Logging = settings.Logging;
        });

        // Validate settings
        services.AddSingleton<IValidateOptions<AzureOpenAISettings>, AzureOpenAISettingsValidator>();

        // Register services
        services.AddScoped<IAzureOpenAIService, AzureOpenAIService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IContentFilterService, ContentFilterService>();

        // Add HTTP client
        services.AddHttpClient("AzureOpenAI", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("User-Agent", "AzureOpenAI-CSharp-Client/1.0");
        });

        return services;
    }
}

/// <summary>
/// Application builder extensions for Azure OpenAI
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Use Azure OpenAI middleware
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder</returns>
    public static IApplicationBuilder UseAzureOpenAI(this IApplicationBuilder app)
    {
        // Add any middleware specific to Azure OpenAI
        app.UseMiddleware<AzureOpenAILoggingMiddleware>();
        app.UseMiddleware<AzureOpenAIRateLimitMiddleware>();

        return app;
    }
}

/// <summary>
/// Azure OpenAI settings validator
/// </summary>
public class AzureOpenAISettingsValidator : IValidateOptions<AzureOpenAISettings>
{
    public ValidateOptionsResult Validate(string? name, AzureOpenAISettings options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Endpoint))
            errors.Add("Endpoint is required");

        if (!Uri.TryCreate(options.Endpoint, UriKind.Absolute, out _))
            errors.Add("Endpoint must be a valid URI");

        if (string.IsNullOrWhiteSpace(options.DeploymentName))
            errors.Add("DeploymentName is required");

        if (!options.UseManagedIdentity && string.IsNullOrWhiteSpace(options.ApiKey))
            errors.Add("ApiKey is required when not using managed identity");

        if (options.TimeoutSeconds <= 0)
            errors.Add("TimeoutSeconds must be greater than 0");

        if (options.MaxRetryAttempts < 0)
            errors.Add("MaxRetryAttempts must be non-negative");

        if (options.RateLimit.RequestsPerMinute <= 0)
            errors.Add("RateLimit.RequestsPerMinute must be greater than 0");

        if (options.RateLimit.TokensPerMinute <= 0)
            errors.Add("RateLimit.TokensPerMinute must be greater than 0");

        return errors.Any()
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}

/// <summary>
/// Azure OpenAI logging middleware
/// </summary>
public class AzureOpenAILoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AzureOpenAILoggingMiddleware> _logger;

    public AzureOpenAILoggingMiddleware(RequestDelegate next, ILogger<AzureOpenAILoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/azureopenai"))
        {
            var startTime = DateTime.UtcNow;
            var requestId = Guid.NewGuid().ToString();

            _logger.LogInformation("Azure OpenAI request started: {RequestId} {Method} {Path}",
                requestId, context.Request.Method, context.Request.Path);

            try
            {
                await _next(context);

                var duration = DateTime.UtcNow - startTime;
                _logger.LogInformation("Azure OpenAI request completed: {RequestId} {StatusCode} {Duration}ms",
                    requestId, context.Response.StatusCode, duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                _logger.LogError(ex, "Azure OpenAI request failed: {RequestId} {Duration}ms",
                    requestId, duration.TotalMilliseconds);
                throw;
            }
        }
        else
        {
            await _next(context);
        }
    }
}

/// <summary>
/// Azure OpenAI rate limiting middleware
/// </summary>
public class AzureOpenAIRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AzureOpenAIRateLimitMiddleware> _logger;
    private static readonly Dictionary<string, RateLimitData> _rateLimitData = new();
    private static readonly object _lock = new();

    public AzureOpenAIRateLimitMiddleware(RequestDelegate next, ILogger<AzureOpenAIRateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/azureopenai"))
        {
            var clientId = GetClientId(context);
            
            if (IsRateLimited(clientId))
            {
                _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
                
                context.Response.StatusCode = 429;
                context.Response.Headers.Add("Retry-After", "60");
                
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            RecordRequest(clientId);
        }

        await _next(context);
    }

    private static string GetClientId(HttpContext context)
    {
        // Try to get client ID from various sources
        if (context.Request.Headers.TryGetValue("X-Client-Id", out var clientId))
            return clientId.ToString();

        if (context.Request.Headers.TryGetValue("Authorization", out var auth))
            return auth.ToString().GetHashCode().ToString();

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static bool IsRateLimited(string clientId)
    {
        lock (_lock)
        {
            if (!_rateLimitData.TryGetValue(clientId, out var data))
                return false;

            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-1);

            // Remove old requests
            data.Requests.RemoveAll(r => r < windowStart);

            // Check if rate limited (60 requests per minute by default)
            return data.Requests.Count >= 60;
        }
    }

    private static void RecordRequest(string clientId)
    {
        lock (_lock)
        {
            if (!_rateLimitData.TryGetValue(clientId, out var data))
            {
                data = new RateLimitData();
                _rateLimitData[clientId] = data;
            }

            data.Requests.Add(DateTime.UtcNow);
        }
    }

    private class RateLimitData
    {
        public List<DateTime> Requests { get; } = new();
    }
}