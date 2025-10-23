using Microsoft.AspNetCore.Mvc;
using Module_06_Dashboards_Reporting.Models;
using Module_06_Dashboards_Reporting.Services;

namespace Module_06_Dashboards_Reporting.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
	private readonly ReportGenerationService _reports;
	private readonly ExecutiveSummaryService _summary;
	private readonly DashboardDefinitionService _definitions;

	public ReportsController(
		ReportGenerationService reports,
		ExecutiveSummaryService summary,
		DashboardDefinitionService definitions)
	{
		_reports = reports;
		_summary = summary;
		_definitions = definitions;
	}

	[HttpPost]
	[ProducesResponseType(typeof(ReportDocument), StatusCodes.Status200OK)]
	public IActionResult Generate([FromBody] ReportRequest request)
	{
		request ??= new ReportRequest();
		var document = _reports.Generate(request);
		return Ok(document);
	}

	[HttpPost("executive-summary")]
	[ProducesResponseType(typeof(ExecutiveSummary), StatusCodes.Status200OK)]
	public IActionResult ExecutiveSummary([FromBody] ReportRequest request)
	{
		request ??= new ReportRequest();
		var document = _reports.Generate(request);
		var summary = _summary.Build(document);
		return Ok(summary);
	}

	[HttpGet("metrics")]
	[ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
	public IActionResult Metrics()
	{
		var metrics = _definitions.GetAll()
			.SelectMany(d => d.Widgets)
			.Select(w => w.Metric)
			.Where(m => !string.IsNullOrWhiteSpace(m))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

		return Ok(metrics);
	}
}
