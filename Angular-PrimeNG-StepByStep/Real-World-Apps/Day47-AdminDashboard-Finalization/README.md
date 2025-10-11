# Day 47 — Admin Dashboard: Finalization & Polish

Objectives
- Harden the dashboard with auth guard, http interceptor, and loading indicators.
- Polish UI: consistent spacing, empty states, error boundaries.
- Prepare for deployment.

Add AuthGuard
```ts
import { Injectable } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const isLoggedIn = true; // replace with real check
  if (!isLoggedIn) { const r = new Router(); r.navigate(['/login']); return false; }
  return true;
};
```

HTTP Interceptor (basic)
```ts
import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const cloned = req.clone({ setHeaders: { Authorization: 'Bearer token' } });
    return next.handle(cloned);
  }
}
```

Empty states & skeletons
- Use `<p-skeleton>` where data loads.
- Show friendly empty placeholders with actions.

Deployment checklist
- Enable production mode & AOT.
- Preload strategy for routes.
- Lazy-load charts and large modules.
- Set proper base href.

Exercises
1) Add a global progress bar (e.g., with a top bar) for route changes.
2) Add 404 and 500 pages using `p-card` and icons.
3) Extract a LayoutService to manage sidebar state globally.
