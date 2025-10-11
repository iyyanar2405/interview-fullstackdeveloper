# Day 14: Landing Page Project 🚀

Welcome to Day 14 - the capstone project of Bootstrap Basics! Today you'll build a complete, professional landing page that showcases everything you've learned. This comprehensive project will demonstrate responsive design, modern components, performance optimization, and real-world application of Bootstrap skills.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Build a complete landing page from scratch using Bootstrap
- Apply responsive design principles across all breakpoints
- Integrate modern UI components and interactions
- Implement performance optimization techniques
- Create compelling copy and visual hierarchy
- Deploy a production-ready Bootstrap website
- Document your development process and decisions
- Showcase your Bootstrap mastery with a portfolio piece

## 📚 Project Overview

### Landing Page Requirements

We'll build a modern SaaS product landing page featuring:
- **Hero Section** with compelling headline and CTA
- **Features Section** showcasing product benefits
- **Testimonials** with customer reviews
- **Pricing Plans** with comparison table
- **Contact Form** with validation
- **Footer** with links and social media

### Technical Specifications
- **Fully Responsive:** Works perfectly on all devices
- **Performance Optimized:** Fast loading with minimal bundle size
- **Accessible:** WCAG 2.1 AA compliant
- **SEO Friendly:** Proper meta tags and semantic HTML
- **Interactive:** Smooth animations and hover effects
- **Modern Design:** Clean, professional aesthetic

## 💻 Complete Landing Page Project

### Project Structure
```
landing-page-project/
├── index.html
├── css/
│   ├── custom.css
│   └── bootstrap.min.css
├── js/
│   ├── main.js
│   └── bootstrap.bundle.min.js
├── images/
│   ├── hero-bg.jpg
│   ├── features/
│   └── testimonials/
└── README.md
```

### Complete Landing Page Code

Create `landing-page-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="BootstrapPro - The ultimate Bootstrap learning platform for developers. Master responsive design with hands-on projects and expert guidance.">
    <meta name="keywords" content="Bootstrap, CSS Framework, Responsive Design, Web Development, Learning Platform">
    <meta name="author" content="Bootstrap Learning Journey">
    
    <!-- Open Graph / Facebook -->
    <meta property="og:type" content="website">
    <meta property="og:url" content="https://bootstrappro.dev/">
    <meta property="og:title" content="BootstrapPro - Master Bootstrap Development">
    <meta property="og:description" content="Transform your web development skills with our comprehensive Bootstrap learning platform.">
    <meta property="og:image" content="https://bootstrappro.dev/images/og-image.jpg">
    
    <!-- Twitter -->
    <meta property="twitter:card" content="summary_large_image">
    <meta property="twitter:url" content="https://bootstrappro.dev/">
    <meta property="twitter:title" content="BootstrapPro - Master Bootstrap Development">
    <meta property="twitter:description" content="Transform your web development skills with our comprehensive Bootstrap learning platform.">
    <meta property="twitter:image" content="https://bootstrappro.dev/images/twitter-image.jpg">
    
    <title>BootstrapPro - Master Bootstrap Development | Complete Learning Platform</title>
    
    <!-- Favicon -->
    <link rel="icon" type="image/svg+xml" href="data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 100 100'%3E%3Ctext y='.9em' font-size='90'%3E🚀%3C/text%3E%3C/svg%3E">
    
    <!-- Preload critical resources -->
    <link rel="preload" href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" as="style">
    <link rel="preload" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" as="style">
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    
    <!-- Bootstrap Icons -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    
    <!-- Custom CSS -->
    <style>
        :root {
            --primary-gradient: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            --secondary-gradient: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            --success-gradient: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
            --shadow-light: 0 4px 15px rgba(0,0,0,0.1);
            --shadow-medium: 0 8px 30px rgba(0,0,0,0.15);
            --shadow-heavy: 0 15px 35px rgba(0,0,0,0.2);
            --border-radius: 12px;
            --transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        
        * {
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
        }
        
        body {
            overflow-x: hidden;
        }
        
        /* Hero Section */
        .hero-section {
            background: var(--primary-gradient);
            min-height: 100vh;
            position: relative;
            overflow: hidden;
        }
        
        .hero-section::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="25" cy="25" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="75" cy="25" r="1" fill="rgba(255,255,255,0.05)"/><circle cx="50" cy="50" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="25" cy="75" r="1" fill="rgba(255,255,255,0.05)"/><circle cx="75" cy="75" r="1" fill="rgba(255,255,255,0.1)"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
            opacity: 0.3;
        }
        
        .hero-content {
            position: relative;
            z-index: 2;
        }
        
        .hero-title {
            font-size: clamp(2.5rem, 5vw, 4rem);
            font-weight: 700;
            line-height: 1.1;
            margin-bottom: 1.5rem;
        }
        
        .hero-subtitle {
            font-size: clamp(1.1rem, 2vw, 1.3rem);
            font-weight: 400;
            opacity: 0.9;
            margin-bottom: 2rem;
        }
        
        .cta-button {
            background: var(--secondary-gradient);
            border: none;
            border-radius: var(--border-radius);
            padding: 1rem 2rem;
            font-weight: 600;
            font-size: 1.1rem;
            transition: var(--transition);
            box-shadow: var(--shadow-light);
        }
        
        .cta-button:hover {
            transform: translateY(-2px);
            box-shadow: var(--shadow-medium);
        }
        
        .hero-image {
            max-width: 100%;
            height: auto;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow-heavy);
            animation: float 6s ease-in-out infinite;
        }
        
        @keyframes float {
            0%, 100% { transform: translateY(0px); }
            50% { transform: translateY(-20px); }
        }
        
        /* Stats Section */
        .stats-section {
            background: rgba(255, 255, 255, 0.1);
            backdrop-filter: blur(10px);
            border-top: 1px solid rgba(255, 255, 255, 0.2);
        }
        
        .stat-item {
            text-align: center;
            color: white;
        }
        
        .stat-number {
            font-size: 2.5rem;
            font-weight: 700;
            line-height: 1;
        }
        
        .stat-label {
            font-size: 0.9rem;
            opacity: 0.8;
            margin-top: 0.5rem;
        }
        
        /* Features Section */
        .section-title {
            font-size: clamp(2rem, 4vw, 3rem);
            font-weight: 700;
            margin-bottom: 1rem;
        }
        
        .section-subtitle {
            font-size: 1.2rem;
            color: #6c757d;
            margin-bottom: 3rem;
        }
        
        .feature-card {
            background: white;
            border-radius: var(--border-radius);
            padding: 2rem;
            box-shadow: var(--shadow-light);
            border: none;
            transition: var(--transition);
            height: 100%;
        }
        
        .feature-card:hover {
            transform: translateY(-5px);
            box-shadow: var(--shadow-medium);
        }
        
        .feature-icon {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 2rem;
            margin: 0 auto 1.5rem;
            background: var(--primary-gradient);
            color: white;
        }
        
        .feature-title {
            font-size: 1.3rem;
            font-weight: 600;
            margin-bottom: 1rem;
        }
        
        .feature-description {
            color: #6c757d;
            line-height: 1.6;
        }
        
        /* Testimonials Section */
        .testimonials-section {
            background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
        }
        
        .testimonial-card {
            background: white;
            border-radius: var(--border-radius);
            padding: 2rem;
            box-shadow: var(--shadow-light);
            border: none;
            position: relative;
            margin-bottom: 2rem;
        }
        
        .testimonial-card::before {
            content: '"';
            position: absolute;
            top: -10px;
            left: 20px;
            font-size: 4rem;
            color: var(--bs-primary);
            font-family: Georgia, serif;
        }
        
        .testimonial-text {
            font-size: 1.1rem;
            line-height: 1.6;
            margin-bottom: 1.5rem;
            font-style: italic;
        }
        
        .testimonial-author {
            display: flex;
            align-items: center;
            gap: 1rem;
        }
        
        .author-avatar {
            width: 60px;
            height: 60px;
            border-radius: 50%;
            object-fit: cover;
        }
        
        .author-info h6 {
            margin: 0;
            font-weight: 600;
        }
        
        .author-info small {
            color: #6c757d;
        }
        
        /* Pricing Section */
        .pricing-card {
            background: white;
            border-radius: var(--border-radius);
            padding: 2rem;
            box-shadow: var(--shadow-light);
            border: none;
            transition: var(--transition);
            position: relative;
            height: 100%;
        }
        
        .pricing-card:hover {
            transform: translateY(-5px);
            box-shadow: var(--shadow-medium);
        }
        
        .pricing-card.featured {
            background: var(--primary-gradient);
            color: white;
            transform: scale(1.05);
        }
        
        .pricing-card.featured .price {
            color: white;
        }
        
        .popular-badge {
            position: absolute;
            top: -10px;
            left: 50%;
            transform: translateX(-50%);
            background: var(--secondary-gradient);
            color: white;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
        }
        
        .plan-name {
            font-size: 1.5rem;
            font-weight: 700;
            margin-bottom: 0.5rem;
        }
        
        .plan-description {
            color: #6c757d;
            margin-bottom: 1.5rem;
        }
        
        .pricing-card.featured .plan-description {
            color: rgba(255, 255, 255, 0.8);
        }
        
        .price {
            font-size: 3rem;
            font-weight: 700;
            color: var(--bs-primary);
            margin-bottom: 1.5rem;
        }
        
        .price-period {
            font-size: 1rem;
            font-weight: 400;
            color: #6c757d;
        }
        
        .feature-list {
            list-style: none;
            padding: 0;
            margin-bottom: 2rem;
        }
        
        .feature-list li {
            padding: 0.5rem 0;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .feature-list .bi-check {
            color: #28a745;
            font-weight: bold;
        }
        
        .pricing-card.featured .feature-list .bi-check {
            color: white;
        }
        
        /* Contact Section */
        .contact-section {
            background: var(--primary-gradient);
            color: white;
        }
        
        .contact-form {
            background: rgba(255, 255, 255, 0.1);
            backdrop-filter: blur(10px);
            border-radius: var(--border-radius);
            padding: 2rem;
            border: 1px solid rgba(255, 255, 255, 0.2);
        }
        
        .form-control {
            background: rgba(255, 255, 255, 0.1);
            border: 1px solid rgba(255, 255, 255, 0.3);
            color: white;
            border-radius: 8px;
        }
        
        .form-control::placeholder {
            color: rgba(255, 255, 255, 0.7);
        }
        
        .form-control:focus {
            background: rgba(255, 255, 255, 0.2);
            border-color: rgba(255, 255, 255, 0.5);
            box-shadow: 0 0 0 0.2rem rgba(255, 255, 255, 0.25);
            color: white;
        }
        
        /* Footer */
        .footer {
            background: #212529;
            color: white;
        }
        
        .footer-link {
            color: #adb5bd;
            text-decoration: none;
            transition: var(--transition);
        }
        
        .footer-link:hover {
            color: white;
        }
        
        .social-icon {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.1);
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            text-decoration: none;
            transition: var(--transition);
        }
        
        .social-icon:hover {
            background: var(--bs-primary);
            color: white;
            transform: translateY(-2px);
        }
        
        /* Animations */
        .animate-on-scroll {
            opacity: 0;
            transform: translateY(30px);
            transition: all 0.8s ease-out;
        }
        
        .animate-on-scroll.animated {
            opacity: 1;
            transform: translateY(0);
        }
        
        /* Responsive adjustments */
        @media (max-width: 768px) {
            .hero-section {
                min-height: 80vh;
            }
            
            .pricing-card.featured {
                transform: none;
                margin-bottom: 2rem;
            }
            
            .stat-number {
                font-size: 2rem;
            }
        }
        
        /* Loading animation */
        .loading-spinner {
            display: none;
            width: 20px;
            height: 20px;
            border: 2px solid transparent;
            border-top: 2px solid currentColor;
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        
        .btn.loading .loading-spinner {
            display: inline-block;
        }
        
        .btn.loading .btn-text {
            opacity: 0;
        }
    </style>
</head>
<body>
    <!-- Navigation -->
    <nav class="navbar navbar-expand-lg navbar-dark fixed-top" style="background: rgba(0,0,0,0.1); backdrop-filter: blur(10px);">
        <div class="container">
            <a class="navbar-brand fw-bold fs-3" href="#">
                🚀 BootstrapPro
            </a>
            
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" 
                    data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item">
                        <a class="nav-link" href="#features">Features</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#testimonials">Testimonials</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#pricing">Pricing</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#contact">Contact</a>
                    </li>
                </ul>
                
                <div class="d-flex gap-2">
                    <button class="btn btn-outline-light">Login</button>
                    <button class="btn btn-light">Sign Up</button>
                </div>
            </div>
        </div>
    </nav>
    
    <!-- Hero Section -->
    <section class="hero-section d-flex align-items-center">
        <div class="container hero-content">
            <div class="row align-items-center">
                <div class="col-lg-6">
                    <h1 class="hero-title text-white animate-on-scroll">
                        Master Bootstrap Development in 56 Days
                    </h1>
                    <p class="hero-subtitle text-white animate-on-scroll">
                        Transform your web development skills with our comprehensive Bootstrap 
                        learning platform. From beginner to expert with hands-on projects, 
                        real-world examples, and expert guidance.
                    </p>
                    
                    <div class="d-flex flex-column flex-sm-row gap-3 animate-on-scroll">
                        <button class="cta-button btn btn-lg text-white">
                            <i class="bi bi-play-fill me-2"></i>
                            Start Your Journey
                        </button>
                        <button class="btn btn-outline-light btn-lg">
                            <i class="bi bi-download me-2"></i>
                            Download Curriculum
                        </button>
                    </div>
                    
                    <div class="mt-4 animate-on-scroll">
                        <small class="text-white opacity-75">
                            <i class="bi bi-check-circle me-2"></i>
                            56 comprehensive lessons • 
                            <i class="bi bi-check-circle me-2"></i>
                            Hands-on projects • 
                            <i class="bi bi-check-circle me-2"></i>
                            Expert support
                        </small>
                    </div>
                </div>
                
                <div class="col-lg-6 text-center animate-on-scroll">
                    <div class="hero-image mt-4 mt-lg-0">
                        <img src="https://via.placeholder.com/600x400/667eea/ffffff?text=Bootstrap+Learning+Platform" 
                             class="img-fluid hero-image" 
                             alt="Bootstrap Learning Platform Dashboard"
                             loading="lazy">
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Stats Section -->
        <div class="stats-section position-absolute bottom-0 w-100 py-4">
            <div class="container">
                <div class="row text-center">
                    <div class="col-6 col-md-3">
                        <div class="stat-item">
                            <div class="stat-number" data-count="10000">0</div>
                            <div class="stat-label">Students</div>
                        </div>
                    </div>
                    <div class="col-6 col-md-3">
                        <div class="stat-item">
                            <div class="stat-number" data-count="56">0</div>
                            <div class="stat-label">Lessons</div>
                        </div>
                    </div>
                    <div class="col-6 col-md-3">
                        <div class="stat-item">
                            <div class="stat-number" data-count="98">0</div>
                            <div class="stat-label">% Success Rate</div>
                        </div>
                    </div>
                    <div class="col-6 col-md-3">
                        <div class="stat-item">
                            <div class="stat-number" data-count="24">0</div>
                            <div class="stat-label">7 Support</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    
    <!-- Features Section -->
    <section id="features" class="py-5" style="padding-top: 8rem !important;">
        <div class="container">
            <div class="row">
                <div class="col-12 text-center animate-on-scroll">
                    <h2 class="section-title">Why Choose BootstrapPro?</h2>
                    <p class="section-subtitle">
                        Everything you need to become a Bootstrap expert, all in one place
                    </p>
                </div>
            </div>
            
            <div class="row g-4">
                <div class="col-md-6 col-lg-4">
                    <div class="feature-card animate-on-scroll">
                        <div class="feature-icon">
                            <i class="bi bi-laptop"></i>
                        </div>
                        <h3 class="feature-title">Hands-On Learning</h3>
                        <p class="feature-description">
                            Learn by building real projects. Each lesson includes practical 
                            exercises and live coding examples to reinforce your understanding.
                        </p>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="feature-card animate-on-scroll">
                        <div class="feature-icon bg-success">
                            <i class="bi bi-phone"></i>
                        </div>
                        <h3 class="feature-title">Mobile-First Approach</h3>
                        <p class="feature-description">
                            Master responsive design from the ground up. Learn to create 
                            websites that look perfect on any device, from mobile to desktop.
                        </p>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="feature-card animate-on-scroll">
                        <div class="feature-icon bg-warning">
                            <i class="bi bi-speedometer2"></i>
                        </div>
                        <h3 class="feature-title">Performance Optimization</h3>
                        <p class="feature-description">
                            Learn advanced techniques to optimize your Bootstrap sites for 
                            lightning-fast loading times and exceptional user experiences.
                        </p>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="feature-card animate-on-scroll">
                        <div class="feature-icon bg-info">
                            <i class="bi bi-people"></i>
                        </div>
                        <h3 class="feature-title">Expert Community</h3>
                        <p class="feature-description">
                            Join thousands of developers in our supportive community. Get help, 
                            share projects, and collaborate with fellow learners.
                        </p>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="feature-card animate-on-scroll">
                        <div class="feature-icon bg-danger">
                            <i class="bi bi-award"></i>
                        </div>
                        <h3 class="feature-title">Industry Recognition</h3>
                        <p class="feature-description">
                            Earn certificates and build a portfolio that showcases your 
                            Bootstrap expertise to potential employers and clients.
                        </p>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="feature-card animate-on-scroll">
                        <div class="feature-icon bg-secondary">
                            <i class="bi bi-clock"></i>
                        </div>
                        <h3 class="feature-title">Lifetime Access</h3>
                        <p class="feature-description">
                            Get lifetime access to all course materials, updates, and new 
                            content. Learn at your own pace, whenever and wherever you want.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </section>
    
    <!-- Testimonials Section -->
    <section id="testimonials" class="testimonials-section py-5">
        <div class="container">
            <div class="row">
                <div class="col-12 text-center animate-on-scroll">
                    <h2 class="section-title">What Our Students Say</h2>
                    <p class="section-subtitle">
                        Join thousands of developers who have transformed their careers
                    </p>
                </div>
            </div>
            
            <div class="row g-4">
                <div class="col-md-6 col-lg-4">
                    <div class="testimonial-card animate-on-scroll">
                        <p class="testimonial-text">
                            "BootstrapPro completely changed how I approach web development. 
                            The hands-on projects and clear explanations made complex concepts 
                            easy to understand. I landed my dream job within 3 months!"
                        </p>
                        <div class="testimonial-author">
                            <img src="https://via.placeholder.com/60x60/28a745/ffffff?text=SM" 
                                 alt="Sarah Martinez" class="author-avatar">
                            <div class="author-info">
                                <h6>Sarah Martinez</h6>
                                <small>Frontend Developer at TechCorp</small>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="testimonial-card animate-on-scroll">
                        <p class="testimonial-text">
                            "The structured curriculum and real-world projects helped me 
                            transition from backend to full-stack development. The community 
                            support is incredible - always someone ready to help!"
                        </p>
                        <div class="testimonial-author">
                            <img src="https://via.placeholder.com/60x60/007bff/ffffff?text=MJ" 
                                 alt="Michael Johnson" class="author-avatar">
                            <div class="author-info">
                                <h6>Michael Johnson</h6>
                                <small>Full-Stack Developer at StartupXYZ</small>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="testimonial-card animate-on-scroll">
                        <p class="testimonial-text">
                            "As a freelancer, Bootstrap skills are essential. This course 
                            taught me advanced techniques that helped me deliver better 
                            projects faster. My client satisfaction rates are through the roof!"
                        </p>
                        <div class="testimonial-author">
                            <img src="https://via.placeholder.com/60x60/dc3545/ffffff?text=EW" 
                                 alt="Emily Wilson" class="author-avatar">
                            <div class="author-info">
                                <h6>Emily Wilson</h6>
                                <small>Freelance Web Developer</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    
    <!-- Pricing Section -->
    <section id="pricing" class="py-5">
        <div class="container">
            <div class="row">
                <div class="col-12 text-center animate-on-scroll">
                    <h2 class="section-title">Choose Your Learning Path</h2>
                    <p class="section-subtitle">
                        Flexible pricing options to fit your needs and budget
                    </p>
                </div>
            </div>
            
            <div class="row g-4 justify-content-center">
                <div class="col-md-6 col-lg-4">
                    <div class="pricing-card animate-on-scroll">
                        <h3 class="plan-name">Starter</h3>
                        <p class="plan-description">Perfect for beginners getting started</p>
                        <div class="price">
                            $29
                            <span class="price-period">/month</span>
                        </div>
                        <ul class="feature-list">
                            <li><i class="bi bi-check"></i> Bootstrap Basics (Days 1-14)</li>
                            <li><i class="bi bi-check"></i> Community Support</li>
                            <li><i class="bi bi-check"></i> Basic Projects</li>
                            <li><i class="bi bi-check"></i> Course Materials</li>
                            <li><i class="bi bi-check"></i> Mobile Learning App</li>
                        </ul>
                        <button class="btn btn-outline-primary w-100">
                            Get Started
                        </button>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="pricing-card featured animate-on-scroll">
                        <div class="popular-badge">Most Popular</div>
                        <h3 class="plan-name">Professional</h3>
                        <p class="plan-description">Complete Bootstrap mastery program</p>
                        <div class="price">
                            $59
                            <span class="price-period">/month</span>
                        </div>
                        <ul class="feature-list">
                            <li><i class="bi bi-check"></i> Complete 56-Day Curriculum</li>
                            <li><i class="bi bi-check"></i> Advanced Components</li>
                            <li><i class="bi bi-check"></i> Real-World Projects</li>
                            <li><i class="bi bi-check"></i> 1-on-1 Mentorship</li>
                            <li><i class="bi bi-check"></i> Certificate of Completion</li>
                            <li><i class="bi bi-check"></i> Lifetime Updates</li>
                        </ul>
                        <button class="btn btn-light w-100">
                            Start Professional
                        </button>
                    </div>
                </div>
                
                <div class="col-md-6 col-lg-4">
                    <div class="pricing-card animate-on-scroll">
                        <h3 class="plan-name">Enterprise</h3>
                        <p class="plan-description">For teams and organizations</p>
                        <div class="price">
                            $99
                            <span class="price-period">/month</span>
                        </div>
                        <ul class="feature-list">
                            <li><i class="bi bi-check"></i> Everything in Professional</li>
                            <li><i class="bi bi-check"></i> Team Management Dashboard</li>
                            <li><i class="bi bi-check"></i> Custom Learning Paths</li>
                            <li><i class="bi bi-check"></i> Priority Support</li>
                            <li><i class="bi bi-check"></i> White-label Solutions</li>
                            <li><i class="bi bi-check"></i> Advanced Analytics</li>
                        </ul>
                        <button class="btn btn-outline-primary w-100">
                            Contact Sales
                        </button>
                    </div>
                </div>
            </div>
            
            <div class="row mt-5">
                <div class="col-12 text-center animate-on-scroll">
                    <p class="text-muted">
                        <i class="bi bi-shield-check me-2"></i>
                        30-day money-back guarantee • 
                        <i class="bi bi-credit-card me-2"></i>
                        Secure payment • 
                        <i class="bi bi-arrow-clockwise me-2"></i>
                        Cancel anytime
                    </p>
                </div>
            </div>
        </div>
    </section>
    
    <!-- Contact Section -->
    <section id="contact" class="contact-section py-5">
        <div class="container">
            <div class="row">
                <div class="col-12 text-center animate-on-scroll">
                    <h2 class="section-title text-white">Ready to Get Started?</h2>
                    <p class="section-subtitle text-white opacity-75">
                        Have questions? We're here to help you on your Bootstrap journey
                    </p>
                </div>
            </div>
            
            <div class="row justify-content-center">
                <div class="col-lg-8">
                    <div class="contact-form animate-on-scroll">
                        <form id="contactForm">
                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label for="firstName" class="form-label">First Name</label>
                                    <input type="text" class="form-control" id="firstName" 
                                           placeholder="Your first name" required>
                                </div>
                                <div class="col-md-6">
                                    <label for="lastName" class="form-label">Last Name</label>
                                    <input type="text" class="form-control" id="lastName" 
                                           placeholder="Your last name" required>
                                </div>
                                <div class="col-12">
                                    <label for="email" class="form-label">Email Address</label>
                                    <input type="email" class="form-control" id="email" 
                                           placeholder="your.email@example.com" required>
                                </div>
                                <div class="col-12">
                                    <label for="interest" class="form-label">I'm interested in</label>
                                    <select class="form-select" id="interest" required>
                                        <option value="">Choose your interest...</option>
                                        <option value="starter">Starter Plan</option>
                                        <option value="professional">Professional Plan</option>
                                        <option value="enterprise">Enterprise Plan</option>
                                        <option value="custom">Custom Solution</option>
                                    </select>
                                </div>
                                <div class="col-12">
                                    <label for="message" class="form-label">Message</label>
                                    <textarea class="form-control" id="message" rows="4" 
                                              placeholder="Tell us about your goals and how we can help..."></textarea>
                                </div>
                                <div class="col-12">
                                    <div class="form-check">
                                        <input class="form-check-input" type="checkbox" id="newsletter" checked>
                                        <label class="form-check-label" for="newsletter">
                                            Yes, I'd like to receive updates about new courses and features
                                        </label>
                                    </div>
                                </div>
                                <div class="col-12">
                                    <button type="submit" class="btn btn-light btn-lg w-100">
                                        <span class="btn-text">
                                            <i class="bi bi-send me-2"></i>
                                            Send Message
                                        </span>
                                        <span class="loading-spinner ms-2"></span>
                                    </button>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </section>
    
    <!-- Footer -->
    <footer class="footer py-5">
        <div class="container">
            <div class="row g-4">
                <div class="col-lg-4">
                    <h3 class="fw-bold mb-3">🚀 BootstrapPro</h3>
                    <p class="text-muted">
                        Empowering developers worldwide with comprehensive Bootstrap education. 
                        From beginner to expert in 56 days.
                    </p>
                    <div class="d-flex gap-3">
                        <a href="#" class="social-icon">
                            <i class="bi bi-facebook"></i>
                        </a>
                        <a href="#" class="social-icon">
                            <i class="bi bi-twitter"></i>
                        </a>
                        <a href="#" class="social-icon">
                            <i class="bi bi-linkedin"></i>
                        </a>
                        <a href="#" class="social-icon">
                            <i class="bi bi-youtube"></i>
                        </a>
                        <a href="#" class="social-icon">
                            <i class="bi bi-github"></i>
                        </a>
                    </div>
                </div>
                
                <div class="col-lg-2 col-md-6">
                    <h5 class="fw-bold mb-3">Courses</h5>
                    <ul class="list-unstyled">
                        <li class="mb-2"><a href="#" class="footer-link">Bootstrap Basics</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Advanced Components</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Real Projects</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Performance</a></li>
                    </ul>
                </div>
                
                <div class="col-lg-2 col-md-6">
                    <h5 class="fw-bold mb-3">Resources</h5>
                    <ul class="list-unstyled">
                        <li class="mb-2"><a href="#" class="footer-link">Documentation</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Examples</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Templates</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Community</a></li>
                    </ul>
                </div>
                
                <div class="col-lg-2 col-md-6">
                    <h5 class="fw-bold mb-3">Support</h5>
                    <ul class="list-unstyled">
                        <li class="mb-2"><a href="#" class="footer-link">Help Center</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Contact Us</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Status</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">FAQ</a></li>
                    </ul>
                </div>
                
                <div class="col-lg-2 col-md-6">
                    <h5 class="fw-bold mb-3">Company</h5>
                    <ul class="list-unstyled">
                        <li class="mb-2"><a href="#" class="footer-link">About</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Blog</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Careers</a></li>
                        <li class="mb-2"><a href="#" class="footer-link">Press</a></li>
                    </ul>
                </div>
            </div>
            
            <hr class="my-4">
            
            <div class="row align-items-center">
                <div class="col-md-6">
                    <p class="mb-0 text-muted">
                        &copy; 2025 BootstrapPro. All rights reserved.
                    </p>
                </div>
                <div class="col-md-6 text-md-end">
                    <div class="d-flex justify-content-md-end gap-3">
                        <a href="#" class="footer-link">Privacy Policy</a>
                        <a href="#" class="footer-link">Terms of Service</a>
                        <a href="#" class="footer-link">Cookies</a>
                    </div>
                </div>
            </div>
        </div>
    </footer>
    
    <!-- Back to Top Button -->
    <button id="backToTop" class="btn btn-primary position-fixed bottom-0 end-0 m-3 rounded-circle" 
            style="display: none; z-index: 1000; width: 50px; height: 50px;">
        <i class="bi bi-arrow-up"></i>
    </button>
    
    <!-- Bootstrap JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <!-- Custom JavaScript -->
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // Smooth scrolling for navigation links
            document.querySelectorAll('a[href^="#"]').forEach(anchor => {
                anchor.addEventListener('click', function (e) {
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
            
            // Navbar background on scroll
            const navbar = document.querySelector('.navbar');
            window.addEventListener('scroll', function() {
                if (window.scrollY > 100) {
                    navbar.style.background = 'rgba(0,0,0,0.9)';
                } else {
                    navbar.style.background = 'rgba(0,0,0,0.1)';
                }
            });
            
            // Animate elements on scroll
            const observerOptions = {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            };
            
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('animated');
                    }
                });
            }, observerOptions);
            
            document.querySelectorAll('.animate-on-scroll').forEach(el => {
                observer.observe(el);
            });
            
            // Counter animation
            function animateCounter(element, target) {
                let current = 0;
                const increment = target / 100;
                const timer = setInterval(() => {
                    current += increment;
                    if (current >= target) {
                        current = target;
                        clearInterval(timer);
                    }
                    element.textContent = Math.floor(current);
                }, 20);
            }
            
            // Start counter animation when stats section is visible
            const statsObserver = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        document.querySelectorAll('[data-count]').forEach(counter => {
                            const target = parseInt(counter.dataset.count);
                            animateCounter(counter, target);
                        });
                        statsObserver.unobserve(entry.target);
                    }
                });
            });
            
            const statsSection = document.querySelector('.stats-section');
            if (statsSection) {
                statsObserver.observe(statsSection);
            }
            
            // Contact form handling
            const contactForm = document.getElementById('contactForm');
            contactForm.addEventListener('submit', function(e) {
                e.preventDefault();
                
                const submitBtn = this.querySelector('button[type="submit"]');
                submitBtn.classList.add('loading');
                
                // Simulate form submission
                setTimeout(() => {
                    submitBtn.classList.remove('loading');
                    
                    // Show success message
                    const alert = document.createElement('div');
                    alert.className = 'alert alert-success mt-3';
                    alert.innerHTML = `
                        <i class="bi bi-check-circle me-2"></i>
                        Thank you for your message! We'll get back to you within 24 hours.
                    `;
                    
                    contactForm.appendChild(alert);
                    contactForm.reset();
                    
                    // Remove alert after 5 seconds
                    setTimeout(() => {
                        alert.remove();
                    }, 5000);
                }, 2000);
            });
            
            // Back to top button
            const backToTopBtn = document.getElementById('backToTop');
            
            window.addEventListener('scroll', function() {
                if (window.scrollY > 300) {
                    backToTopBtn.style.display = 'block';
                } else {
                    backToTopBtn.style.display = 'none';
                }
            });
            
            backToTopBtn.addEventListener('click', function() {
                window.scrollTo({
                    top: 0,
                    behavior: 'smooth'
                });
            });
            
            // Pricing card hover effects
            document.querySelectorAll('.pricing-card').forEach(card => {
                card.addEventListener('mouseenter', function() {
                    if (!this.classList.contains('featured')) {
                        this.style.transform = 'translateY(-10px) scale(1.02)';
                    }
                });
                
                card.addEventListener('mouseleave', function() {
                    if (!this.classList.contains('featured')) {
                        this.style.transform = 'translateY(0) scale(1)';
                    }
                });
            });
            
            // Feature card stagger animation
            const featureCards = document.querySelectorAll('.feature-card');
            featureCards.forEach((card, index) => {
                card.style.animationDelay = `${index * 0.1}s`;
            });
            
            // Testimonial card stagger animation  
            const testimonialCards = document.querySelectorAll('.testimonial-card');
            testimonialCards.forEach((card, index) => {
                card.style.animationDelay = `${index * 0.2}s`;
            });
            
            console.log('BootstrapPro Landing Page Loaded! 🚀');
        });
    </script>
    
    <!-- Schema.org structured data for SEO -->
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "EducationalOrganization",
        "name": "BootstrapPro",
        "description": "Master Bootstrap Development in 56 Days - Complete learning platform for web developers",
        "url": "https://bootstrappro.dev",
        "logo": "https://bootstrappro.dev/logo.png",
        "foundingDate": "2025",
        "address": {
            "@type": "PostalAddress",
            "addressCountry": "US"
        },
        "contactPoint": {
            "@type": "ContactPoint",
            "telephone": "+1-555-0123",
            "contactType": "customer support"
        },
        "offers": [{
            "@type": "Offer",
            "name": "Professional Plan",
            "price": "59",
            "priceCurrency": "USD",
            "description": "Complete 56-Day Bootstrap Curriculum"
        }]
    }
    </script>
</body>
</html>
```

## 📋 Landing Page Project Checklist

### Design & UX
- [ ] Compelling hero section with clear value proposition
- [ ] Logical content flow and visual hierarchy
- [ ] Consistent branding and color scheme
- [ ] Professional typography and spacing
- [ ] Engaging animations and micro-interactions

### Responsive Design
- [ ] Mobile-first approach implemented
- [ ] Perfect layout across all breakpoints
- [ ] Touch-friendly interface elements
- [ ] Optimized images for different screen sizes
- [ ] Accessible navigation on all devices

### Performance
- [ ] Optimized images and assets
- [ ] Minimal CSS and JavaScript bundles
- [ ] Fast loading times (< 3 seconds)
- [ ] Efficient animations and transitions
- [ ] CDN implementation for static assets

### SEO & Accessibility
- [ ] Semantic HTML structure
- [ ] Proper meta tags and descriptions
- [ ] Alt text for all images
- [ ] ARIA labels where needed
- [ ] Keyboard navigation support
- [ ] Schema.org structured data

### Functionality
- [ ] Working contact form with validation
- [ ] Smooth scroll navigation
- [ ] Interactive elements (buttons, cards)
- [ ] Error handling and user feedback
- [ ] Cross-browser compatibility

## 🎉 Day 14 Complete - Bootstrap Basics Mastery!

### ✅ Project Achievements
- **Complete Landing Page:** Professional SaaS product landing page built from scratch
- **Responsive Design:** Flawless experience across all devices and screen sizes
- **Modern UI/UX:** Contemporary design with smooth animations and interactions
- **Performance Optimized:** Fast-loading, production-ready website
- **SEO Friendly:** Properly structured for search engines and social sharing
- **Accessible:** WCAG 2.1 AA compliant with keyboard navigation support

### 🏆 Bootstrap Basics Phase Complete!

**Congratulations!** You've successfully completed the Bootstrap Basics phase (Days 8-14) and built:

#### Days 8-14 Summary:
- **Day 08:** Bootstrap Setup & Project Structure
- **Day 09:** Grid System & Responsive Layouts  
- **Day 10:** Typography & Colors Mastery
- **Day 11:** Flexbox & Utilities Power
- **Day 12:** Responsive Design Patterns
- **Day 13:** Performance & Optimization
- **Day 14:** Complete Landing Page Project

### 🚀 What You've Mastered:
- Bootstrap framework fundamentals and setup
- Complete grid system and responsive breakpoints
- Typography system and color utilities
- Flexbox utilities and layout control
- Mobile-first responsive design approach
- Performance optimization techniques
- Real-world project development

## 🔗 Additional Resources

- **[Bootstrap Official Docs](https://getbootstrap.com/docs/5.3/)** - Complete framework reference
- **[Bootstrap Examples](https://getbootstrap.com/docs/5.3/examples/)** - Official example templates
- **[Bootstrap Icons](https://icons.getbootstrap.com/)** - Complete icon library
- **[Can I Use Bootstrap](https://caniuse.com/?search=bootstrap)** - Browser compatibility info

---

## 🎯 What's Next?

**Bootstrap Components Phase (Days 15-28)** awaits! You'll master:
- Buttons, Cards, and Navigation components
- Modals, Forms, and Interactive elements
- Advanced components like Carousel and Accordion
- Custom component creation and styling

**You've built a solid foundation!** Your Bootstrap Basics knowledge is now complete, and you're ready to dive into the exciting world of Bootstrap components.

---

**Remember: You've just built a production-ready landing page using only Bootstrap fundamentals - that's the power of what you've learned!** 🚀✨