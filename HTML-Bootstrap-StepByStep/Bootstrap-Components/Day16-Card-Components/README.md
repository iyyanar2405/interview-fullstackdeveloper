# Day 16: Card Components & Layouts 🃏

Welcome to Day 16 - mastering Bootstrap's most versatile component system! Today you'll explore Bootstrap cards, learning to create flexible, powerful content containers that form the backbone of modern web layouts and user interfaces.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master Bootstrap card anatomy and structure
- Create complex card layouts and combinations
- Design responsive card grids and collections
- Implement advanced card features (headers, footers, overlays)
- Build interactive card components with JavaScript
- Optimize card performance and accessibility
- Create professional card-based interfaces
- Design custom card themes and styling

## 📚 Bootstrap Card System Deep Dive

### Card Fundamentals

Bootstrap cards provide a flexible container for displaying content in a consistent, visually appealing format. Cards can contain headers, bodies, footers, images, and various interactive elements.

### Core Card Classes
```css
/* Base card structure */
.card                    /* Main card container */
.card-header            /* Optional header section */
.card-body              /* Main content area */
.card-footer            /* Optional footer section */
.card-title             /* Card title styling */
.card-subtitle          /* Card subtitle styling */
.card-text              /* Card body text */
.card-link              /* Card link styling */

/* Card image classes */
.card-img-top           /* Image at top of card */
.card-img-bottom        /* Image at bottom of card */
.card-img               /* Full card image */
.card-img-overlay       /* Overlay content on image */

/* Card layout utilities */
.card-group             /* Equal height card groups */
.card-deck              /* Flexible card layouts */
.card-columns           /* Masonry-style columns */
```

## 💻 Comprehensive Card Components

### Complete Card Showcase

Create `card-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 16: Bootstrap Card Components | Advanced Card Layouts</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <style>
        :root {
            --primary-gradient: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            --success-gradient: linear-gradient(135deg, #56ab2f 0%, #a8e6cf 100%);
            --danger-gradient: linear-gradient(135deg, #ff416c 0%, #ff4b2b 100%);
            --warning-gradient: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            --info-gradient: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
            --card-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            --card-shadow-hover: 0 8px 25px rgba(0, 0, 0, 0.15);
            --card-shadow-active: 0 2px 4px rgba(0, 0, 0, 0.1);
            --border-radius: 12px;
            --transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            min-height: 100vh;
        }
        
        .section-header {
            background: var(--primary-gradient);
            color: white;
            padding: 4rem 0;
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
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="20" cy="20" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="80" cy="20" r="1" fill="rgba(255,255,255,0.05)"/><circle cx="50" cy="50" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="20" cy="80" r="1" fill="rgba(255,255,255,0.05)"/><circle cx="80" cy="80" r="1" fill="rgba(255,255,255,0.1)"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
            opacity: 0.3;
        }
        
        .section-header .content {
            position: relative;
            z-index: 2;
        }
        
        .demo-section {
            background: white;
            border-radius: var(--border-radius);
            padding: 2.5rem;
            margin-bottom: 3rem;
            box-shadow: var(--card-shadow);
            border: 1px solid rgba(0,0,0,0.05);
        }
        
        .demo-title {
            color: #2d3748;
            font-weight: 700;
            margin-bottom: 1.5rem;
            padding-bottom: 0.75rem;
            border-bottom: 3px solid #e2e8f0;
            position: relative;
        }
        
        .demo-title::after {
            content: '';
            position: absolute;
            bottom: -3px;
            left: 0;
            width: 60px;
            height: 3px;
            background: var(--primary-gradient);
            border-radius: 2px;
        }
        
        /* Enhanced Card Styles */
        .card {
            border: none;
            border-radius: var(--border-radius);
            box-shadow: var(--card-shadow);
            transition: var(--transition);
            overflow: hidden;
        }
        
        .card:hover {
            transform: translateY(-5px);
            box-shadow: var(--card-shadow-hover);
        }
        
        .card:active {
            transform: translateY(-2px);
            box-shadow: var(--card-shadow-active);
        }
        
        /* Gradient Card Headers */
        .card-header-gradient-primary {
            background: var(--primary-gradient);
            color: white;
            border: none;
        }
        
        .card-header-gradient-success {
            background: var(--success-gradient);
            color: white;
            border: none;
        }
        
        .card-header-gradient-danger {
            background: var(--danger-gradient);
            color: white;
            border: none;
        }
        
        .card-header-gradient-warning {
            background: var(--warning-gradient);
            color: white;
            border: none;
        }
        
        .card-header-gradient-info {
            background: var(--info-gradient);
            color: white;
            border: none;
        }
        
        /* Pricing Card Styles */
        .pricing-card {
            position: relative;
            text-align: center;
            transition: var(--transition);
        }
        
        .pricing-card.featured {
            transform: scale(1.05);
            z-index: 10;
        }
        
        .pricing-card.featured .card-header {
            background: var(--primary-gradient);
            color: white;
        }
        
        .popular-badge {
            position: absolute;
            top: -15px;
            left: 50%;
            transform: translateX(-50%);
            background: var(--danger-gradient);
            color: white;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            box-shadow: var(--card-shadow);
        }
        
        .price {
            font-size: 3rem;
            font-weight: 700;
            color: #2d3748;
            margin: 1rem 0;
        }
        
        .price-period {
            font-size: 1rem;
            color: #718096;
            font-weight: 400;
        }
        
        /* Profile Card Styles */
        .profile-card {
            text-align: center;
            position: relative;
        }
        
        .profile-avatar {
            width: 100px;
            height: 100px;
            border-radius: 50%;
            border: 4px solid white;
            box-shadow: var(--card-shadow);
            margin: -50px auto 1rem;
            position: relative;
            z-index: 5;
        }
        
        .profile-cover {
            height: 120px;
            background: var(--primary-gradient);
            margin: -1rem -1rem 0;
            border-radius: var(--border-radius) var(--border-radius) 0 0;
        }
        
        .social-stats {
            display: flex;
            justify-content: space-around;
            padding: 1rem 0;
            border-top: 1px solid #e2e8f0;
            margin-top: 1rem;
        }
        
        .stat-item {
            text-align: center;
        }
        
        .stat-number {
            font-size: 1.5rem;
            font-weight: 700;
            color: #2d3748;
            display: block;
        }
        
        .stat-label {
            font-size: 0.8rem;
            color: #718096;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        /* Image Overlay Cards */
        .card-img-overlay {
            background: linear-gradient(135deg, rgba(0,0,0,0.4) 0%, rgba(0,0,0,0.6) 100%);
            color: white;
            display: flex;
            flex-direction: column;
            justify-content: flex-end;
        }
        
        .overlay-content {
            background: rgba(0,0,0,0.3);
            backdrop-filter: blur(10px);
            padding: 1.5rem;
            border-radius: 8px;
            margin-top: auto;
        }
        
        /* Product Card Styles */
        .product-card {
            position: relative;
            overflow: hidden;
        }
        
        .product-badge {
            position: absolute;
            top: 10px;
            right: 10px;
            z-index: 10;
        }
        
        .product-actions {
            position: absolute;
            top: 10px;
            left: 10px;
            opacity: 0;
            transition: var(--transition);
        }
        
        .product-card:hover .product-actions {
            opacity: 1;
        }
        
        .price-original {
            text-decoration: line-through;
            color: #718096;
            font-size: 0.9rem;
        }
        
        .price-current {
            color: #e53e3e;
            font-weight: 700;
            font-size: 1.2rem;
        }
        
        /* Dashboard Card Styles */
        .dashboard-card {
            background: white;
            border-left: 4px solid;
            transition: var(--transition);
        }
        
        .dashboard-card.card-primary {
            border-left-color: #667eea;
        }
        
        .dashboard-card.card-success {
            border-left-color: #56ab2f;
        }
        
        .dashboard-card.card-danger {
            border-left-color: #ff416c;
        }
        
        .dashboard-card.card-warning {
            border-left-color: #f093fb;
        }
        
        .dashboard-icon {
            width: 60px;
            height: 60px;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
            color: white;
        }
        
        .dashboard-icon.icon-primary {
            background: var(--primary-gradient);
        }
        
        .dashboard-icon.icon-success {
            background: var(--success-gradient);
        }
        
        .dashboard-icon.icon-danger {
            background: var(--danger-gradient);
        }
        
        .dashboard-icon.icon-warning {
            background: var(--warning-gradient);
        }
        
        /* Timeline Card */
        .timeline-card {
            border-left: 3px solid #e2e8f0;
            margin-left: 20px;
            position: relative;
        }
        
        .timeline-card::before {
            content: '';
            position: absolute;
            left: -8px;
            top: 20px;
            width: 12px;
            height: 12px;
            border-radius: 50%;
            background: var(--primary-gradient);
        }
        
        /* Masonry Layout */
        .masonry-container {
            column-count: 3;
            column-gap: 1.5rem;
        }
        
        .masonry-item {
            break-inside: avoid;
            margin-bottom: 1.5rem;
        }
        
        @media (max-width: 992px) {
            .masonry-container {
                column-count: 2;
            }
        }
        
        @media (max-width: 576px) {
            .masonry-container {
                column-count: 1;
            }
        }
        
        /* Loading Animation */
        .card-loading {
            position: relative;
            overflow: hidden;
        }
        
        .card-loading::after {
            content: '';
            position: absolute;
            top: 0;
            left: -100%;
            width: 100%;
            height: 100%;
            background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent);
            animation: loading 1.5s infinite;
        }
        
        @keyframes loading {
            0% { left: -100%; }
            100% { left: 100%; }
        }
        
        /* Flip Card Animation */
        .flip-card {
            background-color: transparent;
            perspective: 1000px;
            height: 300px;
        }
        
        .flip-card-inner {
            position: relative;
            width: 100%;
            height: 100%;
            text-align: center;
            transition: transform 0.6s;
            transform-style: preserve-3d;
        }
        
        .flip-card:hover .flip-card-inner {
            transform: rotateY(180deg);
        }
        
        .flip-card-front, .flip-card-back {
            position: absolute;
            width: 100%;
            height: 100%;
            backface-visibility: hidden;
            border-radius: var(--border-radius);
            box-shadow: var(--card-shadow);
        }
        
        .flip-card-back {
            background: var(--primary-gradient);
            color: white;
            transform: rotateY(180deg);
            display: flex;
            align-items: center;
            justify-content: center;
            flex-direction: column;
        }
        
        /* Interactive Elements */
        .card-interactive {
            cursor: pointer;
            user-select: none;
        }
        
        .card-interactive:hover {
            transform: translateY(-5px) scale(1.02);
        }
        
        .card-interactive:active {
            transform: translateY(-2px) scale(1.01);
        }
        
        /* Responsive Cards */
        @media (max-width: 768px) {
            .card-horizontal {
                flex-direction: column;
            }
            
            .card-horizontal .card-img-left {
                width: 100%;
                height: 200px;
                object-fit: cover;
            }
            
            .pricing-card.featured {
                transform: none;
                margin-bottom: 2rem;
            }
        }
    </style>
</head>
<body>
    <!-- Header -->
    <header class="section-header">
        <div class="container">
            <div class="content text-center">
                <h1 class="display-4 fw-bold mb-3">Day 16: Card Components & Layouts</h1>
                <p class="lead mb-0">Master Bootstrap's versatile card system for creating beautiful, flexible content containers</p>
            </div>
        </div>
    </header>

    <div class="container py-4">
        
        <!-- Basic Card Structure -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-layout-text-window me-2"></i>
                Basic Card Structure & Anatomy
            </h2>
            <p class="text-muted mb-4">Understanding the fundamental components that make up a Bootstrap card.</p>
            
            <div class="row g-4">
                <div class="col-lg-4">
                    <div class="card">
                        <div class="card-header">
                            <h5 class="card-title mb-0">Card Header</h5>
                        </div>
                        <div class="card-body">
                            <h6 class="card-subtitle mb-2 text-muted">Card Subtitle</h6>
                            <p class="card-text">This is the main content area of the card. You can include any HTML content here including text, images, buttons, and more.</p>
                            <a href="#" class="card-link">Card Link</a>
                            <a href="#" class="card-link">Another Link</a>
                        </div>
                        <div class="card-footer text-muted">
                            Card Footer
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card">
                        <div class="card-header card-header-gradient-primary">
                            <h5 class="card-title mb-0">
                                <i class="bi bi-star me-2"></i>
                                Enhanced Header
                            </h5>
                        </div>
                        <div class="card-body">
                            <p class="card-text">Cards with gradient headers provide better visual hierarchy and brand consistency.</p>
                            <div class="d-flex gap-2">
                                <button class="btn btn-primary btn-sm">Action</button>
                                <button class="btn btn-outline-secondary btn-sm">Cancel</button>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card border-0">
                        <div class="card-body text-center">
                            <div class="dashboard-icon icon-success mx-auto mb-3">
                                <i class="bi bi-check-circle"></i>
                            </div>
                            <h5 class="card-title">Borderless Card</h5>
                            <p class="card-text">Clean, minimal design without borders for modern interfaces.</p>
                            <div class="progress mb-3">
                                <div class="progress-bar bg-success" style="width: 75%"></div>
                            </div>
                            <small class="text-success">75% Complete</small>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Image Cards -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-image me-2"></i>
                Image Cards & Visual Content
            </h2>
            <p class="text-muted mb-4">Incorporating images into card designs for enhanced visual appeal and content presentation.</p>
            
            <div class="row g-4">
                <div class="col-lg-4">
                    <div class="card">
                        <img src="https://via.placeholder.com/400x200/667eea/ffffff?text=Card+Image+Top" 
                             class="card-img-top" alt="Card image top">
                        <div class="card-body">
                            <h5 class="card-title">Image Top Card</h5>
                            <p class="card-text">Perfect for blog posts, articles, or any content where the image serves as a preview or header.</p>
                            <p class="card-text">
                                <small class="text-muted">Last updated 3 mins ago</small>
                            </p>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">Image Bottom Card</h5>
                            <p class="card-text">Sometimes you want the content first and the image as supporting visual material.</p>
                            <a href="#" class="btn btn-primary">Learn More</a>
                        </div>
                        <img src="https://via.placeholder.com/400x200/56ab2f/ffffff?text=Card+Image+Bottom" 
                             class="card-img-bottom" alt="Card image bottom">
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card text-white">
                        <img src="https://via.placeholder.com/400x300/ff416c/ffffff?text=Overlay+Background" 
                             class="card-img" alt="Card background">
                        <div class="card-img-overlay">
                            <div class="overlay-content">
                                <h5 class="card-title">Image Overlay</h5>
                                <p class="card-text">Create stunning hero sections with text overlaid on images.</p>
                                <p class="card-text">
                                    <small>Perfect for portfolios and showcases</small>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Card Layouts & Groups -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-grid-3x3 me-2"></i>
                Card Groups & Layout Systems
            </h2>
            <p class="text-muted mb-4">Organizing multiple cards into cohesive layouts and groups for better content structure.</p>
            
            <h5 class="mb-3">Equal Height Card Group</h5>
            <div class="card-group mb-4">
                <div class="card">
                    <img src="https://via.placeholder.com/300x150/667eea/ffffff?text=Group+1" 
                         class="card-img-top" alt="Group card 1">
                    <div class="card-body">
                        <h5 class="card-title">Group Card 1</h5>
                        <p class="card-text">This is a wider card with supporting text below as a natural lead-in to additional content.</p>
                    </div>
                    <div class="card-footer">
                        <small class="text-muted">Last updated 3 mins ago</small>
                    </div>
                </div>
                <div class="card">
                    <img src="https://via.placeholder.com/300x150/56ab2f/ffffff?text=Group+2" 
                         class="card-img-top" alt="Group card 2">
                    <div class="card-body">
                        <h5 class="card-title">Group Card 2</h5>
                        <p class="card-text">This card has supporting text below as a natural lead-in to additional content. It's a bit longer than the first card.</p>
                    </div>
                    <div class="card-footer">
                        <small class="text-muted">Last updated 5 mins ago</small>
                    </div>
                </div>
                <div class="card">
                    <img src="https://via.placeholder.com/300x150/ff416c/ffffff?text=Group+3" 
                         class="card-img-top" alt="Group card 3">
                    <div class="card-body">
                        <h5 class="card-title">Group Card 3</h5>
                        <p class="card-text">This is a wider card with supporting text below as a natural lead-in to additional content. This card has even longer content than the previous ones to demonstrate equal height behavior.</p>
                    </div>
                    <div class="card-footer">
                        <small class="text-muted">Last updated 7 mins ago</small>
                    </div>
                </div>
            </div>
            
            <h5 class="mb-3">Flexible Grid Layout</h5>
            <div class="row g-3">
                <div class="col-sm-6 col-lg-3">
                    <div class="card dashboard-card card-primary">
                        <div class="card-body d-flex align-items-center">
                            <div class="dashboard-icon icon-primary me-3">
                                <i class="bi bi-people"></i>
                            </div>
                            <div>
                                <h6 class="card-title mb-1">Users</h6>
                                <h4 class="mb-0">2,547</h4>
                                <small class="text-success">
                                    <i class="bi bi-arrow-up"></i> 12%
                                </small>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-sm-6 col-lg-3">
                    <div class="card dashboard-card card-success">
                        <div class="card-body d-flex align-items-center">
                            <div class="dashboard-icon icon-success me-3">
                                <i class="bi bi-graph-up"></i>
                            </div>
                            <div>
                                <h6 class="card-title mb-1">Revenue</h6>
                                <h4 class="mb-0">$12,847</h4>
                                <small class="text-success">
                                    <i class="bi bi-arrow-up"></i> 8%
                                </small>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-sm-6 col-lg-3">
                    <div class="card dashboard-card card-warning">
                        <div class="card-body d-flex align-items-center">
                            <div class="dashboard-icon icon-warning me-3">
                                <i class="bi bi-cart"></i>
                            </div>
                            <div>
                                <h6 class="card-title mb-1">Orders</h6>
                                <h4 class="mb-0">1,247</h4>
                                <small class="text-warning">
                                    <i class="bi bi-arrow-right"></i> 0%
                                </small>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-sm-6 col-lg-3">
                    <div class="card dashboard-card card-danger">
                        <div class="card-body d-flex align-items-center">
                            <div class="dashboard-icon icon-danger me-3">
                                <i class="bi bi-exclamation-triangle"></i>
                            </div>
                            <div>
                                <h6 class="card-title mb-1">Issues</h6>
                                <h4 class="mb-0">23</h4>
                                <small class="text-danger">
                                    <i class="bi bi-arrow-down"></i> 15%
                                </small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Specialized Card Types -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-star me-2"></i>
                Specialized Card Types
            </h2>
            <p class="text-muted mb-4">Purpose-built card designs for specific use cases like pricing, profiles, and products.</p>
            
            <div class="row g-4 mb-5">
                <div class="col-lg-4">
                    <div class="card pricing-card">
                        <div class="card-header">
                            <h4 class="card-title">Basic Plan</h4>
                            <p class="card-text text-muted">Perfect for individuals</p>
                        </div>
                        <div class="card-body">
                            <div class="price">
                                $9<span class="price-period">/mo</span>
                            </div>
                            <ul class="list-unstyled">
                                <li><i class="bi bi-check text-success me-2"></i>Up to 5 projects</li>
                                <li><i class="bi bi-check text-success me-2"></i>10GB storage</li>
                                <li><i class="bi bi-check text-success me-2"></i>Email support</li>
                                <li><i class="bi bi-x text-danger me-2"></i>Priority support</li>
                                <li><i class="bi bi-x text-danger me-2"></i>Advanced features</li>
                            </ul>
                            <button class="btn btn-outline-primary w-100">Choose Plan</button>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card pricing-card featured">
                        <div class="popular-badge">Most Popular</div>
                        <div class="card-header">
                            <h4 class="card-title">Pro Plan</h4>
                            <p class="card-text">Best for professionals</p>
                        </div>
                        <div class="card-body">
                            <div class="price">
                                $29<span class="price-period">/mo</span>
                            </div>
                            <ul class="list-unstyled">
                                <li><i class="bi bi-check text-success me-2"></i>Unlimited projects</li>
                                <li><i class="bi bi-check text-success me-2"></i>100GB storage</li>
                                <li><i class="bi bi-check text-success me-2"></i>Priority support</li>
                                <li><i class="bi bi-check text-success me-2"></i>Advanced features</li>
                                <li><i class="bi bi-check text-success me-2"></i>Team collaboration</li>
                            </ul>
                            <button class="btn btn-primary w-100">Choose Plan</button>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card pricing-card">
                        <div class="card-header">
                            <h4 class="card-title">Enterprise</h4>
                            <p class="card-text text-muted">For large organizations</p>
                        </div>
                        <div class="card-body">
                            <div class="price">
                                $99<span class="price-period">/mo</span>
                            </div>
                            <ul class="list-unstyled">
                                <li><i class="bi bi-check text-success me-2"></i>Everything in Pro</li>
                                <li><i class="bi bi-check text-success me-2"></i>Unlimited storage</li>
                                <li><i class="bi bi-check text-success me-2"></i>24/7 phone support</li>
                                <li><i class="bi bi-check text-success me-2"></i>Custom integrations</li>
                                <li><i class="bi bi-check text-success me-2"></i>SLA guarantee</li>
                            </ul>
                            <button class="btn btn-outline-primary w-100">Contact Sales</button>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="row g-4">
                <div class="col-lg-4">
                    <div class="card profile-card">
                        <div class="profile-cover"></div>
                        <img src="https://via.placeholder.com/100x100/ffffff/667eea?text=JS" 
                             class="profile-avatar" alt="Profile avatar">
                        <div class="card-body">
                            <h5 class="card-title">John Smith</h5>
                            <p class="card-text text-muted">Full Stack Developer</p>
                            <p class="card-text">
                                <small class="text-muted">
                                    <i class="bi bi-geo-alt me-1"></i>San Francisco, CA
                                </small>
                            </p>
                            <div class="social-stats">
                                <div class="stat-item">
                                    <span class="stat-number">1.2k</span>
                                    <span class="stat-label">Followers</span>
                                </div>
                                <div class="stat-item">
                                    <span class="stat-number">847</span>
                                    <span class="stat-label">Following</span>
                                </div>
                                <div class="stat-item">
                                    <span class="stat-number">125</span>
                                    <span class="stat-label">Posts</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card product-card">
                        <div class="product-badge">
                            <span class="badge bg-danger">Sale</span>
                        </div>
                        <div class="product-actions">
                            <button class="btn btn-sm btn-light rounded-circle me-2">
                                <i class="bi bi-heart"></i>
                            </button>
                            <button class="btn btn-sm btn-light rounded-circle">
                                <i class="bi bi-eye"></i>
                            </button>
                        </div>
                        <img src="https://via.placeholder.com/300x200/4facfe/ffffff?text=Product+Image" 
                             class="card-img-top" alt="Product">
                        <div class="card-body">
                            <h6 class="card-title">Premium Headphones</h6>
                            <div class="d-flex align-items-center mb-2">
                                <div class="text-warning me-2">
                                    <i class="bi bi-star-fill"></i>
                                    <i class="bi bi-star-fill"></i>
                                    <i class="bi bi-star-fill"></i>
                                    <i class="bi bi-star-fill"></i>
                                    <i class="bi bi-star"></i>
                                </div>
                                <small class="text-muted">(4.2)</small>
                            </div>
                            <div class="d-flex align-items-center justify-content-between">
                                <div>
                                    <span class="price-original me-2">$199.99</span>
                                    <span class="price-current">$149.99</span>
                                </div>
                                <button class="btn btn-primary btn-sm">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="flip-card">
                        <div class="flip-card-inner">
                            <div class="flip-card-front">
                                <div class="card h-100">
                                    <div class="card-body d-flex flex-column justify-content-center text-center">
                                        <div class="dashboard-icon icon-info mx-auto mb-3">
                                            <i class="bi bi-info-circle"></i>
                                        </div>
                                        <h5 class="card-title">Hover to Flip</h5>
                                        <p class="card-text">Interactive card with flip animation effect.</p>
                                    </div>
                                </div>
                            </div>
                            <div class="flip-card-back">
                                <h5>Back Side</h5>
                                <p>This is the back side of the card with additional information and actions.</p>
                                <button class="btn btn-light">Action Button</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Interactive & Advanced Cards -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-lightning me-2"></i>
                Interactive & Advanced Card Features
            </h2>
            <p class="text-muted mb-4">Advanced card interactions, animations, and dynamic content loading.</p>
            
            <div class="row g-4 mb-4">
                <div class="col-lg-6">
                    <div class="card card-interactive" onclick="toggleCardContent(this)">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h5 class="card-title mb-0">Expandable Card</h5>
                            <i class="bi bi-chevron-down transition-transform"></i>
                        </div>
                        <div class="card-body">
                            <p class="card-text">Click anywhere on this card to expand and see more content.</p>
                            <div class="collapse" id="expandableContent">
                                <hr>
                                <p class="card-text">This is the expanded content that appears when you click on the card. It can contain any HTML content including forms, images, and interactive elements.</p>
                                <div class="d-flex gap-2">
                                    <button class="btn btn-primary btn-sm">Primary Action</button>
                                    <button class="btn btn-outline-secondary btn-sm">Secondary</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-6">
                    <div class="card">
                        <div class="card-header">
                            <h5 class="card-title mb-0">Loading States</h5>
                        </div>
                        <div class="card-body">
                            <p class="card-text">Demonstrate different loading states for dynamic content.</p>
                            <div class="d-flex gap-2 mb-3">
                                <button class="btn btn-primary btn-sm" onclick="showLoading()">Show Loading</button>
                                <button class="btn btn-success btn-sm" onclick="showContent()">Load Content</button>
                                <button class="btn btn-secondary btn-sm" onclick="resetCard()">Reset</button>
                            </div>
                            <div id="dynamicContent">
                                <div class="text-center py-4">
                                    <p class="text-muted">Click "Show Loading" to see the loading animation</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="row g-4">
                <div class="col-lg-4">
                    <div class="card timeline-card">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-start mb-2">
                                <h6 class="card-title mb-0">Project Started</h6>
                                <small class="text-muted">2 hours ago</small>
                            </div>
                            <p class="card-text">New project "Bootstrap Cards" has been initiated with all team members assigned.</p>
                            <div class="d-flex gap-1">
                                <img src="https://via.placeholder.com/30x30/667eea/ffffff?text=A" 
                                     class="rounded-circle" width="30" height="30" alt="User">
                                <img src="https://via.placeholder.com/30x30/56ab2f/ffffff?text=B" 
                                     class="rounded-circle" width="30" height="30" alt="User">
                                <img src="https://via.placeholder.com/30x30/ff416c/ffffff?text=C" 
                                     class="rounded-circle" width="30" height="30" alt="User">
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card timeline-card">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-start mb-2">
                                <h6 class="card-title mb-0">Design Review</h6>
                                <small class="text-muted">1 day ago</small>
                            </div>
                            <p class="card-text">Initial design mockups have been reviewed and approved by the client.</p>
                            <div class="progress mb-2">
                                <div class="progress-bar bg-warning" style="width: 60%"></div>
                            </div>
                            <small class="text-muted">60% Complete</small>
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4">
                    <div class="card timeline-card">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-start mb-2">
                                <h6 class="card-title mb-0">Development Phase</h6>
                                <small class="text-muted">3 days ago</small>
                            </div>
                            <p class="card-text">Frontend development has begun using Bootstrap 5 and modern JavaScript.</p>
                            <span class="badge bg-success">Completed</span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Masonry Layout -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-bricks me-2"></i>
                Masonry Layout & Creative Arrangements
            </h2>
            <p class="text-muted mb-4">Creative card layouts including masonry-style arrangements for Pinterest-like interfaces.</p>
            
            <div class="masonry-container">
                <div class="masonry-item">
                    <div class="card">
                        <img src="https://via.placeholder.com/300x200/667eea/ffffff?text=Tall+Image" 
                             class="card-img-top" alt="Masonry item">
                        <div class="card-body">
                            <h6 class="card-title">Short Content</h6>
                            <p class="card-text">This card has less content.</p>
                        </div>
                    </div>
                </div>
                
                <div class="masonry-item">
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title">Text Only Card</h6>
                            <p class="card-text">This card contains only text content and demonstrates how cards of different heights arrange themselves in a masonry layout. The layout automatically adjusts to create an aesthetically pleasing arrangement.</p>
                            <p class="card-text">Additional paragraphs make this card taller than others, showcasing the masonry effect.</p>
                        </div>
                    </div>
                </div>
                
                <div class="masonry-item">
                    <div class="card">
                        <img src="https://via.placeholder.com/300x150/56ab2f/ffffff?text=Wide+Image" 
                             class="card-img-top" alt="Masonry item">
                        <div class="card-body">
                            <h6 class="card-title">Medium Content</h6>
                            <p class="card-text">This card has a moderate amount of content to demonstrate different card heights in the masonry layout.</p>
                            <button class="btn btn-outline-primary btn-sm">Read More</button>
                        </div>
                    </div>
                </div>
                
                <div class="masonry-item">
                    <div class="card">
                        <div class="card-body text-center">
                            <div class="dashboard-icon icon-danger mx-auto mb-3">
                                <i class="bi bi-heart"></i>
                            </div>
                            <h6 class="card-title">Icon Card</h6>
                            <p class="card-text">Simple card with just an icon and minimal text.</p>
                        </div>
                    </div>
                </div>
                
                <div class="masonry-item">
                    <div class="card">
                        <img src="https://via.placeholder.com/300x250/ff416c/ffffff?text=Square+Image" 
                             class="card-img-top" alt="Masonry item">
                        <div class="card-body">
                            <h6 class="card-title">Another Card</h6>
                            <p class="card-text">This demonstrates how different sized images and content create an interesting masonry effect.</p>
                            <div class="d-flex justify-content-between align-items-center">
                                <small class="text-muted">2 mins ago</small>
                                <div>
                                    <button class="btn btn-sm btn-outline-primary">
                                        <i class="bi bi-heart"></i>
                                    </button>
                                    <button class="btn btn-sm btn-outline-secondary">
                                        <i class="bi bi-share"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="masonry-item">
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title">Quote Card</h6>
                            <blockquote class="blockquote">
                                <p>"Design is not just what it looks like and feels like. Design is how it works."</p>
                                <footer class="blockquote-footer mt-2">
                                    Steve Jobs
                                </footer>
                            </blockquote>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
    </div>

    <!-- Bootstrap JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <!-- Custom JavaScript -->
    <script>
        // Expandable card functionality
        function toggleCardContent(card) {
            const collapseElement = card.querySelector('.collapse');
            const chevron = card.querySelector('.bi-chevron-down, .bi-chevron-up');
            
            if (collapseElement) {
                const bsCollapse = new bootstrap.Collapse(collapseElement, {
                    toggle: true
                });
                
                // Toggle chevron direction
                if (chevron) {
                    chevron.classList.toggle('bi-chevron-down');
                    chevron.classList.toggle('bi-chevron-up');
                }
            }
        }
        
        // Loading state functionality
        function showLoading() {
            const content = document.getElementById('dynamicContent');
            content.innerHTML = `
                <div class="text-center py-4">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <p class="text-muted mt-2">Loading content...</p>
                </div>
            `;
            content.parentElement.parentElement.classList.add('card-loading');
        }
        
        function showContent() {
            const content = document.getElementById('dynamicContent');
            setTimeout(() => {
                content.innerHTML = `
                    <div class="alert alert-success">
                        <h6 class="alert-heading">Content Loaded!</h6>
                        <p class="mb-0">This content was loaded dynamically. You can include any HTML content here including forms, tables, charts, or interactive elements.</p>
                    </div>
                    <div class="row g-3">
                        <div class="col-6">
                            <div class="card border-success">
                                <div class="card-body text-center">
                                    <h6 class="card-title text-success">Success</h6>
                                    <p class="card-text">Data loaded successfully</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="card border-info">
                                <div class="card-body text-center">
                                    <h6 class="card-title text-info">Info</h6>
                                    <p class="card-text">Additional information</p>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                content.parentElement.parentElement.classList.remove('card-loading');
            }, 1500);
        }
        
        function resetCard() {
            const content = document.getElementById('dynamicContent');
            content.innerHTML = `
                <div class="text-center py-4">
                    <p class="text-muted">Click "Show Loading" to see the loading animation</p>
                </div>
            `;
            content.parentElement.parentElement.classList.remove('card-loading');
        }
        
        // Add hover effects to interactive cards
        document.querySelectorAll('.card-interactive').forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.cursor = 'pointer';
            });
        });
        
        // Initialize tooltips if any
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
        
        // Add some dynamic interactivity to product cards
        document.querySelectorAll('.product-card .btn-light').forEach(btn => {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const icon = this.querySelector('i');
                if (icon.classList.contains('bi-heart')) {
                    icon.classList.toggle('bi-heart');
                    icon.classList.toggle('bi-heart-fill');
                    this.classList.toggle('btn-light');
                    this.classList.toggle('btn-danger');
                }
            });
        });
        
        console.log('🃏 Day 16 - Card Components loaded successfully!');
    </script>
</body>
</html>
```

## 📋 Day 16 Card Component Checklist

### Card Structure Mastery
- [ ] Basic card anatomy (header, body, footer)
- [ ] Card titles, subtitles, and text styling
- [ ] Card links and navigation elements
- [ ] Borderless and custom styled cards

### Image Integration
- [ ] Top and bottom image placement
- [ ] Image overlay techniques
- [ ] Responsive image handling
- [ ] Background image cards

### Layout & Organization
- [ ] Card groups for equal heights
- [ ] Flexible grid layouts
- [ ] Masonry-style arrangements
- [ ] Timeline and sequential layouts

### Specialized Card Types
- [ ] Pricing comparison cards
- [ ] Profile/user cards
- [ ] Product showcase cards
- [ ] Dashboard stat cards
- [ ] Interactive flip cards

### Advanced Features
- [ ] Loading states and animations
- [ ] Expandable/collapsible content
- [ ] Dynamic content loading
- [ ] Hover effects and transitions

### Responsive Design
- [ ] Mobile-first card layouts
- [ ] Breakpoint-specific arrangements
- [ ] Touch-friendly interactions
- [ ] Adaptive content display

## 🎯 Day 16 Complete!

### ✅ Achievements Unlocked:
- **Card Mastery:** Complete understanding of Bootstrap card anatomy
- **Layout Systems:** Mastered card groups, grids, and masonry layouts
- **Specialized Cards:** Built pricing, profile, product, and dashboard cards
- **Interactive Features:** Implemented loading states, animations, and dynamic content
- **Responsive Design:** Created mobile-friendly card layouts
- **Advanced Styling:** Custom gradients, shadows, and visual effects

### 🔗 Key Takeaways:
1. **Flexible Container:** Cards are Bootstrap's most versatile content containers
2. **Consistent Structure:** Always follow card anatomy for predictable layouts
3. **Image Integration:** Proper image handling enhances visual appeal
4. **Layout Planning:** Choose appropriate card layouts for content type
5. **Interactive Design:** Adding interactions improves user engagement
6. **Performance:** Optimize images and animations for smooth experience

## 📖 Additional Resources

- **[Bootstrap Cards Documentation](https://getbootstrap.com/docs/5.3/components/card/)** - Official card component guide
- **[CSS Grid and Cards](https://css-tricks.com/snippets/css/complete-guide-grid/)** - Advanced grid layouts
- **[Card Design Patterns](https://ui-patterns.com/patterns/cards)** - UI design patterns for cards
- **[Masonry Layout Guide](https://css-tricks.com/piecing-together-approaches-for-a-css-masonry-layout/)** - CSS masonry techniques

---

**Next up: Day 17 - Navigation Components** where you'll master Bootstrap's navigation system including navbars, breadcrumbs, and tabs! 🧭