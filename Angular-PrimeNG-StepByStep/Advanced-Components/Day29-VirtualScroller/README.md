# Day 29: VirtualScroller and Scroller - Blazing Fast Lists

Render tens of thousands of items smoothly using VirtualScroller. Today you’ll master list/grid virtualization, lazy loading, variable item sizes, and sticky headers.

Note: We touched VirtualScroller briefly on Day 23; this lesson goes deeper with advanced patterns.

## What you’ll build
- 10k-item contact list with search and sticky header
- Virtualized grid of products with responsive columns
- Lazy-loaded infinite scroll with caching

## Setup

```ts
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
```

## 1) Virtualized Contact List

```ts
// contact-virtual-list.component.ts
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { InputTextModule } from 'primeng/inputtext';

interface Contact { id: number; name: string; email: string; city: string; }

@Component({
  selector: 'app-contact-virtual-list',
  standalone: true,
  imports: [CommonModule, VirtualScrollerModule, InputTextModule],
  templateUrl: './contact-virtual-list.component.html',
  styleUrls: ['./contact-virtual-list.component.scss']
})
export class ContactVirtualListComponent implements OnInit {
  items: Contact[] = [];
  filtered: Contact[] = [];
  query = '';

  ngOnInit() {
    this.items = Array.from({ length: 10000 }, (_, i) => ({
      id: i+1,
      name: `Person ${i+1}`,
      email: `person${i+1}@example.com`,
      city: ['NY','SF','LA','TX','SEA'][i % 5]
    }));
    this.filtered = this.items;
  }

  onSearch() {
    const q = this.query.toLowerCase();
    this.filtered = q ? this.items.filter(x => x.name.toLowerCase().includes(q) || x.email.includes(q)) : this.items;
  }
}
```

```html
<!-- contact-virtual-list.component.html -->
<div class="card">
  <div class="toolbar">
    <input pInputText type="text" placeholder="Search" [(ngModel)]="query" (input)="onSearch()" />
    <div class="count">{{filtered.length}} contacts</div>
  </div>

  <p-virtualScroller [items]="filtered" [itemSize]="56" style="height: 480px">
    <ng-template pTemplate="header">
      <div class="header sticky">Name • Email • City</div>
    </ng-template>

    <ng-template let-c pTemplate="item">
      <div class="row">
        <div class="name">{{c.name}}</div>
        <div class="email">{{c.email}}</div>
        <div class="city">{{c.city}}</div>
      </div>
    </ng-template>

    <ng-template pTemplate="footer">
      <div class="footer">End of list</div>
    </ng-template>
  </p-virtualScroller>
</div>
```

```scss
/* contact-virtual-list.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.toolbar { display:flex; justify-content:space-between; align-items:center; margin-bottom:.5rem; }
.count { color:#64748b; font-size:.9rem; }
.header { padding:.5rem; color:#475569; background:#f8fafc; border-bottom:1px solid #e2e8f0; }
.sticky { position: sticky; top: 0; z-index: 1; }
.row { display:grid; grid-template-columns: 1fr 2fr 120px; align-items:center; padding:.5rem; border-bottom:1px solid #f1f5f9; }
.name { font-weight:600; }
.email { color:#64748b; }
```

## 2) Virtualized Product Grid

```ts
// product-virtual-grid.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { TagModule } from 'primeng/tag';

interface Product { id:number; name:string; price:number; tag?:string; }

@Component({
  selector: 'app-product-virtual-grid',
  standalone: true,
  imports: [CommonModule, VirtualScrollerModule, TagModule],
  templateUrl: './product-virtual-grid.component.html',
  styleUrls: ['./product-virtual-grid.component.scss']
})
export class ProductVirtualGridComponent implements OnInit {
  items: Product[] = [];
  colWidth = 240; // px

  ngOnInit() {
    this.items = Array.from({ length: 5000 }, (_, i) => ({
      id: i+1,
      name: `Item ${i+1}`,
      price: Math.floor(Math.random()*900)+50,
      tag: ['New','Sale','Hot'][i % 3]
    }));
  }
}
```

```html
<!-- product-virtual-grid.component.html -->
<div class="card">
  <p-virtualScroller [items]="items" [itemSize]="280" scrollDirection="vertical" style="height: 700px">
    <ng-template pTemplate="item" let-item let-options="options">
      <div class="grid-row" [style.gridTemplateColumns]="`repeat(auto-fill, minmax(${colWidth}px, 1fr))`">
        <div class="cell" *ngFor="let i of [0,1,2,3]">
          <div class="tile">
            <div class="title">{{item.name}}</div>
            <div class="price">{{item.price | currency:'USD'}}</div>
            <p-tag [value]="item.tag"></p-tag>
          </div>
        </div>
      </div>
    </ng-template>
  </p-virtualScroller>
</div>
```

```scss
/* product-virtual-grid.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.grid-row { display:grid; gap:.75rem; padding:.5rem; }
.cell .tile { border:1px solid #e2e8f0; border-radius:8px; padding:.75rem; background:#ffffff; }
.title { font-weight:600; }
.price { color:#0ea5e9; font-weight:700; }
```

## 3) Lazy Loading with Cache

```ts
// lazy-virtual-list.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VirtualScrollerModule, ScrollToOptions } from 'primeng/virtualscroller';

@Component({
  selector: 'app-lazy-virtual-list',
  standalone: true,
  imports: [CommonModule, VirtualScrollerModule],
  template: `
  <p-virtualScroller [items]="items" [lazy]="true" [itemSize]="60" style="height: 400px"
    (onLazyLoad)="load($event)">
    <ng-template pTemplate="item" let-item>
      <div class="row">{{item?.name || 'Loading...'}}</div>
    </ng-template>
  </p-virtualScroller>`,
  styles: [`.row{padding:.5rem;border-bottom:1px solid #f1f5f9;}`]
})
export class LazyVirtualListComponent {
  items: any[] = Array(10000); // placeholders
  cache = new Map<number, any[]>();

  load(event: { first: number; last: number }) {
    const page = Math.floor(event.first / 50);
    if (this.cache.has(page)) {
      this.apply(page);
    } else {
      setTimeout(() => {
        const data = Array.from({ length: 50 }, (_, i) => ({ name: `Row ${page*50 + i + 1}` }));
        this.cache.set(page, data);
        this.apply(page);
      }, 600);
    }
  }

  apply(page: number) {
    const start = page * 50;
    this.items.splice(start, 50, ...this.cache.get(page)!);
    this.items = [...this.items]; // trigger change detection
  }
}
```

## Best Practices
- Measure `itemSize` accurately; incorrect values cause jumpy scroll
- For dynamic heights, consider fixed-height wrappers or the Scroller component
- Cache pages when lazy-loading to avoid re-fetching
- Avoid heavy templates in each item; extract dumb components only if needed

## Exercises
- Add range selection and keyboard navigation to the virtual list
- Implement a sticky group header for each alphabet letter
- Add “scroll to index” control and ensure smooth behavior

## Summary
You’re ready to render massive lists with VirtualScroller efficiently. Next: Day 30 - Advanced Table patterns that combine everything you’ve learned.