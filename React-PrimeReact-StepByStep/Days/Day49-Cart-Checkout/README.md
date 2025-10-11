# Day 49 — Cart & Checkout

## Objectives
- Implement cart state and order summary
- Build checkout form with address and payment fields
- Validate and place order

## Cart Context
```tsx
type CartItem = Product & { qty: number };

const CartContext = createContext<{ items: CartItem[]; add: (p: Product) => void; remove: (id: string) => void } | null>(null);

export function CartProvider({ children }: { children: React.ReactNode }) {
  const [items, setItems] = useState<CartItem[]>([]);
  const add = (p: Product) => setItems((cur) => {
    const i = cur.findIndex(x => x.id === p.id);
    if (i >= 0) { const copy = [...cur]; copy[i] = { ...copy[i], qty: copy[i].qty + 1 }; return copy; }
    return [...cur, { ...p, qty: 1 }];
  });
  const remove = (id: string) => setItems((cur) => cur.filter(x => x.id !== id));
  return <CartContext.Provider value={{ items, add, remove }}>{children}</CartContext.Provider>;
}

export const useCart = () => {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('Wrap with CartProvider');
  return ctx;
};
```

## CartPage
```tsx
function CartPage() {
  const { items, remove } = useCart();
  const total = items.reduce((s, i) => s + i.price * i.qty, 0);

  return (
    <div className="grid">
      <div className="col-12 md:col-8">
        <DataTable value={items} dataKey="id">
          <Column field="name" header="Product" />
          <Column field="qty" header="Qty" />
          <Column field="price" header="Price" body={(r) => `$${r.price.toFixed(2)}`} />
          <Column header="" body={(r) => <Button icon="pi pi-times" text onClick={() => remove(r.id)} />} />
        </DataTable>
      </div>
      <div className="col-12 md:col-4">
        <div className="p-3 border-1 surface-border border-round">
          <div className="flex justify-content-between">
            <span>Total</span><span className="font-bold">${total.toFixed(2)}</span>
          </div>
          <Link to="/checkout"><Button label="Checkout" className="w-full mt-3" /></Link>
        </div>
      </div>
    </div>
  );
}
```

## Checkout Page
```tsx
type Checkout = { name: string; email: string; address: string; city: string; zip: string; card: string };

function CheckoutPage() {
  const { handleSubmit, register, formState: { errors } } = useForm<Checkout>();
  const onSubmit = (v: Checkout) => alert('Order placed!');

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="grid gap-3">
      <div className="grid col-12 md:col-8">
        <InputText placeholder="Name" {...register('name', { required: 'Required' })} className={errors.name && 'p-invalid'} />
        <InputText placeholder="Email" {...register('email', { required: 'Required' })} className={errors.email && 'p-invalid'} />
        <InputText placeholder="Address" {...register('address', { required: 'Required' })} className={errors.address && 'p-invalid'} />
        <div className="grid">
          <div className="col-6"><InputText placeholder="City" {...register('city', { required: 'Required' })} /></div>
          <div className="col-6"><InputText placeholder="ZIP" {...register('zip', { required: 'Required' })} /></div>
        </div>
        <InputMask mask="9999 9999 9999 9999" placeholder="Card Number" {...register('card', { required: 'Required' })} />
      </div>
      <div className="col-12"><Button type="submit" label="Place Order" /></div>
    </form>
  );
}
```

## Checklist
- [ ] Cart add/remove works
- [ ] Totals compute correctly
- [ ] Checkout validation done
- [ ] Success flow handled