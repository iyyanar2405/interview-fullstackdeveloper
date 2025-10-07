# 02 — Workspace, App & Libs

Generate an Angular app and feature libraries with Nx.

## Generate Angular app
```powershell
npx nx g @nx/angular:application web --routing --style=scss --standalone
npx nx serve web
```

## Generate libraries
```powershell
# domain-driven grouping
npx nx g @nx/angular:library shared-ui --standalone
npx nx g @nx/angular:library shared-data-access
npx nx g @nx/angular:library features-todos --routing --standalone
```

## Import a lib in the app
- Export a component from `shared-ui`
- Add it to `web` app imports and template

Next: Generators, tagging, and enforcing boundaries.
