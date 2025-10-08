# Workflow 04 — Webhook → Jira/Trello Ticket

Goal: Receive a webhook and create a task in Jira or Trello based on payload.

Steps
1) HTTP Trigger node
- Method: POST; Path: /task

2) Set node
- Normalize fields: summary, details, priority, labels

3) IF node
- If system == "jira" → Jira node (Create Issue)
- Else → Trello node (Create Card)

4) Jira/Trello nodes
- Auth via OAuth/API keys; map fields accordingly

5) Respond node (HTTP Response)
- 200 with created task ID

Export JSON and store here.
