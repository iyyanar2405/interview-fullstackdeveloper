using AI.ModelManagement.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;

namespace AI.ModelManagement.Services;

#region Fallback Service

public interface IFallbackService
{
    Task<FallbackResponse> GetFallbackModelAsync(FallbackRequest request);
    Task<bool> ShouldTriggerFallbackAsync(string modelId, string errorType);
    Task RecordFailureAsync(string modelId, string errorType);
}

public class FallbackService : IFallbackService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<FallbackService> _logger;
    private readonly FallbackConfiguration _config;
    private readonly IModelSelectionService _modelSelection;
    private readonly Dictionary<string, int> _failureCount = new();

    public FallbackService(
        IMemoryCache cache,
        ILogger<FallbackService> logger,
        IOptions<ModelManagementSettings> settings,
        IModelSelectionService modelSelection)
    {
        _cache = cache;
        _logger = logger;
        _config = settings.Value.FallbackConfig;
        _modelSelection = modelSelection;
    }

    public async Task<FallbackResponse> GetFallbackModelAsync(FallbackRequest request)
    {
        _logger.LogInformation("Finding fallback for model {ModelId} due to: {Reason}", 
            request.OriginalModelId, request.FailureReason);

        var triedModels = new List<string> { request.OriginalModelId };
        var attemptNumber = 1;

        // Get fallback models based on strategy
        var fallbackCandidates = await GetFallbackCandidatesAsync(request);

        foreach (var candidate in fallbackCandidates)
        {
            if (await IsModelHealthyAsync(candidate.ModelId))
            {
                return new FallbackResponse
                {
                    FallbackModel = candidate,
                    Success = true,
                    Reason = $"Fallback to {candidate.Name} using {_config.Strategy} strategy",
                    AttemptNumber = attemptNumber,
                    TriedModels = triedModels
                };
            }

            triedModels.Add(candidate.ModelId);
            attemptNumber++;
        }

        return new FallbackResponse
        {
            Success = false,
            Reason = "No healthy fallback models available",
            AttemptNumber = attemptNumber,
            TriedModels = triedModels
        };
    }

    public async Task<bool> ShouldTriggerFallbackAsync(string modelId, string errorType)
    {
        var failureKey = $"failure_{modelId}";
        var failures = _cache.GetOrCreate(failureKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return 0;
        });

        // Trigger fallback if failures exceed threshold
        return await Task.FromResult(failures >= 3 || 
            errorType.Contains("RateLimit") || 
            errorType.Contains("Unavailable"));
    }

    public async Task RecordFailureAsync(string modelId, string errorType)
    {
        var failureKey = $"failure_{modelId}";
        var failures = _cache.GetOrCreate(failureKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return 0;
        });

        _cache.Set(failureKey, failures + 1);
        _logger.LogWarning("Recorded failure for model {ModelId}: {ErrorType} (Total: {Count})", 
            modelId, errorType, failures + 1);

        await Task.CompletedTask;
    }

    private async Task<List<ModelConfiguration>> GetFallbackCandidatesAsync(FallbackRequest request)
    {
        var allModels = await _modelSelection.GetAvailableModelsAsync();

        var candidates = _config.Strategy switch
        {
            FallbackStrategy.Sequential => GetSequentialFallbacks(request, allModels),
            FallbackStrategy.CostBased => GetCostBasedFallbacks(request, allModels),
            FallbackStrategy.QualityBased => GetQualityBasedFallbacks(request, allModels),
            FallbackStrategy.LoadBased => GetLoadBasedFallbacks(request, allModels),
            FallbackStrategy.Random => GetRandomFallbacks(request, allModels),
            _ => allModels.Where(m => m.ModelId != request.OriginalModelId).ToList()
        };

        return candidates.Take(5).ToList();
    }

    private List<ModelConfiguration> GetSequentialFallbacks(
        FallbackRequest request, 
        List<ModelConfiguration> allModels)
    {
        return allModels
            .Where(m => m.ModelId != request.OriginalModelId)
            .Where(m => request.RequiredCapabilities.All(rc => m.Capabilities.Contains(rc)))
            .OrderBy(m => m.Priority)
            .ToList();
    }

    private List<ModelConfiguration> GetCostBasedFallbacks(
        FallbackRequest request, 
        List<ModelConfiguration> allModels)
    {
        return allModels
            .Where(m => m.ModelId != request.OriginalModelId)
            .Where(m => request.RequiredCapabilities.All(rc => m.Capabilities.Contains(rc)))
            .OrderBy(m => m.Pricing.InputTokenCost + m.Pricing.OutputTokenCost)
            .ToList();
    }

    private List<ModelConfiguration> GetQualityBasedFallbacks(
        FallbackRequest request, 
        List<ModelConfiguration> allModels)
    {
        return allModels
            .Where(m => m.ModelId != request.OriginalModelId)
            .Where(m => request.RequiredCapabilities.All(rc => m.Capabilities.Contains(rc)))
            .OrderByDescending(m => m.Limits.ContextWindow)
            .ToList();
    }

    private List<ModelConfiguration> GetLoadBasedFallbacks(
        FallbackRequest request, 
        List<ModelConfiguration> allModels)
    {
        // Prefer models with higher rate limits
        return allModels
            .Where(m => m.ModelId != request.OriginalModelId)
            .Where(m => request.RequiredCapabilities.All(rc => m.Capabilities.Contains(rc)))
            .OrderByDescending(m => m.Limits.MaxRequestsPerMinute)
            .ToList();
    }

    private List<ModelConfiguration> GetRandomFallbacks(
        FallbackRequest request, 
        List<ModelConfiguration> allModels)
    {
        var random = new Random();
        return allModels
            .Where(m => m.ModelId != request.OriginalModelId)
            .Where(m => request.RequiredCapabilities.All(rc => m.Capabilities.Contains(rc)))
            .OrderBy(_ => random.Next())
            .ToList();
    }

    private async Task<bool> IsModelHealthyAsync(string modelId)
    {
        var healthKey = $"health_{modelId}";
        var isHealthy = _cache.GetOrCreate(healthKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return true; // Assume healthy unless proven otherwise
        });

        return await Task.FromResult(isHealthy);
    }
}

#endregion

#region Cost Optimization Service

public interface ICostOptimizationService
{
    Task<CostEntry> TrackCostAsync(CostTrackingRequest request);
    Task<CostReport> GetCostReportAsync(DateTime startDate, DateTime endDate);
    Task<bool> CheckBudgetAsync(decimal estimatedCost);
    Task<List<CostAlert>> GetActiveAlertsAsync();
    Task<decimal> GetRemainingBudgetAsync(string period); // "daily" or "monthly"
}

public class CostOptimizationService : ICostOptimizationService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CostOptimizationService> _logger;
    private readonly CostOptimizationConfig _config;
    private readonly IModelSelectionService _modelSelection;
    private readonly List<CostEntry> _costHistory = new();

    public CostOptimizationService(
        IMemoryCache cache,
        ILogger<CostOptimizationService> logger,
        IOptions<ModelManagementSettings> settings,
        IModelSelectionService modelSelection)
    {
        _cache = cache;
        _logger = logger;
        _config = settings.Value.CostConfig;
        _modelSelection = modelSelection;
    }

    public async Task<CostEntry> TrackCostAsync(CostTrackingRequest request)
    {
        var model = await _modelSelection.GetModelByIdAsync(request.ModelId);
        if (model == null)
        {
            throw new ArgumentException($"Model {request.ModelId} not found");
        }

        var inputCost = (model.Pricing.InputTokenCost * request.InputTokens) / 1000m;
        var outputCost = (model.Pricing.OutputTokenCost * request.OutputTokens) / 1000m;
        var totalCost = inputCost + outputCost + model.Pricing.RequestCost;

        var entry = new CostEntry
        {
            Timestamp = DateTime.UtcNow,
            ModelId = request.ModelId,
            UserId = request.UserId,
            InputTokens = request.InputTokens,
            OutputTokens = request.OutputTokens,
            Cost = totalCost,
            Tags = request.Tags
        };

        _costHistory.Add(entry);

        // Update daily total
        var dailyKey = $"cost_daily_{DateTime.UtcNow:yyyyMMdd}";
        var dailyTotal = _cache.GetOrCreate(dailyKey, cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
            return 0m;
        });
        _cache.Set(dailyKey, dailyTotal + totalCost);

        // Update monthly total
        var monthlyKey = $"cost_monthly_{DateTime.UtcNow:yyyyMM}";
        var monthlyTotal = _cache.GetOrCreate(monthlyKey, cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(31);
            return 0m;
        });
        _cache.Set(monthlyKey, monthlyTotal + totalCost);

        // Check for alerts
        await CheckCostAlertsAsync(dailyTotal + totalCost, monthlyTotal + totalCost);

        _logger.LogInformation("Tracked cost: ${Cost:F4} for model {ModelId} (Input: {Input} tokens, Output: {Output} tokens)",
            totalCost, request.ModelId, request.InputTokens, request.OutputTokens);

        return entry;
    }

    public async Task<CostReport> GetCostReportAsync(DateTime startDate, DateTime endDate)
    {
        var entries = _costHistory
            .Where(e => e.Timestamp >= startDate && e.Timestamp <= endDate)
            .ToList();

        var totalCost = entries.Sum(e => e.Cost);
        var costByModel = entries
            .GroupBy(e => e.ModelId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Cost));

        var costByUser = entries
            .GroupBy(e => e.UserId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Cost));

        var modelGroups = entries.GroupBy(e => e.ModelId);
        var costByProvider = new Dictionary<string, decimal>();
        foreach (var group in modelGroups)
        {
            var model = await _modelSelection.GetModelByIdAsync(group.Key);
            if (model != null)
            {
                var provider = model.Provider.ToString();
                if (!costByProvider.ContainsKey(provider))
                    costByProvider[provider] = 0;
                costByProvider[provider] += group.Sum(e => e.Cost);
            }
        }

        var topExpenses = entries
            .OrderByDescending(e => e.Cost)
            .Take(10)
            .ToList();

        var days = (endDate - startDate).Days + 1;
        var averageDailyCost = totalCost / days;
        var projectedMonthlyCost = averageDailyCost * 30;

        var report = new CostReport
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalCost = totalCost,
            CostByModel = costByModel,
            CostByProvider = costByProvider,
            CostByUser = costByUser,
            TopExpenses = topExpenses,
            Trend = new CostTrend
            {
                AverageDailyCost = averageDailyCost,
                ProjectedMonthlyCost = projectedMonthlyCost,
                GrowthRate = CalculateGrowthRate(entries),
                Alerts = await GetActiveAlertsAsync()
            }
        };

        return report;
    }

    public async Task<bool> CheckBudgetAsync(decimal estimatedCost)
    {
        var dailyRemaining = await GetRemainingBudgetAsync("daily");
        var monthlyRemaining = await GetRemainingBudgetAsync("monthly");

        return estimatedCost <= dailyRemaining && estimatedCost <= monthlyRemaining;
    }

    public async Task<List<CostAlert>> GetActiveAlertsAsync()
    {
        var alerts = new List<CostAlert>();
        var dailyKey = $"cost_daily_{DateTime.UtcNow:yyyyMMdd}";
        var monthlyKey = $"cost_monthly_{DateTime.UtcNow:yyyyMM}";

        var dailyTotal = _cache.GetOrCreate(dailyKey, _ => 0m);
        var monthlyTotal = _cache.GetOrCreate(monthlyKey, _ => 0m);

        // Daily budget alert
        if (_config.DailyBudget > 0)
        {
            var dailyPercentage = (dailyTotal / _config.DailyBudget) * 100;
            if (dailyPercentage >= _config.AlertThresholdPercentage)
            {
                alerts.Add(new CostAlert
                {
                    AlertType = "DailyBudget",
                    Message = $"Daily budget at {dailyPercentage:F1}%",
                    CurrentAmount = dailyTotal,
                    ThresholdAmount = _config.DailyBudget,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Monthly budget alert
        if (_config.MonthlyBudget > 0)
        {
            var monthlyPercentage = (monthlyTotal / _config.MonthlyBudget) * 100;
            if (monthlyPercentage >= _config.AlertThresholdPercentage)
            {
                alerts.Add(new CostAlert
                {
                    AlertType = "MonthlyBudget",
                    Message = $"Monthly budget at {monthlyPercentage:F1}%",
                    CurrentAmount = monthlyTotal,
                    ThresholdAmount = _config.MonthlyBudget,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        return await Task.FromResult(alerts);
    }

    public async Task<decimal> GetRemainingBudgetAsync(string period)
    {
        if (period.ToLower() == "daily")
        {
            var dailyKey = $"cost_daily_{DateTime.UtcNow:yyyyMMdd}";
            var dailyTotal = _cache.GetOrCreate(dailyKey, _ => 0m);
            return await Task.FromResult(Math.Max(0, _config.DailyBudget - dailyTotal));
        }
        else if (period.ToLower() == "monthly")
        {
            var monthlyKey = $"cost_monthly_{DateTime.UtcNow:yyyyMM}";
            var monthlyTotal = _cache.GetOrCreate(monthlyKey, _ => 0m);
            return await Task.FromResult(Math.Max(0, _config.MonthlyBudget - monthlyTotal));
        }

        return 0m;
    }

    private async Task CheckCostAlertsAsync(decimal dailyTotal, decimal monthlyTotal)
    {
        if (!_config.EnableCostAlerts) return;

        var alerts = await GetActiveAlertsAsync();
        foreach (var alert in alerts)
        {
            _logger.LogWarning("Cost Alert: {Message}", alert.Message);
        }
    }

    private double CalculateGrowthRate(List<CostEntry> entries)
    {
        if (entries.Count < 2) return 0;

        var sortedEntries = entries.OrderBy(e => e.Timestamp).ToList();
        var midPoint = sortedEntries.Count / 2;
        
        var firstHalfTotal = sortedEntries.Take(midPoint).Sum(e => (double)e.Cost);
        var secondHalfTotal = sortedEntries.Skip(midPoint).Sum(e => (double)e.Cost);

        if (firstHalfTotal == 0) return 0;

        return ((secondHalfTotal - firstHalfTotal) / firstHalfTotal) * 100;
    }
}

#endregion
