\connect ecom_training

-- Top products by revenue
SELECT p.name, SUM(oi.quantity * oi.unit_price) AS revenue
FROM sales.order_items oi
JOIN sales.products p ON p.product_id = oi.product_id
GROUP BY p.name
ORDER BY revenue DESC
LIMIT 5;

-- Cohort by order month
SELECT to_char(o.order_date, 'YYYY-MM') AS order_month, COUNT(DISTINCT o.customer_id) AS active_customers
FROM sales.orders o
GROUP BY 1
ORDER BY 1;
