USE ecom_training;

-- Top products by revenue
SELECT p.name, SUM(oi.quantity * oi.unit_price) AS revenue
FROM order_items oi
JOIN products p ON p.product_id = oi.product_id
GROUP BY p.name
ORDER BY revenue DESC
LIMIT 5;

-- Cohort by order month
SELECT DATE_FORMAT(o.order_date, '%Y-%m') AS order_month, COUNT(DISTINCT o.customer_id) AS active_customers
FROM orders o
GROUP BY order_month
ORDER BY order_month;
