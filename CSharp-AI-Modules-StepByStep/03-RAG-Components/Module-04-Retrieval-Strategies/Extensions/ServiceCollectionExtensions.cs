using AI.RetrievalStrategies.Models;
using AI.RetrievalStrategies.Services;
using Azure.AI.OpenAI;
using Azure;

namespace AI.RetrievalStrategies.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRetrievalStrategies(
        this IServiceCollection services,
        Action<RetrievalSettings> configure)
    {
        // Configure settings
        services.Configure(configure);

        var settings = new RetrievalSettings();
        configure(settings);

        // Register OpenAI Client
        if (!string.IsNullOrEmpty(settings.OpenAIApiKey))
        {
            services.AddSingleton(sp =>
            {
                if (!string.IsNullOrEmpty(settings.OpenAIEndpoint))
                {
                    return new OpenAIClient(
                        new Uri(settings.OpenAIEndpoint),
                        new AzureKeyCredential(settings.OpenAIApiKey));
                }
                else
                {
                    return new OpenAIClient(settings.OpenAIApiKey);
                }
            });
        }

        // Register services
        services.AddScoped<IRetrievalService, RetrievalService>();
        services.AddScoped<IReRankingService, ReRankingService>();
        services.AddScoped<IQueryExpansionService, QueryExpansionService>();
        services.AddScoped<IContextOptimizationService, ContextOptimizationService>();
        services.AddScoped<ISelfQueryService, SelfQueryService>();

        // Add memory cache for caching
        if (settings.EnableCaching)
        {
            services.AddMemoryCache();
        }

        // Add HTTP client
        services.AddHttpClient();

        return services;
    }

    public static IServiceCollection AddRetrievalStrategies(
        this IServiceCollection services,
        RetrievalSettings settings)
    {
        return services.AddRetrievalStrategies(options =>
        {
            options.OpenAIApiKey = settings.OpenAIApiKey;
            options.OpenAIEndpoint = settings.OpenAIEndpoint;
            options.EmbeddingModel = settings.EmbeddingModel;
            options.ChatModel = settings.ChatModel;
            options.DefaultTopK = settings.DefaultTopK;
            options.DefaultScoreThreshold = settings.DefaultScoreThreshold;
            options.EnableCaching = settings.EnableCaching;
            options.CacheDurationMinutes = settings.CacheDurationMinutes;
            options.MaxContextTokens = settings.MaxContextTokens;
            options.MaxQueryExpansions = settings.MaxQueryExpansions;
            options.HybridSemanticWeight = settings.HybridSemanticWeight;
            options.HybridKeywordWeight = settings.HybridKeywordWeight;
            options.MMRLambda = settings.MMRLambda;
            options.RRFConstant = settings.RRFConstant;
            options.VectorDatabaseEndpoint = settings.VectorDatabaseEndpoint;
            options.RedisConnectionString = settings.RedisConnectionString;
        });
    }
}
