# Day 42 — Admin Dashboard: Layout & Navigation

Objectives
- Build a responsive layout with Sidebar + Topbar + Content.
- Implement Menubar and TieredMenu navigation, Breadcrumbs.
- Add responsive breakpoints using PrimeFlex.

Components
- Menubar, Sidebar, Breadcrumb, TieredMenu, Button, Badge

Layout Tips
- Use CSS grid/flex for 2-column layout.
- Collapse sidebar on small screens; toggle with a button.
- Keep content scrollable, not the whole page.

Topbar component
```ts
import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenubarModule } from 'primeng/menubar';
import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, MenubarModule, ButtonModule, BadgeModule],
  template: `
    <div class="topbar">
      <button pButton icon="pi pi-bars" (click)="toggle.emit()"></button>
      <p-menubar [model]="menu" class="flex-1"></p-menubar>
      <button pButton icon="pi pi-bell" pBadge value="3" class="p-button-text"></button>
      <button pButton icon="pi pi-user"></button>
    </div>
  `,
  styles: [`.topbar{ display:flex; align-items:center; gap:.5rem; padding:.5rem; border-bottom:1px solid var(--surface-border); }`]
})
export class TopbarComponent {
  @Output() toggle = new EventEmitter<void>();
  menu = [{ label: 'Home', icon: 'pi pi-home', routerLink: ['/dashboard'] }];
}
```

Breadcrumb setup
```ts
import { Component, Input } from '@angular/core';
import { BreadcrumbModule } from 'primeng/breadcrumb';

@Component({
  selector: 'app-breadcrumbs',
  standalone: true,
  imports: [BreadcrumbModule],
  template: `<p-breadcrumb [model]="items" [home]="home"></p-breadcrumb>`
})
export class BreadcrumbsComponent {
  @Input() items = [] as any[];
  home = { icon: 'pi pi-home', routerLink: '/' };
}
```

Tasks
- Wire Topbar + Sidebar + Breadcrumbs into shell.
- Add active link styling on navigation.
- Ensure keyboard accessibility (tab cycle, skip-to-content).

Exercises
1) Add a notifications overlay panel with a list of alerts.
2) Collapse sidebar automatically below 768px using media query.
3) Persist sidebar collapsed state in localStorage.
