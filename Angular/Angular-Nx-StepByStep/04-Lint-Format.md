# 04 — Lint & Format

Set up ESLint and formatters with Nx targets.

## Lint
```powershell
npx nx lint web
npx nx lint shared-ui
```

Configure rules in `eslint.config.js` and enable `@nx/enforce-module-boundaries`.

## Format
```powershell
npx nx format:write
npx nx format:check
```

Next: Unit testing with Jest.
