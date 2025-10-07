# 12 — Build & Deployment

Build for production and deploy.

## Build
```powershell
ng build --configuration production
```

## Environments
- Use `fileReplacements` in `angular.json` for env-specific settings

## Deployment targets
- Static hosting (NGINX, Firebase Hosting, S3+CloudFront)
- Dockerize the built app and serve with NGINX

## SSR (optional)
- Consider Angular SSR for SEO and faster TTFB on content-heavy pages
