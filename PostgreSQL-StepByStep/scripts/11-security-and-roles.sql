\connect ecom_training

-- Create read-only role and user example
DO $$ BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'readonly') THEN
    CREATE ROLE readonly NOINHERIT;
  END IF;
END $$;

GRANT CONNECT ON DATABASE ecom_training TO readonly;
GRANT USAGE ON SCHEMA sales TO readonly;
GRANT SELECT ON ALL TABLES IN SCHEMA sales TO readonly;
ALTER DEFAULT PRIVILEGES IN SCHEMA sales GRANT SELECT ON TABLES TO readonly;

-- Create a user and grant role (example only)
-- CREATE USER learner WITH PASSWORD 'changeme';
-- GRANT readonly TO learner;
