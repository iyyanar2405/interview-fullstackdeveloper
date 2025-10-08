# Tech Vocabulary Glossary (Architecture, DevOps, Product, Risk & Incident)

Use these definitions and examples to build your Week 2 vocabulary deck.

## Architecture
- Monolith — Single-codebase application handling multiple concerns; simple to deploy, can be hard to scale.
- Microservice — Small, independently deployable service owning a specific capability; improves autonomy but adds ops overhead.
- Latency — Time from request to first byte of response.
- Throughput — Amount of work done per unit time (req/s, msgs/s).
- Scalability — Ability to handle increased load by adding resources.
- Horizontal scaling — Add more instances; vertical: more CPU/RAM.
- Coupling — Degree of dependency between components; aim for low coupling.
- Cohesion — How closely related a module’s responsibilities are; aim for high cohesion.
- API Gateway — Single entry point routing to backend services; adds cross-cutting concerns like auth/rate limiting.
- Circuit breaker — Pattern that stops calls to failing dependencies to prevent cascading failures.
- Idempotency — Operation can be repeated without changing result (e.g., PUT with same body).
- Event-driven — Systems communicating via events (pub/sub) rather than direct calls.
- CQRS — Separate read/write models to optimize performance and scalability.
- Cache — Fast storage layer (in-memory/edge) to reduce latency and backend load.
- Consistency model — How quickly data becomes consistent (strong vs eventual).
- Data partitioning (sharding) — Splitting data across nodes for scale.
- SLA/SLO/SLA breach — Service commitments; SLO is target; SLA is contract.
- Observability — Ability to understand system state via logs/metrics/traces.
- Backpressure — Mechanism to prevent overload by slowing or queuing producers.
- Canary release — Deploy to a small subset first to reduce risk.
- Blue/green — Two identical environments to enable fast switch/rollback.

## DevOps
- CI/CD — Continuous Integration/Continuous Delivery or Deployment.
- Pipeline — Automated steps from commit to deploy (build, test, scan, release).
- Artifact — Built package or image produced by the pipeline.
- Rollout — Gradual deployment; Rollback — Revert to previous version.
- Feature flag — Toggle functionality without redeploying.
- Infrastructure as Code (IaC) — Define infra with declarative files (e.g., Terraform, Bicep).
- Immutable infrastructure — Replace rather than patch running servers.
- Observability stack — Logs, metrics, traces tools (ELK, Prometheus, Grafana, OpenTelemetry).
- SRE — Site Reliability Engineering; applies SW engineering to reliability.
- Error budget — Allowed unreliability within SLOs; informs release pace.
- MTTR/MTBF — Mean Time To Recovery/Between Failures; reliability metrics.
- Secrets management — Secure storage for tokens/keys (Vault, Key Vault, KMS).
- Container registry — Store and distribute container images.
- Orchestrator — System to manage containers (Kubernetes, ECS, AKS, EKS).
- GitOps — Operate infra/apps via Git as the source of truth and controllers.
- Shift left — Perform testing/security earlier in the lifecycle.
- SBOM — Software Bill of Materials; list of components in an artifact.
- Vulnerability scanning — Automated check for known CVEs in images/deps.
- Deployment window/freeze — Allowed time or moratorium for releases.
- Runbook — Step-by-step operational procedure for common tasks/incidents.

## Product
- MVP — Minimum Viable Product; smallest thing to validate value.
- Roadmap — Forward-looking plan of themes/outcomes and time horizons.
- Churn — Customers leaving or canceling.
- Activation — First value moment; user completes key action.
- Retention — Users continue to use product over time.
- North Star Metric — Single metric representing product value delivered.
- Persona — Archetype representing a user segment.
- Job To Be Done (JTBD) — What users aim to accomplish; context over features.
- A/B testing — Experiment with variants to measure impact.
- Cohort analysis — Group users by start time or attribute to track behavior.
- CAC/LTV — Customer Acquisition Cost / Lifetime Value.
- Funnel — Steps from awareness to conversion; drop-off analysis.
- Segmentation — Grouping users by attributes/behavior for targeting.
- Backlog — Ordered list of work items; emergent and prioritized.
- WSJF — Weighted Shortest Job First prioritization method.
- RICE — Prioritization: Reach × Impact × Confidence ÷ Effort.
- MoSCoW — Must/Should/Could/Won’t have prioritization.
- NPS — Net Promoter Score; measures willingness to recommend.
- Changelog — Record of changes shipped (user-facing).
- Feature parity — Matching competitor/legacy functionality to migrate.

## Risk & Incident
- Risk — Uncertain event that could affect objectives (probability × impact).
- Issue — Realized risk/problem requiring action.
- Severity (Sev1–Sev4) — Business/user impact level (P1–P4 similar).
- Impact — Who/what is affected and how badly.
- Likelihood/Probability — Chance the risk occurs.
- Mitigation — Steps to reduce likelihood/impact of a risk.
- Contingency — What you do if the risk happens.
- Trigger — Early warning signal that a risk is about to occur.
- Root cause — Fundamental reason an incident happened.
- RCA — Root Cause Analysis; structured post-incident document.
- MTTR — Mean Time To Recovery; time to restore service.
- Incident Commander (IC) — Role coordinating response.
- Scribe/Comms lead — Roles capturing timeline and external comms.
- Status page — Public updates during customer-facing incidents.
- Blameless postmortem — Learning-focused incident review.
- Runbook/Playbook — Pre-defined steps to handle incidents.
- Escalation path — Who to call and in what order.
- SLAs/SLOs — Commitments/targets that shape incident priority.
- Change freeze — Temporary halt to deployments to stabilize.
- CAPA — Corrective and Preventive Actions after incidents.

## How to practice
- Add terms to Anki/Quizlet with your own examples.
- Use them in daily deliverables (tickets, emails, ADRs).
- Teach one term/day to a teammate to solidify understanding.
