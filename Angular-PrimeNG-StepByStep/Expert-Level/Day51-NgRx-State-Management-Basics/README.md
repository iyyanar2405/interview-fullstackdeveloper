# Day 51 — NgRx State Management Basics

Level up your Angular apps with predictable state management using NgRx. Today you’ll wire up a minimal feature store, selectors, and effects, and connect them to PrimeNG UI.

## Learning objectives

- Understand Redux patterns in Angular and NgRx building blocks
- Create feature state, actions, reducer, and selectors
- Use Effects to handle async operations with HttpClient
- Connect PrimeNG components to the store (Table, Toast)
- Apply best practices for folder structure and testing hooks

## Prerequisites

- Angular 16+ project created and running
- PrimeNG and PrimeIcons installed and configured
- Basic knowledge of RxJS and Angular services

## Setup

Install NgRx packages for a feature-based setup.

```powershell
npm install @ngrx/store @ngrx/effects @ngrx/entity @ngrx/store-devtools --save
```

Optional devtools in development only (environment check recommended).

## Project structure (suggested)

```
src/app/
  state/
    app.reducer.ts
    app.selectors.ts
  features/products/
    + store/
      actions.ts
      reducer.ts
      selectors.ts
      effects.ts
      models.ts
      adapter.ts (optional with @ngrx/entity)
    + components/
      products-page/
        products-page.component.ts|html|scss
    products.module.ts
```

## Step 1 — Define models

Create a simple product model.

```ts
export interface Product {
  id: string;
  name: string;
  price: number;
  stock: number;
}
```

## Step 2 — Define actions

```ts
import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Product } from './models';

export const ProductsActions = createActionGroup({
  source: 'Products',
  events: {
    'Load': emptyProps(),
    'Load Success': props<{ products: Product[] }>(),
    'Load Failure': props<{ error: string }>(),
    'Select': props<{ id: string }>(),
  },
});
```

## Step 3 — Create reducer

Using @ngrx/entity simplifies collections.

```ts
import { createFeature, createReducer, on } from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { Product } from './models';
import { ProductsActions } from './actions';

export interface ProductsState extends EntityState<Product> {
  loading: boolean;
  error?: string | null;
  selectedId?: string | null;
}

const adapter = createEntityAdapter<Product>({ selectId: (p) => p.id });

const initialState: ProductsState = adapter.getInitialState({
  loading: false,
  error: null,
  selectedId: null,
});

const baseReducer = createReducer(
  initialState,
  on(ProductsActions.load, (state) => ({ ...state, loading: true, error: null })),
  on(ProductsActions.loadSuccess, (state, { products }) =>
    adapter.setAll(products, { ...state, loading: false })
  ),
  on(ProductsActions.loadFailure, (state, { error }) => ({ ...state, loading: false, error })),
  on(ProductsActions.select, (state, { id }) => ({ ...state, selectedId: id }))
);

export const productsFeature = createFeature({
  name: 'products',
  reducer: baseReducer,
});

export const {
  name: productsFeatureKey,
  reducer: productsReducer,
  selectProductsState,
} = productsFeature;

export const { selectAll: selectAllProducts, selectEntities: selectProductEntities } = adapter.getSelectors(selectProductsState);
```

## Step 4 — Selectors

Add memoized selectors, including derived values.

```ts
import { createSelector } from '@ngrx/store';
import { selectAllProducts, selectProductEntities, selectProductsState } from './reducer';

export const selectLoading = createSelector(selectProductsState, (s) => s.loading);
export const selectError = createSelector(selectProductsState, (s) => s.error);
export const selectSelectedId = createSelector(selectProductsState, (s) => s.selectedId);
export const selectSelectedProduct = createSelector(
  selectProductEntities,
  selectSelectedId,
  (entities, id) => (id ? entities[id] ?? null : null)
);

export const selectInventoryValue = createSelector(selectAllProducts, (items) =>
  items.reduce((sum, p) => sum + p.price * p.stock, 0)
);
```

## Step 5 — Effects and service

```ts
import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { ProductsActions } from './actions';
import { catchError, map, of, switchMap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Product } from './models';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  constructor(private http: HttpClient) {}
  getAll() {
    return this.http.get<Product[]>('/api/products');
  }
}

@Injectable()
export class ProductsEffects {
  load$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductsActions.load),
      switchMap(() =>
        this.api.getAll().pipe(
          map((products) => ProductsActions.loadSuccess({ products })),
          catchError((err) => of(ProductsActions.loadFailure({ error: err?.message ?? 'Load failed' })))
        )
      )
    )
  );

  constructor(private actions$: Actions, private api: ProductsService) {}
}
```

## Step 6 — Register store and effects

In feature module (or root) register the reducers and effects.

```ts
import { NgModule } from '@angular/core';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { productsReducer, productsFeatureKey } from './store/reducer';
import { ProductsEffects } from './store/effects';

@NgModule({
  imports: [
    StoreModule.forFeature(productsFeatureKey, productsReducer),
    EffectsModule.forFeature([ProductsEffects]),
  ],
})
export class ProductsStoreModule {}
```

In `AppModule` (root):

```ts
StoreModule.forRoot({}, {}),
EffectsModule.forRoot([]),
// optionally
StoreDevtoolsModule.instrument({ maxAge: 25, logOnly: environment.production })
```

## Step 7 — Wire up UI with PrimeNG

Example using `p-table` and Toast for error.

```html
<p-toolbar>
  <div class="p-toolbar-group-left">
    <button pButton label="Load" icon="pi pi-refresh" (click)="reload()"></button>
  </div>
  <div class="p-toolbar-group-right">
    <span class="p-text-secondary">Inventory: {{ (inventory$ | async) | currency }}</span>
  </div>
</p-toolbar>

<p-progressBar *ngIf="(loading$ | async)" mode="indeterminate"></p-progressBar>

<p-table [value]="products$ | async" selectionMode="single" (onRowSelect)="onSelect($event.data)">
  <ng-template pTemplate="header">
    <tr>
      <th>Name</th>
      <th>Price</th>
      <th>Stock</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-p>
    <tr>
      <td>{{ p.name }}</td>
      <td>{{ p.price | currency }}</td>
      <td>{{ p.stock }}</td>
    </tr>
  </ng-template>
</p-table>

<p-toast></p-toast>
```

```ts
export class ProductsPageComponent {
  products$ = this.store.select(selectAllProducts);
  loading$ = this.store.select(selectLoading);
  inventory$ = this.store.select(selectInventoryValue);

  constructor(private store: Store, private message: MessageService) {}

  ngOnInit() {
    this.store.dispatch(ProductsActions.load());
    this.store.select(selectError).subscribe((e) => {
      if (e) this.message.add({ severity: 'error', summary: 'Error', detail: e });
    });
  }

  reload() {
    this.store.dispatch(ProductsActions.load());
  }

  onSelect(p: Product) {
    this.store.dispatch(ProductsActions.select({ id: p.id }));
  }
}
```

Module pieces for PrimeNG services:

```ts
providers: [MessageService]
```

## Tests (sketch)

- Reducer: given actions -> next state
- Selectors: memoization and derived values
- Effects: success and failure streams using marble tests

## Checklist

- [ ] Feature folder with actions/reducer/selectors/effects
- [ ] Registered StoreModule and EffectsModule
- [ ] PrimeNG connected to store
- [ ] Devtools configured for dev builds
- [ ] Basic tests outlined

## Key takeaways

- Keep state normalized; prefer @ngrx/entity for collections
- Selectors are the public API of your state
- Effects are for side effects only; keep them pure and focused
- Co-locate feature store with feature module for scalability

## Next day

Continue with Effects patterns and advanced selectors: optimistic updates, error handling strategies, and router-store integration.
