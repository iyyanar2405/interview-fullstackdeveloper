# 06 — Frontend Integration (React/Next), CORS & Auth (Days 43–49)

Goal: Make backend-frontend collaboration fast and low-friction with explicit contracts.

Day 43: API contract for UI
- Define request/response shapes with examples (happy/error)
- Artifact: UI API contract doc (JSON examples + error codes)

Day 44: CORS matrix
- Origins, methods, headers, credentials, cache TTL
- Artifact: CORS configuration matrix + env overrides

Day 45: Auth flows
- SPA login, token storage, refresh, logout, silent renew
- Artifact: Sequence diagrams (OAuth2/OIDC) + code snippets

Day 46: Error/UI states
- Validation, 401/403/429/5xx patterns; retry/backoff guidance
- Artifact: UI error handling playbook + UX copy blocks

Day 47: Versioned schemas
- Additive changes, feature flags, fallbacks for older clients
- Artifact: Compatibility guide + sample deprecation notice

Day 48: E2E smoke path
- Define 3–5 critical flows; data setup/cleanup notes
- Artifact: E2E smoke script (steps + expected results)

Day 49: Retro
- What removes handoffs and rework? What still blocks velocity?
- Artifact: Actions + owners
