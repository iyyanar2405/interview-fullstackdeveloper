using System.Collections.Concurrent;
using Module_02_CICD_Pipelines.Models;

namespace Module_02_CICD_Pipelines.Services;

public sealed class PipelineRunHistoryService
{
    private readonly ConcurrentDictionary<Guid, PipelineRunRecord> _runs = new();

    public PipelineRunRecord Add(PipelineRequest request, string templateName, string triggeredBy)
    {
        var run = new PipelineRunRecord
        {
            RunId = Guid.NewGuid(),
            ProjectName = request.ProjectName,
            Provider = request.Provider,
            TemplateName = templateName,
            TriggeredBy = triggeredBy,
            QueuedAt = DateTimeOffset.UtcNow,
            Status = "Queued"
        };

        _runs[run.RunId] = run;
        return Clone(run);
    }

    public void Complete(Guid runId, PipelinePlan plan, List<PipelineStageLog> logs)
    {
        if (!_runs.TryGetValue(runId, out var run))
        {
            return;
        }

        run.Plan = plan;
        run.StageLogs = logs;
        run.CompletedAt = DateTimeOffset.UtcNow;
        run.Status = "Succeeded";
    }

    public void Fail(Guid runId, string reason, List<PipelineStageLog> logs)
    {
        if (!_runs.TryGetValue(runId, out var run))
        {
            return;
        }

        run.StageLogs = logs;
        run.CompletedAt = DateTimeOffset.UtcNow;
        run.Status = "Failed";
        run.FailureReason = reason;
    }

    public IReadOnlyCollection<PipelineRunRecord> GetRecent(int count = 20)
    {
        return _runs.Values
            .OrderByDescending(r => r.QueuedAt)
            .Take(count)
            .Select(Clone)
            .ToArray();
    }

    public PipelineRunRecord? Get(Guid runId)
    {
        return _runs.TryGetValue(runId, out var run) ? Clone(run) : null;
    }

    private static PipelineRunRecord Clone(PipelineRunRecord run)
    {
        return new PipelineRunRecord
        {
            RunId = run.RunId,
            ProjectName = run.ProjectName,
            Provider = run.Provider,
            TemplateName = run.TemplateName,
            Status = run.Status,
            QueuedAt = run.QueuedAt,
            CompletedAt = run.CompletedAt,
            Plan = run.Plan,
            StageLogs = run.StageLogs.Select(log => new PipelineStageLog
            {
                Stage = log.Stage,
                Logs = new List<string>(log.Logs),
                Status = log.Status,
                Duration = log.Duration
            }).ToList(),
            TriggeredBy = run.TriggeredBy,
            FailureReason = run.FailureReason
        };
    }
}
