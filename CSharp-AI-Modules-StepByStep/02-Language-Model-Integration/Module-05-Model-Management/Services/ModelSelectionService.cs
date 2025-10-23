using AI.ModelManagement.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AI.ModelManagement.Services;

public interface IModelSelectionService
{
    Task<ModelSelectionResponse> SelectModelAsync(ModelSelectionRequest request);
    Task<List<ModelConfiguration>> GetAvailableModelsAsync();
    Task<ModelConfiguration?> GetModelByIdAsync(string modelId);
    Task UpdateModelStatusAsync(string modelId, ModelStatus status);
}

public class ModelSelectionService : IModelSelectionService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<ModelSelectionService> _logger;
    private readonly ModelManagementSettings _settings;

    public ModelSelectionService(
        IMemoryCache cache,
        ILogger<ModelSelectionService> logger,
        IOptions<ModelManagementSettings> settings)
    {
        _cache = cache;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<ModelSelectionResponse> SelectModelAsync(ModelSelectionRequest request)
    {
        _logger.LogInformation("Selecting model using strategy: {Strategy}", request.Strategy);

        var availableModels = await GetEligibleModelsAsync(request);
        
        if (!availableModels.Any())
        {
            throw new InvalidOperationException("No models available matching the requirements");
        }

        var selectedModel = request.Strategy switch
        {
            SelectionStrategy.CostOptimized => SelectCostOptimizedModel(availableModels, request),
            SelectionStrategy.PerformanceOptimized => SelectPerformanceOptimizedModel(availableModels, request),
            SelectionStrategy.Balanced => SelectBalancedModel(availableModels, request),
            SelectionStrategy.CapabilityBased => SelectCapabilityBasedModel(availableModels, request),
            SelectionStrategy.LoadBalanced => SelectLoadBalancedModel(availableModels, request),
            SelectionStrategy.Weighted => SelectWeightedModel(availableModels, request),
            _ => availableModels.First()
        };

        var fallbackModels = await GetFallbackModelsAsync(selectedModel, availableModels, request);

        var response = new ModelSelectionResponse
        {
            SelectedModel = selectedModel,
            FallbackModels = fallbackModels,
            Reasoning = GenerateReasoning(selectedModel, availableModels, request),
            EstimatedCost = EstimateCost(selectedModel, 1000), // Estimate for 1000 tokens
            EstimatedLatencyMs = EstimateLatency(selectedModel)
        };

        _logger.LogInformation("Selected model: {ModelId} with {FallbackCount} fallbacks", 
            selectedModel.ModelId, fallbackModels.Count);

        return response;
    }

    public async Task<List<ModelConfiguration>> GetAvailableModelsAsync()
    {
        return await Task.FromResult(_settings.Models
            .Where(m => m.Status == ModelStatus.Available)
            .OrderBy(m => m.Priority)
            .ToList());
    }

    public async Task<ModelConfiguration?> GetModelByIdAsync(string modelId)
    {
        return await Task.FromResult(_settings.Models
            .FirstOrDefault(m => m.ModelId == modelId));
    }

    public async Task UpdateModelStatusAsync(string modelId, ModelStatus status)
    {
        var model = _settings.Models.FirstOrDefault(m => m.ModelId == modelId);
        if (model != null)
        {
            model.Status = status;
            _logger.LogInformation("Updated model {ModelId} status to {Status}", modelId, status);
        }
        await Task.CompletedTask;
    }

    #region Private Helper Methods

    private async Task<List<ModelConfiguration>> GetEligibleModelsAsync(ModelSelectionRequest request)
    {
        var models = await GetAvailableModelsAsync();

        // Filter by required capabilities
        if (request.RequiredCapabilities.Any())
        {
            models = models.Where(m => 
                request.RequiredCapabilities.All(rc => m.Capabilities.Contains(rc))
            ).ToList();
        }

        // Apply constraints
        if (request.Constraints != null)
        {
            models = ApplyConstraints(models, request.Constraints);
        }

        return models;
    }

    private List<ModelConfiguration> ApplyConstraints(
        List<ModelConfiguration> models, 
        ModelConstraints constraints)
    {
        if (constraints.PreferredProviders != null && constraints.PreferredProviders.Any())
        {
            var preferred = models.Where(m => constraints.PreferredProviders.Contains(m.Provider)).ToList();
            if (preferred.Any())
                models = preferred;
        }

        if (constraints.ExcludedProviders != null)
        {
            models = models.Where(m => !constraints.ExcludedProviders.Contains(m.Provider)).ToList();
        }

        if (constraints.ExcludedModels != null)
        {
            models = models.Where(m => !constraints.ExcludedModels.Contains(m.ModelId)).ToList();
        }

        if (constraints.MinContextWindow.HasValue)
        {
            models = models.Where(m => m.Limits.ContextWindow >= constraints.MinContextWindow.Value).ToList();
        }

        return models;
    }

    private ModelConfiguration SelectCostOptimizedModel(
        List<ModelConfiguration> models, 
        ModelSelectionRequest request)
    {
        return models.OrderBy(m => m.Pricing.InputTokenCost + m.Pricing.OutputTokenCost).First();
    }

    private ModelConfiguration SelectPerformanceOptimizedModel(
        List<ModelConfiguration> models, 
        ModelSelectionRequest request)
    {
        // Prioritize models with higher context windows and better performance characteristics
        return models
            .OrderByDescending(m => m.Limits.ContextWindow)
            .ThenByDescending(m => m.Limits.MaxRequestsPerMinute)
            .First();
    }

    private ModelConfiguration SelectBalancedModel(
        List<ModelConfiguration> models, 
        ModelSelectionRequest request)
    {
        // Score each model based on cost and performance
        var scoredModels = models.Select(m => new
        {
            Model = m,
            Score = CalculateBalancedScore(m)
        }).OrderByDescending(x => x.Score);

        return scoredModels.First().Model;
    }

    private double CalculateBalancedScore(ModelConfiguration model)
    {
        // Normalize cost (lower is better)
        var costScore = 1.0 / (1.0 + (double)(model.Pricing.InputTokenCost + model.Pricing.OutputTokenCost));
        
        // Normalize performance (higher is better)
        var performanceScore = (double)model.Limits.MaxRequestsPerMinute / 10000.0;
        
        // Context window score
        var contextScore = (double)model.Limits.ContextWindow / 200000.0;

        return (costScore * 0.4) + (performanceScore * 0.3) + (contextScore * 0.3);
    }

    private ModelConfiguration SelectCapabilityBasedModel(
        List<ModelConfiguration> models, 
        ModelSelectionRequest request)
    {
        // Select model with the most matching capabilities
        return models
            .OrderByDescending(m => m.Capabilities.Count)
            .ThenBy(m => m.Pricing.InputTokenCost)
            .First();
    }

    private ModelConfiguration SelectLoadBalancedModel(
        List<ModelConfiguration> models, 
        ModelSelectionRequest request)
    {
        // Simple round-robin based on cache
        var cacheKey = "load_balance_index";
        var index = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return 0;
        });

        var selectedModel = models[index % models.Count];
        _cache.Set(cacheKey, (index + 1) % models.Count);

        return selectedModel;
    }

    private ModelConfiguration SelectWeightedModel(
        List<ModelConfiguration> models, 
        ModelSelectionRequest request)
    {
        var scoredModels = models.Select(m => new
        {
            Model = m,
            Score = CalculateWeightedScore(m, request.WeightingFactors)
        }).OrderByDescending(x => x.Score);

        return scoredModels.First().Model;
    }

    private double CalculateWeightedScore(
        ModelConfiguration model, 
        Dictionary<string, double> weights)
    {
        double score = 0;

        if (weights.TryGetValue("cost", out var costWeight))
        {
            var costScore = 1.0 / (1.0 + (double)(model.Pricing.InputTokenCost + model.Pricing.OutputTokenCost));
            score += costScore * costWeight;
        }

        if (weights.TryGetValue("performance", out var perfWeight))
        {
            var perfScore = (double)model.Limits.MaxRequestsPerMinute / 10000.0;
            score += perfScore * perfWeight;
        }

        if (weights.TryGetValue("context", out var contextWeight))
        {
            var contextScore = (double)model.Limits.ContextWindow / 200000.0;
            score += contextScore * contextWeight;
        }

        if (weights.TryGetValue("priority", out var priorityWeight))
        {
            score += model.Priority * priorityWeight;
        }

        return score;
    }

    private async Task<List<ModelConfiguration>> GetFallbackModelsAsync(
        ModelConfiguration selectedModel,
        List<ModelConfiguration> availableModels,
        ModelSelectionRequest request)
    {
        var fallbacks = availableModels
            .Where(m => m.ModelId != selectedModel.ModelId)
            .Where(m => request.RequiredCapabilities.All(rc => m.Capabilities.Contains(rc)))
            .OrderBy(m => m.Priority)
            .Take(3)
            .ToList();

        return await Task.FromResult(fallbacks);
    }

    private SelectionReasoning GenerateReasoning(
        ModelConfiguration selectedModel,
        List<ModelConfiguration> availableModels,
        ModelSelectionRequest request)
    {
        var reasoning = new SelectionReasoning
        {
            PrimaryReason = $"Selected using {request.Strategy} strategy",
            ConsideredFactors = new List<string>
            {
                $"Available models: {availableModels.Count}",
                $"Required capabilities: {string.Join(", ", request.RequiredCapabilities)}",
                $"Strategy: {request.Strategy}"
            }
        };

        reasoning.Scores[selectedModel.ModelId] = CalculateBalancedScore(selectedModel);

        foreach (var model in availableModels.Take(5))
        {
            if (model.ModelId != selectedModel.ModelId)
            {
                reasoning.Scores[model.ModelId] = CalculateBalancedScore(model);
            }
        }

        return reasoning;
    }

    private decimal EstimateCost(ModelConfiguration model, int tokens)
    {
        var inputCost = (model.Pricing.InputTokenCost * tokens) / 1000m;
        var outputCost = (model.Pricing.OutputTokenCost * tokens) / 1000m;
        return inputCost + outputCost + model.Pricing.RequestCost;
    }

    private int EstimateLatency(ModelConfiguration model)
    {
        // Simple estimation based on provider and model type
        return model.Provider switch
        {
            ModelProvider.OpenAI => 500,
            ModelProvider.AzureOpenAI => 400,
            ModelProvider.Anthropic => 600,
            ModelProvider.Cohere => 550,
            ModelProvider.Local => 200,
            _ => 500
        };
    }

    #endregion
}
