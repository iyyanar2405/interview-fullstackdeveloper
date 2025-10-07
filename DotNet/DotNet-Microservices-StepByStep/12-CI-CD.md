# 12 — CI/CD

Automate build, test, and deploy.

## Build & test
- Restore, build, run unit/integration tests

## Container images
- Build and tag Docker images per service
- Push to registry (GHCR, ACR, Docker Hub)

## Deploy
- Dev: docker compose
- Staging/Prod: Kubernetes with GitOps or GitHub Actions

## GitHub Actions (outline)
- Jobs: build, test, publish images, deploy manifests
- Cache NuGet for speed
