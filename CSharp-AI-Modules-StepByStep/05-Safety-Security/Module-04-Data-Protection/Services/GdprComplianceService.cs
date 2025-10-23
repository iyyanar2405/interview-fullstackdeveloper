using Module_04_Data_Protection.Models;

namespace Module_04_Data_Protection.Services;

public interface IGdprComplianceService
{
    Task<GdprRequest> SubmitRequestAsync(string subjectId, GdprRequestType type, string reason);
    Task<GdprDataExport> ExportDataAsync(string subjectId);
    Task<bool> EraseDataAsync(string subjectId);
    Task<bool> UpdateConsentAsync(string subjectId, string purpose, bool granted);
    Task<List<ConsentRecord>> GetConsentsAsync(string subjectId);
    Task<ComplianceReport> GenerateComplianceReportAsync(DateTime startDate, DateTime endDate);
}

public class GdprComplianceService : IGdprComplianceService
{
    private readonly ILogger<GdprComplianceService> _logger;
    private readonly Dictionary<string, DataSubject> _subjects;
    private readonly List<GdprRequest> _requests;
    private readonly IAuditLoggingService _auditService;

    public GdprComplianceService(
        ILogger<GdprComplianceService> logger,
        IAuditLoggingService auditService)
    {
        _logger = logger;
        _auditService = auditService;
        _subjects = new Dictionary<string, DataSubject>();
        _requests = new List<GdprRequest>();
    }

    public async Task<GdprRequest> SubmitRequestAsync(string subjectId, GdprRequestType type, string reason)
    {
        _logger.LogInformation("GDPR request submitted. Subject: {SubjectId}, Type: {Type}", subjectId, type);

        var request = new GdprRequest
        {
            RequestId = Guid.NewGuid().ToString(),
            SubjectId = subjectId,
            RequestType = type,
            Reason = reason,
            Status = GdprRequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        _requests.Add(request);

        await _auditService.LogAsync("GdprRequest", subjectId, request.RequestId, 
            $"{type} request submitted", AuditSeverity.Warning);

        // Auto-process some requests
        switch (type)
        {
            case GdprRequestType.DataAccess:
                request.Status = GdprRequestStatus.InProgress;
                await ExportDataAsync(subjectId);
                request.Status = GdprRequestStatus.Completed;
                request.CompletedAt = DateTime.UtcNow;
                break;

            case GdprRequestType.ConsentWithdrawal:
                request.Status = GdprRequestStatus.InProgress;
                // Withdraw all consents
                if (_subjects.ContainsKey(subjectId))
                {
                    foreach (var consent in _subjects[subjectId].Consents)
                    {
                        consent.Granted = false;
                        consent.RevokedAt = DateTime.UtcNow;
                    }
                }
                request.Status = GdprRequestStatus.Completed;
                request.CompletedAt = DateTime.UtcNow;
                break;
        }

        return request;
    }

    public async Task<GdprDataExport> ExportDataAsync(string subjectId)
    {
        _logger.LogInformation("Exporting data for subject: {SubjectId}", subjectId);

        if (!_subjects.ContainsKey(subjectId))
        {
            throw new KeyNotFoundException($"Data subject {subjectId} not found");
        }

        var subject = _subjects[subjectId];

        var export = new GdprDataExport
        {
            SubjectId = subjectId,
            PersonalData = subject.PersonalData,
            Consents = subject.Consents,
            ActivityLog = new List<string> { "Account created", "Profile updated", "Consent granted" },
            ExportedAt = DateTime.UtcNow,
            Format = "JSON"
        };

        await _auditService.LogAsync("DataExport", subjectId, subjectId, 
            "Personal data exported", AuditSeverity.Information);

        return export;
    }

    public async Task<bool> EraseDataAsync(string subjectId)
    {
        _logger.LogInformation("Erasing data for subject: {SubjectId}", subjectId);

        if (!_subjects.ContainsKey(subjectId))
        {
            return false;
        }

        // In production, securely delete from all systems
        _subjects.Remove(subjectId);

        await _auditService.LogAsync("DataErasure", subjectId, subjectId, 
            "Personal data erased (right to be forgotten)", AuditSeverity.Critical);

        return true;
    }

    public async Task<bool> UpdateConsentAsync(string subjectId, string purpose, bool granted)
    {
        _logger.LogInformation("Updating consent. Subject: {SubjectId}, Purpose: {Purpose}, Granted: {Granted}", 
            subjectId, purpose, granted);

        if (!_subjects.ContainsKey(subjectId))
        {
            _subjects[subjectId] = new DataSubject
            {
                SubjectId = subjectId,
                Email = $"{subjectId}@example.com",
                Consents = new List<ConsentRecord>()
            };
        }

        var subject = _subjects[subjectId];
        var consent = subject.Consents.FirstOrDefault(c => c.Purpose == purpose);

        if (consent == null)
        {
            consent = new ConsentRecord
            {
                ConsentId = Guid.NewGuid().ToString(),
                Purpose = purpose,
                Granted = granted,
                GrantedAt = granted ? DateTime.UtcNow : null
            };
            subject.Consents.Add(consent);
        }
        else
        {
            consent.Granted = granted;
            if (granted)
            {
                consent.GrantedAt = DateTime.UtcNow;
                consent.RevokedAt = null;
            }
            else
            {
                consent.RevokedAt = DateTime.UtcNow;
            }
        }

        await _auditService.LogAsync("ConsentUpdate", subjectId, consent.ConsentId, 
            $"Consent {(granted ? "granted" : "revoked")} for {purpose}", AuditSeverity.Information);

        return true;
    }

    public async Task<List<ConsentRecord>> GetConsentsAsync(string subjectId)
    {
        if (!_subjects.ContainsKey(subjectId))
        {
            return new List<ConsentRecord>();
        }

        return await Task.FromResult(_subjects[subjectId].Consents);
    }

    public async Task<ComplianceReport> GenerateComplianceReportAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("Generating compliance report. Period: {Start} to {End}", startDate, endDate);

        var requestsInPeriod = _requests.Where(r => 
            r.RequestedAt >= startDate && r.RequestedAt <= endDate).ToList();

        var completedRequests = requestsInPeriod.Where(r => r.Status == GdprRequestStatus.Completed).Count();
        var pendingRequests = requestsInPeriod.Where(r => r.Status == GdprRequestStatus.Pending).Count();

        var controls = new Dictionary<string, ComplianceStatus>
        {
            ["DataAccess"] = ComplianceStatus.Compliant,
            ["DataErasure"] = ComplianceStatus.Compliant,
            ["ConsentManagement"] = ComplianceStatus.Compliant,
            ["DataPortability"] = ComplianceStatus.Compliant,
            ["BreachNotification"] = pendingRequests > 0 ? ComplianceStatus.PartiallyCompliant : ComplianceStatus.Compliant
        };

        var issues = new List<ComplianceIssue>();

        if (pendingRequests > 0)
        {
            issues.Add(new ComplianceIssue
            {
                IssueId = Guid.NewGuid().ToString(),
                Control = "RequestProcessing",
                Description = $"{pendingRequests} GDPR requests pending (> 30 days)",
                Severity = IssueSeverity.Medium,
                IdentifiedAt = DateTime.UtcNow,
                Status = IssueStatus.Open
            });
        }

        var complianceScore = (controls.Count(c => c.Value == ComplianceStatus.Compliant) * 100.0) / controls.Count;

        var report = new ComplianceReport
        {
            ReportId = Guid.NewGuid().ToString(),
            GeneratedAt = DateTime.UtcNow,
            PeriodStart = startDate,
            PeriodEnd = endDate,
            Framework = ComplianceFramework.GDPR,
            Controls = controls,
            Issues = issues,
            ComplianceScore = complianceScore
        };

        await _auditService.LogAsync("ComplianceReport", null, report.ReportId, 
            $"Compliance report generated (Score: {complianceScore:F1}%)", AuditSeverity.Information);

        return report;
    }
}
