USE EcomTraining;
GO

-- View estimated plan (Ctrl+L in SSMS)
SELECT * FROM sales.vw_OrderTotals WHERE OrderTotal > 50;

-- Common operators to watch: Key Lookup, Sort, Hash Match, Nested Loops
-- Tuning exercise: add an index to avoid key lookup on vw_OrderTotals source tables
