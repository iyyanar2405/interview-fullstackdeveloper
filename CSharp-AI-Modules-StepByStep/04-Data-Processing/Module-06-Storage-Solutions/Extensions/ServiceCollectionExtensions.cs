using Module_06_Storage_Solutions.Services;
using StackExchange.Redis;

namespace Module_06_Storage_Solutions.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageSolutions(this IServiceCollection services, IConfiguration configuration)
    {
        // Register all storage services
        services.AddScoped<ISqlServerService, SqlServerService>();
        services.AddScoped<IPostgreSqlService, PostgreSqlService>();
        services.AddScoped<IMongoDbService, MongoDbService>();
        services.AddScoped<IRedisService, RedisService>();
        services.AddScoped<IAzureStorageService, AzureStorageService>();
        services.AddScoped<IAwsStorageService, AwsStorageService>();
        services.AddScoped<ICachingService, CachingService>();
        services.AddScoped<IStorageManagerService, StorageManagerService>();

        // Add memory cache
        services.AddMemoryCache();

        // Add distributed cache (Redis)
        var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "StorageSolutions_";
            });
        }

        return services;
    }
}
