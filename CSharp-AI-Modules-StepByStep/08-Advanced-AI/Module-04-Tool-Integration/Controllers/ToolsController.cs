using Microsoft.AspNetCore.Mvc;
using Module_04_Tool_Integration.Models;
using Module_04_Tool_Integration.Services;

namespace Module_04_Tool_Integration.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ToolsController : ControllerBase
{
    private readonly ToolExecutionService _executionService;

    public ToolsController(ToolExecutionService executionService)
    {
        _executionService = executionService;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<ToolDescriptor>> GetTools()
    {
        return Ok(_executionService.GetTools());
    }

    [HttpGet("{toolType}/{toolId}")]
    public ActionResult<ToolDescriptor> GetTool(string toolType, string toolId)
    {
        var tool = _executionService.GetTool(toolType, toolId);
        if (tool is null)
        {
            return NotFound();
        }

        return Ok(tool);
    }

    [HttpPost("execute")]
    public ActionResult<object> ExecuteTool([FromBody] ToolExecutionRequest request)
    {
        var runId = _executionService.SubmitExecution(request);
        return Accepted(new { runId });
    }
}
