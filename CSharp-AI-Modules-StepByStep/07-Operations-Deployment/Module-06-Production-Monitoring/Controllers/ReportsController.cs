using Microsoft.AspNetCore.Mvc;
using Module_06_Production_Monitoring.Models;
using Module_06_Production_Monitoring.Services;

namespace Module_06_Production_Monitoring.Controllers;

[ApiController]
[Route("api/monitoring/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly MonitoringReportService _reportService;

    public ReportsController(MonitoringReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("{reportType}")]
    [ProducesResponseType(typeof(MonitoringReport), StatusCodes.Status200OK)]
    public IActionResult GetReport(string reportType)
    {
        var report = _reportService.GenerateReport(reportType);
        return Ok(report);
    }
}
