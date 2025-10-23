using Microsoft.AspNetCore.Mvc;
using Module_02_Multi_Agent_Systems.Models;
using Module_02_Multi_Agent_Systems.Services;

namespace Module_02_Multi_Agent_Systems.Controllers;

[ApiController]
[Route("api/multi-agent/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly TeamRegistryService _teams;
    private readonly PlaybookLibraryService _playbooks;
    private readonly TaskBoardService _board;
    private readonly TaskDispatchQueueService _queue;

    public TasksController(
        TeamRegistryService teams,
        PlaybookLibraryService playbooks,
        TaskBoardService board,
        TaskDispatchQueueService queue)
    {
        _teams = teams;
        _playbooks = playbooks;
        _board = board;
        _queue = queue;
    }

    [HttpPost]
    [ProducesResponseType(typeof(MultiAgentTask), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitTask([FromBody] TaskSubmissionRequest request, CancellationToken cancellationToken)
    {
        if (_teams.GetTeam(request.TeamName) is null)
        {
            return NotFound($"Team {request.TeamName} not found");
        }

        if (_playbooks.GetPlaybook(request.PlaybookId) is null)
        {
            return NotFound($"Playbook {request.PlaybookId} not found");
        }

        var task = new MultiAgentTask
        {
            TeamName = request.TeamName,
            PlaybookId = request.PlaybookId,
            Description = request.Description,
            State = AgentTaskState.Queued,
            CreatedAt = DateTimeOffset.UtcNow,
            Timeline =
            {
                new CoordinationLog
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    RoleId = "system",
                    ChannelId = "submission",
                    Message = "Task submitted and queued"
                }
            }
        };

        var stored = _board.Add(task);
        await _queue.QueueAsync(stored, cancellationToken);

        return Accepted(stored);
    }

    [HttpGet("{taskId}")]
    [ProducesResponseType(typeof(MultiAgentTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTask(string taskId)
    {
        var task = _board.Get(taskId);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost("{taskId}/consensus/{roleId}")]
    [ProducesResponseType(typeof(MultiAgentTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SubmitConsensus(string taskId, string roleId, [FromBody] ConsensusSubmission submission)
    {
        var task = _board.Get(taskId);
        if (task is null)
        {
            return NotFound();
        }

        var vote = new ConsensusVote
        {
            RoleId = roleId,
            Approved = submission.Approved,
            Rationale = submission.Rationale,
            Timestamp = DateTimeOffset.UtcNow
        };

        task.Votes.Add(vote);
        if (!submission.Approved)
        {
            task.State = AgentTaskState.Escalated;
        }
        else if (task.State == AgentTaskState.AwaitingConsensus)
        {
            task.State = AgentTaskState.InProgress;
        }

        _board.Update(task);
        return Ok(task);
    }
}
