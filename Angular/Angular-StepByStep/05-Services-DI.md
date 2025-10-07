# 05 — Services & DI

Provide shared logic and data access via injectable services.

## Injectable service
```ts
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private items = ['Buy milk', 'Write code'];
  list() { return [...this.items]; }
  add(name: string) { this.items = [...this.items, name]; }
}
```

## Inject in a component
```ts
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TodoService } from './todo.service';

@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [FormsModule],
  template: `
    <input [(ngModel)]="text" placeholder="Add todo"/>
    <button (click)="add()">Add</button>
    <ul><li *ngFor="let t of todos">{{t}}</li></ul>
  `
})
export class TodosComponent {
  private svc = inject(TodoService);
  todos = this.svc.list();
  text = '';
  add(){ if(this.text.trim()){ this.svc.add(this.text.trim()); this.todos = this.svc.list(); this.text=''; } }
}
```

Next: Routing with standalone APIs.
