# Day 36 — Slider, InputSwitch, and Range Patterns

Objectives
- Use Slider and InputSwitch with reactive forms.
- Implement range selection and value formatting.
- Toggle sections of a form with switches.

Components
- p-slider (single and range)
- p-inputSwitch
- p-inputNumber (for formatted sync)

Example: Pricing & Features Form
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { SliderModule } from 'primeng/slider';
import { InputSwitchModule } from 'primeng/inputswitch';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-pricing-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SliderModule, InputSwitchModule, InputNumberModule, ButtonModule],
  templateUrl: './pricing-form.component.html'
})
export class PricingFormComponent {
  form = this.fb.group({
    price: [50, [Validators.min(0), Validators.max(100)]],
    discountRange: this.fb.control([10, 30]),
    yearlyBilling: [false],
    advancedOptions: [false],
    maxUsers: [10, [Validators.min(1), Validators.max(1000)]]
  });

  constructor(private fb: FormBuilder) {
    this.form.get('advancedOptions')!.valueChanges.subscribe(on => {
      const ctrl = this.form.get('maxUsers')!;
      on ? ctrl.enable() : ctrl.disable();
    });
    if (!this.form.value.advancedOptions) this.form.get('maxUsers')!.disable();
  }

  get discountLabel() {
    const [min, max] = this.form.value.discountRange as number[];
    return `${min}% - ${max}%`;
  }

  submit() {
    console.log('Pricing', this.form.getRawValue());
  }
}
```

Template
```html
<h2>Pricing Options</h2>
<form [formGroup]="form" (ngSubmit)="submit()" class="p-fluid p-formgrid p-grid">
  <div class="p-field p-col-12 p-md-6">
    <label>Base Price</label>
    <p-inputNumber formControlName="price" [suffix]="' USD'" [min]="0" [max]="100" [showButtons]="true"></p-inputNumber>
    <p-slider formControlName="price" [min]="0" [max]="100"></p-slider>
  </div>

  <div class="p-field p-col-12 p-md-6">
    <label>Discount Range ({{ discountLabel }})</label>
    <p-slider formControlName="discountRange" [range]="true" [min]="0" [max]="90" [step]="5"></p-slider>
  </div>

  <div class="p-field p-col-12 p-md-6">
    <label>Yearly Billing</label>
    <p-inputSwitch formControlName="yearlyBilling"></p-inputSwitch>
  </div>

  <div class="p-field p-col-12 p-md-6">
    <label>Advanced Options</label>
    <p-inputSwitch formControlName="advancedOptions"></p-inputSwitch>
  </div>

  <div class="p-field p-col-12 p-md-6" *ngIf="form.get('advancedOptions')!.value">
    <label>Max Users</label>
    <p-inputNumber formControlName="maxUsers" [min]="1" [max]="1000" [showButtons]="true"></p-inputNumber>
  </div>

  <div class="p-col-12">
    <button pButton type="submit" label="Save" icon="pi pi-check"></button>
  </div>
</form>
```

UX Tips
- Always sync slider with number input for precision.
- Provide textual labels for ranges.
- Debounce slider input when saving to API.

Exercises
1) Add a disabled period where slider cannot be moved when loading.
2) Add tooltip showing current slider value while dragging.
3) Add a second range for "trial period" days.
