# Day 10: HTTP Client & APIs 🌐

## 📋 Learning Objectives
- Use HttpClient for API calls
- Handle HTTP requests (GET, POST, PUT, DELETE)
- Manage errors and loading states
- Implement interceptors
- Work with Observables

---

## 🚀 Setup

```typescript
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  imports: [HttpClientModule]
})
export class AppModule { }
```

---

## 📡 HTTP Service

```typescript
// services/api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'https://jsonplaceholder.typicode.com';
  
  constructor(private http: HttpClient) {}
  
  // GET request
  getUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/users`)
      .pipe(
        retry(3),
        catchError(this.handleError)
      );
  }
  
  // GET by ID
  getUserById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/users/${id}`)
      .pipe(catchError(this.handleError));
  }
  
  // POST request
  createUser(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/users`, user, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    }).pipe(catchError(this.handleError));
  }
  
  // PUT request
  updateUser(id: number, user: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/users/${id}`, user)
      .pipe(catchError(this.handleError));
  }
  
  // DELETE request
  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/users/${id}`)
      .pipe(catchError(this.handleError));
  }
  
  // Error handling
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    console.error(errorMessage);
    return throwError(() => errorMessage);
  }
}
```

---

## 🎯 Component Using API

```typescript
export class UserListComponent implements OnInit, OnDestroy {
  users: any[] = [];
  loading = false;
  error = '';
  subscription!: Subscription;
  
  constructor(private apiService: ApiService) {}
  
  ngOnInit() {
    this.loadUsers();
  }
  
  loadUsers() {
    this.loading = true;
    this.error = '';
    
    this.subscription = this.apiService.getUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = err;
        this.loading = false;
      }
    });
  }
  
  createUser() {
    const newUser = {
      name: 'John Doe',
      email: 'john@example.com'
    };
    
    this.apiService.createUser(newUser).subscribe({
      next: (user) => {
        console.log('User created:', user);
        this.loadUsers();
      },
      error: (err) => console.error('Error creating user:', err)
    });
  }
  
  deleteUser(id: number) {
    if (confirm('Delete this user?')) {
      this.apiService.deleteUser(id).subscribe({
        next: () => {
          this.users = this.users.filter(u => u.id !== id);
        },
        error: (err) => console.error('Error deleting user:', err)
      });
    }
  }
  
  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
```

```html
<div class="container">
  <h1>User List</h1>
  
  <div *ngIf="loading" class="loading">Loading...</div>
  
  <div *ngIf="error" class="error">{{ error }}</div>
  
  <button (click)="loadUsers()">Refresh</button>
  <button (click)="createUser()">Add User</button>
  
  <table *ngIf="!loading && !error">
    <thead>
      <tr>
        <th>ID</th>
        <th>Name</th>
        <th>Email</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let user of users">
        <td>{{ user.id }}</td>
        <td>{{ user.name }}</td>
        <td>{{ user.email }}</td>
        <td>
          <button (click)="deleteUser(user.id)">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>
</div>
```

---

## 🔐 HTTP Interceptor

```bash
ng g interceptor interceptors/auth
```

```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('token');
    
    if (token) {
      const cloned = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
      return next.handle(cloned);
    }
    
    return next.handle(req);
  }
}
```

**Register:**
```typescript
import { HTTP_INTERCEPTORS } from '@angular/common/http';

@NgModule({
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ]
})
export class AppModule { }
```

---

## 🎯 CRUD Example

```typescript
// services/product.service.ts
export class ProductService {
  private apiUrl = 'http://localhost:3000/api/products';
  
  constructor(private http: HttpClient) {}
  
  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }
  
  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }
  
  create(product: Product): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }
  
  update(id: number, product: Product): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, product);
  }
  
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  
  search(term: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}?search=${term}`);
  }
}
```

---

✅ **Foundation Phase Complete!** 🎉

**Next: PrimeNG Components (Days 11-20)** 🎨

You've mastered:
- ✅ Angular Setup
- ✅ TypeScript
- ✅ Components
- ✅ Directives
- ✅ Pipes
- ✅ Services & DI
- ✅ Routing
- ✅ Forms (Template & Reactive)
- ✅ HTTP & APIs

**Ready for PrimeNG!** 🚀
