using AI.Embeddings.Models;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AI.Embeddings.Services;

#region Embedding Service Interface

public interface IEmbeddingService
{
    Task<EmbeddingResponse> GenerateEmbeddingsAsync(EmbeddingRequest request);
    Task<EmbeddingResult> GenerateSingleEmbeddingAsync(string text, EmbeddingModel model, EmbeddingProvider provider);
    EmbeddingModelInfo GetModelInfo(EmbeddingModel model);
    List<EmbeddingModelInfo> GetAvailableModels();
}

#endregion

#region OpenAI Embedding Service

public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly OpenAIClient _client;
    private readonly EmbeddingSettings _settings;
    private readonly IEmbeddingCacheService _cacheService;

    public OpenAIEmbeddingService(
        IOptions<EmbeddingSettings> settings,
        IEmbeddingCacheService cacheService)
    {
        _settings = settings.Value;
        _cacheService = cacheService;
        _client = new OpenAIClient(_settings.OpenAIApiKey);
    }

    public async Task<EmbeddingResponse> GenerateEmbeddingsAsync(EmbeddingRequest request)
    {
        var response = new EmbeddingResponse
        {
            Model = request.Model,
            Provider = request.Provider
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var uncachedTexts = new List<(string text, int originalIndex)>();
            var results = new List<EmbeddingResult>();

            // Check cache first
            if (request.Options.UseCache && _settings.EnableCaching)
            {
                for (int i = 0; i < request.Texts.Count; i++)
                {
                    var cached = await _cacheService.GetAsync(request.Texts[i], request.Model);
                    if (cached != null)
                    {
                        results.Add(new EmbeddingResult
                        {
                            Index = i,
                            Text = request.Texts[i],
                            Embedding = cached.Embedding,
                            Dimensions = cached.Embedding.Length,
                            Metadata = new EmbeddingMetadata
                            {
                                FromCache = true,
                                CacheKey = cached.Key,
                                TokenCount = EstimateTokenCount(request.Texts[i])
                            }
                        });
                        response.Usage.CacheHits++;
                    }
                    else
                    {
                        uncachedTexts.Add((request.Texts[i], i));
                        response.Usage.CacheMisses++;
                    }
                }
            }
            else
            {
                uncachedTexts = request.Texts.Select((t, i) => (t, i)).ToList();
            }

            // Generate embeddings for uncached texts
            if (uncachedTexts.Any())
            {
                var embeddingOptions = new EmbeddingsOptions(
                    GetDeploymentName(request.Model),
                    uncachedTexts.Select(t => t.text).ToList())
                {
                    User = request.Options.User,
                    Dimensions = request.Options.Dimensions
                };

                var azureResponse = await _client.GetEmbeddingsAsync(embeddingOptions);

                for (int i = 0; i < azureResponse.Value.Data.Count; i++)
                {
                    var embeddingItem = azureResponse.Value.Data[i];
                    var (text, originalIndex) = uncachedTexts[i];
                    
                    var embedding = embeddingItem.Embedding.ToArray();
                    
                    if (request.Options.Normalize)
                    {
                        embedding = NormalizeVector(embedding);
                    }

                    var result = new EmbeddingResult
                    {
                        Index = originalIndex,
                        Text = text,
                        Embedding = embedding,
                        Dimensions = embedding.Length,
                        Metadata = new EmbeddingMetadata
                        {
                            FromCache = false,
                            TokenCount = EstimateTokenCount(text)
                        }
                    };

                    results.Add(result);

                    // Cache the result
                    if (request.Options.UseCache && _settings.EnableCaching)
                    {
                        await _cacheService.SetAsync(text, request.Model, embedding, _settings.CacheDurationMinutes);
                    }
                }

                response.Usage.PromptTokens = azureResponse.Value.Usage.PromptTokens;
                response.Usage.TotalTokens = azureResponse.Value.Usage.TotalTokens;
                response.Usage.EstimatedCost = CalculateCost(azureResponse.Value.Usage.TotalTokens, request.Model);
            }

            response.Results = results.OrderBy(r => r.Index).ToList();
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add($"OpenAI embedding generation failed: {ex.Message}");
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;

        return response;
    }

    public async Task<EmbeddingResult> GenerateSingleEmbeddingAsync(string text, EmbeddingModel model, EmbeddingProvider provider)
    {
        var request = new EmbeddingRequest
        {
            Texts = new List<string> { text },
            Model = model,
            Provider = provider
        };

        var response = await GenerateEmbeddingsAsync(request);

        if (!response.Success || !response.Results.Any())
        {
            throw new Exception($"Failed to generate embedding: {string.Join(", ", response.Errors)}");
        }

        return response.Results.First();
    }

    public EmbeddingModelInfo GetModelInfo(EmbeddingModel model)
    {
        return model switch
        {
            EmbeddingModel.TextEmbedding3Small => new EmbeddingModelInfo
            {
                Model = model,
                Name = "text-embedding-3-small",
                DisplayName = "Text Embedding 3 Small",
                DefaultDimensions = 1536,
                MaxTokens = 8191,
                CostPer1KTokens = 0.00002m,
                Provider = EmbeddingProvider.OpenAI,
                SupportsCustomDimensions = true,
                AvailableDimensions = new List<int> { 512, 1536 }
            },
            EmbeddingModel.TextEmbedding3Large => new EmbeddingModelInfo
            {
                Model = model,
                Name = "text-embedding-3-large",
                DisplayName = "Text Embedding 3 Large",
                DefaultDimensions = 3072,
                MaxTokens = 8191,
                CostPer1KTokens = 0.00013m,
                Provider = EmbeddingProvider.OpenAI,
                SupportsCustomDimensions = true,
                AvailableDimensions = new List<int> { 256, 1024, 3072 }
            },
            EmbeddingModel.TextEmbeddingAda002 => new EmbeddingModelInfo
            {
                Model = model,
                Name = "text-embedding-ada-002",
                DisplayName = "Text Embedding Ada 002",
                DefaultDimensions = 1536,
                MaxTokens = 8191,
                CostPer1KTokens = 0.0001m,
                Provider = EmbeddingProvider.OpenAI,
                SupportsCustomDimensions = false
            },
            _ => throw new NotSupportedException($"Model {model} is not supported")
        };
    }

    public List<EmbeddingModelInfo> GetAvailableModels()
    {
        return new List<EmbeddingModelInfo>
        {
            GetModelInfo(EmbeddingModel.TextEmbedding3Small),
            GetModelInfo(EmbeddingModel.TextEmbedding3Large),
            GetModelInfo(EmbeddingModel.TextEmbeddingAda002)
        };
    }

    private string GetDeploymentName(EmbeddingModel model)
    {
        return model switch
        {
            EmbeddingModel.TextEmbedding3Small => "text-embedding-3-small",
            EmbeddingModel.TextEmbedding3Large => "text-embedding-3-large",
            EmbeddingModel.TextEmbeddingAda002 => "text-embedding-ada-002",
            _ => throw new NotSupportedException($"Model {model} is not supported")
        };
    }

    private float[] NormalizeVector(float[] vector)
    {
        var magnitude = Math.Sqrt(vector.Sum(v => v * v));
        return vector.Select(v => (float)(v / magnitude)).ToArray();
    }

    private int EstimateTokenCount(string text)
    {
        // Rough estimation: ~4 characters per token
        return text.Length / 4;
    }

    private decimal CalculateCost(int tokens, EmbeddingModel model)
    {
        var modelInfo = GetModelInfo(model);
        return (tokens / 1000m) * modelInfo.CostPer1KTokens;
    }
}

#endregion

#region Azure OpenAI Embedding Service

public class AzureOpenAIEmbeddingService : IEmbeddingService
{
    private readonly OpenAIClient _client;
    private readonly EmbeddingSettings _settings;
    private readonly IEmbeddingCacheService _cacheService;

    public AzureOpenAIEmbeddingService(
        IOptions<EmbeddingSettings> settings,
        IEmbeddingCacheService cacheService)
    {
        _settings = settings.Value;
        _cacheService = cacheService;

        if (string.IsNullOrEmpty(_settings.AzureEndpoint) || string.IsNullOrEmpty(_settings.AzureApiKey))
        {
            throw new ArgumentException("Azure endpoint and API key must be configured");
        }

        _client = new OpenAIClient(
            new Uri(_settings.AzureEndpoint),
            new AzureKeyCredential(_settings.AzureApiKey));
    }

    public async Task<EmbeddingResponse> GenerateEmbeddingsAsync(EmbeddingRequest request)
    {
        // Similar implementation to OpenAI but using Azure endpoint
        var response = new EmbeddingResponse
        {
            Model = request.Model,
            Provider = EmbeddingProvider.Azure
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var embeddingOptions = new EmbeddingsOptions(
                GetDeploymentName(request.Model),
                request.Texts)
            {
                User = request.Options.User,
                Dimensions = request.Options.Dimensions
            };

            var azureResponse = await _client.GetEmbeddingsAsync(embeddingOptions);

            response.Results = azureResponse.Value.Data.Select((item, index) =>
            {
                var embedding = item.Embedding.ToArray();
                
                if (request.Options.Normalize)
                {
                    embedding = NormalizeVector(embedding);
                }

                return new EmbeddingResult
                {
                    Index = index,
                    Text = request.Texts[index],
                    Embedding = embedding,
                    Dimensions = embedding.Length,
                    Metadata = new EmbeddingMetadata
                    {
                        TokenCount = EstimateTokenCount(request.Texts[index])
                    }
                };
            }).ToList();

            response.Usage.PromptTokens = azureResponse.Value.Usage.PromptTokens;
            response.Usage.TotalTokens = azureResponse.Value.Usage.TotalTokens;
            response.Usage.EstimatedCost = CalculateCost(azureResponse.Value.Usage.TotalTokens, request.Model);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add($"Azure OpenAI embedding generation failed: {ex.Message}");
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;

        return response;
    }

    public async Task<EmbeddingResult> GenerateSingleEmbeddingAsync(string text, EmbeddingModel model, EmbeddingProvider provider)
    {
        var request = new EmbeddingRequest
        {
            Texts = new List<string> { text },
            Model = model,
            Provider = provider
        };

        var response = await GenerateEmbeddingsAsync(request);
        return response.Results.First();
    }

    public EmbeddingModelInfo GetModelInfo(EmbeddingModel model)
    {
        return model switch
        {
            EmbeddingModel.AzureTextEmbedding3Small => new EmbeddingModelInfo
            {
                Model = model,
                Name = "text-embedding-3-small",
                DisplayName = "Azure Text Embedding 3 Small",
                DefaultDimensions = 1536,
                MaxTokens = 8191,
                CostPer1KTokens = 0.00002m,
                Provider = EmbeddingProvider.Azure,
                SupportsCustomDimensions = true
            },
            EmbeddingModel.AzureTextEmbedding3Large => new EmbeddingModelInfo
            {
                Model = model,
                Name = "text-embedding-3-large",
                DisplayName = "Azure Text Embedding 3 Large",
                DefaultDimensions = 3072,
                MaxTokens = 8191,
                CostPer1KTokens = 0.00013m,
                Provider = EmbeddingProvider.Azure,
                SupportsCustomDimensions = true
            },
            _ => throw new NotSupportedException($"Model {model} is not supported for Azure")
        };
    }

    public List<EmbeddingModelInfo> GetAvailableModels()
    {
        return new List<EmbeddingModelInfo>
        {
            GetModelInfo(EmbeddingModel.AzureTextEmbedding3Small),
            GetModelInfo(EmbeddingModel.AzureTextEmbedding3Large)
        };
    }

    private string GetDeploymentName(EmbeddingModel model)
    {
        return model switch
        {
            EmbeddingModel.AzureTextEmbedding3Small => "text-embedding-3-small",
            EmbeddingModel.AzureTextEmbedding3Large => "text-embedding-3-large",
            _ => throw new NotSupportedException($"Model {model} is not supported for Azure")
        };
    }

    private float[] NormalizeVector(float[] vector)
    {
        var magnitude = Math.Sqrt(vector.Sum(v => v * v));
        return vector.Select(v => (float)(v / magnitude)).ToArray();
    }

    private int EstimateTokenCount(string text)
    {
        return text.Length / 4;
    }

    private decimal CalculateCost(int tokens, EmbeddingModel model)
    {
        var modelInfo = GetModelInfo(model);
        return (tokens / 1000m) * modelInfo.CostPer1KTokens;
    }
}

#endregion

#region Embedding Service Factory

public interface IEmbeddingServiceFactory
{
    IEmbeddingService GetService(EmbeddingProvider provider);
}

public class EmbeddingServiceFactory : IEmbeddingServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<EmbeddingProvider, Type> _serviceTypes;

    public EmbeddingServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _serviceTypes = new Dictionary<EmbeddingProvider, Type>
        {
            { EmbeddingProvider.OpenAI, typeof(OpenAIEmbeddingService) },
            { EmbeddingProvider.Azure, typeof(AzureOpenAIEmbeddingService) }
        };
    }

    public IEmbeddingService GetService(EmbeddingProvider provider)
    {
        if (!_serviceTypes.ContainsKey(provider))
        {
            throw new NotSupportedException($"Provider {provider} is not supported");
        }

        return (IEmbeddingService)_serviceProvider.GetService(_serviceTypes[provider])!;
    }
}

#endregion
