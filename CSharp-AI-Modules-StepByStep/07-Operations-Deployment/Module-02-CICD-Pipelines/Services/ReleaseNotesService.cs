using System.Linq;
using Module_02_CICD_Pipelines.Models;

namespace Module_02_CICD_Pipelines.Services;

public sealed class ReleaseNotesService
{
    public Dictionary<string, string> BuildReleaseNotes(PipelineRequest request, PipelinePlan plan)
    {
        var notes = new Dictionary<string, string>
        {
            ["project"] = request.ProjectName,
            ["provider"] = plan.Provider,
            ["environments"] = string.Join(", ", plan.DeploymentStages),
            ["checks"] = string.Join(", ", plan.RecommendedChecks)
        };

        if (plan.EnvironmentMatrix.Count > 0)
        {
            notes["matrix"] = string.Join("; ", plan.EnvironmentMatrix.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
        }

        return notes;
    }
}
