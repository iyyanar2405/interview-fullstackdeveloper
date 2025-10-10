use('ecom_training');

// Example document shapes
const customer = { first_name: 'Ada', last_name: 'Lovelace', email: 'ada@example.com', created_at: new Date() };
const product = { sku: 'SKU-1', name: 'Keyboard', price: 49.99, created_at: new Date() };

printjson(customer);
printjson(product);
