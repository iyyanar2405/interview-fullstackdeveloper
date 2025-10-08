# 11 — Performance: Async, Caching, Tasks & Cost (Days 78–84)

Goal: Raise throughput and lower cost with measured, reversible optimizations.

Day 78: Profiling plan
- CPU, memory, I/O; representative workloads; budgets
- Artifact: Profiling checklist + target budgets

Day 79: Async/concurrency
- FastAPI concurrency, Django async views, thread/process pools
- Artifact: Concurrency model memo + safety checklist

Day 80: Caching
- HTTP (ETag/Cache-Control), Redis key design, invalidation rules
- Artifact: Caching strategy doc + key naming conventions

Day 81: Background tasks/queues
- Celery/RQ/Arq; retries, DLQ, idempotency keys
- Artifact: Task design guide + retry/backoff policy

Day 82: Database performance
- Indexes, N+1, pagination, connection pooling
- Artifact: DB tuning playbook + checklist

Day 83: Cost awareness
- Hot paths, egress, storage tiers; perf vs cost tradeoffs
- Artifact: Cost plan + monitoring queries

Day 84: Retro
- Perf report (before/after); next experiments
- Artifact: Report + actions
