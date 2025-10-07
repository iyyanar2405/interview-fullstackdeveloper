# 14 — Azure Deployment (Nx Angular)

Options to deploy an Angular app built with Nx on Azure.

## 1) Azure Static Web Apps (recommended for SPA)
- Automatic global CDN, SSL, auth, routes

### Build output path
By default: `dist/apps/web`

### GitHub Actions workflow (created by portal)
- App location: `/`
- Output location: `dist/apps/web`
- Api location: leave empty (unless using functions)

### SPA rewrites
Add `/index.html` fallback in `staticwebapp.config.json`:
```json
{
  "navigationFallback": { "rewrite": "/index.html", "exclude": ["/assets/*", "/api/*"] }
}
```

## 2) Azure Storage Static Website (+ CDN optional)
- Build and upload files to `$web` container
- Enable static website hosting

```powershell
npx nx build web --configuration=production
az storage blob upload-batch -s dist/apps/web -d '$web' --account-name <account>
```

## 3) Azure App Service (container or static)
- Serve static files with NGINX container or Node static server
- Configure SPA fallback to `/index.html`

## Tips
- Cache busting via hashed filenames is default
- Use `nx affected --target=build` in CI
- For multi-app workspaces, set the correct output path per app
