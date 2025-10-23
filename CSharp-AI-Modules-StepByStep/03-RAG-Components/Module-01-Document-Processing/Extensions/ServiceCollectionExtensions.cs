using AI.DocumentProcessing.Services;
using AI.DocumentProcessing.Models;

namespace AI.DocumentProcessing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentProcessing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register settings
        services.Configure<DocumentProcessingSettings>(
            configuration.GetSection("DocumentProcessing"));

        // Register services
        services.AddSingleton<IOCRService>(provider =>
        {
            var tessDataPath = configuration["DocumentProcessing:TessDataPath"];
            return new OCRService(tessDataPath);
        });

        services.AddScoped<IDocumentExtractionService, DocumentExtractionService>();
        services.AddScoped<IChunkingService, ChunkingService>();
        services.AddScoped<IPreprocessingService, PreprocessingService>();
        services.AddScoped<IMetadataService, MetadataService>();
        services.AddScoped<IBatchProcessingService, BatchProcessingService>();

        // Add memory cache for caching extraction results
        services.AddMemoryCache();

        return services;
    }
}
