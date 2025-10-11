# Day 06: Services & Dependency Injection 💉

## 📋 Learning Objectives

- Understand services and dependency injection (DI)
- Create and provide services
- Use services across components
- Implement singleton and multi-instance patterns
- Share data between components using services
- Master Angular's hierarchical injector system

---

## 🎯 What is a Service?

A service is a class with a specific purpose:
- **Business logic**
- **Data fetching** (API calls)
- **State management**
- **Utility functions**
- **Logging**

Services promote **code reusability** and **separation of concerns**.

---

## 🔨 Creating Services

### Method 1: Angular CLI (Recommended)

```bash
# Generate service
ng generate service services/user

# Shorthand
ng g s services/user

# Service with providedIn root
ng g s services/data --skip-tests
```

### Generated Service Structure

```typescript
// user.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'  // Singleton across entire app
})
export class UserService {
  constructor() { }
}
```

---

## 💉 Dependency Injection (DI)

DI is a design pattern where dependencies are provided to a class instead of the class creating them.

### Injecting Services

```typescript
// Component using service
import { Component, OnInit } from '@angular/core';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-user-list',
  template: `
    <div *ngFor="let user of users">
      {{ user.name }}
    </div>
  `
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  
  // Inject service via constructor
  constructor(private userService: UserService) {}
  
  ngOnInit() {
    this.users = this.userService.getUsers();
  }
}
```

---

## 🏗️ Complete Service Examples

### 1. User Service

```typescript
// models/user.model.ts
export interface User {
  id: number;
  name: string;
  email: string;
  role: 'admin' | 'user' | 'guest';
}
```

```typescript
// services/user.service.ts
import { Injectable } from '@angular/core';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private users: User[] = [
    { id: 1, name: 'John Doe', email: 'john@example.com', role: 'admin' },
    { id: 2, name: 'Jane Smith', email: 'jane@example.com', role: 'user' },
    { id: 3, name: 'Bob Johnson', email: 'bob@example.com', role: 'user' }
  ];
  
  constructor() {}
  
  // Get all users
  getUsers(): User[] {
    return [...this.users]; // Return copy to prevent mutation
  }
  
  // Get user by ID
  getUserById(id: number): User | undefined {
    return this.users.find(user => user.id === id);
  }
  
  // Add new user
  addUser(user: User): void {
    this.users.push(user);
  }
  
  // Update user
  updateUser(id: number, updatedUser: Partial<User>): void {
    const index = this.users.findIndex(u => u.id === id);
    if (index !== -1) {
      this.users[index] = { ...this.users[index], ...updatedUser };
    }
  }
  
  // Delete user
  deleteUser(id: number): void {
    this.users = this.users.filter(u => u.id !== id);
  }
  
  // Get users by role
  getUsersByRole(role: string): User[] {
    return this.users.filter(u => u.role === role);
  }
}
```

### 2. Logger Service

```typescript
// services/logger.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoggerService {
  private logs: string[] = [];
  
  log(message: string): void {
    const timestamp = new Date().toISOString();
    const logMessage = `[${timestamp}] ${message}`;
    this.logs.push(logMessage);
    console.log(logMessage);
  }
  
  error(message: string): void {
    const timestamp = new Date().toISOString();
    const errorMessage = `[${timestamp}] ERROR: ${message}`;
    this.logs.push(errorMessage);
    console.error(errorMessage);
  }
  
  warn(message: string): void {
    const timestamp = new Date().toISOString();
    const warnMessage = `[${timestamp}] WARN: ${message}`;
    this.logs.push(warnMessage);
    console.warn(warnMessage);
  }
  
  getLogs(): string[] {
    return [...this.logs];
  }
  
  clearLogs(): void {
    this.logs = [];
  }
}
```

**Usage:**
```typescript
export class AppComponent {
  constructor(private logger: LoggerService) {}
  
  saveData() {
    this.logger.log('Saving data...');
    // Save logic
    this.logger.log('Data saved successfully');
  }
}
```

### 3. Data Sharing Service

```typescript
// services/data-sharing.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataSharingService {
  // BehaviorSubject to share data
  private messageSource = new BehaviorSubject<string>('Default Message');
  currentMessage$ = this.messageSource.asObservable();
  
  private userSource = new BehaviorSubject<any>(null);
  currentUser$ = this.userSource.asObservable();
  
  constructor() {}
  
  changeMessage(message: string): void {
    this.messageSource.next(message);
  }
  
  updateUser(user: any): void {
    this.userSource.next(user);
  }
}
```

**Component A (Sender):**
```typescript
export class ComponentA {
  constructor(private dataService: DataSharingService) {}
  
  sendMessage() {
    this.dataService.changeMessage('Hello from Component A!');
  }
}
```

**Component B (Receiver):**
```typescript
export class ComponentB implements OnInit {
  message = '';
  
  constructor(private dataService: DataSharingService) {}
  
  ngOnInit() {
    this.dataService.currentMessage$.subscribe(
      message => this.message = message
    );
  }
}
```

---

## 🔧 Service Providers

### 1. Root Level (Singleton)

```typescript
@Injectable({
  providedIn: 'root'  // Single instance for entire app
})
export class GlobalService {}
```

### 2. Module Level

```typescript
// app.module.ts
@NgModule({
  providers: [UserService]  // Single instance per module
})
export class AppModule {}
```

### 3. Component Level

```typescript
@Component({
  selector: 'app-user',
  providers: [UserService]  // New instance for each component
})
export class UserComponent {}
```

---

## 🏢 Service Hierarchical Injector

```
Root Injector (providedIn: 'root')
    ↓
Module Injector (providers: [])
    ↓
Component Injector (providers: [])
```

**Rule:** Angular looks up the injector tree until it finds the service.

---

## 🎯 Practical Exercise 1: Shopping Cart Service

```typescript
// models/product.model.ts
export interface Product {
  id: number;
  name: string;
  price: number;
  image: string;
}

export interface CartItem extends Product {
  quantity: number;
}
```

```typescript
// services/cart.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Product, CartItem } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItems: CartItem[] = [];
  private cartSubject = new BehaviorSubject<CartItem[]>([]);
  cart$: Observable<CartItem[]> = this.cartSubject.asObservable();
  
  constructor() {
    // Load from localStorage if available
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      this.cartItems = JSON.parse(savedCart);
      this.cartSubject.next(this.cartItems);
    }
  }
  
  addToCart(product: Product): void {
    const existingItem = this.cartItems.find(item => item.id === product.id);
    
    if (existingItem) {
      existingItem.quantity++;
    } else {
      this.cartItems.push({ ...product, quantity: 1 });
    }
    
    this.updateCart();
  }
  
  removeFromCart(productId: number): void {
    this.cartItems = this.cartItems.filter(item => item.id !== productId);
    this.updateCart();
  }
  
  updateQuantity(productId: number, quantity: number): void {
    const item = this.cartItems.find(item => item.id === productId);
    if (item) {
      item.quantity = quantity;
      if (item.quantity <= 0) {
        this.removeFromCart(productId);
      } else {
        this.updateCart();
      }
    }
  }
  
  clearCart(): void {
    this.cartItems = [];
    this.updateCart();
  }
  
  getCartItems(): CartItem[] {
    return [...this.cartItems];
  }
  
  getTotal(): number {
    return this.cartItems.reduce(
      (total, item) => total + (item.price * item.quantity), 
      0
    );
  }
  
  getItemCount(): number {
    return this.cartItems.reduce((count, item) => count + item.quantity, 0);
  }
  
  private updateCart(): void {
    this.cartSubject.next(this.cartItems);
    localStorage.setItem('cart', JSON.stringify(this.cartItems));
  }
}
```

**Product List Component:**
```typescript
export class ProductListComponent implements OnInit {
  products: Product[] = [
    { id: 1, name: 'Laptop', price: 999, image: 'laptop.jpg' },
    { id: 2, name: 'Phone', price: 599, image: 'phone.jpg' },
    { id: 3, name: 'Headphones', price: 99, image: 'headphones.jpg' }
  ];
  
  constructor(private cartService: CartService) {}
  
  ngOnInit() {}
  
  addToCart(product: Product) {
    this.cartService.addToCart(product);
  }
}
```

```html
<div class="products">
  <div *ngFor="let product of products" class="product-card">
    <img [src]="product.image" [alt]="product.name">
    <h3>{{ product.name }}</h3>
    <p>{{ product.price | currency }}</p>
    <button (click)="addToCart(product)">Add to Cart</button>
  </div>
</div>
```

**Cart Component:**
```typescript
export class CartComponent implements OnInit, OnDestroy {
  cartItems: CartItem[] = [];
  total = 0;
  subscription!: Subscription;
  
  constructor(private cartService: CartService) {}
  
  ngOnInit() {
    this.subscription = this.cartService.cart$.subscribe(items => {
      this.cartItems = items;
      this.total = this.cartService.getTotal();
    });
  }
  
  updateQuantity(productId: number, quantity: number) {
    this.cartService.updateQuantity(productId, quantity);
  }
  
  removeItem(productId: number) {
    this.cartService.removeFromCart(productId);
  }
  
  clearCart() {
    if (confirm('Clear cart?')) {
      this.cartService.clearCart();
    }
  }
  
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
```

---

## 🎯 Practical Exercise 2: Authentication Service

```typescript
// services/auth.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

interface User {
  id: number;
  username: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  currentUser$: Observable<User | null> = this.currentUserSubject.asObservable();
  
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  isAuthenticated$: Observable<boolean> = this.isAuthenticatedSubject.asObservable();
  
  constructor() {
    // Check if user is already logged in
    const savedUser = localStorage.getItem('currentUser');
    if (savedUser) {
      this.currentUserSubject.next(JSON.parse(savedUser));
      this.isAuthenticatedSubject.next(true);
    }
  }
  
  login(username: string, password: string): boolean {
    // Simulate authentication
    if (username && password) {
      const user: User = {
        id: 1,
        username: username,
        email: `${username}@example.com`,
        role: 'user'
      };
      
      localStorage.setItem('currentUser', JSON.stringify(user));
      this.currentUserSubject.next(user);
      this.isAuthenticatedSubject.next(true);
      return true;
    }
    return false;
  }
  
  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }
  
  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }
  
  isLoggedIn(): boolean {
    return this.isAuthenticatedSubject.value;
  }
  
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user ? user.role === role : false;
  }
}
```

---

## ✅ Day 06 Checklist

- [ ] Created services using Angular CLI
- [ ] Understood @Injectable decorator
- [ ] Injected services via constructor
- [ ] Implemented singleton pattern (providedIn: 'root')
- [ ] Created user management service
- [ ] Built logger service
- [ ] Implemented data sharing service with BehaviorSubject
- [ ] Created shopping cart service (Exercise 1)
- [ ] Built authentication service (Exercise 2)
- [ ] Understood hierarchical injection

---

## 🔑 Key Takeaways

1. **Services centralize business logic** and promote reusability
2. **Use @Injectable()** to make a class injectable
3. **providedIn: 'root'** creates a singleton service
4. **BehaviorSubject** is perfect for sharing state
5. **Dependency injection** makes testing easier
6. **Services should be stateless** when possible
7. **Use services for API calls, state management, and utilities**

---

## 📚 Additional Resources

- [Angular Services](https://angular.io/guide/architecture-services)
- [Dependency Injection](https://angular.io/guide/dependency-injection)
- [Hierarchical Injectors](https://angular.io/guide/hierarchical-dependency-injection)

---

## 🎯 Next Steps

Tomorrow (Day 07), we'll cover:
- **Routing and Navigation**
- Setting up routes
- Route parameters
- Child routes
- Route guards

Great job! Services are the backbone of Angular apps! 💪
