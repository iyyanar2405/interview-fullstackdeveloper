using System.Text;
using Module_06_Specialized_Features.Models;

namespace Module_06_Specialized_Features.Services;

public sealed class CodeGenerationService
{
    private readonly SpecializedFeatureCatalogService _catalog;

    public CodeGenerationService(SpecializedFeatureCatalogService catalog)
    {
        _catalog = catalog;
    }

    public async Task<(SpecializedExecutionResult Result, List<SpecializedArtifact> Artifacts)> GenerateAsync(SpecializedTaskRequest request, CancellationToken cancellationToken)
    {
        if (!request.Parameters.TryGetValue("templateId", out var templateId))
        {
            throw new InvalidOperationException("templateId parameter is required for code generation.");
        }

        var template = _catalog.GetCodeTemplate(templateId) ?? throw new InvalidOperationException($"Code template '{templateId}' not found.");
        var problem = request.Parameters.TryGetValue("problem", out var prompt) ? prompt : template.Description;
        var moduleName = request.Parameters.TryGetValue("module", out var module) ? module : "Module";

        var startedAt = DateTimeOffset.UtcNow;
        await Task.Delay(Random.Shared.Next(150, 280), cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine("// Auto-generated solution snippet");
        builder.AppendLine($"// Template: {template.Name}");
        builder.AppendLine($"// Problem: {problem}");
        builder.AppendLine();

        foreach (var line in template.Scaffolding)
        {
            builder.AppendLine(line.Replace("{{Module}}", moduleName));
        }

        var code = builder.ToString();
        var completedAt = DateTimeOffset.UtcNow;

        var outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["language"] = template.Language,
            ["lines"] = code.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length.ToString(),
            ["qualityCheckCount"] = template.QualityChecks.Count.ToString()
        };

        var recommendations = template.QualityChecks
            .Select(check => $"Confirm: {check}")
            .Concat(template.TestHints.Select(hint => $"Test Hint: {hint}"))
            .ToList();

        var result = new SpecializedExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Generated {template.Language} snippet for '{problem}'.",
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
                FeatureType = SpecializedFeatureKinds.CodeGeneration,
                ArtifactType = "code",
                Name = $"{template.TemplateId}-solution.{ResolveExtension(template.Language)}",
                Content = code,
                GeneratedAt = completedAt
            }
        };

        return (result, artifacts);
    }

    private static string ResolveExtension(string language)
    {
        return language.ToLowerInvariant() switch
        {
            "c#" or "csharp" => "cs",
            "python" => "py",
            "javascript" => "js",
            "typescript" => "ts",
            "java" => "java",
            _ => "txt"
        };
    }
}
