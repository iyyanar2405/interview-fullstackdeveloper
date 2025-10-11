# Day 11: PrimeNG Installation & Theme Setup 🎨

Get started with PrimeNG by installing it and setting up themes in your Angular application.

## 🎯 Learning Objectives

- Install PrimeNG and dependencies
- Configure PrimeNG in Angular project
- Understand PrimeNG theme system
- Apply and customize themes
- Set up PrimeIcons
- Configure global styles

## 📦 Installation Steps

### Step 1: Install PrimeNG

```bash
# Install PrimeNG
npm install primeng

# Install PrimeIcons
npm install primeicons

# Install Angular animations (required)
npm install @angular/animations
```

### Step 2: Configure angular.json

Update your `angular.json` to include PrimeNG styles:

```json
{
  "projects": {
    "your-app": {
      "architect": {
        "build": {
          "options": {
            "styles": [
              "node_modules/primeng/resources/themes/lara-light-blue/theme.css",
              "node_modules/primeng/resources/primeng.min.css",
              "node_modules/primeicons/primeicons.css",
              "src/styles.css"
            ]
          }
        }
      }
    }
  }
}
```

### Step 3: Import BrowserAnimationsModule

Update `app.module.ts`:

```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule  // Required for PrimeNG
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

## 🎨 Available Themes

PrimeNG provides numerous pre-built themes:

### Material Design Themes
- `md-light-indigo`
- `md-light-deeppurple`
- `md-dark-indigo`
- `md-dark-deeppurple`

### Bootstrap Themes
- `bootstrap4-light-blue`
- `bootstrap4-light-purple`
- `bootstrap4-dark-blue`
- `bootstrap4-dark-purple`

### Lara Themes (Modern)
- `lara-light-blue`
- `lara-light-indigo`
- `lara-light-purple`
- `lara-light-teal`
- `lara-dark-blue`
- `lara-dark-indigo`
- `lara-dark-purple`
- `lara-dark-teal`

### Other Themes
- `saga-blue`
- `saga-green`
- `saga-orange`
- `saga-purple`
- `vela-blue`
- `vela-green`
- `vela-orange`
- `vela-purple`
- `arya-blue`
- `arya-green`
- `arya-orange`
- `arya-purple`

## 💻 First PrimeNG Component

### Step 1: Import PrimeNG Module

Create or update `app.module.ts`:

```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// PrimeNG Modules
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';

import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    // PrimeNG Modules
    ButtonModule,
    CardModule,
    InputTextModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

### Step 2: Use PrimeNG Components

Update `app.component.html`:

```html
<div class="container">
  <h1>Welcome to PrimeNG!</h1>
  
  <!-- PrimeNG Card -->
  <p-card header="Getting Started" subheader="Your first PrimeNG application">
    <p>PrimeNG is a rich set of open source native Angular UI components.</p>
    
    <!-- PrimeNG Input -->
    <div class="p-field">
      <label for="username">Username</label>
      <input id="username" type="text" pInputText placeholder="Enter username" />
    </div>
    
    <!-- PrimeNG Buttons -->
    <div class="button-group">
      <p-button label="Primary" styleClass="p-button-primary"></p-button>
      <p-button label="Success" styleClass="p-button-success"></p-button>
      <p-button label="Info" styleClass="p-button-info"></p-button>
      <p-button label="Warning" styleClass="p-button-warning"></p-button>
      <p-button label="Danger" styleClass="p-button-danger"></p-button>
    </div>
  </p-card>
</div>
```

### Step 3: Add Component Styles

Update `app.component.css`:

```css
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

h1 {
  color: #495057;
  margin-bottom: 30px;
  text-align: center;
}

.p-field {
  margin-bottom: 20px;
}

.p-field label {
  display: block;
  margin-bottom: 5px;
  font-weight: 600;
}

.button-group {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
  margin-top: 20px;
}
```

## 🎨 Theme Customization

### Method 1: Using CSS Variables

Create `src/styles.css`:

```css
/* Override PrimeNG theme variables */
:root {
  --primary-color: #6366f1;
  --primary-color-text: #ffffff;
  --surface-0: #ffffff;
  --surface-50: #fafafa;
  --surface-100: #f5f5f5;
  --surface-200: #eeeeee;
  --text-color: #495057;
  --text-color-secondary: #6c757d;
}

/* Custom styles */
body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  margin: 0;
  padding: 0;
}

.p-component {
  font-family: inherit;
}
```

### Method 2: Theme Designer (Advanced)

PrimeNG provides a Theme Designer tool for creating custom themes:

1. Visit [PrimeNG Theme Designer](https://www.primefaces.org/designer/primeng)
2. Customize colors and styles
3. Export the theme
4. Include in your project

### Method 3: SCSS Customization

If using SCSS, create `src/styles.scss`:

```scss
// Import PrimeNG theme
@import "primeng/resources/themes/lara-light-blue/theme.css";
@import "primeng/resources/primeng.min.css";
@import "primeicons/primeicons.css";

// Custom variables
$primary-color: #6366f1;
$secondary-color: #64748b;
$success-color: #10b981;
$info-color: #3b82f6;
$warning-color: #f59e0b;
$danger-color: #ef4444;

// Override PrimeNG styles
.p-button {
  &.p-button-primary {
    background-color: $primary-color;
    border-color: $primary-color;
    
    &:hover {
      background-color: darken($primary-color, 10%);
      border-color: darken($primary-color, 10%);
    }
  }
}

.p-card {
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  border-radius: 8px;
}
```

## 🎯 PrimeIcons Usage

PrimeIcons provides 200+ icons for use with PrimeNG components.

```html
<!-- Using icons in buttons -->
<p-button icon="pi pi-check" label="Save"></p-button>
<p-button icon="pi pi-times" label="Cancel" styleClass="p-button-danger"></p-button>
<p-button icon="pi pi-search" label="Search" styleClass="p-button-info"></p-button>

<!-- Icon-only buttons -->
<p-button icon="pi pi-plus" styleClass="p-button-rounded"></p-button>
<p-button icon="pi pi-pencil" styleClass="p-button-rounded p-button-success"></p-button>
<p-button icon="pi pi-trash" styleClass="p-button-rounded p-button-danger"></p-button>

<!-- Icons in HTML -->
<i class="pi pi-home"></i>
<i class="pi pi-user"></i>
<i class="pi pi-envelope"></i>
<i class="pi pi-cog"></i>

<!-- Icons with custom size and color -->
<i class="pi pi-star" style="font-size: 2rem; color: gold;"></i>
```

## 🏃 Hands-On Practice

### Exercise 1: Create Welcome Page

```typescript
// app.component.ts
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'PrimeNG Demo';
  features = [
    { name: 'Rich Components', icon: 'pi-th-large' },
    { name: 'Responsive Design', icon: 'pi-mobile' },
    { name: 'Free & Open Source', icon: 'pi-github' },
    { name: 'Professional Support', icon: 'pi-users' }
  ];
}
```

```html
<!-- app.component.html -->
<div class="app-container">
  <header class="app-header">
    <h1><i class="pi pi-prime"></i> {{ title }}</h1>
    <p>Build amazing Angular applications with PrimeNG</p>
  </header>

  <div class="feature-grid">
    <p-card *ngFor="let feature of features" class="feature-card">
      <div class="feature-content">
        <i class="pi {{ feature.icon }}" style="font-size: 3rem; color: #6366f1;"></i>
        <h3>{{ feature.name }}</h3>
      </div>
    </p-card>
  </div>

  <div class="demo-section">
    <p-card header="Try PrimeNG Components">
      <div class="p-field">
        <label for="email">Email</label>
        <input id="email" type="email" pInputText placeholder="your@email.com" />
      </div>

      <div class="p-field">
        <label for="password">Password</label>
        <input id="password" type="password" pInputText placeholder="Enter password" />
      </div>

      <div class="button-row">
        <p-button label="Sign In" icon="pi pi-sign-in" styleClass="p-button-primary"></p-button>
        <p-button label="Register" icon="pi pi-user-plus" styleClass="p-button-success"></p-button>
      </div>
    </p-card>
  </div>
</div>
```

```css
/* app.component.css */
.app-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.app-header {
  text-align: center;
  padding: 40px 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 10px;
  margin-bottom: 40px;
}

.app-header h1 {
  margin: 0;
  font-size: 2.5em;
}

.app-header p {
  margin: 10px 0 0 0;
  font-size: 1.2em;
  opacity: 0.9;
}

.feature-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 40px;
}

.feature-card {
  text-align: center;
}

.feature-content {
  padding: 20px;
}

.feature-content h3 {
  margin-top: 15px;
  color: #495057;
}

.demo-section {
  max-width: 500px;
  margin: 0 auto;
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

.p-field input {
  width: 100%;
}

.button-row {
  display: flex;
  gap: 10px;
  justify-content: center;
  margin-top: 20px;
}
```

### Exercise 2: Theme Switcher

```typescript
// app.component.ts
import { Component } from '@angular/core';

interface Theme {
  name: string;
  file: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  themes: Theme[] = [
    { name: 'Lara Light Blue', file: 'lara-light-blue' },
    { name: 'Lara Dark Blue', file: 'lara-dark-blue' },
    { name: 'Lara Light Indigo', file: 'lara-light-indigo' },
    { name: 'Lara Dark Indigo', file: 'lara-dark-indigo' },
    { name: 'Material Light', file: 'md-light-indigo' },
    { name: 'Material Dark', file: 'md-dark-indigo' }
  ];
  
  selectedTheme: Theme = this.themes[0];
  
  changeTheme(theme: Theme) {
    this.selectedTheme = theme;
    const themeLink = document.getElementById('app-theme') as HTMLLinkElement;
    themeLink.href = `node_modules/primeng/resources/themes/${theme.file}/theme.css`;
  }
}
```

```html
<!-- index.html - add this in head -->
<link id="app-theme" rel="stylesheet" type="text/css" href="node_modules/primeng/resources/themes/lara-light-blue/theme.css">

<!-- app.component.html -->
<div class="theme-switcher">
  <p-card header="Theme Switcher">
    <p>Current Theme: {{ selectedTheme.name }}</p>
    <div class="theme-buttons">
      <p-button 
        *ngFor="let theme of themes"
        [label]="theme.name"
        (onClick)="changeTheme(theme)"
        [styleClass]="selectedTheme.file === theme.file ? 'p-button-success' : 'p-button-secondary'">
      </p-button>
    </div>
  </p-card>
</div>
```

## ✅ Day 11 Checklist

- [ ] PrimeNG installed successfully
- [ ] PrimeIcons installed
- [ ] BrowserAnimationsModule configured
- [ ] Theme imported in angular.json
- [ ] First PrimeNG component working
- [ ] Understand theme system
- [ ] Practice with different components
- [ ] Complete exercises

## 🎯 Key Takeaways

1. **PrimeNG** requires @angular/animations
2. **Themes** are easily switchable via CSS imports
3. **PrimeIcons** provides 200+ icons
4. **Each component** needs its module imported
5. **Customization** is possible through CSS variables

## 📚 Additional Resources

- [PrimeNG Showcase](https://primeng.org/showcase)
- [PrimeNG GitHub](https://github.com/primefaces/primeng)
- [PrimeIcons](https://primeng.org/icons)
- [Theme Designer](https://www.primefaces.org/designer/primeng)

## 🚀 Next Steps

Tomorrow in **Day 12: Button, InputText, Textarea Components**, you'll learn:
- Button component and variants
- Input components
- Form field basics
- Component styling

---

**Excellent Work!** 🎉 You've successfully set up PrimeNG in your Angular application!
