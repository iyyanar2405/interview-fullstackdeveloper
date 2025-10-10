USE ecom_training;

-- Additional checks/uniques already defined; example extra check
ALTER TABLE order_items
  ADD CONSTRAINT chk_unit_price_nonnegative CHECK (unit_price >= 0);
