# Day 59 — Deployment & Production Build

Ship your Angular + PrimeNG app with confidence. Create optimized builds, set base paths, and serve via a static web server or Docker.

## Learning objectives

- Produce optimized production builds
- Configure base href for subpath deployments
- Serve SPA with correct fallback routing

## 1) Production build

```powershell
ng build --configuration production
```

Notes:
- Minification, optimization, and budgets apply
- Verify output under `dist/<project-name>/`

## 2) Base paths

If deploying under a subpath like `/app/`, set base href:

```powershell
ng build --configuration production --base-href /app/
```

Or configure in `index.html`:
```html
<base href="/app/" />
```

## 3) Static hosting (Nginx example)

Minimal SPA config:

```
location / {
  try_files $uri $uri/ /index.html;
}
```

## 4) Environment configuration

- Use `fileReplacements` in `angular.json` for prod envs
- Store API endpoints in environment files

## 5) Optional: Docker (Nginx)

```
# Dockerfile
FROM nginx:alpine
COPY dist/my-primeng-app /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
```

## 6) Post-deploy checks

- Inspect network for 200/304 responses and cache headers
- Confirm lazy-loaded chunks resolve correctly
- Validate deep links load (SPA fallback)

## Checklist

- [ ] Built with production config
- [ ] Base href matches deployment path
- [ ] Server fallback to index.html configured
- [ ] Lazy chunks reachable in production

## Key takeaways

- Ensure correct base paths and SPA fallback to avoid 404s on refresh
- Production testing is essential before go-live

## Next day

Build your final capstone project to showcase end-to-end mastery.
