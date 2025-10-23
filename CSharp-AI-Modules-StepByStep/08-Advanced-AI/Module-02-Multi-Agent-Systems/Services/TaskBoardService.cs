using System.Collections.Concurrent;
using System.Linq;
using Module_02_Multi_Agent_Systems.Models;

namespace Module_02_Multi_Agent_Systems.Services;

public sealed class TaskBoardService
{
    private readonly ConcurrentDictionary<string, MultiAgentTask> _tasks = new(StringComparer.OrdinalIgnoreCase);

    public MultiAgentTask Add(MultiAgentTask task)
    {
        var clone = Clone(task);
        _tasks[clone.TaskId] = clone;
        return Clone(clone);
    }

    public MultiAgentTask? Get(string taskId)
    {
        return _tasks.TryGetValue(taskId, out var task) ? Clone(task) : null;
    }

    public IReadOnlyCollection<MultiAgentTask> GetByTeam(string teamName)
    {
        return _tasks.Values
            .Where(task => task.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(task => task.CreatedAt)
            .Select(Clone)
            .ToArray();
    }

    public void Update(MultiAgentTask task)
    {
        _tasks[task.TaskId] = Clone(task);
    }

    private static MultiAgentTask Clone(MultiAgentTask task)
    {
        return new MultiAgentTask
        {
            TaskId = task.TaskId,
            TeamName = task.TeamName,
            PlaybookId = task.PlaybookId,
            Description = task.Description,
            State = task.State,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            Timeline = task.Timeline.Select(log => new CoordinationLog
            {
                Timestamp = log.Timestamp,
                ChannelId = log.ChannelId,
                RoleId = log.RoleId,
                Message = log.Message
            }).ToList(),
            Votes = task.Votes.Select(vote => new ConsensusVote
            {
                RoleId = vote.RoleId,
                Approved = vote.Approved,
                Rationale = vote.Rationale,
                Timestamp = vote.Timestamp
            }).ToList(),
            Outputs = new Dictionary<string, string>(task.Outputs, StringComparer.OrdinalIgnoreCase)
        };
    }
}
