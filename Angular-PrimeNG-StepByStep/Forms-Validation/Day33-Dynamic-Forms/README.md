# Day 33 — Dynamic Forms (Metadata-Driven) with PrimeNG

Objectives
- Build reactive forms from a JSON schema (metadata).
- Add/remove controls at runtime.
- Apply conditional fields and dynamic validators.
- Render PrimeNG inputs based on control type.

What you'll build
- A Profile form generated from metadata with text, number, dropdown, calendar, and checkbox controls, including conditional fields.

Concepts
- Schema-driven form config
- Control registry and factory
- Conditional visibility and enable/disable
- Dynamic validators and async validators

Schema model
```ts
export type ControlType = 'text' | 'number' | 'dropdown' | 'calendar' | 'checkbox';

export interface OptionItem { label: string; value: any }

export interface DynamicControl {
  key: string;
  type: ControlType;
  label: string;
  placeholder?: string;
  options?: OptionItem[]; // for dropdown
  validators?: Array<'required' | 'email' | 'min' | 'max' | 'minLength' | 'maxLength'>;
  validatorArgs?: Record<string, any>; // { min: 18, max: 120 }
  conditional?: {
    dependsOn: string; // key the condition listens to
    showWhenEquals?: any; // value to match to show
  };
}

export interface DynamicFormSchema {
  title: string;
  controls: DynamicControl[];
}
```

Component skeleton (standalone)
```ts
import { Component, computed, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-dynamic-form-demo',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputTextModule, InputNumberModule, DropdownModule, CalendarModule, CheckboxModule, ButtonModule],
  templateUrl: './dynamic-form-demo.component.html',
})
export class DynamicFormDemoComponent {
  schema: DynamicFormSchema = {
    title: 'Dynamic Profile',
    controls: [
      { key: 'fullName', type: 'text', label: 'Full Name', validators: ['required'], placeholder: 'Jane Doe' },
      { key: 'email', type: 'text', label: 'Email', validators: ['required', 'email'], placeholder: 'jane@example.com' },
      { key: 'age', type: 'number', label: 'Age', validators: ['min', 'max'], validatorArgs: { min: 18, max: 120 } },
      { key: 'country', type: 'dropdown', label: 'Country', validators: ['required'], options: [
        { label: 'India', value: 'IN' },
        { label: 'USA', value: 'US' },
        { label: 'UK', value: 'UK' },
      ] },
      { key: 'newsletter', type: 'checkbox', label: 'Subscribe to Newsletter?' },
      { key: 'dob', type: 'calendar', label: 'Date of Birth', conditional: { dependsOn: 'newsletter', showWhenEquals: true } },
    ],
  };

  form: FormGroup = this.fb.group({});

  constructor(private fb: FormBuilder) {
    this.buildForm(this.schema);
    this.setupConditionalFields();
  }

  private buildForm(schema: DynamicFormSchema) {
    for (const c of schema.controls) {
      const validators = this.mapValidators(c);
      this.form.addControl(c.key, this.fb.control(null, validators));
    }
  }

  private mapValidators(c: DynamicControl) {
    const v = [] as any[];
    const args = c.validatorArgs || {};
    (c.validators || []).forEach(name => {
      switch (name) {
        case 'required': v.push(Validators.required); break;
        case 'email': v.push(Validators.email); break;
        case 'min': if (args.min != null) v.push(Validators.min(args.min)); break;
        case 'max': if (args.max != null) v.push(Validators.max(args.max)); break;
        case 'minLength': if (args.minLength != null) v.push(Validators.minLength(args.minLength)); break;
        case 'maxLength': if (args.maxLength != null) v.push(Validators.maxLength(args.maxLength)); break;
      }
    });
    return v;
  }

  private setupConditionalFields() {
    for (const c of this.schema.controls) {
      if (c.conditional) {
        const dep = this.form.get(c.conditional.dependsOn)!;
        dep.valueChanges.subscribe(val => {
          const ctrl = this.form.get(c.key)!;
          const shouldShow = val === c.conditional!.showWhenEquals;
          if (!shouldShow) {
            ctrl.reset();
            ctrl.disable({ emitEvent: false });
          } else {
            ctrl.enable({ emitEvent: false });
          }
        });
        // initialize
        const initVal = this.form.get(c.conditional.dependsOn)!.value;
        const shouldShow = initVal === c.conditional.showWhenEquals;
        const ctrl = this.form.get(c.key)!;
        shouldShow ? ctrl.enable({ emitEvent: false }) : ctrl.disable({ emitEvent: false });
      }
    }
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    console.log('Dynamic form value', this.form.getRawValue());
  }
}
```

Template (dynamic renderer)
```html
<h2>{{ schema.title }}</h2>
<form [formGroup]="form" class="p-fluid p-formgrid p-grid" (ngSubmit)="submit()">
  <ng-container *ngFor="let c of schema.controls">
    <div class="p-field p-col-12 p-md-6" [ngSwitch]="c.type">

      <!-- Text / Email -->
      <ng-container *ngSwitchCase="'text'">
        <label [for]="c.key">{{ c.label }}</label>
        <input pInputText [id]="c.key" [placeholder]="c.placeholder" [formControlName]="c.key" />
      </ng-container>

      <!-- Number -->
      <ng-container *ngSwitchCase="'number'">
        <label [for]="c.key">{{ c.label }}</label>
        <p-inputNumber [inputId]="c.key" [formControlName]="c.key" [min]="c.validatorArgs?.min" [max]="c.validatorArgs?.max"></p-inputNumber>
      </ng-container>

      <!-- Dropdown -->
      <ng-container *ngSwitchCase="'dropdown'">
        <label [for]="c.key">{{ c.label }}</label>
        <p-dropdown [options]="c.options || []" optionLabel="label" optionValue="value" [formControlName]="c.key" [placeholder]="c.placeholder"></p-dropdown>
      </ng-container>

      <!-- Calendar -->
      <ng-container *ngSwitchCase="'calendar'">
        <label [for]="c.key">{{ c.label }}</label>
        <p-calendar [formControlName]="c.key" dateFormat="yy-mm-dd" inputId="{{c.key}}"></p-calendar>
      </ng-container>

      <!-- Checkbox -->
      <ng-container *ngSwitchCase="'checkbox'">
        <p-checkbox [inputId]="c.key" [binary]="true" [formControlName]="c.key"></p-checkbox>
        <label [for]="c.key" class="p-ml-2">{{ c.label }}</label>
      </ng-container>

    </div>
  </ng-container>

  <div class="p-col-12">
    <button pButton type="submit" label="Submit" icon="pi pi-check"></button>
  </div>
</form>
```

Exercises
1) Add support for a new control type: textarea.
2) Implement async email availability validator; show a spinner while validating.
3) Add conditional enable: enable a field when another has a value > X.

Checklist
- [ ] Generates FormGroup from schema
- [ ] Renders PrimeNG components by type
- [ ] Conditional show/enable works
- [ ] Dynamic validators applied
- [ ] Submit returns expected model
