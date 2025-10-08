# Agentic MLOps — Beginner → Advanced → Expert (Step-by-Step)

A practical, runnable workspace to learn and implement agentic MLOps—from zero to production-grade with experiment tracking, registry, artifact store, automated evaluations, and monitoring.

## Outcomes
- Beginner: Local MLflow + MinIO stack; log metrics/models; reproducible runs
- Advanced: Registry, model versioning, automated evals, promotion workflows
- Expert: Agentic pipelines (data curation, eval, monitoring triage), governance & SLOs

## Quickstart (local)
1) Copy env
- Duplicate `.env.example` to `.env` and adjust credentials if needed

2) Start services (Postgres + MinIO)
- Use Docker Desktop, then run the compose stack per your shell (see TUTORIAL.md)

3) Start MLflow server
- Run MLflow locally (instructions in TUTORIAL.md) pointing to Postgres (backend) and MinIO (artifacts)

4) Run the example training script
- See `examples/` for a minimal sklearn model that logs to MLflow

5) View
- MLflow UI: http://localhost:5000
- MinIO Console: http://localhost:9001 (user: from `.env`)

## Structure
- docker-compose.yml — Postgres + MinIO (artifact store) for local dev
- .env.example — Environment variables for local stack
- TUTORIAL.md — Full beginner → advanced → expert walkthrough
- examples/ — Minimal training script + requirements to log to MLflow
- Templates-&-Checklists.md — Copy-ready templates for PRs, model cards, runbooks
- Troubleshooting.md — Fix common issues (ports, creds, buckets, paths)

## Table of Contents
- [TUTORIAL — Beginner → Advanced → Expert](./TUTORIAL.md)
- [00 — Foundations & Concepts](./00-Foundations-&-Concepts.md)
- [01 — Local Stack Setup (Postgres & MinIO)](./01-Local-Stack-Setup-Postgres-&-MinIO.md)
- [02 — MLflow Server & Tracking](./02-MLflow-Server-&-Tracking.md)
- [03 — Logging Metrics, Params, and Artifacts](./03-Logging-Metrics-Params-&-Artifacts.md)
- [04 — Model Registry & Promotion](./04-Model-Registry-&-Promotion.md)
- [05 — Data Contracts & Versioning](./05-Data-Contracts-&-Versioning.md)
- [06 — Automated Evaluations (agentic)](./06-Automated-Evaluations-agentic.md)
- [07 — Pipelines & Orchestration (agentic)](./07-Pipelines-&-Orchestration-agentic.md)
- [08 — Deployment Patterns (batch/online)](./08-Deployment-Patterns-batch-online.md)
- [09 — Monitoring & Drift (agentic triage)](./09-Monitoring-&-Drift-agentic-triage.md)
- [10 — Governance, Security, and SLOs](./10-Governance-Security-&-SLOs.md)
- [11 — Cost & Performance](./11-Cost-&-Performance.md)
- [12 — Release & Change Management](./12-Release-&-Change-Management.md)
- [13 — Capstone & Portfolio](./13-Capstone-&-Portfolio.md)

Links will be added as chapters are created. Start with TUTORIAL.md for the full guided path.
