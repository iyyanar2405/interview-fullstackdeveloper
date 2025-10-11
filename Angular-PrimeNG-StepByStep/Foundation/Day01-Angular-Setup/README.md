# Day 01: Angular Setup & Environment Configuration 🚀

Learn how to set up your Angular development environment and create your first Angular application.

## 🎯 Learning Objectives

- Install Node.js and npm
- Install Angular CLI
- Create a new Angular project
- Understand Angular project structure
- Run your first Angular application
- Use Angular DevTools

## 📋 Prerequisites

- Basic command line knowledge
- Text editor (VS Code recommended)
- Basic HTML/CSS/JavaScript knowledge

## 🛠️ Installation Steps

### 1. Install Node.js

Download and install Node.js from [nodejs.org](https://nodejs.org/)

```bash
# Verify Node.js installation
node --version
# Should output: v18.x.x or higher

# Verify npm installation
npm --version
# Should output: 9.x.x or higher
```

### 2. Install Angular CLI

```bash
# Install Angular CLI globally
npm install -g @angular/cli

# Verify Angular CLI installation
ng version

# Output should show:
# Angular CLI: 17.x.x
# Node: 18.x.x
# Package Manager: npm 9.x.x
```

### 3. Create Your First Angular Project

```bash
# Create new Angular project
ng new my-first-app

# You'll be prompted with:
# ? Would you like to add Angular routing? (y/N) → Type 'y'
# ? Which stylesheet format would you like to use? → Select 'CSS'

# Navigate to project directory
cd my-first-app

# Run the development server
ng serve

# Open browser at: http://localhost:4200
```

## 📁 Angular Project Structure

```
my-first-app/
├── node_modules/          # Dependencies
├── src/                   # Source code
│   ├── app/              # Application components
│   │   ├── app.component.ts      # Root component
│   │   ├── app.component.html    # Root template
│   │   ├── app.component.css     # Root styles
│   │   ├── app.component.spec.ts # Unit tests
│   │   └── app.module.ts         # Root module
│   ├── assets/           # Static files (images, etc.)
│   ├── environments/     # Environment configurations
│   ├── index.html        # Main HTML file
│   ├── main.ts          # Entry point
│   └── styles.css       # Global styles
├── angular.json          # Angular configuration
├── package.json          # npm dependencies
├── tsconfig.json         # TypeScript configuration
└── README.md            # Project documentation
```

## 💻 Understanding Key Files

### 1. **app.component.ts** (Component TypeScript File)

```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',           // HTML tag name
  templateUrl: './app.component.html',  // Template file
  styleUrls: ['./app.component.css']    // Style file
})
export class AppComponent {
  title = 'my-first-app';         // Component property
}
```

### 2. **app.component.html** (Component Template)

```html
<div class="container">
  <h1>Welcome to {{ title }}!</h1>
  <p>Your Angular app is running successfully!</p>
</div>
```

### 3. **app.component.css** (Component Styles)

```css
.container {
  text-align: center;
  padding: 50px;
  font-family: Arial, sans-serif;
}

h1 {
  color: #dd0031;
  font-size: 3em;
}

p {
  color: #666;
  font-size: 1.2em;
}
```

### 4. **app.module.ts** (Application Module)

```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent    // Components, directives, pipes
  ],
  imports: [
    BrowserModule   // Required modules
  ],
  providers: [],    // Services
  bootstrap: [AppComponent]  // Root component
})
export class AppModule { }
```

## 🔧 Essential Angular CLI Commands

```bash
# Create new component
ng generate component component-name
# Shorthand: ng g c component-name

# Create new service
ng generate service service-name
# Shorthand: ng g s service-name

# Create new module
ng generate module module-name
# Shorthand: ng g m module-name

# Create new directive
ng generate directive directive-name

# Create new pipe
ng generate pipe pipe-name

# Run development server
ng serve
# With custom port: ng serve --port 4300

# Build for production
ng build
# Build with optimization: ng build --prod

# Run unit tests
ng test

# Run end-to-end tests
ng e2e

# Generate documentation
ng doc component-name
```

## 🎨 VS Code Extensions (Recommended)

Install these extensions for better Angular development:

1. **Angular Language Service** - IntelliSense for Angular
2. **Angular Snippets** - Code snippets
3. **Prettier** - Code formatter
4. **ESLint** - Linting
5. **Angular DevTools** - Browser extension

```bash
# Install Angular DevTools (Chrome/Edge)
# Search "Angular DevTools" in browser extension store
```

## 🏃 Hands-On Practice

### Exercise 1: Modify the Default App

Update `app.component.ts`:

```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'My Angular Journey';
  subtitle = 'Learning Angular with PrimeNG';
  currentDate = new Date();
  
  features = [
    'Component-Based Architecture',
    'Two-Way Data Binding',
    'Dependency Injection',
    'Reactive Programming'
  ];
}
```

Update `app.component.html`:

```html
<div class="app-container">
  <header>
    <h1>{{ title }}</h1>
    <h2>{{ subtitle }}</h2>
    <p>Today is: {{ currentDate | date:'fullDate' }}</p>
  </header>
  
  <main>
    <h3>Why Learn Angular?</h3>
    <ul>
      <li *ngFor="let feature of features">{{ feature }}</li>
    </ul>
  </main>
  
  <footer>
    <p>&copy; 2024 Angular Learning Journey</p>
  </footer>
</div>
```

Update `app.component.css`:

```css
.app-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

header {
  text-align: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 40px;
  border-radius: 10px;
  margin-bottom: 30px;
}

header h1 {
  margin: 0;
  font-size: 2.5em;
}

header h2 {
  margin: 10px 0;
  font-weight: 300;
}

main {
  background: #f5f5f5;
  padding: 30px;
  border-radius: 10px;
}

main h3 {
  color: #667eea;
  margin-top: 0;
}

ul {
  list-style: none;
  padding: 0;
}

li {
  background: white;
  margin: 10px 0;
  padding: 15px;
  border-left: 4px solid #667eea;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

footer {
  text-align: center;
  margin-top: 30px;
  color: #666;
}
```

### Exercise 2: Add Interactivity

Add a button with click event:

```typescript
// app.component.ts
export class AppComponent {
  title = 'My Angular Journey';
  clickCount = 0;
  
  handleClick() {
    this.clickCount++;
    console.log('Button clicked!', this.clickCount);
  }
}
```

```html
<!-- app.component.html -->
<div class="interactive-section">
  <button (click)="handleClick()">Click Me!</button>
  <p>Button clicked {{ clickCount }} times</p>
</div>
```

## 📝 Common Issues & Solutions

### Issue 1: `ng` command not found

**Solution:**
```bash
# Reinstall Angular CLI
npm uninstall -g @angular/cli
npm install -g @angular/cli
```

### Issue 2: Port 4200 already in use

**Solution:**
```bash
# Use different port
ng serve --port 4300
```

### Issue 3: Module not found errors

**Solution:**
```bash
# Clear npm cache and reinstall
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

## ✅ Day 1 Checklist

- [ ] Node.js and npm installed
- [ ] Angular CLI installed
- [ ] First Angular project created
- [ ] Development server running
- [ ] Project structure understood
- [ ] Basic component modified
- [ ] Angular DevTools installed
- [ ] VS Code extensions installed

## 🎯 Key Takeaways

1. **Angular CLI** is essential for Angular development
2. **Components** are the building blocks of Angular apps
3. **Modules** organize the application
4. **Data binding** connects template and component
5. **Angular follows** component-based architecture

## 📚 Additional Resources

- [Angular Official Tutorial](https://angular.io/tutorial)
- [Angular CLI Documentation](https://angular.io/cli)
- [Angular Style Guide](https://angular.io/guide/styleguide)
- [TypeScript in 5 Minutes](https://www.typescriptlang.org/docs/handbook/typescript-in-5-minutes.html)

## 🚀 Next Steps

Tomorrow in **Day 02: TypeScript Fundamentals**, you'll learn:
- TypeScript basics
- Types and interfaces
- Classes and decorators
- ES6+ features used in Angular

---

**Congratulations!** 🎉 You've completed Day 1 and set up your Angular development environment!
