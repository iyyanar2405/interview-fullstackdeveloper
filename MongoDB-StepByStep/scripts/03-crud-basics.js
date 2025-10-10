use('ecom_training');

// Inserts
db.customers.updateOne({ email: 'ada@example.com' }, { $setOnInsert: { first_name: 'Ada', last_name: 'Lovelace', email: 'ada@example.com', created_at: new Date() } }, { upsert: true });
db.customers.updateOne({ email: 'alan@example.com' }, { $setOnInsert: { first_name: 'Alan', last_name: 'Turing', email: 'alan@example.com', created_at: new Date() } }, { upsert: true });

db.products.updateOne({ sku: 'SKU-1' }, { $setOnInsert: { sku: 'SKU-1', name: 'Keyboard', price: 49.99, created_at: new Date() } }, { upsert: true });
db.products.updateOne({ sku: 'SKU-2' }, { $setOnInsert: { sku: 'SKU-2', name: 'Mouse', price: 19.99, created_at: new Date() } }, { upsert: true });

// Create an order with items (embedding items for simplicity)
const customerDoc = db.customers.findOne({ email: 'ada@example.com' });
const order = {
  customer_id: customerDoc._id,
  order_date: new Date(),
  status: 'NEW',
  items: [
    { sku: 'SKU-1', quantity: 1, unit_price: 49.99 },
    { sku: 'SKU-2', quantity: 2, unit_price: 19.99 }
  ]
};
const res = db.orders.insertOne(order);
print('Inserted order id:', res.insertedId);

// Basic finds
printjson(db.customers.find().toArray());
printjson(db.products.find().toArray());
printjson(db.orders.find().toArray());

// Update
db.customers.updateOne({ email: 'ada@example.com' }, { $set: { last_name: 'Lovelace-Byron' } });

// Delete
db.products.deleteOne({ sku: 'SKU-2' });
