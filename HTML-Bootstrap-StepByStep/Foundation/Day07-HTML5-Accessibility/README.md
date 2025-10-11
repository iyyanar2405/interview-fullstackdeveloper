# Day 07: HTML5 Semantic Elements & Accessibility ♿

Welcome to Day 7 - the final day of our HTML Foundation! Today we'll master HTML5 semantic elements and web accessibility, ensuring your websites work perfectly for everyone, including users with disabilities.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Master all HTML5 semantic elements and their proper usage
- Implement comprehensive web accessibility (WCAG guidelines)
- Use ARIA attributes effectively for enhanced accessibility
- Create screen reader-friendly content
- Build fully accessible page layouts
- Test and validate accessibility compliance
- Understand accessibility best practices and legal requirements

## 📚 Lesson Content

### HTML5 Semantic Elements

Semantic HTML provides meaning to content, improving SEO, accessibility, and code maintainability.

#### Document Structure Elements
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Semantic HTML5 Document</title>
</head>
<body>
    <!-- Page header with site navigation -->
    <header role="banner">
        <h1>TechBlog</h1>
        <nav role="navigation" aria-label="Main navigation">
            <ul>
                <li><a href="/" aria-current="page">Home</a></li>
                <li><a href="/articles">Articles</a></li>
                <li><a href="/about">About</a></li>
                <li><a href="/contact">Contact</a></li>
            </ul>
        </nav>
    </header>
    
    <!-- Main content area -->
    <main role="main">
        <!-- Primary article content -->
        <article>
            <header>
                <h1>Understanding Web Accessibility</h1>
                <p>Published on <time datetime="2025-01-15">January 15, 2025</time> 
                   by <address>Jane Smith</address></p>
            </header>
            
            <section>
                <h2>Introduction</h2>
                <p>Web accessibility ensures all users can interact with web content effectively...</p>
            </section>
            
            <section>
                <h2>Key Principles</h2>
                <p>The WCAG guidelines focus on four main principles...</p>
            </section>
            
            <aside>
                <h3>Related Articles</h3>
                <ul>
                    <li><a href="/semantic-html">Semantic HTML Guide</a></li>
                    <li><a href="/aria-attributes">ARIA Best Practices</a></li>
                </ul>
            </aside>
        </article>
        
        <!-- Sidebar content -->
        <aside role="complementary">
            <section>
                <h2>Newsletter Signup</h2>
                <form>
                    <!-- form content -->
                </form>
            </section>
        </aside>
    </main>
    
    <!-- Site footer -->
    <footer role="contentinfo">
        <p>&copy; 2025 TechBlog. All rights reserved.</p>
    </footer>
</body>
</html>
```

#### Content Sectioning Elements
```html
<!-- Article with multiple sections -->
<article itemscope itemtype="http://schema.org/BlogPosting">
    <header>
        <h1 itemprop="headline">The Future of Web Development</h1>
        <p>
            By <span itemprop="author">John Doe</span> on 
            <time datetime="2025-01-15" itemprop="datePublished">January 15, 2025</time>
        </p>
    </header>
    
    <section>
        <h2>Current Trends</h2>
        <p>Modern web development is evolving rapidly...</p>
        
        <figure>
            <img src="trends-chart.jpg" alt="Web development trends showing increased use of modern frameworks">
            <figcaption>
                <strong>Figure 1:</strong> Web development framework adoption rates 2020-2025
            </figcaption>
        </figure>
    </section>
    
    <section>
        <h2>Emerging Technologies</h2>
        <p>Several technologies are shaping the future...</p>
        
        <details>
            <summary>Advanced Frameworks</summary>
            <p>Next.js, Nuxt.js, and SvelteKit are leading the way in meta-frameworks...</p>
        </details>
    </section>
    
    <section>
        <h2>Conclusion</h2>
        <p>The web development landscape continues to evolve...</p>
    </section>
</article>

<!-- Complementary sidebar -->
<aside>
    <section>
        <h2>Author Bio</h2>
        <p>John Doe is a senior web developer with over 10 years of experience...</p>
    </section>
    
    <section>
        <h2>Categories</h2>
        <nav aria-label="Article categories">
            <ul>
                <li><a href="/category/javascript">JavaScript</a></li>
                <li><a href="/category/frameworks">Frameworks</a></li>
                <li><a href="/category/performance">Performance</a></li>
            </ul>
        </nav>
    </section>
</aside>
```

### ARIA (Accessible Rich Internet Applications)

ARIA attributes provide additional semantic information for assistive technologies.

#### Essential ARIA Attributes
```html
<!-- Landmark roles -->
<nav role="navigation" aria-label="Primary navigation">
    <ul>
        <li><a href="/" aria-current="page">Home</a></li>
        <li><a href="/products">Products</a></li>
        <li><a href="/services">Services</a></li>
    </ul>
</nav>

<main role="main" aria-label="Main content">
    <article role="article">
        <h1>Article Title</h1>
        <p>Article content...</p>
    </article>
</main>

<aside role="complementary" aria-label="Sidebar">
    <h2>Related Links</h2>
    <ul>
        <li><a href="/link1">Related Article 1</a></li>
    </ul>
</aside>

<!-- Form accessibility -->
<form role="form" aria-labelledby="contact-heading">
    <h2 id="contact-heading">Contact Form</h2>
    
    <fieldset>
        <legend>Personal Information</legend>
        
        <label for="name">
            Name <span aria-label="required">*</span>
        </label>
        <input type="text" 
               id="name" 
               name="name" 
               required
               aria-required="true"
               aria-describedby="name-help name-error">
        <div id="name-help" class="help-text">Enter your full name</div>
        <div id="name-error" class="error" role="alert" aria-live="polite">
            <!-- Error message will appear here -->
        </div>
        
        <label for="email">
            Email <span aria-label="required">*</span>
        </label>
        <input type="email" 
               id="email" 
               name="email" 
               required
               aria-required="true"
               aria-describedby="email-help"
               aria-invalid="false">
        <div id="email-help" class="help-text">We'll never share your email</div>
    </fieldset>
    
    <button type="submit" aria-describedby="submit-help">
        Send Message
    </button>
    <div id="submit-help" class="help-text">
        By submitting, you agree to our privacy policy
    </div>
</form>
```

#### Interactive ARIA Patterns
```html
<!-- Expandable content -->
<div class="accordion">
    <h3>
        <button type="button" 
                aria-expanded="false" 
                aria-controls="panel1"
                id="accordion1">
            Web Development Basics
        </button>
    </h3>
    <div id="panel1" 
         role="region" 
         aria-labelledby="accordion1"
         hidden>
        <p>Learn the fundamentals of web development including HTML, CSS, and JavaScript...</p>
    </div>
</div>

<!-- Tab interface -->
<div class="tab-container">
    <div role="tablist" aria-label="Programming languages">
        <button role="tab" 
                aria-selected="true" 
                aria-controls="html-panel" 
                id="html-tab"
                tabindex="0">
            HTML
        </button>
        <button role="tab" 
                aria-selected="false" 
                aria-controls="css-panel" 
                id="css-tab"
                tabindex="-1">
            CSS
        </button>
        <button role="tab" 
                aria-selected="false" 
                aria-controls="js-panel" 
                id="js-tab"
                tabindex="-1">
            JavaScript
        </button>
    </div>
    
    <div id="html-panel" 
         role="tabpanel" 
         aria-labelledby="html-tab">
        <p>HTML (HyperText Markup Language) is the standard markup language...</p>
    </div>
    
    <div id="css-panel" 
         role="tabpanel" 
         aria-labelledby="css-tab" 
         hidden>
        <p>CSS (Cascading Style Sheets) describes the presentation...</p>
    </div>
    
    <div id="js-panel" 
         role="tabpanel" 
         aria-labelledby="js-tab" 
         hidden>
        <p>JavaScript is a programming language that adds interactivity...</p>
    </div>
</div>

<!-- Live regions for dynamic content -->
<div aria-live="polite" aria-atomic="true" id="status-message">
    <!-- Dynamic status messages appear here -->
</div>

<div aria-live="assertive" id="error-announcements">
    <!-- Critical error messages appear here -->
</div>
```

### Screen Reader Optimization

Optimize content specifically for screen reader users:

#### Skip Links and Navigation
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Accessible Website</title>
    <style>
        .skip-link {
            position: absolute;
            top: -40px;
            left: 6px;
            background: #000;
            color: #fff;
            padding: 8px;
            text-decoration: none;
            border-radius: 4px;
            z-index: 1000;
        }
        
        .skip-link:focus {
            top: 6px;
        }
        
        .sr-only {
            position: absolute;
            width: 1px;
            height: 1px;
            padding: 0;
            margin: -1px;
            overflow: hidden;
            clip: rect(0, 0, 0, 0);
            border: 0;
        }
    </style>
</head>
<body>
    <!-- Skip links -->
    <a href="#main-content" class="skip-link">Skip to main content</a>
    <a href="#navigation" class="skip-link">Skip to navigation</a>
    
    <!-- Navigation with proper heading structure -->
    <nav id="navigation" aria-label="Main navigation">
        <h2 class="sr-only">Main Navigation</h2>
        <ul>
            <li><a href="/">Home</a></li>
            <li><a href="/about">About</a></li>
            <li><a href="/services">Services</a></li>
            <li><a href="/contact">Contact</a></li>
        </ul>
    </nav>
    
    <main id="main-content">
        <h1>Welcome to Our Accessible Website</h1>
        <p>This website follows WCAG 2.1 AA guidelines...</p>
    </main>
</body>
</html>
```

#### Descriptive Content
```html
<!-- Detailed alt text for complex images -->
<figure>
    <img src="sales-chart.png" 
         alt="Bar chart showing monthly sales from January to December 2024. January: $10,000, February: $12,000, March: $15,000, peaking in December at $25,000.">
    <figcaption>
        <strong>2024 Monthly Sales Report</strong><br>
        <a href="sales-data.csv" download>Download raw data (CSV)</a>
    </figcaption>
</figure>

<!-- Informative link text -->
<p>Learn more about our services by visiting our 
   <a href="/services" aria-describedby="services-desc">complete services page</a>.
</p>
<div id="services-desc" class="sr-only">
    Opens in same window, shows detailed list of all services with pricing
</div>

<!-- Clear button purposes -->
<button type="button" 
        aria-label="Delete item from shopping cart"
        onclick="removeItem(123)">
    <span aria-hidden="true">×</span>
    <span class="sr-only">Remove iPhone 15 Pro from cart</span>
</button>

<!-- Descriptive headings -->
<h2>Contact Information and Business Hours</h2>
<p>Phone: <a href="tel:+15551234567">(555) 123-4567</a></p>
<p>Email: <a href="mailto:info@example.com">info@example.com</a></p>
<p>Hours: Monday-Friday 9AM-5PM EST</p>
```

## 💻 Hands-On Practice

### Exercise 1: Fully Accessible Blog Website

Create `accessible-blog.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="TechInsights - Your source for web development tutorials, accessibility guides, and modern programming practices">
    <title>TechInsights - Web Development Blog</title>
    
    <style>
        /* Reset and base styles */
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #fff;
        }
        
        /* Skip links */
        .skip-link {
            position: absolute;
            top: -40px;
            left: 6px;
            background: #0066cc;
            color: #fff;
            padding: 8px;
            text-decoration: none;
            border-radius: 4px;
            z-index: 1000;
            font-weight: bold;
        }
        
        .skip-link:focus {
            top: 6px;
        }
        
        /* Screen reader only content */
        .sr-only {
            position: absolute;
            width: 1px;
            height: 1px;
            padding: 0;
            margin: -1px;
            overflow: hidden;
            clip: rect(0, 0, 0, 0);
            border: 0;
        }
        
        /* Focus styles */
        a:focus, button:focus, input:focus, textarea:focus, select:focus {
            outline: 3px solid #0066cc;
            outline-offset: 2px;
        }
        
        /* Header and navigation */
        header {
            background: #2c3e50;
            color: #fff;
            padding: 1rem 0;
            border-bottom: 3px solid #0066cc;
        }
        
        .header-content {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 1rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
        }
        
        .logo {
            font-size: 1.5rem;
            font-weight: bold;
            color: #fff;
            text-decoration: none;
        }
        
        nav ul {
            list-style: none;
            display: flex;
            gap: 2rem;
        }
        
        nav a {
            color: #fff;
            text-decoration: none;
            padding: 0.5rem;
            border-radius: 4px;
            transition: background 0.3s;
        }
        
        nav a:hover, nav a:focus {
            background: rgba(255, 255, 255, 0.1);
        }
        
        nav a[aria-current="page"] {
            background: #0066cc;
            font-weight: bold;
        }
        
        /* Main layout */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 2rem 1rem;
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 3rem;
        }
        
        /* Article styles */
        article {
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            padding: 2rem;
            margin-bottom: 2rem;
        }
        
        article header {
            background: none;
            color: #333;
            padding: 0;
            margin-bottom: 1.5rem;
            border: none;
        }
        
        .article-meta {
            color: #666;
            font-size: 0.9rem;
            margin-bottom: 1rem;
        }
        
        .article-meta time {
            font-weight: bold;
        }
        
        h1, h2, h3, h4, h5, h6 {
            margin-bottom: 1rem;
            line-height: 1.2;
        }
        
        h1 {
            font-size: 2.5rem;
            color: #2c3e50;
        }
        
        h2 {
            font-size: 2rem;
            color: #34495e;
            border-left: 4px solid #0066cc;
            padding-left: 1rem;
            margin-top: 2rem;
        }
        
        h3 {
            font-size: 1.5rem;
            color: #2c3e50;
        }
        
        p {
            margin-bottom: 1rem;
        }
        
        /* Sidebar styles */
        aside {
            background: #f8f9fa;
            padding: 1.5rem;
            border-radius: 8px;
            height: fit-content;
        }
        
        aside section {
            margin-bottom: 2rem;
        }
        
        aside h2 {
            font-size: 1.2rem;
            border-left: none;
            padding-left: 0;
            margin-top: 0;
            margin-bottom: 1rem;
        }
        
        /* Form styles */
        .newsletter-form {
            background: #fff;
            padding: 1.5rem;
            border-radius: 8px;
            border: 2px solid #0066cc;
        }
        
        .form-group {
            margin-bottom: 1rem;
        }
        
        label {
            display: block;
            margin-bottom: 0.5rem;
            font-weight: bold;
        }
        
        input, textarea {
            width: 100%;
            padding: 0.75rem;
            border: 2px solid #ddd;
            border-radius: 4px;
            font-size: 1rem;
        }
        
        input:focus, textarea:focus {
            border-color: #0066cc;
        }
        
        .error {
            color: #d32f2f;
            font-size: 0.9rem;
            margin-top: 0.25rem;
        }
        
        .success {
            color: #2e7d32;
            font-size: 0.9rem;
            margin-top: 0.25rem;
        }
        
        button {
            background: #0066cc;
            color: #fff;
            border: none;
            padding: 0.75rem 1.5rem;
            border-radius: 4px;
            cursor: pointer;
            font-size: 1rem;
            font-weight: bold;
            transition: background 0.3s;
        }
        
        button:hover, button:focus {
            background: #0052a3;
        }
        
        /* Accessibility features */
        .breadcrumb {
            margin-bottom: 1rem;
            font-size: 0.9rem;
        }
        
        .breadcrumb ol {
            list-style: none;
            display: flex;
            gap: 0.5rem;
        }
        
        .breadcrumb li::after {
            content: ">";
            margin-left: 0.5rem;
            color: #666;
        }
        
        .breadcrumb li:last-child::after {
            display: none;
        }
        
        /* High contrast mode support */
        @media (prefers-color-scheme: dark) {
            body {
                background: #1a1a1a;
                color: #e0e0e0;
            }
            
            article {
                background: #2d2d2d;
                color: #e0e0e0;
            }
            
            aside {
                background: #333;
                color: #e0e0e0;
            }
        }
        
        /* Reduced motion support */
        @media (prefers-reduced-motion: reduce) {
            * {
                animation-duration: 0.01ms !important;
                animation-iteration-count: 1 !important;
                transition-duration: 0.01ms !important;
            }
        }
        
        /* Mobile responsive */
        @media (max-width: 768px) {
            .container {
                grid-template-columns: 1fr;
                gap: 2rem;
            }
            
            .header-content {
                flex-direction: column;
                gap: 1rem;
            }
            
            nav ul {
                flex-direction: column;
                gap: 0.5rem;
                text-align: center;
            }
        }
    </style>
</head>
<body>
    <!-- Skip links for keyboard navigation -->
    <a href="#main-content" class="skip-link">Skip to main content</a>
    <a href="#navigation" class="skip-link">Skip to navigation</a>
    <a href="#sidebar" class="skip-link">Skip to sidebar</a>
    
    <!-- Page header with site branding and navigation -->
    <header role="banner">
        <div class="header-content">
            <a href="/" class="logo" aria-label="TechInsights home page">
                <span aria-hidden="true">💻</span> TechInsights
            </a>
            
            <nav id="navigation" role="navigation" aria-label="Main navigation">
                <h2 class="sr-only">Main Navigation Menu</h2>
                <ul>
                    <li><a href="/" aria-current="page">Home</a></li>
                    <li><a href="/tutorials">Tutorials</a></li>
                    <li><a href="/accessibility">Accessibility</a></li>
                    <li><a href="/about">About</a></li>
                    <li><a href="/contact">Contact</a></li>
                </ul>
            </nav>
        </div>
    </header>
    
    <!-- Main content container -->
    <div class="container">
        <!-- Primary content area -->
        <main id="main-content" role="main" aria-label="Main content">
            <!-- Breadcrumb navigation -->
            <nav aria-label="Breadcrumb" class="breadcrumb">
                <ol>
                    <li><a href="/">Home</a></li>
                    <li><a href="/tutorials">Tutorials</a></li>
                    <li aria-current="page">Web Accessibility Guide</li>
                </ol>
            </nav>
            
            <!-- Main article -->
            <article role="article" itemscope itemtype="http://schema.org/BlogPosting">
                <header>
                    <h1 itemprop="headline">Complete Guide to Web Accessibility</h1>
                    <div class="article-meta">
                        <p>
                            Published on <time datetime="2025-01-15" itemprop="datePublished">January 15, 2025</time>
                            by <span itemprop="author" itemscope itemtype="http://schema.org/Person">
                                <span itemprop="name">Sarah Johnson</span>
                            </span>
                            • <span itemprop="timeRequired">15 min read</span>
                        </p>
                        <p>
                            <strong>Topics:</strong> 
                            <span itemprop="keywords">Accessibility, WCAG, ARIA, Screen Readers, Web Standards</span>
                        </p>
                    </div>
                </header>
                
                <div itemprop="articleBody">
                    <section>
                        <h2 id="introduction">Introduction to Web Accessibility</h2>
                        <p>
                            Web accessibility ensures that websites and digital content are usable by everyone, 
                            including people with disabilities. This comprehensive guide covers the essential 
                            principles, techniques, and best practices for creating inclusive web experiences.
                        </p>
                        
                        <p>
                            According to the World Health Organization, over 1 billion people worldwide have 
                            some form of disability. By making your website accessible, you're not only doing 
                            the right thing ethically, but you're also expanding your potential audience and 
                            often improving usability for all users.
                        </p>
                    </section>
                    
                    <section>
                        <h2 id="wcag-principles">The Four WCAG Principles</h2>
                        <p>
                            The Web Content Accessibility Guidelines (WCAG) 2.1 are built around four main principles, 
                            often remembered by the acronym <strong>POUR</strong>:
                        </p>
                        
                        <dl>
                            <dt><strong>Perceivable</strong></dt>
                            <dd>
                                Information and user interface components must be presentable to users in ways 
                                they can perceive. This includes providing text alternatives for images, 
                                captions for videos, and sufficient color contrast.
                            </dd>
                            
                            <dt><strong>Operable</strong></dt>
                            <dd>
                                User interface components and navigation must be operable by all users. 
                                This means making all functionality available via keyboard, providing users 
                                enough time to read content, and avoiding content that causes seizures.
                            </dd>
                            
                            <dt><strong>Understandable</strong></dt>
                            <dd>
                                Information and the operation of user interfaces must be understandable. 
                                This includes making text readable, making content appear and operate 
                                in predictable ways, and helping users avoid and correct mistakes.
                            </dd>
                            
                            <dt><strong>Robust</strong></dt>
                            <dd>
                                Content must be robust enough to be interpreted reliably by a wide variety 
                                of user agents, including assistive technologies. This primarily means 
                                using valid, semantic HTML and ensuring compatibility with screen readers.
                            </dd>
                        </dl>
                    </section>
                    
                    <section>
                        <h2 id="semantic-html">Semantic HTML Foundation</h2>
                        <p>
                            Semantic HTML forms the foundation of accessible web content. By using the correct 
                            elements for their intended purpose, you provide inherent meaning and structure 
                            that assistive technologies can understand and navigate.
                        </p>
                        
                        <h3>Essential Semantic Elements</h3>
                        <ul>
                            <li><code>&lt;header&gt;</code> - Page or section header</li>
                            <li><code>&lt;nav&gt;</code> - Navigation sections</li>
                            <li><code>&lt;main&gt;</code> - Primary content area</li>
                            <li><code>&lt;article&gt;</code> - Self-contained content</li>
                            <li><code>&lt;section&gt;</code> - Thematic groupings</li>
                            <li><code>&lt;aside&gt;</code> - Sidebar or complementary content</li>
                            <li><code>&lt;footer&gt;</code> - Page or section footer</li>
                        </ul>
                        
                        <figure>
                            <img src="semantic-html-structure.png" 
                                 alt="Diagram showing HTML5 semantic elements layout with header at top containing navigation, main content area in center with article and aside sections, and footer at bottom"
                                 width="600" 
                                 height="400">
                            <figcaption>
                                <strong>Figure 1:</strong> Typical semantic HTML5 page structure showing 
                                the relationship between different sectioning elements
                            </figcaption>
                        </figure>
                    </section>
                    
                    <section>
                        <h2 id="aria-attributes">ARIA Attributes and Roles</h2>
                        <p>
                            ARIA (Accessible Rich Internet Applications) attributes provide additional 
                            semantic information to assistive technologies when native HTML semantics 
                            are insufficient.
                        </p>
                        
                        <h3>Key ARIA Concepts</h3>
                        <dl>
                            <dt><strong>Roles</strong></dt>
                            <dd>Define what an element is or does (e.g., button, dialog, navigation)</dd>
                            
                            <dt><strong>Properties</strong></dt>
                            <dd>Define properties of elements (e.g., aria-required, aria-readonly)</dd>
                            
                            <dt><strong>States</strong></dt>
                            <dd>Define current conditions of elements (e.g., aria-expanded, aria-checked)</dd>
                        </dl>
                        
                        <details>
                            <summary>Common ARIA Attributes Examples</summary>
                            <div>
                                <p><strong>Form Labels and Descriptions:</strong></p>
                                <ul>
                                    <li><code>aria-label</code> - Provides accessible name</li>
                                    <li><code>aria-labelledby</code> - References element(s) that label this element</li>
                                    <li><code>aria-describedby</code> - References element(s) that describe this element</li>
                                </ul>
                                
                                <p><strong>Interactive Elements:</strong></p>
                                <ul>
                                    <li><code>aria-expanded</code> - Indicates if collapsible element is expanded</li>
                                    <li><code>aria-current</code> - Indicates current item in a set</li>
                                    <li><code>aria-hidden</code> - Hides decorative elements from screen readers</li>
                                </ul>
                            </div>
                        </details>
                    </section>
                    
                    <section>
                        <h2 id="testing-tools">Testing and Validation Tools</h2>
                        <p>
                            Regular testing is crucial for maintaining accessibility. Here are essential tools 
                            and techniques for evaluating your website's accessibility:
                        </p>
                        
                        <h3>Automated Testing Tools</h3>
                        <ul>
                            <li><strong>axe DevTools</strong> - Browser extension for automated accessibility testing</li>
                            <li><strong>WAVE</strong> - Web accessibility evaluation tool</li>
                            <li><strong>Lighthouse</strong> - Built into Chrome DevTools</li>
                            <li><strong>Pa11y</strong> - Command line accessibility testing tool</li>
                        </ul>
                        
                        <h3>Manual Testing Techniques</h3>
                        <ol>
                            <li><strong>Keyboard Navigation</strong> - Navigate using only Tab, Enter, Space, and arrow keys</li>
                            <li><strong>Screen Reader Testing</strong> - Use NVDA (Windows), JAWS, or VoiceOver (Mac)</li>
                            <li><strong>Color Contrast</strong> - Verify text meets WCAG contrast ratios</li>
                            <li><strong>Zoom Testing</strong> - Test usability at 200% zoom level</li>
                            <li><strong>Focus Management</strong> - Ensure focus is visible and logical</li>
                        </ol>
                    </section>
                </div>
                
                <footer>
                    <p>
                        <strong>About the Author:</strong> Sarah Johnson is a web accessibility consultant 
                        with over 8 years of experience helping organizations create inclusive digital experiences. 
                        She holds certifications in WCAG 2.1 and assistive technology training.
                    </p>
                    
                    <nav aria-label="Article actions">
                        <ul style="list-style: none; display: flex; gap: 1rem; margin-top: 1rem;">
                            <li><a href="#" onclick="return false;">Share on Twitter</a></li>
                            <li><a href="#" onclick="return false;">Share on LinkedIn</a></li>
                            <li><a href="#" onclick="return false;">Save to Reading List</a></li>
                        </ul>
                    </nav>
                </footer>
            </article>
            
            <!-- Related articles section -->
            <section aria-labelledby="related-heading">
                <h2 id="related-heading">Related Articles</h2>
                <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 1rem;">
                    <article style="padding: 1rem; background: #f8f9fa; border-radius: 8px;">
                        <h3><a href="/semantic-html-guide">Semantic HTML Best Practices</a></h3>
                        <p>Learn how to choose the right HTML elements for better accessibility and SEO.</p>
                        <p><small><time datetime="2025-01-10">January 10, 2025</time> • 8 min read</small></p>
                    </article>
                    
                    <article style="padding: 1rem; background: #f8f9fa; border-radius: 8px;">
                        <h3><a href="/color-contrast-guide">Color and Contrast Guidelines</a></h3>
                        <p>Ensure your color choices meet accessibility standards and work for everyone.</p>
                        <p><small><time datetime="2025-01-05">January 5, 2025</time> • 12 min read</small></p>
                    </article>
                </div>
            </section>
        </main>
        
        <!-- Sidebar with complementary content -->
        <aside id="sidebar" role="complementary" aria-label="Sidebar">
            <h2 class="sr-only">Sidebar Content</h2>
            
            <!-- Newsletter signup -->
            <section aria-labelledby="newsletter-heading">
                <div class="newsletter-form">
                    <h2 id="newsletter-heading">Stay Updated</h2>
                    <p>Get the latest accessibility tips and web development insights delivered to your inbox.</p>
                    
                    <form role="form" aria-labelledby="newsletter-heading" novalidate>
                        <div class="form-group">
                            <label for="newsletter-email">
                                Email Address <span aria-label="required">*</span>
                            </label>
                            <input type="email" 
                                   id="newsletter-email" 
                                   name="email" 
                                   required
                                   aria-required="true"
                                   aria-describedby="email-help email-error"
                                   placeholder="you@example.com">
                            <div id="email-help" class="sr-only">
                                We'll never share your email address
                            </div>
                            <div id="email-error" role="alert" aria-live="polite" class="error">
                                <!-- Error messages appear here -->
                            </div>
                        </div>
                        
                        <div class="form-group">
                            <fieldset>
                                <legend>Content Preferences</legend>
                                <label>
                                    <input type="checkbox" name="preferences" value="tutorials" checked>
                                    Tutorials and Guides
                                </label>
                                <label>
                                    <input type="checkbox" name="preferences" value="news">
                                    Industry News
                                </label>
                                <label>
                                    <input type="checkbox" name="preferences" value="events">
                                    Events and Webinars
                                </label>
                            </fieldset>
                        </div>
                        
                        <button type="submit" aria-describedby="submit-info">
                            Subscribe to Newsletter
                        </button>
                        <div id="submit-info" class="sr-only">
                            By subscribing, you agree to our privacy policy
                        </div>
                        
                        <div id="form-status" aria-live="polite" aria-atomic="true">
                            <!-- Success/error messages appear here -->
                        </div>
                    </form>
                </div>
            </section>
            
            <!-- Table of contents -->
            <section aria-labelledby="toc-heading">
                <h2 id="toc-heading">Table of Contents</h2>
                <nav aria-labelledby="toc-heading">
                    <ol>
                        <li><a href="#introduction">Introduction to Web Accessibility</a></li>
                        <li><a href="#wcag-principles">The Four WCAG Principles</a></li>
                        <li><a href="#semantic-html">Semantic HTML Foundation</a></li>
                        <li><a href="#aria-attributes">ARIA Attributes and Roles</a></li>
                        <li><a href="#testing-tools">Testing and Validation Tools</a></li>
                    </ol>
                </nav>
            </section>
            
            <!-- Popular categories -->
            <section aria-labelledby="categories-heading">
                <h2 id="categories-heading">Popular Categories</h2>
                <nav aria-labelledby="categories-heading">
                    <ul style="list-style: none;">
                        <li><a href="/category/accessibility">♿ Accessibility (24 articles)</a></li>
                        <li><a href="/category/html">📝 HTML & Semantic Markup (18 articles)</a></li>
                        <li><a href="/category/css">🎨 CSS & Styling (32 articles)</a></li>
                        <li><a href="/category/javascript">⚡ JavaScript & Interactivity (28 articles)</a></li>
                        <li><a href="/category/testing">🧪 Testing & Quality Assurance (15 articles)</a></li>
                    </ul>
                </nav>
            </section>
            
            <!-- Quick accessibility check -->
            <section aria-labelledby="quick-check-heading">
                <h2 id="quick-check-heading">Quick Accessibility Checklist</h2>
                <div role="group" aria-labelledby="quick-check-heading">
                    <p><strong>Before publishing, verify:</strong></p>
                    <ul>
                        <li>✅ All images have alt text</li>
                        <li>✅ Headings are in logical order</li>
                        <li>✅ Links have descriptive text</li>
                        <li>✅ Forms are properly labeled</li>
                        <li>✅ Color contrast meets WCAG AA</li>
                        <li>✅ Keyboard navigation works</li>
                    </ul>
                    <p><a href="/accessibility-checklist">View complete checklist →</a></p>
                </div>
            </section>
        </aside>
    </div>
    
    <!-- Site footer -->
    <footer role="contentinfo">
        <div style="max-width: 1200px; margin: 0 auto; padding: 2rem 1rem; border-top: 1px solid #ddd;">
            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 2rem;">
                <section>
                    <h3>About TechInsights</h3>
                    <p>
                        We're dedicated to making the web more accessible and inclusive through 
                        education, best practices, and practical tutorials.
                    </p>
                </section>
                
                <section>
                    <h3>Quick Links</h3>
                    <nav aria-label="Footer navigation">
                        <ul style="list-style: none;">
                            <li><a href="/privacy">Privacy Policy</a></li>
                            <li><a href="/terms">Terms of Service</a></li>
                            <li><a href="/accessibility-statement">Accessibility Statement</a></li>
                            <li><a href="/contact">Contact Us</a></li>
                        </ul>
                    </nav>
                </section>
                
                <section>
                    <h3>Follow Us</h3>
                    <nav aria-label="Social media links">
                        <ul style="list-style: none; display: flex; gap: 1rem;">
                            <li>
                                <a href="https://twitter.com/techinsights" 
                                   aria-label="Follow us on Twitter (opens in new window)"
                                   target="_blank" 
                                   rel="noopener noreferrer">
                                    Twitter
                                </a>
                            </li>
                            <li>
                                <a href="https://linkedin.com/company/techinsights" 
                                   aria-label="Follow us on LinkedIn (opens in new window)"
                                   target="_blank" 
                                   rel="noopener noreferrer">
                                    LinkedIn
                                </a>
                            </li>
                            <li>
                                <a href="https://github.com/techinsights" 
                                   aria-label="View our code on GitHub (opens in new window)"
                                   target="_blank" 
                                   rel="noopener noreferrer">
                                    GitHub
                                </a>
                            </li>
                        </ul>
                    </nav>
                </section>
            </div>
            
            <div style="text-align: center; margin-top: 2rem; padding-top: 2rem; border-top: 1px solid #ddd;">
                <p>&copy; 2025 TechInsights. All rights reserved.</p>
                <p>
                    <small>
                        This website follows WCAG 2.1 AA guidelines. 
                        <a href="/accessibility-statement">Learn more about our accessibility commitment</a>.
                    </small>
                </p>
            </div>
        </div>
    </footer>
    
    <!-- JavaScript for enhanced accessibility -->
    <script>
        // Enhanced form validation with accessibility
        document.addEventListener('DOMContentLoaded', function() {
            const form = document.querySelector('form[role="form"]');
            const emailInput = document.getElementById('newsletter-email');
            const errorDiv = document.getElementById('email-error');
            const statusDiv = document.getElementById('form-status');
            
            // Real-time email validation
            emailInput.addEventListener('blur', function() {
                const email = this.value.trim();
                if (email && !isValidEmail(email)) {
                    this.setAttribute('aria-invalid', 'true');
                    errorDiv.textContent = 'Please enter a valid email address.';
                    errorDiv.style.display = 'block';
                } else {
                    this.setAttribute('aria-invalid', 'false');
                    errorDiv.textContent = '';
                    errorDiv.style.display = 'none';
                }
            });
            
            // Form submission
            form.addEventListener('submit', function(e) {
                e.preventDefault();
                
                const email = emailInput.value.trim();
                
                if (!email) {
                    emailInput.setAttribute('aria-invalid', 'true');
                    errorDiv.textContent = 'Email address is required.';
                    errorDiv.style.display = 'block';
                    emailInput.focus();
                    return;
                }
                
                if (!isValidEmail(email)) {
                    emailInput.setAttribute('aria-invalid', 'true');
                    errorDiv.textContent = 'Please enter a valid email address.';
                    errorDiv.style.display = 'block';
                    emailInput.focus();
                    return;
                }
                
                // Simulate form submission
                statusDiv.innerHTML = '<div class="success">✅ Thank you for subscribing! Please check your email to confirm.</div>';
                form.reset();
                emailInput.setAttribute('aria-invalid', 'false');
                errorDiv.style.display = 'none';
            });
            
            function isValidEmail(email) {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                return emailRegex.test(email);
            }
        });
        
        // Smooth scrolling for anchor links with focus management
        document.querySelectorAll('a[href^="#"]').forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const targetId = this.getAttribute('href').substring(1);
                const targetElement = document.getElementById(targetId);
                
                if (targetElement) {
                    targetElement.scrollIntoView({ behavior: 'smooth' });
                    
                    // Set focus to target for keyboard users
                    if (!targetElement.hasAttribute('tabindex')) {
                        targetElement.setAttribute('tabindex', '-1');
                    }
                    targetElement.focus();
                }
            });
        });
        
        // Announce dynamic content changes
        function announceToScreenReader(message, priority = 'polite') {
            const announcement = document.createElement('div');
            announcement.setAttribute('aria-live', priority);
            announcement.setAttribute('aria-atomic', 'true');
            announcement.className = 'sr-only';
            announcement.textContent = message;
            
            document.body.appendChild(announcement);
            
            setTimeout(() => {
                document.body.removeChild(announcement);
            }, 1000);
        }
    </script>
</body>
</html>
```

## 📋 Accessibility Testing Checklist

### Automated Testing
- [ ] Run axe DevTools extension
- [ ] Check Lighthouse accessibility score
- [ ] Validate HTML markup
- [ ] Test color contrast ratios
- [ ] Verify heading structure

### Manual Testing
- [ ] Navigate entire site using only keyboard
- [ ] Test with screen reader (NVDA, VoiceOver, JAWS)
- [ ] Verify focus indicators are visible
- [ ] Test at 200% zoom level
- [ ] Check form error handling

### Content Review
- [ ] All images have appropriate alt text
- [ ] Links have descriptive text
- [ ] Headings are in logical order (h1, h2, h3)
- [ ] Form labels are properly associated
- [ ] Error messages are clear and helpful

## 🎉 Foundation Week Complete! 🎉

Congratulations! You've completed the **HTML Foundation Week** and built a strong foundation in:

### ✅ Week 1 Achievements
- **Day 01:** HTML Document Structure & Basic Elements
- **Day 02:** Text Elements & Semantic Markup  
- **Day 03:** Lists, Links & Navigation Systems
- **Day 04:** Images, Media & Multimedia Content
- **Day 05:** Forms & Input Elements
- **Day 06:** Tables & Data Display
- **Day 07:** Semantic Elements & Web Accessibility

### 🏆 Skills Mastered
- Semantic HTML5 markup
- Accessibility best practices (WCAG 2.1 AA)
- Screen reader optimization
- Form creation and validation
- Media integration and optimization
- Table structure and data presentation
- ARIA attributes and roles

### 📊 Foundation Complete Checklist
- [ ] ✅ HTML document structure mastered
- [ ] ✅ All semantic elements understood
- [ ] ✅ Forms and inputs implemented correctly
- [ ] ✅ Media and multimedia integrated
- [ ] ✅ Tables created accessibly
- [ ] ✅ Full accessibility compliance achieved
- [ ] ✅ Ready for Bootstrap integration

## 🔗 Additional Resources

- **[WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)** - Official accessibility guidelines
- **[MDN Accessibility](https://developer.mozilla.org/en-US/docs/Web/Accessibility)** - Comprehensive accessibility docs
- **[WebAIM](https://webaim.org/)** - Accessibility resources and tools
- **[A11y Project](https://www.a11yproject.com/)** - Community accessibility checklist

---

## 🚀 What's Next?

Starting **Day 08**, we begin the **Bootstrap Framework Phase**:
- Bootstrap 5 setup and configuration
- Grid system mastery
- Component library exploration
- Responsive design patterns
- Custom theming and styling

**Outstanding work completing the HTML Foundation!** You now have the solid semantic HTML and accessibility knowledge needed to build professional, inclusive websites. Tomorrow we'll enhance these skills with Bootstrap's powerful framework! 🎯✨

---

**Remember: Accessibility isn't optional - it's essential for creating truly inclusive web experiences!** ♿🌟