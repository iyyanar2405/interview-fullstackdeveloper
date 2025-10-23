using Module_02_CICD_Pipelines.Models;

namespace Module_02_CICD_Pipelines.Services;

public sealed class PipelineInsightsService
{
    public List<string> BuildStageSteps(PipelineTemplate template, PipelineRequest request)
    {
        var steps = new List<string>();
        if (request.IncludeTests)
        {
            steps.Add("run-tests");
        }

        if (request.IncludeSecurityScanning)
        {
            steps.Add("security-scan");
        }

        if (request.IncludeInfrastructure)
        {
            steps.Add("terraform-validate");
        }

        if (request.IncludeDeployment)
        {
            steps.Add("deployment-approval");
        }

        steps.AddRange(template.QualityChecks);
        return steps.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public List<PipelineStageLog> CreateStageLogs(PipelinePlan plan, PipelineRequest request)
    {
        var logs = new List<PipelineStageLog>();
        foreach (var stage in plan.DeploymentStages)
        {
            logs.Add(new PipelineStageLog
            {
                Stage = stage,
                Logs = new List<string>
                {
                    $"Starting stage {stage}",
                    $"Applying recommended checks: {string.Join(", ", plan.RecommendedChecks)}",
                    "Stage completed successfully"
                },
                Status = "Succeeded",
                Duration = TimeSpan.FromMinutes(Random.Shared.Next(2, 7))
            });
        }

        if (!request.IncludeDeployment)
        {
            logs.Add(new PipelineStageLog
            {
                Stage = "validation",
                Logs = new List<string> { "Validation complete" },
                Status = "Succeeded",
                Duration = TimeSpan.FromMinutes(1)
            });
        }

        return logs;
    }
}
