# Day 32: Validation & Error Handling Patterns

Make forms resilient and user-friendly. Today you’ll build a reusable validation message component, async validators, server-side errors mapping, and UX patterns with PrimeNG Messages/Toast/Tooltip.

What you’ll build
- Reusable <app-field-errors> to show control errors
- Async validator for unique email
- Submit error handling: map server errors to specific fields
- Inline, tooltip, and toast error patterns

Prerequisites
- Day 31 completed (Reactive Forms)
- PrimeNG: MessagesModule, MessageModule, TooltipModule, ToastModule

1) Reusable error component

```ts
// field-errors.component.ts
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-field-errors',
  standalone: true,
  imports: [CommonModule],
  template: `
    <ng-container *ngIf="control && control.touched && control.errors">
      <small class="error" *ngIf="control.errors['required']">This field is required.</small>
      <small class="error" *ngIf="control.errors['email']">Invalid email.</small>
      <small class="error" *ngIf="control.errors['minlength']">Min length: {{control.errors['minlength'].requiredLength}}</small>
      <small class="error" *ngIf="control.errors['maxlength']">Max length: {{control.errors['maxlength'].requiredLength}}</small>
      <small class="error" *ngIf="control.errors['pattern']">Invalid format.</small>
      <small class="error" *ngIf="control.errors['unique']">Already taken.</small>
      <small class="error" *ngIf="control.errors['server']">{{control.errors['server']}}</small>
    </ng-container>
  `,
  styles: [`.error{color:#ef4444;display:block;font-size:.8rem}`]
})
export class FieldErrorsComponent {
  @Input() control!: AbstractControl | null;
}
```

2) Async validator (unique email)

```ts
// unique-email.validator.ts
import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { of } from 'rxjs';
import { delay, map } from 'rxjs/operators';

export function uniqueEmailValidator(): AsyncValidatorFn {
  return (control: AbstractControl) => {
    const email = control.value as string;
    if (!email) return of(null);
    // mock API: emails ending with test.com are already taken
    return of(email.endsWith('@test.com')).pipe(
      delay(600),
      map(isTaken => (isTaken ? { unique: true } : null))
    );
  };
}
```

3) Form using reusable errors + async validator

```ts
// profile-form.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { FieldErrorsComponent } from './field-errors.component';
import { uniqueEmailValidator } from './unique-email.validator';

@Component({
  selector: 'app-profile-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputTextModule, ButtonModule, ToastModule, FieldErrorsComponent],
  providers: [MessageService],
  templateUrl: './profile-form.component.html',
  styleUrls: ['./profile-form.component.scss']
})
export class ProfileFormComponent {
  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email], [uniqueEmailValidator()]]
  });

  constructor(private fb: FormBuilder, private messages: MessageService) {}

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    // mock server error
    setTimeout(() => {
      // simulate a server-side validation error for name
      this.form.get('name')?.setErrors({ server: 'Name contains restricted words.'});
      this.messages.add({ severity: 'error', summary: 'Update failed', detail: 'Please fix form errors.'});
    }, 700);
  }
}
```

```html
<!-- profile-form.component.html -->
<p-toast></p-toast>
<div class="card">
  <h3>Profile</h3>
  <form [formGroup]="form" (ngSubmit)="submit()" class="form">
    <div class="field">
      <label>Name</label>
      <input pInputText formControlName="name" />
      <app-field-errors [control]="form.get('name')"></app-field-errors>
    </div>

    <div class="field">
      <label>Email</label>
      <input pInputText formControlName="email" pTooltip="Use your work email" tooltipPosition="right" />
      <app-field-errors [control]="form.get('email')"></app-field-errors>
    </div>

    <div class="actions">
      <button pButton type="submit" label="Save" [disabled]="form.invalid"></button>
    </div>
  </form>
</div>
```

```scss
/* profile-form.component.scss */
.card{padding:1rem;background:#fff;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,.06)}
.field{margin-bottom:.75rem}
label{display:block;margin-bottom:.25rem;font-weight:600}
.actions{display:flex;justify-content:flex-end}
```

4) Error display patterns
- Inline small text (as above) for critical fields
- Tooltip hints for optional guidance
- Toast for submit-level messages
- Disable submit until valid and dirty

5) Mapping server errors
Map API errors to form controls so users see what to fix.

```ts
applyServerErrors(form: any, errors: Record<string,string>) {
  Object.entries(errors).forEach(([field, msg]) => {
    const ctrl = form.get(field);
    if (ctrl) ctrl.setErrors({ ...(ctrl.errors||{}), server: msg });
  });
}
```

Exercises
- Add username async validator hitting a real API
- Show a Messages component listing all form errors at once
- Add a global error banner for network outages

Summary
You now have a reusable, scalable validation system with async checks and server mapping. Next: Day 33 — Dynamic Forms.