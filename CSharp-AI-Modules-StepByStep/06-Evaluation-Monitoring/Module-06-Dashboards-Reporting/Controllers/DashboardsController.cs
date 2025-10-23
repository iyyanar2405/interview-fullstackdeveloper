using Microsoft.AspNetCore.Mvc;
using Module_06_Dashboards_Reporting.Models;
using Module_06_Dashboards_Reporting.Services;

namespace Module_06_Dashboards_Reporting.Controllers;

[ApiController]
[Route("api/dashboards")]
public sealed class DashboardsController : ControllerBase
{
	private readonly DashboardDefinitionService _definitions;
	private readonly DashboardSnapshotService _snapshots;
	private readonly DashboardComposerService _composer;

	public DashboardsController(
		DashboardDefinitionService definitions,
		DashboardSnapshotService snapshots,
		DashboardComposerService composer)
	{
		_definitions = definitions;
		_snapshots = snapshots;
		_composer = composer;
	}

	[HttpGet]
	[ProducesResponseType(typeof(IReadOnlyCollection<DashboardDefinition>), StatusCodes.Status200OK)]
	public IActionResult GetDashboards()
	{
		return Ok(_definitions.GetAll());
	}

	[HttpPost]
	[ProducesResponseType(typeof(DashboardDefinition), StatusCodes.Status201Created)]
	public IActionResult Create([FromBody] DashboardDefinition definition)
	{
		var created = _definitions.Add(definition);
		return CreatedAtAction(nameof(GetDashboard), new { dashboardId = created.DashboardId }, created);
	}

	[HttpGet("{dashboardId}")]
	[ProducesResponseType(typeof(DashboardDefinition), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetDashboard([FromRoute] string dashboardId)
	{
		var dashboard = _definitions.Get(dashboardId);
		return dashboard is null ? NotFound() : Ok(dashboard);
	}

	[HttpPut("{dashboardId}")]
	[ProducesResponseType(typeof(DashboardDefinition), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Update([FromRoute] string dashboardId, [FromBody] DashboardDefinition definition)
	{
		var updated = _definitions.Update(dashboardId, definition);
		return updated is null ? NotFound() : Ok(updated);
	}

	[HttpDelete("{dashboardId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public IActionResult Delete([FromRoute] string dashboardId)
	{
		_definitions.Delete(dashboardId);
		return NoContent();
	}

	[HttpGet("{dashboardId}/snapshot")]
	[ProducesResponseType(typeof(DashboardSnapshot), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetSnapshot([FromRoute] string dashboardId)
	{
		var snapshot = _snapshots.GetLatest(dashboardId) ?? _composer.Compose(dashboardId);
		return snapshot is null ? NotFound() : Ok(snapshot);
	}

	[HttpGet("{dashboardId}/history")]
	[ProducesResponseType(typeof(IReadOnlyCollection<DashboardSnapshot>), StatusCodes.Status200OK)]
	public IActionResult GetHistory([FromRoute] string dashboardId, [FromQuery] int count = 10)
	{
		count = Math.Clamp(count, 1, 100);
		return Ok(_snapshots.GetHistory(dashboardId, count));
	}

	[HttpGet("{dashboardId}/kpis")]
	[ProducesResponseType(typeof(IReadOnlyCollection<KpiSnapshot>), StatusCodes.Status200OK)]
	public IActionResult GetKpis([FromRoute] string dashboardId)
	{
		var kpis = _snapshots.GetLatestKpis(dashboardId);
		return Ok(kpis);
	}
}
