\connect ecom_training

-- Trigger function to set unit_price from products on insert
CREATE OR REPLACE FUNCTION sales.trg_set_unit_price()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
BEGIN
  NEW.unit_price := (SELECT price FROM sales.products WHERE product_id = NEW.product_id);
  RETURN NEW;
END;$$;

DROP TRIGGER IF EXISTS trg_order_items_set_unit_price ON sales.order_items;
CREATE TRIGGER trg_order_items_set_unit_price
BEFORE INSERT ON sales.order_items
FOR EACH ROW EXECUTE FUNCTION sales.trg_set_unit_price();
