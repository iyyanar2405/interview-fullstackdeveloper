# Day 04: Angular Directives 🎯

## 📋 Learning Objectives

By the end of this lesson, you will be able to:
- Understand the three types of directives in Angular
- Master structural directives (ngIf, ngFor, ngSwitch)
- Use attribute directives (ngClass, ngStyle)
- Create custom directives
- Apply directives to build dynamic UIs
- Implement advanced directive patterns

---

## 🎯 What are Directives?

Directives are classes that add additional behavior to elements in your Angular applications.

### Three Types of Directives

1. **Component Directives** - Components with templates
2. **Structural Directives** - Change DOM structure (*ngIf, *ngFor, *ngSwitch)
3. **Attribute Directives** - Change appearance or behavior (ngClass, ngStyle)

---

## 🏗️ Structural Directives

Structural directives change the DOM layout by adding, removing, or manipulating elements.

### 1. *ngIf - Conditional Rendering

```html
<!-- Basic ngIf -->
<div *ngIf="isLoggedIn">
  <h2>Welcome back!</h2>
</div>

<!-- With else -->
<div *ngIf="isLoggedIn; else loggedOut">
  <h2>Welcome back!</h2>
</div>
<ng-template #loggedOut>
  <h2>Please log in</h2>
</ng-template>

<!-- With then and else -->
<div *ngIf="isLoggedIn; then loggedInBlock else loggedOutBlock"></div>
<ng-template #loggedInBlock>
  <h2>Welcome back!</h2>
</ng-template>
<ng-template #loggedOutBlock>
  <h2>Please log in</h2>
</ng-template>

<!-- With as (aliasing) -->
<div *ngIf="user$ | async as user">
  <p>Hello, {{ user.name }}!</p>
</div>

<!-- Multiple conditions -->
<div *ngIf="isLoggedIn && user.role === 'admin'">
  <h2>Admin Panel</h2>
</div>
```

```typescript
export class AppComponent {
  isLoggedIn = false;
  user = { name: 'John', role: 'admin' };
  user$ = of({ name: 'John', email: 'john@example.com' });
}
```

### 2. *ngFor - Lists and Iteration

```html
<!-- Basic ngFor -->
<ul>
  <li *ngFor="let item of items">
    {{ item }}
  </li>
</ul>

<!-- With index -->
<ul>
  <li *ngFor="let item of items; let i = index">
    {{ i + 1 }}. {{ item }}
  </li>
</ul>

<!-- With first, last, even, odd -->
<ul>
  <li *ngFor="let item of items; let first = first; let last = last; let even = even"
      [class.first]="first"
      [class.last]="last"
      [class.even]="even">
    {{ item }}
  </li>
</ul>

<!-- All available variables -->
<div *ngFor="let user of users;
             let i = index;
             let first = first;
             let last = last;
             let even = even;
             let odd = odd;
             let count = count">
  <p>
    #{{ i }} - {{ user.name }}
    <span *ngIf="first">(First)</span>
    <span *ngIf="last">(Last)</span>
    Total: {{ count }}
  </p>
</div>

<!-- With trackBy for performance -->
<div *ngFor="let user of users; trackBy: trackByUserId">
  {{ user.name }}
</div>
```

```typescript
export class AppComponent {
  items = ['Apple', 'Banana', 'Orange'];
  
  users = [
    { id: 1, name: 'John' },
    { id: 2, name: 'Jane' },
    { id: 3, name: 'Bob' }
  ];
  
  // TrackBy function for performance
  trackByUserId(index: number, user: any): number {
    return user.id;
  }
}
```

### 3. *ngSwitch - Multiple Conditions

```html
<!-- ngSwitch -->
<div [ngSwitch]="userRole">
  <div *ngSwitchCase="'admin'">
    <h2>Admin Dashboard</h2>
    <p>Full access granted</p>
  </div>
  
  <div *ngSwitchCase="'editor'">
    <h2>Editor Panel</h2>
    <p>Content editing access</p>
  </div>
  
  <div *ngSwitchCase="'viewer'">
    <h2>Viewer Mode</h2>
    <p>Read-only access</p>
  </div>
  
  <div *ngSwitchDefault>
    <h2>Guest</h2>
    <p>Limited access</p>
  </div>
</div>

<!-- With complex conditions -->
<div [ngSwitch]="status">
  <div *ngSwitchCase="'loading'">
    <p>Loading...</p>
  </div>
  <div *ngSwitchCase="'success'">
    <p>Data loaded successfully!</p>
  </div>
  <div *ngSwitchCase="'error'">
    <p>Error loading data</p>
  </div>
  <div *ngSwitchDefault>
    <p>No data</p>
  </div>
</div>
```

```typescript
export class AppComponent {
  userRole = 'admin'; // 'admin', 'editor', 'viewer', or 'guest'
  status = 'loading'; // 'loading', 'success', 'error'
}
```

---

## 🎨 Attribute Directives

Attribute directives change the appearance or behavior of elements.

### 1. ngClass - Dynamic CSS Classes

```html
<!-- Single class -->
<div [ngClass]="'active'">Content</div>

<!-- Conditional class -->
<div [ngClass]="isActive ? 'active' : 'inactive'">Content</div>

<!-- Object syntax -->
<div [ngClass]="{
  'active': isActive,
  'disabled': isDisabled,
  'highlighted': isHighlighted
}">Content</div>

<!-- Array syntax -->
<div [ngClass]="['base-class', 'another-class', dynamicClass]">Content</div>

<!-- Method returning class -->
<div [ngClass]="getClasses()">Content</div>

<!-- Multiple conditions -->
<div [ngClass]="{
  'text-success': status === 'success',
  'text-error': status === 'error',
  'text-warning': status === 'warning',
  'font-bold': isImportant,
  'text-large': isLarge
}">
  {{ message }}
</div>
```

```typescript
export class AppComponent {
  isActive = true;
  isDisabled = false;
  isHighlighted = true;
  dynamicClass = 'special';
  status = 'success';
  isImportant = true;
  isLarge = false;
  
  getClasses() {
    return {
      'active': this.isActive,
      'disabled': this.isDisabled
    };
  }
}
```

### 2. ngStyle - Dynamic Inline Styles

```html
<!-- Object syntax -->
<div [ngStyle]="{
  'color': textColor,
  'font-size': fontSize + 'px',
  'background-color': bgColor
}">Styled Content</div>

<!-- Conditional styles -->
<div [ngStyle]="{
  'color': isError ? 'red' : 'green',
  'font-weight': isImportant ? 'bold' : 'normal'
}">Content</div>

<!-- Method returning styles -->
<div [ngStyle]="getStyles()">Content</div>

<!-- Complex example -->
<div [ngStyle]="{
  'width': width + '%',
  'height': height + 'px',
  'transform': 'rotate(' + rotation + 'deg)',
  'opacity': opacity,
  'border': '2px solid ' + borderColor
}">
  Animated Content
</div>
```

```typescript
export class AppComponent {
  textColor = 'blue';
  fontSize = 16;
  bgColor = '#f0f0f0';
  isError = false;
  isImportant = true;
  
  width = 50;
  height = 200;
  rotation = 45;
  opacity = 0.8;
  borderColor = '#333';
  
  getStyles() {
    return {
      'color': this.textColor,
      'font-size': this.fontSize + 'px'
    };
  }
}
```

### 3. Other Built-in Attribute Directives

```html
<!-- ngModel - Two-way data binding -->
<input [(ngModel)]="username" placeholder="Enter username">

<!-- ngModelOptions -->
<input 
  [(ngModel)]="searchTerm"
  [ngModelOptions]="{ updateOn: 'blur' }">

<!-- Multiple directives -->
<div 
  *ngIf="isVisible"
  [ngClass]="{'active': isActive}"
  [ngStyle]="{'color': textColor}">
  Combined Directives
</div>
```

---

## 🔧 Creating Custom Directives

### 1. Attribute Directive - Highlight

```bash
ng generate directive directives/highlight
```

```typescript
// highlight.directive.ts
import { Directive, ElementRef, HostListener, Input, OnInit } from '@angular/core';

@Directive({
  selector: '[appHighlight]'
})
export class HighlightDirective implements OnInit {
  @Input() appHighlight = '';
  @Input() defaultColor = 'yellow';
  
  constructor(private el: ElementRef) {}
  
  ngOnInit() {
    this.el.nativeElement.style.backgroundColor = this.defaultColor;
  }
  
  @HostListener('mouseenter') onMouseEnter() {
    this.highlight(this.appHighlight || this.defaultColor);
  }
  
  @HostListener('mouseleave') onMouseLeave() {
    this.highlight('');
  }
  
  private highlight(color: string) {
    this.el.nativeElement.style.backgroundColor = color;
  }
}
```

**Usage:**
```html
<!-- Default yellow -->
<p appHighlight>Hover over me!</p>

<!-- Custom color -->
<p [appHighlight]="'lightblue'">Hover for blue!</p>

<!-- With default -->
<p appHighlight [defaultColor]="'lightgreen'">Hover for color!</p>
```

### 2. Structural Directive - Unless

```bash
ng generate directive directives/unless
```

```typescript
// unless.directive.ts
import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[appUnless]'
})
export class UnlessDirective {
  private hasView = false;
  
  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}
  
  @Input() set appUnless(condition: boolean) {
    if (!condition && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (condition && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
```

**Usage:**
```html
<!-- Shows content when condition is FALSE -->
<div *appUnless="isLoggedIn">
  <p>Please log in to continue</p>
</div>
```

### 3. Advanced Custom Directive - Permission

```typescript
// permission.directive.ts
import { Directive, Input, TemplateRef, ViewContainerRef, OnInit } from '@angular/core';

@Directive({
  selector: '[appPermission]'
})
export class PermissionDirective implements OnInit {
  @Input() appPermission: string[] = [];
  
  private userPermissions = ['read', 'write']; // From auth service
  
  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}
  
  ngOnInit() {
    const hasPermission = this.appPermission.some(
      permission => this.userPermissions.includes(permission)
    );
    
    if (hasPermission) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }
}
```

**Usage:**
```html
<!-- Show only if user has permission -->
<button *appPermission="['admin', 'write']">
  Delete
</button>

<div *appPermission="['read']">
  <p>Content visible to readers</p>
</div>
```

---

## 🎯 Practical Exercise 1: Task List with Directives

```typescript
// app.component.ts
export class AppComponent {
  tasks: Task[] = [
    { id: 1, title: 'Learn Angular', completed: true, priority: 'high' },
    { id: 2, title: 'Build an app', completed: false, priority: 'medium' },
    { id: 3, title: 'Deploy to production', completed: false, priority: 'high' }
  ];
  
  filter: 'all' | 'completed' | 'active' = 'all';
  newTaskTitle = '';
  
  get filteredTasks() {
    switch (this.filter) {
      case 'completed':
        return this.tasks.filter(t => t.completed);
      case 'active':
        return this.tasks.filter(t => !t.completed);
      default:
        return this.tasks;
    }
  }
  
  addTask() {
    if (this.newTaskTitle.trim()) {
      this.tasks.push({
        id: Date.now(),
        title: this.newTaskTitle,
        completed: false,
        priority: 'medium'
      });
      this.newTaskTitle = '';
    }
  }
  
  toggleTask(task: Task) {
    task.completed = !task.completed;
  }
  
  deleteTask(id: number) {
    this.tasks = this.tasks.filter(t => t.id !== id);
  }
  
  trackByTaskId(index: number, task: Task): number {
    return task.id;
  }
}

interface Task {
  id: number;
  title: string;
  completed: boolean;
  priority: 'low' | 'medium' | 'high';
}
```

```html
<!-- app.component.html -->
<div class="task-app">
  <h1>Task Manager</h1>
  
  <!-- Add Task Form -->
  <div class="add-task">
    <input 
      [(ngModel)]="newTaskTitle"
      (keyup.enter)="addTask()"
      placeholder="Add new task..."
      class="task-input">
    <button (click)="addTask()" class="btn-add">Add</button>
  </div>
  
  <!-- Filter Buttons -->
  <div class="filters">
    <button 
      *ngFor="let f of ['all', 'active', 'completed']"
      (click)="filter = f"
      [ngClass]="{'active': filter === f}"
      class="btn-filter">
      {{ f | titlecase }}
    </button>
  </div>
  
  <!-- Task List -->
  <div class="task-list">
    <div 
      *ngIf="filteredTasks.length === 0"
      class="no-tasks">
      No tasks found
    </div>
    
    <div 
      *ngFor="let task of filteredTasks; trackBy: trackByTaskId"
      class="task-item"
      [ngClass]="{
        'completed': task.completed,
        'priority-high': task.priority === 'high',
        'priority-medium': task.priority === 'medium',
        'priority-low': task.priority === 'low'
      }">
      
      <input 
        type="checkbox"
        [checked]="task.completed"
        (change)="toggleTask(task)">
      
      <span 
        class="task-title"
        [ngStyle]="{
          'text-decoration': task.completed ? 'line-through' : 'none',
          'opacity': task.completed ? 0.6 : 1
        }">
        {{ task.title }}
      </span>
      
      <span class="priority-badge">
        {{ task.priority }}
      </span>
      
      <button 
        (click)="deleteTask(task.id)"
        class="btn-delete">
        Delete
      </button>
    </div>
  </div>
  
  <!-- Summary -->
  <div class="summary">
    <span>Total: {{ tasks.length }}</span>
    <span>Active: {{ tasks.filter(t => !t.completed).length }}</span>
    <span>Completed: {{ tasks.filter(t => t.completed).length }}</span>
  </div>
</div>
```

```css
/* app.component.css */
.task-app {
  max-width: 600px;
  margin: 40px auto;
  padding: 20px;
}

.add-task {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.task-input {
  flex: 1;
  padding: 10px;
  border: 2px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

.btn-add {
  padding: 10px 20px;
  background: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.filters {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.btn-filter {
  padding: 8px 16px;
  border: 2px solid #2196f3;
  background: white;
  color: #2196f3;
  border-radius: 4px;
  cursor: pointer;
}

.btn-filter.active {
  background: #2196f3;
  color: white;
}

.task-list {
  border: 1px solid #ddd;
  border-radius: 4px;
  overflow: hidden;
}

.task-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 15px;
  border-bottom: 1px solid #eee;
  transition: background 0.2s;
}

.task-item:hover {
  background: #f5f5f5;
}

.task-item.completed {
  background: #f0f0f0;
}

.task-item.priority-high {
  border-left: 4px solid #f44336;
}

.task-item.priority-medium {
  border-left: 4px solid #ff9800;
}

.task-item.priority-low {
  border-left: 4px solid #4caf50;
}

.task-title {
  flex: 1;
}

.priority-badge {
  padding: 4px 8px;
  background: #e0e0e0;
  border-radius: 12px;
  font-size: 12px;
}

.btn-delete {
  padding: 6px 12px;
  background: #f44336;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.no-tasks {
  padding: 40px;
  text-align: center;
  color: #999;
}

.summary {
  display: flex;
  justify-content: space-around;
  margin-top: 20px;
  padding: 15px;
  background: #f5f5f5;
  border-radius: 4px;
}
```

---

## ✅ Day 04 Checklist

- [ ] Understood three types of directives
- [ ] Mastered *ngIf with else and as
- [ ] Used *ngFor with index, first, last, even, odd
- [ ] Implemented trackBy for performance
- [ ] Applied *ngSwitch for multiple conditions
- [ ] Used ngClass for dynamic CSS classes
- [ ] Applied ngStyle for dynamic inline styles
- [ ] Created custom attribute directive (Highlight)
- [ ] Created custom structural directive (Unless)
- [ ] Built task manager with directives (Exercise)

---

## 🔑 Key Takeaways

1. **Structural directives** change the DOM structure (add/remove elements)
2. **Always use trackBy** with *ngFor for better performance
3. **ngClass and ngStyle** provide dynamic styling capabilities
4. **Custom directives** encapsulate reusable DOM manipulation logic
5. **HostListener** responds to events on the host element
6. **TemplateRef and ViewContainerRef** are used for structural directives

---

## 📚 Additional Resources

- [Angular Directives Guide](https://angular.io/guide/attribute-directives)
- [Structural Directives](https://angular.io/guide/structural-directives)
- [Built-in Directives](https://angular.io/guide/built-in-directives)

---

## 🎯 Next Steps

Tomorrow (Day 05), we'll explore:
- **Pipes** for data transformation
- Built-in pipes (date, currency, uppercase, etc.)
- Creating custom pipes
- Pure vs Impure pipes

Excellent work on Day 04! You can now build dynamic, data-driven UIs! 🎉
