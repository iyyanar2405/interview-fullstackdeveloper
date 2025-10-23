using Microsoft.AspNetCore.Mvc;
using Module_05_Cloud_Deployment.Models;
using Module_05_Cloud_Deployment.Services;

namespace Module_05_Cloud_Deployment.Controllers;

[ApiController]
[Route("api/cloud-blueprints")]
public sealed class BlueprintsController : ControllerBase
{
    private readonly CloudBlueprintService _blueprints;

    public BlueprintsController(CloudBlueprintService blueprints)
    {
        _blueprints = blueprints;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CloudBlueprint>), StatusCodes.Status200OK)]
    public IActionResult GetBlueprints()
    {
        return Ok(_blueprints.GetAll());
    }

    [HttpGet("{name}")]
    [ProducesResponseType(typeof(CloudBlueprint), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetBlueprint([FromRoute] string name)
    {
        var blueprint = _blueprints.Get(name);
        return blueprint is null ? NotFound() : Ok(blueprint);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CloudBlueprint), StatusCodes.Status201Created)]
    public IActionResult Upsert([FromBody] CloudBlueprint blueprint)
    {
        var saved = _blueprints.Upsert(blueprint);
        return CreatedAtAction(nameof(GetBlueprint), new { name = saved.Name }, saved);
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete([FromRoute] string name)
    {
        _blueprints.Delete(name);
        return NoContent();
    }
}
