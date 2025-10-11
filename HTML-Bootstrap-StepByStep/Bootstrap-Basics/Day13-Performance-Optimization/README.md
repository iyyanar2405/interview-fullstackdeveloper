# Day 13: Performance & Optimization ⚡

Welcome to Day 13! Today you'll master Bootstrap performance optimization techniques, learn to create custom builds, implement tree shaking, and deploy optimized Bootstrap projects. You'll discover how to make your Bootstrap sites lightning-fast and production-ready.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Understand Bootstrap performance bottlenecks and solutions
- Create custom Bootstrap builds with only needed components
- Implement CSS and JavaScript tree shaking techniques
- Optimize images and media for better performance
- Use CDN strategies and caching effectively
- Measure and monitor Bootstrap site performance
- Deploy optimized Bootstrap projects to production
- Apply advanced optimization techniques and best practices

## 📚 Lesson Content

### Understanding Bootstrap Performance

Bootstrap is feature-rich but can be optimized significantly by including only what you need.

#### Bootstrap Bundle Size Analysis
```html
<!-- Full Bootstrap CSS (uncompressed): ~200KB -->
<!-- Full Bootstrap CSS (compressed): ~25KB -->
<!-- Full Bootstrap JS (uncompressed): ~150KB -->
<!-- Full Bootstrap JS (compressed): ~20KB -->

<!-- Performance considerations -->
<div class="container">
    <div class="alert alert-info">
        <h5><i class="bi bi-info-circle me-2"></i>Bootstrap Size Breakdown</h5>
        <div class="row g-3">
            <div class="col-md-6">
                <h6>CSS Components</h6>
                <ul class="small mb-0">
                    <li>Grid System: ~8KB</li>
                    <li>Typography: ~3KB</li>
                    <li>Buttons: ~4KB</li>
                    <li>Forms: ~6KB</li>
                    <li>Components: ~15KB</li>
                    <li>Utilities: ~12KB</li>
                </ul>
            </div>
            <div class="col-md-6">
                <h6>JavaScript Plugins</h6>
                <ul class="small mb-0">
                    <li>Modal: ~5KB</li>
                    <li>Dropdown: ~3KB</li>
                    <li>Carousel: ~4KB</li>
                    <li>Collapse: ~2KB</li>
                    <li>Tooltip/Popover: ~8KB</li>
                    <li>Tab: ~2KB</li>
                </ul>
            </div>
        </div>
    </div>
</div>
```

#### Performance Audit Checklist
```html
<div class="container">
    <div class="row g-4">
        <div class="col-lg-6">
            <div class="card border-success">
                <div class="card-header bg-success text-white">
                    <h5 class="mb-0">
                        <i class="bi bi-speedometer2 me-2"></i>Performance Checklist
                    </h5>
                </div>
                <div class="card-body">
                    <div class="form-check mb-2">
                        <input class="form-check-input" type="checkbox" id="check1">
                        <label class="form-check-label" for="check1">
                            Use only necessary Bootstrap components
                        </label>
                    </div>
                    <div class="form-check mb-2">
                        <input class="form-check-input" type="checkbox" id="check2">
                        <label class="form-check-label" for="check2">
                            Implement custom builds with SASS
                        </label>
                    </div>
                    <div class="form-check mb-2">
                        <input class="form-check-input" type="checkbox" id="check3">
                        <label class="form-check-label" for="check3">
                            Optimize and compress images
                        </label>
                    </div>
                    <div class="form-check mb-2">
                        <input class="form-check-input" type="checkbox" id="check4">
                        <label class="form-check-label" for="check4">
                            Enable gzip compression
                        </label>
                    </div>
                    <div class="form-check mb-2">
                        <input class="form-check-input" type="checkbox" id="check5">
                        <label class="form-check-label" for="check5">
                            Use CDN for Bootstrap assets
                        </label>
                    </div>
                    <div class="form-check mb-2">
                        <input class="form-check-input" type="checkbox" id="check6">
                        <label class="form-check-label" for="check6">
                            Minimize HTTP requests
                        </label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" id="check7">
                        <label class="form-check-label" for="check7">
                            Implement lazy loading for images
                        </label>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="col-lg-6">
            <div class="card border-primary">
                <div class="card-header bg-primary text-white">
                    <h5 class="mb-0">
                        <i class="bi bi-tools me-2"></i>Optimization Tools
                    </h5>
                </div>
                <div class="card-body">
                    <ul class="list-unstyled">
                        <li class="mb-2">
                            <i class="bi bi-check text-success me-2"></i>
                            <strong>Lighthouse:</strong> Google's performance audit tool
                        </li>
                        <li class="mb-2">
                            <i class="bi bi-check text-success me-2"></i>
                            <strong>WebPageTest:</strong> Detailed performance analysis
                        </li>
                        <li class="mb-2">
                            <i class="bi bi-check text-success me-2"></i>
                            <strong>GTmetrix:</strong> Performance monitoring
                        </li>
                        <li class="mb-2">
                            <i class="bi bi-check text-success me-2"></i>
                            <strong>PurgeCSS:</strong> Remove unused CSS
                        </li>
                        <li class="mb-2">
                            <i class="bi bi-check text-success me-2"></i>
                            <strong>Webpack:</strong> Bundle optimization
                        </li>
                        <li class="mb-0">
                            <i class="bi bi-check text-success me-2"></i>
                            <strong>Parcel/Vite:</strong> Fast build tools
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
```

### Custom Bootstrap Builds

Create tailored Bootstrap builds with only the components you need.

#### SASS Custom Build Setup
```scss
// custom.scss - Custom Bootstrap build
// Import only what you need

// 1. Required Bootstrap imports
@import "node_modules/bootstrap/scss/functions";
@import "node_modules/bootstrap/scss/variables";
@import "node_modules/bootstrap/scss/mixins";

// 2. Custom variable overrides
$primary: #6f42c1;
$secondary: #6c757d;
$border-radius: 0.5rem;
$font-family-sans-serif: 'Inter', system-ui, sans-serif;

// 3. Layout & components (import only what you use)
@import "node_modules/bootstrap/scss/root";
@import "node_modules/bootstrap/scss/reboot";
@import "node_modules/bootstrap/scss/type";
@import "node_modules/bootstrap/scss/images";
@import "node_modules/bootstrap/scss/containers";
@import "node_modules/bootstrap/scss/grid";

// 4. Components (comment out unused components)
@import "node_modules/bootstrap/scss/buttons";
@import "node_modules/bootstrap/scss/nav";
@import "node_modules/bootstrap/scss/navbar";
@import "node_modules/bootstrap/scss/card";
@import "node_modules/bootstrap/scss/forms";
// @import "node_modules/bootstrap/scss/accordion";
// @import "node_modules/bootstrap/scss/alert";
// @import "node_modules/bootstrap/scss/badge";
// @import "node_modules/bootstrap/scss/breadcrumb";
// @import "node_modules/bootstrap/scss/carousel";
// @import "node_modules/bootstrap/scss/modal";

// 5. Utilities (selective import)
@import "node_modules/bootstrap/scss/utilities";

// 6. Custom utilities (add only what you need)
$custom-utilities: (
  "opacity": (
    property: opacity,
    values: (
      0: 0,
      25: .25,
      50: .5,
      75: .75,
      100: 1,
    )
  ),
  "line-height": (
    property: line-height,
    class: lh,
    values: (
      1: 1,
      sm: 1.25,
      base: 1.5,
      lg: 2,
    )
  )
);

$utilities: map-merge($utilities, $custom-utilities);
```

#### Package.json Build Configuration
```json
{
  "name": "bootstrap-optimized-project",
  "version": "1.0.0",
  "description": "Optimized Bootstrap project",
  "scripts": {
    "build:css": "sass src/scss/custom.scss dist/css/custom.min.css --style compressed",
    "build:js": "webpack --mode production",
    "build": "npm run build:css && npm run build:js",
    "watch:css": "sass --watch src/scss/custom.scss dist/css/custom.css",
    "serve": "live-server dist/",
    "optimize": "npm run build && npm run purge:css",
    "purge:css": "purgecss --css dist/css/custom.min.css --content dist/*.html --output dist/css/"
  },
  "devDependencies": {
    "sass": "^1.58.0",
    "webpack": "^5.75.0",
    "webpack-cli": "^5.0.1",
    "purgecss": "^5.0.0",
    "live-server": "^1.2.2"
  }
}
```

### JavaScript Optimization

Optimize Bootstrap JavaScript for better performance.

#### Custom JavaScript Build
```javascript
// custom-bootstrap.js - Import only needed plugins

// Core Bootstrap functionality
import { Toast } from 'bootstrap';
import { Modal } from 'bootstrap';
import { Dropdown } from 'bootstrap';
import { Collapse } from 'bootstrap';

// Don't import unused plugins:
// import { Carousel } from 'bootstrap';
// import { Accordion } from 'bootstrap';
// import { Alert } from 'bootstrap';
// import { Button } from 'bootstrap';
// import { Offcanvas } from 'bootstrap';
// import { Popover } from 'bootstrap';
// import { ScrollSpy } from 'bootstrap';
// import { Tab } from 'bootstrap';
// import { Tooltip } from 'bootstrap';

// Initialize components
document.addEventListener('DOMContentLoaded', function() {
    // Initialize modals
    const modals = document.querySelectorAll('.modal');
    modals.forEach(modal => new Modal(modal));
    
    // Initialize dropdowns
    const dropdowns = document.querySelectorAll('[data-bs-toggle="dropdown"]');
    dropdowns.forEach(dropdown => new Dropdown(dropdown));
    
    // Initialize toasts
    const toasts = document.querySelectorAll('.toast');
    toasts.forEach(toast => new Toast(toast));
    
    console.log('Custom Bootstrap initialized with minimal components');
});

// Export for external use
export { Toast, Modal, Dropdown, Collapse };
```

#### Webpack Configuration for Optimization
```javascript
// webpack.config.js
const path = require('path');

module.exports = {
    entry: './src/js/custom-bootstrap.js',
    output: {
        filename: 'bootstrap-custom.min.js',
        path: path.resolve(__dirname, 'dist/js'),
        clean: true
    },
    optimization: {
        minimize: true,
        usedExports: true,
        sideEffects: false
    },
    resolve: {
        alias: {
            bootstrap: path.resolve(__dirname, 'node_modules/bootstrap/js/dist')
        }
    },
    externals: {
        // Don't bundle if using CDN for other libraries
        jquery: 'jQuery'
    }
};
```

### Image and Media Optimization

Optimize images and media content for better performance.

```html
<div class="container">
    <!-- Optimized Images -->
    <section class="mb-5">
        <h4>Image Optimization Techniques</h4>
        
        <!-- Responsive images with lazy loading -->
        <div class="row g-4 mb-4">
            <div class="col-md-4">
                <div class="card">
                    <img src="https://via.placeholder.com/400x200/6f42c1/ffffff?text=WebP+Format" 
                         class="card-img-top" 
                         alt="WebP optimized image"
                         loading="lazy"
                         decoding="async">
                    <div class="card-body">
                        <h6 class="card-title">WebP Format</h6>
                        <p class="card-text small">
                            Modern image format with better compression
                        </p>
                    </div>
                </div>
            </div>
            
            <div class="col-md-4">
                <div class="card">
                    <picture>
                        <source srcset="https://via.placeholder.com/400x200/28a745/ffffff?text=Responsive+WebP" type="image/webp">
                        <img src="https://via.placeholder.com/400x200/28a745/ffffff?text=Responsive+JPG" 
                             class="card-img-top" 
                             alt="Responsive image with fallback"
                             loading="lazy">
                    </picture>
                    <div class="card-body">
                        <h6 class="card-title">Picture Element</h6>
                        <p class="card-text small">
                            Multiple sources with format fallbacks
                        </p>
                    </div>
                </div>
            </div>
            
            <div class="col-md-4">
                <div class="card">
                    <img src="data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 400 200'%3E%3Crect width='400' height='200' fill='%23ffc107'/%3E%3Ctext x='50%25' y='50%25' dominant-baseline='middle' text-anchor='middle' fill='%23000'%3ESVG Placeholder%3C/text%3E%3C/svg%3E" 
                         class="card-img-top" 
                         alt="SVG placeholder"
                         loading="lazy">
                    <div class="card-body">
                        <h6 class="card-title">SVG Placeholder</h6>
                        <p class="card-text small">
                            Lightweight SVG for placeholders
                        </p>
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Image optimization guidelines -->
        <div class="alert alert-primary">
            <h6 class="alert-heading">
                <i class="bi bi-image me-2"></i>Image Optimization Guidelines
            </h6>
            <ul class="mb-0">
                <li><strong>Format:</strong> Use WebP with JPEG/PNG fallback</li>
                <li><strong>Compression:</strong> Optimize file size without visible quality loss</li>
                <li><strong>Dimensions:</strong> Serve appropriate sizes for different breakpoints</li>
                <li><strong>Lazy Loading:</strong> Load images as they enter viewport</li>
                <li><strong>Alt Text:</strong> Always provide descriptive alt text</li>
            </ul>
        </div>
    </section>
    
    <!-- Performance Metrics Display -->
    <section class="mb-5">
        <h4>Performance Monitoring</h4>
        
        <div class="row g-4">
            <div class="col-md-3">
                <div class="card text-center border-primary">
                    <div class="card-body">
                        <div class="display-6 text-primary mb-2">
                            <i class="bi bi-speedometer2"></i>
                        </div>
                        <h6 class="card-title">Page Speed</h6>
                        <div class="h4 text-success" id="pageSpeed">98</div>
                        <small class="text-muted">Google PageSpeed Score</small>
                    </div>
                </div>
            </div>
            
            <div class="col-md-3">
                <div class="card text-center border-success">
                    <div class="card-body">
                        <div class="display-6 text-success mb-2">
                            <i class="bi bi-clock"></i>
                        </div>
                        <h6 class="card-title">Load Time</h6>
                        <div class="h4 text-info" id="loadTime">1.2s</div>
                        <small class="text-muted">First Contentful Paint</small>
                    </div>
                </div>
            </div>
            
            <div class="col-md-3">
                <div class="card text-center border-warning">
                    <div class="card-body">
                        <div class="display-6 text-warning mb-2">
                            <i class="bi bi-file-earmark"></i>
                        </div>
                        <h6 class="card-title">Bundle Size</h6>
                        <div class="h4 text-primary" id="bundleSize">45KB</div>
                        <small class="text-muted">Total CSS + JS (gzipped)</small>
                    </div>
                </div>
            </div>
            
            <div class="col-md-3">
                <div class="card text-center border-info">
                    <div class="card-body">
                        <div class="display-6 text-info mb-2">
                            <i class="bi bi-graph-up"></i>
                        </div>
                        <h6 class="card-title">CLS Score</h6>
                        <div class="h4 text-success" id="clsScore">0.05</div>
                        <small class="text-muted">Cumulative Layout Shift</small>
                    </div>
                </div>
            </div>
        </div>
    </section>
</div>
```

### Production Deployment Optimization

Optimize your Bootstrap site for production deployment.

```html
<div class="container">
    <!-- Production Checklist -->
    <div class="row g-4 mb-5">
        <div class="col-lg-6">
            <div class="card border-success">
                <div class="card-header bg-success text-white">
                    <h5 class="mb-0">
                        <i class="bi bi-rocket me-2"></i>Production Deployment
                    </h5>
                </div>
                <div class="card-body">
                    <div class="accordion" id="deploymentAccordion">
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button" type="button" 
                                        data-bs-toggle="collapse" data-bs-target="#step1">
                                    1. Build Optimization
                                </button>
                            </h2>
                            <div id="step1" class="accordion-collapse collapse show" 
                                 data-bs-parent="#deploymentAccordion">
                                <div class="accordion-body small">
                                    <ul class="mb-0">
                                        <li>Minify CSS and JavaScript files</li>
                                        <li>Compress images and optimize formats</li>
                                        <li>Remove unused CSS with PurgeCSS</li>
                                        <li>Bundle and tree-shake JavaScript</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button collapsed" type="button" 
                                        data-bs-toggle="collapse" data-bs-target="#step2">
                                    2. Server Configuration
                                </button>
                            </h2>
                            <div id="step2" class="accordion-collapse collapse" 
                                 data-bs-parent="#deploymentAccordion">
                                <div class="accordion-body small">
                                    <ul class="mb-0">
                                        <li>Enable gzip/brotli compression</li>
                                        <li>Set proper cache headers</li>
                                        <li>Configure HTTP/2 push</li>
                                        <li>Enable CDN for static assets</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button collapsed" type="button" 
                                        data-bs-toggle="collapse" data-bs-target="#step3">
                                    3. Performance Monitoring
                                </button>
                            </h2>
                            <div id="step3" class="accordion-collapse collapse" 
                                 data-bs-parent="#deploymentAccordion">
                                <div class="accordion-body small">
                                    <ul class="mb-0">
                                        <li>Set up Google Analytics</li>
                                        <li>Monitor Core Web Vitals</li>
                                        <li>Use Real User Monitoring (RUM)</li>
                                        <li>Regular performance audits</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="col-lg-6">
            <div class="card border-primary">
                <div class="card-header bg-primary text-white">
                    <h5 class="mb-0">
                        <i class="bi bi-gear me-2"></i>Build Tools & Automation
                    </h5>
                </div>
                <div class="card-body">
                    <h6>Recommended Build Stack:</h6>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                        <span class="badge bg-primary">Vite</span>
                        <span class="badge bg-success">Webpack</span>
                        <span class="badge bg-warning text-dark">Parcel</span>
                        <span class="badge bg-info">Rollup</span>
                    </div>
                    
                    <h6>CI/CD Integration:</h6>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                        <span class="badge bg-secondary">GitHub Actions</span>
                        <span class="badge bg-primary">Netlify</span>
                        <span class="badge bg-success">Vercel</span>
                        <span class="badge bg-warning text-dark">AWS</span>
                    </div>
                    
                    <h6>Performance Tools:</h6>
                    <div class="d-flex flex-wrap gap-2">
                        <span class="badge bg-info">Lighthouse CI</span>
                        <span class="badge bg-danger">Bundle Analyzer</span>
                        <span class="badge bg-dark text-white">WebPageTest</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
```

## 💻 Hands-On Practice

### Exercise 1: Complete Performance Optimization Showcase

Create `performance-optimization-showcase.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bootstrap Performance & Optimization</title>
    
    <!-- Optimized Bootstrap CSS (custom build) -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" 
          rel="stylesheet">
    
    <!-- Preload critical resources -->
    <link rel="preload" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" 
          as="style" onload="this.onload=null;this.rel='stylesheet'">
    
    <!-- Critical CSS inlined -->
    <style>
        .hero-section {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 70vh;
        }
        
        .performance-card {
            transition: all 0.3s ease;
            border: none;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }
        
        .performance-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 30px rgba(0,0,0,0.15);
        }
        
        .metric-display {
            font-size: 2.5rem;
            font-weight: 700;
            line-height: 1;
        }
        
        .optimization-icon {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 2rem;
            margin: 0 auto 1rem;
        }
        
        .before-after {
            background: repeating-linear-gradient(
                45deg,
                transparent,
                transparent 10px,
                rgba(255,255,255,0.1) 10px,
                rgba(255,255,255,0.1) 20px
            );
        }
        
        .performance-gauge {
            position: relative;
            width: 120px;
            height: 120px;
            margin: 0 auto;
        }
        
        .gauge-circle {
            transform: rotate(-90deg);
        }
        
        .loading-simulation {
            height: 4px;
            background: #e9ecef;
            border-radius: 2px;
            overflow: hidden;
        }
        
        .loading-bar {
            height: 100%;
            background: linear-gradient(90deg, #28a745, #20c997);
            border-radius: 2px;
            animation: loadProgress 3s ease-in-out;
        }
        
        @keyframes loadProgress {
            0% { width: 0%; }
            100% { width: 100%; }
        }
        
        .optimization-tip {
            border-left: 4px solid #28a745;
            background: rgba(40, 167, 69, 0.1);
        }
        
        .code-block {
            background: #f8f9fa;
            border: 1px solid #e9ecef;
            border-radius: 6px;
            padding: 1rem;
            font-family: 'Courier New', monospace;
            font-size: 0.875rem;
            overflow-x: auto;
        }
    </style>
</head>
<body>
    <!-- Hero Section -->
    <header class="hero-section text-white d-flex align-items-center">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-lg-8">
                    <h1 class="display-3 fw-bold mb-4">
                        Performance & Optimization
                    </h1>
                    <p class="lead fs-4 mb-4">
                        Master Bootstrap optimization techniques to create lightning-fast, 
                        production-ready websites that deliver exceptional user experiences.
                    </p>
                    <div class="d-flex flex-wrap gap-3 mb-4">
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Tree Shaking</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Custom Builds</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Performance</span>
                        <span class="badge bg-light text-dark fs-6 px-3 py-2">Production</span>
                    </div>
                    
                    <!-- Loading simulation -->
                    <div class="mb-3">
                        <small class="text-light opacity-75">Page Load Simulation:</small>
                        <div class="loading-simulation">
                            <div class="loading-bar"></div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 text-center">
                    <div class="performance-gauge">
                        <svg width="120" height="120" class="gauge-circle">
                            <circle cx="60" cy="60" r="50" fill="none" stroke="rgba(255,255,255,0.2)" stroke-width="8"/>
                            <circle cx="60" cy="60" r="50" fill="none" stroke="#28a745" stroke-width="8" 
                                    stroke-dasharray="314" stroke-dashoffset="31.4" stroke-linecap="round">
                                <animate attributeName="stroke-dashoffset" values="314;31.4" dur="2s" fill="freeze"/>
                            </circle>
                        </svg>
                        <div class="position-absolute top-50 start-50 translate-middle text-center">
                            <div class="metric-display">95</div>
                            <small>Performance Score</small>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </header>
    
    <!-- Main Content -->
    <main class="py-5">
        <div class="container">
            <!-- Performance Metrics -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Performance Metrics</h2>
                        <p class="lead text-muted">
                            Real-time performance indicators and optimization results
                        </p>
                    </div>
                </div>
                
                <div class="row g-4">
                    <div class="col-md-6 col-lg-3">
                        <div class="performance-card card text-center">
                            <div class="card-body">
                                <div class="optimization-icon bg-primary text-white">
                                    <i class="bi bi-speedometer2"></i>
                                </div>
                                <h5 class="card-title">Page Speed</h5>
                                <div class="metric-display text-success">98</div>
                                <p class="text-muted mb-0">Google PageSpeed Score</p>
                                <small class="text-success">
                                    <i class="bi bi-arrow-up"></i> +15 improvement
                                </small>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-md-6 col-lg-3">
                        <div class="performance-card card text-center">
                            <div class="card-body">
                                <div class="optimization-icon bg-success text-white">
                                    <i class="bi bi-clock"></i>
                                </div>
                                <h5 class="card-title">Load Time</h5>
                                <div class="metric-display text-info">1.2s</div>
                                <p class="text-muted mb-0">First Contentful Paint</p>
                                <small class="text-success">
                                    <i class="bi bi-arrow-down"></i> -2.3s faster
                                </small>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-md-6 col-lg-3">
                        <div class="performance-card card text-center">
                            <div class="card-body">
                                <div class="optimization-icon bg-warning text-dark">
                                    <i class="bi bi-file-earmark"></i>
                                </div>
                                <h5 class="card-title">Bundle Size</h5>
                                <div class="metric-display text-primary">45KB</div>
                                <p class="text-muted mb-0">Total CSS + JS (gzipped)</p>
                                <small class="text-success">
                                    <i class="bi bi-arrow-down"></i> -75% reduction
                                </small>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-md-6 col-lg-3">
                        <div class="performance-card card text-center">
                            <div class="card-body">
                                <div class="optimization-icon bg-info text-white">
                                    <i class="bi bi-graph-up"></i>
                                </div>
                                <h5 class="card-title">Core Web Vitals</h5>
                                <div class="d-flex justify-content-around text-center">
                                    <div>
                                        <div class="fw-bold text-success">Good</div>
                                        <small class="text-muted">LCP</small>
                                    </div>
                                    <div>
                                        <div class="fw-bold text-success">Good</div>
                                        <small class="text-muted">FID</small>
                                    </div>
                                    <div>
                                        <div class="fw-bold text-success">Good</div>
                                        <small class="text-muted">CLS</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            
            <!-- Optimization Techniques -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Optimization Techniques</h2>
                        <p class="lead text-muted">
                            Proven strategies to boost your Bootstrap site performance
                        </p>
                    </div>
                </div>
                
                <div class="row g-4">
                    <!-- Custom Builds -->
                    <div class="col-lg-6">
                        <div class="performance-card card h-100">
                            <div class="card-header bg-primary text-white">
                                <h5 class="mb-0">
                                    <i class="bi bi-scissors me-2"></i>Custom Bootstrap Builds
                                </h5>
                            </div>
                            <div class="card-body">
                                <p class="card-text">
                                    Create tailored Bootstrap builds with only the components you need, 
                                    reducing bundle size by up to 80%.
                                </p>
                                
                                <div class="optimization-tip p-3 rounded mb-3">
                                    <h6 class="text-success mb-2">
                                        <i class="bi bi-lightbulb me-2"></i>Pro Tip
                                    </h6>
                                    <p class="mb-0 small">
                                        Use SASS imports to include only necessary components. 
                                        A typical custom build can be under 30KB!
                                    </p>
                                </div>
                                
                                <div class="code-block mb-3">
<pre class="mb-0">// custom.scss
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";
@import "bootstrap/scss/grid";
@import "bootstrap/scss/buttons";
// Skip unused components</pre>
                                </div>
                                
                                <div class="before-after p-3 rounded">
                                    <div class="row text-center">
                                        <div class="col-6">
                                            <div class="text-danger fw-bold">Before</div>
                                            <div>180KB CSS</div>
                                        </div>
                                        <div class="col-6">
                                            <div class="text-success fw-bold">After</div>
                                            <div>35KB CSS</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Tree Shaking -->
                    <div class="col-lg-6">
                        <div class="performance-card card h-100">
                            <div class="card-header bg-success text-white">
                                <h5 class="mb-0">
                                    <i class="bi bi-tree me-2"></i>JavaScript Tree Shaking
                                </h5>
                            </div>
                            <div class="card-body">
                                <p class="card-text">
                                    Remove unused JavaScript code automatically during the build process, 
                                    keeping only the functions your site actually uses.
                                </p>
                                
                                <div class="optimization-tip p-3 rounded mb-3">
                                    <h6 class="text-success mb-2">
                                        <i class="bi bi-lightbulb me-2"></i>Pro Tip
                                    </h6>
                                    <p class="mb-0 small">
                                        Use ES6 imports and modern bundlers like Webpack or Vite 
                                        for automatic dead code elimination.
                                    </p>
                                </div>
                                
                                <div class="code-block mb-3">
<pre class="mb-0">// Import only what you need
import { Modal, Dropdown } from 'bootstrap';

// Instead of importing everything
// import * from 'bootstrap';</pre>
                                </div>
                                
                                <div class="before-after p-3 rounded">
                                    <div class="row text-center">
                                        <div class="col-6">
                                            <div class="text-danger fw-bold">Before</div>
                                            <div>150KB JS</div>
                                        </div>
                                        <div class="col-6">
                                            <div class="text-success fw-bold">After</div>
                                            <div>45KB JS</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Image Optimization -->
                    <div class="col-lg-6">
                        <div class="performance-card card h-100">
                            <div class="card-header bg-warning text-dark">
                                <h5 class="mb-0">
                                    <i class="bi bi-image me-2"></i>Image Optimization
                                </h5>
                            </div>
                            <div class="card-body">
                                <p class="card-text">
                                    Optimize images with modern formats, lazy loading, and responsive 
                                    sizing to dramatically improve load times.
                                </p>
                                
                                <div class="row g-3 mb-3">
                                    <div class="col-4 text-center">
                                        <div class="bg-primary text-white rounded p-2 mb-2">
                                            <i class="bi bi-file-image"></i>
                                        </div>
                                        <small>WebP Format</small>
                                    </div>
                                    <div class="col-4 text-center">
                                        <div class="bg-success text-white rounded p-2 mb-2">
                                            <i class="bi bi-eye-slash"></i>
                                        </div>
                                        <small>Lazy Loading</small>
                                    </div>
                                    <div class="col-4 text-center">
                                        <div class="bg-info text-white rounded p-2 mb-2">
                                            <i class="bi bi-arrows-fullscreen"></i>
                                        </div>
                                        <small>Responsive</small>
                                    </div>
                                </div>
                                
                                <div class="code-block mb-3">
<pre class="mb-0">&lt;img src="image.webp" 
     loading="lazy"
     class="img-fluid"
     alt="Optimized image"&gt;</pre>
                                </div>
                                
                                <div class="before-after p-3 rounded">
                                    <div class="row text-center">
                                        <div class="col-6">
                                            <div class="text-danger fw-bold">Before</div>
                                            <div>500KB Images</div>
                                        </div>
                                        <div class="col-6">
                                            <div class="text-success fw-bold">After</div>
                                            <div>125KB Images</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- CDN & Caching -->
                    <div class="col-lg-6">
                        <div class="performance-card card h-100">
                            <div class="card-header bg-info text-white">
                                <h5 class="mb-0">
                                    <i class="bi bi-cloud me-2"></i>CDN & Caching
                                </h5>
                            </div>
                            <div class="card-body">
                                <p class="card-text">
                                    Leverage CDNs and browser caching to serve assets faster 
                                    and reduce server load for repeat visitors.
                                </p>
                                
                                <div class="row g-3 mb-3">
                                    <div class="col-4 text-center">
                                        <div class="bg-primary text-white rounded p-2 mb-2">
                                            <i class="bi bi-globe"></i>
                                        </div>
                                        <small>Global CDN</small>
                                    </div>
                                    <div class="col-4 text-center">
                                        <div class="bg-success text-white rounded p-2 mb-2">
                                            <i class="bi bi-arrow-clockwise"></i>
                                        </div>
                                        <small>Browser Cache</small>
                                    </div>
                                    <div class="col-4 text-center">
                                        <div class="bg-warning text-dark rounded p-2 mb-2">
                                            <i class="bi bi-lightning"></i>
                                        </div>
                                        <small>Edge Servers</small>
                                    </div>
                                </div>
                                
                                <div class="optimization-tip p-3 rounded mb-3">
                                    <h6 class="text-success mb-2">
                                        <i class="bi bi-lightbulb me-2"></i>Pro Tip
                                    </h6>
                                    <p class="mb-0 small">
                                        Use jsDelivr or cdnjs for Bootstrap assets. Enable 
                                        long-term caching with proper headers.
                                    </p>
                                </div>
                                
                                <div class="before-after p-3 rounded">
                                    <div class="row text-center">
                                        <div class="col-6">
                                            <div class="text-danger fw-bold">Before</div>
                                            <div>3.5s Load</div>
                                        </div>
                                        <div class="col-6">
                                            <div class="text-success fw-bold">After</div>
                                            <div>0.8s Load</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            
            <!-- Build Tools & Workflow -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Build Tools & Workflow</h2>
                        <p class="lead text-muted">
                            Modern build tools and automation for optimized production builds
                        </p>
                    </div>
                </div>
                
                <div class="row g-4">
                    <div class="col-lg-4">
                        <div class="performance-card card text-center h-100">
                            <div class="card-body">
                                <div class="optimization-icon bg-primary text-white">
                                    <i class="bi bi-lightning-charge"></i>
                                </div>
                                <h5 class="card-title">Vite</h5>
                                <p class="card-text">
                                    Lightning-fast development server with instant HMR 
                                    and optimized production builds.
                                </p>
                                <div class="d-flex justify-content-center gap-2 mb-3">
                                    <span class="badge bg-success">Fast</span>
                                    <span class="badge bg-primary">Modern</span>
                                </div>
                                <button class="btn btn-primary btn-sm">Learn More</button>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-lg-4">
                        <div class="performance-card card text-center h-100">
                            <div class="card-body">
                                <div class="optimization-icon bg-success text-white">
                                    <i class="bi bi-box"></i>
                                </div>
                                <h5 class="card-title">Webpack</h5>
                                <p class="card-text">
                                    Powerful bundler with advanced optimization features, 
                                    code splitting, and tree shaking.
                                </p>
                                <div class="d-flex justify-content-center gap-2 mb-3">
                                    <span class="badge bg-warning text-dark">Flexible</span>
                                    <span class="badge bg-info">Powerful</span>
                                </div>
                                <button class="btn btn-success btn-sm">Learn More</button>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-lg-4">
                        <div class="performance-card card text-center h-100">
                            <div class="card-body">
                                <div class="optimization-icon bg-warning text-dark">
                                    <i class="bi bi-gift"></i>
                                </div>
                                <h5 class="card-title">Parcel</h5>
                                <p class="card-text">
                                    Zero-configuration build tool that automatically 
                                    optimizes your Bootstrap projects.
                                </p>
                                <div class="d-flex justify-content-center gap-2 mb-3">
                                    <span class="badge bg-primary">Simple</span>
                                    <span class="badge bg-secondary">Auto</span>
                                </div>
                                <button class="btn btn-warning btn-sm">Learn More</button>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            
            <!-- Performance Testing -->
            <section class="mb-5">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h2 class="display-5 mb-3">Performance Testing Tools</h2>
                        <p class="lead text-muted">
                            Essential tools for measuring and monitoring your site's performance
                        </p>
                    </div>
                </div>
                
                <div class="row g-4">
                    <div class="col-md-6">
                        <div class="performance-card card">
                            <div class="card-header bg-dark text-white">
                                <h5 class="mb-0">
                                    <i class="bi bi-speedometer me-2"></i>Testing Tools
                                </h5>
                            </div>
                            <div class="card-body">
                                <div class="row g-3">
                                    <div class="col-6">
                                        <div class="text-center">
                                            <div class="bg-primary text-white rounded p-3 mb-2">
                                                <i class="bi bi-google"></i>
                                            </div>
                                            <h6>Lighthouse</h6>
                                            <small class="text-muted">Google's audit tool</small>
                                        </div>
                                    </div>
                                    <div class="col-6">
                                        <div class="text-center">
                                            <div class="bg-success text-white rounded p-3 mb-2">
                                                <i class="bi bi-graph-up"></i>
                                            </div>
                                            <h6>WebPageTest</h6>
                                            <small class="text-muted">Detailed analysis</small>
                                        </div>
                                    </div>
                                    <div class="col-6">
                                        <div class="text-center">
                                            <div class="bg-warning text-dark rounded p-3 mb-2">
                                                <i class="bi bi-bar-chart"></i>
                                            </div>
                                            <h6>GTmetrix</h6>
                                            <small class="text-muted">Performance monitoring</small>
                                        </div>
                                    </div>
                                    <div class="col-6">
                                        <div class="text-center">
                                            <div class="bg-info text-white rounded p-3 mb-2">
                                                <i class="bi bi-cpu"></i>
                                            </div>
                                            <h6>DevTools</h6>
                                            <small class="text-muted">Browser profiling</small>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-md-6">
                        <div class="performance-card card">
                            <div class="card-header bg-primary text-white">
                                <h5 class="mb-0">
                                    <i class="bi bi-clipboard-check me-2"></i>Optimization Checklist
                                </h5>
                            </div>
                            <div class="card-body">
                                <div class="form-check mb-2">
                                    <input class="form-check-input" type="checkbox" id="opt1" checked>
                                    <label class="form-check-label" for="opt1">
                                        ✅ Custom Bootstrap build implemented
                                    </label>
                                </div>
                                <div class="form-check mb-2">
                                    <input class="form-check-input" type="checkbox" id="opt2" checked>
                                    <label class="form-check-label" for="opt2">
                                        ✅ JavaScript tree shaking enabled
                                    </label>
                                </div>
                                <div class="form-check mb-2">
                                    <input class="form-check-input" type="checkbox" id="opt3" checked>
                                    <label class="form-check-label" for="opt3">
                                        ✅ Images optimized and lazy-loaded
                                    </label>
                                </div>
                                <div class="form-check mb-2">
                                    <input class="form-check-input" type="checkbox" id="opt4" checked>
                                    <label class="form-check-label" for="opt4">
                                        ✅ CDN and caching configured
                                    </label>
                                </div>
                                <div class="form-check mb-2">
                                    <input class="form-check-input" type="checkbox" id="opt5" checked>
                                    <label class="form-check-label" for="opt5">
                                        ✅ Compression enabled (gzip/brotli)
                                    </label>
                                </div>
                                <div class="form-check mb-2">
                                    <input class="form-check-input" type="checkbox" id="opt6" checked>
                                    <label class="form-check-label" for="opt6">
                                        ✅ Performance monitoring setup
                                    </label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" id="opt7" checked>
                                    <label class="form-check-label" for="opt7">
                                        ✅ Core Web Vitals optimized
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </main>
    
    <!-- Footer -->
    <footer class="bg-dark text-white py-5">
        <div class="container">
            <div class="row">
                <div class="col-lg-8">
                    <h3 class="display-6 mb-3">Performance Optimization Complete!</h3>
                    <p class="lead mb-3">
                        You've mastered Bootstrap performance optimization. Your sites now 
                        load lightning-fast and provide exceptional user experiences.
                    </p>
                    <div class="d-flex flex-wrap gap-2">
                        <span class="badge bg-primary fs-6 px-3 py-2">95+ PageSpeed</span>
                        <span class="badge bg-success fs-6 px-3 py-2">75% Smaller</span>
                        <span class="badge bg-warning text-dark fs-6 px-3 py-2">3x Faster</span>
                        <span class="badge bg-info text-dark fs-6 px-3 py-2">Production Ready</span>
                    </div>
                </div>
                <div class="col-lg-4 text-lg-end">
                    <h5 class="text-light">Ready for the final challenge?</h5>
                    <p class="text-light opacity-75">
                        Tomorrow we'll build a complete landing page project!
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
                        Optimized with Bootstrap 5.3.2 • 45KB Total Size
                    </small>
                </div>
            </div>
        </div>
    </footer>
    
    <!-- Optimized Bootstrap JavaScript (only what we need) -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js" 
            defer></script>
    
    <!-- Custom JavaScript -->
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // Performance monitoring simulation
            function simulatePerformanceMetrics() {
                const metrics = {
                    pageSpeed: Math.floor(Math.random() * 5) + 95,
                    loadTime: (Math.random() * 0.5 + 1.0).toFixed(1),
                    bundleSize: Math.floor(Math.random() * 10) + 40,
                    clsScore: (Math.random() * 0.05 + 0.01).toFixed(3)
                };
                
                // Update metrics if elements exist
                const elements = {
                    pageSpeed: document.getElementById('pageSpeed'),
                    loadTime: document.getElementById('loadTime'),
                    bundleSize: document.getElementById('bundleSize'),
                    clsScore: document.getElementById('clsScore')
                };
                
                Object.keys(elements).forEach(key => {
                    if (elements[key]) {
                        elements[key].textContent = metrics[key] + 
                            (key === 'loadTime' ? 's' : key === 'bundleSize' ? 'KB' : '');
                    }
                });
            }
            
            // Update metrics every 5 seconds
            simulatePerformanceMetrics();
            setInterval(simulatePerformanceMetrics, 5000);
            
            // Animate performance cards on scroll
            const observerOptions = {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            };
            
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.style.opacity = '1';
                        entry.target.style.transform = 'translateY(0)';
                    }
                });
            }, observerOptions);
            
            // Observe performance cards
            document.querySelectorAll('.performance-card').forEach(card => {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'all 0.6s ease';
                observer.observe(card);
            });
            
            // Loading animation restart on click
            document.querySelector('.loading-simulation')?.addEventListener('click', function() {
                const bar = this.querySelector('.loading-bar');
                bar.style.animation = 'none';
                bar.offsetHeight; // Trigger reflow
                bar.style.animation = 'loadProgress 3s ease-in-out';
            });
            
            console.log('Performance Optimization Demo Loaded! ⚡');
        });
    </script>
</body>
</html>
```

## 📋 Performance & Optimization Mastery Checklist

### Build Optimization
- [ ] Create custom Bootstrap builds with only needed components
- [ ] Implement SASS-based customization and optimization
- [ ] Configure build tools (Webpack, Vite, or Parcel)
- [ ] Enable JavaScript tree shaking and dead code elimination

### Asset Optimization
- [ ] Optimize and compress images (WebP format preferred)
- [ ] Implement lazy loading for images and media
- [ ] Minify CSS and JavaScript files
- [ ] Use appropriate image sizes for different breakpoints

### Delivery Optimization
- [ ] Configure CDN for Bootstrap assets
- [ ] Enable gzip/brotli compression
- [ ] Set proper caching headers
- [ ] Implement HTTP/2 and resource hints

### Performance Monitoring
- [ ] Set up Google Lighthouse audits
- [ ] Monitor Core Web Vitals (LCP, FID, CLS)
- [ ] Use Real User Monitoring (RUM)
- [ ] Regular performance testing and optimization

### Production Deployment
- [ ] Optimize bundle sizes for production
- [ ] Configure proper server settings
- [ ] Implement performance budgets
- [ ] Set up automated performance testing

## 🎉 Day 13 Complete!

### ✅ Today's Achievements
- **Custom Builds:** Create optimized Bootstrap builds with only needed components
- **Tree Shaking:** Eliminate unused JavaScript code automatically
- **Asset Optimization:** Optimize images, CSS, and JavaScript for maximum performance
- **CDN & Caching:** Leverage content delivery networks and browser caching
- **Performance Monitoring:** Set up tools to measure and track performance metrics
- **Production Deployment:** Deploy optimized Bootstrap sites to production

### 🏆 Skills Gained
- Bootstrap performance optimization techniques
- Custom build creation with SASS
- Modern build tool configuration (Webpack, Vite, Parcel)
- Asset optimization and compression strategies
- Performance monitoring and measurement
- Production deployment best practices

## 🔗 Additional Resources

- **[Bootstrap Customize Docs](https://getbootstrap.com/docs/5.3/customize/overview/)** - Official customization guide
- **[Web.dev Performance](https://web.dev/performance/)** - Google's performance best practices
- **[Lighthouse](https://developers.google.com/web/tools/lighthouse)** - Google's performance audit tool
- **[WebPageTest](https://www.webpagetest.org/)** - Detailed performance testing

---

## 🚀 What's Next?

Tomorrow in **Day 14: Landing Page Project**, you'll apply everything you've learned to build a complete, professional landing page:
- Modern responsive design
- Custom Bootstrap optimization
- Advanced components and interactions
- Performance-optimized deployment

**Outstanding work on Day 13!** You now have the skills to create lightning-fast Bootstrap sites that perform exceptionally well across all devices and network conditions.

---

**Remember: Performance is not just about speed – it's about providing the best possible user experience!** ⚡✨