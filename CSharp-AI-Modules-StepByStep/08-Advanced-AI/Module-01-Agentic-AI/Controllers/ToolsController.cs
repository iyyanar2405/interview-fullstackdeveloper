using Microsoft.AspNetCore.Mvc;
using Module_01_Agentic_AI.Models;
using Module_01_Agentic_AI.Services;

namespace Module_01_Agentic_AI.Controllers;

[ApiController]
[Route("api/tools")]
public sealed class ToolsController : ControllerBase
{
    private readonly ToolCatalogService _catalog;

    public ToolsController(ToolCatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ToolDefinition>), StatusCodes.Status200OK)]
    public IActionResult GetTools()
    {
        return Ok(_catalog.GetTools());
    }

    [HttpGet("{toolName}")]
    [ProducesResponseType(typeof(ToolDefinition), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTool(string toolName)
    {
        var tool = _catalog.GetTool(toolName);
        return tool is null ? NotFound() : Ok(tool);
    }
}
