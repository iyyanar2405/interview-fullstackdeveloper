use('ecom_training');

// Referencing example: order_items separate collection referencing order_id
// (For high cardinality and frequent updates.)

db.createCollection('order_items_ref');

const order = db.orders.findOne();
db.order_items_ref.insertMany([
  { order_id: order._id, sku: 'SKU-1', quantity: 1, unit_price: 49.99 },
  { order_id: order._id, sku: 'SKU-2', quantity: 2, unit_price: 19.99 }
]);
