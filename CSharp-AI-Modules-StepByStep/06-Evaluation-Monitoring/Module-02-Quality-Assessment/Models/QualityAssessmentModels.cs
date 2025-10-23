namespace Module_02_Quality_Assessment.Models;

public sealed class EvaluationRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public string? ExpectedAnswer { get; set; }
        = null; // Optional ground truth for correctness checks
    public string? Context { get; set; }
        = null; // Additional documents or conversation context
    public List<string> Guidelines { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public sealed class QualityDimensionScore
{
    public string Dimension { get; init; } = string.Empty;
    public double Score { get; init; }
        = 0; // 0-100 scale for easy interpretation
    public string Explanation { get; init; } = string.Empty;
}

public sealed class BiasMatch
{
    public string Term { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Snippet { get; init; } = string.Empty;
}

public sealed class BiasReport
{
    public bool ContainsBias { get; init; }
        = false;
    public double BiasProbability { get; init; }
        = 0; // 0-1 scale representing heuristic confidence
    public IReadOnlyCollection<BiasMatch> Matches { get; init; }
        = Array.Empty<BiasMatch>();
}

public sealed class EvaluationResult
{
    public string EvaluationId { get; init; } = Guid.NewGuid().ToString("N");
    public DateTimeOffset EvaluatedAt { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyCollection<QualityDimensionScore> Scores { get; init; }
        = Array.Empty<QualityDimensionScore>();
    public double OverallScore { get; init; }
        = 0; // Weighted composite score
    public bool MeetsQualityThreshold { get; init; }
        = true;
    public BiasReport Bias { get; init; } = new();
    public IReadOnlyCollection<string> Flags { get; init; }
        = Array.Empty<string>();
    public IDictionary<string, string> Metadata { get; init; }
        = new Dictionary<string, string>();
}

public sealed class EvaluationSummary
{
    public TimeSpan Window { get; init; } = TimeSpan.FromMinutes(60);
    public int EvaluationCount { get; init; }
        = 0;
    public double AverageScore { get; init; }
        = 0;
    public double PassRate { get; init; }
        = 0;
    public double AverageRelevance { get; init; }
        = 0;
    public double AverageCoherence { get; init; }
        = 0;
    public double AverageCorrectness { get; init; }
        = 0;
    public int BiasAlertCount { get; init; }
        = 0;
    public DateTimeOffset GeneratedAt { get; init; }
        = DateTimeOffset.UtcNow;
}

public sealed class FeedbackSubmission
{
    public string ResponseId { get; set; } = string.Empty;
    public double Rating { get; set; }
        = 0; // 1-5 rating scale
    public string? Comments { get; set; }
        = null;
    public string? SubmittedBy { get; set; }
        = null;
    public IDictionary<string, string> Metadata { get; set; }
        = new Dictionary<string, string>();
}

public sealed class FeedbackAggregate
{
    public string ResponseId { get; init; } = string.Empty;
    public double AverageRating { get; init; } = 0;
    public int RatingCount { get; init; } = 0;
    public IReadOnlyCollection<string> RecentComments { get; init; }
        = Array.Empty<string>();
}

public sealed class QualityWeights
{
    public double Relevance { get; set; } = 0.35;
    public double Coherence { get; set; } = 0.25;
    public double Correctness { get; set; } = 0.30;
    public double Style { get; set; } = 0.10;
}

public sealed class QualityThresholds
{
    public double PassingScore { get; set; } = 0.75; // 0-1 scale
    public double BiasProbability { get; set; } = 0.35;
    public double MinimumDimensionScore { get; set; } = 0.50;
}

public sealed class QualityAssessmentOptions
{
    public QualityWeights Weights { get; set; } = new();
    public QualityThresholds Thresholds { get; set; } = new();
    public IReadOnlyCollection<string> CriticalKeywords { get; set; }
        = Array.Empty<string>();
    public Dictionary<string, List<string>> BiasLexicon { get; set; }
        = new(StringComparer.OrdinalIgnoreCase);
}
