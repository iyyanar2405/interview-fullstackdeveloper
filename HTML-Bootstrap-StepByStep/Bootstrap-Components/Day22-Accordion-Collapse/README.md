# Day 22: Accordion & Collapse Components 📂

Welcome to Day 22! Master Bootstrap's accordion and collapse components to create expandable content sections, FAQs, and interactive information displays that save space and enhance user experience.

## 🎯 Learning Objectives

- Master accordion and collapse components
- Create FAQ sections and expandable content
- Build nested accordions and complex layouts
- Implement custom styling and animations
- Add icons and visual indicators
- Control expand/collapse with JavaScript
- Ensure accessibility with proper ARIA attributes

## 📚 Core Collapse & Accordion Classes

```css
.collapse              /* Base collapsible element */
.collapse.show         /* Visible state */
.collapsing            /* Transitioning state */
.accordion             /* Accordion container */
.accordion-item        /* Individual accordion item */
.accordion-header      /* Header section */
.accordion-button      /* Toggle button */
.accordion-body        /* Content section */
.accordion-flush       /* Remove borders and rounded corners */
```

## 💻 Complete Accordion Showcase

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Day 22: Accordion & Collapse | Interactive Content</title>
    
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
    
    <style>
        :root {
            --primary-gradient: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            --success-gradient: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
            --card-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            padding: 2rem 0;
        }
        
        .section-header {
            background: var(--primary-gradient);
            color: white;
            padding: 4rem 0;
            margin-bottom: 3rem;
            border-radius: 15px;
        }
        
        .demo-section {
            background: white;
            border-radius: 15px;
            padding: 2.5rem;
            margin-bottom: 2rem;
            box-shadow: var(--card-shadow);
        }
        
        .demo-title {
            color: #2d3748;
            font-weight: 700;
            margin-bottom: 1.5rem;
            border-bottom: 3px solid #e2e8f0;
            padding-bottom: 0.75rem;
        }
        
        /* Custom Accordion Styles */
        .accordion-button {
            font-weight: 600;
            background-color: #f8f9fa;
        }
        
        .accordion-button:not(.collapsed) {
            background: var(--primary-gradient);
            color: white;
        }
        
        .accordion-button:focus {
            box-shadow: none;
            border-color: rgba(102, 126, 234, 0.5);
        }
        
        .accordion-button::after {
            filter: invert(1);
        }
        
        .accordion-button:not(.collapsed)::after {
            filter: invert(0);
        }
        
        /* Icon Accordion */
        .icon-accordion .accordion-button {
            padding-left: 3.5rem;
            position: relative;
        }
        
        .icon-accordion .accordion-button::before {
            content: '';
            position: absolute;
            left: 1.25rem;
            width: 24px;
            height: 24px;
            background-size: contain;
            background-repeat: no-repeat;
        }
        
        .icon-accordion .accordion-item:nth-child(1) .accordion-button::before {
            content: '🚀';
        }
        
        .icon-accordion .accordion-item:nth-child(2) .accordion-button::before {
            content: '💡';
        }
        
        .icon-accordion .accordion-item:nth-child(3) .accordion-button::before {
            content: '⚡';
        }
    </style>
</head>
<body>
    <div class="container">
        <header class="section-header text-center">
            <h1 class="display-4 fw-bold mb-3">Day 22: Accordion & Collapse</h1>
            <p class="lead">Create expandable content sections with style</p>
        </header>
        
        <!-- Basic Accordion -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-list-nested me-2"></i>
                Basic Accordion - FAQ Section
            </h2>
            
            <div class="accordion" id="faqAccordion">
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#faq1">
                            What is Bootstrap?
                        </button>
                    </h2>
                    <div id="faq1" class="accordion-collapse collapse show" data-bs-parent="#faqAccordion">
                        <div class="accordion-body">
                            Bootstrap is a powerful, feature-packed frontend toolkit. Build anything—from prototype to production—in minutes with the world's most popular framework.
                        </div>
                    </div>
                </div>
                
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faq2">
                            How do I install Bootstrap?
                        </button>
                    </h2>
                    <div id="faq2" class="accordion-collapse collapse" data-bs-parent="#faqAccordion">
                        <div class="accordion-body">
                            You can include Bootstrap via CDN, download the compiled files, or install via npm or yarn. The CDN is the quickest way to get started for development.
                        </div>
                    </div>
                </div>
                
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faq3">
                            Is Bootstrap mobile-friendly?
                        </button>
                    </h2>
                    <div id="faq3" class="accordion-collapse collapse" data-bs-parent="#faqAccordion">
                        <div class="accordion-body">
                            Yes! Bootstrap is mobile-first and fully responsive. All components are designed to work seamlessly across devices of all sizes.
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Flush Accordion -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-arrows-collapse me-2"></i>
                Flush Accordion
            </h2>
            
            <div class="accordion accordion-flush" id="flushAccordion">
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#flush1">
                            Feature One
                        </button>
                    </h2>
                    <div id="flush1" class="accordion-collapse collapse" data-bs-parent="#flushAccordion">
                        <div class="accordion-body">Comprehensive grid system for responsive layouts.</div>
                    </div>
                </div>
                
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#flush2">
                            Feature Two
                        </button>
                    </h2>
                    <div id="flush2" class="accordion-collapse collapse" data-bs-parent="#flushAccordion">
                        <div class="accordion-body">Pre-styled components for rapid development.</div>
                    </div>
                </div>
                
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#flush3">
                            Feature Three
                        </button>
                    </h2>
                    <div id="flush3" class="accordion-collapse collapse" data-bs-parent="#flushAccordion">
                        <div class="accordion-body">Powerful JavaScript plugins for interactive elements.</div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Icon Accordion -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-emoji-smile me-2"></i>
                Icon Accordion
            </h2>
            
            <div class="accordion icon-accordion" id="iconAccordion">
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#icon1">
                            Getting Started
                        </button>
                    </h2>
                    <div id="icon1" class="accordion-collapse collapse" data-bs-parent="#iconAccordion">
                        <div class="accordion-body">Quick start guide to begin your Bootstrap journey.</div>
                    </div>
                </div>
                
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#icon2">
                            Best Practices
                        </button>
                    </h2>
                    <div id="icon2" class="accordion-collapse collapse" data-bs-parent="#iconAccordion">
                        <div class="accordion-body">Learn the recommended ways to use Bootstrap effectively.</div>
                    </div>
                </div>
                
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#icon3">
                            Advanced Techniques
                        </button>
                    </h2>
                    <div id="icon3" class="accordion-collapse collapse" data-bs-parent="#iconAccordion">
                        <div class="accordion-body">Master advanced customization and optimization strategies.</div>
                    </div>
                </div>
            </div>
        </section>
        
        <!-- Simple Collapse -->
        <section class="demo-section">
            <h2 class="demo-title">
                <i class="bi bi-chevron-down me-2"></i>
                Simple Collapse Toggle
            </h2>
            
            <button class="btn btn-primary mb-3" type="button" data-bs-toggle="collapse" data-bs-target="#collapseExample">
                Toggle Content
            </button>
            
            <div class="collapse" id="collapseExample">
                <div class="card card-body">
                    This content can be toggled with the button above. Great for showing/hiding additional information without navigation.
                </div>
            </div>
        </section>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        console.log('📂 Day 22 - Accordion & Collapse loaded!');
    </script>
</body>
</html>
```

## 📋 Checklist

- [ ] Basic accordion
- [ ] Flush accordion (borderless)
- [ ] Custom styled accordions
- [ ] Icon accordions
- [ ] Simple collapse toggle
- [ ] Nested accordions
- [ ] JavaScript control
- [ ] Accessibility (ARIA)

## 🎯 Complete!

**Next: Day 23 - Tooltips & Popovers** 💬
