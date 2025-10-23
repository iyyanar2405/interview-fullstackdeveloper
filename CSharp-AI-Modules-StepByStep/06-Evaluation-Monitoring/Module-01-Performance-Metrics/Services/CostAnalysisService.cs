using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Module_01_Performance_Metrics.Models;

namespace Module_01_Performance_Metrics.Services;

public sealed class CostAnalysisService
{
    private readonly ConcurrentQueue<CostRecord> _costs = new();
    private readonly IOptionsMonitor<CostSettings> _settings;
    private readonly int _maxEntries;

    public CostAnalysisService(IOptionsMonitor<CostSettings> settings)
    {
        _settings = settings;
        _maxEntries = 5000;
    }

    public void RecordCost(RequestMetric metric)
    {
        var settings = _settings.CurrentValue;
        var tokens = metric.TokensUsed ?? settings.DefaultTokenUsage;
        var durationSeconds = metric.Duration.TotalSeconds;
        var requestSizeMb = metric.RequestBytes / 1024d / 1024d;
        var responseSizeMb = metric.ResponseBytes.HasValue ? metric.ResponseBytes.Value / 1024d / 1024d : 0;
        var sizeCost = (requestSizeMb + responseSizeMb) * settings.CostPerMegabyte;
        var durationCost = durationSeconds * settings.CostPerSecond;
        var tokenCost = tokens / 1000d * settings.TokenCostPerThousand;
        var totalCost = settings.BaseCostPerRequest + sizeCost + durationCost + tokenCost;

        var record = new CostRecord
        {
            Path = metric.Path,
            Cost = Math.Round(totalCost, 6),
            DurationMs = metric.Duration.TotalMilliseconds,
            TokensUsed = tokens,
            StatusCode = metric.StatusCode
        };

        _costs.Enqueue(record);
        while (_costs.Count > _maxEntries && _costs.TryDequeue(out _))
        {
        }
    }

    public CostSummary GetSummary(TimeSpan window)
    {
        var cutoff = DateTimeOffset.UtcNow - window;
        var windowCosts = _costs.Where(c => c.Timestamp >= cutoff).ToList();
        if (windowCosts.Count == 0)
        {
            return new CostSummary
            {
                Window = window,
                TotalCost = 0,
                AverageCostPerRequest = 0,
                AverageTokensPerRequest = 0,
                CostPerMinute = 0,
                RequestCount = 0
            };
        }

        var totalCost = windowCosts.Sum(c => c.Cost);
        var totalTokens = windowCosts.Sum(c => c.TokensUsed ?? 0);
        var minutes = Math.Max(window.TotalMinutes, 1);

        return new CostSummary
        {
            Window = window,
            TotalCost = totalCost,
            AverageCostPerRequest = totalCost / windowCosts.Count,
            AverageTokensPerRequest = totalTokens / windowCosts.Count,
            CostPerMinute = totalCost / minutes,
            RequestCount = windowCosts.Count
        };
    }

    public IReadOnlyCollection<CostRecord> GetRecent(int count)
    {
        return _costs.Reverse().Take(count).ToArray();
    }
}
