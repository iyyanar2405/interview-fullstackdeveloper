# Day 21: PrimeNG Table - Basic Features

**Table of Contents:**
1. [Introduction to PrimeNG Table](#introduction-to-primeng-table)
2. [Setting Up Your Project](#setting-up-your-project)
3. [Table Structure and Templates](#table-structure-and-templates)
4. [Column Definition and Features](#column-definition-and-features)
5. [Data Binding Approaches](#data-binding-approaches)
6. [Row Features](#row-features)
7. [Styling Tables](#styling-tables)
8. [Practical Exercise: User Management Table](#practical-exercise-user-management-table)
9. [Best Practices](#best-practices)
10. [Key Takeaways](#key-takeaways)
11. [Resources and Further Reading](#resources-and-further-reading)

## Introduction to PrimeNG Table

The PrimeNG Table component is a powerful data visualization tool that displays data in a tabular format. It offers extensive features like sorting, filtering, pagination, row selection, column toggling, and more. This component is one of the most commonly used and important components in PrimeNG, making it essential to master for enterprise-level applications.

### Core Features:
- Responsive design
- Data binding through arrays or streams
- Dynamic and fixed columns
- Column templates for custom rendering
- Row selection (single, multiple)
- Row expansion
- Lazy loading for large datasets
- Export capabilities (CSV, Excel, PDF)
- Accessibility compliance

### Why Use PrimeNG Table?

Unlike basic HTML tables or simple Angular material tables, PrimeNG Table offers enterprise-grade functionality right out of the box:

- **Complete Solution:** No need for separate components for pagination, sorting, etc.
- **Flexibility:** Works with any data source and can be completely customized
- **Performance:** Optimized rendering for large datasets
- **Integration:** Works seamlessly with other PrimeNG components

## Setting Up Your Project

Before we dive into Table features, make sure you have a PrimeNG project set up with the required dependencies:

1. First, ensure you have the PrimeNG module imported in your app:

```typescript
// app.module.ts (for module-based approach)
import { TableModule } from 'primeng/table';

@NgModule({
  imports: [
    // other imports
    TableModule
  ],
  // ...
})
export class AppModule { }
```

Or for standalone components:

```typescript
// your component.ts
import { Component } from '@angular/core';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-table-demo',
  templateUrl: './table-demo.component.html',
  standalone: true,
  imports: [TableModule]
})
export class TableDemoComponent {
  // ...
}
```

2. Make sure to import the required CSS:

```typescript
// styles.scss
@import "primeng/resources/themes/lara-light-blue/theme.css";
@import "primeng/resources/primeng.css";
```

## Table Structure and Templates

The basic structure of a PrimeNG Table consists of:

1. The `<p-table>` component wrapper
2. Column definitions with `<ng-template pTemplate="header">` for headers
3. Row definitions with `<ng-template pTemplate="body">` for data rows

### Basic Table Structure:

```html
<p-table [value]="products">
  <ng-template pTemplate="header">
    <tr>
      <th>Code</th>
      <th>Name</th>
      <th>Category</th>
      <th>Price</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr>
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.category}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
```

### Available Templates:

The PrimeNG Table offers various templates for customizing different parts of the table:

- `header`: The column headers
- `body`: The data rows
- `footer`: The column footers
- `caption`: Table caption displayed on top
- `summary`: Table summary displayed at bottom
- `emptymessage`: Displayed when there is no data
- `loadingbody`: Displayed during data loading
- `expansion`: Used for expandable rows
- `groupheader`: Used for grouped data
- `groupfooter`: Footer for grouped data

### Example with Multiple Templates:

```html
<p-table [value]="products" [loading]="loading">
  <!-- Caption Template -->
  <ng-template pTemplate="caption">
    <div class="p-d-flex p-jc-between p-ai-center">
      <h3>Product Inventory</h3>
      <button pButton label="Refresh" icon="pi pi-refresh"></button>
    </div>
  </ng-template>
  
  <!-- Header Template -->
  <ng-template pTemplate="header">
    <tr>
      <th>Code</th>
      <th>Name</th>
      <th>Price</th>
      <th>Actions</th>
    </tr>
  </ng-template>
  
  <!-- Body Template -->
  <ng-template pTemplate="body" let-product>
    <tr>
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.price | currency:'USD'}}</td>
      <td>
        <button pButton icon="pi pi-pencil" class="p-button-sm"></button>
        <button pButton icon="pi pi-trash" class="p-button-danger p-button-sm"></button>
      </td>
    </tr>
  </ng-template>
  
  <!-- Empty Message Template -->
  <ng-template pTemplate="emptymessage">
    <tr>
      <td colspan="4" class="text-center">No products found</td>
    </tr>
  </ng-template>
  
  <!-- Loading Template -->
  <ng-template pTemplate="loadingbody">
    <tr>
      <td colspan="4" class="text-center">Loading products...</td>
    </tr>
  </ng-template>
  
  <!-- Summary Template -->
  <ng-template pTemplate="summary">
    <div>There are {{products ? products.length : 0}} products</div>
  </ng-template>
</p-table>
```

## Column Definition and Features

PrimeNG Tables allow you to define columns in two ways:

### 1. Using Direct Templates:

```html
<p-table [value]="products">
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
      <td>{{product.name}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
```

### 2. Using Dynamic Columns (p-column):

```html
<p-table [value]="products">
  <ng-template pTemplate="header">
    <tr>
      <th *ngFor="let col of columns">
        {{col.header}}
      </th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr>
      <td *ngFor="let col of columns">
        {{product[col.field]}}
      </td>
    </tr>
  </ng-template>
</p-table>
```

```typescript
export class TableDemoComponent {
  products: Product[];
  
  columns = [
    { field: 'code', header: 'Code' },
    { field: 'name', header: 'Name' },
    { field: 'price', header: 'Price' }
  ];
  
  // ...
}
```

### Column Features:

PrimeNG Tables provide several column features:

#### Frozen Columns

Columns can be frozen (fixed) at the left or right side:

```html
<p-table [value]="products" [scrollable]="true" scrollHeight="400px">
  <ng-template pTemplate="header">
    <tr>
      <th pFrozenColumn style="width: 100px">Code</th>
      <th>Name</th>
      <th>Category</th>
      <th>Quantity</th>
      <th pFrozenColumn alignFrozen="right" style="width: 100px">Price</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr>
      <td pFrozenColumn>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.category}}</td>
      <td>{{product.quantity}}</td>
      <td pFrozenColumn alignFrozen="right">{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
```

#### Column Groups

You can group columns with headers spanning multiple columns:

```html
<p-table [value]="products">
  <ng-template pTemplate="header">
    <tr>
      <th rowspan="2">Code</th>
      <th colspan="2">Product Details</th>
      <th rowspan="2">Price</th>
    </tr>
    <tr>
      <th>Name</th>
      <th>Category</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr>
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.category}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
```

#### Column Resizing

Enable column resizing:

```html
<p-table [value]="products" [resizableColumns]="true">
  <!-- ... -->
</p-table>
```

#### Column Reordering

Enable column reordering with drag and drop:

```html
<p-table [value]="products" [reorderableColumns]="true">
  <!-- ... -->
</p-table>
```

## Data Binding Approaches

PrimeNG Tables support multiple ways to bind data:

### 1. Static Array Binding:

```typescript
export class TableDemoComponent implements OnInit {
  products: Product[] = [
    { code: 'P100', name: 'Product 1', category: 'Electronics', price: 999 },
    { code: 'P101', name: 'Product 2', category: 'Home', price: 199 },
    // more products
  ];
}
```

```html
<p-table [value]="products">
  <!-- templates -->
</p-table>
```

### 2. Asynchronous Data Loading:

```typescript
export class TableDemoComponent implements OnInit {
  products: Product[];
  loading: boolean = true;
  
  constructor(private productService: ProductService) {}
  
  ngOnInit() {
    this.productService.getProducts().subscribe(
      data => {
        this.products = data;
        this.loading = false;
      },
      error => {
        console.error('Error loading products', error);
        this.loading = false;
      }
    );
  }
}
```

```html
<p-table [value]="products" [loading]="loading">
  <!-- templates -->
</p-table>
```

### 3. Observable Binding:

```typescript
export class TableDemoComponent implements OnInit {
  products$: Observable<Product[]>;
  
  constructor(private productService: ProductService) {}
  
  ngOnInit() {
    this.products$ = this.productService.getProducts();
  }
}
```

```html
<p-table [value]="products$ | async" [loading]="(products$ | async) === null">
  <!-- templates -->
</p-table>
```

### 4. Virtual Scrolling for Large Datasets:

For extremely large datasets, use virtual scrolling:

```html
<p-table [value]="products" [virtualScroll]="true" [virtualRowHeight]="34" 
         scrollHeight="400px" [rows]="20" [lazy]="true" (onLazyLoad)="loadProducts($event)">
  <!-- templates -->
</p-table>
```

```typescript
loadProducts(event: LazyLoadEvent) {
  // Load data based on event parameters (first, rows, etc.)
  this.productService.getProducts(event.first, event.rows)
    .subscribe(data => this.products = data);
}
```

## Row Features

PrimeNG Table offers numerous row-level features:

### Row Selection

#### Single Selection:

```html
<p-table [value]="products" [(selection)]="selectedProduct" 
         dataKey="code" selectionMode="single">
  <ng-template pTemplate="header">
    <tr>
      <th>Code</th>
      <th>Name</th>
      <th>Price</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr [pSelectableRow]="product">
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
<div *ngIf="selectedProduct">
  Selected Product: {{selectedProduct.name}}
</div>
```

#### Multiple Selection:

```html
<p-table [value]="products" [(selection)]="selectedProducts" 
         dataKey="code" selectionMode="multiple">
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
<div *ngIf="selectedProducts.length > 0">
  Selected Products: {{selectedProducts.length}}
</div>
```

### Row Expansion

Expand rows to show more details:

```html
<p-table [value]="products" dataKey="code">
  <ng-template pTemplate="header">
    <tr>
      <th style="width: 3rem"></th>
      <th>Code</th>
      <th>Name</th>
      <th>Price</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product let-expanded="expanded">
    <tr>
      <td>
        <button type="button" pButton pRipple [pRowToggler]="product" 
                class="p-button-text p-button-rounded p-button-plain" 
                [icon]="expanded ? 'pi pi-chevron-down' : 'pi pi-chevron-right'">
        </button>
      </td>
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
  <ng-template pTemplate="rowexpansion" let-product>
    <tr>
      <td colspan="4">
        <div class="p-p-3">
          <h4>Details for {{product.name}}</h4>
          <p>Category: {{product.category}}</p>
          <p>Description: {{product.description}}</p>
          <p>Inventory Status: {{product.inventoryStatus}}</p>
        </div>
      </td>
    </tr>
  </ng-template>
</p-table>
```

### Row Grouping

Group rows by a common property:

```html
<p-table [value]="products" sortField="category" sortMode="single" groupRowsBy="category">
  <ng-template pTemplate="header">
    <tr>
      <th>Code</th>
      <th>Name</th>
      <th>Category</th>
      <th>Price</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="groupheader" let-product>
    <tr>
      <td colspan="4">
        <span class="font-bold">Category: {{product.category}}</span>
      </td>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr>
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.category}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
```

### Row Styling

Apply conditional styling to rows:

```html
<p-table [value]="products">
  <ng-template pTemplate="body" let-product let-rowIndex="rowIndex">
    <tr [ngClass]="{'p-highlight': product.price > 1000, 'out-of-stock': product.quantity <= 0, 'row-alternate': rowIndex % 2 !== 0}">
      <td>{{product.code}}</td>
      <td>{{product.name}}</td>
      <td>{{product.price | currency:'USD'}}</td>
    </tr>
  </ng-template>
</p-table>
```

```css
/* in your component CSS */
.out-of-stock {
  background-color: #ffcdd2 !important;
  color: #c63737 !important;
}

.row-alternate {
  background-color: #f8f9fa;
}

/* The p-highlight class is built-in to PrimeNG */
```

## Styling Tables

PrimeNG Tables offer several styling options:

### 1. Table Size Variants:

```html
<!-- Small -->
<p-table [value]="products" styleClass="p-datatable-sm">
  <!-- templates -->
</p-table>

<!-- Default -->
<p-table [value]="products">
  <!-- templates -->
</p-table>

<!-- Large -->
<p-table [value]="products" styleClass="p-datatable-lg">
  <!-- templates -->
</p-table>
```

### 2. Grid Lines:

```html
<!-- With grid lines (default) -->
<p-table [value]="products">
  <!-- templates -->
</p-table>

<!-- Without grid lines -->
<p-table [value]="products" [showGridlines]="false">
  <!-- templates -->
</p-table>
```

### 3. Striped Rows:

```html
<p-table [value]="products" styleClass="p-datatable-striped">
  <!-- templates -->
</p-table>
```

### 4. Row Hover:

```html
<p-table [value]="products" [rowHover]="true">
  <!-- templates -->
</p-table>
```

### 5. Custom Classes:

```html
<p-table [value]="products" styleClass="my-custom-table">
  <!-- templates -->
</p-table>
```

```css
/* In your component styles */
:host ::ng-deep .my-custom-table {
  /* Custom styles */
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

:host ::ng-deep .my-custom-table .p-datatable-thead > tr > th {
  background-color: #f1f5f9;
  color: #334155;
  font-weight: 600;
}
```

## Practical Exercise: User Management Table

Let's create a complete user management table with basic features:

```typescript
// user-table.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';

interface User {
  id: number;
  name: string;
  email: string;
  role: string;
  status: string;
  lastLogin: Date;
}

@Component({
  selector: 'app-user-table',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, FormsModule, TagModule],
  templateUrl: './user-table.component.html',
  styleUrls: ['./user-table.component.scss']
})
export class UserTableComponent implements OnInit {
  users: User[] = [];
  selectedUsers: User[] = [];
  globalFilterValue: string = '';
  loading: boolean = true;

  ngOnInit() {
    // Simulate API call with timeout
    setTimeout(() => {
      this.users = [
        {
          id: 1,
          name: 'John Smith',
          email: 'john.smith@example.com',
          role: 'Admin',
          status: 'Active',
          lastLogin: new Date('2023-10-10T08:30:00')
        },
        {
          id: 2,
          name: 'Sarah Johnson',
          email: 'sarah.j@example.com',
          role: 'Editor',
          status: 'Active',
          lastLogin: new Date('2023-10-09T14:45:00')
        },
        {
          id: 3,
          name: 'Michael Brown',
          email: 'mbrown@example.com',
          role: 'Viewer',
          status: 'Inactive',
          lastLogin: new Date('2023-09-28T11:20:00')
        },
        {
          id: 4,
          name: 'Emily Davis',
          email: 'emily.davis@example.com',
          role: 'Editor',
          status: 'Active',
          lastLogin: new Date('2023-10-10T10:15:00')
        },
        {
          id: 5,
          name: 'Robert Wilson',
          email: 'rwilson@example.com',
          role: 'Admin',
          status: 'Active',
          lastLogin: new Date('2023-10-08T16:30:00')
        },
        {
          id: 6,
          name: 'Jennifer Lee',
          email: 'jlee@example.com',
          role: 'Viewer',
          status: 'Pending',
          lastLogin: null
        }
      ];
      this.loading = false;
    }, 1000);
  }

  getSeverity(status: string) {
    switch (status) {
      case 'Active':
        return 'success';
      case 'Pending':
        return 'warning';
      case 'Inactive':
        return 'danger';
      default:
        return 'info';
    }
  }

  onGlobalFilter(event: Event) {
    this.globalFilterValue = (event.target as HTMLInputElement).value;
  }

  deleteUser(user: User) {
    // In a real app, you would call an API here
    this.users = this.users.filter(u => u.id !== user.id);
    alert(`User ${user.name} has been deleted`);
  }
}
```

```html
<!-- user-table.component.html -->
<div class="card">
  <h2>User Management</h2>
  
  <div class="p-d-flex p-jc-between p-mb-4">
    <div class="p-input-icon-left">
      <i class="pi pi-search"></i>
      <input 
        pInputText 
        type="text" 
        placeholder="Search users..." 
        (input)="onGlobalFilter($event)" 
      />
    </div>
    <button pButton label="Add New User" icon="pi pi-plus" class="p-button-success"></button>
  </div>
  
  <p-table 
    #dt 
    [value]="users" 
    [globalFilterFields]="['name', 'email', 'role']"
    [(selection)]="selectedUsers"
    [loading]="loading"
    dataKey="id"
    [rowHover]="true"
    styleClass="p-datatable-sm"
    [paginator]="true" 
    [rows]="5" 
    [showCurrentPageReport]="true" 
    currentPageReportTemplate="Showing {first} to {last} of {totalRecords} users"
  >
    <!-- Table Header -->
    <ng-template pTemplate="header">
      <tr>
        <th style="width: 3rem">
          <p-tableHeaderCheckbox></p-tableHeaderCheckbox>
        </th>
        <th pSortableColumn="name">Name <p-sortIcon field="name"></p-sortIcon></th>
        <th pSortableColumn="email">Email <p-sortIcon field="email"></p-sortIcon></th>
        <th pSortableColumn="role">Role <p-sortIcon field="role"></p-sortIcon></th>
        <th pSortableColumn="status">Status <p-sortIcon field="status"></p-sortIcon></th>
        <th pSortableColumn="lastLogin">Last Login <p-sortIcon field="lastLogin"></p-sortIcon></th>
        <th style="width: 8rem">Actions</th>
      </tr>
    </ng-template>
    
    <!-- Table Body -->
    <ng-template pTemplate="body" let-user>
      <tr>
        <td>
          <p-tableCheckbox [value]="user"></p-tableCheckbox>
        </td>
        <td>{{ user.name }}</td>
        <td>{{ user.email }}</td>
        <td>{{ user.role }}</td>
        <td>
          <p-tag 
            [value]="user.status" 
            [severity]="getSeverity(user.status)"
          ></p-tag>
        </td>
        <td>
          {{ user.lastLogin ? (user.lastLogin | date:'medium') : 'Never logged in' }}
        </td>
        <td>
          <button pButton pRipple icon="pi pi-pencil" class="p-button-rounded p-button-text p-mr-2"></button>
          <button pButton pRipple icon="pi pi-trash" class="p-button-rounded p-button-text p-button-danger" (click)="deleteUser(user)"></button>
        </td>
      </tr>
    </ng-template>
    
    <!-- Empty Message Template -->
    <ng-template pTemplate="emptymessage">
      <tr>
        <td colspan="7" class="text-center p-p-4">
          No users found. Try clearing your search or adding new users.
        </td>
      </tr>
    </ng-template>
    
    <!-- Loading Template -->
    <ng-template pTemplate="loadingbody">
      <tr>
        <td colspan="7" class="text-center p-p-4">
          Loading users... Please wait.
        </td>
      </tr>
    </ng-template>
    
    <!-- Summary -->
    <ng-template pTemplate="summary">
      <div class="p-d-flex p-jc-between p-ai-center">
        <div>
          <span *ngIf="selectedUsers.length > 0">
            {{selectedUsers.length}} users selected
          </span>
        </div>
        <div>
          Total: {{users.length}} users
        </div>
      </div>
    </ng-template>
  </p-table>
</div>
```

```scss
/* user-table.component.scss */
.card {
  padding: 1.5rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

h2 {
  margin-top: 0;
  margin-bottom: 1.5rem;
  color: #495057;
}

:host ::ng-deep .p-button {
  margin-right: 0.5rem;
}

:host ::ng-deep .p-datatable {
  .p-datatable-header {
    background-color: #ffffff;
    border: none;
    padding: 1rem;
  }
  
  .p-paginator {
    padding: 1rem;
  }
  
  .p-datatable-thead > tr > th {
    background-color: #f8f9fa;
    color: #495057;
    font-weight: 600;
  }
}

.text-center {
  text-align: center;
}
```

## Best Practices

When working with PrimeNG Tables, consider these best practices:

### 1. Use Reactive Forms for Inline Editing

```typescript
import { FormBuilder, FormGroup } from '@angular/forms';

// In component
formGroup: FormGroup;

constructor(private fb: FormBuilder) {
  this.formGroup = this.fb.group({
    // form controls
  });
}

onRowEditInit(product: Product) {
  this.formGroup.patchValue(product);
}

onRowEditSave() {
  // Save changes
}
```

### 2. Implement Lazy Loading for Large Datasets

For tables with thousands of records, always implement lazy loading to improve performance:

```typescript
loadLazy(event: LazyLoadEvent) {
  this.loading = true;
  
  // Calculate the data to fetch based on event.first and event.rows
  this.service.getProducts(event.first, event.rows, event.sortField, event.sortOrder).subscribe(
    res => {
      this.products = res.items;
      this.totalRecords = res.totalCount;
      this.loading = false;
    }
  );
}
```

### 3. Use Typed Data Models

Always define interfaces for your data:

```typescript
interface Product {
  id: number;
  code: string;
  name: string;
  description: string;
  price: number;
  category: string;
  quantity: number;
  inventoryStatus: string;
  rating: number;
}
```

### 4. Export Options

Implement export functionality for business applications:

```typescript
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';

// In component
export() {
  import('xlsx').then(xlsx => {
    const worksheet = xlsx.utils.json_to_sheet(this.products);
    const workbook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
    const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
    this.saveAsExcelFile(excelBuffer, 'products');
  });
}

saveAsExcelFile(buffer: any, fileName: string): void {
  import('file-saver').then(FileSaver => {
    let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    let EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], { type: EXCEL_TYPE });
    FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
  });
}
```

### 5. Handle State Persistence

For a better user experience, consider saving and restoring the table state:

```typescript
@ViewChild('dt') table: Table;

saveState() {
  localStorage.setItem('tableState', JSON.stringify({
    first: this.table.first,
    rows: this.table.rows,
    sortField: this.table.sortField,
    sortOrder: this.table.sortOrder,
    filters: this.table.filters,
    globalFilter: this.globalFilterValue
  }));
}

restoreState() {
  const state = JSON.parse(localStorage.getItem('tableState'));
  if (state) {
    this.table.first = state.first;
    this.table.rows = state.rows;
    this.table.sortField = state.sortField;
    this.table.sortOrder = state.sortOrder;
    this.table.filters = state.filters;
    this.globalFilterValue = state.globalFilter;
  }
}
```

## Key Takeaways

- PrimeNG Table is a feature-rich component that handles all aspects of tabular data display
- The component uses templates to provide maximum flexibility for customization
- Key features include sorting, filtering, pagination, selection, and row expansion
- For large datasets, use lazy loading and virtual scrolling
- Combine with other PrimeNG components like Buttons, InputText, and Dropdown for a complete UI

## Resources and Further Reading

- [PrimeNG Table Documentation](https://primeng.org/table)
- [PrimeNG Table Examples](https://primeng.org/table#examples)
- [PrimeNG TableState API](https://primeng.org/table#tablestate)
- [PrimeNG UI Kit for Figma](https://www.primefaces.org/designer/primeng)
- [PrimeNG Blog: Advanced Table Patterns](https://www.primefaces.org/primeng-blog)

## Checklist for Practice

- [ ] Create a basic table with static data
- [ ] Implement sorting and filtering
- [ ] Add pagination
- [ ] Implement row selection (single and multiple)
- [ ] Create expandable rows
- [ ] Style the table with different themes
- [ ] Implement lazy loading for a large dataset
- [ ] Add export functionality to Excel or CSV
- [ ] Create a custom filter component
- [ ] Implement inline editing
- [ ] Handle state persistence with localStorage

---

Next up in Day 22, we'll explore advanced Table features including pagination, filtering, sorting, row selection, and inline editing in greater depth!