# Day 40 — Forms Architecture & Best Practices (PrimeNG + Angular)

Objectives
- Structure complex forms with scalability in mind.
- Build reusable form controls and error display components.
- Manage form state, persistence, and performance.

Topics
1) Folder structure and separation of concerns
2) Reusable control components (ControlValueAccessor)
3) Reusable field error component
4) Facade/service for form data load/save
5) Autosave and dirty tracking
6) Performance tuning (OnPush, trackBy, detached change detection)

Recommended structure
```
app/
  forms/
    controls/
      phone-input/
      currency-input/
    validators/
      async/
      sync/
    components/
      field-errors/
    services/
      form-facade.service.ts
    utils/
      form-helpers.ts
```

Custom control example (ControlValueAccessor)
```ts
import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { InputMaskModule } from 'primeng/inputmask';

@Component({
  selector: 'app-phone-input',
  standalone: true,
  imports: [InputMaskModule],
  template: `<p-inputMask [mask]="mask" [unmask]="true" (onComplete)="onTouched()" (onInput)="onChange($event.value)" [value]="value"></p-inputMask>`,
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => PhoneInputComponent),
    multi: true
  }]
})
export class PhoneInputComponent implements ControlValueAccessor {
  @Input() mask = '(999) 999-9999';
  value: string | null = null;
  onChange = (v: any) => {};
  onTouched = () => {};
  writeValue(v: any): void { this.value = v; }
  registerOnChange(fn: any): void { this.onChange = fn; }
  registerOnTouched(fn: any): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void { /* pass to internal input via @ViewChild if needed */ }
}
```

Field error component
```ts
import { Component, Input } from '@angular/core';
import { AbstractControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-field-errors',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <ng-container *ngIf="control && control.touched && control.errors as e">
      <small class="p-error" *ngIf="e['required']">This field is required.</small>
      <small class="p-error" *ngIf="e['email']">Please enter a valid email.</small>
      <small class="p-error" *ngIf="e['minlength']">Minimum length not met.</small>
      <small class="p-error" *ngIf="e['maxlength']">Maximum length exceeded.</small>
      <small class="p-error" *ngIf="e['server']">{{ e['server'] }}</small>
    </ng-container>
  `
})
export class FieldErrorsComponent {
  @Input() control!: AbstractControl | null;
}
```

Form facade
```ts
import { Injectable } from '@angular/core';
import { delay, of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProfileFormFacade {
  load() { return of({ name: 'Jane', email: 'jane@example.com' }).pipe(delay(400)); }
  save(model: any) { return of(model).pipe(delay(400)); }
}
```

Container using facade and reusable controls
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FieldErrorsComponent } from '../components/field-errors.component';
import { PhoneInputComponent } from '../controls/phone-input.component';
import { ProfileFormFacade } from '../services/profile-form.facade';

@Component({
  selector: 'app-profile-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonModule, InputTextModule, FieldErrorsComponent, PhoneInputComponent],
  template: `
    <form [formGroup]="form" (ngSubmit)="submit()" class="p-fluid">
      <div class="p-field">
        <label>Name</label>
        <input pInputText formControlName="name" />
        <app-field-errors [control]="form.get('name')"></app-field-errors>
      </div>
      <div class="p-field">
        <label>Email</label>
        <input pInputText formControlName="email" />
        <app-field-errors [control]="form.get('email')"></app-field-errors>
      </div>
      <div class="p-field">
        <label>Phone</label>
        <app-phone-input formControlName="phone"></app-phone-input>
      </div>
      <button pButton type="submit" label="Save" icon="pi pi-check" [disabled]="form.invalid || saving"></button>
    </form>
  `
})
export class ProfileFormContainerComponent {
  form = this.fb.group({ name: ['', Validators.required], email: ['', [Validators.required, Validators.email]], phone: [''] });
  saving = false;
  constructor(private fb: FormBuilder, private facade: ProfileFormFacade) {
    this.facade.load().subscribe(v => this.form.patchValue(v));
  }
  submit() {
    if (this.form.invalid) return;
    this.saving = true;
    this.facade.save(this.form.value).subscribe(() => this.saving = false);
  }
}
```

Performance checklist
- Use OnPush strategy for heavy forms.
- Avoid re-creating arrays/objects in templates; compute via signals or memoized getters.
- Use trackBy with *ngFor for dynamic lists.
- Debounce valueChanges before API calls.
- Prefer getRawValue() when disabled controls are important.

Exercises
1) Extract and reuse the NotifierService from Day 39.
2) Create a currency-input control with InputNumber and CVA.
3) Add an autosave service with RxJS auditTime.
