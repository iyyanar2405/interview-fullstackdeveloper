# Day 47 — E-Commerce Setup

## Objectives
- Initialize storefront routes and layout
- Define product types and mock data service
- Shared UI components for cards and filters

## Types and Service
```tsx
type Product = { id: string; name: string; price: number; category: string; rating: number; image: string; inventoryStatus: 'INSTOCK' | 'LOWSTOCK' | 'OUTOFSTOCK' };

const catalog = {
  list: async (): Promise<Product[]> => Array.from({ length: 100 }).map((_, i) => ({ id: String(i), name: `Product ${i}`, price: (i % 10) * 10 + 9.99, category: ['Accessories', 'Clothing', 'Electronics'][i % 3], rating: (i % 5) + 1, image: 'placeholder.png', inventoryStatus: ['INSTOCK', 'LOWSTOCK', 'OUTOFSTOCK'][i % 3] }))
};
```

## Product Card
```tsx
function ProductCard({ p, onAdd }: { p: Product; onAdd: (p: Product) => void }) {
  return (
    <div className="p-3 border-1 surface-border border-round flex flex-column gap-2">
      <img src={p.image} alt={p.name} className="w-full border-round" />
      <div className="text-900 font-bold">{p.name}</div>
      <div className="flex align-items-center justify-content-between">
        <span className="text-xl">${p.price.toFixed(2)}</span>
        <Button icon="pi pi-shopping-cart" onClick={() => onAdd(p)} disabled={p.inventoryStatus === 'OUTOFSTOCK'} />
      </div>
    </div>
  );
}
```

## Routes
```tsx
export function StorefrontRoutes() {
  return (
    <Routes>
      <Route path="/shop" element={<CatalogPage />} />
      <Route path="/cart" element={<CartPage />} />
      <Route path="/checkout" element={<CheckoutPage />} />
    </Routes>
  );
}
```

## Checklist
- [ ] Catalog service returns data
- [ ] Product card reusable
- [ ] Routes render pages
- [ ] Basic layout ready