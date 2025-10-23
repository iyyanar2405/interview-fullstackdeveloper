namespace Module_06_Specialized_Features.Models;

public static class SpecializedFeatureKinds
{
    public const string CodeGeneration = "code-generation";
    public const string DataAnalysis = "data-analysis";
    public const string ContentCreation = "content-creation";
    public const string DocumentProcessing = "document-processing";
    public const string MultimodalSynthesis = "multimodal";

    public static bool TryNormalize(string? value, out string normalized)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = value.Trim().ToLowerInvariant();
        return normalized is CodeGeneration or DataAnalysis or ContentCreation or DocumentProcessing or MultimodalSynthesis;
    }
}

public sealed class SpecializedFeaturesOptions
{
    public SpecializedSimulationOptions Simulation { get; set; } = new();
    public List<CodeTemplateDefinition> CodeTemplates { get; set; } = new();
    public List<DataAnalysisWorkflowDefinition> AnalysisWorkflows { get; set; } = new();
    public List<ContentStyleDefinition> ContentStyles { get; set; } = new();
    public List<DocumentPipelineDefinition> DocumentPipelines { get; set; } = new();
    public List<ImageModelDefinition> ImageModels { get; set; } = new();
}

public sealed class SpecializedSimulationOptions
{
    public int WorkerIntervalMilliseconds { get; set; } = 400;
    public double FailureProbability { get; set; } = 0.08;
    public int MaxHistoryEntries { get; set; } = 250;
    public int MaxArtifacts { get; set; } = 300;
}

public sealed class CodeTemplateDefinition
{
    public string TemplateId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Scaffolding { get; set; } = new();
    public List<string> TestHints { get; set; } = new();
    public List<string> QualityChecks { get; set; } = new();
}

public sealed class DataAnalysisWorkflowDefinition
{
    public string WorkflowId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Dataset { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public List<string> Steps { get; set; } = new();
    public List<string> Metrics { get; set; } = new();
}

public sealed class ContentStyleDefinition
{
    public string StyleId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Tone { get; set; } = string.Empty;
    public List<string> Guidelines { get; set; } = new();
    public List<string> SampleOpeners { get; set; } = new();
}

public sealed class DocumentPipelineDefinition
{
    public string PipelineId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Stages { get; set; } = new();
    public List<string> OutputArtifacts { get; set; } = new();
}

public sealed class ImageModelDefinition
{
    public string ModelId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Modalities { get; set; } = new();
    public List<string> Capabilities { get; set; } = new();
    public List<string> SamplePrompts { get; set; } = new();
}

public sealed class SpecializedTaskRequest
{
    public string FeatureType { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = "system";
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;
    public Dictionary<string, string> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public SpecializedTaskRequest Clone()
    {
        return new SpecializedTaskRequest
        {
            FeatureType = FeatureType,
            RequestedBy = RequestedBy,
            CorrelationId = CorrelationId,
            RequestedAt = RequestedAt,
            Parameters = new Dictionary<string, string>(Parameters, StringComparer.OrdinalIgnoreCase)
        };
    }
}

public sealed class SpecializedExecutionResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public Dictionary<string, string> Outputs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Recommendations { get; set; } = new();
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public DateTimeOffset CompletedAt { get; set; } = DateTimeOffset.UtcNow;

    public SpecializedExecutionResult Clone()
    {
        return new SpecializedExecutionResult
        {
            Success = Success,
            Status = Status,
            Summary = Summary,
            Outputs = new Dictionary<string, string>(Outputs, StringComparer.OrdinalIgnoreCase),
            Recommendations = Recommendations.ToList(),
            Duration = Duration,
            CompletedAt = CompletedAt
        };
    }
}

public enum SpecializedRunStatus
{
    Pending,
    Running,
    Completed,
    Failed
}

public sealed class SpecializedRunLog
{
    public string RunId { get; set; } = string.Empty;
    public string FeatureType { get; set; } = string.Empty;
    public SpecializedTaskRequest Request { get; set; } = new();
    public SpecializedExecutionResult? Result { get; set; }
    public SpecializedRunStatus Status { get; set; } = SpecializedRunStatus.Pending;
    public string Notes { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
        = null;
    public DateTimeOffset? CompletedAt { get; set; }
        = null;

    public SpecializedRunLog Clone()
    {
        return new SpecializedRunLog
        {
            RunId = RunId,
            FeatureType = FeatureType,
            Request = Request.Clone(),
            Result = Result?.Clone(),
            Status = Status,
            Notes = Notes,
            CreatedAt = CreatedAt,
            StartedAt = StartedAt,
            CompletedAt = CompletedAt
        };
    }
}

public sealed class SpecializedArtifact
{
    public string RunId { get; set; } = string.Empty;
    public string FeatureType { get; set; } = string.Empty;
    public string ArtifactType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;

    public SpecializedArtifact Clone()
    {
        return new SpecializedArtifact
        {
            RunId = RunId,
            FeatureType = FeatureType,
            ArtifactType = ArtifactType,
            Name = Name,
            Content = Content,
            GeneratedAt = GeneratedAt
        };
    }
}

public sealed class SpecializedInsight
{
    public string Category { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public double Score { get; set; } = 0.0;
}

public sealed class SpecializedDashboard
{
    public List<SpecializedInsight> Insights { get; set; } = new();
    public Dictionary<string, int> RunCountsByFeature { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, double> AverageDurationByFeature { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class SpecializedTaskWorkItem
{
    public SpecializedTaskWorkItem(string runId, SpecializedTaskRequest request)
    {
        RunId = runId;
        Request = request;
    }

    public string RunId { get; }
    public SpecializedTaskRequest Request { get; }
}
