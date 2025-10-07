# 06 — E2E testing

Configure E2E with Cypress or Playwright in Nx.

## Cypress (default)
```powershell
npx nx g @nx/angular:application web
npx nx g @nx/angular:cypress-project web-e2e --project=web
npx nx e2e web-e2e
```

## Playwright (alternative)
```powershell
npm i -D @nx/playwright
npx nx g @nx/playwright:configuration web
npx nx e2e web-e2e
```

- E2E project lives under `apps/web-e2e`
- Targets available via `project.json`

Refer to Nx E2E docs for latest options and CI usage.
