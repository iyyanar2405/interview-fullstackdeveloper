# Day 41 — Admin Dashboard: Project Setup & Architecture

Objectives
- Define the admin dashboard scope, modules, and routes.
- Plan folder structure, theme, and layout with PrimeFlex.
- Create core shell: Menubar/Sidebar + Content + Footer.

Architecture
- Modules: dashboard, users, products, orders, reports, shared, core.
- Core: layout, auth guard, http interceptors.
- Shared: reusable components, directives, pipes, models.

Folder structure
```
admin-dashboard/
  core/
    layout/
      shell.component.ts
    guards/
    interceptors/
  shared/
    components/
    pipes/
    models/
  features/
    dashboard/
    users/
    products/
    orders/
    reports/
  app.routes.ts
```

Shell component (standalone)
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenubarModule } from 'primeng/menubar';
import { SidebarModule } from 'primeng/sidebar';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, MenubarModule, SidebarModule, ButtonModule, RouterModule],
  template: `
    <p-menubar [model]="menu"></p-menubar>
    <div class="layout">
      <p-sidebar [(visible)]="sidebar" [modal]="false" [baseZIndex]="1000">
        <ul class="nav">
          <li><a routerLink="/dashboard">Dashboard</a></li>
          <li><a routerLink="/users">Users</a></li>
          <li><a routerLink="/products">Products</a></li>
          <li><a routerLink="/orders">Orders</a></li>
          <li><a routerLink="/reports">Reports</a></li>
        </ul>
      </p-sidebar>
      <div class="content">
        <router-outlet></router-outlet>
      </div>
    </div>
    <footer class="footer">© {{year}} Admin Dashboard</footer>
  `,
  styles: [`
    .layout{ display:flex; min-height: calc(100vh - 56px); }
    .content{ flex:1; padding:1rem; }
    .footer{ text-align:center; padding:.5rem; border-top:1px solid var(--surface-border); }
    .nav{ list-style:none; padding:0; }
    .nav li{ margin:.5rem 0; }
    .nav a{ text-decoration:none; }
  `]
})
export class ShellComponent {
  sidebar = true;
  year = new Date().getFullYear();
  menu = [{ label: 'Admin', icon: 'pi pi-cog' }];
}
```

Routing plan
```ts
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.page').then(m => m.DashboardPage) },
  { path: 'users', loadComponent: () => import('./features/users/users.page').then(m => m.UsersPage) },
  { path: 'products', loadComponent: () => import('./features/products/products.page').then(m => m.ProductsPage) },
  { path: 'orders', loadComponent: () => import('./features/orders/orders.page').then(m => m.OrdersPage) },
  { path: 'reports', loadComponent: () => import('./features/reports/reports.page').then(m => m.ReportsPage) },
];
```

Tasks
- Scaffold shell and routes.
- Add PrimeNG theme, PrimeFlex, and base layout.
- Create placeholder feature pages.

Exercises
1) Add breadcrumb using Breadcrumb component.
2) Add user profile menu with overlay panel.
3) Add dark/light theme toggle.
