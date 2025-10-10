USE ecom_training;

EXPLAIN SELECT * FROM vw_order_totals WHERE order_total > 50;

-- Look for using index, filesort, temporary; add indexes as needed
