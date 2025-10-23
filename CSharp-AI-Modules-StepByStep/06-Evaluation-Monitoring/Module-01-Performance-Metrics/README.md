# Module 01: Performance Metrics

High-resolution performance monitoring for AI workloads built with ASP.NET Core.

## Features

- **Request Traceability**: Collects latency, request/response sizes, status codes, success/error ratios, and percentile distributions.
- **Throughput Analytics**: Computes requests per second/minute/hour across configurable windows.
- **Resource Telemetry**: Samples CPU, memory, managed heap, thread counts, and concurrent requests.
- **Cost Tracking**: Estimates per-request cost using configurable pricing for duration, payload size, and token usage.
- **SLA Monitoring**: Evaluates configurable SLO/SLA targets with compliance scoring and violation reporting.
- **Exports & Dashboards**: Produces JSON dashboard snapshots and CSV exports; exposes Prometheus `/metrics` and Swagger UI.

## Project Layout

```
Module-01-Performance-Metrics/
├── Controllers/
│   └── PerformanceMetricsController.cs
├── HostedServices/
│   └── ResourceSamplingHostedService.cs
├── Middleware/
│   └── RequestMetricsMiddleware.cs
├── Models/
│   └── PerformanceMetricModels.cs
├── Services/
│   ├── CostAnalysisService.cs
│   ├── MetricsExportService.cs
│   ├── RequestMetricsService.cs
│   ├── ResourceUtilizationService.cs
│   └── SLAMonitoringService.cs
├── Module-01-Performance-Metrics.csproj
├── Program.cs
└── appsettings.json
```

## Running the API

```powershell
cd Module-01-Performance-Metrics
 dotnet restore
 dotnet run
```

Default URLs: `https://localhost:7101` and `http://localhost:5101`

Swagger UI is available at `/swagger`.

## Key Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/performance/summary` | Consolidated dashboard (latency, throughput, resource, cost, SLA). |
| GET | `/api/performance/throughput?minutes=5` | Throughput metrics for the requested window. |
| GET | `/api/performance/resource?minutes=10` | CPU/memory/thread summaries. |
| GET | `/api/performance/cost?minutes=60` | Cost analytics for the requested window. |
| GET | `/api/performance/sla?minutes=60` | SLA compliance report. |
| GET | `/api/performance/export?format=json&minutes=60` | JSON snapshot (default). |
| GET | `/api/performance/export?format=csv&minutes=60` | CSV export suitable for spreadsheets. |
| GET | `/metrics` | Prometheus-compatible metrics. |
| GET | `/health` | Application health probe. |

## Configuration Highlights

- `CostSettings`: Tune pricing for request cost estimation (duration, payload, tokens).
- `SLA.Targets`: Configure SLA checks (metric name, comparison, threshold, window minutes).
- `ResourceSampling`: Control sampling interval and history depth for resource telemetry.

## Extending the Module

- Feed real token usage via the `X-Token-Usage` header to improve cost accuracy.
- Wire in persistent storage (SQL/NoSQL) by replacing the in-memory queues in services.
- Add OpenTelemetry exporters for distributed tracing or connect to Application Insights.
- Integrate alerting by watching SLA violations emitted from `SLAMonitoringService`.

## Testing Tips

- Use tools like `bombardier`, `wrk`, or `hey` to generate load and observe latency distribution shifts.
- Monitor `/metrics` with Prometheus + Grafana dashboards for live visibility.
- Adjust `CostSettings` to match your provider billing for accurate FinOps reporting.
- Run chaos/load scenarios and inspect `/api/performance/sla` for violation detection.
