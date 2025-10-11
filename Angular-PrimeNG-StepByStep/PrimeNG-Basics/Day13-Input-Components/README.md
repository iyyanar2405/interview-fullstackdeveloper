# Day 13: Input Components 📝

## 📋 Learning Objectives
- Use InputText for text inputs
- Implement InputNumber for numeric values
- Master Textarea for multi-line text
- Apply input icons and styling
- Handle validation and formatting

---

## 📝 InputText

### Installation
```typescript
import { InputTextModule } from 'primeng/inputtext';
```

### Basic Usage
```html
<!-- Basic input -->
<input type="text" pInputText placeholder="Enter name" />

<!-- With ngModel -->
<input type="text" pInputText [(ngModel)]="value" />

<!-- Disabled -->
<input type="text" pInputText [disabled]="true" value="Disabled" />

<!-- With icon (left) -->
<span class="p-input-icon-left">
  <i class="pi pi-search"></i>
  <input type="text" pInputText placeholder="Search" />
</span>

<!-- With icon (right) -->
<span class="p-input-icon-right">
  <i class="pi pi-spin pi-spinner"></i>
  <input type="text" pInputText />
</span>

<!-- With both icons -->
<span class="p-input-icon-left p-input-icon-right">
  <i class="pi pi-search"></i>
  <input type="text" pInputText />
  <i class="pi pi-times"></i>
</span>

<!-- Sizes -->
<input type="text" pInputText placeholder="Small" class="p-inputtext-sm" />
<input type="text" pInputText placeholder="Normal" />
<input type="text" pInputText placeholder="Large" class="p-inputtext-lg" />

<!-- Help text -->
<small class="block">Enter your username</small>
<input type="text" pInputText />

<!-- With validation -->
<input type="text" pInputText [class.ng-invalid]="isInvalid" />
<small *ngIf="isInvalid" class="p-error">This field is required</small>
```

---

## 🔢 InputNumber

### Installation
```typescript
import { InputNumberModule } from 'primeng/inputnumber';
```

### Basic Usage
```html
<!-- Basic number input -->
<p-inputNumber [(ngModel)]="value"></p-inputNumber>

<!-- With min/max -->
<p-inputNumber [(ngModel)]="value" [min]="0" [max]="100"></p-inputNumber>

<!-- With step -->
<p-inputNumber [(ngModel)]="value" [step]="0.25"></p-inputNumber>

<!-- Show buttons -->
<p-inputNumber [(ngModel)]="value" [showButtons]="true"></p-inputNumber>

<!-- Vertical buttons -->
<p-inputNumber [(ngModel)]="value" [showButtons]="true" 
               buttonLayout="vertical"></p-inputNumber>

<!-- Stacked buttons -->
<p-inputNumber [(ngModel)]="value" [showButtons]="true" 
               buttonLayout="stacked"></p-inputNumber>

<!-- Currency -->
<p-inputNumber [(ngModel)]="price" mode="currency" currency="USD" 
               locale="en-US"></p-inputNumber>

<p-inputNumber [(ngModel)]="price" mode="currency" currency="EUR" 
               locale="de-DE"></p-inputNumber>

<p-inputNumber [(ngModel)]="price" mode="currency" currency="INR" 
               locale="en-IN"></p-inputNumber>

<!-- Decimal -->
<p-inputNumber [(ngModel)]="value" mode="decimal" 
               [minFractionDigits]="2" [maxFractionDigits]="5"></p-inputNumber>

<!-- Percentage -->
<p-inputNumber [(ngModel)]="percentage" suffix="%" 
               [min]="0" [max]="100"></p-inputNumber>

<!-- Custom prefix/suffix -->
<p-inputNumber [(ngModel)]="value" prefix="$"></p-inputNumber>
<p-inputNumber [(ngModel)]="value" suffix=" kg"></p-inputNumber>

<!-- Disabled -->
<p-inputNumber [(ngModel)]="value" [disabled]="true"></p-inputNumber>
```

### TypeScript
```typescript
export class InputNumberDemoComponent {
  value: number = 0;
  price: number = 99.99;
  percentage: number = 50;
  quantity: number = 1;
}
```

---

## 📄 Textarea

### Installation
```typescript
import { InputTextareaModule } from 'primeng/inputtextarea';
```

### Basic Usage
```html
<!-- Basic textarea -->
<textarea pInputTextarea [(ngModel)]="text" rows="5" cols="30"></textarea>

<!-- Auto resize -->
<textarea pInputTextarea [(ngModel)]="text" [autoResize]="true"></textarea>

<!-- With placeholder -->
<textarea pInputTextarea placeholder="Enter description" 
          rows="5" cols="30"></textarea>

<!-- Disabled -->
<textarea pInputTextarea [disabled]="true" rows="3">Disabled content</textarea>

<!-- With character count -->
<textarea pInputTextarea [(ngModel)]="text" [maxlength]="200" 
          rows="5"></textarea>
<small>{{ text.length }}/200 characters</small>
```

---

## 🎯 Practical Exercise: User Profile Form

```typescript
export class UserProfileComponent implements OnInit {
  userForm!: FormGroup;
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      age: [null, [Validators.required, Validators.min(18), Validators.max(100)]],
      salary: [null, Validators.required],
      bio: ['', [Validators.required, Validators.maxLength(500)]],
      website: ['']
    });
  }
  
  onSubmit() {
    if (this.userForm.valid) {
      console.log('Form submitted:', this.userForm.value);
    } else {
      this.markFormGroupTouched(this.userForm);
    }
  }
  
  markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }
  
  get f() { return this.userForm.controls; }
}
```

```html
<div class="card">
  <h2>User Profile</h2>
  
  <form [formGroup]="userForm" (ngSubmit)="onSubmit()">
    <!-- First Name -->
    <div class="p-field">
      <label for="firstName">First Name*</label>
      <input id="firstName" type="text" pInputText formControlName="firstName" 
             [class.ng-invalid]="f['firstName'].invalid && f['firstName'].touched" />
      <small *ngIf="f['firstName'].invalid && f['firstName'].touched" class="p-error">
        First name is required (min 2 characters)
      </small>
    </div>
    
    <!-- Last Name -->
    <div class="p-field">
      <label for="lastName">Last Name*</label>
      <input id="lastName" type="text" pInputText formControlName="lastName" 
             [class.ng-invalid]="f['lastName'].invalid && f['lastName'].touched" />
      <small *ngIf="f['lastName'].invalid && f['lastName'].touched" class="p-error">
        Last name is required (min 2 characters)
      </small>
    </div>
    
    <!-- Email -->
    <div class="p-field">
      <label for="email">Email*</label>
      <span class="p-input-icon-left">
        <i class="pi pi-envelope"></i>
        <input id="email" type="email" pInputText formControlName="email" 
               placeholder="user@example.com"
               [class.ng-invalid]="f['email'].invalid && f['email'].touched" />
      </span>
      <small *ngIf="f['email'].invalid && f['email'].touched" class="p-error">
        Valid email is required
      </small>
    </div>
    
    <!-- Phone -->
    <div class="p-field">
      <label for="phone">Phone*</label>
      <span class="p-input-icon-left">
        <i class="pi pi-phone"></i>
        <input id="phone" type="tel" pInputText formControlName="phone" 
               placeholder="+1 (555) 123-4567"
               [class.ng-invalid]="f['phone'].invalid && f['phone'].touched" />
      </span>
      <small *ngIf="f['phone'].invalid && f['phone'].touched" class="p-error">
        Phone number is required
      </small>
    </div>
    
    <!-- Age -->
    <div class="p-field">
      <label for="age">Age*</label>
      <p-inputNumber id="age" formControlName="age" 
                     [showButtons]="true" [min]="18" [max]="100"
                     [class.ng-invalid]="f['age'].invalid && f['age'].touched">
      </p-inputNumber>
      <small *ngIf="f['age'].invalid && f['age'].touched" class="p-error">
        Age must be between 18 and 100
      </small>
    </div>
    
    <!-- Salary -->
    <div class="p-field">
      <label for="salary">Expected Salary*</label>
      <p-inputNumber id="salary" formControlName="salary" 
                     mode="currency" currency="USD" locale="en-US"
                     [class.ng-invalid]="f['salary'].invalid && f['salary'].touched">
      </p-inputNumber>
      <small *ngIf="f['salary'].invalid && f['salary'].touched" class="p-error">
        Salary is required
      </small>
    </div>
    
    <!-- Bio -->
    <div class="p-field">
      <label for="bio">Bio*</label>
      <textarea id="bio" pInputTextarea formControlName="bio" 
                [autoResize]="true" rows="5" [maxlength]="500"
                placeholder="Tell us about yourself..."
                [class.ng-invalid]="f['bio'].invalid && f['bio'].touched">
      </textarea>
      <small class="block">{{ f['bio'].value?.length || 0 }}/500 characters</small>
      <small *ngIf="f['bio'].invalid && f['bio'].touched" class="p-error block">
        Bio is required (max 500 characters)
      </small>
    </div>
    
    <!-- Website -->
    <div class="p-field">
      <label for="website">Website</label>
      <span class="p-input-icon-left">
        <i class="pi pi-globe"></i>
        <input id="website" type="url" pInputText formControlName="website" 
               placeholder="https://example.com" />
      </span>
    </div>
    
    <!-- Actions -->
    <div class="p-field">
      <p-button label="Save Profile" icon="pi pi-check" 
                type="submit" [disabled]="userForm.invalid"></p-button>
      <p-button label="Cancel" icon="pi pi-times" 
                severity="secondary" [text]="true" 
                type="button"></p-button>
    </div>
  </form>
</div>
```

```css
.card {
  max-width: 600px;
  margin: 20px auto;
  padding: 30px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.p-field {
  margin-bottom: 20px;
}

.p-field label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #495057;
}

.p-field input,
.p-field textarea {
  width: 100%;
}

.p-field p-inputNumber {
  width: 100%;
}

.p-error {
  color: #f44336;
  font-size: 0.875rem;
  margin-top: 4px;
}

.block {
  display: block;
}
```

---

✅ **Day 13 Complete!** Tomorrow: Checkbox, RadioButton & ToggleButton ☑️
