# Day 09: Bootstrap Grid System Mastery 📐

Welcome to Day 9! Today you'll master Bootstrap's powerful 12-column grid system - the foundation of responsive layout design. This system allows you to create complex, flexible layouts that adapt perfectly to any screen size.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master Bootstrap's 12-column grid system completely
- Understand responsive breakpoints and grid classes
- Create complex multi-column layouts with proper alignment
- Implement grid nesting for advanced layout patterns
- Use offset classes and reordering for precise positioning
- Build responsive layouts that work on all devices
- Apply advanced grid techniques for real-world projects

## 📚 Lesson Content

### Bootstrap Grid System Overview

Bootstrap's grid system is built with **flexbox** and allows up to **12 columns** across the page. It uses containers, rows, and columns to layout and align content.

#### Grid Structure
```html
<div class="container">           <!-- Container -->
    <div class="row">             <!-- Row -->
        <div class="col">         <!-- Column -->
            Content goes here
        </div>
    </div>
</div>
```

#### How the Grid Works
1. **Containers** provide a means to center and horizontally pad content
2. **Rows** are wrappers for columns with negative margins
3. **Columns** have horizontal padding (gutters) for spacing
4. **Content** should be placed within columns

### Responsive Breakpoints

Bootstrap 5 uses these breakpoints for responsive design:

| Breakpoint | Class Infix | Screen Width |
|------------|-------------|--------------|
| Extra small | (none) | <576px |
| Small | `sm` | ≥576px |
| Medium | `md` | ≥768px |
| Large | `lg` | ≥992px |
| Extra large | `xl` | ≥1200px |
| Extra extra large | `xxl` | ≥1400px |

### Basic Grid Classes

#### Equal Width Columns
```html
<div class="container">
    <div class="row">
        <div class="col">
            <div class="bg-light p-3 border">Column 1</div>
        </div>
        <div class="col">
            <div class="bg-light p-3 border">Column 2</div>
        </div>
        <div class="col">
            <div class="bg-light p-3 border">Column 3</div>
        </div>
    </div>
</div>
```

#### Specific Column Widths
```html
<div class="container">
    <div class="row">
        <div class="col-4">
            <div class="bg-primary text-white p-3">4 columns wide</div>
        </div>
        <div class="col-8">
            <div class="bg-secondary text-white p-3">8 columns wide</div>
        </div>
    </div>
    
    <div class="row mt-3">
        <div class="col-3">
            <div class="bg-success text-white p-3">3 columns</div>
        </div>
        <div class="col-6">
            <div class="bg-warning text-dark p-3">6 columns</div>
        </div>
        <div class="col-3">
            <div class="bg-info text-white p-3">3 columns</div>
        </div>
    </div>
</div>
```

### Responsive Grid Classes

#### Mobile-First Responsive Design
```html
<div class="container">
    <div class="row">
        <!-- Stack on mobile, 2 columns on tablet+, 3 columns on desktop+ -->
        <div class="col-12 col-md-6 col-lg-4">
            <div class="bg-primary text-white p-3 mb-3">
                <h5>Column 1</h5>
                <p>Full width on mobile, half on tablet, third on desktop</p>
            </div>
        </div>
        <div class="col-12 col-md-6 col-lg-4">
            <div class="bg-secondary text-white p-3 mb-3">
                <h5>Column 2</h5>
                <p>Full width on mobile, half on tablet, third on desktop</p>
            </div>
        </div>
        <div class="col-12 col-md-12 col-lg-4">
            <div class="bg-success text-white p-3 mb-3">
                <h5>Column 3</h5>
                <p>Full width on mobile/tablet, third on desktop</p>
            </div>
        </div>
    </div>
</div>
```

#### Mix and Match Breakpoints
```html
<div class="container">
    <div class="row">
        <!-- Extra small: 12 cols, Small: 6 cols, Medium: 4 cols, Large: 3 cols -->
        <div class="col-12 col-sm-6 col-md-4 col-lg-3">
            <div class="bg-info text-white p-3 mb-3">Responsive Column</div>
        </div>
        <div class="col-12 col-sm-6 col-md-4 col-lg-3">
            <div class="bg-warning text-dark p-3 mb-3">Responsive Column</div>
        </div>
        <div class="col-12 col-sm-6 col-md-4 col-lg-3">
            <div class="bg-danger text-white p-3 mb-3">Responsive Column</div>
        </div>
        <div class="col-12 col-sm-6 col-md-12 col-lg-3">
            <div class="bg-dark text-white p-3 mb-3">Responsive Column</div>
        </div>
    </div>
</div>
```

### Grid Alignment

#### Vertical Alignment
```html
<div class="container">
    <!-- Align entire row -->
    <div class="row align-items-start bg-light" style="height: 200px;">
        <div class="col">
            <div class="bg-primary text-white p-3">Align Start</div>
        </div>
        <div class="col">
            <div class="bg-secondary text-white p-3">Align Start</div>
        </div>
    </div>
    
    <div class="row align-items-center bg-light mt-3" style="height: 200px;">
        <div class="col">
            <div class="bg-success text-white p-3">Align Center</div>
        </div>
        <div class="col">
            <div class="bg-warning text-dark p-3">Align Center</div>
        </div>
    </div>
    
    <div class="row align-items-end bg-light mt-3" style="height: 200px;">
        <div class="col">
            <div class="bg-info text-white p-3">Align End</div>
        </div>
        <div class="col">
            <div class="bg-danger text-white p-3">Align End</div>
        </div>
    </div>
    
    <!-- Align individual columns -->
    <div class="row bg-light mt-3" style="height: 200px;">
        <div class="col align-self-start">
            <div class="bg-primary text-white p-3">Self Start</div>
        </div>
        <div class="col align-self-center">
            <div class="bg-secondary text-white p-3">Self Center</div>
        </div>
        <div class="col align-self-end">
            <div class="bg-success text-white p-3">Self End</div>
        </div>
    </div>
</div>
```

#### Horizontal Alignment
```html
<div class="container">
    <!-- Left alignment (default) -->
    <div class="row justify-content-start">
        <div class="col-4">
            <div class="bg-primary text-white p-3">Start</div>
        </div>
    </div>
    
    <!-- Center alignment -->
    <div class="row justify-content-center mt-3">
        <div class="col-4">
            <div class="bg-secondary text-white p-3">Center</div>
        </div>
    </div>
    
    <!-- Right alignment -->
    <div class="row justify-content-end mt-3">
        <div class="col-4">
            <div class="bg-success text-white p-3">End</div>
        </div>
    </div>
    
    <!-- Space around -->
    <div class="row justify-content-around mt-3">
        <div class="col-2">
            <div class="bg-warning text-dark p-3">Around</div>
        </div>
        <div class="col-2">
            <div class="bg-info text-white p-3">Around</div>
        </div>
    </div>
    
    <!-- Space between -->
    <div class="row justify-content-between mt-3">
        <div class="col-2">
            <div class="bg-danger text-white p-3">Between</div>
        </div>
        <div class="col-2">
            <div class="bg-dark text-white p-3">Between</div>
        </div>
    </div>
</div>
```

### Offsetting Columns

Use offset classes to move columns to the right:

```html
<div class="container">
    <div class="row">
        <div class="col-4">
            <div class="bg-primary text-white p-3">Column 1</div>
        </div>
        <div class="col-4 offset-4">
            <div class="bg-secondary text-white p-3">Column 2 (offset 4)</div>
        </div>
    </div>
    
    <div class="row mt-3">
        <div class="col-3 offset-3">
            <div class="bg-success text-white p-3">Offset 3</div>
        </div>
        <div class="col-3 offset-3">
            <div class="bg-warning text-dark p-3">Offset 3</div>
        </div>
    </div>
    
    <!-- Responsive offsets -->
    <div class="row mt-3">
        <div class="col-sm-5 offset-sm-2 col-md-6 offset-md-0">
            <div class="bg-info text-white p-3">Responsive Offset</div>
        </div>
    </div>
</div>
```

### Nesting Grids

You can nest grids within columns:

```html
<div class="container">
    <div class="row">
        <div class="col-9">
            <div class="bg-light p-3 border">
                <h5>Level 1: 9 columns</h5>
                <div class="row">
                    <div class="col-8">
                        <div class="bg-primary text-white p-3">
                            <h6>Level 2: 8 of 9</h6>
                            <div class="row">
                                <div class="col-6">
                                    <div class="bg-secondary text-white p-2">Level 3: 6 of 8</div>
                                </div>
                                <div class="col-6">
                                    <div class="bg-success text-white p-2">Level 3: 6 of 8</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-4">
                        <div class="bg-warning text-dark p-3">Level 2: 4 of 9</div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-3">
            <div class="bg-info text-white p-3">Level 1: 3 columns</div>
        </div>
    </div>
</div>
```

### Column Reordering

Change the visual order of columns without changing HTML structure:

```html
<div class="container">
    <div class="row">
        <div class="col order-last">
            <div class="bg-primary text-white p-3">First in HTML, last visually</div>
        </div>
        <div class="col order-first">
            <div class="bg-secondary text-white p-3">Second in HTML, first visually</div>
        </div>
        <div class="col">
            <div class="bg-success text-white p-3">Third in HTML, middle visually</div>
        </div>
    </div>
    
    <!-- Responsive ordering -->
    <div class="row mt-3">
        <div class="col-12 col-md-4 order-md-3">
            <div class="bg-warning text-dark p-3">Order 3 on medium+</div>
        </div>
        <div class="col-12 col-md-4 order-md-1">
            <div class="bg-info text-white p-3">Order 1 on medium+</div>
        </div>
        <div class="col-12 col-md-4 order-md-2">
            <div class="bg-danger text-white p-3">Order 2 on medium+</div>
        </div>
    </div>
</div>
```

## 💻 Hands-On Practice

### Exercise 1: Complete Responsive Layout

Create `responsive-grid-layout.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bootstrap Grid System - Complete Layout</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" 
          rel="stylesheet">
    
    <!-- Bootstrap Icons -->
    <link rel="stylesheet" 
          href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <style>
        .demo-box {
            background: #f8f9fa;
            border: 2px dashed #dee2e6;
            padding: 1rem;
            text-align: center;
            margin-bottom: 1rem;
            min-height: 60px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .demo-content {
            background: #e9ecef;
            padding: 2rem;
            margin-bottom: 1rem;
            border-radius: 8px;
        }
        
        .height-demo {
            min-height: 200px;
        }
        
        .section-title {
            background: linear-gradient(45deg, #007bff, #6610f2);
            color: white;
            padding: 1rem;
            margin-bottom: 2rem;
            border-radius: 8px;
            text-align: center;
        }
        
        .breakpoint-indicator {
            position: fixed;
            top: 10px;
            right: 10px;
            background: rgba(0,0,0,0.8);
            color: white;
            padding: 5px 10px;
            border-radius: 4px;
            font-size: 12px;
            z-index: 1000;
        }
        
        /* Visual indicators for different screen sizes */
        .breakpoint-indicator::after {
            content: "XS (<576px)";
        }
        
        @media (min-width: 576px) {
            .breakpoint-indicator::after {
                content: "SM (≥576px)";
            }
        }
        
        @media (min-width: 768px) {
            .breakpoint-indicator::after {
                content: "MD (≥768px)";
            }
        }
        
        @media (min-width: 992px) {
            .breakpoint-indicator::after {
                content: "LG (≥992px)";
            }
        }
        
        @media (min-width: 1200px) {
            .breakpoint-indicator::after {
                content: "XL (≥1200px)";
            }
        }
        
        @media (min-width: 1400px) {
            .breakpoint-indicator::after {
                content: "XXL (≥1400px)";
            }
        }
    </style>
</head>
<body>
    <!-- Breakpoint Indicator -->
    <div class="breakpoint-indicator"></div>
    
    <!-- Header -->
    <header class="bg-dark text-white py-4 mb-5">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-md-6">
                    <h1 class="mb-0">
                        <i class="bi bi-grid-3x3-gap-fill me-2"></i>
                        Bootstrap Grid Mastery
                    </h1>
                </div>
                <div class="col-md-6 text-md-end">
                    <p class="mb-0">Responsive Design in Action</p>
                </div>
            </div>
        </div>
    </header>
    
    <div class="container">
        <!-- Basic Grid Demo -->
        <section class="mb-5">
            <div class="section-title">
                <h2>1. Basic Grid System</h2>
                <p class="mb-0">Understanding the 12-column foundation</p>
            </div>
            
            <div class="row mb-4">
                <div class="col-12">
                    <div class="demo-box bg-primary text-white">
                        <span>col-12 (Full width)</span>
                    </div>
                </div>
            </div>
            
            <div class="row mb-4">
                <div class="col-6">
                    <div class="demo-box bg-success text-white">col-6</div>
                </div>
                <div class="col-6">
                    <div class="demo-box bg-info text-white">col-6</div>
                </div>
            </div>
            
            <div class="row mb-4">
                <div class="col-4">
                    <div class="demo-box bg-warning text-dark">col-4</div>
                </div>
                <div class="col-4">
                    <div class="demo-box bg-danger text-white">col-4</div>
                </div>
                <div class="col-4">
                    <div class="demo-box bg-secondary text-white">col-4</div>
                </div>
            </div>
            
            <div class="row mb-4">
                <div class="col-3">
                    <div class="demo-box bg-primary text-white">col-3</div>
                </div>
                <div class="col-3">
                    <div class="demo-box bg-success text-white">col-3</div>
                </div>
                <div class="col-3">
                    <div class="demo-box bg-warning text-dark">col-3</div>
                </div>
                <div class="col-3">
                    <div class="demo-box bg-info text-white">col-3</div>
                </div>
            </div>
            
            <div class="row">
                <div class="col-2">
                    <div class="demo-box bg-dark text-white">2</div>
                </div>
                <div class="col-2">
                    <div class="demo-box bg-primary text-white">2</div>
                </div>
                <div class="col-2">
                    <div class="demo-box bg-success text-white">2</div>
                </div>
                <div class="col-2">
                    <div class="demo-box bg-warning text-dark">2</div>
                </div>
                <div class="col-2">
                    <div class="demo-box bg-info text-white">2</div>
                </div>
                <div class="col-2">
                    <div class="demo-box bg-danger text-white">2</div>
                </div>
            </div>
        </section>
        
        <!-- Responsive Grid -->
        <section class="mb-5">
            <div class="section-title">
                <h2>2. Responsive Grid System</h2>
                <p class="mb-0">Resize your browser to see the magic happen!</p>
            </div>
            
            <div class="alert alert-info">
                <h5><i class="bi bi-info-circle me-2"></i>Try This:</h5>
                <p class="mb-0">Resize your browser window and watch how these columns adapt at different breakpoints.</p>
            </div>
            
            <div class="row">
                <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
                    <div class="demo-content bg-primary text-white">
                        <h6>Responsive Column 1</h6>
                        <small>
                            XS: 12 cols<br>
                            SM: 6 cols<br>
                            MD: 4 cols<br>
                            LG: 3 cols<br>
                            XL: 2 cols
                        </small>
                    </div>
                </div>
                <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
                    <div class="demo-content bg-success text-white">
                        <h6>Responsive Column 2</h6>
                        <small>Adapts to screen size</small>
                    </div>
                </div>
                <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
                    <div class="demo-content bg-warning text-dark">
                        <h6>Responsive Column 3</h6>
                        <small>Mobile-first design</small>
                    </div>
                </div>
                <div class="col-12 col-sm-6 col-md-12 col-lg-3 col-xl-2">
                    <div class="demo-content bg-info text-white">
                        <h6>Responsive Column 4</h6>
                        <small>Different at MD</small>
                    </div>
                </div>
                <div class="col-12 col-sm-12 col-md-6 col-lg-6 col-xl-2">
                    <div class="demo-content bg-danger text-white">
                        <h6>Responsive Column 5</h6>
                        <small>Varies by breakpoint</small>
                    </div>
                </div>
                <div class="col-12 col-sm-12 col-md-6 col-lg-6 col-xl-2">
                    <div class="demo-content bg-dark text-white">
                        <h6>Responsive Column 6</h6>
                        <small>Perfect adaptation</small>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Grid Alignment -->
        <section class="mb-5">
            <div class="section-title">
                <h2>3. Grid Alignment</h2>
                <p class="mb-0">Vertical and horizontal alignment options</p>
            </div>
            
            <h4>Vertical Alignment</h4>
            <div class="row align-items-start height-demo mb-4" style="background: #f8f9fa;">
                <div class="col-4">
                    <div class="demo-box bg-primary text-white">align-items-start</div>
                </div>
                <div class="col-4">
                    <div class="demo-box bg-success text-white">Top aligned</div>
                </div>
                <div class="col-4">
                    <div class="demo-box bg-warning text-dark">All columns</div>
                </div>
            </div>
            
            <div class="row align-items-center height-demo mb-4" style="background: #f8f9fa;">
                <div class="col-4">
                    <div class="demo-box bg-info text-white">align-items-center</div>
                </div>
                <div class="col-4">
                    <div class="demo-box bg-danger text-white">Center aligned</div>
                </div>
                <div class="col-4">
                    <div class="demo-box bg-secondary text-white">Perfectly centered</div>
                </div>
            </div>
            
            <div class="row align-items-end height-demo mb-4" style="background: #f8f9fa;">
                <div class="col-4">
                    <div class="demo-box bg-dark text-white">align-items-end</div>
                </div>
                <div class="col-4">
                    <div class="demo-box bg-primary text-white">Bottom aligned</div>
                </div>
                <div class="col-4">
                    <div class="demo-box bg-success text-white">All at bottom</div>
                </div>
            </div>
            
            <h4>Individual Column Alignment</h4>
            <div class="row height-demo mb-4" style="background: #f8f9fa;">
                <div class="col-4 align-self-start">
                    <div class="demo-box bg-primary text-white">align-self-start</div>
                </div>
                <div class="col-4 align-self-center">
                    <div class="demo-box bg-success text-white">align-self-center</div>
                </div>
                <div class="col-4 align-self-end">
                    <div class="demo-box bg-warning text-dark">align-self-end</div>
                </div>
            </div>
            
            <h4>Horizontal Alignment</h4>
            <div class="row justify-content-start mb-3">
                <div class="col-4">
                    <div class="demo-box bg-primary text-white">justify-content-start</div>
                </div>
            </div>
            
            <div class="row justify-content-center mb-3">
                <div class="col-4">
                    <div class="demo-box bg-success text-white">justify-content-center</div>
                </div>
            </div>
            
            <div class="row justify-content-end mb-3">
                <div class="col-4">
                    <div class="demo-box bg-warning text-dark">justify-content-end</div>
                </div>
            </div>
            
            <div class="row justify-content-around mb-3">
                <div class="col-2">
                    <div class="demo-box bg-info text-white">Around 1</div>
                </div>
                <div class="col-2">
                    <div class="demo-box bg-danger text-white">Around 2</div>
                </div>
            </div>
            
            <div class="row justify-content-between">
                <div class="col-2">
                    <div class="demo-box bg-dark text-white">Between 1</div>
                </div>
                <div class="col-2">
                    <div class="demo-box bg-secondary text-white">Between 2</div>
                </div>
            </div>
        </section>
        
        <!-- Offsetting Columns -->
        <section class="mb-5">
            <div class="section-title">
                <h2>4. Offsetting Columns</h2>
                <p class="mb-0">Create space and precise positioning</p>
            </div>
            
            <div class="row mb-3">
                <div class="col-4">
                    <div class="demo-box bg-primary text-white">col-4</div>
                </div>
                <div class="col-4 offset-4">
                    <div class="demo-box bg-success text-white">col-4 offset-4</div>
                </div>
            </div>
            
            <div class="row mb-3">
                <div class="col-3 offset-3">
                    <div class="demo-box bg-warning text-dark">col-3 offset-3</div>
                </div>
                <div class="col-3 offset-3">
                    <div class="demo-box bg-info text-white">col-3 offset-3</div>
                </div>
            </div>
            
            <div class="row mb-3">
                <div class="col-6 offset-3">
                    <div class="demo-box bg-danger text-white">col-6 offset-3 (centered)</div>
                </div>
            </div>
            
            <h5>Responsive Offsets</h5>
            <div class="row">
                <div class="col-sm-5 offset-sm-2 col-md-6 offset-md-0">
                    <div class="demo-box bg-dark text-white">
                        Responsive Offset<br>
                        <small>SM: offset-2, MD+: no offset</small>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Nested Grids -->
        <section class="mb-5">
            <div class="section-title">
                <h2>5. Nested Grids</h2>
                <p class="mb-0">Grids within grids for complex layouts</p>
            </div>
            
            <div class="row">
                <div class="col-9">
                    <div class="demo-content bg-light border">
                        <h5>Level 1: 9 columns wide</h5>
                        <div class="row">
                            <div class="col-8">
                                <div class="demo-content bg-primary text-white">
                                    <h6>Level 2: 8 of 9 columns</h6>
                                    <div class="row">
                                        <div class="col-6">
                                            <div class="demo-box bg-success text-white">
                                                Level 3<br>6 of 8
                                            </div>
                                        </div>
                                        <div class="col-6">
                                            <div class="demo-box bg-warning text-dark">
                                                Level 3<br>6 of 8
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-4">
                                <div class="demo-content bg-info text-white">
                                    <h6>Level 2: 4 of 9 columns</h6>
                                    <p class="mb-0">Sidebar content area</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-3">
                    <div class="demo-content bg-secondary text-white">
                        <h5>Level 1: 3 columns</h5>
                        <p class="mb-0">Main sidebar area</p>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Column Reordering -->
        <section class="mb-5">
            <div class="section-title">
                <h2>6. Column Reordering</h2>
                <p class="mb-0">Change visual order without changing HTML</p>
            </div>
            
            <div class="alert alert-warning">
                <h6><i class="bi bi-exclamation-triangle me-2"></i>Note:</h6>
                <p class="mb-0">The HTML order is: First → Second → Third, but visual order changes with classes.</p>
            </div>
            
            <h5>Basic Ordering</h5>
            <div class="row mb-4">
                <div class="col order-last">
                    <div class="demo-box bg-primary text-white">
                        <strong>First in HTML</strong><br>
                        <span class="badge bg-light text-dark">order-last</span><br>
                        Shows last visually
                    </div>
                </div>
                <div class="col order-first">
                    <div class="demo-box bg-success text-white">
                        <strong>Second in HTML</strong><br>
                        <span class="badge bg-light text-dark">order-first</span><br>
                        Shows first visually
                    </div>
                </div>
                <div class="col">
                    <div class="demo-box bg-warning text-dark">
                        <strong>Third in HTML</strong><br>
                        <span class="badge bg-dark text-light">no order</span><br>
                        Shows in middle
                    </div>
                </div>
            </div>
            
            <h5>Responsive Ordering</h5>
            <div class="row">
                <div class="col-12 col-md-4 order-md-3">
                    <div class="demo-box bg-info text-white">
                        <strong>First in HTML</strong><br>
                        <span class="badge bg-light text-dark">order-md-3</span><br>
                        Third on medium+ screens
                    </div>
                </div>
                <div class="col-12 col-md-4 order-md-1">
                    <div class="demo-box bg-danger text-white">
                        <strong>Second in HTML</strong><br>
                        <span class="badge bg-light text-dark">order-md-1</span><br>
                        First on medium+ screens
                    </div>
                </div>
                <div class="col-12 col-md-4 order-md-2">
                    <div class="demo-box bg-dark text-white">
                        <strong>Third in HTML</strong><br>
                        <span class="badge bg-light text-dark">order-md-2</span><br>
                        Second on medium+ screens
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Real World Example -->
        <section class="mb-5">
            <div class="section-title">
                <h2>7. Real-World Layout Example</h2>
                <p class="mb-0">Putting it all together: A complete responsive page layout</p>
            </div>
            
            <div class="border rounded p-3" style="background: #f8f9fa;">
                <!-- Header -->
                <div class="row mb-3">
                    <div class="col-12">
                        <div class="bg-dark text-white p-3 rounded">
                            <div class="row align-items-center">
                                <div class="col-md-6">
                                    <h4 class="mb-0">Website Header</h4>
                                </div>
                                <div class="col-md-6 text-md-end">
                                    Navigation Menu
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Main Content Area -->
                <div class="row">
                    <!-- Main Content (reorders on mobile) -->
                    <div class="col-12 col-lg-8 order-2 order-lg-1 mb-3">
                        <div class="bg-white p-4 rounded border h-100">
                            <h5>Main Content Area</h5>
                            <p>This is the primary content area. On large screens, it appears on the left. On mobile devices, it appears below the sidebar due to the order classes.</p>
                            
                            <!-- Nested content grid -->
                            <div class="row">
                                <div class="col-sm-6 col-lg-4">
                                    <div class="bg-primary text-white p-3 rounded mb-3">
                                        <h6>Feature 1</h6>
                                        <p class="mb-0">Content block</p>
                                    </div>
                                </div>
                                <div class="col-sm-6 col-lg-4">
                                    <div class="bg-success text-white p-3 rounded mb-3">
                                        <h6>Feature 2</h6>
                                        <p class="mb-0">Content block</p>
                                    </div>
                                </div>
                                <div class="col-sm-12 col-lg-4">
                                    <div class="bg-warning text-dark p-3 rounded mb-3">
                                        <h6>Feature 3</h6>
                                        <p class="mb-0">Content block</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Sidebar (reorders on mobile) -->
                    <div class="col-12 col-lg-4 order-1 order-lg-2 mb-3">
                        <div class="bg-light p-4 rounded border h-100">
                            <h5>Sidebar</h5>
                            <p>This sidebar appears first on mobile but second on desktop screens.</p>
                            
                            <div class="mb-3">
                                <div class="bg-info text-white p-3 rounded">
                                    <h6>Widget 1</h6>
                                    <p class="mb-0">Sidebar content</p>
                                </div>
                            </div>
                            
                            <div class="mb-3">
                                <div class="bg-secondary text-white p-3 rounded">
                                    <h6>Widget 2</h6>
                                    <p class="mb-0">More sidebar content</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Footer -->
                <div class="row mt-3">
                    <div class="col-12">
                        <div class="bg-dark text-white p-3 rounded">
                            <div class="row">
                                <div class="col-md-6">
                                    <h6>Footer Section 1</h6>
                                    <p class="mb-0">Copyright information</p>
                                </div>
                                <div class="col-md-6 text-md-end">
                                    <h6>Footer Section 2</h6>
                                    <p class="mb-0">Contact information</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Grid Debugging Tips -->
        <section class="mb-5">
            <div class="section-title">
                <h2>8. Grid Debugging Tips</h2>
                <p class="mb-0">Techniques for troubleshooting grid layouts</p>
            </div>
            
            <div class="alert alert-info">
                <h5><i class="bi bi-lightbulb me-2"></i>Pro Tips:</h5>
                <ul class="mb-0">
                    <li><strong>Browser DevTools:</strong> Inspect elements to see actual column widths</li>
                    <li><strong>Background Colors:</strong> Add temporary background colors to visualize columns</li>
                    <li><strong>Border Debug:</strong> Add borders to see column boundaries</li>
                    <li><strong>Check Total Width:</strong> Ensure column numbers don't exceed 12</li>
                    <li><strong>Responsive Testing:</strong> Test at all breakpoints</li>
                </ul>
            </div>
            
            <div class="row">
                <div class="col-12 col-md-6">
                    <div class="bg-white border p-3 rounded">
                        <h6>Debugging Example</h6>
                        <p>Add background colors and borders to visualize your grid structure during development.</p>
                        <pre class="bg-light p-2 rounded"><code>.debug-grid .col-* {
  background: rgba(255,0,0,0.1);
  border: 1px solid red;
}</code></pre>
                    </div>
                </div>
                <div class="col-12 col-md-6">
                    <div class="bg-white border p-3 rounded">
                        <h6>Common Issues</h6>
                        <ul class="mb-0">
                            <li>Forgetting <code>.row</code> wrapper</li>
                            <li>Columns exceeding 12 total</li>
                            <li>Missing responsive classes</li>
                            <li>Incorrect nesting structure</li>
                        </ul>
                    </div>
                </div>
            </div>
        </section>
    </div>
    
    <!-- Footer -->
    <footer class="bg-dark text-white py-4 mt-5">
        <div class="container">
            <div class="row">
                <div class="col-md-6">
                    <h5>Grid System Mastery Complete!</h5>
                    <p class="mb-0">You've learned Bootstrap's powerful grid system.</p>
                </div>
                <div class="col-md-6 text-md-end">
                    <p class="mb-0">Ready to build amazing responsive layouts! 🚀</p>
                </div>
            </div>
        </div>
    </footer>
    
    <!-- Bootstrap JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <!-- Custom JavaScript for Interactive Features -->
    <script>
        // Highlight columns on hover for better understanding
        document.querySelectorAll('.demo-box').forEach(box => {
            box.addEventListener('mouseenter', function() {
                this.style.transform = 'scale(1.05)';
                this.style.transition = 'transform 0.2s ease';
                this.style.boxShadow = '0 4px 8px rgba(0,0,0,0.2)';
            });
            
            box.addEventListener('mouseleave', function() {
                this.style.transform = 'scale(1)';
                this.style.boxShadow = 'none';
            });
        });
        
        // Add click to copy for code examples
        document.querySelectorAll('code').forEach(code => {
            code.style.cursor = 'pointer';
            code.title = 'Click to copy';
            
            code.addEventListener('click', function() {
                navigator.clipboard.writeText(this.textContent).then(() => {
                    // Show temporary feedback
                    const original = this.textContent;
                    this.textContent = 'Copied!';
                    setTimeout(() => {
                        this.textContent = original;
                    }, 1000);
                });
            });
        });
        
        console.log('Bootstrap Grid System Demo Loaded! 📐');
        console.log('Resize your browser window to see responsive behavior.');
    </script>
</body>
</html>
```

### Exercise 2: Portfolio Grid Layout

Create a portfolio layout using advanced grid techniques:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Portfolio Grid Layout</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        .portfolio-item {
            background: #f8f9fa;
            height: 200px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 8px;
            transition: transform 0.3s ease;
        }
        
        .portfolio-item:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.1);
        }
    </style>
</head>
<body>
    <div class="container py-5">
        <h1 class="text-center mb-5">Portfolio Grid Layout</h1>
        
        <!-- Portfolio Grid -->
        <div class="row g-4">
            <!-- Featured Project (Large) -->
            <div class="col-12 col-md-8">
                <div class="portfolio-item bg-primary text-white h-100">
                    <div class="text-center">
                        <h3>Featured Project</h3>
                        <p>Large featured project space</p>
                    </div>
                </div>
            </div>
            
            <!-- Two smaller projects -->
            <div class="col-12 col-md-4">
                <div class="row g-4 h-100">
                    <div class="col-12">
                        <div class="portfolio-item bg-success text-white">
                            <div class="text-center">
                                <h5>Project 2</h5>
                                <p>Secondary project</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="portfolio-item bg-warning text-dark">
                            <div class="text-center">
                                <h5>Project 3</h5>
                                <p>Secondary project</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Three equal projects -->
            <div class="col-12 col-sm-6 col-lg-4">
                <div class="portfolio-item bg-info text-white">
                    <div class="text-center">
                        <h5>Project 4</h5>
                        <p>Equal size project</p>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-lg-4">
                <div class="portfolio-item bg-danger text-white">
                    <div class="text-center">
                        <h5>Project 5</h5>
                        <p>Equal size project</p>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-4">
                <div class="portfolio-item bg-dark text-white">
                    <div class="text-center">
                        <h5>Project 6</h5>
                        <p>Equal size project</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
```

## 📋 Grid System Mastery Checklist

### Basic Grid Understanding
- [ ] Understand the 12-column system
- [ ] Know container, row, column structure
- [ ] Can create equal-width columns
- [ ] Can create specific-width columns

### Responsive Design
- [ ] Understand all 6 breakpoints (xs, sm, md, lg, xl, xxl)
- [ ] Can create mobile-first responsive layouts
- [ ] Can mix breakpoints for complex responsive behavior
- [ ] Test layouts at all screen sizes

### Advanced Techniques
- [ ] Master vertical and horizontal alignment
- [ ] Use offset classes for positioning
- [ ] Create nested grids effectively
- [ ] Implement column reordering
- [ ] Debug grid layout issues

### Real-World Application
- [ ] Built complete responsive page layout
- [ ] Created portfolio-style grid layouts
- [ ] Understand when to use different grid patterns
- [ ] Can troubleshoot common grid problems

## 🎉 Day 9 Complete!

### ✅ Today's Achievements
- **Grid System Mastery:** Complete understanding of Bootstrap's 12-column grid
- **Responsive Design:** Mobile-first responsive layout creation
- **Advanced Techniques:** Alignment, offsetting, nesting, and reordering
- **Real-World Practice:** Built complex, professional grid layouts
- **Debugging Skills:** Learned to troubleshoot grid issues

### 🏆 Skills Gained
- Bootstrap grid system architecture
- Responsive breakpoint system
- Flexbox-based alignment techniques
- Advanced grid patterns and techniques
- Real-world layout implementation
- Grid debugging and troubleshooting

## 🔗 Additional Resources

- **[Bootstrap Grid System Docs](https://getbootstrap.com/docs/5.3/layout/grid/)** - Official grid documentation
- **[CSS Grid vs Flexbox vs Bootstrap](https://css-tricks.com/css-grid-replace-flexbox/)** - Layout comparison
- **[Bootstrap Grid Examples](https://getbootstrap.com/docs/5.3/examples/grid/)** - Official grid examples
- **[Grid Debugging Tools](https://firefox-source-docs.mozilla.org/devtools-user/page_inspector/how_to/examine_grid_layouts/index.html)** - Browser dev tools

---

## 🚀 What's Next?

Tomorrow in **Day 10: Bootstrap Typography & Colors**, you'll learn:
- Typography system and text utilities
- Color palette and theme system
- Custom typography and color schemes
- Text formatting and display options

**Fantastic work on Day 9!** You've mastered Bootstrap's grid system - the foundation of all responsive layouts. Tomorrow we'll make your content look amazing with typography and colors.

---

**Remember: The grid system is your layout superpower - master it and you can build anything!** 📐✨