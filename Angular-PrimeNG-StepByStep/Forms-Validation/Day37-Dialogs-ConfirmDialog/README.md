# Day 37 — Dialog & ConfirmDialog Form Flows

Objectives
- Wrap create/edit forms in p-dialog with full validation.
- Use ConfirmDialog for destructive actions.
- Manage focus, scrolling, and accessibility.

Components
- p-dialog, p-confirmDialog, ConfirmationService
- p-toast for feedback

Setup service & module
```ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Product { id?: number; name: string; price: number }

@Injectable({ providedIn: 'root' })
export class ProductDialogService {
  private _open$ = new BehaviorSubject<{ open: boolean; product?: Product | null }>({ open: false });
  opened$ = this._open$.asObservable();

  open(product?: Product) { this._open$.next({ open: true, product: product || null }); }
  close() { this._open$.next({ open: false }); }
}
```

Component
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-product-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, ButtonModule, InputTextModule, InputNumberModule, ConfirmDialogModule, ToastModule],
  providers: [MessageService, ConfirmationService],
  templateUrl: './product-dialog.component.html'
})
export class ProductDialogComponent {
  visible = false;
  form = this.fb.group({
    id: [null],
    name: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]]
  });

  constructor(private fb: FormBuilder, private ms: MessageService, private confirm: ConfirmationService) {}

  openForCreate() { this.form.reset({ id: null, name: '', price: 0 }); this.visible = true; }
  openForEdit(p: any) { this.form.reset(p); this.visible = true; }
  close() { this.visible = false; }

  save() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    // simulate save
    this.ms.add({ severity: 'success', summary: 'Saved', detail: 'Product saved' });
    this.close();
  }

  delete() {
    this.confirm.confirm({
      message: 'Delete this product?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.ms.add({ severity: 'info', summary: 'Deleted', detail: 'Product deleted' });
        this.close();
      }
    });
  }
}
```

Template
```html
<p-toast></p-toast>
<p-confirmDialog></p-confirmDialog>

<button pButton label="New Product" icon="pi pi-plus" (click)="openForCreate()"></button>

<p-dialog [(visible)]="visible" header="Product" [modal]="true" [style]="{ width: '450px' }" [draggable]="false">
  <form [formGroup]="form" (ngSubmit)="save()" class="p-fluid">
    <div class="p-field">
      <label>Name</label>
      <input pInputText formControlName="name" />
    </div>
    <div class="p-field">
      <label>Price</label>
      <p-inputNumber formControlName="price" [min]="0" [mode]="'currency'" currency="USD"></p-inputNumber>
    </div>
    <p-footer>
      <button pButton type="button" label="Delete" icon="pi pi-trash" severity="danger" (click)="delete()" class="p-mr-2"></button>
      <button pButton type="button" label="Cancel" icon="pi pi-times" (click)="close()" class="p-button-text p-mr-2"></button>
      <button pButton type="submit" label="Save" icon="pi pi-check"></button>
    </p-footer>
  </form>
</p-dialog>
```

Accessibility & UX
- Focus first input when dialog opens (use setTimeout or Angular CDK FocusTrap).
- Restore focus to trigger button on close.
- Disable closing while submitting to prevent data loss.

Exercises
1) Add a loading state on Save with button spinner.
2) Validate unique product name via async API.
3) Add keyboard shortcuts (Esc to close, Ctrl+S to save).
