# Day 34 — FormArray & Nested Forms with PrimeNG

Objectives
- Manage dynamic lists of controls with FormArray.
- Build nested FormGroups with validation.
- Add/remove rows with PrimeNG table and inputs.
- Validate each row and show inline errors.

Scenario
- Build a Contact List form where users can add multiple contacts, each with name, email, phones (FormArray), and a preferred flag.

Models
```ts
interface Phone { number: string; type: 'mobile' | 'home' | 'work' }
interface Contact { name: string; email: string; preferred: boolean; phones: Phone[] }
```

Component outline
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-contact-list-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputTextModule, ButtonModule, TableModule, CheckboxModule, DropdownModule],
  templateUrl: './contact-list-form.component.html'
})
export class ContactListFormComponent {
  form = this.fb.group({
    contacts: this.fb.array([]),
  });

  phoneTypes = [
    { label: 'Mobile', value: 'mobile' },
    { label: 'Home', value: 'home' },
    { label: 'Work', value: 'work' },
  ];

  constructor(private fb: FormBuilder) {
    this.addContact();
  }

  get contacts(): FormArray { return this.form.get('contacts') as FormArray; }

  private createPhoneGroup(): FormGroup {
    return this.fb.group({
      number: ['', [Validators.required, Validators.minLength(7)]],
      type: ['mobile', Validators.required],
    });
  }

  private createContactGroup(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      preferred: [false],
      phones: this.fb.array([this.createPhoneGroup()])
    });
  }

  addContact() { this.contacts.push(this.createContactGroup()); }
  removeContact(i: number) { this.contacts.removeAt(i); }

  phonesAt(i: number) { return (this.contacts.at(i).get('phones') as FormArray); }
  addPhone(i: number) { this.phonesAt(i).push(this.createPhoneGroup()); }
  removePhone(i: number, j: number) { this.phonesAt(i).removeAt(j); }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    console.log('Contacts', this.form.value);
  }
}
```

Template
```html
<h2>Contact List</h2>
<form [formGroup]="form" (ngSubmit)="submit()">
  <p-table [value]="contacts.controls" dataKey="index" responsiveLayout="scroll">
    <ng-template pTemplate="header">
      <tr>
        <th>#</th>
        <th>Name</th>
        <th>Email</th>
        <th>Preferred</th>
        <th>Phones</th>
        <th>Actions</th>
      </tr>
    </ng-template>
    <ng-template pTemplate="body" let-row let-i="rowIndex">
      <tr [formGroup]="row">
        <td>{{ i + 1 }}</td>
        <td>
          <input pInputText formControlName="name" placeholder="Full name" />
          <div class="p-error" *ngIf="row.get('name')?.touched && row.get('name')?.invalid">Name required</div>
        </td>
        <td>
          <input pInputText formControlName="email" placeholder="email@example.com" />
          <div class="p-error" *ngIf="row.get('email')?.touched && row.get('email')?.invalid">Valid email required</div>
        </td>
        <td>
          <p-checkbox binary="true" formControlName="preferred"></p-checkbox>
        </td>
        <td>
          <div formArrayName="phones">
            <div *ngFor="let ph of row.get('phones').controls; let j = index" [formGroupName]="j" class="p-d-flex p-ai-center p-mb-2">
              <input pInputText formControlName="number" placeholder="Phone" class="p-mr-2" />
              <p-dropdown [options]="phoneTypes" optionLabel="label" optionValue="value" formControlName="type" class="p-mr-2"></p-dropdown>
              <button pButton icon="pi pi-trash" severity="danger" type="button" (click)="removePhone(i, j)"></button>
            </div>
            <button pButton icon="pi pi-plus" label="Add Phone" type="button" (click)="addPhone(i)"></button>
          </div>
        </td>
        <td>
          <button pButton icon="pi pi-trash" severity="danger" type="button" (click)="removeContact(i)"></button>
        </td>
      </tr>
    </ng-template>
    <ng-template pTemplate="footer">
      <tr>
        <td colspan="6">
          <button pButton icon="pi pi-plus" label="Add Contact" type="button" (click)="addContact()"></button>
        </td>
      </tr>
    </ng-template>
  </p-table>

  <div class="p-mt-3">
    <button pButton type="submit" label="Save" icon="pi pi-check"></button>
  </div>
</form>
```

Validation tips
- Disable Save until form valid.
- Highlight first invalid control on submit.
- If at least one preferred must be selected, add a custom validator on `contacts` array.

Exercises
1) Add custom validator to ensure unique emails across contacts.
2) Add mask for phone number using InputMask.
3) Add drag-and-drop reordering for contacts.
