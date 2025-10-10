USE ecom_training;

-- Index on orders(customer_id)
CREATE INDEX ix_orders_customer_id ON orders(customer_id);

-- Covering index on order_items(order_id, product_id) including quantity, unit_price
CREATE INDEX ix_items_order_product ON order_items(order_id, product_id);
-- MySQL lacks INCLUDE clause; consider wide index or secondary lookup
