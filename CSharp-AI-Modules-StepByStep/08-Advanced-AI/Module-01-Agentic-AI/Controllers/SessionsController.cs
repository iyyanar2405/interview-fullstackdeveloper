using Microsoft.AspNetCore.Mvc;
using Module_01_Agentic_AI.Models;
using Module_01_Agentic_AI.Services;

namespace Module_01_Agentic_AI.Controllers;

[ApiController]
[Route("api/sessions")]
public sealed class SessionsController : ControllerBase
{
    private readonly AgentCatalogService _agents;
    private readonly AgentSessionService _sessions;

    public SessionsController(AgentCatalogService agents, AgentSessionService sessions)
    {
        _agents = agents;
        _sessions = sessions;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AgentSession), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create([FromBody] SessionCreationRequest request)
    {
        if (_agents.GetAgent(request.AgentId) is null)
        {
            return NotFound($"Agent {request.AgentId} not found");
        }

        var session = _sessions.CreateSession(request.AgentId, request.Purpose);
        return CreatedAtAction(nameof(GetById), new { sessionId = session.SessionId }, session);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SessionSummary>), StatusCodes.Status200OK)]
    public IActionResult GetActive()
    {
        return Ok(_sessions.GetActiveSessions());
    }

    [HttpGet("{sessionId}")]
    [ProducesResponseType(typeof(AgentSession), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(string sessionId)
    {
        var session = _sessions.GetSession(sessionId);
        return session is null ? NotFound() : Ok(session);
    }

    [HttpPost("{sessionId}/end")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult EndSession(string sessionId)
    {
        _sessions.EndSession(sessionId);
        return Accepted();
    }
}
