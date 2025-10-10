# Day 2: Setup and Basic Syntax

## Setting Up jQuery

### Method 1: CDN (Content Delivery Network) - Recommended for Learning

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>jQuery Setup</title>
</head>
<body>
    <h1>Hello jQuery!</h1>
    
    <!-- jQuery CDN -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script>
        // Your jQuery code here
        $(document).ready(function() {
            console.log("jQuery is ready!");
        });
    </script>
</body>
</html>
```

### Method 2: Download and Host Locally

1. Download jQuery from [jquery.com](https://jquery.com/download/)
2. Save it in your project folder
3. Include it in your HTML:

```html
<script src="js/jquery-3.7.1.min.js"></script>
```

### Method 3: Package Managers

```bash
# npm
npm install jquery

# yarn
yarn add jquery
```

## Basic jQuery Syntax

### The jQuery Function

jQuery uses the `$` symbol as a shortcut for the `jQuery` function:

```javascript
// These are equivalent
jQuery(document).ready(function() { });
$(document).ready(function() { });
```

### Document Ready

Always wrap your jQuery code in the document ready function:

```javascript
$(document).ready(function() {
    // Your jQuery code here
});

// Shorthand version
$(function() {
    // Your jQuery code here
});
```

### Basic Syntax Structure

```javascript
$(selector).action()
```

- `$`: jQuery function
- `selector`: CSS-style selector to find HTML elements
- `action()`: Method to perform on the selected elements

## Common Selectors

### Element Selectors
```javascript
$('p')        // All <p> elements
$('div')      // All <div> elements
$('h1')       // All <h1> elements
```

### ID Selectors
```javascript
$('#myId')    // Element with id="myId"
```

### Class Selectors
```javascript
$('.myClass') // All elements with class="myClass"
```

### Attribute Selectors
```javascript
$('[name="email"]')     // Elements with name="email"
$('input[type="text"]') // Text input elements
```

## Basic Methods

### Text and HTML
```javascript
$('#myDiv').text('Hello World');           // Set text content
$('#myDiv').html('<b>Hello World</b>');    // Set HTML content
var content = $('#myDiv').text();          // Get text content
```

### Show and Hide
```javascript
$('#myDiv').hide();    // Hide element
$('#myDiv').show();    // Show element
$('#myDiv').toggle();  // Toggle visibility
```

### CSS Manipulation
```javascript
$('#myDiv').css('color', 'red');              // Set single property
$('#myDiv').css({                             // Set multiple properties
    'color': 'red',
    'font-size': '16px',
    'background-color': 'yellow'
});
```

## Method Chaining

jQuery allows you to chain multiple methods together:

```javascript
$('#myDiv')
    .hide()
    .css('color', 'red')
    .show()
    .fadeOut();
```

## The `this` Keyword

In jQuery event handlers, `this` refers to the DOM element:

```javascript
$('.button').click(function() {
    $(this).hide(); // Hide the clicked button
});
```

## jQuery vs $ Conflict Resolution

If another library uses `$`, you can use `noConflict()`:

```javascript
var jq = jQuery.noConflict();
jq(document).ready(function() {
    jq('#myDiv').hide();
});
```

## Best Practices

1. **Always use document ready**:
```javascript
$(document).ready(function() {
    // Your code here
});
```

2. **Cache jQuery objects**:
```javascript
// Good
var $myDiv = $('#myDiv');
$myDiv.hide();
$myDiv.css('color', 'red');

// Avoid
$('#myDiv').hide();
$('#myDiv').css('color', 'red');
```

3. **Use meaningful variable names**:
```javascript
var $navigationMenu = $('#nav-menu');
var $submitButton = $('#submit-btn');
```

4. **Prefix jQuery variables with $**:
```javascript
var $element = $('#myElement'); // jQuery object
var element = document.getElementById('myElement'); // DOM element
```

## Common Mistakes to Avoid

1. **Not waiting for document ready**:
```javascript
// Wrong - might run before DOM is loaded
$('#myButton').click(function() { });

// Correct
$(document).ready(function() {
    $('#myButton').click(function() { });
});
```

2. **Repeated selections**:
```javascript
// Inefficient
$('#myDiv').hide();
$('#myDiv').css('color', 'red');
$('#myDiv').addClass('highlight');

// Better
$('#myDiv').hide().css('color', 'red').addClass('highlight');
```

## Practice Exercises

Create an HTML file with the following exercises:

### Exercise 1: Basic Setup
Create a simple HTML page that includes jQuery and displays "jQuery is working!" in the console.

### Exercise 2: Text Manipulation
Create a button that changes the text of a paragraph when clicked.

### Exercise 3: CSS Manipulation
Create buttons that change the color, font size, and background color of a div.

### Exercise 4: Method Chaining
Create a single button that hides a div, changes its text, and shows it again using method chaining.

## Summary

Today you learned how to set up jQuery in your projects and the basic syntax structure. Key points:

- jQuery uses the `$` function and CSS-style selectors
- Always wrap code in `$(document).ready()`
- jQuery supports method chaining
- Cache jQuery objects for better performance
- Use meaningful variable names and prefix jQuery objects with `$`

Tomorrow we'll dive deeper into jQuery selectors and learn how to target specific elements effectively.