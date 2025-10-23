# Module 01: Containerization

Reference containerization toolkit for AI services using ASP.NET Core, Docker, and Compose automation.

## Features

- **Template Catalog**: Manage reusable Dockerfile templates for .NET, Python, and other runtimes.
- **Dockerfile Generation**: Produce multi-stage Dockerfiles tailored to runtime version, telemetry, and security options.
- **Compose Profiles**: Generate ready-to-run `docker-compose.yml` files for local dev, observability, and integration stacks.
- **Build Queue Simulation**: Background worker simulates container builds, producing plans, optimization reports, and history records.
- **Optimization Insights**: Layer analysis and actionable recommendations to slim containers and harden builds.

## Project Layout

```
Module-01-Containerization/
├── Controllers/
│   ├── BuildsController.cs
│   └── ContainerizationController.cs
├── HostedServices/
│   └── ContainerBuildWorker.cs
├── Models/
│   └── ContainerModels.cs
├── Services/
│   ├── ContainerBuildHistoryService.cs
│   ├── ContainerBuildPlannerService.cs
│   ├── ContainerBuildQueueService.cs
│   ├── DockerComposeService.cs
│   ├── DockerfileGeneratorService.cs
│   ├── DockerfileTemplateService.cs
│   └── ImageOptimizationService.cs
├── Module-01-Containerization.csproj
├── Program.cs
├── README.md
└── appsettings.json
```

## Running the API

```powershell
cd Module-01-Containerization
 dotnet restore
 dotnet run
```

Swagger UI: `/swagger`
Health endpoint: `/health`

## Key Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/containerization/templates` | List available Dockerfile templates. |
| POST | `/api/containerization/templates` | Register a new Dockerfile template. |
| POST | `/api/containerization/plan` | Generate Dockerfile + Compose plan for a request. |
| POST | `/api/containerization/queue` | Queue a simulated container build. |
| GET | `/api/containerization/compose-profiles` | Discover compose profiles for different stacks. |
| GET | `/api/builds` | Review recent simulated build records. |
| GET | `/api/builds/{id}` | Inspect a specific build plan, optimization report, and status. |

## Configuration Highlights

- **Containerization.Templates**: Declare multi-stage build recipes, labels, and optimization tips.
- **Containerization.ComposeProfiles**: Pre-built Compose stacks (database, observability, etc.).
- **Containerization.Simulation**: Tune worker concurrency, delay, and registry suggestions.

## Extending the Module

- Plug in real build pipelines by replacing the simulation worker with Docker/ACR CLI integrations.
- Surface SBOM scanning or vulnerability reports alongside optimization guidance.
- Add registry credential rotation using secret stores (Key Vault, AWS Secrets Manager).
- Emit build events to message queues (Service Bus, Kafka) for downstream automation.

## Testing Tips

- POST `/api/containerization/plan` with different runtimes to validate Dockerfile generation.
- Queue builds and poll `/api/builds` to observe status transitions and optimization output.
- Customize `appsettings.json` with your own templates and compose profiles to mirror production stacks.
- Chain the generated Compose YAML into `docker compose up` to validate local deployments quickly.
