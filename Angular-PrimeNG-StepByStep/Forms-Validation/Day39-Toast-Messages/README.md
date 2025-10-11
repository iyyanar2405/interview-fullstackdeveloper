# Day 39 — Toast & Messages Patterns

Objectives
- Use Toast and Messages for global and inline feedback.
- Centralize error handling and success notifications.
- Map server errors to forms and show helpful messages.

Components
- p-toast, p-messages, p-message
- MessageService

Global notifier service
```ts
import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({ providedIn: 'root' })
export class NotifierService {
  constructor(private ms: MessageService) {}
  success(detail: string, summary = 'Success') { this.ms.add({ severity: 'success', summary, detail }); }
  info(detail: string, summary = 'Info') { this.ms.add({ severity: 'info', summary, detail }); }
  warn(detail: string, summary = 'Warning') { this.ms.add({ severity: 'warn', summary, detail }); }
  error(detail: string, summary = 'Error') { this.ms.add({ severity: 'error', summary, detail }); }
}
```

Form component
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { NotifierService } from './notifier.service';

@Component({
  selector: 'app-feedback-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ToastModule, MessagesModule, MessageModule, ButtonModule],
  providers: [MessageService],
  templateUrl: './feedback-form.component.html'
})
export class FeedbackFormComponent {
  form = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    message: ['', [Validators.required, Validators.minLength(10)]],
  });

  inlineMsgs: any[] = [];

  constructor(private fb: FormBuilder, private notify: NotifierService) {}

  async submit() {
    if (this.form.invalid) {
      this.inlineMsgs = [{ severity: 'warn', summary: 'Validation', detail: 'Please fix validation errors.' }];
      this.form.markAllAsTouched();
      return;
    }
    try {
      // await api.save(this.form.value)
      this.notify.success('Feedback submitted');
      this.inlineMsgs = [];
      this.form.reset();
    } catch (e: any) {
      // map server errors
      if (e?.fieldErrors) {
        for (const err of e.fieldErrors) {
          this.form.get(err.field)?.setErrors({ server: err.message });
        }
        this.inlineMsgs = [{ severity: 'error', summary: 'Server Error', detail: 'Please review highlighted fields.' }];
      } else {
        this.notify.error('Unexpected error');
      }
    }
  }
}
```

Template
```html
<p-toast></p-toast>
<p-messages [(value)]="inlineMsgs"></p-messages>

<form [formGroup]="form" (ngSubmit)="submit()" class="p-fluid">
  <div class="p-field">
    <label>Title</label>
    <input pInputText formControlName="title" />
    <small class="p-error" *ngIf="form.get('title')?.errors?.['server']">{{ form.get('title')?.errors?.['server'] }}</small>
  </div>
  <div class="p-field">
    <label>Message</label>
    <textarea pInputTextarea formControlName="message" rows="4"></textarea>
    <small class="p-error" *ngIf="form.get('message')?.errors?.['server']">{{ form.get('message')?.errors?.['server'] }}</small>
  </div>
  <button pButton type="submit" label="Send" icon="pi pi-send"></button>
</form>
```

Patterns
- Use Toast for global success/info; use inline Messages for validation issues.
- On navigation away with unsaved changes, use confirm + toast to inform saved as draft.
- Standardize message texts and severities.

Exercises
1) Add a global error interceptor to show toast on HTTP errors.
2) Add growl-like stacked toasts for multiple actions.
3) Auto-clear inline messages after 5 seconds.
