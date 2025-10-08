# Troubleshooting — n8n

Common issues and fixes.

- Cannot reach http://localhost:5678
  - Check containers: `docker ps`
  - Logs: `docker compose logs n8n`
  - Port in use? Change N8N_PORT in .env

- Webhook not receiving
  - Ensure WEBHOOK_URL matches public URL if behind tunnel/ngrok
  - In-editor test URL works only while editor is open

- OAuth redirects failing
  - Add correct redirect URLs in provider app settings (http://localhost:5678/rest/oauth2-credential/callback)

- Data lost after restart
  - Ensure volumes exist and are mounted (n8n_data, pg_data)

- Slack/Jira errors
  - Re-auth credentials, verify scopes/permissions

- High CPU/memory
  - Limit executions saved, clean old executions, increase resources
