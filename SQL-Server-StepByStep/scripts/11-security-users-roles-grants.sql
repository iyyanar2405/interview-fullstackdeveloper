USE EcomTraining;
GO

-- Create a read-only user example (SQL Auth demo; use Windows Auth in enterprises)
CREATE LOGIN readonly_login WITH PASSWORD = 'P@ssw0rd!';
CREATE USER readonly_user FOR LOGIN readonly_login;
EXEC sp_addrolemember 'db_datareader', 'readonly_user';
-- Revoke risky perms just in case
REVOKE UPDATE, DELETE ON SCHEMA::sales FROM readonly_user;
