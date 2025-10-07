# 07 — State (Redux/NgRx)

Use NgRx in an Nx Angular app with provideStore and feature libs.

## Install
```powershell
npm i @ngrx/store @ngrx/effects @ngrx/entity @ngrx/store-devtools
```

## Provide store in app
```ts
bootstrapApplication(AppComponent, {
  providers: [provideStore(), provideEffects(), provideStoreDevtools()]
});
```

## Generate feature state in a lib
```powershell
npx nx g @ngrx/schematics:feature state --project=features-todos --group
```

Wire reducers/effects in the app or feature route providers.
