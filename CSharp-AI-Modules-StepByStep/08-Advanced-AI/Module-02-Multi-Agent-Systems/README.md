````markdown
# Module 02: Multi-Agent Systems

Reference multi-agent collaboration API showcasing teaming, coordination workflows, message routing, and consensus simulation for distributed AI assistants.

## Features

- Team registry with mission statements, role definitions, and decision rights
- Communication channel catalog supporting synchronous and asynchronous collaboration
- Playbook library encoding coordination steps and hand-offs between agent roles
- Task board tracking playbook execution state, timeline events, votes, and outcomes
- Message router for publishing and retrieving channel activity across the swarm
- Background coordination worker simulating consensus, escalation, and completion flows

## Project Layout

```
Module-02-Multi-Agent-Systems/
├── Controllers/
│   ├── ChannelsController.cs
│   ├── PlaybooksController.cs
│   ├── TasksController.cs
│   └── TeamsController.cs
├── HostedServices/
│   └── CoordinationWorker.cs
├── Models/
│   └── MultiAgentModels.cs
├── Services/
│   ├── ChannelRegistryService.cs
│   ├── CoordinationEngineService.cs
│   ├── MessageRouterService.cs
│   ├── PlaybookLibraryService.cs
│   ├── TaskBoardService.cs
│   ├── TaskDispatchQueueService.cs
│   └── TeamRegistryService.cs
├── Module-02-Multi-Agent-Systems.csproj
├── Program.cs
├── README.md
└── appsettings.json
```

## Running the API

```powershell
cd Module-02-Multi-Agent-Systems
 dotnet restore
 dotnet run
```

Swagger UI: `/swagger`
Health endpoint: `/health`

## Key Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/multi-agent/teams` | List configured teams, missions, and role compositions. |
| GET | `/api/multi-agent/teams/{team}` | Inspect a specific team's definition. |
| GET | `/api/multi-agent/teams/{team}/tasks` | View recent coordination tasks for the team. |
| GET | `/api/multi-agent/channels` | Explore channel capabilities and message types. |
| GET | `/api/multi-agent/channels/{id}/messages` | Stream recent collaboration activity. |
| POST | `/api/multi-agent/channels/{id}/messages` | Publish a message on a coordination channel. |
| GET | `/api/multi-agent/playbooks` | Review available coordination playbooks. |
| POST | `/api/multi-agent/tasks` | Submit a playbook-driven multi-agent task. |
| GET | `/api/multi-agent/tasks/{taskId}` | Check task state, timeline, votes, and outputs. |
| POST | `/api/multi-agent/tasks/{taskId}/consensus/{roleId}` | Submit manual consensus votes when needed. |

## Configuration Highlights

- **MultiAgent.Teams**: Teams, role personas, capacities, and preferred channels.
- **MultiAgent.Channels**: Communication surfaces with message type policies.
- **MultiAgent.Playbooks**: Step-by-step coordination templates executed by the worker.
- **MultiAgent.Simulation**: Tuning knobs for coordination cadence and consensus behaviour.

## Extending the Module

- Integrate real transport layers (SignalR, Service Bus) in `MessageRouterService` for distributed agents.
- Persist task board and message streams in durable storage for analytics and replay.
- Enrich playbooks with branching conditions or probabilistic routing between steps.
- Emit telemetry and metrics from the coordination worker to observe throughput and success rates.

## Testing Tips

- Submit a task for `incident-response` using playbook `p0-incident` and observe state transitions.
- Poll `/api/multi-agent/channels/war-room/messages` to trace the simulated coordination dialogue.
- Toggle `ConsensusFailureRate` to rehearse escalation handling paths.
- Post manual consensus votes to `/api/multi-agent/tasks/{taskId}/consensus/{roleId}` to override automation.
````
