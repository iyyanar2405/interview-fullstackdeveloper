# Day 04: Images, Media & Multimedia 🖼️

Welcome to Day 4! Today we'll master multimedia content in HTML, including images, audio, video, and modern responsive media techniques that make websites engaging and accessible.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Implement responsive and accessible images
- Integrate audio and video content properly
- Use figure and figcaption for media context
- Understand modern image formats and optimization
- Create multimedia-rich web pages
- Apply accessibility best practices for media content

## 📚 Lesson Content

### HTML Images

Images are crucial for engaging web experiences. Let's master all aspects of HTML images.

#### Basic Image Syntax
```html
<img src="path/to/image.jpg" alt="Descriptive text about the image">
```

#### Essential Image Attributes
```html
<img src="photo.jpg" 
     alt="A sunset over the ocean with orange and pink colors"
     width="800" 
     height="600"
     title="Beautiful sunset at Santa Monica Beach"
     loading="lazy">
```

**Attribute Explanations:**
- `src`: Image file path or URL
- `alt`: Alternative text for accessibility and SEO
- `width/height`: Dimensions in pixels
- `title`: Tooltip text (optional)
- `loading`: Lazy loading for performance

#### Responsive Images with `srcset`
```html
<!-- Different image sizes for different screen densities -->
<img src="image-800.jpg"
     srcset="image-400.jpg 400w,
             image-800.jpg 800w,
             image-1200.jpg 1200w"
     sizes="(max-width: 600px) 400px,
            (max-width: 1000px) 800px,
            1200px"
     alt="Responsive image example">

<!-- Different images for different screen sizes -->
<picture>
    <source media="(max-width: 600px)" srcset="mobile-image.jpg">
    <source media="(max-width: 1000px)" srcset="tablet-image.jpg">
    <img src="desktop-image.jpg" alt="Adaptive image example">
</picture>
```

#### Modern Image Formats
```html
<picture>
    <!-- Modern formats for browsers that support them -->
    <source srcset="image.avif" type="image/avif">
    <source srcset="image.webp" type="image/webp">
    <!-- Fallback for older browsers -->
    <img src="image.jpg" alt="Modern format example">
</picture>
```

### Figure and Figcaption

Use `<figure>` and `<figcaption>` to group media with captions:

```html
<figure>
    <img src="web-development.jpg" 
         alt="Developer coding on multiple monitors">
    <figcaption>
        A web developer working on a responsive website layout
    </figcaption>
</figure>

<figure>
    <img src="chart.png" 
         alt="Bar chart showing website traffic growth">
    <figcaption>
        <strong>Figure 1:</strong> Website traffic increased by 150% 
        after implementing responsive design
    </figcaption>
</figure>
```

### HTML Audio

Add audio content to your web pages:

#### Basic Audio Implementation
```html
<audio controls>
    <source src="audio.mp3" type="audio/mpeg">
    <source src="audio.ogg" type="audio/ogg">
    Your browser does not support the audio element.
</audio>
```

#### Advanced Audio with Attributes
```html
<audio controls 
       autoplay 
       muted 
       loop 
       preload="auto">
    <source src="podcast-episode.mp3" type="audio/mpeg">
    <source src="podcast-episode.ogg" type="audio/ogg">
    <p>Your browser doesn't support audio. 
       <a href="podcast-episode.mp3">Download the audio file</a>.</p>
</audio>
```

**Audio Attributes:**
- `controls`: Show play/pause/volume controls
- `autoplay`: Start playing automatically (use with caution)
- `muted`: Start muted (required for autoplay in many browsers)
- `loop`: Repeat audio when it ends
- `preload`: How much to preload (none, metadata, auto)

### HTML Video

Embed video content effectively:

#### Basic Video Implementation
```html
<video controls width="640" height="360">
    <source src="demo-video.mp4" type="video/mp4">
    <source src="demo-video.webm" type="video/webm">
    Your browser does not support the video element.
</video>
```

#### Advanced Video with Poster and Attributes
```html
<video controls 
       width="800" 
       height="450"
       poster="video-thumbnail.jpg"
       preload="metadata">
    <source src="tutorial.mp4" type="video/mp4">
    <source src="tutorial.webm" type="video/webm">
    
    <!-- Subtitles/Captions -->
    <track src="subtitles-en.vtt" 
           kind="subtitles" 
           srclang="en" 
           label="English">
    <track src="subtitles-es.vtt" 
           kind="subtitles" 
           srclang="es" 
           label="Español">
    
    <p>Your browser doesn't support video. 
       <a href="tutorial.mp4">Download the video</a>.</p>
</video>
```

#### Video with Figure Caption
```html
<figure>
    <video controls width="100%" height="auto">
        <source src="bootstrap-tutorial.mp4" type="video/mp4">
        <source src="bootstrap-tutorial.webm" type="video/webm">
    </video>
    <figcaption>
        <strong>Tutorial:</strong> Getting Started with Bootstrap 5 Grid System
    </figcaption>
</figure>
```

### Embedded Content

Use `<iframe>` for embedding external content:

```html
<!-- YouTube Video -->
<iframe width="560" 
        height="315" 
        src="https://www.youtube.com/embed/VIDEO_ID"
        title="YouTube video player"
        frameborder="0"
        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
        allowfullscreen>
</iframe>

<!-- Google Maps -->
<iframe src="https://www.google.com/maps/embed?pb=..."
        width="600"
        height="450"
        style="border:0;"
        allowfullscreen=""
        loading="lazy"
        referrerpolicy="no-referrer-when-downgrade"
        title="Office location map">
</iframe>

<!-- CodePen Embed -->
<iframe height="400" 
        style="width: 100%;" 
        scrolling="no" 
        title="CSS Grid Example"
        src="https://codepen.io/username/embed/HASH"
        frameborder="no" 
        loading="lazy"
        allowtransparency="true" 
        allowfullscreen="true">
</iframe>
```

## 💻 Hands-On Practice

### Exercise 1: Photography Portfolio

Create `photography-portfolio.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Sarah Johnson Photography - Portfolio</title>
</head>
<body>
    <header>
        <h1>Sarah Johnson Photography</h1>
        <nav>
            <ul>
                <li><a href="#portfolio">Portfolio</a></li>
                <li><a href="#about">About</a></li>
                <li><a href="#services">Services</a></li>
                <li><a href="#contact">Contact</a></li>
            </ul>
        </nav>
    </header>
    
    <main>
        <!-- Hero Section with Background Image -->
        <section id="hero">
            <figure>
                <picture>
                    <source media="(max-width: 600px)" 
                            srcset="hero-mobile.jpg">
                    <source media="(max-width: 1200px)" 
                            srcset="hero-tablet.jpg">
                    <img src="hero-desktop.jpg" 
                         alt="Stunning landscape photograph of mountain sunrise"
                         width="1200" 
                         height="600">
                </picture>
                <figcaption>
                    <h2>Capturing Life's Beautiful Moments</h2>
                    <p>Professional photography services for weddings, portraits, and events</p>
                </figcaption>
            </figure>
        </section>
        
        <!-- Portfolio Gallery -->
        <section id="portfolio">
            <h2>Featured Work</h2>
            
            <article>
                <h3>Wedding Photography</h3>
                <div class="gallery">
                    <figure>
                        <img src="wedding-1.jpg" 
                             alt="Bride and groom exchanging vows outdoors"
                             width="400" 
                             height="300"
                             loading="lazy">
                        <figcaption>
                            <strong>Emma & David's Wedding</strong><br>
                            Central Park, New York - June 2025
                        </figcaption>
                    </figure>
                    
                    <figure>
                        <img src="wedding-2.jpg" 
                             alt="Wedding reception with dancing guests"
                             width="400" 
                             height="300"
                             loading="lazy">
                        <figcaption>
                            <strong>Reception Celebration</strong><br>
                            The Plaza Hotel, New York
                        </figcaption>
                    </figure>
                    
                    <figure>
                        <img src="wedding-3.jpg" 
                             alt="Close-up of wedding rings on bouquet"
                             width="400" 
                             height="300"
                             loading="lazy">
                        <figcaption>
                            <strong>Detail Shot</strong><br>
                            Wedding rings and bridal bouquet
                        </figcaption>
                    </figure>
                </div>
            </article>
            
            <article>
                <h3>Portrait Sessions</h3>
                <div class="gallery">
                    <figure>
                        <picture>
                            <source srcset="portrait-1.avif" type="image/avif">
                            <source srcset="portrait-1.webp" type="image/webp">
                            <img src="portrait-1.jpg" 
                                 alt="Professional headshot of business woman"
                                 width="400" 
                                 height="500"
                                 loading="lazy">
                        </picture>
                        <figcaption>
                            <strong>Professional Headshot</strong><br>
                            Corporate portrait session
                        </figcaption>
                    </figure>
                    
                    <figure>
                        <picture>
                            <source srcset="family-portrait.avif" type="image/avif">
                            <source srcset="family-portrait.webp" type="image/webp">
                            <img src="family-portrait.jpg" 
                                 alt="Happy family of four in autumn setting"
                                 width="400" 
                                 height="300"
                                 loading="lazy">
                        </picture>
                        <figcaption>
                            <strong>Family Portrait</strong><br>
                            Autumn family session in Central Park
                        </figcaption>
                    </figure>
                </div>
            </article>
            
            <article>
                <h3>Event Photography</h3>
                <div class="gallery">
                    <figure>
                        <img src="corporate-event.jpg" 
                             alt="Corporate conference with speaker on stage"
                             width="600" 
                             height="400"
                             loading="lazy">
                        <figcaption>
                            <strong>Tech Conference 2025</strong><br>
                            Manhattan Convention Center - 500 attendees
                        </figcaption>
                    </figure>
                    
                    <figure>
                        <img src="birthday-party.jpg" 
                             alt="Children's birthday party with cake and decorations"
                             width="400" 
                             height="300"
                             loading="lazy">
                        <figcaption>
                            <strong>Birthday Celebration</strong><br>
                            Isabella's 8th birthday party
                        </figcaption>
                    </figure>
                </div>
            </article>
        </section>
        
        <!-- About Section with Video -->
        <section id="about">
            <h2>About Sarah</h2>
            <div class="about-content">
                <figure>
                    <img src="sarah-headshot.jpg" 
                         alt="Professional headshot of Sarah Johnson, photographer"
                         width="300" 
                         height="400">
                    <figcaption>Sarah Johnson, Professional Photographer</figcaption>
                </figure>
                
                <div class="about-text">
                    <p>With over 10 years of experience in professional photography, 
                       I specialize in capturing authentic moments that tell your unique story.</p>
                    
                    <p>My approach combines technical expertise with artistic vision to create 
                       images that you'll treasure for a lifetime.</p>
                    
                    <h3>Behind the Scenes</h3>
                    <figure>
                        <video controls 
                               width="500" 
                               height="300"
                               poster="video-poster.jpg">
                            <source src="behind-scenes.mp4" type="video/mp4">
                            <source src="behind-scenes.webm" type="video/webm">
                            <track src="subtitles.vtt" 
                                   kind="subtitles" 
                                   srclang="en" 
                                   label="English">
                            <p>Your browser doesn't support video. 
                               <a href="behind-scenes.mp4">Watch the video</a>.</p>
                        </video>
                        <figcaption>
                            Behind the scenes of a wedding photoshoot
                        </figcaption>
                    </figure>
                </div>
            </div>
        </section>
        
        <!-- Client Testimonials with Audio -->
        <section id="testimonials">
            <h2>Client Testimonials</h2>
            
            <article>
                <blockquote>
                    <p>"Sarah captured our wedding day perfectly. Every photo tells a story 
                       and brings back wonderful memories."</p>
                </blockquote>
                <cite>- Emma & David, Wedding Clients</cite>
                
                <figure>
                    <audio controls preload="metadata">
                        <source src="testimonial-emma.mp3" type="audio/mpeg">
                        <source src="testimonial-emma.ogg" type="audio/ogg">
                        <p>Audio testimonial not available. 
                           <a href="testimonial-emma.mp3">Download audio</a>.</p>
                    </audio>
                    <figcaption>Audio testimonial from Emma (2 minutes)</figcaption>
                </figure>
            </article>
            
            <article>
                <blockquote>
                    <p>"Professional, creative, and delivered exactly what we needed 
                       for our corporate event."</p>
                </blockquote>
                <cite>- Tech Corp Marketing Team</cite>
            </article>
        </section>
        
        <!-- Equipment & Services -->
        <section id="services">
            <h2>Services & Equipment</h2>
            
            <article>
                <h3>Photography Services</h3>
                <ul>
                    <li>Wedding Photography (Full day coverage)</li>
                    <li>Portrait Sessions (Individual, Family, Corporate)</li>
                    <li>Event Photography (Corporate, Private parties)</li>
                    <li>Product Photography (E-commerce, Marketing)</li>
                    <li>Real Estate Photography</li>
                </ul>
            </article>
            
            <article>
                <h3>Professional Equipment</h3>
                <figure>
                    <img src="camera-equipment.jpg" 
                         alt="Professional camera equipment including DSLR, lenses, and lighting"
                         width="600" 
                         height="400"
                         loading="lazy">
                    <figcaption>
                        Professional-grade Canon and Sony equipment for all photography needs
                    </figcaption>
                </figure>
                
                <dl>
                    <dt>Cameras</dt>
                    <dd>Canon EOS R5, Sony A7R IV - Full-frame mirrorless cameras</dd>
                    
                    <dt>Lenses</dt>
                    <dd>24-70mm f/2.8, 70-200mm f/2.8, 85mm f/1.4 portrait lens</dd>
                    
                    <dt>Lighting</dt>
                    <dd>Professional strobes, continuous LED panels, reflectors</dd>
                    
                    <dt>Accessories</dt>
                    <dd>Professional tripods, wireless triggers, backup equipment</dd>
                </dl>
            </article>
        </section>
    </main>
    
    <footer>
        <section id="contact">
            <h2>Contact Information</h2>
            <address>
                <p><strong>Sarah Johnson Photography</strong></p>
                <p>New York, NY</p>
                <p>Phone: <a href="tel:+1234567890">(123) 456-7890</a></p>
                <p>Email: <a href="mailto:sarah@sarahjohnsonphoto.com">sarah@sarahjohnsonphoto.com</a></p>
            </address>
            
            <div class="social-media">
                <h3>Follow My Work</h3>
                <ul>
                    <li><a href="https://instagram.com/sarahjphoto" target="_blank">Instagram</a></li>
                    <li><a href="https://facebook.com/sarahjohnsonphoto" target="_blank">Facebook</a></li>
                    <li><a href="https://pinterest.com/sarahjphoto" target="_blank">Pinterest</a></li>
                </ul>
            </div>
        </section>
        
        <p>&copy; 2025 Sarah Johnson Photography. All rights reserved.</p>
    </footer>
</body>
</html>
```

### Exercise 2: Educational Media Page

Create `multimedia-learning.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Web Development Multimedia Learning Center</title>
</head>
<body>
    <header>
        <h1>Web Development Learning Center</h1>
        <nav>
            <ul>
                <li><a href="#videos">Video Tutorials</a></li>
                <li><a href="#audio">Podcasts</a></li>
                <li><a href="#images">Visual Guides</a></li>
                <li><a href="#interactive">Interactive Content</a></li>
            </ul>
        </nav>
    </header>
    
    <main>
        <!-- Video Tutorials Section -->
        <section id="videos">
            <h2>📹 Video Tutorials</h2>
            
            <article>
                <h3>HTML Fundamentals Series</h3>
                
                <figure>
                    <video controls 
                           width="640" 
                           height="360"
                           poster="html-basics-poster.jpg">
                        <source src="html-basics-tutorial.mp4" type="video/mp4">
                        <source src="html-basics-tutorial.webm" type="video/webm">
                        
                        <track src="subtitles-en.vtt" 
                               kind="subtitles" 
                               srclang="en" 
                               label="English"
                               default>
                        <track src="subtitles-es.vtt" 
                               kind="subtitles" 
                               srclang="es" 
                               label="Español">
                        
                        <p>Your browser doesn't support video. 
                           <a href="html-basics-tutorial.mp4">Download the tutorial</a>.</p>
                    </video>
                    <figcaption>
                        <strong>Episode 1:</strong> HTML Document Structure and Semantic Elements (15:30)
                    </figcaption>
                </figure>
                
                <figure>
                    <video controls 
                           width="640" 
                           height="360"
                           poster="html-forms-poster.jpg">
                        <source src="html-forms-tutorial.mp4" type="video/mp4">
                        <source src="html-forms-tutorial.webm" type="video/webm">
                        
                        <track src="forms-subtitles-en.vtt" 
                               kind="subtitles" 
                               srclang="en" 
                               label="English"
                               default>
                        
                        <p>Video not supported. <a href="html-forms-tutorial.mp4">Download</a>.</p>
                    </video>
                    <figcaption>
                        <strong>Episode 2:</strong> Creating Accessible HTML Forms (22:45)
                    </figcaption>
                </figure>
            </article>
            
            <article>
                <h3>Bootstrap Framework Mastery</h3>
                
                <!-- Embedded YouTube Video -->
                <figure>
                    <iframe width="640" 
                            height="360" 
                            src="https://www.youtube.com/embed/dQw4w9WgXcQ"
                            title="Bootstrap 5 Grid System Tutorial"
                            frameborder="0"
                            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                            allowfullscreen>
                    </iframe>
                    <figcaption>
                        <strong>Bootstrap 5 Grid System:</strong> Master responsive layouts in 30 minutes
                    </figcaption>
                </figure>
            </article>
        </section>
        
        <!-- Audio Podcasts Section -->
        <section id="audio">
            <h2>🎧 Web Development Podcasts</h2>
            
            <article>
                <h3>Weekly Web Dev Talks</h3>
                
                <figure>
                    <audio controls preload="metadata">
                        <source src="podcast-episode-1.mp3" type="audio/mpeg">
                        <source src="podcast-episode-1.ogg" type="audio/ogg">
                        <p>Audio not supported. 
                           <a href="podcast-episode-1.mp3">Download Episode 1</a>.</p>
                    </audio>
                    <figcaption>
                        <strong>Episode 1:</strong> "The Future of Web Development" 
                        with Jane Smith (45:20)
                    </figcaption>
                </figure>
                
                <p><strong>Show Notes:</strong></p>
                <ul>
                    <li>Modern JavaScript frameworks comparison</li>
                    <li>CSS Grid vs Flexbox - when to use which</li>
                    <li>Web accessibility trends for 2025</li>
                    <li>Performance optimization strategies</li>
                </ul>
                
                <figure>
                    <audio controls preload="metadata">
                        <source src="podcast-episode-2.mp3" type="audio/mpeg">
                        <source src="podcast-episode-2.ogg" type="audio/ogg">
                        <p>Audio not supported. 
                           <a href="podcast-episode-2.mp3">Download Episode 2</a>.</p>
                    </audio>
                    <figcaption>
                        <strong>Episode 2:</strong> "Bootstrap vs Custom CSS" 
                        with Mike Johnson (38:15)
                    </figcaption>
                </figure>
            </article>
        </section>
        
        <!-- Visual Guides Section -->
        <section id="images">
            <h2>📊 Visual Learning Guides</h2>
            
            <article>
                <h3>HTML Element Hierarchy</h3>
                <figure>
                    <picture>
                        <source media="(max-width: 600px)" 
                                srcset="html-hierarchy-mobile.svg">
                        <source media="(max-width: 1000px)" 
                                srcset="html-hierarchy-tablet.svg">
                        <img src="html-hierarchy-desktop.svg" 
                             alt="Diagram showing HTML document structure hierarchy"
                             width="800" 
                             height="600">
                    </picture>
                    <figcaption>
                        <strong>Figure 1:</strong> HTML5 Document Structure - 
                        Shows the relationship between html, head, body, and semantic elements
                    </figcaption>
                </figure>
            </article>
            
            <article>
                <h3>CSS Box Model Visualization</h3>
                <figure>
                    <img src="css-box-model-diagram.png" 
                         alt="CSS box model showing content, padding, border, and margin"
                         width="600" 
                         height="400"
                         loading="lazy">
                    <figcaption>
                        <strong>Figure 2:</strong> CSS Box Model - 
                        Understanding content, padding, border, and margin relationships
                    </figcaption>
                </figure>
            </article>
            
            <article>
                <h3>Bootstrap Grid System</h3>
                <figure>
                    <picture>
                        <source srcset="bootstrap-grid.avif" type="image/avif">
                        <source srcset="bootstrap-grid.webp" type="image/webp">
                        <img src="bootstrap-grid.png" 
                             alt="Bootstrap 12-column grid system with responsive breakpoints"
                             width="1000" 
                             height="600"
                             loading="lazy">
                    </picture>
                    <figcaption>
                        <strong>Figure 3:</strong> Bootstrap 5 Grid System - 
                        12-column layout with responsive breakpoint examples
                    </figcaption>
                </figure>
            </article>
            
            <article>
                <h3>Code Screenshots</h3>
                <figure>
                    <img src="html-code-example.png" 
                         alt="Screenshot of HTML code with syntax highlighting"
                         width="700" 
                         height="400"
                         loading="lazy">
                    <figcaption>
                        <strong>Code Example 1:</strong> Semantic HTML structure 
                        for a blog post layout
                    </figcaption>
                </figure>
                
                <figure>
                    <img src="css-flexbox-code.png" 
                         alt="Screenshot of CSS flexbox properties with visual result"
                         width="700" 
                         height="350"
                         loading="lazy">
                    <figcaption>
                        <strong>Code Example 2:</strong> CSS Flexbox properties 
                        for centering content
                    </figcaption>
                </figure>
            </article>
        </section>
        
        <!-- Interactive Content Section -->
        <section id="interactive">
            <h2>🔧 Interactive Learning Tools</h2>
            
            <article>
                <h3>CodePen Examples</h3>
                
                <figure>
                    <iframe height="400" 
                            style="width: 100%;" 
                            scrolling="no" 
                            title="HTML Semantic Elements Demo"
                            src="https://codepen.io/username/embed/HASH1"
                            frameborder="no" 
                            loading="lazy"
                            allowtransparency="true" 
                            allowfullscreen="true">
                        <p>Codepen not supported. 
                           <a href="https://codepen.io/username/pen/HASH1">View the code</a>.</p>
                    </iframe>
                    <figcaption>
                        <strong>Interactive Demo:</strong> HTML5 Semantic Elements in Action
                    </figcaption>
                </figure>
                
                <figure>
                    <iframe height="500" 
                            style="width: 100%;" 
                            scrolling="no" 
                            title="Bootstrap Grid Playground"
                            src="https://codepen.io/username/embed/HASH2"
                            frameborder="no" 
                            loading="lazy"
                            allowtransparency="true" 
                            allowfullscreen="true">
                        <p>Codepen not supported. 
                           <a href="https://codepen.io/username/pen/HASH2">View the code</a>.</p>
                    </iframe>
                    <figcaption>
                        <strong>Interactive Demo:</strong> Bootstrap Grid System Playground
                    </figcaption>
                </figure>
            </article>
            
            <article>
                <h3>External Learning Resources</h3>
                
                <figure>
                    <iframe src="https://developer.mozilla.org/en-US/docs/Web/HTML"
                            width="100%"
                            height="600"
                            title="MDN HTML Documentation"
                            loading="lazy">
                        <p>External content not supported. 
                           <a href="https://developer.mozilla.org/en-US/docs/Web/HTML">
                           Visit MDN HTML Documentation</a>.</p>
                    </iframe>
                    <figcaption>
                        <strong>Reference:</strong> MDN Web Docs - HTML Documentation
                    </figcaption>
                </figure>
            </article>
        </section>
        
        <!-- Download Resources -->
        <section id="downloads">
            <h2>📥 Downloadable Resources</h2>
            
            <article>
                <h3>Course Materials</h3>
                <ul>
                    <li>
                        <a href="html-cheatsheet.pdf" download>
                            📄 HTML5 Cheat Sheet (PDF - 2.5MB)
                        </a>
                    </li>
                    <li>
                        <a href="bootstrap-guide.pdf" download="bootstrap-complete-guide.pdf">
                            📄 Bootstrap 5 Complete Guide (PDF - 4.2MB)
                        </a>
                    </li>
                    <li>
                        <a href="code-examples.zip" download>
                            📦 All Code Examples (ZIP - 1.8MB)
                        </a>
                    </li>
                    <li>
                        <a href="project-templates.zip" download>
                            🎨 Starter Templates (ZIP - 3.1MB)
                        </a>
                    </li>
                </ul>
                
                <figure>
                    <img src="cheatsheet-preview.jpg" 
                         alt="Preview of HTML5 cheat sheet showing element categories"
                         width="400" 
                         height="300"
                         loading="lazy">
                    <figcaption>
                        Preview of the downloadable HTML5 cheat sheet
                    </figcaption>
                </figure>
            </article>
        </section>
    </main>
    
    <footer>
        <p>&copy; 2025 Web Development Learning Center. 
           All media content is for educational purposes.</p>
        <p>
            <a href="#videos">Videos</a> | 
            <a href="#audio">Audio</a> | 
            <a href="#images">Images</a> | 
            <a href="#interactive">Interactive</a>
        </p>
    </footer>
</body>
</html>
```

## 📋 Media Accessibility Best Practices

### Image Accessibility
```html
<!-- Good alt text - descriptive and contextual -->
<img src="chart.png" 
     alt="Sales increased 40% from January to June 2025">

<!-- Bad alt text - too generic -->
<img src="chart.png" alt="chart">

<!-- Decorative images - empty alt -->
<img src="decorative-border.png" alt="" role="presentation">

<!-- Complex images need detailed descriptions -->
<figure>
    <img src="complex-diagram.png" 
         alt="Workflow diagram showing 5 steps of web development process">
    <figcaption>
        Detailed description: The diagram shows the web development process 
        starting with Planning, followed by Design, Development, Testing, 
        and ending with Deployment. Each step connects to the next with arrows.
    </figcaption>
</figure>
```

### Video Accessibility
```html
<video controls>
    <source src="tutorial.mp4" type="video/mp4">
    
    <!-- Captions for hearing impaired -->
    <track src="captions.vtt" 
           kind="captions" 
           srclang="en" 
           label="English Captions"
           default>
    
    <!-- Subtitles for different languages -->
    <track src="subtitles-es.vtt" 
           kind="subtitles" 
           srclang="es" 
           label="Spanish Subtitles">
    
    <!-- Audio descriptions for visually impaired -->
    <track src="descriptions.vtt" 
           kind="descriptions" 
           srclang="en" 
           label="Audio Descriptions">
    
    <p>Video not supported. <a href="tutorial.mp4">Download video</a>.</p>
</video>
```

## 🎉 Day 4 Checklist

- [ ] Implemented responsive images with srcset and picture elements
- [ ] Used modern image formats (WebP, AVIF) with fallbacks
- [ ] Added audio content with proper controls and formats
- [ ] Embedded video with captions and accessibility features
- [ ] Used figure and figcaption for media context
- [ ] Created multimedia-rich photography portfolio
- [ ] Built educational media page with various content types
- [ ] Applied media accessibility best practices
- [ ] Implemented lazy loading for performance

## 🔗 Additional Resources

- **[MDN Images Guide](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/img)** - Complete image documentation
- **[WebP Image Format](https://developers.google.com/speed/webp)** - Modern image format
- **[Web Video Best Practices](https://web.dev/video/)** - Video optimization guide
- **[WebVTT Caption Guide](https://developer.mozilla.org/en-US/docs/Web/API/WebVTT_API)** - Video caption format

---

## 🚀 What's Next?

Tomorrow in **Day 05: Forms & Input Elements**, you'll master:
- Creating accessible and user-friendly forms
- Different input types and form controls
- Form validation and user experience
- Advanced form patterns and techniques

**Amazing work on Day 4!** You've learned to create rich, multimedia experiences that engage users while maintaining accessibility.

---

**Remember: Great media enhances the user experience but should never be a barrier to accessibility!** 🎬✨