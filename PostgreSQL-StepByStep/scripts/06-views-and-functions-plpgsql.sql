\connect ecom_training

-- View: order totals
CREATE OR REPLACE VIEW sales.vw_order_totals AS
SELECT o.order_id, o.customer_id, SUM(oi.quantity * oi.unit_price) AS order_total
FROM sales.orders o
JOIN sales.order_items oi ON oi.order_id = o.order_id
GROUP BY o.order_id, o.customer_id;

-- Function: create order and return id
CREATE OR REPLACE FUNCTION sales.create_order(p_customer_id INT)
RETURNS INT LANGUAGE plpgsql AS $$
DECLARE v_order_id INT;
BEGIN
  INSERT INTO sales.orders(customer_id) VALUES(p_customer_id) RETURNING order_id INTO v_order_id;
  RETURN v_order_id;
END;$$;
