# Day 45 — Admin Dashboard: Forms & Validation Patterns

Objectives
- Apply advanced form patterns to product/user forms in the dashboard.
- Use reusable FieldErrors and custom CVA components.
- Map server errors, show toasts, and handle confirm flows.

Patterns to implement
- Reusable FieldErrors component (from Day 40)
- Async validators (unique name/email)
- Confirm before navigation if dirty
- Notifier service for toasts (from Day 39)

Product form example
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, Validators, FormBuilder, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { of } from 'rxjs';
import { delay, map } from 'rxjs/operators';

function uniqueNameValidator(): AsyncValidatorFn {
  return (c: AbstractControl) => of(['Laptop','Phone']).pipe(delay(300), map(list => list.includes(c.value) ? { unique: true } : null));
}

@Component({
  selector: 'app-product-form-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputTextModule, InputNumberModule, DropdownModule, ButtonModule],
  templateUrl: './product-form.page.html'
})
export class ProductFormPage {
  categories = [{label:'Electronics',value:'elec'},{label:'Fashion',value:'fashion'}];
  form = this.fb.group({
    name: ['', { validators:[Validators.required, Validators.minLength(3)], asyncValidators:[uniqueNameValidator()], updateOn: 'blur' }],
    price: [0, [Validators.required, Validators.min(0)]],
    category: [null, Validators.required],
    stock: [10, [Validators.min(0)]]
  });

  constructor(private fb: FormBuilder) {}
  submit(){ if(this.form.invalid){ this.form.markAllAsTouched(); return;} console.log(this.form.value); }
}
```

Template
```html
<h2>Product Form</h2>
<form [formGroup]="form" (ngSubmit)="submit()" class="p-fluid p-formgrid p-grid">
  <div class="p-field p-col-12 p-md-6">
    <label>Name</label>
    <input pInputText formControlName="name" />
    <small class="p-error" *ngIf="form.get('name')?.hasError('unique')">Name already exists.</small>
  </div>
  <div class="p-field p-col-12 p-md-6">
    <label>Price</label>
    <p-inputNumber formControlName="price" [mode]="'currency'" currency="USD"></p-inputNumber>
  </div>
  <div class="p-field p-col-12 p-md-6">
    <label>Category</label>
    <p-dropdown [options]="categories" formControlName="category" optionLabel="label" optionValue="value"></p-dropdown>
  </div>
  <div class="p-field p-col-12 p-md-6">
    <label>Stock</label>
    <p-inputNumber formControlName="stock"></p-inputNumber>
  </div>
  <div class="p-col-12">
    <button pButton type="submit" label="Save" icon="pi pi-check"></button>
  </div>
</form>
```

Exercises
1) Add file upload for product image (Day 28 pattern).
2) Add Dialog-driven create/edit from Day 37.
3) Add form autosave with auditTime(1000) and show toast on save.
