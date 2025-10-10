-- Backup: use pg_dump
-- pg_dump -h localhost -U postgres -d ecom_training -F c -f ecom_training.dump

-- Restore: pg_restore to a new DB
-- createdb -h localhost -U postgres ecom_training_restored
-- pg_restore -h localhost -U postgres -d ecom_training_restored -F c ecom_training.dump
