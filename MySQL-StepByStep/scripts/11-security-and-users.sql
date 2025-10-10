-- Create user and grant minimal permissions (example)
CREATE USER IF NOT EXISTS 'learner'@'%' IDENTIFIED BY 'changeme';
GRANT SELECT ON ecom_training.* TO 'learner'@'%';
-- Revoke risky perms; grant additional as needed for exercises
