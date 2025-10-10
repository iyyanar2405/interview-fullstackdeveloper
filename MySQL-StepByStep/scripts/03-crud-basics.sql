USE ecom_training;

-- Inserts (ignore duplicates by unique keys)
INSERT IGNORE INTO customers(first_name, last_name, email) VALUES
('Ada','Lovelace','ada@example.com'),
('Alan','Turing','alan@example.com');

INSERT IGNORE INTO products(sku, name, price) VALUES
('SKU-1','Keyboard',49.99),
('SKU-2','Mouse',19.99);

-- Create an order
INSERT INTO orders(customer_id) VALUES (1);
SET @order_id = LAST_INSERT_ID();
INSERT INTO order_items(order_id, product_id, quantity, unit_price)
VALUES (@order_id, 1, 1, 49.99), (@order_id, 2, 2, 19.99);

-- Basic selects
SELECT * FROM customers;
SELECT * FROM products;
SELECT * FROM orders;
SELECT * FROM order_items;

-- Update
UPDATE customers SET last_name = 'Lovelace-Byron' WHERE customer_id = 1;

-- Delete
DELETE FROM products WHERE sku = 'SKU-2';
