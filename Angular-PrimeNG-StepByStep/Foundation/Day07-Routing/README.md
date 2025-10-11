# Day 07: Routing & Navigation 🧭

## 📋 Learning Objectives
- Set up Angular Router
- Create routes and navigate between views
- Use route parameters and query parameters
- Implement child routes
- Apply route guards for authentication
- Understand lazy loading

---

## 🚀 Setting Up Router

### Install Router (usually included)
```bash
ng new my-app --routing
```

### app-routing.module.ts
```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { AboutComponent } from './pages/about/about.component';
import { ContactComponent } from './pages/contact/contact.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'about', component: AboutComponent },
  { path: 'contact', component: ContactComponent },
  { path: '**', component: NotFoundComponent }  // 404
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
```

### app.component.html
```html
<nav>
  <a routerLink="/home" routerLinkActive="active">Home</a>
  <a routerLink="/about" routerLinkActive="active">About</a>
  <a routerLink="/contact" routerLinkActive="active">Contact</a>
</nav>

<router-outlet></router-outlet>
```

---

## 🎯 Route Parameters

### Dynamic Routes
```typescript
const routes: Routes = [
  { path: 'user/:id', component: UserDetailComponent },
  { path: 'product/:id', component: ProductDetailComponent },
  { path: 'blog/:category/:postId', component: BlogPostComponent }
];
```

### Accessing Route Parameters
```typescript
import { ActivatedRoute } from '@angular/router';

export class UserDetailComponent implements OnInit {
  userId!: number;
  
  constructor(private route: ActivatedRoute) {}
  
  ngOnInit() {
    // Snapshot (one-time read)
    this.userId = +this.route.snapshot.paramMap.get('id')!;
    
    // Observable (reactive)
    this.route.paramMap.subscribe(params => {
      this.userId = +params.get('id')!;
      this.loadUser(this.userId);
    });
  }
}
```

### Query Parameters
```html
<!-- Navigate with query params -->
<a [routerLink]="['/products']" [queryParams]="{category: 'electronics', sort: 'price'}">
  Electronics
</a>
```

```typescript
export class ProductsComponent implements OnInit {
  constructor(private route: ActivatedRoute) {}
  
  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      const category = params.get('category');
      const sort = params.get('sort');
      this.loadProducts(category, sort);
    });
  }
}
```

---

## 🔒 Route Guards

### AuthGuard
```bash
ng g guard guards/auth
```

```typescript
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}
  
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    if (this.authService.isLoggedIn()) {
      return true;
    }
    
    this.router.navigate(['/login'], {
      queryParams: { returnUrl: state.url }
    });
    return false;
  }
}
```

### Apply Guard
```typescript
const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] }
];
```

---

## 🎯 Programmatic Navigation

```typescript
import { Router } from '@angular/router';

export class HomeComponent {
  constructor(private router: Router) {}
  
  goToAbout() {
    this.router.navigate(['/about']);
  }
  
  goToUser(id: number) {
    this.router.navigate(['/user', id]);
  }
  
  goToProducts(category: string) {
    this.router.navigate(['/products'], {
      queryParams: { category: category }
    });
  }
  
  goBack() {
    window.history.back();
  }
}
```

---

## 🌳 Child Routes

```typescript
const routes: Routes = [
  {
    path: 'admin',
    component: AdminComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboardComponent },
      { path: 'users', component: AdminUsersComponent },
      { path: 'settings', component: AdminSettingsComponent }
    ]
  }
];
```

```html
<!-- admin.component.html -->
<div class="admin-layout">
  <aside>
    <a routerLink="dashboard">Dashboard</a>
    <a routerLink="users">Users</a>
    <a routerLink="settings">Settings</a>
  </aside>
  <main>
    <router-outlet></router-outlet>
  </main>
</div>
```

---

## ⚡ Lazy Loading

```typescript
const routes: Routes = [
  {
    path: 'admin',
    loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule)
  },
  {
    path: 'shop',
    loadChildren: () => import('./shop/shop.module').then(m => m.ShopModule)
  }
];
```

---

✅ **Day 07 Complete!** Tomorrow: Template-Driven Forms 📝
