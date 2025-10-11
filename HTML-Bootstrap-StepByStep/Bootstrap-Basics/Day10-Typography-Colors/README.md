# Day 10: Bootstrap Typography & Colors 🎨

Welcome to Day 10! Today you'll master Bootstrap's typography system and color palette to create beautiful, consistent, and accessible text styling. You'll learn to use Bootstrap's type scale, color utilities, and theming system effectively.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master Bootstrap's typography system and text utilities
- Understand and apply Bootstrap's color palette and theme system
- Create consistent text hierarchy and visual design
- Use display headings, lead text, and text formatting utilities
- Apply color utilities for text, backgrounds, and borders
- Customize typography and colors for brand consistency
- Implement accessible color combinations and contrast ratios
- Build visually appealing content with proper typography

## 📚 Lesson Content

### Bootstrap Typography System

Bootstrap provides a comprehensive typography system based on a modular scale that ensures consistent and readable text across all screen sizes.

#### Typography Fundamentals
```html
<!-- Base font settings (applied automatically) -->
<body>
    <!-- Bootstrap sets: -->
    <!-- font-family: system font stack -->
    <!-- font-size: 1rem (16px default) -->
    <!-- line-height: 1.5 -->
    <!-- color: #212529 (dark gray) -->
</body>
```

#### Headings and Display Headings
```html
<div class="container">
    <!-- Standard Headings (h1-h6) -->
    <h1>Heading 1 (2.5rem)</h1>
    <h2>Heading 2 (2rem)</h2>
    <h3>Heading 3 (1.75rem)</h3>
    <h4>Heading 4 (1.5rem)</h4>
    <h5>Heading 5 (1.25rem)</h5>
    <h6>Heading 6 (1rem)</h6>
    
    <!-- Heading classes on any element -->
    <p class="h1">Paragraph styled as h1</p>
    <div class="h2">Div styled as h2</div>
    <span class="h3">Span styled as h3</span>
    
    <!-- Display Headings (larger, lighter) -->
    <h1 class="display-1">Display 1</h1>
    <h1 class="display-2">Display 2</h1>
    <h1 class="display-3">Display 3</h1>
    <h1 class="display-4">Display 4</h1>
    <h1 class="display-5">Display 5</h1>
    <h1 class="display-6">Display 6</h1>
    
    <!-- Customizable display headings -->
    <h1 class="display-4 fw-normal">Custom Display Heading</h1>
    <h2 class="display-6 text-primary fw-bold">Colored Display Heading</h2>
</div>
```

#### Text Elements and Utilities
```html
<div class="container">
    <!-- Lead text (larger, emphasized) -->
    <p class="lead">
        This is a lead paragraph. It stands out from regular paragraphs 
        and is perfect for introductions or important statements.
    </p>
    
    <!-- Regular paragraphs -->
    <p>
        This is a regular paragraph with standard text size and weight. 
        Bootstrap provides consistent spacing and typography for readability.
    </p>
    
    <!-- Text sizes -->
    <p class="fs-1">Font size 1 (largest)</p>
    <p class="fs-2">Font size 2</p>
    <p class="fs-3">Font size 3</p>
    <p class="fs-4">Font size 4</p>
    <p class="fs-5">Font size 5</p>
    <p class="fs-6">Font size 6 (smallest)</p>
    
    <!-- Text weights -->
    <p class="fw-bold">Bold text</p>
    <p class="fw-bolder">Bolder text (relative to parent)</p>
    <p class="fw-semibold">Semibold text</p>
    <p class="fw-medium">Medium weight text</p>
    <p class="fw-normal">Normal weight text</p>
    <p class="fw-light">Light weight text</p>
    <p class="fw-lighter">Lighter text (relative to parent)</p>
    
    <!-- Text styles -->
    <p class="fst-italic">Italic text</p>
    <p class="fst-normal">Normal font style</p>
    
    <!-- Text decoration -->
    <p class="text-decoration-underline">Underlined text</p>
    <p class="text-decoration-line-through">Line through text</p>
    <p class="text-decoration-none">Remove text decoration</p>
</div>
```

#### Text Transformation and Alignment
```html
<div class="container">
    <!-- Text transform -->
    <p class="text-lowercase">LOWERCASED TEXT</p>
    <p class="text-uppercase">uppercased text</p>
    <p class="text-capitalize">capitalized text</p>
    
    <!-- Text alignment -->
    <p class="text-start">Start aligned text (left in LTR)</p>
    <p class="text-center">Center aligned text</p>
    <p class="text-end">End aligned text (right in LTR)</p>
    
    <!-- Responsive text alignment -->
    <p class="text-sm-start text-md-center text-lg-end">
        Responsive alignment: start on small, center on medium, end on large
    </p>
    
    <!-- Text wrapping -->
    <div class="badge bg-primary text-wrap" style="width: 6rem;">
        This text should wrap.
    </div>
    
    <div class="text-nowrap bg-light" style="width: 8rem;">
        This text should not wrap and will overflow.
    </div>
    
    <!-- Text truncation -->
    <div class="text-truncate" style="width: 150px;">
        This text is quite long and will be truncated with an ellipsis.
    </div>
</div>
```

### Bootstrap Color System

Bootstrap 5 provides a comprehensive color system with semantic color names and utility classes.

#### Brand Colors (Theme Colors)
```html
<div class="container">
    <!-- Primary brand colors -->
    <div class="row g-3 mb-4">
        <div class="col-md-3">
            <div class="bg-primary text-white p-3 rounded">
                <h6 class="mb-1">Primary</h6>
                <small>#0d6efd</small>
            </div>
        </div>
        <div class="col-md-3">
            <div class="bg-secondary text-white p-3 rounded">
                <h6 class="mb-1">Secondary</h6>
                <small>#6c757d</small>
            </div>
        </div>
        <div class="col-md-3">
            <div class="bg-success text-white p-3 rounded">
                <h6 class="mb-1">Success</h6>
                <small>#198754</small>
            </div>
        </div>
        <div class="col-md-3">
            <div class="bg-danger text-white p-3 rounded">
                <h6 class="mb-1">Danger</h6>
                <small>#dc3545</small>
            </div>
        </div>
    </div>
    
    <div class="row g-3 mb-4">
        <div class="col-md-3">
            <div class="bg-warning text-dark p-3 rounded">
                <h6 class="mb-1">Warning</h6>
                <small>#ffc107</small>
            </div>
        </div>
        <div class="col-md-3">
            <div class="bg-info text-dark p-3 rounded">
                <h6 class="mb-1">Info</h6>
                <small>#0dcaf0</small>
            </div>
        </div>
        <div class="col-md-3">
            <div class="bg-light text-dark p-3 rounded border">
                <h6 class="mb-1">Light</h6>
                <small>#f8f9fa</small>
            </div>
        </div>
        <div class="col-md-3">
            <div class="bg-dark text-white p-3 rounded">
                <h6 class="mb-1">Dark</h6>
                <small>#212529</small>
            </div>
        </div>
    </div>
</div>
```

#### Text Colors
```html
<div class="container">
    <!-- Semantic text colors -->
    <p class="text-primary">Primary text color</p>
    <p class="text-secondary">Secondary text color</p>
    <p class="text-success">Success text color</p>
    <p class="text-danger">Danger text color</p>
    <p class="text-warning">Warning text color</p>
    <p class="text-info">Info text color</p>
    <p class="text-light bg-dark p-2">Light text color (on dark background)</p>
    <p class="text-dark">Dark text color</p>
    <p class="text-muted">Muted text color</p>
    <p class="text-white bg-dark p-2">White text color</p>
    
    <!-- Text emphasis colors -->
    <p class="text-primary-emphasis">Primary emphasis text</p>
    <p class="text-secondary-emphasis">Secondary emphasis text</p>
    <p class="text-success-emphasis">Success emphasis text</p>
    <p class="text-danger-emphasis">Danger emphasis text</p>
    <p class="text-warning-emphasis">Warning emphasis text</p>
    <p class="text-info-emphasis">Info emphasis text</p>
    <p class="text-light-emphasis bg-dark p-2">Light emphasis text</p>
    <p class="text-dark-emphasis">Dark emphasis text</p>
    
    <!-- Text opacity -->
    <div class="text-primary">Primary text
        <div class="text-primary text-opacity-75">75% opacity</div>
        <div class="text-primary text-opacity-50">50% opacity</div>
        <div class="text-primary text-opacity-25">25% opacity</div>
    </div>
</div>
```

#### Background Colors
```html
<div class="container">
    <!-- Solid background colors -->
    <div class="row g-3 mb-4">
        <div class="col-md-4">
            <div class="bg-primary text-white p-3 rounded">
                <h6>Primary Background</h6>
                <p class="mb-0">White text on primary background</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="bg-success text-white p-3 rounded">
                <h6>Success Background</h6>
                <p class="mb-0">White text on success background</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="bg-warning text-dark p-3 rounded">
                <h6>Warning Background</h6>
                <p class="mb-0">Dark text on warning background</p>
            </div>
        </div>
    </div>
    
    <!-- Subtle background colors -->
    <div class="row g-3 mb-4">
        <div class="col-md-4">
            <div class="bg-primary-subtle text-primary-emphasis p-3 rounded border">
                <h6>Primary Subtle</h6>
                <p class="mb-0">Subtle background with emphasis text</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="bg-success-subtle text-success-emphasis p-3 rounded border">
                <h6>Success Subtle</h6>
                <p class="mb-0">Subtle background with emphasis text</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="bg-warning-subtle text-warning-emphasis p-3 rounded border">
                <h6>Warning Subtle</h6>
                <p class="mb-0">Subtle background with emphasis text</p>
            </div>
        </div>
    </div>
    
    <!-- Background opacity -->
    <div class="bg-success p-3 text-white rounded">
        <div class="bg-light bg-opacity-10 p-2 rounded mb-2">10% white overlay</div>
        <div class="bg-light bg-opacity-25 p-2 rounded mb-2">25% white overlay</div>
        <div class="bg-light bg-opacity-50 p-2 rounded mb-2">50% white overlay</div>
        <div class="bg-light bg-opacity-75 p-2 rounded">75% white overlay</div>
    </div>
</div>
```

#### Link Colors
```html
<div class="container">
    <!-- Link color utilities -->
    <p><a href="#" class="link-primary">Primary link</a></p>
    <p><a href="#" class="link-secondary">Secondary link</a></p>
    <p><a href="#" class="link-success">Success link</a></p>
    <p><a href="#" class="link-danger">Danger link</a></p>
    <p><a href="#" class="link-warning">Warning link</a></p>
    <p><a href="#" class="link-info">Info link</a></p>
    <p><a href="#" class="link-light">Light link</a> (better on dark backgrounds)</p>
    <p><a href="#" class="link-dark">Dark link</a></p>
    
    <!-- Link with custom hover effects -->
    <p><a href="#" class="link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover">Primary link with hover effect</a></p>
    
    <!-- Links on colored backgrounds -->
    <div class="bg-dark p-3 rounded">
        <p><a href="#" class="link-light">Light link on dark background</a></p>
        <p><a href="#" class="link-warning">Warning link on dark background</a></p>
    </div>
</div>
```

## 💻 Hands-On Practice

### Exercise 1: Typography Showcase Website

Create `typography-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bootstrap Typography & Colors Showcase</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" 
          rel="stylesheet">
    
    <!-- Bootstrap Icons -->
    <link rel="stylesheet" 
          href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <style>
        .section-divider {
            height: 2px;
            background: linear-gradient(90deg, transparent, #0d6efd, transparent);
            margin: 3rem 0;
        }
        
        .color-swatch {
            height: 100px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 8px;
            margin-bottom: 1rem;
            position: relative;
            overflow: hidden;
        }
        
        .color-info {
            text-align: center;
            font-weight: 500;
        }
        
        .typography-example {
            background: #f8f9fa;
            padding: 2rem;
            border-radius: 8px;
            margin-bottom: 2rem;
            border-left: 4px solid #0d6efd;
        }
        
        .gradient-text {
            background: linear-gradient(45deg, #0d6efd, #6610f2);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }
        
        .text-shadow {
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }
        
        .demo-card {
            transition: all 0.3s ease;
            border: none;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        .demo-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.15);
        }
        
        .accessibility-good {
            border-left: 4px solid #198754;
        }
        
        .accessibility-bad {
            border-left: 4px solid #dc3545;
        }
    </style>
</head>
<body>
    <!-- Hero Section with Typography -->
    <header class="bg-primary text-white py-5 mb-5">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-lg-8">
                    <h1 class="display-3 fw-bold mb-3 text-shadow">
                        Typography & Colors
                    </h1>
                    <p class="lead fs-4 mb-4">
                        Master Bootstrap's typography system and color palette to create 
                        beautiful, accessible, and consistent designs.
                    </p>
                    <div class="d-flex flex-wrap gap-3">
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Typography</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Colors</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Accessibility</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Design System</span>
                    </div>
                </div>
                <div class="col-lg-4 text-center">
                    <i class="bi bi-palette display-1 text-light opacity-75"></i>
                </div>
            </div>
        </div>
    </header>
    
    <div class="container">
        <!-- Typography Hierarchy -->
        <section class="mb-5">
            <div class="row mb-4">
                <div class="col-12">
                    <h2 class="display-5 gradient-text mb-3">Typography Hierarchy</h2>
                    <p class="lead text-muted">
                        Bootstrap's typography system provides consistent scaling and spacing 
                        for all text elements across your design.
                    </p>
                </div>
            </div>
            
            <div class="typography-example">
                <h1 class="mb-3">Heading 1 - Main Page Title <small class="text-muted">(2.5rem)</small></h1>
                <h2 class="mb-3">Heading 2 - Section Title <small class="text-muted">(2rem)</small></h2>
                <h3 class="mb-3">Heading 3 - Subsection Title <small class="text-muted">(1.75rem)</small></h3>
                <h4 class="mb-3">Heading 4 - Content Title <small class="text-muted">(1.5rem)</small></h4>
                <h5 class="mb-3">Heading 5 - Content Subtitle <small class="text-muted">(1.25rem)</small></h5>
                <h6 class="mb-4">Heading 6 - Small Title <small class="text-muted">(1rem)</small></h6>
                
                <p class="lead mb-3">
                    This is lead text - perfect for introductions and important statements. 
                    It's larger and lighter than regular paragraphs.
                </p>
                
                <p class="mb-3">
                    This is regular paragraph text with Bootstrap's default styling. 
                    It provides good readability with proper line-height and spacing.
                </p>
                
                <p class="mb-0">
                    <small class="text-muted">This is small text for fine print, captions, or secondary information.</small>
                </p>
            </div>
        </section>
        
        <!-- Display Headings -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Display Headings</h2>
            <p class="lead mb-4">Larger, more impactful headings for hero sections and major announcements.</p>
            
            <div class="row">
                <div class="col-lg-6">
                    <div class="bg-light p-4 rounded">
                        <h1 class="display-1 mb-2">Display 1</h1>
                        <h2 class="display-2 mb-2">Display 2</h2>
                        <h3 class="display-3 mb-2">Display 3</h3>
                        <h4 class="display-4 mb-2">Display 4</h4>
                        <h5 class="display-5 mb-2">Display 5</h5>
                        <h6 class="display-6 mb-0">Display 6</h6>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="bg-dark text-white p-4 rounded">
                        <h1 class="display-4 fw-bold text-warning mb-3">
                            Creative Display
                        </h1>
                        <h2 class="display-6 fw-normal text-light mb-3">
                            Custom Styling
                        </h2>
                        <p class="lead mb-0">
                            Display headings can be customized with color, weight, 
                            and other utilities for unique designs.
                        </p>
                    </div>
                </div>
            </div>
        </section>
        
        <div class="section-divider"></div>
        
        <!-- Font Utilities -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Font Utilities</h2>
            
            <div class="row g-4">
                <!-- Font Sizes -->
                <div class="col-md-6">
                    <div class="card demo-card h-100">
                        <div class="card-header bg-primary text-white">
                            <h5 class="mb-0"><i class="bi bi-type me-2"></i>Font Sizes</h5>
                        </div>
                        <div class="card-body">
                            <p class="fs-1 mb-2">Font size 1 (.fs-1)</p>
                            <p class="fs-2 mb-2">Font size 2 (.fs-2)</p>
                            <p class="fs-3 mb-2">Font size 3 (.fs-3)</p>
                            <p class="fs-4 mb-2">Font size 4 (.fs-4)</p>
                            <p class="fs-5 mb-2">Font size 5 (.fs-5)</p>
                            <p class="fs-6 mb-0">Font size 6 (.fs-6)</p>
                        </div>
                    </div>
                </div>
                
                <!-- Font Weights -->
                <div class="col-md-6">
                    <div class="card demo-card h-100">
                        <div class="card-header bg-success text-white">
                            <h5 class="mb-0"><i class="bi bi-type-bold me-2"></i>Font Weights</h5>
                        </div>
                        <div class="card-body">
                            <p class="fw-bold mb-2">Bold text (.fw-bold)</p>
                            <p class="fw-bolder mb-2">Bolder text (.fw-bolder)</p>
                            <p class="fw-semibold mb-2">Semibold text (.fw-semibold)</p>
                            <p class="fw-medium mb-2">Medium text (.fw-medium)</p>
                            <p class="fw-normal mb-2">Normal text (.fw-normal)</p>
                            <p class="fw-light mb-2">Light text (.fw-light)</p>
                            <p class="fw-lighter mb-0">Lighter text (.fw-lighter)</p>
                        </div>
                    </div>
                </div>
                
                <!-- Text Transform -->
                <div class="col-md-6">
                    <div class="card demo-card h-100">
                        <div class="card-header bg-warning text-dark">
                            <h5 class="mb-0"><i class="bi bi-type-strikethrough me-2"></i>Text Transform</h5>
                        </div>
                        <div class="card-body">
                            <p class="text-lowercase mb-2">LOWERCASED TEXT (.text-lowercase)</p>
                            <p class="text-uppercase mb-2">uppercased text (.text-uppercase)</p>
                            <p class="text-capitalize mb-2">capitalized each word (.text-capitalize)</p>
                            <p class="fst-italic mb-2">Italic text (.fst-italic)</p>
                            <p class="text-decoration-underline mb-2">Underlined text</p>
                            <p class="text-decoration-line-through mb-0">Line through text</p>
                        </div>
                    </div>
                </div>
                
                <!-- Text Alignment -->
                <div class="col-md-6">
                    <div class="card demo-card h-100">
                        <div class="card-header bg-info text-white">
                            <h5 class="mb-0"><i class="bi bi-text-center me-2"></i>Text Alignment</h5>
                        </div>
                        <div class="card-body">
                            <p class="text-start mb-2">Start aligned text (.text-start)</p>
                            <p class="text-center mb-2">Center aligned text (.text-center)</p>
                            <p class="text-end mb-2">End aligned text (.text-end)</p>
                            <div class="text-truncate mb-2" style="width: 200px;">
                                This text will be truncated (.text-truncate)
                            </div>
                            <p class="text-nowrap mb-0" style="width: 100px; background: #f8f9fa;">
                                No wrap text (.text-nowrap)
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <div class="section-divider"></div>
        
        <!-- Color System -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Bootstrap Color System</h2>
            <p class="lead mb-4">A comprehensive color palette for consistent theming and semantic meaning.</p>
            
            <!-- Theme Colors -->
            <h3 class="h4 mb-3">Theme Colors</h3>
            <div class="row g-3 mb-5">
                <div class="col-md-3">
                    <div class="color-swatch bg-primary text-white">
                        <div class="color-info">
                            <strong>Primary</strong><br>
                            <small>#0d6efd</small>
                        </div>
                    </div>
                    <div class="text-center">
                        <code class="small">.bg-primary</code><br>
                        <code class="small">.text-primary</code>
                    </div>
                </div>
                
                <div class="col-md-3">
                    <div class="color-swatch bg-secondary text-white">
                        <div class="color-info">
                            <strong>Secondary</strong><br>
                            <small>#6c757d</small>
                        </div>
                    </div>
                    <div class="text-center">
                        <code class="small">.bg-secondary</code><br>
                        <code class="small">.text-secondary</code>
                    </div>
                </div>
                
                <div class="col-md-3">
                    <div class="color-swatch bg-success text-white">
                        <div class="color-info">
                            <strong>Success</strong><br>
                            <small>#198754</small>
                        </div>
                    </div>
                    <div class="text-center">
                        <code class="small">.bg-success</code><br>
                        <code class="small">.text-success</code>
                    </div>
                </div>
                
                <div class="col-md-3">
                    <div class="color-swatch bg-danger text-white">
                        <div class="color-info">
                            <strong>Danger</strong><br>
                            <small>#dc3545</small>
                        </div>
                    </div>
                    <div class="text-center">
                        <code class="small">.bg-danger</code><br>
                        <code class="small">.text-danger</code>
                    </div>
                </div>
                
                <div class="col-md-3">
                    <div class="color-swatch bg-warning text-dark">
                        <div class="color-info">
                            <strong>Warning</strong><br>
                            <small>#ffc107</small>
                        </div>
                    </div>
                    <div class="text-center">
                        <code class="small">.bg-warning</code><br>
                        <code class="small">.text-warning</code>
                    </div>
                </div>
                
                <div class="col-md-3">
                    <div class="color-swatch bg-info text-dark">
                        <div class="color-info">
                            <strong>Info</strong><br>
                            <small>#0dcaf0</small>
                        </div>
                    </div>
                    <div class="text-center">
                        <code class="small">.bg-info</code><br>
                        <code class="small">.text-info</code>
                    </div>
                </div>
                
                <div class="col-md-3">
                    <div class="color-swatch bg-light text-dark border">
                        <div class="color-info">
                            <strong>Light</strong><br>
                            <small>#f8f9fa</small>
                        </div>
                    </div>
                    <div class="text-center">
                        <code class="small">.bg-light</code><br>
                        <code class="small">.text-light</code>
                    </div>
                </div>
                
                <div class="col-md-3">
                    <div class="color-swatch bg-dark text-white">
                        <div class="color-info">
                            <strong>Dark</strong><br>
                            <small>#212529</small>
                        </div>
                    </div>
                    <div class="text-center">
                        <code class="small">.bg-dark</code><br>
                        <code class="small">.text-dark</code>
                    </div>
                </div>
            </div>
            
            <!-- Text Color Examples -->
            <h3 class="h4 mb-3">Text Colors in Action</h3>
            <div class="row g-4 mb-5">
                <div class="col-lg-6">
                    <div class="bg-light p-4 rounded">
                        <h5 class="text-primary mb-3">Text Color Examples</h5>
                        <p class="text-primary">Primary text conveys brand identity</p>
                        <p class="text-success">Success text indicates positive actions</p>
                        <p class="text-danger">Danger text warns of errors or problems</p>
                        <p class="text-warning">Warning text suggests caution</p>
                        <p class="text-info">Info text provides helpful information</p>
                        <p class="text-secondary">Secondary text for less important content</p>
                        <p class="text-muted mb-0">Muted text for subtle information</p>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="bg-dark p-4 rounded">
                        <h5 class="text-light mb-3">Text on Dark Backgrounds</h5>
                        <p class="text-light">Light text for readability on dark backgrounds</p>
                        <p class="text-warning">Warning color works well on dark</p>
                        <p class="text-info">Info color provides good contrast</p>
                        <p class="text-success">Success color remains visible</p>
                        <p class="text-danger">Danger color for alerts</p>
                        <p class="text-white mb-0">Pure white for maximum contrast</p>
                    </div>
                </div>
            </div>
            
            <!-- Link Colors -->
            <h3 class="h4 mb-3">Link Colors</h3>
            <div class="row g-4 mb-5">
                <div class="col-lg-6">
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title">Standard Link Colors</h6>
                            <p><a href="#" class="link-primary">Primary link color</a></p>
                            <p><a href="#" class="link-secondary">Secondary link color</a></p>
                            <p><a href="#" class="link-success">Success link color</a></p>
                            <p><a href="#" class="link-danger">Danger link color</a></p>
                            <p><a href="#" class="link-warning">Warning link color</a></p>
                            <p class="mb-0"><a href="#" class="link-info">Info link color</a></p>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="card bg-dark text-white">
                        <div class="card-body">
                            <h6 class="card-title">Links on Dark Background</h6>
                            <p><a href="#" class="link-light">Light link color</a></p>
                            <p><a href="#" class="link-warning">Warning link color</a></p>
                            <p><a href="#" class="link-info">Info link color</a></p>
                            <p><a href="#" class="link-success">Success link color</a></p>
                            <p class="mb-0"><a href="#" class="link-danger">Danger link color</a></p>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <div class="section-divider"></div>
        
        <!-- Color Accessibility -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Color Accessibility</h2>
            <p class="lead mb-4">
                Proper color contrast is essential for accessibility. Bootstrap's color combinations 
                are designed to meet WCAG guidelines.
            </p>
            
            <div class="row g-4">
                <div class="col-lg-6">
                    <div class="border rounded p-4 accessibility-good">
                        <h5 class="text-success mb-3">
                            <i class="bi bi-check-circle-fill me-2"></i>Good Contrast Examples
                        </h5>
                        
                        <div class="mb-3">
                            <div class="bg-primary text-white p-3 rounded">
                                <strong>White text on Primary background</strong><br>
                                Excellent contrast ratio for readability
                            </div>
                        </div>
                        
                        <div class="mb-3">
                            <div class="bg-dark text-white p-3 rounded">
                                <strong>White text on Dark background</strong><br>
                                Maximum contrast for easy reading
                            </div>
                        </div>
                        
                        <div class="mb-3">
                            <div class="bg-warning text-dark p-3 rounded">
                                <strong>Dark text on Warning background</strong><br>
                                High contrast maintains readability
                            </div>
                        </div>
                        
                        <div class="mb-0">
                            <div class="bg-light text-dark p-3 rounded border">
                                <strong>Dark text on Light background</strong><br>
                                Classic combination with great contrast
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="border rounded p-4 accessibility-bad">
                        <h5 class="text-danger mb-3">
                            <i class="bi bi-x-circle-fill me-2"></i>Poor Contrast Examples
                        </h5>
                        
                        <div class="alert alert-warning">
                            <strong>Note:</strong> These examples show what to avoid for accessibility.
                        </div>
                        
                        <div class="mb-3">
                            <div class="bg-light text-white p-3 rounded border">
                                <strong>White text on Light background</strong><br>
                                <small class="text-muted">Very poor contrast - avoid this!</small>
                            </div>
                        </div>
                        
                        <div class="mb-3">
                            <div class="bg-warning text-warning p-3 rounded" style="background-color: #fff3cd !important;">
                                <strong>Warning text on Warning background</strong><br>
                                <small>Same color text and background - invisible!</small>
                            </div>
                        </div>
                        
                        <div class="mb-0">
                            <div class="bg-secondary text-muted p-3 rounded">
                                <strong>Muted text on Secondary background</strong><br>
                                <small>Low contrast makes this hard to read</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Accessibility Guidelines -->
            <div class="mt-4">
                <div class="alert alert-info">
                    <h6 class="alert-heading">
                        <i class="bi bi-info-circle me-2"></i>Accessibility Guidelines
                    </h6>
                    <ul class="mb-0">
                        <li><strong>WCAG AA:</strong> Minimum contrast ratio of 4.5:1 for normal text</li>
                        <li><strong>WCAG AAA:</strong> Enhanced contrast ratio of 7:1 for normal text</li>
                        <li><strong>Large Text:</strong> Reduced ratios (3:1 AA, 4.5:1 AAA) for text 18pt+ or 14pt+ bold</li>
                        <li><strong>Color Alone:</strong> Don't rely solely on color to convey information</li>
                        <li><strong>Testing:</strong> Use browser dev tools or online contrast checkers</li>
                    </ul>
                </div>
            </div>
        </section>
        
        <div class="section-divider"></div>
        
        <!-- Practical Examples -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Practical Design Examples</h2>
            <p class="lead mb-4">
                See how typography and colors work together in real-world components and layouts.
            </p>
            
            <div class="row g-4">
                <!-- Card Example -->
                <div class="col-lg-4">
                    <div class="card demo-card h-100">
                        <div class="card-header bg-primary text-white">
                            <h5 class="mb-0">Product Card</h5>
                        </div>
                        <div class="card-body">
                            <h6 class="card-title text-primary">Premium Headphones</h6>
                            <p class="card-text text-muted">High-quality wireless headphones with noise cancellation.</p>
                            <div class="d-flex justify-content-between align-items-center">
                                <span class="fs-4 fw-bold text-success">$299.99</span>
                                <small class="text-decoration-line-through text-muted">$399.99</small>
                            </div>
                        </div>
                        <div class="card-footer bg-light">
                            <div class="d-grid">
                                <button class="btn btn-primary">Add to Cart</button>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Alert Example -->
                <div class="col-lg-4">
                    <div class="card demo-card h-100">
                        <div class="card-header bg-success text-white">
                            <h5 class="mb-0">Status Messages</h5>
                        </div>
                        <div class="card-body">
                            <div class="alert alert-success">
                                <h6 class="alert-heading text-success-emphasis">Success!</h6>
                                <p class="mb-0">Your account has been created successfully.</p>
                            </div>
                            
                            <div class="alert alert-warning">
                                <h6 class="alert-heading text-warning-emphasis">Warning!</h6>
                                <p class="mb-0">Please verify your email address.</p>
                            </div>
                            
                            <div class="alert alert-danger mb-0">
                                <h6 class="alert-heading text-danger-emphasis">Error!</h6>
                                <p class="mb-0">Invalid username or password.</p>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Article Example -->
                <div class="col-lg-4">
                    <div class="card demo-card h-100">
                        <div class="card-header bg-info text-dark">
                            <h5 class="mb-0">Article Preview</h5>
                        </div>
                        <div class="card-body">
                            <small class="text-muted text-uppercase fw-semibold">Technology</small>
                            <h6 class="card-title mt-2">The Future of Web Development</h6>
                            <p class="card-text text-muted small">
                                Explore the latest trends and technologies shaping the future 
                                of web development in 2025 and beyond.
                            </p>
                            <div class="d-flex justify-content-between align-items-center">
                                <small class="text-muted">Jan 15, 2025</small>
                                <a href="#" class="link-primary text-decoration-none">Read More →</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Typography Best Practices -->
        <section class="mb-5">
            <h2 class="display-6 mb-4">Typography & Color Best Practices</h2>
            
            <div class="row g-4">
                <div class="col-lg-6">
                    <div class="card border-success">
                        <div class="card-header bg-success text-white">
                            <h5 class="mb-0">
                                <i class="bi bi-check-circle me-2"></i>Best Practices
                            </h5>
                        </div>
                        <div class="card-body">
                            <ul class="list-unstyled">
                                <li class="mb-2">
                                    <i class="bi bi-check text-success me-2"></i>
                                    <strong>Hierarchy:</strong> Use heading levels (h1-h6) logically
                                </li>
                                <li class="mb-2">
                                    <i class="bi bi-check text-success me-2"></i>
                                    <strong>Readability:</strong> Maintain proper line height and spacing
                                </li>
                                <li class="mb-2">
                                    <i class="bi bi-check text-success me-2"></i>
                                    <strong>Contrast:</strong> Ensure WCAG AA compliance minimum
                                </li>
                                <li class="mb-2">
                                    <i class="bi bi-check text-success me-2"></i>
                                    <strong>Consistency:</strong> Use Bootstrap's utility classes
                                </li>
                                <li class="mb-2">
                                    <i class="bi bi-check text-success me-2"></i>
                                    <strong>Semantic Colors:</strong> Use color meanings consistently
                                </li>
                                <li class="mb-0">
                                    <i class="bi bi-check text-success me-2"></i>
                                    <strong>Responsive:</strong> Test typography at all screen sizes
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="card border-danger">
                        <div class="card-header bg-danger text-white">
                            <h5 class="mb-0">
                                <i class="bi bi-x-circle me-2"></i>Common Mistakes
                            </h5>
                        </div>
                        <div class="card-body">
                            <ul class="list-unstyled">
                                <li class="mb-2">
                                    <i class="bi bi-x text-danger me-2"></i>
                                    <strong>Poor Contrast:</strong> Using light colors on light backgrounds
                                </li>
                                <li class="mb-2">
                                    <i class="bi bi-x text-danger me-2"></i>
                                    <strong>Too Many Fonts:</strong> Mixing multiple font families
                                </li>
                                <li class="mb-2">
                                    <i class="bi bi-x text-danger me-2"></i>
                                    <strong>Wrong Hierarchy:</strong> Skipping heading levels
                                </li>
                                <li class="mb-2">
                                    <i class="bi bi-x text-danger me-2"></i>
                                    <strong>Color Only:</strong> Relying solely on color for meaning
                                </li>
                                <li class="mb-2">
                                    <i class="bi bi-x text-danger me-2"></i>
                                    <strong>Overuse:</strong> Too many colors or font weights
                                </li>
                                <li class="mb-0">
                                    <i class="bi bi-x text-danger me-2"></i>
                                    <strong>Mobile Issues:</strong> Text too small on mobile devices
                                </li>
                            </ul>
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
                    <h3 class="display-6 mb-3">Typography & Colors Mastery Complete!</h3>
                    <p class="lead mb-3">
                        You've learned to create beautiful, accessible designs using Bootstrap's 
                        typography system and color palette.
                    </p>
                    <div class="d-flex flex-wrap gap-2">
                        <span class="badge bg-primary fs-6 px-3 py-2">Typography System</span>
                        <span class="badge bg-success fs-6 px-3 py-2">Color Utilities</span>
                        <span class="badge bg-warning text-dark fs-6 px-3 py-2">Accessibility</span>
                        <span class="badge bg-info text-dark fs-6 px-3 py-2">Design Consistency</span>
                    </div>
                </div>
                <div class="col-lg-4 text-lg-end">
                    <h5 class="text-light">Ready for the next lesson?</h5>
                    <p class="text-light opacity-75">
                        Tomorrow we'll explore Bootstrap's Flexbox utilities and layout helpers!
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
                        Built with Bootstrap 5.3.2 and lots of ❤️
                    </small>
                </div>
            </div>
        </div>
    </footer>
    
    <!-- Bootstrap JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <!-- Custom JavaScript -->
    <script>
        // Add interactive features
        document.addEventListener('DOMContentLoaded', function() {
            // Smooth scroll for internal links
            document.querySelectorAll('a[href^="#"]').forEach(link => {
                link.addEventListener('click', function(e) {
                    e.preventDefault();
                    const target = document.querySelector(this.getAttribute('href'));
                    if (target) {
                        target.scrollIntoView({ behavior: 'smooth' });
                    }
                });
            });
            
            // Copy code on click
            document.querySelectorAll('code').forEach(code => {
                code.style.cursor = 'pointer';
                code.title = 'Click to copy';
                
                code.addEventListener('click', function() {
                    navigator.clipboard.writeText(this.textContent).then(() => {
                        // Visual feedback
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
            
            // Highlight color swatches on hover
            document.querySelectorAll('.color-swatch').forEach(swatch => {
                swatch.addEventListener('mouseenter', function() {
                    this.style.transform = 'scale(1.05)';
                    this.style.transition = 'transform 0.3s ease';
                });
                
                swatch.addEventListener('mouseleave', function() {
                    this.style.transform = 'scale(1)';
                });
            });
            
            console.log('Bootstrap Typography & Colors Demo Loaded! 🎨');
        });
    </script>
</body>
</html>
```

## 📋 Typography & Colors Mastery Checklist

### Typography System
- [ ] Understand Bootstrap's type scale (h1-h6, fs-1 to fs-6)
- [ ] Use display headings for impact (display-1 to display-6)
- [ ] Apply font weight utilities (fw-bold, fw-light, etc.)
- [ ] Implement text transforms and alignment
- [ ] Create proper typography hierarchy

### Color System
- [ ] Master theme colors (primary, secondary, success, etc.)
- [ ] Apply text color utilities effectively
- [ ] Use background color utilities properly
- [ ] Implement link color variations
- [ ] Understand color opacity utilities

### Accessibility
- [ ] Ensure proper color contrast ratios
- [ ] Test with accessibility tools
- [ ] Avoid using color alone for meaning
- [ ] Use semantic color meanings consistently
- [ ] Verify readability at all screen sizes

### Practical Application
- [ ] Build cohesive design systems
- [ ] Create accessible color combinations
- [ ] Apply consistent typography patterns
- [ ] Test across different devices and browsers

## 🎉 Day 10 Complete!

### ✅ Today's Achievements
- **Typography Mastery:** Complete understanding of Bootstrap's type system
- **Color System:** Comprehensive knowledge of Bootstrap's color utilities
- **Accessibility Focus:** WCAG-compliant color and typography practices
- **Real-World Examples:** Practical application in cards, alerts, and layouts
- **Best Practices:** Guidelines for consistent, accessible design

### 🏆 Skills Gained
- Bootstrap typography hierarchy and utilities
- Complete color system and theming
- Accessibility-first design approach
- Semantic color usage and meaning
- Typography and color best practices
- Design system consistency

## 🔗 Additional Resources

- **[Bootstrap Typography Docs](https://getbootstrap.com/docs/5.3/content/typography/)** - Official typography guide
- **[Bootstrap Colors Docs](https://getbootstrap.com/docs/5.3/utilities/colors/)** - Complete color utilities
- **[WebAIM Color Contrast Checker](https://webaim.org/resources/contrastchecker/)** - Test color combinations
- **[Material Design Color Tool](https://material.io/resources/color/)** - Color palette inspiration

---

## 🚀 What's Next?

Tomorrow in **Day 11: Bootstrap Flexbox & Utilities**, you'll learn:
- Flexbox utilities for advanced layouts
- Spacing and sizing utilities
- Display and positioning helpers
- Border and shadow utilities

**Excellent work on Day 10!** You now have a solid foundation in Bootstrap's typography and color systems. Tomorrow we'll explore the powerful utility classes that make Bootstrap so flexible and efficient.

---

**Remember: Great typography and thoughtful color choices are the foundation of excellent user experience!** 🎨✨