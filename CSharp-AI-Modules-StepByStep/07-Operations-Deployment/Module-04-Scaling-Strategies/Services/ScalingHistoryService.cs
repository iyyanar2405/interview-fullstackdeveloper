using System.Collections.Concurrent;
using System.Linq;
using Module_04_Scaling_Strategies.Models;

namespace Module_04_Scaling_Strategies.Services;

public sealed class ScalingHistoryService
{
    private readonly ConcurrentDictionary<Guid, ScalingRunRecord> _records = new();

    public ScalingRunRecord Add(ScalingQueueItem item)
    {
        var record = new ScalingRunRecord
        {
            RunId = item.RunId,
            ProfileName = item.Plan.ProfileName,
            Scenario = item.Request.Scenario?.Name ?? "custom",
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

    public void Complete(Guid runId, IEnumerable<ScalingSimulationTimelineEntry> timeline)
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

    public IReadOnlyCollection<ScalingRunRecord> GetRecent(int count = 20)
    {
        return _records.Values
            .OrderByDescending(r => r.RequestedAt)
            .Take(count)
            .Select(Clone)
            .ToArray();
    }

    public ScalingRunRecord? Get(Guid runId)
    {
        return _records.TryGetValue(runId, out var record) ? Clone(record) : null;
    }

    private static ScalingRunRecord Clone(ScalingRunRecord record)
    {
        var clone = new ScalingRunRecord
        {
            RunId = record.RunId,
            ProfileName = record.ProfileName,
            Scenario = record.Scenario,
            RequestedBy = record.RequestedBy,
            RequestedAt = record.RequestedAt,
            CompletedAt = record.CompletedAt,
            Status = record.Status,
            FailureReason = record.FailureReason,
            Plan = record.Plan,
            Timeline = record.Timeline.Select(entry => new ScalingSimulationTimelineEntry
            {
                Timestamp = entry.Timestamp,
                CpuPercentage = entry.CpuPercentage,
                RequestsPerSecond = entry.RequestsPerSecond,
                ActualInstances = entry.ActualInstances,
                TargetInstances = entry.TargetInstances,
                Note = entry.Note
            }).ToArray(),
        };

        foreach (var entry in record.Log)
        {
            clone.AppendLog(entry);
        }

        return clone;
    }
}
