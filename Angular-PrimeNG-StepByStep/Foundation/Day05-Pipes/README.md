# Day 05: Angular Pipes 🔄

## 📋 Learning Objectives

By the end of this lesson, you will be able to:
- Understand what pipes are and why they're useful
- Use built-in Angular pipes
- Chain multiple pipes together
- Create custom pipes
- Understand pure vs impure pipes
- Apply pipes for data transformation

---

## 🎯 What are Pipes?

Pipes transform data in templates. They take data as input and return a transformed output.

**Syntax:**
```html
{{ value | pipeName }}
{{ value | pipeName: parameter }}
{{ value | pipe1 | pipe2 | pipe3 }}
```

---

## 🔧 Built-in Pipes

### 1. String Pipes

```html
<!-- UpperCasePipe -->
<p>{{ 'hello world' | uppercase }}</p>
<!-- Output: HELLO WORLD -->

<!-- LowerCasePipe -->
<p>{{ 'HELLO WORLD' | lowercase }}</p>
<!-- Output: hello world -->

<!-- TitleCasePipe -->
<p>{{ 'hello world' | titlecase }}</p>
<!-- Output: Hello World -->

<!-- SlicePipe -->
<p>{{ 'Hello World' | slice:0:5 }}</p>
<!-- Output: Hello -->

<p>{{ 'Hello World' | slice:6 }}</p>
<!-- Output: World -->
```

### 2. Number Pipes

```typescript
export class AppComponent {
  price = 1234.5678;
  percentage = 0.259;
  largeNumber = 1234567890;
}
```

```html
<!-- DecimalPipe (number) -->
<p>{{ price | number }}</p>
<!-- Output: 1,234.568 -->

<p>{{ price | number:'1.2-2' }}</p>
<!-- Output: 1,234.57 (min 1 integer, 2-2 decimals) -->

<p>{{ price | number:'3.1-5' }}</p>
<!-- Output: 1,234.5678 (min 3 integers, 1-5 decimals) -->

<!-- PercentPipe -->
<p>{{ percentage | percent }}</p>
<!-- Output: 25.9% -->

<p>{{ percentage | percent:'1.2-2' }}</p>
<!-- Output: 25.90% -->

<!-- CurrencyPipe -->
<p>{{ price | currency }}</p>
<!-- Output: $1,234.57 (default USD) -->

<p>{{ price | currency:'EUR' }}</p>
<!-- Output: €1,234.57 -->

<p>{{ price | currency:'INR':'symbol':'1.0-0' }}</p>
<!-- Output: ₹1,235 -->

<p>{{ price | currency:'USD':'code' }}</p>
<!-- Output: USD1,234.57 -->
```

### 3. Date Pipes

```typescript
export class AppComponent {
  today = new Date();
  birthday = new Date(1990, 0, 15);
  timestamp = 1634567890000;
}
```

```html
<!-- DatePipe -->
<p>{{ today | date }}</p>
<!-- Output: Oct 11, 2025 -->

<!-- Predefined formats -->
<p>{{ today | date:'short' }}</p>
<!-- Output: 10/11/25, 3:45 PM -->

<p>{{ today | date:'medium' }}</p>
<!-- Output: Oct 11, 2025, 3:45:30 PM -->

<p>{{ today | date:'long' }}</p>
<!-- Output: October 11, 2025 at 3:45:30 PM GMT+5 -->

<p>{{ today | date:'full' }}</p>
<!-- Output: Saturday, October 11, 2025 at 3:45:30 PM GMT+05:30 -->

<p>{{ today | date:'shortDate' }}</p>
<!-- Output: 10/11/25 -->

<p>{{ today | date:'mediumDate' }}</p>
<!-- Output: Oct 11, 2025 -->

<p>{{ today | date:'longDate' }}</p>
<!-- Output: October 11, 2025 -->

<p>{{ today | date:'fullDate' }}</p>
<!-- Output: Saturday, October 11, 2025 -->

<p>{{ today | date:'shortTime' }}</p>
<!-- Output: 3:45 PM -->

<!-- Custom formats -->
<p>{{ today | date:'yyyy-MM-dd' }}</p>
<!-- Output: 2025-10-11 -->

<p>{{ today | date:'dd/MM/yyyy' }}</p>
<!-- Output: 11/10/2025 -->

<p>{{ today | date:'MMM d, y, h:mm a' }}</p>
<!-- Output: Oct 11, 2025, 3:45 PM -->

<p>{{ today | date:'EEEE, MMMM d, y' }}</p>
<!-- Output: Saturday, October 11, 2025 -->

<!-- Timezone -->
<p>{{ today | date:'short':'UTC' }}</p>
<p>{{ today | date:'short':'IST' }}</p>
```

### 4. JSON Pipe

```typescript
export class AppComponent {
  user = {
    id: 1,
    name: 'John Doe',
    email: 'john@example.com',
    roles: ['admin', 'user']
  };
}
```

```html
<!-- JSONPipe - for debugging -->
<pre>{{ user | json }}</pre>
<!-- Output:
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "roles": ["admin", "user"]
}
-->
```

### 5. Array Pipes

```typescript
export class AppComponent {
  numbers = [1, 2, 3, 4, 5];
  users = [
    { name: 'John', age: 30 },
    { name: 'Jane', age: 25 },
    { name: 'Bob', age: 35 }
  ];
}
```

```html
<!-- SlicePipe with arrays -->
<ul>
  <li *ngFor="let num of numbers | slice:0:3">{{ num }}</li>
</ul>
<!-- Output: 1, 2, 3 -->

<!-- KeyValuePipe -->
<div *ngFor="let item of user | keyvalue">
  <b>{{ item.key }}:</b> {{ item.value }}
</div>
```

### 6. Async Pipe

```typescript
import { Observable, of, interval } from 'rxjs';
import { map } from 'rxjs/operators';

export class AppComponent {
  // Observable
  user$ = of({ name: 'John', email: 'john@example.com' });
  
  // Promise
  dataPromise = this.loadData();
  
  // Time observable
  time$ = interval(1000).pipe(
    map(() => new Date())
  );
  
  async loadData() {
    return { status: 'loaded' };
  }
}
```

```html
<!-- AsyncPipe - automatically subscribes and unsubscribes -->
<div *ngIf="user$ | async as user">
  <p>{{ user.name }}</p>
  <p>{{ user.email }}</p>
</div>

<p>{{ dataPromise | async | json }}</p>

<p>Current time: {{ time$ | async | date:'medium' }}</p>
```

---

## 🔗 Chaining Pipes

```html
<!-- Chain multiple pipes -->
<p>{{ today | date:'fullDate' | uppercase }}</p>
<!-- Output: SATURDAY, OCTOBER 11, 2025 -->

<p>{{ price | currency:'USD':'symbol':'1.0-0' | lowercase }}</p>
<!-- Output: $1,235 -->

<p>{{ 'hello world' | titlecase | slice:0:5 }}</p>
<!-- Output: Hello -->

<!-- Complex chaining -->
<p>{{ user$ | async | json | slice:0:50 }}</p>
```

---

## 🎨 Creating Custom Pipes

### 1. Simple Custom Pipe - ExponentialStrength

```bash
ng generate pipe pipes/exponential-strength
```

```typescript
// exponential-strength.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'exponentialStrength'
})
export class ExponentialStrengthPipe implements PipeTransform {
  transform(value: number, exponent: number = 1): number {
    return Math.pow(value, exponent);
  }
}
```

**Usage:**
```html
<p>{{ 2 | exponentialStrength:3 }}</p>
<!-- Output: 8 (2^3) -->

<p>{{ 5 | exponentialStrength:2 }}</p>
<!-- Output: 25 (5^2) -->
```

### 2. Filter Pipe

```bash
ng generate pipe pipes/filter
```

```typescript
// filter.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filter'
})
export class FilterPipe implements PipeTransform {
  transform(items: any[], searchText: string, property?: string): any[] {
    if (!items) return [];
    if (!searchText) return items;
    
    searchText = searchText.toLowerCase();
    
    return items.filter(item => {
      if (property) {
        return item[property].toLowerCase().includes(searchText);
      }
      return JSON.stringify(item).toLowerCase().includes(searchText);
    });
  }
}
```

**Usage:**
```typescript
export class AppComponent {
  searchTerm = '';
  users = [
    { name: 'John Doe', email: 'john@example.com' },
    { name: 'Jane Smith', email: 'jane@example.com' },
    { name: 'Bob Johnson', email: 'bob@example.com' }
  ];
}
```

```html
<input [(ngModel)]="searchTerm" placeholder="Search users...">

<ul>
  <li *ngFor="let user of users | filter:searchTerm:'name'">
    {{ user.name }} - {{ user.email }}
  </li>
</ul>
```

### 3. Time Ago Pipe

```typescript
// time-ago.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgo'
})
export class TimeAgoPipe implements PipeTransform {
  transform(value: Date | string): string {
    const now = new Date();
    const date = new Date(value);
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    
    if (seconds < 60) return 'just now';
    
    const intervals: { [key: string]: number } = {
      year: 31536000,
      month: 2592000,
      week: 604800,
      day: 86400,
      hour: 3600,
      minute: 60
    };
    
    for (const [name, secondsInInterval] of Object.entries(intervals)) {
      const interval = Math.floor(seconds / secondsInInterval);
      if (interval >= 1) {
        return interval === 1 
          ? `1 ${name} ago`
          : `${interval} ${name}s ago`;
      }
    }
    
    return 'just now';
  }
}
```

**Usage:**
```html
<p>Posted {{ post.createdAt | timeAgo }}</p>
<!-- Output: Posted 2 hours ago -->
```

### 4. Truncate Pipe

```typescript
// truncate.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'truncate'
})
export class TruncatePipe implements PipeTransform {
  transform(value: string, limit: number = 50, trail: string = '...'): string {
    if (!value) return '';
    
    return value.length > limit 
      ? value.substring(0, limit) + trail
      : value;
  }
}
```

**Usage:**
```html
<p>{{ longText | truncate:100:'...' }}</p>
<p>{{ description | truncate:50 }}</p>
```

### 5. Safe HTML Pipe

```typescript
// safe-html.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'safeHtml'
})
export class SafeHtmlPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}
  
  transform(value: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(value);
  }
}
```

**Usage:**
```html
<div [innerHTML]="htmlContent | safeHtml"></div>
```

---

## 🔄 Pure vs Impure Pipes

### Pure Pipes (Default)

- Called only when input value or reference changes
- Better performance
- Most pipes are pure

```typescript
@Pipe({
  name: 'purePipe',
  pure: true  // default
})
export class PurePipe implements PipeTransform {
  transform(value: any): any {
    console.log('Pure pipe called');
    return value;
  }
}
```

### Impure Pipes

- Called on every change detection cycle
- Use sparingly (performance impact)
- Useful for filtering/sorting arrays

```typescript
@Pipe({
  name: 'impureFilter',
  pure: false  // impure
})
export class ImpureFilterPipe implements PipeTransform {
  transform(items: any[], filter: any): any[] {
    console.log('Impure pipe called');
    if (!items || !filter) return items;
    return items.filter(item => item.name.includes(filter));
  }
}
```

---

## 🎯 Practical Exercise: Blog Post Component

```typescript
// blog-post.component.ts
export class BlogPostComponent {
  posts = [
    {
      id: 1,
      title: 'Getting Started with Angular',
      content: 'Angular is a powerful framework for building web applications. It provides a comprehensive solution with built-in features like routing, forms, HTTP client, and more.',
      author: 'john doe',
      views: 1234,
      likes: 89,
      createdAt: new Date('2025-10-09T10:30:00'),
      published: true
    },
    {
      id: 2,
      title: 'Understanding RxJS Observables',
      content: 'RxJS is a library for reactive programming using Observables. It makes it easier to compose asynchronous or callback-based code.',
      author: 'jane smith',
      views: 567890,
      likes: 432,
      createdAt: new Date('2025-10-08T14:20:00'),
      published: true
    },
    {
      id: 3,
      title: 'TypeScript Best Practices',
      content: 'TypeScript adds static typing to JavaScript, which helps catch errors early and improves code quality.',
      author: 'bob johnson',
      views: 98765,
      likes: 234,
      createdAt: new Date('2025-10-10T09:15:00'),
      published: false
    }
  ];
  
  searchTerm = '';
}
```

```html
<!-- blog-post.component.html -->
<div class="blog-container">
  <h1>Tech Blog</h1>
  
  <input 
    [(ngModel)]="searchTerm" 
    placeholder="Search posts..."
    class="search-input">
  
  <div class="posts">
    <article 
      *ngFor="let post of posts | filter:searchTerm:'title'"
      class="post-card">
      
      <div class="post-header">
        <h2>{{ post.title | titlecase }}</h2>
        <span class="status" [class.published]="post.published">
          {{ post.published ? 'Published' : 'Draft' | uppercase }}
        </span>
      </div>
      
      <div class="post-meta">
        <span>By {{ post.author | titlecase }}</span>
        <span>{{ post.createdAt | timeAgo }}</span>
      </div>
      
      <p class="post-content">
        {{ post.content | truncate:100 }}
      </p>
      
      <div class="post-stats">
        <span>👁️ {{ post.views | number }}</span>
        <span>❤️ {{ post.likes }}</span>
        <span>📅 {{ post.createdAt | date:'MMM d, y' }}</span>
      </div>
      
      <button class="read-more">Read More</button>
    </article>
  </div>
</div>
```

---

## ✅ Day 05 Checklist

- [ ] Used built-in string pipes (uppercase, lowercase, titlecase, slice)
- [ ] Applied number pipes (number, currency, percent)
- [ ] Formatted dates with date pipe
- [ ] Used async pipe with Observables
- [ ] Chained multiple pipes together
- [ ] Created custom exponentialStrength pipe
- [ ] Created filter pipe for searching
- [ ] Created timeAgo pipe
- [ ] Created truncate pipe
- [ ] Understood pure vs impure pipes
- [ ] Built blog post component with multiple pipes

---

## 🔑 Key Takeaways

1. **Pipes transform data** in templates without changing the source
2. **Chain pipes** for complex transformations
3. **Use async pipe** for Observables (automatic subscription management)
4. **Custom pipes** encapsulate reusable transformation logic
5. **Pure pipes** are more performant (default)
6. **Impure pipes** run on every change detection (use sparingly)
7. **Date and currency pipes** handle localization automatically

---

## 📚 Additional Resources

- [Angular Pipes Guide](https://angular.io/guide/pipes)
- [Pipe Precedence](https://angular.io/guide/pipes-precedence)
- [Custom Pipes](https://angular.io/guide/pipes-custom-data-trans)

---

## 🎯 Next Steps

Tomorrow (Day 06), we'll explore:
- **Services and Dependency Injection**
- Creating services
- Singleton vs multiple instances
- Providing services
- Service communication

Excellent progress! You can now transform data beautifully! 🎨
