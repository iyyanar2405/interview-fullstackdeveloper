using AI.ModelManagement.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AI.ModelManagement.Services;

public interface IPerformanceMonitoringService
{
    Task RecordMetricsAsync(PerformanceMetrics metrics);
    Task<ModelHealthCheck> CheckModelHealthAsync(string modelId);
    Task<PerformanceReport> GetPerformanceReportAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<string, HealthStatus>> GetAllModelsHealthAsync();
    Task<List<string>> GetRecommendationsAsync();
}

public class PerformanceMonitoringService : IPerformanceMonitoringService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<PerformanceMonitoringService> _logger;
    private readonly MonitoringSettings _settings;
    private readonly IModelSelectionService _modelSelection;
    private readonly List<PerformanceMetrics> _metricsHistory = new();

    public PerformanceMonitoringService(
        IMemoryCache cache,
        ILogger<PerformanceMonitoringService> logger,
        IOptions<ModelManagementSettings> settings,
        IModelSelectionService modelSelection)
    {
        _cache = cache;
        _logger = logger;
        _settings = settings.Value.MonitoringSettings;
        _modelSelection = modelSelection;

        if (_settings.EnableHealthChecks)
        {
            StartHealthCheckLoop();
        }
    }

    public async Task RecordMetricsAsync(PerformanceMetrics metrics)
    {
        if (!_settings.EnablePerformanceTracking)
            return;

        metrics.Timestamp = DateTime.UtcNow;
        _metricsHistory.Add(metrics);

        // Keep only recent metrics based on retention policy
        var cutoffDate = DateTime.UtcNow.AddDays(-_settings.MetricsRetentionDays);
        _metricsHistory.RemoveAll(m => m.Timestamp < cutoffDate);

        // Update real-time stats in cache
        await UpdateRealtimeStatsAsync(metrics);

        _logger.LogDebug("Recorded metrics for model {ModelId}: Latency={Latency}ms, Success={Success}",
            metrics.ModelId, metrics.LatencyMs, metrics.Success);
    }

    public async Task<ModelHealthCheck> CheckModelHealthAsync(string modelId)
    {
        var recentMetrics = _metricsHistory
            .Where(m => m.ModelId == modelId)
            .Where(m => m.Timestamp >= DateTime.UtcNow.AddMinutes(-5))
            .ToList();

        if (!recentMetrics.Any())
        {
            return new ModelHealthCheck
            {
                ModelId = modelId,
                Status = HealthStatus.Unknown,
                LastChecked = DateTime.UtcNow,
                ResponseTimeMs = 0,
                SuccessRate = 0,
                AverageLatencyMs = 0,
                Issues = new List<string> { "No recent metrics available" }
            };
        }

        var successCount = recentMetrics.Count(m => m.Success);
        var successRate = (double)successCount / recentMetrics.Count;
        var avgLatency = recentMetrics.Average(m => m.LatencyMs);
        var issues = new List<string>();

        // Determine health status
        var status = HealthStatus.Healthy;
        
        if (successRate < 0.5)
        {
            status = HealthStatus.Unhealthy;
            issues.Add($"Low success rate: {successRate:P0}");
        }
        else if (successRate < 0.9)
        {
            status = HealthStatus.Degraded;
            issues.Add($"Degraded success rate: {successRate:P0}");
        }

        if (avgLatency > 5000)
        {
            status = HealthStatus.Unhealthy;
            issues.Add($"High latency: {avgLatency:F0}ms");
        }
        else if (avgLatency > 2000)
        {
            if (status == HealthStatus.Healthy)
                status = HealthStatus.Degraded;
            issues.Add($"Elevated latency: {avgLatency:F0}ms");
        }

        var errorTypes = recentMetrics
            .Where(m => !m.Success && m.ErrorType != null)
            .GroupBy(m => m.ErrorType)
            .OrderByDescending(g => g.Count())
            .Take(3);

        foreach (var errorGroup in errorTypes)
        {
            issues.Add($"Frequent error: {errorGroup.Key} ({errorGroup.Count()} occurrences)");
        }

        return await Task.FromResult(new ModelHealthCheck
        {
            ModelId = modelId,
            Status = status,
            LastChecked = DateTime.UtcNow,
            ResponseTimeMs = (int)avgLatency,
            SuccessRate = successRate,
            AverageLatencyMs = avgLatency,
            Issues = issues
        });
    }

    public async Task<PerformanceReport> GetPerformanceReportAsync(DateTime startDate, DateTime endDate)
    {
        var relevantMetrics = _metricsHistory
            .Where(m => m.Timestamp >= startDate && m.Timestamp <= endDate)
            .ToList();

        var modelPerformance = new Dictionary<string, ModelPerformance>();

        var modelGroups = relevantMetrics.GroupBy(m => m.ModelId);
        foreach (var group in modelGroups)
        {
            var metrics = group.ToList();
            var successful = metrics.Count(m => m.Success);
            var failed = metrics.Count - successful;
            var latencies = metrics.Select(m => m.LatencyMs).OrderBy(l => l).ToList();

            modelPerformance[group.Key] = new ModelPerformance
            {
                ModelId = group.Key,
                TotalRequests = metrics.Count,
                SuccessfulRequests = successful,
                FailedRequests = failed,
                SuccessRate = (double)successful / metrics.Count,
                AverageLatencyMs = metrics.Average(m => m.LatencyMs),
                P95LatencyMs = GetPercentile(latencies, 0.95),
                P99LatencyMs = GetPercentile(latencies, 0.99),
                TotalTokens = metrics.Sum(m => m.TokensPerSecond),
                AverageTokensPerSecond = metrics.Where(m => m.TokensPerSecond > 0).Any() 
                    ? metrics.Where(m => m.TokensPerSecond > 0).Average(m => m.TokensPerSecond) 
                    : 0,
                AverageQualityScore = metrics.Where(m => m.QualityScore.HasValue).Any()
                    ? metrics.Where(m => m.QualityScore.HasValue).Average(m => m.QualityScore!.Value)
                    : null
            };
        }

        var summary = GenerateSummary(modelPerformance);

        return await Task.FromResult(new PerformanceReport
        {
            StartDate = startDate,
            EndDate = endDate,
            ModelPerformance = modelPerformance,
            Summary = summary
        });
    }

    public async Task<Dictionary<string, HealthStatus>> GetAllModelsHealthAsync()
    {
        var models = await _modelSelection.GetAvailableModelsAsync();
        var healthStatus = new Dictionary<string, HealthStatus>();

        foreach (var model in models)
        {
            var health = await CheckModelHealthAsync(model.ModelId);
            healthStatus[model.ModelId] = health.Status;
        }

        return healthStatus;
    }

    public async Task<List<string>> GetRecommendationsAsync()
    {
        var recommendations = new List<string>();
        var report = await GetPerformanceReportAsync(
            DateTime.UtcNow.AddDays(-7), 
            DateTime.UtcNow);

        // Analyze performance and generate recommendations
        foreach (var (modelId, performance) in report.ModelPerformance)
        {
            if (performance.SuccessRate < 0.9)
            {
                recommendations.Add($"Consider replacing {modelId} - low success rate ({performance.SuccessRate:P0})");
            }

            if (performance.AverageLatencyMs > 3000)
            {
                recommendations.Add($"Model {modelId} has high latency ({performance.AverageLatencyMs:F0}ms) - consider alternatives");
            }

            if (performance.P99LatencyMs > 10000)
            {
                recommendations.Add($"Model {modelId} has inconsistent performance - P99 latency is {performance.P99LatencyMs:F0}ms");
            }
        }

        // Compare models and suggest optimization
        if (report.ModelPerformance.Count > 1)
        {
            var sortedBySuccessRate = report.ModelPerformance
                .OrderByDescending(kv => kv.Value.SuccessRate)
                .ToList();

            var best = sortedBySuccessRate.First();
            recommendations.Add($"Most reliable model: {best.Key} with {best.Value.SuccessRate:P0} success rate");

            var sortedByLatency = report.ModelPerformance
                .OrderBy(kv => kv.Value.AverageLatencyMs)
                .ToList();

            var fastest = sortedByLatency.First();
            recommendations.Add($"Fastest model: {fastest.Key} with {fastest.Value.AverageLatencyMs:F0}ms average latency");
        }

        return recommendations;
    }

    #region Private Helper Methods

    private async Task UpdateRealtimeStatsAsync(PerformanceMetrics metrics)
    {
        var statsKey = $"stats_{metrics.ModelId}";
        var stats = _cache.GetOrCreate(statsKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return new
            {
                TotalRequests = 0,
                SuccessfulRequests = 0,
                TotalLatency = 0.0,
                LastUpdate = DateTime.UtcNow
            };
        });

        var updatedStats = new
        {
            TotalRequests = stats.TotalRequests + 1,
            SuccessfulRequests = stats.SuccessfulRequests + (metrics.Success ? 1 : 0),
            TotalLatency = stats.TotalLatency + metrics.LatencyMs,
            LastUpdate = DateTime.UtcNow
        };

        _cache.Set(statsKey, updatedStats, TimeSpan.FromMinutes(5));

        await Task.CompletedTask;
    }

    private double GetPercentile(List<int> sortedValues, double percentile)
    {
        if (!sortedValues.Any())
            return 0;

        int index = (int)Math.Ceiling(sortedValues.Count * percentile) - 1;
        index = Math.Max(0, Math.Min(index, sortedValues.Count - 1));
        return sortedValues[index];
    }

    private PerformanceSummary GenerateSummary(Dictionary<string, ModelPerformance> modelPerformance)
    {
        if (!modelPerformance.Any())
        {
            return new PerformanceSummary
            {
                TotalRequests = 0,
                OverallSuccessRate = 0,
                AverageLatencyMs = 0,
                Recommendations = new List<string> { "No performance data available" }
            };
        }

        var totalRequests = modelPerformance.Sum(kv => kv.Value.TotalRequests);
        var totalSuccessful = modelPerformance.Sum(kv => kv.Value.SuccessfulRequests);
        var overallSuccessRate = (double)totalSuccessful / totalRequests;

        var weightedLatency = modelPerformance.Sum(kv => 
            kv.Value.AverageLatencyMs * kv.Value.TotalRequests) / totalRequests;

        var fastestModel = modelPerformance
            .OrderBy(kv => kv.Value.AverageLatencyMs)
            .First().Key;

        var mostReliableModel = modelPerformance
            .OrderByDescending(kv => kv.Value.SuccessRate)
            .First().Key;

        var mostUsedModel = modelPerformance
            .OrderByDescending(kv => kv.Value.TotalRequests)
            .First().Key;

        var recommendations = new List<string>
        {
            $"Overall success rate: {overallSuccessRate:P1}",
            $"Average latency: {weightedLatency:F0}ms",
            $"Best performance: {fastestModel}",
            $"Most reliable: {mostReliableModel}",
            $"Most used: {mostUsedModel}"
        };

        return new PerformanceSummary
        {
            TotalRequests = totalRequests,
            OverallSuccessRate = overallSuccessRate,
            AverageLatencyMs = weightedLatency,
            FastestModel = fastestModel,
            MostReliableModel = mostReliableModel,
            MostUsedModel = mostUsedModel,
            Recommendations = recommendations
        };
    }

    private void StartHealthCheckLoop()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    var models = await _modelSelection.GetAvailableModelsAsync();
                    foreach (var model in models)
                    {
                        var health = await CheckModelHealthAsync(model.ModelId);
                        
                        if (health.Status == HealthStatus.Unhealthy)
                        {
                            _logger.LogWarning("Model {ModelId} is unhealthy: {Issues}",
                                model.ModelId, string.Join(", ", health.Issues));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during health check loop");
                }

                await Task.Delay(TimeSpan.FromSeconds(_settings.HealthCheckIntervalSeconds));
            }
        });
    }

    #endregion
}
