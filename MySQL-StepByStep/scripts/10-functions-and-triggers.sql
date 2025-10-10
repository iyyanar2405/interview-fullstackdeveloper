USE ecom_training;

-- Stored function: line total
DROP FUNCTION IF EXISTS line_total;
DELIMITER $$
CREATE FUNCTION line_total(qty INT, price DECIMAL(10,2)) RETURNS DECIMAL(10,2)
DETERMINISTIC
BEGIN
  RETURN qty * price;
END $$
DELIMITER ;

-- Trigger: set unit_price from products on insert
DROP TRIGGER IF EXISTS trg_items_set_unit_price;
DELIMITER $$
CREATE TRIGGER trg_items_set_unit_price
BEFORE INSERT ON order_items
FOR EACH ROW
BEGIN
  SET NEW.unit_price = (SELECT price FROM products WHERE product_id = NEW.product_id);
END $$
DELIMITER ;
