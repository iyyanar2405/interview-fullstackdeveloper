# n8n Workflow — Step by Step Implementation

This guide scaffolds and runs n8n locally with Docker and walks you through 4 production-style workflows. Tailored for Windows/PowerShell.

## What you’ll get
- Dockerized n8n with Postgres and persistent volumes
- .env example pre-wired for Windows paths
- 4 guided workflows with exports (.json) and docs
- Templates, checklists, and troubleshooting tips

➡ For a full step-by-step walkthrough, see [TUTORIAL.md](./TUTORIAL.md).

## Prereqs
- Docker Desktop (Windows) with WSL2 backend
- A Slack workspace (optional), Google account (optional), Jira/Trello (optional)

## Structure
- docker-compose.yml
- .env.example
- workflows/
  - 01-http-trigger-to-slack.md (+ export)
  - 02-scheduled-api-to-sheets.md (+ export)
  - 03-error-handling-patterns.md (+ snippets)
  - 04-webhook-to-jira-or-trello.md (+ export)
- Templates-&-Checklists.md
- Troubleshooting.md

## Quickstart (PowerShell)
1. Copy .env.example to .env and adjust values
2. Start stack
3. Open n8n at http://localhost:5678

Commands (run from this folder):

```powershell
# 1) Copy env
Copy-Item .env.example .env -Force

# 2) Start containers
docker compose up -d

# 3) View logs (optional)
docker compose logs -f --tail=100

# 4) Stop containers (when done)
docker compose down
```

Notes
- Change `GENERIC_TIMEZONE` in `.env` as needed
- If port 5678 is busy, change `N8N_PORT` in `.env`
