# Troubleshooting — SSAS/MDX

- Processing fails due to key errors: enable UnknownMember and review foreign keys
- Slow queries: set attribute relationships; add aggregations; reduce calculated members on large axes
- Security issues: verify role filters and test with SSMS `Change User` feature
- Deployment errors: check target instance version/compat; rebuild deployment script
