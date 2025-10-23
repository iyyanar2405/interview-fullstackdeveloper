using System.Linq;
using Microsoft.Extensions.Options;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class ComplianceInsightsService
{
    private readonly IReadOnlyCollection<ComplianceRule> _rules;
    private readonly ILogger<ComplianceInsightsService> _logger;

    public ComplianceInsightsService(IOptions<InfrastructureOptions> options, ILogger<ComplianceInsightsService> logger)
    {
        _rules = options.Value.ComplianceRules;
        _logger = logger;
        if (_rules.Count == 0)
        {
            _logger.LogWarning("No compliance rules configured; evaluations will always return pass.");
        }
    }

    public IReadOnlyCollection<ComplianceResult> Evaluate(IEnumerable<IaCResource> resources)
    {
        var resourceList = resources.ToList();
        var results = new List<ComplianceResult>();

        foreach (var rule in _rules)
        {
            var appliesTo = resourceList
                .Where(r => RuleApplies(rule, r))
                .ToList();

            if (appliesTo.Count == 0)
            {
                results.Add(new ComplianceResult
                {
                    RuleId = rule.Id,
                    Severity = rule.Severity,
                    Status = "not-applicable",
                    Message = "Rule did not match any resources"
                });
                continue;
            }

            var passed = appliesTo.All(resource => MeetsRequirements(rule, resource));
            results.Add(new ComplianceResult
            {
                RuleId = rule.Id,
                Severity = rule.Severity,
                Status = passed ? "pass" : "fail",
                Message = passed
                    ? "All resources satisfy declared requirements"
                    : $"{appliesTo.Count(r => !MeetsRequirements(rule, r))} resource(s) missing required settings"
            });
        }

        if (_rules.Count == 0)
        {
            results.Add(new ComplianceResult
            {
                RuleId = "baseline",
                Severity = "low",
                Status = "pass",
                Message = "No compliance rules defined"
            });
        }

        return results;
    }

    private static bool RuleApplies(ComplianceRule rule, IaCResource resource)
    {
        return rule.AppliesTo.Count == 0 || rule.AppliesTo.Any(type => resource.Type.Contains(type, StringComparison.OrdinalIgnoreCase));
    }

    private static bool MeetsRequirements(ComplianceRule rule, IaCResource resource)
    {
        foreach (var requirement in rule.Requirements)
        {
            if (!TryMatchRequirement(resource, requirement))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryMatchRequirement(IaCResource resource, KeyValuePair<string, string> requirement)
    {
        if (resource.Properties.TryGetValue(requirement.Key, out var propertyValue) &&
            string.Equals(propertyValue, requirement.Value, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (resource.SecurityControls.TryGetValue(requirement.Key, out var controlValue) &&
            string.Equals(controlValue, requirement.Value, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
