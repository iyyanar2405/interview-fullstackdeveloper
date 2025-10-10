-- Run on master for backups path or use default instance settings
DECLARE @BackupPath NVARCHAR(260) = 'C:\\Backups';

-- FULL backup (adjust path)
BACKUP DATABASE EcomTraining TO DISK = @BackupPath + '\\EcomTraining_full.bak' WITH INIT, STATS = 5;

-- Maintenance notes: UPDATE STATISTICS, index rebuild/reorg via Ola Hallengren scripts (external)
