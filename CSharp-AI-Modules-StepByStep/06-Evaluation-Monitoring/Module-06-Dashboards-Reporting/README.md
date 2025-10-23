# Module 06: Dashboards & Reporting

Real-time dashboards, historical analytics, and executive reporting for AI systems built on ASP.NET Core.

## Features

- **Dynamic Dashboards**: Manage dashboard definitions, widgets, refresh cadence, and tag-based organization via REST.
- **Metric Aggregation**: Stream simulated metric data, maintain rolling history, and compute windowed aggregates.
- **Snapshot History**: Persist dashboard snapshots and KPI callouts for historical comparisons and auditability.
- **Report Generation**: Produce structured reports with KPI highlights, trend sections, and tabular breakdowns.
- **Executive Summaries**: Generate narrative summaries highlighting strengths, risks, and recommended actions.

## Project Layout

```
Module-06-Dashboards-Reporting/
├── Controllers/
│   ├── DashboardsController.cs
│   └── ReportsController.cs
├── HostedServices/
│   ├── DashboardSnapshotHostedService.cs
│   └── MetricSimulationHostedService.cs
├── Models/
│   └── DashboardModels.cs
├── Services/
│   ├── DashboardComposerService.cs
│   ├── DashboardDefinitionService.cs
│   ├── DashboardSnapshotService.cs
│   ├── ExecutiveSummaryService.cs
│   ├── MetricArchiveService.cs
│   └── ReportGenerationService.cs
├── Module-06-Dashboards-Reporting.csproj
├── Program.cs
├── README.md
└── appsettings.json
```

## Running the API

```powershell
cd Module-06-Dashboards-Reporting
 dotnet restore
 dotnet run
```

Swagger UI is available at `/swagger`. Health probe exposed at `/health`.

## Key Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/dashboards` | List dashboard definitions. |
| POST | `/api/dashboards` | Create a dashboard + widget layout. |
| GET | `/api/dashboards/{id}/snapshot` | Latest composed dashboard snapshot. |
| GET | `/api/dashboards/{id}/history?count=20` | Historical snapshots for comparisons. |
| GET | `/api/dashboards/{id}/kpis` | Current KPI callouts for the dashboard. |
| POST | `/api/reports` | Generate a multi-section report for requested metrics. |
| POST | `/api/reports/executive-summary` | Produce an executive narrative for stakeholders. |
| GET | `/api/reports/metrics` | Discover available metric identifiers. |

## Configuration Highlights

- **Dashboard.SeedDashboards**: Provision starter dashboards, widgets, and thresholds.
- **Dashboard.SnapshotHistory**: Controls snapshot retention depth for historical review.
- **Reporting.AllowedFormats**: Restrict report formats (json/csv/pdf, etc.).
- **MetricsSimulation**: Configure baseline values, volatility, and cadence for the synthetic metric stream.

## Extending the Module

- Replace the in-memory `MetricArchiveService` with a time-series database (Prometheus, InfluxDB, TimescaleDB).
- Integrate real metric pipelines by publishing `MetricTrendPoint` data from Observability stacks.
- Add authentication/authorization policies to protect dashboard and reporting endpoints.
- Export reports to PDF or PowerPoint by layering in document rendering services.

## Testing Tips

- Hit `/api/reports` with custom metric lists and time ranges to validate aggregation logic.
- Adjust `MetricsSimulation` baselines to emulate traffic spikes and watch dashboards react.
- Chain with Module-05 alerting to raise notifications when dashboard KPIs breach thresholds.
- Feed the executive summary output into communications workflows (Teams, Slack, email digests).
