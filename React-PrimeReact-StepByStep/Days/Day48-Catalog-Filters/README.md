# Day 48 — Catalog Filters

## Objectives
- Implement filtering and sorting for catalog
- Use PrimeReact components: Dropdown, MultiSelect, Rating, Slider
- Combine client-side filters with pagination

## Filters State
```tsx
export default function CatalogPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [category, setCategory] = useState<string | null>(null);
  const [rating, setRating] = useState<number | null>(null);
  const [price, setPrice] = useState<[number, number]>([0, 100]);
  const [sort, setSort] = useState<string>('price-asc');

  useEffect(() => { catalog.list().then(setProducts); }, []);

  const filtered = products
    .filter(p => !category || p.category === category)
    .filter(p => !rating || p.rating >= rating)
    .filter(p => p.price >= price[0] && p.price <= price[1])
    .sort((a, b) => sort === 'price-asc' ? a.price - b.price : b.price - a.price);

  return (
    <div className="grid">
      <div className="col-12 md:col-3">
        <div className="p-3 border-1 surface-border border-round flex flex-column gap-3">
          <Dropdown value={category} onChange={(e) => setCategory(e.value)} options={[{ label: 'All', value: null }, { label: 'Accessories', value: 'Accessories' }, { label: 'Clothing', value: 'Clothing' }, { label: 'Electronics', value: 'Electronics' }]} placeholder="Category" />
          <Rating value={rating ?? 0} onChange={(e) => setRating(e.value ?? null)} cancel={true} />
          <span>Price</span>
          <Slider value={price} onChange={(e) => setPrice(e.value)} range max={200} />
          <Dropdown value={sort} onChange={(e) => setSort(e.value)} options={[{ label: 'Price: Low to High', value: 'price-asc' }, { label: 'Price: High to Low', value: 'price-desc' }]} />
        </div>
      </div>
      <div className="col-12 md:col-9 grid">
        {filtered.map((p) => (
          <div className="col-12 sm:col-6 lg:col-4" key={p.id}>
            <ProductCard p={p} onAdd={() => {}} />
          </div>
        ))}
      </div>
    </div>
  );
}
```

## Checklist
- [ ] Filters update list in real-time
- [ ] Sort works both directions
- [ ] Range slider limits applied
- [ ] Paging or virtual scroll for long lists