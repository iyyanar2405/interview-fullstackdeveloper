# Troubleshooting — Agentic MLOps

- Ports already in use
  - Stop conflicting services; change mapped ports in docker-compose.yml
- MinIO auth errors
  - Verify MINIO_ROOT_USER/PASSWORD; ensure bucket exists (minio-setup service)
- MLflow can’t write artifacts
  - Check MLFLOW_S3_ENDPOINT_URL env and bucket policy; verify creds
- DB connection issues
  - Confirm Postgres is up and POSTGRES_* env match
- SSL or proxy issues
  - For local dev, prefer http endpoints; configure proxies appropriately
- Dependency version conflicts
  - Use the provided requirements; pin and upgrade cautiously
