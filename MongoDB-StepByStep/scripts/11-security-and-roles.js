// Example (adjust for local/Atlas):
// Create read-only user on ecom_training
use('ecom_training');
db.createUser({ user: 'learner', pwd: 'changeme', roles: [ { role: 'read', db: 'ecom_training' } ] });
