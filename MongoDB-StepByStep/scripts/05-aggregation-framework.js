use('ecom_training');

// Order totals by customer
printjson(db.orders.aggregate([
  { $unwind: '$items' },
  { $group: { _id: '$customer_id', total: { $sum: { $multiply: ['$items.quantity', '$items.unit_price'] } } } },
  { $sort: { total: -1 } }
]).toArray());
