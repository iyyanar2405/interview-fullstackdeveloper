use('ecom_training');

// JSON Schema validation on products
db.runCommand({
  collMod: 'products',
  validator: {
    $jsonSchema: {
      bsonType: 'object',
      required: ['sku','name','price'],
      properties: {
        sku: { bsonType: 'string', minLength: 1 },
        name: { bsonType: 'string', minLength: 1 },
        price: { bsonType: 'double', minimum: 0 }
      }
    }
  },
  validationLevel: 'moderate'
});
