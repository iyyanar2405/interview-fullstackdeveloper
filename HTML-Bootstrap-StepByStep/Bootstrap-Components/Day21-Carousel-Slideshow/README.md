# Day 21: Carousel & Slideshow Components 🎠

Welcome to Day 21! Today you'll master Bootstrap's powerful carousel component to create stunning image sliders, product showcases, testimonial rotators, and dynamic content presentations with smooth transitions and interactive controls.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master Bootstrap's carousel component structure
- Create image sliders and product showcases
- Implement custom carousel controls and indicators
- Build testimonial and content carousels
- Add captions, overlays, and custom animations
- Optimize carousel performance and accessibility
- Create responsive carousels for all devices
- Integrate carousels with JavaScript for advanced features

## 📚 Bootstrap Carousel System Deep Dive

### Carousel Fundamentals

The Bootstrap carousel is a slideshow component for cycling through elements—images or slides of text—like a carousel.

### Core Carousel Classes
```css
/* Base Carousel */
.carousel                    /* Base carousel wrapper */
.carousel-inner              /* Container for slides */
.carousel-item               /* Individual slide */
.carousel-item.active        /* Current active slide */

/* Controls */
.carousel-control-prev       /* Previous button */
.carousel-control-next       /* Next button */
.carousel-control-prev-icon  /* Previous icon */
.carousel-control-next-icon  /* Next icon */

/* Indicators */
.carousel-indicators         /* Slide indicator dots */

/* Captions */
.carousel-caption            /* Caption overlay on slides */

/* Options */
.carousel-fade               /* Fade transition instead of slide */
.carousel-dark               /* Dark variant for light backgrounds */
```

## 💻 Comprehensive Carousel Components

### Complete Carousel Showcase

Create `carousel-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 21: Bootstrap Carousel & Slideshow | Dynamic Content Presentations</title>
    
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css">
    
    <style>
        :root {
            --primary-gradient: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
            --card-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
            --border-radius: 15px;
            --transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            line-height: 1.6;
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
            top: -50%;
            right: -10%;
            width: 500px;
            height: 500px;
            background: rgba(255, 255, 255, 0.1);
            border-radius: 50%;
            animation: float 6s ease-in-out infinite;
        }
        
        @keyframes float {
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-20px); }
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
        }
        
        .demo-title {
            color: #1e293b;
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
        }
        
        /* Enhanced Carousel Styles */
        .carousel {
            border-radius: var(--border-radius);
            overflow: hidden;
            box-shadow: var(--card-shadow);
        }
        
        .carousel-item {
            height: 500px;
            background: #f8f9fa;
            position: relative;
        }
        
        .carousel-item img {
            object-fit: cover;
            width: 100%;
            height: 100%;
        }
        
        .carousel-caption {
            background: rgba(0, 0, 0, 0.6);
            backdrop-filter: blur(10px);
            padding: 2rem;
            border-radius: var(--border-radius);
            bottom: 2rem;
            left: 50%;
            transform: translateX(-50%);
            width: 80%;
        }
        
        .carousel-control-prev,
        .carousel-control-next {
            width: 60px;
            height: 60px;
            background: rgba(255, 255, 255, 0.9);
            border-radius: 50%;
            top: 50%;
            transform: translateY(-50%);
            opacity: 0.8;
            transition: var(--transition);
        }
        
        .carousel-control-prev {
            left: 20px;
        }
        
        .carousel-control-next {
            right: 20px;
        }
        
        .carousel-control-prev:hover,
        .carousel-control-next:hover {
            opacity: 1;
            transform: translateY(-50%) scale(1.1);
        }
        
        .carousel-control-prev-icon,
        .carousel-control-next-icon {
            filter: invert(1);
        }
        
        .carousel-indicators [data-bs-target] {
            width: 12px;
            height: 12px;
            border-radius: 50%;
            margin: 0 5px;
            border: 2px solid white;
            background-color: rgba(255, 255, 255, 0.5);
        }
        
        .carousel-indicators .active {
            background-color: white;
        }
        
        /* Testimonial Carousel */
        .testimonial-carousel .carousel-item {
            height: auto;
            min-height: 300px;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 3rem;
        }
        
        .testimonial-card {
            background: white;
            padding: 2rem;
            border-radius: var(--border-radius);
            text-align: center;
            max-width: 600px;
        }
        
        .testimonial-avatar {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            margin: 0 auto 1rem;
            border: 4px solid #6366f1;
        }
        
        .testimonial-text {
            font-size: 1.1rem;
            font-style: italic;
            color: #475569;
            margin-bottom: 1rem;
        }
        
        .testimonial-author {
            font-weight: 600;
            color: #1e293b;
        }
        
        .testimonial-role {
            color: #64748b;
            font-size: 0.9rem;
        }
        
        /* Product Carousel */
        .product-carousel .carousel-item {
            height: 400px;
            padding: 2rem;
        }
        
        .product-card {
            background: white;
            border-radius: var(--border-radius);
            padding: 2rem;
            text-align: center;
            height: 100%;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }
        
        .product-image {
            max-height: 200px;
            object-fit: contain;
            margin-bottom: 1rem;
        }
        
        /* Thumbnail Carousel */
        .thumbnail-carousel {
            margin-top: 1rem;
        }
        
        .thumbnail-item {
            cursor: pointer;
            opacity: 0.6;
            transition: var(--transition);
            border: 3px solid transparent;
            border-radius: 8px;
        }
        
        .thumbnail-item:hover,
        .thumbnail-item.active {
            opacity: 1;
            border-color: #6366f1;
        }
        
        @media (max-width: 768px) {
            .carousel-item {
                height: 300px;
            }
            
            .carousel-caption {
                width: 90%;
                padding: 1rem;
            }
            
            .carousel-control-prev,
            .carousel-control-next {
                width: 40px;
                height: 40px;
            }
        }
    </style>
</head>
<body>
    <header class="section-header">
        <div class="container">
            <div class="content text-center">
                <h1 class="display-4 fw-bold mb-3">Day 21: Carousel & Slideshow</h1>
                <p class="lead mb-0">Create stunning, dynamic content presentations with Bootstrap carousels</p>
            </div>
        </div>
    </header>

    <div class="container py-4">
        
        <!-- Basic Carousel -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-images me-2"></i>
                Basic Image Carousel
            </h2>
            <p class="text-muted mb-4">A classic image slider with captions, controls, and indicators.</p>
            
            <div id="heroCarousel" class="carousel slide" data-bs-ride="carousel">
                <div class="carousel-indicators">
                    <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="0" class="active"></button>
                    <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="1"></button>
                    <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="2"></button>
                </div>
                
                <div class="carousel-inner">
                    <div class="carousel-item active">
                        <svg width="100%" height="500" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="xMidYMid slice">
                            <defs>
                                <linearGradient id="grad1" x1="0%" y1="0%" x2="100%" y2="100%">
                                    <stop offset="0%" style="stop-color:#667eea;stop-opacity:1" />
                                    <stop offset="100%" style="stop-color:#764ba2;stop-opacity:1" />
                                </linearGradient>
                            </defs>
                            <rect width="100%" height="100%" fill="url(#grad1)"/>
                            <text x="50%" y="50%" fill="white" font-size="48" text-anchor="middle" dy=".3em">Slide 1</text>
                        </svg>
                        <div class="carousel-caption">
                            <h3>First Slide Label</h3>
                            <p>Some representative placeholder content for the first slide.</p>
                        </div>
                    </div>
                    
                    <div class="carousel-item">
                        <svg width="100%" height="500" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="xMidYMid slice">
                            <defs>
                                <linearGradient id="grad2" x1="0%" y1="0%" x2="100%" y2="100%">
                                    <stop offset="0%" style="stop-color:#f093fb;stop-opacity:1" />
                                    <stop offset="100%" style="stop-color:#f5576c;stop-opacity:1" />
                                </linearGradient>
                            </defs>
                            <rect width="100%" height="100%" fill="url(#grad2)"/>
                            <text x="50%" y="50%" fill="white" font-size="48" text-anchor="middle" dy=".3em">Slide 2</text>
                        </svg>
                        <div class="carousel-caption">
                            <h3>Second Slide Label</h3>
                            <p>Some representative placeholder content for the second slide.</p>
                        </div>
                    </div>
                    
                    <div class="carousel-item">
                        <svg width="100%" height="500" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="xMidYMid slice">
                            <defs>
                                <linearGradient id="grad3" x1="0%" y1="0%" x2="100%" y2="100%">
                                    <stop offset="0%" style="stop-color:#4facfe;stop-opacity:1" />
                                    <stop offset="100%" style="stop-color:#00f2fe;stop-opacity:1" />
                                </linearGradient>
                            </defs>
                            <rect width="100%" height="100%" fill="url(#grad3)"/>
                            <text x="50%" y="50%" fill="white" font-size="48" text-anchor="middle" dy=".3em">Slide 3</text>
                        </svg>
                        <div class="carousel-caption">
                            <h3>Third Slide Label</h3>
                            <p>Some representative placeholder content for the third slide.</p>
                        </div>
                    </div>
                </div>
                
                <button class="carousel-control-prev" type="button" data-bs-target="#heroCarousel" data-bs-slide="prev">
                    <span class="carousel-control-prev-icon"></span>
                    <span class="visually-hidden">Previous</span>
                </button>
                <button class="carousel-control-next" type="button" data-bs-target="#heroCarousel" data-bs-slide="next">
                    <span class="carousel-control-next-icon"></span>
                    <span class="visually-hidden">Next</span>
                </button>
            </div>
        </section>
        
        <!-- Testimonial Carousel -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-chat-quote me-2"></i>
                Testimonial Carousel
            </h2>
            <p class="text-muted mb-4">Showcase customer reviews and testimonials with elegant transitions.</p>
            
            <div id="testimonialCarousel" class="carousel slide testimonial-carousel" data-bs-ride="carousel">
                <div class="carousel-inner">
                    <div class="carousel-item active">
                        <div class="testimonial-card">
                            <svg class="testimonial-avatar" viewBox="0 0 100 100">
                                <circle cx="50" cy="50" r="48" fill="#6366f1"/>
                                <text x="50" y="60" fill="white" font-size="40" text-anchor="middle">JD</text>
                            </svg>
                            <p class="testimonial-text">"This product has completely transformed our workflow. Highly recommended!"</p>
                            <div class="testimonial-author">John Doe</div>
                            <div class="testimonial-role">CEO, Tech Company</div>
                        </div>
                    </div>
                    
                    <div class="carousel-item">
                        <div class="testimonial-card">
                            <svg class="testimonial-avatar" viewBox="0 0 100 100">
                                <circle cx="50" cy="50" r="48" fill="#8b5cf6"/>
                                <text x="50" y="60" fill="white" font-size="40" text-anchor="middle">JS</text>
                            </svg>
                            <p class="testimonial-text">"Amazing service and support. The team went above and beyond our expectations."</p>
                            <div class="testimonial-author">Jane Smith</div>
                            <div class="testimonial-role">Marketing Director</div>
                        </div>
                    </div>
                    
                    <div class="carousel-item">
                        <div class="testimonial-card">
                            <svg class="testimonial-avatar" viewBox="0 0 100 100">
                                <circle cx="50" cy="50" r="48" fill="#ec4899"/>
                                <text x="50" y="60" fill="white" font-size="40" text-anchor="middle">MB</text>
                            </svg>
                            <p class="testimonial-text">"The best investment we've made this year. Results exceeded our projections."</p>
                            <div class="testimonial-author">Mike Brown</div>
                            <div class="testimonial-role">Operations Manager</div>
                        </div>
                    </div>
                </div>
                
                <button class="carousel-control-prev" type="button" data-bs-target="#testimonialCarousel" data-bs-slide="prev">
                    <span class="carousel-control-prev-icon"></span>
                </button>
                <button class="carousel-control-next" type="button" data-bs-target="#testimonialCarousel" data-bs-slide="next">
                    <span class="carousel-control-next-icon"></span>
                </button>
            </div>
        </section>
        
        <!-- Product Showcase Carousel -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-cart me-2"></i>
                Product Showcase Carousel
            </h2>
            <p class="text-muted mb-4">Display multiple products with detailed information and call-to-action buttons.</p>
            
            <div id="productCarousel" class="carousel slide product-carousel" data-bs-ride="false">
                <div class="carousel-inner">
                    <div class="carousel-item active">
                        <div class="row g-4">
                            <div class="col-md-4">
                                <div class="product-card">
                                    <svg class="product-image" viewBox="0 0 200 200">
                                        <rect width="200" height="200" fill="#e0e7ff"/>
                                        <text x="100" y="110" fill="#6366f1" font-size="24" text-anchor="middle">Product 1</text>
                                    </svg>
                                    <h4>Premium Widget</h4>
                                    <p class="text-muted">High-quality widget for professionals</p>
                                    <h3 class="text-primary">$99.99</h3>
                                    <button class="btn btn-primary">Add to Cart</button>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="product-card">
                                    <svg class="product-image" viewBox="0 0 200 200">
                                        <rect width="200" height="200" fill="#fce7f3"/>
                                        <text x="100" y="110" fill="#ec4899" font-size="24" text-anchor="middle">Product 2</text>
                                    </svg>
                                    <h4>Smart Gadget</h4>
                                    <p class="text-muted">Innovative solution for everyday tasks</p>
                                    <h3 class="text-primary">$149.99</h3>
                                    <button class="btn btn-primary">Add to Cart</button>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="product-card">
                                    <svg class="product-image" viewBox="0 0 200 200">
                                        <rect width="200" height="200" fill="#ccfbf1"/>
                                        <text x="100" y="110" fill="#14b8a6" font-size="24" text-anchor="middle">Product 3</text>
                                    </svg>
                                    <h4>Pro Tool</h4>
                                    <p class="text-muted">Professional-grade equipment</p>
                                    <h3 class="text-primary">$199.99</h3>
                                    <button class="btn btn-primary">Add to Cart</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="carousel-item">
                        <div class="row g-4">
                            <div class="col-md-4">
                                <div class="product-card">
                                    <svg class="product-image" viewBox="0 0 200 200">
                                        <rect width="200" height="200" fill="#fef3c7"/>
                                        <text x="100" y="110" fill="#f59e0b" font-size="24" text-anchor="middle">Product 4</text>
                                    </svg>
                                    <h4>Essential Kit</h4>
                                    <p class="text-muted">Everything you need to get started</p>
                                    <h3 class="text-primary">$79.99</h3>
                                    <button class="btn btn-primary">Add to Cart</button>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="product-card">
                                    <svg class="product-image" viewBox="0 0 200 200">
                                        <rect width="200" height="200" fill="#ddd6fe"/>
                                        <text x="100" y="110" fill="#8b5cf6" font-size="24" text-anchor="middle">Product 5</text>
                                    </svg>
                                    <h4>Deluxe Package</h4>
                                    <p class="text-muted">Premium bundle with extras</p>
                                    <h3 class="text-primary">$299.99</h3>
                                    <button class="btn btn-primary">Add to Cart</button>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="product-card">
                                    <svg class="product-image" viewBox="0 0 200 200">
                                        <rect width="200" height="200" fill="#fecaca"/>
                                        <text x="100" y="110" fill="#ef4444" font-size="24" text-anchor="middle">Product 6</text>
                                    </svg>
                                    <h4>Ultimate Set</h4>
                                    <p class="text-muted">Complete professional solution</p>
                                    <h3 class="text-primary">$399.99</h3>
                                    <button class="btn btn-primary">Add to Cart</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <button class="carousel-control-prev" type="button" data-bs-target="#productCarousel" data-bs-slide="prev">
                    <span class="carousel-control-prev-icon"></span>
                </button>
                <button class="carousel-control-next" type="button" data-bs-target="#productCarousel" data-bs-slide="next">
                    <span class="carousel-control-next-icon"></span>
                </button>
            </div>
        </section>
        
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <script>
        // Auto-pause carousel on hover
        document.querySelectorAll('.carousel').forEach(carousel => {
            carousel.addEventListener('mouseenter', () => {
                const bsCarousel = bootstrap.Carousel.getInstance(carousel);
                if (bsCarousel) bsCarousel.pause();
            });
            
            carousel.addEventListener('mouseleave', () => {
                const bsCarousel = bootstrap.Carousel.getInstance(carousel);
                if (bsCarousel) bsCarousel.cycle();
            });
        });
        
        console.log('🎠 Day 21 - Carousel Components loaded successfully!');
    </script>
</body>
</html>
```

## 📋 Day 21 Carousel Checklist

- [ ] Basic carousel with slides
- [ ] Carousel controls (prev/next)
- [ ] Carousel indicators
- [ ] Carousel captions
- [ ] Fade transitions
- [ ] Auto-play configuration
- [ ] Pause on hover
- [ ] Testimonial carousel
- [ ] Product showcase carousel
- [ ] Responsive carousel designs
- [ ] Custom styling and animations
- [ ] Accessibility features (ARIA labels)

## 🎯 Day 21 Complete!

### ✅ Achievements:
- **Carousel Mastery:** Built various carousel types
- **Visual Impact:** Created engaging slideshow presentations
- **User Control:** Implemented intuitive navigation
- **Responsive Design:** Ensured mobile-friendly carousels
- **Accessibility:** Added proper ARIA labels

### 🔗 Resources:
- [Bootstrap Carousel Docs](https://getbootstrap.com/docs/5.3/components/carousel/)
- [Carousel Options](https://getbootstrap.com/docs/5.3/components/carousel/#options)

---

**Next: Day 22 - Accordion & Collapse Components** 📂
