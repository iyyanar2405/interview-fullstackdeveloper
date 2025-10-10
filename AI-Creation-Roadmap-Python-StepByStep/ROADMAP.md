# AI Creation Roadmap (Python)

## Goals
- Deliver production-grade AI features (quality, latency, cost, safety)
- Python-first stack: FastAPI, LangChain/LangGraph, pgvector/Redis, Ragas/DeepEval, OpenTelemetry, Docker

## Metrics
- Quality: win rate vs baseline, accuracy/F1
- Latency: P95 latency, timeout rate
- Cost: $/1k requests, cache hit-rate
- Safety: PII leak=0, jailbreak block-rate
- Ops: SLO compliance, MTTR

## Reference Architecture
- Ingestion/ETL → Object store + Warehouse
- Indexing: Embeddings → Vector DB (pgvector/Redis/Pinecone)
- Orchestration: LangChain/LangGraph (tools, retries)
- API: FastAPI (rate-limit, auth), workers for batch
- Guardrails: input/output filters, JSON schema validation
- Eval: offline (Ragas/DeepEval) and online feedback
- Observability: traces, prompts/outputs, cost dashboards
- CI/CD: GitHub Actions, tests, eval gates, canary

## Security & Compliance
- Secrets manager (env/Key Vault/Secrets Manager), private networking
- PII/PHI redaction, retention policy, audit trails
- Safety evals, allowlist for web tools

## Milestones (90 days)
- Weeks 0–2: Discovery + Baseline (LLM client, prompt API, eval set v1)
- Weeks 3–4: MVP RAG (indexer, retriever, caching, eval v1)
- Weeks 5–6: Productization (obs, guardrails, CI/CD)
- Weeks 7–8: Advanced RAG & Tools (re-rank, hybrid, function-calling)
- Weeks 9–10: Evaluations & Safety (regression suite, safety tests)
- Weeks 11–12: Scale & Governance (quotas, budgets, model cards)
- Week 13+: Capstone & Handover (SOPs, on-call, deck)

## Risks & Mitigations
- Hallucinations → RAG + citations + guardrails
- Drift → re-index + drift alerts + periodic eval
- Cost spikes → caching + budgets + rate limits
- Data leakage → masking + private networking + reviews
