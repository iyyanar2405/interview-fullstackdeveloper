# Day 56: Final Capstone Project 🎓
Comprehensive multi-page application combining all Bootstrap concepts.

## 🎯 Project Overview
Build a complete **Business Website** with:
- Landing page with hero, features, testimonials
- About page with team profiles
- Services/Products page with cards
- Blog with articles and sidebar
- Contact page with form and map
- Responsive navigation with offcanvas
- Dark mode toggle
- Smooth scrolling
- Custom animations

## 📋 Requirements Checklist
- [ ] Responsive navbar with offcanvas mobile menu
- [ ] Hero section with carousel or video background
- [ ] Grid-based features/services section
- [ ] Cards for products/services
- [ ] Testimonials with carousel
- [ ] Team section with hover effects
- [ ] Blog layout with pagination
- [ ] Contact form with validation
- [ ] Footer with social links
- [ ] Dark mode toggle
- [ ] Smooth scroll behavior
- [ ] Custom CSS for branding
- [ ] Performance optimized
- [ ] Accessibility compliant (WCAG)
- [ ] Cross-browser tested

## 💻 Complete Capstone Template

```html
<!DOCTYPE html>
<html lang="en" data-bs-theme="light">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>TechCorp - Capstone Project</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        html { scroll-behavior: smooth; }
        .hero { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; }
        .feature-icon { width: 64px; height: 64px; background: var(--bs-primary); border-radius: 12px; }
    </style>
</head>
<body>
    <!-- Navigation -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark fixed-top">
        <div class="container">
            <a class="navbar-brand fw-bold" href="#"><i class="bi bi-lightning-charge-fill"></i> TechCorp</a>
            <button class="navbar-toggler" data-bs-toggle="offcanvas" data-bs-target="#navMenu">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item"><a class="nav-link" href="#home">Home</a></li>
                    <li class="nav-item"><a class="nav-link" href="#features">Features</a></li>
                    <li class="nav-item"><a class="nav-link" href="#services">Services</a></li>
                    <li class="nav-item"><a class="nav-link" href="#team">Team</a></li>
                    <li class="nav-item"><a class="nav-link" href="#contact">Contact</a></li>
                </ul>
                <button class="btn btn-outline-light ms-3" id="themeToggle">
                    <i class="bi bi-moon-stars"></i>
                </button>
            </div>
        </div>
    </nav>

    <!-- Offcanvas Mobile Menu -->
    <div class="offcanvas offcanvas-start bg-dark text-white" id="navMenu">
        <div class="offcanvas-header">
            <h5 class="offcanvas-title">Menu</h5>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="offcanvas"></button>
        </div>
        <div class="offcanvas-body">
            <ul class="nav flex-column">
                <li class="nav-item"><a class="nav-link text-white" href="#home">Home</a></li>
                <li class="nav-item"><a class="nav-link text-white" href="#features">Features</a></li>
                <li class="nav-item"><a class="nav-link text-white" href="#services">Services</a></li>
                <li class="nav-item"><a class="nav-link text-white" href="#team">Team</a></li>
                <li class="nav-item"><a class="nav-link text-white" href="#contact">Contact</a></li>
            </ul>
        </div>
    </div>

    <!-- Hero Section -->
    <section id="home" class="hero text-white d-flex align-items-center">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-lg-6">
                    <h1 class="display-3 fw-bold mb-4">Build Amazing Websites</h1>
                    <p class="lead mb-4">Transform your digital presence with cutting-edge web solutions.</p>
                    <button class="btn btn-light btn-lg me-3">Get Started</button>
                    <button class="btn btn-outline-light btn-lg">Learn More</button>
                </div>
                <div class="col-lg-6">
                    <img src="https://via.placeholder.com/600x400" class="img-fluid rounded" alt="Hero">
                </div>
            </div>
        </div>
    </section>

    <!-- Features Section -->
    <section id="features" class="py-5">
        <div class="container py-5">
            <h2 class="text-center mb-5">Our Features</h2>
            <div class="row g-4">
                <div class="col-md-4">
                    <div class="text-center">
                        <div class="feature-icon d-flex align-items-center justify-content-center mx-auto mb-3">
                            <i class="bi bi-lightning-charge text-white fs-3"></i>
                        </div>
                        <h4>Fast Performance</h4>
                        <p class="text-muted">Lightning-fast load times for better user experience.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="text-center">
                        <div class="feature-icon d-flex align-items-center justify-content-center mx-auto mb-3">
                            <i class="bi bi-phone text-white fs-3"></i>
                        </div>
                        <h4>Responsive Design</h4>
                        <p class="text-muted">Perfect display on all devices and screen sizes.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="text-center">
                        <div class="feature-icon d-flex align-items-center justify-content-center mx-auto mb-3">
                            <i class="bi bi-shield-check text-white fs-3"></i>
                        </div>
                        <h4>Secure & Reliable</h4>
                        <p class="text-muted">Enterprise-grade security for peace of mind.</p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Services Section -->
    <section id="services" class="py-5 bg-light">
        <div class="container py-5">
            <h2 class="text-center mb-5">Our Services</h2>
            <div class="row g-4">
                <div class="col-md-6 col-lg-4">
                    <div class="card h-100 shadow-sm">
                        <img src="https://via.placeholder.com/400x250" class="card-img-top" alt="Service">
                        <div class="card-body">
                            <h5 class="card-title">Web Development</h5>
                            <p class="card-text">Custom websites built with modern technologies.</p>
                            <a href="#" class="btn btn-primary">Learn More</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-lg-4">
                    <div class="card h-100 shadow-sm">
                        <img src="https://via.placeholder.com/400x250" class="card-img-top" alt="Service">
                        <div class="card-body">
                            <h5 class="card-title">Mobile Apps</h5>
                            <p class="card-text">Native and cross-platform mobile applications.</p>
                            <a href="#" class="btn btn-primary">Learn More</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-lg-4">
                    <div class="card h-100 shadow-sm">
                        <img src="https://via.placeholder.com/400x250" class="card-img-top" alt="Service">
                        <div class="card-body">
                            <h5 class="card-title">Cloud Solutions</h5>
                            <p class="card-text">Scalable cloud infrastructure and deployment.</p>
                            <a href="#" class="btn btn-primary">Learn More</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Team Section -->
    <section id="team" class="py-5">
        <div class="container py-5">
            <h2 class="text-center mb-5">Meet Our Team</h2>
            <div class="row g-4">
                <div class="col-md-6 col-lg-3">
                    <div class="card text-center">
                        <img src="https://via.placeholder.com/300" class="card-img-top rounded-circle mx-auto mt-3" style="width: 150px; height: 150px;" alt="Team">
                        <div class="card-body">
                            <h5 class="card-title">John Doe</h5>
                            <p class="text-muted">CEO & Founder</p>
                            <div>
                                <a href="#" class="btn btn-sm btn-outline-primary"><i class="bi bi-linkedin"></i></a>
                                <a href="#" class="btn btn-sm btn-outline-primary"><i class="bi bi-twitter"></i></a>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-lg-3">
                    <div class="card text-center">
                        <img src="https://via.placeholder.com/300" class="card-img-top rounded-circle mx-auto mt-3" style="width: 150px; height: 150px;" alt="Team">
                        <div class="card-body">
                            <h5 class="card-title">Jane Smith</h5>
                            <p class="text-muted">Lead Developer</p>
                            <div>
                                <a href="#" class="btn btn-sm btn-outline-primary"><i class="bi bi-linkedin"></i></a>
                                <a href="#" class="btn btn-sm btn-outline-primary"><i class="bi bi-twitter"></i></a>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-lg-3">
                    <div class="card text-center">
                        <img src="https://via.placeholder.com/300" class="card-img-top rounded-circle mx-auto mt-3" style="width: 150px; height: 150px;" alt="Team">
                        <div class="card-body">
                            <h5 class="card-title">Mike Johnson</h5>
                            <p class="text-muted">Designer</p>
                            <div>
                                <a href="#" class="btn btn-sm btn-outline-primary"><i class="bi bi-linkedin"></i></a>
                                <a href="#" class="btn btn-sm btn-outline-primary"><i class="bi bi-twitter"></i></a>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-lg-3">
                    <div class="card text-center">
                        <img src="https://via.placeholder.com/300" class="card-img-top rounded-circle mx-auto mt-3" style="width: 150px; height: 150px;" alt="Team">
                        <div class="card-body">
                            <h5 class="card-title">Sarah Wilson</h5>
                            <p class="text-muted">Marketing</p>
                            <div>
                                <a href="#" class="btn btn-sm btn-outline-primary"><i class="bi bi-linkedin"></i></a>
                                <a href="#" class="btn btn-sm btn-outline-primary"><i class="bi bi-twitter"></i></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Testimonials -->
    <section class="py-5 bg-light">
        <div class="container py-5">
            <h2 class="text-center mb-5">What Clients Say</h2>
            <div id="testimonialCarousel" class="carousel slide" data-bs-ride="carousel">
                <div class="carousel-inner">
                    <div class="carousel-item active">
                        <div class="text-center px-5">
                            <p class="lead fst-italic">"Excellent service and outstanding results!"</p>
                            <p class="fw-bold">- Client A</p>
                        </div>
                    </div>
                    <div class="carousel-item">
                        <div class="text-center px-5">
                            <p class="lead fst-italic">"Professional team, highly recommend!"</p>
                            <p class="fw-bold">- Client B</p>
                        </div>
                    </div>
                </div>
                <button class="carousel-control-prev" data-bs-target="#testimonialCarousel" data-bs-slide="prev">
                    <span class="carousel-control-prev-icon bg-dark rounded"></span>
                </button>
                <button class="carousel-control-next" data-bs-target="#testimonialCarousel" data-bs-slide="next">
                    <span class="carousel-control-next-icon bg-dark rounded"></span>
                </button>
            </div>
        </div>
    </section>

    <!-- Contact Section -->
    <section id="contact" class="py-5">
        <div class="container py-5">
            <h2 class="text-center mb-5">Get In Touch</h2>
            <div class="row">
                <div class="col-lg-6 mx-auto">
                    <form>
                        <div class="mb-3">
                            <label class="form-label">Name</label>
                            <input type="text" class="form-control" required>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Email</label>
                            <input type="email" class="form-control" required>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Message</label>
                            <textarea class="form-control" rows="5" required></textarea>
                        </div>
                        <button type="submit" class="btn btn-primary btn-lg w-100">Send Message</button>
                    </form>
                </div>
            </div>
        </div>
    </section>

    <!-- Footer -->
    <footer class="bg-dark text-white py-4">
        <div class="container">
            <div class="row">
                <div class="col-md-6">
                    <h5>TechCorp</h5>
                    <p class="text-muted">Building the future, one website at a time.</p>
                </div>
                <div class="col-md-6 text-md-end">
                    <a href="#" class="btn btn-outline-light btn-sm me-2"><i class="bi bi-facebook"></i></a>
                    <a href="#" class="btn btn-outline-light btn-sm me-2"><i class="bi bi-twitter"></i></a>
                    <a href="#" class="btn btn-outline-light btn-sm me-2"><i class="bi bi-instagram"></i></a>
                    <a href="#" class="btn btn-outline-light btn-sm"><i class="bi bi-linkedin"></i></a>
                </div>
            </div>
            <hr class="my-3">
            <p class="text-center text-muted mb-0">&copy; 2024 TechCorp. All rights reserved.</p>
        </div>
    </footer>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Dark Mode Toggle
        const themeToggle = document.getElementById('themeToggle');
        const html = document.documentElement;
        
        themeToggle.addEventListener('click', () => {
            const currentTheme = html.getAttribute('data-bs-theme');
            const newTheme = currentTheme === 'light' ? 'dark' : 'light';
            html.setAttribute('data-bs-theme', newTheme);
            themeToggle.innerHTML = newTheme === 'light' 
                ? '<i class="bi bi-moon-stars"></i>' 
                : '<i class="bi bi-sun-fill"></i>';
        });

        // Close offcanvas when clicking nav link
        document.querySelectorAll('.offcanvas a').forEach(link => {
            link.addEventListener('click', () => {
                bootstrap.Offcanvas.getInstance(document.getElementById('navMenu'))?.hide();
            });
        });
    </script>
</body>
</html>
```

## 🎓 Congratulations!

You've completed the **56-Day HTML & Bootstrap Journey**! 

### What You've Learned:
- ✅ HTML5 fundamentals and semantic markup
- ✅ CSS3 styling and animations
- ✅ Bootstrap 5.3 grid system and components
- ✅ Responsive design principles
- ✅ JavaScript integration
- ✅ Real-world project implementations
- ✅ Performance optimization
- ✅ Accessibility best practices
- ✅ Dark mode implementation
- ✅ Framework integrations (React, Vue, Angular)

### Next Steps:
1. **Portfolio**: Showcase your projects on GitHub
2. **Practice**: Build more custom projects
3. **Learn**: Explore React, Vue, or Angular
4. **Contribute**: Join open-source projects
5. **Network**: Share your work on LinkedIn/Twitter

## 📚 Additional Resources
- [Bootstrap Documentation](https://getbootstrap.com/docs/)
- [MDN Web Docs](https://developer.mozilla.org/)
- [CSS-Tricks](https://css-tricks.com/)
- [Frontend Mentor](https://www.frontendmentor.io/)

---

**Keep building amazing things!** 🚀
