USE ecom_training;

-- View: order totals
DROP VIEW IF EXISTS vw_order_totals;
CREATE VIEW vw_order_totals AS
SELECT o.order_id, o.customer_id, SUM(oi.quantity * oi.unit_price) AS order_total
FROM orders o
JOIN order_items oi ON oi.order_id = o.order_id
GROUP BY o.order_id, o.customer_id;

-- Stored procedure: create order
DROP PROCEDURE IF EXISTS create_order;
DELIMITER $$
CREATE PROCEDURE create_order(IN p_customer_id INT)
BEGIN
  INSERT INTO orders(customer_id) VALUES(p_customer_id);
  SELECT LAST_INSERT_ID() AS new_order_id;
END $$
DELIMITER ;
