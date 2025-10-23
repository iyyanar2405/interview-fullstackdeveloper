using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Module_01_Agentic_AI.Models;
using System.Linq;

namespace Module_01_Agentic_AI.Services;

public sealed class PlanSynthesisService
{
    private readonly AgentCatalogService _agentCatalog;
    private readonly ToolCatalogService _toolCatalog;
    private readonly ILogger<PlanSynthesisService> _logger;
    private readonly AgenticOptions _options;

    public PlanSynthesisService(
        AgentCatalogService agentCatalog,
        ToolCatalogService toolCatalog,
        IOptions<AgenticOptions> options,
        ILogger<PlanSynthesisService> logger)
    {
        _agentCatalog = agentCatalog;
        _toolCatalog = toolCatalog;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<List<TaskPlanStep>> CreatePlanAsync(AgentTask task, CancellationToken cancellationToken)
    {
        var agent = _agentCatalog.GetAgent(task.AgentId);
        if (agent is null)
        {
            _logger.LogWarning("Agent {AgentId} missing for task {TaskId}", task.AgentId, task.TaskId);
            return new List<TaskPlanStep>();
        }

        await Task.Delay(_options.Simulation.PlanningDelayMilliseconds, cancellationToken);

        var allowedTools = agent.AllowedTools
            .Select(toolName => _toolCatalog.GetTool(toolName))
            .Where(tool => tool is not null)
            .Cast<ToolDefinition>()
            .ToList();

        if (allowedTools.Count == 0)
        {
            _logger.LogWarning("Agent {AgentId} has no tools configured", agent.Id);
            return new List<TaskPlanStep>();
        }

        var steps = new List<TaskPlanStep>();
        var stageCounter = 1;

        foreach (var capability in agent.Capabilities)
        {
            var tool = allowedTools[(stageCounter - 1) % allowedTools.Count];
            steps.Add(new TaskPlanStep
            {
                Order = stageCounter,
                Thought = $"Use capability '{capability}' to progress goal",
                Tool = tool.Name,
                Input = capability.Contains("research", StringComparison.OrdinalIgnoreCase)
                    ? $"search: {task.Goal}"
                    : capability.Contains("analysis", StringComparison.OrdinalIgnoreCase)
                        ? $"analyze context: {task.Context}"
                        : capability.Contains("generate", StringComparison.OrdinalIgnoreCase)
                            ? $"draft proposal for {task.Goal}"
                            : $"process goal: {task.Goal}",
                ExpectedOutput = capability.Contains("analysis", StringComparison.OrdinalIgnoreCase)
                    ? "insight summary"
                    : capability.Contains("generate", StringComparison.OrdinalIgnoreCase)
                        ? "draft artifact"
                        : "action recommendation"
            });

            stageCounter++;
        }

        // Add closure step encouraging reflection.
        steps.Add(new TaskPlanStep
        {
            Order = stageCounter,
            Thought = "Synthesize outcome and capture reflection",
            Tool = allowedTools.Last().Name,
            Input = "summarize learnings and propose next steps",
            ExpectedOutput = "reflections"
        });

        return steps;
    }
}
