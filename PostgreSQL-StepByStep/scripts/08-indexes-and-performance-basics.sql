\connect ecom_training

-- Index on orders(customer_id)
CREATE INDEX IF NOT EXISTS ix_orders_customer_id ON sales.orders(customer_id);

-- Covering-like index using INCLUDE (Postgres 11+)
CREATE INDEX IF NOT EXISTS ix_order_items_order_product ON sales.order_items(order_id, product_id) INCLUDE (quantity, unit_price);

-- VACUUM/ANALYZE hints: run ANALYZE after bulk loads; autovacuum handles routine maintenance
