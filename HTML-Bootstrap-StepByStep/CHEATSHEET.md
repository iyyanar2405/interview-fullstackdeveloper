# HTML Bootstrap Cheatsheet 📋

**Quick Reference Guide for HTML5 and Bootstrap Development**

---

## 🏗️ HTML5 Document Structure

### Basic Template
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="Page description">
    <title>Page Title</title>
</head>
<body>
    <!-- Content here -->
</body>
</html>
```

### Essential Meta Tags
```html
<!-- Character encoding -->
<meta charset="UTF-8">

<!-- Responsive viewport -->
<meta name="viewport" content="width=device-width, initial-scale=1.0">

<!-- SEO description -->
<meta name="description" content="Page description for search engines">

<!-- Keywords (less important now) -->
<meta name="keywords" content="keyword1, keyword2, keyword3">

<!-- Author -->
<meta name="author" content="Your Name">

<!-- Open Graph for social media -->
<meta property="og:title" content="Page Title">
<meta property="og:description" content="Page description">
<meta property="og:image" content="image-url.jpg">
<meta property="og:url" content="https://yoursite.com">

<!-- Twitter Cards -->
<meta name="twitter:card" content="summary_large_image">
<meta name="twitter:title" content="Page Title">
<meta name="twitter:description" content="Page description">
```

---

## 📝 HTML5 Semantic Elements

### Document Structure
```html
<header>      <!-- Site/page header -->
<nav>         <!-- Navigation links -->
<main>        <!-- Primary content -->
<section>     <!-- Thematic content group -->
<article>     <!-- Standalone content -->
<aside>       <!-- Sidebar content -->
<footer>      <!-- Site/page footer -->
```

### Content Elements
```html
<h1> to <h6>  <!-- Headings (h1 most important) -->
<p>           <!-- Paragraphs -->
<div>         <!-- Generic container -->
<span>        <!-- Inline container -->
<br>          <!-- Line break -->
<hr>          <!-- Horizontal rule -->
```

### Text Formatting
```html
<strong>      <!-- Important text -->
<em>          <!-- Emphasized text -->
<b>           <!-- Bold text (visual only) -->
<i>           <!-- Italic text (visual only) -->
<u>           <!-- Underlined text -->
<s>           <!-- Strikethrough text -->
<mark>        <!-- Highlighted text -->
<small>       <!-- Small text -->
<sub>         <!-- Subscript -->
<sup>         <!-- Superscript -->
<del>         <!-- Deleted text -->
<ins>         <!-- Inserted text -->
```

### Lists
```html
<!-- Unordered list -->
<ul>
    <li>Item 1</li>
    <li>Item 2</li>
</ul>

<!-- Ordered list -->
<ol>
    <li>First item</li>
    <li>Second item</li>
</ol>

<!-- Description list -->
<dl>
    <dt>Term</dt>
    <dd>Definition</dd>
</dl>
```

### Links and Navigation
```html
<!-- Basic link -->
<a href="https://example.com">Link text</a>

<!-- Email link -->
<a href="mailto:email@example.com">Email</a>

<!-- Phone link -->
<a href="tel:+1234567890">Phone</a>

<!-- Internal link -->
<a href="#section-id">Jump to section</a>

<!-- Download link -->
<a href="file.pdf" download>Download PDF</a>

<!-- External link (opens in new tab) -->
<a href="https://example.com" target="_blank" rel="noopener">External</a>
```

### Images and Media
```html
<!-- Basic image -->
<img src="image.jpg" alt="Description" width="300" height="200">

<!-- Responsive image -->
<img src="image.jpg" alt="Description" style="max-width: 100%; height: auto;">

<!-- Figure with caption -->
<figure>
    <img src="image.jpg" alt="Description">
    <figcaption>Image caption</figcaption>
</figure>

<!-- Audio -->
<audio controls>
    <source src="audio.mp3" type="audio/mpeg">
    Your browser does not support audio.
</audio>

<!-- Video -->
<video controls width="640" height="360">
    <source src="video.mp4" type="video/mp4">
    Your browser does not support video.
</video>
```

### Forms
```html
<form action="/submit" method="post">
    <!-- Text input -->
    <label for="name">Name:</label>
    <input type="text" id="name" name="name" required>
    
    <!-- Email input -->
    <label for="email">Email:</label>
    <input type="email" id="email" name="email" required>
    
    <!-- Password input -->
    <label for="password">Password:</label>
    <input type="password" id="password" name="password" required>
    
    <!-- Textarea -->
    <label for="message">Message:</label>
    <textarea id="message" name="message" rows="4" cols="50"></textarea>
    
    <!-- Select dropdown -->
    <label for="country">Country:</label>
    <select id="country" name="country">
        <option value="us">United States</option>
        <option value="uk">United Kingdom</option>
        <option value="ca">Canada</option>
    </select>
    
    <!-- Radio buttons -->
    <fieldset>
        <legend>Choose size:</legend>
        <input type="radio" id="small" name="size" value="small">
        <label for="small">Small</label>
        <input type="radio" id="large" name="size" value="large">
        <label for="large">Large</label>
    </fieldset>
    
    <!-- Checkboxes -->
    <input type="checkbox" id="subscribe" name="subscribe" value="yes">
    <label for="subscribe">Subscribe to newsletter</label>
    
    <!-- Submit button -->
    <button type="submit">Submit</button>
</form>
```

### Tables
```html
<table>
    <caption>Table Caption</caption>
    <thead>
        <tr>
            <th scope="col">Header 1</th>
            <th scope="col">Header 2</th>
            <th scope="col">Header 3</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Data 1</td>
            <td>Data 2</td>
            <td>Data 3</td>
        </tr>
        <tr>
            <td>Data 4</td>
            <td>Data 5</td>
            <td>Data 6</td>
        </tr>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="3">Footer content</td>
        </tr>
    </tfoot>
</table>
```

---

## 🎨 Bootstrap 5 Quick Reference

### CDN Links
```html
<!-- CSS -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">

<!-- JavaScript Bundle -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
```

### Grid System
```html
<div class="container">           <!-- Fixed width container -->
<div class="container-fluid">     <!-- Full width container -->

<div class="row">                 <!-- Row wrapper -->
    <div class="col">             <!-- Equal width column -->
    <div class="col-6">           <!-- 6/12 width column -->
    <div class="col-md-4">        <!-- 4/12 width on medium+ screens -->
    <div class="col-lg-3">        <!-- 3/12 width on large+ screens -->
</div>
```

### Breakpoints
```
xs: <576px    (extra small)
sm: ≥576px    (small)
md: ≥768px    (medium)
lg: ≥992px    (large)
xl: ≥1200px   (extra large)
xxl: ≥1400px  (extra extra large)
```

### Typography
```html
<h1 class="display-1">Display heading</h1>
<p class="lead">Lead paragraph</p>
<p class="text-muted">Muted text</p>
<p class="text-primary">Primary color text</p>
<p class="text-center">Centered text</p>
<p class="text-uppercase">Uppercase text</p>
<p class="fw-bold">Bold text</p>
<p class="fst-italic">Italic text</p>
```

### Colors
```html
<!-- Text colors -->
.text-primary    .text-secondary    .text-success
.text-danger     .text-warning      .text-info
.text-light      .text-dark         .text-muted

<!-- Background colors -->
.bg-primary      .bg-secondary      .bg-success
.bg-danger       .bg-warning        .bg-info
.bg-light        .bg-dark           .bg-white
```

### Spacing
```html
<!-- Margin: m-{size} -->
.m-0    .m-1    .m-2    .m-3    .m-4    .m-5
.mt-3   .mb-2   .ms-1   .me-4   .mx-2   .my-3

<!-- Padding: p-{size} -->
.p-0    .p-1    .p-2    .p-3    .p-4    .p-5
.pt-3   .pb-2   .ps-1   .pe-4   .px-2   .py-3
```

### Display
```html
.d-none          .d-inline         .d-inline-block
.d-block         .d-table          .d-flex
.d-inline-flex   .d-grid

<!-- Responsive display -->
.d-sm-block      .d-md-none        .d-lg-flex
```

### Flexbox
```html
.d-flex                    <!-- Enable flexbox -->
.flex-row                  <!-- Horizontal direction -->
.flex-column              <!-- Vertical direction -->
.justify-content-center   <!-- Center horizontally -->
.justify-content-between  <!-- Space between items -->
.align-items-center       <!-- Center vertically -->
.align-items-start        <!-- Align to start -->
.flex-wrap                <!-- Allow wrapping -->
```

### Buttons
```html
<button class="btn btn-primary">Primary</button>
<button class="btn btn-secondary">Secondary</button>
<button class="btn btn-success">Success</button>
<button class="btn btn-danger">Danger</button>
<button class="btn btn-warning">Warning</button>
<button class="btn btn-info">Info</button>
<button class="btn btn-light">Light</button>
<button class="btn btn-dark">Dark</button>

<!-- Button sizes -->
<button class="btn btn-primary btn-lg">Large</button>
<button class="btn btn-primary">Normal</button>
<button class="btn btn-primary btn-sm">Small</button>

<!-- Outline buttons -->
<button class="btn btn-outline-primary">Outline</button>
```

### Cards
```html
<div class="card">
    <img src="..." class="card-img-top" alt="...">
    <div class="card-body">
        <h5 class="card-title">Card title</h5>
        <p class="card-text">Card content</p>
        <a href="#" class="btn btn-primary">Button</a>
    </div>
    <div class="card-footer">
        <small class="text-muted">Footer text</small>
    </div>
</div>
```

### Navigation
```html
<!-- Basic navbar -->
<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container">
        <a class="navbar-brand" href="#">Brand</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav">
                <li class="nav-item">
                    <a class="nav-link active" href="#">Home</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#">About</a>
                </li>
            </ul>
        </div>
    </div>
</nav>
```

### Forms
```html
<div class="mb-3">
    <label for="email" class="form-label">Email</label>
    <input type="email" class="form-control" id="email">
</div>

<div class="mb-3">
    <label for="select" class="form-label">Select</label>
    <select class="form-select" id="select">
        <option>Choose...</option>
        <option value="1">One</option>
        <option value="2">Two</option>
    </select>
</div>

<div class="mb-3 form-check">
    <input type="checkbox" class="form-check-input" id="check">
    <label class="form-check-label" for="check">Check me</label>
</div>
```

### Alerts
```html
<div class="alert alert-primary" role="alert">Primary alert</div>
<div class="alert alert-danger" role="alert">Danger alert</div>
<div class="alert alert-success alert-dismissible fade show" role="alert">
    Success alert with close button
    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
</div>
```

### Modals
```html
<!-- Trigger button -->
<button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#myModal">
    Launch modal
</button>

<!-- Modal -->
<div class="modal fade" id="myModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Modal title</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                Modal content goes here
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary">Save</button>
            </div>
        </div>
    </div>
</div>
```

---

## 🎯 Common HTML Patterns

### Hero Section
```html
<section class="hero">
    <div class="container">
        <h1>Main Headline</h1>
        <p class="lead">Supporting text that explains the value proposition</p>
        <a href="#" class="btn btn-primary btn-lg">Call to Action</a>
    </div>
</section>
```

### Feature Section
```html
<section class="features">
    <div class="container">
        <div class="row">
            <div class="col-md-4">
                <h3>Feature 1</h3>
                <p>Description of feature 1</p>
            </div>
            <div class="col-md-4">
                <h3>Feature 2</h3>
                <p>Description of feature 2</p>
            </div>
            <div class="col-md-4">
                <h3>Feature 3</h3>
                <p>Description of feature 3</p>
            </div>
        </div>
    </div>
</section>
```

### Contact Form
```html
<form class="needs-validation" novalidate>
    <div class="row">
        <div class="col-md-6 mb-3">
            <label for="firstName">First name</label>
            <input type="text" class="form-control" id="firstName" required>
        </div>
        <div class="col-md-6 mb-3">
            <label for="lastName">Last name</label>
            <input type="text" class="form-control" id="lastName" required>
        </div>
    </div>
    <div class="mb-3">
        <label for="email">Email</label>
        <input type="email" class="form-control" id="email" required>
    </div>
    <div class="mb-3">
        <label for="message">Message</label>
        <textarea class="form-control" id="message" rows="5" required></textarea>
    </div>
    <button class="btn btn-primary" type="submit">Send Message</button>
</form>
```

---

## 🔍 Debugging Tips

### HTML Validation
- Use W3C Markup Validator: https://validator.w3.org/
- Check for unclosed tags
- Verify proper nesting
- Ensure all required attributes are present

### Common Mistakes
- Missing alt attributes on images
- Incorrect heading hierarchy
- Using deprecated elements
- Missing form labels
- Improper semantic element usage

### Browser Developer Tools
- **F12** or **Ctrl+Shift+I** to open DevTools
- **Elements tab**: Inspect HTML structure
- **Console tab**: Check for JavaScript errors
- **Network tab**: Monitor loading times
- **Lighthouse tab**: Audit performance and accessibility

---

## 📱 Responsive Design Checklist

### Mobile-First Approach ✅
- [ ] Start with mobile styles
- [ ] Use relative units (%, em, rem)
- [ ] Test on multiple screen sizes
- [ ] Optimize images for different resolutions
- [ ] Ensure touch targets are large enough (44px minimum)

### Key Breakpoints
```css
/* Mobile First */
/* Small devices (landscape phones, 576px and up) */
@media (min-width: 576px) { ... }

/* Medium devices (tablets, 768px and up) */
@media (min-width: 768px) { ... }

/* Large devices (desktops, 992px and up) */
@media (min-width: 992px) { ... }

/* X-Large devices (large desktops, 1200px and up) */
@media (min-width: 1200px) { ... }
```

---

## 🚀 Performance Tips

### HTML Optimization
- Minimize HTML size
- Use semantic elements properly
- Optimize images (WebP format when possible)
- Minimize HTTP requests
- Use CDN for Bootstrap and other libraries

### Best Practices
- Validate HTML regularly
- Use meaningful alt text for images
- Implement proper heading hierarchy
- Include meta description for SEO
- Test across different browsers and devices

---

**Keep this cheatsheet handy as you build your HTML Bootstrap projects!** 📚✨