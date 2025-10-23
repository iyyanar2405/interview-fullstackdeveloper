using System.Linq;
using Microsoft.Extensions.Options;
using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class GuardrailEvaluationService
{
    private readonly IReadOnlyCollection<ReleaseGuardrail> _guardrails;
    private readonly ILogger<GuardrailEvaluationService> _logger;

    public GuardrailEvaluationService(IOptions<CloudDeploymentOptions> options, ILogger<GuardrailEvaluationService> logger)
    {
        _guardrails = options.Value.Guardrails;
        _logger = logger;

        if (_guardrails.Count == 0)
        {
            _logger.LogWarning("No release guardrails configured; guardrail evaluation will be informational only.");
        }
    }

    public IReadOnlyCollection<ReleaseGuardrail> Evaluate(DeploymentPlan plan)
    {
        if (_guardrails.Count == 0)
        {
            return Array.Empty<ReleaseGuardrail>();
        }

        var matches = new List<ReleaseGuardrail>();
        foreach (var guardrail in _guardrails)
        {
            if (AppliesToPlan(guardrail, plan))
            {
                matches.Add(guardrail);
            }
        }

        _logger.LogInformation("Guardrail evaluation completed for {Blueprint}: {Count} guardrails triggered", plan.BlueprintName, matches.Count);
        return matches;
    }

    private static bool AppliesToPlan(ReleaseGuardrail guardrail, DeploymentPlan plan)
    {
        if (string.IsNullOrWhiteSpace(guardrail.AppliesTo))
        {
            return false;
        }

        return guardrail.AppliesTo.Equals(plan.Provider, StringComparison.OrdinalIgnoreCase) ||
               guardrail.AppliesTo.Equals(plan.Environment, StringComparison.OrdinalIgnoreCase) ||
               plan.Steps.Any(step => step.Name.Contains(guardrail.AppliesTo, StringComparison.OrdinalIgnoreCase));
    }
}
