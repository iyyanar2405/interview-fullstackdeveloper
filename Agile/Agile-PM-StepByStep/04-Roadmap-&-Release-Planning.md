# 04 — Roadmap & Release Planning

Purpose: Communicate where we’re going and how we’ll deliver value incrementally while managing dependencies and risk.

Success criteria
- Roadmap approved by Sponsor and PO, shared with team and stakeholders
- Each release has clear objectives, scope boundaries, and go/no-go criteria
- Dependencies identified with owners and mitigation dates

Roadmap principles
- Outcome-based: phrase as “Reduce onboarding time from 3 days to 1 hour” rather than “Build onboarding module”
- Horizons: Now (0–3 mo), Next (3–6), Later (6–12) to express certainty levels
- Uncertainty bands: date ranges or confidence levels (e.g., 60–80%) instead of hard dates when appropriate
- Optionality: keep strategic options open (A/B approaches) until data narrows the choice

Roadmap structure
- Theme -> Outcome -> Key metrics -> Candidate epics -> Risks/assumptions
- Visual: simple swimlane or Now/Next/Later grid; mark dependencies and confidence

Release planning
- Tranches: MVP → MMP → GA → Hardening; each tranche maps to measurable outcomes
- Vertical slices: aim for user-visible increments; reserve technical enablers only when they unlock significant value/risk reduction
- Dependency map: list internal (teams/components) and external (vendors, compliance windows) dependencies; assign owners
- Go/No-Go criteria: quality gates (test pass rate, defect thresholds), risk acceptance, operational readiness (runbooks, monitoring), legal/compliance approvals
- Rollback plan: versioning, data migration reversibility, feature flags

Templates
- Release one-pager
	- Objective, Scope (in/out), Metrics, Dependencies, Risks, Go/No-Go, Rollback, Owners, Target window
- Dependency register
	- Dependency, Type (internal/external), Owner, Needed by, Status, Mitigation

Real-world scenario
- A payment provider migration requires PCI sign-off and vendor certification.
- PM sequences work: sandbox integration (MVP), pilot with feature flag (MMP), full cutover (GA).
- Adds explicit go/no-go: PCI attestation received; chargeback rate < 0.5% in pilot; monitoring dashboards validated.
- Outcome: Cutover completed within target window; rollback not needed; post-release review captures learnings for future vendor swaps.

Artifacts
- Roadmap view, release plan documents, dependency register, go/no-go checklist stored in the project wiki.
