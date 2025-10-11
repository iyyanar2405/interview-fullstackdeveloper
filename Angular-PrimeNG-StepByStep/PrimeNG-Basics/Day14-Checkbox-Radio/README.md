# Day 14: Checkbox, RadioButton & ToggleButton ☑️

## 📋 Learning Objectives
- Use Checkbox for binary and multiple selections
- Implement RadioButton groups
- Master ToggleButton for on/off states
- Apply TriStateCheckbox
- Handle form validation with selection controls

---

## ☑️ Checkbox Component

### Installation
```typescript
import { CheckboxModule } from 'primeng/checkbox';
```

### Basic Checkbox (Binary)
```html
<!-- Binary checkbox -->
<p-checkbox [(ngModel)]="checked" [binary]="true" inputId="binary"></p-checkbox>
<label for="binary">Accept Terms</label>

<!-- With label -->
<p-checkbox [(ngModel)]="checked" [binary]="true" label="I agree" inputId="agree"></p-checkbox>

<!-- Disabled -->
<p-checkbox [(ngModel)]="checked" [binary]="true" [disabled]="true" label="Disabled"></p-checkbox>
```

```typescript
export class CheckboxDemoComponent {
  checked: boolean = false;
}
```

### Multiple Checkboxes
```html
<div>
  <h3>Select your interests:</h3>
  <div *ngFor="let category of categories" class="p-field-checkbox">
    <p-checkbox 
      [value]="category"
      [(ngModel)]="selectedCategories"
      [inputId]="category.key">
    </p-checkbox>
    <label [for]="category.key">{{ category.name }}</label>
  </div>
</div>

<p>Selected: {{ selectedCategories | json }}</p>
```

```typescript
export class CheckboxDemoComponent {
  categories = [
    { name: 'Technology', key: 'tech' },
    { name: 'Sports', key: 'sports' },
    { name: 'Music', key: 'music' },
    { name: 'Travel', key: 'travel' }
  ];
  
  selectedCategories: any[] = [];
}
```

---

## 🔘 RadioButton Component

### Installation
```typescript
import { RadioButtonModule } from 'primeng/radiobutton';
```

### Basic RadioButton
```html
<div>
  <h3>Select your plan:</h3>
  
  <div class="p-field-radiobutton">
    <p-radioButton 
      value="basic"
      [(ngModel)]="selectedPlan"
      inputId="basic">
    </p-radioButton>
    <label for="basic">Basic - $9.99/month</label>
  </div>
  
  <div class="p-field-radiobutton">
    <p-radioButton 
      value="premium"
      [(ngModel)]="selectedPlan"
      inputId="premium">
    </p-radioButton>
    <label for="premium">Premium - $19.99/month</label>
  </div>
  
  <div class="p-field-radiobutton">
    <p-radioButton 
      value="enterprise"
      [(ngModel)]="selectedPlan"
      inputId="enterprise">
    </p-radioButton>
    <label for="enterprise">Enterprise - Contact Us</label>
  </div>
</div>

<p>Selected Plan: {{ selectedPlan }}</p>
```

```typescript
export class RadioButtonDemoComponent {
  selectedPlan: string = 'basic';
}
```

### RadioButton with Array
```html
<div>
  <h3>Choose payment method:</h3>
  <div *ngFor="let method of paymentMethods" class="p-field-radiobutton">
    <p-radioButton 
      [value]="method.key"
      [(ngModel)]="selectedPayment"
      [inputId]="method.key">
    </p-radioButton>
    <label [for]="method.key">
      <i [class]="method.icon"></i>
      {{ method.name }}
    </label>
  </div>
</div>
```

```typescript
export class RadioButtonDemoComponent {
  paymentMethods = [
    { name: 'Credit Card', key: 'CC', icon: 'pi pi-credit-card' },
    { name: 'PayPal', key: 'PP', icon: 'pi pi-paypal' },
    { name: 'Bank Transfer', key: 'BT', icon: 'pi pi-building' }
  ];
  
  selectedPayment: string = 'CC';
}
```

---

## 🔄 ToggleButton Component

### Installation
```typescript
import { InputSwitchModule } from 'primeng/inputswitch';
```

### Basic ToggleButton
```html
<!-- InputSwitch (modern toggle) -->
<p-inputSwitch [(ngModel)]="checked"></p-inputSwitch>
<label>Enable Notifications</label>

<!-- With change event -->
<p-inputSwitch 
  [(ngModel)]="darkMode"
  (onChange)="onToggleChange($event)">
</p-inputSwitch>
<label>Dark Mode</label>
```

```typescript
export class ToggleButtonDemoComponent {
  checked: boolean = false;
  darkMode: boolean = false;
  
  onToggleChange(event: any) {
    console.log('Toggle changed:', event.checked);
    // Apply dark mode theme
  }
}
```

### Multiple Toggles
```html
<div class="settings-panel">
  <div class="setting-item">
    <label>Email Notifications</label>
    <p-inputSwitch [(ngModel)]="settings.emailNotifications"></p-inputSwitch>
  </div>
  
  <div class="setting-item">
    <label>Push Notifications</label>
    <p-inputSwitch [(ngModel)]="settings.pushNotifications"></p-inputSwitch>
  </div>
  
  <div class="setting-item">
    <label>Two-Factor Authentication</label>
    <p-inputSwitch [(ngModel)]="settings.twoFactor"></p-inputSwitch>
  </div>
  
  <div class="setting-item">
    <label>Auto-save</label>
    <p-inputSwitch [(ngModel)]="settings.autoSave"></p-inputSwitch>
  </div>
</div>

<pre>{{ settings | json }}</pre>
```

```typescript
export class ToggleButtonDemoComponent {
  settings = {
    emailNotifications: true,
    pushNotifications: false,
    twoFactor: true,
    autoSave: true
  };
}
```

---

## 🔺 TriStateCheckbox

### Installation
```typescript
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
```

### Basic TriStateCheckbox
```html
<!-- Three states: true, false, null -->
<p-triStateCheckbox [(ngModel)]="value" inputId="tri"></p-triStateCheckbox>
<label for="tri">{{ getLabel() }}</label>
```

```typescript
export class TriStateCheckboxDemoComponent {
  value: any = null;
  
  getLabel(): string {
    if (this.value === null) return 'Undecided';
    if (this.value === true) return 'Yes';
    return 'No';
  }
}
```

---

## 🎯 Practical Exercise: User Preferences Form

```typescript
export class UserPreferencesComponent implements OnInit {
  preferencesForm!: FormGroup;
  
  interests = [
    { name: 'Technology', key: 'tech' },
    { name: 'Sports', key: 'sports' },
    { name: 'Music', key: 'music' },
    { name: 'Travel', key: 'travel' },
    { name: 'Food', key: 'food' },
    { name: 'Gaming', key: 'gaming' }
  ];
  
  themes = [
    { name: 'Light', key: 'light' },
    { name: 'Dark', key: 'dark' },
    { name: 'Auto', key: 'auto' }
  ];
  
  languages = [
    { name: 'English', key: 'en' },
    { name: 'Spanish', key: 'es' },
    { name: 'French', key: 'fr' },
    { name: 'German', key: 'de' }
  ];
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.preferencesForm = this.fb.group({
      interests: [[]],
      theme: ['light'],
      language: ['en'],
      emailNotifications: [true],
      smsNotifications: [false],
      pushNotifications: [true],
      newsletter: [null],
      publicProfile: [false],
      showEmail: [false],
      allowMessages: [true]
    });
  }
  
  onSubmit() {
    if (this.preferencesForm.valid) {
      console.log('Preferences:', this.preferencesForm.value);
      // Save to backend
    }
  }
  
  resetForm() {
    this.preferencesForm.reset({
      interests: [],
      theme: 'light',
      language: 'en',
      emailNotifications: true,
      smsNotifications: false,
      pushNotifications: true,
      newsletter: null,
      publicProfile: false,
      showEmail: false,
      allowMessages: true
    });
  }
}
```

```html
<div class="preferences-card">
  <h2>User Preferences</h2>
  
  <form [formGroup]="preferencesForm" (ngSubmit)="onSubmit()">
    
    <!-- Interests Section -->
    <div class="section">
      <h3>Interests</h3>
      <p class="section-description">Select topics you're interested in</p>
      
      <div class="checkbox-grid">
        <div *ngFor="let interest of interests" class="p-field-checkbox">
          <p-checkbox 
            [value]="interest"
            formControlName="interests"
            [inputId]="interest.key">
          </p-checkbox>
          <label [for]="interest.key">{{ interest.name }}</label>
        </div>
      </div>
    </div>
    
    <!-- Theme Selection -->
    <div class="section">
      <h3>Theme Preference</h3>
      <div class="radio-group">
        <div *ngFor="let theme of themes" class="p-field-radiobutton">
          <p-radioButton 
            [value]="theme.key"
            formControlName="theme"
            [inputId]="theme.key">
          </p-radioButton>
          <label [for]="theme.key">{{ theme.name }}</label>
        </div>
      </div>
    </div>
    
    <!-- Language Selection -->
    <div class="section">
      <h3>Language</h3>
      <div class="radio-group">
        <div *ngFor="let lang of languages" class="p-field-radiobutton">
          <p-radioButton 
            [value]="lang.key"
            formControlName="language"
            [inputId]="lang.key">
          </p-radioButton>
          <label [for]="lang.key">{{ lang.name }}</label>
        </div>
      </div>
    </div>
    
    <!-- Notification Settings -->
    <div class="section">
      <h3>Notifications</h3>
      
      <div class="toggle-item">
        <label>Email Notifications</label>
        <p-inputSwitch formControlName="emailNotifications"></p-inputSwitch>
      </div>
      
      <div class="toggle-item">
        <label>SMS Notifications</label>
        <p-inputSwitch formControlName="smsNotifications"></p-inputSwitch>
      </div>
      
      <div class="toggle-item">
        <label>Push Notifications</label>
        <p-inputSwitch formControlName="pushNotifications"></p-inputSwitch>
      </div>
      
      <div class="toggle-item">
        <label>Newsletter Subscription</label>
        <p-triStateCheckbox formControlName="newsletter"></p-triStateCheckbox>
        <small class="help-text">
          {{ preferencesForm.get('newsletter')?.value === null ? 'Not decided' : 
             preferencesForm.get('newsletter')?.value ? 'Subscribed' : 'Unsubscribed' }}
        </small>
      </div>
    </div>
    
    <!-- Privacy Settings -->
    <div class="section">
      <h3>Privacy</h3>
      
      <div class="p-field-checkbox">
        <p-checkbox 
          formControlName="publicProfile"
          [binary]="true"
          inputId="publicProfile">
        </p-checkbox>
        <label for="publicProfile">Make my profile public</label>
      </div>
      
      <div class="p-field-checkbox">
        <p-checkbox 
          formControlName="showEmail"
          [binary]="true"
          inputId="showEmail">
        </p-checkbox>
        <label for="showEmail">Show email on profile</label>
      </div>
      
      <div class="p-field-checkbox">
        <p-checkbox 
          formControlName="allowMessages"
          [binary]="true"
          inputId="allowMessages">
        </p-checkbox>
        <label for="allowMessages">Allow messages from other users</label>
      </div>
    </div>
    
    <!-- Form Actions -->
    <div class="form-actions">
      <p-button 
        label="Save Preferences" 
        icon="pi pi-check"
        type="submit">
      </p-button>
      <p-button 
        label="Reset" 
        icon="pi pi-refresh"
        severity="secondary"
        type="button"
        (onClick)="resetForm()">
      </p-button>
    </div>
  </form>
  
  <!-- Preview -->
  <div class="preview-section">
    <h3>Preview</h3>
    <pre>{{ preferencesForm.value | json }}</pre>
  </div>
</div>
```

```css
.preferences-card {
  max-width: 800px;
  margin: 20px auto;
  padding: 30px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.section {
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 1px solid #e0e0e0;
}

.section:last-of-type {
  border-bottom: none;
}

.section h3 {
  margin-bottom: 10px;
  color: #333;
}

.section-description {
  color: #666;
  font-size: 14px;
  margin-bottom: 15px;
}

.checkbox-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 15px;
}

.p-field-checkbox,
.p-field-radiobutton {
  display: flex;
  align-items: center;
  gap: 10px;
}

.radio-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.toggle-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
}

.toggle-item:last-child {
  border-bottom: none;
}

.toggle-item label {
  flex: 1;
  font-weight: 500;
}

.help-text {
  display: block;
  color: #666;
  font-size: 12px;
  margin-top: 4px;
}

.form-actions {
  display: flex;
  gap: 15px;
  margin-top: 30px;
}

.preview-section {
  margin-top: 30px;
  padding: 20px;
  background: #f5f5f5;
  border-radius: 4px;
}

.preview-section h3 {
  margin-bottom: 10px;
}

.preview-section pre {
  background: white;
  padding: 15px;
  border-radius: 4px;
  overflow-x: auto;
}

@media (max-width: 768px) {
  .preferences-card {
    padding: 20px;
  }
  
  .checkbox-grid {
    grid-template-columns: 1fr;
  }
  
  .form-actions {
    flex-direction: column;
  }
}
```

---

## ✅ Day 14 Checklist

- [ ] Implemented binary checkboxes
- [ ] Created multiple checkbox selections
- [ ] Used RadioButton groups
- [ ] Applied InputSwitch toggles
- [ ] Implemented TriStateCheckbox
- [ ] Built user preferences form (Exercise)
- [ ] Validated selection controls
- [ ] Applied styling and layout

---

## 🔑 Key Takeaways

1. **Checkbox** - Use for multiple selections or binary choices
2. **RadioButton** - Use for single selection from multiple options
3. **InputSwitch** - Modern toggle for on/off states
4. **TriStateCheckbox** - For three-state selections (yes/no/undecided)
5. **Form integration** - All controls work with reactive forms
6. **Validation** - Apply validators for required selections
7. **Styling** - Use PrimeNG classes for consistent UI

---

## 📚 Additional Resources

- [PrimeNG Checkbox](https://primeng.org/checkbox)
- [PrimeNG RadioButton](https://primeng.org/radiobutton)
- [PrimeNG InputSwitch](https://primeng.org/inputswitch)
- [PrimeNG TriStateCheckbox](https://primeng.org/tristatecheckbox)

---

✅ **Day 14 Complete!** Tomorrow: Dropdown & MultiSelect 📋
