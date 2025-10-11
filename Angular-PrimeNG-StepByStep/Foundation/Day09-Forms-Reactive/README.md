# Day 09: Reactive Forms 🎯

## 📋 Learning Objectives
- Create reactive forms with FormBuilder
- Implement complex validation
- Handle dynamic forms
- Use FormArrays
- Create custom validators

---

## 🚀 Setup

```typescript
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [ReactiveFormsModule]
})
export class AppModule { }
```

---

## 📝 Basic Reactive Form

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

export class UserFormComponent implements OnInit {
  userForm!: FormGroup;
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.userForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      age: [null, [Validators.min(18), Validators.max(100)]]
    });
  }
  
  onSubmit() {
    if (this.userForm.valid) {
      console.log(this.userForm.value);
    }
  }
  
  get name() { return this.userForm.get('name'); }
  get email() { return this.userForm.get('email'); }
}
```

```html
<form [formGroup]="userForm" (ngSubmit)="onSubmit()">
  <div>
    <label>Name:</label>
    <input formControlName="name">
    <div *ngIf="name?.invalid && name?.touched" class="error">
      <span *ngIf="name?.errors?.['required']">Name required</span>
      <span *ngIf="name?.errors?.['minlength']">Min 3 characters</span>
    </div>
  </div>
  
  <div>
    <label>Email:</label>
    <input formControlName="email">
    <div *ngIf="email?.invalid && email?.touched" class="error">
      <span *ngIf="email?.errors?.['required']">Email required</span>
      <span *ngIf="email?.errors?.['email']">Invalid email</span>
    </div>
  </div>
  
  <button type="submit" [disabled]="userForm.invalid">Submit</button>
</form>
```

---

## 🎯 Complex Registration Form

```typescript
export class RegistrationComponent implements OnInit {
  registrationForm!: FormGroup;
  submitted = false;
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.registrationForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
      profile: this.fb.group({
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        age: [null, [Validators.min(18), Validators.max(100)]],
        phone: ['', Validators.pattern(/^[0-9]{10}$/)]
      }),
      address: this.fb.group({
        street: [''],
        city: ['', Validators.required],
        state: ['', Validators.required],
        zip: ['', [Validators.required, Validators.pattern(/^[0-9]{5,6}$/)]]
      }),
      terms: [false, Validators.requiredTrue]
    }, {
      validators: this.passwordMatchValidator
    });
  }
  
  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword?.setErrors(null);
    }
    return null;
  }
  
  onSubmit() {
    this.submitted = true;
    
    if (this.registrationForm.valid) {
      console.log('Form Data:', this.registrationForm.value);
      // API call
    } else {
      console.log('Form invalid');
      this.markFormGroupTouched(this.registrationForm);
    }
  }
  
  markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
      
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
  
  get f() { return this.registrationForm.controls; }
}
```

---

## 📋 Form Arrays (Dynamic Forms)

```typescript
export class SkillsFormComponent implements OnInit {
  skillsForm!: FormGroup;
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.skillsForm = this.fb.group({
      name: ['', Validators.required],
      skills: this.fb.array([
        this.createSkillFormGroup()
      ])
    });
  }
  
  createSkillFormGroup(): FormGroup {
    return this.fb.group({
      skillName: ['', Validators.required],
      experience: [null, [Validators.required, Validators.min(0)]]
    });
  }
  
  get skills(): FormArray {
    return this.skillsForm.get('skills') as FormArray;
  }
  
  addSkill() {
    this.skills.push(this.createSkillFormGroup());
  }
  
  removeSkill(index: number) {
    this.skills.removeAt(index);
  }
  
  onSubmit() {
    if (this.skillsForm.valid) {
      console.log(this.skillsForm.value);
    }
  }
}
```

```html
<form [formGroup]="skillsForm" (ngSubmit)="onSubmit()">
  <div>
    <label>Name:</label>
    <input formControlName="name">
  </div>
  
  <div formArrayName="skills">
    <h3>Skills</h3>
    <div *ngFor="let skill of skills.controls; let i = index" [formGroupName]="i">
      <input formControlName="skillName" placeholder="Skill">
      <input formControlName="experience" type="number" placeholder="Years">
      <button type="button" (click)="removeSkill(i)">Remove</button>
    </div>
  </div>
  
  <button type="button" (click)="addSkill()">Add Skill</button>
  <button type="submit" [disabled]="skillsForm.invalid">Submit</button>
</form>
```

---

## 🔒 Custom Validators

```typescript
// validators/custom-validators.ts
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class CustomValidators {
  static noWhitespace(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const isWhitespace = (control.value || '').trim().length === 0;
      return isWhitespace ? { whitespace: true } : null;
    };
  }
  
  static strongPassword(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;
      
      const hasNumber = /[0-9]/.test(value);
      const hasUpper = /[A-Z]/.test(value);
      const hasLower = /[a-z]/.test(value);
      const hasSpecial = /[!@#$%^&*]/.test(value);
      
      const valid = hasNumber && hasUpper && hasLower && hasSpecial;
      return !valid ? { weakPassword: true } : null;
    };
  }
}
```

**Usage:**
```typescript
this.registrationForm = this.fb.group({
  username: ['', [
    Validators.required,
    CustomValidators.noWhitespace()
  ]],
  password: ['', [
    Validators.required,
    Validators.minLength(8),
    CustomValidators.strongPassword()
  ]]
});
```

---

✅ **Day 09 Complete!** Tomorrow: HTTP Client & APIs 🌐
