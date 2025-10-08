# Agentic MLOps Tutorial — Beginner → Advanced → Expert

This end-to-end tutorial takes you from a clean machine to a local agentic MLOps stack and workflows. You’ll stand up MLflow with Postgres backend and MinIO artifact store, log experiments, use a model registry, author automated evaluations, and add agentic pipelines that curate data, evaluate models, and triage monitoring signals.

## Prereqs
- Docker Desktop (Windows/macOS/Linux)
- Python 3.10+
- PowerShell (on Windows) or a Unix-like shell

## 1) Prepare environment
1. Copy env file
   - Duplicate `.env.example` to `.env` and keep defaults for local use.
2. Start infra (Postgres + MinIO)
   - In this folder, run Docker Compose to start services.

## 2) Start MLflow server
- Option A: Run MLflow server in a separate terminal pointing to Postgres/MinIO (see ENV)
- Tracking URI: http://localhost:5000
- Backend store: postgresql+psycopg2://mlflow:mlflow123@localhost:5432/mlflow
- Artifact store: s3://mlflow (endpoint http://localhost:9000)

## 3) Test run: log a model
1. Create a venv and install requirements in `examples/`
2. Run `train_sklearn.py` to log params/metrics/model to MLflow
3. Open MLflow UI and verify the run in experiment `agentic-mlops-demo`

## 4) Model registry & promotion
1. From MLflow UI, register the best run as a model (e.g., `diabetes-elasticnet`)
2. Add a stage transition policy: `None` → `Staging` → `Production`
3. Define approval requirements and an eval score threshold

## 5) Automated evaluations (agentic)
- Author an eval script that:
  - Pulls candidate model by version
  - Loads curated eval dataset
  - Computes metrics and slices (e.g., by demographic or feature range)
  - Decides pass/fail and writes evidence back to MLflow
- Outcome: CI can auto-gate promotions using eval evidence

## 6) Agentic pipelines (beginner → advanced → expert)
- Beginner:
  - Agent retrieves data schema from source, validates sample batch
  - Agent runs training script with provided hyperparams
  - Agent publishes result URL
- Advanced:
  - Agent curates eval datasets (time-sliced, stratified)
  - Agent compares candidates, writes a decision memo
  - Agent opens a PR with updated model card
- Expert:
  - Agent watches monitoring signals (drift/anomalies)
  - Agent triages alerts, proposes rollback or retrain, drafts incident notes
  - Human-in-the-loop approves/edits via a checklist

## 7) Monitoring & drift
- Define SLIs/SLOs for the service (latency, error rate, predictive quality)
- Create dashboards and alerts; link alert → runbook → comms template
- Capture postmortems and track follow-ups

## 8) Security & governance
- Secrets in env/Key Vault, RBAC on MinIO and registry
- Audit trails for model changes; reproducible builds; provenance

## 9) Cost & performance
- Optimize container layers; pin deps; cache datasets
- Control experiment fan-out; regularly archive stale artifacts

## 10) Capstone
- Pick a small use case (e.g., tabular regression or classification)
- Deliver: model card, eval report, promotion notes, monitoring dashboard, runbooks

Appendix: Troubleshooting has specifics for common port/creds issues.
