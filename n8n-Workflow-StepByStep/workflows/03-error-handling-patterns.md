# Workflow 03 — Error Handling Patterns

Goal: Demonstrate Try/Catch branches, error triggers, and notifications.

Patterns
- Error Trigger workflow to catch any workflow errors and notify
- Try/Catch using If + Merge to handle fallback logic
- On failure: collect metadata (workflow name, node, error) and notify via Slack/Email

Steps
1) Create Error Trigger workflow
- Add Slack or Email node with a formatted message including {{$json.message}}

2) In a sample workflow, add risky HTTP Request
- Use If node to branch on statusCode >= 400
- Fallback path posts an alert, success path proceeds

3) Centralize notification format in a Set node

Export examples for reuse.
