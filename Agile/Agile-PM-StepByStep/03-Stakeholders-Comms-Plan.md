# 03 — Stakeholders & Communications Plan

Purpose: Identify who matters, how to engage them, and how information will flow. Result is a living plan that reduces surprises and accelerates decisions.

Success criteria
- Stakeholder map approved by Sponsor and Product Owner
- RACI agreed for at least: scope change, budget change, release go/no-go, incident comms
- Cadence calendar published; status template used for two consecutive cycles
- Escalation path documented and tested once (tabletop)

Key artifacts
- Stakeholder register (owner, role, influence/interest, expectations, preferred channel)
- Influence–interest grid and engagement strategy
- RACI matrix for key decisions
- Communications calendar and status report template

Stakeholder mapping
- Identify roles: Sponsor, PO, SM, Architecture, Security, Legal/Compliance, Ops, Support, Finance, Vendors, End-users
- Assess influence (decision power) and interest (impact/attention). Use a 1–5 scale.
- Strategy by quadrant:
	- High influence / High interest: manage closely (1:1s, steering, early previews)
	- High influence / Low interest: keep satisfied (concise monthly brief, decisions only)
	- Low influence / High interest: keep informed (demos, newsletters)
	- Low influence / Low interest: monitor (milestone notes)

RACI for key decisions
- Scope change > x story points or > y% budget
- Release go/no-go
- Incident P1 communication
- Vendor contract changes
Example roles: Sponsor (A), PO (A/R for scope), PM (R), Tech Lead (C/R for release), Security (C), Ops (R for incident comms), Legal (C), Finance (C), SM (C)

Communications plan
- Cadences
	- Weekly: status report (traffic-light), risks/RAID, burndown/burn-up, decisions needed
	- Bi-weekly: demo + feedback notes distributed within 24h
	- Monthly: steering committee (KPIs, budget, roadmap variances)
	- Ad hoc: incident comms (P1 within 30 min, P2 within 2 hours)
- Channels
	- Email summaries for executives; Teams/Slack for working threads; dashboards (Power BI/Jira) for live metrics
	- Confluence/SharePoint as the system of record; meeting notes within 24h
- Escalation
	- Issue owner -> PM -> Sponsor; response time targets documented by priority

Templates
- Weekly status (one-page)
	- Overall RAG: Green/Amber/Red; last week highlights; next week plan
	- Scope: changes approved/pending; Schedule: milestones; Budget: burn vs plan
	- Risks/Issues: top 5 with owners and due dates
	- Decisions needed: 1–3 bullets
- Decision log
	- ID, Decision, Date, Options considered, Outcome, Owner, Next steps
- Stakeholder register columns
	- Name, Org/Role, Influence (1–5), Interest (1–5), Expectations, Preferred channel, Engagement plan, Notes

Real-world scenario
- Situation: Security requests a new control two weeks before release; risk to timeline.
- Action: PM convenes PO, Security, Tech Lead; prepares options: (A) delay 1 week, (B) scope trade-off of two stories, (C) risk acceptance with compensating control.
- Communication: Briefing sent to Sponsor/SteerCo with RAG shift to Amber; decision meeting scheduled within 48h; decision logged; comms to wider stakeholders via weekly status and demo note.
- Outcome: Option B approved; release on time; decision and follow-up audit actions tracked in RAID.

Checklist
- Stakeholder register complete and reviewed
- RACI baselined and published
- Cadence calendar visible to team and execs
- Status and decision templates adopted in repo/wiki
- Escalation contacts verified and tested

Outputs
- Stakeholder map, RACI, comms calendar, status and decision templates; links added to project wiki.
