# Angular Interview Questions & Answers

## Table of Contents
1. [Angular Core Concepts](#angular-core-concepts)
2. [Angular CLI (ng)](#angular-cli-ng)
3. [Nx Workspace](#nx-workspace)
4. [RxJS](#rxjs)
5. [State Management (Redux, NgRx, NGXS)](#state-management)
6. [Apollo Client & GraphQL](#apollo-client--graphql)
7. [UI Libraries (PrimeNG, Angular Material)](#ui-libraries)
8. [TypeScript](#typescript)
9. [Architecture & Design Patterns](#architecture--design-patterns)
10. [Performance Optimization](#performance-optimization)
11. [Version Support & Migration](#version-support--migration)

---

## Angular Core Concepts

### Beginner Level

#### Q1: What is Angular and what are its key features?
**Answer:** 
Angular is a TypeScript-based open-source web application framework developed by Google. Key features include:

- **Component-based architecture**: Applications are built using reusable components
- **Two-way data binding**: Automatic synchronization between model and view
- **Dependency Injection**: Built-in DI system for better modularity
- **TypeScript support**: Strong typing and modern JavaScript features
- **CLI support**: Powerful command-line interface for development
- **Cross-platform**: Web, mobile, and desktop applications

**Example:**
```typescript
// Simple Angular component
import { Component } from '@angular/core';

@Component({
  selector: 'app-hello',
  template: `
    <h1>{{title}}</h1>
    <input [(ngModel)]="name" placeholder="Enter name">
    <p>Hello, {{name}}!</p>
  `
})
export class HelloComponent {
  title = 'Welcome to Angular';
  name = '';
}
```

#### Q2: What is the difference between AngularJS and Angular?
**Answer:**
| AngularJS (1.x) | Angular (2+) |
|-----------------|--------------|
| JavaScript-based | TypeScript-based |
| MVC architecture | Component-based |
| No mobile support | Mobile-first approach |
| Digest cycle | Zone.js for change detection |
| No CLI | Angular CLI |
| Controllers & Scope | Components & Services |

#### Q3: Explain Angular component lifecycle hooks.
**Answer:**
Lifecycle hooks are methods that Angular calls at specific moments in a component's lifecycle:

**Example:**
```typescript
import { Component, OnInit, OnDestroy, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-lifecycle',
  template: '<p>Lifecycle Demo</p>'
})
export class LifecycleComponent implements OnInit, OnDestroy, OnChanges {
  
  constructor() {
    console.log('1. Constructor called');
  }
  
  ngOnChanges(changes: SimpleChanges) {
    console.log('2. ngOnChanges called', changes);
  }
  
  ngOnInit() {
    console.log('3. ngOnInit called');
  }
  
  ngOnDestroy() {
    console.log('4. ngOnDestroy called');
  }
}
```

**Lifecycle Order:**
1. `ngOnChanges` - When input properties change
2. `ngOnInit` - After first ngOnChanges
3. `ngDoCheck` - During every change detection run
4. `ngAfterContentInit` - After content projection
5. `ngAfterContentChecked` - After every check of projected content
6. `ngAfterViewInit` - After component's view initialization
7. `ngAfterViewChecked` - After every check of component's view
8. `ngOnDestroy` - Before component destruction

#### Q4: What is data binding in Angular? Explain different types.
**Answer:**
Data binding is the synchronization between the model and the view.

**Types:**
1. **Interpolation** - One-way (component to view)
2. **Property Binding** - One-way (component to view)
3. **Event Binding** - One-way (view to component)
4. **Two-way Binding** - Bidirectional

**Example:**
```typescript
@Component({
  selector: 'app-binding',
  template: `
    <!-- 1. Interpolation -->
    <h1>{{title}}</h1>
    
    <!-- 2. Property Binding -->
    <img [src]="imageUrl" [alt]="imageAlt">
    <button [disabled]="isDisabled">Click me</button>
    
    <!-- 3. Event Binding -->
    <button (click)="onClick()">Click me</button>
    <input (keyup)="onKeyUp($event)">
    
    <!-- 4. Two-way Binding -->
    <input [(ngModel)]="username">
    <p>Username: {{username}}</p>
  `
})
export class BindingComponent {
  title = 'Data Binding Demo';
  imageUrl = 'assets/logo.png';
  imageAlt = 'Logo';
  isDisabled = false;
  username = '';
  
  onClick() {
    console.log('Button clicked');
  }
  
  onKeyUp(event: any) {
    console.log('Key pressed:', event.target.value);
  }
}
```

### Intermediate Level

#### Q5: Explain Angular services and dependency injection.
**Answer:**
Services are singleton objects that provide specific functionality across components. Dependency Injection (DI) is a design pattern where dependencies are provided to a class rather than created by the class itself.

**Example:**
```typescript
// Service
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root' // Singleton service
})
export class UserService {
  private apiUrl = 'https://api.example.com/users';
  
  constructor(private http: HttpClient) {}
  
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl);
  }
  
  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }
}

// Component using the service
@Component({
  selector: 'app-user-list',
  template: `
    <div *ngFor="let user of users">
      {{user.name}}
    </div>
  `
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  
  constructor(private userService: UserService) {}
  
  ngOnInit() {
    this.userService.getUsers().subscribe(users => {
      this.users = users;
    });
  }
}
```

#### Q6: What are Angular directives? Explain different types.
**Answer:**
Directives are classes that add additional behavior to elements in Angular applications.

**Types:**
1. **Component Directives** - Directives with templates
2. **Structural Directives** - Change DOM layout (*ngIf, *ngFor)
3. **Attribute Directives** - Change appearance/behavior (ngClass, ngStyle)

**Example:**
```typescript
// 1. Structural Directives
@Component({
  template: `
    <!-- *ngIf -->
    <div *ngIf="isLoggedIn">Welcome, {{username}}!</div>
    
    <!-- *ngFor -->
    <ul>
      <li *ngFor="let item of items; let i = index; trackBy: trackByFn">
        {{i + 1}}. {{item.name}}
      </li>
    </ul>
    
    <!-- *ngSwitch -->
    <div [ngSwitch]="userRole">
      <div *ngSwitchCase="'admin'">Admin Panel</div>
      <div *ngSwitchCase="'user'">User Dashboard</div>
      <div *ngSwitchDefault>Guest View</div>
    </div>
  `
})
export class DirectivesComponent {
  isLoggedIn = true;
  username = 'John';
  userRole = 'admin';
  items = [
    { id: 1, name: 'Item 1' },
    { id: 2, name: 'Item 2' }
  ];
  
  trackByFn(index: number, item: any) {
    return item.id;
  }
}

// 2. Custom Attribute Directive
import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[appHighlight]'
})
export class HighlightDirective {
  @Input() appHighlight = 'yellow';
  
  constructor(private el: ElementRef) {}
  
  @HostListener('mouseenter') onMouseEnter() {
    this.highlight(this.appHighlight);
  }
  
  @HostListener('mouseleave') onMouseLeave() {
    this.highlight('');
  }
  
  private highlight(color: string) {
    this.el.nativeElement.style.backgroundColor = color;
  }
}

// Usage: <p appHighlight="lightblue">Highlight me!</p>
```

#### Q7: Explain Angular pipes and create a custom pipe.
**Answer:**
Pipes are used to transform data in templates. Angular provides built-in pipes and allows custom pipes.

**Example:**
```typescript
// Built-in pipes usage
@Component({
  template: `
    <!-- Built-in pipes -->
    <p>{{ name | uppercase }}</p>
    <p>{{ price | currency:'USD':'symbol':'1.2-2' }}</p>
    <p>{{ today | date:'fullDate' }}</p>
    <p>{{ items | json }}</p>
    
    <!-- Custom pipe -->
    <p>{{ text | truncate:10 }}</p>
  `
})
export class PipeComponent {
  name = 'john doe';
  price = 1234.56;
  today = new Date();
  items = ['apple', 'banana'];
  text = 'This is a very long text that needs truncation';
}

// Custom Pipe
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'truncate'
})
export class TruncatePipe implements PipeTransform {
  transform(value: string, limit: number = 10, ellipsis: string = '...'): string {
    if (!value) return '';
    
    return value.length > limit 
      ? value.substring(0, limit) + ellipsis 
      : value;
  }
}

// Pure vs Impure Pipe
@Pipe({
  name: 'impureFilter',
  pure: false // Will be called on every change detection cycle
})
export class ImpureFilterPipe implements PipeTransform {
  transform(items: any[], filter: string): any[] {
    if (!items || !filter) return items;
    return items.filter(item => 
      item.name.toLowerCase().includes(filter.toLowerCase())
    );
  }
}
```

### Expert Level

#### Q8: Explain Angular change detection and OnPush strategy.
**Answer:**
Change detection is the mechanism by which Angular keeps the view in sync with the model.

**Default Strategy:**
- Checks all components on every change detection cycle
- Triggered by events, HTTP requests, timers

**OnPush Strategy:**
- Only checks when:
  - Input properties change (reference)
  - Event is triggered
  - Manually triggered

**Example:**
```typescript
// OnPush Strategy Component
import { Component, Input, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-onpush',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h3>OnPush Component</h3>
    <p>Count: {{count}}</p>
    <p>User: {{user.name}}</p>
    <button (click)="increment()">Increment</button>
    <button (click)="updateUser()">Update User</button>
    <button (click)="updateUserImmutable()">Update User (Immutable)</button>
  `
})
export class OnPushComponent {
  @Input() user: any = { name: 'John', age: 30 };
  count = 0;
  
  constructor(private cdr: ChangeDetectorRef) {}
  
  increment() {
    this.count++; // This will trigger change detection (event)
  }
  
  updateUser() {
    // This won't trigger change detection (same reference)
    this.user.name = 'Jane';
  }
  
  updateUserImmutable() {
    // This will trigger change detection (new reference)
    this.user = { ...this.user, name: 'Jane' };
  }
  
  manualDetection() {
    // Manually trigger change detection
    this.cdr.detectChanges();
  }
}

// Immutable update patterns
export class ImmutableUpdates {
  // Array updates
  addItem(items: any[], newItem: any) {
    return [...items, newItem]; // New array reference
  }
  
  removeItem(items: any[], index: number) {
    return items.filter((_, i) => i !== index);
  }
  
  updateItem(items: any[], index: number, newItem: any) {
    return items.map((item, i) => i === index ? newItem : item);
  }
  
  // Object updates
  updateObject(obj: any, updates: any) {
    return { ...obj, ...updates };
  }
  
  updateNestedObject(obj: any, path: string, value: any) {
    return {
      ...obj,
      [path]: {
        ...obj[path],
        value
      }
    };
  }
}
```

#### Q9: Explain Angular modules and lazy loading.
**Answer:**
Modules organize application functionality and enable lazy loading for better performance.

**Example:**
```typescript
// Feature Module
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { FeatureComponent } from './feature.component';
import { FeatureService } from './feature.service';

@NgModule({
  declarations: [
    FeatureComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: '', component: FeatureComponent }
    ])
  ],
  providers: [
    FeatureService
  ]
})
export class FeatureModule {}

// Lazy Loading Routes
const routes: Routes = [
  {
    path: 'feature',
    loadChildren: () => import('./feature/feature.module').then(m => m.FeatureModule)
  },
  {
    path: 'admin',
    loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule),
    canLoad: [AuthGuard] // Guard for lazy-loaded modules
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}

// Preloading Strategies
import { PreloadAllModules, PreloadingStrategy, Route } from '@angular/router';

// 1. Preload all modules
RouterModule.forRoot(routes, {
  preloadingStrategy: PreloadAllModules
})

// 2. Custom preloading strategy
export class CustomPreloadingStrategy implements PreloadingStrategy {
  preload(route: Route, load: () => Observable<any>): Observable<any> {
    if (route.data && route.data['preload']) {
      return load();
    }
    return of(null);
  }
}

// Usage in routes
{
  path: 'feature',
  loadChildren: () => import('./feature/feature.module').then(m => m.FeatureModule),
  data: { preload: true }
}
```

---

## Angular CLI (ng)

### Beginner Level

#### Q10: What is Angular CLI and what are its main commands?
**Answer:**
Angular CLI is a command-line interface tool for Angular development that automates tasks like project creation, code generation, testing, and deployment.

**Main Commands:**
```bash
# Create new project
ng new my-app
ng new my-app --routing --style=scss --skip-tests

# Generate components, services, etc.
ng generate component my-component
ng g c my-component --skip-tests --inline-style
ng g s services/my-service
ng g m modules/my-module --routing
ng g guard guards/auth
ng g pipe pipes/my-pipe
ng g directive directives/my-directive

# Development server
ng serve
ng serve --port 4200 --open
ng serve --configuration=production

# Build application
ng build
ng build --prod
ng build --configuration=production --aot

# Testing
ng test
ng test --watch=false --browsers=ChromeHeadless
ng e2e

# Linting
ng lint
ng lint --fix

# Update dependencies
ng update
ng update @angular/core @angular/cli
ng update --all

# Add packages
ng add @angular/material
ng add @ngrx/store
ng add @angular/pwa
```

#### Q11: How do you configure different environments in Angular?
**Answer:**
Angular uses environment files to manage different configurations for development, staging, and production.

**Example:**
```typescript
// src/environments/environment.ts (development)
export const environment = {
  production: false,
  apiUrl: 'http://localhost:3000/api',
  appName: 'MyApp Dev',
  enableDebug: true,
  firebaseConfig: {
    apiKey: 'dev-api-key',
    authDomain: 'dev-app.firebaseapp.com'
  }
};

// src/environments/environment.prod.ts (production)
export const environment = {
  production: true,
  apiUrl: 'https://api.myapp.com',
  appName: 'MyApp',
  enableDebug: false,
  firebaseConfig: {
    apiKey: 'prod-api-key',
    authDomain: 'myapp.firebaseapp.com'
  }
};

// src/environments/environment.staging.ts
export const environment = {
  production: true,
  apiUrl: 'https://staging-api.myapp.com',
  appName: 'MyApp Staging',
  enableDebug: true,
  firebaseConfig: {
    apiKey: 'staging-api-key',
    authDomain: 'staging-myapp.firebaseapp.com'
  }
};

// Usage in service
import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;
  
  constructor() {
    if (environment.enableDebug) {
      console.log('Debug mode enabled');
    }
  }
}

// angular.json configuration
{
  "configurations": {
    "production": {
      "fileReplacements": [
        {
          "replace": "src/environments/environment.ts",
          "with": "src/environments/environment.prod.ts"
        }
      ]
    },
    "staging": {
      "fileReplacements": [
        {
          "replace": "src/environments/environment.ts",
          "with": "src/environments/environment.staging.ts"
        }
      ]
    }
  }
}

// Build commands
ng build --configuration=production
ng build --configuration=staging
ng serve --configuration=staging
```

### Intermediate Level

#### Q12: How do you customize Angular CLI and webpack configuration?
**Answer:**
Angular CLI uses webpack internally and provides several ways to customize the build process.

**Example:**
```typescript
// 1. Custom webpack config using @angular-builders/custom-webpack
npm install @angular-builders/custom-webpack --save-dev

// webpack.config.js
const path = require('path');

module.exports = {
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
      '@components': path.resolve(__dirname, 'src/app/components'),
      '@services': path.resolve(__dirname, 'src/app/services'),
      '@environments': path.resolve(__dirname, 'src/environments')
    }
  },
  module: {
    rules: [
      {
        test: /\.scss$/,
        use: [
          'style-loader',
          'css-loader',
          'sass-loader'
        ]
      }
    ]
  },
  plugins: [
    // Custom plugins
  ]
};

// angular.json
{
  "projects": {
    "my-app": {
      "architect": {
        "build": {
          "builder": "@angular-builders/custom-webpack:browser",
          "options": {
            "customWebpackConfig": {
              "path": "./webpack.config.js"
            }
          }
        },
        "serve": {
          "builder": "@angular-builders/custom-webpack:dev-server"
        }
      }
    }
  }
}

// 2. Angular CLI Schematics
ng config cli.defaultCollection @angular/material

// 3. Custom schematics
ng generate @schematics/angular:component --name=my-component

// 4. Angular.json customization
{
  "projects": {
    "my-app": {
      "architect": {
        "build": {
          "options": {
            "assets": [
              "src/favicon.ico",
              "src/assets",
              {
                "glob": "**/*",
                "input": "node_modules/some-package/assets",
                "output": "/assets/some-package"
              }
            ],
            "styles": [
              "src/styles.scss",
              "node_modules/bootstrap/dist/css/bootstrap.min.css"
            ],
            "scripts": [
              "node_modules/jquery/dist/jquery.min.js"
            ]
          }
        }
      }
    }
  }
}
```

---

## Nx Workspace

### Beginner Level

#### Q13: What is Nx and how does it differ from Angular CLI?
**Answer:**
Nx is a build system with monorepo support and advanced tooling for Angular, React, Node.js, and other frameworks.

**Key Differences:**
| Angular CLI | Nx |
|-------------|-----|
| Single app focus | Monorepo support |
| Basic generators | Advanced generators |
| Limited workspace | Multiple apps/libs |
| No dependency graph | Dependency graph |
| Basic testing | Advanced testing tools |

**Example:**
```bash
# Create Nx workspace
npx create-nx-workspace@latest myworkspace
cd myworkspace

# Generate Angular application
nx g @nrwl/angular:app myapp

# Generate library
nx g @nrwl/angular:lib shared-ui
nx g @nrwl/angular:lib data-access

# Generate components in library
nx g @nrwl/angular:component button --project=shared-ui
nx g @nrwl/angular:service user --project=data-access

# Run applications
nx serve myapp
nx build myapp --prod

# Run tests
nx test myapp
nx test shared-ui

# Lint
nx lint myapp

# Affected commands (only run on changed projects)
nx affected:test
nx affected:build
nx affected:lint
```

#### Q14: How do you structure libraries in Nx workspace?
**Answer:**
Nx promotes a structured approach to library organization with different types of libraries.

**Example:**
```typescript
// workspace structure
apps/
  web-app/
  mobile-app/
  admin-app/
libs/
  shared/
    ui/           # Shared UI components
    data-access/  # Shared services and state
    utils/        # Utility functions
  feature/
    auth/         # Authentication feature
    user/         # User management feature
    dashboard/    # Dashboard feature
  domain/
    user/         # User domain models
    order/        # Order domain models

// Library types in nx.json
{
  "projects": {
    "shared-ui": {
      "tags": ["scope:shared", "type:ui"]
    },
    "feature-auth": {
      "tags": ["scope:auth", "type:feature"]
    },
    "data-access-user": {
      "tags": ["scope:user", "type:data-access"]
    }
  }
}

// Enforcement rules in .eslintrc.json
{
  "rules": {
    "@nrwl/nx/enforce-module-boundaries": [
      "error",
      {
        "depConstraints": [
          {
            "sourceTag": "type:feature",
            "onlyDependOnLibsWithTags": ["type:ui", "type:data-access", "type:util"]
          },
          {
            "sourceTag": "type:ui",
            "onlyDependOnLibsWithTags": ["type:util"]
          },
          {
            "sourceTag": "scope:shared",
            "notDependOnLibsWithTags": ["scope:feature"]
          }
        ]
      }
    ]
  }
}

// Shared UI library example
// libs/shared/ui/src/lib/button/button.component.ts
@Component({
  selector: 'app-button',
  template: `
    <button 
      [class]="buttonClasses" 
      [disabled]="disabled"
      (click)="onClick.emit($event)">
      <ng-content></ng-content>
    </button>
  `
})
export class ButtonComponent {
  @Input() variant: 'primary' | 'secondary' = 'primary';
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() disabled = false;
  @Output() onClick = new EventEmitter<Event>();
  
  get buttonClasses() {
    return `btn btn-${this.variant} btn-${this.size}`;
  }
}

// Public API for library
// libs/shared/ui/src/index.ts
export * from './lib/button/button.component';
export * from './lib/card/card.component';
export * from './lib/shared-ui.module';
```

### Intermediate Level

#### Q15: How do you implement code sharing and dependency management in Nx?
**Answer:**
Nx provides powerful tools for managing dependencies and code sharing across projects.

**Example:**
```typescript
// 1. Dependency Graph
nx dep-graph  // Visual dependency graph

// 2. Library imports (TypeScript paths)
// tsconfig.base.json
{
  "compilerOptions": {
    "paths": {
      "@myworkspace/shared/ui": ["libs/shared/ui/src/index.ts"],
      "@myworkspace/shared/data-access": ["libs/shared/data-access/src/index.ts"],
      "@myworkspace/feature/auth": ["libs/feature/auth/src/index.ts"]
    }
  }
}

// Usage in app
import { ButtonComponent } from '@myworkspace/shared/ui';
import { AuthService } from '@myworkspace/feature/auth';

// 3. Publishable libraries
// libs/shared/ui/project.json
{
  "name": "shared-ui",
  "targets": {
    "build": {
      "executor": "@nrwl/angular:package",
      "outputs": ["dist/libs/shared/ui"],
      "options": {
        "project": "libs/shared/ui/ng-package.json"
      }
    },
    "publish": {
      "executor": "nx:run-commands",
      "options": {
        "command": "npm publish dist/libs/shared/ui"
      }
    }
  }
}

// 4. Workspace generators
// tools/generators/feature/index.ts
import { Tree, formatFiles, installPackagesTask, generateFiles, joinPathFragments } from '@nrwl/devkit';
import { libraryGenerator } from '@nrwl/angular/generators';

export default async function (tree: Tree, options: any) {
  // Generate base library
  await libraryGenerator(tree, {
    name: options.name,
    directory: 'feature',
    tags: `scope:${options.name},type:feature`
  });
  
  // Add custom files
  generateFiles(
    tree,
    joinPathFragments(__dirname, 'files'),
    `libs/feature/${options.name}`,
    options
  );
  
  await formatFiles(tree);
  return () => {
    installPackagesTask(tree);
  };
}

// Usage: nx g @myworkspace:feature user

// 5. Affected commands
nx affected:build --base=main --head=HEAD
nx affected:test --parallel --maxParallel=3
nx affected:lint --fix

// 6. Caching
nx build myapp  # First run
nx build myapp  # Cached result (if no changes)

// Cache configuration in nx.json
{
  "tasksRunnerOptions": {
    "default": {
      "runner": "@nrwl/workspace/tasks-runners/default",
      "options": {
        "cacheableOperations": ["build", "test", "lint"],
        "parallel": true,
        "maxParallel": 3
      }
    }
  }
}
```

---

## RxJS

### Beginner Level

#### Q16: What is RxJS and why is it important in Angular?
**Answer:**
RxJS (Reactive Extensions for JavaScript) is a library for reactive programming using Observables. It's essential in Angular for handling asynchronous operations.

**Key Concepts:**
- **Observable**: Stream of data over time
- **Observer**: Consumes the Observable
- **Subscription**: Connection between Observable and Observer
- **Operators**: Functions to transform, filter, combine streams

**Example:**
```typescript
import { Observable, of, from, interval } from 'rxjs';
import { map, filter, take } from 'rxjs/operators';

// 1. Creating Observables
const obs1$ = of(1, 2, 3, 4, 5);
const obs2$ = from([1, 2, 3, 4, 5]);
const obs3$ = from(fetch('/api/users'));
const timer$ = interval(1000);

// 2. Basic subscription
obs1$.subscribe({
  next: value => console.log('Value:', value),
  error: error => console.error('Error:', error),
  complete: () => console.log('Complete')
});

// 3. Using operators
const processedData$ = obs1$.pipe(
  filter(x => x % 2 === 0),  // Only even numbers
  map(x => x * 2),           // Multiply by 2
  take(2)                    // Take only first 2
);

processedData$.subscribe(value => console.log('Processed:', value));
// Output: Processed: 4, Processed: 8

// 4. Angular service example
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private http: HttpClient) {}
  
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>('/api/users').pipe(
      map(users => users.filter(user => user.active)),
      catchError(error => {
        console.error('Error fetching users:', error);
        return of([]); // Return empty array on error
      })
    );
  }
}

// 5. Component usage
@Component({
  selector: 'app-user-list',
  template: `
    <div *ngFor="let user of users$ | async">
      {{user.name}}
    </div>
  `
})
export class UserListComponent implements OnInit, OnDestroy {
  users$: Observable<User[]>;
  private subscription = new Subscription();
  
  constructor(private userService: UserService) {}
  
  ngOnInit() {
    this.users$ = this.userService.getUsers();
    
    // Manual subscription (remember to unsubscribe)
    this.subscription.add(
      this.users$.subscribe(users => {
        console.log('Users loaded:', users.length);
      })
    );
  }
  
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
```

#### Q17: Explain Observable vs Promise in Angular.
**Answer:**
| Observable | Promise |
|------------|---------|
| Lazy (cold) | Eager (hot) |
| Multiple values | Single value |
| Cancellable | Not cancellable |
| Rich operators | Limited methods |
| Synchronous/Asynchronous | Only asynchronous |

**Example:**
```typescript
// Promise example
function fetchUserPromise(id: number): Promise<User> {
  return fetch(`/api/users/${id}`)
    .then(response => response.json())
    .then(user => {
      console.log('User fetched via Promise');
      return user;
    });
}

// Observable example
function fetchUserObservable(id: number): Observable<User> {
  return this.http.get<User>(`/api/users/${id}`).pipe(
    tap(user => console.log('User fetched via Observable')),
    retry(3), // Retry on error
    timeout(5000) // Timeout after 5 seconds
  );
}

// Usage comparison
// Promise - executes immediately
const userPromise = fetchUserPromise(1);
console.log('Promise created'); // User will be fetched

// Observable - lazy, executes only when subscribed
const user$ = fetchUserObservable(1);
console.log('Observable created'); // No HTTP request yet

user$.subscribe(user => console.log(user)); // Now HTTP request is made

// Cancellation
const subscription = user$.subscribe(user => console.log(user));
setTimeout(() => {
  subscription.unsubscribe(); // Cancel the request
}, 1000);

// Multiple values
const clicks$ = fromEvent(document, 'click');
clicks$.subscribe(event => console.log('Clicked')); // Multiple clicks

// Converting Promise to Observable
const userObservable$ = from(fetchUserPromise(1));
```

### Intermediate Level

#### Q18: Explain RxJS operators and provide examples of commonly used ones.
**Answer:**
RxJS operators are functions that transform, filter, combine, or manipulate Observable streams.

**Categories:**
1. **Creation**: of, from, interval, fromEvent
2. **Transformation**: map, flatMap, switchMap, mergeMap
3. **Filtering**: filter, take, skip, distinct
4. **Combination**: merge, concat, combineLatest, forkJoin
5. **Error Handling**: catchError, retry, retryWhen
6. **Utility**: tap, delay, timeout

**Example:**
```typescript
import { 
  of, from, interval, fromEvent, combineLatest, forkJoin, EMPTY, throwError 
} from 'rxjs';
import { 
  map, filter, take, skip, distinctUntilChanged, debounceTime,
  switchMap, mergeMap, concatMap, exhaustMap,
  catchError, retry, retryWhen, delay,
  tap, shareReplay, startWith
} from 'rxjs/operators';

// 1. Transformation Operators
const numbers$ = of(1, 2, 3, 4, 5);

// map - transform each value
const doubled$ = numbers$.pipe(
  map(x => x * 2)
);

// switchMap - switch to new Observable
const searchTerm$ = fromEvent(searchInput, 'input').pipe(
  map(event => event.target.value),
  debounceTime(300),
  distinctUntilChanged(),
  switchMap(term => this.searchService.search(term)) // Cancel previous search
);

// mergeMap - merge multiple Observables
const userIds$ = of(1, 2, 3);
const users$ = userIds$.pipe(
  mergeMap(id => this.userService.getUser(id)) // Parallel requests
);

// concatMap - sequential execution
const sequentialUsers$ = userIds$.pipe(
  concatMap(id => this.userService.getUser(id)) // One after another
);

// exhaustMap - ignore new values while processing
const saveButton$ = fromEvent(saveButton, 'click').pipe(
  exhaustMap(() => this.saveData()) // Ignore clicks while saving
);

// 2. Filtering Operators
const filteredNumbers$ = numbers$.pipe(
  filter(x => x % 2 === 0), // Only even numbers
  take(2),                  // Take first 2
  skip(1)                   // Skip first 1
);

// 3. Combination Operators
const user$ = this.userService.getCurrentUser();
const preferences$ = this.preferencesService.getPreferences();

// combineLatest - emit when any Observable emits
const userWithPreferences$ = combineLatest([user$, preferences$]).pipe(
  map(([user, prefs]) => ({ ...user, preferences: prefs }))
);

// forkJoin - wait for all to complete (like Promise.all)
const allData$ = forkJoin({
  users: this.userService.getUsers(),
  products: this.productService.getProducts(),
  orders: this.orderService.getOrders()
});

// 4. Error Handling
const robustApiCall$ = this.http.get('/api/data').pipe(
  retry(3), // Retry 3 times
  catchError(error => {
    console.error('API call failed:', error);
    return of([]); // Fallback value
  })
);

// Advanced error handling
const retryWithDelay$ = this.http.get('/api/data').pipe(
  retryWhen(errors => 
    errors.pipe(
      delay(1000), // Wait 1 second before retry
      take(3)      // Maximum 3 retries
    )
  ),
  catchError(() => EMPTY) // Complete silently on final error
);

// 5. Utility Operators
const debuggedStream$ = numbers$.pipe(
  tap(x => console.log('Before filter:', x)),
  filter(x => x > 2),
  tap(x => console.log('After filter:', x))
);

// shareReplay - share and replay values
const sharedData$ = this.http.get('/api/expensive-data').pipe(
  shareReplay(1) // Cache last value for multiple subscribers
);

// 6. Complex example - Search with debounce, cancel, and error handling
@Component({
  selector: 'app-search',
  template: `
    <input #searchInput placeholder="Search...">
    <div *ngFor="let result of searchResults$ | async">
      {{result.name}}
    </div>
    <div *ngIf="loading$ | async">Loading...</div>
    <div *ngIf="error$ | async">Error occurred</div>
  `
})
export class SearchComponent implements OnInit {
  searchResults$: Observable<SearchResult[]>;
  loading$: Observable<boolean>;
  error$: Observable<boolean>;
  
  @ViewChild('searchInput', { static: true }) searchInput: ElementRef;
  
  ngOnInit() {
    const search$ = fromEvent(this.searchInput.nativeElement, 'input').pipe(
      map((event: any) => event.target.value),
      debounceTime(300),
      distinctUntilChanged(),
      filter(term => term.length > 2)
    );
    
    this.searchResults$ = search$.pipe(
      switchMap(term => 
        this.searchService.search(term).pipe(
          startWith([]), // Start with empty results
          catchError(() => of([])) // Return empty on error
        )
      )
    );
    
    this.loading$ = search$.pipe(
      switchMap(term => 
        this.searchService.search(term).pipe(
          map(() => false),
          startWith(true),
          catchError(() => of(false))
        )
      )
    );
    
    this.error$ = search$.pipe(
      switchMap(term => 
        this.searchService.search(term).pipe(
          map(() => false),
          catchError(() => of(true))
        )
      ),
      startWith(false)
    );
  }
}
```

### Expert Level

#### Q19: Explain RxJS Subjects and their types with examples.
**Answer:**
Subjects are special types of Observables that can multicast to multiple Observers and can also act as Observers themselves.

**Types:**
1. **Subject**: Basic multicast Observable
2. **BehaviorSubject**: Has current value and emits to new subscribers
3. **ReplaySubject**: Replays specified number of values to new subscribers
4. **AsyncSubject**: Emits only the last value when complete

**Example:**
```typescript
import { Subject, BehaviorSubject, ReplaySubject, AsyncSubject } from 'rxjs';
import { Injectable } from '@angular/core';

// 1. Basic Subject
export class EventBusService {
  private eventSubject = new Subject<any>();
  
  events$ = this.eventSubject.asObservable();
  
  emit(event: any) {
    this.eventSubject.next(event);
  }
}

// Usage
const eventBus = new EventBusService();

// Subscriber 1
eventBus.events$.subscribe(event => console.log('Sub1:', event));

eventBus.emit('Event 1'); // Sub1: Event 1

// Subscriber 2 (won't receive previous events)
eventBus.events$.subscribe(event => console.log('Sub2:', event));

eventBus.emit('Event 2'); // Sub1: Event 2, Sub2: Event 2

// 2. BehaviorSubject (has initial value)
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }
  
  login(user: User) {
    this.currentUserSubject.next(user);
  }
  
  logout() {
    this.currentUserSubject.next(null);
  }
}

// Usage
const authService = new AuthService();

// This subscriber immediately gets the current value (null)
authService.currentUser$.subscribe(user => console.log('Current user:', user));

authService.login({ id: 1, name: 'John' }); // Emits to all subscribers

// New subscriber immediately gets current value
authService.currentUser$.subscribe(user => console.log('New sub:', user));

// 3. ReplaySubject (replays N values)
export class NotificationService {
  private notificationSubject = new ReplaySubject<string>(3); // Keep last 3
  
  notifications$ = this.notificationSubject.asObservable();
  
  addNotification(message: string) {
    this.notificationSubject.next(message);
  }
}

// Usage
const notifications = new NotificationService();

notifications.addNotification('Notification 1');
notifications.addNotification('Notification 2');
notifications.addNotification('Notification 3');
notifications.addNotification('Notification 4');

// New subscriber gets last 3 notifications
notifications.notifications$.subscribe(msg => console.log('Replay:', msg));
// Output: Replay: Notification 2, Replay: Notification 3, Replay: Notification 4

// 4. AsyncSubject (only last value when complete)
const asyncSubject = new AsyncSubject<number>();

asyncSubject.subscribe(value => console.log('Async:', value));

asyncSubject.next(1);
asyncSubject.next(2);
asyncSubject.next(3);
// No output yet

asyncSubject.complete(); // Output: Async: 3 (only last value)

// 5. State Management with BehaviorSubject
interface AppState {
  loading: boolean;
  user: User | null;
  theme: 'light' | 'dark';
}

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private stateSubject = new BehaviorSubject<AppState>({
    loading: false,
    user: null,
    theme: 'light'
  });
  
  state$ = this.stateSubject.asObservable();
  
  // Selectors
  loading$ = this.state$.pipe(map(state => state.loading));
  user$ = this.state$.pipe(map(state => state.user));
  theme$ = this.state$.pipe(map(state => state.theme));
  
  // Actions
  setLoading(loading: boolean) {
    this.updateState({ loading });
  }
  
  setUser(user: User | null) {
    this.updateState({ user });
  }
  
  setTheme(theme: 'light' | 'dark') {
    this.updateState({ theme });
  }
  
  private updateState(partial: Partial<AppState>) {
    const currentState = this.stateSubject.value;
    this.stateSubject.next({ ...currentState, ...partial });
  }
}

// 6. Custom Subject for Complex Scenarios
export class WebSocketSubject extends Subject<any> {
  private socket: WebSocket;
  
  constructor(private url: string) {
    super();
    this.connect();
  }
  
  private connect() {
    this.socket = new WebSocket(this.url);
    
    this.socket.onmessage = (event) => {
      this.next(JSON.parse(event.data));
    };
    
    this.socket.onerror = (error) => {
      this.error(error);
    };
    
    this.socket.onclose = () => {
      this.complete();
    };
  }
  
  send(data: any) {
    if (this.socket.readyState === WebSocket.OPEN) {
      this.socket.send(JSON.stringify(data));
    }
  }
  
  override unsubscribe() {
    this.socket.close();
    super.unsubscribe();
  }
}

// Usage
const wsSubject = new WebSocketSubject('ws://localhost:8080');
wsSubject.subscribe(message => console.log('WS Message:', message));
wsSubject.send({ type: 'ping' });
```

#### Q20: How do you handle memory leaks and subscription management in Angular with RxJS?
**Answer:**
Memory leaks in Angular typically occur when Observables are not properly unsubscribed, causing components to remain in memory.

**Example:**
```typescript
// 1. Manual Subscription Management
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-manual-unsubscribe',
  template: '<p>Manual Unsubscribe Example</p>'
})
export class ManualUnsubscribeComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  
  constructor(private dataService: DataService) {}
  
  ngOnInit() {
    // Add multiple subscriptions
    this.subscription.add(
      this.dataService.getData().subscribe(data => {
        console.log('Data:', data);
      })
    );
    
    this.subscription.add(
      interval(1000).subscribe(tick => {
        console.log('Timer:', tick);
      })
    );
  }
  
  ngOnDestroy() {
    this.subscription.unsubscribe(); // Unsubscribes all added subscriptions
  }
}

// 2. takeUntil Pattern (Recommended)
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-take-until',
  template: '<p>TakeUntil Pattern</p>'
})
export class TakeUntilComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  constructor(private dataService: DataService) {}
  
  ngOnInit() {
    this.dataService.getData().pipe(
      takeUntil(this.destroy$)
    ).subscribe(data => {
      console.log('Data:', data);
    });
    
    interval(1000).pipe(
      takeUntil(this.destroy$)
    ).subscribe(tick => {
      console.log('Timer:', tick);
    });
  }
  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

// 3. Custom Directive for Automatic Unsubscription
import { Directive, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

@Directive()
export abstract class DestroyableComponent implements OnDestroy {
  protected destroy$ = new Subject<void>();
  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

// Usage
@Component({
  selector: 'app-auto-destroy',
  template: '<p>Auto Destroy</p>'
})
export class AutoDestroyComponent extends DestroyableComponent implements OnInit {
  
  ngOnInit() {
    this.dataService.getData().pipe(
      takeUntil(this.destroy$)
    ).subscribe(data => {
      console.log('Data:', data);
    });
  }
}

// 4. Async Pipe (No Manual Unsubscription Needed)
@Component({
  selector: 'app-async-pipe',
  template: `
    <div *ngFor="let item of items$ | async">
      {{item.name}}
    </div>
    <p>Count: {{count$ | async}}</p>
  `
})
export class AsyncPipeComponent implements OnInit {
  items$: Observable<Item[]>;
  count$: Observable<number>;
  
  ngOnInit() {
    this.items$ = this.dataService.getItems();
    this.count$ = this.items$.pipe(
      map(items => items.length)
    );
  }
  // No ngOnDestroy needed - async pipe handles unsubscription
}

// 5. Memory Leak Detection and Prevention
@Injectable({
  providedIn: 'root'
})
export class MemoryLeakDetector {
  private subscriptions = new Map<string, number>();
  
  trackSubscription(componentName: string) {
    const count = this.subscriptions.get(componentName) || 0;
    this.subscriptions.set(componentName, count + 1);
    console.log(`${componentName} subscriptions: ${count + 1}`);
  }
  
  trackUnsubscription(componentName: string) {
    const count = this.subscriptions.get(componentName) || 0;
    if (count > 0) {
      this.subscriptions.set(componentName, count - 1);
      console.log(`${componentName} subscriptions: ${count - 1}`);
    }
  }
  
  getActiveSubscriptions() {
    return this.subscriptions;
  }
}

// 6. shareReplay for Preventing Multiple HTTP Calls
@Injectable({
  providedIn: 'root'
})
export class OptimizedDataService {
  private cache = new Map<string, Observable<any>>();
  
  getData(id: string): Observable<any> {
    if (!this.cache.has(id)) {
      const data$ = this.http.get(`/api/data/${id}`).pipe(
        shareReplay(1), // Cache the result
        finalize(() => this.cache.delete(id)) // Clean up on complete
      );
      this.cache.set(id, data$);
    }
    return this.cache.get(id)!;
  }
}

// 7. Custom Operators for Common Patterns
export function untilDestroyed(component: any) {
  return <T>(source: Observable<T>) => {
    const originalDestroy = component.ngOnDestroy;
    const destroy$ = new Subject();
    
    component.ngOnDestroy = function() {
      destroy$.next(true);
      destroy$.complete();
      if (originalDestroy) {
        originalDestroy.apply(this, arguments);
      }
    };
    
    return source.pipe(takeUntil(destroy$));
  };
}

// Usage
@Component({
  selector: 'app-custom-operator',
  template: '<p>Custom Operator</p>'
})
export class CustomOperatorComponent implements OnInit {
  
  ngOnInit() {
    this.dataService.getData().pipe(
      untilDestroyed(this)
    ).subscribe(data => {
      console.log('Data:', data);
    });
  }
  
  ngOnDestroy() {
    // Auto-handled by custom operator
  }
}

// 8. Best Practices Summary
export class BestPracticesComponent implements OnInit, OnDestroy {
  // ✅ Good: Use async pipe when possible
  data$ = this.dataService.getData();
  
  // ✅ Good: Use takeUntil pattern
  private destroy$ = new Subject<void>();
  
  // ❌ Bad: No unsubscription
  ngOnInit() {
    // Don't do this:
    // this.dataService.getData().subscribe(data => {});
    
    // Do this instead:
    this.dataService.getData().pipe(
      takeUntil(this.destroy$)
    ).subscribe(data => {
      console.log(data);
    });
  }
  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

---

## State Management (Redux, NgRx, NGXS)

### Beginner Level

#### Q21: What is state management and why is it needed in Angular applications?
**Answer:**
State management is a pattern for managing application state in a predictable way. It's needed for:

- **Sharing data** between unrelated components
- **Maintaining consistency** across the application
- **Debugging** and tracking state changes
- **Caching** data and reducing API calls
- **Undo/Redo** functionality
- **Real-time synchronization**

**Example:**
```typescript
// Without state management - prop drilling
@Component({
  selector: 'app-parent',
  template: `
    <app-child [user]="user" [cart]="cart" (updateCart)="updateCart($event)"></app-child>
  `
})
export class ParentComponent {
  user = { id: 1, name: 'John' };
  cart = { items: [], total: 0 };
  
  updateCart(cart: any) {
    this.cart = cart;
  }
}

@Component({
  selector: 'app-child',
  template: `
    <app-grandchild [user]="user" [cart]="cart" (updateCart)="updateCart($event)"></app-grandchild>
  `
})
export class ChildComponent {
  @Input() user: any;
  @Input() cart: any;
  @Output() updateCart = new EventEmitter();
}

// With state management - direct access
@Component({
  selector: 'app-any-component',
  template: `
    <div>User: {{(user$ | async)?.name}}</div>
    <div>Cart Total: {{(cart$ | async)?.total}}</div>
  `
})
export class AnyComponent {
  user$ = this.store.select('user');
  cart$ = this.store.select('cart');
  
  constructor(private store: Store) {}
  
  addToCart(item: any) {
    this.store.dispatch(addToCart({ item }));
  }
}
```

#### Q22: What is NgRx and what are its core concepts?
**Answer:**
NgRx is a reactive state management library for Angular based on Redux pattern.

**Core Concepts:**
1. **Store**: Single source of truth for application state
2. **Actions**: Events that describe what happened
3. **Reducers**: Pure functions that handle state changes
4. **Selectors**: Functions to select pieces of state
5. **Effects**: Handle side effects like API calls

**Example:**
```typescript
// 1. State Interface
export interface AppState {
  users: User[];
  loading: boolean;
  error: string | null;
}

// 2. Actions
import { createAction, props } from '@ngrx/store';

export const loadUsers = createAction('[User] Load Users');
export const loadUsersSuccess = createAction(
  '[User] Load Users Success',
  props<{ users: User[] }>()
);
export const loadUsersFailure = createAction(
  '[User] Load Users Failure',
  props<{ error: string }>()
);

// 3. Reducer
import { createReducer, on } from '@ngrx/store';

const initialState: AppState = {
  users: [],
  loading: false,
  error: null
};

export const userReducer = createReducer(
  initialState,
  on(loadUsers, state => ({
    ...state,
    loading: true,
    error: null
  })),
  on(loadUsersSuccess, (state, { users }) => ({
    ...state,
    users,
    loading: false
  })),
  on(loadUsersFailure, (state, { error }) => ({
    ...state,
    error,
    loading: false
  }))
);

// 4. Selectors
import { createSelector, createFeatureSelector } from '@ngrx/store';

export const selectUserState = createFeatureSelector<AppState>('users');

export const selectUsers = createSelector(
  selectUserState,
  state => state.users
);

export const selectLoading = createSelector(
  selectUserState,
  state => state.loading
);

export const selectActiveUsers = createSelector(
  selectUsers,
  users => users.filter(user => user.active)
);

// 5. Effects
import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';

@Injectable()
export class UserEffects {
  
  loadUsers$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadUsers),
      mergeMap(() =>
        this.userService.getUsers().pipe(
          map(users => loadUsersSuccess({ users })),
          catchError(error => of(loadUsersFailure({ error: error.message })))
        )
      )
    )
  );
  
  constructor(
    private actions$: Actions,
    private userService: UserService
  ) {}
}

// 6. Component Usage
@Component({
  selector: 'app-user-list',
  template: `
    <div *ngIf="loading$ | async">Loading...</div>
    <div *ngIf="error$ | async as error">Error: {{error}}</div>
    <div *ngFor="let user of users$ | async">
      {{user.name}}
    </div>
  `
})
export class UserListComponent implements OnInit {
  users$ = this.store.select(selectUsers);
  loading$ = this.store.select(selectLoading);
  error$ = this.store.select(state => state.users.error);
  
  constructor(private store: Store) {}
  
  ngOnInit() {
    this.store.dispatch(loadUsers());
  }
}
```

### Intermediate Level

#### Q23: Compare NgRx vs NGXS and provide examples.
**Answer:**
| NgRx | NGXS |
|------|------|
| Redux-inspired | MobX/Vuex inspired |
| More boilerplate | Less boilerplate |
| Actions + Reducers | Actions + State classes |
| Immutable updates | Immutable via Immer |
| Effects for side effects | Actions for side effects |

**NgRx Example:**
```typescript
// NgRx - More verbose but explicit
// actions.ts
export const increment = createAction('[Counter] Increment');
export const decrement = createAction('[Counter] Decrement');
export const reset = createAction('[Counter] Reset');

// reducer.ts
export const counterReducer = createReducer(
  0,
  on(increment, state => state + 1),
  on(decrement, state => state - 1),
  on(reset, () => 0)
);

// effects.ts
@Injectable()
export class CounterEffects {
  logActions$ = createEffect(() =>
    this.actions$.pipe(
      tap(action => console.log('Action:', action)),
      map(() => ({ type: 'NO_ACTION' }))
    ), { dispatch: false }
  );
}
```

**NGXS Example:**
```typescript
// NGXS - Less boilerplate, more concise
// actions.ts
export class Increment {
  static readonly type = '[Counter] Increment';
}

export class Decrement {
  static readonly type = '[Counter] Decrement';
}

export class Reset {
  static readonly type = '[Counter] Reset';
}

// state.ts
export interface CounterStateModel {
  count: number;
}

@State<CounterStateModel>({
  name: 'counter',
  defaults: {
    count: 0
  }
})
@Injectable()
export class CounterState {
  
  @Selector()
  static getCount(state: CounterStateModel) {
    return state.count;
  }
  
  @Action(Increment)
  increment(ctx: StateContext<CounterStateModel>) {
    const state = ctx.getState();
    ctx.setState({
      count: state.count + 1
    });
  }
  
  @Action(Decrement)
  decrement(ctx: StateContext<CounterStateModel>) {
    ctx.setState(
      patch({
        count: state => state - 1
      })
    );
  }
  
  @Action(Reset)
  reset(ctx: StateContext<CounterStateModel>) {
    ctx.setState({
      count: 0
    });
  }
}

// Component usage (similar for both)
@Component({
  template: `
    <div>Count: {{count$ | async}}</div>
    <button (click)="increment()">+</button>
    <button (click)="decrement()">-</button>
    <button (click)="reset()">Reset</button>
  `
})
export class CounterComponent {
  // NgRx
  count$ = this.store.select(selectCount);
  
  // NGXS
  // count$ = this.store.select(CounterState.getCount);
  
  constructor(private store: Store) {}
  
  increment() {
    this.store.dispatch(new Increment()); // Both
  }
  
  decrement() {
    this.store.dispatch(new Decrement()); // Both
  }
  
  reset() {
    this.store.dispatch(new Reset()); // Both
  }
}
```

#### Q24: How do you handle complex state updates and side effects in NgRx?
**Answer:**
Complex state management involves nested state updates, async operations, and error handling.

**Example:**
```typescript
// Complex State Structure
export interface AppState {
  users: {
    entities: { [id: number]: User };
    selectedUserId: number | null;
    loading: boolean;
    error: string | null;
  };
  posts: {
    entities: { [id: number]: Post };
    userPosts: { [userId: number]: number[] };
    loading: boolean;
  };
  ui: {
    sidebarOpen: boolean;
    theme: 'light' | 'dark';
    notifications: Notification[];
  };
}

// Entity Adapter for Normalized State
import { createEntityAdapter, EntityAdapter, EntityState } from '@ngrx/entity';

export interface User {
  id: number;
  name: string;
  email: string;
  active: boolean;
}

export interface UserState extends EntityState<User> {
  selectedUserId: number | null;
  loading: boolean;
  error: string | null;
}

export const userAdapter: EntityAdapter<User> = createEntityAdapter<User>();

const initialState: UserState = userAdapter.getInitialState({
  selectedUserId: null,
  loading: false,
  error: null
});

// Complex Actions
export const loadUserWithPosts = createAction(
  '[User] Load User With Posts',
  props<{ userId: number }>()
);

export const updateUserProfile = createAction(
  '[User] Update User Profile',
  props<{ userId: number; updates: Partial<User> }>()
);

export const batchUpdateUsers = createAction(
  '[User] Batch Update Users',
  props<{ updates: { id: number; changes: Partial<User> }[] }>()
);

// Complex Reducer with Entity Adapter
export const userReducer = createReducer(
  initialState,
  on(loadUsersSuccess, (state, { users }) =>
    userAdapter.setAll(users, {
      ...state,
      loading: false
    })
  ),
  on(updateUserProfile, (state, { userId, updates }) =>
    userAdapter.updateOne(
      { id: userId, changes: updates },
      state
    )
  ),
  on(batchUpdateUsers, (state, { updates }) =>
    userAdapter.updateMany(updates, state)
  ),
  on(selectUser, (state, { userId }) => ({
    ...state,
    selectedUserId: userId
  }))
);

// Entity Selectors
const { selectIds, selectEntities, selectAll, selectTotal } = userAdapter.getSelectors();

export const selectUserState = createFeatureSelector<UserState>('users');

export const selectAllUsers = createSelector(selectUserState, selectAll);
export const selectUserEntities = createSelector(selectUserState, selectEntities);
export const selectSelectedUserId = createSelector(selectUserState, state => state.selectedUserId);

export const selectSelectedUser = createSelector(
  selectUserEntities,
  selectSelectedUserId,
  (entities, selectedId) => selectedId ? entities[selectedId] : null
);

// Complex Effects with Multiple API Calls
@Injectable()
export class UserEffects {
  
  // Load user and their posts
  loadUserWithPosts$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadUserWithPosts),
      switchMap(({ userId }) =>
        forkJoin({
          user: this.userService.getUser(userId),
          posts: this.postService.getUserPosts(userId)
        }).pipe(
          map(({ user, posts }) => {
            // Dispatch multiple actions
            this.store.dispatch(loadUsersSuccess({ users: [user] }));
            this.store.dispatch(loadPostsSuccess({ posts }));
            return selectUser({ userId });
          }),
          catchError(error => of(loadUsersFailure({ error: error.message })))
        )
      )
    )
  );
  
  // Auto-save user changes
  autoSaveUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(updateUserProfile),
      debounceTime(1000), // Wait 1 second after last change
      switchMap(({ userId, updates }) =>
        this.userService.updateUser(userId, updates).pipe(
          map(user => updateUserSuccess({ user })),
          catchError(error => of(updateUserFailure({ error: error.message })))
        )
      )
    )
  );
  
  // Optimistic updates with rollback
  optimisticUpdate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(updateUserProfile),
      concatMap(action =>
        of(action).pipe(
          // Optimistic update
          tap(() => console.log('Optimistic update applied')),
          // Make API call
          switchMap(() =>
            this.userService.updateUser(action.userId, action.updates).pipe(
              map(user => updateUserSuccess({ user })),
              catchError(error => {
                // Rollback on error
                this.store.dispatch(rollbackUserUpdate({ userId: action.userId }));
                return of(updateUserFailure({ error: error.message }));
              })
            )
          )
        )
      )
    )
  );
  
  constructor(
    private actions$: Actions,
    private store: Store,
    private userService: UserService,
    private postService: PostService
  ) {}
}

// Meta-reducers for logging and state persistence
export function logger(reducer: ActionReducer<any>): ActionReducer<any> {
  return (state, action) => {
    console.log('State:', state);
    console.log('Action:', action);
    const result = reducer(state, action);
    console.log('New State:', result);
    return result;
  };
}

export function persistState(reducer: ActionReducer<any>): ActionReducer<any> {
  return (state, action) => {
    const result = reducer(state, action);
    localStorage.setItem('app-state', JSON.stringify(result));
    return result;
  };
}

// Store Configuration
StoreModule.forRoot(reducers, {
  metaReducers: [logger, persistState],
  runtimeChecks: {
    strictStateImmutability: true,
    strictActionImmutability: true,
    strictActionSerializability: true,
    strictStateSerializability: true
  }
})
```

### Expert Level

#### Q25: How do you implement advanced NgRx patterns like feature state composition and dynamic reducers?
**Answer:**
Advanced NgRx patterns involve modular state management, dynamic feature loading, and complex state compositions.

**Example:**
```typescript
// 1. Feature State Composition
// shared/state/index.ts
export interface SharedState {
  auth: AuthState;
  router: RouterReducerState;
  ui: UIState;
}

// feature/user/state/index.ts
export interface UserFeatureState extends SharedState {
  users: UserState;
  userProfiles: UserProfileState;
}

// 2. Feature Module with State
@NgModule({
  imports: [
    StoreModule.forFeature('users', userReducer),
    EffectsModule.forFeature([UserEffects, UserProfileEffects])
  ]
})
export class UserStateModule {}

// 3. Dynamic Reducer Registration
@Injectable()
export class DynamicStoreService {
  private dynamicReducers: { [key: string]: ActionReducer<any> } = {};
  
  constructor(private store: Store) {}
  
  addReducer(key: string, reducer: ActionReducer<any>) {
    this.dynamicReducers[key] = reducer;
    this.store.addReducer(key, reducer);
  }
  
  removeReducer(key: string) {
    delete this.dynamicReducers[key];
    this.store.removeReducer(key);
  }
}

// 4. Higher-Order Reducers
export function createGenericListReducer<T>(
  entityName: string,
  adapter: EntityAdapter<T>
) {
  const initialState = adapter.getInitialState({
    loading: false,
    error: null
  });
  
  return createReducer(
    initialState,
    on(createAction(`[${entityName}] Load`), state => ({
      ...state,
      loading: true,
      error: null
    })),
    on(createAction(`[${entityName}] Load Success`, props<{ items: T[] }>()), 
      (state, { items }) => adapter.setAll(items, {
        ...state,
        loading: false
      })
    ),
    on(createAction(`[${entityName}] Load Failure`, props<{ error: string }>()), 
      (state, { error }) => ({
        ...state,
        loading: false,
        error
      })
    )
  );
}

// Usage
export const userReducer = createGenericListReducer('User', userAdapter);
export const postReducer = createGenericListReducer('Post', postAdapter);

// 5. State Composition with Selectors
export const selectUserFeatureState = createFeatureSelector<UserFeatureState>('userFeature');

// Composed selectors across features
export const selectUserWithPosts = createSelector(
  selectUserEntities,
  selectPostEntities,
  selectSelectedUserId,
  (users, posts, selectedUserId) => {
    if (!selectedUserId || !users[selectedUserId]) return null;
    
    const user = users[selectedUserId];
    const userPosts = Object.values(posts).filter(post => post.userId === selectedUserId);
    
    return {
      ...user,
      posts: userPosts,
      postCount: userPosts.length
    };
  }
);

// 6. Advanced Effects Patterns
@Injectable()
export class AdvancedUserEffects {
  
  // Race condition handling
  loadUserWithRace$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadUser),
      switchMap(({ userId }) => // switchMap cancels previous requests
        this.userService.getUser(userId).pipe(
          map(user => loadUserSuccess({ user })),
          catchError(error => of(loadUserFailure({ error: error.message })))
        )
      )
    )
  );
  
  // Queue management
  updateUserQueue$ = createEffect(() =>
    this.actions$.pipe(
      ofType(updateUser),
      concatMap(({ userId, updates }) => // concatMap queues requests
        this.userService.updateUser(userId, updates).pipe(
          map(user => updateUserSuccess({ user })),
          catchError(error => of(updateUserFailure({ error: error.message })))
        )
      )
    )
  );
  
  // Complex async workflows
  userRegistrationFlow$ = createEffect(() =>
    this.actions$.pipe(
      ofType(registerUser),
      exhaustMap(({ userData }) => // exhaustMap ignores new requests while processing
        this.authService.register(userData).pipe(
          mergeMap(user => [
            registerUserSuccess({ user }),
            sendWelcomeEmail({ email: user.email }),
            createUserProfile({ userId: user.id }),
            navigateToWelcome()
          ]),
          catchError(error => of(registerUserFailure({ error: error.message })))
        )
      )
    )
  );
  
  // WebSocket integration
  realtimeUpdates$ = createEffect(() =>
    this.actions$.pipe(
      ofType(connectWebSocket),
      switchMap(() =>
        this.websocketService.connect().pipe(
          map(message => processWebSocketMessage({ message })),
          catchError(error => of(websocketError({ error: error.message })))
        )
      )
    )
  );
  
  constructor(
    private actions$: Actions,
    private userService: UserService,
    private authService: AuthService,
    private websocketService: WebSocketService
  ) {}
}

// 7. State Synchronization Across Tabs
@Injectable()
export class StateSyncService {
  constructor(private store: Store) {
    // Listen for state changes in other tabs
    fromEvent(window, 'storage').pipe(
      filter((event: any) => event.key === 'app-state'),
      map(event => JSON.parse(event.newValue))
    ).subscribe(newState => {
      this.store.dispatch(syncStateFromStorage({ state: newState }));
    });
  }
}

// 8. Custom Store Implementation
export class CustomStore<T> extends BehaviorSubject<T> {
  private reducers: { [key: string]: (state: any, action: any) => any } = {};
  
  constructor(initialState: T) {
    super(initialState);
  }
  
  addReducer(key: string, reducer: (state: any, action: any) => any) {
    this.reducers[key] = reducer;
  }
  
  dispatch(action: any) {
    const currentState = this.value;
    const newState = { ...currentState };
    
    Object.keys(this.reducers).forEach(key => {
      newState[key] = this.reducers[key](currentState[key], action);
    });
    
    this.next(newState);
  }
  
  select<K>(selector: (state: T) => K): Observable<K> {
    return this.pipe(
      map(selector),
      distinctUntilChanged()
    );
  }
}
```

---

## Apollo Client & GraphQL

### Beginner Level

#### Q26: What is GraphQL and how does it differ from REST API?
**Answer:**
GraphQL is a query language and runtime for APIs that allows clients to request exactly the data they need.

**Key Differences:**
| REST | GraphQL |
|------|---------|
| Multiple endpoints | Single endpoint |
| Fixed data structure | Flexible queries |
| Over/under-fetching | Exact data requested |
| Multiple round trips | Single request |
| HTTP verbs (GET, POST) | Query, Mutation, Subscription |

**Example:**
```typescript
// REST API - Multiple requests
// GET /api/users/1
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "address": { ... },  // Unused data
  "preferences": { ... } // Unused data
}

// GET /api/users/1/posts
[
  { "id": 101, "title": "Post 1", "content": "...", "author": { ... } },
  // ...
]

// GraphQL - Single request, exact data
query GetUserWithPosts($userId: ID!) {
  user(id: $userId) {
    id
    name
    posts {
      id
      title
      createdAt
    }
  }
}

// Response - Only requested fields
{
  "data": {
    "user": {
      "id": "1",
      "name": "John Doe",
      "posts": [
        {
          "id": "101",
          "title": "Post 1",
          "createdAt": "2023-01-01T00:00:00Z"
        }
      ]
    }
  }
}
```

#### Q27: How do you set up Apollo Client in Angular?
**Answer:**
Apollo Client is a comprehensive GraphQL client that provides caching, error handling, and real-time subscriptions.

**Setup Example:**
```typescript
// 1. Install dependencies
npm install apollo-angular @apollo/client graphql

// 2. app.module.ts
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ApolloModule, APOLLO_OPTIONS } from 'apollo-angular';
import { ApolloClientOptions, InMemoryCache } from '@apollo/client/core';
import { HttpLink } from 'apollo-angular/http';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  imports: [
    BrowserModule,
    HttpClientModule,
    ApolloModule
  ],
  providers: [
    {
      provide: APOLLO_OPTIONS,
      useFactory: (httpLink: HttpLink): ApolloClientOptions<any> => ({
        link: httpLink.create({
          uri: 'http://localhost:4000/graphql'
        }),
        cache: new InMemoryCache(),
        defaultOptions: {
          watchQuery: {
            errorPolicy: 'all'
          }
        }
      }),
      deps: [HttpLink]
    }
  ]
})
export class AppModule {}

// 3. GraphQL service
import { Injectable } from '@angular/core';
import { Apollo, gql } from 'apollo-angular';
import { Observable } from 'rxjs';

const GET_USERS = gql`
  query GetUsers {
    users {
      id
      name
      email
      active
    }
  }
`;

const GET_USER = gql`
  query GetUser($id: ID!) {
    user(id: $id) {
      id
      name
      email
      posts {
        id
        title
        content
        createdAt
      }
    }
  }
`;

const CREATE_USER = gql`
  mutation CreateUser($input: CreateUserInput!) {
    createUser(input: $input) {
      id
      name
      email
    }
  }
`;

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private apollo: Apollo) {}
  
  getUsers(): Observable<any> {
    return this.apollo.watchQuery({
      query: GET_USERS
    }).valueChanges;
  }
  
  getUser(id: string): Observable<any> {
    return this.apollo.watchQuery({
      query: GET_USER,
      variables: { id }
    }).valueChanges;
  }
  
  createUser(input: any): Observable<any> {
    return this.apollo.mutate({
      mutation: CREATE_USER,
      variables: { input },
      refetchQueries: [{ query: GET_USERS }] // Refresh users list
    });
  }
}

// 4. Component usage
@Component({
  selector: 'app-user-list',
  template: `
    <div *ngIf="loading">Loading...</div>
    <div *ngIf="error">Error: {{error.message}}</div>
    <div *ngFor="let user of users">
      <p>{{user.name}} - {{user.email}}</p>
    </div>
    <button (click)="addUser()">Add User</button>
  `
})
export class UserListComponent implements OnInit {
  users: any[] = [];
  loading = false;
  error: any = null;
  
  constructor(private userService: UserService) {}
  
  ngOnInit() {
    this.userService.getUsers().subscribe(result => {
      this.users = result.data.users;
      this.loading = result.loading;
      this.error = result.error;
    });
  }
  
  addUser() {
    this.userService.createUser({
      name: 'New User',
      email: 'new@example.com'
    }).subscribe(result => {
      console.log('User created:', result.data.createUser);
    });
  }
}
```

### Intermediate Level

#### Q28: How do you handle caching and state management with Apollo Client?
**Answer:**
Apollo Client provides powerful caching mechanisms and can serve as a state management solution.

**Example:**
```typescript
// 1. Cache Configuration
import { InMemoryCache } from '@apollo/client/core';

const cache = new InMemoryCache({
  typePolicies: {
    User: {
      fields: {
        posts: {
          merge(existing = [], incoming) {
            return [...existing, ...incoming];
          }
        }
      }
    },
    Query: {
      fields: {
        users: {
          merge(existing = [], incoming) {
            return incoming; // Replace existing users
          }
        }
      }
    }
  }
});

// 2. Local State Management
const GET_LOCAL_STATE = gql`
  query GetLocalState {
    isLoggedIn @client
    currentUser @client {
      id
      name
      role
    }
    cart @client {
      items {
        id
        quantity
        product {
          id
          name
          price
        }
      }
      total
    }
  }
`;

// Reactive variables for local state
import { makeVar } from '@apollo/client/core';

export const isLoggedInVar = makeVar(false);
export const currentUserVar = makeVar(null);
export const cartVar = makeVar({
  items: [],
  total: 0
});

// 3. Type policies for local state
const cache = new InMemoryCache({
  typePolicies: {
    Query: {
      fields: {
        isLoggedIn: {
          read() {
            return isLoggedInVar();
          }
        },
        currentUser: {
          read() {
            return currentUserVar();
          }
        },
        cart: {
          read() {
            return cartVar();
          }
        }
      }
    }
  }
});

// 4. Local resolvers (alternative approach)
const resolvers = {
  Mutation: {
    updateCart: (_root, { item }, { cache }) => {
      const data = cache.readQuery({ query: GET_LOCAL_STATE });
      const newCart = {
        ...data.cart,
        items: [...data.cart.items, item],
        total: data.cart.total + item.product.price * item.quantity
      };
      
      cache.writeQuery({
        query: GET_LOCAL_STATE,
        data: {
          ...data,
          cart: newCart
        }
      });
      
      return newCart;
    }
  }
};

// 5. Advanced caching strategies
@Injectable({
  providedIn: 'root'
})
export class CacheService {
  constructor(private apollo: Apollo) {}
  
  // Cache-first strategy
  getUserCacheFirst(id: string) {
    return this.apollo.watchQuery({
      query: GET_USER,
      variables: { id },
      fetchPolicy: 'cache-first' // Use cache if available
    }).valueChanges;
  }
  
  // Network-only for fresh data
  getUserNetworkOnly(id: string) {
    return this.apollo.watchQuery({
      query: GET_USER,
      variables: { id },
      fetchPolicy: 'network-only' // Always fetch from network
    }).valueChanges;
  }
  
  // Update cache manually
  updateUserInCache(userId: string, updates: Partial<User>) {
    const userQuery = {
      query: GET_USER,
      variables: { id: userId }
    };
    
    const data = this.apollo.client.readQuery(userQuery);
    if (data) {
      this.apollo.client.writeQuery({
        ...userQuery,
        data: {
          user: { ...data.user, ...updates }
        }
      });
    }
  }
  
  // Optimistic updates
  updateUserOptimistic(userId: string, updates: Partial<User>) {
    return this.apollo.mutate({
      mutation: UPDATE_USER,
      variables: { id: userId, input: updates },
      optimisticResponse: {
        updateUser: {
          __typename: 'User',
          id: userId,
          ...updates
        }
      },
      update: (cache, { data }) => {
        // Update specific queries in cache
        cache.writeQuery({
          query: GET_USER,
          variables: { id: userId },
          data: { user: data.updateUser }
        });
      }
    });
  }
  
  // Pagination with cache
  getUsersPaginated(page: number, limit: number) {
    return this.apollo.watchQuery({
      query: gql`
        query GetUsersPaginated($page: Int!, $limit: Int!) {
          users(page: $page, limit: $limit) {
            nodes {
              id
              name
              email
            }
            pageInfo {
              hasNextPage
              hasPreviousPage
              currentPage
              totalPages
            }
          }
        }
      `,
      variables: { page, limit },
      fetchPolicy: 'cache-and-network'
    }).valueChanges;
  }
  
  // Load more with cache merging
  loadMoreUsers() {
    return this.apollo.watchQuery({
      query: GET_USERS_PAGINATED,
      variables: { page: 1, limit: 10 }
    }).fetchMore({
      variables: { page: 2, limit: 10 },
      updateQuery: (prev, { fetchMoreResult }) => {
        if (!fetchMoreResult) return prev;
        
        return {
          users: {
            ...fetchMoreResult.users,
            nodes: [...prev.users.nodes, ...fetchMoreResult.users.nodes]
          }
        };
      }
    });
  }
}

// 6. Real-time subscriptions with cache updates
const USER_SUBSCRIPTION = gql`
  subscription OnUserUpdate($userId: ID!) {
    userUpdated(userId: $userId) {
      id
      name
      email
      lastSeen
    }
  }
`;

@Component({
  selector: 'app-user-detail',
  template: `
    <div *ngIf="user">
      <h2>{{user.name}}</h2>
      <p>{{user.email}}</p>
      <p>Last seen: {{user.lastSeen | date}}</p>
    </div>
  `
})
export class UserDetailComponent implements OnInit, OnDestroy {
  user: any;
  private subscription: any;
  
  constructor(
    private apollo: Apollo,
    private route: ActivatedRoute
  ) {}
  
  ngOnInit() {
    const userId = this.route.snapshot.params['id'];
    
    // Initial query
    this.apollo.watchQuery({
      query: GET_USER,
      variables: { id: userId }
    }).valueChanges.subscribe(result => {
      this.user = result.data.user;
    });
    
    // Subscribe to real-time updates
    this.subscription = this.apollo.subscribe({
      query: USER_SUBSCRIPTION,
      variables: { userId }
    }).subscribe(result => {
      // Update cache with subscription data
      this.apollo.client.writeQuery({
        query: GET_USER,
        variables: { id: userId },
        data: { user: result.data.userUpdated }
      });
    });
  }
  
  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
```

### Expert Level

#### Q29: How do you implement advanced GraphQL patterns like fragments, batch queries, and error handling?
**Answer:**
Advanced GraphQL patterns improve code reusability, performance, and error handling.

**Example:**
```typescript
// 1. GraphQL Fragments for reusability
const USER_FRAGMENT = gql`
  fragment UserInfo on User {
    id
    name
    email
    avatar
    role
    createdAt
    updatedAt
  }
`;

const POST_FRAGMENT = gql`
  fragment PostInfo on Post {
    id
    title
    content
    publishedAt
    author {
      ...UserInfo
    }
    tags {
      id
      name
    }
    stats {
      views
      likes
      comments
    }
  }
  ${USER_FRAGMENT}
`;

// Using fragments in queries
const GET_USER_WITH_POSTS = gql`
  query GetUserWithPosts($userId: ID!, $postLimit: Int) {
    user(id: $userId) {
      ...UserInfo
      posts(limit: $postLimit) {
        ...PostInfo
      }
      followers {
        ...UserInfo
      }
    }
  }
  ${USER_FRAGMENT}
  ${POST_FRAGMENT}
`;

// 2. Batch queries and DataLoader pattern
const BATCH_QUERY = gql`
  query BatchUserData($userIds: [ID!]!, $postIds: [ID!]!) {
    users(ids: $userIds) {
      ...UserInfo
    }
    posts(ids: $postIds) {
      ...PostInfo
    }
    notifications {
      id
      message
      read
      createdAt
    }
  }
  ${USER_FRAGMENT}
  ${POST_FRAGMENT}
`;

@Injectable({
  providedIn: 'root'
})
export class BatchQueryService {
  private pendingUsers = new Set<string>();
  private pendingPosts = new Set<string>();
  private batchTimeout: any;
  
  constructor(private apollo: Apollo) {}
  
  // Batch multiple requests
  loadUser(id: string): Observable<User> {
    this.pendingUsers.add(id);
    this.scheduleBatch();
    
    return this.apollo.watchQuery({
      query: GET_USER,
      variables: { id }
    }).valueChanges.pipe(
      map(result => result.data.user)
    );
  }
  
  private scheduleBatch() {
    if (this.batchTimeout) {
      clearTimeout(this.batchTimeout);
    }
    
    this.batchTimeout = setTimeout(() => {
      this.executeBatch();
    }, 10); // Batch requests within 10ms
  }
  
  private executeBatch() {
    const userIds = Array.from(this.pendingUsers);
    const postIds = Array.from(this.pendingPosts);
    
    if (userIds.length > 0 || postIds.length > 0) {
      this.apollo.query({
        query: BATCH_QUERY,
        variables: { userIds, postIds }
      }).subscribe();
    }
    
    this.pendingUsers.clear();
    this.pendingPosts.clear();
  }
}

// 3. Advanced Error Handling
interface GraphQLFormattedError {
  message: string;
  locations?: Array<{ line: number; column: number }>;
  path?: Array<string | number>;
  extensions?: {
    code: string;
    exception?: {
      stacktrace: string[];
    };
  };
}

@Injectable({
  providedIn: 'root'
})
export class GraphQLErrorHandler {
  
  handleError(error: any): Observable<never> {
    if (error.graphQLErrors) {
      error.graphQLErrors.forEach((graphQLError: GraphQLFormattedError) => {
        this.handleGraphQLError(graphQLError);
      });
    }
    
    if (error.networkError) {
      this.handleNetworkError(error.networkError);
    }
    
    return throwError(error);
  }
  
  private handleGraphQLError(error: GraphQLFormattedError) {
    switch (error.extensions?.code) {
      case 'UNAUTHENTICATED':
        this.redirectToLogin();
        break;
      case 'FORBIDDEN':
        this.showForbiddenMessage();
        break;
      case 'USER_INPUT_ERROR':
        this.showValidationError(error.message);
        break;
      case 'INTERNAL_SERVER_ERROR':
        this.showServerError();
        break;
      default:
        console.error('GraphQL error:', error);
    }
  }
  
  private handleNetworkError(error: any) {
    if (error.status === 0) {
      this.showOfflineMessage();
    } else if (error.status >= 500) {
      this.showServerError();
    } else {
      console.error('Network error:', error);
    }
  }
  
  private redirectToLogin() {
    // Handle authentication error
  }
  
  private showForbiddenMessage() {
    // Show permission denied message
  }
  
  private showValidationError(message: string) {
    // Show validation error
  }
  
  private showServerError() {
    // Show server error message
  }
  
  private showOfflineMessage() {
    // Show offline message
  }
}

// 4. Custom Apollo Link for advanced features
import { ApolloLink } from '@apollo/client/core';
import { setContext } from '@apollo/client/link/context';
import { onError } from '@apollo/client/link/error';
import { RetryLink } from '@apollo/client/link/retry';

const authLink = setContext((_, { headers }) => {
  const token = localStorage.getItem('token');
  return {
    headers: {
      ...headers,
      authorization: token ? `Bearer ${token}` : ''
    }
  };
});

const errorLink = onError(({ graphQLErrors, networkError, operation, forward }) => {
  if (graphQLErrors) {
    graphQLErrors.forEach(({ message, locations, path }) => {
      console.error(
        `GraphQL error: Message: ${message}, Location: ${locations}, Path: ${path}`
      );
    });
  }
  
  if (networkError) {
    console.error(`Network error: ${networkError}`);
    
    // Retry on network error
    if (networkError.statusCode === 500) {
      return forward(operation);
    }
  }
});

const retryLink = new RetryLink({
  delay: {
    initial: 300,
    max: Infinity,
    jitter: true
  },
  attempts: {
    max: 5,
    retryIf: (error, _operation) => !!error
  }
});

// Logging link
const loggingLink = new ApolloLink((operation, forward) => {
  console.log(`Starting request for ${operation.operationName}`);
  
  return forward(operation).map((data) => {
    console.log(`Ending request for ${operation.operationName}`);
    return data;
  });
});

// Combine links
const link = ApolloLink.from([
  loggingLink,
  errorLink,
  retryLink,
  authLink,
  httpLink
]);

// 5. TypeScript code generation
// Install: npm install @graphql-codegen/cli @graphql-codegen/typescript

// codegen.yml
/*
overwrite: true
schema: "http://localhost:4000/graphql"
documents: "src/**/*.graphql"
generates:
  src/generated/graphql.ts:
    plugins:
      - "typescript"
      - "typescript-operations"
      - "typescript-apollo-angular"
    config:
      withComponent: false
      withHOC: false
      withHooks: false
*/

// Generated types usage
import { GetUsersGQL, GetUsersQuery, User } from '../generated/graphql';

@Injectable({
  providedIn: 'root'
})
export class TypedUserService {
  constructor(private getUsersGQL: GetUsersGQL) {}
  
  getUsers(): Observable<User[]> {
    return this.getUsersGQL.watch().valueChanges.pipe(
      map(result => result.data.users)
    );
  }
}

// 6. Testing GraphQL operations
import { ApolloTestingModule, ApolloTestingController } from 'apollo-angular/testing';

describe('UserService', () => {
  let service: UserService;
  let controller: ApolloTestingController;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ApolloTestingModule],
      providers: [UserService]
    });
    
    service = TestBed.inject(UserService);
    controller = TestBed.inject(ApolloTestingController);
  });
  
  it('should get users', () => {
    const mockUsers = [
      { id: '1', name: 'John', email: 'john@example.com' }
    ];
    
    service.getUsers().subscribe(users => {
      expect(users).toEqual(mockUsers);
    });
    
    const op = controller.expectOne(GET_USERS);
    expect(op.operation.operationName).toEqual('GetUsers');
    
    op.flush({
      data: { users: mockUsers }
    });
    
    controller.verify();
  });
});
```

---

## UI Libraries (PrimeNG, Angular Material)

### Beginner Level

#### Q30: What is Angular Material and how do you set it up?
**Answer:**
Angular Material is Google's official Material Design UI component library for Angular applications.

**Setup and Basic Usage:**
```bash
# Install Angular Material
ng add @angular/material

# Or manual installation
npm install @angular/material @angular/cdk @angular/animations
```

**Example:**
```typescript
// 1. app.module.ts
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Material modules
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@NgModule({
  imports: [
    BrowserAnimationsModule,
    MatToolbarModule,
    MatButtonModule,
    MatCardModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,
    MatSnackBarModule
  ]
})
export class AppModule {}

// 2. Basic component usage
@Component({
  selector: 'app-user-form',
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>User Information</mat-card-title>
      </mat-card-header>
      
      <mat-card-content>
        <form [formGroup]="userForm" (ngSubmit)="onSubmit()">
          <mat-form-field appearance="fill">
            <mat-label>Name</mat-label>
            <input matInput formControlName="name" required>
            <mat-error *ngIf="userForm.get('name')?.hasError('required')">
              Name is required
            </mat-error>
          </mat-form-field>
          
          <mat-form-field appearance="fill">
            <mat-label>Email</mat-label>
            <input matInput type="email" formControlName="email" required>
            <mat-error *ngIf="userForm.get('email')?.hasError('email')">
              Please enter a valid email
            </mat-error>
          </mat-form-field>
          
          <mat-form-field appearance="fill">
            <mat-label>Password</mat-label>
            <input matInput [type]="hidePassword ? 'password' : 'text'" 
                   formControlName="password">
            <button mat-icon-button matSuffix 
                    (click)="hidePassword = !hidePassword">
              <mat-icon>{{hidePassword ? 'visibility_off' : 'visibility'}}</mat-icon>
            </button>
          </mat-form-field>
        </form>
      </mat-card-content>
      
      <mat-card-actions>
        <button mat-raised-button color="primary" 
                [disabled]="userForm.invalid"
                (click)="onSubmit()">
          Submit
        </button>
        <button mat-button (click)="onCancel()">Cancel</button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    mat-form-field {
      width: 100%;
      margin-bottom: 16px;
    }
    mat-card {
      max-width: 500px;
      margin: 20px auto;
    }
  `]
})
export class UserFormComponent {
  hidePassword = true;
  
  userForm = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });
  
  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {}
  
  onSubmit() {
    if (this.userForm.valid) {
      console.log('Form submitted:', this.userForm.value);
      this.snackBar.open('User saved successfully!', 'Close', {
        duration: 3000
      });
    }
  }
  
  onCancel() {
    this.userForm.reset();
  }
}

// 3. Data table with Material
@Component({
  selector: 'app-user-table',
  template: `
    <mat-table [dataSource]="dataSource" matSort>
      <ng-container matColumnDef="id">
        <mat-header-cell *matHeaderCellDef mat-sort-header>ID</mat-header-cell>
        <mat-cell *matCellDef="let user">{{user.id}}</mat-cell>
      </ng-container>
      
      <ng-container matColumnDef="name">
        <mat-header-cell *matHeaderCellDef mat-sort-header>Name</mat-header-cell>
        <mat-cell *matCellDef="let user">{{user.name}}</mat-cell>
      </ng-container>
      
      <ng-container matColumnDef="email">
        <mat-header-cell *matHeaderCellDef mat-sort-header>Email</mat-header-cell>
        <mat-cell *matCellDef="let user">{{user.email}}</mat-cell>
      </ng-container>
      
      <ng-container matColumnDef="actions">
        <mat-header-cell *matHeaderCellDef>Actions</mat-header-cell>
        <mat-cell *matCellDef="let user">
          <button mat-icon-button (click)="editUser(user)">
            <mat-icon>edit</mat-icon>
          </button>
          <button mat-icon-button color="warn" (click)="deleteUser(user)">
            <mat-icon>delete</mat-icon>
          </button>
        </mat-cell>
      </ng-container>
      
      <mat-header-row *matHeaderRowDef="displayedColumns"></mat-header-row>
      <mat-row *matRowDef="let row; columns: displayedColumns;"></mat-row>
    </mat-table>
    
    <mat-paginator [pageSizeOptions]="[5, 10, 20]" 
                   showFirstLastButtons></mat-paginator>
  `
})
export class UserTableComponent implements OnInit, AfterViewInit {
  displayedColumns = ['id', 'name', 'email', 'actions'];
  dataSource = new MatTableDataSource<User>();
  
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  
  ngOnInit() {
    this.dataSource.data = this.getUserData();
  }
  
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  
  editUser(user: User) {
    // Open edit dialog
  }
  
  deleteUser(user: User) {
    // Confirm and delete
  }
}
```

#### Q31: What is PrimeNG and how does it compare to Angular Material?
**Answer:**
PrimeNG is a comprehensive UI component library for Angular with a rich set of components.

**Comparison:**
| PrimeNG | Angular Material |
|---------|-----------------|
| More components | Fewer, focused components |
| Multiple themes | Material Design only |
| Built-in icons | Requires icon library |
| Data components (DataTable, TreeTable) | Basic table component |
| Enterprise focus | Google design system |
| Extensive customization | Material-compliant styling |

**PrimeNG Setup and Usage:**
```bash
# Install PrimeNG
npm install primeng primeicons
```

**Example:**
```typescript
// 1. app.module.ts
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@NgModule({
  imports: [
    ButtonModule,
    InputTextModule,
    TableModule,
    CardModule,
    DropdownModule,
    CalendarModule,
    DialogModule,
    ToastModule,
    ConfirmDialogModule
  ]
})
export class AppModule {}

// 2. angular.json - Add PrimeNG CSS
"styles": [
  "node_modules/primeng/resources/themes/saga-blue/theme.css",
  "node_modules/primeng/resources/primeng.min.css",
  "node_modules/primeicons/primeicons.css",
  "src/styles.css"
]

// 3. PrimeNG DataTable example
@Component({
  selector: 'app-product-table',
  template: `
    <p-card header="Product Management">
      <p-table 
        [value]="products" 
        [paginator]="true" 
        [rows]="10"
        [globalFilterFields]="['name', 'category', 'price']"
        [loading]="loading"
        sortField="name"
        sortOrder="1"
        responsiveLayout="scroll">
        
        <ng-template pTemplate="caption">
          <div class="flex justify-content-between align-items-center">
            <h5>Products</h5>
            <span class="p-input-icon-left">
              <i class="pi pi-search"></i>
              <input 
                pInputText 
                type="text" 
                (input)="applyFilterGlobal($event, 'contains')" 
                placeholder="Search keyword" />
            </span>
          </div>
        </ng-template>
        
        <ng-template pTemplate="header">
          <tr>
            <th pSortableColumn="name">
              Name <p-sortIcon field="name"></p-sortIcon>
            </th>
            <th pSortableColumn="category">
              Category <p-sortIcon field="category"></p-sortIcon>
            </th>
            <th pSortableColumn="price">
              Price <p-sortIcon field="price"></p-sortIcon>
            </th>
            <th pSortableColumn="inventoryStatus">
              Status <p-sortIcon field="inventoryStatus"></p-sortIcon>
            </th>
            <th>Actions</th>
          </tr>
        </ng-template>
        
        <ng-template pTemplate="body" let-product>
          <tr>
            <td>{{product.name}}</td>
            <td>{{product.category}}</td>
            <td>{{product.price | currency}}</td>
            <td>
              <span 
                [class]="'product-badge status-' + product.inventoryStatus.toLowerCase()">
                {{product.inventoryStatus}}
              </span>
            </td>
            <td>
              <p-button 
                icon="pi pi-pencil" 
                styleClass="p-button-rounded p-button-success p-mr-2"
                (onClick)="editProduct(product)">
              </p-button>
              <p-button 
                icon="pi pi-trash" 
                styleClass="p-button-rounded p-button-warning"
                (onClick)="deleteProduct(product)">
              </p-button>
            </td>
          </tr>
        </ng-template>
        
        <ng-template pTemplate="emptymessage">
          <tr>
            <td colspan="5">No products found.</td>
          </tr>
        </ng-template>
      </p-table>
    </p-card>
    
    <!-- Toast for notifications -->
    <p-toast></p-toast>
    
    <!-- Confirmation dialog -->
    <p-confirmDialog 
      header="Confirmation" 
      icon="pi pi-exclamation-triangle">
    </p-confirmDialog>
  `,
  styles: [`
    .product-badge {
      border-radius: 2px;
      padding: .25em .5rem;
      text-transform: uppercase;
      font-weight: 700;
      font-size: 12px;
      letter-spacing: .3px;
    }
    .status-instock {
      background: #C8E6C9;
      color: #256029;
    }
    .status-outofstock {
      background: #FFCDD2;
      color: #C63737;
    }
    .status-lowstock {
      background: #FEEDAF;
      color: #8A5340;
    }
  `]
})
export class ProductTableComponent implements OnInit {
  products: Product[] = [];
  loading = false;
  
  constructor(
    private productService: ProductService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}
  
  ngOnInit() {
    this.loadProducts();
  }
  
  loadProducts() {
    this.loading = true;
    this.productService.getProducts().subscribe(
      products => {
        this.products = products;
        this.loading = false;
      }
    );
  }
  
  applyFilterGlobal(event: Event, stringVal: string) {
    const target = event.target as HTMLInputElement;
    this.table.filterGlobal(target.value, stringVal);
  }
  
  editProduct(product: Product) {
    // Open edit dialog or navigate to edit page
  }
  
  deleteProduct(product: Product) {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete ${product.name}?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.productService.deleteProduct(product.id).subscribe(() => {
          this.products = this.products.filter(p => p.id !== product.id);
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: 'Product Deleted'
          });
        });
      }
    });
  }
}
```

### Intermediate Level

#### Q32: How do you create custom themes and styling for Angular Material?
**Answer:**
Angular Material provides theming system based on Material Design principles with customizable palettes.

**Example:**
```scss
// 1. Custom theme - src/styles.scss
@import '~@angular/material/theming';

// Include the common styles for Angular Material
@include mat-core();

// Define custom palettes
$custom-primary: mat-palette($mat-blue, 600, 100, 900);
$custom-accent: mat-palette($mat-orange, 500, 300, 700);
$custom-warn: mat-palette($mat-red, 600);

// Create the theme object
$custom-theme: mat-light-theme((
  color: (
    primary: $custom-primary,
    accent: $custom-accent,
    warn: $custom-warn,
  )
));

// Include theme styles for core and each component
@include angular-material-theme($custom-theme);

// 2. Multiple themes support
.dark-theme {
  $dark-primary: mat-palette($mat-blue-grey);
  $dark-accent: mat-palette($mat-amber, A200, A100, A400);
  $dark-warn: mat-palette($mat-deep-orange);
  
  $dark-theme: mat-dark-theme((
    color: (
      primary: $dark-primary,
      accent: $dark-accent,
      warn: $dark-warn,
    )
  ));
  
  @include angular-material-theme($dark-theme);
}

// 3. Custom component theming
@mixin custom-component-theme($theme) {
  $color-config: mat-get-color-config($theme);
  $primary-palette: map-get($color-config, 'primary');
  $accent-palette: map-get($color-config, 'accent');
  
  .custom-component {
    background-color: mat-color($primary-palette);
    color: mat-color($primary-palette, default-contrast);
    
    .accent-element {
      color: mat-color($accent-palette);
    }
  }
}

@include custom-component-theme($custom-theme);

.dark-theme {
  @include custom-component-theme($dark-theme);
}

// 4. Typography customization
$custom-typography: mat-typography-config(
  $font-family: '"Helvetica Neue", sans-serif',
  $headline: mat-typography-level(32px, 48px, 700),
  $body-1: mat-typography-level(16px, 24px, 400)
);

@include angular-material-typography($custom-typography);

// 5. Component-specific customization
.custom-button {
  &.mat-raised-button {
    border-radius: 20px;
    text-transform: uppercase;
    font-weight: 600;
    
    &.mat-primary {
      background: linear-gradient(45deg, #2196F3 30%, #21CBF3 90%);
      box-shadow: 0 3px 5px 2px rgba(33, 203, 243, .3);
    }
  }
}

.custom-card {
  &.mat-card {
    border-radius: 15px;
    box-shadow: 0 8px 16px rgba(0,0,0,0.1);
    transition: all 0.3s ease;
    
    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 12px 24px rgba(0,0,0,0.15);
    }
  }
}
```

**Theme Service:**
```typescript
// theme.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private isDarkTheme = new BehaviorSubject<boolean>(false);
  
  isDarkTheme$ = this.isDarkTheme.asObservable();
  
  constructor() {
    // Load theme preference from localStorage
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
      this.isDarkTheme.next(savedTheme === 'dark');
    }
  }
  
  toggleTheme() {
    const newTheme = !this.isDarkTheme.value;
    this.isDarkTheme.next(newTheme);
    localStorage.setItem('theme', newTheme ? 'dark' : 'light');
    
    // Apply theme to body class
    if (newTheme) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
  }
  
  getCurrentTheme(): string {
    return this.isDarkTheme.value ? 'dark' : 'light';
  }
}

// Usage in component
@Component({
  selector: 'app-theme-toggle',
  template: `
    <button 
      mat-icon-button 
      (click)="toggleTheme()"
      [attr.aria-label]="(isDarkTheme$ | async) ? 'Switch to light mode' : 'Switch to dark mode'">
      <mat-icon>{{(isDarkTheme$ | async) ? 'light_mode' : 'dark_mode'}}</mat-icon>
    </button>
  `
})
export class ThemeToggleComponent {
  isDarkTheme$ = this.themeService.isDarkTheme$;
  
  constructor(private themeService: ThemeService) {}
  
  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
```

#### Q33: How do you implement responsive design with UI libraries?
**Answer:**
Both Angular Material and PrimeNG provide responsive design capabilities through CSS Grid, Flexbox, and responsive utilities.

**Example:**
```typescript
// 1. Angular Material with Flex Layout
npm install @angular/flex-layout

// app.module.ts
import { FlexLayoutModule } from '@angular/flex-layout';

@NgModule({
  imports: [FlexLayoutModule]
})
export class AppModule {}

// Responsive component
@Component({
  selector: 'app-responsive-layout',
  template: `
    <!-- Responsive navigation -->
    <mat-toolbar color="primary">
      <button 
        mat-icon-button 
        *ngIf="isMobile$ | async"
        (click)="sidenav.toggle()">
        <mat-icon>menu</mat-icon>
      </button>
      <span>My App</span>
      <span fxFlex></span>
      <div fxHide.xs>
        <button mat-button>Home</button>
        <button mat-button>About</button>
        <button mat-button>Contact</button>
      </div>
    </mat-toolbar>
    
    <!-- Responsive sidenav -->
    <mat-sidenav-container fxFlexFill>
      <mat-sidenav 
        #sidenav 
        [mode]="(isMobile$ | async) ? 'over' : 'side'"
        [opened]="!(isMobile$ | async)">
        <mat-nav-list>
          <a mat-list-item routerLink="/dashboard">Dashboard</a>
          <a mat-list-item routerLink="/users">Users</a>
          <a mat-list-item routerLink="/settings">Settings</a>
        </mat-nav-list>
      </mat-sidenav>
      
      <mat-sidenav-content>
        <!-- Responsive grid -->
        <div 
          fxLayout="row wrap" 
          fxLayoutGap="16px grid"
          fxLayoutAlign="start start">
          
          <div 
            fxFlex="100" 
            fxFlex.gt-sm="50" 
            fxFlex.gt-md="33">
            <mat-card>
              <mat-card-header>
                <mat-card-title>Card 1</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                <p>Content for card 1</p>
              </mat-card-content>
            </mat-card>
          </div>
          
          <div 
            fxFlex="100" 
            fxFlex.gt-sm="50" 
            fxFlex.gt-md="33">
            <mat-card>
              <mat-card-header>
                <mat-card-title>Card 2</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                <p>Content for card 2</p>
              </mat-card-content>
            </mat-card>
          </div>
          
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `
})
export class ResponsiveLayoutComponent {
  isMobile$ = this.breakpointObserver.observe(['(max-width: 768px)'])
    .pipe(map(result => result.matches));
  
  constructor(private breakpointObserver: BreakpointObserver) {}
}

// 2. PrimeNG with PrimeFlex
npm install primeflex

// Include in angular.json
"styles": [
  "node_modules/primeflex/primeflex.css"
]

@Component({
  selector: 'app-primeng-responsive',
  template: `
    <!-- Responsive navigation bar -->
    <p-menubar [model]="items" [style]="{'display': isMobile ? 'none' : 'block'}">
      <ng-template pTemplate="start">
        <img src="assets/logo.png" height="40" class="mr-2">
      </ng-template>
    </p-menubar>
    
    <!-- Mobile menu -->
    <p-sidebar [(visible)]="sidebarVisible" position="left" *ngIf="isMobile">
      <p-menu [model]="items"></p-menu>
    </p-sidebar>
    
    <!-- Mobile toolbar -->
    <p-toolbar *ngIf="isMobile">
      <ng-template pTemplate="left">
        <p-button 
          icon="pi pi-bars" 
          (onClick)="sidebarVisible = true">
        </p-button>
      </ng-template>
      <ng-template pTemplate="right">
        <span>My App</span>
      </ng-template>
    </p-toolbar>
    
    <!-- Responsive grid -->
    <div class="grid">
      <div class="col-12 md:col-6 lg:col-4" *ngFor="let product of products">
        <p-card [header]="product.name">
          <ng-template pTemplate="header">
            <img 
              [src]="product.image" 
              [alt]="product.name"
              class="w-full">
          </ng-template>
          
          <p>{{product.description}}</p>
          
          <ng-template pTemplate="footer">
            <div class="flex justify-content-between align-items-center">
              <span class="text-2xl font-bold">{{product.price | currency}}</span>
              <p-button 
                label="Add to Cart"
                icon="pi pi-shopping-cart"
                [style]="{'width': '100%'}"
                styleClass="p-button-sm">
              </p-button>
            </div>
          </ng-template>
        </p-card>
      </div>
    </div>
    
    <!-- Responsive data table -->
    <p-table 
      [value]="users" 
      [responsive]="true"
      responsiveLayout="scroll"
      [breakpoint]="'960px'">
      
      <ng-template pTemplate="header">
        <tr>
          <th>Name</th>
          <th class="hidden-on-mobile">Email</th>
          <th class="hidden-on-mobile">Phone</th>
          <th>Actions</th>
        </tr>
      </ng-template>
      
      <ng-template pTemplate="body" let-user>
        <tr>
          <td>
            <div>
              {{user.name}}
              <div class="text-sm text-600 visible-on-mobile">
                {{user.email}} | {{user.phone}}
              </div>
            </div>
          </td>
          <td class="hidden-on-mobile">{{user.email}}</td>
          <td class="hidden-on-mobile">{{user.phone}}</td>
          <td>
            <p-button 
              icon="pi pi-pencil" 
              styleClass="p-button-text">
            </p-button>
          </td>
        </tr>
      </ng-template>
    </p-table>
  `,
  styles: [`
    @media (max-width: 768px) {
      .hidden-on-mobile {
        display: none !important;
      }
      .visible-on-mobile {
        display: block !important;
      }
    }
    
    @media (min-width: 769px) {
      .visible-on-mobile {
        display: none !important;
      }
    }
  `]
})
export class PrimeNGResponsiveComponent implements OnInit {
  isMobile = false;
  sidebarVisible = false;
  products: Product[] = [];
  users: User[] = [];
  items: MenuItem[] = [];
  
  constructor(private mediaQuery: MediaQueryService) {}
  
  ngOnInit() {
    this.checkScreenSize();
    this.initializeMenuItems();
    this.loadData();
  }
  
  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.checkScreenSize();
  }
  
  private checkScreenSize() {
    this.isMobile = window.innerWidth < 768;
  }
  
  private initializeMenuItems() {
    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-home',
        routerLink: '/home'
      },
      {
        label: 'Products',
        icon: 'pi pi-list',
        items: [
          {
            label: 'View All',
            routerLink: '/products'
          },
          {
            label: 'Add New',
            routerLink: '/products/new'
          }
        ]
      }
    ];
  }
}

// 3. Custom responsive service
@Injectable({
  providedIn: 'root'
})
export class ResponsiveService {
  private screenSize$ = new BehaviorSubject<string>('large');
  
  constructor() {
    this.checkScreenSize();
    fromEvent(window, 'resize')
      .pipe(debounceTime(100))
      .subscribe(() => this.checkScreenSize());
  }
  
  get isSmall$(): Observable<boolean> {
    return this.screenSize$.pipe(map(size => size === 'small'));
  }
  
  get isMedium$(): Observable<boolean> {
    return this.screenSize$.pipe(map(size => size === 'medium'));
  }
  
  get isLarge$(): Observable<boolean> {
    return this.screenSize$.pipe(map(size => size === 'large'));
  }
  
  private checkScreenSize() {
    const width = window.innerWidth;
    if (width < 768) {
      this.screenSize$.next('small');
    } else if (width < 1024) {
      this.screenSize$.next('medium');
    } else {
      this.screenSize$.next('large');
    }
  }
}
```

### Expert Level

#### Q34: How do you create custom Angular Material components with proper theming support?
**Answer:**
Creating custom components that integrate seamlessly with Angular Material's theming system requires following specific patterns.

**Example:**
```typescript
// 1. Custom button component with theming
import { Component, Input, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';
import { ThemePalette } from '@angular/material/core';

@Component({
  selector: 'app-custom-button',
  template: `
    <button 
      class="custom-button"
      [class.custom-button-primary]="color === 'primary'"
      [class.custom-button-accent]="color === 'accent'"
      [class.custom-button-warn]="color === 'warn'"
      [class.custom-button-raised]="variant === 'raised'"
      [class.custom-button-outlined]="variant === 'outlined'"
      [disabled]="disabled">
      
      <span class="custom-button-focus-overlay"></span>
      <span class="custom-button-ripple-container"></span>
      
      <span class="custom-button-content">
        <mat-icon *ngIf="icon" class="custom-button-icon">{{icon}}</mat-icon>
        <ng-content></ng-content>
      </span>
    </button>
  `,
  styleUrls: ['./custom-button.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    'class': 'custom-button-wrapper',
    '[class.custom-button-disabled]': 'disabled'
  }
})
export class CustomButtonComponent {
  @Input() color: ThemePalette = 'primary';
  @Input() variant: 'flat' | 'raised' | 'outlined' = 'flat';
  @Input() disabled = false;
  @Input() icon?: string;
}

// 2. SCSS with theming support
// custom-button.component.scss
@import '~@angular/material/theming';

@mixin custom-button-theme($theme) {
  $color-config: mat-get-color-config($theme);
  $primary-palette: map-get($color-config, 'primary');
  $accent-palette: map-get($color-config, 'accent');
  $warn-palette: map-get($color-config, 'warn');
  $foreground: map-get($color-config, 'foreground');
  $background: map-get($color-config, 'background');
  
  .custom-button {
    position: relative;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    min-width: 64px;
    height: 36px;
    padding: 0 16px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: all 0.2s ease;
    font-family: inherit;
    font-size: 14px;
    font-weight: 500;
    text-transform: uppercase;
    letter-spacing: 1.25px;
    outline: none;
    overflow: hidden;
    
    // Default flat style
    background-color: transparent;
    color: mat-color($foreground, text);
    
    &:hover {
      background-color: mat-color($foreground, base, 0.04);
    }
    
    &:focus {
      .custom-button-focus-overlay {
        opacity: 0.12;
      }
    }
    
    // Primary color
    &.custom-button-primary {
      color: mat-color($primary-palette);
      
      &:hover {
        background-color: mat-color($primary-palette, 0.04);
      }
      
      &.custom-button-raised {
        background-color: mat-color($primary-palette);
        color: mat-color($primary-palette, default-contrast);
        
        &:hover {
          background-color: mat-color($primary-palette, darker);
        }
      }
      
      &.custom-button-outlined {
        border: 1px solid mat-color($primary-palette);
        color: mat-color($primary-palette);
      }
    }
    
    // Accent color
    &.custom-button-accent {
      color: mat-color($accent-palette);
      
      &:hover {
        background-color: mat-color($accent-palette, 0.04);
      }
      
      &.custom-button-raised {
        background-color: mat-color($accent-palette);
        color: mat-color($accent-palette, default-contrast);
      }
    }
    
    // Warn color
    &.custom-button-warn {
      color: mat-color($warn-palette);
      
      &.custom-button-raised {
        background-color: mat-color($warn-palette);
        color: mat-color($warn-palette, default-contrast);
      }
    }
    
    // Disabled state
    &:disabled {
      cursor: default;
      pointer-events: none;
      background-color: mat-color($foreground, disabled-button);
      color: mat-color($foreground, disabled-text);
    }
  }
  
  .custom-button-focus-overlay {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: currentColor;
    opacity: 0;
    transition: opacity 0.2s ease;
    pointer-events: none;
  }
  
  .custom-button-content {
    display: flex;
    align-items: center;
    gap: 8px;
  }
  
  .custom-button-icon {
    font-size: 18px;
    width: 18px;
    height: 18px;
  }
}

// Include in your theme
@include custom-button-theme($your-theme);

// 3. Custom form field component
@Component({
  selector: 'app-custom-form-field',
  template: `
    <div class="custom-form-field" 
         [class.custom-form-field-focused]="focused"
         [class.custom-form-field-filled]="hasValue"
         [class.custom-form-field-invalid]="hasError">
      
      <div class="custom-form-field-wrapper">
        <div class="custom-form-field-flex">
          <div class="custom-form-field-prefix" *ngIf="prefix">
            <ng-content select="[slot=prefix]"></ng-content>
          </div>
          
          <div class="custom-form-field-infix">
            <ng-content></ng-content>
            <label class="custom-form-field-label" 
                   [attr.for]="inputId"
                   *ngIf="label">
              {{label}}
              <span *ngIf="required" class="custom-form-field-required">*</span>
            </label>
          </div>
          
          <div class="custom-form-field-suffix" *ngIf="suffix">
            <ng-content select="[slot=suffix]"></ng-content>
          </div>
        </div>
        
        <div class="custom-form-field-underline">
          <span class="custom-form-field-ripple"></span>
        </div>
      </div>
      
      <div class="custom-form-field-subscript-wrapper">
        <div class="custom-form-field-hint-wrapper">
          <div class="custom-form-field-hint" *ngIf="hint">{{hint}}</div>
        </div>
        <div class="custom-form-field-error-wrapper">
          <mat-error *ngIf="hasError">
            <ng-content select="mat-error"></ng-content>
          </mat-error>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./custom-form-field.component.scss'],
  encapsulation: ViewEncapsulation.None,
  host: {
    'class': 'custom-form-field-container'
  }
})
export class CustomFormFieldComponent implements AfterContentInit {
  @Input() label?: string;
  @Input() hint?: string;
  @Input() required = false;
  @Input() inputId?: string;
  
  @ContentChild(MatInput) input?: MatInput;
  @ContentChild(MatError) error?: MatError;
  
  focused = false;
  hasValue = false;
  hasError = false;
  
  ngAfterContentInit() {
    if (this.input) {
      // Monitor input state
      this.input.stateChanges.subscribe(() => {
        this.updateState();
      });
      
      // Monitor focus events
      this.input.focused.subscribe(focused => {
        this.focused = focused;
      });
    }
  }
  
  private updateState() {
    if (this.input) {
      this.hasValue = !!this.input.value;
      this.hasError = this.input.errorState;
    }
  }
}

// 4. Dynamic component creation
@Injectable({
  providedIn: 'root'
})
export class DynamicComponentService {
  constructor(
    private componentFactoryResolver: ComponentFactoryResolver,
    private injector: Injector
  ) {}
  
  createComponent<T>(
    component: Type<T>,
    container: ViewContainerRef,
    inputs?: { [key: string]: any }
  ): ComponentRef<T> {
    
    const factory = this.componentFactoryResolver.resolveComponentFactory(component);
    const componentRef = container.createComponent(factory, undefined, this.injector);
    
    // Set inputs
    if (inputs) {
      Object.keys(inputs).forEach(key => {
        componentRef.instance[key] = inputs[key];
      });
    }
    
    return componentRef;
  }
  
  // Create modal dialog dynamically
  createDialog<T, R = any>(
    component: Type<T>,
    config?: {
      data?: any;
      width?: string;
      height?: string;
      panelClass?: string;
    }
  ): MatDialogRef<T, R> {
    
    return this.dialog.open(component, {
      width: config?.width || '500px',
      height: config?.height,
      panelClass: config?.panelClass,
      data: config?.data
    });
  }
}

// 5. Advanced theming utilities
@Injectable({
  providedIn: 'root'
})
export class ThemeUtilsService {
  
  // Extract colors from current theme
  getThemeColor(palette: 'primary' | 'accent' | 'warn', hue: string = 'default'): string {
    const theme = this.getCurrentTheme();
    const colorConfig = mat-get-color-config(theme);
    const targetPalette = colorConfig[palette];
    return mat-color(targetPalette, hue);
  }
  
  // Check if current theme is dark
  isDarkTheme(): boolean {
    return document.body.classList.contains('dark-theme');
  }
  
  // Get contrast color
  getContrastColor(backgroundColor: string): string {
    const rgb = this.hexToRgb(backgroundColor);
    const brightness = (rgb.r * 299 + rgb.g * 587 + rgb.b * 114) / 1000;
    return brightness > 128 ? '#000000' : '#ffffff';
  }
  
  private hexToRgb(hex: string): { r: number, g: number, b: number } {
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
      r: parseInt(result[1], 16),
      g: parseInt(result[2], 16),
      b: parseInt(result[3], 16)
    } : { r: 0, g: 0, b: 0 };
  }
}
```

---

## TypeScript

### Beginner Level

#### Q35: What is TypeScript and why is it used in Angular?
**Answer:**
TypeScript is a strongly-typed superset of JavaScript that compiles to plain JavaScript. Angular is built with TypeScript and leverages its features for better development experience.

**Benefits in Angular:**
- **Type Safety**: Catch errors at compile time
- **IntelliSense**: Better IDE support and autocomplete
- **Refactoring**: Safe code refactoring
- **Decorators**: Enable Angular's metadata system
- **Interfaces**: Define contracts for data structures
- **Generics**: Type-safe reusable components

**Example:**
```typescript
// 1. Basic TypeScript types
interface User {
  id: number;
  name: string;
  email: string;
  isActive: boolean;
  roles: string[];
  profile?: UserProfile; // Optional property
}

interface UserProfile {
  avatar: string;
  bio: string;
  socialLinks: Record<string, string>;
}

// 2. Angular service with TypeScript
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://api.example.com/users';
  
  constructor(private http: HttpClient) {}
  
  // Type-safe method signatures
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl);
  }
  
  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }
  
  createUser(user: Omit<User, 'id'>): Observable<User> {
    return this.http.post<User>(this.apiUrl, user);
  }
  
  updateUser(id: number, updates: Partial<User>): Observable<User> {
    return this.http.patch<User>(`${this.apiUrl}/${id}`, updates);
  }
}

// 3. Component with strong typing
@Component({
  selector: 'app-user-list',
  template: `
    <div *ngFor="let user of users; trackBy: trackByUserId">
      <app-user-card [user]="user" (userUpdated)="onUserUpdated($event)"></app-user-card>
    </div>
  `
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  loading = false;
  error: string | null = null;
  
  constructor(private userService: UserService) {}
  
  ngOnInit(): void {
    this.loadUsers();
  }
  
  trackByUserId(index: number, user: User): number {
    return user.id;
  }
  
  onUserUpdated(updatedUser: User): void {
    const index = this.users.findIndex(user => user.id === updatedUser.id);
    if (index !== -1) {
      this.users[index] = updatedUser;
    }
  }
  
  private loadUsers(): void {
    this.loading = true;
    this.userService.getUsers().subscribe({
      next: (users: User[]) => {
        this.users = users;
        this.loading = false;
      },
      error: (error: any) => {
        this.error = error.message;
        this.loading = false;
      }
    });
  }
}

// 4. Type-safe forms
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';

interface UserFormValue {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-user-form',
  template: `
    <form [formGroup]="userForm" (ngSubmit)="onSubmit()">
      <input formControlName="name" placeholder="Name">
      <input formControlName="email" type="email" placeholder="Email">
      <input formControlName="password" type="password" placeholder="Password">
      <input formControlName="confirmPassword" type="password" placeholder="Confirm Password">
      <button type="submit" [disabled]="userForm.invalid">Submit</button>
    </form>
  `
})
export class UserFormComponent {
  userForm: FormGroup<{
    name: FormControl<string>;
    email: FormControl<string>;
    password: FormControl<string>;
    confirmPassword: FormControl<string>;
  }>;
  
  constructor(private fb: FormBuilder) {
    this.userForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, {
      validators: this.passwordMatchValidator
    });
  }
  
  private passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    
    return password === confirmPassword ? null : { passwordMismatch: true };
  }
  
  onSubmit(): void {
    if (this.userForm.valid) {
      const formValue: UserFormValue = this.userForm.value;
      console.log('Form submitted:', formValue);
    }
  }
}
```

#### Q36: Explain TypeScript decorators and their usage in Angular.
**Answer:**
Decorators are a TypeScript feature that allows you to attach metadata to classes, methods, properties, and parameters. Angular heavily uses decorators for configuration.

**Example:**
```typescript
// 1. Class decorators
@Component({
  selector: 'app-example',
  template: '<p>Example component</p>',
  styleUrls: ['./example.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class ExampleComponent {}

@Injectable({
  providedIn: 'root'
})
export class ExampleService {}

@NgModule({
  declarations: [ExampleComponent],
  imports: [CommonModule],
  providers: [ExampleService],
  exports: [ExampleComponent]
})
export class ExampleModule {}

// 2. Property decorators
export class ComponentExample {
  @Input() data: any;
  @Input('customName') value: string;
  @Input({ required: true }) requiredInput: string;
  
  @Output() dataChanged = new EventEmitter<any>();
  @Output('customEvent') eventEmitter = new EventEmitter<string>();
  
  @ViewChild('template') templateRef: TemplateRef<any>;
  @ViewChild(ChildComponent) childComponent: ChildComponent;
  @ViewChild('element', { static: true }) element: ElementRef;
  
  @ViewChildren(ChildComponent) childComponents: QueryList<ChildComponent>;
  @ContentChild(TemplateRef) contentTemplate: TemplateRef<any>;
  @ContentChildren(ChildComponent) contentChildren: QueryList<ChildComponent>;
  
  @HostBinding('class.active') isActive = false;
  @HostBinding('attr.aria-label') ariaLabel = 'Example';
  @HostBinding('style.color') color = 'blue';
  
  @HostListener('click', ['$event'])
  onClick(event: MouseEvent): void {
    console.log('Component clicked', event);
  }
  
  @HostListener('window:resize', ['$event'])
  onWindowResize(event: Event): void {
    console.log('Window resized', event);
  }
}

// 3. Method decorators (custom example)
function LogMethod(target: any, propertyName: string, descriptor: PropertyDescriptor) {
  const method = descriptor.value;
  
  descriptor.value = function (...args: any[]) {
    console.log(`Calling ${propertyName} with arguments:`, args);
    const result = method.apply(this, args);
    console.log(`Method ${propertyName} returned:`, result);
    return result;
  };
}

export class ServiceExample {
  @LogMethod
  calculateTotal(items: number[]): number {
    return items.reduce((sum, item) => sum + item, 0);
  }
}

// 4. Parameter decorators
@Injectable()
export class ServiceWithDependencies {
  constructor(
    private http: HttpClient,
    @Inject(APP_CONFIG) private config: AppConfig,
    @Optional() private optionalService: OptionalService,
    @Self() private selfService: SelfService,
    @SkipSelf() private parentService: ParentService,
    @Host() private hostService: HostService
  ) {}
}

// 5. Custom decorators
// Property decorator for validation
function MinLength(length: number) {
  return function (target: any, propertyName: string) {
    const privatePropertyName = `_${propertyName}`;
    
    Object.defineProperty(target, propertyName, {
      get: function () {
        return this[privatePropertyName];
      },
      set: function (value: string) {
        if (value.length < length) {
          throw new Error(`${propertyName} must be at least ${length} characters long`);
        }
        this[privatePropertyName] = value;
      }
    });
  };
}

// Usage of custom decorator
export class User {
  @MinLength(2)
  name: string;
  
  @MinLength(5)
  email: string;
}

// Method decorator for caching
function Cacheable(ttl: number = 5000) {
  return function (target: any, propertyName: string, descriptor: PropertyDescriptor) {
    const cache = new Map<string, { value: any; timestamp: number }>();
    const originalMethod = descriptor.value;
    
    descriptor.value = function (...args: any[]) {
      const key = JSON.stringify(args);
      const cached = cache.get(key);
      
      if (cached && Date.now() - cached.timestamp < ttl) {
        console.log(`Cache hit for ${propertyName}`);
        return cached.value;
      }
      
      const result = originalMethod.apply(this, args);
      cache.set(key, { value: result, timestamp: Date.now() });
      console.log(`Cache miss for ${propertyName}, result cached`);
      
      return result;
    };
  };
}

// Usage
export class DataService {
  @Cacheable(10000) // Cache for 10 seconds
  getData(id: string): Observable<any> {
    return this.http.get(`/api/data/${id}`);
  }
}
```

### Intermediate Level

#### Q37: How do you use TypeScript generics in Angular applications?
**Answer:**
Generics allow you to create reusable components and services that work with multiple types while maintaining type safety.

**Example:**
```typescript
// 1. Generic service for CRUD operations
export interface BaseEntity {
  id: number | string;
  createdAt: Date;
  updatedAt: Date;
}

@Injectable()
export abstract class BaseService<T extends BaseEntity, TCreate = Omit<T, keyof BaseEntity>, TUpdate = Partial<TCreate>> {
  
  constructor(
    protected http: HttpClient,
    protected baseUrl: string
  ) {}
  
  getAll(): Observable<T[]> {
    return this.http.get<T[]>(this.baseUrl);
  }
  
  getById(id: T['id']): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${id}`);
  }
  
  create(data: TCreate): Observable<T> {
    return this.http.post<T>(this.baseUrl, data);
  }
  
  update(id: T['id'], data: TUpdate): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}/${id}`, data);
  }
  
  delete(id: T['id']): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
  
  // Generic search method
  search<TSearch>(query: TSearch): Observable<T[]> {
    return this.http.post<T[]>(`${this.baseUrl}/search`, query);
  }
}

// 2. Specific service implementations
interface User extends BaseEntity {
  name: string;
  email: string;
  role: 'admin' | 'user';
}

interface CreateUser {
  name: string;
  email: string;
  role: 'admin' | 'user';
}

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService<User, CreateUser> {
  constructor(http: HttpClient) {
    super(http, '/api/users');
  }
  
  // Additional user-specific methods
  getUsersByRole(role: User['role']): Observable<User[]> {
    return this.http.get<User[]>(`${this.baseUrl}?role=${role}`);
  }
}

interface Product extends BaseEntity {
  name: string;
  price: number;
  category: string;
  inStock: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService extends BaseService<Product> {
  constructor(http: HttpClient) {
    super(http, '/api/products');
  }
  
  getByCategory(category: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}?category=${category}`);
  }
}

// 3. Generic component for data tables
@Component({
  selector: 'app-data-table',
  template: `
    <table>
      <thead>
        <tr>
          <th *ngFor="let column of columns">{{column.header}}</th>
          <th *ngIf="actions.length > 0">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let item of data; trackBy: trackByFn">
          <td *ngFor="let column of columns">
            {{getColumnValue(item, column.key)}}
          </td>
          <td *ngIf="actions.length > 0">
            <button 
              *ngFor="let action of actions"
              [class]="action.cssClass"
              (click)="action.handler(item)">
              {{action.label}}
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  `
})
export class DataTableComponent<T extends BaseEntity> implements OnInit {
  @Input() data: T[] = [];
  @Input() columns: TableColumn<T>[] = [];
  @Input() actions: TableAction<T>[] = [];
  @Input() trackByFn: TrackByFunction<T> = (index, item) => item.id;
  
  getColumnValue(item: T, key: keyof T): any {
    return item[key];
  }
}

interface TableColumn<T> {
  key: keyof T;
  header: string;
  sortable?: boolean;
  formatter?: (value: any) => string;
}

interface TableAction<T> {
  label: string;
  handler: (item: T) => void;
  cssClass?: string;
  visible?: (item: T) => boolean;
}

// Usage
@Component({
  selector: 'app-user-list',
  template: `
    <app-data-table 
      [data]="users"
      [columns]="userColumns"
      [actions]="userActions">
    </app-data-table>
  `
})
export class UserListComponent {
  users: User[] = [];
  
  userColumns: TableColumn<User>[] = [
    { key: 'id', header: 'ID' },
    { key: 'name', header: 'Name' },
    { key: 'email', header: 'Email' },
    { key: 'role', header: 'Role' }
  ];
  
  userActions: TableAction<User>[] = [
    {
      label: 'Edit',
      handler: (user) => this.editUser(user),
      cssClass: 'btn-primary'
    },
    {
      label: 'Delete',
      handler: (user) => this.deleteUser(user),
      cssClass: 'btn-danger',
      visible: (user) => user.role !== 'admin'
    }
  ];
  
  editUser(user: User): void {
    // Edit logic
  }
  
  deleteUser(user: User): void {
    // Delete logic
  }
}

// 4. Generic form component
export interface FormConfig<T> {
  fields: FormFieldConfig<T>[];
  validators?: ValidatorFn[];
}

export interface FormFieldConfig<T> {
  key: keyof T;
  label: string;
  type: 'text' | 'email' | 'password' | 'number' | 'select' | 'checkbox';
  required?: boolean;
  validators?: ValidatorFn[];
  options?: { value: any; label: string }[]; // for select
}

@Component({
  selector: 'app-dynamic-form',
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <div *ngFor="let field of config.fields" class="form-field">
        <label [for]="field.key.toString()">{{field.label}}</label>
        
        <input 
          *ngIf="field.type !== 'select' && field.type !== 'checkbox'"
          [id]="field.key.toString()"
          [type]="field.type"
          [formControlName]="field.key.toString()"
          class="form-control">
        
        <select 
          *ngIf="field.type === 'select'"
          [id]="field.key.toString()"
          [formControlName]="field.key.toString()"
          class="form-control">
          <option *ngFor="let option of field.options" [value]="option.value">
            {{option.label}}
          </option>
        </select>
        
        <input 
          *ngIf="field.type === 'checkbox'"
          type="checkbox"
          [id]="field.key.toString()"
          [formControlName]="field.key.toString()">
        
        <div *ngIf="form.get(field.key.toString())?.errors && form.get(field.key.toString())?.touched"
             class="error-message">
          Field is invalid
        </div>
      </div>
      
      <button type="submit" [disabled]="form.invalid">Submit</button>
    </form>
  `
})
export class DynamicFormComponent<T> implements OnInit {
  @Input() config!: FormConfig<T>;
  @Input() initialValue?: Partial<T>;
  @Output() formSubmit = new EventEmitter<T>();
  
  form!: FormGroup;
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit(): void {
    this.buildForm();
  }
  
  private buildForm(): void {
    const formControls: { [K in keyof T]?: FormControl } = {};
    
    this.config.fields.forEach(field => {
      const validators = field.validators || [];
      if (field.required) {
        validators.push(Validators.required);
      }
      
      formControls[field.key] = new FormControl(
        this.initialValue?.[field.key] || null,
        validators
      );
    });
    
    this.form = this.fb.group(formControls);
  }
  
  onSubmit(): void {
    if (this.form.valid) {
      this.formSubmit.emit(this.form.value as T);
    }
  }
}

// 5. Generic HTTP interceptor
@Injectable()
export class GenericHttpInterceptor<TRequest = any, TResponse = any> implements HttpInterceptor {
  
  intercept(req: HttpRequest<TRequest>, next: HttpHandler): Observable<HttpEvent<TResponse>> {
    // Generic request transformation
    const modifiedReq = req.clone({
      setHeaders: {
        'Content-Type': 'application/json',
        'X-Timestamp': Date.now().toString()
      }
    });
    
    return next.handle(modifiedReq).pipe(
      map(event => {
        if (event instanceof HttpResponse) {
          // Generic response transformation
          console.log('Response received:', event.body);
        }
        return event;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('HTTP Error:', error);
        return throwError(error);
      })
    );
  }
}

// 6. Utility types for Angular
type ComponentInputs<T> = {
  [K in keyof T as T[K] extends Function ? never : K]: T[K];
};

type ComponentOutputs<T> = {
  [K in keyof T as T[K] extends EventEmitter<any> ? K : never]: T[K];
};

// Extract input and output types from a component
type UserComponentInputs = ComponentInputs<UserFormComponent>;
type UserComponentOutputs = ComponentOutputs<UserFormComponent>;

// Conditional types for API responses
type ApiResponse<T> = {
  success: true;
  data: T;
} | {
  success: false;
  error: string;
};

// Type guard function
function isSuccessResponse<T>(response: ApiResponse<T>): response is { success: true; data: T } {
  return response.success;
}

// Usage
function handleApiResponse<T>(response: ApiResponse<T>): T | null {
  if (isSuccessResponse(response)) {
    return response.data; // TypeScript knows this is the success case
  } else {
    console.error(response.error); // TypeScript knows this is the error case
    return null;
  }
}
```

### Expert Level

#### Q38: How do you implement advanced TypeScript patterns like mapped types, conditional types, and template literal types in Angular?
**Answer:**
Advanced TypeScript patterns enable sophisticated type manipulation and create more flexible, type-safe Angular applications.

**Example:**
```typescript
// 1. Mapped Types for Form Generation
type FormControl<T> = {
  value: T;
  errors: string[] | null;
  touched: boolean;
  valid: boolean;
};

type FormGroup<T> = {
  [K in keyof T]: FormControl<T[K]>;
} & {
  valid: boolean;
  errors: Record<string, string[]> | null;
};

// Optional fields in forms
type OptionalFormFields<T> = {
  [K in keyof T]?: FormControl<T[K]>;
};

// Pick only required fields
type RequiredFormFields<T> = {
  [K in keyof T as undefined extends T[K] ? never : K]: FormControl<T[K]>;
};

interface User {
  id: number;
  name: string;
  email: string;
  age?: number;
  preferences?: UserPreferences;
}

interface UserPreferences {
  theme: 'light' | 'dark';
  notifications: boolean;
}

// Generate form type
type UserForm = FormGroup<User>;
type UserFormRequired = RequiredFormFields<User>; // Only id, name, email
type UserFormOptional = OptionalFormFields<User>; // All fields optional

// 2. Conditional Types for API Responses
type ApiEndpoint<T extends string> = 
  T extends 'users' ? User[] :
  T extends 'user' ? User :
  T extends 'products' ? Product[] :
  T extends 'product' ? Product :
  never;

type ApiMethod<T extends string> = 
  T extends `${string}s` ? 'GET' | 'POST' :  // Plural endpoints
  T extends `${string}` ? 'GET' | 'PATCH' | 'DELETE' : // Singular endpoints
  never;

// Generic API service with conditional types
@Injectable({
  providedIn: 'root'
})
export class TypedApiService {
  constructor(private http: HttpClient) {}
  
  request<T extends string>(
    endpoint: T,
    method: ApiMethod<T>,
    body?: any
  ): Observable<ApiEndpoint<T>> {
    const url = `/api/${endpoint}`;
    
    switch (method) {
      case 'GET':
        return this.http.get<ApiEndpoint<T>>(url);
      case 'POST':
        return this.http.post<ApiEndpoint<T>>(url, body);
      case 'PATCH':
        return this.http.patch<ApiEndpoint<T>>(url, body);
      case 'DELETE':
        return this.http.delete<ApiEndpoint<T>>(url);
      default:
        throw new Error(`Unsupported method: ${method}`);
    }
  }
}

// Usage with full type safety
const apiService = new TypedApiService();

// TypeScript infers return type as Observable<User[]>
const users$ = apiService.request('users', 'GET');

// TypeScript infers return type as Observable<User>
const user$ = apiService.request('user', 'GET');

// 3. Template Literal Types for Route Type Safety
type RouteParams<T extends string> = 
  T extends `${infer Start}/:${infer Param}/${infer Rest}`
    ? { [K in Param | keyof RouteParams<`${Start}/${Rest}`>]: string }
    : T extends `${infer Start}/:${infer Param}`
    ? { [K in Param]: string }
    : {};

type AppRoutes = 
  | '/home'
  | '/users'
  | '/users/:id'
  | '/users/:id/posts'
  | '/users/:id/posts/:postId'
  | '/products/:category/:id';

// Extract params from route
type UserRouteParams = RouteParams<'/users/:id'>; // { id: string }
type PostRouteParams = RouteParams<'/users/:id/posts/:postId'>; // { id: string, postId: string }
type ProductRouteParams = RouteParams<'/products/:category/:id'>; // { category: string, id: string }

// Type-safe router service
@Injectable({
  providedIn: 'root'
})
export class TypedRouter {
  constructor(private router: Router) {}
  
  navigate<T extends AppRoutes>(
    route: T,
    params: RouteParams<T> extends Record<string, never> ? void : RouteParams<T>
  ): Promise<boolean> {
    if (params) {
      let url = route as string;
      Object.entries(params).forEach(([key, value]) => {
        url = url.replace(`:${key}`, value);
      });
      return this.router.navigate([url]);
    }
    return this.router.navigate([route]);
  }
}

// Usage
const typedRouter = new TypedRouter();

// No params needed
typedRouter.navigate('/home');

// Params required and type-checked
typedRouter.navigate('/users/:id', { id: '123' });
typedRouter.navigate('/users/:id/posts/:postId', { id: '123', postId: '456' });

// 4. Advanced Component Props Validation
type ComponentProps<T> = {
  [K in keyof T as T[K] extends Function ? never : K]: T[K];
};

type RequiredProps<T> = {
  [K in keyof T as undefined extends T[K] ? never : K]: T[K];
};

type OptionalProps<T> = {
  [K in keyof T as undefined extends T[K] ? K : never]?: T[K];
};

// Deep readonly for immutable props
type DeepReadonly<T> = {
  readonly [P in keyof T]: T[P] extends object ? DeepReadonly<T[P]> : T[P];
};

// Generic component base class
export abstract class TypedComponent<TProps = {}> {
  protected props!: DeepReadonly<TProps>;
  
  constructor() {
    // Validate props at runtime
    this.validateProps();
  }
  
  private validateProps(): void {
    // Runtime validation logic
  }
  
  // Type-safe prop updates
  protected updateProp<K extends keyof TProps>(
    key: K,
    value: TProps[K]
  ): void {
    // Immutable update
    this.props = {
      ...this.props,
      [key]: value
    };
  }
}

// 5. Event System with Template Literal Types
type EventMap = {
  'user:created': { user: User };
  'user:updated': { user: User; changes: Partial<User> };
  'user:deleted': { userId: string };
  'auth:login': { user: User; token: string };
  'auth:logout': {};
  'notification:show': { message: string; type: 'success' | 'error' | 'warning' };
};

type EventName = keyof EventMap;

@Injectable({
  providedIn: 'root'
})
export class TypedEventBus {
  private subjects = new Map<EventName, Subject<any>>();
  
  emit<T extends EventName>(event: T, data: EventMap[T]): void {
    const subject = this.getSubject(event);
    subject.next(data);
  }
  
  on<T extends EventName>(event: T): Observable<EventMap[T]> {
    return this.getSubject(event).asObservable();
  }
  
  private getSubject<T extends EventName>(event: T): Subject<EventMap[T]> {
    if (!this.subjects.has(event)) {
      this.subjects.set(event, new Subject<EventMap[T]>());
    }
    return this.subjects.get(event)!;
  }
}

// Usage with full type safety
@Component({
  selector: 'app-user-manager',
  template: ''
})
export class UserManagerComponent implements OnInit {
  constructor(private eventBus: TypedEventBus) {}
  
  ngOnInit(): void {
    // Type-safe event subscription
    this.eventBus.on('user:created').subscribe(({ user }) => {
      console.log('New user created:', user.name);
    });
    
    this.eventBus.on('user:updated').subscribe(({ user, changes }) => {
      console.log(`User ${user.name} updated:`, changes);
    });
  }
  
  createUser(userData: Omit<User, 'id'>): void {
    const user: User = { ...userData, id: Date.now() };
    
    // Type-safe event emission
    this.eventBus.emit('user:created', { user });
  }
}

// 6. State Management with Mapped Types
type ActionType<T extends Record<string, any>> = {
  [K in keyof T]: {
    type: K;
    payload: T[K];
  };
}[keyof T];

type StateActions = {
  'SET_LOADING': boolean;
  'SET_USER': User;
  'UPDATE_USER': Partial<User>;
  'SET_USERS': User[];
  'ADD_USER': User;
  'REMOVE_USER': string; // user id
  'SET_ERROR': string | null;
};

type AppAction = ActionType<StateActions>;

// Type-safe reducer
function appReducer(state: AppState, action: AppAction): AppState {
  switch (action.type) {
    case 'SET_LOADING':
      return { ...state, loading: action.payload };
    
    case 'SET_USER':
      return { ...state, currentUser: action.payload };
    
    case 'UPDATE_USER':
      return {
        ...state,
        currentUser: state.currentUser 
          ? { ...state.currentUser, ...action.payload }
          : null
      };
    
    case 'SET_USERS':
      return { ...state, users: action.payload };
    
    case 'ADD_USER':
      return { ...state, users: [...state.users, action.payload] };
    
    case 'REMOVE_USER':
      return {
        ...state,
        users: state.users.filter(user => user.id.toString() !== action.payload)
      };
    
    case 'SET_ERROR':
      return { ...state, error: action.payload };
    
    default:
      return state;
  }
}

// 7. Utility Types for Angular Testing
type ComponentTestingProps<T> = {
  [K in keyof T as T[K] extends Function ? never : K]: T[K];
};

type ComponentTestingMethods<T> = {
  [K in keyof T as T[K] extends Function ? K : never]: jest.MockedFunction<T[K]>;
};

// Helper for creating component test fixtures
function createComponentFixture<T>(
  component: Type<T>,
  overrides?: Partial<ComponentTestingProps<T>>
): ComponentFixture<T> {
  const fixture = TestBed.createComponent(component);
  
  if (overrides) {
    Object.assign(fixture.componentInstance, overrides);
  }
  
  fixture.detectChanges();
  return fixture;
}

// Type-safe mock creation
function createMockService<T>(service: Type<T>): ComponentTestingMethods<T> {
  const mockMethods = {} as ComponentTestingMethods<T>;
  
  // Implementation would introspect the service and create mocks
  // This is a simplified version
  
  return mockMethods;
}
```

---

## Architecture & Design Patterns

### Beginner Level

#### Q39: What are the main architectural patterns used in Angular applications?
**Answer:**
Angular applications commonly use several architectural patterns to organize code and manage complexity:

**1. Model-View-Controller (MVC) / Model-View-ViewModel (MVVM)**
**2. Component-Based Architecture**
**3. Dependency Injection Pattern**
**4. Observer Pattern (via RxJS)**
**5. Singleton Pattern (Services)**

**Example:**
```typescript
// 1. Component-Based Architecture
// Parent Component
@Component({
  selector: 'app-dashboard',
  template: `
    <app-header [user]="currentUser" (logout)="onLogout()"></app-header>
    <app-sidebar [menuItems]="menuItems"></app-sidebar>
    <app-main-content>
      <app-user-list [users]="users$ | async"></app-user-list>
      <app-statistics [data]="statistics$ | async"></app-statistics>
    </app-main-content>
    <app-footer></app-footer>
  `
})
export class DashboardComponent implements OnInit {
  currentUser: User;
  menuItems: MenuItem[];
  users$ = this.userService.getUsers();
  statistics$ = this.analyticsService.getStatistics();
  
  constructor(
    private userService: UserService,
    private analyticsService: AnalyticsService,
    private authService: AuthService
  ) {}
  
  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    this.menuItems = this.getMenuItems();
  }
  
  onLogout() {
    this.authService.logout();
  }
}

// 2. Smart vs Dumb Components Pattern
// Smart Component (Container)
@Component({
  selector: 'app-user-container',
  template: `
    <app-user-list 
      [users]="users$ | async"
      [loading]="loading$ | async"
      (userSelected)="onUserSelected($event)"
      (userDeleted)="onUserDeleted($event)">
    </app-user-list>
  `
})
export class UserContainerComponent {
  users$ = this.store.select(selectUsers);
  loading$ = this.store.select(selectUsersLoading);
  
  constructor(private store: Store) {}
  
  onUserSelected(user: User) {
    this.store.dispatch(selectUser({ userId: user.id }));
  }
  
  onUserDeleted(user: User) {
    this.store.dispatch(deleteUser({ userId: user.id }));
  }
}

// Dumb Component (Presentational)
@Component({
  selector: 'app-user-list',
  template: `
    <div *ngIf="loading" class="loading">Loading...</div>
    <div *ngIf="!loading" class="user-list">
      <div 
        *ngFor="let user of users; trackBy: trackByUserId"
        class="user-item"
        (click)="userSelected.emit(user)">
        <h3>{{user.name}}</h3>
        <p>{{user.email}}</p>
        <button (click)="userDeleted.emit(user); $event.stopPropagation()">
          Delete
        </button>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent {
  @Input() users: User[] = [];
  @Input() loading = false;
  @Output() userSelected = new EventEmitter<User>();
  @Output() userDeleted = new EventEmitter<User>();
  
  trackByUserId(index: number, user: User): number {
    return user.id;
  }
}

// 3. Service Layer Pattern
// Data Access Layer
@Injectable({
  providedIn: 'root'
})
export class UserRepository {
  constructor(private http: HttpClient) {}
  
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>('/api/users');
  }
  
  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`/api/users/${id}`);
  }
  
  createUser(user: CreateUserDto): Observable<User> {
    return this.http.post<User>('/api/users', user);
  }
  
  updateUser(id: number, user: UpdateUserDto): Observable<User> {
    return this.http.put<User>(`/api/users/${id}`, user);
  }
  
  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`/api/users/${id}`);
  }
}

// Business Logic Layer
@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(
    private userRepository: UserRepository,
    private cacheService: CacheService,
    private validationService: ValidationService
  ) {}
  
  getUsers(): Observable<User[]> {
    return this.cacheService.get('users', () => 
      this.userRepository.getUsers().pipe(
        map(users => users.filter(user => user.isActive))
      )
    );
  }
  
  createUser(userData: CreateUserDto): Observable<User> {
    const validationResult = this.validationService.validateUser(userData);
    if (!validationResult.isValid) {
      return throwError(validationResult.errors);
    }
    
    return this.userRepository.createUser(userData).pipe(
      tap(() => this.cacheService.invalidate('users'))
    );
  }
  
  getUsersWithPermissions(permission: string): Observable<User[]> {
    return this.getUsers().pipe(
      map(users => users.filter(user => 
        user.permissions.includes(permission)
      ))
    );
  }
}

// 4. Facade Pattern
@Injectable({
  providedIn: 'root'
})
export class UserManagementFacade {
  constructor(
    private userService: UserService,
    private authService: AuthService,
    private notificationService: NotificationService,
    private auditService: AuditService
  ) {}
  
  async createUserWithNotification(userData: CreateUserDto): Promise<User> {
    try {
      // Check permissions
      if (!this.authService.hasPermission('CREATE_USER')) {
        throw new Error('Insufficient permissions');
      }
      
      // Create user
      const user = await this.userService.createUser(userData).toPromise();
      
      // Log audit
      this.auditService.log({
        action: 'USER_CREATED',
        userId: user.id,
        performedBy: this.authService.getCurrentUser().id
      });
      
      // Send notification
      this.notificationService.success(`User ${user.name} created successfully`);
      
      return user;
    } catch (error) {
      this.notificationService.error('Failed to create user');
      throw error;
    }
  }
}
```

#### Q40: How do you implement lazy loading and code splitting in Angular?
**Answer:**
Lazy loading allows you to load feature modules only when needed, reducing initial bundle size and improving performance.

**Example:**
```typescript
// 1. Basic Lazy Loading Setup
// app-routing.module.ts
const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  {
    path: 'users',
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule),
    canLoad: [AuthGuard] // Prevent loading if not authenticated
  },
  {
    path: 'admin',
    loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule),
    canLoad: [AdminGuard],
    data: { preload: true } // Preload this module
  },
  {
    path: 'reports',
    loadChildren: () => import('./reports/reports.module').then(m => m.ReportsModule),
    data: { preload: false } // Don't preload
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    // Preloading strategy
    preloadingStrategy: CustomPreloadingStrategy,
    enableTracing: false // Set to true for debugging
  })],
  exports: [RouterModule]
})
export class AppRoutingModule {}

// 2. Custom Preloading Strategy
@Injectable({
  providedIn: 'root'
})
export class CustomPreloadingStrategy implements PreloadingStrategy {
  
  preload(route: Route, load: () => Observable<any>): Observable<any> {
    // Check if route should be preloaded
    if (route.data && route.data['preload']) {
      console.log('Preloading: ' + route.path);
      return load();
    }
    
    return of(null);
  }
}

// 3. Feature Module with Lazy Loading
// users/users.module.ts
@NgModule({
  declarations: [
    UserListComponent,
    UserDetailComponent,
    UserFormComponent
  ],
  imports: [
    CommonModule,
    UsersRoutingModule,
    SharedModule,
    ReactiveFormsModule
  ],
  providers: [
    UserService,
    UserResolver
  ]
})
export class UsersModule {}

// users/users-routing.module.ts
const routes: Routes = [
  {
    path: '',
    component: UserListComponent
  },
  {
    path: 'new',
    component: UserFormComponent,
    canActivate: [PermissionGuard],
    data: { permission: 'CREATE_USER' }
  },
  {
    path: ':id',
    component: UserDetailComponent,
    resolve: { user: UserResolver }
  },
  {
    path: ':id/edit',
    component: UserFormComponent,
    resolve: { user: UserResolver },
    canActivate: [PermissionGuard],
    data: { permission: 'EDIT_USER' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule {}

// 4. Advanced Code Splitting with Dynamic Imports
@Component({
  selector: 'app-dynamic-component-loader',
  template: `
    <div class="component-container">
      <button (click)="loadComponent('chart')">Load Chart</button>
      <button (click)="loadComponent('map')">Load Map</button>
      <button (click)="loadComponent('calendar')">Load Calendar</button>
      
      <div #dynamicContainer class="dynamic-content"></div>
    </div>
  `
})
export class DynamicComponentLoaderComponent {
  @ViewChild('dynamicContainer', { read: ViewContainerRef }) 
  container!: ViewContainerRef;
  
  private loadedComponents = new Map<string, ComponentRef<any>>();
  
  constructor(
    private componentFactoryResolver: ComponentFactoryResolver,
    private injector: Injector
  ) {}
  
  async loadComponent(type: string) {
    // Check if component is already loaded
    if (this.loadedComponents.has(type)) {
      return;
    }
    
    try {
      let component: any;
      
      switch (type) {
        case 'chart':
          const chartModule = await import('./charts/chart.component');
          component = chartModule.ChartComponent;
          break;
          
        case 'map':
          const mapModule = await import('./maps/map.component');
          component = mapModule.MapComponent;
          break;
          
        case 'calendar':
          const calendarModule = await import('./calendar/calendar.component');
          component = calendarModule.CalendarComponent;
          break;
          
        default:
          throw new Error(`Unknown component type: ${type}`);
      }
      
      const factory = this.componentFactoryResolver.resolveComponentFactory(component);
      const componentRef = this.container.createComponent(factory, undefined, this.injector);
      
      this.loadedComponents.set(type, componentRef);
      
    } catch (error) {
      console.error('Failed to load component:', error);
    }
  }
  
  ngOnDestroy() {
    // Clean up loaded components
    this.loadedComponents.forEach(componentRef => {
      componentRef.destroy();
    });
    this.loadedComponents.clear();
  }
}

// 5. Micro-frontend Architecture with Module Federation
// webpack.config.js (for host application)
const ModuleFederationPlugin = require('@module-federation/webpack');

module.exports = {
  plugins: [
    new ModuleFederationPlugin({
      name: 'host',
      remotes: {
        userModule: 'userModule@http://localhost:4201/remoteEntry.js',
        productModule: 'productModule@http://localhost:4202/remoteEntry.js',
      },
    }),
  ],
};

// Dynamic remote module loading
@Injectable({
  providedIn: 'root'
})
export class MicrofrontendService {
  
  async loadRemoteModule(remoteName: string, moduleName: string) {
    try {
      // @ts-ignore
      const remoteContainer = window[remoteName];
      
      if (!remoteContainer) {
        throw new Error(`Remote container ${remoteName} not found`);
      }
      
      const factory = await remoteContainer.get(moduleName);
      const module = factory();
      
      return module;
    } catch (error) {
      console.error('Failed to load remote module:', error);
      throw error;
    }
  }
  
  async loadRemoteComponent(remoteName: string, componentName: string) {
    const module = await this.loadRemoteModule(remoteName, componentName);
    return module.default || module[componentName];
  }
}

// 6. Progressive Loading with Intersection Observer
@Directive({
  selector: '[appLazyLoad]'
})
export class LazyLoadDirective implements OnInit, OnDestroy {
  @Input() appLazyLoad: string; // Component type to load
  @Output() componentLoaded = new EventEmitter<any>();
  
  private observer: IntersectionObserver;
  
  constructor(
    private el: ElementRef,
    private viewContainer: ViewContainerRef,
    private componentLoader: DynamicComponentService
  ) {}
  
  ngOnInit() {
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            this.loadComponent();
            this.observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.1 }
    );
    
    this.observer.observe(this.el.nativeElement);
  }
  
  private async loadComponent() {
    try {
      const component = await this.componentLoader.loadComponent(this.appLazyLoad);
      const componentRef = this.viewContainer.createComponent(component);
      this.componentLoaded.emit(componentRef);
    } catch (error) {
      console.error('Failed to lazy load component:', error);
    }
  }
  
  ngOnDestroy() {
    if (this.observer) {
      this.observer.disconnect();
    }
  }
}

// Usage
// <div appLazyLoad="heavy-chart" (componentLoaded)="onChartLoaded($event)">
//   <div class="placeholder">Loading chart...</div>
// </div>

// 7. Bundle Optimization and Analysis
// Build with bundle analysis
// ng build --prod --stats-json
// npx webpack-bundle-analyzer dist/stats.json

// Custom webpack configuration for optimization
const path = require('path');

module.exports = {
  optimization: {
    splitChunks: {
      chunks: 'all',
      cacheGroups: {
        vendor: {
          test: /[\\/]node_modules[\\/]/,
          name: 'vendors',
          chunks: 'all',
        },
        common: {
          name: 'common',
          minChunks: 2,
          chunks: 'all',
          enforce: true
        }
      }
    }
  }
};
```

### Intermediate Level

#### Q41: How do you implement effective state management architecture in large Angular applications?
**Answer:**
Large applications require carefully planned state management to maintain data consistency and application performance.

**Example:**
```typescript
// 1. Layered State Architecture
// Core State (Global)
export interface CoreState {
  auth: AuthState;
  ui: UIState;
  router: RouterState;
}

// Feature States
export interface UserFeatureState {
  users: UserState;
  userPreferences: UserPreferencesState;
}

export interface ProductFeatureState {
  products: ProductState;
  categories: CategoryState;
  inventory: InventoryState;
}

// Combined Application State
export interface AppState extends CoreState {
  userFeature: UserFeatureState;
  productFeature: ProductFeatureState;
}

// 2. State Management Service Pattern
@Injectable({
  providedIn: 'root'
})
export class StateManagerService {
  private store$ = new BehaviorSubject<AppState>(initialState);
  
  // State selectors
  select<T>(selector: (state: AppState) => T): Observable<T> {
    return this.store$.pipe(
      map(selector),
      distinctUntilChanged()
    );
  }
  
  // State updates
  updateState(updater: (state: AppState) => AppState): void {
    const currentState = this.store$.value;
    const newState = updater(currentState);
    this.store$.next(newState);
  }
  
  // Async state updates
  updateStateAsync<T>(
    asyncOperation: Observable<T>,
    stateUpdater: (state: AppState, result: T) => AppState
  ): Observable<T> {
    return asyncOperation.pipe(
      tap(result => {
        this.updateState(state => stateUpdater(state, result));
      })
    );
  }
  
  // Optimistic updates with rollback
  optimisticUpdate<T>(
    optimisticUpdater: (state: AppState) => AppState,
    asyncOperation: Observable<T>,
    successUpdater: (state: AppState, result: T) => AppState,
    rollbackUpdater: (state: AppState) => AppState
  ): Observable<T> {
    // Apply optimistic update
    this.updateState(optimisticUpdater);
    
    return asyncOperation.pipe(
      tap(result => {
        // Apply success update
        this.updateState(state => successUpdater(state, result));
      }),
      catchError(error => {
        // Rollback on error
        this.updateState(rollbackUpdater);
        return throwError(error);
      })
    );
  }
}

// 3. Feature State Services
@Injectable({
  providedIn: 'root'
})
export class UserStateService {
  private state$ = new BehaviorSubject<UserState>(initialUserState);
  
  // Selectors
  users$ = this.state$.pipe(map(state => state.entities));
  loading$ = this.state$.pipe(map(state => state.loading));
  selectedUser$ = this.state$.pipe(map(state => state.selectedUser));
  
  constructor(
    private userRepository: UserRepository,
    private stateManager: StateManagerService
  ) {}
  
  // Actions
  loadUsers(): Observable<User[]> {
    this.updateState(state => ({ ...state, loading: true }));
    
    return this.userRepository.getUsers().pipe(
      tap(users => {
        this.updateState(state => ({
          ...state,
          entities: users,
          loading: false,
          error: null
        }));
      }),
      catchError(error => {
        this.updateState(state => ({
          ...state,
          loading: false,
          error: error.message
        }));
        return throwError(error);
      })
    );
  }
  
  selectUser(userId: string): void {
    this.updateState(state => ({
      ...state,
      selectedUserId: userId,
      selectedUser: state.entities.find(u => u.id === userId) || null
    }));
  }
  
  updateUser(userId: string, updates: Partial<User>): Observable<User> {
    return this.stateManager.optimisticUpdate(
      // Optimistic update
      state => ({
        ...state,
        entities: state.entities.map(user =>
          user.id === userId ? { ...user, ...updates } : user
        )
      }),
      // Async operation
      this.userRepository.updateUser(userId, updates),
      // Success update
      (state, updatedUser) => ({
        ...state,
        entities: state.entities.map(user =>
          user.id === userId ? updatedUser : user
        )
      }),
      // Rollback
      state => ({
        ...state,
        entities: state.entities.map(user =>
          user.id === userId ? 
            state.entities.find(u => u.id === userId) || user : 
            user
        )
      })
    );
  }
  
  private updateState(updater: (state: UserState) => UserState): void {
    const currentState = this.state$.value;
    const newState = updater(currentState);
    this.state$.next(newState);
  }
}

// 4. Cross-Feature State Coordination
@Injectable({
  providedIn: 'root'
})
export class StateCoordinatorService {
  constructor(
    private userStateService: UserStateService,
    private productStateService: ProductStateService,
    private notificationService: NotificationService
  ) {
    this.setupCrossFeatureEffects();
  }
  
  private setupCrossFeatureEffects(): void {
    // When user changes, update related product data
    this.userStateService.selectedUser$.pipe(
      filter(user => !!user),
      switchMap(user => this.productStateService.loadUserProducts(user.id))
    ).subscribe();
    
    // Global error handling
    merge(
      this.userStateService.state$.pipe(map(state => state.error)),
      this.productStateService.state$.pipe(map(state => state.error))
    ).pipe(
      filter(error => !!error),
      tap(error => this.notificationService.showError(error))
    ).subscribe();
  }
}

// 5. State Persistence and Hydration
@Injectable({
  providedIn: 'root'
})
export class StatePersistenceService {
  private readonly STORAGE_KEY = 'app_state';
  
  constructor(private stateManager: StateManagerService) {
    this.setupPersistence();
    this.hydrateState();
  }
  
  private setupPersistence(): void {
    // Debounce state changes to avoid excessive storage writes
    this.stateManager.select(state => state).pipe(
      debounceTime(1000),
      tap(state => this.persistState(state))
    ).subscribe();
  }
  
  private persistState(state: AppState): void {
    try {
      // Only persist specific parts of state
      const persistableState = {
        auth: {
          user: state.auth.user,
          isAuthenticated: state.auth.isAuthenticated
        },
        ui: {
          theme: state.ui.theme,
          sidebarCollapsed: state.ui.sidebarCollapsed
        }
      };
      
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(persistableState));
    } catch (error) {
      console.warn('Failed to persist state:', error);
    }
  }
  
  private hydrateState(): void {
    try {
      const savedState = localStorage.getItem(this.STORAGE_KEY);
      if (savedState) {
        const parsedState = JSON.parse(savedState);
        this.stateManager.updateState(currentState => ({
          ...currentState,
          ...parsedState
        }));
      }
    } catch (error) {
      console.warn('Failed to hydrate state:', error);
    }
  }
}

// 6. State DevTools Integration
@Injectable({
  providedIn: 'root'
})
export class StateDevToolsService {
  private devTools: any;
  
  constructor(private stateManager: StateManagerService) {
    this.setupDevTools();
  }
  
  private setupDevTools(): void {
    if (typeof window !== 'undefined' && (window as any).__REDUX_DEVTOOLS_EXTENSION__) {
      this.devTools = (window as any).__REDUX_DEVTOOLS_EXTENSION__.connect({
        name: 'Angular State Manager'
      });
      
      // Send state changes to devtools
      this.stateManager.select(state => state).subscribe(state => {
        this.devTools.send('STATE_UPDATE', state);
      });
      
      // Handle time-travel debugging
      this.devTools.subscribe((message: any) => {
        if (message.type === 'DISPATCH' && message.state) {
          this.stateManager.updateState(() => JSON.parse(message.state));
        }
      });
    }
  }
}

// 7. State Testing Utilities
export class StateTestingUtils {
  static createMockState(overrides: Partial<AppState> = {}): AppState {
    return {
      ...initialState,
      ...overrides
    };
  }
  
  static createStateObserver<T>(): {
    observer: jasmine.Spy;
    values: T[];
  } {
    const values: T[] = [];
    const observer = jasmine.createSpy('observer').and.callFake((value: T) => {
      values.push(value);
    });
    
    return { observer, values };
  }
  
  static waitForStateChange<T>(
    observable: Observable<T>,
    predicate: (value: T) => boolean,
    timeout = 5000
  ): Promise<T> {
    return new Promise((resolve, reject) => {
      const subscription = observable.pipe(
        filter(predicate),
        take(1),
        timeout(timeout)
      ).subscribe({
        next: resolve,
        error: reject
      });
    });
  }
}

// Usage in tests
describe('UserStateService', () => {
  let service: UserStateService;
  let stateObserver: jasmine.Spy;
  
  beforeEach(() => {
    const { observer } = StateTestingUtils.createStateObserver<UserState>();
    stateObserver = observer;
    service.state$.subscribe(stateObserver);
  });
  
  it('should load users', async () => {
    service.loadUsers().subscribe();
    
    const finalState = await StateTestingUtils.waitForStateChange(
      service.state$,
      state => !state.loading && state.entities.length > 0
    );
    
    expect(finalState.entities).toBeDefined();
    expect(finalState.loading).toBe(false);
  });
});
```

### Expert Level

#### Q42: How do you implement advanced performance optimization techniques in Angular?
**Answer:**
Advanced performance optimization involves multiple strategies including change detection optimization, bundle optimization, memory management, and runtime performance tuning.

**Example:**
```typescript
// 1. Advanced Change Detection Optimization
@Component({
  selector: 'app-optimized-list',
  template: `
    <div class="list-container">
      <!-- Virtual scrolling for large lists -->
      <cdk-virtual-scroll-viewport itemSize="50" class="viewport">
        <div 
          *cdkVirtualFor="let item of items; let i = index; trackBy: trackByFn"
          class="list-item"
          [class.selected]="item.id === selectedId"
          (click)="selectItem(item)">
          
          <!-- Optimized sub-components with OnPush -->
          <app-list-item 
            [item]="item"
            [selected]="item.id === selectedId"
            (itemAction)="handleItemAction($event)">
          </app-list-item>
        </div>
      </cdk-virtual-scroll-viewport>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    'class': 'optimized-list'
  }
})
export class OptimizedListComponent implements OnInit, OnDestroy {
  @Input() set data(value: any[]) {
    // Immutable input handling
    if (value !== this._data) {
      this._data = value;
      this.processData();
    }
  }
  get data() { return this._data; }
  private _data: any[] = [];
  
  items: OptimizedItem[] = [];
  selectedId: string | null = null;
  
  private destroy$ = new Subject<void>();
  
  constructor(
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) {}
  
  ngOnInit() {
    // Use OnPush with manual change detection
    this.setupDataStreams();
  }
  
  private setupDataStreams() {
    // Batch updates to avoid excessive change detection
    this.dataService.data$.pipe(
      debounceTime(16), // ~60fps
      takeUntil(this.destroy$)
    ).subscribe(data => {
      this.items = this.transformData(data);
      this.cdr.markForCheck();
    });
  }
  
  // Optimized trackBy function
  trackByFn = (index: number, item: OptimizedItem): string => {
    return item.id; // Use unique identifier
  };
  
  selectItem(item: OptimizedItem) {
    // Run outside Angular zone for non-Angular operations
    this.ngZone.runOutsideAngular(() => {
      this.analytics.track('item_selected', { itemId: item.id });
    });
    
    this.selectedId = item.id;
    this.cdr.markForCheck();
  }
  
  private processData() {
    // Use web workers for heavy computations
    if (this._data.length > 1000) {
      this.processDataInWorker();
    } else {
      this.items = this.transformData(this._data);
    }
  }
  
  private processDataInWorker() {
    const worker = new Worker('./data-processing.worker', { type: 'module' });
    worker.postMessage(this._data);
    
    worker.onmessage = ({ data }) => {
      this.items = data;
      this.cdr.markForCheck();
      worker.terminate();
    };
  }
  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

// 2. Memory Optimization Strategies
@Injectable({
  providedIn: 'root'
})
export class MemoryOptimizedService {
  private cache = new Map<string, WeakRef<any>>();
  private subscription: Subscription = new Subscription();
  
  constructor() {
    this.setupMemoryManagement();
  }
  
  private setupMemoryManagement() {
    // Periodic cleanup of weak references
    interval(30000).subscribe(() => this.cleanupWeakRefs());
    
    // Monitor memory usage
    if ('memory' in performance) {
      interval(10000).subscribe(() => this.checkMemoryUsage());
    }
  }
  
  // Use weak references for caching
  getCachedData<T>(key: string, factory: () => T): T {
    const weakRef = this.cache.get(key);
    let data = weakRef?.deref();
    
    if (!data) {
      data = factory();
      this.cache.set(key, new WeakRef(data));
    }
    
    return data;
  }
  
  private cleanupWeakRefs() {
    for (const [key, weakRef] of this.cache.entries()) {
      if (!weakRef.deref()) {
        this.cache.delete(key);
      }
    }
  }
  
  private checkMemoryUsage() {
    const memory = (performance as any).memory;
    const usagePercent = (memory.usedJSHeapSize / memory.jsHeapSizeLimit) * 100;
    
    if (usagePercent > 80) {
      console.warn('High memory usage detected:', usagePercent.toFixed(2) + '%');
      this.triggerGarbageCollection();
    }
  }
  
  private triggerGarbageCollection() {
    // Clear caches
    this.cache.clear();
    
    // Suggest garbage collection (if available)
    if ('gc' in window) {
      (window as any).gc();
    }
  }
}

// 3. Bundle Splitting and Lazy Loading Optimization
@Injectable({
  providedIn: 'root'
})
export class ModulePreloadingService {
  private preloadedModules = new Set<string>();
  
  constructor(
    private router: Router,
    private injector: Injector
  ) {
    this.setupIntelligentPreloading();
  }
  
  private setupIntelligentPreloading() {
    // Preload based on user behavior
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      debounceTime(2000) // Wait for user to settle
    ).subscribe(() => {
      this.preloadLikelyNextRoutes();
    });
    
    // Preload on idle
    this.preloadOnIdle();
  }
  
  private preloadLikelyNextRoutes() {
    const currentRoute = this.router.url;
    const likelyRoutes = this.predictNextRoutes(currentRoute);
    
    likelyRoutes.forEach(route => {
      if (!this.preloadedModules.has(route)) {
        this.preloadRoute(route);
      }
    });
  }
  
  private predictNextRoutes(currentRoute: string): string[] {
    // AI/ML-based route prediction or simple heuristics
    const routePatterns = {
      '/dashboard': ['/users', '/reports'],
      '/users': ['/users/:id', '/users/new'],
      '/products': ['/products/:id', '/inventory']
    };
    
    return routePatterns[currentRoute] || [];
  }
  
  private preloadRoute(route: string) {
    // Find and preload the module
    const config = this.router.config.find(r => r.path === route.split('/')[1]);
    
    if (config && config.loadChildren) {
      this.preloadedModules.add(route);
      // Trigger module loading
      import(/* webpackChunkName: "preloaded" */ `${config.loadChildren}`);
    }
  }
  
  private preloadOnIdle() {
    if ('requestIdleCallback' in window) {
      const preload = () => {
        (window as any).requestIdleCallback(() => {
          this.preloadLowPriorityModules();
        });
      };
      
      // Start preloading after initial load
      setTimeout(preload, 3000);
    }
  }
  
  private preloadLowPriorityModules() {
    const lowPriorityModules = [
      () => import('./admin/admin.module'),
      () => import('./settings/settings.module'),
      () => import('./help/help.module')
    ];
    
    lowPriorityModules.forEach(moduleLoader => {
      moduleLoader().catch(err => {
        console.warn('Failed to preload module:', err);
      });
    });
  }
}

// 4. Runtime Performance Monitoring
@Injectable({
  providedIn: 'root'
})
export class PerformanceMonitorService {
  private metrics = new Map<string, number[]>();
  private observer: PerformanceObserver | null = null;
  
  constructor() {
    this.setupPerformanceMonitoring();
  }
  
  private setupPerformanceMonitoring() {
    // Monitor long tasks
    if ('PerformanceObserver' in window) {
      this.observer = new PerformanceObserver((list) => {
        for (const entry of list.getEntries()) {
          if (entry.duration > 50) { // > 50ms is considered long
            console.warn('Long task detected:', entry);
            this.reportPerformanceIssue('long-task', entry.duration);
          }
        }
      });
      
      this.observer.observe({ entryTypes: ['longtask'] });
    }
    
    // Monitor custom metrics
    this.setupCustomMetrics();
  }
  
  private setupCustomMetrics() {
    // Component render time tracking
    this.trackComponentRenderTimes();
    
    // API response time tracking
    this.trackApiResponseTimes();
    
    // Memory usage tracking
    this.trackMemoryUsage();
  }
  
  markStart(label: string) {
    performance.mark(`${label}-start`);
  }
  
  markEnd(label: string) {
    performance.mark(`${label}-end`);
    performance.measure(label, `${label}-start`, `${label}-end`);
    
    const measure = performance.getEntriesByName(label)[0];
    this.addMetric(label, measure.duration);
    
    // Clean up marks
    performance.clearMarks(`${label}-start`);
    performance.clearMarks(`${label}-end`);
    performance.clearMeasures(label);
  }
  
  private addMetric(name: string, value: number) {
    if (!this.metrics.has(name)) {
      this.metrics.set(name, []);
    }
    
    const values = this.metrics.get(name)!;
    values.push(value);
    
    // Keep only last 100 measurements
    if (values.length > 100) {
      values.shift();
    }
    
    // Report if performance degrades
    this.checkPerformanceDegradation(name, values);
  }
  
  private checkPerformanceDegradation(name: string, values: number[]) {
    if (values.length < 10) return;
    
    const recent = values.slice(-5);
    const baseline = values.slice(0, 10);
    
    const recentAvg = recent.reduce((a, b) => a + b) / recent.length;
    const baselineAvg = baseline.reduce((a, b) => a + b) / baseline.length;
    
    if (recentAvg > baselineAvg * 1.5) { // 50% increase
      this.reportPerformanceIssue('degradation', recentAvg, {
        metric: name,
        baseline: baselineAvg,
        current: recentAvg
      });
    }
  }
  
  private reportPerformanceIssue(type: string, value: number, details?: any) {
    // Send to analytics/monitoring service
    console.warn(`Performance issue detected: ${type}`, { value, details });
  }
  
  getMetrics(): { [key: string]: { avg: number; min: number; max: number } } {
    const result: any = {};
    
    for (const [name, values] of this.metrics.entries()) {
      if (values.length > 0) {
        result[name] = {
          avg: values.reduce((a, b) => a + b) / values.length,
          min: Math.min(...values),
          max: Math.max(...values)
        };
      }
    }
    
    return result;
  }
}

// 5. Component Performance Decorator
export function PerformanceTrack(componentName?: string) {
  return function <T extends { new(...args: any[]): {} }>(constructor: T) {
    const name = componentName || constructor.name;
    
    return class extends constructor {
      constructor(...args: any[]) {
        super(...args);
        
        // Track component initialization time
        const initStart = performance.now();
        
        // Hook into lifecycle
        const originalNgOnInit = this.ngOnInit;
        const originalNgAfterViewInit = this.ngAfterViewInit;
        
        this.ngOnInit = function() {
          const initEnd = performance.now();
          console.log(`${name} init time: ${initEnd - initStart}ms`);
          
          if (originalNgOnInit) {
            const renderStart = performance.now();
            originalNgOnInit.call(this);
            const renderEnd = performance.now();
            console.log(`${name} render time: ${renderEnd - renderStart}ms`);
          }
        };
        
        this.ngAfterViewInit = function() {
          if (originalNgAfterViewInit) {
            originalNgAfterViewInit.call(this);
          }
          
          const totalTime = performance.now() - initStart;
          console.log(`${name} total load time: ${totalTime}ms`);
        };
      }
    };
  };
}

// Usage
@PerformanceTrack('UserListComponent')
@Component({
  selector: 'app-user-list',
  template: '...'
})
export class UserListComponent implements OnInit, AfterViewInit {
  ngOnInit() {
    // Component initialization
  }
  
  ngAfterViewInit() {
    // View initialization
  }
}

// 6. Image Optimization Service
@Injectable({
  providedIn: 'root'
})
export class ImageOptimizationService {
  private imageCache = new Map<string, HTMLImageElement>();
  private observer: IntersectionObserver;
  
  constructor() {
    this.setupLazyLoading();
  }
  
  private setupLazyLoading() {
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            this.loadImage(entry.target as HTMLImageElement);
            this.observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.1, rootMargin: '50px' }
    );
  }
  
  optimizeImage(
    url: string,
    options: {
      width?: number;
      height?: number;
      quality?: number;
      format?: 'webp' | 'jpeg' | 'png';
    } = {}
  ): string {
    // Generate optimized image URL
    const params = new URLSearchParams();
    
    if (options.width) params.set('w', options.width.toString());
    if (options.height) params.set('h', options.height.toString());
    if (options.quality) params.set('q', options.quality.toString());
    if (options.format) params.set('f', options.format);
    
    return `${url}?${params.toString()}`;
  }
  
  lazyLoad(img: HTMLImageElement) {
    this.observer.observe(img);
  }
  
  private async loadImage(img: HTMLImageElement) {
    const src = img.dataset['src'];
    if (!src) return;
    
    try {
      // Check cache first
      if (this.imageCache.has(src)) {
        img.src = src;
        return;
      }
      
      // Preload image
      const preloadImg = new Image();
      preloadImg.onload = () => {
        img.src = src;
        this.imageCache.set(src, preloadImg);
        img.classList.add('loaded');
      };
      
      preloadImg.onerror = () => {
        img.classList.add('error');
      };
      
      preloadImg.src = src;
      
    } catch (error) {
      console.error('Failed to load image:', error);
    }
  }
}
```

---

## Performance Optimization

### Beginner Level

#### Q43: What are the basic performance optimization techniques in Angular?
**Answer:**
Basic performance optimization focuses on reducing bundle size, optimizing change detection, and improving loading times.

**Key Techniques:**

**Example:**
```typescript
// 1. OnPush Change Detection Strategy
@Component({
  selector: 'app-user-card',
  template: `
    <div class="user-card">
      <img [src]="user.avatar" [alt]="user.name">
      <h3>{{user.name}}</h3>
      <p>{{user.email}}</p>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserCardComponent {
  @Input() user: User;
}

// 2. TrackBy Functions for *ngFor
@Component({
  selector: 'app-user-list',
  template: `
    <div *ngFor="let user of users; trackBy: trackByUserId" class="user-item">
      <app-user-card [user]="user"></app-user-card>
    </div>
  `
})
export class UserListComponent {
  users: User[] = [];
  
  trackByUserId(index: number, user: User): number {
    return user.id; // Use unique identifier instead of index
  }
}

// 3. Lazy Loading Images
@Directive({
  selector: '[appLazyLoad]'
})
export class LazyLoadDirective implements OnInit {
  @Input() appLazyLoad: string;
  
  constructor(private el: ElementRef) {}
  
  ngOnInit() {
    const img = this.el.nativeElement;
    const observer = new IntersectionObserver((entries) => {
      if (entries[0].isIntersecting) {
        img.src = this.appLazyLoad;
        observer.disconnect();
      }
    });
    
    observer.observe(img);
  }
}

// Usage: <img appLazyLoad="path/to/image.jpg" alt="Lazy loaded image">

// 4. Async Pipe Usage
@Component({
  selector: 'app-data-display',
  template: `
    <div *ngIf="data$ | async as data">
      <div *ngFor="let item of data">{{item.name}}</div>
    </div>
  `
})
export class DataDisplayComponent {
  data$ = this.dataService.getData(); // No manual subscription needed
  
  constructor(private dataService: DataService) {}
}

// 5. Preloading Strategies
const routes: Routes = [
  {
    path: 'users',
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    preloadingStrategy: PreloadAllModules // Preload all lazy modules
  })]
})
export class AppRoutingModule {}
```

### Intermediate Level

#### Q44: How do you optimize Angular applications for production?
**Answer:**
Production optimization involves build optimization, tree shaking, compression, and runtime performance improvements.

**Example:**
```typescript
// 1. Build Optimization Configuration
// angular.json
{
  "projects": {
    "my-app": {
      "architect": {
        "build": {
          "configurations": {
            "production": {
              "optimization": true,
              "outputHashing": "all",
              "sourceMap": false,
              "namedChunks": false,
              "extractLicenses": true,
              "vendorChunk": false,
              "buildOptimizer": true,
              "budgets": [
                {
                  "type": "initial",
                  "maximumWarning": "2mb",
                  "maximumError": "5mb"
                }
              ]
            }
          }
        }
      }
    }
  }
}

// 2. Tree Shaking and Dead Code Elimination
// Import only what you need
import { map, filter } from 'rxjs/operators'; // ✅ Good
// import * as operators from 'rxjs/operators'; // ❌ Bad

// Use barrel exports carefully
// shared/index.ts
export { ButtonComponent } from './button/button.component';
export { CardComponent } from './card/card.component';
// Don't export everything: export * from './components';

// 3. Lazy Loading with Preloading Strategy
@Injectable()
export class CustomPreloadingStrategy implements PreloadingStrategy {
  preload(route: Route, load: () => Observable<any>): Observable<any> {
    // Only preload routes marked for preloading
    if (route.data && route.data['preload']) {
      console.log('Preloading: ' + route.path);
      return load();
    }
    return of(null);
  }
}

// 4. Service Worker for Caching
ng add @angular/pwa

// Custom service worker configuration
// ngsw-config.json
{
  "index": "/index.html",
  "assetGroups": [
    {
      "name": "app",
      "installMode": "prefetch",
      "resources": {
        "files": [
          "/favicon.ico",
          "/index.html",
          "/manifest.webmanifest",
          "/*.css",
          "/*.js"
        ]
      }
    },
    {
      "name": "assets",
      "installMode": "lazy",
      "updateMode": "prefetch",
      "resources": {
        "files": [
          "/assets/**",
          "/*.(eot|svg|cur|jpg|png|webp|gif|otf|ttf|woff|woff2|ani)"
        ]
      }
    }
  ],
  "dataGroups": [
    {
      "name": "api-cache",
      "urls": ["/api/**"],
      "cacheConfig": {
        "strategy": "performance",
        "maxSize": 100,
        "maxAge": "1h"
      }
    }
  ]
}

// 5. Virtual Scrolling for Large Lists
@Component({
  selector: 'app-virtual-list',
  template: `
    <cdk-virtual-scroll-viewport itemSize="50" class="viewport">
      <div *cdkVirtualFor="let item of items; let i = index" class="item">
        Item #{{i}}: {{item.name}}
      </div>
    </cdk-virtual-scroll-viewport>
  `,
  styles: [`
    .viewport {
      height: 400px;
      width: 100%;
    }
    .item {
      height: 50px;
      display: flex;
      align-items: center;
      padding: 0 16px;
    }
  `]
})
export class VirtualListComponent {
  items = Array.from({length: 100000}, (_, i) => ({
    id: i,
    name: `Item ${i}`
  }));
}

// 6. Memory Leak Prevention
@Component({
  selector: 'app-leak-free',
  template: '...'
})
export class LeakFreeComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  ngOnInit() {
    // Use takeUntil for subscription management
    this.dataService.getData().pipe(
      takeUntil(this.destroy$)
    ).subscribe(data => {
      // Handle data
    });
    
    // Avoid memory leaks with intervals
    interval(1000).pipe(
      takeUntil(this.destroy$)
    ).subscribe(tick => {
      // Handle tick
    });
  }
  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

// 7. Image Optimization
@Component({
  selector: 'app-optimized-image',
  template: `
    <!-- Use responsive images -->
    <picture>
      <source media="(max-width: 768px)" srcset="image-mobile.webp" type="image/webp">
      <source media="(max-width: 768px)" srcset="image-mobile.jpg">
      <source srcset="image-desktop.webp" type="image/webp">
      <img src="image-desktop.jpg" alt="Optimized image" loading="lazy">
    </picture>
    
    <!-- Use Angular's optimized image directive (Angular 15+) -->
    <img ngSrc="hero.jpg" alt="Hero image" priority width="400" height="200">
  `
})
export class OptimizedImageComponent {}
```

---

## Version Support & Migration

### Beginner Level

#### Q45: What are the major differences between Angular versions?
**Answer:**
Angular follows semantic versioning with major releases every 6 months. Each version introduces new features, improvements, and sometimes breaking changes.

**Version Timeline & Key Features:**

| Version | Release Date | Key Features |
|---------|-------------|--------------|
| Angular 2 | Sep 2016 | Complete rewrite, TypeScript, Mobile-first |
| Angular 4 | Mar 2017 | Angular Universal, Angular CLI, Smaller bundles |
| Angular 5 | Nov 2017 | Angular CLI Workspaces, Angular Forms |
| Angular 6 | May 2018 | ng update, ng add, Tree-shakable providers |
| Angular 7 | Oct 2018 | Bundle budgets, CLI Prompts, Virtual Scrolling |
| Angular 8 | May 2019 | Dynamic imports, Web Workers, Ivy renderer (opt-in) |
| Angular 9 | Feb 2020 | Ivy renderer by default, Tree-shaking improvements |
| Angular 10 | Jun 2020 | New date range picker, New Angular Material Design |
| Angular 11 | Nov 2020 | Automatic font inlining, Hot module replacement |
| Angular 12 | May 2021 | Ivy everywhere, Strict mode, Sass support for inline styles |
| Angular 13 | Nov 2021 | View Engine deprecated, Angular Package Format changes |
| Angular 14 | Jun 2022 | Standalone components, Optional injectors, Angular CLI auto-completion |
| Angular 15 | Nov 2022 | Stable standalone APIs, Image directive, MDC-based Angular Material |
| Angular 16 | May 2023 | Signals (developer preview), Required inputs, Standalone ng new collection |
| Angular 17 | Nov 2023 | New brand identity, New lifecycle, View Transitions API |

**Example Migration:**
```typescript
// Angular 8 to 9 Migration Example

// Before (Angular 8 - View Engine)
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

@NgModule({
  imports: [BrowserModule],
  entryComponents: [DynamicComponent] // Required for dynamic components
})
export class AppModule {}

// After (Angular 9+ - Ivy)
@NgModule({
  imports: [BrowserModule]
  // entryComponents no longer needed with Ivy
})
export class AppModule {}

// Dynamic component loading improvements
@Component({
  template: '<ng-container #container></ng-container>'
})
export class DynamicLoaderComponent {
  @ViewChild('container', { read: ViewContainerRef }) container!: ViewContainerRef;
  
  loadComponent() {
    // Ivy allows easier dynamic component loading
    this.container.createComponent(DynamicComponent);
  }
}
```

#### Q46: How do you migrate from AngularJS to Angular?
**Answer:**
Migrating from AngularJS (1.x) to Angular (2+) requires a systematic approach due to architectural differences.

**Migration Strategies:**

**Example:**
```typescript
// 1. Big Bang Migration (Small apps)
// AngularJS (1.x)
angular.module('myApp', [])
  .controller('UserController', function($scope, $http) {
    $scope.users = [];
    
    $http.get('/api/users').then(function(response) {
      $scope.users = response.data;
    });
    
    $scope.addUser = function(user) {
      $http.post('/api/users', user).then(function(response) {
        $scope.users.push(response.data);
      });
    };
  });

// Angular (Modern)
@Component({
  selector: 'app-user',
  template: `
    <div *ngFor="let user of users">{{user.name}}</div>
    <button (click)="addUser()">Add User</button>
  `
})
export class UserComponent implements OnInit {
  users: User[] = [];
  
  constructor(private http: HttpClient) {}
  
  ngOnInit() {
    this.http.get<User[]>('/api/users').subscribe(users => {
      this.users = users;
    });
  }
  
  addUser() {
    const newUser = { name: 'New User', email: 'user@example.com' };
    this.http.post<User>('/api/users', newUser).subscribe(user => {
      this.users.push(user);
    });
  }
}

// 2. Hybrid Migration (Large apps)
// Using @angular/upgrade for gradual migration
import { UpgradeModule } from '@angular/upgrade/static';
import { NgModule } from '@angular/core';

@NgModule({
  imports: [
    BrowserModule,
    UpgradeModule,
    HttpClientModule
  ],
  declarations: [
    NewAngularComponent
  ],
  entryComponents: [
    NewAngularComponent
  ]
})
export class AppModule {
  constructor(private upgrade: UpgradeModule) {}
  
  ngDoBootstrap() {
    this.upgrade.bootstrap(document.body, ['myAngularJSApp'], { strictDi: true });
  }
}

// Downgrade Angular component for use in AngularJS
import { downgradeComponent } from '@angular/upgrade/static';

angular.module('myAngularJSApp')
  .directive('newAngularComponent', 
    downgradeComponent({ component: NewAngularComponent })
  );

// Upgrade AngularJS service for use in Angular
import { UpgradeModule } from '@angular/upgrade/static';

@Injectable()
export class UpgradedService {
  constructor(@Inject('legacyService') private legacyService: any) {}
  
  getData() {
    return this.legacyService.getData();
  }
}

// 3. Component Migration Patterns
// AngularJS Component
angular.module('myApp')
  .component('userList', {
    template: `
      <div ng-repeat="user in $ctrl.users">
        {{user.name}}
      </div>
    `,
    controller: function() {
      this.users = [];
      this.loadUsers = function() {
        // Load users
      };
    }
  });

// Angular Component
@Component({
  selector: 'app-user-list',
  template: `
    <div *ngFor="let user of users">
      {{user.name}}
    </div>
  `
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  
  ngOnInit() {
    this.loadUsers();
  }
  
  private loadUsers() {
    // Load users
  }
}

// 4. Service Migration
// AngularJS Service
angular.module('myApp')
  .service('UserService', function($http) {
    this.getUsers = function() {
      return $http.get('/api/users');
    };
  });

// Angular Service
@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private http: HttpClient) {}
  
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>('/api/users');
  }
}

// 5. Routing Migration
// AngularJS UI-Router
angular.module('myApp')
  .config(function($stateProvider) {
    $stateProvider
      .state('users', {
        url: '/users',
        template: '<user-list></user-list>'
      })
      .state('user-detail', {
        url: '/users/:id',
        template: '<user-detail></user-detail>',
        controller: 'UserDetailController'
      });
  });

// Angular Router
const routes: Routes = [
  { path: 'users', component: UserListComponent },
  { path: 'users/:id', component: UserDetailComponent },
  { path: '', redirectTo: '/users', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
```

### Intermediate Level

#### Q47: How do you handle breaking changes when upgrading Angular versions?
**Answer:**
Breaking changes require careful planning, testing, and systematic updates to maintain application functionality.

**Example:**
```typescript
// 1. Angular 11 to 12 Migration
// Before (Angular 11)
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  template: `
    <form [formGroup]="form">
      <input formControlName="name">
    </form>
  `
})
export class FormComponent {
  form: FormGroup;
  
  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: [''] // Implicit any type
    });
  }
}

// After (Angular 12 - Strict Forms)
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';

interface UserForm {
  name: FormControl<string>;
}

@Component({
  template: `
    <form [formGroup]="form">
      <input formControlName="name">
    </form>
  `
})
export class FormComponent {
  form: FormGroup<UserForm>;
  
  constructor(private fb: FormBuilder) {
    this.form = this.fb.group<UserForm>({
      name: new FormControl('', { nonNullable: true })
    });
  }
}

// 2. Angular 13 Migration - View Engine Removal
// Update angular.json to remove View Engine options
{
  "projects": {
    "my-app": {
      "architect": {
        "build": {
          "options": {
            // Remove these View Engine specific options:
            // "enableIvy": false,
            // "aot": false
          }
        }
      }
    }
  }
}

// Update components that relied on View Engine specifics
// Before (View Engine)
@Component({
  template: '<ng-content></ng-content>',
  encapsulation: ViewEncapsulation.Native // Not supported in Ivy
})
export class LegacyComponent {}

// After (Ivy)
@Component({
  template: '<ng-content></ng-content>',
  encapsulation: ViewEncapsulation.ShadowDom // Ivy equivalent
})
export class ModernComponent {}

// 3. Angular 14 Migration - Standalone Components
// Before (Module-based)
@NgModule({
  declarations: [UserComponent],
  imports: [CommonModule, ReactiveFormsModule],
  exports: [UserComponent]
})
export class UserModule {}

// After (Standalone)
@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: '...'
})
export class UserComponent {}

// Bootstrap standalone component
// main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, {
  providers: [
    // Configure providers here
    importProvidersFrom(HttpClientModule),
    provideRouter(routes)
  ]
});

// 4. Angular 15 Migration - Image Directive
// Before (Regular img tag)
@Component({
  template: `
    <img src="hero.jpg" alt="Hero image" style="width: 400px; height: 200px;">
  `
})
export class ImageComponent {}

// After (Optimized NgOptimizedImage)
import { NgOptimizedImage } from '@angular/common';

@Component({
  standalone: true,
  imports: [NgOptimizedImage],
  template: `
    <img ngSrc="hero.jpg" alt="Hero image" width="400" height="200" priority>
  `
})
export class OptimizedImageComponent {}

// 5. Angular 16 Migration - Required Inputs
// Before (Optional inputs with validation)
@Component({
  selector: 'app-user-card',
  template: '...'
})
export class UserCardComponent implements OnInit {
  @Input() user?: User;
  
  ngOnInit() {
    if (!this.user) {
      throw new Error('User input is required');
    }
  }
}

// After (Required inputs)
@Component({
  selector: 'app-user-card',
  template: '...'
})
export class UserCardComponent {
  @Input({ required: true }) user!: User; // Compiler enforces requirement
}

// 6. Migration Automation with ng update
// Update Angular CLI and core packages
ng update @angular/cli @angular/core

// Update Angular Material
ng update @angular/material

// Update specific packages with migration schematics
ng update @angular/core --migrate-only --from=14 --to=15

// 7. Custom Migration Schematic
import { Rule, SchematicContext, Tree } from '@angular-devkit/schematics';
import { visitTypeScriptFiles } from '@angular/cdk/schematics';

export function migrate(): Rule {
  return (tree: Tree, context: SchematicContext) => {
    visitTypeScriptFiles(tree, filePath => {
      const content = tree.read(filePath)?.toString();
      if (!content) return;
      
      // Replace deprecated APIs
      const updatedContent = content
        .replace(/ViewEncapsulation\.Native/g, 'ViewEncapsulation.ShadowDom')
        .replace(/enableIvy:\s*false/g, '// enableIvy removed in Angular 13');
      
      if (content !== updatedContent) {
        tree.overwrite(filePath, updatedContent);
        context.logger.info(`Updated ${filePath}`);
      }
    });
  };
}
```

### Expert Level

#### Q48: How do you implement version compatibility and maintain backward compatibility in Angular libraries?
**Answer:**
Library compatibility requires careful API design, versioning strategy, and migration support for consuming applications.

**Example:**
```typescript
// 1. Semantic Versioning Strategy for Libraries
// package.json
{
  "name": "@myorg/ui-library",
  "version": "2.1.0", // MAJOR.MINOR.PATCH
  "peerDependencies": {
    "@angular/core": "^13.0.0 || ^14.0.0 || ^15.0.0",
    "@angular/common": "^13.0.0 || ^14.0.0 || ^15.0.0"
  }
}

// 2. Backward Compatibility Patterns
// Version 1.x API (deprecated but supported)
@Component({
  selector: 'lib-button',
  template: `
    <button 
      [class]="getButtonClasses()"
      [disabled]="disabled"
      (click)="onClick.emit($event)">
      <ng-content></ng-content>
    </button>
  `
})
export class ButtonComponent implements OnInit {
  // Deprecated properties (still supported)
  /** @deprecated Use 'variant' instead. Will be removed in v3.0 */
  @Input() type: 'primary' | 'secondary' = 'primary';
  
  // New API
  @Input() variant: 'filled' | 'outlined' | 'text' = 'filled';
  @Input() color: 'primary' | 'accent' | 'warn' = 'primary';
  @Input() disabled = false;
  @Output() onClick = new EventEmitter<Event>();
  
  ngOnInit() {
    // Handle backward compatibility
    if (this.type && !this.variant) {
      this.variant = this.type === 'primary' ? 'filled' : 'outlined';
      console.warn('ButtonComponent: "type" input is deprecated. Use "variant" instead.');
    }
  }
  
  getButtonClasses(): string {
    return `btn btn-${this.variant} btn-${this.color}`;
  }
}

// 3. Progressive API Enhancement
export interface ButtonConfigV1 {
  type?: 'primary' | 'secondary';
  disabled?: boolean;
}

export interface ButtonConfigV2 extends Omit<ButtonConfigV1, 'type'> {
  variant?: 'filled' | 'outlined' | 'text';
  color?: 'primary' | 'accent' | 'warn';
  size?: 'small' | 'medium' | 'large';
}

// Factory function for configuration migration
export function migrateButtonConfig(config: ButtonConfigV1): ButtonConfigV2 {
  const { type, ...rest } = config;
  
  return {
    ...rest,
    variant: type === 'primary' ? 'filled' : 'outlined',
    color: 'primary'
  };
}

// 4. Conditional Feature Loading Based on Angular Version
import { VERSION as ANGULAR_VERSION } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FeatureDetectionService {
  private angularVersion = ANGULAR_VERSION.major;
  
  supportsStandaloneComponents(): boolean {
    return this.angularVersion >= 14;
  }
  
  supportsSignals(): boolean {
    return this.angularVersion >= 16;
  }
  
  supportsControlFlow(): boolean {
    return this.angularVersion >= 17;
  }
  
  getOptimalChangeDetectionStrategy(): ChangeDetectionStrategy {
    // Use OnPush more aggressively in newer versions
    return this.angularVersion >= 14 
      ? ChangeDetectionStrategy.OnPush 
      : ChangeDetectionStrategy.Default;
  }
}

// 5. Multi-Version Build Support
// build-config.ts
export interface BuildConfig {
  angularVersion: number;
  features: {
    standalone: boolean;
    signals: boolean;
    strictForms: boolean;
  };
}

const buildConfigs: { [version: string]: BuildConfig } = {
  'ng13': {
    angularVersion: 13,
    features: { standalone: false, signals: false, strictForms: false }
  },
  'ng14': {
    angularVersion: 14,
    features: { standalone: true, signals: false, strictForms: false }
  },
  'ng16': {
    angularVersion: 16,
    features: { standalone: true, signals: true, strictForms: true }
  }
};

// Conditional compilation based on target version
declare const NG_VERSION: string;
const config = buildConfigs[NG_VERSION] || buildConfigs['ng16'];

// Component with version-specific features
@Component({
  selector: 'lib-adaptive-component',
  // Conditional standalone based on Angular version
  ...(config.features.standalone ? { standalone: true } : {}),
  template: `
    <div class="adaptive-component">
      <!-- Use signals if available -->
      @if (config.features.signals) {
        <div>Signal value: {{signalValue()}}</div>
      } @else {
        <div>Observable value: {{observableValue | async}}</div>
      }
    </div>
  `
})
export class AdaptiveComponent {
  // Conditional signal usage
  ...(config.features.signals 
    ? { signalValue: signal('Hello from signals!') }
    : { observableValue: of('Hello from observables!') }
  )
}

// 6. Migration Schematics for Library Updates
// migration.json
{
  "schematics": {
    "migration-v2": {
      "version": "2.0.0",
      "description": "Updates to version 2 APIs",
      "factory": "./schematics/migration-v2/index.js"
    }
  }
}

// Migration schematic implementation
import { Rule, SchematicContext, Tree } from '@angular-devkit/schematics';
import { getWorkspace } from '@schematics/angular/utility/workspace';
import { visitTypeScriptFiles } from '@angular/cdk/schematics';
import * as ts from 'typescript';

export function updateToV2(): Rule {
  return async (tree: Tree, context: SchematicContext) => {
    const workspace = await getWorkspace(tree);
    
    // Update each project
    for (const [projectName, project] of workspace.projects) {
      const projectPath = project.root;
      
      visitTypeScriptFiles(tree, filePath => {
        if (!filePath.startsWith(projectPath)) return;
        
        const content = tree.read(filePath);
        if (!content) return;
        
        const sourceFile = ts.createSourceFile(
          filePath,
          content.toString(),
          ts.ScriptTarget.Latest,
          true
        );
        
        const updatedContent = updateButtonComponentUsage(sourceFile);
        if (updatedContent !== content.toString()) {
          tree.overwrite(filePath, updatedContent);
          context.logger.info(`Updated ${filePath}`);
        }
      });
    }
  };
}

function updateButtonComponentUsage(sourceFile: ts.SourceFile): string {
  const transformer: ts.TransformerFactory<ts.SourceFile> = context => {
    return node => {
      const visitor = (node: ts.Node): ts.Node => {
        // Update template literals containing button components
        if (ts.isStringLiteral(node) || ts.isNoSubstitutionTemplateLiteral(node)) {
          const text = node.text;
          const updatedText = text
            .replace(/type="primary"/g, 'variant="filled"')
            .replace(/type="secondary"/g, 'variant="outlined"');
          
          if (text !== updatedText) {
            return ts.factory.createStringLiteral(updatedText);
          }
        }
        
        return ts.visitEachChild(node, visitor, context);
      };
      
      return ts.visitNode(node, visitor);
    };
  };
  
  const result = ts.transform(sourceFile, [transformer]);
  const printer = ts.createPrinter();
  return printer.printFile(result.transformed[0]);
}

// 7. Testing Across Multiple Angular Versions
// .github/workflows/test-matrix.yml
name: Test Matrix
on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        angular-version: [13, 14, 15, 16, 17]
        node-version: [16, 18, 20]
    
    steps:
      - uses: actions/checkout@v3
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: ${{ matrix.node-version }}
      
      - name: Install Angular CLI
        run: npm install -g @angular/cli@${{ matrix.angular-version }}
      
      - name: Install dependencies
        run: npm ci
      
      - name: Run tests
        run: npm run test:ci
        env:
          NG_VERSION: ${{ matrix.angular-version }}

// Package.json scripts for multi-version testing
{
  "scripts": {
    "test:ng13": "ng test --configuration=ng13",
    "test:ng14": "ng test --configuration=ng14",
    "test:ng15": "ng test --configuration=ng15",
    "test:ng16": "ng test --configuration=ng16",
    "test:all-versions": "npm run test:ng13 && npm run test:ng14 && npm run test:ng15 && npm run test:ng16",
    "build:all-versions": "npm run build:ng13 && npm run build:ng14 && npm run build:ng15 && npm run build:ng16"
  }
}

// 8. Documentation for Migration
// MIGRATION.md
/*
# Migration Guide

## Upgrading from v1.x to v2.x

### Breaking Changes

1. **Button Component**
   - `type` input has been replaced with `variant`
   - Migration: `type="primary"` → `variant="filled"`
   - Migration: `type="secondary"` → `variant="outlined"`

### Automatic Migration

Run the migration schematic:
```bash
ng update @myorg/ui-library
```

### Manual Updates Required

1. Update custom CSS classes that reference old button types
2. Update any TypeScript code that programmatically sets button types

### Deprecation Timeline

- v2.0: `type` input deprecated but still functional
- v2.x: Deprecation warnings added
- v3.0: `type` input will be removed
*/
```

---

## Conclusion

This comprehensive Angular interview questions document covers:

1. **Angular Core Concepts** - Components, services, dependency injection, lifecycle hooks
2. **Angular CLI** - Project setup, code generation, build optimization
3. **Nx Workspace** - Monorepo management, code sharing, workspace organization
4. **RxJS** - Reactive programming, observables, operators, memory management
5. **State Management** - NgRx, NGXS, service-based patterns
6. **Apollo Client & GraphQL** - Data fetching, caching, real-time subscriptions
7. **UI Libraries** - Angular Material, PrimeNG, theming, responsive design
8. **TypeScript** - Advanced types, decorators, generics, Angular integration
9. **Architecture & Performance** - Design patterns, optimization techniques, lazy loading
10. **Version Support** - Migration strategies, breaking changes, compatibility

Each section progresses from beginner to expert level, providing practical examples and real-world scenarios that demonstrate both theoretical knowledge and hands-on experience with Angular development.

**Key Takeaways:**
- Angular is a comprehensive framework requiring knowledge across multiple domains
- Performance optimization is crucial for production applications
- State management patterns scale with application complexity
- Migration strategies are essential for maintaining long-term projects
- Modern Angular embraces reactive programming and TypeScript's advanced features

This document serves as both an interview preparation guide and a reference for Angular development best practices.

---

*Document completed with comprehensive coverage of Angular, Nx, RxJS, state management, GraphQL, UI libraries, TypeScript, architecture patterns, performance optimization, and version migration strategies.*