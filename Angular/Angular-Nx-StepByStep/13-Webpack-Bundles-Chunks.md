# 13 — Webpack, Bundles, and Chunks (Nx Angular)

Understand bundling in Angular (via Webpack under the hood), code-splitting, and how to analyze bundle size in an Nx workspace.

Note: Angular uses the Angular CLI builder which wraps Webpack. You typically configure behavior through Angular CLI options rather than direct webpack.config.js.

## Code-splitting with lazy routes
```ts
export const routes: Routes = [
  {
    path: 'admin',
    loadChildren: () => import('./admin/routes').then(m => m.ADMIN_ROUTES)
  }
];
```
This creates a separate chunk for the admin feature.

## Standalone lazy components
```ts
{
  path: 'chart',
  loadComponent: () => import('@my-org/shared-ui').then(m => m.SalesChartComponent)
}
```

## Build and analyze
```powershell
npx nx build web --configuration=production --stats-json
```

### Webpack Bundle Analyzer
```powershell
npm i -D webpack-bundle-analyzer
npx webpack-bundle-analyzer dist/apps/web/stats.json
```

Or use source-map-explorer:
```powershell
npm i -D source-map-explorer
npx source-map-explorer "dist/apps/web/**/*.js"
```

## Budgets (alerts on size)
Configure budgets in `project.json` or angular.json under build options:
```json
"budgets": [
  { "type": "bundle", "name": "main", "maximumWarning": "500kb", "maximumError": "1mb" },
  { "type": "initial", "maximumWarning": "1mb", "maximumError": "2mb" }
]
```

## Cache-busting and long-term caching
- Angular CLI uses content hashing in filenames by default in production
- Keep vendor and app code split for better cache efficiency

## Nx tips
- Use libraries for clear boundaries; lazy-load heavy features
- Check the dependency graph to avoid oversized shared deps
- Prefer standalone, tree-shakeable components
