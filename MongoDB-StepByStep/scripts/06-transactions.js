// Requires a replica set to use transactions.
// Start mongod with --replSet, initiate rs.initiate(), then connect.

use('ecom_training');

const session = db.getMongo().startSession();
const ordersCol = session.getDatabase('ecom_training').orders;

try {
  session.startTransaction();
  const cust = db.customers.findOne({ email: 'ada@example.com' });
  const ins = ordersCol.insertOne({ customer_id: cust._id, order_date: new Date(), status: 'PROCESSING', items: [] });
  // Read the just-inserted doc within the transaction
  const found = ordersCol.findOne({ _id: ins.insertedId });
  print('Found in txn (not yet committed):');
  printjson(found);
  session.abortTransaction();
  print('Transaction aborted.');
} catch (e) {
  print('Error:', e.message);
  session.abortTransaction();
} finally {
  session.endSession();
}
