````markdown
# Module 06: Production Monitoring

Reference production observability API highlighting telemetry ingestion, alerting, SLO tracking, and incident management patterns for cloud workloads.

## Features

- Dashboard catalog with configurable widgets for service golden signals and custom views
- In-memory telemetry store supporting metrics, logs, traces, and discrete events ingestion
- Alert evaluation pipeline with active alert tracking, auto-incident creation, and severity mapping
- SLO evaluation engine calculating compliance windows and surfacing breaching objectives
- Monitoring reports combining telemetry snapshots, alert states, SLO status, and incident summaries
- Background simulation worker generating realistic telemetry to exercise the monitoring pipeline

## Project Layout

```
Module-06-Production-Monitoring/
├── Controllers/
│   ├── AlertsController.cs
│   ├── DashboardsController.cs
│   ├── IncidentsController.cs
│   ├── ReportsController.cs
│   └── TelemetryController.cs
├── HostedServices/
│   └── MonitoringSimulationWorker.cs
├── Models/
│   └── MonitoringModels.cs
├── Services/
│   ├── AlertingService.cs
│   ├── DashboardCatalogService.cs
│   ├── IncidentService.cs
│   ├── MonitoringReportService.cs
│   ├── SloEvaluationService.cs
│   └── TelemetryStoreService.cs
├── Module-06-Production-Monitoring.csproj
├── Program.cs
├── README.md
└── appsettings.json
```

## Running the API

```powershell
cd Module-06-Production-Monitoring
 dotnet restore
 dotnet run
```

Swagger UI: `/swagger`
Health endpoint: `/health`

## Key Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/monitoring/dashboards` | List dashboards available to analytics stakeholders. |
| POST | `/api/monitoring/dashboards` | Create or update dashboard definitions. |
| POST | `/api/monitoring/telemetry/ingest` | Push metrics, logs, traces, and events into the telemetry store. |
| GET | `/api/monitoring/telemetry/snapshot` | Retrieve a synthesized monitoring snapshot. |
| POST | `/api/monitoring/alerts/evaluate` | Force alert evaluation across configured rules. |
| GET | `/api/monitoring/alerts` | Inspect currently active alerts. |
| GET | `/api/monitoring/incidents/active` | List active production incidents. |
| POST | `/api/monitoring/incidents` | Declare a new incident tied to an alert. |
| POST | `/api/monitoring/incidents/{id}/resolve` | Resolve incidents and capture postmortem scaffolding. |
| GET | `/api/monitoring/reports/{type}` | Generate monitoring reports (shift handover, executive, etc.). |

## Configuration Highlights

- **Monitoring.Dashboards**: Seed dashboards and widgets to visualise golden signals per service.
- **Monitoring.AlertRules**: Threshold-based alert definitions with severity and entity targeting.
- **Monitoring.ServiceLevelObjectives**: SLO definitions mapping metrics and target directions.
- **Monitoring.Reports**: Toggle sections included in monitoring reports for different audiences.
- **Monitoring.Simulation**: Controls telemetry cadence, regions, services, and incident likelihood for the worker.

## Extending the Module

- Back the telemetry store with a durable data service (Azure Monitor, Prometheus, Elastic).
- Integrate with paging/on-call systems to surface incidents to responders.
- Expand alert operators to support compound conditions or anomaly detection models.
- Pipe generated reports to collaboration tools or ticketing systems.

## Testing Tips

- Adjust `IncidentLikelihood` in configuration to rehearse incident volume scenarios.
- POST synthetic metrics via `/api/monitoring/telemetry/ingest` and validate alert transitions.
- Generate reports for `shift-handover` and `executive-daily` to ensure summaries align.
- Modify SLO targets and observe breaching state changes through `/api/monitoring/telemetry/snapshot`.
````
