using AI.ResponseProcessing.Services;

namespace AI.ResponseProcessing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResponseProcessing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register configuration
        services.Configure<Models.ResponseProcessingSettings>(
            configuration.GetSection("ResponseProcessing"));

        // Register memory cache
        services.AddMemoryCache();

        // Register services
        services.AddSingleton<IResponseParserService, ResponseParserService>();
        services.AddSingleton<IValidationService, ValidationService>();
        services.AddSingleton<IOutputFormattingService, OutputFormattingService>();
        services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
        services.AddSingleton<IRateLimitingService, RateLimitingService>();

        // Register HTTP clients with Polly policies
        services.AddHttpClient("ResponseProcessing")
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static Polly.IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Polly.Extensions.Http.HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static Polly.IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Polly.Extensions.Http.HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}
