using AI.RAGPipeline.Models;
using AI.RAGPipeline.Services;
using Azure.AI.OpenAI;

namespace AI.RAGPipeline.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRAGPipeline(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<RAGPipelineSettings>(
            configuration.GetSection("RAGPipeline"));

        // OpenAI Client
        var apiKey = configuration.GetValue<string>("RAGPipeline:OpenAIApiKey");
        if (!string.IsNullOrEmpty(apiKey))
        {
            services.AddSingleton(new OpenAIClient(apiKey));
        }
        else
        {
            var azureEndpoint = configuration.GetValue<string>("RAGPipeline:AzureOpenAIEndpoint");
            var azureKey = configuration.GetValue<string>("RAGPipeline:AzureOpenAIKey");
            if (!string.IsNullOrEmpty(azureEndpoint) && !string.IsNullOrEmpty(azureKey))
            {
                services.AddSingleton(new OpenAIClient(
                    new Uri(azureEndpoint),
                    new Azure.AzureKeyCredential(azureKey)));
            }
        }

        // Core Services
        services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
        services.AddScoped<IQueryProcessingService, QueryProcessingService>();
        services.AddScoped<IContextRetrievalService, ContextRetrievalService>();
        services.AddScoped<IResponseGenerationService, ResponseGenerationService>();
        services.AddScoped<IEvaluationService, EvaluationService>();
        
        // Helper Services
        services.AddScoped<IEmbeddingGenerationService, EmbeddingGenerationService>();
        services.AddScoped<IVectorStorageService, VectorStorageService>();
        services.AddScoped<IKeywordSearchService, KeywordSearchService>();

        // Orchestrator
        services.AddScoped<IRAGOrchestrator, RAGOrchestrator>();

        // HTTP Client with Polly
        services.AddHttpClient("OpenAI")
            .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    private static Polly.IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Polly.Extensions.Http.HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
