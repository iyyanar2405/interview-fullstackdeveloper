using System.Collections.Concurrent;
using Module_01_Containerization.Models;

namespace Module_01_Containerization.Services;

public sealed class ContainerBuildHistoryService
{
    private readonly ConcurrentDictionary<Guid, ContainerBuildRecord> _records = new();

    public ContainerBuildRecord AddRecord(ContainerBuildRequest request, string requestedBy, string registry, string templateName)
    {
        var record = new ContainerBuildRecord
        {
            BuildId = Guid.NewGuid(),
            ProjectName = request.ProjectName,
            TemplateName = templateName,
            Request = request,
            RequestedBy = requestedBy,
            Registry = registry,
            RequestedAt = DateTimeOffset.UtcNow,
            Status = "Queued"
        };

        _records[record.BuildId] = record;
        return record;
    }

    public void CompleteRecord(Guid buildId, ContainerBuildPlan plan)
    {
        if (!_records.TryGetValue(buildId, out var record))
        {
            return;
        }

        record.Plan = plan;
        record.CompletedAt = DateTimeOffset.UtcNow;
        record.Status = "Completed";
    }

    public IReadOnlyCollection<ContainerBuildRecord> GetRecent(int count = 20)
    {
        return _records.Values
            .OrderByDescending(r => r.RequestedAt)
            .Take(count)
            .Select(Clone)
            .ToArray();
    }

    public ContainerBuildRecord? Get(Guid buildId)
    {
        return _records.TryGetValue(buildId, out var record) ? Clone(record) : null;
    }

    private static ContainerBuildRecord Clone(ContainerBuildRecord record)
    {
        return new ContainerBuildRecord
        {
            BuildId = record.BuildId,
            ProjectName = record.ProjectName,
            RequestedAt = record.RequestedAt,
            CompletedAt = record.CompletedAt,
            RequestedBy = record.RequestedBy,
            Request = record.Request,
            Plan = record.Plan,
            Status = record.Status,
            Registry = record.Registry,
            TemplateName = record.TemplateName,
            FailureReason = record.FailureReason
        };
    }

    public void FailRecord(Guid buildId, string reason)
    {
        if (!_records.TryGetValue(buildId, out var record))
        {
            return;
        }

        record.Status = "Failed";
        record.CompletedAt = DateTimeOffset.UtcNow;
        record.FailureReason = reason;
    }
}
