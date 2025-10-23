using Microsoft.AspNetCore.Mvc;
using Module_01_Performance_Metrics.Models;
using Module_01_Performance_Metrics.Services;

namespace Module_01_Performance_Metrics.Controllers;

[ApiController]
[Route("api/performance")] 
public sealed class PerformanceMetricsController : ControllerBase
{
    private static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(60);
    private readonly RequestMetricsService _requestMetricsService;
    private readonly ResourceUtilizationService _resourceUtilizationService;
    private readonly CostAnalysisService _costAnalysisService;
    private readonly SLAMonitoringService _slaMonitoringService;
    private readonly MetricsExportService _metricsExportService;

    public PerformanceMetricsController(
        RequestMetricsService requestMetricsService,
        ResourceUtilizationService resourceUtilizationService,
        CostAnalysisService costAnalysisService,
        SLAMonitoringService slaMonitoringService,
        MetricsExportService metricsExportService)
    {
        _requestMetricsService = requestMetricsService;
        _resourceUtilizationService = resourceUtilizationService;
        _costAnalysisService = costAnalysisService;
        _slaMonitoringService = slaMonitoringService;
        _metricsExportService = metricsExportService;
    }

    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<PerformanceDashboardSnapshot> GetSummary([FromQuery] int minutes = 60)
    {
        var window = TimeSpan.FromMinutes(Math.Clamp(minutes, 1, 1440));
        return new PerformanceDashboardSnapshot
        {
            RequestSummary = _requestMetricsService.GetSummary(window),
            Throughput = _requestMetricsService.GetThroughput(window),
            Resource = _resourceUtilizationService.GetSummary(window),
            Cost = _costAnalysisService.GetSummary(window),
            Sla = _slaMonitoringService.Evaluate(window)
        };
    }

    [HttpGet("throughput")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ThroughputSnapshot> GetThroughput([FromQuery] int minutes = 5)
    {
        var window = TimeSpan.FromMinutes(Math.Clamp(minutes, 1, 1440));
        return _requestMetricsService.GetThroughput(window);
    }

    [HttpGet("resource")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ResourceSummary> GetResource([FromQuery] int minutes = 10)
    {
        var window = TimeSpan.FromMinutes(Math.Clamp(minutes, 1, 1440));
        return _resourceUtilizationService.GetSummary(window);
    }

    [HttpGet("cost")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<CostSummary> GetCost([FromQuery] int minutes = 60)
    {
        var window = TimeSpan.FromMinutes(Math.Clamp(minutes, 1, 1440));
        return _costAnalysisService.GetSummary(window);
    }

    [HttpGet("sla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<SLAReport> GetSla([FromQuery] int minutes = 60)
    {
        var window = TimeSpan.FromMinutes(Math.Clamp(minutes, 1, 1440));
        return _slaMonitoringService.Evaluate(window);
    }

    [HttpGet("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Export([FromQuery] string format = "json", [FromQuery] int minutes = 60)
    {
        var window = TimeSpan.FromMinutes(Math.Clamp(minutes, 1, 1440));
        return format.ToLowerInvariant() switch
        {
            "csv" => File(System.Text.Encoding.UTF8.GetBytes(_metricsExportService.ExportCsv(window)), "text/csv", "performance-metrics.csv"),
            _ => Ok(_metricsExportService.CaptureSnapshot(window))
        };
    }
}
