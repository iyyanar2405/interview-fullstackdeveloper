using Microsoft.AspNetCore.Mvc;
using Module_01_Agentic_AI.Models;
using Module_01_Agentic_AI.Services;

namespace Module_01_Agentic_AI.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly AgentCatalogService _agents;
    private readonly AgentTaskRegistryService _registry;
    private readonly AgentTaskQueueService _queue;

    public TasksController(
        AgentCatalogService agents,
        AgentTaskRegistryService registry,
        AgentTaskQueueService queue)
    {
        _agents = agents;
        _registry = registry;
        _queue = queue;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AgentTask), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreationRequest request, CancellationToken cancellationToken)
    {
        if (_agents.GetAgent(request.AgentId) is null)
        {
            return NotFound($"Agent {request.AgentId} not found");
        }

        var task = new AgentTask
        {
            AgentId = request.AgentId,
            Goal = request.Goal,
            Context = request.Context,
            Status = AgentTaskStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _registry.AddTask(task);
        await _queue.QueueAsync(task, cancellationToken);

        return Accepted(task);
    }

    [HttpGet("{taskId}")]
    [ProducesResponseType(typeof(TaskStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTaskStatus(string taskId)
    {
        var task = _registry.GetTask(taskId);
        if (task is null)
        {
            return NotFound();
        }

        var response = new TaskStatusResponse
        {
            Status = task.Status,
            OutcomeSummary = task.OutcomeSummary,
            Execution = task.Execution,
            Reflection = task.Reflection
        };

        return Ok(response);
    }

    [HttpGet("agent/{agentId}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AgentTask>), StatusCodes.Status200OK)]
    public IActionResult GetAgentTasks(string agentId)
    {
        var tasks = _registry.GetTasksForAgent(agentId);
        return Ok(tasks);
    }
}
