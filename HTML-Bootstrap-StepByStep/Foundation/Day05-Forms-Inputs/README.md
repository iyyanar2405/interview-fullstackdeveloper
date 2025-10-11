# Day 05: Forms & Input Elements 📝

Welcome to Day 5! Today we'll master HTML forms - the gateway for user interaction on the web. You'll learn to create accessible, user-friendly forms that collect data effectively and provide excellent user experience.

## 🎯 Learning Objectives

By the end of this lesson, you will:
- Create comprehensive forms with all input types
- Implement form validation and user feedback
- Apply accessibility best practices for forms
- Use fieldsets, labels, and proper form structure
- Handle different form submission methods
- Create modern, responsive form layouts
- Understand form security considerations

## 📚 Lesson Content

### Form Basics

Forms allow users to submit data to web servers. Every form needs proper structure and accessibility.

#### Basic Form Structure
```html
<form action="/submit-contact" method="post">
    <fieldset>
        <legend>Contact Information</legend>
        
        <label for="name">Full Name:</label>
        <input type="text" id="name" name="name" required>
        
        <label for="email">Email Address:</label>
        <input type="email" id="email" name="email" required>
        
        <button type="submit">Submit</button>
    </fieldset>
</form>
```

**Key Elements:**
- `<form>`: Container for all form elements
- `action`: Where form data is sent
- `method`: How data is sent (GET or POST)
- `<fieldset>`: Groups related form controls
- `<legend>`: Caption for fieldset
- `<label>`: Describes input elements
- `for` attribute: Links label to input

### Input Types

HTML5 provides many input types for different data:

#### Text Inputs
```html
<!-- Basic text input -->
<label for="username">Username:</label>
<input type="text" 
       id="username" 
       name="username" 
       placeholder="Enter your username"
       minlength="3"
       maxlength="20"
       required>

<!-- Password input -->
<label for="password">Password:</label>
<input type="password" 
       id="password" 
       name="password" 
       minlength="8"
       required>

<!-- Email input with validation -->
<label for="email">Email:</label>
<input type="email" 
       id="email" 
       name="email" 
       placeholder="user@example.com"
       required>

<!-- Phone number -->
<label for="phone">Phone Number:</label>
<input type="tel" 
       id="phone" 
       name="phone" 
       pattern="[0-9]{3}-[0-9]{3}-[0-9]{4}"
       placeholder="123-456-7890">

<!-- URL input -->
<label for="website">Website:</label>
<input type="url" 
       id="website" 
       name="website" 
       placeholder="https://example.com">

<!-- Search input -->
<label for="search">Search:</label>
<input type="search" 
       id="search" 
       name="search" 
       placeholder="Search our site...">
```

#### Number and Date Inputs
```html
<!-- Number input with range -->
<label for="age">Age:</label>
<input type="number" 
       id="age" 
       name="age" 
       min="13" 
       max="120" 
       value="25">

<!-- Range slider -->
<label for="rating">Rating (1-10):</label>
<input type="range" 
       id="rating" 
       name="rating" 
       min="1" 
       max="10" 
       value="5"
       oninput="ratingValue.textContent = this.value">
<output id="ratingValue">5</output>

<!-- Date inputs -->
<label for="birthdate">Birth Date:</label>
<input type="date" 
       id="birthdate" 
       name="birthdate" 
       min="1900-01-01" 
       max="2010-12-31">

<label for="appointment">Appointment Time:</label>
<input type="datetime-local" 
       id="appointment" 
       name="appointment">

<label for="meeting-time">Meeting Time:</label>
<input type="time" 
       id="meeting-time" 
       name="meeting-time">

<label for="project-month">Project Month:</label>
<input type="month" 
       id="project-month" 
       name="project-month">

<label for="project-week">Project Week:</label>
<input type="week" 
       id="project-week" 
       name="project-week">
```

#### Selection Inputs
```html
<!-- Radio buttons (single choice) -->
<fieldset>
    <legend>Preferred Contact Method:</legend>
    
    <input type="radio" id="contact-email" name="contact-method" value="email" checked>
    <label for="contact-email">Email</label>
    
    <input type="radio" id="contact-phone" name="contact-method" value="phone">
    <label for="contact-phone">Phone</label>
    
    <input type="radio" id="contact-text" name="contact-method" value="text">
    <label for="contact-text">Text Message</label>
</fieldset>

<!-- Checkboxes (multiple choices) -->
<fieldset>
    <legend>Interests (select all that apply):</legend>
    
    <input type="checkbox" id="interest-web" name="interests" value="web-development">
    <label for="interest-web">Web Development</label>
    
    <input type="checkbox" id="interest-design" name="interests" value="design">
    <label for="interest-design">Design</label>
    
    <input type="checkbox" id="interest-marketing" name="interests" value="marketing">
    <label for="interest-marketing">Marketing</label>
    
    <input type="checkbox" id="interest-business" name="interests" value="business">
    <label for="interest-business">Business</label>
</fieldset>

<!-- Dropdown select -->
<label for="country">Country:</label>
<select id="country" name="country" required>
    <option value="">Choose a country</option>
    <option value="US">United States</option>
    <option value="CA">Canada</option>
    <option value="UK">United Kingdom</option>
    <option value="AU">Australia</option>
    <option value="DE">Germany</option>
    <option value="FR">France</option>
</select>

<!-- Multi-select dropdown -->
<label for="skills">Technical Skills:</label>
<select id="skills" name="skills" multiple size="5">
    <optgroup label="Frontend">
        <option value="html">HTML5</option>
        <option value="css">CSS3</option>
        <option value="javascript">JavaScript</option>
        <option value="react">React</option>
        <option value="vue">Vue.js</option>
    </optgroup>
    <optgroup label="Backend">
        <option value="node">Node.js</option>
        <option value="python">Python</option>
        <option value="php">PHP</option>
        <option value="java">Java</option>
    </optgroup>
</select>
```

#### File and Advanced Inputs
```html
<!-- File upload -->
<label for="resume">Upload Resume:</label>
<input type="file" 
       id="resume" 
       name="resume" 
       accept=".pdf,.doc,.docx"
       required>

<!-- Multiple file upload -->
<label for="portfolio">Portfolio Images:</label>
<input type="file" 
       id="portfolio" 
       name="portfolio" 
       accept="image/*" 
       multiple>

<!-- Color picker -->
<label for="brand-color">Brand Color:</label>
<input type="color" 
       id="brand-color" 
       name="brand-color" 
       value="#3498db">

<!-- Hidden input -->
<input type="hidden" name="form-version" value="2.1">
```

### Textarea and Text Areas

For longer text input:

```html
<!-- Basic textarea -->
<label for="message">Message:</label>
<textarea id="message" 
          name="message" 
          rows="5" 
          cols="40" 
          placeholder="Enter your message here..."
          maxlength="500"
          required></textarea>

<!-- Textarea with character counter -->
<label for="bio">Biography (max 250 characters):</label>
<textarea id="bio" 
          name="bio" 
          rows="4" 
          maxlength="250"
          oninput="bioCounter.textContent = this.value.length + '/250'"></textarea>
<div id="bioCounter">0/250</div>
```

### Form Validation

HTML5 provides built-in validation with custom messages:

#### Built-in Validation Attributes
```html
<form novalidate>
    <!-- Required field -->
    <label for="email">Email (required):</label>
    <input type="email" 
           id="email" 
           name="email" 
           required
           title="Please enter a valid email address">
    
    <!-- Pattern validation -->
    <label for="zip">ZIP Code:</label>
    <input type="text" 
           id="zip" 
           name="zip" 
           pattern="[0-9]{5}"
           title="Please enter a 5-digit ZIP code"
           placeholder="12345">
    
    <!-- Length validation -->
    <label for="username">Username (3-15 characters):</label>
    <input type="text" 
           id="username" 
           name="username" 
           minlength="3" 
           maxlength="15"
           required>
    
    <!-- Number range validation -->
    <label for="quantity">Quantity (1-10):</label>
    <input type="number" 
           id="quantity" 
           name="quantity" 
           min="1" 
           max="10" 
           required>
    
    <button type="submit">Submit</button>
</form>
```

#### Custom Validation Messages
```html
<script>
    // Custom validation messages
    document.getElementById('email').addEventListener('invalid', function(e) {
        if (this.validity.valueMissing) {
            this.setCustomValidity('Please enter your email address');
        } else if (this.validity.typeMismatch) {
            this.setCustomValidity('Please enter a valid email address');
        } else {
            this.setCustomValidity('');
        }
    });
    
    document.getElementById('email').addEventListener('input', function(e) {
        this.setCustomValidity('');
    });
</script>
```

### Form Accessibility

Proper accessibility ensures forms work for all users:

#### ARIA Labels and Descriptions
```html
<form>
    <!-- Form with ARIA landmarks -->
    <fieldset role="group" aria-labelledby="billing-info">
        <legend id="billing-info">Billing Information</legend>
        
        <!-- Required field indication -->
        <label for="card-number">
            Credit Card Number 
            <span aria-label="required">*</span>
        </label>
        <input type="text" 
               id="card-number" 
               name="card-number" 
               required
               aria-required="true"
               aria-describedby="card-help">
        <div id="card-help" class="help-text">
            Enter your 16-digit card number without spaces
        </div>
        
        <!-- Error message association -->
        <label for="expiry">Expiry Date:</label>
        <input type="text" 
               id="expiry" 
               name="expiry" 
               pattern="[0-9]{2}/[0-9]{2}"
               aria-describedby="expiry-error"
               aria-invalid="false">
        <div id="expiry-error" class="error" role="alert" style="display: none;">
            Please enter expiry date in MM/YY format
        </div>
    </fieldset>
    
    <!-- Form submission buttons -->
    <div class="form-actions">
        <button type="submit" aria-describedby="submit-help">
            Complete Purchase
        </button>
        <button type="reset">Clear Form</button>
        <div id="submit-help" class="help-text">
            By submitting, you agree to our terms and conditions
        </div>
    </div>
</form>
```

## 💻 Hands-On Practice

### Exercise 1: Complete Registration Form

Create `registration-form.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>TechConf 2025 - Registration Form</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
            line-height: 1.6;
        }
        
        fieldset {
            margin: 20px 0;
            padding: 20px;
            border: 2px solid #ddd;
            border-radius: 8px;
        }
        
        legend {
            font-weight: bold;
            color: #333;
            padding: 0 10px;
        }
        
        label {
            display: block;
            margin: 10px 0 5px;
            font-weight: bold;
        }
        
        input, select, textarea {
            width: 100%;
            padding: 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 14px;
        }
        
        input[type="radio"], input[type="checkbox"] {
            width: auto;
            margin-right: 8px;
        }
        
        .form-row {
            display: flex;
            gap: 15px;
        }
        
        .form-row > div {
            flex: 1;
        }
        
        .help-text {
            font-size: 12px;
            color: #666;
            margin-top: 5px;
        }
        
        .error {
            color: #d32f2f;
            font-size: 12px;
            margin-top: 5px;
        }
        
        .required {
            color: #d32f2f;
        }
        
        .form-actions {
            text-align: center;
            margin: 30px 0;
        }
        
        button {
            background: #007bff;
            color: white;
            padding: 12px 30px;
            border: none;
            border-radius: 4px;
            font-size: 16px;
            cursor: pointer;
            margin: 0 10px;
        }
        
        button:hover {
            background: #0056b3;
        }
        
        button[type="reset"] {
            background: #6c757d;
        }
        
        button[type="reset"]:hover {
            background: #545b62;
        }
    </style>
</head>
<body>
    <header>
        <h1>TechConf 2025 Registration</h1>
        <p>Join us for the premier technology conference of the year! 
           Please complete all required fields marked with <span class="required">*</span></p>
    </header>
    
    <main>
        <form action="/register" method="post" novalidate>
            <!-- Personal Information -->
            <fieldset>
                <legend>Personal Information</legend>
                
                <div class="form-row">
                    <div>
                        <label for="first-name">
                            First Name <span class="required">*</span>
                        </label>
                        <input type="text" 
                               id="first-name" 
                               name="first-name" 
                               required
                               aria-required="true"
                               minlength="2">
                    </div>
                    
                    <div>
                        <label for="last-name">
                            Last Name <span class="required">*</span>
                        </label>
                        <input type="text" 
                               id="last-name" 
                               name="last-name" 
                               required
                               aria-required="true"
                               minlength="2">
                    </div>
                </div>
                
                <label for="email">
                    Email Address <span class="required">*</span>
                </label>
                <input type="email" 
                       id="email" 
                       name="email" 
                       required
                       aria-required="true"
                       aria-describedby="email-help">
                <div id="email-help" class="help-text">
                    We'll use this for conference updates and your digital badge
                </div>
                
                <div class="form-row">
                    <div>
                        <label for="phone">Phone Number</label>
                        <input type="tel" 
                               id="phone" 
                               name="phone" 
                               pattern="[0-9]{3}-[0-9]{3}-[0-9]{4}"
                               placeholder="123-456-7890">
                    </div>
                    
                    <div>
                        <label for="birth-date">Date of Birth</label>
                        <input type="date" 
                               id="birth-date" 
                               name="birth-date" 
                               min="1940-01-01" 
                               max="2010-12-31">
                    </div>
                </div>
                
                <label for="company">Company/Organization</label>
                <input type="text" 
                       id="company" 
                       name="company" 
                       aria-describedby="company-help">
                <div id="company-help" class="help-text">
                    This will appear on your conference badge
                </div>
                
                <label for="job-title">Job Title</label>
                <input type="text" 
                       id="job-title" 
                       name="job-title">
                
                <label for="website">Personal/Company Website</label>
                <input type="url" 
                       id="website" 
                       name="website" 
                       placeholder="https://example.com">
            </fieldset>
            
            <!-- Conference Preferences -->
            <fieldset>
                <legend>Conference Preferences</legend>
                
                <fieldset>
                    <legend>Registration Type <span class="required">*</span></legend>
                    
                    <input type="radio" 
                           id="reg-early" 
                           name="registration-type" 
                           value="early-bird" 
                           required>
                    <label for="reg-early">
                        Early Bird - $299 (Save $100!)
                    </label>
                    
                    <input type="radio" 
                           id="reg-standard" 
                           name="registration-type" 
                           value="standard">
                    <label for="reg-standard">
                        Standard - $399
                    </label>
                    
                    <input type="radio" 
                           id="reg-student" 
                           name="registration-type" 
                           value="student">
                    <label for="reg-student">
                        Student - $199 (ID required at check-in)
                    </label>
                    
                    <input type="radio" 
                           id="reg-vip" 
                           name="registration-type" 
                           value="vip">
                    <label for="reg-vip">
                        VIP - $599 (Includes all workshops and networking dinner)
                    </label>
                </fieldset>
                
                <fieldset>
                    <legend>Interested Tracks (Select all that apply):</legend>
                    
                    <input type="checkbox" 
                           id="track-web" 
                           name="tracks" 
                           value="web-development">
                    <label for="track-web">Web Development & Frontend</label>
                    
                    <input type="checkbox" 
                           id="track-mobile" 
                           name="tracks" 
                           value="mobile">
                    <label for="track-mobile">Mobile App Development</label>
                    
                    <input type="checkbox" 
                           id="track-ai" 
                           name="tracks" 
                           value="artificial-intelligence">
                    <label for="track-ai">Artificial Intelligence & Machine Learning</label>
                    
                    <input type="checkbox" 
                           id="track-cloud" 
                           name="tracks" 
                           value="cloud">
                    <label for="track-cloud">Cloud Computing & DevOps</label>
                    
                    <input type="checkbox" 
                           id="track-security" 
                           name="tracks" 
                           value="cybersecurity">
                    <label for="track-security">Cybersecurity</label>
                    
                    <input type="checkbox" 
                           id="track-blockchain" 
                           name="tracks" 
                           value="blockchain">
                    <label for="track-blockchain">Blockchain & Cryptocurrency</label>
                </fieldset>
                
                <label for="experience-level">Experience Level</label>
                <select id="experience-level" name="experience-level" required>
                    <option value="">Select your experience level</option>
                    <option value="beginner">Beginner (0-2 years)</option>
                    <option value="intermediate">Intermediate (3-5 years)</option>
                    <option value="advanced">Advanced (6-10 years)</option>
                    <option value="expert">Expert (10+ years)</option>
                </select>
                
                <label for="programming-languages">
                    Primary Programming Languages
                </label>
                <select id="programming-languages" 
                        name="programming-languages" 
                        multiple 
                        size="6"
                        aria-describedby="lang-help">
                    <optgroup label="Frontend">
                        <option value="javascript">JavaScript</option>
                        <option value="typescript">TypeScript</option>
                        <option value="html-css">HTML/CSS</option>
                    </optgroup>
                    <optgroup label="Backend">
                        <option value="python">Python</option>
                        <option value="java">Java</option>
                        <option value="csharp">C#</option>
                        <option value="php">PHP</option>
                        <option value="ruby">Ruby</option>
                        <option value="go">Go</option>
                        <option value="rust">Rust</option>
                    </optgroup>
                    <optgroup label="Mobile">
                        <option value="swift">Swift</option>
                        <option value="kotlin">Kotlin</option>
                        <option value="flutter">Flutter/Dart</option>
                        <option value="react-native">React Native</option>
                    </optgroup>
                </select>
                <div id="lang-help" class="help-text">
                    Hold Ctrl (Cmd on Mac) to select multiple languages
                </div>
            </fieldset>
            
            <!-- Dietary & Accessibility -->
            <fieldset>
                <legend>Dietary Restrictions & Accessibility</legend>
                
                <fieldset>
                    <legend>Dietary Restrictions:</legend>
                    
                    <input type="checkbox" 
                           id="diet-vegetarian" 
                           name="dietary" 
                           value="vegetarian">
                    <label for="diet-vegetarian">Vegetarian</label>
                    
                    <input type="checkbox" 
                           id="diet-vegan" 
                           name="dietary" 
                           value="vegan">
                    <label for="diet-vegan">Vegan</label>
                    
                    <input type="checkbox" 
                           id="diet-gluten" 
                           name="dietary" 
                           value="gluten-free">
                    <label for="diet-gluten">Gluten-Free</label>
                    
                    <input type="checkbox" 
                           id="diet-kosher" 
                           name="dietary" 
                           value="kosher">
                    <label for="diet-kosher">Kosher</label>
                    
                    <input type="checkbox" 
                           id="diet-halal" 
                           name="dietary" 
                           value="halal">
                    <label for="diet-halal">Halal</label>
                    
                    <input type="checkbox" 
                           id="diet-other" 
                           name="dietary" 
                           value="other">
                    <label for="diet-other">Other (please specify below)</label>
                </fieldset>
                
                <label for="dietary-notes">
                    Additional Dietary Requirements
                </label>
                <textarea id="dietary-notes" 
                          name="dietary-notes" 
                          rows="3" 
                          placeholder="Please describe any specific dietary needs or allergies..."></textarea>
                
                <label for="accessibility">
                    Accessibility Requirements
                </label>
                <textarea id="accessibility" 
                          name="accessibility" 
                          rows="3" 
                          placeholder="Please describe any accessibility accommodations you need..."></textarea>
            </fieldset>
            
            <!-- Workshop Registration -->
            <fieldset>
                <legend>Optional Workshop Add-ons</legend>
                <p>Pre-conference workshops (additional cost applies):</p>
                
                <input type="checkbox" 
                       id="workshop-react" 
                       name="workshops" 
                       value="react-masterclass">
                <label for="workshop-react">
                    React Masterclass - $149 (Sunday, 9AM-5PM)
                </label>
                
                <input type="checkbox" 
                       id="workshop-python" 
                       name="workshops" 
                       value="python-ai">
                <label for="workshop-python">
                    Python for AI/ML - $179 (Sunday, 9AM-5PM)
                </label>
                
                <input type="checkbox" 
                       id="workshop-cloud" 
                       name="workshops" 
                       value="aws-certification">
                <label for="workshop-cloud">
                    AWS Certification Prep - $199 (Saturday, 1PM-6PM)
                </label>
                
                <input type="checkbox" 
                       id="workshop-security" 
                       name="workshops" 
                       value="ethical-hacking">
                <label for="workshop-security">
                    Ethical Hacking Basics - $159 (Saturday, 9AM-1PM)
                </label>
            </fieldset>
            
            <!-- Additional Information -->
            <fieldset>
                <legend>Additional Information</legend>
                
                <label for="referral">How did you hear about TechConf 2025?</label>
                <select id="referral" name="referral">
                    <option value="">Please select</option>
                    <option value="social-media">Social Media</option>
                    <option value="colleague">Colleague/Friend</option>
                    <option value="previous-attendee">Previous Attendee</option>
                    <option value="company">Company Announcement</option>
                    <option value="tech-blog">Tech Blog/Website</option>
                    <option value="email">Email Newsletter</option>
                    <option value="search-engine">Search Engine</option>
                    <option value="other">Other</option>
                </select>
                
                <label for="comments">
                    Questions or Comments
                </label>
                <textarea id="comments" 
                          name="comments" 
                          rows="4" 
                          maxlength="1000"
                          placeholder="Any questions about the conference or special requests..."
                          oninput="commentCounter.textContent = this.value.length + '/1000'"></textarea>
                <div id="commentCounter" class="help-text">0/1000</div>
                
                <label for="t-shirt-size">Conference T-Shirt Size</label>
                <select id="t-shirt-size" name="t-shirt-size">
                    <option value="">Select size</option>
                    <option value="xs">Extra Small</option>
                    <option value="s">Small</option>
                    <option value="m">Medium</option>
                    <option value="l">Large</option>
                    <option value="xl">Extra Large</option>
                    <option value="xxl">XXL</option>
                    <option value="xxxl">XXXL</option>
                </select>
                
                <label for="emergency-contact">Emergency Contact Name</label>
                <input type="text" 
                       id="emergency-contact" 
                       name="emergency-contact">
                
                <label for="emergency-phone">Emergency Contact Phone</label>
                <input type="tel" 
                       id="emergency-phone" 
                       name="emergency-phone" 
                       pattern="[0-9]{3}-[0-9]{3}-[0-9]{4}"
                       placeholder="123-456-7890">
            </fieldset>
            
            <!-- Agreement and Submission -->
            <fieldset>
                <legend>Terms and Conditions</legend>
                
                <input type="checkbox" 
                       id="terms" 
                       name="terms" 
                       value="agreed" 
                       required
                       aria-required="true">
                <label for="terms">
                    I agree to the 
                    <a href="/terms" target="_blank">Terms and Conditions</a> 
                    and 
                    <a href="/privacy" target="_blank">Privacy Policy</a> 
                    <span class="required">*</span>
                </label>
                
                <input type="checkbox" 
                       id="marketing" 
                       name="marketing" 
                       value="yes">
                <label for="marketing">
                    I'd like to receive updates about future TechConf events and related tech news
                </label>
                
                <input type="checkbox" 
                       id="photo-consent" 
                       name="photo-consent" 
                       value="yes">
                <label for="photo-consent">
                    I consent to being photographed/filmed during the conference for promotional purposes
                </label>
            </fieldset>
            
            <!-- Hidden fields for tracking -->
            <input type="hidden" name="form-version" value="2025.1">
            <input type="hidden" name="registration-date" id="reg-date">
            
            <div class="form-actions">
                <button type="submit">Complete Registration</button>
                <button type="reset">Clear Form</button>
            </div>
        </form>
    </main>
    
    <footer>
        <p>Questions? Contact us at <a href="mailto:support@techconf2025.com">support@techconf2025.com</a> 
           or call <a href="tel:+15551234567">(555) 123-4567</a></p>
    </footer>
    
    <script>
        // Set registration date
        document.getElementById('reg-date').value = new Date().toISOString();
        
        // Form validation enhancements
        const form = document.querySelector('form');
        const inputs = form.querySelectorAll('input, select, textarea');
        
        // Custom validation messages
        inputs.forEach(input => {
            input.addEventListener('invalid', function(e) {
                e.preventDefault();
                
                if (this.validity.valueMissing) {
                    if (this.type === 'email') {
                        this.setCustomValidity('Please enter your email address');
                    } else if (this.type === 'radio') {
                        this.setCustomValidity('Please select an option');
                    } else {
                        this.setCustomValidity('This field is required');
                    }
                } else if (this.validity.typeMismatch) {
                    if (this.type === 'email') {
                        this.setCustomValidity('Please enter a valid email address');
                    } else if (this.type === 'url') {
                        this.setCustomValidity('Please enter a valid URL (http://example.com)');
                    }
                } else if (this.validity.patternMismatch) {
                    if (this.type === 'tel') {
                        this.setCustomValidity('Please enter phone number in format: 123-456-7890');
                    }
                } else if (this.validity.tooShort) {
                    this.setCustomValidity(`Please enter at least ${this.minLength} characters`);
                } else {
                    this.setCustomValidity('');
                }
            });
            
            input.addEventListener('input', function() {
                this.setCustomValidity('');
            });
        });
        
        // Form submission handling
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            
            // Validate required radio buttons
            const registrationType = document.querySelector('input[name="registration-type"]:checked');
            if (!registrationType) {
                alert('Please select a registration type');
                document.getElementById('reg-early').focus();
                return;
            }
            
            // Show confirmation (in real app, this would submit to server)
            alert('Registration form completed! In a real application, this would be submitted to the server.');
        });
    </script>
</body>
</html>
```

## 📋 Form Best Practices

### 1. Accessibility Guidelines
```html
<!-- Always associate labels with inputs -->
<label for="username">Username:</label>
<input type="text" id="username" name="username">

<!-- Use fieldset for radio button groups -->
<fieldset>
    <legend>Preferred contact method:</legend>
    <input type="radio" id="email" name="contact" value="email">
    <label for="email">Email</label>
    <!-- more radio buttons -->
</fieldset>

<!-- Provide help text and error messages -->
<label for="password">Password:</label>
<input type="password" 
       id="password" 
       name="password" 
       aria-describedby="pwd-help pwd-error"
       aria-invalid="false">
<div id="pwd-help">Must be at least 8 characters</div>
<div id="pwd-error" role="alert" style="display: none;">
    Password is too short
</div>
```

### 2. User Experience Tips
```html
<!-- Use appropriate input types for mobile keyboards -->
<input type="email" placeholder="user@example.com">
<input type="tel" placeholder="(555) 123-4567">
<input type="number" placeholder="Enter age">

<!-- Provide clear placeholders and help text -->
<input type="text" 
       placeholder="First name" 
       aria-describedby="name-help">
<div id="name-help">As it appears on your ID</div>

<!-- Group related information -->
<fieldset>
    <legend>Billing Address</legend>
    <!-- address fields -->
</fieldset>
```

## 🎉 Day 5 Checklist

- [ ] Created comprehensive registration form with all input types
- [ ] Implemented proper form structure with fieldsets and legends
- [ ] Added form validation with custom error messages
- [ ] Applied accessibility best practices with ARIA attributes
- [ ] Used appropriate input types for different data
- [ ] Included help text and user guidance
- [ ] Created responsive form layout
- [ ] Added JavaScript form enhancements

## 🔗 Additional Resources

- **[MDN Forms Guide](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/form)** - Complete form documentation
- **[Web Form Accessibility](https://webaim.org/techniques/forms/)** - Accessibility guidelines
- **[HTML5 Input Types](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input)** - All input type options
- **[Form Validation Guide](https://web.dev/learn/forms/validation/)** - Client and server validation

---

## 🚀 What's Next?

Tomorrow in **Day 06: Tables & Data Display**, you'll learn:
- Creating accessible and semantic tables
- Table headers, captions, and structure
- Responsive table design patterns
- Data presentation best practices

**Excellent work on Day 5!** You've mastered form creation and can now build user-friendly, accessible forms that collect data effectively.

---

**Remember: Good forms make users happy and improve conversion rates!** 📝✨