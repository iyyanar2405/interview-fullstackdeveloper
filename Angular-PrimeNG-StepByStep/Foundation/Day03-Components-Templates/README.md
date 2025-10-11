# Day 03: Angular Components & Templates 🎨

## 📋 Learning Objectives

By the end of this lesson, you will be able to:
- Create and structure Angular components
- Understand component lifecycle hooks
- Master template syntax and data binding
- Implement component communication (parent-child)
- Use @Input() and @Output() decorators
- Apply best practices for component design

---

## 🎯 What is an Angular Component?

A component is the fundamental building block of Angular applications. It consists of:

1. **TypeScript Class** - Logic and data
2. **HTML Template** - View/UI
3. **CSS Styles** - Styling (optional)
4. **Metadata** - Configuration via @Component decorator

### Component Anatomy

```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-hello',           // How to use it in HTML
  templateUrl: './hello.component.html',  // Template file
  styleUrls: ['./hello.component.css']    // Style file
})
export class HelloComponent {
  // Component logic here
  title = 'Hello Angular';
  
  greet() {
    console.log('Hello!');
  }
}
```

---

## 🔨 Creating Components

### Method 1: Using Angular CLI (Recommended)

```bash
# Create a component
ng generate component components/user-profile

# Or shorthand
ng g c components/user-profile

# Create without test file
ng g c components/user-profile --skip-tests

# Create inline template and styles
ng g c components/user-profile --inline-template --inline-style
```

### Method 2: Manual Creation

Create three files:
- `user-profile.component.ts`
- `user-profile.component.html`
- `user-profile.component.css`

---

## 🎨 Template Syntax

### 1. Interpolation

```html
<!-- user-profile.component.html -->
<h1>{{ title }}</h1>
<p>Welcome, {{ firstName }} {{ lastName }}!</p>
<p>Age: {{ age }}</p>
<p>2 + 2 = {{ 2 + 2 }}</p>
<p>Birthday: {{ getBirthday() }}</p>
```

```typescript
// user-profile.component.ts
export class UserProfileComponent {
  title = 'User Profile';
  firstName = 'John';
  lastName = 'Doe';
  age = 30;
  
  getBirthday(): string {
    return '1994-01-15';
  }
}
```

### 2. Property Binding

```html
<!-- Bind to element properties -->
<img [src]="imageUrl" [alt]="imageAlt">
<button [disabled]="isDisabled">Click Me</button>
<div [class.active]="isActive">Content</div>
<p [style.color]="textColor">Colored Text</p>

<!-- Attribute binding -->
<button [attr.aria-label]="buttonLabel">Submit</button>
<td [attr.colspan]="columnSpan">Data</td>
```

```typescript
export class UserProfileComponent {
  imageUrl = 'assets/profile.jpg';
  imageAlt = 'Profile picture';
  isDisabled = false;
  isActive = true;
  textColor = 'blue';
  buttonLabel = 'Submit Form';
  columnSpan = 2;
}
```

### 3. Event Binding

```html
<!-- Click event -->
<button (click)="handleClick()">Click Me</button>

<!-- With $event -->
<button (click)="handleClickWithEvent($event)">Click</button>

<!-- Input event -->
<input (input)="onInput($event)" placeholder="Type something">

<!-- Mouse events -->
<div (mouseenter)="onMouseEnter()" (mouseleave)="onMouseLeave()">
  Hover over me
</div>

<!-- Form events -->
<form (submit)="onSubmit($event)">
  <input type="text" [(ngModel)]="username">
  <button type="submit">Submit</button>
</form>
```

```typescript
export class UserProfileComponent {
  handleClick() {
    console.log('Button clicked!');
  }
  
  handleClickWithEvent(event: MouseEvent) {
    console.log('Event:', event);
    console.log('Target:', event.target);
  }
  
  onInput(event: Event) {
    const target = event.target as HTMLInputElement;
    console.log('Value:', target.value);
  }
  
  onMouseEnter() {
    console.log('Mouse entered');
  }
  
  onMouseLeave() {
    console.log('Mouse left');
  }
  
  username = '';
  
  onSubmit(event: Event) {
    event.preventDefault();
    console.log('Form submitted:', this.username);
  }
}
```

### 4. Two-Way Binding

```html
<!-- Requires FormsModule in imports -->
<input [(ngModel)]="username" placeholder="Enter username">
<p>You entered: {{ username }}</p>

<!-- Expanded form (how it works internally) -->
<input 
  [ngModel]="username" 
  (ngModelChange)="username = $event"
  placeholder="Enter username">
```

```typescript
// In component
import { FormsModule } from '@angular/forms';

// In app.module.ts or standalone component
imports: [FormsModule]

// Component class
export class UserProfileComponent {
  username = '';
}
```

---

## 🔄 Component Lifecycle Hooks

Angular calls lifecycle hooks in the following order:

```typescript
import { Component, OnInit, OnChanges, OnDestroy, AfterViewInit } from '@angular/core';

export class LifecycleComponent implements OnInit, OnChanges, OnDestroy, AfterViewInit {
  
  // 1. Called when input properties change
  ngOnChanges(changes: SimpleChanges) {
    console.log('OnChanges', changes);
  }
  
  // 2. Called once after first ngOnChanges
  ngOnInit() {
    console.log('OnInit - Component initialized');
    // Initialization logic here
    // API calls, subscriptions setup
  }
  
  // 3. Called after Angular checks component's content
  ngDoCheck() {
    console.log('DoCheck');
  }
  
  // 4. Called after content (ng-content) is initialized
  ngAfterContentInit() {
    console.log('AfterContentInit');
  }
  
  // 5. Called after content is checked
  ngAfterContentChecked() {
    console.log('AfterContentChecked');
  }
  
  // 6. Called after component's view is initialized
  ngAfterViewInit() {
    console.log('AfterViewInit - View initialized');
    // DOM manipulation here
  }
  
  // 7. Called after view is checked
  ngAfterViewChecked() {
    console.log('AfterViewChecked');
  }
  
  // 8. Called just before Angular destroys the component
  ngOnDestroy() {
    console.log('OnDestroy - Cleanup');
    // Unsubscribe from observables
    // Cleanup resources
  }
}
```

### Most Common Hooks

#### ngOnInit() - Initialization
```typescript
export class UserComponent implements OnInit {
  users: User[] = [];
  
  ngOnInit() {
    // Perfect place for:
    // - Initial data loading
    // - Setting up subscriptions
    // - API calls
    this.loadUsers();
  }
  
  loadUsers() {
    // Fetch users from API
  }
}
```

#### ngOnDestroy() - Cleanup
```typescript
export class UserComponent implements OnInit, OnDestroy {
  private subscription: Subscription;
  
  ngOnInit() {
    this.subscription = this.userService.getUsers()
      .subscribe(users => this.users = users);
  }
  
  ngOnDestroy() {
    // Prevent memory leaks
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
```

---

## 👨‍👧 Component Communication

### 1. Parent to Child - @Input()

**Parent Component:**
```typescript
// parent.component.ts
export class ParentComponent {
  userName = 'John Doe';
  userAge = 30;
}
```

```html
<!-- parent.component.html -->
<app-child [name]="userName" [age]="userAge"></app-child>
```

**Child Component:**
```typescript
// child.component.ts
import { Component, Input } from '@angular/core';

export class ChildComponent {
  @Input() name!: string;      // Receive from parent
  @Input() age!: number;        // Receive from parent
  
  // With alias
  @Input('userName') displayName!: string;
  
  // With default value
  @Input() country: string = 'USA';
}
```

```html
<!-- child.component.html -->
<h2>Child Component</h2>
<p>Name: {{ name }}</p>
<p>Age: {{ age }}</p>
```

### 2. Child to Parent - @Output()

**Child Component:**
```typescript
// child.component.ts
import { Component, Output, EventEmitter } from '@angular/core';

export class ChildComponent {
  @Output() messageEvent = new EventEmitter<string>();
  @Output() dataEvent = new EventEmitter<any>();
  
  sendMessage() {
    this.messageEvent.emit('Hello from child!');
  }
  
  sendData() {
    const data = { id: 1, name: 'John' };
    this.dataEvent.emit(data);
  }
}
```

```html
<!-- child.component.html -->
<button (click)="sendMessage()">Send Message</button>
<button (click)="sendData()">Send Data</button>
```

**Parent Component:**
```typescript
// parent.component.ts
export class ParentComponent {
  receivedMessage = '';
  receivedData: any;
  
  handleMessage(message: string) {
    this.receivedMessage = message;
    console.log('Message from child:', message);
  }
  
  handleData(data: any) {
    this.receivedData = data;
    console.log('Data from child:', data);
  }
}
```

```html
<!-- parent.component.html -->
<app-child 
  (messageEvent)="handleMessage($event)"
  (dataEvent)="handleData($event)">
</app-child>

<p>Received: {{ receivedMessage }}</p>
<p>Data: {{ receivedData | json }}</p>
```

### 3. Template Reference Variables

```html
<!-- Get reference to child component -->
<app-child #childRef></app-child>
<button (click)="childRef.someMethod()">Call Child Method</button>

<!-- Get reference to element -->
<input #nameInput type="text">
<button (click)="logValue(nameInput.value)">Log Value</button>
```

---

## 🎯 Practical Exercise 1: User Card Component

Create a reusable user card component that displays user information.

### Step 1: Generate Component

```bash
ng g c components/user-card --skip-tests
```

### Step 2: Define Component

```typescript
// user-card.component.ts
import { Component, Input, Output, EventEmitter } from '@angular/core';

export interface User {
  id: number;
  name: string;
  email: string;
  avatar: string;
  role: string;
}

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.css']
})
export class UserCardComponent {
  @Input() user!: User;
  @Output() viewProfile = new EventEmitter<number>();
  @Output() deleteUser = new EventEmitter<number>();
  
  isHovered = false;
  
  onViewProfile() {
    this.viewProfile.emit(this.user.id);
  }
  
  onDelete() {
    if (confirm(`Delete ${this.user.name}?`)) {
      this.deleteUser.emit(this.user.id);
    }
  }
  
  onMouseEnter() {
    this.isHovered = true;
  }
  
  onMouseLeave() {
    this.isHovered = false;
  }
}
```

### Step 3: Create Template

```html
<!-- user-card.component.html -->
<div class="user-card" 
     [class.hovered]="isHovered"
     (mouseenter)="onMouseEnter()"
     (mouseleave)="onMouseLeave()">
  
  <div class="user-avatar">
    <img [src]="user.avatar" [alt]="user.name">
  </div>
  
  <div class="user-info">
    <h3>{{ user.name }}</h3>
    <p class="email">{{ user.email }}</p>
    <span class="role" [class]="'role-' + user.role.toLowerCase()">
      {{ user.role }}
    </span>
  </div>
  
  <div class="user-actions">
    <button (click)="onViewProfile()" class="btn-view">
      View Profile
    </button>
    <button (click)="onDelete()" class="btn-delete">
      Delete
    </button>
  </div>
</div>
```

### Step 4: Add Styles

```css
/* user-card.component.css */
.user-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin: 10px;
  transition: all 0.3s ease;
  background: white;
}

.user-card.hovered {
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
  transform: translateY(-2px);
}

.user-avatar img {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  object-fit: cover;
}

.user-info h3 {
  margin: 10px 0 5px;
  color: #333;
}

.email {
  color: #666;
  font-size: 14px;
}

.role {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  margin-top: 8px;
}

.role-admin {
  background: #e3f2fd;
  color: #1976d2;
}

.role-user {
  background: #f3e5f5;
  color: #7b1fa2;
}

.user-actions {
  margin-top: 15px;
  display: flex;
  gap: 10px;
}

.user-actions button {
  flex: 1;
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-view {
  background: #4caf50;
  color: white;
}

.btn-view:hover {
  background: #45a049;
}

.btn-delete {
  background: #f44336;
  color: white;
}

.btn-delete:hover {
  background: #da190b;
}
```

### Step 5: Use in Parent Component

```typescript
// app.component.ts
export class AppComponent {
  users: User[] = [
    {
      id: 1,
      name: 'John Doe',
      email: 'john@example.com',
      avatar: 'https://i.pravatar.cc/150?img=1',
      role: 'Admin'
    },
    {
      id: 2,
      name: 'Jane Smith',
      email: 'jane@example.com',
      avatar: 'https://i.pravatar.cc/150?img=2',
      role: 'User'
    },
    {
      id: 3,
      name: 'Bob Johnson',
      email: 'bob@example.com',
      avatar: 'https://i.pravatar.cc/150?img=3',
      role: 'User'
    }
  ];
  
  handleViewProfile(userId: number) {
    console.log('View profile for user:', userId);
    // Navigate to profile page
  }
  
  handleDeleteUser(userId: number) {
    this.users = this.users.filter(u => u.id !== userId);
    console.log('Deleted user:', userId);
  }
}
```

```html
<!-- app.component.html -->
<div class="user-list">
  <h1>User Management</h1>
  <div class="cards-container">
    <app-user-card
      *ngFor="let user of users"
      [user]="user"
      (viewProfile)="handleViewProfile($event)"
      (deleteUser)="handleDeleteUser($event)">
    </app-user-card>
  </div>
</div>
```

---

## 🎯 Practical Exercise 2: Counter Component

Create an interactive counter with parent-child communication.

```typescript
// counter.component.ts
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'app-counter',
  template: `
    <div class="counter">
      <button (click)="decrement()" [disabled]="count <= min">-</button>
      <span class="count">{{ count }}</span>
      <button (click)="increment()" [disabled]="count >= max">+</button>
      <button (click)="reset()" class="reset">Reset</button>
    </div>
  `,
  styles: [`
    .counter {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 20px;
      background: #f5f5f5;
      border-radius: 8px;
    }
    
    button {
      width: 40px;
      height: 40px;
      font-size: 20px;
      border: none;
      background: #2196f3;
      color: white;
      border-radius: 4px;
      cursor: pointer;
    }
    
    button:hover:not(:disabled) {
      background: #1976d2;
    }
    
    button:disabled {
      background: #ccc;
      cursor: not-allowed;
    }
    
    .count {
      font-size: 24px;
      font-weight: bold;
      min-width: 50px;
      text-align: center;
    }
    
    .reset {
      width: auto;
      padding: 0 20px;
      background: #ff5722;
    }
    
    .reset:hover {
      background: #f4511e;
    }
  `]
})
export class CounterComponent implements OnInit {
  @Input() initialValue = 0;
  @Input() min = 0;
  @Input() max = 100;
  @Input() step = 1;
  @Output() countChange = new EventEmitter<number>();
  
  count = 0;
  
  ngOnInit() {
    this.count = this.initialValue;
  }
  
  increment() {
    if (this.count < this.max) {
      this.count += this.step;
      this.countChange.emit(this.count);
    }
  }
  
  decrement() {
    if (this.count > this.min) {
      this.count -= this.step;
      this.countChange.emit(this.count);
    }
  }
  
  reset() {
    this.count = this.initialValue;
    this.countChange.emit(this.count);
  }
}
```

**Usage:**
```html
<app-counter 
  [initialValue]="10"
  [min]="0"
  [max]="50"
  [step]="5"
  (countChange)="onCountChange($event)">
</app-counter>

<p>Current count: {{ currentCount }}</p>
```

---

## ✅ Day 03 Checklist

- [ ] Created components using Angular CLI
- [ ] Understood component anatomy (@Component decorator)
- [ ] Practiced template syntax (interpolation, property binding, event binding)
- [ ] Implemented two-way binding with [(ngModel)]
- [ ] Learned and used lifecycle hooks (ngOnInit, ngOnDestroy)
- [ ] Implemented parent-child communication with @Input()
- [ ] Implemented child-parent communication with @Output()
- [ ] Created user card component (Exercise 1)
- [ ] Created counter component (Exercise 2)
- [ ] Tested component interactions

---

## 🔑 Key Takeaways

1. **Components are the building blocks** of Angular applications
2. **Use CLI to generate components** for consistency
3. **Template syntax provides powerful data binding** capabilities
4. **Lifecycle hooks enable precise control** over component behavior
5. **@Input() and @Output() enable component communication**
6. **Always clean up in ngOnDestroy()** to prevent memory leaks
7. **Use template reference variables** for direct element/component access
8. **Keep components focused and reusable**

---

## 📚 Additional Resources

- [Angular Components Guide](https://angular.io/guide/component-overview)
- [Lifecycle Hooks](https://angular.io/guide/lifecycle-hooks)
- [Component Interaction](https://angular.io/guide/component-interaction)
- [Template Syntax](https://angular.io/guide/template-syntax)

---

## 🎯 Next Steps

Tomorrow (Day 04), we'll dive into:
- **Directives** (Structural and Attribute)
- ngIf, ngFor, ngSwitch
- ngClass, ngStyle
- Creating custom directives

Great job completing Day 03! You now understand Angular components and can build interactive UIs! 🚀
