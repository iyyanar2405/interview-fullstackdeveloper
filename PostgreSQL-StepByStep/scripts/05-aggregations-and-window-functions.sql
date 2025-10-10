\connect ecom_training

-- Aggregations
SELECT o.customer_id, SUM(oi.quantity * oi.unit_price) AS total
FROM sales.orders o
JOIN sales.order_items oi ON oi.order_id = o.order_id
GROUP BY o.customer_id
HAVING SUM(oi.quantity * oi.unit_price) > 50;

-- Window functions
SELECT o.order_id,
       SUM(oi.quantity * oi.unit_price) OVER (PARTITION BY o.customer_id ORDER BY o.order_id ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS running_total
FROM sales.orders o
JOIN sales.order_items oi ON oi.order_id = o.order_id;
