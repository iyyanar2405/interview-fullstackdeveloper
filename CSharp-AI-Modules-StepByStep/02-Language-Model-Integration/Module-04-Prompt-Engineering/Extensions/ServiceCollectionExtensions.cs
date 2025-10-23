using AI.PromptEngineering.Models;
using AI.PromptEngineering.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.PromptEngineering.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPromptEngineering(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure settings
        services.Configure<PromptEngineeringSettings>(configuration.GetSection("PromptEngineering"));

        // Register services
        services.AddScoped<IPromptTemplateService, PromptTemplateService>();
        services.AddScoped<IFewShotService, FewShotService>();
        services.AddScoped<IChainOfThoughtService, ChainOfThoughtService>();
        services.AddScoped<IPromptOptimizationService, PromptOptimizationService>();

        // Add memory cache
        services.AddMemoryCache();

        return services;
    }
}
