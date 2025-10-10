use('ecom_training');

// Top products by revenue (from embedded items)
printjson(db.orders.aggregate([
  { $unwind: '$items' },
  { $group: { _id: '$items.sku', revenue: { $sum: { $multiply: ['$items.quantity', '$items.unit_price'] } } } },
  { $sort: { revenue: -1 } },
  { $limit: 5 }
]).toArray());

// Monthly active customers
printjson(db.orders.aggregate([
  { $group: { _id: { $dateToString: { date: '$order_date', format: '%Y-%m' } }, active_customers: { $addToSet: '$customer_id' } } },
  { $project: { month: '$_id', active_customers: { $size: '$active_customers' }, _id: 0 } },
  { $sort: { month: 1 } }
]).toArray());
