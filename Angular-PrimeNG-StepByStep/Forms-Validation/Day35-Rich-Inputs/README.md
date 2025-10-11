# Day 35 — Rich Inputs: Editor, ColorPicker, Rating with Reactive Forms

Objectives
- Integrate rich PrimeNG inputs with reactive forms.
- Persist rich content and validate length/range.
- Provide live previews and accessibility tips.

Components covered
- Editor (p-editor)
- ColorPicker (p-colorPicker)
- Rating (p-rating)
- Chips (p-chips) bonus

Setup
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, Validators, FormBuilder } from '@angular/forms';
import { EditorModule } from 'primeng/editor';
import { ColorPickerModule } from 'primeng/colorpicker';
import { RatingModule } from 'primeng/rating';
import { ChipsModule } from 'primeng/chips';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-rich-inputs-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditorModule, ColorPickerModule, RatingModule, ChipsModule, ButtonModule],
  templateUrl: './rich-inputs-form.component.html'
})
export class RichInputsFormComponent {
  form = this.fb.group({
    description: ['', [Validators.required, Validators.minLength(20)]],
    themeColor: ['#3B82F6', Validators.required],
    rating: [3, [Validators.min(1), Validators.max(5)]],
    tags: [[] as string[]]
  });

  get descLength() { return (this.form.value.description || '').length; }

  constructor(private fb: FormBuilder) {}

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    console.log('Rich form', this.form.value);
  }
}
```

Template
```html
<h2>Product Review</h2>
<form [formGroup]="form" (ngSubmit)="submit()" class="p-fluid p-formgrid p-grid">
  <div class="p-field p-col-12">
    <label>Description (min 20 chars)</label>
    <p-editor formControlName="description" [style]="{height:'200px'}"></p-editor>
    <small class="p-text-secondary">{{ descLength }}/2000</small>
  </div>

  <div class="p-field p-col-12 p-md-6">
    <label>Theme Color</label>
    <p-colorPicker formControlName="themeColor" format="hex"></p-colorPicker>
    <div class="color-preview" [style.background]="form.value.themeColor"></div>
  </div>

  <div class="p-field p-col-12 p-md-6">
    <label>Rating (1-5)</label>
    <p-rating formControlName="rating" [cancel]="false"></p-rating>
  </div>

  <div class="p-field p-col-12">
    <label>Tags</label>
    <p-chips formControlName="tags" separator="," allowDuplicate="false"></p-chips>
  </div>

  <div class="p-col-12">
    <button pButton type="submit" label="Submit" icon="pi pi-check"></button>
  </div>
</form>

<style>
.color-preview{ width:2rem; height:2rem; border:1px solid var(--surface-border); display:inline-block; margin-left:.5rem; vertical-align:middle; }
</style>
```

Validation hints
- Use custom validator to strip HTML and check text length for Editor.
- Ensure color value format (hex/rgb) if using a specific format.
- Announce rating changes for screen readers via aria-live.

Exercises
1) Add a live markdown preview (if using a markdown editor alternative).
2) Restrict tags to a whitelist and show invalid tag messages.
3) Persist the form to localStorage and restore on load.
