using System.Collections.Concurrent;
using Module_04_Tool_Integration.Models;
using Microsoft.Extensions.Options;

namespace Module_04_Tool_Integration.Services;

public sealed class ToolExecutionHistoryService
{
    private readonly ConcurrentDictionary<string, ToolRunLog> _runs = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentQueue<string> _runOrder = new();
    private readonly ConcurrentQueue<DataExtractRecord> _extracts = new();
    private readonly ConcurrentQueue<FileArtifactRecord> _artifacts = new();
    private readonly ToolIntegrationOptions _options;

    public ToolExecutionHistoryService(IOptions<ToolIntegrationOptions> options)
    {
        _options = options.Value;
    }

    public void RegisterPending(string runId, ToolExecutionRequest request)
    {
        var log = new ToolRunLog
        {
            RunId = runId,
            ToolId = request.ToolId,
            ToolType = request.ToolType,
            Request = request.Clone(),
            Status = ToolRunStatus.Pending,
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
            log.Status = ToolRunStatus.Running;
            log.StartedAt = DateTimeOffset.UtcNow;
        }
    }

    public void CompleteSuccess(string runId, ToolExecutionOutcome outcome)
    {
        if (!_runs.TryGetValue(runId, out var log))
        {
            return;
        }

        log.Status = ToolRunStatus.Completed;
        log.Result = outcome.Result;
        log.CompletedAt = outcome.Result.CompletedAt;
        log.Notes = outcome.Result.Summary;

        foreach (var extract in outcome.Extracts)
        {
            _extracts.Enqueue(extract);
        }

        foreach (var artifact in outcome.Artifacts)
        {
            _artifacts.Enqueue(artifact);
        }

        TrimRecords();
    }

    public void CompleteFailure(string runId, string reason)
    {
        if (!_runs.TryGetValue(runId, out var log))
        {
            return;
        }

        log.Status = ToolRunStatus.Failed;
        log.CompletedAt = DateTimeOffset.UtcNow;
        log.Notes = reason;
        log.Result = new ToolExecutionResult
        {
            Success = false,
            Status = "failed",
            Summary = reason,
            Outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            CompletedAt = log.CompletedAt ?? DateTimeOffset.UtcNow
        };
    }

    public IReadOnlyCollection<ToolRunLog> GetRuns()
    {
        return _runs.Values
            .OrderByDescending(r => r.CreatedAt)
            .Select(log => log.Clone())
            .ToArray();
    }

    public ToolRunLog? GetRun(string runId)
    {
        return _runs.TryGetValue(runId, out var log) ? log.Clone() : null;
    }

    public IReadOnlyCollection<DataExtractRecord> GetExtracts()
    {
        return _extracts.Select(record => record.Clone()).ToArray();
    }

    public IReadOnlyCollection<FileArtifactRecord> GetArtifacts()
    {
        return _artifacts.Select(record => record.Clone()).ToArray();
    }

    private void TrimRuns()
    {
        while (_runOrder.TryPeek(out var oldestRun) && _runs.Count > _options.Simulation.MaxLogEntries)
        {
            if (_runOrder.TryDequeue(out var runId))
            {
                _runs.TryRemove(runId, out _);
            }
        }
    }

    private void TrimRecords()
    {
        while (_extracts.Count > _options.Simulation.MaxDataRecords && _extracts.TryDequeue(out _))
        {
        }

        while (_artifacts.Count > _options.Simulation.MaxDataRecords && _artifacts.TryDequeue(out _))
        {
        }
    }
}
