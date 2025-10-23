using Module_02_Quality_Assessment.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Module_02_Quality_Assessment.Services;

/// <summary>
/// Coordinates the evaluation pipeline: quality scoring, bias detection, and metric persistence.
/// </summary>
public sealed class EvaluationPipelineService
{
    private readonly ResponseEvaluationService _responseEvaluation;
    private readonly BiasDetectionService _biasDetection;
    private readonly QualityMetricsService _metrics;
    private readonly QualityAssessmentOptions _options;
    private readonly ILogger<EvaluationPipelineService> _logger;

    public EvaluationPipelineService(
        ResponseEvaluationService responseEvaluation,
        BiasDetectionService biasDetection,
        QualityMetricsService metrics,
        IOptions<QualityAssessmentOptions> options,
        ILogger<EvaluationPipelineService> logger)
    {
        _responseEvaluation = responseEvaluation;
        _biasDetection = biasDetection;
        _metrics = metrics;
        _options = options.Value;
        _logger = logger;
    }

    public Task<EvaluationResult> EvaluateAsync(EvaluationRequest request)
    {
        var qualityResult = _responseEvaluation.Evaluate(request);
        var bias = _biasDetection.Analyze(request.Response);

        var meetsThreshold = qualityResult.MeetsQualityThreshold;
        var flags = qualityResult.Flags.ToList();
        if (bias.ContainsBias && bias.BiasProbability >= _options.Thresholds.BiasProbability)
        {
            flags.Add("BIAS_ALERT");
            meetsThreshold = false;
        }

        foreach (var keyword in _options.CriticalKeywords)
        {
            if (!string.IsNullOrWhiteSpace(keyword) && !request.Response.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                flags.Add($"MISSING_KEYWORD_{keyword.ToUpperInvariant()}");
            }
        }

        if (flags.Any(f => f.StartsWith("MISSING_KEYWORD", StringComparison.OrdinalIgnoreCase)))
        {
            meetsThreshold = false;
        }

        var enriched = new EvaluationResult
        {
            EvaluationId = qualityResult.EvaluationId,
            EvaluatedAt = qualityResult.EvaluatedAt,
            Scores = qualityResult.Scores,
            OverallScore = qualityResult.OverallScore,
            MeetsQualityThreshold = meetsThreshold,
            Bias = bias,
            Flags = flags.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            Metadata = qualityResult.Metadata
        };

        _metrics.Record(enriched);
        _logger.LogInformation(
            "Evaluation completed for response. Score: {Score}, Passed: {Passed}, Bias: {BiasProbability}",
            enriched.OverallScore,
            enriched.MeetsQualityThreshold,
            bias.BiasProbability);

        return Task.FromResult(enriched);
    }
}
