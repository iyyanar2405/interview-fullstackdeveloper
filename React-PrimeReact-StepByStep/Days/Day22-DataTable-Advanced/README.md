# Day 22 — DataTable Advanced

## Objectives
- Sorting, filtering, pagination
- Selection modes
- Row expansion

## Advanced Features
```tsx
function AdvancedDataTable() {
  const [products, setProducts] = useState([]);
  const [selectedProducts, setSelectedProducts] = useState([]);
  const [filters, setFilters] = useState({
    'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
    'name': { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }] },
    'category': { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.EQUALS }] },
    'inventoryStatus': { operator: FilterOperator.OR, constraints: [{ value: null, matchMode: FilterMatchMode.EQUALS }] }
  });
  const [globalFilterValue, setGlobalFilterValue] = useState('');

  const onGlobalFilterChange = (e) => {
    const value = e.target.value;
    let _filters = { ...filters };
    _filters['global'].value = value;
    setFilters(_filters);
    setGlobalFilterValue(value);
  };

  const renderHeader = () => {
    return (
      <div className="flex justify-content-between">
        <Button type="button" icon="pi pi-filter-slash" label="Clear" outlined onClick={clearFilter} />
        <span className="p-input-icon-left">
          <i className="pi pi-search" />
          <InputText value={globalFilterValue} onChange={onGlobalFilterChange} placeholder="Keyword Search" />
        </span>
      </div>
    );
  };

  return (
    <DataTable 
      value={products} 
      paginator 
      rows={10} 
      dataKey="id"
      filters={filters} 
      filterDisplay="menu"
      globalFilterFields={['name', 'category', 'inventoryStatus']}
      header={renderHeader()}
      selection={selectedProducts} 
      onSelectionChange={(e) => setSelectedProducts(e.value)}
      selectionMode="multiple"
      tableStyle={{ minWidth: '75rem' }}
    >
      <Column selectionMode="multiple" headerStyle={{ width: '3rem' }}></Column>
      <Column field="name" header="Name" sortable filter filterPlaceholder="Search by name" />
      <Column field="category" header="Category" showFilterMatchModes={false} filterMenuStyle={{ width: '14rem' }} />
      <Column field="inventoryStatus" header="Status" filterMenuStyle={{ width: '14rem' }} />
      <Column field="price" header="Price" sortable body={priceBodyTemplate} />
    </DataTable>
  );
}
```

## Row Expansion
```tsx
const allowExpansion = (rowData) => {
  return rowData.orders && rowData.orders.length > 0;
};

const rowExpansionTemplate = (data) => {
  return (
    <div className="p-3">
      <h5>Orders for {data.name}</h5>
      <DataTable value={data.orders}>
        <Column field="id" header="Id" sortable></Column>
        <Column field="customer" header="Customer" sortable></Column>
        <Column field="date" header="Date" sortable></Column>
        <Column field="amount" header="Amount" body={priceBodyTemplate} />
        <Column field="status" header="Status" body={statusOrderBodyTemplate} />
      </DataTable>
    </div>
  );
};
```

## Exercise
- Add advanced filtering and sorting
- Implement row expansion with nested data

## Checklist
- [ ] Filtering works on multiple columns
- [ ] Pagination functional
- [ ] Row selection works
- [ ] Row expansion displays nested data