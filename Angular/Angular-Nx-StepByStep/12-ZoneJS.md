# 12 — Zone.js in Angular (Nx)

Understand Zone.js defaults, coalescing, and zoneless setups in Angular.

## Zone.js basics
- Angular uses Zone.js to patch async APIs and trigger change detection
- Included by default in Angular CLI/Nx apps

## Change detection coalescing
- Reduce redundant change detection cycles
- In `bootstrapApplication`, enable coalescing:
```ts
bootstrapApplication(AppComponent, {
  providers: [{ provide: ChangeDetectionScheduler, useValue: coalesceEvents() }]
});
```

## Zoneless Angular
- Run without Zone.js for performance-sensitive apps
- Disable zone by setting `ngZone: 'noop'` at bootstrap
```ts
bootstrapApplication(AppComponent, {
  ngZone: 'noop'
});
```
- Then trigger updates via Signals, `ChangeDetectorRef`, or RxJS

## Signals interplay
- Signals can drive change detection without zones
- Combine `toSignal` for store selectors and manual triggers where needed

## Nx considerations
- Keep the configuration at app entry point; libs remain framework-agnostic
- Measure with Angular DevTools to validate performance impact
