# Module 05 - Memory Systems

Implements an AI memory substrate that models short-term, long-term, episodic, semantic, and working memories. The sample demonstrates ingestion, retrieval, analytics, and automated consolidation of memories across multiple agent profiles.

## Project Structure

- `Models/MemoryModels.cs` – options, definitions, memory entries, retrieval requests, consolidation events.
- `Services/MemoryProfileCatalogService.cs` – exposes configured memory profiles and store definitions.
- `Services/MemoryStoreService.cs` – in-memory store for all memories with retention, tagging, and capacity enforcement.
- `Services/MemoryRetrievalService.cs` – scoring and retrieval pipeline supporting tag/search filters and insight generation.
- `Services/MemoryAnalyticsService.cs` – aggregates per-profile statistics and tag clouds.
- `Services/MemoryConsolidationService.cs` – auto-promotes memories and records consolidation events.
- `HostedServices/MemoryConsolidationWorker.cs` – background worker that periodically runs consolidation cycles.
- `Controllers/MemoryProfilesController.cs` – endpoints for profile metadata and summaries.
- `Controllers/MemoryEntriesController.cs` – ingestion API, store snapshots, and recent entries.
- `Controllers/MemoryRetrievalController.cs` – retrieval endpoint returning ranked memories and insights.
- `Controllers/MemoryAnalyticsController.cs` – high-level analytics snapshot per profile.
- `Controllers/MemoryConsolidationController.cs` – exposes recent consolidation events.
- `appsettings.json` – seeds representative memory profiles and simulation settings.
- `Program.cs` – service registration, Serilog logging, Swagger, and health checks.

## Endpoints

- `GET /api/memoryprofiles` – list configured profiles.
- `GET /api/memoryprofiles/{profileId}` – fetch profile definition.
- `GET /api/memoryprofiles/{profileId}/summary` – aggregated store statistics.
- `POST /api/memoryentries/ingest` – ingest a single memory entry.
- `POST /api/memoryentries/ingest/batch` – ingest multiple entries at once.
- `GET /api/memoryentries/{profileId}/{storeId}/snapshot` – store-level metrics.
- `GET /api/memoryentries/{profileId}/{storeId}/entries?limit=50` – recent entries.
- `POST /api/memoryretrieval/retrieve` – run retrieval against one or more stores.
- `GET /api/memoryanalytics/{profileId}` – analytics snapshot with insights.
- `GET /api/memoryconsolidation/events` – recent consolidation events.

## Running the Module

```powershell
cd CSharp-AI-Modules-StepByStep/08-Advanced-AI/Module-05-Memory-Systems
dotnet build
dotnet run
```

Swagger UI is available at `https://localhost:{port}/swagger` when running in Development. Health probe is available at `/health`.
