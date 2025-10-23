using System.Text;
using Module_02_CICD_Pipelines.Models;

namespace Module_02_CICD_Pipelines.Services;

public sealed class PipelineGeneratorService
{
    public PipelinePlan Generate(PipelineTemplate template, PipelineRequest request)
    {
        var yaml = template.Provider switch
        {
            PipelineProvider.GitHubActions => GenerateGitHubActions(template, request),
            PipelineProvider.AzureDevOps => GenerateAzurePipeline(template, request),
            PipelineProvider.GitLabCi => GenerateGitLabPipeline(template, request),
            _ => GenerateGenericPipeline(template, request)
        };

        var matrix = request.UseMatrix
            ? request.RuntimeVersions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            : new Dictionary<string, string>();

        var recommended = new List<string>(template.QualityChecks);
        if (request.IncludeSecurityScanning && !recommended.Contains("security-scan", StringComparer.OrdinalIgnoreCase))
        {
            recommended.Add("security-scan");
        }

        if (request.IncludeTests && !recommended.Contains("unit-tests", StringComparer.OrdinalIgnoreCase))
        {
            recommended.Add("unit-tests");
        }

        if (request.IncludeDeployment)
        {
            recommended.Add("deployment-approval-gates");
        }

        return new PipelinePlan
        {
            PipelineYaml = yaml,
            Provider = template.Provider.ToString(),
            EnvironmentMatrix = matrix,
            RecommendedChecks = recommended.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            DeploymentStages = request.Environments
        };
    }

    private static string GenerateGitHubActions(PipelineTemplate template, PipelineRequest request)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"name: {template.Name}");
        builder.AppendLine("on:");
        builder.AppendLine($"  {template.Trigger}:\n    branches:\n      - {request.DefaultBranch}");

        builder.AppendLine("jobs:");
        foreach (var job in template.Jobs)
        {
            builder.AppendLine($"  {job.Name}:");
            builder.AppendLine($"    runs-on: {job.RunsOn}");
            if (request.UseMatrix)
            {
                builder.AppendLine("    strategy:");
                builder.AppendLine("      matrix:");
                foreach (var kvp in request.RuntimeVersions)
                {
                    builder.AppendLine($"        {kvp.Key}: [{kvp.Value}]");
                }
            }

            if (job.Environment.Count > 0)
            {
                builder.AppendLine("    env:");
                foreach (var env in job.Environment)
                {
                    builder.AppendLine($"      {env.Key}: {env.Value}");
                }
            }

            builder.AppendLine("    steps:");
            foreach (var step in job.Steps)
            {
                builder.AppendLine($"      - {step}");
            }
        }

        return builder.ToString();
    }

    private static string GenerateAzurePipeline(PipelineTemplate template, PipelineRequest request)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"trigger:\n  branches:\n    include:\n      - {request.DefaultBranch}");
        builder.AppendLine("stages:");
        foreach (var env in request.Environments)
        {
            builder.AppendLine($"- stage: {env.ToUpperInvariant()}");
            builder.AppendLine($"  displayName: {env.ToUpperInvariant()} Stage");
            builder.AppendLine("  jobs:");
            foreach (var job in template.Jobs)
            {
                builder.AppendLine("  - job: " + job.Name);
                builder.AppendLine("    pool:");
                builder.AppendLine("      vmImage: " + job.RunsOn);
                builder.AppendLine("    steps:");
                foreach (var step in job.Steps)
                {
                    builder.AppendLine("    - " + step);
                }
            }
        }

        return builder.ToString();
    }

    private static string GenerateGitLabPipeline(PipelineTemplate template, PipelineRequest request)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"stages: [{string.Join(", ", request.Environments)}]");
        foreach (var job in template.Jobs)
        {
            builder.AppendLine($"{job.Name}:");
            builder.AppendLine($"  stage: {request.Environments.FirstOrDefault("build")}");
            builder.AppendLine($"  image: {job.RunsOn}");
            builder.AppendLine("  script:");
            foreach (var step in job.Steps)
            {
                builder.AppendLine($"    - {step}");
            }
        }

        return builder.ToString();
    }

    private static string GenerateGenericPipeline(PipelineTemplate template, PipelineRequest request)
    {
        var builder = new StringBuilder();
        builder.AppendLine("pipeline:");
        builder.AppendLine($"  provider: {template.Provider}");
        builder.AppendLine($"  trigger: {template.Trigger}");
        builder.AppendLine("  stages:");
        foreach (var stage in request.Environments)
        {
            builder.AppendLine($"    - {stage}");
        }

        builder.AppendLine("  jobs:");
        foreach (var job in template.Jobs)
        {
            builder.AppendLine($"    - name: {job.Name}");
            builder.AppendLine($"      image: {job.RunsOn}");
            builder.AppendLine("      steps:");
            foreach (var step in job.Steps)
            {
                builder.AppendLine($"        - {step}");
            }
        }

        return builder.ToString();
    }
}
