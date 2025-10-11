# Day 29: SASS Customization & Theming 🎨

Learn to customize Bootstrap at the source level using SASS variables, create custom themes, and build a unique design system.

## 🎯 Learning Objectives

- Understand Bootstrap's SASS architecture
- Customize variables and create themes
- Override default styles systematically
- Build a custom color palette
- Create reusable SASS mixins
- Compile custom Bootstrap builds

## 📚 Bootstrap SASS Structure

```scss
bootstrap/
├── scss/
│   ├── _functions.scss       // Utility functions
│   ├── _variables.scss        // Default variables
│   ├── _maps.scss             // Color & breakpoint maps
│   ├── _mixins.scss           // Reusable mixins
│   ├── _utilities.scss        // Utility API
│   ├── _root.scss             // CSS custom properties
│   ├── _reboot.scss           // Normalize styles
│   └── bootstrap.scss         // Main import file
```

## 💻 Custom Theme Example

### Step 1: Create `custom.scss`

```scss
// 1. Include functions first (so you can manipulate colors, SVGs, calc, etc)
@import "../node_modules/bootstrap/scss/functions";

// 2. Include any default variable overrides here
$primary: #6366f1;
$secondary: #8b5cf6;
$success: #10b981;
$info: #3b82f6;
$warning: #f59e0b;
$danger: #ef4444;

$body-bg: #f8fafc;
$body-color: #1e293b;

$font-family-base: 'Inter', system-ui, -apple-system, sans-serif;
$font-size-base: 1rem;
$line-height-base: 1.6;

// Customize spacing
$spacer: 1rem;
$spacers: (
  0: 0,
  1: $spacer * .25,
  2: $spacer * .5,
  3: $spacer,
  4: $spacer * 1.5,
  5: $spacer * 3,
  6: $spacer * 4,
  7: $spacer * 5,
);

// Border radius
$border-radius: .75rem;
$border-radius-sm: .5rem;
$border-radius-lg: 1rem;

// Shadows
$box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
$box-shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
$box-shadow-lg: 0 20px 25px -5px rgba(0, 0, 0, 0.1);

// Button customization
$btn-padding-y: .75rem;
$btn-padding-x: 1.5rem;
$btn-font-weight: 600;
$btn-border-radius: $border-radius;

// Card customization
$card-border-radius: $border-radius;
$card-box-shadow: $box-shadow;
$card-cap-bg: transparent;

// 3. Include remainder of required Bootstrap stylesheets
@import "../node_modules/bootstrap/scss/variables";

// 4. Include any default map overrides here
$theme-colors: (
  "primary": $primary,
  "secondary": $secondary,
  "success": $success,
  "info": $info,
  "warning": $warning,
  "danger": $danger,
  "light": #f1f5f9,
  "dark": #0f172a,
);

// 5. Include remainder of required parts
@import "../node_modules/bootstrap/scss/maps";
@import "../node_modules/bootstrap/scss/mixins";
@import "../node_modules/bootstrap/scss/root";

// 6. Optionally include any other parts as needed
@import "../node_modules/bootstrap/scss/utilities";
@import "../node_modules/bootstrap/scss/reboot";
@import "../node_modules/bootstrap/scss/type";
@import "../node_modules/bootstrap/scss/images";
@import "../node_modules/bootstrap/scss/containers";
@import "../node_modules/bootstrap/scss/grid";
@import "../node_modules/bootstrap/scss/tables";
@import "../node_modules/bootstrap/scss/forms";
@import "../node_modules/bootstrap/scss/buttons";
@import "../node_modules/bootstrap/scss/transitions";
@import "../node_modules/bootstrap/scss/dropdown";
@import "../node_modules/bootstrap/scss/button-group";
@import "../node_modules/bootstrap/scss/nav";
@import "../node_modules/bootstrap/scss/navbar";
@import "../node_modules/bootstrap/scss/card";
@import "../node_modules/bootstrap/scss/accordion";
@import "../node_modules/bootstrap/scss/breadcrumb";
@import "../node_modules/bootstrap/scss/pagination";
@import "../node_modules/bootstrap/scss/badge";
@import "../node_modules/bootstrap/scss/alert";
@import "../node_modules/bootstrap/scss/progress";
@import "../node_modules/bootstrap/scss/list-group";
@import "../node_modules/bootstrap/scss/close";
@import "../node_modules/bootstrap/scss/toasts";
@import "../node_modules/bootstrap/scss/modal";
@import "../node_modules/bootstrap/scss/tooltip";
@import "../node_modules/bootstrap/scss/popover";
@import "../node_modules/bootstrap/scss/carousel";
@import "../node_modules/bootstrap/scss/spinners";
@import "../node_modules/bootstrap/scss/offcanvas";
@import "../node_modules/bootstrap/scss/placeholders";

// 7. Optionally include utilities API last to generate classes based on the Sass map in `_utilities.scss`
@import "../node_modules/bootstrap/scss/helpers";
@import "../node_modules/bootstrap/scss/utilities/api";

// 8. Add custom styles
.btn-gradient {
  background: linear-gradient(135deg, $primary 0%, $secondary 100%);
  border: none;
  color: white;
  
  &:hover {
    transform: translateY(-2px);
    box-shadow: $box-shadow-lg;
  }
}

.card-hover {
  transition: all 0.3s ease;
  
  &:hover {
    transform: translateY(-5px);
    box-shadow: $box-shadow-lg;
  }
}
```

## 💻 Complete Demo HTML

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 29: SASS Customization & Theming</title>
    
    <!-- Use your custom compiled CSS instead of Bootstrap CDN -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    
    <style>
        /* Simulating custom SASS compilation results */
        :root {
            --bs-primary: #6366f1;
            --bs-secondary: #8b5cf6;
            --bs-success: #10b981;
            --bs-border-radius: 0.75rem;
        }
        
        body {
            background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
        }
        
        .btn-gradient {
            background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
            border: none;
            color: white;
            padding: 0.75rem 1.5rem;
            font-weight: 600;
            border-radius: 0.75rem;
            transition: all 0.3s ease;
        }
        
        .btn-gradient:hover {
            transform: translateY(-2px);
            box-shadow: 0 20px 25px -5px rgba(99, 102, 241, 0.3);
            color: white;
        }
        
        .card-hover {
            transition: all 0.3s ease;
            border-radius: 0.75rem;
            border: none;
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
        }
        
        .card-hover:hover {
            transform: translateY(-5px);
            box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.2);
        }
        
        .demo-section {
            background: white;
            border-radius: 1rem;
            padding: 3rem;
            margin-bottom: 2rem;
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
        }
        
        .color-swatch {
            width: 100%;
            height: 100px;
            border-radius: 0.75rem;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-weight: 600;
            margin-bottom: 0.5rem;
        }
    </style>
</head>
<body>
    <div class="container py-5">
        <header class="text-center mb-5">
            <h1 class="display-3 fw-bold" style="background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%); -webkit-background-clip: text; -webkit-text-fill-color: transparent;">
                Day 29: SASS Customization
            </h1>
            <p class="lead text-muted">Build custom themes with Bootstrap SASS</p>
        </header>
        
        <!-- Custom Color Palette -->
        <section class="demo-section">
            <h2 class="mb-4">
                <i class="bi bi-palette me-2"></i>
                Custom Color Palette
            </h2>
            
            <div class="row g-3">
                <div class="col-md-3">
                    <div class="color-swatch" style="background: #6366f1;">Primary</div>
                    <code>#6366f1</code>
                </div>
                <div class="col-md-3">
                    <div class="color-swatch" style="background: #8b5cf6;">Secondary</div>
                    <code>#8b5cf6</code>
                </div>
                <div class="col-md-3">
                    <div class="color-swatch" style="background: #10b981;">Success</div>
                    <code>#10b981</code>
                </div>
                <div class="col-md-3">
                    <div class="color-swatch" style="background: #ef4444;">Danger</div>
                    <code>#ef4444</code>
                </div>
            </div>
        </section>
        
        <!-- Custom Buttons -->
        <section class="demo-section">
            <h2 class="mb-4">
                <i class="bi bi-mouse me-2"></i>
                Custom Button Styles
            </h2>
            
            <div class="d-flex flex-wrap gap-3">
                <button class="btn btn-primary">Custom Primary</button>
                <button class="btn btn-secondary">Custom Secondary</button>
                <button class="btn btn-success">Custom Success</button>
                <button class="btn-gradient">Gradient Button</button>
                <button class="btn btn-outline-primary">Outline Primary</button>
            </div>
        </section>
        
        <!-- Custom Cards -->
        <section class="demo-section">
            <h2 class="mb-4">
                <i class="bi bi-card-heading me-2"></i>
                Custom Card Components
            </h2>
            
            <div class="row g-4">
                <div class="col-md-4">
                    <div class="card card-hover">
                        <div class="card-body">
                            <h5 class="card-title">Feature One</h5>
                            <p class="card-text">Custom styled card with hover effects.</p>
                            <a href="#" class="btn-gradient text-decoration-none px-4 py-2 rounded">Learn More</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card card-hover">
                        <div class="card-body">
                            <h5 class="card-title">Feature Two</h5>
                            <p class="card-text">Smooth transitions and shadows.</p>
                            <a href="#" class="btn-gradient text-decoration-none px-4 py-2 rounded">Learn More</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card card-hover">
                        <div class="card-body">
                            <h5 class="card-title">Feature Three</h5>
                            <p class="card-text">Modern design aesthetic.</p>
                            <a href="#" class="btn-gradient text-decoration-none px-4 py-2 rounded">Learn More</a>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- SASS Variables Reference -->
        <section class="demo-section">
            <h2 class="mb-4">
                <i class="bi bi-code-slash me-2"></i>
                Common SASS Variables to Customize
            </h2>
            
            <div class="table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Variable</th>
                            <th>Default</th>
                            <th>Description</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><code>$primary</code></td>
                            <td>#0d6efd</td>
                            <td>Primary brand color</td>
                        </tr>
                        <tr>
                            <td><code>$border-radius</code></td>
                            <td>0.375rem</td>
                            <td>Default border radius</td>
                        </tr>
                        <tr>
                            <td><code>$font-family-base</code></td>
                            <td>system-ui</td>
                            <td>Base font family</td>
                        </tr>
                        <tr>
                            <td><code>$spacer</code></td>
                            <td>1rem</td>
                            <td>Base spacing unit</td>
                        </tr>
                        <tr>
                            <td><code>$box-shadow</code></td>
                            <td>Various</td>
                            <td>Default shadow styles</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </section>
        
        <!-- Setup Instructions -->
        <section class="demo-section">
            <h2 class="mb-4">
                <i class="bi bi-gear me-2"></i>
                Setup Instructions
            </h2>
            
            <ol class="lead">
                <li class="mb-3">
                    <strong>Install Bootstrap via npm:</strong>
                    <pre class="bg-dark text-light p-3 rounded mt-2"><code>npm install bootstrap@5.3.2</code></pre>
                </li>
                <li class="mb-3">
                    <strong>Create your custom SASS file</strong> (e.g., <code>custom.scss</code>)
                </li>
                <li class="mb-3">
                    <strong>Import Bootstrap SASS with your customizations</strong>
                </li>
                <li class="mb-3">
                    <strong>Compile SASS to CSS:</strong>
                    <pre class="bg-dark text-light p-3 rounded mt-2"><code>sass custom.scss custom.css</code></pre>
                </li>
                <li>
                    <strong>Link your custom CSS</strong> in your HTML
                </li>
            </ol>
        </section>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>console.log('🎨 Day 29 - SASS Customization loaded!');</script>
</body>
</html>
```

## 📋 Checklist

- [ ] Understand Bootstrap SASS structure
- [ ] Override color variables
- [ ] Customize typography
- [ ] Modify spacing scale
- [ ] Adjust border radius
- [ ] Create custom shadows
- [ ] Build gradient utilities
- [ ] Compile custom build
- [ ] Test responsiveness
- [ ] Document theme variables

## 🎯 Key Takeaways

1. **Always import functions first** - needed for color manipulation
2. **Override variables before importing** - the order matters
3. **Use SASS maps** for theme colors and spacers
4. **Create reusable mixins** for common patterns
5. **Compile for production** - minify your custom CSS

**Next: Day 30 - Bootstrap JavaScript Plugins** ⚡
