# Module 04: Application Insights

Azure Application Insights integration playbook for ASP.NET Core services.

## Capabilities

- **Automatic Instrumentation**: Requests, dependencies, performance counters, and event counters wired through AI SDK modules.
- **Custom Telemetry**: Exposes helpers to send bespoke events, metrics, availability pings, and dependency spans.
- **Role Naming**: Telemetry initializer assigns `cloud_RoleName`/`cloud_RoleInstance` for easy filtering across services.
- **Noise Reduction**: Telemetry processor drops `/health` and `/swagger` requests to reduce ingestion volume.
- **Heartbeat Service**: Hosted background worker that emits uptime metrics and availability telemetry every minute.
- **Serilog Bridge**: Structured logs complement Application Insights traces for a full observability ribbon.

## Project Layout

```
Module-04-Application-Insights/
├── Controllers/
│   └── TelemetryController.cs
├── Services/
│   ├── TelemetryHeartbeatService.cs
│   └── TelemetryService.cs
├── Telemetry/
│   ├── CloudRoleTelemetryInitializer.cs
│   └── TelemetryFilterProcessor.cs
├── Module-04-Application-Insights.csproj
├── Program.cs
├── appsettings.json
└── README.md
```

## Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| POST | `/api/telemetry/event` | Emits a custom event with optional payload properties. |
| POST | `/api/telemetry/metric?name=foo&value=42` | Sends a business metric point. |
| GET | `/api/telemetry/dependency?delayMs=200` | Simulates and records an outgoing dependency call. |
| GET | `/api/telemetry/exception` | Throws an exception and records it in Application Insights. |
| GET | `/health` | Liveness probe (filtered from telemetry). |
| GET | `/swagger` | Swagger UI and OpenAPI spec. |

## Quick Start

```powershell
cd Module-04-Application-Insights
 dotnet restore
 dotnet run
```

Update `appsettings.json` with your Application Insights connection string. When running locally, telemetry is buffered and flushed periodically—exit the app gracefully or call `TelemetryClient.Flush()` to force send.

## Configuration Notes

- `ApplicationInsights:ConnectionString` supports full connection strings or legacy instrumentation keys.
- `ApplicationInsights:RoleName` helps differentiate services in Application Insights analytics queries.
- Add more processors (sampling, redaction) by calling `AddApplicationInsightsTelemetryProcessor` in `Program.cs`.
- Leverage `TelemetryService` from business code to track domain-specific signal (orders processed, failures, etc.).

Pair this module with Modules 01–03 for a comprehensive telemetry, logging, and tracing stack built on Azure Monitor.
