# Day 17-20: Panel Components & Navigation

## 📦 Day 17: Card, Panel & Fieldset

### Card Component
```typescript
import { CardModule } from 'primeng/card';
```

```html
<!-- Basic Card -->
<p-card header="Simple Card" subheader="Subtitle">
  <p>Card content goes here.</p>
  <ng-template pTemplate="footer">
    <p-button label="Save" icon="pi pi-check"></p-button>
    <p-button label="Cancel" severity="secondary"></p-button>
  </ng-template>
</p-card>

<!-- Card with Header Template -->
<p-card>
  <ng-template pTemplate="header">
    <img src="assets/card-header.jpg" alt="Header Image" />
  </ng-template>
  <h3>Custom Header</h3>
  <p>Content with custom header image</p>
</p-card>
```

### Panel Component
```typescript
import { PanelModule } from 'primeng/panel';
```

```html
<!-- Collapsible Panel -->
<p-panel header="Account Settings" [toggleable]="true">
  <p>Panel content can be toggled.</p>
</p-panel>

<!-- Panel with Icons -->
<p-panel header="User Profile" [toggleable]="true" [collapsed]="false">
  <ng-template pTemplate="icons">
    <button pButton icon="pi pi-cog" class="p-panel-header-icon"></button>
  </ng-template>
  <p>Profile information here.</p>
</p-panel>
```

### Fieldset Component
```typescript
import { FieldsetModule } from 'primeng/fieldset';
```

```html
<!-- Fieldset -->
<p-fieldset legend="Personal Information" [toggleable]="true">
  <p>Form fields go here.</p>
</p-fieldset>
```

---

## 🎨 Day 18: TabView & Accordion

### TabView
```typescript
import { TabViewModule } from 'primeng/tabview';
```

```html
<!-- Basic Tabs -->
<p-tabView>
  <p-tabPanel header="Profile">
    <p>Profile content</p>
  </p-tabPanel>
  <p-tabPanel header="Settings">
    <p>Settings content</p>
  </p-tabPanel>
  <p-tabPanel header="History">
    <p>History content</p>
  </p-tabPanel>
</p-tabView>

<!-- Tabs with Icons -->
<p-tabView>
  <p-tabPanel header="Dashboard" leftIcon="pi pi-chart-line">
    <p>Dashboard</p>
  </p-tabPanel>
  <p-tabPanel header="Users" leftIcon="pi pi-users">
    <p>Users</p>
  </p-tabPanel>
</p-tabView>

<!-- Closable Tabs -->
<p-tabView>
  <p-tabPanel *ngFor="let tab of tabs" [header]="tab.title" [closable]="true">
    {{ tab.content }}
  </p-tabPanel>
</p-tabView>
```

### Accordion
```typescript
import { AccordionModule } from 'primeng/accordion';
```

```html
<!-- Basic Accordion -->
<p-accordion>
  <p-accordionTab header="Header 1">
    <p>Content 1</p>
  </p-accordionTab>
  <p-accordionTab header="Header 2">
    <p>Content 2</p>
  </p-accordionTab>
  <p-accordionTab header="Header 3">
    <p>Content 3</p>
  </p-accordionTab>
</p-accordion>

<!-- Multiple Open -->
<p-accordion [multiple]="true">
  <p-accordionTab header="FAQ 1">
    <p>Answer 1</p>
  </p-accordionTab>
  <p-accordionTab header="FAQ 2">
    <p>Answer 2</p>
  </p-accordionTab>
</p-accordion>
```

---

## 📐 Day 19: Grid System & Divider

### PrimeNG Grid (FlexGrid)
```html
<!-- 12-column grid -->
<div class="grid">
  <div class="col-12 md:col-6 lg:col-4">
    <p-card>Column 1</p-card>
  </div>
  <div class="col-12 md:col-6 lg:col-4">
    <p-card>Column 2</p-card>
  </div>
  <div class="col-12 md:col-6 lg:col-4">
    <p-card>Column 3</p-card>
  </div>
</div>

<!-- Responsive Grid -->
<div class="grid">
  <div class="col-12 sm:col-6 md:col-4 lg:col-3">Item</div>
  <div class="col-12 sm:col-6 md:col-4 lg:col-3">Item</div>
  <div class="col-12 sm:col-6 md:col-4 lg:col-3">Item</div>
  <div class="col-12 sm:col-6 md:col-4 lg:col-3">Item</div>
</div>
```

### Divider
```typescript
import { DividerModule } from 'primeng/divider';
```

```html
<!-- Horizontal Divider -->
<p>Content before</p>
<p-divider></p-divider>
<p>Content after</p>

<!-- With Content -->
<p-divider align="center">
  <b>OR</b>
</p-divider>

<!-- Vertical Divider -->
<div class="flex">
  <p>Content 1</p>
  <p-divider layout="vertical"></p-divider>
  <p>Content 2</p>
</div>
```

---

## 🍔 Day 20: Menu Components

### Menu
```typescript
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
```

```typescript
export class MenuDemoComponent {
  items: MenuItem[] = [
    {
      label: 'File',
      items: [
        { label: 'New', icon: 'pi pi-plus', command: () => this.create() },
        { label: 'Open', icon: 'pi pi-download' },
        { separator: true },
        { label: 'Quit', icon: 'pi pi-times' }
      ]
    },
    {
      label: 'Edit',
      items: [
        { label: 'Undo', icon: 'pi pi-undo' },
        { label: 'Redo', icon: 'pi pi-replay' }
      ]
    }
  ];
  
  create() {
    console.log('Create new');
  }
}
```

```html
<p-menu [model]="items"></p-menu>
```

### Menubar
```typescript
import { MenubarModule } from 'primeng/menubar';
```

```html
<p-menubar [model]="items">
  <ng-template pTemplate="start">
    <img src="assets/logo.png" height="40" class="mr-2" />
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
  { label: 'Football' },
  { label: 'Countries' },
  { label: 'Spain' }
];

home = { icon: 'pi pi-home', routerLink: '/' };
```

### Steps
```typescript
import { StepsModule } from 'primeng/steps';
```

```html
<p-steps [model]="steps" [activeIndex]="activeIndex"></p-steps>
```

```typescript
steps = [
  { label: 'Personal Info' },
  { label: 'Address' },
  { label: 'Payment' },
  { label: 'Confirmation' }
];

activeIndex = 0;
```

---

## 🎯 Complete Navigation Example

```typescript
export class AppLayoutComponent {
  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: ['/dashboard']
    },
    {
      label: 'Users',
      icon: 'pi pi-users',
      items: [
        { label: 'List', icon: 'pi pi-list', routerLink: ['/users'] },
        { label: 'Add New', icon: 'pi pi-plus', routerLink: ['/users/new'] },
        { label: 'Import', icon: 'pi pi-upload' }
      ]
    },
    {
      label: 'Products',
      icon: 'pi pi-shopping-cart',
      items: [
        { label: 'Catalog', routerLink: ['/products'] },
        { label: 'Categories', routerLink: ['/categories'] },
        { label: 'Inventory', routerLink: ['/inventory'] }
      ]
    },
    {
      label: 'Reports',
      icon: 'pi pi-chart-bar',
      items: [
        { label: 'Sales', icon: 'pi pi-money-bill' },
        { label: 'Analytics', icon: 'pi pi-chart-line' },
        { label: 'Export', icon: 'pi pi-download' }
      ]
    },
    {
      label: 'Settings',
      icon: 'pi pi-cog',
      routerLink: ['/settings']
    }
  ];
  
  breadcrumbs: MenuItem[] = [
    { label: 'Dashboard', routerLink: '/dashboard' },
    { label: 'Users', routerLink: '/users' },
    { label: 'Edit User' }
  ];
  
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };
}
```

```html
<!-- App Layout -->
<div class="layout-wrapper">
  <!-- Topbar -->
  <div class="layout-topbar">
    <img src="assets/logo.png" alt="Logo" class="layout-logo" />
    <p-menubar [model]="menuItems">
      <ng-template pTemplate="end">
        <p-button icon="pi pi-bell" [rounded]="true" [text]="true" badge="3"></p-button>
        <p-button icon="pi pi-user" [rounded]="true" [text]="true"></p-button>
      </ng-template>
    </p-menubar>
  </div>
  
  <!-- Breadcrumb -->
  <div class="layout-breadcrumb">
    <p-breadcrumb [model]="breadcrumbs" [home]="home"></p-breadcrumb>
  </div>
  
  <!-- Main Content -->
  <div class="layout-main">
    <router-outlet></router-outlet>
  </div>
</div>
```

```css
.layout-wrapper {
  min-height: 100vh;
}

.layout-topbar {
  display: flex;
  align-items: center;
  padding: 1rem 2rem;
  background: #1976d2;
  color: white;
}

.layout-logo {
  height: 40px;
  margin-right: 2rem;
}

.layout-breadcrumb {
  padding: 1rem 2rem;
  background: #f5f5f5;
  border-bottom: 1px solid #ddd;
}

.layout-main {
  padding: 2rem;
}
```

---

## ✅ Days 17-20 Complete!

### Summary
- **Day 17:** Card, Panel, Fieldset components
- **Day 18:** TabView, Accordion for organizing content
- **Day 19:** Grid system, responsive layout, Divider
- **Day 20:** Complete navigation (Menu, Menubar, Breadcrumb, Steps)

---

## 🎉 **PrimeNG Basics Phase COMPLETE!**

You've mastered:
- ✅ Installation & Themes (Day 11)
- ✅ Buttons & Actions (Day 12)
- ✅ Input Components (Day 13)
- ✅ Selection Controls (Day 14)
- ✅ Dropdowns & MultiSelect (Day 15)
- ✅ Calendar & Dates (Day 16)
- ✅ Container Components (Day 17)
- ✅ Tabs & Accordion (Day 18)
- ✅ Layout & Grid (Day 19)
- ✅ Navigation (Day 20)

**Next Phase:** Advanced Components (Days 21-30) - Tables, DataViews, Charts! 🚀
