USE ecom_training;

START TRANSACTION;
UPDATE orders SET status = 'PROCESSING' WHERE order_id = 1;
DO SLEEP(2);
ROLLBACK;

-- Isolation demo: run in two sessions to observe locks
