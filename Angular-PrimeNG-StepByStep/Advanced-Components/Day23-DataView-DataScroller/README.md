# Day 23: DataView and DataScroller - Flexible List Layouts

In this lesson, you'll master PrimeNG's DataView and DataScroller components for building beautiful, flexible list and grid layouts with infinite scroll. These are great alternatives to Table when you need card-style UIs or Pinterest-like layouts.

## What you'll build
- Product catalog with list/grid view toggle
- Responsive cards with images, rating, tags, and actions
- Infinite scroll with lazy loading using DataScroller and VirtualScroller
- Filters: category, price range, rating, search
- Skeleton loading placeholders

## Prerequisites
- Angular + PrimeNG project set up (from previous days)
- Installed modules: DataViewModule, DataScrollerModule, VirtualScrollerModule, ButtonModule, DropdownModule, InputTextModule, RatingModule, TagModule, SliderModule, PanelModule, SkeletonModule

## 1) Setup

```ts
// app.module.ts (if using NgModule) or import into a standalone component
import { DataViewModule } from 'primeng/dataview';
import { DataScrollerModule } from 'primeng/datascroller';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { RatingModule } from 'primeng/rating';
import { TagModule } from 'primeng/tag';
import { SliderModule } from 'primeng/slider';
import { PanelModule } from 'primeng/panel';
import { SkeletonModule } from 'primeng/skeleton';
```

## 2) Data model

```ts
export interface Product {
  id: number;
  name: string;
  image: string;
  price: number;
  category: 'Electronics' | 'Clothing' | 'Fitness' | 'Home';
  rating: number; // 0-5
  inventoryStatus: 'INSTOCK' | 'LOWSTOCK' | 'OUTOFSTOCK';
  description?: string;
}
```

## 3) DataView: List/Grid toggle with filtering

```ts
// product-catalog.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataViewModule } from 'primeng/dataview';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { RatingModule } from 'primeng/rating';
import { TagModule } from 'primeng/tag';
import { SliderModule } from 'primeng/slider';
import { PanelModule } from 'primeng/panel';

@Component({
  selector: 'app-product-catalog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DataViewModule,
    DropdownModule,
    InputTextModule,
    ButtonModule,
    RatingModule,
    TagModule,
    SliderModule,
    PanelModule
  ],
  templateUrl: './product-catalog.component.html',
  styleUrls: ['./product-catalog.component.scss']
})
export class ProductCatalogComponent implements OnInit {
  products: Product[] = [];
  layout: 'list' | 'grid' = 'grid';
  loading = true;

  // filters
  search = '';
  selectedCategory: any = null;
  categories = [
    { label: 'All', value: null },
    { label: 'Electronics', value: 'Electronics' },
    { label: 'Clothing', value: 'Clothing' },
    { label: 'Fitness', value: 'Fitness' },
    { label: 'Home', value: 'Home' },
  ];
  priceRange: number[] = [0, 1500];
  minPrice = 0;
  maxPrice = 1500;
  minRating = 0;

  ngOnInit() {
    // Simulate API
    setTimeout(() => {
      this.products = this.mockProducts(48);
      this.loading = false;
    }, 800);
  }

  filtered(): Product[] {
    return this.products
      .filter(p => !this.selectedCategory || p.category === this.selectedCategory)
      .filter(p => p.price >= this.priceRange[0] && p.price <= this.priceRange[1])
      .filter(p => p.rating >= this.minRating)
      .filter(p => p.name.toLowerCase().includes(this.search.toLowerCase()));
  }

  getSeverity(status: Product['inventoryStatus']) {
    return status === 'INSTOCK' ? 'success' : status === 'LOWSTOCK' ? 'warning' : 'danger';
  }

  private mockProducts(n: number): Product[] {
    const cats = ['Electronics','Clothing','Fitness','Home'] as const;
    return Array.from({ length: n }, (_, i) => ({
      id: i + 1,
      name: `Product ${i + 1}`,
      image: `https://picsum.photos/seed/p${i}/300/200`,
      price: Math.floor(Math.random() * 1500) + 50,
      category: cats[Math.floor(Math.random() * cats.length)],
      rating: Math.floor(Math.random() * 6),
      inventoryStatus: ['INSTOCK','LOWSTOCK','OUTOFSTOCK'][Math.floor(Math.random()*3)] as Product['inventoryStatus'],
      description: 'Nice product with awesome features.'
    }));
  }
}
```

```html
<!-- product-catalog.component.html -->
<div class="card">
  <div class="toolbar">
    <div class="filters">
      <span class="p-input-icon-left">
        <i class="pi pi-search"></i>
        <input pInputText [(ngModel)]="search" placeholder="Search products" />
      </span>
      <p-dropdown [options]="categories" [(ngModel)]="selectedCategory" placeholder="Category"></p-dropdown>
      <div class="price-filter">
        <label>Price</label>
        <p-slider [(ngModel)]="priceRange" [range]="true" [min]="minPrice" [max]="maxPrice"></p-slider>
        <div class="range">{{priceRange[0] | currency:'USD'}} - {{priceRange[1] | currency:'USD'}}</div>
      </div>
      <div class="rating-filter">
        <label>Min Rating</label>
        <p-rating [(ngModel)]="minRating" [cancel]="false"></p-rating>
      </div>
    </div>
    <div class="actions">
      <button pButton icon="pi pi-bars" [class.active]="layout==='list'" (click)="layout='list'" label="List" class="p-button-text"></button>
      <button pButton icon="pi pi-th-large" [class.active]="layout==='grid'" (click)="layout='grid'" label="Grid" class="p-button-text"></button>
    </div>
  </div>

  <p-dataView [value]="filtered()" [layout]="layout">
    <ng-template pTemplate="header">
      <div class="header">{{filtered().length}} items</div>
    </ng-template>

    <ng-template let-product pTemplate="listItem">
      <div class="list-item">
        <img [src]="product.image" alt="{{product.name}}" />
        <div class="details">
          <div class="title">{{product.name}}</div>
          <div class="desc">{{product.description}}</div>
          <div class="meta">
            <p-tag [value]="product.inventoryStatus" [severity]="getSeverity(product.inventoryStatus)"></p-tag>
            <p-rating [ngModel]="product.rating" [readonly]="true" [cancel]="false"></p-rating>
          </div>
        </div>
        <div class="price">{{product.price | currency:'USD'}}</div>
        <div class="actions">
          <button pButton icon="pi pi-shopping-cart" label="Add"></button>
          <button pButton icon="pi pi-heart" class="p-button-text"></button>
        </div>
      </div>
    </ng-template>

    <ng-template let-product pTemplate="gridItem">
      <div class="grid-item">
        <img [src]="product.image" alt="{{product.name}}" />
        <div class="content">
          <div class="title">{{product.name}}</div>
          <div class="meta">
            <p-tag [value]="product.inventoryStatus" [severity]="getSeverity(product.inventoryStatus)"></p-tag>
            <p-rating [ngModel]="product.rating" [readonly]="true" [cancel]="false"></p-rating>
          </div>
          <div class="desc">{{product.description}}</div>
        </div>
        <div class="footer">
          <div class="price">{{product.price | currency:'USD'}}</div>
          <div class="actions">
            <button pButton icon="pi pi-shopping-cart" label="Add"></button>
            <button pButton icon="pi pi-heart" class="p-button-text"></button>
          </div>
        </div>
      </div>
    </ng-template>

    <ng-template pTemplate="empty">
      <div class="empty">No products found.</div>
    </ng-template>
  </p-dataView>
</div>
```

```scss
/* product-catalog.component.scss */
.card { padding: 1rem; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,.06); }
.toolbar { display: flex; justify-content: space-between; flex-wrap: wrap; gap: .75rem; margin-bottom: 1rem; }
.filters { display: flex; gap: .75rem; align-items: center; flex-wrap: wrap; }
.actions .p-button.active { background: var(--primary-color); color: #fff; }
.price-filter, .rating-filter { display: flex; align-items: center; gap: .5rem; min-width: 220px; }
.header { font-weight: 600; color: #475569; margin: .5rem 0; }
.list-item { display:flex; gap:1rem; align-items:center; padding:.75rem; border-bottom:1px solid #f1f5f9; }
.list-item img { width: 120px; height: 80px; object-fit: cover; border-radius: 6px; }
.list-item .details { flex:1; }
.list-item .title { font-weight:600; margin-bottom:.25rem; }
.list-item .desc { color:#64748b; font-size:.9rem; }
.list-item .meta { display:flex; gap:.5rem; align-items:center; margin-top:.25rem; }
.list-item .price { font-weight:700; }
.list-item .actions { display:flex; gap:.5rem; }
.grid-item { display:flex; flex-direction: column; border:1px solid #e2e8f0; border-radius:8px; overflow:hidden; }
.grid-item img { width:100%; height:160px; object-fit:cover; }
.grid-item .content { padding:.75rem; }
.grid-item .title { font-weight:600; margin-bottom:.25rem; }
.grid-item .meta { display:flex; gap:.5rem; align-items:center; margin-bottom:.25rem; }
.grid-item .desc { color:#64748b; font-size:.9rem; }
.grid-item .footer { display:flex; justify-content:space-between; align-items:center; padding:.75rem; border-top:1px solid #e2e8f0; }
.grid-item .price { font-weight:700; }
.empty { padding:2rem; text-align:center; color:#64748b; }
```

## 4) DataScroller: Infinite scroll

```ts
// infinite-products.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataScrollerModule } from 'primeng/datascroller';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'app-infinite-products',
  standalone: true,
  imports: [CommonModule, DataScrollerModule, ButtonModule, TagModule, SkeletonModule],
  templateUrl: './infinite-products.component.html',
  styleUrls: ['./infinite-products.component.scss']
})
export class InfiniteProductsComponent implements OnInit {
  items: Product[] = [];
  loading = false;
  total = 200; // pretend server total
  chunk = 20;

  ngOnInit() {
    this.loadMore();
  }

  loadMore() {
    if (this.loading || this.items.length >= this.total) return;
    this.loading = true;
    setTimeout(() => {
      const start = this.items.length;
      const more = Array.from({ length: this.chunk }, (_, i) => ({
        id: start + i + 1,
        name: `Product ${start + i + 1}`,
        image: `https://picsum.photos/seed/inf${start + i}/300/160`,
        price: Math.floor(Math.random()*1200)+50,
        category: ['Electronics','Clothing','Fitness','Home'][Math.floor(Math.random()*4)] as Product['category'],
        rating: Math.floor(Math.random()*6),
        inventoryStatus: ['INSTOCK','LOWSTOCK','OUTOFSTOCK'][Math.floor(Math.random()*3)] as Product['inventoryStatus'],
      } as Product));
      this.items = [...this.items, ...more];
      this.loading = false;
    }, 800);
  }
}
```

```html
<!-- infinite-products.component.html -->
<div class="card">
  <p-dataScroller [value]="items" [rows]="10" [buffer]="0.9" (onLazyLoad)="loadMore()">
    <ng-template pTemplate="header">
      <div class="header">Infinite Products ({{items.length}}/{{total}})</div>
    </ng-template>

    <ng-template let-product pTemplate="item">
      <div class="item">
        <img [src]="product.image" alt="{{product.name}}" />
        <div class="info">
          <div class="title">{{product.name}}</div>
          <div class="sub">
            <p-tag [value]="product.category"></p-tag>
            <span class="price">{{product.price | currency:'USD'}}</span>
          </div>
        </div>
      </div>
    </ng-template>

    <ng-template pTemplate="footer">
      <div class="footer">
        <button pButton label="Load More" icon="pi pi-chevron-down" (click)="loadMore()" [disabled]="loading || items.length>=total"></button>
      </div>
    </ng-template>
  </p-dataScroller>

  <div class="skeletons" *ngIf="loading">
    <p-skeleton height="80px" styleClass="mb"></p-skeleton>
    <p-skeleton height="80px" styleClass="mb"></p-skeleton>
  </div>
</div>
```

```scss
/* infinite-products.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.header { font-weight:600; margin-bottom:.5rem; }
.item { display:flex; gap:1rem; align-items:center; padding:.5rem 0; border-bottom:1px solid #f1f5f9; }
.item img { width:120px; height:80px; object-fit:cover; border-radius:6px; }
.item .info { flex:1; }
.item .title { font-weight:600; }
.item .sub { display:flex; justify-content:space-between; align-items:center; color:#64748b; }
.footer { text-align:center; padding:.75rem 0; }
.mb { margin-bottom:.5rem; }
```

## 5) VirtualScroller (bonus)
For very large lists, VirtualScroller renders only visible items.

```html
<p-virtualScroller [items]="products" [itemSize]="90" style="height: 500px">
  <ng-template let-product pTemplate="item">
    <div class="list-item">
      <!-- reuse the list-item styles/content -->
      {{product.name}}
    </div>
  </ng-template>
</p-virtualScroller>
```

## Tips & Best practices
- Prefer DataView for card/list UIs; use Table for tabular data
- Use skeletons to mask loading
- Debounce search inputs if hitting server
- For infinite scroll, keep a server cursor/offset
- VirtualScroller is best for 1000+ items lists

## Exercises
- Add a sort dropdown (Price: Low→High, High→Low, Rating)
- Persist filters in localStorage and restore on load
- Add “favorite” toggle and filter by favorites

## Summary
You now know how to build flexible, attractive list/grid experiences with DataView, and scale them with DataScroller/VirtualScroller for performance. Next up: Day 24 - Tree and TreeTable for hierarchical data!