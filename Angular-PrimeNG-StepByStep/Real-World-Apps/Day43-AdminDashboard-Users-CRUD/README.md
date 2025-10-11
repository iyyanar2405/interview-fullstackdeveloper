# Day 43 — Admin Dashboard: Users Management (CRUD)

Objectives
- Build a Users page with a PrimeNG Table for listing, filtering, and pagination.
- Implement Create/Edit in Dialog and ConfirmDialog for delete.
- Validate forms and show toasts for feedback.

Data Model
```ts
export interface User { id: number; name: string; email: string; role: 'admin' | 'user' | 'manager'; active: boolean }
```

Service (mock)
```ts
import { Injectable } from '@angular/core';
import { delay, of } from 'rxjs';
import { User } from './user.model';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private users: User[] = [
    { id: 1, name: 'Alice', email: 'alice@example.com', role: 'admin', active: true },
    { id: 2, name: 'Bob', email: 'bob@example.com', role: 'user', active: false }
  ];

  list() { return of(this.users).pipe(delay(200)); }
  upsert(u: Partial<User>) {
    if (u.id) { this.users = this.users.map(x => x.id === u.id ? { ...x, ...u } as User : x); }
    else { const id = Math.max(...this.users.map(x => x.id)) + 1; this.users.push({ id, ...(u as any) }); }
    return of(true).pipe(delay(200));
  }
  delete(id: number) { this.users = this.users.filter(x => x.id !== id); return of(true).pipe(delay(200)); }
}
```

Page component
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { UsersService } from './users.service';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-users-page',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, TagModule, DialogModule, ConfirmDialogModule, ToastModule, InputTextModule, DropdownModule, InputSwitchModule, ReactiveFormsModule],
  providers: [MessageService, ConfirmationService],
  templateUrl: './users.page.html'
})
export class UsersPage {
  users: any[] = [];
  loading = false;
  visible = false;
  form = this.fb.group({ id: [null], name: ['', Validators.required], email: ['', [Validators.required, Validators.email]], role: ['user', Validators.required], active: [true] });

  roles = [
    { label: 'Admin', value: 'admin' },
    { label: 'User', value: 'user' },
    { label: 'Manager', value: 'manager' }
  ];

  constructor(private svc: UsersService, private fb: FormBuilder, private ms: MessageService, private confirm: ConfirmationService) { this.load(); }

  load() { this.loading = true; this.svc.list().subscribe(v => { this.users = v; this.loading = false; }); }
  create() { this.form.reset({ id: null, name: '', email: '', role: 'user', active: true }); this.visible = true; }
  edit(row: any) { this.form.reset(row); this.visible = true; }
  save() { if (this.form.invalid) { this.form.markAllAsTouched(); return; } this.svc.upsert(this.form.value).subscribe(() => { this.ms.add({severity:'success', summary:'Saved'}); this.visible = false; this.load(); }); }
  remove(row: any) { this.confirm.confirm({ message: 'Delete user?', header: 'Confirm', accept: () => this.svc.delete(row.id).subscribe(() => { this.ms.add({severity:'info', summary:'Deleted'}); this.load(); }) }); }
}
```

Template
```html
<p-toast></p-toast>
<p-confirmDialog></p-confirmDialog>
<div class="p-d-flex p-jc-between p-ai-center p-mb-3">
  <h2>Users</h2>
  <button pButton label="New" icon="pi pi-plus" (click)="create()"></button>
</div>
<p-table [value]="users" [paginator]="true" [rows]="10" [loading]="loading" [responsiveLayout]="'scroll'" [globalFilterFields]="['name','email','role']">
  <ng-template pTemplate="caption">
    <div class="p-d-flex p-jc-between">
      <span class="p-input-icon-left">
        <i class="pi pi-search"></i>
        <input pInputText type="text" (input)="$event.target && table.filterGlobal(($event.target as HTMLInputElement).value, 'contains')" placeholder="Search" />
      </span>
    </div>
  </ng-template>
  <ng-template pTemplate="header">
    <tr>
      <th pSortableColumn="name">Name <p-sortIcon field="name"></p-sortIcon></th>
      <th pSortableColumn="email">Email <p-sortIcon field="email"></p-sortIcon></th>
      <th>Role</th>
      <th>Active</th>
      <th>Actions</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-row>
    <tr>
      <td>{{ row.name }}</td>
      <td>{{ row.email }}</td>
      <td><p-tag [value]="row.role"></p-tag></td>
      <td><i class="pi" [ngClass]="row.active ? 'pi-check-circle text-green-500' : 'pi-times-circle text-red-500'"></i></td>
      <td>
        <button pButton icon="pi pi-pencil" class="p-button-text" (click)="edit(row)"></button>
        <button pButton icon="pi pi-trash" class="p-button-text p-button-danger" (click)="remove(row)"></button>
      </td>
    </tr>
  </ng-template>
</p-table>

<p-dialog [(visible)]="visible" header="User" [modal]="true" [style]="{ width: '450px' }">
  <form [formGroup]="form" (ngSubmit)="save()" class="p-fluid">
    <div class="p-field"><label>Name</label><input pInputText formControlName="name" /></div>
    <div class="p-field"><label>Email</label><input pInputText formControlName="email" /></div>
    <div class="p-field"><label>Role</label><p-dropdown [options]="roles" optionLabel="label" optionValue="value" formControlName="role"></p-dropdown></div>
    <div class="p-field"><label>Active</label><p-inputSwitch formControlName="active"></p-inputSwitch></div>
    <p-footer>
      <button pButton type="button" label="Cancel" class="p-button-text" (click)="visible=false"></button>
      <button pButton type="submit" label="Save" icon="pi pi-check"></button>
    </p-footer>
  </form>
</p-dialog>
```

Exercises
1) Add server-side pagination and filtering.
2) Add column filters and export to CSV.
3) Add role-based color tags and badge counts.
