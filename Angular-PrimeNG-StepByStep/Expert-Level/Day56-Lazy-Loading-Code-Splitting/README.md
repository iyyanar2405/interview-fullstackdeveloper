# Day 56 — Lazy Loading & Code Splitting

Reduce initial bundle size by loading features on demand. Today you’ll configure lazy modules, standalone component routes, and preloading strategies.

## Learning objectives

- Configure lazy-loaded routes (modules and standalone)
- Use dynamic imports for code splitting
- Apply preloading for UX balance
- Audit bundle size impact

## 1) Lazy modules

```ts
// app-routing.module.ts
const routes: Routes = [
  { path: 'admin', loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule) },
  { path: '', pathMatch: 'full', redirectTo: 'home' },
];
```

Admin module defines its own child routes and imports heavy PrimeNG modules locally.

## 2) Standalone components

```ts
// app.routes.ts (standalone style)
export const routes: Routes = [
  { path: 'product/:id', loadComponent: () => import('./product/product-page.component').then(c => c.ProductPageComponent) },
];
```

## 3) Preloading strategies

```ts
@NgModule({
  imports: [RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
```

Custom strategy (e.g., only preload routes with data.preload):

```ts
@Injectable({ providedIn: 'root' })
export class SelectivePreloadingStrategy implements PreloadingStrategy {
  preload(route: Route, load: () => Observable<any>): Observable<any> {
    return route.data?.['preload'] ? load() : of(null);
  }
}
```

## 4) PrimeNG considerations

- Import heavy components (Table, Chart, FileUpload) only in the lazy feature that needs them
- Keep shared modules slim; avoid re-exporting everything globally

## 5) Verify code splitting

- Confirm network panel loads chunks when navigating to lazy routes
- Ensure SSR or static host serves chunk files correctly

## Checklist

- [ ] Routes converted to lazy where sensible
- [ ] Heavy PrimeNG modules localized to lazy features
- [ ] Preloading configured (all or selective)
- [ ] Verified chunks load on navigation

## Key takeaways

- Route-level lazy loading provides the biggest wins
- Keep shared imports minimal to preserve code-splitting benefits

## Next day

Write robust tests for components using PrimeNG.
