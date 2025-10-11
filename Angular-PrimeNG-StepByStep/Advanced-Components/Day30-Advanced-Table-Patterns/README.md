# Day 30: Advanced Table Patterns - Enterprise Playbook

Today is a synthesis day. You’ll implement enterprise-grade table UX that combines editing, grouping, aggregation, responsiveness, and performance techniques.

## What you’ll build
- Master/Detail table with expandable panels and inline editors
- Row grouping with aggregates per group + grand totals
- Sticky header/footer, responsive column priorities, and persistence

## 1) Master/Detail with Inline Editors

```html
<p-table [value]="orders" dataKey="id" [rowHover]="true">
  <ng-template pTemplate="header">
    <tr>
      <th style="width:2rem"></th>
      <th>Order #</th>
      <th>Customer</th>
      <th>Date</th>
      <th>Total</th>
      <th>Status</th>
      <th style="width:8rem">Actions</th>
    </tr>
  </ng-template>

  <ng-template pTemplate="body" let-order let-expanded="expanded">
    <tr>
      <td>
        <button pButton [pRowToggler]="order" class="p-button-text p-button-rounded"
                [icon]="expanded ? 'pi pi-chevron-down' : 'pi pi-chevron-right'"></button>
      </td>
      <td>{{order.code}}</td>
      <td>{{order.customer}}</td>
      <td>{{order.date | date:'mediumDate'}}</td>
      <td>{{order.total | currency:'USD'}}</td>
      <td>
        <p-tag [value]="order.status" [severity]="statusSeverity(order.status)"></p-tag>
      </td>
      <td>
        <button pButton icon="pi pi-pencil" class="p-button-text p-button-rounded" (click)="edit(order)"></button>
        <button pButton icon="pi pi-trash" class="p-button-text p-button-rounded p-button-danger" (click)="remove(order)"></button>
      </td>
    </tr>
  </ng-template>

  <ng-template pTemplate="rowexpansion" let-order>
    <tr>
      <td colspan="7">
        <div class="p-3">
          <h4>Items for {{order.code}}</h4>
          <p-table [value]="order.items" dataKey="sku" editMode="row" class="p-datatable-sm">
            <ng-template pTemplate="header">
              <tr>
                <th>SKU</th>
                <th>Name</th>
                <th>Qty</th>
                <th>Price</th>
                <th>Subtotal</th>
                <th style="width:6rem">Edit</th>
              </tr>
            </ng-template>
            <ng-template pTemplate="body" let-item let-editing="editing" let-ri="rowIndex">
              <tr [pEditableRow]="item">
                <td>{{item.sku}}</td>
                <td>{{item.name}}</td>
                <td pEditableColumn>
                  <p-cellEditor>
                    <ng-template pTemplate="input"><input pInputText type="number" [(ngModel)]="item.qty"></ng-template>
                    <ng-template pTemplate="output">{{item.qty}}</ng-template>
                  </p-cellEditor>
                </td>
                <td>{{item.price | currency:'USD'}}</td>
                <td>{{item.qty * item.price | currency:'USD'}}</td>
                <td>
                  <button pButton pInitEditableRow icon="pi pi-pencil" class="p-button-text p-button-rounded" *ngIf="!editing"></button>
                  <button pButton pSaveEditableRow icon="pi pi-check" class="p-button-text p-button-rounded p-button-success" *ngIf="editing"></button>
                  <button pButton pCancelEditableRow icon="pi pi-times" class="p-button-text p-button-rounded p-button-danger" *ngIf="editing"></button>
                </td>
              </tr>
            </ng-template>
            <ng-template pTemplate="footer">
              <tr>
                <td colspan="4" class="text-right">Order Total:</td>
                <td colspan="2">{{order.items.reduce((s:any,i:any)=>s+i.qty*i.price,0) | currency:'USD'}}</td>
              </tr>
            </ng-template>
          </p-table>
        </div>
      </td>
    </tr>
  </ng-template>
</p-table>
```

## 2) Row Grouping with Aggregates

```html
<p-table [value]="sales" rowGroupMode="subheader" groupRowsBy="region" sortMode="single" sortField="region">
  <ng-template pTemplate="header">
    <tr>
      <th>Region</th>
      <th>Rep</th>
      <th>Product</th>
      <th>Qty</th>
      <th>Total</th>
    </tr>
  </ng-template>

  <ng-template pTemplate="groupheader" let-rowData>
    <tr class="group-row">
      <td colspan="5">Region: {{rowData.region}}</td>
    </tr>
  </ng-template>

  <ng-template pTemplate="body" let-r>
    <tr>
      <td>{{r.region}}</td>
      <td>{{r.rep}}</td>
      <td>{{r.product}}</td>
      <td>{{r.qty}}</td>
      <td>{{r.total | currency:'USD'}}</td>
    </tr>
  </ng-template>

  <ng-template pTemplate="groupfooter" let-rowData>
    <tr class="group-footer">
      <td colspan="3" class="text-right">Region Total:</td>
      <td colspan="2">{{sumByRegion(rowData.region) | currency:'USD'}}</td>
    </tr>
  </ng-template>

  <ng-template pTemplate="summary">
    <div class="p-2 text-right font-bold">Grand Total: {{grandTotal() | currency:'USD'}}</div>
  </ng-template>
</p-table>
```

## 3) Sticky Header/Footer and Responsive Columns

```html
<p-table [value]="rows" [scrollable]="true" scrollHeight="400px" [resizableColumns]="true">
  <ng-template pTemplate="header">
    <tr>
      <th class="sticky">Name</th>
      <th class="priority-1">Email</th>
      <th class="priority-2">City</th>
      <th class="priority-3">Phone</th>
      <th class="priority-4">Notes</th>
    </tr>
  </ng-template>
  <!-- body ... -->
  <ng-template pTemplate="footer">
    <tr><td colspan="5">Showing {{rows.length}} rows</td></tr>
  </ng-template>
</p-table>
```

```scss
/* responsive priorities */
@media (max-width: 1024px) { .priority-4 { display: none; } }
@media (max-width: 768px) { .priority-3 { display: none; } }
@media (max-width: 640px) { .priority-2 { display: none; } }
.sticky { position: sticky; top: 0; background: #f8fafc; z-index: 1; }
.text-right { text-align: right; }
.font-bold { font-weight: 600; }
```

## 4) State Persistence Service

```ts
// table-state.service.ts
import { Injectable } from '@angular/core';
import { Table } from 'primeng/table';

@Injectable({ providedIn: 'root' })
export class TableStateService {
  save(key: string, table: Table, extra: any = {}) {
    const state = {
      first: table.first, rows: table.rows,
      sortField: table.sortField, sortOrder: table.sortOrder,
      filters: table.filters, multiSortMeta: table.multiSortMeta,
      ...extra
    };
    localStorage.setItem(key, JSON.stringify(state));
  }
  restore(key: string, table: Table) {
    const s = localStorage.getItem(key); if (!s) return null;
    const state = JSON.parse(s);
    table.first = state.first; table.rows = state.rows;
    table.sortField = state.sortField; table.sortOrder = state.sortOrder;
    table.filters = state.filters; table.multiSortMeta = state.multiSortMeta;
    return state;
  }
}
```

## 5) Performance Checklist
- Prefer lazy loading with server-side paging/sort/filter
- Use virtual scroll for very large datasets
- Minimize heavy content in cells; memoize templates where possible
- Avoid frequent immutable rebuilds of the entire data array when unnecessary

## Exercises
- Add row drag & drop to reorder items and persist order
- Add column chooser with persisted visible columns per user
- Implement server-driven filtering with debounced inputs

## Summary
You’ve assembled a production-grade table experience. Phase 3 (Advanced Components) completed. Next up: Phase 4 (Forms & Validation), Days 31-40.