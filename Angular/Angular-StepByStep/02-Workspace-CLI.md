# 02 — Workspace & CLI

Understand Angular workspace layout and core CLI commands.

## Workspace overview
- `src/main.ts` bootstraps the app with standalone APIs
- `src/app/` contains components, services, and routes
- `angular.json` holds build/test configurations
- `tsconfig*.json` TypeScript configs

## CLI basics
```powershell
# generate a standalone component
ng g component features/todo-list --standalone --inline-style --inline-template

# generate a service
ng g service core/todo

# run
ng serve

# build
ng build --configuration production
```

## Script targets
- `ng serve` dev server with HMR
- `ng test` run unit tests (Karma/Jasmine by default)
- `ng e2e` end-to-end (if Cypress/Protractor configured)

Next: Components and templates.
