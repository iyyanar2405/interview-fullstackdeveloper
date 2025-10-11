# Day 38 — Sidebar/Drawer Forms & Multi-Step Wizard

Objectives
- Use p-sidebar as a contextual form container.
- Implement a simple 3-step wizard with validation per step.
- Manage dirty state and confirm before close.

Components
- p-sidebar, p-steps (or custom stepper), p-toast

Wizard Overview
- Step 1: Basic Info
- Step 2: Details
- Step 3: Review & Submit

Component
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { SidebarModule } from 'primeng/sidebar';
import { StepsModule } from 'primeng/steps';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-wizard-sidebar',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SidebarModule, StepsModule, ButtonModule, InputTextModule, ToastModule],
  providers: [MessageService],
  templateUrl: './wizard-sidebar.component.html'
})
export class WizardSidebarComponent {
  visible = false;
  activeIndex = 0;

  form = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    details: this.fb.group({ company: ['', Validators.required], role: ['', Validators.required] })
  });

  steps = [
    { label: 'Basic' },
    { label: 'Details' },
    { label: 'Review' }
  ];

  constructor(private fb: FormBuilder, private ms: MessageService) {}

  open() { this.visible = true; this.activeIndex = 0; }
  close() {
    if (this.form.dirty) {
      if (!confirm('Discard changes?')) return;
    }
    this.visible = false;
  }

  next() {
    if (this.activeIndex === 0 && (this.form.get('name')!.invalid || this.form.get('email')!.invalid)) {
      this.form.get('name')!.markAsTouched();
      this.form.get('email')!.markAsTouched();
      return;
    }
    if (this.activeIndex === 1 && (this.form.get('details.company')!.invalid || this.form.get('details.role')!.invalid)) {
      this.form.get('details.company')!.markAsTouched();
      this.form.get('details.role')!.markAsTouched();
      return;
    }
    this.activeIndex = Math.min(this.activeIndex + 1, 2);
  }

  prev() { this.activeIndex = Math.max(this.activeIndex - 1, 0); }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.ms.add({ severity: 'success', summary: 'Submitted', detail: 'Wizard completed' });
    this.visible = false;
  }
}
```

Template
```html
<p-toast></p-toast>
<button pButton label="Open Wizard" icon="pi pi-sliders-h" (click)="open()"></button>

<p-sidebar [(visible)]="visible" position="right" [style]="{ width: '35rem' }" [modal]="true">
  <p-steps [model]="steps" [activeIndex]="activeIndex"></p-steps>

  <div [ngSwitch]="activeIndex" class="p-mt-3">
    <div *ngSwitchCase="0">
      <div class="p-fluid">
        <div class="p-field">
          <label>Name</label>
          <input pInputText formControlName="name" />
        </div>
        <div class="p-field">
          <label>Email</label>
          <input pInputText formControlName="email" />
        </div>
      </div>
      <div>
        <button pButton label="Next" icon="pi pi-arrow-right" (click)="next()"></button>
      </div>
    </div>

    <div *ngSwitchCase="1" [formGroup]="form.get('details') as FormGroup">
      <div class="p-fluid">
        <div class="p-field">
          <label>Company</label>
          <input pInputText formControlName="company" />
        </div>
        <div class="p-field">
          <label>Role</label>
          <input pInputText formControlName="role" />
        </div>
      </div>
      <div>
        <button pButton label="Back" icon="pi pi-arrow-left" class="p-button-text p-mr-2" (click)="prev()"></button>
        <button pButton label="Next" icon="pi pi-arrow-right" (click)="next()"></button>
      </div>
    </div>

    <div *ngSwitchCase="2">
      <h3>Review</h3>
      <pre>{{ form.value | json }}</pre>
      <div>
        <button pButton label="Back" icon="pi pi-arrow-left" class="p-button-text p-mr-2" (click)="prev()"></button>
        <button pButton label="Submit" icon="pi pi-check" (click)="submit()"></button>
      </div>
    </div>
  </div>
</p-sidebar>
```

Tips
- Trap focus inside sidebar for keyboard users.
- Save draft to localStorage on step change.
- Use DynamicDialog for multiple wizard variants.

Exercises
1) Replace confirm() with p-confirmDialog.
2) Add progress bar showing completion percentage.
3) Make steps navigable by clicking (with validation guards).
