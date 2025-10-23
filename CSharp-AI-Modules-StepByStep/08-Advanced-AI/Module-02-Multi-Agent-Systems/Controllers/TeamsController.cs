using Microsoft.AspNetCore.Mvc;
using Module_02_Multi_Agent_Systems.Models;
using Module_02_Multi_Agent_Systems.Services;

namespace Module_02_Multi_Agent_Systems.Controllers;

[ApiController]
[Route("api/multi-agent/teams")]
public sealed class TeamsController : ControllerBase
{
    private readonly TeamRegistryService _teams;
    private readonly TaskBoardService _board;

    public TeamsController(TeamRegistryService teams, TaskBoardService board)
    {
        _teams = teams;
        _board = board;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AgentTeamDefinition>), StatusCodes.Status200OK)]
    public IActionResult GetTeams()
    {
        return Ok(_teams.GetTeams());
    }

    [HttpGet("{teamName}")]
    [ProducesResponseType(typeof(AgentTeamDefinition), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTeam(string teamName)
    {
        var team = _teams.GetTeam(teamName);
        return team is null ? NotFound() : Ok(team);
    }

    [HttpGet("{teamName}/tasks")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MultiAgentTask>), StatusCodes.Status200OK)]
    public IActionResult GetTeamTasks(string teamName)
    {
        return Ok(_board.GetByTeam(teamName));
    }
}
