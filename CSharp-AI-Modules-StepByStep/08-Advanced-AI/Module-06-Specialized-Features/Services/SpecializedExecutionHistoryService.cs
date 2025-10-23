using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Module_06_Specialized_Features.Models;

namespace Module_06_Specialized_Features.Services;

public sealed class SpecializedExecutionHistoryService
{
    private readonly ConcurrentDictionary<string, SpecializedRunLog> _runs = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentQueue<string> _runOrder = new();
    private readonly ConcurrentQueue<SpecializedArtifact> _artifacts = new();
    private readonly SpecializedFeaturesOptions _options;

    public SpecializedExecutionHistoryService(IOptions<SpecializedFeaturesOptions> options)
    {
        _options = options.Value;
    }

    public void RegisterPending(string runId, SpecializedTaskRequest request)
    {
        var log = new SpecializedRunLog
        {
            RunId = runId,
            FeatureType = request.FeatureType,
            Request = request.Clone(),
            Status = SpecializedRunStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _runs[runId] = log;
        _runOrder.Enqueue(runId);
        TrimRuns();
    }

    public void MarkRunning(string runId)
    {
        if (_runs.TryGetValue(runId, out var log))
        {
            log.Status = SpecializedRunStatus.Running;
            log.StartedAt = DateTimeOffset.UtcNow;
        }
    }

    public void CompleteSuccess(string runId, SpecializedExecutionResult result, IEnumerable<SpecializedArtifact> artifacts, string notes)
    {
        if (!_runs.TryGetValue(runId, out var log))
        {
            return;
        }

        log.Status = SpecializedRunStatus.Completed;
        log.Result = result;
        log.CompletedAt = result.CompletedAt;
        log.Notes = notes;

        foreach (var artifact in artifacts)
        {
            _artifacts.Enqueue(artifact);
        }

        TrimArtifacts();
    }

    public void CompleteFailure(string runId, string reason)
    {
        if (!_runs.TryGetValue(runId, out var log))
        {
            return;
        }

        log.Status = SpecializedRunStatus.Failed;
        log.CompletedAt = DateTimeOffset.UtcNow;
        log.Notes = reason;
        log.Result = new SpecializedExecutionResult
        {
            Success = false,
            Status = "failed",
            Summary = reason,
            Outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            CompletedAt = log.CompletedAt ?? DateTimeOffset.UtcNow
        };
    }

    public IReadOnlyCollection<SpecializedRunLog> GetRuns()
    {
        return _runs.Values
            .OrderByDescending(run => run.CreatedAt)
            .Select(run => run.Clone())
            .ToArray();
    }

    public SpecializedRunLog? GetRun(string runId)
    {
        return _runs.TryGetValue(runId, out var log) ? log.Clone() : null;
    }

    public IReadOnlyCollection<SpecializedArtifact> GetArtifacts()
    {
        return _artifacts.Select(artifact => artifact.Clone()).ToArray();
    }

    private void TrimRuns()
    {
        var max = _options.Simulation.MaxHistoryEntries;
        if (max <= 0)
        {
            return;
        }

        while (_runOrder.TryPeek(out var runId) && _runs.Count > max)
        {
            if (_runOrder.TryDequeue(out var dequeued))
            {
                _runs.TryRemove(dequeued, out _);
            }
        }
    }

    private void TrimArtifacts()
    {
        var max = _options.Simulation.MaxArtifacts;
        if (max <= 0)
        {
            return;
        }

        while (_artifacts.Count > max && _artifacts.TryDequeue(out _))
        {
        }
    }
}
