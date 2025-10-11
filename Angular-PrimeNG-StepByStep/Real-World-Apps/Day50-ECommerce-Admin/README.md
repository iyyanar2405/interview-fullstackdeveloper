# Day 50 — E‑Commerce: Admin Panel

Objectives
- Build an admin panel for products and orders using PrimeNG Table + Dialog patterns.
- Add file upload for product images and use Chart for sales overview.
- Secure routes with a simple guard hook.

Products admin page
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { FileUploadModule } from 'primeng/fileupload';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-products-admin-page',
  standalone: true,
  imports: [CommonModule, TableModule, DialogModule, ButtonModule, InputTextModule, InputNumberModule, FileUploadModule, ReactiveFormsModule],
  templateUrl: './products-admin.page.html'
})
export class ProductsAdminPage {
  products: any[] = [];
  visible = false;
  form = this.fb.group({ id:[null], name:['', Validators.required], price:[0, Validators.required] });

  constructor(private fb: FormBuilder){}
  new(){ this.form.reset({id:null, name:'', price:0}); this.visible = true; }
  edit(p:any){ this.form.reset(p); this.visible = true; }
  save(){ if(this.form.invalid){ this.form.markAllAsTouched(); return;} /* save */ this.visible = false; }
}
```

Template (snippet)
```html
<div class="p-d-flex p-jc-between p-ai-center p-mb-2">
  <h2>Products</h2>
  <button pButton label="New" icon="pi pi-plus" (click)="new()"></button>
</div>
<p-table [value]="products" [paginator]="true" [rows]="10">
  <ng-template pTemplate="header"><tr><th>Name</th><th>Price</th><th></th></tr></ng-template>
  <ng-template pTemplate="body" let-row>
    <tr>
      <td>{{row.name}}</td>
      <td>{{row.price | currency}}</td>
      <td>
        <button pButton icon="pi pi-pencil" class="p-button-text" (click)="edit(row)"></button>
      </td>
    </tr>
  </ng-template>
</p-table>

<p-dialog [(visible)]="visible" header="Product" [modal]="true">
  <form [formGroup]="form" (ngSubmit)="save()" class="p-fluid">
    <div class="p-field"><label>Name</label><input pInputText formControlName="name"></div>
    <div class="p-field"><label>Price</label><p-inputNumber formControlName="price" [mode]="'currency'" currency="USD"></p-inputNumber></div>
    <div class="p-field"><label>Image</label><p-fileUpload mode="basic" auto="true"></p-fileUpload></div>
    <p-footer><button pButton type="submit" label="Save" icon="pi pi-check"></button></p-footer>
  </form>
</p-dialog>
```

Exercises
1) Add orders table with status tags and date filters.
2) Add sales chart with date range filters.
3) Add role-based access guard for admin routes.
