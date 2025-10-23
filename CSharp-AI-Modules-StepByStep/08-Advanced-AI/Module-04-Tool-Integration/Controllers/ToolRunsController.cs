using Microsoft.AspNetCore.Mvc;
using Module_04_Tool_Integration.Models;
using Module_04_Tool_Integration.Services;

namespace Module_04_Tool_Integration.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ToolRunsController : ControllerBase
{
    private readonly ToolExecutionService _executionService;

    public ToolRunsController(ToolExecutionService executionService)
    {
        _executionService = executionService;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<ToolRunLog>> GetRuns()
    {
        return Ok(_executionService.GetRuns());
    }

    [HttpGet("{runId}")]
    public ActionResult<ToolRunLog> GetRun(string runId)
    {
        var run = _executionService.GetRun(runId);
        if (run is null)
        {
            return NotFound();
        }

        return Ok(run);
    }

    [HttpGet("extracts")]
    public ActionResult<IReadOnlyCollection<DataExtractRecord>> GetExtracts()
    {
        return Ok(_executionService.GetExtracts());
    }

    [HttpGet("artifacts")]
    public ActionResult<IReadOnlyCollection<FileArtifactRecord>> GetArtifacts()
    {
        return Ok(_executionService.GetArtifacts());
    }
}
