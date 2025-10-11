# Day 59 — Deployment

## Objectives
- Build and deploy React + PrimeReact app
- Configure environment variables and base path
- Optimize assets and cache headers

## Steps
- Set homepage/base path if deploying to subfolder
- Build with your bundler (Vite/CRA) and preview locally
- Configure server to serve index.html for SPA routes
- Add proper cache headers for static assets

## Vite Example
- Set base in vite.config.ts if needed: base: '/app/'
- Run build and deploy dist/ to static hosting

## Environment Variables
- VITE_API_URL, VITE_ENV = 'production'
- Avoid leaking secrets into client

## Checklist
- [ ] Production build succeeds
- [ ] SPA rewrite rules in place
- [ ] Env vars documented
- [ ] Basic uptime monitoring set