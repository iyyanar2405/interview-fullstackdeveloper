# 01 — Setup & Tooling

Get your environment ready and create a new Angular app using standalone APIs.

## Prerequisites
- Node.js LTS (18+)
- npm (bundled with Node)
- Angular CLI (globally)

## Install and verify
```powershell
node -v
npm -v
npm i -g @angular/cli
ng version
```

## Create a new app (standalone)
```powershell
ng new my-app --standalone --routing --style=scss
cd my-app
ng serve
```

Open http://localhost:4200/

## VS Code extensions (optional)
- Angular Language Service
- ESLint
- Prettier

Next: Understand the workspace and CLI essentials.
