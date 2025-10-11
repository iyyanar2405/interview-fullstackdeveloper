# Day 31: Reactive Forms with PrimeNG

Today you’ll wire up Angular Reactive Forms end-to-end using PrimeNG inputs. We’ll build a complete Account Registration form with typed models, form builder, custom validators, dynamic disabling, and UX polish.

What you’ll build
- Typed reactive form with FormBuilder
- PrimeNG inputs (InputText, InputNumber, Dropdown, Calendar, Radio/Checkbox, InputSwitch)
- Custom validators (required, pattern, min/max, cross-field)
- Disable/enable logic, pristine/dirty handling
- Error messages with PrimeNG Message/Tooltip patterns

Prerequisites
- Angular FormsModule/ReactiveFormsModule set up
- PrimeNG modules: InputTextModule, DropdownModule, CalendarModule, RadioButtonModule, CheckboxModule, InputNumberModule, InputSwitchModule, ButtonModule, ToastModule

1) Model and setup

```ts
// account.model.ts
export interface AccountForm {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  country: string;
  dob: Date | null;
  gender: 'male' | 'female' | 'other' | '';
  interests: string[];
  terms: boolean;
  newsletter: boolean;
}
```

2) Component: form builder + validators

```ts
// account-registration.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { RadioButtonModule } from 'primeng/radiobutton';
import { CheckboxModule } from 'primeng/checkbox';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-account-registration',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    InputTextModule, InputNumberModule, DropdownModule, CalendarModule,
    RadioButtonModule, CheckboxModule, InputSwitchModule,
    ButtonModule, ToastModule
  ],
  providers: [MessageService],
  templateUrl: './account-registration.component.html',
  styleUrls: ['./account-registration.component.scss']
})
export class AccountRegistrationComponent implements OnInit {
  form = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', []],
    country: ['', Validators.required],
    dob: [null as Date | null, Validators.required],
    gender: ['' as 'male' | 'female' | 'other' | '', Validators.required],
    interests: this.fb.nonNullable.control<string[]>([]),
    terms: [false, Validators.requiredTrue],
    newsletter: [true]
  }, { validators: [adultValidator] });

  countries = [
    { label: 'Select Country', value: '' },
    { label: 'United States', value: 'US' },
    { label: 'United Kingdom', value: 'UK' },
    { label: 'India', value: 'IN' },
    { label: 'Germany', value: 'DE' }
  ];

  interestOptions = ['Sports', 'Music', 'Travel', 'Cooking', 'Coding'];

  submitting = false;

  constructor(private fb: FormBuilder, private messages: MessageService) {}

  ngOnInit(): void {}

  get f() { return this.form.controls; }

  submit() {
    if (this.form.invalid) {
      this.touchAll(this.form);
      this.messages.add({ severity: 'warn', summary: 'Validation', detail: 'Please fix the highlighted fields.'});
      return;
    }

    this.submitting = true;
    setTimeout(() => {
      this.submitting = false;
      this.messages.add({ severity: 'success', summary: 'Account Created', detail: `${this.f.firstName.value} ${this.f.lastName.value}` });
      this.form.reset({ newsletter: true, interests: [] });
    }, 1000);
  }

  private touchAll(control: AbstractControl) {
    control.markAsTouched();
    if ((control as any).controls) {
      (control as any).controls.forEach((c: AbstractControl) => this.touchAll(c));
    }
  }
}

export function adultValidator(group: AbstractControl): ValidationErrors | null {
  const dob = group.get('dob')?.value as Date | null;
  if (!dob) return null;
  const ageMs = Date.now() - new Date(dob).getTime();
  const age = ageMs / (365.25 * 24 * 60 * 60 * 1000);
  return age >= 18 ? null : { underage: true };
}
```

3) Template: PrimeNG inputs + errors

```html
<!-- account-registration.component.html -->
<p-toast></p-toast>
<div class="card">
  <h2>Create Account</h2>

  <form [formGroup]="form" (ngSubmit)="submit()" class="grid form-grid">
    <div class="col-12 md:col-6 field">
      <label>First Name *</label>
      <input pInputText formControlName="firstName" placeholder="John" />
      <small class="error" *ngIf="f.firstName.touched && f.firstName.errors?.['required']">First name is required.</small>
      <small class="error" *ngIf="f.firstName.touched && f.firstName.errors?.['minlength']">Min 2 characters.</small>
    </div>

    <div class="col-12 md:col-6 field">
      <label>Last Name *</label>
      <input pInputText formControlName="lastName" placeholder="Doe" />
      <small class="error" *ngIf="f.lastName.touched && f.lastName.errors?.['required']">Last name is required.</small>
      <small class="error" *ngIf="f.lastName.touched && f.lastName.errors?.['minlength']">Min 2 characters.</small>
    </div>

    <div class="col-12 md:col-6 field">
      <label>Email *</label>
      <input pInputText type="email" formControlName="email" placeholder="you@example.com" />
      <small class="error" *ngIf="f.email.touched && f.email.errors?.['required']">Email is required.</small>
      <small class="error" *ngIf="f.email.touched && f.email.errors?.['email']">Invalid email.</small>
    </div>

    <div class="col-12 md:col-6 field">
      <label>Phone</label>
      <input pInputText formControlName="phone" placeholder="Optional" />
    </div>

    <div class="col-12 md:col-6 field">
      <label>Country *</label>
      <p-dropdown [options]="countries" formControlName="country" optionLabel="label" optionValue="value"></p-dropdown>
      <small class="error" *ngIf="f.country.touched && f.country.errors?.['required']">Country is required.</small>
    </div>

    <div class="col-12 md:col-6 field">
      <label>Date of Birth *</label>
      <p-calendar formControlName="dob" [maxDate]="new Date()" dateFormat="dd M yy" [showIcon]="true"></p-calendar>
      <small class="error" *ngIf="f.dob.touched && f.dob.errors?.['required']">DOB is required.</small>
      <small class="error" *ngIf="form.touched && form.errors?.['underage']">You must be 18+.</small>
    </div>

    <div class="col-12 field">
      <label>Gender *</label>
      <div class="p-formgrid p-grid">
        <div class="p-field-radiobutton">
          <p-radioButton name="gender" inputId="m" value="male" formControlName="gender"></p-radioButton>
          <label for="m">Male</label>
        </div>
        <div class="p-field-radiobutton">
          <p-radioButton name="gender" inputId="f" value="female" formControlName="gender"></p-radioButton>
          <label for="f">Female</label>
        </div>
        <div class="p-field-radiobutton">
          <p-radioButton name="gender" inputId="o" value="other" formControlName="gender"></p-radioButton>
          <label for="o">Other</label>
        </div>
      </div>
      <small class="error" *ngIf="f.gender.touched && f.gender.errors?.['required']">Select one.</small>
    </div>

    <div class="col-12 field">
      <label>Interests</label>
      <div class="chips">
        <p-checkbox *ngFor="let opt of interestOptions" [binary]="true" [inputId]="opt"
                    [ngModel]="f.interests.value?.includes(opt)" (onChange)="
                      $event.checked ? f.interests.setValue([...(f.interests.value||[]), opt])
                                     : f.interests.setValue((f.interests.value||[]).filter(x=>x!==opt))
                    "></p-checkbox>
        <label *ngFor="let opt of interestOptions" [for]="opt" class="chip-label">{{opt}}</label>
      </div>
    </div>

    <div class="col-12 md:col-6 field switch">
      <label>Subscribe to newsletter</label>
      <p-inputSwitch formControlName="newsletter"></p-inputSwitch>
    </div>

    <div class="col-12 field terms">
      <p-checkbox formControlName="terms" [binary]="true" inputId="t"></p-checkbox>
      <label for="t">I agree to the Terms *</label>
      <small class="error" *ngIf="f.terms.touched && f.terms.errors?.['required']">You must accept the terms.</small>
    </div>

    <div class="col-12 actions">
      <button pButton type="button" label="Reset" class="p-button-text" (click)="form.reset({ newsletter: true, interests: [] })"></button>
      <button pButton type="submit" label="Create Account" [loading]="submitting" [disabled]="form.invalid"></button>
    </div>
  </form>
</div>
```

4) Styles

```scss
/* account-registration.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.form-grid { row-gap: .75rem; }
.field label { display:block; margin-bottom:.25rem; font-weight:600; }
.error { color:#ef4444; font-size:.8rem; }
.actions { display:flex; gap:.5rem; justify-content:flex-end; margin-top:.5rem; }
.chips { display:flex; gap:.5rem; flex-wrap:wrap; align-items:center; }
.chip-label { margin-right: .75rem; }
.switch { display:flex; align-items:center; justify-content:space-between; }
.terms { display:flex; align-items:center; gap:.5rem; }
```

5) Tips and patterns
- Keep forms typed with interfaces for safety
- Centralize common validators and helper methods
- Disable submit until form.valid and not submitting
- For large forms, use Feature Modules or Standalone components per section
- Use trackBy for lists of dynamic form controls (FormArray)

Exercises
- Add a FormArray for multiple phone numbers with add/remove
- Add a mask for phone input and a pattern validator
- Persist form draft in localStorage and restore on reload

Summary
You can now build robust reactive forms using PrimeNG controls with validation and UX polish. Next: Day 32 — Validation & Error Handling patterns.