# Day 15: Button Components & Interactions 🔘

Welcome to Day 15 - the beginning of Bootstrap Components mastery! Today you'll explore Bootstrap's comprehensive button system, learning to create engaging, accessible, and interactive button components that form the foundation of modern web interfaces.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master all Bootstrap button variants and styles
- Create custom button combinations and groups
- Implement advanced button states and interactions
- Build accessible button components with ARIA support
- Design responsive button layouts across breakpoints
- Integrate buttons with JavaScript for dynamic behavior
- Optimize button performance and loading states
- Create professional button-driven interfaces

## 📚 Bootstrap Button System Deep Dive

### Button Fundamentals

Bootstrap provides a comprehensive button system with multiple variants, sizes, and states designed for maximum flexibility and accessibility.

### Core Button Classes
```css
/* Base button class - always required */
.btn

/* Style variants */
.btn-primary, .btn-secondary, .btn-success
.btn-danger, .btn-warning, .btn-info
.btn-light, .btn-dark, .btn-link

/* Outline variants */
.btn-outline-primary, .btn-outline-secondary
.btn-outline-success, .btn-outline-danger
.btn-outline-warning, .btn-outline-info
.btn-outline-light, .btn-outline-dark

/* Size variants */
.btn-lg, .btn-sm

/* State classes */
.active, .disabled
```

## 💻 Comprehensive Button Components

### Complete Button Showcase

Create `button-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 15: Bootstrap Button Components | Advanced Button System</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <style>
        :root {
            --custom-primary: #6f42c1;
            --custom-success: #198754;
            --custom-danger: #dc3545;
            --custom-warning: #ffc107;
            --custom-info: #0dcaf0;
            --gradient-primary: linear-gradient(45deg, #667eea 0%, #764ba2 100%);
            --gradient-success: linear-gradient(45deg, #56ab2f 0%, #a8e6cf 100%);
            --gradient-danger: linear-gradient(45deg, #ff416c 0%, #ff4b2b 100%);
            --shadow-soft: 0 4px 15px rgba(0,0,0,0.1);
            --shadow-medium: 0 8px 25px rgba(0,0,0,0.15);
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
        }
        
        .section-header {
            background: var(--gradient-primary);
            color: white;
            padding: 3rem 0;
            margin-bottom: 3rem;
            position: relative;
            overflow: hidden;
        }
        
        .section-header::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="25" cy="25" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="75" cy="75" r="1" fill="rgba(255,255,255,0.05)"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
            opacity: 0.3;
        }
        
        .section-header .content {
            position: relative;
            z-index: 2;
        }
        
        .demo-section {
            background: white;
            border-radius: 12px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: var(--shadow-soft);
            border: 1px solid #e9ecef;
        }
        
        .demo-title {
            color: var(--custom-primary);
            font-weight: 600;
            margin-bottom: 1.5rem;
            padding-bottom: 0.5rem;
            border-bottom: 2px solid #e9ecef;
        }
        
        /* Custom Button Styles */
        .btn-gradient-primary {
            background: var(--gradient-primary);
            border: none;
            color: white;
            transition: all 0.3s ease;
        }
        
        .btn-gradient-primary:hover {
            background: linear-gradient(45deg, #5a67d8 0%, #667eea 100%);
            color: white;
            transform: translateY(-2px);
            box-shadow: var(--shadow-medium);
        }
        
        .btn-gradient-success {
            background: var(--gradient-success);
            border: none;
            color: white;
            transition: all 0.3s ease;
        }
        
        .btn-gradient-success:hover {
            background: linear-gradient(45deg, #4a9b2f 0%, #8fd4a8 100%);
            color: white;
            transform: translateY(-2px);
            box-shadow: var(--shadow-medium);
        }
        
        .btn-gradient-danger {
            background: var(--gradient-danger);
            border: none;
            color: white;
            transition: all 0.3s ease;
        }
        
        .btn-gradient-danger:hover {
            background: linear-gradient(45deg, #e6356c 0%, #e6422b 100%);
            color: white;
            transform: translateY(-2px);
            box-shadow: var(--shadow-medium);
        }
        
        /* Floating Action Button */
        .btn-floating {
            border-radius: 50%;
            width: 56px;
            height: 56px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
            transition: all 0.3s ease;
            box-shadow: var(--shadow-soft);
        }
        
        .btn-floating:hover {
            transform: translateY(-3px) scale(1.1);
            box-shadow: var(--shadow-medium);
        }
        
        /* Loading Button Animation */
        .btn-loading {
            position: relative;
            color: transparent !important;
        }
        
        .btn-loading::after {
            content: '';
            position: absolute;
            width: 20px;
            height: 20px;
            top: 50%;
            left: 50%;
            margin-left: -10px;
            margin-top: -10px;
            border: 2px solid transparent;
            border-top-color: currentColor;
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        
        /* Pulse Animation */
        .btn-pulse {
            animation: pulse 2s infinite;
        }
        
        @keyframes pulse {
            0% { transform: scale(1); }
            50% { transform: scale(1.05); }
            100% { transform: scale(1); }
        }
        
        /* Ripple Effect */
        .btn-ripple {
            position: relative;
            overflow: hidden;
        }
        
        .btn-ripple::after {
            content: '';
            position: absolute;
            top: 50%;
            left: 50%;
            width: 0;
            height: 0;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.3);
            transform: translate(-50%, -50%);
            transition: width 0.6s, height 0.6s;
        }
        
        .btn-ripple:active::after {
            width: 300px;
            height: 300px;
        }
        
        /* Button Group Enhancements */
        .btn-group-custom .btn {
            border-radius: 0;
            border-right: 1px solid rgba(255,255,255,0.2);
        }
        
        .btn-group-custom .btn:first-child {
            border-top-left-radius: 8px;
            border-bottom-left-radius: 8px;
        }
        
        .btn-group-custom .btn:last-child {
            border-top-right-radius: 8px;
            border-bottom-right-radius: 8px;
            border-right: none;
        }
        
        /* Social Media Buttons */
        .btn-social {
            border-radius: 8px;
            padding: 0.75rem 1.5rem;
            font-weight: 500;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-facebook {
            background: #1877f2;
            color: white;
            border: none;
        }
        
        .btn-facebook:hover {
            background: #166fe5;
            color: white;
            transform: translateY(-2px);
        }
        
        .btn-twitter {
            background: #1da1f2;
            color: white;
            border: none;
        }
        
        .btn-twitter:hover {
            background: #1a91da;
            color: white;
            transform: translateY(-2px);
        }
        
        .btn-linkedin {
            background: #0077b5;
            color: white;
            border: none;
        }
        
        .btn-linkedin:hover {
            background: #006396;
            color: white;
            transform: translateY(-2px);
        }
        
        .btn-github {
            background: #333;
            color: white;
            border: none;
        }
        
        .btn-github:hover {
            background: #24292e;
            color: white;
            transform: translateY(-2px);
        }
        
        /* Code Display */
        .code-block {
            background: #f8f9fa;
            border: 1px solid #e9ecef;
            border-radius: 8px;
            padding: 1rem;
            margin-top: 1rem;
            font-family: 'Courier New', monospace;
            font-size: 0.9rem;
            overflow-x: auto;
        }
        
        /* Interactive Demo Controls */
        .demo-controls {
            background: #f8f9fa;
            border-radius: 8px;
            padding: 1rem;
            margin-bottom: 1rem;
        }
        
        .demo-controls label {
            font-weight: 500;
            margin-right: 0.5rem;
        }
        
        /* Responsive Button Grid */
        .button-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            margin-top: 1rem;
        }
        
        @media (max-width: 768px) {
            .button-grid {
                grid-template-columns: 1fr;
            }
            
            .btn-group-vertical {
                width: 100%;
            }
            
            .btn-group-vertical .btn {
                width: 100%;
            }
        }
    </style>
</head>
<body>
    <!-- Header -->
    <header class="section-header">
        <div class="container">
            <div class="content text-center">
                <h1 class="display-4 fw-bold mb-3">Day 15: Button Components & Interactions</h1>
                <p class="lead mb-0">Master Bootstrap's comprehensive button system with advanced interactions, states, and accessibility features</p>
            </div>
        </div>
    </header>

    <div class="container py-4">
        
        <!-- Basic Button Variants -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-palette me-2"></i>
                Basic Button Variants
            </h2>
            <p class="text-muted mb-4">Bootstrap provides 8 contextual button styles, each serving different purposes in your interface design.</p>
            
            <div class="row g-3">
                <div class="col-md-6">
                    <h5>Solid Buttons</h5>
                    <div class="d-flex flex-wrap gap-2">
                        <button type="button" class="btn btn-primary">Primary</button>
                        <button type="button" class="btn btn-secondary">Secondary</button>
                        <button type="button" class="btn btn-success">Success</button>
                        <button type="button" class="btn btn-danger">Danger</button>
                        <button type="button" class="btn btn-warning">Warning</button>
                        <button type="button" class="btn btn-info">Info</button>
                        <button type="button" class="btn btn-light">Light</button>
                        <button type="button" class="btn btn-dark">Dark</button>
                        <button type="button" class="btn btn-link">Link</button>
                    </div>
                </div>
                
                <div class="col-md-6">
                    <h5>Outline Buttons</h5>
                    <div class="d-flex flex-wrap gap-2">
                        <button type="button" class="btn btn-outline-primary">Primary</button>
                        <button type="button" class="btn btn-outline-secondary">Secondary</button>
                        <button type="button" class="btn btn-outline-success">Success</button>
                        <button type="button" class="btn btn-outline-danger">Danger</button>
                        <button type="button" class="btn btn-outline-warning">Warning</button>
                        <button type="button" class="btn btn-outline-info">Info</button>
                        <button type="button" class="btn btn-outline-light">Light</button>
                        <button type="button" class="btn btn-outline-dark">Dark</button>
                    </div>
                </div>
            </div>
            
            <div class="code-block">
                <strong>HTML:</strong><br>
                &lt;button class="btn btn-primary"&gt;Primary&lt;/button&gt;<br>
                &lt;button class="btn btn-outline-primary"&gt;Outline Primary&lt;/button&gt;
            </div>
        </section>
        
        <!-- Button Sizes -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-arrows-fullscreen me-2"></i>
                Button Sizes & Responsive Design
            </h2>
            <p class="text-muted mb-4">Three different button sizes to fit various design contexts and user interface requirements.</p>
            
            <div class="row g-4">
                <div class="col-md-4">
                    <h5>Large Buttons</h5>
                    <div class="d-grid gap-2">
                        <button type="button" class="btn btn-primary btn-lg">Large Primary</button>
                        <button type="button" class="btn btn-outline-secondary btn-lg">Large Outline</button>
                    </div>
                </div>
                
                <div class="col-md-4">
                    <h5>Default Size</h5>
                    <div class="d-grid gap-2">
                        <button type="button" class="btn btn-success">Default Success</button>
                        <button type="button" class="btn btn-outline-warning">Default Outline</button>
                    </div>
                </div>
                
                <div class="col-md-4">
                    <h5>Small Buttons</h5>
                    <div class="d-grid gap-2">
                        <button type="button" class="btn btn-danger btn-sm">Small Danger</button>
                        <button type="button" class="btn btn-outline-info btn-sm">Small Outline</button>
                    </div>
                </div>
            </div>
            
            <div class="mt-4">
                <h5>Block-Level Buttons</h5>
                <div class="d-grid gap-2">
                    <button class="btn btn-primary" type="button">Block level button</button>
                    <button class="btn btn-outline-secondary" type="button">Block level button</button>
                </div>
            </div>
            
            <div class="code-block">
                <strong>Size Classes:</strong><br>
                &lt;button class="btn btn-primary btn-lg"&gt;Large&lt;/button&gt;<br>
                &lt;button class="btn btn-primary"&gt;Default&lt;/button&gt;<br>
                &lt;button class="btn btn-primary btn-sm"&gt;Small&lt;/button&gt;<br>
                <strong>Block Level:</strong><br>
                &lt;div class="d-grid"&gt;&lt;button class="btn btn-primary"&gt;Block&lt;/button&gt;&lt;/div&gt;
            </div>
        </section>
        
        <!-- Button States -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-toggles me-2"></i>
                Button States & Interactions
            </h2>
            <p class="text-muted mb-4">Different button states provide visual feedback for user interactions and system status.</p>
            
            <div class="row g-4">
                <div class="col-md-6">
                    <h5>Interactive States</h5>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                        <button type="button" class="btn btn-primary">Normal</button>
                        <button type="button" class="btn btn-primary active">Active</button>
                        <button type="button" class="btn btn-primary" disabled>Disabled</button>
                        <button type="button" class="btn btn-primary btn-loading" id="loadingBtn">Loading</button>
                    </div>
                    
                    <h5>Toggle Buttons</h5>
                    <div class="d-flex gap-2">
                        <input type="checkbox" class="btn-check" id="btn-check-1" autocomplete="off">
                        <label class="btn btn-outline-primary" for="btn-check-1">Toggle</label>
                        
                        <input type="radio" class="btn-check" name="options" id="option1" autocomplete="off" checked>
                        <label class="btn btn-outline-success" for="option1">Option 1</label>
                        
                        <input type="radio" class="btn-check" name="options" id="option2" autocomplete="off">
                        <label class="btn btn-outline-success" for="option2">Option 2</label>
                    </div>
                </div>
                
                <div class="col-md-6">
                    <h5>Custom Animation States</h5>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                        <button type="button" class="btn btn-success btn-pulse">Pulse Effect</button>
                        <button type="button" class="btn btn-info btn-ripple">Ripple Effect</button>
                        <button type="button" class="btn btn-warning" id="bounceBtn">Bounce</button>
                    </div>
                    
                    <h5>State Controls</h5>
                    <div class="demo-controls">
                        <button class="btn btn-sm btn-outline-primary" onclick="toggleActive()">Toggle Active</button>
                        <button class="btn btn-sm btn-outline-secondary" onclick="toggleDisabled()">Toggle Disabled</button>
                        <button class="btn btn-sm btn-outline-success" onclick="toggleLoading()">Toggle Loading</button>
                    </div>
                </div>
            </div>
            
            <div class="code-block">
                <strong>State Classes:</strong><br>
                &lt;button class="btn btn-primary active"&gt;Active&lt;/button&gt;<br>
                &lt;button class="btn btn-primary" disabled&gt;Disabled&lt;/button&gt;<br>
                <strong>Toggle Buttons:</strong><br>
                &lt;input type="checkbox" class="btn-check" id="btn-check-1"&gt;<br>
                &lt;label class="btn btn-outline-primary" for="btn-check-1"&gt;Toggle&lt;/label&gt;
            </div>
        </section>
        
        <!-- Button Groups -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-collection me-2"></i>
                Button Groups & Toolbars
            </h2>
            <p class="text-muted mb-4">Group related buttons together for better organization and user experience.</p>
            
            <div class="row g-4">
                <div class="col-md-6">
                    <h5>Horizontal Button Groups</h5>
                    <div class="btn-group mb-3" role="group">
                        <button type="button" class="btn btn-outline-primary">Left</button>
                        <button type="button" class="btn btn-outline-primary">Middle</button>
                        <button type="button" class="btn btn-outline-primary">Right</button>
                    </div>
                    
                    <div class="btn-group btn-group-custom mb-3" role="group">
                        <button type="button" class="btn btn-primary">
                            <i class="bi bi-house"></i>
                        </button>
                        <button type="button" class="btn btn-primary">
                            <i class="bi bi-person"></i>
                        </button>
                        <button type="button" class="btn btn-primary">
                            <i class="bi bi-gear"></i>
                        </button>
                    </div>
                    
                    <h5>Mixed Button Groups</h5>
                    <div class="btn-group" role="group">
                        <button type="button" class="btn btn-success">Save</button>
                        <button type="button" class="btn btn-warning">Edit</button>
                        <button type="button" class="btn btn-danger">Delete</button>
                    </div>
                </div>
                
                <div class="col-md-6">
                    <h5>Vertical Button Groups</h5>
                    <div class="btn-group-vertical" role="group">
                        <button type="button" class="btn btn-outline-secondary">
                            <i class="bi bi-file-text me-2"></i>New Document
                        </button>
                        <button type="button" class="btn btn-outline-secondary">
                            <i class="bi bi-folder-open me-2"></i>Open File
                        </button>
                        <button type="button" class="btn btn-outline-secondary">
                            <i class="bi bi-save me-2"></i>Save Document
                        </button>
                        <button type="button" class="btn btn-outline-secondary">
                            <i class="bi bi-printer me-2"></i>Print
                        </button>
                    </div>
                    
                    <h5>Button Toolbar</h5>
                    <div class="btn-toolbar" role="toolbar">
                        <div class="btn-group me-2" role="group">
                            <button type="button" class="btn btn-outline-primary btn-sm">
                                <i class="bi bi-type-bold"></i>
                            </button>
                            <button type="button" class="btn btn-outline-primary btn-sm">
                                <i class="bi bi-type-italic"></i>
                            </button>
                            <button type="button" class="btn btn-outline-primary btn-sm">
                                <i class="bi bi-type-underline"></i>
                            </button>
                        </div>
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-outline-secondary btn-sm">
                                <i class="bi bi-text-left"></i>
                            </button>
                            <button type="button" class="btn btn-outline-secondary btn-sm">
                                <i class="bi bi-text-center"></i>
                            </button>
                            <button type="button" class="btn btn-outline-secondary btn-sm">
                                <i class="bi bi-text-right"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="code-block">
                <strong>Button Groups:</strong><br>
                &lt;div class="btn-group" role="group"&gt;<br>
                &nbsp;&nbsp;&lt;button class="btn btn-outline-primary"&gt;Left&lt;/button&gt;<br>
                &nbsp;&nbsp;&lt;button class="btn btn-outline-primary"&gt;Right&lt;/button&gt;<br>
                &lt;/div&gt;
            </div>
        </section>
        
        <!-- Custom Button Styles -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-brush me-2"></i>
                Custom Button Styles & Gradients
            </h2>
            <p class="text-muted mb-4">Create unique button designs with custom styles, gradients, and special effects.</p>
            
            <div class="row g-4">
                <div class="col-md-6">
                    <h5>Gradient Buttons</h5>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                        <button type="button" class="btn btn-gradient-primary">Gradient Primary</button>
                        <button type="button" class="btn btn-gradient-success">Gradient Success</button>
                        <button type="button" class="btn btn-gradient-danger">Gradient Danger</button>
                    </div>
                    
                    <h5>Floating Action Buttons</h5>
                    <div class="d-flex gap-3">
                        <button type="button" class="btn btn-primary btn-floating">
                            <i class="bi bi-plus"></i>
                        </button>
                        <button type="button" class="btn btn-success btn-floating">
                            <i class="bi bi-heart"></i>
                        </button>
                        <button type="button" class="btn btn-danger btn-floating">
                            <i class="bi bi-trash"></i>
                        </button>
                        <button type="button" class="btn btn-warning btn-floating">
                            <i class="bi bi-star"></i>
                        </button>
                    </div>
                </div>
                
                <div class="col-md-6">
                    <h5>Social Media Buttons</h5>
                    <div class="d-grid gap-2">
                        <button type="button" class="btn btn-social btn-facebook">
                            <i class="bi bi-facebook"></i>
                            Continue with Facebook
                        </button>
                        <button type="button" class="btn btn-social btn-twitter">
                            <i class="bi bi-twitter"></i>
                            Sign in with Twitter
                        </button>
                        <button type="button" class="btn btn-social btn-linkedin">
                            <i class="bi bi-linkedin"></i>
                            Connect LinkedIn
                        </button>
                        <button type="button" class="btn btn-social btn-github">
                            <i class="bi bi-github"></i>
                            Login with GitHub
                        </button>
                    </div>
                </div>
            </div>
            
            <div class="code-block">
                <strong>Custom CSS:</strong><br>
                .btn-gradient-primary {<br>
                &nbsp;&nbsp;background: linear-gradient(45deg, #667eea 0%, #764ba2 100%);<br>
                &nbsp;&nbsp;border: none;<br>
                &nbsp;&nbsp;color: white;<br>
                }
            </div>
        </section>
        
        <!-- Button with Icons -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-image me-2"></i>
                Buttons with Icons & Content
            </h2>
            <p class="text-muted mb-4">Enhance button clarity and visual appeal with icons, badges, and mixed content.</p>
            
            <div class="row g-4">
                <div class="col-md-6">
                    <h5>Icon Buttons</h5>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                        <button type="button" class="btn btn-primary">
                            <i class="bi bi-download me-2"></i>Download
                        </button>
                        <button type="button" class="btn btn-success">
                            <i class="bi bi-check-circle me-2"></i>Confirm
                        </button>
                        <button type="button" class="btn btn-danger">
                            <i class="bi bi-trash me-2"></i>Delete
                        </button>
                    </div>
                    
                    <h5>Icon-Only Buttons</h5>
                    <div class="d-flex gap-2">
                        <button type="button" class="btn btn-outline-primary">
                            <i class="bi bi-search"></i>
                        </button>
                        <button type="button" class="btn btn-outline-success">
                            <i class="bi bi-heart"></i>
                        </button>
                        <button type="button" class="btn btn-outline-warning">
                            <i class="bi bi-star"></i>
                        </button>
                        <button type="button" class="btn btn-outline-danger">
                            <i class="bi bi-x-circle"></i>
                        </button>
                    </div>
                </div>
                
                <div class="col-md-6">
                    <h5>Buttons with Badges</h5>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                        <button type="button" class="btn btn-primary">
                            Messages <span class="badge bg-light text-dark">4</span>
                        </button>
                        <button type="button" class="btn btn-success">
                            Notifications <span class="badge bg-light text-dark">12</span>
                        </button>
                    </div>
                    
                    <h5>Multi-line Buttons</h5>
                    <div class="d-flex gap-2">
                        <button type="button" class="btn btn-outline-primary text-start" style="width: 150px;">
                            <div class="fw-bold">Premium Plan</div>
                            <small class="text-muted">$29/month</small>
                        </button>
                        <button type="button" class="btn btn-outline-success text-start" style="width: 150px;">
                            <div class="fw-bold">Pro Plan</div>
                            <small class="text-muted">$49/month</small>
                        </button>
                    </div>
                </div>
            </div>
            
            <div class="code-block">
                <strong>Icon Buttons:</strong><br>
                &lt;button class="btn btn-primary"&gt;<br>
                &nbsp;&nbsp;&lt;i class="bi bi-download me-2"&gt;&lt;/i&gt;Download<br>
                &lt;/button&gt;<br>
                <strong>With Badges:</strong><br>
                &lt;button class="btn btn-primary"&gt;<br>
                &nbsp;&nbsp;Messages &lt;span class="badge bg-light text-dark"&gt;4&lt;/span&gt;<br>
                &lt;/button&gt;
            </div>
        </section>
        
        <!-- Interactive Demo -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-controller me-2"></i>
                Interactive Button Demo
            </h2>
            <p class="text-muted mb-4">Try different button combinations and see the results in real-time.</p>
            
            <div class="row g-4">
                <div class="col-md-4">
                    <div class="demo-controls">
                        <h6>Customize Button</h6>
                        <div class="mb-3">
                            <label for="buttonStyle" class="form-label">Style:</label>
                            <select id="buttonStyle" class="form-select form-select-sm">
                                <option value="btn-primary">Primary</option>
                                <option value="btn-secondary">Secondary</option>
                                <option value="btn-success">Success</option>
                                <option value="btn-danger">Danger</option>
                                <option value="btn-outline-primary">Outline Primary</option>
                                <option value="btn-outline-success">Outline Success</option>
                            </select>
                        </div>
                        <div class="mb-3">
                            <label for="buttonSize" class="form-label">Size:</label>
                            <select id="buttonSize" class="form-select form-select-sm">
                                <option value="btn-sm">Small</option>
                                <option value="" selected>Default</option>
                                <option value="btn-lg">Large</option>
                            </select>
                        </div>
                        <div class="mb-3">
                            <label for="buttonText" class="form-label">Text:</label>
                            <input type="text" id="buttonText" class="form-control form-control-sm" value="Custom Button">
                        </div>
                        <div class="form-check mb-2">
                            <input class="form-check-input" type="checkbox" id="addIcon">
                            <label class="form-check-label" for="addIcon">Add Icon</label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="makeDisabled">
                            <label class="form-check-label" for="makeDisabled">Disabled</label>
                        </div>
                    </div>
                </div>
                
                <div class="col-md-8">
                    <h6>Preview:</h6>
                    <div class="p-4 bg-light rounded">
                        <button type="button" class="btn btn-primary" id="demoButton">
                            Custom Button
                        </button>
                    </div>
                    
                    <h6 class="mt-3">Generated Code:</h6>
                    <div class="code-block" id="generatedCode">
                        &lt;button type="button" class="btn btn-primary"&gt;Custom Button&lt;/button&gt;
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Accessibility -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-universal-access me-2"></i>
                Button Accessibility
            </h2>
            <p class="text-muted mb-4">Ensure your buttons are accessible to all users with proper ARIA attributes and keyboard navigation.</p>
            
            <div class="row g-4">
                <div class="col-md-6">
                    <h5>ARIA Labels & Descriptions</h5>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                        <button type="button" class="btn btn-primary" aria-label="Save document">
                            <i class="bi bi-save"></i>
                        </button>
                        <button type="button" class="btn btn-danger" aria-label="Delete item" aria-describedby="deleteHelp">
                            <i class="bi bi-trash"></i>
                        </button>
                        <button type="button" class="btn btn-success" aria-pressed="false" aria-label="Toggle favorite">
                            <i class="bi bi-heart"></i>
                        </button>
                    </div>
                    <small id="deleteHelp" class="text-muted">This action cannot be undone</small>
                </div>
                
                <div class="col-md-6">
                    <h5>Keyboard Navigation</h5>
                    <div class="d-flex flex-wrap gap-2">
                        <button type="button" class="btn btn-outline-primary" tabindex="1">Tab Order 1</button>
                        <button type="button" class="btn btn-outline-primary" tabindex="2">Tab Order 2</button>
                        <button type="button" class="btn btn-outline-primary" tabindex="3">Tab Order 3</button>
                    </div>
                    <small class="text-muted mt-2 d-block">Try navigating with Tab key</small>
                </div>
            </div>
            
            <div class="code-block">
                <strong>Accessibility Best Practices:</strong><br>
                &lt;button aria-label="Save document"&gt;&lt;i class="bi bi-save"&gt;&lt;/i&gt;&lt;/button&gt;<br>
                &lt;button aria-describedby="help-text"&gt;Delete&lt;/button&gt;<br>
                &lt;button aria-pressed="false"&gt;Toggle&lt;/button&gt;
            </div>
        </section>
        
    </div>

    <!-- Bootstrap JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <!-- Custom JavaScript -->
    <script>
        // Button state management
        let testButton = null;
        
        function toggleActive() {
            const btn = document.getElementById('loadingBtn');
            btn.classList.toggle('active');
        }
        
        function toggleDisabled() {
            const btn = document.getElementById('loadingBtn');
            btn.disabled = !btn.disabled;
        }
        
        function toggleLoading() {
            const btn = document.getElementById('loadingBtn');
            btn.classList.toggle('btn-loading');
            btn.textContent = btn.classList.contains('btn-loading') ? 'Loading...' : 'Loading';
        }
        
        // Bounce animation
        document.getElementById('bounceBtn').addEventListener('click', function() {
            this.style.animation = 'none';
            setTimeout(() => {
                this.style.animation = 'bounce 0.5s ease';
            }, 10);
        });
        
        // Interactive demo functionality
        function updateDemoButton() {
            const button = document.getElementById('demoButton');
            const style = document.getElementById('buttonStyle').value;
            const size = document.getElementById('buttonSize').value;
            const text = document.getElementById('buttonText').value;
            const addIcon = document.getElementById('addIcon').checked;
            const makeDisabled = document.getElementById('makeDisabled').checked;
            
            // Reset classes
            button.className = 'btn';
            
            // Add style
            button.classList.add(style);
            
            // Add size
            if (size) {
                button.classList.add(size);
            }
            
            // Set text and icon
            const iconHtml = addIcon ? '<i class="bi bi-star me-2"></i>' : '';
            button.innerHTML = iconHtml + text;
            
            // Set disabled state
            button.disabled = makeDisabled;
            
            // Update generated code
            let codeHtml = `&lt;button type="button" class="btn ${style}`;
            if (size) codeHtml += ` ${size}`;
            codeHtml += `"`;
            if (makeDisabled) codeHtml += ` disabled`;
            codeHtml += `&gt;`;
            if (addIcon) codeHtml += `&lt;i class="bi bi-star me-2"&gt;&lt;/i&gt;`;
            codeHtml += `${text}&lt;/button&gt;`;
            
            document.getElementById('generatedCode').innerHTML = codeHtml;
        }
        
        // Attach event listeners for demo
        document.getElementById('buttonStyle').addEventListener('change', updateDemoButton);
        document.getElementById('buttonSize').addEventListener('change', updateDemoButton);
        document.getElementById('buttonText').addEventListener('input', updateDemoButton);
        document.getElementById('addIcon').addEventListener('change', updateDemoButton);
        document.getElementById('makeDisabled').addEventListener('change', updateDemoButton);
        
        // Ripple effect implementation
        document.querySelectorAll('.btn-ripple').forEach(button => {
            button.addEventListener('click', function(e) {
                const ripple = document.createElement('span');
                const rect = this.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                const x = e.clientX - rect.left - size / 2;
                const y = e.clientY - rect.top - size / 2;
                
                ripple.style.cssText = `
                    position: absolute;
                    width: ${size}px;
                    height: ${size}px;
                    left: ${x}px;
                    top: ${y}px;
                    background: rgba(255, 255, 255, 0.3);
                    border-radius: 50%;
                    transform: scale(0);
                    animation: ripple 0.6s linear;
                    pointer-events: none;
                `;
                
                this.appendChild(ripple);
                
                setTimeout(() => {
                    ripple.remove();
                }, 600);
            });
        });
        
        // Add ripple animation styles
        if (!document.querySelector('#ripple-styles')) {
            const style = document.createElement('style');
            style.id = 'ripple-styles';
            style.textContent = `
                @keyframes ripple {
                    to {
                        transform: scale(4);
                        opacity: 0;
                    }
                }
                
                @keyframes bounce {
                    0%, 20%, 53%, 80%, 100% {
                        transform: translateY(0);
                    }
                    40%, 43% {
                        transform: translateY(-20px);
                    }
                    70% {
                        transform: translateY(-10px);
                    }
                    90% {
                        transform: translateY(-4px);
                    }
                }
            `;
            document.head.appendChild(style);
        }
        
        console.log('🔘 Day 15 - Button Components loaded successfully!');
    </script>
</body>
</html>
```

## 📋 Day 15 Button Component Checklist

### Basic Button Mastery
- [ ] All 8 Bootstrap button variants (primary, secondary, success, etc.)
- [ ] Outline button styles and appropriate usage contexts
- [ ] Three button sizes and responsive considerations
- [ ] Block-level buttons with grid system integration

### Advanced Button Features
- [ ] Button state management (active, disabled, loading)
- [ ] Toggle buttons with checkbox and radio functionality
- [ ] Button groups (horizontal, vertical, mixed)
- [ ] Custom button animations and effects

### Interactive Elements
- [ ] Icon integration with proper spacing
- [ ] Badge integration for notifications/counters
- [ ] Multi-line button content and layouts
- [ ] Social media button styling patterns

### Accessibility & Best Practices
- [ ] ARIA labels and descriptions
- [ ] Keyboard navigation support
- [ ] Focus management and visual indicators
- [ ] Screen reader compatibility

### Custom Styling
- [ ] Gradient button effects
- [ ] Floating action buttons
- [ ] Ripple and bounce animations
- [ ] Custom color schemes and themes

## 🎯 Day 15 Complete!

### ✅ Achievements Unlocked:
- **Button Variants:** Mastered all 9 Bootstrap button styles
- **Interactive States:** Implemented active, disabled, and loading states
- **Advanced Grouping:** Created complex button groups and toolbars
- **Custom Styling:** Built gradient, floating, and animated buttons
- **Accessibility:** Implemented proper ARIA attributes and keyboard navigation
- **Real-world Application:** Created interactive demo with live code generation

### 🔗 Key Takeaways:
1. **Semantic Usage:** Different button variants serve specific UI purposes
2. **State Management:** Proper button states enhance user experience
3. **Group Organization:** Button groups improve interface organization
4. **Accessibility First:** Always include proper ARIA attributes
5. **Performance:** Optimize animations and effects for smooth interactions

## 📖 Additional Resources

- **[Bootstrap Buttons Documentation](https://getbootstrap.com/docs/5.3/components/buttons/)** - Official button component guide
- **[Button Group Documentation](https://getbootstrap.com/docs/5.3/components/button-group/)** - Official button group reference
- **[ARIA Button Patterns](https://www.w3.org/WAI/ARIA/apg/patterns/button/)** - W3C accessibility guidelines
- **[CSS Button Animations](https://css-tricks.com/css-button-styling-guide/)** - Advanced button styling techniques

---

**Next up: Day 16 - Card Components & Layouts** where you'll master Bootstrap's versatile card system for content organization! 🃏