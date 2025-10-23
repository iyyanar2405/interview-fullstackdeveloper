using Microsoft.AspNetCore.Mvc;
using Module_01_Agentic_AI.Models;
using Module_01_Agentic_AI.Services;

namespace Module_01_Agentic_AI.Controllers;

[ApiController]
[Route("api/agents")]
public sealed class AgentsController : ControllerBase
{
    private readonly AgentCatalogService _catalog;
    private readonly MemoryStoreService _memory;

    public AgentsController(AgentCatalogService catalog, MemoryStoreService memory)
    {
        _catalog = catalog;
        _memory = memory;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AgentDefinition>), StatusCodes.Status200OK)]
    public IActionResult GetAgents()
    {
        return Ok(_catalog.GetAgents());
    }

    [HttpGet("{agentId}")]
    [ProducesResponseType(typeof(AgentDefinition), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAgent(string agentId)
    {
        var agent = _catalog.GetAgent(agentId);
        return agent is null ? NotFound() : Ok(agent);
    }

    [HttpGet("{agentId}/memories")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AgentMemoryEntry>), StatusCodes.Status200OK)]
    public IActionResult GetMemories(string agentId, [FromQuery] int limit = 50)
    {
        var memories = _memory.GetMemories(agentId, limit);
        return Ok(memories);
    }
}
