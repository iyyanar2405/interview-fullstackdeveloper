use('ecom_training');

// Compound index for common query pattern
db.orders.createIndex({ customer_id: 1, order_date: -1 });

// Query: latest orders for a customer
const customer = db.customers.findOne({ email: 'ada@example.com' });
printjson(db.orders.find({ customer_id: customer._id }).sort({ order_date: -1 }).limit(5).toArray());
