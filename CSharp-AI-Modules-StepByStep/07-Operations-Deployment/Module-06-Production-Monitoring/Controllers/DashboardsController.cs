using Microsoft.AspNetCore.Mvc;
using Module_06_Production_Monitoring.Models;
using Module_06_Production_Monitoring.Services;

namespace Module_06_Production_Monitoring.Controllers;

[ApiController]
[Route("api/monitoring/dashboards")]
public sealed class DashboardsController : ControllerBase
{
    private readonly DashboardCatalogService _catalog;

    public DashboardsController(DashboardCatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<DashboardDefinition>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(_catalog.GetAll());
    }

    [HttpGet("{name}")]
    [ProducesResponseType(typeof(DashboardDefinition), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(string name)
    {
        var dashboard = _catalog.Get(name);
        return dashboard is null ? NotFound() : Ok(dashboard);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DashboardDefinition), StatusCodes.Status200OK)]
    public IActionResult Upsert([FromBody] DashboardDefinition definition)
    {
        definition ??= new DashboardDefinition();
        var saved = _catalog.Upsert(definition);
        return Ok(saved);
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(string name)
    {
        _catalog.Delete(name);
        return NoContent();
    }
}
