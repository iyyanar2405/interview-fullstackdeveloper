# Day 48 — E‑Commerce: Setup & Product Catalog

Objectives
- Initialize the e‑commerce app section: routes, theme, and shared modules.
- Build product catalog grid with filters (category, price, rating) using PrimeNG components.

Components
- DataView or Grid + Card, Dropdown, MultiSelect, Slider, Rating, Chips

Catalog page
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataViewModule } from 'primeng/dataview';
import { CardModule } from 'primeng/card';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { SliderModule } from 'primeng/slider';
import { RatingModule } from 'primeng/rating';

@Component({
  selector: 'app-catalog-page',
  standalone: true,
  imports: [CommonModule, DataViewModule, CardModule, DropdownModule, MultiSelectModule, SliderModule, RatingModule],
  templateUrl: './catalog.page.html'
})
export class CatalogPage {
  products: any[] = [ { id:1, name:'Phone X', price:799, rating:4.5, categories:['Mobile'] } ];
  categories = [{label:'Mobile',value:'Mobile'},{label:'Laptop',value:'Laptop'}];
  selectedCats: string[] = [];
  priceRange: number[] = [0, 2000];
  minRating = 0;
}
```

Template
```html
<h2>Product Catalog</h2>
<div class="p-d-flex p-ai-center p-gap-3 p-mb-3">
  <p-multiSelect [options]="categories" [(ngModel)]="selectedCats" defaultLabel="Categories" optionLabel="label" optionValue="value"></p-multiSelect>
  <div style="min-width:200px">
    <label>Price</label>
    <p-slider [(ngModel)]="priceRange" [range]="true" [min]="0" [max]="2000"></p-slider>
  </div>
  <div>
    <label>Rating ≥ {{ minRating }}</label>
    <p-rating [(ngModel)]="minRating" [cancel]="false"></p-rating>
  </div>
</div>

<p-dataView [value]="products" layout="grid" [paginator]="true" [rows]="12">
  <ng-template pTemplate="listItem" let-p>
    <p-card header="{{p.name}}" subheader="${{p.price}}">
      <div class="p-d-flex p-ai-center p-jc-between">
        <p-rating [modelValue]="p.rating" [readonly]="true" [cancel]="false"></p-rating>
        <button pButton label="Add to Cart" icon="pi pi-cart-plus"></button>
      </div>
    </p-card>
  </ng-template>
  <ng-template pTemplate="gridItem" let-p>
    <div class="p-col-12 p-md-4 p-lg-3">
      <p-card header="{{p.name}}" subheader="${{p.price}}">
        <p-rating [modelValue]="p.rating" [readonly]="true" [cancel]="false"></p-rating>
        <button pButton label="Add to Cart" icon="pi pi-cart-plus" class="p-mt-2"></button>
      </p-card>
    </div>
  </ng-template>
</p-dataView>
```

Exercises
1) Add search with AutoComplete.
2) Add category chips with removable filters.
3) Add skeletons on initial load and when filtering.
