# 01 — Setup Nx

Install Nx and create a workspace for Angular development.

## Install Nx
```powershell
npm i -g nx@latest
nx --version
```

## Create a workspace
```powershell
npx create-nx-workspace@latest my-org --preset=apps
cd my-org
```

Add Angular capabilities:
```powershell
npm i -D @nx/angular@latest
```

Verify Nx is working:
```powershell
npx nx graph
```

Next: Generate an Angular app and libraries.
