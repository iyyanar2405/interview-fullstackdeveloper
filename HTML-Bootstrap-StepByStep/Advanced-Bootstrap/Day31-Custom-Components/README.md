# Day 31: Custom Component Architecture 🏗️

Learn to build reusable custom components that extend Bootstrap's functionality while maintaining consistency and best practices.

## 🎯 Objectives
- Design modular component architecture
- Create reusable UI patterns
- Extend Bootstrap components
- Build component library
- Document component APIs

## 💻 Custom Components

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 31: Custom Component Architecture</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body { background: #f8f9fa; padding: 3rem 0; }
        .demo-section { background: white; border-radius: 15px; padding: 3rem; margin-bottom: 2rem; box-shadow: 0 10px 30px rgba(0,0,0,0.1); }
        
        /* Custom Notification Component */
        .notification-card {
            border-left: 4px solid;
            padding: 1rem 1.5rem;
            margin-bottom: 1rem;
            border-radius: 8px;
            background: white;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            transition: all 0.3s;
        }
        .notification-card:hover { transform: translateX(5px); box-shadow: 0 4px 12px rgba(0,0,0,0.15); }
        .notification-card.info { border-color: #0dcaf0; background: #cff4fc; }
        .notification-card.success { border-color: #198754; background: #d1e7dd; }
        .notification-card.warning { border-color: #ffc107; background: #fff3cd; }
        .notification-card.danger { border-color: #dc3545; background: #f8d7da; }
        
        /* Custom Stats Card */
        .stats-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 2rem;
            border-radius: 15px;
            text-align: center;
            transition: all 0.3s;
        }
        .stats-card:hover { transform: translateY(-10px); box-shadow: 0 20px 40px rgba(0,0,0,0.2); }
        .stats-card .stat-value { font-size: 3rem; font-weight: 700; }
        .stats-card .stat-label { font-size: 1rem; opacity: 0.9; }
        
        /* Custom Pricing Card */
        .pricing-card {
            border: 2px solid #e9ecef;
            border-radius: 15px;
            padding: 2rem;
            text-align: center;
            transition: all 0.3s;
            height: 100%;
        }
        .pricing-card:hover { border-color: #667eea; transform: scale(1.05); box-shadow: 0 15px 35px rgba(0,0,0,0.15); }
        .pricing-card.featured { border-color: #667eea; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; }
        .pricing-price { font-size: 3rem; font-weight: 700; }
        
        /* Custom Timeline */
        .timeline { position: relative; padding: 2rem 0; }
        .timeline::before { content: ''; position: absolute; left: 50%; top: 0; bottom: 0; width: 2px; background: #dee2e6; }
        .timeline-item { position: relative; margin-bottom: 3rem; }
        .timeline-item:nth-child(odd) { padding-right: 50%; padding-left: 0; }
        .timeline-item:nth-child(even) { padding-left: 50%; padding-right: 0; }
        .timeline-content { background: white; padding: 1.5rem; border-radius: 10px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .timeline-marker { position: absolute; top: 0; left: 50%; transform: translateX(-50%); width: 20px; height: 20px; background: #667eea; border-radius: 50%; border: 4px solid white; box-shadow: 0 0 0 4px #e9ecef; }
    </style>
</head>
<body>
    <div class="container">
        <h1 class="text-center mb-5">Day 31: Custom Components</h1>
        
        <!-- Notification Cards -->
        <section class="demo-section">
            <h2 class="mb-4">Custom Notification Cards</h2>
            <div class="notification-card info">
                <strong><i class="bi bi-info-circle me-2"></i>Information</strong>
                <p class="mb-0 mt-2">This is an informational message.</p>
            </div>
            <div class="notification-card success">
                <strong><i class="bi bi-check-circle me-2"></i>Success</strong>
                <p class="mb-0 mt-2">Operation completed successfully!</p>
            </div>
            <div class="notification-card warning">
                <strong><i class="bi bi-exclamation-triangle me-2"></i>Warning</strong>
                <p class="mb-0 mt-2">Please review your input.</p>
            </div>
            <div class="notification-card danger">
                <strong><i class="bi bi-x-circle me-2"></i>Error</strong>
                <p class="mb-0 mt-2">An error occurred.</p>
            </div>
        </section>
        
        <!-- Stats Cards -->
        <section class="demo-section">
            <h2 class="mb-4">Custom Stats Cards</h2>
            <div class="row g-4">
                <div class="col-md-3">
                    <div class="stats-card">
                        <div class="stat-value">1,234</div>
                        <div class="stat-label">Total Users</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stats-card" style="background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);">
                        <div class="stat-value">567</div>
                        <div class="stat-label">Active Projects</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stats-card" style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);">
                        <div class="stat-value">89%</div>
                        <div class="stat-label">Success Rate</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stats-card" style="background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);">
                        <div class="stat-value">$45K</div>
                        <div class="stat-label">Revenue</div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Pricing Cards -->
        <section class="demo-section">
            <h2 class="mb-4">Custom Pricing Cards</h2>
            <div class="row g-4">
                <div class="col-md-4">
                    <div class="pricing-card">
                        <h3>Basic</h3>
                        <div class="pricing-price">$9<small>/mo</small></div>
                        <ul class="list-unstyled mt-4">
                            <li class="mb-2">✓ 10 Projects</li>
                            <li class="mb-2">✓ 5 GB Storage</li>
                            <li class="mb-2">✓ Email Support</li>
                        </ul>
                        <button class="btn btn-outline-primary mt-3">Choose Plan</button>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="pricing-card featured">
                        <h3>Pro</h3>
                        <div class="pricing-price">$29<small>/mo</small></div>
                        <ul class="list-unstyled mt-4">
                            <li class="mb-2">✓ Unlimited Projects</li>
                            <li class="mb-2">✓ 50 GB Storage</li>
                            <li class="mb-2">✓ Priority Support</li>
                        </ul>
                        <button class="btn btn-light mt-3">Choose Plan</button>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="pricing-card">
                        <h3>Enterprise</h3>
                        <div class="pricing-price">$99<small>/mo</small></div>
                        <ul class="list-unstyled mt-4">
                            <li class="mb-2">✓ Everything in Pro</li>
                            <li class="mb-2">✓ Unlimited Storage</li>
                            <li class="mb-2">✓ 24/7 Support</li>
                        </ul>
                        <button class="btn btn-outline-primary mt-3">Contact Sales</button>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Timeline -->
        <section class="demo-section">
            <h2 class="mb-4">Custom Timeline</h2>
            <div class="timeline">
                <div class="timeline-item">
                    <div class="timeline-marker"></div>
                    <div class="timeline-content">
                        <h5>Project Started</h5>
                        <p class="text-muted mb-0">January 2024</p>
                    </div>
                </div>
                <div class="timeline-item">
                    <div class="timeline-marker"></div>
                    <div class="timeline-content">
                        <h5>First Milestone</h5>
                        <p class="text-muted mb-0">March 2024</p>
                    </div>
                </div>
                <div class="timeline-item">
                    <div class="timeline-marker"></div>
                    <div class="timeline-content">
                        <h5>Beta Release</h5>
                        <p class="text-muted mb-0">June 2024</p>
                    </div>
                </div>
            </div>
        </section>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>console.log('🏗️ Day 31 - Custom Components loaded!');</script>
</body>
</html>
```

## 📋 Checklist
- [ ] Notification cards
- [ ] Stats cards
- [ ] Pricing cards
- [ ] Timeline component
- [ ] Feature cards
- [ ] Testimonial cards
- [ ] Component documentation
- [ ] Reusable CSS classes
- [ ] Consistent design patterns

**Next: Day 32 - Advanced Grid Techniques** 📐
