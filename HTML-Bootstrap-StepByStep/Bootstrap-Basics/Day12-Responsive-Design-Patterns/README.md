# Day 12: Responsive Design Patterns 📱

Welcome to Day 12! Today you'll master advanced responsive design techniques using Bootstrap's mobile-first approach. You'll learn to create layouts that adapt seamlessly across all device sizes using responsive utilities, breakpoint management, and proven design patterns.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master Bootstrap's mobile-first responsive approach
- Understand and implement all responsive breakpoints
- Use responsive utilities for display, spacing, and layout
- Create adaptive navigation patterns
- Build responsive image and media solutions  
- Implement responsive typography and spacing
- Design mobile-first layouts that scale up beautifully
- Apply responsive design best practices and testing

## 📚 Lesson Content

### Bootstrap Responsive Breakpoints

Bootstrap uses a mobile-first approach with 6 responsive breakpoints that define how your layout adapts to different screen sizes.

#### Breakpoint System Overview
```html
<div class="container">
    <!-- Breakpoint Reference -->
    <div class="row mb-4">
        <div class="col-12">
            <h4>Bootstrap 5 Breakpoints</h4>
            <div class="table-responsive">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Breakpoint</th>
                            <th>Class infix</th>
                            <th>Dimensions</th>
                            <th>Description</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Extra small</td>
                            <td><code>xs</code> (none)</td>
                            <td>&lt;576px</td>
                            <td>Portrait phones</td>
                        </tr>
                        <tr>
                            <td>Small</td>
                            <td><code>sm</code></td>
                            <td>≥576px</td>
                            <td>Landscape phones</td>
                        </tr>
                        <tr>
                            <td>Medium</td>
                            <td><code>md</code></td>
                            <td>≥768px</td>
                            <td>Tablets</td>
                        </tr>
                        <tr>
                            <td>Large</td>
                            <td><code>lg</code></td>
                            <td>≥992px</td>
                            <td>Desktops</td>
                        </tr>
                        <tr>
                            <td>Extra large</td>
                            <td><code>xl</code></td>
                            <td>≥1200px</td>
                            <td>Large desktops</td>
                        </tr>
                        <tr>
                            <td>Extra extra large</td>
                            <td><code>xxl</code></td>
                            <td>≥1400px</td>
                            <td>Larger desktops</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    
    <!-- Breakpoint Indicator -->
    <div class="row mb-4">
        <div class="col-12">
            <div class="alert alert-info">
                <h5>Current Breakpoint:</h5>
                <div class="d-block d-sm-none">📱 Extra Small (xs) - &lt;576px</div>
                <div class="d-none d-sm-block d-md-none">📱 Small (sm) - ≥576px</div>
                <div class="d-none d-md-block d-lg-none">📱 Medium (md) - ≥768px</div>
                <div class="d-none d-lg-block d-xl-none">💻 Large (lg) - ≥992px</div>
                <div class="d-none d-xl-block d-xxl-none">💻 Extra Large (xl) - ≥1200px</div>
                <div class="d-none d-xxl-block">💻 Extra Extra Large (xxl) - ≥1400px</div>
            </div>
        </div>
    </div>
</div>
```

#### Responsive Grid Layout
```html
<div class="container">
    <!-- Progressive Enhancement Grid -->
    <div class="row g-3 mb-4">
        <!-- Mobile: 1 column, Tablet: 2 columns, Desktop: 3 columns -->
        <div class="col-12 col-md-6 col-lg-4">
            <div class="bg-primary text-white p-3 rounded text-center">
                <h6>Card 1</h6>
                <p class="mb-0">Mobile: 12/12, Tablet: 6/12, Desktop: 4/12</p>
            </div>
        </div>
        <div class="col-12 col-md-6 col-lg-4">
            <div class="bg-success text-white p-3 rounded text-center">
                <h6>Card 2</h6>
                <p class="mb-0">Responsive columns adapt to screen size</p>
            </div>
        </div>
        <div class="col-12 col-md-6 col-lg-4">
            <div class="bg-warning text-dark p-3 rounded text-center">
                <h6>Card 3</h6>
                <p class="mb-0">Mobile-first approach scales up</p>
            </div>
        </div>
    </div>
    
    <!-- Complex Responsive Layout -->
    <div class="row g-3 mb-4">
        <!-- Sidebar: Full width on mobile, 1/3 on desktop -->
        <div class="col-12 col-lg-4 order-2 order-lg-1">
            <div class="bg-secondary text-white p-3 rounded">
                <h6>Sidebar</h6>
                <p class="mb-0">Mobile: Full width, Desktop: 1/3 width</p>
                <small>Order changes: First on desktop, second on mobile</small>
            </div>
        </div>
        
        <!-- Main content: Full width on mobile, 2/3 on desktop -->
        <div class="col-12 col-lg-8 order-1 order-lg-2">
            <div class="bg-info text-dark p-3 rounded">
                <h6>Main Content</h6>
                <p class="mb-0">Mobile: Full width, Desktop: 2/3 width</p>
                <small>Order changes: Second on desktop, first on mobile</small>
            </div>
        </div>
    </div>
</div>
```

### Responsive Display Utilities

Control element visibility across different breakpoints with display utilities.

```html
<div class="container">
    <!-- Basic Responsive Display -->
    <div class="row mb-4">
        <div class="col-12">
            <h4>Responsive Visibility</h4>
            
            <!-- Hidden on mobile, visible on larger screens -->
            <div class="d-none d-md-block bg-primary text-white p-3 mb-2 rounded">
                <i class="bi bi-eye me-2"></i>
                Hidden on mobile (xs, sm), visible on tablet and up (md+)
            </div>
            
            <!-- Visible only on mobile -->
            <div class="d-block d-md-none bg-success text-white p-3 mb-2 rounded">
                <i class="bi bi-phone me-2"></i>
                Visible only on mobile (xs, sm), hidden on tablet and up (md+)
            </div>
            
            <!-- Visible only on desktop -->
            <div class="d-none d-lg-block d-xl-none bg-warning text-dark p-3 mb-2 rounded">
                <i class="bi bi-laptop me-2"></i>
                Visible only on large screens (lg), hidden on xl+
            </div>
            
            <!-- Complex visibility pattern -->
            <div class="d-none d-sm-block d-md-none d-lg-block bg-danger text-white p-3 rounded">
                <i class="bi bi-magic me-2"></i>
                Complex: Hidden on xs, visible on sm, hidden on md, visible on lg+
            </div>
        </div>
    </div>
    
    <!-- Responsive Flex Display -->
    <div class="row mb-4">
        <div class="col-12">
            <h4>Responsive Flex Layouts</h4>
            
            <!-- Stack on mobile, flex row on desktop -->
            <div class="d-flex flex-column flex-md-row gap-3 mb-3">
                <div class="bg-primary text-white p-3 rounded flex-grow-1">
                    <strong>Item 1</strong><br>
                    <small>Stacked on mobile, row on tablet+</small>
                </div>
                <div class="bg-success text-white p-3 rounded flex-grow-1">
                    <strong>Item 2</strong><br>
                    <small>Responsive flex direction</small>
                </div>
                <div class="bg-warning text-dark p-3 rounded flex-grow-1">
                    <strong>Item 3</strong><br>
                    <small>Adapts to screen size</small>
                </div>
            </div>
        </div>
    </div>
</div>
```

### Responsive Navigation Patterns

Create navigation that adapts to different screen sizes and interaction methods.

```html
<div class="container">
    <!-- Desktop Navigation -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary rounded mb-4">
        <div class="container-fluid">
            <!-- Brand -->
            <a class="navbar-brand" href="#">
                <i class="bi bi-house-door me-2"></i>
                Brand
            </a>
            
            <!-- Mobile toggle button -->
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" 
                    data-bs-target="#navbarResponsive">
                <span class="navbar-toggler-icon"></span>
            </button>
            
            <!-- Collapsible navigation -->
            <div class="collapse navbar-collapse" id="navbarResponsive">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item">
                        <a class="nav-link active" href="#">Home</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">About</a>
                    </li>
                    <li class="nav-item dropdown">
                        <a class="nav-link dropdown-toggle" href="#" role="button" 
                           data-bs-toggle="dropdown">
                            Services
                        </a>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item" href="#">Web Design</a></li>
                            <li><a class="dropdown-item" href="#">Development</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li><a class="dropdown-item" href="#">Consulting</a></li>
                        </ul>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">Contact</a>
                    </li>
                </ul>
                
                <!-- Right-aligned items -->
                <div class="d-flex flex-column flex-lg-row gap-2">
                    <button class="btn btn-outline-light">Login</button>
                    <button class="btn btn-light">Sign Up</button>
                </div>
            </div>
        </div>
    </nav>
    
    <!-- Tab Navigation (Responsive) -->
    <div class="bg-light p-3 rounded mb-4">
        <h5 class="mb-3">Responsive Tab Navigation</h5>
        
        <!-- Desktop: Horizontal tabs -->
        <ul class="nav nav-tabs d-none d-md-flex" role="tablist">
            <li class="nav-item">
                <button class="nav-link active" data-bs-toggle="tab" data-bs-target="#tab1">
                    Dashboard
                </button>
            </li>
            <li class="nav-item">
                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab2">
                    Analytics
                </button>
            </li>
            <li class="nav-item">
                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab3">
                    Settings
                </button>
            </li>
        </ul>
        
        <!-- Mobile: Dropdown select -->
        <select class="form-select d-md-none mb-3" id="mobileTabSelect">
            <option value="tab1" selected>Dashboard</option>
            <option value="tab2">Analytics</option>
            <option value="tab3">Settings</option>
        </select>
        
        <!-- Tab content -->
        <div class="tab-content">
            <div class="tab-pane fade show active" id="tab1">
                <h6>Dashboard Content</h6>
                <p class="mb-0">Dashboard information and metrics display here.</p>
            </div>
            <div class="tab-pane fade" id="tab2">
                <h6>Analytics Content</h6>
                <p class="mb-0">Charts and analytics data would be shown here.</p>
            </div>
            <div class="tab-pane fade" id="tab3">
                <h6>Settings Content</h6>
                <p class="mb-0">Configuration options and preferences.</p>
            </div>
        </div>
    </div>
</div>
```

### Responsive Images and Media

Handle images and media content that scales appropriately across devices.

```html
<div class="container">
    <!-- Responsive Images -->
    <div class="row mb-4">
        <div class="col-12">
            <h4>Responsive Images</h4>
            
            <!-- Basic responsive image -->
            <div class="mb-3">
                <img src="https://via.placeholder.com/800x400/6f42c1/ffffff?text=Responsive+Image" 
                     class="img-fluid rounded" alt="Responsive image">
                <small class="text-muted d-block mt-1">
                    img-fluid makes images scale nicely to parent width
                </small>
            </div>
            
            <!-- Different images for different breakpoints -->
            <div class="mb-3">
                <picture>
                    <source media="(min-width: 768px)" 
                            srcset="https://via.placeholder.com/800x300/28a745/ffffff?text=Desktop+Image">
                    <source media="(min-width: 576px)" 
                            srcset="https://via.placeholder.com/600x200/ffc107/000000?text=Tablet+Image">
                    <img src="https://via.placeholder.com/400x300/dc3545/ffffff?text=Mobile+Image" 
                         class="img-fluid rounded" alt="Responsive image sources">
                </picture>
                <small class="text-muted d-block mt-1">
                    Different images loaded based on screen size
                </small>
            </div>
        </div>
    </div>
    
    <!-- Responsive Video -->
    <div class="row mb-4">
        <div class="col-12">
            <h4>Responsive Video</h4>
            
            <!-- Video with responsive wrapper -->
            <div class="ratio ratio-16x9 mb-3">
                <video class="rounded" controls>
                    <source src="https://www.w3schools.com/html/mov_bbb.mp4" type="video/mp4">
                    Your browser does not support the video tag.
                </video>
            </div>
            
            <!-- Different aspect ratios -->
            <div class="row g-3">
                <div class="col-md-4">
                    <div class="ratio ratio-1x1">
                        <div class="bg-primary text-white d-flex align-items-center justify-content-center rounded">
                            <strong>1:1 Ratio</strong>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="ratio ratio-4x3">
                        <div class="bg-success text-white d-flex align-items-center justify-content-center rounded">
                            <strong>4:3 Ratio</strong>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="ratio ratio-21x9">
                        <div class="bg-warning text-dark d-flex align-items-center justify-content-center rounded">
                            <strong>21:9 Ratio</strong>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
```

### Responsive Typography and Spacing

Scale text and spacing appropriately across different screen sizes.

```html
<div class="container">
    <!-- Responsive Typography -->
    <div class="row mb-4">
        <div class="col-12">
            <h4>Responsive Typography</h4>
            
            <!-- Font sizes that change with breakpoints -->
            <h1 class="display-6 display-md-4 display-lg-2 mb-3">
                Responsive Heading
            </h1>
            <p class="fs-6 fs-md-5 fs-lg-4 mb-3">
                This paragraph text gets larger on bigger screens: 
                fs-6 on mobile, fs-5 on tablet, fs-4 on desktop.
            </p>
            
            <!-- Responsive text alignment -->
            <p class="text-center text-md-start text-lg-end mb-3">
                Text alignment changes: center on mobile, left on tablet, right on desktop.
            </p>
            
            <!-- Responsive line height and spacing -->
            <div class="lh-sm lh-md-base lh-lg-lg">
                <p class="mb-2 mb-md-3 mb-lg-4">
                    Line height and margins adapt to screen size for optimal readability.
                    On mobile, tighter spacing conserves screen real estate. On larger 
                    screens, more generous spacing improves readability.
                </p>
            </div>
        </div>
    </div>
    
    <!-- Responsive Spacing -->
    <div class="row mb-4">
        <div class="col-12">
            <h4>Responsive Spacing</h4>
            
            <!-- Cards with responsive spacing -->
            <div class="row g-2 g-md-3 g-lg-4">
                <div class="col-12 col-sm-6 col-lg-4">
                    <div class="bg-primary text-white p-2 p-md-3 p-lg-4 rounded">
                        <h6>Card 1</h6>
                        <p class="mb-0">Padding increases with screen size</p>
                    </div>
                </div>
                <div class="col-12 col-sm-6 col-lg-4">
                    <div class="bg-success text-white p-2 p-md-3 p-lg-4 rounded">
                        <h6>Card 2</h6>
                        <p class="mb-0">Gutters also scale responsively</p>
                    </div>
                </div>
                <div class="col-12 col-sm-6 col-lg-4">
                    <div class="bg-warning text-dark p-2 p-md-3 p-lg-4 rounded">
                        <h6>Card 3</h6>
                        <p class="mb-0">Better UX on all devices</p>
                    </div>
                </div>
            </div>
            
            <!-- Responsive margins -->
            <div class="mt-3 mt-md-4 mt-lg-5">
                <div class="bg-info text-dark p-3 rounded">
                    <p class="mb-0">
                        This container has responsive top margins: 
                        mt-3 on mobile, mt-4 on tablet, mt-5 on desktop.
                    </p>
                </div>
            </div>
        </div>
    </div>
</div>
```

## 💻 Hands-On Practice

### Exercise 1: Complete Responsive Design Showcase

Create `responsive-patterns-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bootstrap Responsive Design Patterns</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" 
          rel="stylesheet">
    
    <!-- Bootstrap Icons -->
    <link rel="stylesheet" 
          href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <style>
        .responsive-demo {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }
        
        .breakpoint-indicator {
            position: fixed;
            bottom: 20px;
            right: 20px;
            background: rgba(0, 0, 0, 0.8);
            color: white;
            padding: 10px 15px;
            border-radius: 20px;
            font-size: 0.8rem;
            z-index: 1000;
        }
        
        .demo-card {
            transition: all 0.3s ease;
            border: none;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            overflow: hidden;
        }
        
        .demo-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 30px rgba(0,0,0,0.15);
        }
        
        .feature-icon {
            width: 60px;
            height: 60px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
            margin: 0 auto 1rem;
        }
        
        .responsive-grid-demo {
            background: repeating-linear-gradient(
                45deg,
                transparent,
                transparent 10px,
                rgba(255,255,255,0.1) 10px,
                rgba(255,255,255,0.1) 20px
            );
        }
        
        .mobile-first-example {
            border-left: 4px solid #28a745;
            background: rgba(40, 167, 69, 0.1);
        }
        
        .device-preview {
            border: 8px solid #333;
            border-radius: 20px;
            overflow: hidden;
            background: #333;
            padding: 20px;
            margin: 0 auto;
        }
        
        .device-screen {
            background: white;
            border-radius: 10px;
            min-height: 300px;
            overflow: hidden;
        }
        
        @media (max-width: 575.98px) {
            .device-preview {
                border-width: 4px;
                padding: 10px;
            }
        }
        
        .navigation-demo {
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        .responsive-table-wrapper {
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        .image-comparison {
            position: relative;
            overflow: hidden;
            border-radius: 10px;
        }
        
        .comparison-slider {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background: rgba(0,0,0,0.7);
            color: white;
            padding: 5px 10px;
            border-radius: 15px;
            font-size: 0.8rem;
        }
    </style>
</head>
<body class="responsive-demo">
    <!-- Breakpoint Indicator -->
    <div class="breakpoint-indicator">
        <div class="d-block d-sm-none">📱 XS (&lt;576px)</div>
        <div class="d-none d-sm-block d-md-none">📱 SM (≥576px)</div>
        <div class="d-none d-md-block d-lg-none">💻 MD (≥768px)</div>
        <div class="d-none d-lg-block d-xl-none">💻 LG (≥992px)</div>
        <div class="d-none d-xl-block d-xxl-none">💻 XL (≥1200px)</div>
        <div class="d-none d-xxl-block">💻 XXL (≥1400px)</div>
    </div>
    
    <!-- Hero Section -->
    <header class="text-white py-5">
        <div class="container">
            <div class="row align-items-center min-vh-100">
                <div class="col-lg-6">
                    <h1 class="display-4 display-md-3 display-lg-2 fw-bold mb-4">
                        Responsive Design Mastery
                    </h1>
                    <p class="lead fs-5 fs-md-4 mb-4 mb-lg-5">
                        Learn to create layouts that adapt beautifully across all devices 
                        using Bootstrap's mobile-first approach.
                    </p>
                    <div class="d-flex flex-column flex-sm-row gap-3">
                        <button class="btn btn-light btn-lg">
                            <i class="bi bi-play-fill me-2"></i>
                            Start Learning
                        </button>
                        <button class="btn btn-outline-light btn-lg">
                            <i class="bi bi-download me-2"></i>
                            Download Guide
                        </button>
                    </div>
                </div>
                <div class="col-lg-6 text-center">
                    <div class="device-preview d-none d-lg-block">
                        <div class="device-screen p-4">
                            <div class="bg-primary text-white p-3 rounded mb-3">
                                <h6>Responsive Preview</h6>
                            </div>
                            <div class="row g-2">
                                <div class="col-6">
                                    <div class="bg-success text-white p-2 rounded text-center">
                                        <small>Card 1</small>
                                    </div>
                                </div>
                                <div class="col-6">
                                    <div class="bg-warning text-dark p-2 rounded text-center">
                                        <small>Card 2</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </header>
    
    <!-- Main Content -->
    <main class="bg-light py-5">
        <div class="container">
            <!-- Mobile-First Approach -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Mobile-First Approach</h2>
                        <p class="lead text-muted">
                            Start with mobile design and progressively enhance for larger screens
                        </p>
                    </div>
                </div>
                
                <div class="mobile-first-example p-4 rounded mb-4">
                    <h4 class="text-success mb-3">
                        <i class="bi bi-phone me-2"></i>
                        Progressive Enhancement Example
                    </h4>
                    
                    <!-- Mobile: Stack vertically -->
                    <!-- Tablet: 2 columns -->  
                    <!-- Desktop: 4 columns -->
                    <div class="row g-3">
                        <div class="col-12 col-md-6 col-xl-3">
                            <div class="demo-card card text-center">
                                <div class="card-body">
                                    <div class="feature-icon bg-primary text-white mx-auto">
                                        <i class="bi bi-phone"></i>
                                    </div>
                                    <h6 class="card-title">Mobile First</h6>
                                    <p class="card-text small">
                                        Design starts with mobile constraints
                                    </p>
                                </div>
                            </div>
                        </div>
                        
                        <div class="col-12 col-md-6 col-xl-3">
                            <div class="demo-card card text-center">
                                <div class="card-body">
                                    <div class="feature-icon bg-success text-white mx-auto">
                                        <i class="bi bi-tablet"></i>
                                    </div>
                                    <h6 class="card-title">Tablet Enhanced</h6>
                                    <p class="card-text small">
                                        Two columns on tablet screens
                                    </p>
                                </div>
                            </div>
                        </div>
                        
                        <div class="col-12 col-md-6 col-xl-3">
                            <div class="demo-card card text-center">
                                <div class="card-body">
                                    <div class="feature-icon bg-warning text-dark mx-auto">
                                        <i class="bi bi-laptop"></i>
                                    </div>
                                    <h6 class="card-title">Desktop Optimized</h6>
                                    <p class="card-text small">
                                        Four columns on large screens
                                    </p>
                                </div>
                            </div>
                        </div>
                        
                        <div class="col-12 col-md-6 col-xl-3">
                            <div class="demo-card card text-center">
                                <div class="card-body">
                                    <div class="feature-icon bg-info text-white mx-auto">
                                        <i class="bi bi-display"></i>
                                    </div>
                                    <h6 class="card-title">Large Display</h6>
                                    <p class="card-text small">
                                        Maintains optimal layout
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            
            <!-- Responsive Navigation -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Responsive Navigation</h2>
                        <p class="lead text-muted">
                            Navigation patterns that adapt to different screen sizes
                        </p>
                    </div>
                </div>
                
                <!-- Advanced Responsive Navbar -->
                <nav class="navbar navbar-expand-lg navbar-dark bg-dark rounded navigation-demo mb-4">
                    <div class="container-fluid">
                        <a class="navbar-brand" href="#">
                            <i class="bi bi-bootstrap me-2"></i>
                            ResponsiveSite
                        </a>
                        
                        <!-- Mobile menu button -->
                        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" 
                                data-bs-target="#mainNavbar">
                            <span class="navbar-toggler-icon"></span>
                        </button>
                        
                        <div class="collapse navbar-collapse" id="mainNavbar">
                            <!-- Main navigation -->
                            <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                                <li class="nav-item">
                                    <a class="nav-link active" href="#">Home</a>
                                </li>
                                <li class="nav-item dropdown">
                                    <a class="nav-link dropdown-toggle" href="#" role="button" 
                                       data-bs-toggle="dropdown">
                                        Products
                                    </a>
                                    <ul class="dropdown-menu">
                                        <li><a class="dropdown-item" href="#">
                                            <i class="bi bi-laptop me-2"></i>Laptops
                                        </a></li>
                                        <li><a class="dropdown-item" href="#">
                                            <i class="bi bi-phone me-2"></i>Phones
                                        </a></li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li><a class="dropdown-item" href="#">
                                            <i class="bi bi-headphones me-2"></i>Accessories
                                        </a></li>
                                    </ul>
                                </li>
                                <li class="nav-item">
                                    <a class="nav-link" href="#">About</a>
                                </li>
                                <li class="nav-item">
                                    <a class="nav-link" href="#">Contact</a>
                                </li>
                            </ul>
                            
                            <!-- Search and user actions -->
                            <div class="d-flex flex-column flex-lg-row align-items-lg-center gap-2">
                                <form class="d-flex">
                                    <input class="form-control me-2" type="search" placeholder="Search">
                                    <button class="btn btn-outline-light" type="submit">
                                        <i class="bi bi-search"></i>
                                    </button>
                                </form>
                                <div class="d-flex gap-2">
                                    <button class="btn btn-outline-light">Login</button>
                                    <button class="btn btn-light">Sign Up</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </nav>
                
                <!-- Breadcrumb navigation -->
                <nav aria-label="breadcrumb">
                    <ol class="breadcrumb bg-white p-3 rounded">
                        <li class="breadcrumb-item"><a href="#">Home</a></li>
                        <li class="breadcrumb-item"><a href="#">Products</a></li>
                        <li class="breadcrumb-item active">Responsive Design</li>
                    </ol>
                </nav>
            </section>
            
            <!-- Responsive Grid Layouts -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Responsive Grid Patterns</h2>
                        <p class="lead text-muted">
                            Common layout patterns that adapt across breakpoints
                        </p>
                    </div>
                </div>
                
                <!-- Pattern 1: Equal columns -->
                <div class="mb-4">
                    <h5 class="mb-3">Pattern 1: Equal Columns</h5>
                    <div class="responsive-grid-demo p-3 rounded">
                        <div class="row g-3">
                            <div class="col-12 col-sm-6 col-lg-3">
                                <div class="bg-primary text-white p-3 rounded text-center">
                                    <h6>Column 1</h6>
                                    <small>Mobile: 12/12<br>Tablet: 6/12<br>Desktop: 3/12</small>
                                </div>
                            </div>
                            <div class="col-12 col-sm-6 col-lg-3">
                                <div class="bg-success text-white p-3 rounded text-center">
                                    <h6>Column 2</h6>
                                    <small>Equal width<br>columns that<br>stack on mobile</small>
                                </div>
                            </div>
                            <div class="col-12 col-sm-6 col-lg-3">
                                <div class="bg-warning text-dark p-3 rounded text-center">
                                    <h6>Column 3</h6>
                                    <small>Responsive<br>breakpoints<br>control layout</small>
                                </div>
                            </div>
                            <div class="col-12 col-sm-6 col-lg-3">
                                <div class="bg-info text-white p-3 rounded text-center">
                                    <h6>Column 4</h6>
                                    <small>Mobile-first<br>approach<br>scales up</small>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Pattern 2: Sidebar layout -->
                <div class="mb-4">
                    <h5 class="mb-3">Pattern 2: Sidebar Layout</h5>
                    <div class="responsive-grid-demo p-3 rounded">
                        <div class="row g-3">
                            <!-- Sidebar: Full width on mobile, 1/4 on desktop -->
                            <div class="col-12 col-lg-3 order-2 order-lg-1">
                                <div class="bg-secondary text-white p-3 rounded">
                                    <h6>Sidebar</h6>
                                    <ul class="list-unstyled mb-0">
                                        <li><a href="#" class="text-white text-decoration-none">
                                            <i class="bi bi-house me-2"></i>Dashboard
                                        </a></li>
                                        <li class="mt-2"><a href="#" class="text-white text-decoration-none">
                                            <i class="bi bi-graph-up me-2"></i>Analytics
                                        </a></li>
                                        <li class="mt-2"><a href="#" class="text-white text-decoration-none">
                                            <i class="bi bi-gear me-2"></i>Settings
                                        </a></li>
                                    </ul>
                                </div>
                            </div>
                            
                            <!-- Main content: Full width on mobile, 3/4 on desktop -->
                            <div class="col-12 col-lg-9 order-1 order-lg-2">
                                <div class="bg-light p-3 rounded">
                                    <h6>Main Content Area</h6>
                                    <p class="mb-3">
                                        This main content area takes full width on mobile devices 
                                        and appears above the sidebar. On desktop, it takes 3/4 
                                        of the width and appears to the right of the sidebar.
                                    </p>
                                    
                                    <!-- Content cards -->
                                    <div class="row g-3">
                                        <div class="col-12 col-md-6">
                                            <div class="demo-card card">
                                                <div class="card-body">
                                                    <h6 class="card-title">Content Card 1</h6>
                                                    <p class="card-text small mb-0">
                                                        Nested responsive layout within main content.
                                                    </p>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-12 col-md-6">
                                            <div class="demo-card card">
                                                <div class="card-body">
                                                    <h6 class="card-title">Content Card 2</h6>
                                                    <p class="card-text small mb-0">
                                                        Two columns on tablet, single column on mobile.
                                                    </p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Pattern 3: Complex asymmetric layout -->
                <div class="mb-4">
                    <h5 class="mb-3">Pattern 3: Asymmetric Layout</h5>
                    <div class="responsive-grid-demo p-3 rounded">
                        <div class="row g-3">
                            <!-- Hero section: Full width -->
                            <div class="col-12">
                                <div class="bg-primary text-white p-4 rounded text-center">
                                    <h5>Hero Section</h5>
                                    <p class="mb-0">Full width across all breakpoints</p>
                                </div>
                            </div>
                            
                            <!-- Featured content: 2/3 width on desktop -->
                            <div class="col-12 col-lg-8">
                                <div class="bg-success text-white p-3 rounded">
                                    <h6>Featured Content</h6>
                                    <p class="mb-0">
                                        Full width on mobile, 2/3 width on desktop. 
                                        Contains the main content and highlights.
                                    </p>
                                </div>
                            </div>
                            
                            <!-- Secondary sidebar: 1/3 width on desktop -->
                            <div class="col-12 col-lg-4">
                                <div class="bg-warning text-dark p-3 rounded">
                                    <h6>Secondary Sidebar</h6>
                                    <p class="mb-0">
                                        Related links, ads, or secondary information.
                                    </p>
                                </div>
                            </div>
                            
                            <!-- Three equal columns -->
                            <div class="col-12 col-md-4">
                                <div class="bg-info text-white p-3 rounded text-center">
                                    <h6>Service 1</h6>
                                    <p class="mb-0">Stacks on mobile</p>
                                </div>
                            </div>
                            <div class="col-12 col-md-4">
                                <div class="bg-danger text-white p-3 rounded text-center">
                                    <h6>Service 2</h6>
                                    <p class="mb-0">Three columns on tablet+</p>
                                </div>
                            </div>
                            <div class="col-12 col-md-4">
                                <div class="bg-dark text-white p-3 rounded text-center">
                                    <h6>Service 3</h6>
                                    <p class="mb-0">Responsive layout</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            
            <!-- Responsive Tables -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Responsive Tables</h2>
                        <p class="lead text-muted">
                            Table layouts that work on all screen sizes
                        </p>
                    </div>
                </div>
                
                <div class="responsive-table-wrapper">
                    <div class="table-responsive">
                        <table class="table table-striped table-hover mb-0">
                            <thead class="table-dark">
                                <tr>
                                    <th scope="col">#</th>
                                    <th scope="col">Product</th>
                                    <th scope="col">Category</th>
                                    <th scope="col" class="d-none d-md-table-cell">Description</th>
                                    <th scope="col">Price</th>
                                    <th scope="col" class="d-none d-lg-table-cell">Stock</th>
                                    <th scope="col">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <th scope="row">1</th>
                                    <td>Laptop Pro</td>
                                    <td>Electronics</td>
                                    <td class="d-none d-md-table-cell">High-performance laptop for professionals</td>
                                    <td>$1,299</td>
                                    <td class="d-none d-lg-table-cell">15</td>
                                    <td>
                                        <div class="btn-group" role="group">
                                            <button class="btn btn-sm btn-outline-primary">
                                                <i class="bi bi-eye"></i>
                                                <span class="d-none d-xl-inline ms-1">View</span>
                                            </button>
                                            <button class="btn btn-sm btn-outline-success">
                                                <i class="bi bi-pencil"></i>
                                                <span class="d-none d-xl-inline ms-1">Edit</span>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th scope="row">2</th>
                                    <td>Smartphone X</td>
                                    <td>Mobile</td>
                                    <td class="d-none d-md-table-cell">Latest smartphone with advanced features</td>
                                    <td>$899</td>
                                    <td class="d-none d-lg-table-cell">32</td>
                                    <td>
                                        <div class="btn-group" role="group">
                                            <button class="btn btn-sm btn-outline-primary">
                                                <i class="bi bi-eye"></i>
                                                <span class="d-none d-xl-inline ms-1">View</span>
                                            </button>
                                            <button class="btn btn-sm btn-outline-success">
                                                <i class="bi bi-pencil"></i>
                                                <span class="d-none d-xl-inline ms-1">Edit</span>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th scope="row">3</th>
                                    <td>Headphones</td>
                                    <td>Audio</td>
                                    <td class="d-none d-md-table-cell">Wireless noise-cancelling headphones</td>
                                    <td>$299</td>
                                    <td class="d-none d-lg-table-cell">8</td>
                                    <td>
                                        <div class="btn-group" role="group">
                                            <button class="btn btn-sm btn-outline-primary">
                                                <i class="bi bi-eye"></i>
                                                <span class="d-none d-xl-inline ms-1">View</span>
                                            </button>
                                            <button class="btn btn-sm btn-outline-success">
                                                <i class="bi bi-pencil"></i>
                                                <span class="d-none d-xl-inline ms-1">Edit</span>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="d-block d-md-none bg-light p-3 border-top">
                        <small class="text-muted">
                            <i class="bi bi-info-circle me-1"></i>
                            Some columns are hidden on mobile for better readability.
                        </small>
                    </div>
                </div>
            </section>
            
            <!-- Testing & Best Practices -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Testing & Best Practices</h2>
                        <p class="lead text-muted">
                            Essential practices for responsive design success
                        </p>
                    </div>
                </div>
                
                <div class="row g-4">
                    <div class="col-lg-6">
                        <div class="demo-card card border-success h-100">
                            <div class="card-header bg-success text-white">
                                <h5 class="mb-0">
                                    <i class="bi bi-check-circle me-2"></i>Best Practices
                                </h5>
                            </div>
                            <div class="card-body">
                                <ul class="list-unstyled">
                                    <li class="mb-3">
                                        <i class="bi bi-phone text-success me-2"></i>
                                        <strong>Mobile First:</strong> Design for mobile, then enhance for larger screens
                                    </li>
                                    <li class="mb-3">
                                        <i class="bi bi-eye text-success me-2"></i>
                                        <strong>Progressive Disclosure:</strong> Show essential content first
                                    </li>
                                    <li class="mb-3">
                                        <i class="bi bi-speedometer text-success me-2"></i>
                                        <strong>Performance:</strong> Optimize images and content for mobile
                                    </li>
                                    <li class="mb-3">
                                        <i class="bi bi-universal-access text-success me-2"></i>
                                        <strong>Accessibility:</strong> Ensure touch targets are large enough
                                    </li>
                                    <li class="mb-0">
                                        <i class="bi bi-device-tablet text-success me-2"></i>
                                        <strong>Test Real Devices:</strong> Use actual devices, not just browser tools
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-lg-6">
                        <div class="demo-card card border-warning h-100">
                            <div class="card-header bg-warning text-dark">
                                <h5 class="mb-0">
                                    <i class="bi bi-exclamation-triangle me-2"></i>Common Pitfalls
                                </h5>
                            </div>
                            <div class="card-body">
                                <ul class="list-unstyled">
                                    <li class="mb-3">
                                        <i class="bi bi-x-circle text-danger me-2"></i>
                                        <strong>Desktop First:</strong> Starting with desktop layouts
                                    </li>
                                    <li class="mb-3">
                                        <i class="bi bi-x-circle text-danger me-2"></i>
                                        <strong>Fixed Widths:</strong> Using fixed pixel widths instead of percentages
                                    </li>
                                    <li class="mb-3">
                                        <i class="bi bi-x-circle text-danger me-2"></i>
                                        <strong>Too Many Breakpoints:</strong> Creating breakpoints for every device
                                    </li>
                                    <li class="mb-3">
                                        <i class="bi bi-x-circle text-danger me-2"></i>
                                        <strong>Ignoring Touch:</strong> Small buttons and links on mobile
                                    </li>
                                    <li class="mb-0">
                                        <i class="bi bi-x-circle text-danger me-2"></i>
                                        <strong>No Testing:</strong> Not testing on actual mobile devices
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </main>
    
    <!-- Footer -->
    <footer class="bg-dark text-white py-5">
        <div class="container">
            <div class="row g-4">
                <div class="col-lg-8">
                    <h3 class="display-6 mb-3">Responsive Design Mastery Complete!</h3>
                    <p class="lead mb-3">
                        You've mastered Bootstrap's responsive design system. Your layouts 
                        now adapt beautifully across all devices and screen sizes.
                    </p>
                    <div class="d-flex flex-wrap gap-2">
                        <span class="badge bg-primary fs-6 px-3 py-2">Mobile-First</span>
                        <span class="badge bg-success fs-6 px-3 py-2">Breakpoints</span>
                        <span class="badge bg-warning text-dark fs-6 px-3 py-2">Responsive Utilities</span>
                        <span class="badge bg-info text-dark fs-6 px-3 py-2">Adaptive Layouts</span>
                    </div>
                </div>
                <div class="col-lg-4 text-lg-end">
                    <h5 class="text-light">Ready for the next challenge?</h5>
                    <p class="text-light opacity-75">
                        Tomorrow we'll optimize performance and learn advanced techniques!
                    </p>
                </div>
            </div>
            <hr class="my-4">
            <div class="row align-items-center">
                <div class="col-md-6">
                    <p class="mb-0">&copy; 2025 Bootstrap Learning Journey</p>
                </div>
                <div class="col-md-6 text-md-end">
                    <small class="text-light opacity-75">
                        Built with Bootstrap 5.3.2 • Mobile-First Design
                    </small>
                </div>
            </div>
        </div>
    </footer>
    
    <!-- Bootstrap JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <!-- Custom JavaScript -->
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // Mobile tab select functionality
            const mobileTabSelect = document.getElementById('mobileTabSelect');
            if (mobileTabSelect) {
                mobileTabSelect.addEventListener('change', function() {
                    const targetTab = document.querySelector(`[data-bs-target="#${this.value}"]`);
                    if (targetTab) {
                        const tab = new bootstrap.Tab(targetTab);
                        tab.show();
                    }
                });
            }
            
            // Responsive breakpoint logger
            function logBreakpoint() {
                const width = window.innerWidth;
                let breakpoint = 'xs';
                
                if (width >= 1400) breakpoint = 'xxl';
                else if (width >= 1200) breakpoint = 'xl';
                else if (width >= 992) breakpoint = 'lg';
                else if (width >= 768) breakpoint = 'md';
                else if (width >= 576) breakpoint = 'sm';
                
                console.log(`Current breakpoint: ${breakpoint} (${width}px)`);
            }
            
            // Log initial breakpoint
            logBreakpoint();
            
            // Log breakpoint on resize (debounced)
            let resizeTimer;
            window.addEventListener('resize', function() {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(logBreakpoint, 250);
            });
            
            // Add hover effects to demo cards
            document.querySelectorAll('.demo-card').forEach(card => {
                card.addEventListener('mouseenter', function() {
                    this.style.transform = 'translateY(-5px) scale(1.02)';
                });
                
                card.addEventListener('mouseleave', function() {
                    this.style.transform = 'translateY(0) scale(1)';
                });
            });
            
            // Smooth scroll for anchor links
            document.querySelectorAll('a[href^="#"]').forEach(link => {
                link.addEventListener('click', function(e) {
                    e.preventDefault();
                    const target = document.querySelector(this.getAttribute('href'));
                    if (target) {
                        target.scrollIntoView({ 
                            behavior: 'smooth',
                            block: 'start'
                        });
                    }
                });
            });
            
            console.log('Responsive Design Patterns Demo Loaded! 📱💻');
        });
    </script>
</body>
</html>
```

## 📋 Responsive Design Mastery Checklist

### Mobile-First Approach
- [ ] Understand mobile-first philosophy and benefits
- [ ] Design for smallest screen first, then enhance
- [ ] Use progressive enhancement techniques
- [ ] Prioritize essential content for mobile

### Breakpoint Management
- [ ] Master all 6 Bootstrap breakpoints (xs, sm, md, lg, xl, xxl)
- [ ] Use appropriate breakpoints for design changes
- [ ] Avoid creating too many custom breakpoints
- [ ] Test breakpoint transitions thoroughly

### Responsive Utilities
- [ ] Use responsive display utilities (d-none, d-block, etc.)
- [ ] Apply responsive spacing (m-*, p-* with breakpoints)
- [ ] Implement responsive text alignment and sizing
- [ ] Master responsive flexbox utilities

### Layout Patterns
- [ ] Create responsive navigation patterns
- [ ] Build adaptive sidebar layouts
- [ ] Implement responsive card grids
- [ ] Design mobile-friendly tables

### Testing & Optimization
- [ ] Test on real devices, not just browser tools
- [ ] Verify touch targets are appropriately sized
- [ ] Ensure good performance on mobile networks
- [ ] Check accessibility across all breakpoints

## 🎉 Day 12 Complete!

### ✅ Today's Achievements
- **Mobile-First Mastery:** Complete understanding of mobile-first design approach
- **Breakpoint System:** Comprehensive knowledge of all Bootstrap breakpoints
- **Responsive Utilities:** Advanced use of responsive display, spacing, and layout utilities
- **Navigation Patterns:** Adaptive navigation that works across all devices
- **Layout Mastery:** Complex responsive layouts with proper content hierarchy
- **Best Practices:** Professional responsive design techniques and testing strategies

### 🏆 Skills Gained
- Mobile-first design philosophy and implementation
- Bootstrap responsive breakpoint system
- Advanced responsive utility classes
- Adaptive navigation and layout patterns
- Performance optimization for mobile devices
- Professional responsive design workflow

## 🔗 Additional Resources

- **[Bootstrap Breakpoints Docs](https://getbootstrap.com/docs/5.3/layout/breakpoints/)** - Complete breakpoint reference
- **[Responsive Design Guidelines](https://web.dev/responsive-web-design-basics/)** - Google's responsive design guide
- **[Mobile-First Design](https://bradfrost.com/blog/web/mobile-first-responsive-web-design/)** - Mobile-first methodology
- **[Can I Use](https://caniuse.com/)** - Browser support for responsive features

---

## 🚀 What's Next?

Tomorrow in **Day 13: Performance & Optimization**, you'll learn:
- Bootstrap performance optimization techniques
- Custom builds and tree shaking
- CSS purging and minimization
- Production deployment best practices

**Excellent work on Day 12!** You now have the skills to create truly responsive designs that work beautifully on any device. Tomorrow we'll focus on optimizing your Bootstrap projects for maximum performance.

---

**Remember: Great responsive design is invisible to users – it just works naturally across all devices!** 📱💻✨