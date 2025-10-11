# Day 56 — Code Splitting

## Objectives
- Dynamically import large pages/components
- Preload on hover or near-view interactions
- Organize route-based and component-based splitting

## Route-based
```tsx
const CatalogPage = React.lazy(() => import('../pages/CatalogPage'));

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/shop" element={<Suspense fallback={<ProgressBar mode='indeterminate' style={{ height: 3 }} />}> <CatalogPage /> </Suspense>} />
    </Routes>
  );
}
```

## Component-based
```tsx
const HeavyChart = React.lazy(() => import('./HeavyChart'));

function AnalyticsPanel() {
  const [show, setShow] = useState(false);
  return (
    <div>
      <Button label="Show Analytics" onClick={() => setShow(true)} />
      {show && (
        <Suspense fallback={<Skeleton height="12rem" />}>
          <HeavyChart />
        </Suspense>
      )}
    </div>
  );
}
```

## Preloading
```tsx
const preloadCatalog = () => import('../pages/CatalogPage');

<Button label="Shop" onMouseEnter={preloadCatalog} onFocus={preloadCatalog} />
```

## Checklist
- [ ] Suspense fallbacks present
- [ ] Preload critical paths
- [ ] Split by route and component
- [ ] No layout shift during load