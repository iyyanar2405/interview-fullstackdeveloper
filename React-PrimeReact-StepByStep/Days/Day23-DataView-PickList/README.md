# Day 23 — DataView, PickList & OrderList

## Objectives
- DataView for flexible data display
- PickList for item selection
- OrderList for reordering items

## DataView Component
```tsx
function DataViewDemo() {
  const [products, setProducts] = useState([]);
  const [layout, setLayout] = useState('grid');
  const [sortKey, setSortKey] = useState(null);
  const [sortOrder, setSortOrder] = useState(null);
  const [sortField, setSortField] = useState(null);

  const getSeverity = (product) => {
    switch (product.inventoryStatus) {
      case 'INSTOCK': return 'success';
      case 'LOWSTOCK': return 'warning';
      case 'OUTOFSTOCK': return 'danger';
      default: return null;
    }
  };

  const listItem = (product) => {
    return (
      <div className="col-12">
        <div className="flex flex-column xl:flex-row xl:align-items-start p-4 gap-4">
          <img className="w-9 sm:w-16rem xl:w-10rem shadow-2 block xl:block mx-auto border-round" src={`images/product/${product.image}`} alt={product.name} />
          <div className="flex flex-column sm:flex-row justify-content-between align-items-center xl:align-items-start flex-1 gap-4">
            <div className="flex flex-column align-items-center sm:align-items-start gap-3">
              <div className="text-2xl font-bold text-900">{product.name}</div>
              <Rating value={product.rating} readOnly cancel={false}></Rating>
              <div className="flex align-items-center gap-3">
                <span className="flex align-items-center gap-2">
                  <i className="pi pi-tag"></i>
                  <span className="font-semibold">{product.category}</span>
                </span>
                <Tag value={product.inventoryStatus} severity={getSeverity(product)}></Tag>
              </div>
            </div>
            <div className="flex sm:flex-column align-items-center sm:align-items-end gap-3 sm:gap-2">
              <span className="text-2xl font-semibold">${product.price}</span>
              <Button icon="pi pi-shopping-cart" className="p-button-rounded" disabled={product.inventoryStatus === 'OUTOFSTOCK'}></Button>
            </div>
          </div>
        </div>
      </div>
    );
  };

  const gridItem = (product) => {
    return (
      <div className="col-12 sm:col-6 lg:col-12 xl:col-4 p-2">
        <div className="p-4 border-1 surface-border surface-card border-round">
          <div className="flex flex-wrap align-items-center justify-content-between gap-2">
            <div className="flex align-items-center gap-2">
              <i className="pi pi-tag"></i>
              <span className="font-semibold">{product.category}</span>
            </div>
            <Tag value={product.inventoryStatus} severity={getSeverity(product)}></Tag>
          </div>
          <div className="flex flex-column align-items-center gap-3 py-5">
            <img className="w-9 shadow-2 border-round" src={`images/product/${product.image}`} alt={product.name} />
            <div className="text-2xl font-bold">{product.name}</div>
            <Rating value={product.rating} readOnly cancel={false}></Rating>
          </div>
          <div className="flex align-items-center justify-content-between">
            <span className="text-2xl font-semibold">${product.price}</span>
            <Button icon="pi pi-shopping-cart" className="p-button-rounded" disabled={product.inventoryStatus === 'OUTOFSTOCK'}></Button>
          </div>
        </div>
      </div>
    );
  };

  const itemTemplate = (product, layout) => {
    if (!product) return;
    return layout === 'list' ? listItem(product) : gridItem(product);
  };

  const header = () => {
    return (
      <div className="flex justify-content-between">
        <Dropdown options={sortOptions} value={sortKey} optionLabel="label" placeholder="Sort By Price" onChange={onSortChange} />
        <DataViewLayoutOptions layout={layout} onChange={(e) => setLayout(e.value)} />
      </div>
    );
  };

  return (
    <DataView 
      value={products} 
      itemTemplate={itemTemplate} 
      layout={layout} 
      header={header()} 
      sortField={sortField} 
      sortOrder={sortOrder}
      paginator 
      rows={9}
    />
  );
}
```

## PickList Component
```tsx
function PickListDemo() {
  const [source, setSource] = useState([]);
  const [target, setTarget] = useState([]);

  const onChange = (event) => {
    setSource(event.source);
    setTarget(event.target);
  };

  const itemTemplate = (item) => {
    return (
      <div className="flex flex-wrap p-2 align-items-center gap-3">
        <img className="w-4rem shadow-2 flex-shrink-0 border-round" src={`images/product/${item.image}`} alt={item.name} />
        <div className="flex-1 flex flex-column gap-2">
          <span className="font-bold">{item.name}</span>
          <div className="flex align-items-center gap-2">
            <i className="pi pi-tag text-sm"></i>
            <span>{item.category}</span>
          </div>
        </div>
        <span className="font-bold text-900">${item.price}</span>
      </div>
    );
  };

  return (
    <PickList 
      source={source} 
      target={target} 
      onChange={onChange} 
      itemTemplate={itemTemplate}
      sourceHeader="Available" 
      targetHeader="Selected" 
      sourceStyle={{ height: '24rem' }} 
      targetStyle={{ height: '24rem' }}
    />
  );
}
```

## OrderList Component
```tsx
function OrderListDemo() {
  const [products, setProducts] = useState([]);

  const itemTemplate = (item) => {
    return (
      <div className="flex flex-wrap p-2 align-items-center gap-3">
        <img className="w-4rem shadow-2 flex-shrink-0 border-round" src={`images/product/${item.image}`} alt={item.name} />
        <div className="flex-1 flex flex-column gap-2 xl:mr-8">
          <span className="font-bold">{item.name}</span>
          <div className="flex align-items-center gap-2">
            <i className="pi pi-tag text-sm"></i>
            <span>{item.category}</span>
          </div>
        </div>
        <span className="font-bold text-900">${item.price}</span>
      </div>
    );
  };

  return (
    <OrderList 
      value={products} 
      onChange={(e) => setProducts(e.value)} 
      itemTemplate={itemTemplate} 
      header="Products"
    />
  );
}
```

## Exercise
- Create a product comparison interface using PickList
- Build a priority task manager with OrderList
- Display products in both grid and list views with DataView

## Checklist
- [ ] DataView displays in both layouts
- [ ] PickList moves items between lists
- [ ] OrderList reorders items correctly
- [ ] Custom templates render properly