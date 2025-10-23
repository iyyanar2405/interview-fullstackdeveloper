# Module 05: Cloud Deployment

Reference cloud deployment orchestration API showcasing multi-cloud rollout planning, guardrail validation, traffic strategy guidance, and simulated release execution.

## Features

- Blueprint catalog spanning Azure App Service, Container Apps, and AWS ECS rollouts
- Automated deployment plan generation with runbooks, traffic strategies, and resource manifests
- Guardrail evaluation and integration summaries for observability, secrets, and artifact logistics
- Deployment queue with background worker simulating release timelines and outcomes
- Endpoints for previewing plans, queueing runs, and inspecting deployment history

## Project Layout

```
Module-05-Cloud-Deployment/
├── Controllers/
│   ├── BlueprintsController.cs
│   └── DeploymentsController.cs
├── HostedServices/
│   └── DeploymentWorker.cs
├── Models/
│   └── CloudDeploymentModels.cs
├── Services/
│   ├── CloudBlueprintService.cs
│   ├── DeploymentHistoryService.cs
│   ├── DeploymentPlanService.cs
│   ├── DeploymentQueueService.cs
│   ├── DeploymentSimulationService.cs
│   ├── GuardrailEvaluationService.cs
│   ├── IntegrationComposerService.cs
│   ├── ReleaseRunbookService.cs
│   ├── ResourceManifestService.cs
│   └── TrafficStrategyService.cs
├── Module-05-Cloud-Deployment.csproj
├── Program.cs
├── README.md
└── appsettings.json
```

## Running the API

```powershell
cd Module-05-Cloud-Deployment
 dotnet restore
 dotnet run
```

Swagger UI: `/swagger`
Health endpoint: `/health`

## Key Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/cloud-blueprints` | List configured deployment blueprints. |
| POST | `/api/cloud-blueprints` | Create or update a cloud deployment blueprint. |
| POST | `/api/cloud-deployments/plan` | Generate a detailed deployment plan. |
| POST | `/api/cloud-deployments/preview` | Preview guardrails, manifests, and traffic plan. |
| POST | `/api/cloud-deployments/queue` | Queue a simulated deployment run. |
| GET | `/api/cloud-deployments/runs` | Inspect recent deployment executions. |
| GET | `/api/cloud-deployments/runs/{id}` | Retrieve a specific deployment run record. |

## Configuration Highlights

- **CloudDeployment.Blueprints**: Seed deployment topologies per provider, including components and traffic strategies.
- **CloudDeployment.Guardrails**: Guardrail policies surfaced in plans for operator awareness.
- **CloudDeployment.Simulation**: Controls worker cadence, failure rates, and traffic split guidance.
- **CloudDeployment.Integrations**: Defaults for observability, secrets, and artifact handling.

## Extending the Module

- Plug in real infrastructure provisioning scripts (Terraform, ARM/Bicep, CloudFormation).
- Emit deployment telemetry to event hubs or incident management systems.
- Integrate with CI/CD platforms to trigger actual releases upon approval.
- Expand guardrail catalog to enforce organisation compliance or cost constraints.

## Testing Tips

- Create custom blueprints covering AKS or multi-region deployments and verify plan output.
- Use `/api/cloud-deployments/preview` to validate guardrail annotations before queueing runs.
- Queue deployments and poll `/api/cloud-deployments/runs` to observe timeline simulation.
- Adjust `FailureRate` in configuration to rehearse rollback and remediation workflows.
