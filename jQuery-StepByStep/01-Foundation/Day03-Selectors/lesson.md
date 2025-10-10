# Day 3: jQuery Selectors Mastery

## Understanding jQuery Selectors

Selectors are the heart of jQuery. They allow you to find and select HTML elements in the DOM. jQuery uses CSS selector syntax, making it familiar and powerful.

## Basic Selectors

### Universal Selector
```javascript
$('*')  // Selects all elements
```

### Element Selectors
```javascript
$('p')      // All <p> elements
$('div')    // All <div> elements
$('h1, h2') // All <h1> AND <h2> elements
```

### ID Selector (Most Specific)
```javascript
$('#myId')     // Element with id="myId"
$('#header')   // Element with id="header"
```

### Class Selector
```javascript
$('.myClass')      // All elements with class="myClass"
$('.btn')          // All elements with class="btn"
$('.btn.primary')  // Elements with BOTH classes "btn" AND "primary"
```

## Hierarchical Selectors

### Descendant Selector (Space)
```javascript
$('div p')        // All <p> elements inside <div> elements
$('#main .content') // Elements with class="content" inside id="main"
```

### Child Selector (>)
```javascript
$('div > p')      // Direct <p> children of <div> elements
$('#nav > li')    // Direct <li> children of element with id="nav"
```

### Adjacent Sibling Selector (+)
```javascript
$('h1 + p')       // <p> elements immediately following <h1>
$('.title + .content') // Elements with class="content" immediately after class="title"
```

### General Sibling Selector (~)
```javascript
$('h1 ~ p')       // All <p> elements that are siblings after <h1>
```

## Attribute Selectors

### Basic Attribute Selectors
```javascript
$('[name]')           // Elements with name attribute
$('[name="email"]')   // Elements with name="email"
$('[name!="email"]')  // Elements without name="email"
```

### Attribute Value Patterns
```javascript
$('[class^="btn"]')   // class starts with "btn"
$('[class$="large"]') // class ends with "large"
$('[class*="nav"]')   // class contains "nav"
$('[href~="jquery"]') // href contains word "jquery"
```

### Multiple Attributes
```javascript
$('input[type="text"][name="username"]') // Input with both attributes
```

## Pseudo-class Selectors

### Position-based
```javascript
$('li:first')         // First <li> element
$('li:last')          // Last <li> element
$('li:first-child')   // First child <li> of its parent
$('li:last-child')    // Last child <li> of its parent
$('li:nth-child(2)')  // Second child <li>
$('li:nth-child(odd)') // Odd-positioned <li> elements
$('li:nth-child(even)') // Even-positioned <li> elements
```

### Content-based
```javascript
$(':empty')           // Empty elements
$(':not(.class)')     // Elements without specified class
$(':contains("text")') // Elements containing specific text
$(':has("p")')        // Elements that contain <p> elements
```

### Form-specific
```javascript
$(':input')           // All form input elements
$(':text')            // Text input elements
$(':password')        // Password input elements
$(':radio')           // Radio button inputs
$(':checkbox')        // Checkbox inputs
$(':selected')        // Selected option elements
$(':checked')         // Checked radio/checkbox elements
$(':disabled')        // Disabled form elements
$(':enabled')         // Enabled form elements
```

### Visibility
```javascript
$(':visible')         // Visible elements
$(':hidden')          // Hidden elements
```

## jQuery-specific Selectors

### Index-based
```javascript
$('li:eq(2)')         // Third <li> element (0-based index)
$('li:gt(2)')         // <li> elements after index 2
$('li:lt(2)')         // <li> elements before index 2
```

### Header Elements
```javascript
$(':header')          // All header elements (h1, h2, h3, etc.)
```

## Advanced Selector Techniques

### Selector Variables
```javascript
var selector = '.dynamic-class';
$(selector)           // Use variable as selector
```

### Context Selectors
```javascript
$('p', '#content')    // Find <p> within #content
$('.item', this)      // Find .item within current element context
```

### Multiple Selectors
```javascript
$('h1, h2, h3')       // Multiple element types
$('.class1, .class2') // Multiple classes
$('#id1, #id2')       // Multiple IDs
```

## Selector Performance Tips

### Performance Ranking (Fastest to Slowest)
1. **ID selectors**: `$('#myId')` - Fastest
2. **Element selectors**: `$('div')` - Fast
3. **Class selectors**: `$('.myClass')` - Good
4. **Attribute selectors**: `$('[name="value"]')` - Slower
5. **Pseudo-class selectors**: `$(':visible')` - Slowest

### Best Practices
```javascript
// Good - Start with ID or element, then filter
$('#container').find('.item')

// Avoid - Complex selectors
$('div.container > ul li:nth-child(odd) a[href^="http"]')

// Cache selectors
var $items = $('.item');
$items.addClass('active');
$items.fadeIn();
```

## Selector Methods

### Testing Selectors
```javascript
$('#myDiv').length        // Number of matched elements
$('#myDiv').is(':visible') // Test if element matches condition
```

### Filtering
```javascript
$('li').first()           // First matched element
$('li').last()            // Last matched element
$('li').eq(2)             // Element at index 2
$('li').filter('.active') // Filter by selector
$('li').not('.inactive')  // Exclude elements
```

### Traversal
```javascript
$('#item').parent()       // Parent element
$('#item').children()     // Direct children
$('#item').siblings()     // Sibling elements
$('#item').next()         // Next sibling
$('#item').prev()         // Previous sibling
```

## Common Selector Patterns

### Form Elements
```javascript
// All required fields
$('input[required]')

// All empty text inputs
$('input[type="text"]').filter(function() {
    return $(this).val() === '';
});

// All checked checkboxes
$('input:checkbox:checked')
```

### Navigation Menus
```javascript
// All navigation links
$('#nav a')

// Active menu item
$('#nav .active')

// Dropdown menus
$('#nav .dropdown')
```

### Content Areas
```javascript
// All paragraphs with text
$('p:not(:empty)')

// External links
$('a[href^="http"]:not([href*="' + location.hostname + '"])')

// Images without alt text
$('img:not([alt])')
```

## Debugging Selectors

### Console Testing
```javascript
// Test selectors in browser console
console.log($('.my-selector').length);
console.log($('.my-selector'));

// Highlight selected elements
$('.my-selector').css('background-color', 'yellow');
```

### Selector Validation
```javascript
function testSelector(selector) {
    var $elements = $(selector);
    console.log('Selector "' + selector + '" found ' + $elements.length + ' elements');
    return $elements;
}

testSelector('.my-class');
```

## Practice Exercises

### Exercise 1: Basic Selection
Create HTML with various elements and practice selecting them using different selector types.

### Exercise 2: Form Selectors
Create a form and use selectors to find specific input types, required fields, and validation states.

### Exercise 3: Navigation Menu
Build a navigation menu and use hierarchical selectors to style different levels.

### Exercise 4: Dynamic Content
Create a list and use pseudo-class selectors to style odd/even items differently.

## Common Mistakes

1. **Forgetting quotes in attribute selectors**:
```javascript
// Wrong
$([name=email])

// Correct
$('[name="email"]')
```

2. **Confusing child vs descendant selectors**:
```javascript
$('div p')    // All p inside div (any level)
$('div > p')  // Only direct p children of div
```

3. **Performance issues with complex selectors**:
```javascript
// Slow
$('div.container ul li a.link')

// Better
$('.container').find('a.link')
```

## Summary

jQuery selectors provide powerful and flexible ways to select DOM elements:

- Use CSS selector syntax you already know
- Start specific (ID) and get more general (class, element)
- Understand the difference between child and descendant selectors
- Use pseudo-class selectors for dynamic selection
- Cache selectors for better performance
- Test selectors in the browser console

Tomorrow we'll learn about DOM manipulation - how to modify, add, and remove elements once we've selected them.