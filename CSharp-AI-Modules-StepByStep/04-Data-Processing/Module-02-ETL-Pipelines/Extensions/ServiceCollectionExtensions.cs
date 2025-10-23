using AI.ETL.Services;

namespace AI.ETL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddETLServices(this IServiceCollection services)
    {
        // Register ETL services
        services.AddScoped<IExtractionService, ExtractionService>();
        services.AddScoped<ITransformationService, TransformationService>();
        services.AddScoped<ILoadService, LoadService>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IETLPipelineService, ETLPipelineService>();

        return services;
    }
}
