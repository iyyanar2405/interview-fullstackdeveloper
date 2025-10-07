# 05 — Backlog & Prioritization

Goal: Maintain a high-signal, low-inventory backlog and make trade-offs visible and data-informed.

Backlog quality (DEEP)
- Detailed appropriately: top items have clear acceptance criteria and test notes; lower items are lean
- Estimated: use story points or t-shirt sizes; track historical throughput
- Emergent: add/remove/refactor as we learn; avoid “backlog as graveyard”
- Prioritized: explicit order with rationale

Prioritization methods
- WSJF = (User/Business value + Time criticality + Risk reduction/Opportunity enablement) / Job size
- RICE = (Reach × Impact × Confidence) / Effort; keep scales consistent (e.g., Impact 0.25–3x)
- MoSCoW; Kano; Value vs Risk for workshops and stakeholder alignment

Operational practices
- Definition of Ready (DoR): INVEST, dependencies identified, test notes, UX assets ready
- Definition of Done (DoD): code, tests, docs, security checks, monitoring, deployed
- Splitting patterns (SPIDR): Spike, Path, Interface, Data, Rules; avoid horizontal-only slices
- Regular refinement: time-boxed; prune stale items; cap WIP by capping top-of-backlog size (e.g., <= 2 sprints)

Scoring templates
- WSJF sheet: columns for BV, TC, RR/OE, Size, Score; relative sizing against a baseline
- RICE sheet: Reach (users/period), Impact (scale), Confidence (%), Effort (pts), Score

Example
- Two items: "Self-serve password reset" vs "Refactor payment service".
	- Password reset: BV 8, TC 6, RR 2, Size 5 → WSJF = 3.2
	- Refactor: BV 3, TC 2, RR 8, Size 8 → WSJF = 1.6 → lower priority unless risk is urgent

Anti-patterns
- Everything is P1; priority churn without data; skipping refinement leading to carryovers; conflating urgency with size

Outputs
- Prioritized backlog with rationale notes, DoR/DoD published, and shared scoring sheets.
