using Module_06_Specialized_Features.Models;

namespace Module_06_Specialized_Features.Services;

public sealed class DataAnalysisService
{
    private readonly SpecializedFeatureCatalogService _catalog;

    public DataAnalysisService(SpecializedFeatureCatalogService catalog)
    {
        _catalog = catalog;
    }

    public async Task<(SpecializedExecutionResult Result, List<SpecializedArtifact> Artifacts)> AnalyzeAsync(SpecializedTaskRequest request, CancellationToken cancellationToken)
    {
        if (!request.Parameters.TryGetValue("workflowId", out var workflowId))
        {
            throw new InvalidOperationException("workflowId parameter is required for data analysis.");
        }

        var workflow = _catalog.GetAnalysisWorkflow(workflowId) ?? throw new InvalidOperationException($"Analysis workflow '{workflowId}' not found.");
        var focusMetric = request.Parameters.TryGetValue("metric", out var metric) ? metric : workflow.Metrics.FirstOrDefault() ?? "accuracy";

        var startedAt = DateTimeOffset.UtcNow;
        await Task.Delay(Random.Shared.Next(200, 320), cancellationToken);

        var samplesEvaluated = Random.Shared.Next(500, 2000);
        var mean = Math.Round(Random.Shared.NextDouble() * 0.6 + 0.3, 3);
        var stdDev = Math.Round(Random.Shared.NextDouble() * 0.1, 3);

        var outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["workflow"] = workflow.Name,
            ["dataset"] = workflow.Dataset,
            ["metric"] = focusMetric,
            ["mean"] = mean.ToString("F3"),
            ["stdDev"] = stdDev.ToString("F3"),
            ["samples"] = samplesEvaluated.ToString()
        };

        var recommendations = new List<string>
        {
            "Review anomalies for potential data quality issues.",
            "Consider additional feature engineering iterations.",
            "Validate results with a holdout dataset."
        };

        var completedAt = DateTimeOffset.UtcNow;
        var result = new SpecializedExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Computed {focusMetric} metrics on {workflow.Dataset}.",
            Outputs = outputs,
            Recommendations = recommendations,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        var artifacts = new List<SpecializedArtifact>
        {
            new SpecializedArtifact
            {
                RunId = string.Empty,
                FeatureType = SpecializedFeatureKinds.DataAnalysis,
                ArtifactType = "report",
                Name = $"{workflow.WorkflowId}-metrics.json",
                Content = System.Text.Json.JsonSerializer.Serialize(outputs, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                }),
                GeneratedAt = completedAt
            }
        };

        return (result, artifacts);
    }
}
