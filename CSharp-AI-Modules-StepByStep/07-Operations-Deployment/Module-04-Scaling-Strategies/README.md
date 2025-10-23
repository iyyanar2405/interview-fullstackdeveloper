# Module 04: Scaling Strategies

Reference scaling and auto-scaling toolkit for AI-focused workloads that generates capacity plans, cost projections, and simulated scaling runs.

## Features

- Manage scaling profiles with provider-specific thresholds and cost models
- Generate full scaling plans with forecasts, cost projections, and policy documents
- Forecast capacity requirements based on historical snapshots and load scenarios
- Simulate scaling executions to validate decisions before deployment
- Track run history and surface insights about utilisation and savings opportunities

## Project Layout

```
Module-04-Scaling-Strategies/
├── Controllers/
│   ├── ScalingController.cs
│   └── SimulationsController.cs
├── HostedServices/
│   └── ScalingSimulationWorker.cs
├── Models/
│   └── ScalingModels.cs
├── Services/
│   ├── CapacityForecastService.cs
│   ├── CostProjectionService.cs
│   ├── ScalingHistoryService.cs
│   ├── ScalingPolicyBuilderService.cs
│   ├── ScalingProfileService.cs
│   ├── ScalingQueueService.cs
│   ├── ScalingRecommendationService.cs
│   └── ScalingSimulationService.cs
├── Module-04-Scaling-Strategies.csproj
├── Program.cs
├── README.md
└── appsettings.json
```

## Running the API

```powershell
cd Module-04-Scaling-Strategies
 dotnet restore
 dotnet run
```

Swagger UI: `/swagger`
Health endpoint: `/health`

## Key Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/scaling/profiles` | List scaling profiles and thresholds. |
| POST | `/api/scaling/profiles` | Create or update a scaling profile. |
| POST | `/api/scaling/plan` | Build a scaling plan with actions and cost projections. |
| POST | `/api/scaling/forecast` | Generate a capacity forecast timeline. |
| POST | `/api/scaling-simulations/queue` | Queue a simulated scaling run for a profile and scenario. |
| GET | `/api/scaling-simulations/runs` | Retrieve recent simulated scaling runs. |
| GET | `/api/scaling-simulations/runs/{id}` | Inspect individual run details and timeline. |

## Configuration Highlights

- **Scaling.Profiles**: Seed scaling policies for web, worker, or compute workloads with cost details.
- **Scaling.Forecast**: Tune default growth assumptions and forecast horizon used in plan generation.
- **Scaling.Simulation**: Define ready-made load scenarios and worker cadence for background simulations.

## Extending the Module

- Add provider-specific policy renderers for AWS Auto Scaling, GKE, or Azure Virtual Machine Scale Sets.
- Persist run history to a database or event stream for long-term analytics.
- Feed real telemetry from Azure Monitor, Prometheus, or Application Insights into the snapshot payloads.
- Expose webhook events when simulations reach scaling thresholds to drive downstream automation.

## Testing Tips

- Submit `/api/scaling/plan` requests with varying snapshots to observe different scaling actions.
- Queue simulations and poll `/api/scaling-simulations/runs` to watch the timeline evolve.
- Modify `appsettings.json` cost figures to explore cost-saving opportunities surfaced by the API.
- Extend load scenarios and confirm forecasts adjust using `/api/scaling/forecast`.
