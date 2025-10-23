using AI.Anthropic.Models;
using AI.Anthropic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace AI.Anthropic.Extensions;

/// <summary>
/// Service collection extensions for Anthropic Claude integration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Anthropic Claude services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddAnthropicClaude(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Anthropic settings
        services.Configure<AnthropicSettings>(configuration.GetSection("Anthropic"));

        // Add HTTP client for Anthropic API
        services.AddHttpClient<IClaudeService, ClaudeService>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<AnthropicSettings>>().Value;
            
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", settings.ApiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", settings.Version);
            client.DefaultRequestHeaders.Add("User-Agent", "Claude-CSharp-SDK/1.0.0");
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
        {
            MaxConnectionsPerServer = 10
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Register services
        services.AddScoped<IClaudeService, ClaudeService>();
        services.AddScoped<IConstitutionalAIService, ConstitutionalAIService>();
        services.AddScoped<ITokenService, TokenService>();

        // Add validators
        services.AddTransient<IValidator<ClaudeRequest>, ClaudeRequestValidator>();
        services.AddTransient<IValidator<ClaudeStreamRequest>, ClaudeStreamRequestValidator>();
        services.AddTransient<IValidator<LongContextRequest>, LongContextRequestValidator>();

        // Configure JSON serialization
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.WriteIndented = false;
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        // Add memory cache for token estimates and other caching needs
        services.AddMemoryCache();

        // Add rate limiting
        services.AddRateLimiting(configuration);

        return services;
    }

    /// <summary>
    /// Add Swagger documentation for Anthropic Claude API
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddClaudeSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Anthropic Claude API",
                Version = "v1",
                Description = "API for interacting with Anthropic Claude models with Constitutional AI features",
                Contact = new OpenApiContact
                {
                    Name = "Claude API Support",
                    Email = "support@anthropic.com",
                    Url = new Uri("https://www.anthropic.com/support")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Add security definition
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "x-api-key",
                Description = "Anthropic API Key"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Add custom schema filters
            c.SchemaFilter<EnumSchemaFilter>();
            c.OperationFilter<SwaggerDefaultValues>();
        });

        return services;
    }

    /// <summary>
    /// Add rate limiting for Claude API endpoints
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection for chaining</returns>
    private static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("ClaudeApi", config =>
            {
                config.PermitLimit = configuration.GetValue<int>("RateLimit:Claude:PermitLimit", 100);
                config.Window = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimit:Claude:WindowMinutes", 1));
                config.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                config.QueueLimit = configuration.GetValue<int>("RateLimit:Claude:QueueLimit", 50);
            });

            options.AddFixedWindowLimiter("ClaudeStream", config =>
            {
                config.PermitLimit = configuration.GetValue<int>("RateLimit:ClaudeStream:PermitLimit", 20);
                config.Window = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimit:ClaudeStream:WindowMinutes", 1));
                config.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                config.QueueLimit = configuration.GetValue<int>("RateLimit:ClaudeStream:QueueLimit", 10);
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", token);
            };
        });

        return services;
    }

    /// <summary>
    /// Get retry policy for HTTP client
    /// </summary>
    /// <returns>Retry policy</returns>
    private static Polly.Extensions.Http.IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Polly.Extensions.Http.HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogWarning("Retry {RetryCount} for {OperationKey} after {Delay}ms",
                        retryCount, context.OperationKey, timespan.TotalMilliseconds);
                });
    }

    /// <summary>
    /// Get circuit breaker policy for HTTP client
    /// </summary>
    /// <returns>Circuit breaker policy</returns>
    private static Polly.CircuitBreaker.IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Polly.Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, timespan) =>
                {
                    // Log circuit breaker opened
                },
                onReset: () =>
                {
                    // Log circuit breaker reset
                });
    }
}

/// <summary>
/// Application builder extensions for Anthropic Claude integration
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Use Anthropic Claude middleware
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder for chaining</returns>
    public static IApplicationBuilder UseAnthropicClaude(this IApplicationBuilder app)
    {
        // Add request logging middleware
        app.UseMiddleware<ClaudeRequestLoggingMiddleware>();

        // Add rate limiting
        app.UseRateLimiter();

        return app;
    }

    /// <summary>
    /// Use Claude Swagger documentation
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Application builder for chaining</returns>
    public static IApplicationBuilder UseClaudeSwagger(
        this IApplicationBuilder app,
        IConfiguration configuration)
    {
        var enableSwagger = configuration.GetValue<bool>("Swagger:Enabled", true);
        
        if (enableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Anthropic Claude API v1");
                c.RoutePrefix = configuration.GetValue<string>("Swagger:RoutePrefix", "swagger");
                c.DocumentTitle = "Anthropic Claude API Documentation";
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
                c.ShowExtensions();
                c.EnableValidator();
            });
        }

        return app;
    }
}

/// <summary>
/// Request logging middleware for Claude API calls
/// </summary>
public class ClaudeRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ClaudeRequestLoggingMiddleware> _logger;

    public ClaudeRequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<ClaudeRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString("N")[..8];

        // Log request start for Claude API endpoints
        if (context.Request.Path.StartsWithSegments("/api/claude"))
        {
            _logger.LogInformation("Claude API Request {RequestId} started: {Method} {Path}",
                requestId, context.Request.Method, context.Request.Path);

            context.Items["RequestId"] = requestId;
            context.Items["StartTime"] = startTime;
        }

        await _next(context);

        // Log request completion for Claude API endpoints
        if (context.Request.Path.StartsWithSegments("/api/claude"))
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Claude API Request {RequestId} completed: {StatusCode} in {Duration}ms",
                requestId, context.Response.StatusCode, duration.TotalMilliseconds);
        }
    }
}

/// <summary>
/// Swagger enum schema filter
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(name => schema.Enum.Add(new Microsoft.OpenApi.Any.OpenApiString(name)));
        }
    }
}

/// <summary>
/// Swagger default values operation filter
/// </summary>
public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
            var response = operation.Responses[responseKey];

            foreach (var contentType in response.Content.Keys)
            {
                if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
                {
                    response.Content.Remove(contentType);
                }
            }
        }

        if (operation.Parameters == null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions
                .FirstOrDefault(p => p.Name == parameter.Name);

            parameter.Description ??= description?.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description?.DefaultValue != null)
            {
                parameter.Schema.Default = new Microsoft.OpenApi.Any.OpenApiString(description.DefaultValue.ToString());
            }

            parameter.Required |= description?.IsRequired ?? false;
        }
    }
}

/// <summary>
/// Extension methods for Polly context
/// </summary>
public static class PollyContextExtensions
{
    private const string LoggerKey = "Logger";

    public static Context WithLogger(this Context context, ILogger logger)
    {
        context[LoggerKey] = logger;
        return context;
    }

    public static ILogger? GetLogger(this Context context)
    {
        return context.TryGetValue(LoggerKey, out var logger) ? logger as ILogger : null;
    }
}

/// <summary>
/// Validation extensions
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validate object using FluentValidation
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="obj">Object to validate</param>
    /// <param name="validator">Validator</param>
    /// <returns>Validation result</returns>
    public static async Task<FluentValidation.Results.ValidationResult> ValidateAsync<T>(
        this T obj,
        IValidator<T> validator)
    {
        return await validator.ValidateAsync(obj);
    }

    /// <summary>
    /// Check if validation result is valid
    /// </summary>
    /// <param name="result">Validation result</param>
    /// <returns>True if valid</returns>
    public static bool IsValid(this FluentValidation.Results.ValidationResult result)
    {
        return result.IsValid;
    }

    /// <summary>
    /// Get validation errors as problem details
    /// </summary>
    /// <param name="result">Validation result</param>
    /// <returns>Problem details</returns>
    public static ProblemDetails ToProblemDetails(this FluentValidation.Results.ValidationResult result)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = "One or more validation errors occurred."
        };

        foreach (var error in result.Errors)
        {
            if (!problemDetails.Extensions.ContainsKey("errors"))
            {
                problemDetails.Extensions["errors"] = new Dictionary<string, List<string>>();
            }

            var errors = (Dictionary<string, List<string>>)problemDetails.Extensions["errors"]!;
            
            if (!errors.ContainsKey(error.PropertyName))
            {
                errors[error.PropertyName] = new List<string>();
            }

            errors[error.PropertyName].Add(error.ErrorMessage);
        }

        return problemDetails;
    }
}