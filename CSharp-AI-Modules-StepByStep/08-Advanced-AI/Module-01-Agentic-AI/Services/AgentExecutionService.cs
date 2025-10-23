using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Module_01_Agentic_AI.Models;
using System.Linq;

namespace Module_01_Agentic_AI.Services;

public sealed class AgentExecutionService
{
    private readonly PlanSynthesisService _planning;
    private readonly ToolCatalogService _tools;
    private readonly MemoryStoreService _memory;
    private readonly AgentTaskRegistryService _registry;
    private readonly ILogger<AgentExecutionService> _logger;
    private readonly SimulationSettings _settings;
    private readonly Random _random = new();

    public AgentExecutionService(
        PlanSynthesisService planning,
        ToolCatalogService tools,
        MemoryStoreService memory,
        AgentTaskRegistryService registry,
        IOptions<AgenticOptions> options,
        ILogger<AgentExecutionService> logger)
    {
        _planning = planning;
        _tools = tools;
        _memory = memory;
        _registry = registry;
        _logger = logger;
        _settings = options.Value.Simulation;
    }

    public async Task ExecuteAsync(AgentTask task, CancellationToken cancellationToken)
    {
        try
        {
            task.Status = AgentTaskStatus.Planning;
            _registry.UpdateTask(task);

            var plan = await _planning.CreatePlanAsync(task, cancellationToken);
            task.Plan = plan;

            task.Status = AgentTaskStatus.Executing;
            _registry.UpdateTask(task);

            var order = 1;
            foreach (var step in plan)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var invocation = await _tools.ExecuteAsync(step.Tool, step.Input, cancellationToken);
                var record = new TaskStepRecord
                {
                    Order = order,
                    Tool = step.Tool,
                    Input = step.Input,
                    Output = invocation.Output,
                    Success = invocation.Success,
                    Timestamp = DateTimeOffset.UtcNow,
                    Artifacts = invocation.Artifacts
                };
                task.Execution.Add(record);
                _memory.AddMemory(new AgentMemoryEntry
                {
                    AgentId = task.AgentId,
                    Channel = "execution",
                    Content = invocation.Output,
                    Salience = invocation.Success ? 0.8 : 0.4,
                    Timestamp = record.Timestamp
                });

                if (!invocation.Success)
                {
                    task.Status = AgentTaskStatus.Failed;
                    task.OutcomeSummary = invocation.Output;
                    task.CompletedAt = DateTimeOffset.UtcNow;
                    _registry.UpdateTask(task);
                    return;
                }

                order++;
            }

            if (_random.NextDouble() <= _settings.ReflectionProbability)
            {
                task.Status = AgentTaskStatus.Reflecting;
                _registry.UpdateTask(task);

                task.Reflection = new AgentReflection
                {
                    Summary = $"Reflecting on goal '{task.Goal}' with {task.Execution.Count} steps",
                    FollowUpQuestions =
                    {
                        "What context signals improved success?",
                        "Are additional tools required?"
                    },
                    ImprovementIdeas =
                    {
                        "Expand toolkit with evaluation function",
                        "Automate verification step"
                    }
                };
            }

            task.Status = AgentTaskStatus.Completed;
            task.CompletedAt = DateTimeOffset.UtcNow;
            task.OutcomeSummary = task.Execution.Count == 0
                ? "No actions executed"
                : string.Join("; ", task.Execution.Select(step => step.Output));

            _registry.UpdateTask(task);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Agent task {TaskId} cancelled", task.TaskId);
            task.Status = AgentTaskStatus.Failed;
            task.OutcomeSummary = "Cancelled";
            _registry.UpdateTask(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent task {TaskId} execution failed", task.TaskId);
            task.Status = AgentTaskStatus.Failed;
            task.OutcomeSummary = ex.Message;
            task.CompletedAt = DateTimeOffset.UtcNow;
            _registry.UpdateTask(task);
        }
    }
}
