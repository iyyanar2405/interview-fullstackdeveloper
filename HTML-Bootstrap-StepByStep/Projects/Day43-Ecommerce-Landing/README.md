# Day 43: E-Commerce Landing Page Project 🛒

Build a complete, professional e-commerce landing page with product catalog, shopping cart, and checkout experience.

## 🎯 Learning Objectives
- Create responsive e-commerce layouts
- Implement shopping cart functionality with JavaScript
- Build product grids with filters and sorting
- Design promotional banners and CTAs
- Create offcanvas shopping cart
- Implement product quick view modals
- Build category navigation
- Design checkout flow

## 🛍️ Project Features
- ✅ Sticky navigation with cart icon
- ✅ Hero banner with promotional content
- ✅ Category badges and filters
- ✅ Product grid with hover effects
- ✅ Product cards with ratings
- ✅ Shopping cart offcanvas
- ✅ Product quick view modal
- ✅ Featured products carousel
- ✅ Customer testimonials
- ✅ Newsletter subscription
- ✅ Footer with social links

## 💻 Complete E-Commerce Page

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>ShopHub - Your Online Store</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        .product-card { transition: transform 0.3s; }
        .product-card:hover { transform: translateY(-10px); box-shadow: 0 10px 30px rgba(0,0,0,0.1); }
        .badge-category { cursor: pointer; transition: all 0.3s; }
        .badge-category:hover { transform: scale(1.1); }
    </style>
</head>
<body>
    <!-- Top Bar -->
    <div class="bg-dark text-white py-2">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-md-6">
                    <small><i class="bi bi-truck me-2"></i>Free Shipping on Orders Over $50</small>
                </div>
                <div class="col-md-6 text-end">
                    <small class="me-3"><i class="bi bi-telephone me-1"></i>1-800-SHOPHUB</small>
                    <small><i class="bi bi-envelope me-1"></i>support@shophub.com</small>
                </div>
            </div>
        </div>
    </div>

    <!-- Navigation -->
    <nav class="navbar navbar-expand-lg navbar-light bg-white sticky-top shadow-sm">
        <div class="container">
            <a class="navbar-brand fw-bold fs-3 text-primary" href="#">
                <i class="bi bi-shop"></i> ShopHub
            </a>
            <button class="navbar-toggler" data-bs-toggle="collapse" data-bs-target="#navMenu">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navMenu">
                <ul class="navbar-nav mx-auto">
                    <li class="nav-item"><a class="nav-link active" href="#home">Home</a></li>
                    <li class="nav-item"><a class="nav-link" href="#products">Products</a></li>
                    <li class="nav-item"><a class="nav-link" href="#deals">Deals</a></li>
                    <li class="nav-item"><a class="nav-link" href="#about">About</a></li>
                    <li class="nav-item"><a class="nav-link" href="#contact">Contact</a></li>
                </ul>
                <div class="d-flex align-items-center gap-3">
                    <button class="btn btn-outline-primary position-relative" data-bs-toggle="offcanvas" data-bs-target="#cartOffcanvas">
                        <i class="bi bi-cart3"></i>
                        <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" id="cartCount">3</span>
                    </button>
                    <button class="btn btn-primary">Sign In</button>
                </div>
            </div>
        </div>
    </nav>

    <!-- Hero Section -->
    <section id="home" class="bg-gradient" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
        <div class="container py-5">
            <div class="row align-items-center text-white">
                <div class="col-lg-6 py-5">
                    <span class="badge bg-warning text-dark mb-3">🔥 Hot Deal</span>
                    <h1 class="display-3 fw-bold mb-4">Summer Collection</h1>
                    <p class="lead mb-4">Discover the latest trends with up to 50% off on selected items. Limited time offer!</p>
                    <div class="d-flex gap-3">
                        <a href="#products" class="btn btn-light btn-lg px-4">
                            <i class="bi bi-bag-check me-2"></i>Shop Now
                        </a>
                        <a href="#deals" class="btn btn-outline-light btn-lg px-4">View Deals</a>
                    </div>
                </div>
                <div class="col-lg-6 text-center">
                    <img src="https://via.placeholder.com/500x400/667eea/ffffff?text=Summer+Fashion" class="img-fluid rounded shadow-lg" alt="Hero">
                </div>
            </div>
        </div>
    </section>

    <!-- Categories -->
    <section class="py-4 bg-light">
        <div class="container">
            <div class="d-flex flex-wrap justify-content-center gap-2">
                <span class="badge badge-category bg-primary fs-6 p-3">All Products</span>
                <span class="badge badge-category bg-secondary fs-6 p-3">Electronics</span>
                <span class="badge badge-category bg-success fs-6 p-3">Fashion</span>
                <span class="badge badge-category bg-warning fs-6 p-3">Home & Garden</span>
                <span class="badge badge-category bg-info fs-6 p-3">Sports</span>
                <span class="badge badge-category bg-danger fs-6 p-3">Books</span>
            </div>
        </div>
    </section>

    <!-- Products Grid -->
    <section id="products" class="py-5">
        <div class="container">
            <div class="row mb-4">
                <div class="col-md-6">
                    <h2 class="fw-bold">Featured Products</h2>
                    <p class="text-muted">Discover our handpicked selection</p>
                </div>
                <div class="col-md-6 text-end">
                    <div class="btn-group">
                        <button class="btn btn-outline-secondary">
                            <i class="bi bi-funnel me-1"></i>Filter
                        </button>
                        <button class="btn btn-outline-secondary">
                            <i class="bi bi-sort-down me-1"></i>Sort
                        </button>
                    </div>
                </div>
            </div>

            <div class="row g-4">
                <!-- Product 1 -->
                <div class="col-md-6 col-lg-3">
                    <div class="card product-card h-100 border-0 shadow-sm">
                        <div class="position-relative">
                            <img src="https://via.placeholder.com/300x300/4A90E2/ffffff?text=Wireless+Headphones" class="card-img-top" alt="Product">
                            <span class="badge bg-danger position-absolute top-0 end-0 m-2">-20%</span>
                            <div class="position-absolute bottom-0 end-0 m-2">
                                <button class="btn btn-sm btn-light rounded-circle" data-bs-toggle="modal" data-bs-target="#quickView">
                                    <i class="bi bi-eye"></i>
                                </button>
                            </div>
                        </div>
                        <div class="card-body">
                            <small class="text-muted">Electronics</small>
                            <h5 class="card-title mt-1">Wireless Headphones</h5>
                            <div class="text-warning mb-2">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star"></i>
                                <small class="text-muted">(124)</small>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <span class="text-muted text-decoration-line-through">$149.99</span>
                                    <h5 class="text-primary mb-0">$119.99</h5>
                                </div>
                                <button class="btn btn-primary btn-sm" onclick="addToCart('Wireless Headphones', 119.99)">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Product 2 -->
                <div class="col-md-6 col-lg-3">
                    <div class="card product-card h-100 border-0 shadow-sm">
                        <div class="position-relative">
                            <img src="https://via.placeholder.com/300x300/E74C3C/ffffff?text=Smart+Watch" class="card-img-top" alt="Product">
                            <span class="badge bg-success position-absolute top-0 end-0 m-2">New</span>
                        </div>
                        <div class="card-body">
                            <small class="text-muted">Electronics</small>
                            <h5 class="card-title mt-1">Smart Watch Pro</h5>
                            <div class="text-warning mb-2">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-half"></i>
                                <small class="text-muted">(89)</small>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <h5 class="text-primary mb-0">$299.99</h5>
                                <button class="btn btn-primary btn-sm" onclick="addToCart('Smart Watch Pro', 299.99)">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Product 3 -->
                <div class="col-md-6 col-lg-3">
                    <div class="card product-card h-100 border-0 shadow-sm">
                        <img src="https://via.placeholder.com/300x300/9B59B6/ffffff?text=Designer+Bag" class="card-img-top" alt="Product">
                        <div class="card-body">
                            <small class="text-muted">Fashion</small>
                            <h5 class="card-title mt-1">Designer Handbag</h5>
                            <div class="text-warning mb-2">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <small class="text-muted">(256)</small>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <h5 class="text-primary mb-0">$89.99</h5>
                                <button class="btn btn-primary btn-sm" onclick="addToCart('Designer Handbag', 89.99)">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Product 4 -->
                <div class="col-md-6 col-lg-3">
                    <div class="card product-card h-100 border-0 shadow-sm">
                        <div class="position-relative">
                            <img src="https://via.placeholder.com/300x300/27AE60/ffffff?text=Running+Shoes" class="card-img-top" alt="Product">
                            <span class="badge bg-warning position-absolute top-0 end-0 m-2">Sale</span>
                        </div>
                        <div class="card-body">
                            <small class="text-muted">Sports</small>
                            <h5 class="card-title mt-1">Running Shoes</h5>
                            <div class="text-warning mb-2">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star"></i>
                                <small class="text-muted">(178)</small>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <h5 class="text-primary mb-0">$79.99</h5>
                                <button class="btn btn-primary btn-sm" onclick="addToCart('Running Shoes', 79.99)">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Product 5 -->
                <div class="col-md-6 col-lg-3">
                    <div class="card product-card h-100 border-0 shadow-sm">
                        <img src="https://via.placeholder.com/300x300/F39C12/ffffff?text=Laptop+Stand" class="card-img-top" alt="Product">
                        <div class="card-body">
                            <small class="text-muted">Home & Office</small>
                            <h5 class="card-title mt-1">Ergonomic Laptop Stand</h5>
                            <div class="text-warning mb-2">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-half"></i>
                                <small class="text-muted">(92)</small>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <h5 class="text-primary mb-0">$49.99</h5>
                                <button class="btn btn-primary btn-sm" onclick="addToCart('Laptop Stand', 49.99)">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Product 6 -->
                <div class="col-md-6 col-lg-3">
                    <div class="card product-card h-100 border-0 shadow-sm">
                        <img src="https://via.placeholder.com/300x300/3498DB/ffffff?text=Coffee+Maker" class="card-img-top" alt="Product">
                        <div class="card-body">
                            <small class="text-muted">Home & Kitchen</small>
                            <h5 class="card-title mt-1">Smart Coffee Maker</h5>
                            <div class="text-warning mb-2">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star"></i>
                                <small class="text-muted">(145)</small>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <h5 class="text-primary mb-0">$129.99</h5>
                                <button class="btn btn-primary btn-sm" onclick="addToCart('Coffee Maker', 129.99)">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Product 7 -->
                <div class="col-md-6 col-lg-3">
                    <div class="card product-card h-100 border-0 shadow-sm">
                        <img src="https://via.placeholder.com/300x300/E67E22/ffffff?text=Yoga+Mat" class="card-img-top" alt="Product">
                        <div class="card-body">
                            <small class="text-muted">Sports & Fitness</small>
                            <h5 class="card-title mt-1">Premium Yoga Mat</h5>
                            <div class="text-warning mb-2">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <small class="text-muted">(203)</small>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <h5 class="text-primary mb-0">$39.99</h5>
                                <button class="btn btn-primary btn-sm" onclick="addToCart('Yoga Mat', 39.99)">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Product 8 -->
                <div class="col-md-6 col-lg-3">
                    <div class="card product-card h-100 border-0 shadow-sm">
                        <img src="https://via.placeholder.com/300x300/1ABC9C/ffffff?text=Book+Collection" class="card-img-top" alt="Product">
                        <div class="card-body">
                            <small class="text-muted">Books</small>
                            <h5 class="card-title mt-1">Bestseller Collection</h5>
                            <div class="text-warning mb-2">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-half"></i>
                                <small class="text-muted">(312)</small>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <h5 class="text-primary mb-0">$29.99</h5>
                                <button class="btn btn-primary btn-sm" onclick="addToCart('Book Collection', 29.99)">
                                    <i class="bi bi-cart-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="text-center mt-5">
                <button class="btn btn-outline-primary btn-lg">Load More Products</button>
            </div>
        </div>
    </section>

    <!-- Features Section -->
    <section class="py-5 bg-light">
        <div class="container">
            <div class="row g-4 text-center">
                <div class="col-md-3">
                    <div class="p-4">
                        <i class="bi bi-truck fs-1 text-primary mb-3"></i>
                        <h5>Free Shipping</h5>
                        <p class="text-muted">On orders over $50</p>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="p-4">
                        <i class="bi bi-arrow-clockwise fs-1 text-success mb-3"></i>
                        <h5>Easy Returns</h5>
                        <p class="text-muted">30-day return policy</p>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="p-4">
                        <i class="bi bi-shield-check fs-1 text-info mb-3"></i>
                        <h5>Secure Payment</h5>
                        <p class="text-muted">100% secure transactions</p>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="p-4">
                        <i class="bi bi-headset fs-1 text-warning mb-3"></i>
                        <h5>24/7 Support</h5>
                        <p class="text-muted">Dedicated customer service</p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Testimonials -->
    <section class="py-5">
        <div class="container">
            <h2 class="text-center mb-5">What Our Customers Say</h2>
            <div id="testimonialCarousel" class="carousel slide" data-bs-ride="carousel">
                <div class="carousel-inner">
                    <div class="carousel-item active">
                        <div class="row justify-content-center">
                            <div class="col-md-8 text-center">
                                <i class="bi bi-quote fs-1 text-primary"></i>
                                <p class="lead fst-italic">"Amazing products and excellent customer service! Highly recommend ShopHub for all your shopping needs."</p>
                                <div class="d-flex justify-content-center align-items-center mt-3">
                                    <img src="https://via.placeholder.com/60" class="rounded-circle me-3" alt="Customer">
                                    <div class="text-start">
                                        <h6 class="mb-0">Sarah Johnson</h6>
                                        <small class="text-muted">Verified Buyer</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="carousel-item">
                        <div class="row justify-content-center">
                            <div class="col-md-8 text-center">
                                <i class="bi bi-quote fs-1 text-primary"></i>
                                <p class="lead fst-italic">"Fast delivery and great quality products. Will definitely shop here again!"</p>
                                <div class="d-flex justify-content-center align-items-center mt-3">
                                    <img src="https://via.placeholder.com/60" class="rounded-circle me-3" alt="Customer">
                                    <div class="text-start">
                                        <h6 class="mb-0">Mike Chen</h6>
                                        <small class="text-muted">Verified Buyer</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <button class="carousel-control-prev" data-bs-target="#testimonialCarousel" data-bs-slide="prev">
                    <span class="carousel-control-prev-icon bg-dark rounded-circle"></span>
                </button>
                <button class="carousel-control-next" data-bs-target="#testimonialCarousel" data-bs-slide="next">
                    <span class="carousel-control-next-icon bg-dark rounded-circle"></span>
                </button>
            </div>
        </div>
    </section>

    <!-- Newsletter -->
    <section class="py-5 bg-primary text-white">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-md-6 mb-3 mb-md-0">
                    <h3>Subscribe to Our Newsletter</h3>
                    <p class="mb-0">Get exclusive deals and updates directly to your inbox</p>
                </div>
                <div class="col-md-6">
                    <form class="row g-2">
                        <div class="col-8">
                            <input type="email" class="form-control form-control-lg" placeholder="Your email address">
                        </div>
                        <div class="col-4">
                            <button class="btn btn-light btn-lg w-100">Subscribe</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </section>

    <!-- Footer -->
    <footer class="bg-dark text-white py-5">
        <div class="container">
            <div class="row g-4">
                <div class="col-md-3">
                    <h5 class="mb-3"><i class="bi bi-shop"></i> ShopHub</h5>
                    <p class="text-muted">Your trusted online shopping destination for quality products at great prices.</p>
                    <div class="d-flex gap-2">
                        <a href="#" class="btn btn-outline-light btn-sm"><i class="bi bi-facebook"></i></a>
                        <a href="#" class="btn btn-outline-light btn-sm"><i class="bi bi-twitter"></i></a>
                        <a href="#" class="btn btn-outline-light btn-sm"><i class="bi bi-instagram"></i></a>
                        <a href="#" class="btn btn-outline-light btn-sm"><i class="bi bi-linkedin"></i></a>
                    </div>
                </div>
                <div class="col-md-3">
                    <h6 class="mb-3">Shop</h6>
                    <ul class="list-unstyled">
                        <li><a href="#" class="text-muted text-decoration-none">All Products</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">New Arrivals</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">Best Sellers</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">Sale</a></li>
                    </ul>
                </div>
                <div class="col-md-3">
                    <h6 class="mb-3">Customer Service</h6>
                    <ul class="list-unstyled">
                        <li><a href="#" class="text-muted text-decoration-none">Contact Us</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">Shipping Info</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">Returns</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">FAQ</a></li>
                    </ul>
                </div>
                <div class="col-md-3">
                    <h6 class="mb-3">Company</h6>
                    <ul class="list-unstyled">
                        <li><a href="#" class="text-muted text-decoration-none">About Us</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">Careers</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">Privacy Policy</a></li>
                        <li><a href="#" class="text-muted text-decoration-none">Terms of Service</a></li>
                    </ul>
                </div>
            </div>
            <hr class="my-4 bg-secondary">
            <div class="row">
                <div class="col-md-6">
                    <p class="text-muted mb-0">&copy; 2024 ShopHub. All rights reserved.</p>
                </div>
                <div class="col-md-6 text-md-end">
                    <img src="https://via.placeholder.com/50x30/ffffff/000000?text=VISA" class="me-2" alt="Visa">
                    <img src="https://via.placeholder.com/50x30/ffffff/000000?text=MC" class="me-2" alt="Mastercard">
                    <img src="https://via.placeholder.com/50x30/ffffff/000000?text=AMEX" alt="Amex">
                </div>
            </div>
        </div>
    </footer>

    <!-- Shopping Cart Offcanvas -->
    <div class="offcanvas offcanvas-end" tabindex="-1" id="cartOffcanvas">
        <div class="offcanvas-header border-bottom">
            <h5 class="offcanvas-title">Shopping Cart <span class="badge bg-primary" id="cartItemCount">3</span></h5>
            <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
        </div>
        <div class="offcanvas-body">
            <!-- Cart Item 1 -->
            <div class="d-flex mb-3 pb-3 border-bottom">
                <img src="https://via.placeholder.com/80" class="rounded me-3" alt="Product">
                <div class="flex-grow-1">
                    <h6 class="mb-1">Wireless Headphones</h6>
                    <small class="text-muted">Electronics</small>
                    <div class="d-flex justify-content-between align-items-center mt-2">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-secondary">-</button>
                            <button class="btn btn-outline-secondary">1</button>
                            <button class="btn btn-outline-secondary">+</button>
                        </div>
                        <strong class="text-primary">$119.99</strong>
                    </div>
                </div>
                <button class="btn btn-sm btn-light ms-2"><i class="bi bi-trash"></i></button>
            </div>

            <!-- Cart Item 2 -->
            <div class="d-flex mb-3 pb-3 border-bottom">
                <img src="https://via.placeholder.com/80" class="rounded me-3" alt="Product">
                <div class="flex-grow-1">
                    <h6 class="mb-1">Smart Watch Pro</h6>
                    <small class="text-muted">Electronics</small>
                    <div class="d-flex justify-content-between align-items-center mt-2">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-secondary">-</button>
                            <button class="btn btn-outline-secondary">1</button>
                            <button class="btn btn-outline-secondary">+</button>
                        </div>
                        <strong class="text-primary">$299.99</strong>
                    </div>
                </div>
                <button class="btn btn-sm btn-light ms-2"><i class="bi bi-trash"></i></button>
            </div>

            <!-- Cart Item 3 -->
            <div class="d-flex mb-3 pb-3 border-bottom">
                <img src="https://via.placeholder.com/80" class="rounded me-3" alt="Product">
                <div class="flex-grow-1">
                    <h6 class="mb-1">Designer Handbag</h6>
                    <small class="text-muted">Fashion</small>
                    <div class="d-flex justify-content-between align-items-center mt-2">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-secondary">-</button>
                            <button class="btn btn-outline-secondary">1</button>
                            <button class="btn btn-outline-secondary">+</button>
                        </div>
                        <strong class="text-primary">$89.99</strong>
                    </div>
                </div>
                <button class="btn btn-sm btn-light ms-2"><i class="bi bi-trash"></i></button>
            </div>
        </div>
        <div class="offcanvas-footer border-top p-3">
            <div class="d-flex justify-content-between mb-3">
                <span>Subtotal:</span>
                <strong id="cartSubtotal">$509.97</strong>
            </div>
            <div class="d-flex justify-content-between mb-3">
                <span>Shipping:</span>
                <strong class="text-success">FREE</strong>
            </div>
            <div class="d-flex justify-content-between mb-3 fs-5">
                <strong>Total:</strong>
                <strong class="text-primary" id="cartTotal">$509.97</strong>
            </div>
            <button class="btn btn-primary btn-lg w-100 mb-2">Proceed to Checkout</button>
            <button class="btn btn-outline-secondary w-100" data-bs-dismiss="offcanvas">Continue Shopping</button>
        </div>
    </div>

    <!-- Quick View Modal -->
    <div class="modal fade" id="quickView" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header border-0">
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-6">
                            <img src="https://via.placeholder.com/500" class="img-fluid rounded" alt="Product">
                        </div>
                        <div class="col-md-6">
                            <span class="badge bg-danger mb-2">-20%</span>
                            <h3>Wireless Headphones</h3>
                            <div class="text-warning mb-3">
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star-fill"></i>
                                <i class="bi bi-star"></i>
                                <small class="text-muted">(124 reviews)</small>
                            </div>
                            <h4 class="text-primary mb-3">$119.99 <small class="text-muted text-decoration-line-through">$149.99</small></h4>
                            <p class="text-muted">Premium wireless headphones with active noise cancellation, 30-hour battery life, and crystal-clear sound quality.</p>
                            <div class="mb-3">
                                <label class="form-label">Color:</label>
                                <div class="d-flex gap-2">
                                    <button class="btn btn-outline-secondary">Black</button>
                                    <button class="btn btn-outline-secondary">White</button>
                                    <button class="btn btn-outline-secondary">Blue</button>
                                </div>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Quantity:</label>
                                <div class="input-group" style="width: 120px;">
                                    <button class="btn btn-outline-secondary">-</button>
                                    <input type="text" class="form-control text-center" value="1">
                                    <button class="btn btn-outline-secondary">+</button>
                                </div>
                            </div>
                            <button class="btn btn-primary btn-lg w-100 mb-2">
                                <i class="bi bi-cart-plus me-2"></i>Add to Cart
                            </button>
                            <button class="btn btn-outline-secondary w-100">
                                <i class="bi bi-heart me-2"></i>Add to Wishlist
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Shopping Cart Functionality
        let cartCount = 3;
        let cartTotal = 509.97;

        function addToCart(productName, price) {
            cartCount++;
            cartTotal += price;
            updateCartDisplay();
            
            // Show toast notification
            const toastHTML = `
                <div class="toast align-items-center text-white bg-success border-0 position-fixed top-0 end-0 m-3" role="alert">
                    <div class="d-flex">
                        <div class="toast-body">
                            <i class="bi bi-check-circle me-2"></i>${productName} added to cart!
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                    </div>
                </div>
            `;
            document.body.insertAdjacentHTML('beforeend', toastHTML);
            const toastElement = document.querySelector('.toast:last-child');
            const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
            toast.show();
            toastElement.addEventListener('hidden.bs.toast', () => toastElement.remove());
        }

        function updateCartDisplay() {
            document.getElementById('cartCount').textContent = cartCount;
            document.getElementById('cartItemCount').textContent = cartCount;
            document.getElementById('cartSubtotal').textContent = `$${cartTotal.toFixed(2)}`;
            document.getElementById('cartTotal').textContent = `$${cartTotal.toFixed(2)}`;
        }

        // Category Filter
        document.querySelectorAll('.badge-category').forEach(badge => {
            badge.addEventListener('click', function() {
                document.querySelectorAll('.badge-category').forEach(b => {
                    b.classList.remove('bg-primary');
                    b.classList.add('bg-secondary');
                });
                this.classList.remove('bg-secondary');
                this.classList.add('bg-primary');
            });
        });

        // Smooth Scroll
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) target.scrollIntoView({ behavior: 'smooth' });
            });
        });
    </script>
</body>
</html>
```

## 📝 Practice Exercises

1. **Add Product Filtering**: Implement working category filters
2. **Shopping Cart Logic**: Complete add/remove cart functionality
3. **Product Search**: Add a search bar with live filtering
4. **Price Range Filter**: Add min/max price sliders
5. **Product Wishlist**: Implement add to favorites
6. **Checkout Form**: Create multi-step checkout process

## ✅ Completion Checklist

- [ ] Responsive navigation with cart counter
- [ ] Hero section with promotional banner
- [ ] Category filter badges (functional)
- [ ] Product grid (8+ products)
- [ ] Product cards with ratings and prices
- [ ] Shopping cart offcanvas
- [ ] Quick view modal
- [ ] Add to cart functionality
- [ ] Toast notifications
- [ ] Footer with social icons

**Next: Day 44 - Admin Dashboard** 📊
