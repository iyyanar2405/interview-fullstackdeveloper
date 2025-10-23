using Microsoft.Extensions.Options;
using Module_06_Production_Monitoring.Models;
using System.Linq;

namespace Module_06_Production_Monitoring.Services;

public sealed class MonitoringReportService
{
    private readonly IOptions<MonitoringOptions> _options;
    private readonly TelemetryStoreService _telemetryStore;
    private readonly SloEvaluationService _sloEvaluationService;
    private readonly AlertingService _alertingService;
    private readonly IncidentService _incidentService;

    public MonitoringReportService(
        IOptions<MonitoringOptions> options,
        TelemetryStoreService telemetryStore,
        SloEvaluationService sloEvaluationService,
        AlertingService alertingService,
        IncidentService incidentService)
    {
        _options = options;
        _telemetryStore = telemetryStore;
        _sloEvaluationService = sloEvaluationService;
        _alertingService = alertingService;
        _incidentService = incidentService;
    }

    public MonitoringReport GenerateReport(string reportType)
    {
        var definition = _options.Value.Reports.FirstOrDefault(r => r.Type.Equals(reportType, StringComparison.OrdinalIgnoreCase));
        if (definition is null)
        {
            throw new InvalidOperationException($"Report type '{reportType}' is not configured.");
        }

        var snapshot = _telemetryStore.CaptureSnapshot();
        var sloStatuses = _sloEvaluationService.Evaluate();
        var alerts = _alertingService.GetActiveAlerts();
        var incidents = _incidentService.GetActiveIncidents();

        var report = new MonitoringReport
        {
            ReportType = definition.Type,
            GeneratedAt = DateTimeOffset.UtcNow,
            Notes = definition.Notes,
            Snapshot = snapshot,
            SloStatuses = sloStatuses.ToList(),
            ActiveAlerts = alerts.ToList(),
            ActiveIncidents = incidents.ToList(),
            Sections = new List<MonitoringReportSection>()
        };

        if (definition.IncludeTelemetrySummary)
        {
            report.Sections.Add(new MonitoringReportSection
            {
                Title = "Telemetry Summary",
                Content = $"Captured {snapshot.ActiveMetrics.Count} metrics and {snapshot.RecentEvents.Count} events"
            });
        }

        if (definition.IncludeSloStatus)
        {
            var unhealthy = report.SloStatuses.Count(status => status.Status == SloState.Breaching);
            report.Sections.Add(new MonitoringReportSection
            {
                Title = "SLO Status",
                Content = unhealthy == 0 ? "All SLOs healthy" : $"{unhealthy} SLO(s) breaching"
            });
        }

        if (definition.IncludeAlertSummary)
        {
            report.Sections.Add(new MonitoringReportSection
            {
                Title = "Alerts",
                Content = report.ActiveAlerts.Count == 0 ? "No active alerts" : string.Join(", ", report.ActiveAlerts.Select(alert => alert.Name))
            });
        }

        if (definition.IncludeIncidentSummary)
        {
            report.Sections.Add(new MonitoringReportSection
            {
                Title = "Incidents",
                Content = report.ActiveIncidents.Count == 0 ? "No active incidents" : string.Join(", ", report.ActiveIncidents.Select(incident => incident.Summary))
            });
        }

        return report;
    }
}
