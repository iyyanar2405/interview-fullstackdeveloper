\connect ecom_training

EXPLAIN (ANALYZE, BUFFERS)
SELECT * FROM sales.vw_order_totals WHERE order_total > 50;

-- Inspect nodes: Seq Scan, Index Scan/Only Scan, Hash Join, Sort
-- Tuning exercise: add/select indexes to reduce cost and improve plan
