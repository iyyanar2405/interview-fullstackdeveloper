USE ecom_training;

-- Joins
SELECT o.order_id, c.first_name, c.last_name, oi.quantity, p.name, oi.unit_price
FROM orders o
JOIN customers c ON c.customer_id = o.customer_id
JOIN order_items oi ON oi.order_id = o.order_id
JOIN products p ON p.product_id = oi.product_id;

-- Set ops
SELECT email FROM customers
UNION
SELECT 'guest@example.com';
