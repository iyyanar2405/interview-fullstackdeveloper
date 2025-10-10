USE EcomTraining;
GO

BEGIN TRAN;
UPDATE sales.Orders SET Status = 'PROCESSING' WHERE OrderId = 1;
-- Simulate some work
WAITFOR DELAY '00:00:02';
ROLLBACK TRAN;

-- Isolation demo (run in two sessions to observe locks)
-- SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
-- BEGIN TRAN; SELECT * FROM sales.Orders WITH (HOLDLOCK) WHERE OrderId = 1; -- Session A
-- UPDATE sales.Orders SET Status='CLOSED' WHERE OrderId=1; -- Session B will block until A commits/rolls back
