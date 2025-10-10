// Connect to mongosh with: mongosh "mongodb://localhost:27017"
// Then: load('MongoDB-StepByStep/scripts/01-create-db-and-collections.js')

const dbName = 'ecom_training';
use(dbName);

// Helper to create a collection only if it doesn't exist
function ensureCollection(name, options = {}) {
	const exists = db.getCollectionNames().includes(name);
	if (!exists) {
		db.createCollection(name, options);
		print(`Created collection: ${name}`);
	} else {
		print(`Collection exists: ${name}`);
	}
}

// Collections
ensureCollection('customers');
ensureCollection('products');
ensureCollection('orders');
ensureCollection('order_items');

// Useful indexes (unique creation is idempotent)
db.customers.createIndex({ email: 1 }, { name: 'ux_customers_email', unique: true });
db.products.createIndex({ sku: 1 }, { name: 'ux_products_sku', unique: true });
