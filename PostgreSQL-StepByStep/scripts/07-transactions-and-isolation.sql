\connect ecom_training

BEGIN;
UPDATE sales.orders SET status = 'PROCESSING' WHERE order_id = 1;
-- Simulate work
DO $$ BEGIN PERFORM pg_sleep(2); END $$;
ROLLBACK;

-- Isolation demo: run in two sessions to observe locks and isolation levels
