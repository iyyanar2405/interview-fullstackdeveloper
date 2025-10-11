# Day 49 — E‑Commerce: Cart & Checkout

Objectives
- Implement shopping cart service with add/update/remove.
- Build Cart page with table, quantities, price totals.
- Create checkout form with address and payment details.

Cart service (basic)
```ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface CartItem { id:number; name:string; price:number; qty:number }

@Injectable({ providedIn: 'root' })
export class CartService {
  private _items$ = new BehaviorSubject<CartItem[]>([]);
  items$ = this._items$.asObservable();
  get items() { return this._items$.value; }

  add(item: CartItem) {
    const idx = this.items.findIndex(i => i.id === item.id);
    if (idx >= 0) this.items[idx] = { ...this.items[idx], qty: this.items[idx].qty + item.qty };
    else this.items.push(item);
    this._items$.next([...this.items]);
  }
  updateQty(id:number, qty:number){ this._items$.next(this.items.map(i => i.id===id?{...i, qty}:i)); }
  remove(id:number){ this._items$.next(this.items.filter(i => i.id!==id)); }
  clear(){ this._items$.next([]); }
}
```

Cart page
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { CartService } from './cart.service';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputNumberModule],
  templateUrl: './cart.page.html'
})
export class CartPage {
  total = 0;
  constructor(public cart: CartService){ cart.items$.subscribe(_ => this.total = cart.items.reduce((s,i)=>s+i.price*i.qty,0)); }
}
```

Cart template
```html
<h2>Cart</h2>
<p-table [value]="cart.items">
  <ng-template pTemplate="header">
    <tr><th>Product</th><th>Price</th><th>Qty</th><th>Total</th><th></th></tr>
  </ng-template>
  <ng-template pTemplate="body" let-row>
    <tr>
      <td>{{row.name}}</td>
      <td>{{row.price | currency}}</td>
      <td><p-inputNumber [min]="1" [showButtons]="true" [ngModel]="row.qty" (onInput)="cart.updateQty(row.id, $event.value)"></p-inputNumber></td>
      <td>{{row.price * row.qty | currency}}</td>
      <td><button pButton icon="pi pi-trash" class="p-button-text p-button-danger" (click)="cart.remove(row.id)"></button></td>
    </tr>
  </ng-template>
</p-table>
<div class="p-d-flex p-jc-end p-mt-2">
  <h3>Total: {{ total | currency }}</h3>
</div>
```

Checkout form (summary)
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputTextModule, DropdownModule, ButtonModule],
  templateUrl: './checkout.page.html'
})
export class CheckoutPage {
  form = this.fb.group({
    name: ['', Validators.required],
    address: ['', Validators.required],
    city: ['', Validators.required],
    country: ['', Validators.required],
    payment: this.fb.group({ cardNumber:['', Validators.required], expiry:['', Validators.required], cvc:['', Validators.required] })
  });
  constructor(private fb: FormBuilder){}
  submit(){ if(this.form.invalid){ this.form.markAllAsTouched(); return;} console.log('Order placed', this.form.value); }
}
```

Exercises
1) Persist cart to localStorage.
2) Add coupon code and apply discount.
3) Add order summary sidebar with shipping and tax.
