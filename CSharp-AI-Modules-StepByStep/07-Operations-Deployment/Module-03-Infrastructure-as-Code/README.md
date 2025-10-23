# Module 03: Infrastructure as Code

Sample Infrastructure as Code automation layer for AI workloads covering template management, plan generation, compliance checks, and provisioning simulation.

## Features

- Template catalog for Terraform, ARM, and Bicep resources
- Plan generation with Terraform, ARM template, and Bicep artefacts
- Compliance evaluation against configurable policy rules
- Drift estimation to highlight configuration risks
- Provisioning queue with background worker simulation and run history

## Project Layout

```
Module-03-Infrastructure-as-Code/
├── Controllers/
│   ├── InfrastructureController.cs
│   └── ProvisioningController.cs
├── HostedServices/
│   └── ProvisioningWorker.cs
├── Models/
│   └── IaCModels.cs
├── Services/
│   ├── ArmTemplateService.cs
│   ├── BicepTemplateService.cs
│   ├── ComplianceInsightsService.cs
│   ├── DriftDetectionService.cs
│   ├── InfrastructurePlanService.cs
│   ├── InfrastructureTemplateService.cs
│   ├── ProvisioningHistoryService.cs
│   ├── ProvisioningQueueService.cs
│   └── TerraformManifestService.cs
├── Module-03-Infrastructure-as-Code.csproj
├── Program.cs
├── README.md
└── appsettings.json
```

## Running the API

```powershell
cd Module-03-Infrastructure-as-Code
 dotnet restore
 dotnet run
```

Swagger UI: `/swagger`
Health endpoint: `/health`

## Key Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/infrastructure/templates` | List registered IaC templates. |
| POST | `/api/infrastructure/templates` | Create or update an IaC template. |
| POST | `/api/infrastructure/plan` | Generate Terraform, ARM, and Bicep artefacts. |
| POST | `/api/infrastructure/preview` | Produce a plan along with drift estimation. |
| POST | `/api/provisioning/queue` | Queue a simulated provisioning run. |
| GET | `/api/provisioning/runs` | Retrieve recent provisioning runs. |
| GET | `/api/provisioning/runs/{id}` | Inspect a specific provisioning run. |

## Configuration Highlights

- **Infrastructure.Templates**: Seed Terraform-based templates spanning Azure and AWS resources.
- **Infrastructure.ComplianceRules**: Declarative policy rules that validate plan outputs.
- **Infrastructure.Simulation**: Controls request defaults, worker cadence, and drift likelihood.

## Extending the Module

- Add providers for Pulumi, AWS CloudFormation, or Google Deployment Manager.
- Surface policy insights to external systems such as Azure Policy or AWS Config.
- Replace the simulation worker with actual provisioning actions through Terraform CLI, Bicep CLI, or cloud SDKs.
- Emit plan and run artefacts to storage accounts or Git repositories for auditing.

## Testing Tips

- Submit a plan request with different environments to confirm variable overrides.
- Queue provisioning runs and query `/api/provisioning/runs` to observe status transitions and drift analysis.
- Extend `appsettings.json` with additional compliance rules to enforce organisation standards.
- Review the generated Terraform, ARM, and Bicep artefacts to validate resource synthesis.
