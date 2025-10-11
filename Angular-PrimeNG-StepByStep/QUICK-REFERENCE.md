# PrimeNG Component Quick Reference Guide 📚

A comprehensive reference for all PrimeNG components with examples and common use cases.

## 📑 Table of Contents

1. [Form Components](#form-components)
2. [Button Components](#button-components)
3. [Data Components](#data-components)
4. [Panel Components](#panel-components)
5. [Overlay Components](#overlay-components)
6. [Menu Components](#menu-components)
7. [Chart Components](#chart-components)
8. [Messages Components](#messages-components)
9. [File Components](#file-components)
10. [Misc Components](#misc-components)

---

## 🎯 Form Components

### InputText
```typescript
import { InputTextModule } from 'primeng/inputtext';
```

```html
<!-- Basic input -->
<input type="text" pInputText placeholder="Enter text" />

<!-- With FormControl -->
<input type="text" pInputText [(ngModel)]="value" />

<!-- Disabled -->
<input type="text" pInputText [disabled]="true" />

<!-- With icon -->
<span class="p-input-icon-left">
  <i class="pi pi-search"></i>
  <input type="text" pInputText placeholder="Search" />
</span>
```

### Dropdown
```typescript
import { DropdownModule } from 'primeng/dropdown';
```

```html
<p-dropdown 
  [options]="cities" 
  [(ngModel)]="selectedCity"
  optionLabel="name"
  placeholder="Select a City">
</p-dropdown>
```

```typescript
cities = [
  { name: 'New York', code: 'NY' },
  { name: 'Rome', code: 'RM' },
  { name: 'London', code: 'LDN' }
];
```

### Calendar
```typescript
import { CalendarModule } from 'primeng/calendar';
```

```html
<!-- Date picker -->
<p-calendar [(ngModel)]="date" dateFormat="yy-mm-dd"></p-calendar>

<!-- Date range -->
<p-calendar [(ngModel)]="rangeDates" selectionMode="range"></p-calendar>

<!-- Time picker -->
<p-calendar [(ngModel)]="time" [timeOnly]="true"></p-calendar>

<!-- Date & time -->
<p-calendar [(ngModel)]="datetime" [showTime]="true"></p-calendar>
```

### Checkbox
```typescript
import { CheckboxModule } from 'primeng/checkbox';
```

```html
<!-- Binary -->
<p-checkbox [(ngModel)]="checked" [binary]="true" label="Accept"></p-checkbox>

<!-- Multiple -->
<p-checkbox 
  *ngFor="let category of categories" 
  [value]="category"
  [(ngModel)]="selectedCategories">
</p-checkbox>
```

### RadioButton
```typescript
import { RadioButtonModule } from 'primeng/radiobutton';
```

```html
<div *ngFor="let option of options">
  <p-radioButton 
    [value]="option.key"
    [(ngModel)]="selectedOption"
    [label]="option.name">
  </p-radioButton>
</div>
```

### InputNumber
```typescript
import { InputNumberModule } from 'primeng/inputnumber';
```

```html
<!-- Basic -->
<p-inputNumber [(ngModel)]="value"></p-inputNumber>

<!-- With min/max -->
<p-inputNumber [(ngModel)]="value" [min]="0" [max]="100"></p-inputNumber>

<!-- Currency -->
<p-inputNumber 
  [(ngModel)]="price" 
  mode="currency" 
  currency="USD">
</p-inputNumber>

<!-- Percentage -->
<p-inputNumber 
  [(ngModel)]="percentage" 
  [min]="0" 
  [max]="100" 
  suffix="%">
</p-inputNumber>
```

### MultiSelect
```typescript
import { MultiSelectModule } from 'primeng/multiselect';
```

```html
<p-multiSelect 
  [options]="cities"
  [(ngModel)]="selectedCities"
  optionLabel="name"
  placeholder="Choose Cities">
</p-multiSelect>
```

### AutoComplete
```typescript
import { AutoCompleteModule } from 'primeng/autocomplete';
```

```html
<p-autoComplete 
  [(ngModel)]="selectedCountry"
  [suggestions]="filteredCountries"
  (completeMethod)="filterCountry($event)"
  field="name">
</p-autoComplete>
```

```typescript
filterCountry(event: any) {
  this.filteredCountries = this.countries.filter(
    c => c.name.toLowerCase().includes(event.query.toLowerCase())
  );
}
```

### Textarea
```typescript
import { InputTextareaModule } from 'primeng/inputtextarea';
```

```html
<textarea pInputTextarea [(ngModel)]="text" rows="5" cols="30"></textarea>

<!-- Auto resize -->
<textarea 
  pInputTextarea 
  [(ngModel)]="text"
  [autoResize]="true">
</textarea>
```

---

## 🔘 Button Components

### Button
```typescript
import { ButtonModule } from 'primeng/button';
```

```html
<!-- Basic buttons -->
<p-button label="Primary"></p-button>
<p-button label="Secondary" styleClass="p-button-secondary"></p-button>
<p-button label="Success" styleClass="p-button-success"></p-button>
<p-button label="Info" styleClass="p-button-info"></p-button>
<p-button label="Warning" styleClass="p-button-warning"></p-button>
<p-button label="Danger" styleClass="p-button-danger"></p-button>

<!-- With icons -->
<p-button icon="pi pi-check" label="Save"></p-button>
<p-button icon="pi pi-times" label="Cancel" iconPos="right"></p-button>

<!-- Icon only -->
<p-button icon="pi pi-search"></p-button>

<!-- Rounded -->
<p-button icon="pi pi-check" styleClass="p-button-rounded"></p-button>

<!-- Text button -->
<p-button label="Text" styleClass="p-button-text"></p-button>

<!-- Outlined -->
<p-button label="Outlined" styleClass="p-button-outlined"></p-button>

<!-- Raised -->
<p-button label="Raised" styleClass="p-button-raised"></p-button>

<!-- Loading -->
<p-button label="Loading" [loading]="loading"></p-button>
```

### SplitButton
```typescript
import { SplitButtonModule } from 'primeng/splitbutton';
```

```html
<p-splitButton 
  label="Save"
  icon="pi pi-check"
  [model]="items"
  (onClick)="save()">
</p-splitButton>
```

```typescript
items = [
  { label: 'Update', icon: 'pi pi-refresh', command: () => this.update() },
  { label: 'Delete', icon: 'pi pi-times', command: () => this.delete() }
];
```

---

## 📊 Data Components

### Table
```typescript
import { TableModule } from 'primeng/table';
```

```html
<!-- Basic table -->
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
      <td>{{ product.code }}</td>
      <td>{{ product.name }}</td>
      <td>{{ product.price | currency }}</td>
    </tr>
  </ng-template>
</p-table>

<!-- With pagination -->
<p-table 
  [value]="products"
  [paginator]="true"
  [rows]="10"
  [rowsPerPageOptions]="[10, 25, 50]">
  <!-- ... -->
</p-table>

<!-- With sorting -->
<p-table [value]="products">
  <ng-template pTemplate="header">
    <tr>
      <th pSortableColumn="name">Name <p-sortIcon field="name"></p-sortIcon></th>
      <th pSortableColumn="price">Price <p-sortIcon field="price"></p-sortIcon></th>
    </tr>
  </ng-template>
  <!-- ... -->
</p-table>

<!-- With filtering -->
<p-table [value]="products" [globalFilterFields]="['name', 'category']">
  <ng-template pTemplate="caption">
    <input pInputText type="text" (input)="dt.filterGlobal($event.target.value, 'contains')" />
  </ng-template>
  <!-- ... -->
</p-table>

<!-- With selection -->
<p-table [value]="products" [(selection)]="selectedProducts">
  <ng-template pTemplate="header">
    <tr>
      <th style="width: 3rem">
        <p-tableHeaderCheckbox></p-tableHeaderCheckbox>
      </th>
      <th>Name</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-product>
    <tr>
      <td>
        <p-tableCheckbox [value]="product"></p-tableCheckbox>
      </td>
      <td>{{ product.name }}</td>
    </tr>
  </ng-template>
</p-table>
```

### DataView
```typescript
import { DataViewModule } from 'primeng/dataview';
```

```html
<p-dataView [value]="products" [paginator]="true" [rows]="9">
  <ng-template let-product pTemplate="listItem">
    <div class="product-list-item">
      <img [src]="'assets/images/' + product.image" [alt]="product.name" />
      <div class="product-list-detail">
        <div class="product-name">{{ product.name }}</div>
        <div class="product-description">{{ product.description }}</div>
      </div>
      <div class="product-list-action">
        <span class="product-price">${{ product.price }}</span>
        <p-button icon="pi pi-shopping-cart" label="Add to Cart"></p-button>
      </div>
    </div>
  </ng-template>
</p-dataView>
```

### Tree
```typescript
import { TreeModule } from 'primeng/tree';
```

```html
<p-tree [value]="files" selectionMode="single" [(selection)]="selectedFile"></p-tree>
```

```typescript
files = [
  {
    label: 'Documents',
    data: 'Documents Folder',
    expandedIcon: 'pi pi-folder-open',
    collapsedIcon: 'pi pi-folder',
    children: [
      { label: 'Work', data: 'Work Folder', icon: 'pi pi-fw pi-folder' },
      { label: 'Home', data: 'Home Folder', icon: 'pi pi-fw pi-folder' }
    ]
  }
];
```

### Paginator
```typescript
import { PaginatorModule } from 'primeng/paginator';
```

```html
<p-paginator 
  [rows]="10"
  [totalRecords]="120"
  [rowsPerPageOptions]="[10, 20, 30]"
  (onPageChange)="paginate($event)">
</p-paginator>
```

---

## 📦 Panel Components

### Card
```typescript
import { CardModule } from 'primeng/card';
```

```html
<p-card header="Simple Card" subheader="Subtitle">
  <p>Content goes here.</p>
  <ng-template pTemplate="footer">
    <p-button label="Save" icon="pi pi-check"></p-button>
  </ng-template>
</p-card>
```

### Panel
```typescript
import { PanelModule } from 'primeng/panel';
```

```html
<p-panel header="Header" [toggleable]="true">
  Content
</p-panel>
```

### Fieldset
```typescript
import { FieldsetModule } from 'primeng/fieldset';
```

```html
<p-fieldset legend="Legend" [toggleable]="true">
  Content
</p-fieldset>
```

### Accordion
```typescript
import { AccordionModule } from 'primeng/accordion';
```

```html
<p-accordion>
  <p-accordionTab header="Header 1">
    Content 1
  </p-accordionTab>
  <p-accordionTab header="Header 2">
    Content 2
  </p-accordionTab>
  <p-accordionTab header="Header 3">
    Content 3
  </p-accordionTab>
</p-accordion>
```

### TabView
```typescript
import { TabViewModule } from 'primeng/tabview';
```

```html
<p-tabView>
  <p-tabPanel header="Header 1">
    Content 1
  </p-tabPanel>
  <p-tabPanel header="Header 2">
    Content 2
  </p-tabPanel>
  <p-tabPanel header="Header 3">
    Content 3
  </p-tabPanel>
</p-tabView>
```

---

## 🎭 Overlay Components

### Dialog
```typescript
import { DialogModule } from 'primeng/dialog';
```

```html
<p-dialog 
  header="Dialog Title"
  [(visible)]="displayDialog"
  [modal]="true"
  [style]="{width: '50vw'}">
  <p>Dialog content goes here.</p>
  <ng-template pTemplate="footer">
    <p-button label="Save" icon="pi pi-check" (click)="displayDialog=false"></p-button>
  </ng-template>
</p-dialog>

<p-button label="Show Dialog" (click)="displayDialog=true"></p-button>
```

### ConfirmDialog
```typescript
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
```

```html
<p-confirmDialog header="Confirmation" icon="pi pi-exclamation-triangle"></p-confirmDialog>

<p-button (click)="confirm()" label="Delete"></p-button>
```

```typescript
constructor(private confirmationService: ConfirmationService) {}

confirm() {
  this.confirmationService.confirm({
    message: 'Are you sure you want to delete?',
    accept: () => {
      // Delete logic
    }
  });
}
```

### Sidebar
```typescript
import { SidebarModule } from 'primeng/sidebar';
```

```html
<p-sidebar [(visible)]="visibleSidebar">
  <h3>Sidebar</h3>
  <p>Content</p>
</p-sidebar>

<p-button (click)="visibleSidebar=true" icon="pi pi-arrow-right"></p-button>
```

### OverlayPanel
```typescript
import { OverlayPanelModule } from 'primeng/overlaypanel';
```

```html
<p-button (click)="op.toggle($event)" label="Toggle"></p-button>

<p-overlayPanel #op>
  <ng-template pTemplate>
    <p>Content</p>
  </ng-template>
</p-overlayPanel>
```

### Tooltip
```typescript
import { TooltipModule } from 'primeng/tooltip';
```

```html
<input type="text" pInputText pTooltip="Enter your username" />

<!-- Position -->
<input type="text" pInputText pTooltip="Tooltip text" tooltipPosition="top" />
```

---

## 🍔 Menu Components

### Menu
```typescript
import { MenuModule } from 'primeng/menu';
```

```html
<p-menu [model]="items"></p-menu>
```

```typescript
items = [
  { label: 'New', icon: 'pi pi-plus', command: () => this.create() },
  { label: 'Open', icon: 'pi pi-download', command: () => this.open() },
  { separator: true },
  { label: 'Quit', icon: 'pi pi-times', command: () => this.quit() }
];
```

### Menubar
```typescript
import { MenubarModule } from 'primeng/menubar';
```

```html
<p-menubar [model]="items">
  <ng-template pTemplate="start">
    <img src="logo.png" height="40" />
  </ng-template>
  <ng-template pTemplate="end">
    <input type="text" pInputText placeholder="Search" />
  </ng-template>
</p-menubar>
```

### TieredMenu
```typescript
import { TieredMenuModule } from 'primeng/tieredmenu';
```

```html
<p-tieredMenu [model]="items"></p-tieredMenu>
```

### Breadcrumb
```typescript
import { BreadcrumbModule } from 'primeng/breadcrumb';
```

```html
<p-breadcrumb [model]="items" [home]="home"></p-breadcrumb>
```

```typescript
items = [
  { label: 'Categories' },
  { label: 'Sports' },
  { label: 'Football' }
];
home = { icon: 'pi pi-home', url: '/' };
```

---

## 📈 Chart Components

```typescript
import { ChartModule } from 'primeng/chart';
```

```html
<!-- Line Chart -->
<p-chart type="line" [data]="data" [options]="options"></p-chart>

<!-- Bar Chart -->
<p-chart type="bar" [data]="data"></p-chart>

<!-- Pie Chart -->
<p-chart type="pie" [data]="data"></p-chart>

<!-- Doughnut Chart -->
<p-chart type="doughnut" [data]="data"></p-chart>
```

```typescript
data = {
  labels: ['January', 'February', 'March'],
  datasets: [
    {
      label: 'Sales',
      data: [65, 59, 80],
      backgroundColor: '#42A5F5'
    }
  ]
};
```

---

## 💬 Messages Components

### Toast
```typescript
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
```

```html
<p-toast></p-toast>
<p-button (click)="showSuccess()" label="Success"></p-button>
```

```typescript
constructor(private messageService: MessageService) {}

showSuccess() {
  this.messageService.add({
    severity: 'success',
    summary: 'Success',
    detail: 'Message sent'
  });
}
```

### Messages
```typescript
import { MessagesModule } from 'primeng/messages';
```

```html
<p-messages [(value)]="msgs"></p-messages>
```

```typescript
msgs = [
  { severity: 'info', summary: 'Info', detail: 'PrimeNG rocks' }
];
```

---

## 📎 File Components

### FileUpload
```typescript
import { FileUploadModule } from 'primeng/fileupload';
```

```html
<p-fileUpload 
  name="myfile[]"
  url="./upload.php"
  (onUpload)="onUpload($event)"
  multiple="multiple"
  accept="image/*"
  maxFileSize="1000000">
</p-fileUpload>
```

---

## 🎨 Misc Components

### ProgressBar
```typescript
import { ProgressBarModule } from 'primeng/progressbar';
```

```html
<p-progressBar [value]="value"></p-progressBar>
```

### ProgressSpinner
```typescript
import { ProgressSpinnerModule } from 'primeng/progressspinner';
```

```html
<p-progressSpinner></p-progressSpinner>
```

### Avatar
```typescript
import { AvatarModule } from 'primeng/avatar';
```

```html
<p-avatar label="P" size="large" shape="circle"></p-avatar>
<p-avatar image="user.png"></p-avatar>
<p-avatar icon="pi pi-user" styleClass="p-avatar-lg"></p-avatar>
```

### Badge
```typescript
import { BadgeModule } from 'primeng/badge';
```

```html
<i class="pi pi-bell" pBadge value="2"></i>
<p-badge value="8" severity="danger"></p-badge>
```

### Tag
```typescript
import { TagModule } from 'primeng/tag';
```

```html
<p-tag value="New" severity="success"></p-tag>
<p-tag value="Primary" severity="primary"></p-tag>
<p-tag icon="pi pi-check" value="Confirmed"></p-tag>
```

### Chip
```typescript
import { ChipModule } from 'primeng/chip';
```

```html
<p-chip label="Action"></p-chip>
<p-chip label="Comedy" icon="pi pi-star"></p-chip>
<p-chip label="Apple" image="avatar.png" removable="true"></p-chip>
```

---

## 🎯 Common Patterns

### Form with Validation
```html
<form [formGroup]="userForm" (ngSubmit)="onSubmit()">
  <div class="p-field">
    <label for="name">Name*</label>
    <input id="name" type="text" pInputText formControlName="name" />
    <small class="p-error" *ngIf="userForm.get('name').invalid && userForm.get('name').touched">
      Name is required
    </small>
  </div>
  
  <p-button type="submit" label="Submit" [disabled]="userForm.invalid"></p-button>
</form>
```

### CRUD Table
```html
<p-table [value]="products" [paginator]="true" [rows]="10">
  <ng-template pTemplate="caption">
    <div class="flex">
      <p-button icon="pi pi-plus" label="New" (click)="openNew()"></p-button>
    </div>
  </ng-template>
  
  <ng-template pTemplate="header">
    <tr>
      <th>Name</th>
      <th>Price</th>
      <th>Actions</th>
    </tr>
  </ng-template>
  
  <ng-template pTemplate="body" let-product>
    <tr>
      <td>{{ product.name }}</td>
      <td>{{ product.price | currency }}</td>
      <td>
        <p-button icon="pi pi-pencil" styleClass="p-button-rounded p-button-success" (click)="edit(product)"></p-button>
        <p-button icon="pi pi-trash" styleClass="p-button-rounded p-button-danger" (click)="delete(product)"></p-button>
      </td>
    </tr>
  </ng-template>
</p-table>
```

---

## 📚 Best Practices

1. **Import only needed modules** to reduce bundle size
2. **Use reactive forms** for complex forms
3. **Implement virtual scrolling** for large datasets
4. **Use lazy loading** for tables and data views
5. **Apply proper validation** with meaningful error messages
6. **Use tooltips** for better UX
7. **Implement confirm dialogs** for destructive actions
8. **Use toasts** for user feedback
9. **Apply consistent theming** across the app
10. **Follow Angular style guide** for component structure

---

This quick reference covers the most commonly used PrimeNG components. For detailed documentation, visit [PrimeNG Showcase](https://primeng.org/showcase).
