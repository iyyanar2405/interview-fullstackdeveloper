# Day 22: PrimeNG Table - Advanced Features

**Table of Contents:**
1. [Introduction to Advanced Table Features](#introduction-to-advanced-table-features)
2. [Pagination](#pagination)
3. [Sorting](#sorting)
4. [Filtering](#filtering)
5. [Row Selection and Context Menu](#row-selection-and-context-menu)
6. [Inline Editing](#inline-editing)
7. [Column Toggling and Reordering](#column-toggling-and-reordering)
8. [Table State Persistence](#table-state-persistence)
9. [Advanced Export Options](#advanced-export-options)
10. [Practical Exercise: Product Inventory System](#practical-exercise-product-inventory-system)
11. [Best Practices](#best-practices)
12. [Key Takeaways](#key-takeaways)
13. [Resources and Further Reading](#resources-and-further-reading)

## Introduction to Advanced Table Features

Building on our basic table skills from Day 21, today we'll explore the advanced features that make PrimeNG Tables truly enterprise-ready. These features address common requirements in complex business applications, such as handling large datasets, enabling comprehensive data manipulation, and providing powerful user interactions.

### What We'll Cover:

- **Pagination**: Client-side and server-side approaches
- **Sorting**: Single and multi-column sorting capabilities
- **Filtering**: Row, column, global, and custom filtering
- **Row Selection**: Advanced selection strategies and context menus
- **Inline Editing**: Enabling direct table edits
- **Column Management**: Toggle visibility and reorder columns
- **State Management**: Save and restore table configurations
- **Export Capabilities**: PDF, Excel, and CSV exports

### Why These Features Matter:

These advanced features transform a basic data display into a powerful data management system. They significantly improve user experience by allowing users to find, organize, and manipulate data efficiently. In enterprise applications, these capabilities often reduce the need for separate screens and operations, streamlining workflows.

## Pagination

Pagination is essential for handling large datasets efficiently. PrimeNG offers both client-side and server-side pagination approaches.

### Client-Side Pagination

Client-side pagination works by loading all data at once, then displaying it page by page:

```html
<p-table 
  [value]="products" 
  [paginator]="true" 
  [rows]="10" 
  [showCurrentPageReport]="true"
  currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
  [rowsPerPageOptions]="[10, 25, 50]">
  <!-- Table templates -->
</p-table>
```

### Server-Side Pagination (Lazy Loading)

For large datasets, server-side pagination is more efficient:

```html
<p-table 
  [value]="products" 
  [lazy]="true" 
  (onLazyLoad)="loadProducts($event)"
  [paginator]="true" 
  [rows]="10" 
  [totalRecords]="totalRecords" 
  [loading]="loading">
  <!-- Table templates -->
</p-table>
```

```typescript
export class ProductTableComponent {
  products: Product[] = [];
  totalRecords: number;
  loading: boolean = false;
  
  constructor(private productService: ProductService) {}
  
  loadProducts(event: LazyLoadEvent) {
    this.loading = true;
    
    // Simulate API call with setTimeout
    setTimeout(() => {
      this.productService.getProducts(
        event.first, 
        event.rows, 
        event.sortField, 
        event.sortOrder, 
        event.filters
      ).subscribe(res => {
        this.products = res.data;
        this.totalRecords = res.total;
        this.loading = false;
      });
    }, 1000);
  }
}
```

### Advanced Pagination Features

PrimeNG provides additional pagination customization:

```html
<p-table 
  [value]="products" 
  [paginator]="true" 
  [rows]="10"
  [showCurrentPageReport]="true" 
  [showJumpToPageDropdown]="true"
  [showPageLinks]="false"
  [rowsPerPageOptions]="[10, 25, 50, 100]">
  <!-- Table templates -->
</p-table>
```

### Pagination Templates

Customize the paginator appearance using templates:

```html
<p-table [value]="products" [paginator]="true" [rows]="10">
  <!-- Table templates -->
  
  <ng-template pTemplate="paginatorleft">
    <p-button type="button" icon="pi pi-refresh" styleClass="p-button-text"></p-button>
  </ng-template>
  
  <ng-template pTemplate="paginatorright">
    <p-button type="button" icon="pi pi-download" styleClass="p-button-text"></p-button>
  </ng-template>
</p-table>
```

## Sorting

PrimeNG Tables provide flexible sorting capabilities:

### Single Column Sorting

```html
<p-table [value]="products" [sortField]="sortField" [sortOrder]="sortOrder">
  <ng-template pTemplate="header">
    <tr>
      <th pSortableColumn="code">Code <p-sortIcon field="code"></p-sortIcon></th>
      <th pSortableColumn="name">Name <p-sortIcon field="name"></p-sortIcon></th>
      <th pSortableColumn="price">Price <p-sortIcon field="price"></p-sortIcon></th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr>
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
```

### Multi-Column Sorting

Enable multi-column sorting with the `sortMode` property:

```html
<p-table [value]="products" sortMode="multiple">
  <!-- Use the same header template as single sorting -->
</p-table>
```

### Custom Sorting Logic

Implement custom sorting logic with the `customSort` property:

```html
<p-table [value]="products" [customSort]="true" (sortFunction)="customSort($event)">
  <!-- Table templates -->
</p-table>
```

```typescript
customSort(event: SortEvent) {
  event.data.sort((data1, data2) => {
    // For strings - case insensitive sorting
    if (typeof data1[event.field] === 'string') {
      return data1[event.field].localeCompare(data2[event.field]) * event.order;
    }
    // For numbers and dates
    else {
      return (data1[event.field] < data2[event.field] ? -1 : 1) * event.order;
    }
  });
}
```

### Server-Side Sorting

For server-side sorting, process the sort parameters in the `onLazyLoad` event:

```typescript
loadProducts(event: LazyLoadEvent) {
  // event.sortField contains the field to sort by
  // event.sortOrder contains the sort direction (1 for ascending, -1 for descending)
  // event.multiSortMeta contains the sort information for multi-column sorting
  
  this.productService.getProducts({
    sortField: event.sortField,
    sortOrder: event.sortOrder,
    multiSortMeta: event.multiSortMeta
  }).subscribe(res => {
    this.products = res.data;
    this.totalRecords = res.total;
  });
}
```

## Filtering

PrimeNG Tables support various filtering mechanisms:

### Global Filtering

Filter across all columns:

```html
<div class="p-d-flex p-mb-3">
  <span class="p-input-icon-left">
    <i class="pi pi-search"></i>
    <input pInputText type="text" (input)="dt.filterGlobal($event.target.value, 'contains')" 
           placeholder="Search keyword" />
  </span>
</div>

<p-table #dt [value]="products" [globalFilterFields]="['name', 'code', 'category']">
  <!-- Table templates -->
</p-table>
```

### Column Filtering

Enable filtering on specific columns:

```html
<p-table [value]="products" [filterDelay]="0">
  <ng-template pTemplate="header">
    <tr>
      <th>
        <div class="p-d-flex p-jc-between p-ai-center">
          Code
          <p-columnFilter type="text" field="code" display="menu"></p-columnFilter>
        </div>
      </th>
      <th>
        <div class="p-d-flex p-jc-between p-ai-center">
          Name
          <p-columnFilter type="text" field="name" display="menu"></p-columnFilter>
        </div>
      </th>
      <th>
        <div class="p-d-flex p-jc-between p-ai-center">
          Category
          <p-columnFilter field="category" matchMode="equals" display="menu">
            <ng-template pTemplate="filter" let-value let-filter="filterCallback">
              <p-dropdown 
                [options]="categories" 
                [style]="{'min-width':'12rem'}" 
                [showClear]="true"
                placeholder="Any" 
                (onChange)="filter($event.value)">
              </p-dropdown>
            </ng-template>
          </p-columnFilter>
        </div>
      </th>
      <th>
        <div class="p-d-flex p-jc-between p-ai-center">
          Price
          <p-columnFilter type="numeric" field="price" display="menu"></p-columnFilter>
        </div>
      </th>
    </tr>
  </ng-template>
  <!-- Body template -->
</p-table>
```

### Filter Display Modes

PrimeNG offers different filter display modes:

1. **Menu Mode**: Opens a popup menu
2. **Row Mode**: Displays filters in a separate row

```html
<!-- Menu Mode (default) -->
<p-columnFilter type="text" field="name" display="menu"></p-columnFilter>

<!-- Row Mode -->
<p-table [value]="products">
  <ng-template pTemplate="header">
    <tr>
      <th>Code</th>
      <th>Name</th>
      <th>Category</th>
      <th>Price</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="filter" let-columns>
    <tr>
      <th>
        <p-columnFilter type="text" field="code" display="row"></p-columnFilter>
      </th>
      <th>
        <p-columnFilter type="text" field="name" display="row"></p-columnFilter>
      </th>
      <th>
        <p-columnFilter field="category" matchMode="equals" display="row">
          <ng-template pTemplate="filter" let-value let-filter="filterCallback">
            <p-dropdown 
              [options]="categories" 
              placeholder="Any" 
              [showClear]="true"
              (onChange)="filter($event.value)">
            </p-dropdown>
          </ng-template>
        </p-columnFilter>
      </th>
      <th>
        <p-columnFilter type="numeric" field="price" display="row"></p-columnFilter>
      </th>
    </tr>
  </ng-template>
  <!-- Body template -->
</p-table>
```

### Custom Filter Templates

Create custom filter UI for specific data types:

```html
<!-- Date Range Filter -->
<p-columnFilter field="date" matchMode="custom" [showMatchModes]="false" [showOperator]="false" display="menu">
  <ng-template pTemplate="filter" let-value let-filter="filterCallback">
    <p-calendar 
      [(ngModel)]="dateRangeFilter" 
      selectionMode="range" 
      [readonlyInput]="true" 
      [showButtonBar]="true"
      placeholder="Date Range"
      (onSelect)="filter($event)"
      (onClear)="filter(null)">
    </p-calendar>
  </ng-template>
</p-columnFilter>
```

```typescript
dateRangeFilter: Date[];

// Custom filter implementation in component
filterByDateRange(value, filter) {
  if (filter === undefined || filter === null || (filter[0] === null && filter[1] === null)) {
    return true;
  }

  if (value === undefined || value === null) {
    return false;
  }

  const dateValue = new Date(value);
  
  if (filter[0] !== null && filter[1] !== null) {
    return dateValue >= filter[0] && dateValue <= filter[1];
  }
  if (filter[0] !== null) {
    return dateValue >= filter[0];
  }
  if (filter[1] !== null) {
    return dateValue <= filter[1];
  }
  return true;
}
```

### Filter Operators and Match Modes

PrimeNG provides multiple filter match modes:

- `startsWith`: Value starts with the filter
- `contains`: Value contains the filter
- `endsWith`: Value ends with the filter
- `equals`: Value equals the filter
- `notEquals`: Value does not equal the filter
- `in`: Value is in a list of values
- `lt`: Value is less than the filter
- `lte`: Value is less than or equal to the filter
- `gt`: Value is greater than the filter
- `gte`: Value is greater than or equal to the filter
- `between`: Value is between two values
- `custom`: Custom filter logic

```html
<p-columnFilter 
  field="name" 
  matchMode="contains" 
  [showMatchModes]="true" 
  [matchModeOptions]="matchModeOptions">
</p-columnFilter>
```

```typescript
matchModeOptions = [
  { label: 'Contains', value: 'contains' },
  { label: 'Starts With', value: 'startsWith' },
  { label: 'Ends With', value: 'endsWith' },
  { label: 'Equals', value: 'equals' }
];
```

## Row Selection and Context Menu

PrimeNG offers multiple row selection modes and context menu integration:

### Advanced Multiple Selection

Enable checkboxes for multiple selection:

```html
<p-table 
  [value]="products" 
  [(selection)]="selectedProducts" 
  dataKey="id"
  selectionMode="multiple">
  
  <ng-template pTemplate="header">
    <tr>
      <th style="width: 3rem">
        <p-tableHeaderCheckbox></p-tableHeaderCheckbox>
      </th>
      <th>Code</th>
      <th>Name</th>
      <th>Price</th>
    </tr>
  </ng-template>
  
  <ng-template pTemplate="body" let-product>
    <tr [pSelectableRow]="product">
      <td>
        <p-tableCheckbox [value]="product"></p-tableCheckbox>
      </td>
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>

<div class="p-mt-3" *ngIf="selectedProducts.length > 0">
  <p>Selected Products: {{selectedProducts.length}}</p>
  <button pButton label="Delete Selected" class="p-button-danger"></button>
</div>
```

### Context Menu

Add right-click context menus to rows:

```html
<p-contextMenu #cm [model]="contextMenuItems"></p-contextMenu>

<p-table [value]="products" [contextMenu]="cm" [(contextMenuSelection)]="selectedProduct">
  <ng-template pTemplate="header">
    <tr>
      <th>Code</th>
      <th>Name</th>
      <th>Price</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr [pContextMenuRow]="product">
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
```

```typescript
import { MenuItem } from 'primeng/api';

// In component class
contextMenuItems: MenuItem[];
selectedProduct: Product;

ngOnInit() {
  this.contextMenuItems = [
    { label: 'View Details', icon: 'pi pi-eye', command: () => this.viewDetails(this.selectedProduct) },
    { label: 'Edit', icon: 'pi pi-pencil', command: () => this.editProduct(this.selectedProduct) },
    { label: 'Delete', icon: 'pi pi-trash', command: () => this.deleteProduct(this.selectedProduct) }
  ];
}

viewDetails(product: Product) {
  // View implementation
}

editProduct(product: Product) {
  // Edit implementation
}

deleteProduct(product: Product) {
  // Delete implementation
}
```

### Row Selection Events

Handle selection events:

```html
<p-table 
  [value]="products" 
  [(selection)]="selectedProduct" 
  dataKey="id"
  selectionMode="single"
  (onRowSelect)="onRowSelect($event)"
  (onRowUnselect)="onRowUnselect($event)">
  <!-- Templates -->
</p-table>
```

```typescript
onRowSelect(event: any) {
  this.messageService.add({
    severity: 'info', 
    summary: 'Product Selected', 
    detail: `You selected ${event.data.name}`
  });
}

onRowUnselect(event: any) {
  this.messageService.add({
    severity: 'info', 
    summary: 'Product Unselected', 
    detail: `You unselected ${event.data.name}`
  });
}
```

## Inline Editing

PrimeNG Tables support row and cell editing:

### Row Editing

```html
<p-table [value]="products" dataKey="id" editMode="row">
  <ng-template pTemplate="header">
    <tr>
      <th>Code</th>
      <th>Name</th>
      <th>Price</th>
      <th>Actions</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product let-editing="editing" let-ri="rowIndex">
    <tr [pEditableRow]="product">
      <td>
        <p-cellEditor>
          <ng-template pTemplate="input">
            <input pInputText type="text" [(ngModel)]="product.code">
          </ng-template>
          <ng-template pTemplate="output">
            {{product.code}}
          </ng-template>
        </p-cellEditor>
      </td>
      <td>
        <p-cellEditor>
          <ng-template pTemplate="input">
            <input pInputText type="text" [(ngModel)]="product.name">
          </ng-template>
          <ng-template pTemplate="output">
            {{product.name}}
          </ng-template>
        </p-cellEditor>
      </td>
      <td>
        <p-cellEditor>
          <ng-template pTemplate="input">
            <input pInputText type="text" [(ngModel)]="product.price">
          </ng-template>
          <ng-template pTemplate="output">
            {{product.price | currency:'USD'}}
          </ng-template>
        </p-cellEditor>
      </td>
      <td>
        <div class="p-d-flex">
          <button pButton pRipple type="button" pInitEditableRow
                  icon="pi pi-pencil" class="p-button-rounded p-button-text"
                  (click)="onRowEditInit(product)" *ngIf="!editing"></button>
          <button pButton pRipple type="button" pSaveEditableRow
                  icon="pi pi-check" class="p-button-rounded p-button-text p-button-success"
                  (click)="onRowEditSave(product)" *ngIf="editing"></button>
          <button pButton pRipple type="button" pCancelEditableRow
                  icon="pi pi-times" class="p-button-rounded p-button-text p-button-danger"
                  (click)="onRowEditCancel(product, ri)" *ngIf="editing"></button>
        </div>
      </td>
    </tr>
  </ng-template>
</p-table>
```

```typescript
clonedProducts: { [s: string]: Product } = {};

onRowEditInit(product: Product) {
  this.clonedProducts[product.id] = {...product};
}

onRowEditSave(product: Product) {
  if (product.price > 0) {
    delete this.clonedProducts[product.id];
    this.messageService.add({
      severity: 'success', 
      summary: 'Success', 
      detail: 'Product is updated'
    });
  } else {
    this.messageService.add({
      severity: 'error', 
      summary: 'Error', 
      detail: 'Invalid Price'
    });
  }
}

onRowEditCancel(product: Product, index: number) {
  this.products[index] = this.clonedProducts[product.id];
  delete this.clonedProducts[product.id];
}
```

### Cell Editing

Enable editing on specific cells:

```html
<p-table [value]="products" dataKey="id" editMode="cell">
  <ng-template pTemplate="header">
    <tr>
      <th>Code</th>
      <th>Name</th>
      <th>Price</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr>
      <td>{{product.code}}</td>
      <td pEditableColumn>
        <p-cellEditor>
          <ng-template pTemplate="input">
            <input pInputText type="text" [(ngModel)]="product.name">
          </ng-template>
          <ng-template pTemplate="output">
            {{product.name}}
          </ng-template>
        </p-cellEditor>
      </td>
      <td pEditableColumn>
        <p-cellEditor>
          <ng-template pTemplate="input">
            <p-inputNumber [(ngModel)]="product.price" mode="currency" currency="USD"></p-inputNumber>
          </ng-template>
          <ng-template pTemplate="output">
            {{product.price | currency:'USD'}}
          </ng-template>
        </p-cellEditor>
      </td>
    </tr>
  </ng-template>
</p-table>
```

```typescript
onCellEditComplete(event: any) {
  let { data, field, value, originalEvent } = event;
  
  // Validate the new value
  if (field === 'price' && value <= 0) {
    // Invalid price
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Price must be positive'
    });
    // Revert the value
    data[field] = this.originalProduct[data.id][field];
  } else {
    // Valid edit, store original for potential cancellation
    if (!this.originalProduct[data.id]) {
      this.originalProduct[data.id] = {...data};
    }
    
    // Update the product in the backend
    this.productService.update(data)
      .subscribe(
        () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: `Product ${field} updated`
          });
        },
        error => {
          // Revert on error
          data[field] = this.originalProduct[data.id][field];
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to update product'
          });
        }
      );
  }
}
```

## Column Toggling and Reordering

Allow users to customize which columns are visible:

### Column Toggle

```html
<p-multiSelect 
  [options]="cols" 
  [(ngModel)]="selectedColumns" 
  optionLabel="header" 
  selectedItemsLabel="{0} columns selected" 
  [style]="{minWidth: '200px'}"
  placeholder="Choose Columns">
</p-multiSelect>

<p-table [value]="products" [columns]="selectedColumns">
  <ng-template pTemplate="header" let-columns>
    <tr>
      <th *ngFor="let col of columns">
        {{col.header}}
      </th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product let-columns="columns">
    <tr>
      <td *ngFor="let col of columns">
        {{product[col.field]}}
      </td>
    </tr>
  </ng-template>
</p-table>
```

```typescript
cols: any[] = [
  { field: 'code', header: 'Code' },
  { field: 'name', header: 'Name' },
  { field: 'category', header: 'Category' },
  { field: 'quantity', header: 'Quantity' },
  { field: 'price', header: 'Price' },
  { field: 'rating', header: 'Rating' }
];

selectedColumns: any[] = this.cols; // By default, all columns are selected
```

### Column Reordering

Allow users to reorder columns with drag and drop:

```html
<p-table [value]="products" [reorderableColumns]="true">
  <ng-template pTemplate="header">
    <tr>
      <th pReorderableColumn>Code</th>
      <th pReorderableColumn>Name</th>
      <th pReorderableColumn>Category</th>
      <th pReorderableColumn>Price</th>
    </tr>
  </ng-template>
  <!-- Body template -->
</p-table>
```

## Table State Persistence

Saving and restoring table state improves user experience:

```typescript
import { Table } from 'primeng/table';

@ViewChild('dt') table: Table;

// Save table state when component is destroyed
ngOnDestroy() {
  this.saveTableState();
}

// Save state on demand or periodically
saveTableState() {
  const state = {
    first: this.table.first,
    rows: this.table.rows,
    sortField: this.table.sortField,
    sortOrder: this.table.sortOrder,
    filters: this.table.filters,
    multiSortMeta: this.table.multiSortMeta,
    selection: this.selectedProducts,
    columnWidths: this.getColumnWidths(),
    columnOrder: this.getColumnOrder(),
    visibleColumns: this.selectedColumns.map(col => col.field)
  };
  
  localStorage.setItem('productTableState', JSON.stringify(state));
}

// Restore state when component initializes
restoreTableState() {
  const stateString = localStorage.getItem('productTableState');
  if (stateString) {
    const state = JSON.parse(stateString);
    
    // Apply pagination state
    this.table.first = state.first;
    this.table.rows = state.rows;
    
    // Apply sorting state
    this.table.sortField = state.sortField;
    this.table.sortOrder = state.sortOrder;
    this.table.multiSortMeta = state.multiSortMeta;
    
    // Apply filters
    this.table.filters = state.filters;
    
    // Restore selection
    this.selectedProducts = state.selection;
    
    // Restore visible columns
    this.selectedColumns = this.cols.filter(col => 
      state.visibleColumns.includes(col.field)
    );
  }
}

getColumnWidths() {
  // Get the current column widths from the DOM
  const headers = this.el.nativeElement.querySelectorAll('.p-datatable-thead > tr > th');
  const widths = {};
  
  headers.forEach((header, index) => {
    const field = this.selectedColumns[index].field;
    widths[field] = header.offsetWidth;
  });
  
  return widths;
}

getColumnOrder() {
  // Get the current column order
  return this.selectedColumns.map(col => col.field);
}
```

## Advanced Export Options

Enable exporting table data to different formats:

### Export to Excel

```typescript
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';

exportExcel() {
  // Convert data to worksheet
  const worksheet = XLSX.utils.json_to_sheet(this.products);
  
  // Create workbook and add the worksheet
  const workbook = { Sheets: { 'Products': worksheet }, SheetNames: ['Products'] };
  
  // Generate Excel file buffer
  const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
  
  // Save file
  this.saveAsFile(excelBuffer, "ProductList.xlsx");
}

saveAsFile(buffer: any, fileName: string): void {
  let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
  let blob = new Blob([buffer], { type: EXCEL_TYPE });
  FileSaver.saveAs(blob, fileName);
}
```

### Export to PDF

Using jsPDF and html2canvas:

```typescript
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

exportPDF() {
  // Get the table element
  const element = document.getElementById('product-table');
  
  // Create a canvas from the table
  html2canvas(element).then((canvas) => {
    const imgData = canvas.toDataURL('image/png');
    const pdf = new jsPDF('p', 'mm', 'a4');
    
    // Calculate dimensions
    const imgWidth = 210;  // A4 width in mm
    const pageHeight = 297; // A4 height in mm
    const imgHeight = canvas.height * imgWidth / canvas.width;
    let heightLeft = imgHeight;
    let position = 0;

    // Add image to PDF
    pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
    heightLeft -= pageHeight;

    // Add new pages if needed for large tables
    while (heightLeft >= 0) {
      position = heightLeft - imgHeight;
      pdf.addPage();
      pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
      heightLeft -= pageHeight;
    }
    
    // Save the PDF
    pdf.save('ProductList.pdf');
  });
}
```

### Export to CSV

Using a simple converter:

```typescript
exportCSV() {
  // Convert data to CSV format
  const replacer = (key, value) => value === null ? '' : value;
  const header = Object.keys(this.products[0]);
  let csv = this.products.map(row => header.map(fieldName => 
    JSON.stringify(row[fieldName], replacer)).join(','));
  csv.unshift(header.join(','));
  
  // Create CSV content
  const csvArray = csv.join('\r\n');
  
  // Create a Blob and download
  const blob = new Blob([csvArray], {type: 'text/csv'});
  const url = window.URL.createObjectURL(blob);
  
  // Create temporary link and trigger download
  const a = document.createElement('a');
  a.setAttribute('style', 'display:none;');
  a.href = url;
  a.download = 'ProductList.csv';
  document.body.appendChild(a);
  a.click();
  window.URL.revokeObjectURL(url);
  document.body.removeChild(a);
}
```

## Practical Exercise: Product Inventory System

Let's create a comprehensive product inventory system that incorporates many of the advanced features we've covered:

```typescript
// product-inventory.component.ts
import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table, TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { InputNumberModule } from 'primeng/inputnumber';
import { ContextMenuModule } from 'primeng/contextmenu';
import { ToastModule } from 'primeng/toast';
import { MessageService, MenuItem } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';

interface Product {
  id: number;
  code: string;
  name: string;
  description: string;
  category: string;
  price: number;
  quantity: number;
  inventoryStatus: string;
  rating: number;
  date: Date;
}

interface Column {
  field: string;
  header: string;
  dataType?: string;
}

@Component({
  selector: 'app-product-inventory',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    TableModule, 
    ButtonModule, 
    InputTextModule, 
    MultiSelectModule,
    DropdownModule,
    CalendarModule,
    InputNumberModule,
    ContextMenuModule,
    ToastModule,
    TagModule
  ],
  providers: [MessageService],
  templateUrl: './product-inventory.component.html',
  styleUrls: ['./product-inventory.component.scss']
})
export class ProductInventoryComponent implements OnInit, OnDestroy {
  @ViewChild('dt') table: Table;

  products: Product[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  selectedProducts: Product[] = [];
  selectedProduct: Product;
  clonedProducts: { [s: string]: Product } = {};
  contextMenuItems: MenuItem[] = [];
  
  globalFilterValue: string = '';
  dateRangeFilter: Date[] = [];
  
  cols: Column[] = [
    { field: 'code', header: 'Code', dataType: 'text' },
    { field: 'name', header: 'Name', dataType: 'text' },
    { field: 'description', header: 'Description', dataType: 'text' },
    { field: 'category', header: 'Category', dataType: 'text' },
    { field: 'price', header: 'Price', dataType: 'numeric' },
    { field: 'quantity', header: 'Quantity', dataType: 'numeric' },
    { field: 'inventoryStatus', header: 'Status', dataType: 'text' },
    { field: 'rating', header: 'Rating', dataType: 'numeric' },
    { field: 'date', header: 'Date', dataType: 'date' }
  ];
  selectedColumns: Column[] = [];
  
  categories: any[] = [
    { label: 'Electronics', value: 'Electronics' },
    { label: 'Clothing', value: 'Clothing' },
    { label: 'Fitness', value: 'Fitness' },
    { label: 'Home', value: 'Home' }
  ];
  
  statuses: any[] = [
    { label: 'In Stock', value: 'INSTOCK' },
    { label: 'Low Stock', value: 'LOWSTOCK' },
    { label: 'Out of Stock', value: 'OUTOFSTOCK' }
  ];
  
  constructor(private messageService: MessageService) {}
  
  ngOnInit() {
    // Set selected columns to all columns by default
    this.selectedColumns = this.cols;
    
    // Load products
    this.loadProducts({ first: 0, rows: 10 });
    
    // Initialize context menu
    this.contextMenuItems = [
      { label: 'View', icon: 'pi pi-eye', command: () => this.viewProduct(this.selectedProduct) },
      { label: 'Edit', icon: 'pi pi-pencil', command: () => this.editProduct(this.selectedProduct) },
      { label: 'Delete', icon: 'pi pi-trash', command: () => this.deleteProduct(this.selectedProduct) }
    ];
    
    // Try to restore table state
    this.restoreTableState();
  }
  
  ngOnDestroy() {
    // Save table state when component is destroyed
    this.saveTableState();
  }
  
  loadProducts(event: any) {
    this.loading = true;
    
    // In a real application, this would be an API call with filters, sorting, etc.
    setTimeout(() => {
      // Generate mock data for demo
      const mockData: Product[] = [];
      const categories = ['Electronics', 'Clothing', 'Fitness', 'Home'];
      const statuses = ['INSTOCK', 'LOWSTOCK', 'OUTOFSTOCK'];
      
      for (let i = 0; i < 100; i++) {
        const category = categories[Math.floor(Math.random() * categories.length)];
        const status = statuses[Math.floor(Math.random() * statuses.length)];
        
        mockData.push({
          id: i + 1,
          code: `PROD-${1000 + i}`,
          name: `Product ${i + 1}`,
          description: `Description for Product ${i + 1}`,
          category: category,
          price: Math.floor(Math.random() * 1000) + 1,
          quantity: Math.floor(Math.random() * 100),
          inventoryStatus: status,
          rating: Math.floor(Math.random() * 5) + 1,
          date: new Date(2023, Math.floor(Math.random() * 12), Math.floor(Math.random() * 28) + 1)
        });
      }
      
      // Apply filtering, sorting, pagination here based on event parameters
      // For demo, just slice the array for pagination
      const startIndex = event.first;
      const endIndex = startIndex + event.rows;
      this.products = mockData.slice(startIndex, endIndex);
      this.totalRecords = mockData.length;
      this.loading = false;
    }, 1000);
  }
  
  onGlobalFilter(event: Event) {
    this.table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
    this.globalFilterValue = (event.target as HTMLInputElement).value;
  }
  
  onRowSelect(event: any) {
    this.messageService.add({
      severity: 'info',
      summary: 'Product Selected',
      detail: `You selected ${event.data.name}`
    });
  }
  
  onRowEditInit(product: Product) {
    this.clonedProducts[product.id] = {...product};
  }
  
  onRowEditSave(product: Product) {
    if (product.price > 0) {
      delete this.clonedProducts[product.id];
      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Product is updated'
      });
    } else {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Invalid Price'
      });
    }
  }
  
  onRowEditCancel(product: Product, index: number) {
    this.products[index] = this.clonedProducts[product.id];
    delete this.clonedProducts[product.id];
  }
  
  viewProduct(product: Product) {
    this.messageService.add({
      severity: 'info',
      summary: 'Product Details',
      detail: `Viewing ${product.name}`
    });
  }
  
  editProduct(product: Product) {
    this.messageService.add({
      severity: 'info',
      summary: 'Edit Product',
      detail: `Editing ${product.name}`
    });
  }
  
  deleteProduct(product: Product) {
    this.products = this.products.filter(p => p.id !== product.id);
    this.messageService.add({
      severity: 'warn',
      summary: 'Product Deleted',
      detail: `${product.name} has been removed`
    });
  }
  
  deleteSelectedProducts() {
    if (this.selectedProducts && this.selectedProducts.length > 0) {
      const selectedIds = this.selectedProducts.map(product => product.id);
      this.products = this.products.filter(product => !selectedIds.includes(product.id));
      
      this.messageService.add({
        severity: 'warn',
        summary: 'Products Deleted',
        detail: `${this.selectedProducts.length} products have been removed`
      });
      
      this.selectedProducts = [];
    } else {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'No products selected'
      });
    }
  }
  
  saveTableState() {
    if (this.table) {
      const state = {
        first: this.table.first,
        rows: this.table.rows,
        sortField: this.table.sortField,
        sortOrder: this.table.sortOrder,
        filters: this.table.filters,
        multiSortMeta: this.table.multiSortMeta,
        globalFilter: this.globalFilterValue,
        selectedColumns: this.selectedColumns.map(col => col.field)
      };
      
      localStorage.setItem('productTableState', JSON.stringify(state));
    }
  }
  
  restoreTableState() {
    const stateString = localStorage.getItem('productTableState');
    if (stateString && this.table) {
      const state = JSON.parse(stateString);
      
      // Apply pagination state
      setTimeout(() => {
        this.table.first = state.first;
        this.table.rows = state.rows;
        
        // Apply sorting state
        this.table.sortField = state.sortField;
        this.table.sortOrder = state.sortOrder;
        this.table.multiSortMeta = state.multiSortMeta;
        
        // Apply filters
        this.table.filters = state.filters;
        
        // Apply global filter
        this.globalFilterValue = state.globalFilter;
        if (this.globalFilterValue) {
          this.table.filterGlobal(this.globalFilterValue, 'contains');
        }
        
        // Restore visible columns
        if (state.selectedColumns) {
          this.selectedColumns = this.cols.filter(col => 
            state.selectedColumns.includes(col.field)
          );
        }
      }, 0);
    }
  }
  
  exportExcel() {
    import('xlsx').then(xlsx => {
      // Get all data for export, not just current page
      const allProducts = [...this.products]; // In a real app, you'd fetch all filtered products
      
      // Map data to only include visible columns
      const exportData = allProducts.map(product => {
        const obj: any = {};
        this.selectedColumns.forEach(col => {
          obj[col.header] = product[col.field];
        });
        return obj;
      });
      
      const worksheet = xlsx.utils.json_to_sheet(exportData);
      const workbook = { Sheets: { 'Products': worksheet }, SheetNames: ['Products'] };
      const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
      
      this.saveAsExcelFile(excelBuffer, 'products');
    });
  }
  
  saveAsExcelFile(buffer: any, fileName: string): void {
    let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    let EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], { type: EXCEL_TYPE });
    FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
  }
  
  getSeverity(status: string) {
    switch (status) {
      case 'INSTOCK':
        return 'success';
      case 'LOWSTOCK':
        return 'warning';
      case 'OUTOFSTOCK':
        return 'danger';
      default:
        return null;
    }
  }
}
```

```html
<!-- product-inventory.component.html -->
<div class="card">
  <p-toast></p-toast>
  <p-contextMenu #cm [model]="contextMenuItems"></p-contextMenu>
  
  <div class="p-grid p-fluid">
    <div class="p-col-12">
      <div class="p-card">
        <div class="p-card-title">
          <h2>Product Inventory</h2>
        </div>
        <div class="p-card-body">
          <!-- Table Controls -->
          <div class="p-grid p-mb-4">
            <div class="p-col-12 p-md-6 p-lg-4">
              <span class="p-input-icon-left">
                <i class="pi pi-search"></i>
                <input 
                  pInputText 
                  type="text" 
                  placeholder="Global Search..." 
                  [(ngModel)]="globalFilterValue" 
                  (input)="onGlobalFilter($event)" />
              </span>
            </div>
            <div class="p-col-12 p-md-6 p-lg-5">
              <p-multiSelect 
                [options]="cols" 
                [(ngModel)]="selectedColumns" 
                optionLabel="header" 
                selectedItemsLabel="{0} columns selected" 
                [style]="{minWidth: '200px'}"
                placeholder="Choose Columns">
              </p-multiSelect>
            </div>
            <div class="p-col-12 p-md-12 p-lg-3 text-right">
              <button 
                pButton 
                type="button" 
                label="Export Excel" 
                icon="pi pi-file-excel"
                class="p-button-success p-mr-2"
                (click)="exportExcel()">
              </button>
              <button 
                pButton 
                type="button" 
                label="Delete Selected" 
                icon="pi pi-trash"
                class="p-button-danger"
                [disabled]="!selectedProducts || selectedProducts.length === 0"
                (click)="deleteSelectedProducts()">
              </button>
            </div>
          </div>
          
          <!-- Data Table -->
          <p-table 
            #dt 
            [value]="products" 
            [columns]="selectedColumns"
            [lazy]="true" 
            [paginator]="true" 
            [rows]="10"
            [totalRecords]="totalRecords" 
            [rowsPerPageOptions]="[10, 25, 50]"
            [showCurrentPageReport]="true" 
            currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
            [loading]="loading"
            [(selection)]="selectedProducts" 
            [contextMenu]="cm"
            [(contextMenuSelection)]="selectedProduct"
            [globalFilterFields]="['name', 'code', 'category', 'description']"
            (onLazyLoad)="loadProducts($event)"
            (onRowSelect)="onRowSelect($event)"
            [reorderableColumns]="true"
            [resizableColumns]="true"
            styleClass="p-datatable-sm"
            [rowHover]="true"
            dataKey="id"
            editMode="row">
            
            <!-- Caption Template -->
            <ng-template pTemplate="caption">
              <div class="table-header">
                Product List
              </div>
            </ng-template>
            
            <!-- Column Header Template -->
            <ng-template pTemplate="header" let-columns>
              <tr>
                <th style="width: 3rem">
                  <p-tableHeaderCheckbox></p-tableHeaderCheckbox>
                </th>
                <th *ngFor="let col of columns" 
                    [pSortableColumn]="col.field" 
                    pReorderableColumn
                    [style.max-width]="col.field === 'description' ? '200px' : 'auto'">
                  {{col.header}}
                  <p-sortIcon [field]="col.field"></p-sortIcon>
                  <p-columnFilter 
                    *ngIf="col.field" 
                    [field]="col.field" 
                    [matchMode]="col.dataType === 'date' ? 'dateIs' : 'contains'"
                    [type]="col.dataType === 'numeric' ? 'numeric' : 
                           col.dataType === 'date' ? 'date' : 'text'" 
                    display="menu"
                    [showMatchModes]="col.dataType !== 'date'">
                    <ng-template 
                      pTemplate="filter" 
                      let-value 
                      let-filter="filterCallback"
                      *ngIf="col.field === 'category'">
                      <p-dropdown 
                        [options]="categories" 
                        [style]="{'min-width':'12rem'}" 
                        [showClear]="true"
                        placeholder="Any" 
                        (onChange)="filter($event.value)">
                      </p-dropdown>
                    </ng-template>
                    <ng-template 
                      pTemplate="filter" 
                      let-value 
                      let-filter="filterCallback"
                      *ngIf="col.field === 'inventoryStatus'">
                      <p-dropdown 
                        [options]="statuses" 
                        [style]="{'min-width':'12rem'}" 
                        [showClear]="true"
                        placeholder="Any" 
                        (onChange)="filter($event.value)">
                      </p-dropdown>
                    </ng-template>
                  </p-columnFilter>
                </th>
                <th style="width: 8rem">Actions</th>
              </tr>
            </ng-template>
            
            <!-- Row Template -->
            <ng-template pTemplate="body" let-product let-editing="editing" let-ri="rowIndex" let-columns="columns">
              <tr [pSelectableRow]="product" [pContextMenuRow]="product" [pEditableRow]="product">
                <td>
                  <p-tableCheckbox [value]="product"></p-tableCheckbox>
                </td>
                <td *ngFor="let col of columns">
                  <p-cellEditor>
                    <ng-template pTemplate="input">
                      <!-- Input template based on data type -->
                      <input 
                        pInputText 
                        *ngIf="col.dataType === 'text'" 
                        [(ngModel)]="product[col.field]" 
                        [style.width]="'100%'" />
                      
                      <p-inputNumber 
                        *ngIf="col.dataType === 'numeric'" 
                        [(ngModel)]="product[col.field]" 
                        [showButtons]="col.field === 'quantity'"
                        [min]="0"
                        [style.width]="'100%'">
                      </p-inputNumber>
                      
                      <p-calendar 
                        *ngIf="col.dataType === 'date'" 
                        [(ngModel)]="product[col.field]" 
                        [showTime]="true" 
                        [style.width]="'100%'">
                      </p-calendar>
                      
                      <p-dropdown 
                        *ngIf="col.field === 'category'" 
                        [options]="categories" 
                        [(ngModel)]="product[col.field]" 
                        optionValue="value" 
                        optionLabel="label"
                        [style.width]="'100%'">
                      </p-dropdown>
                      
                      <p-dropdown 
                        *ngIf="col.field === 'inventoryStatus'" 
                        [options]="statuses" 
                        [(ngModel)]="product[col.field]" 
                        optionValue="value" 
                        optionLabel="label"
                        [style.width]="'100%'">
                      </p-dropdown>
                    </ng-template>
                    <ng-template pTemplate="output">
                      <!-- Output template based on field and data type -->
                      <span *ngIf="col.field !== 'price' && 
                                   col.field !== 'date' && 
                                   col.field !== 'inventoryStatus'">
                        {{product[col.field]}}
                      </span>
                      
                      <span *ngIf="col.field === 'price'">
                        {{product[col.field] | currency:'USD'}}
                      </span>
                      
                      <span *ngIf="col.field === 'date'">
                        {{product[col.field] | date:'medium'}}
                      </span>
                      
                      <p-tag 
                        *ngIf="col.field === 'inventoryStatus'" 
                        [value]="product[col.field] === 'INSTOCK' ? 'In Stock' : 
                                product[col.field] === 'LOWSTOCK' ? 'Low Stock' : 'Out of Stock'" 
                        [severity]="getSeverity(product[col.field])">
                      </p-tag>
                    </ng-template>
                  </p-cellEditor>
                </td>
                <td>
                  <div class="p-d-flex">
                    <button 
                      pButton 
                      pRipple 
                      type="button" 
                      pInitEditableRow
                      icon="pi pi-pencil" 
                      class="p-button-rounded p-button-text p-mr-2"
                      (click)="onRowEditInit(product)" 
                      *ngIf="!editing">
                    </button>
                    <button 
                      pButton 
                      pRipple 
                      type="button" 
                      pSaveEditableRow
                      icon="pi pi-check" 
                      class="p-button-rounded p-button-text p-button-success p-mr-2"
                      (click)="onRowEditSave(product)" 
                      *ngIf="editing">
                    </button>
                    <button 
                      pButton 
                      pRipple 
                      type="button" 
                      pCancelEditableRow
                      icon="pi pi-times" 
                      class="p-button-rounded p-button-text p-button-danger"
                      (click)="onRowEditCancel(product, ri)" 
                      *ngIf="editing">
                    </button>
                  </div>
                </td>
              </tr>
            </ng-template>
            
            <!-- Empty Message Template -->
            <ng-template pTemplate="emptymessage">
              <tr>
                <td colspan="9">No products found.</td>
              </tr>
            </ng-template>
            
            <!-- Summary Template -->
            <ng-template pTemplate="summary">
              <div class="p-d-flex p-jc-between p-ai-center">
                <div *ngIf="selectedProducts.length > 0">
                  <span>{{selectedProducts.length}} products selected</span>
                </div>
                <div>
                  <span>Total Products: {{totalRecords}}</span>
                </div>
              </div>
            </ng-template>
          </p-table>
        </div>
      </div>
    </div>
  </div>
</div>
```

```scss
/* product-inventory.component.scss */
:host ::ng-deep {
  .p-datatable {
    .p-datatable-header {
      background: #f8f9fa;
    }
    
    .p-datatable-thead > tr > th {
      background: #f8f9fa;
      color: #495057;
      font-weight: 600;
      padding: 0.75rem;
    }
    
    .p-datatable-tbody > tr > td {
      padding: 0.75rem;
    }
    
    .p-row-editing {
      background-color: #e6f7ff !important;
    }
    
    .p-column-filter-menu-button {
      width: 2rem;
      height: 2rem;
    }
  }
  
  .p-button {
    margin-right: 0.5rem;
  }
}

.card {
  padding: 1rem;
  border-radius: 6px;
  margin-bottom: 2rem;
  background: white;
  box-shadow: 0 2px 4px rgba(0,0,0,.1);
}

.table-header {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.text-right {
  text-align: right;
}
```

## Best Practices

When working with advanced Table features, follow these best practices:

### 1. Performance Optimization

- **Lazy Loading**: Always use lazy loading for large datasets (>100 rows)
- **Debounce Filters**: Add debounce to filter inputs to reduce API calls
- **Virtualization**: Use virtual scrolling for extremely large datasets
- **Selective Rendering**: Only render visible columns

```typescript
// Debounce example for filters
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';

// In component
filterSubject = new Subject<string>();

ngOnInit() {
  this.filterSubject
    .pipe(
      debounceTime(400),
      distinctUntilChanged()
    )
    .subscribe(value => {
      this.table.filterGlobal(value, 'contains');
    });
}

// Use this method for inputs
onFilterChange(event: Event) {
  const value = (event.target as HTMLInputElement).value;
  this.filterSubject.next(value);
}
```

### 2. Accessibility Improvements

- **Proper ARIA Attributes**: Ensure screen reader compatibility
- **Keyboard Navigation**: Implement keyboard shortcuts for common actions
- **Focus Management**: Maintain proper focus during interactions

```typescript
// Add keyboard shortcuts
@HostListener('document:keydown', ['$event'])
handleKeyboardEvent(event: KeyboardEvent) {
  // Ctrl+F for focus on filter
  if (event.ctrlKey && event.key === 'f') {
    event.preventDefault();
    this.filterInput.nativeElement.focus();
  }
  
  // Escape key to exit edit mode
  if (event.key === 'Escape' && this.editingRow) {
    this.cancelEdit();
  }
}
```

### 3. Error Handling

Handle errors gracefully, especially in server interactions:

```typescript
loadProducts(event: LazyLoadEvent) {
  this.loading = true;
  
  this.productService.getProducts(event)
    .pipe(
      catchError(error => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error Loading Data',
          detail: 'Could not load products. Please try again later.'
        });
        this.loading = false;
        return of({ data: [], total: 0 }); // Return empty result
      })
    )
    .subscribe(result => {
      this.products = result.data;
      this.totalRecords = result.total;
      this.loading = false;
    });
}
```

### 4. Configuration Reuse

Extract table configurations to make them reusable:

```typescript
// table-config.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TableConfigService {
  getDefaultConfig() {
    return {
      rows: 10,
      rowsPerPageOptions: [5, 10, 25, 50],
      showCurrentPageReport: true,
      paginator: true,
      globalFilterFields: ['name', 'code', 'category', 'description'],
      sortMode: 'multiple',
      resizableColumns: true,
      reorderableColumns: true,
      rowHover: true,
      stripedRows: false
    };
  }
  
  getExportOptions() {
    return {
      csv: true,
      excel: true,
      pdf: false
    };
  }
}
```

## Key Takeaways

- **Advanced Pagination**: Implement both client-side and server-side pagination for optimal performance
- **Rich Filtering**: Leverage global, column, and custom filters to help users find data quickly
- **Flexible Sorting**: Provide single and multi-column sorting capabilities
- **Row Interactions**: Implement selection, context menus, and inline editing for enhanced user experience
- **State Management**: Save and restore table state for user convenience
- **Export Options**: Allow users to export data in various formats for external use
- **Performance Considerations**: Always optimize for large datasets using lazy loading and virtual scrolling

## Resources and Further Reading

- [PrimeNG Table Documentation - Advanced](https://primeng.org/table)
- [PrimeNG Filter Documentation](https://primeng.org/filterservice)
- [Exporting Data in Angular](https://www.primefaces.org/primeng-blog/data-export)
- [State Management in Angular Tables](https://blog.angular-university.io/angular-material-data-table/)
- [Performance Optimization for Large Datasets](https://blog.angular-university.io/angular-debugging/)
- [Table Accessibility Best Practices](https://www.w3.org/WAI/tutorials/tables/)

## Checklist for Practice

- [ ] Implement server-side pagination with lazy loading
- [ ] Create a table with multi-column sorting
- [ ] Implement global, column, and custom filters
- [ ] Add row selection with context menu
- [ ] Implement row and cell editing
- [ ] Enable column toggling and reordering
- [ ] Add table state persistence
- [ ] Implement export to Excel and CSV
- [ ] Create a custom filter component
- [ ] Optimize table performance for large datasets
- [ ] Add keyboard shortcuts for common actions

---

Next up in Day 23, we'll explore the DataView and DataScroller components, which provide alternative ways to display data with more flexible layouts!