# 08 — HTTP & Interceptors

Fetch data with HttpClient and handle auth/errors in interceptors.

## Provide HttpClient
```ts
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { bootstrapApplication } from '@angular/platform-browser';

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(withInterceptors([authInterceptor]))
  ]
});
```

## Service using HttpClient
```ts
@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}
  getTodos(){ return this.http.get<Todo[]>("/api/todos"); }
}
```

## Auth interceptor
```ts
import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');
  const authReq = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` }}) : req;
  return next(authReq);
};
```

Next: State management with RxJS and Signals.
