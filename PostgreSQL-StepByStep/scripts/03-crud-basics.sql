\connect ecom_training

-- Inserts
INSERT INTO sales.customers(first_name, last_name, email) VALUES
('Ada','Lovelace','ada@example.com'),
('Alan','Turing','alan@example.com')
ON CONFLICT (email) DO NOTHING;

INSERT INTO sales.products(sku, name, price) VALUES
('SKU-1','Keyboard',49.99),
('SKU-2','Mouse',19.99)
ON CONFLICT (sku) DO NOTHING;

-- Create an order
WITH new_order AS (
  INSERT INTO sales.orders(customer_id) VALUES (1)
  RETURNING order_id
)
INSERT INTO sales.order_items(order_id, product_id, quantity, unit_price)
SELECT order_id, 1, 1, 49.99 FROM new_order
UNION ALL
SELECT order_id, 2, 2, 19.99 FROM new_order;

-- Basic selects
SELECT * FROM sales.customers;
SELECT * FROM sales.products;
SELECT * FROM sales.orders;
SELECT * FROM sales.order_items;

-- Update
UPDATE sales.customers SET last_name = 'Lovelace-Byron' WHERE customer_id = 1;

-- Delete
DELETE FROM sales.products WHERE sku = 'SKU-2';
