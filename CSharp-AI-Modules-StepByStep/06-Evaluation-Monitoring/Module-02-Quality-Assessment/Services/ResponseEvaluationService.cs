using System.Text.RegularExpressions;
using Module_02_Quality_Assessment.Models;
using Microsoft.Extensions.Options;

namespace Module_02_Quality_Assessment.Services;

/// <summary>
/// Performs lightweight heuristic evaluation of model responses across multiple dimensions.
/// </summary>
public sealed class ResponseEvaluationService
{
    private readonly QualityAssessmentOptions _options;
    private static readonly Regex SentenceRegex = new("[.!?]+", RegexOptions.Compiled);

    public ResponseEvaluationService(IOptions<QualityAssessmentOptions> options)
    {
        _options = options.Value;
    }

    public EvaluationResult Evaluate(EvaluationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Response))
        {
            return BuildEmptyResult("EMPTY_RESPONSE");
        }

        var sanitizedResponse = request.Response.Trim();
        var sanitizedPrompt = request.Prompt.Trim();

        var relevance = CalculateRelevance(sanitizedPrompt, sanitizedResponse);
        var coherence = CalculateCoherence(sanitizedResponse);
        var correctness = CalculateCorrectness(request.ExpectedAnswer, sanitizedResponse);
        var style = CalculateStyle(sanitizedResponse, request.Guidelines);

        var scores = new List<QualityDimensionScore>
        {
            new() { Dimension = "Relevance", Score = relevance.Score, Explanation = relevance.Explanation },
            new() { Dimension = "Coherence", Score = coherence.Score, Explanation = coherence.Explanation },
            new() { Dimension = "Correctness", Score = correctness.Score, Explanation = correctness.Explanation },
            new() { Dimension = "Style", Score = style.Score, Explanation = style.Explanation }
        };

        var overallNormalized = CalculateWeightedScore(scores.Select(s => (s.Dimension, s.Score / 100)).ToList());
        var meetsThreshold = overallNormalized >= _options.Thresholds.PassingScore
            && scores.All(s => (s.Score / 100) >= _options.Thresholds.MinimumDimensionScore);

        return new EvaluationResult
        {
            Scores = scores,
            OverallScore = Math.Round(overallNormalized * 100, 2),
            MeetsQualityThreshold = meetsThreshold,
            Flags = BuildFlags(scores, meetsThreshold),
            Metadata = request.Metadata
        };
    }

    private static (double Score, string Explanation) CalculateRelevance(string prompt, string response)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return (70, "Prompt unavailable; default relevance applied");
        }

        var promptTerms = Tokenize(prompt);
        var responseTerms = Tokenize(response);

        if (promptTerms.Count == 0 || responseTerms.Count == 0)
        {
            return (40, "Insufficient lexical overlap between prompt and response");
        }

        var overlap = promptTerms.Intersect(responseTerms, StringComparer.OrdinalIgnoreCase).Count();
        var relevanceScore = Math.Min(100, (double)overlap / promptTerms.Count * 100);
        var explanation = overlap switch
        {
            0 => "No key term overlap found",
            < 3 => "Limited overlap between prompt and response",
            _ => "Strong lexical overlap detected"
        };

        return (Math.Round(relevanceScore, 2), explanation);
    }

    private static (double Score, string Explanation) CalculateCoherence(string response)
    {
        var sentences = SentenceRegex.Split(response)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();

        if (sentences.Length <= 1)
        {
            return (60, "Single sentence response limits coherence assessment");
        }

        var averageSentenceLength = sentences.Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length).Average();
        var variance = sentences.Select(s => Math.Pow(s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length - averageSentenceLength, 2)).Average();
        var dispersion = Math.Sqrt(variance);

        var coherenceScore = 100 - Math.Min(60, dispersion * 10);
        var explanation = dispersion switch
        {
            < 2 => "Consistent sentence structure detected",
            < 4 => "Moderate sentence variation",
            _ => "High variance in sentence length"
        };

        return (Math.Round(coherenceScore, 2), explanation);
    }

    private static (double Score, string Explanation) CalculateCorrectness(string? expected, string response)
    {
        if (string.IsNullOrWhiteSpace(expected))
        {
            return (70, "Expected answer not provided; default correctness applied");
        }

        var expectedTokens = Tokenize(expected);
        var responseTokens = Tokenize(response);
        if (expectedTokens.Count == 0)
        {
            return (60, "Expected answer too short for comparison");
        }

        var overlap = expectedTokens.Intersect(responseTokens, StringComparer.OrdinalIgnoreCase).Count();
        var correctnessScore = (double)overlap / expectedTokens.Count * 100;
        var explanation = overlap switch
        {
            0 => "No expected concepts detected",
            < 3 => "Partial overlap with expected answer",
            _ => "Response covers expected concepts"
        };

        return (Math.Round(correctnessScore, 2), explanation);
    }

    private static (double Score, string Explanation) CalculateStyle(string response, IReadOnlyCollection<string> guidelines)
    {
        var trimmed = response.Trim();
        var length = trimmed.Length;
        var sentences = SentenceRegex.Split(response).Count(s => !string.IsNullOrWhiteSpace(s));
        var readability = sentences == 0 ? length : length / sentences;
        var guidelinePenalty = guidelines.Any()
            ? guidelines.Count(g => !response.Contains(g, StringComparison.OrdinalIgnoreCase)) * 5
            : 0;

        var baseScore = 100;
        if (trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            baseScore -= 25;
        }

        if (readability > 250)
        {
            baseScore -= 20;
        }

        baseScore -= guidelinePenalty;
        baseScore = Math.Clamp(baseScore, 30, 100);

        var explanation = guidelinePenalty > 0
            ? "Some requested style guidelines are missing"
            : readability switch
            {
                < 120 => "Concise and readable response",
                < 220 => "Moderately dense response",
                _ => "Response may be overly verbose"
            };

        return (Math.Round(baseScore, 2), explanation);
    }

    private double CalculateWeightedScore(IReadOnlyCollection<(string Dimension, double Score)> scores)
    {
        if (scores.Count == 0)
        {
            return 0;
        }

        double weighted = 0;
        foreach (var (dimension, score) in scores)
        {
            var weight = dimension switch
            {
                "Relevance" => _options.Weights.Relevance,
                "Coherence" => _options.Weights.Coherence,
                "Correctness" => _options.Weights.Correctness,
                "Style" => _options.Weights.Style,
                _ => 0
            };
            weighted += score * weight;
        }

        return weighted;
    }

    private static IReadOnlyCollection<string> BuildFlags(IEnumerable<QualityDimensionScore> scores, bool meetsThreshold)
    {
        var flags = new List<string>();
        if (!meetsThreshold)
        {
            flags.Add("QUALITY_THRESHOLD_FAILED");
        }

        foreach (var score in scores)
        {
            if (score.Score < 60)
            {
                flags.Add($"LOW_{score.Dimension.ToUpperInvariant()}_{Math.Round(score.Score)}");
            }
        }

        return flags;
    }

    private static EvaluationResult BuildEmptyResult(string flag)
    {
        return new EvaluationResult
        {
            Scores = Array.Empty<QualityDimensionScore>(),
            OverallScore = 0,
            MeetsQualityThreshold = false,
            Flags = new[] { flag }
        };
    }

    private static List<string> Tokenize(string input)
    {
        return input
            .Split(new[] { ' ', '\n', '\r', '\t', ',', '.', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => t.Length > 2)
            .Distinct()
            .ToList();
    }
}
