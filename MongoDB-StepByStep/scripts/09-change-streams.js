// Requires replica set or sharded cluster
use('ecom_training');

const changeStream = db.orders.watch();
print('Watching for changes... (Press Ctrl+C to exit)');
while (true) {
  const hasNext = changeStream.hasNext();
  if (hasNext) printjson(changeStream.next());
}
