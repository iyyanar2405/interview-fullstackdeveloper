using AI.Embeddings.Models;
using AI.Embeddings.Services;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace AI.Embeddings.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmbeddingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register settings
        services.Configure<EmbeddingSettings>(
            configuration.GetSection("EmbeddingSettings"));

        var settings = configuration.GetSection("EmbeddingSettings").Get<EmbeddingSettings>()
            ?? new EmbeddingSettings();

        // Register embedding services
        services.AddScoped<OpenAIEmbeddingService>();
        services.AddScoped<AzureOpenAIEmbeddingService>();
        services.AddScoped<IEmbeddingServiceFactory, EmbeddingServiceFactory>();

        // Register similarity service
        services.AddSingleton<ISimilarityService, SimilarityService>();

        // Register batch processing service
        services.AddScoped<IBatchEmbeddingService, BatchEmbeddingService>();

        // Register semantic search service
        services.AddScoped<ISemanticSearchService, SemanticSearchService>();

        // Register analytics service
        services.AddSingleton<IEmbeddingAnalyticsService, EmbeddingAnalyticsService>();

        // Register cache service based on strategy
        RegisterCacheService(services, settings);

        return services;
    }

    private static void RegisterCacheService(IServiceCollection services, EmbeddingSettings settings)
    {
        switch (settings.CacheStrategy)
        {
            case CacheStrategy.Memory:
                services.AddMemoryCache(options =>
                {
                    options.SizeLimit = settings.MaxCacheEntries;
                });
                services.AddSingleton<IEmbeddingCacheService, MemoryEmbeddingCacheService>();
                break;

            case CacheStrategy.Redis:
                if (!string.IsNullOrEmpty(settings.RedisConnectionString))
                {
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = settings.RedisConnectionString;
                    });
                    services.AddSingleton<IEmbeddingCacheService, RedisEmbeddingCacheService>();
                }
                else
                {
                    throw new InvalidOperationException("Redis connection string is required for Redis caching");
                }
                break;

            case CacheStrategy.Hybrid:
                services.AddMemoryCache(options =>
                {
                    options.SizeLimit = settings.MaxCacheEntries / 2;
                });
                
                if (!string.IsNullOrEmpty(settings.RedisConnectionString))
                {
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = settings.RedisConnectionString;
                    });
                }
                
                services.AddSingleton<IEmbeddingCacheService, MemoryEmbeddingCacheService>();
                break;

            case CacheStrategy.None:
            default:
                services.AddSingleton<IEmbeddingCacheService, NoCacheService>();
                break;
        }
    }
}

#region No Cache Service

public class NoCacheService : IEmbeddingCacheService
{
    public Task<CachedEmbedding?> GetAsync(string text, EmbeddingModel model)
    {
        return Task.FromResult<CachedEmbedding?>(null);
    }

    public Task SetAsync(string text, EmbeddingModel model, float[] embedding, int expirationMinutes)
    {
        return Task.CompletedTask;
    }

    public Task<CacheStatistics> GetStatisticsAsync()
    {
        return Task.FromResult(new CacheStatistics());
    }

    public Task ClearAsync()
    {
        return Task.CompletedTask;
    }
}

#endregion
