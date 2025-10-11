# Day 08: Template-Driven Forms 📝

## 📋 Learning Objectives
- Create template-driven forms
- Implement form validation
- Handle form submission
- Display validation errors
- Use ngModel for two-way binding

---

## 🚀 Setup

### Import FormsModule
```typescript
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [FormsModule]
})
export class AppModule { }
```

---

## 📝 Basic Form

```html
<form #userForm="ngForm" (ngSubmit)="onSubmit(userForm)">
  <div>
    <label>Name:</label>
    <input type="text" name="name" [(ngModel)]="user.name" required>
  </div>
  
  <div>
    <label>Email:</label>
    <input type="email" name="email" [(ngModel)]="user.email" required email>
  </div>
  
  <button type="submit" [disabled]="userForm.invalid">Submit</button>
</form>
```

```typescript
export class UserFormComponent {
  user = {
    name: '',
    email: ''
  };
  
  onSubmit(form: NgForm) {
    if (form.valid) {
      console.log('Form submitted:', this.user);
    }
  }
}
```

---

## ✅ Validation

### Built-in Validators
```html
<input type="text" name="username" 
       [(ngModel)]="user.username"
       required
       minlength="3"
       maxlength="20"
       pattern="[a-zA-Z0-9]+"
       #username="ngModel">

<!-- Display errors -->
<div *ngIf="username.invalid && username.touched" class="error">
  <p *ngIf="username.errors?.['required']">Username is required</p>
  <p *ngIf="username.errors?.['minlength']">Min 3 characters</p>
  <p *ngIf="username.errors?.['pattern']">Alphanumeric only</p>
</div>
```

### Complete Registration Form
```html
<form #registrationForm="ngForm" (ngSubmit)="onRegister(registrationForm)">
  <!-- Username -->
  <div class="form-group">
    <label>Username*</label>
    <input type="text" name="username" 
           [(ngModel)]="formData.username"
           required minlength="3" #username="ngModel">
    <small *ngIf="username.invalid && username.touched" class="error">
      Username required (min 3 chars)
    </small>
  </div>
  
  <!-- Email -->
  <div class="form-group">
    <label>Email*</label>
    <input type="email" name="email"
           [(ngModel)]="formData.email"
           required email #email="ngModel">
    <small *ngIf="email.invalid && email.touched" class="error">
      Valid email required
    </small>
  </div>
  
  <!-- Password -->
  <div class="form-group">
    <label>Password*</label>
    <input type="password" name="password"
           [(ngModel)]="formData.password"
           required minlength="8" #password="ngModel">
    <small *ngIf="password.invalid && password.touched" class="error">
      Password required (min 8 chars)
    </small>
  </div>
  
  <!-- Age -->
  <div class="form-group">
    <label>Age</label>
    <input type="number" name="age"
           [(ngModel)]="formData.age"
           min="18" max="100" #age="ngModel">
    <small *ngIf="age.invalid && age.touched" class="error">
      Age must be 18-100
    </small>
  </div>
  
  <!-- Country -->
  <div class="form-group">
    <label>Country*</label>
    <select name="country" [(ngModel)]="formData.country" required>
      <option value="">Select Country</option>
      <option value="US">United States</option>
      <option value="UK">United Kingdom</option>
      <option value="IN">India</option>
    </select>
  </div>
  
  <!-- Terms -->
  <div class="form-group">
    <label>
      <input type="checkbox" name="terms" 
             [(ngModel)]="formData.terms" required>
      I accept terms and conditions*
    </label>
  </div>
  
  <button type="submit" [disabled]="registrationForm.invalid">
    Register
  </button>
</form>

<pre>{{ formData | json }}</pre>
```

```typescript
export class RegistrationComponent {
  formData = {
    username: '',
    email: '',
    password: '',
    age: null,
    country: '',
    terms: false
  };
  
  onRegister(form: NgForm) {
    if (form.valid) {
      console.log('Registration successful:', this.formData);
      // API call here
      form.reset();
    }
  }
}
```

---

✅ **Day 08 Complete!** Tomorrow: Reactive Forms 🎯
