# Day 21 — DataTable Basics

## Objectives
- DataTable component fundamentals
- Basic column configuration
- Data binding patterns

## Basic DataTable
```tsx
interface Product {
  id: string;
  code: string;
  name: string;
  description: string;
  image: string;
  price: number;
  category: string;
  quantity: number;
  inventoryStatus: string;
  rating: number;
}

function DataTableDemo() {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    // Fetch products
    setProducts(mockProducts);
  }, []);

  return (
    <DataTable value={products} tableStyle={{ minWidth: '50rem' }}>
      <Column field="code" header="Code"></Column>
      <Column field="name" header="Name"></Column>
      <Column field="category" header="Category"></Column>
      <Column field="quantity" header="Quantity"></Column>
      <Column field="price" header="Price" body={priceBodyTemplate}></Column>
    </DataTable>
  );
}

const priceBodyTemplate = (product: Product) => {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(product.price);
};
```

## Column Templates
```tsx
const imageBodyTemplate = (product: Product) => {
  return <img src={`images/product/${product.image}`} alt={product.image} className="w-6rem shadow-2 border-round" />;
};

const ratingBodyTemplate = (product: Product) => {
  return <Rating value={product.rating} readOnly cancel={false} />;
};

const statusBodyTemplate = (product: Product) => {
  return <Tag value={product.inventoryStatus} severity={getSeverity(product)}></Tag>;
};
```

## Exercise
- Create a products table with custom templates
- Add image, rating, and status columns

## Checklist
- [ ] DataTable renders with data
- [ ] Custom column templates work
- [ ] Data formatting applied