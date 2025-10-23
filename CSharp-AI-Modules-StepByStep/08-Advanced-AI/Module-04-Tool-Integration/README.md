# Module 04 - Tool Integration

Simulates a comprehensive tool-execution environment that allows agents to call external APIs, query databases, manipulate virtual file systems, scrape the web, and run custom behaviours. The module demonstrates pluggable tool registries, asynchronous execution, and persistent run history.

## Project Structure

- `Models/ToolIntegrationModels.cs` – domain models for tool definitions, execution requests, results, and history records.
- `Services/ToolCatalogService.cs` – in-memory catalog seeded from configuration for all tool types.
- `Services/ToolExecutionQueueService.cs` – channel-backed queue for asynchronous execution requests.
- `Services/ToolExecutionHistoryService.cs` – captures run logs, data extracts, and file artifacts with configurable retention.
- `Services/ApiGatewayService.cs` – simulates REST API invocations.
- `Services/DatabaseExecutorService.cs` – simulates SQL-style queries and aggregates over seed data.
- `Services/FileSystemAutomationService.cs` – manages a virtual file system supporting list/read/write/delete.
- `Services/WebScrapingService.cs` – returns scraped samples using CSS selectors.
- `Services/CustomToolService.cs` – runs scripted behaviours for arbitrary tools.
- `Services/ToolExecutionOrchestrator.cs` – routes work items to the correct executor service.
- `Services/ToolExecutionService.cs` – façade used by controllers for tool discovery and run history access.
- `HostedServices/ToolExecutionWorker.cs` – background worker that drains the queue and executes work.
- `Controllers/ToolsController.cs` – endpoints for listing tools and submitting executions.
- `Controllers/ToolRunsController.cs` – endpoints for run logs, data extracts, and artifacts.
- `appsettings.json` – seeds tool definitions and simulation settings.
- `Program.cs` – application bootstrap with DI, Serilog logging, Swagger, and health checks.

## Endpoints

- `GET /api/tools` – list all registered tools.
- `GET /api/tools/{toolType}/{toolId}` – retrieve a specific tool descriptor.
- `POST /api/tools/execute` – submit a tool execution request (returns `runId`).
- `GET /api/toolruns` – list historical runs with execution status.
- `GET /api/toolruns/{runId}` – fetch a specific run log.
- `GET /api/toolruns/extracts` – view captured data extracts.
- `GET /api/toolruns/artifacts` – view generated file artifacts.

## Running the Module

```powershell
cd CSharp-AI-Modules-StepByStep/08-Advanced-AI/Module-04-Tool-Integration
dotnet build
dotnet run
```

Swagger UI is available at `https://localhost:{port}/swagger` when running in Development. Health probe is available at `/health`.
