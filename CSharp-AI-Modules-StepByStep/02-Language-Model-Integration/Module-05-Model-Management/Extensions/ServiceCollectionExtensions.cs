using AI.ModelManagement.Services;
using Microsoft.Extensions.Caching.Memory;

namespace AI.ModelManagement.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModelManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register configuration
        services.Configure<Models.ModelManagementSettings>(
            configuration.GetSection("ModelManagement"));

        // Register memory cache
        services.AddMemoryCache();

        // Register services
        services.AddSingleton<IModelSelectionService, ModelSelectionService>();
        services.AddSingleton<IFallbackService, FallbackService>();
        services.AddSingleton<ICostOptimizationService, CostOptimizationService>();
        services.AddSingleton<IPerformanceMonitoringService, PerformanceMonitoringService>();

        // Register HTTP clients with Polly policies
        services.AddHttpClient("ModelManagement")
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
