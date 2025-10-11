# Day 28: Offcanvas Components 📱

Master Bootstrap's offcanvas component for creating sliding sidebars, mobile menus, and overlay panels that enhance navigation and user experience.

## 💻 Complete HTML

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 28: Offcanvas Components</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; padding: 3rem 0; }
        .demo-section { background: white; border-radius: 15px; padding: 3rem; margin-bottom: 2rem; box-shadow: 0 20px 60px rgba(0,0,0,0.3); }
        .demo-title { color: #2d3748; font-weight: 700; margin-bottom: 2rem; }
        .offcanvas { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; }
        .offcanvas-header { border-bottom: 1px solid rgba(255,255,255,0.2); }
        .offcanvas-body .nav-link { color: white; padding: 0.75rem 1rem; border-radius: 8px; transition: all 0.3s; }
        .offcanvas-body .nav-link:hover { background: rgba(255,255,255,0.1); }
        .btn-close { filter: invert(1); }
    </style>
</head>
<body>
    <div class="container">
        <div class="text-center text-white mb-5">
            <h1 class="display-3 fw-bold">Day 28: Offcanvas</h1>
            <p class="lead">Sliding sidebars and mobile menus</p>
        </div>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-layout-sidebar me-2"></i>
                Offcanvas Directions
            </h2>
            
            <button class="btn btn-primary" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasLeft">
                Open Left
            </button>
            
            <button class="btn btn-success ms-2" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasRight">
                Open Right
            </button>
            
            <button class="btn btn-info ms-2" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasTop">
                Open Top
            </button>
            
            <button class="btn btn-warning ms-2" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasBottom">
                Open Bottom
            </button>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-menu-button-wide me-2"></i>
                Mobile Navigation Menu
            </h2>
            
            <button class="btn btn-dark" type="button" data-bs-toggle="offcanvas" data-bs-target="#mobileMenu">
                <i class="bi bi-list"></i> Open Menu
            </button>
            
            <p class="mt-3 text-muted">This demonstrates a common use case: a mobile-friendly navigation menu that slides in from the side.</p>
        </section>
        
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-cart me-2"></i>
                Shopping Cart Sidebar
            </h2>
            
            <button class="btn btn-primary" type="button" data-bs-toggle="offcanvas" data-bs-target="#cartOffcanvas">
                <i class="bi bi-cart3"></i> View Cart (3)
            </button>
        </section>
    </div>
    
    <!-- Left Offcanvas -->
    <div class="offcanvas offcanvas-start" tabindex="-1" id="offcanvasLeft">
        <div class="offcanvas-header">
            <h5 class="offcanvas-title">Left Sidebar</h5>
            <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
        </div>
        <div class="offcanvas-body">
            <p>Content for the left offcanvas sidebar.</p>
            <nav class="nav flex-column">
                <a class="nav-link" href="#"><i class="bi bi-house me-2"></i>Home</a>
                <a class="nav-link" href="#"><i class="bi bi-grid me-2"></i>Dashboard</a>
                <a class="nav-link" href="#"><i class="bi bi-gear me-2"></i>Settings</a>
            </nav>
        </div>
    </div>
    
    <!-- Right Offcanvas -->
    <div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasRight">
        <div class="offcanvas-header">
            <h5 class="offcanvas-title">Right Sidebar</h5>
            <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
        </div>
        <div class="offcanvas-body">
            <p>Content for the right offcanvas sidebar.</p>
        </div>
    </div>
    
    <!-- Top Offcanvas -->
    <div class="offcanvas offcanvas-top" tabindex="-1" id="offcanvasTop">
        <div class="offcanvas-header">
            <h5 class="offcanvas-title">Top Panel</h5>
            <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
        </div>
        <div class="offcanvas-body">
            <p>Content slides in from the top.</p>
        </div>
    </div>
    
    <!-- Bottom Offcanvas -->
    <div class="offcanvas offcanvas-bottom" tabindex="-1" id="offcanvasBottom">
        <div class="offcanvas-header">
            <h5 class="offcanvas-title">Bottom Panel</h5>
            <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
        </div>
        <div class="offcanvas-body">
            <p>Content slides in from the bottom.</p>
        </div>
    </div>
    
    <!-- Mobile Menu Offcanvas -->
    <div class="offcanvas offcanvas-start" tabindex="-1" id="mobileMenu">
        <div class="offcanvas-header">
            <h5 class="offcanvas-title">Navigation</h5>
            <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
        </div>
        <div class="offcanvas-body">
            <nav class="nav flex-column">
                <a class="nav-link active" href="#"><i class="bi bi-house-door me-2"></i>Home</a>
                <a class="nav-link" href="#"><i class="bi bi-folder me-2"></i>Products</a>
                <a class="nav-link" href="#"><i class="bi bi-info-circle me-2"></i>About</a>
                <a class="nav-link" href="#"><i class="bi bi-telephone me-2"></i>Contact</a>
                <a class="nav-link" href="#"><i class="bi bi-person me-2"></i>Account</a>
            </nav>
            <hr class="my-4" style="border-color: rgba(255,255,255,0.2);">
            <button class="btn btn-light w-100"><i class="bi bi-box-arrow-right me-2"></i>Logout</button>
        </div>
    </div>
    
    <!-- Cart Offcanvas -->
    <div class="offcanvas offcanvas-end" tabindex="-1" id="cartOffcanvas">
        <div class="offcanvas-header">
            <h5 class="offcanvas-title"><i class="bi bi-cart3 me-2"></i>Shopping Cart</h5>
            <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
        </div>
        <div class="offcanvas-body">
            <div class="d-flex justify-content-between align-items-center mb-3 pb-3" style="border-bottom: 1px solid rgba(255,255,255,0.2);">
                <div>
                    <h6 class="mb-0">Product Name 1</h6>
                    <small>$29.99</small>
                </div>
                <button class="btn btn-sm btn-light">×</button>
            </div>
            <div class="d-flex justify-content-between align-items-center mb-3 pb-3" style="border-bottom: 1px solid rgba(255,255,255,0.2);">
                <div>
                    <h6 class="mb-0">Product Name 2</h6>
                    <small>$49.99</small>
                </div>
                <button class="btn btn-sm btn-light">×</button>
            </div>
            <div class="d-flex justify-content-between align-items-center mb-3 pb-3" style="border-bottom: 1px solid rgba(255,255,255,0.2);">
                <div>
                    <h6 class="mb-0">Product Name 3</h6>
                    <small>$19.99</small>
                </div>
                <button class="btn btn-sm btn-light">×</button>
            </div>
            
            <div class="mt-auto pt-4">
                <div class="d-flex justify-content-between mb-3">
                    <strong>Total:</strong>
                    <strong>$99.97</strong>
                </div>
                <button class="btn btn-light w-100 mb-2">View Cart</button>
                <button class="btn btn-warning w-100">Checkout</button>
            </div>
        </div>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        console.log('📱 Day 28 - Offcanvas Components loaded!');
        
        // Optional: Track offcanvas events
        document.querySelectorAll('.offcanvas').forEach(offcanvas => {
            offcanvas.addEventListener('show.bs.offcanvas', function () {
                console.log('Offcanvas opening:', this.id);
            });
        });
    </script>
</body>
</html>
```

## 📋 Checklist
- [ ] Left offcanvas
- [ ] Right offcanvas
- [ ] Top offcanvas
- [ ] Bottom offcanvas
- [ ] Mobile navigation menu
- [ ] Shopping cart sidebar
- [ ] Custom styling
- [ ] Backdrop & scroll options
- [ ] Accessibility (ARIA)
- [ ] JavaScript events

## 🎯 Day 28 Complete!

### ✅ Bootstrap Components Phase Complete!
You've now mastered all major Bootstrap components (Days 15-28):
- Buttons, Cards, Navigation
- Modals, Forms, Tables
- Carousels, Accordions
- Tooltips, Popovers, Dropdowns
- Badges, Progress, Pagination
- Alerts, Toasts, Offcanvas

**Next: Day 29 - SASS Customization & Theming** 🎨
