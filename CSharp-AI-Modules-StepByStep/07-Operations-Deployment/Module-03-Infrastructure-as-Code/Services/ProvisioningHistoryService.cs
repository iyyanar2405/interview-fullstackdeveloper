using System.Collections.Concurrent;
using System.Linq;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class ProvisioningHistoryService
{
    private readonly ConcurrentDictionary<Guid, ProvisioningRunRecord> _records = new();

    public ProvisioningRunRecord Add(ProvisioningQueueItem item)
    {
        var record = new ProvisioningRunRecord
        {
            RunId = item.RunId,
            ProjectName = item.Request.ProjectName,
            Environment = item.Request.Environment,
            Provider = item.Plan.Provider,
            RequestedBy = string.IsNullOrWhiteSpace(item.RequestedBy) ? item.Request.Parameters.GetValueOrDefault("requestedBy", "system") : item.RequestedBy,
            RequestedAt = item.RequestedAt,
            Status = "Queued",
            Plan = item.Plan
        };

        record.AppendLog($"Run queued at {record.RequestedAt:u}");
        _records[record.RunId] = record;
        return Clone(record);
    }

    public void Start(Guid runId)
    {
        if (!_records.TryGetValue(runId, out var record))
        {
            return;
        }

        record.Status = "Running";
        record.AppendLog($"Run started at {DateTimeOffset.UtcNow:u}");
    }

    public void Complete(Guid runId, InfrastructurePlan plan, IEnumerable<DriftFinding> drift, IEnumerable<ComplianceResult> compliance)
    {
        if (!_records.TryGetValue(runId, out var record))
        {
            return;
        }

        record.Plan = plan;
        record.Status = "Completed";
        record.CompletedAt = DateTimeOffset.UtcNow;
        record.DriftAnalysis = drift.ToList();
        record.Compliance = compliance.ToList();
        record.AppendLog($"Run completed at {record.CompletedAt:u}");
    }

    public void Fail(Guid runId, string reason)
    {
        if (!_records.TryGetValue(runId, out var record))
        {
            return;
        }

        record.Status = "Failed";
        record.CompletedAt = DateTimeOffset.UtcNow;
        record.FailureReason = reason;
        record.AppendLog($"Run failed: {reason}");
    }

    public void Log(Guid runId, string message)
    {
        if (_records.TryGetValue(runId, out var record))
        {
            record.AppendLog(message);
        }
    }

    public IReadOnlyCollection<ProvisioningRunRecord> GetRecent(int count = 20)
    {
        return _records.Values
            .OrderByDescending(r => r.RequestedAt)
            .Take(count)
            .Select(Clone)
            .ToArray();
    }

    public ProvisioningRunRecord? Get(Guid runId)
    {
        return _records.TryGetValue(runId, out var record) ? Clone(record) : null;
    }

    private static ProvisioningRunRecord Clone(ProvisioningRunRecord record)
    {
        var clone = new ProvisioningRunRecord
        {
            RunId = record.RunId,
            ProjectName = record.ProjectName,
            Environment = record.Environment,
            Provider = record.Provider,
            RequestedBy = record.RequestedBy,
            RequestedAt = record.RequestedAt,
            CompletedAt = record.CompletedAt,
            Status = record.Status,
            FailureReason = record.FailureReason,
            Plan = record.Plan,
            DriftAnalysis = record.DriftAnalysis.Select(finding => new DriftFinding
            {
                ResourceName = finding.ResourceName,
                Property = finding.Property,
                DesiredValue = finding.DesiredValue,
                ActualValue = finding.ActualValue,
                Impact = finding.Impact
            }).ToList(),
            Compliance = record.Compliance.Select(result => new ComplianceResult
            {
                RuleId = result.RuleId,
                Status = result.Status,
                Severity = result.Severity,
                Message = result.Message
            }).ToList()
        };

        foreach (var log in record.ExecutionLog)
        {
            clone.AppendLog(log);
        }

        return clone;
    }
}
