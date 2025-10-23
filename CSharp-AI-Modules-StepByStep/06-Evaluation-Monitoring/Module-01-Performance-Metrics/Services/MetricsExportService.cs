using System.Text;
using Module_01_Performance_Metrics.Models;

namespace Module_01_Performance_Metrics.Services;

public sealed class MetricsExportService
{
    private readonly RequestMetricsService _requestMetricsService;
    private readonly ResourceUtilizationService _resourceUtilizationService;
    private readonly CostAnalysisService _costAnalysisService;
    private readonly SLAMonitoringService _slaMonitoringService;

    public MetricsExportService(
        RequestMetricsService requestMetricsService,
        ResourceUtilizationService resourceUtilizationService,
        CostAnalysisService costAnalysisService,
        SLAMonitoringService slaMonitoringService)
    {
        _requestMetricsService = requestMetricsService;
        _resourceUtilizationService = resourceUtilizationService;
        _costAnalysisService = costAnalysisService;
        _slaMonitoringService = slaMonitoringService;
    }

    public ExportEnvelope CaptureSnapshot(TimeSpan window)
    {
        var summary = _requestMetricsService.GetSummary(window);
        var throughput = _requestMetricsService.GetThroughput(window);
        var resource = _resourceUtilizationService.GetSummary(window);
        var cost = _costAnalysisService.GetSummary(window);
        var sla = _slaMonitoringService.Evaluate(window);

        return new ExportEnvelope
        {
            Format = "json",
            Snapshot = new PerformanceDashboardSnapshot
            {
                RequestSummary = summary,
                Throughput = throughput,
                Resource = resource,
                Cost = cost,
                Sla = sla
            }
        };
    }

    public string ExportCsv(TimeSpan window)
    {
        var summary = _requestMetricsService.GetSummary(window);
        var throughput = _requestMetricsService.GetThroughput(window);
        var resource = _resourceUtilizationService.GetSummary(window);
        var cost = _costAnalysisService.GetSummary(window);
        var sla = _slaMonitoringService.Evaluate(window);

        var builder = new StringBuilder();
        builder.AppendLine("Metric,Value");
        builder.AppendLine($"TotalRequests,{summary.TotalRequests}");
        builder.AppendLine($"SuccessRate,{summary.SuccessRate:P2}");
        builder.AppendLine($"ErrorRate,{summary.ErrorRate:P2}");
        builder.AppendLine($"AvgLatencyMs,{summary.AverageLatencyMs:F2}");
        builder.AppendLine($"P95LatencyMs,{summary.P95LatencyMs:F2}");
        builder.AppendLine($"ThroughputPerSecond,{throughput.RequestsPerSecond:F2}");
        builder.AppendLine($"AverageCpu,{resource.AverageCpuUsage:F2}");
        builder.AppendLine($"AverageMemoryMb,{resource.AverageMemoryUsageMb:F2}");
        builder.AppendLine($"TotalCost,{cost.TotalCost:F6}");
        builder.AppendLine($"CostPerRequest,{cost.AverageCostPerRequest:F6}");
        builder.AppendLine($"SLACompliance,{sla.ComplianceRate:P2}");
        return builder.ToString();
    }
}
