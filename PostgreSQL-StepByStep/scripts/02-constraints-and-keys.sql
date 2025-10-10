\connect ecom_training

-- additional constraints/uniques are already defined in table DDL; here we show example checks
ALTER TABLE IF EXISTS sales.order_items
  ADD CONSTRAINT IF NOT EXISTS chk_unit_price_nonnegative CHECK (unit_price >= 0);
