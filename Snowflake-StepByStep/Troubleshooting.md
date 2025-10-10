# Troubleshooting

- Cannot PUT files: ensure SnowSQL has access to local path; use absolute paths
- COPY INTO errors: check file format, header rows, NULL handling; use VALIDATION_MODE=RETURN_ERRORS
- Permissions: verify role grants for warehouse, db, schema, and stages
- Time Travel queries: ensure retention period covers your timestamp
- Materialized views: be aware of edition/credit costs and refresh behavior
