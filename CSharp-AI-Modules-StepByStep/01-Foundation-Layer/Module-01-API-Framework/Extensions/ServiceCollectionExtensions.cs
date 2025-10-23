namespace AI.Foundation.API.Extensions;

/// <summary>
/// Extension methods for service collection to register foundation services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add foundation services to the DI container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFoundationServices(this IServiceCollection services)
    {
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        // Add HTTP client factory
        services.AddHttpClient();

        // Add memory cache
        services.AddMemoryCache();

        // Add custom services
        services.AddSingleton<IApiMetricsService, ApiMetricsService>();
        services.AddScoped<IRequestContextService, RequestContextService>();

        // Configure JSON serialization options
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
        });

        return services;
    }

    /// <summary>
    /// Add CORS policies for AI API
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAICorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AIApiPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            options.AddPolicy("ProductionPolicy", builder =>
            {
                builder.WithOrigins("https://yourdomain.com", "https://api.yourdomain.com")
                       .WithMethods("GET", "POST", "PUT", "DELETE")
                       .WithHeaders("Content-Type", "Authorization");
            });
        });

        return services;
    }

    /// <summary>
    /// Add API versioning support
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApiVersioningSupport(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = Microsoft.AspNetCore.Mvc.ApiVersionReader.Combine(
                new Microsoft.AspNetCore.Mvc.QueryStringApiVersionReader("version"),
                new Microsoft.AspNetCore.Mvc.HeaderApiVersionReader("X-Version")
            );
        }).AddApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}

/// <summary>
/// API metrics service interface
/// </summary>
public interface IApiMetricsService
{
    void IncrementRequestCount(string endpoint, string method);
    void RecordResponseTime(string endpoint, string method, long milliseconds);
    void IncrementErrorCount(string endpoint, string method, int statusCode);
    ApiMetrics GetMetrics();
}

/// <summary>
/// API metrics service implementation
/// </summary>
public class ApiMetricsService : IApiMetricsService
{
    private readonly Dictionary<string, long> _requestCounts = new();
    private readonly Dictionary<string, List<long>> _responseTimes = new();
    private readonly Dictionary<string, long> _errorCounts = new();
    private readonly object _lock = new();

    public void IncrementRequestCount(string endpoint, string method)
    {
        var key = $"{method}:{endpoint}";
        lock (_lock)
        {
            _requestCounts[key] = _requestCounts.GetValueOrDefault(key, 0) + 1;
        }
    }

    public void RecordResponseTime(string endpoint, string method, long milliseconds)
    {
        var key = $"{method}:{endpoint}";
        lock (_lock)
        {
            if (!_responseTimes.ContainsKey(key))
                _responseTimes[key] = new List<long>();
            
            _responseTimes[key].Add(milliseconds);
            
            // Keep only last 1000 entries to prevent memory issues
            if (_responseTimes[key].Count > 1000)
                _responseTimes[key].RemoveAt(0);
        }
    }

    public void IncrementErrorCount(string endpoint, string method, int statusCode)
    {
        var key = $"{method}:{endpoint}:{statusCode}";
        lock (_lock)
        {
            _errorCounts[key] = _errorCounts.GetValueOrDefault(key, 0) + 1;
        }
    }

    public ApiMetrics GetMetrics()
    {
        lock (_lock)
        {
            return new ApiMetrics
            {
                RequestCounts = new Dictionary<string, long>(_requestCounts),
                ResponseTimes = _responseTimes.ToDictionary(
                    kvp => kvp.Key, 
                    kvp => new ResponseTimeStats
                    {
                        Average = kvp.Value.Average(),
                        Min = kvp.Value.Min(),
                        Max = kvp.Value.Max(),
                        Count = kvp.Value.Count
                    }),
                ErrorCounts = new Dictionary<string, long>(_errorCounts),
                Timestamp = DateTime.UtcNow
            };
        }
    }
}

/// <summary>
/// API metrics data
/// </summary>
public class ApiMetrics
{
    public Dictionary<string, long> RequestCounts { get; set; } = new();
    public Dictionary<string, ResponseTimeStats> ResponseTimes { get; set; } = new();
    public Dictionary<string, long> ErrorCounts { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Response time statistics
/// </summary>
public class ResponseTimeStats
{
    public double Average { get; set; }
    public long Min { get; set; }
    public long Max { get; set; }
    public int Count { get; set; }
}

/// <summary>
/// Request context service interface
/// </summary>
public interface IRequestContextService
{
    string GetRequestId();
    string GetUserAgent();
    string GetClientIP();
    Dictionary<string, string> GetHeaders();
}

/// <summary>
/// Request context service implementation
/// </summary>
public class RequestContextService : IRequestContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetRequestId()
    {
        return _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
    }

    public string GetUserAgent()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
    }

    public string GetClientIP()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return "Unknown";

        // Check for forwarded IP first
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    public Dictionary<string, string> GetHeaders()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return new Dictionary<string, string>();

        return context.Request.Headers
            .Where(h => !IsSensitiveHeader(h.Key))
            .ToDictionary(h => h.Key, h => h.Value.ToString());
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