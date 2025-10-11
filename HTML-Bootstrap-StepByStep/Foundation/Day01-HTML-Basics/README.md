# Day 01: HTML Basics & Document Structure 📝

Welcome to your HTML Bootstrap learning journey! Today we'll master the fundamentals of HTML document structure and set up a solid foundation for web development.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Understand HTML5 document structure and anatomy
- Create properly formatted HTML documents
- Use essential meta tags for SEO and responsive design
- Validate HTML code and follow best practices
- Set up a basic development workflow

## 📚 Lesson Content

### What is HTML?

**HTML (HyperText Markup Language)** is the standard markup language for creating web pages. It describes the structure and content of web pages using elements and tags.

**Key Concepts:**
- **Elements**: Building blocks of HTML (paragraphs, headings, links)
- **Tags**: Keywords enclosed in angle brackets `<tag>`
- **Attributes**: Additional information about elements
- **Semantic Markup**: Using HTML elements for their intended meaning

### HTML5 Document Structure

Every HTML5 document follows this basic structure:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Document Title</title>
</head>
<body>
    <!-- Your content goes here -->
</body>
</html>
```

**Structure Breakdown:**

1. **`<!DOCTYPE html>`**: Declares HTML5 document type
2. **`<html>`**: Root element containing all content
3. **`<head>`**: Contains metadata not displayed on page
4. **`<body>`**: Contains visible page content

### Essential Meta Tags

```html
<head>
    <!-- Character encoding for text -->
    <meta charset="UTF-8">
    
    <!-- Responsive design viewport -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <!-- Page description for SEO -->
    <meta name="description" content="Brief page description for search engines">
    
    <!-- Keywords for SEO (less important now) -->
    <meta name="keywords" content="html, css, web development">
    
    <!-- Author information -->
    <meta name="author" content="Your Name">
    
    <!-- Page title (appears in browser tab) -->
    <title>Descriptive Page Title</title>
</head>
```

### HTML Elements Anatomy

```html
<!-- Opening tag + Content + Closing tag -->
<tagname attribute="value">Content goes here</tagname>

<!-- Self-closing tags (no content) -->
<tagname attribute="value" />

<!-- Examples -->
<h1>This is a heading</h1>
<p class="intro">This is a paragraph with a class attribute.</p>
<img src="image.jpg" alt="Description" />
<br />
```

### Basic HTML Elements

```html
<!-- Headings (h1 is most important, h6 least important) -->
<h1>Main Page Title</h1>
<h2>Section Heading</h2>
<h3>Subsection Heading</h3>

<!-- Paragraphs -->
<p>This is a paragraph of text content.</p>

<!-- Line breaks -->
<br />

<!-- Horizontal rule -->
<hr />

<!-- Comments (not displayed) -->
<!-- This is a comment -->
```

### Text Formatting Elements

```html
<!-- Bold and Strong -->
<b>Bold text (visual only)</b>
<strong>Strong text (semantic importance)</strong>

<!-- Italic and Emphasis -->
<i>Italic text (visual only)</i>
<em>Emphasized text (semantic emphasis)</em>

<!-- Other formatting -->
<u>Underlined text</u>
<s>Strikethrough text</s>
<mark>Highlighted text</mark>
<small>Small text</small>
<sub>Subscript</sub>
<sup>Superscript</sup>
```

## 💻 Hands-On Practice

### Exercise 1: Create Your First HTML Page

Create a file named `my-first-page.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="My first HTML page learning web development">
    <meta name="author" content="Your Name">
    <title>My First HTML Page</title>
</head>
<body>
    <h1>Welcome to My First HTML Page!</h1>
    
    <h2>About Me</h2>
    <p>Hello! I'm learning <strong>HTML</strong> and <em>web development</em>. 
       This is my first step towards becoming a web developer.</p>
    
    <h2>My Goals</h2>
    <p>By the end of this course, I will be able to:</p>
    <!-- We'll add a list here tomorrow! -->
    
    <hr>
    
    <p><small>Created on: October 2025</small></p>
</body>
</html>
```

### Exercise 2: Personal Information Page

Create `about-me.html` with the following structure:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="Personal information page">
    <title>About Me - [Your Name]</title>
</head>
<body>
    <h1>About [Your Name]</h1>
    
    <h2>Personal Information</h2>
    <p><strong>Name:</strong> [Your Full Name]</p>
    <p><strong>Location:</strong> [Your City, Country]</p>
    <p><strong>Occupation:</strong> [Your Job/Student Status]</p>
    
    <h2>Introduction</h2>
    <p>Write a brief paragraph about yourself, your interests, 
       and why you're learning web development.</p>
    
    <h2>Fun Facts</h2>
    <p>Share 2-3 interesting facts about yourself using different 
       text formatting elements like <em>emphasis</em>, 
       <strong>strong importance</strong>, or <mark>highlighting</mark>.</p>
    
    <h2>Learning Journey</h2>
    <p>Describe your goals for this HTML Bootstrap course.</p>
    
    <hr>
    
    <p><small>Last updated: <time>October 2025</time></small></p>
</body>
</html>
```

## 🔧 Development Tips

### Setting Up VS Code for HTML

1. **Install Extensions:**
   - HTML CSS Support
   - Auto Rename Tag
   - Live Server
   - Prettier - Code formatter

2. **Enable Emmet:**
   - Type `!` and press Tab for HTML5 template
   - Type `h1` and press Tab for `<h1></h1>`
   - Type `p.intro` and press Tab for `<p class="intro"></p>`

3. **Use Live Server:**
   - Right-click HTML file → "Open with Live Server"
   - Automatically refreshes browser when you save

### HTML Best Practices

✅ **Do:**
- Always include `<!DOCTYPE html>`
- Use semantic HTML elements
- Include `lang` attribute in `<html>`
- Add `alt` attributes to images
- Use proper heading hierarchy (h1→h2→h3)
- Validate your HTML code
- Use meaningful indentation

❌ **Don't:**
- Skip closing tags
- Use deprecated elements like `<font>` or `<center>`
- Use HTML for styling (use CSS instead)
- Forget meta viewport tag
- Use non-semantic elements when semantic ones exist

### HTML Validation

**Online Validators:**
- W3C Markup Validator: https://validator.w3.org/
- HTML5 Validator: https://html5.validator.nu/

**VS Code Validation:**
- Install "HTMLHint" extension
- Automatically highlights errors and warnings

## 🎯 Mini Project: Personal Homepage

Create a complete personal homepage with the following requirements:

**File: `homepage.html`**

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="[Your Name] - Personal Homepage">
    <meta name="keywords" content="personal, homepage, web developer">
    <meta name="author" content="[Your Name]">
    <title>[Your Name] - Personal Homepage</title>
</head>
<body>
    <!-- Header Section -->
    <h1>Welcome to [Your Name]'s Homepage</h1>
    
    <!-- Introduction -->
    <h2>Hello, World!</h2>
    <p>I'm <strong>[Your Name]</strong>, and I'm on an exciting journey 
       to master <em>web development</em>. This homepage represents 
       <mark>Day 1</mark> of my HTML Bootstrap learning adventure!</p>
    
    <!-- About Section -->
    <h2>About Me</h2>
    <p>Write a compelling paragraph about who you are, what you do, 
       and what drives your passion for web development.</p>
    
    <!-- Current Learning -->
    <h2>What I'm Learning</h2>
    <p>Currently diving deep into:</p>
    <p><strong>HTML5:</strong> Semantic markup and document structure</p>
    <p><strong>Upcoming:</strong> CSS styling and Bootstrap framework</p>
    
    <!-- Goals Section -->
    <h2>My 56-Day Challenge</h2>
    <p>I've committed to a <strong>56-day intensive course</strong> to master 
       HTML and Bootstrap. My goal is to build <em>amazing, responsive websites</em> 
       and become a proficient front-end developer.</p>
    
    <!-- Quote Section -->
    <h2>Favorite Quote</h2>
    <p><em>"The best time to plant a tree was 20 years ago. 
       The second best time is now."</em></p>
    <p><small>- Chinese Proverb</small></p>
    
    <!-- Footer -->
    <hr>
    <p><small>Created with ❤️ on Day 1 of my web development journey | 
       October 2025</small></p>
</body>
</html>
```

## 📋 Homework Assignments

### Assignment 1: Document Structure Practice
Create three different HTML pages:
1. `recipe.html` - A favorite recipe with ingredients and instructions
2. `review.html` - A movie or book review
3. `tutorial.html` - A simple how-to guide

Each page should:
- Use proper HTML5 structure
- Include appropriate meta tags
- Use semantic heading hierarchy
- Apply text formatting where appropriate
- Validate without errors

### Assignment 2: Experiment with Elements
Create `experiments.html` and try:
- Different heading combinations (h1-h6)
- Various text formatting elements
- Different meta tag configurations
- HTML comments to document your code

## 🎉 Day 1 Checklist

- [ ] Created first HTML5 document with proper structure
- [ ] Used essential meta tags for SEO and responsiveness
- [ ] Applied basic text formatting elements
- [ ] Followed HTML best practices and validation
- [ ] Completed personal homepage mini-project
- [ ] Set up development environment with Live Server
- [ ] Validated HTML code for errors

## 🔗 Additional Resources

### Documentation
- [MDN HTML Basics](https://developer.mozilla.org/en-US/docs/Learn/Getting_started_with_the_web/HTML_basics)
- [W3Schools HTML Tutorial](https://www.w3schools.com/html/)
- [HTML5 Specification](https://www.w3.org/TR/html52/)

### Tools
- [HTML Validator](https://validator.w3.org/)
- [Can I Use](https://caniuse.com/) - Browser compatibility
- [HTML5 Outliner](https://gsnedders.html5.org/outliner/) - Document structure

### Practice
- [freeCodeCamp HTML Course](https://www.freecodecamp.org/learn/responsive-web-design/)
- [Codecademy HTML Course](https://www.codecademy.com/learn/learn-html)

---

## 🚀 What's Next?

Tomorrow in **Day 02: Text Elements & Semantic Markup**, we'll dive deeper into:
- Semantic HTML5 elements (article, section, aside)
- Advanced text formatting and structure
- Building meaningful document outlines
- Creating accessible content structure

**Great job completing Day 1!** You've laid the foundation for your web development journey. Keep practicing and experimenting with HTML structure.

---

**Remember**: The key to mastering HTML is consistent practice. Try to code along with every example and don't hesitate to experiment beyond the provided exercises!

**Happy Coding!** 🎉