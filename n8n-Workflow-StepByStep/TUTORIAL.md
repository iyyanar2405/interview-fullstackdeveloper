# n8n Complete Step-by-Step Tutorial (Windows + Docker)

This tutorial guides you from zero to four working n8n workflows locally, with tips for production hardening.

## 1) Install & prerequisites
- Docker Desktop with WSL2 backend
- PowerShell terminal
- Optional accounts: Slack, Google, Jira/Trello

## 2) Clone/open the project
- Navigate to `n8n-Workflow-StepByStep` folder in this repo

## 3) Configure environment
- Copy `.env.example` to `.env`
- Edit values if needed (timezone, port, auth)

## 4) Start services
- In PowerShell (from this folder):
  - `docker compose up -d`
  - Visit http://localhost:5678 (auth from `.env`)

## 5) Create your first workflow (HTTP → Slack)
- Nodes: HTTP Trigger → Set → Slack
- HTTP Trigger: POST /notify
- Set: pick `title`, `severity`, `description`
- Slack: Post message to a channel (create Slack app/bot and OAuth credential in n8n)
- Test with a POST payload (see workflow doc `workflows/01-http-trigger-to-slack.md`)
- Export JSON and save in `workflows/`

## 6) Scheduled API → Google Sheets
- Nodes: Cron → HTTP Request → Set → Google Sheets (Append)
- Cron: every 5 minutes (example)
- HTTP Request: GET a public API
- Google Sheets: connect via OAuth; map fields
- Export JSON and save

## 7) Error handling patterns
- Create an Error Trigger workflow to notify on any workflow errors
- Use If node to branch on failures in sample flows
- Centralize alert format in a Set node

## 8) Webhook → Jira or Trello
- Nodes: HTTP Trigger → Set → IF → Jira/Trello → Respond
- Post to /task; route based on `system` field; return created ID

## 9) Persist, backup, and update
- Volumes `n8n_data` and `pg_data` persist data
- Backup by stopping containers and archiving `n8n_data` + DB dump
- Update: `docker pull n8nio/n8n:latest` then `docker compose up -d`

## 10) Production tips
- Use a reverse proxy (Caddy/Traefik/Nginx) with HTTPS
- Set a real `N8N_HOST`, `WEBHOOK_URL`, and secure secrets
- Limit stored executions; monitor CPU/memory

## 11) Troubleshooting
- See `Troubleshooting.md` for ports, OAuth redirects, webhooks, volumes

## 12) Templates
- See `Templates-&-Checklists.md` for auth/testing/observability/deployment

You now have a working n8n setup and four reusable workflow patterns. Explore more nodes in the n8n UI and keep your exports versioned in this repo.
