use('ecom_training');

db.setProfilingLevel(1, { slowms: 50 });

// Explain a query
const customer = db.customers.findOne({ email: 'ada@example.com' });
printjson(db.orders.find({ customer_id: customer._id }).sort({ order_date: -1 }).limit(5).explain('executionStats'));
