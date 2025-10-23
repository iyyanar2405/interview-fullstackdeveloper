using AI.VectorDatabases.Models;
using AI.VectorDatabases.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AI.VectorDatabases.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVectorDatabases(
        this IServiceCollection services,
        Action<VectorDatabaseSettings> configure)
    {
        // Configure settings
        services.Configure(configure);

        // Register vector database services
        services.AddSingleton<QdrantVectorDatabaseService>();
        services.AddSingleton<InMemoryVectorDatabaseService>();
        
        // Register factory
        services.AddSingleton<IVectorDatabaseServiceFactory, VectorDatabaseServiceFactory>();

        // Register additional services
        services.AddScoped<SearchService>();
        services.AddScoped<IndexManagementService>();

        // Add memory cache for in-memory provider
        services.AddMemoryCache();

        // Add HTTP client for Pinecone and other HTTP-based providers
        services.AddHttpClient();

        return services;
    }

    public static IServiceCollection AddVectorDatabases(
        this IServiceCollection services,
        VectorDatabaseSettings settings)
    {
        return services.AddVectorDatabases(options =>
        {
            options.DefaultProvider = settings.DefaultProvider;
            options.QdrantHost = settings.QdrantHost;
            options.QdrantPort = settings.QdrantPort;
            options.QdrantApiKey = settings.QdrantApiKey;
            options.QdrantUseTls = settings.QdrantUseTls;
            options.InMemoryMaxCollections = settings.InMemoryMaxCollections;
            options.InMemoryMaxPointsPerCollection = settings.InMemoryMaxPointsPerCollection;
        });
    }
}
