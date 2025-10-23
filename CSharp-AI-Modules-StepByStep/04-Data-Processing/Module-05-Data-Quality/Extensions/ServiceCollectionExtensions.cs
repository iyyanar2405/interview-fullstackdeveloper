using AI.DataQuality.Services;

namespace AI.DataQuality.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataQualityServices(this IServiceCollection services)
    {
        // Register data quality services
        services.AddScoped<IDataValidationService, DataValidationService>();
        services.AddScoped<IDataProfilingService, DataProfilingService>();
        services.AddScoped<IAnomalyDetectionService, AnomalyDetectionService>();
        services.AddScoped<IDataCleansingService, DataCleansingService>();
        services.AddScoped<IDataGovernanceService, DataGovernanceService>();

        return services;
    }
}
