# 13 — Real-world Scenarios

Battle-tested playbooks for common project challenges.

1) Scope creep near release
- Actions: quantify impact, propose trade-offs, seek decision via decision log; adjust plan and comms

2) Cross-team dependency slips
- Actions: escalate early per RACI, negotiate fallback, split scope to reduce coupling, update dependency register

3) Missed timeline with exec pressure
- Actions: publish forecast ranges, show burn-up and constraint analysis; create recovery plan with options

4) Production incident during sprint
- Actions: invoke incident runbook, protect team focus, adjust sprint scope transparently; post-incident review

5) Vendor delay or quality issue
- Actions: enforce SOW terms, activate penalty/bonus clauses, spin up alternative path; weekly checkpoints

6) Stakeholder conflict on priorities
- Actions: run prioritization workshop (WSJF/RICE), align on outcomes; PO decision backed by data

7) Surprise audit request
- Actions: gather evidence from CI/CD, security scans, approvals; share governance map and mitigate gaps

8) Major outage during launch
- Actions: ECAB, stop-the-line, clear customer comms, canary rollback; RCAs and action items tracked in RAID
# 13 — Real-world Scenarios

Short plays you can reuse.

1) Date vs Quality Conflict
- Context: Exec wants fixed date; quality risks high
- Play: Show burn-up and risk heatmap; propose scope trade; secure decision in SteerCo

2) Cross-team Dependency Slips
- Context: Upstream API team delayed
- Play: Mitigation plan; feature flag; staged release; negotiate SLA/OLA

3) Compliance Surprise
- Context: New data residency rule
- Play: DPIA; data map; phased rollout; audit trail and approvals

4) Vendor Outage
- Context: Third-party auth down
- Play: Incident comms; fallback; post-incident RCAs; reliability SLAs in contract
