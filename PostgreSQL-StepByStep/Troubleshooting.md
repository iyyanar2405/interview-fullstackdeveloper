# Troubleshooting

- psql connection errors: verify host/port, credentials, pg_hba.conf, service running
- Permission denied: check role grants and search_path
- Slow queries: EXPLAIN (ANALYZE), missing indexes, VACUUM/ANALYZE
- Locking/contention: pg_locks, identify blockers, adjust isolation
