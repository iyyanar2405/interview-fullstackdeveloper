using Microsoft.AspNetCore.Mvc;
using Module_01_Containerization.Models;
using Module_01_Containerization.Services;

namespace Module_01_Containerization.Controllers;

[ApiController]
[Route("api/builds")]
public sealed class BuildsController : ControllerBase
{
    private readonly ContainerBuildHistoryService _history;

    public BuildsController(ContainerBuildHistoryService history)
    {
        _history = history;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ContainerBuildRecord>), StatusCodes.Status200OK)]
    public IActionResult GetBuilds([FromQuery] int count = 20)
    {
        count = Math.Clamp(count, 1, 100);
        return Ok(_history.GetRecent(count));
    }

    [HttpGet("{buildId:guid}")]
    [ProducesResponseType(typeof(ContainerBuildRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetBuild([FromRoute] Guid buildId)
    {
        var record = _history.Get(buildId);
        return record is null ? NotFound() : Ok(record);
    }
}
