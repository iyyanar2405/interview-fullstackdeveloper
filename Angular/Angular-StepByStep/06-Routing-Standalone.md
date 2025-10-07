# 06 — Routing (Standalone)

Configure router with standalone components, lazy loading, and guards.

## Define routes
```ts
import { Routes } from '@angular/router';
import { HomeComponent } from './home.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'about', loadComponent: () => import('./about.component').then(m => m.AboutComponent) },
  { path: '**', redirectTo: '' }
];
```

## Bootstrap router
```ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app.component';
import { routes } from './app.routes';

bootstrapApplication(AppComponent, { providers: [provideRouter(routes)] });
```

## Simple guard
```ts
import { CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = () => {
  const isLoggedIn = true; // replace with real check
  return isLoggedIn;
};
```

Next: Forms (template-driven and reactive).
