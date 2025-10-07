# 03 — Generators & Boundaries

Use Nx generators, tagging, and enforce module boundaries.

## Generators
```powershell
npx nx g @nx/angular:component shared-ui:hello --standalone --export
npx nx g @nx/angular:service shared-data-access:api
```

## Tags and boundaries
- Set `tags` in `project.json` (e.g., `scope:shared`, `type:ui`)
- Configure `@nx/enforce-module-boundaries` in `eslint.config.js` to restrict imports

## Dependency graph
```powershell
npx nx graph
```

Next: Linting and formatting.
