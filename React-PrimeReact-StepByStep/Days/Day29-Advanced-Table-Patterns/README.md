# Day 29 — Advanced Table Patterns

## Objectives
- Master advanced DataTable patterns: row expansion, row grouping, editing
- Implement context menus and selection models
- Optimize large table UX

## Row Expansion
```tsx
function TableRowExpansion() {
  const [products, setProducts] = useState<Product[]>([]);
  const [expandedRows, setExpandedRows] = useState<any | null>(null);

  const rowExpansionTemplate = (data: Product) => {
    return (
      <div className="p-3">
        <h5 className="m-0">Details for {data.name}</h5>
        <div className="mt-2 text-600">Category: {data.category}</div>
        <div className="text-600">Inventory: {data.inventoryStatus}</div>
      </div>
    );
  };

  return (
    <DataTable value={products} expandedRows={expandedRows} onRowToggle={(e) => setExpandedRows(e.data)} rowExpansionTemplate={rowExpansionTemplate} dataKey="id">
      <Column expander style={{ width: '3rem' }} />
      <Column field="name" header="Name" />
      <Column field="price" header="Price" />
    </DataTable>
  );
}
```

## Row Grouping
```tsx
function TableRowGrouping() {
  const [orders, setOrders] = useState<Order[]>([]);

  return (
    <DataTable value={orders} rowGroupMode="subheader" groupRowsBy="customer" sortMode="single" sortField="customer" sortOrder={1} expandableRowGroups>
      <Column field="customer" header="Customer" />
      <Column field="product" header="Product" />
      <Column field="quantity" header="Qty" />
    </DataTable>
  );
}
```

## Cell Editing
```tsx
function TableCellEditing() {
  const [products, setProducts] = useState<Product[]>([]);

  const onCellEditComplete = (e: DataTableCellEditCompleteParams) => {
    const { rowData, newValue, field } = e;
    rowData[field] = newValue;
    setProducts([...products]);
  };

  const textEditor = (options: ColumnEditorOptions) => (
    <InputText type="text" value={options.value} onChange={(e) => options.editorCallback?.(e.target.value)} />
  );

  return (
    <DataTable value={products} editMode="cell" dataKey="id" onCellEditComplete={onCellEditComplete}>
      <Column field="name" header="Name" editor={textEditor} />
      <Column field="price" header="Price" editor={textEditor} />
    </DataTable>
  );
}
```

## Context Menu
```tsx
function TableContextMenu() {
  const [products, setProducts] = useState<Product[]>([]);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const cm = useRef<ContextMenu>(null);
  const menuModel = [
    { label: 'View', icon: 'pi pi-search', command: () => {/* ... */} },
    { label: 'Delete', icon: 'pi pi-trash', command: () => {/* ... */} }
  ];

  return (
    <div className="card">
      <ContextMenu model={menuModel} ref={cm} />
      <DataTable value={products} selectionMode="single" selection={selectedProduct} onSelectionChange={(e) => setSelectedProduct(e.value)} contextMenuSelection={selectedProduct} onContextMenuSelectionChange={(e) => setSelectedProduct(e.value)} onContextMenu={(e) => cm.current?.show(e.originalEvent)} dataKey="id">
        <Column field="name" header="Name" />
        <Column field="price" header="Price" />
      </DataTable>
    </div>
  );
}
```

## Exercises
- Combine expansion + editing in a single table
- Add context menu actions for selected rows
- Group by category and compute group totals

## Checklist
- [ ] Expansion template renders details
- [ ] Cell editing persists changes
- [ ] Context menu actions work
- [ ] Grouping improves readability