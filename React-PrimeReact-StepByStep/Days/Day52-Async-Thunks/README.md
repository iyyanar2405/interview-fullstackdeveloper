# Day 52 — Async Thunks

## Objectives
- Handle async requests with Redux Toolkit createAsyncThunk
- Manage loading/error states in slices
- Display progress with PrimeReact components

## Setup
```tsx
import { configureStore, createSlice, createAsyncThunk } from '@reduxjs/toolkit';

type Product = { id: string; name: string };

export const fetchProducts = createAsyncThunk<Product[]>('products/fetch', async () => {
  await new Promise((r) => setTimeout(r, 400));
  return Array.from({ length: 5 }).map((_, i) => ({ id: String(i), name: `Product ${i}` }));
});

const productsSlice = createSlice({
  name: 'products',
  initialState: { items: [] as Product[], loading: false, error: '' },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchProducts.pending, (s) => { s.loading = true; s.error = ''; })
      .addCase(fetchProducts.fulfilled, (s, a) => { s.items = a.payload; s.loading = false; })
      .addCase(fetchProducts.rejected, (s, a) => { s.loading = false; s.error = a.error.message || 'Error'; });
  }
});

export const store = configureStore({ reducer: { products: productsSlice.reducer } });
```

## Usage
```tsx
function ProductsList() {
  const dispatch = useAppDispatch();
  const { items, loading, error } = useAppSelector((s) => s.products);

  useEffect(() => { dispatch(fetchProducts()); }, [dispatch]);

  if (loading) return <ProgressBar mode="indeterminate" style={{ height: '6px' }} />;
  if (error) return <Message severity="error" text={error} />;

  return <ul>{items.map(p => <li key={p.id}>{p.name}</li>)}</ul>;
}
```

## Checklist
- [ ] Async thunk dispatches lifecycle actions
- [ ] Loading and error shown in UI
- [ ] Typed hooks for dispatch/select
- [ ] No race conditions on re-renders