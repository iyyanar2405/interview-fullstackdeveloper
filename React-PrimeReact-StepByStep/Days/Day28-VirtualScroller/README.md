# Day 28 — VirtualScroller

## Objectives
- Render large lists efficiently with VirtualScroller
- Implement lazy loading and infinite scroll
- Grid and list templates

## Basic VirtualScroller
```tsx
function VirtualList() {
  const items = Array.from({ length: 10000 }).map((_, i) => `Item #${i}`);

  const itemTemplate = (item: string, options: VirtualScrollerTemplateOptions) => (
    <div className={`p-3 border-bottom-1 surface-border ${options.even ? 'surface-50' : ''}`}>{item}</div>
  );

  return (
    <VirtualScroller items={items} itemSize={40} itemTemplate={itemTemplate} style={{ height: '400px' }} />
  );
}
```

## Grid VirtualScroller
```tsx
function VirtualGrid() {
  const items = Array.from({ length: 2000 }).map((_, i) => ({ id: i, title: `Card ${i}` }));

  const itemTemplate = (item: any, options: VirtualScrollerTemplateOptions) => (
    <div className="col-6 md:col-3 p-2">
      <div className="p-3 border-1 surface-border border-round">{item.title}</div>
    </div>
  );

  return (
    <VirtualScroller 
      items={items} 
      itemSize={[100, 200]} 
      itemTemplate={itemTemplate}
      orientation="vertical" 
      className="grid" 
      style={{ height: '400px' }}
    />
  );
}
```

## Lazy Loading (infinite scroll)
```tsx
function VirtualInfinite() {
  const [lazyItems, setLazyItems] = useState<any[]>(Array.from({ length: 1000 }));
  const [loading, setLoading] = useState(false);

  const onLazyLoad = (e: VirtualScrollerLazyEvent) => {
    if (loading) return;
    setLoading(true);

    setTimeout(() => {
      const newItems = [...lazyItems];
      for (let i = e.first; i < e.last; i++) {
        newItems[i] = { label: `Item #${i}` };
      }
      setLazyItems(newItems);
      setLoading(false);
    }, 600);
  };

  const itemTemplate = (item: any, options: VirtualScrollerTemplateOptions) => {
    const content = item ? item.label : <Skeleton width="8rem" height="1rem" />;
    return <div className="p-2 border-bottom-1 surface-border">{content}</div>;
  };

  return (
    <VirtualScroller 
      items={lazyItems} 
      itemSize={40} 
      itemTemplate={itemTemplate} 
      lazy onLazyLoad={onLazyLoad} 
      style={{ height: '400px' }}
      showLoader 
      loading={loading} 
      loadingTemplate={<div className="p-2">Loading...</div>}
    />
  );
}
```

## Exercises
- Convert an existing long list to VirtualScroller
- Create a virtualized grid of product cards
- Add lazy loading and a Skeleton placeholder

## Checklist
- [ ] Smooth scroll performance
- [ ] Lazy loading fills ranges
- [ ] Grid/list templates responsive
- [ ] Skeleton renders during load