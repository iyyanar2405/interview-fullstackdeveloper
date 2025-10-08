# Templates & Checklists — n8n

## Workflow README Template
- Goal
- Trigger(s)
- Inputs/Outputs
- Auth/Env requirements
- Steps
- Error handling
- Export/Import notes

## Connection/Auth Checklist
- OAuth/App credentials created
- Redirect URLs configured
- Tokens tested
- Secrets stored securely (env)

## Testing Checklist
- Unit payload cases
- Happy path and failures
- Rate-limit/backoff behavior
- Idempotency checks

## Observability
- Enable execution save
- Error Trigger workflow set up
- Notifications wired

## Deployment
- .env configured
- Volumes mounted
- Backups (DB and .n8n folder)
- Update process documented
