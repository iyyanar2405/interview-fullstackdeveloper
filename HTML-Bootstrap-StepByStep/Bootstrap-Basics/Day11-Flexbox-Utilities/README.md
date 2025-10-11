# Day 11: Bootstrap Flexbox & Utilities 🔧

Welcome to Day 11! Today you'll master Bootstrap's powerful flexbox utilities and spacing system that make complex layouts simple and intuitive. You'll learn to create flexible, responsive layouts with precise control over alignment, distribution, and spacing.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master Bootstrap's flexbox utility classes
- Understand the complete spacing and sizing system
- Use display and positioning utilities effectively
- Create complex responsive layouts with flexbox
- Apply border, shadow, and visual utilities
- Build professional layouts with utility-first approach
- Optimize layouts for all screen sizes
- Combine utilities for advanced design patterns

## 📚 Lesson Content

### Bootstrap Flexbox System

Bootstrap's flexbox utilities provide complete control over flex containers and items with responsive variations.

#### Flex Container Basics
```html
<div class="container">
    <!-- Basic flex container -->
    <div class="d-flex bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2 me-2">Item 1</div>
        <div class="bg-success text-white p-2 me-2">Item 2</div>
        <div class="bg-warning text-dark p-2">Item 3</div>
    </div>
    
    <!-- Inline flex container -->
    <div class="d-inline-flex bg-light p-3 mb-3 border rounded">
        <div class="bg-info text-dark p-2 me-2">Inline 1</div>
        <div class="bg-secondary text-white p-2">Inline 2</div>
    </div>
    
    <!-- Responsive flex containers -->
    <div class="d-sm-flex d-md-inline-flex bg-light p-3 border rounded">
        <div class="bg-danger text-white p-2 me-2">Responsive</div>
        <div class="bg-dark text-white p-2">Flex Container</div>
    </div>
</div>
```

#### Flex Direction
```html
<div class="container">
    <!-- Row direction (default) -->
    <div class="d-flex flex-row bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2 me-2">1</div>
        <div class="bg-success text-white p-2 me-2">2</div>
        <div class="bg-warning text-dark p-2">3</div>
    </div>
    
    <!-- Row reverse -->
    <div class="d-flex flex-row-reverse bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2 ms-2">1</div>
        <div class="bg-success text-white p-2 ms-2">2</div>
        <div class="bg-warning text-dark p-2">3</div>
    </div>
    
    <!-- Column direction -->
    <div class="d-flex flex-column bg-light p-3 mb-3 border rounded" style="height: 200px;">
        <div class="bg-primary text-white p-2 mb-2">1</div>
        <div class="bg-success text-white p-2 mb-2">2</div>
        <div class="bg-warning text-dark p-2">3</div>
    </div>
    
    <!-- Column reverse -->
    <div class="d-flex flex-column-reverse bg-light p-3 mb-3 border rounded" style="height: 200px;">
        <div class="bg-primary text-white p-2 mt-2">1</div>
        <div class="bg-success text-white p-2 mt-2">2</div>
        <div class="bg-warning text-dark p-2">3</div>
    </div>
    
    <!-- Responsive direction -->
    <div class="d-flex flex-column flex-md-row bg-light p-3 border rounded">
        <div class="bg-info text-dark p-2 me-md-2 mb-2 mb-md-0">Responsive</div>
        <div class="bg-secondary text-white p-2">Direction</div>
    </div>
</div>
```

#### Justify Content (Main Axis)
```html
<div class="container">
    <!-- Start (default) -->
    <div class="d-flex justify-content-start bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2">Start</div>
        <div class="bg-success text-white p-2 ms-2">Alignment</div>
    </div>
    
    <!-- End -->
    <div class="d-flex justify-content-end bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2">End</div>
        <div class="bg-success text-white p-2 ms-2">Alignment</div>
    </div>
    
    <!-- Center -->
    <div class="d-flex justify-content-center bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2">Center</div>
        <div class="bg-success text-white p-2 ms-2">Alignment</div>
    </div>
    
    <!-- Space Between -->
    <div class="d-flex justify-content-between bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2">Space</div>
        <div class="bg-success text-white p-2">Between</div>
        <div class="bg-warning text-dark p-2">Items</div>
    </div>
    
    <!-- Space Around -->
    <div class="d-flex justify-content-around bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2">Space</div>
        <div class="bg-success text-white p-2">Around</div>
        <div class="bg-warning text-dark p-2">Items</div>
    </div>
    
    <!-- Space Evenly -->
    <div class="d-flex justify-content-evenly bg-light p-3 mb-3 border rounded">
        <div class="bg-primary text-white p-2">Space</div>
        <div class="bg-success text-white p-2">Evenly</div>
        <div class="bg-warning text-dark p-2">Items</div>
    </div>
    
    <!-- Responsive justify content -->
    <div class="d-flex justify-content-start justify-content-md-center justify-content-lg-end bg-light p-3 border rounded">
        <div class="bg-info text-dark p-2">Responsive</div>
        <div class="bg-secondary text-white p-2 ms-2">Justify</div>
    </div>
</div>
```

#### Align Items (Cross Axis)
```html
<div class="container">
    <!-- Stretch (default) -->
    <div class="d-flex align-items-stretch bg-light p-3 mb-3 border rounded" style="height: 100px;">
        <div class="bg-primary text-white p-2 me-2">Stretch</div>
        <div class="bg-success text-white p-2 me-2">Default</div>
        <div class="bg-warning text-dark p-2">Behavior</div>
    </div>
    
    <!-- Start -->
    <div class="d-flex align-items-start bg-light p-3 mb-3 border rounded" style="height: 100px;">
        <div class="bg-primary text-white p-2 me-2">Start</div>
        <div class="bg-success text-white p-2 me-2">Alignment</div>
        <div class="bg-warning text-dark p-2">Top</div>
    </div>
    
    <!-- Center -->
    <div class="d-flex align-items-center bg-light p-3 mb-3 border rounded" style="height: 100px;">
        <div class="bg-primary text-white p-2 me-2">Center</div>
        <div class="bg-success text-white p-2 me-2">Alignment</div>
        <div class="bg-warning text-dark p-2">Middle</div>
    </div>
    
    <!-- End -->
    <div class="d-flex align-items-end bg-light p-3 mb-3 border rounded" style="height: 100px;">
        <div class="bg-primary text-white p-2 me-2">End</div>
        <div class="bg-success text-white p-2 me-2">Alignment</div>
        <div class="bg-warning text-dark p-2">Bottom</div>
    </div>
    
    <!-- Baseline -->
    <div class="d-flex align-items-baseline bg-light p-3 mb-3 border rounded" style="height: 100px;">
        <div class="bg-primary text-white p-2 me-2 fs-1">Big</div>
        <div class="bg-success text-white p-2 me-2">Normal</div>
        <div class="bg-warning text-dark p-2 fs-6">Small</div>
    </div>
    
    <!-- Individual item alignment -->
    <div class="d-flex bg-light p-3 border rounded" style="height: 120px;">
        <div class="bg-primary text-white p-2 me-2 align-self-start">Self Start</div>
        <div class="bg-success text-white p-2 me-2 align-self-center">Self Center</div>
        <div class="bg-warning text-dark p-2 me-2 align-self-end">Self End</div>
        <div class="bg-info text-dark p-2 align-self-stretch">Self Stretch</div>
    </div>
</div>
```

### Bootstrap Spacing System

Bootstrap's spacing utilities provide consistent margin and padding using a mathematical scale.

#### Margin and Padding Scale
```html
<div class="container">
    <!-- Margin examples -->
    <h4>Margin Examples</h4>
    <div class="bg-light p-3 mb-4 border rounded">
        <div class="bg-primary text-white p-2 m-0 mb-2">m-0 (0)</div>
        <div class="bg-primary text-white p-2 m-1 mb-2">m-1 (0.25rem)</div>
        <div class="bg-primary text-white p-2 m-2 mb-2">m-2 (0.5rem)</div>
        <div class="bg-primary text-white p-2 m-3 mb-2">m-3 (1rem)</div>
        <div class="bg-primary text-white p-2 m-4 mb-2">m-4 (1.5rem)</div>
        <div class="bg-primary text-white p-2 m-5">m-5 (3rem)</div>
    </div>
    
    <!-- Padding examples -->
    <h4>Padding Examples</h4>
    <div class="bg-light p-3 mb-4 border rounded">
        <div class="bg-success text-white p-0 mb-2">p-0 (no padding)</div>
        <div class="bg-success text-white p-1 mb-2">p-1 (0.25rem)</div>
        <div class="bg-success text-white p-2 mb-2">p-2 (0.5rem)</div>
        <div class="bg-success text-white p-3 mb-2">p-3 (1rem)</div>
        <div class="bg-success text-white p-4 mb-2">p-4 (1.5rem)</div>
        <div class="bg-success text-white p-5">p-5 (3rem)</div>
    </div>
    
    <!-- Directional spacing -->
    <h4>Directional Spacing</h4>
    <div class="bg-light p-3 mb-4 border rounded">
        <div class="bg-warning text-dark p-2 mt-3">mt-3 (margin-top)</div>
        <div class="bg-warning text-dark p-2 mb-3">mb-3 (margin-bottom)</div>
        <div class="bg-warning text-dark p-2 ms-3">ms-3 (margin-start/left)</div>
        <div class="bg-warning text-dark p-2 me-3">me-3 (margin-end/right)</div>
        <div class="bg-warning text-dark p-2 mx-3">mx-3 (margin horizontal)</div>
        <div class="bg-warning text-dark p-2 my-3">my-3 (margin vertical)</div>
    </div>
    
    <!-- Auto margins for centering -->
    <div class="bg-light p-3 border rounded">
        <div class="bg-info text-dark p-2 mx-auto" style="width: 200px;">mx-auto (centered)</div>
    </div>
</div>
```

#### Responsive Spacing
```html
<div class="container">
    <!-- Responsive margins -->
    <div class="bg-light p-3 mb-4 border rounded">
        <div class="bg-primary text-white p-2 m-2 m-md-4 m-lg-5">
            Responsive margin: m-2 (small), m-md-4 (medium), m-lg-5 (large)
        </div>
    </div>
    
    <!-- Responsive padding -->
    <div class="bg-light border rounded">
        <div class="bg-success text-white p-2 p-md-3 p-lg-4">
            Responsive padding: p-2 (small), p-md-3 (medium), p-lg-4 (large)
        </div>
    </div>
</div>
```

### Display Utilities

Control element display behavior with Bootstrap's display utilities.

```html
<div class="container">
    <!-- Basic display utilities -->
    <div class="d-block bg-primary text-white p-2 mb-2">d-block (block element)</div>
    <div class="d-inline bg-success text-white p-2 me-2">d-inline</div>
    <div class="d-inline bg-warning text-dark p-2 me-2">inline elements</div>
    <div class="d-inline bg-info text-dark p-2">on same line</div>
    
    <div class="mt-3">
        <div class="d-inline-block bg-danger text-white p-2 me-2">d-inline-block</div>
        <div class="d-inline-block bg-secondary text-white p-2">inline but blocklike</div>
    </div>
    
    <!-- Responsive display -->
    <div class="d-none d-md-block bg-dark text-white p-2 mt-3">
        Hidden on mobile, visible on medium screens and up
    </div>
    
    <div class="d-block d-lg-none bg-warning text-dark p-2 mt-2">
        Visible on small/medium, hidden on large screens and up
    </div>
    
    <!-- Print utilities -->
    <div class="d-print-none bg-info text-dark p-2 mt-2">
        Hidden when printing (d-print-none)
    </div>
    
    <div class="d-none d-print-block bg-success text-white p-2 mt-2">
        Only visible when printing (d-none d-print-block)
    </div>
</div>
```

### Position Utilities

```html
<div class="container">
    <!-- Position examples -->
    <div class="position-relative bg-light p-4 mb-4 border rounded" style="height: 200px;">
        <div class="position-absolute top-0 start-0 bg-primary text-white p-2">
            top-0 start-0
        </div>
        <div class="position-absolute top-0 end-0 bg-success text-white p-2">
            top-0 end-0
        </div>
        <div class="position-absolute bottom-0 start-0 bg-warning text-dark p-2">
            bottom-0 start-0
        </div>
        <div class="position-absolute bottom-0 end-0 bg-danger text-white p-2">
            bottom-0 end-0
        </div>
        <div class="position-absolute top-50 start-50 translate-middle bg-info text-dark p-2">
            Centered with translate-middle
        </div>
    </div>
    
    <!-- Sticky positioning -->
    <div class="position-sticky bg-secondary text-white p-2 mb-4" style="top: 10px;">
        Sticky element (position-sticky)
    </div>
    
    <!-- Fixed positioning -->
    <div class="position-fixed bottom-0 end-0 bg-dark text-white p-2 m-3 rounded">
        Fixed position (bottom-right)
    </div>
</div>
```

## 💻 Hands-On Practice

### Exercise 1: Complete Flexbox & Utilities Showcase

Create `flexbox-utilities-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bootstrap Flexbox & Utilities Mastery</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" 
          rel="stylesheet">
    
    <!-- Bootstrap Icons -->
    <link rel="stylesheet" 
          href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <style>
        .demo-item {
            min-height: 50px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 500;
            border-radius: 6px;
            transition: all 0.3s ease;
        }
        
        .demo-container {
            background: #f8f9fa;
            border: 2px dashed #dee2e6;
            border-radius: 8px;
            min-height: 100px;
            position: relative;
        }
        
        .demo-container::before {
            content: attr(data-label);
            position: absolute;
            top: -10px;
            left: 10px;
            background: #fff;
            padding: 2px 8px;
            font-size: 0.75rem;
            color: #6c757d;
            border-radius: 4px;
        }
        
        .utility-card {
            transition: all 0.3s ease;
            border: none;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        .utility-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.15);
        }
        
        .spacing-demo {
            background: repeating-linear-gradient(
                45deg,
                #f8f9fa,
                #f8f9fa 10px,
                #e9ecef 10px,
                #e9ecef 20px
            );
        }
        
        .interactive-demo {
            cursor: pointer;
            user-select: none;
        }
        
        .interactive-demo:hover {
            opacity: 0.8;
        }
        
        .gradient-bg {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        }
        
        .glass-effect {
            background: rgba(255, 255, 255, 0.1);
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }
    </style>
</head>
<body>
    <!-- Hero Section -->
    <header class="gradient-bg text-white py-5 mb-5">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-lg-8">
                    <h1 class="display-3 fw-bold mb-3">
                        Flexbox & Utilities
                    </h1>
                    <p class="lead fs-4 mb-4">
                        Master Bootstrap's powerful flexbox system and utility classes 
                        for creating flexible, responsive layouts with ease.
                    </p>
                    <div class="d-flex flex-wrap gap-3">
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Flexbox</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Spacing</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Utilities</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Responsive</span>
                    </div>
                </div>
                <div class="col-lg-4 text-center">
                    <i class="bi bi-grid-3x3-gap display-1 opacity-75"></i>
                </div>
            </div>
        </div>
    </header>
    
    <div class="container">
        <!-- Flexbox Direction -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Flex Direction Control</h2>
            <p class="lead mb-4">Control the direction of flex items with responsive direction utilities.</p>
            
            <div class="row g-4">
                <div class="col-lg-6">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-primary text-white">
                            <h5 class="mb-0">
                                <i class="bi bi-arrow-right me-2"></i>Row Direction
                            </h5>
                        </div>
                        <div class="card-body">
                            <div class="demo-container d-flex flex-row p-3 mb-3" data-label="flex-row">
                                <div class="demo-item bg-primary text-white me-2">1</div>
                                <div class="demo-item bg-success text-white me-2">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                            
                            <div class="demo-container d-flex flex-row-reverse p-3" data-label="flex-row-reverse">
                                <div class="demo-item bg-primary text-white ms-2">1</div>
                                <div class="demo-item bg-success text-white ms-2">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-success text-white">
                            <h5 class="mb-0">
                                <i class="bi bi-arrow-down me-2"></i>Column Direction
                            </h5>
                        </div>
                        <div class="card-body">
                            <div class="demo-container d-flex flex-column p-3 mb-3" data-label="flex-column" style="height: 150px;">
                                <div class="demo-item bg-primary text-white mb-2">1</div>
                                <div class="demo-item bg-success text-white mb-2">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                            
                            <div class="demo-container d-flex flex-column-reverse p-3" data-label="flex-column-reverse" style="height: 150px;">
                                <div class="demo-item bg-primary text-white mt-2">1</div>
                                <div class="demo-item bg-success text-white mt-2">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Justify Content -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Justify Content (Main Axis)</h2>
            <p class="lead mb-4">Control the alignment and distribution of flex items along the main axis.</p>
            
            <div class="row g-3">
                <div class="col-lg-4">
                    <div class="utility-card card">
                        <div class="card-header bg-info text-dark">
                            <h6 class="mb-0">justify-content-start</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex justify-content-start p-2" data-label="start">
                                <div class="demo-item bg-primary text-white me-1">1</div>
                                <div class="demo-item bg-success text-white me-1">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="utility-card card">
                        <div class="card-header bg-info text-dark">
                            <h6 class="mb-0">justify-content-center</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex justify-content-center p-2" data-label="center">
                                <div class="demo-item bg-primary text-white me-1">1</div>
                                <div class="demo-item bg-success text-white me-1">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="utility-card card">
                        <div class="card-header bg-info text-dark">
                            <h6 class="mb-0">justify-content-end</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex justify-content-end p-2" data-label="end">
                                <div class="demo-item bg-primary text-white me-1">1</div>
                                <div class="demo-item bg-success text-white me-1">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="utility-card card">
                        <div class="card-header bg-warning text-dark">
                            <h6 class="mb-0">justify-content-between</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex justify-content-between p-2" data-label="between">
                                <div class="demo-item bg-primary text-white">1</div>
                                <div class="demo-item bg-success text-white">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="utility-card card">
                        <div class="card-header bg-warning text-dark">
                            <h6 class="mb-0">justify-content-around</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex justify-content-around p-2" data-label="around">
                                <div class="demo-item bg-primary text-white">1</div>
                                <div class="demo-item bg-success text-white">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="utility-card card">
                        <div class="card-header bg-warning text-dark">
                            <h6 class="mb-0">justify-content-evenly</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex justify-content-evenly p-2" data-label="evenly">
                                <div class="demo-item bg-primary text-white">1</div>
                                <div class="demo-item bg-success text-white">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Align Items -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Align Items (Cross Axis)</h2>
            <p class="lead mb-4">Control the alignment of flex items along the cross axis.</p>
            
            <div class="row g-3">
                <div class="col-lg-6">
                    <div class="utility-card card">
                        <div class="card-header bg-danger text-white">
                            <h6 class="mb-0">align-items-start</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex align-items-start p-2" data-label="start" style="height: 120px;">
                                <div class="demo-item bg-primary text-white me-1">1</div>
                                <div class="demo-item bg-success text-white me-1">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="utility-card card">
                        <div class="card-header bg-danger text-white">
                            <h6 class="mb-0">align-items-center</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex align-items-center p-2" data-label="center" style="height: 120px;">
                                <div class="demo-item bg-primary text-white me-1">1</div>
                                <div class="demo-item bg-success text-white me-1">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="utility-card card">
                        <div class="card-header bg-danger text-white">
                            <h6 class="mb-0">align-items-end</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex align-items-end p-2" data-label="end" style="height: 120px;">
                                <div class="demo-item bg-primary text-white me-1">1</div>
                                <div class="demo-item bg-success text-white me-1">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="utility-card card">
                        <div class="card-header bg-danger text-white">
                            <h6 class="mb-0">align-items-stretch</h6>
                        </div>
                        <div class="card-body p-2">
                            <div class="demo-container d-flex align-items-stretch p-2" data-label="stretch" style="height: 120px;">
                                <div class="demo-item bg-primary text-white me-1">1</div>
                                <div class="demo-item bg-success text-white me-1">2</div>
                                <div class="demo-item bg-warning text-dark">3</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Spacing System -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Bootstrap Spacing System</h2>
            <p class="lead mb-4">Consistent spacing using Bootstrap's mathematical scale from 0 to 5.</p>
            
            <div class="row g-4">
                <div class="col-lg-6">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-secondary text-white">
                            <h5 class="mb-0">
                                <i class="bi bi-arrows-expand me-2"></i>Margin Scale
                            </h5>
                        </div>
                        <div class="card-body spacing-demo">
                            <div class="bg-primary text-white p-2 m-0 mb-2 rounded">m-0 (0)</div>
                            <div class="bg-primary text-white p-2 m-1 mb-2 rounded">m-1 (0.25rem)</div>
                            <div class="bg-primary text-white p-2 m-2 mb-2 rounded">m-2 (0.5rem)</div>
                            <div class="bg-primary text-white p-2 m-3 mb-2 rounded">m-3 (1rem)</div>
                            <div class="bg-primary text-white p-2 m-4 mb-2 rounded">m-4 (1.5rem)</div>
                            <div class="bg-primary text-white p-2 m-5 rounded">m-5 (3rem)</div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-success text-white">
                            <h5 class="mb-0">
                                <i class="bi bi-arrows-collapse me-2"></i>Padding Scale
                            </h5>
                        </div>
                        <div class="card-body">
                            <div class="bg-success text-white p-0 mb-2 rounded text-center">p-0</div>
                            <div class="bg-success text-white p-1 mb-2 rounded text-center">p-1</div>
                            <div class="bg-success text-white p-2 mb-2 rounded text-center">p-2</div>
                            <div class="bg-success text-white p-3 mb-2 rounded text-center">p-3</div>
                            <div class="bg-success text-white p-4 mb-2 rounded text-center">p-4</div>
                            <div class="bg-success text-white p-5 rounded text-center">p-5</div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-info text-dark">
                            <h5 class="mb-0">
                                <i class="bi bi-arrows-angle-expand me-2"></i>Directional Spacing
                            </h5>
                        </div>
                        <div class="card-body spacing-demo">
                            <div class="bg-warning text-dark p-2 mt-3 mb-1 rounded">mt-3 (margin-top)</div>
                            <div class="bg-warning text-dark p-2 mb-3 mb-1 rounded">mb-3 (margin-bottom)</div>
                            <div class="bg-warning text-dark p-2 ms-3 mb-1 rounded">ms-3 (margin-start)</div>
                            <div class="bg-warning text-dark p-2 me-3 mb-1 rounded">me-3 (margin-end)</div>
                            <div class="bg-warning text-dark p-2 mx-3 mb-1 rounded">mx-3 (horizontal)</div>
                            <div class="bg-warning text-dark p-2 my-3 rounded">my-3 (vertical)</div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-dark text-white">
                            <h5 class="mb-0">
                                <i class="bi bi-arrows-fullscreen me-2"></i>Auto & Negative
                            </h5>
                        </div>
                        <div class="card-body">
                            <div class="bg-info text-dark p-2 mx-auto mb-3 rounded text-center" style="width: 150px;">
                                mx-auto (centered)
                            </div>
                            <div class="bg-danger text-white p-2 ms-n2 mb-2 rounded">ms-n2 (negative margin)</div>
                            <div class="bg-danger text-white p-2 mt-n1 rounded">mt-n1 (negative margin)</div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Interactive Demo -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Interactive Flexbox Demo</h2>
            <p class="lead mb-4">Click the buttons to see flexbox properties in action!</p>
            
            <div class="utility-card card">
                <div class="card-header bg-primary text-white">
                    <h5 class="mb-0">
                        <i class="bi bi-play-circle me-2"></i>Interactive Controls
                    </h5>
                </div>
                <div class="card-body">
                    <!-- Controls -->
                    <div class="row g-3 mb-4">
                        <div class="col-md-4">
                            <label class="form-label fw-bold">Direction:</label>
                            <div class="btn-group w-100" role="group">
                                <button type="button" class="btn btn-outline-primary active" data-flex="flex-row">Row</button>
                                <button type="button" class="btn btn-outline-primary" data-flex="flex-column">Column</button>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label fw-bold">Justify:</label>
                            <select class="form-select" id="justifySelect">
                                <option value="justify-content-start">Start</option>
                                <option value="justify-content-center">Center</option>
                                <option value="justify-content-end">End</option>
                                <option value="justify-content-between">Between</option>
                                <option value="justify-content-around">Around</option>
                                <option value="justify-content-evenly">Evenly</option>
                            </select>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label fw-bold">Align:</label>
                            <select class="form-select" id="alignSelect">
                                <option value="align-items-start">Start</option>
                                <option value="align-items-center">Center</option>
                                <option value="align-items-end">End</option>
                                <option value="align-items-stretch">Stretch</option>
                            </select>
                        </div>
                    </div>
                    
                    <!-- Demo Container -->
                    <div id="interactiveDemo" class="demo-container d-flex flex-row justify-content-start align-items-start p-3" 
                         data-label="Interactive Demo" style="height: 200px;">
                        <div class="demo-item bg-primary text-white me-2 mb-2">Item 1</div>
                        <div class="demo-item bg-success text-white me-2 mb-2">Item 2</div>
                        <div class="demo-item bg-warning text-dark me-2 mb-2">Item 3</div>
                        <div class="demo-item bg-info text-dark mb-2">Item 4</div>
                    </div>
                    
                    <!-- Current Classes Display -->
                    <div class="mt-3">
                        <strong>Current Classes:</strong>
                        <code id="currentClasses" class="ms-2">d-flex flex-row justify-content-start align-items-start</code>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Real-World Examples -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Real-World Layout Examples</h2>
            <p class="lead mb-4">See how flexbox utilities create common UI patterns.</p>
            
            <div class="row g-4">
                <!-- Card Layout -->
                <div class="col-lg-4">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-success text-white">
                            <h6 class="mb-0">Product Card Layout</h6>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <img src="https://via.placeholder.com/200x120/6f42c1/ffffff?text=Product" 
                                 class="card-img-top mb-3 rounded" alt="Product">
                            <h6 class="card-title">Premium Headphones</h6>
                            <p class="card-text flex-grow-1 text-muted">
                                High-quality wireless headphones with noise cancellation.
                            </p>
                            <div class="d-flex justify-content-between align-items-center mt-auto">
                                <span class="h5 text-success mb-0">$299</span>
                                <button class="btn btn-primary btn-sm">Add to Cart</button>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Navigation Bar -->
                <div class="col-lg-8">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-info text-dark">
                            <h6 class="mb-0">Navigation Bar Layout</h6>
                        </div>
                        <div class="card-body">
                            <nav class="d-flex justify-content-between align-items-center bg-dark text-white p-3 rounded mb-3">
                                <div class="d-flex align-items-center">
                                    <i class="bi bi-house-door me-2"></i>
                                    <span class="fw-bold">Brand</span>
                                </div>
                                <div class="d-flex gap-3">
                                    <a href="#" class="text-white text-decoration-none">Home</a>
                                    <a href="#" class="text-white text-decoration-none">About</a>
                                    <a href="#" class="text-white text-decoration-none">Contact</a>
                                </div>
                                <button class="btn btn-outline-light btn-sm">Login</button>
                            </nav>
                            
                            <!-- Mobile Navigation -->
                            <nav class="d-flex flex-column d-md-none bg-secondary text-white p-3 rounded">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <span class="fw-bold">Mobile Brand</span>
                                    <i class="bi bi-list"></i>
                                </div>
                                <div class="d-flex flex-column gap-2">
                                    <a href="#" class="text-white text-decoration-none">Home</a>
                                    <a href="#" class="text-white text-decoration-none">About</a>
                                    <a href="#" class="text-white text-decoration-none">Contact</a>
                                </div>
                            </nav>
                        </div>
                    </div>
                </div>
                
                <!-- Footer Layout -->
                <div class="col-lg-6">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-warning text-dark">
                            <h6 class="mb-0">Footer Layout</h6>
                        </div>
                        <div class="card-body">
                            <footer class="bg-dark text-white p-4 rounded">
                                <div class="d-flex flex-column flex-md-row justify-content-between align-items-center">
                                    <div class="mb-3 mb-md-0">
                                        <p class="mb-0">&copy; 2025 Company Name</p>
                                    </div>
                                    <div class="d-flex gap-3">
                                        <a href="#" class="text-white"><i class="bi bi-facebook"></i></a>
                                        <a href="#" class="text-white"><i class="bi bi-twitter"></i></a>
                                        <a href="#" class="text-white"><i class="bi bi-instagram"></i></a>
                                    </div>
                                </div>
                            </footer>
                        </div>
                    </div>
                </div>
                
                <!-- Feature List -->
                <div class="col-lg-6">
                    <div class="utility-card card h-100">
                        <div class="card-header bg-danger text-white">
                            <h6 class="mb-0">Feature List Layout</h6>
                        </div>
                        <div class="card-body">
                            <div class="d-flex flex-column gap-3">
                                <div class="d-flex align-items-center">
                                    <div class="bg-success text-white rounded-circle p-2 me-3">
                                        <i class="bi bi-check"></i>
                                    </div>
                                    <div>
                                        <h6 class="mb-1">Fast Performance</h6>
                                        <small class="text-muted">Lightning-fast loading times</small>
                                    </div>
                                </div>
                                
                                <div class="d-flex align-items-center">
                                    <div class="bg-primary text-white rounded-circle p-2 me-3">
                                        <i class="bi bi-shield"></i>
                                    </div>
                                    <div>
                                        <h6 class="mb-1">Secure</h6>
                                        <small class="text-muted">End-to-end encryption</small>
                                    </div>
                                </div>
                                
                                <div class="d-flex align-items-center">
                                    <div class="bg-warning text-dark rounded-circle p-2 me-3">
                                        <i class="bi bi-heart"></i>
                                    </div>
                                    <div>
                                        <h6 class="mb-1">User-Friendly</h6>
                                        <small class="text-muted">Intuitive interface design</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
    
    <!-- Footer -->
    <footer class="bg-dark text-white py-5 mt-5">
        <div class="container">
            <div class="row">
                <div class="col-lg-8">
                    <h3 class="display-6 mb-3">Flexbox & Utilities Mastery Complete!</h3>
                    <p class="lead mb-3">
                        You've mastered Bootstrap's flexbox system and utility classes. 
                        These are the building blocks for any modern responsive layout.
                    </p>
                    <div class="d-flex flex-wrap gap-2">
                        <span class="badge bg-primary fs-6 px-3 py-2">Flexbox Control</span>
                        <span class="badge bg-success fs-6 px-3 py-2">Spacing System</span>
                        <span class="badge bg-warning text-dark fs-6 px-3 py-2">Layout Utilities</span>
                        <span class="badge bg-info text-dark fs-6 px-3 py-2">Responsive Design</span>
                    </div>
                </div>
                <div class="col-lg-4 text-lg-end">
                    <h5 class="text-light">Ready for the next challenge?</h5>
                    <p class="text-light opacity-75">
                        Tomorrow we'll explore advanced responsive design patterns!
                    </p>
                </div>
            </div>
        </div>
    </footer>
    
    <!-- Bootstrap JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <!-- Custom JavaScript -->
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            const interactiveDemo = document.getElementById('interactiveDemo');
            const justifySelect = document.getElementById('justifySelect');
            const alignSelect = document.getElementById('alignSelect');
            const currentClasses = document.getElementById('currentClasses');
            const flexButtons = document.querySelectorAll('[data-flex]');
            
            let currentDirection = 'flex-row';
            let currentJustify = 'justify-content-start';
            let currentAlign = 'align-items-start';
            
            // Update classes display
            function updateClasses() {
                const classes = `d-flex ${currentDirection} ${currentJustify} ${currentAlign}`;
                currentClasses.textContent = classes;
                
                // Apply classes to demo
                interactiveDemo.className = `demo-container d-flex ${currentDirection} ${currentJustify} ${currentAlign} p-3`;
            }
            
            // Direction buttons
            flexButtons.forEach(button => {
                button.addEventListener('click', function() {
                    // Remove active from siblings
                    this.parentNode.querySelectorAll('.btn').forEach(btn => btn.classList.remove('active'));
                    this.classList.add('active');
                    
                    currentDirection = this.dataset.flex;
                    updateClasses();
                });
            });
            
            // Justify select
            justifySelect.addEventListener('change', function() {
                currentJustify = this.value;
                updateClasses();
            });
            
            // Align select
            alignSelect.addEventListener('change', function() {
                currentAlign = this.value;
                updateClasses();
            });
            
            // Hover effects for demo items
            document.querySelectorAll('.demo-item').forEach(item => {
                item.addEventListener('mouseenter', function() {
                    this.style.transform = 'scale(1.1)';
                });
                
                item.addEventListener('mouseleave', function() {
                    this.style.transform = 'scale(1)';
                });
            });
            
            // Copy code functionality
            document.querySelectorAll('code').forEach(code => {
                code.style.cursor = 'pointer';
                code.title = 'Click to copy';
                
                code.addEventListener('click', function() {
                    navigator.clipboard.writeText(this.textContent).then(() => {
                        const original = this.style.backgroundColor;
                        this.style.backgroundColor = '#28a745';
                        this.style.color = 'white';
                        
                        setTimeout(() => {
                            this.style.backgroundColor = original;
                            this.style.color = '';
                        }, 500);
                    });
                });
            });
            
            console.log('Flexbox & Utilities Demo Loaded! 🔧');
        });
    </script>
</body>
</html>
```

## 📋 Flexbox & Utilities Mastery Checklist

### Flexbox Fundamentals
- [ ] Understand flex containers (d-flex, d-inline-flex)
- [ ] Master flex direction utilities (flex-row, flex-column)
- [ ] Control flex wrap behavior (flex-wrap, flex-nowrap)
- [ ] Use responsive flex utilities effectively

### Alignment & Distribution
- [ ] Master justify-content for main axis alignment
- [ ] Control align-items for cross axis alignment
- [ ] Use align-self for individual item alignment
- [ ] Understand space distribution (between, around, evenly)

### Spacing System
- [ ] Apply margin utilities (m-0 to m-5, directional)
- [ ] Use padding utilities (p-0 to p-5, directional)
- [ ] Implement responsive spacing
- [ ] Use auto margins for centering and positioning

### Utility Classes
- [ ] Master display utilities (d-block, d-none, etc.)
- [ ] Use position utilities effectively
- [ ] Apply sizing utilities (w-100, h-100, etc.)
- [ ] Implement responsive utility variations

## 🎉 Day 11 Complete!

### ✅ Today's Achievements
- **Flexbox Mastery:** Complete control over flex containers and items
- **Spacing System:** Mathematical spacing scale for consistent layouts  
- **Utility Classes:** Display, position, and sizing utilities
- **Interactive Learning:** Hands-on demo with real-time class updates
- **Real-World Applications:** Navigation bars, cards, and common layouts

### 🏆 Skills Gained
- Bootstrap flexbox utility system
- Comprehensive spacing and sizing control
- Layout utility classes and responsive variations
- Interactive design patterns and components
- Professional layout construction techniques

## 🔗 Additional Resources

- **[Bootstrap Flex Docs](https://getbootstrap.com/docs/5.3/utilities/flex/)** - Complete flexbox utilities
- **[Bootstrap Spacing Docs](https://getbootstrap.com/docs/5.3/utilities/spacing/)** - Margin and padding system
- **[Flexbox Froggy](https://flexboxfroggy.com/)** - Interactive flexbox game
- **[CSS Tricks Flexbox Guide](https://css-tricks.com/snippets/css/a-guide-to-flexbox/)** - Comprehensive flexbox reference

---

## 🚀 What's Next?

Tomorrow in **Day 12: Responsive Design Patterns**, you'll learn:
- Advanced responsive design techniques
- Mobile-first development approach
- Breakpoint management strategies
- Responsive utility patterns

**Outstanding work on Day 11!** You now have powerful layout tools at your disposal. Tomorrow we'll focus on creating truly responsive designs that work perfectly on all devices.

---

**Remember: Flexbox and utilities are your best friends for creating flexible, maintainable layouts!** 🔧✨