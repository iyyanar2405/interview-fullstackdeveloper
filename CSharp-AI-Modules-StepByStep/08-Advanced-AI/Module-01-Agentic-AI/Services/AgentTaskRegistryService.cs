using System.Collections.Concurrent;
using System.Linq;
using Module_01_Agentic_AI.Models;

namespace Module_01_Agentic_AI.Services;

public sealed class AgentTaskRegistryService
{
    private readonly ConcurrentDictionary<string, AgentTask> _tasks = new(StringComparer.OrdinalIgnoreCase);

    public AgentTask AddTask(AgentTask task)
    {
        var clone = Clone(task);
        _tasks[clone.TaskId] = clone;
        return Clone(clone);
    }

    public AgentTask? GetTask(string taskId)
    {
        return _tasks.TryGetValue(taskId, out var task) ? Clone(task) : null;
    }

    public IReadOnlyCollection<AgentTask> GetTasksForAgent(string agentId)
    {
        return _tasks.Values.Where(task => task.AgentId.Equals(agentId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(task => task.CreatedAt)
            .Select(Clone)
            .ToArray();
    }

    public void UpdateTask(AgentTask task)
    {
        _tasks[task.TaskId] = Clone(task);
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
            CapturedMemories = task.CapturedMemories.Select(memory => new AgentMemoryEntry
            {
                AgentId = memory.AgentId,
                Channel = memory.Channel,
                Content = memory.Content,
                Salience = memory.Salience,
                Timestamp = memory.Timestamp
            }).ToList(),
            Reflection = task.Reflection is null ? null : new AgentReflection
            {
                Summary = task.Reflection.Summary,
                FollowUpQuestions = task.Reflection.FollowUpQuestions.ToList(),
                ImprovementIdeas = task.Reflection.ImprovementIdeas.ToList()
            },
            OutcomeSummary = task.OutcomeSummary
        };
    }
}
