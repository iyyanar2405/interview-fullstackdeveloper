using System.Collections.Concurrent;
using System.Linq;
using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class DeploymentHistoryService
{
    private readonly ConcurrentDictionary<Guid, DeploymentRunRecord> _records = new();

    public DeploymentRunRecord Add(DeploymentQueueItem item)
    {
        var record = new DeploymentRunRecord
        {
            RunId = item.RunId,
            BlueprintName = item.Plan.BlueprintName,
            Provider = item.Plan.Provider,
            Environment = item.Plan.Environment,
            RequestedBy = string.IsNullOrWhiteSpace(item.Request.RequestedBy) ? "system" : item.Request.RequestedBy,
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
        if (_records.TryGetValue(runId, out var record))
        {
            record.Status = "Running";
            record.AppendLog($"Run started at {DateTimeOffset.UtcNow:u}");
        }
    }

    public void Complete(Guid runId, IEnumerable<DeploymentTimelineEntry> timeline)
    {
        if (_records.TryGetValue(runId, out var record))
        {
            record.Status = "Completed";
            record.CompletedAt = DateTimeOffset.UtcNow;
            record.Timeline = timeline.ToList();
            record.AppendLog($"Run completed at {record.CompletedAt:u}");
        }
    }

    public void Fail(Guid runId, string reason)
    {
        if (_records.TryGetValue(runId, out var record))
        {
            record.Status = "Failed";
            record.CompletedAt = DateTimeOffset.UtcNow;
            record.FailureReason = reason;
            record.AppendLog($"Run failed: {reason}");
        }
    }

    public void Log(Guid runId, string message)
    {
        if (_records.TryGetValue(runId, out var record))
        {
            record.AppendLog(message);
        }
    }

    public IReadOnlyCollection<DeploymentRunRecord> GetRecent(int count = 20)
    {
        return _records.Values
            .OrderByDescending(r => r.RequestedAt)
            .Take(count)
            .Select(Clone)
            .ToArray();
    }

    public DeploymentRunRecord? Get(Guid runId)
    {
        return _records.TryGetValue(runId, out var record) ? Clone(record) : null;
    }

    private static DeploymentRunRecord Clone(DeploymentRunRecord record)
    {
        var clone = new DeploymentRunRecord
        {
            RunId = record.RunId,
            BlueprintName = record.BlueprintName,
            Provider = record.Provider,
            Environment = record.Environment,
            RequestedBy = record.RequestedBy,
            RequestedAt = record.RequestedAt,
            CompletedAt = record.CompletedAt,
            Status = record.Status,
            FailureReason = record.FailureReason,
            Plan = record.Plan,
            Timeline = record.Timeline.Select(entry => new DeploymentTimelineEntry
            {
                Timestamp = entry.Timestamp,
                Step = entry.Step,
                Status = entry.Status,
                Detail = entry.Detail
            }).ToArray()
        };

        foreach (var log in record.Log)
        {
            clone.AppendLog(log);
        }

        return clone;
    }
}
