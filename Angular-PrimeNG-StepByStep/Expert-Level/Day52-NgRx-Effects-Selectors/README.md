# Day 52 — NgRx Effects & Selectors Deep Dive

Go beyond the basics: master effect patterns, advanced selectors, router-store, and robust error handling. We’ll enhance the Day 51 store with real-world scenarios and best practices.

## Learning objectives

- Compose selectors across features and compute derived state
- Implement advanced Effects: debounce, switchMap/concatMap/mergeMap/exhaustMap
- Handle optimistic updates, error and retry strategies
- Integrate router-store for URL-driven state
- Structure tests for Effects and Selectors

## Prerequisites

- Completed Day 51 (feature store set up, actions/reducer/selectors/effects)
- HttpClient, PrimeNG Toast, and basic routing configured

## Selector composition patterns

Create cross-feature selectors and memoized computations.

```ts
// products/selectors.ts
export const selectExpensiveProducts = createSelector(selectAllProducts, (items) =>
  items.filter((p) => p.price > 1000)
);

// cart/selectors.ts
export const selectCartTotal = createSelector(selectCartItems, (items) =>
  items.reduce((sum, ci) => sum + ci.qty * ci.price, 0)
);

// dashboard/selectors.ts
export const selectDashboardSummary = createSelector(
  selectInventoryValue,
  selectCartTotal,
  (inventory, cartTotal) => ({ inventory, cartTotal, ratio: inventory ? cartTotal / inventory : 0 })
);
```

Tips:
- Selectors are the public API of your state; avoid selecting raw slices in components
- Derive everything you can; keep state minimal and normalized

## Advanced Effects patterns

```ts
// effects.ts
loadOnRouteEnter$ = createEffect(() =>
  this.actions$.pipe(
    ofType(ROUTER_NAVIGATED), // with @ngrx/router-store
    filter(({ payload }) => payload.event.url.startsWith('/products')),
    map(() => ProductsActions.load())
  )
);

search$ = createEffect(() =>
  this.actions$.pipe(
    ofType(ProductsActions.search),
    debounceTime(300),
    distinctUntilChanged((a, b) => a.query === b.query),
    switchMap(({ query }) => this.api.search(query).pipe(
      map((products) => ProductsActions.searchSuccess({ products })),
      catchError((err) => of(ProductsActions.searchFailure({ error: formatHttpError(err) })))
    ))
  )
);

create$ = createEffect(() =>
  this.actions$.pipe(
    ofType(ProductsActions.create),
    concatMap(({ product }) => this.api.create(product).pipe(
      map((created) => ProductsActions.createSuccess({ product: created })),
      catchError((err) => of(ProductsActions.createFailure({ error: formatHttpError(err) })))
    ))
  )
);

updateOptimistic$ = createEffect(() =>
  this.actions$.pipe(
    ofType(ProductsActions.updateOptimistic),
    mergeMap(({ update }) => this.api.update(update).pipe(
      map((saved) => ProductsActions.updateSuccess({ product: saved })),
      catchError((err) => of(ProductsActions.updateRevert({ error: formatHttpError(err), update })))
    ))
  )
);
```

Operator guide:
- switchMap: cancel prior (search, typeahead)
- concatMap: queue sequentially (create/update one-by-one)
- mergeMap: parallelize (bulk operations)
- exhaustMap: ignore re-entrance until complete (login)

## Error handling and retries

```ts
import { retry, retryWhen, delay, scan } from 'rxjs';

const withBackoff = (max = 3, ms = 500) => retryWhen((errors) =>
  errors.pipe(
    scan((acc, err) => ({ count: acc.count + 1, err }), { count: 0 as number, err: null as any }),
    tap(({ count }) => console.warn(`Retrying... ${count}`)),
    delay(ms),
    takeWhile(({ count }) => count < max)
  )
);

load$ = createEffect(() =>
  this.actions$.pipe(
    ofType(ProductsActions.load),
    switchMap(() => this.api.getAll().pipe(
      withBackoff(3, 300),
      map((products) => ProductsActions.loadSuccess({ products })),
      catchError((err) => of(ProductsActions.loadFailure({ error: formatHttpError(err) })))
    ))
  )
);
```

Centralize formatting:
```ts
export function formatHttpError(err: any): string {
  const msg = err?.error?.message ?? err?.message ?? 'Unexpected error';
  const status = err?.status ? ` (HTTP ${err.status})` : '';
  return `${msg}${status}`;
}
```

## Router-store integration

Install and register:
```ts
import { StoreRouterConnectingModule, routerReducer } from '@ngrx/router-store';

StoreModule.forRoot({ router: routerReducer }),
StoreRouterConnectingModule.forRoot(),
```

Selectors from router state:
```ts
import { getSelectors, RouterReducerState } from '@ngrx/router-store';

export const selectRouter = (state: { router: RouterReducerState }) => state.router;
export const {
  selectCurrentRoute, // current activated route
  selectQueryParams,  // current query params
  selectRouteParams,  // current route params
  selectUrl           // current url
} = getSelectors(selectRouter);

export const selectProductIdFromRoute = createSelector(selectRouteParams, (p) => p['id'] as string | undefined);
export const selectProductFromRoute = createSelector(selectProductIdFromRoute, selectProductEntities, (id, entities) => id ? entities[id] ?? null : null);
```

## Component integration examples

```ts
export class ProductsSearchComponent {
  results$ = this.store.select(selectExpensiveProducts);
  loading$ = this.store.select(selectLoading);

  onQuery(q: string) {
    this.store.dispatch(ProductsActions.search({ query: q }));
  }
}
```

```html
<p-inputText type="text" (input)="onQuery($event.target.value)" placeholder="Search products" />
<p-progressBar *ngIf="(loading$ | async)" mode="indeterminate"></p-progressBar>
<p-table [value]="results$ | async"></p-table>
```

## Testing Effects & Selectors (outline)

- Use provideMockStore for selector tests; assert projector outputs
- Use provideMockActions and TestBed for Effects; marble test success/failure
- Mock HttpClient in services or stub API layer

## Checklist

- [ ] Router-store registered and working
- [ ] Debounced search effect implemented
- [ ] Optimistic update and revert flow
- [ ] Cross-feature selectors composed
- [ ] Error formatting and retry strategy

## Key takeaways

- Choose the right flattening operator for each use case
- Drive UI off selectors; keep components thin
- Let routes drive data loading via router-store
- Centralize error handling and retries for consistency

## Next day

Move to theming and customization: create a custom PrimeNG theme and tweak CSS variables for brand identity.
