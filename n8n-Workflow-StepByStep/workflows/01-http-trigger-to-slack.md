# Workflow 01 — HTTP Trigger → Slack Notification

Goal: Receive a JSON payload via webhook and post a formatted message to Slack.

Steps
1) Create workflow with HTTP Trigger node
- Method: POST; Path: /notify
- Test URL will be visible in node; production uses WEBHOOK_URL

2) Add Set node (optional)
- Map fields: title, severity, description from incoming JSON

3) Add Slack node (Post message)
- Auth: OAuth2 (create Slack app/bot beforehand)
- Channel: choose test channel
- Message: Use expressions like `{{$json.title}}` and blocks if needed

4) Test
- Send sample POST to webhook test URL with JSON body
- Verify Slack message formatting

Export
- After finishing, click Export → Download to JSON and save in this folder.

Sample payload
```
{
  "title": "Service down",
  "severity": "high",
  "description": "Checkout API returning 500s"
}
```
