using System.Collections.Concurrent;
using Module_02_Quality_Assessment.Models;

namespace Module_02_Quality_Assessment.Services;

/// <summary>
/// Aggregates evaluation results and provides rolling quality metrics over configurable windows.
/// </summary>
public sealed class QualityMetricsService
{
    private readonly ConcurrentQueue<(EvaluationResult Result, DateTimeOffset Timestamp)> _evaluations = new();
    private readonly int _maxEntries;

    public QualityMetricsService(int maxEntries = 5000)
    {
        _maxEntries = Math.Max(1000, maxEntries);
    }

    public void Record(EvaluationResult result)
    {
        _evaluations.Enqueue((result, DateTimeOffset.UtcNow));
        while (_evaluations.Count > _maxEntries && _evaluations.TryDequeue(out _))
        {
        }
    }

    public EvaluationSummary GetSummary(TimeSpan window)
    {
        var cutoff = DateTimeOffset.UtcNow - window;
        var windowResults = _evaluations
            .Where(e => e.Timestamp >= cutoff)
            .Select(e => e.Result)
            .ToList();

        if (windowResults.Count == 0)
        {
            return new EvaluationSummary { Window = window };
        }

        double AverageDimension(string dimension) => windowResults
            .SelectMany(r => r.Scores)
            .Where(s => s.Dimension.Equals(dimension, StringComparison.OrdinalIgnoreCase))
            .DefaultIfEmpty(new QualityDimensionScore { Score = 0 })
            .Average(s => s.Score);

        var summary = new EvaluationSummary
        {
            Window = window,
            EvaluationCount = windowResults.Count,
            AverageScore = Math.Round(windowResults.Average(r => r.OverallScore), 2),
            PassRate = Math.Round(windowResults.Count(r => r.MeetsQualityThreshold) / (double)windowResults.Count, 3),
            AverageRelevance = Math.Round(AverageDimension("Relevance"), 2),
            AverageCoherence = Math.Round(AverageDimension("Coherence"), 2),
            AverageCorrectness = Math.Round(AverageDimension("Correctness"), 2),
            BiasAlertCount = windowResults.Count(r => r.Bias.ContainsBias),
            GeneratedAt = DateTimeOffset.UtcNow
        };

        return summary;
    }

    public IReadOnlyCollection<EvaluationResult> GetRecent(int count = 20)
    {
        return _evaluations
            .Reverse()
            .Take(count)
            .Select(e => e.Result)
            .ToArray();
    }
}
