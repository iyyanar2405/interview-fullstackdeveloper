using System.Collections.Concurrent;
using System.Linq;
using Module_01_Agentic_AI.Models;

namespace Module_01_Agentic_AI.Services;

public sealed class AgentSessionService
{
    private readonly ConcurrentDictionary<string, AgentSession> _sessions = new(StringComparer.OrdinalIgnoreCase);

    public AgentSession CreateSession(string agentId, string purpose)
    {
        var session = new AgentSession
        {
            AgentId = agentId,
            StartedAt = DateTimeOffset.UtcNow
        };

        _sessions[session.SessionId] = Clone(session);
        return Clone(session);
    }

    public AgentSession? GetSession(string sessionId)
    {
        return _sessions.TryGetValue(sessionId, out var session) ? Clone(session) : null;
    }

    public IReadOnlyCollection<SessionSummary> GetActiveSessions()
    {
        return _sessions.Values
            .Where(session => session.EndedAt is null)
            .Select(session => new SessionSummary
            {
                SessionId = session.SessionId,
                AgentId = session.AgentId,
                StartedAt = session.StartedAt,
                EndedAt = session.EndedAt,
                TaskCount = session.Tasks.Count,
                MemoryCount = session.Memories.Count
            })
            .OrderByDescending(summary => summary.StartedAt)
            .ToArray();
    }

    public void AppendTask(string sessionId, AgentTask task)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return;
        }

        session.Tasks.Add(Clone(task));
    }

    public void AppendMemory(string sessionId, AgentMemoryEntry entry)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return;
        }

        session.Memories.Add(Clone(entry));
    }

    public void EndSession(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.EndedAt = DateTimeOffset.UtcNow;
        }
    }

    private static AgentSession Clone(AgentSession session)
    {
        return new AgentSession
        {
            SessionId = session.SessionId,
            AgentId = session.AgentId,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt,
            Tasks = session.Tasks.Select(Clone).ToList(),
            Memories = session.Memories.Select(Clone).ToList()
        };
    }

    private static AgentTask Clone(AgentTask task)
    {
        return new AgentTask
        {
            TaskId = task.TaskId,
            AgentId = task.AgentId,
            Goal = task.Goal,
            Context = task.Context,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            Plan = task.Plan.Select(step => new TaskPlanStep
            {
                Order = step.Order,
                Thought = step.Thought,
                Tool = step.Tool,
                Input = step.Input,
                ExpectedOutput = step.ExpectedOutput
            }).ToList(),
            Execution = task.Execution.Select(record => new TaskStepRecord
            {
                Order = record.Order,
                Tool = record.Tool,
                Input = record.Input,
                Output = record.Output,
                Success = record.Success,
                Timestamp = record.Timestamp,
                Artifacts = record.Artifacts.Select(artifact => new ExecutionArtifact
                {
                    Type = artifact.Type,
                    Description = artifact.Description,
                    Data = artifact.Data
                }).ToList()
            }).ToList(),
            CapturedMemories = task.CapturedMemories.Select(Clone).ToList(),
            Reflection = task.Reflection is null ? null : new AgentReflection
            {
                Summary = task.Reflection.Summary,
                FollowUpQuestions = task.Reflection.FollowUpQuestions.ToList(),
                ImprovementIdeas = task.Reflection.ImprovementIdeas.ToList()
            },
            OutcomeSummary = task.OutcomeSummary
        };
    }

    private static AgentMemoryEntry Clone(AgentMemoryEntry entry)
    {
        return new AgentMemoryEntry
        {
            AgentId = entry.AgentId,
            Channel = entry.Channel,
            Content = entry.Content,
            Salience = entry.Salience,
            Timestamp = entry.Timestamp
        };
    }
}
