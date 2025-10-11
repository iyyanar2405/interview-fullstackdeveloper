# Day 45: Portfolio Website 💼

Create a stunning professional portfolio website to showcase your projects, skills, and experience with modern design and animations.

## 🎯 Learning Objectives
- Design modern portfolio layouts
- Create project showcases with modals
- Build skills section with progress bars
- Implement contact forms
- Add smooth scrolling and animations
- Design responsive hero sections
- Create project filtering

## 💼 Project Features
- ✅ Animated hero section
- ✅ About section with profile
- ✅ Skills with progress bars
- ✅ Portfolio grid with filters
- ✅ Project detail modals
- ✅ Testimonials carousel
- ✅ Contact form with validation
- ✅ Social media links
- ✅ Downloadable resume
- ✅ Smooth scroll navigation

## 💻 Complete Portfolio Website

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>John Doe - Full Stack Developer</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        html { scroll-behavior: smooth; }
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .navbar { transition: background 0.3s; }
        .navbar.scrolled { background: rgba(0,0,0,0.9) !important; }
        .hero { 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            position: relative;
        }
        .hero::after {
            content: '';
            position: absolute;
            bottom: 0;
            left: 0;
            right: 0;
            height: 100px;
            background: linear-gradient(to bottom, transparent, white);
        }
        .project-card {
            overflow: hidden;
            transition: transform 0.3s;
            cursor: pointer;
        }
        .project-card:hover { transform: translateY(-10px); }
        .project-card img {
            transition: transform 0.5s;
        }
        .project-card:hover img {
            transform: scale(1.1);
        }
        .skill-progress {
            height: 10px;
            border-radius: 10px;
        }
        .section-title {
            position: relative;
            padding-bottom: 15px;
        }
        .section-title::after {
            content: '';
            position: absolute;
            bottom: 0;
            left: 50%;
            transform: translateX(-50%);
            width: 60px;
            height: 3px;
            background: #667eea;
        }
        .filter-btn.active {
            background: #667eea !important;
            color: white !important;
        }
    </style>
</head>
<body>
    <!-- Navigation -->
    <nav class="navbar navbar-expand-lg navbar-dark fixed-top" style="background: transparent;">
        <div class="container">
            <a class="navbar-brand fw-bold" href="#">JD<span class="text-warning">.</span></a>
            <button class="navbar-toggler" data-bs-toggle="collapse" data-bs-target="#navMenu">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navMenu">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item"><a class="nav-link" href="#home">Home</a></li>
                    <li class="nav-item"><a class="nav-link" href="#about">About</a></li>
                    <li class="nav-item"><a class="nav-link" href="#skills">Skills</a></li>
                    <li class="nav-item"><a class="nav-link" href="#portfolio">Portfolio</a></li>
                    <li class="nav-item"><a class="nav-link" href="#testimonials">Testimonials</a></li>
                    <li class="nav-item"><a class="nav-link" href="#contact">Contact</a></li>
                </ul>
                <a href="#contact" class="btn btn-warning ms-3">Hire Me</a>
            </div>
        </div>
    </nav>

    <!-- Hero Section -->
    <section id="home" class="hero d-flex align-items-center text-white">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-lg-6 mb-4 mb-lg-0">
                    <span class="badge bg-warning text-dark mb-3">👋 Welcome to my portfolio</span>
                    <h1 class="display-3 fw-bold mb-3">Hi, I'm John Doe</h1>
                    <h2 class="h3 mb-4">Full Stack Developer & UI/UX Designer</h2>
                    <p class="lead mb-4">I create beautiful, responsive websites and applications that help businesses grow.</p>
                    <div class="d-flex gap-3 mb-4">
                        <a href="#portfolio" class="btn btn-light btn-lg px-4">
                            <i class="bi bi-briefcase me-2"></i>View Work
                        </a>
                        <a href="#contact" class="btn btn-outline-light btn-lg px-4">
                            <i class="bi bi-envelope me-2"></i>Contact Me
                        </a>
                    </div>
                    <div class="d-flex gap-3">
                        <a href="#" class="btn btn-outline-light btn-lg rounded-circle">
                            <i class="bi bi-github"></i>
                        </a>
                        <a href="#" class="btn btn-outline-light btn-lg rounded-circle">
                            <i class="bi bi-linkedin"></i>
                        </a>
                        <a href="#" class="btn btn-outline-light btn-lg rounded-circle">
                            <i class="bi bi-twitter"></i>
                        </a>
                        <a href="#" class="btn btn-outline-light btn-lg rounded-circle">
                            <i class="bi bi-dribbble"></i>
                        </a>
                    </div>
                </div>
                <div class="col-lg-6 text-center">
                    <img src="https://via.placeholder.com/500x500/667eea/ffffff?text=Profile" class="img-fluid rounded-circle shadow-lg" style="max-width: 400px;" alt="Profile">
                </div>
            </div>
        </div>
    </section>

    <!-- About Section -->
    <section id="about" class="py-5">
        <div class="container py-5">
            <h2 class="text-center section-title mb-5">About Me</h2>
            <div class="row align-items-center">
                <div class="col-lg-5 mb-4 mb-lg-0">
                    <img src="https://via.placeholder.com/500x600/667eea/ffffff?text=About+Photo" class="img-fluid rounded shadow" alt="About">
                </div>
                <div class="col-lg-7">
                    <h3 class="mb-3">I'm a Passionate Full Stack Developer</h3>
                    <p class="lead text-muted mb-4">With 5+ years of experience in building web applications and digital experiences.</p>
                    <p class="mb-4">I specialize in creating modern, responsive websites using cutting-edge technologies. My passion is transforming ideas into reality through clean, efficient code and beautiful design.</p>
                    
                    <div class="row g-4 mb-4">
                        <div class="col-md-6">
                            <div class="d-flex">
                                <i class="bi bi-check-circle-fill text-success fs-4 me-3"></i>
                                <div>
                                    <h5>Web Development</h5>
                                    <p class="text-muted mb-0">Building scalable web applications</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="d-flex">
                                <i class="bi bi-check-circle-fill text-success fs-4 me-3"></i>
                                <div>
                                    <h5>UI/UX Design</h5>
                                    <p class="text-muted mb-0">Creating beautiful user experiences</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="d-flex">
                                <i class="bi bi-check-circle-fill text-success fs-4 me-3"></i>
                                <div>
                                    <h5>Responsive Design</h5>
                                    <p class="text-muted mb-0">Mobile-first approach</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="d-flex">
                                <i class="bi bi-check-circle-fill text-success fs-4 me-3"></i>
                                <div>
                                    <h5>Performance</h5>
                                    <p class="text-muted mb-0">Optimized & fast applications</p>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="d-flex gap-3">
                        <a href="#" class="btn btn-primary btn-lg">
                            <i class="bi bi-download me-2"></i>Download CV
                        </a>
                        <a href="#contact" class="btn btn-outline-primary btn-lg">
                            Get in Touch
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Skills Section -->
    <section id="skills" class="py-5 bg-light">
        <div class="container py-5">
            <h2 class="text-center section-title mb-5">My Skills</h2>
            <div class="row">
                <div class="col-lg-6 mb-4">
                    <h5>HTML/CSS</h5>
                    <div class="progress skill-progress mb-3">
                        <div class="progress-bar" style="width: 95%">95%</div>
                    </div>

                    <h5>JavaScript/TypeScript</h5>
                    <div class="progress skill-progress mb-3">
                        <div class="progress-bar bg-success" style="width: 90%">90%</div>
                    </div>

                    <h5>React/Next.js</h5>
                    <div class="progress skill-progress mb-3">
                        <div class="progress-bar bg-info" style="width: 85%">85%</div>
                    </div>

                    <h5>Node.js/Express</h5>
                    <div class="progress skill-progress mb-3">
                        <div class="progress-bar bg-warning" style="width: 80%">80%</div>
                    </div>
                </div>

                <div class="col-lg-6 mb-4">
                    <h5>MongoDB/PostgreSQL</h5>
                    <div class="progress skill-progress mb-3">
                        <div class="progress-bar bg-danger" style="width: 85%">85%</div>
                    </div>

                    <h5>Python/Django</h5>
                    <div class="progress skill-progress mb-3">
                        <div class="progress-bar bg-secondary" style="width: 75%">75%</div>
                    </div>

                    <h5>UI/UX Design</h5>
                    <div class="progress skill-progress mb-3">
                        <div class="progress-bar" style="width: 88%">88%</div>
                    </div>

                    <h5>Git/DevOps</h5>
                    <div class="progress skill-progress mb-3">
                        <div class="progress-bar bg-success" style="width: 82%">82%</div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Portfolio Section -->
    <section id="portfolio" class="py-5">
        <div class="container py-5">
            <h2 class="text-center section-title mb-3">My Portfolio</h2>
            <p class="text-center text-muted mb-5">Check out my latest projects</p>

            <!-- Filter Buttons -->
            <div class="text-center mb-5">
                <button class="btn btn-outline-primary filter-btn active m-1" data-filter="all">All Projects</button>
                <button class="btn btn-outline-primary filter-btn m-1" data-filter="web">Web Design</button>
                <button class="btn btn-outline-primary filter-btn m-1" data-filter="app">Web Apps</button>
                <button class="btn btn-outline-primary filter-btn m-1" data-filter="mobile">Mobile</button>
            </div>

            <!-- Portfolio Grid -->
            <div class="row g-4" id="portfolioGrid">
                <!-- Project 1 -->
                <div class="col-md-6 col-lg-4 portfolio-item" data-category="web">
                    <div class="card project-card border-0 shadow-sm h-100" data-bs-toggle="modal" data-bs-target="#projectModal1">
                        <div class="overflow-hidden">
                            <img src="https://via.placeholder.com/400x300/667eea/ffffff?text=E-commerce+Website" class="card-img-top" alt="Project">
                        </div>
                        <div class="card-body">
                            <span class="badge bg-primary mb-2">Web Design</span>
                            <h5 class="card-title">E-Commerce Platform</h5>
                            <p class="card-text text-muted">Modern online shopping experience with cart and checkout</p>
                            <div class="d-flex gap-2">
                                <span class="badge bg-light text-dark">React</span>
                                <span class="badge bg-light text-dark">Node.js</span>
                                <span class="badge bg-light text-dark">MongoDB</span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Project 2 -->
                <div class="col-md-6 col-lg-4 portfolio-item" data-category="app">
                    <div class="card project-card border-0 shadow-sm h-100" data-bs-toggle="modal" data-bs-target="#projectModal2">
                        <div class="overflow-hidden">
                            <img src="https://via.placeholder.com/400x300/764ba2/ffffff?text=Task+Manager" class="card-img-top" alt="Project">
                        </div>
                        <div class="card-body">
                            <span class="badge bg-success mb-2">Web App</span>
                            <h5 class="card-title">Task Management App</h5>
                            <p class="card-text text-muted">Collaborative project management tool</p>
                            <div class="d-flex gap-2">
                                <span class="badge bg-light text-dark">Vue.js</span>
                                <span class="badge bg-light text-dark">Firebase</span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Project 3 -->
                <div class="col-md-6 col-lg-4 portfolio-item" data-category="mobile">
                    <div class="card project-card border-0 shadow-sm h-100" data-bs-toggle="modal" data-bs-target="#projectModal3">
                        <div class="overflow-hidden">
                            <img src="https://via.placeholder.com/400x300/f093fb/ffffff?text=Fitness+App" class="card-img-top" alt="Project">
                        </div>
                        <div class="card-body">
                            <span class="badge bg-warning mb-2">Mobile App</span>
                            <h5 class="card-title">Fitness Tracker</h5>
                            <p class="card-text text-muted">Track workouts and monitor progress</p>
                            <div class="d-flex gap-2">
                                <span class="badge bg-light text-dark">React Native</span>
                                <span class="badge bg-light text-dark">Redux</span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Project 4 -->
                <div class="col-md-6 col-lg-4 portfolio-item" data-category="web">
                    <div class="card project-card border-0 shadow-sm h-100">
                        <div class="overflow-hidden">
                            <img src="https://via.placeholder.com/400x300/4facfe/ffffff?text=Blog+Platform" class="card-img-top" alt="Project">
                        </div>
                        <div class="card-body">
                            <span class="badge bg-primary mb-2">Web Design</span>
                            <h5 class="card-title">Blog Platform</h5>
                            <p class="card-text text-muted">Content management system for bloggers</p>
                            <div class="d-flex gap-2">
                                <span class="badge bg-light text-dark">Next.js</span>
                                <span class="badge bg-light text-dark">Prisma</span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Project 5 -->
                <div class="col-md-6 col-lg-4 portfolio-item" data-category="app">
                    <div class="card project-card border-0 shadow-sm h-100">
                        <div class="overflow-hidden">
                            <img src="https://via.placeholder.com/400x300/00f2fe/ffffff?text=Social+Network" class="card-img-top" alt="Project">
                        </div>
                        <div class="card-body">
                            <span class="badge bg-success mb-2">Web App</span>
                            <h5 class="card-title">Social Network</h5>
                            <p class="card-text text-muted">Connect and share with friends</p>
                            <div class="d-flex gap-2">
                                <span class="badge bg-light text-dark">MERN Stack</span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Project 6 -->
                <div class="col-md-6 col-lg-4 portfolio-item" data-category="web">
                    <div class="card project-card border-0 shadow-sm h-100">
                        <div class="overflow-hidden">
                            <img src="https://via.placeholder.com/400x300/fbc2eb/ffffff?text=Portfolio+Site" class="card-img-top" alt="Project">
                        </div>
                        <div class="card-body">
                            <span class="badge bg-primary mb-2">Web Design</span>
                            <h5 class="card-title">Portfolio Website</h5>
                            <p class="card-text text-muted">Showcase work with modern design</p>
                            <div class="d-flex gap-2">
                                <span class="badge bg-light text-dark">HTML</span>
                                <span class="badge bg-light text-dark">CSS</span>
                                <span class="badge bg-light text-dark">JS</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Testimonials Section -->
    <section id="testimonials" class="py-5 bg-light">
        <div class="container py-5">
            <h2 class="text-center section-title mb-5">What Clients Say</h2>
            <div id="testimonialCarousel" class="carousel slide" data-bs-ride="carousel">
                <div class="carousel-inner">
                    <div class="carousel-item active">
                        <div class="row justify-content-center">
                            <div class="col-lg-8">
                                <div class="card border-0 shadow">
                                    <div class="card-body p-5 text-center">
                                        <i class="bi bi-quote fs-1 text-primary mb-3"></i>
                                        <p class="lead mb-4">"John delivered an exceptional website that exceeded our expectations. Professional, responsive, and a pleasure to work with!"</p>
                                        <div class="d-flex justify-content-center align-items-center">
                                            <img src="https://via.placeholder.com/60" class="rounded-circle me-3" alt="Client">
                                            <div class="text-start">
                                                <h6 class="mb-0">Sarah Johnson</h6>
                                                <small class="text-muted">CEO, TechCorp</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="carousel-item">
                        <div class="row justify-content-center">
                            <div class="col-lg-8">
                                <div class="card border-0 shadow">
                                    <div class="card-body p-5 text-center">
                                        <i class="bi bi-quote fs-1 text-primary mb-3"></i>
                                        <p class="lead mb-4">"Outstanding work! The website is fast, beautiful, and exactly what we needed for our business growth."</p>
                                        <div class="d-flex justify-content-center align-items-center">
                                            <img src="https://via.placeholder.com/60" class="rounded-circle me-3" alt="Client">
                                            <div class="text-start">
                                                <h6 class="mb-0">Mike Chen</h6>
                                                <small class="text-muted">Founder, StartupHub</small>
                                            </div>
                                        </div>
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

    <!-- Contact Section -->
    <section id="contact" class="py-5">
        <div class="container py-5">
            <h2 class="text-center section-title mb-3">Get In Touch</h2>
            <p class="text-center text-muted mb-5">Have a project in mind? Let's work together!</p>
            
            <div class="row">
                <div class="col-lg-4 mb-4">
                    <div class="card border-0 shadow-sm h-100 text-center p-4">
                        <i class="bi bi-envelope fs-1 text-primary mb-3"></i>
                        <h5>Email</h5>
                        <p class="text-muted">john.doe@example.com</p>
                    </div>
                </div>
                <div class="col-lg-4 mb-4">
                    <div class="card border-0 shadow-sm h-100 text-center p-4">
                        <i class="bi bi-telephone fs-1 text-success mb-3"></i>
                        <h5>Phone</h5>
                        <p class="text-muted">+1 (555) 123-4567</p>
                    </div>
                </div>
                <div class="col-lg-4 mb-4">
                    <div class="card border-0 shadow-sm h-100 text-center p-4">
                        <i class="bi bi-geo-alt fs-1 text-danger mb-3"></i>
                        <h5>Location</h5>
                        <p class="text-muted">San Francisco, CA</p>
                    </div>
                </div>
            </div>

            <div class="row mt-4">
                <div class="col-lg-8 mx-auto">
                    <div class="card border-0 shadow">
                        <div class="card-body p-4">
                            <form>
                                <div class="row g-3">
                                    <div class="col-md-6">
                                        <label class="form-label">Your Name</label>
                                        <input type="text" class="form-control form-control-lg" required>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Your Email</label>
                                        <input type="email" class="form-control form-control-lg" required>
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Subject</label>
                                        <input type="text" class="form-control form-control-lg" required>
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Message</label>
                                        <textarea class="form-control form-control-lg" rows="5" required></textarea>
                                    </div>
                                    <div class="col-12">
                                        <button type="submit" class="btn btn-primary btn-lg w-100">
                                            <i class="bi bi-send me-2"></i>Send Message
                                        </button>
                                    </div>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Footer -->
    <footer class="bg-dark text-white py-4">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-md-6 text-center text-md-start mb-3 mb-md-0">
                    <p class="mb-0">&copy; 2024 John Doe. All rights reserved.</p>
                </div>
                <div class="col-md-6 text-center text-md-end">
                    <a href="#" class="btn btn-outline-light btn-sm me-2"><i class="bi bi-github"></i></a>
                    <a href="#" class="btn btn-outline-light btn-sm me-2"><i class="bi bi-linkedin"></i></a>
                    <a href="#" class="btn btn-outline-light btn-sm me-2"><i class="bi bi-twitter"></i></a>
                    <a href="#" class="btn btn-outline-light btn-sm"><i class="bi bi-dribbble"></i></a>
                </div>
            </div>
        </div>
    </footer>

    <!-- Project Modal 1 -->
    <div class="modal fade" id="projectModal1" tabindex="-1">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header border-0">
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <img src="https://via.placeholder.com/800x500/667eea/ffffff?text=E-commerce+Full+View" class="img-fluid rounded mb-4" alt="Project">
                    <h3>E-Commerce Platform</h3>
                    <p class="lead text-muted">A full-featured online shopping platform</p>
                    <p>Built a complete e-commerce solution with product catalog, shopping cart, user authentication, payment integration, and admin dashboard. The platform handles thousands of daily transactions and provides seamless shopping experience across all devices.</p>
                    <h5>Technologies Used:</h5>
                    <div class="d-flex gap-2 mb-4">
                        <span class="badge bg-primary">React</span>
                        <span class="badge bg-success">Node.js</span>
                        <span class="badge bg-info">MongoDB</span>
                        <span class="badge bg-warning">Stripe</span>
                    </div>
                    <div class="d-flex gap-2">
                        <a href="#" class="btn btn-primary"><i class="bi bi-globe me-2"></i>Live Demo</a>
                        <a href="#" class="btn btn-outline-secondary"><i class="bi bi-github me-2"></i>View Code</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Additional Project Modals (2 & 3) would follow same structure -->

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Navbar scroll effect
        window.addEventListener('scroll', function() {
            const navbar = document.querySelector('.navbar');
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });

        // Portfolio Filter
        document.querySelectorAll('.filter-btn').forEach(button => {
            button.addEventListener('click', function() {
                const filter = this.dataset.filter;
                
                // Update active button
                document.querySelectorAll('.filter-btn').forEach(btn => btn.classList.remove('active'));
                this.classList.add('active');
                
                // Filter items
                document.querySelectorAll('.portfolio-item').forEach(item => {
                    if (filter === 'all' || item.dataset.category === filter) {
                        item.style.display = 'block';
                    } else {
                        item.style.display = 'none';
                    }
                });
            });
        });

        // Smooth scroll for nav links
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                    // Close mobile menu if open
                    const navCollapse = document.querySelector('.navbar-collapse');
                    if (navCollapse.classList.contains('show')) {
                        bootstrap.Collapse.getInstance(navCollapse).hide();
                    }
                }
            });
        });
    </script>
</body>
</html>
```

## 📝 Practice Exercises

1. **Add Animations**: Implement scroll animations with Intersection Observer
2. **Form Validation**: Add real-time form validation
3. **Email Integration**: Connect contact form to backend
4. **Blog Section**: Add blog posts section
5. **Dark Mode**: Implement dark mode toggle
6. **Project Filtering**: Enhance with animated transitions

## ✅ Completion Checklist

- [ ] Animated hero section
- [ ] About section with profile
- [ ] Skills with progress bars
- [ ] Portfolio grid with filters
- [ ] Project modals
- [ ] Testimonials carousel
- [ ] Contact form
- [ ] Social media links
- [ ] Smooth scroll navigation
- [ ] Responsive design

**Next: Day 46 - Blog Theme** 📝
