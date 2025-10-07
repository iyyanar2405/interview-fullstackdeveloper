# 06 — CI/CD & Branching

## Branching models
- Trunk-based (preferred): short-lived feature branches, small PRs, feature flags
- Git Flow (when releases are complex): develop, release, hotfix branches

## CI gates
- Build, lint, unit tests, coverage threshold, vulnerability scan
- PR checks: status required before merge

## Release
- SemVer tags, changelog, automated release notes
- Feature flags for gradual rollout

Tooling: GitHub Actions, Azure DevOps, GitLab CI.
