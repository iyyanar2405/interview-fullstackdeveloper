using AI.DataSources.Services;

namespace AI.DataSources.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataSourceServices(this IServiceCollection services)
    {
        // Register all data source services
        services.AddScoped<ISqlServerService, SqlServerService>();
        services.AddScoped<IPostgreSqlService, PostgreSqlService>();
        services.AddScoped<IMongoDbService, MongoDbService>();
        services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<IAwsS3Service, AwsS3Service>();
        services.AddScoped<IApiClientService, ApiClientService>();
        services.AddScoped<IFileSystemService, FileSystemService>();

        return services;
    }
}
