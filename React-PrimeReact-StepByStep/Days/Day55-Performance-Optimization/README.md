# Day 55 — Performance Optimization

## Objectives
- Prevent unnecessary renders and heavy work on main thread
- Apply React.useMemo/useCallback and PrimeReact virtualization
- Analyze with React DevTools Profiler

## Patterns
```tsx
function ExpensiveList({ items }: { items: any[] }) {
  const rows = useMemo(() => items.map(i => ({ ...i, total: i.price * i.qty })), [items]);
  const renderRow = useCallback((row) => <div className="p-2 border-bottom-1 surface-border">{row.total}</div>, []);

  return (
    <VirtualScroller items={rows} itemSize={40} itemTemplate={renderRow} style={{ height: 400 }} />
  );
}
```

## Avoid Re-renders
```tsx
const ProductCard = React.memo(function ProductCard({ p, onAdd }: { p: Product; onAdd: (p: Product) => void }) {
  return (
    <div className="p-3 border-1 surface-border border-round">
      <div className="font-bold">{p.name}</div>
      <Button label="Add" onClick={() => onAdd(p)} />
    </div>
  );
});
```

## Network and Bundle
- Debounce search inputs
- Use Suspense + lazy for big routes
- Split vendor chunks

## Checklist
- [ ] Profiler shows fewer commits
- [ ] Virtualization used for long lists
- [ ] Memoization prevents re-renders
- [ ] Bundle split reduces TTI