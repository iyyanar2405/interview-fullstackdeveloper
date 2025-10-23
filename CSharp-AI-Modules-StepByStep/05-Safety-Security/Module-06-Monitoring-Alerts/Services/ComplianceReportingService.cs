using Module_06_Monitoring_Alerts.Models;

namespace Module_06_Monitoring_Alerts.Services;

/// <summary>
/// Service for generating compliance reports across frameworks
/// </summary>
public class ComplianceReportingService
{
    private readonly List<ComplianceReport> _reports = new();

    /// <summary>
    /// Generate a compliance report based on framework and input controls
    /// </summary>
    public Task<ComplianceReport> GenerateReportAsync(ComplianceFramework framework, DateTime periodStart, DateTime periodEnd, Dictionary<string, ControlStatus> controls, List<ComplianceViolation> violations)
    {
        var report = new ComplianceReport
        {
            ReportName = $"{framework} Compliance Report",
            Framework = framework,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            GeneratedAt = DateTime.UtcNow,
            Controls = controls,
            Violations = violations,
            OverallStatus = CalculateOverallStatus(controls, violations),
            ComplianceScore = CalculateComplianceScore(controls, violations),
            Recommendations = GenerateRecommendations(framework, violations)
        };

        _reports.Add(report);
        return Task.FromResult(report);
    }

    /// <summary>
    /// Get reports generated in a period
    /// </summary>
    public Task<List<ComplianceReport>> GetReportsAsync(DateTime? start = null, DateTime? end = null)
    {
        var reports = _reports.AsEnumerable();

        if (start.HasValue)
            reports = reports.Where(r => r.GeneratedAt >= start.Value);

        if (end.HasValue)
            reports = reports.Where(r => r.GeneratedAt <= end.Value);

        return Task.FromResult(reports.OrderByDescending(r => r.GeneratedAt).ToList());
    }

    /// <summary>
    /// Get a specific compliance report by ID
    /// </summary>
    public Task<ComplianceReport?> GetReportAsync(string reportId)
    {
        return Task.FromResult(_reports.FirstOrDefault(r => r.ReportId == reportId));
    }

    /// <summary>
    /// Delete a compliance report
    /// </summary>
    public Task<bool> DeleteReportAsync(string reportId)
    {
        var report = _reports.FirstOrDefault(r => r.ReportId == reportId);
        if (report is null)
            return Task.FromResult(false);

        _reports.Remove(report);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Export report as JSON
    /// </summary>
    public Task<string> ExportReportAsync(string reportId)
    {
        var report = _reports.FirstOrDefault(r => r.ReportId == reportId);
        if (report is null)
            return Task.FromResult(string.Empty);

        var json = System.Text.Json.JsonSerializer.Serialize(report, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        return Task.FromResult(json);
    }

    private ComplianceStatus CalculateOverallStatus(Dictionary<string, ControlStatus> controls, List<ComplianceViolation> violations)
    {
        if (violations.Any(v => v.Severity == ViolationSeverity.Critical))
            return ComplianceStatus.NonCompliant;

        var nonCompliantControls = controls.Count(c => c.Value.Status == ComplianceStatus.NonCompliant);
        var totalControls = controls.Count;

        if (totalControls == 0)
            return ComplianceStatus.NotApplicable;

        var nonCompliantRatio = nonCompliantControls / (double)totalControls;

        if (nonCompliantRatio > 0.2)
            return ComplianceStatus.NonCompliant;
        if (nonCompliantRatio > 0.05)
            return ComplianceStatus.PartiallyCompliant;

        return ComplianceStatus.Compliant;
    }

    private double CalculateComplianceScore(Dictionary<string, ControlStatus> controls, List<ComplianceViolation> violations)
    {
        if (controls.Count == 0)
            return 100;

        double score = controls.Sum(c => c.Value.Status switch
        {
            ComplianceStatus.Compliant => 1.0,
            ComplianceStatus.PartiallyCompliant => 0.6,
            ComplianceStatus.NonCompliant => 0.2,
            _ => 0.8
        });

        double baseScore = (score / controls.Count) * 100;

        foreach (var violation in violations)
        {
            baseScore -= violation.Severity switch
            {
                ViolationSeverity.Minor => 1,
                ViolationSeverity.Moderate => 5,
                ViolationSeverity.Major => 10,
                ViolationSeverity.Critical => 20,
                _ => 0
            };
        }

        return Math.Max(0, Math.Min(100, baseScore));
    }

    private List<string> GenerateRecommendations(ComplianceFramework framework, List<ComplianceViolation> violations)
    {
        if (violations.Count == 0)
            return new List<string> { "All controls are compliant. Continue monitoring and auditing." };

        var recommendations = new List<string>();

        foreach (var violation in violations)
        {
            var severity = violation.Severity switch
            {
                ViolationSeverity.Critical => "Immediate action required",
                ViolationSeverity.Major => "High priority remediation",
                ViolationSeverity.Moderate => "Schedule remediation",
                _ => "Monitor and review"
            };

            recommendations.Add($"{severity} for control {violation.ControlId}: {violation.Description}");
        }

        recommendations.Add(framework switch
        {
            ComplianceFramework.GDPR => "Ensure data subject rights processes are documented and tested",
            ComplianceFramework.HIPAA => "Review PHI access logs and audit trails",
            ComplianceFramework.SOC2 => "Validate security controls align with auditor requirements",
            ComplianceFramework.PCIDSS => "Verify encryption of cardholder data at rest and in transit",
            ComplianceFramework.ISO27001 => "Update risk treatment plan and re-run ISO control audits",
            _ => "Review compliance framework documentation for additional guidance"
        });

        return recommendations;
    }
}
