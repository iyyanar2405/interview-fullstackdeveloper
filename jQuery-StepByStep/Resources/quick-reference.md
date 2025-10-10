# jQuery Quick Reference Guide

## Basic Syntax
```javascript
$(document).ready(function() {
    // Your code here
});

// Shorthand
$(function() {
    // Your code here
});
```

## Selectors Quick Reference

| Selector Type | Syntax | Example | Description |
|---------------|--------|---------|-------------|
| Universal | `*` | `$('*')` | All elements |
| Element | `element` | `$('p')` | All `<p>` elements |
| ID | `#id` | `$('#myId')` | Element with id="myId" |
| Class | `.class` | `$('.myClass')` | Elements with class="myClass" |
| Multiple | `sel1, sel2` | `$('h1, h2')` | Multiple selectors |
| Descendant | `A B` | `$('div p')` | `<p>` inside `<div>` |
| Child | `A > B` | `$('div > p')` | Direct `<p>` children of `<div>` |
| Adjacent | `A + B` | `$('h1 + p')` | `<p>` immediately after `<h1>` |
| Attribute | `[attr]` | `$('[required]')` | Elements with attribute |
| Attr Value | `[attr=val]` | `$('[type="text"]')` | Attribute equals value |
| First | `:first` | `$('li:first')` | First matched element |
| Last | `:last` | `$('li:last')` | Last matched element |
| Even | `:even` | `$('tr:even')` | Even-indexed elements |
| Odd | `:odd` | `$('tr:odd')` | Odd-indexed elements |
| Nth Child | `:nth-child(n)` | `$('li:nth-child(2)')` | Nth child element |
| Contains | `:contains(text)` | `$(':contains("Hello")')` | Elements containing text |
| Visible | `:visible` | `$('div:visible')` | Visible elements |
| Hidden | `:hidden` | `$('div:hidden')` | Hidden elements |

## DOM Manipulation

### Content Methods
```javascript
// Get/Set text content
$('#element').text()
$('#element').text('New content')

// Get/Set HTML content
$('#element').html()
$('#element').html('<b>Bold text</b>')

// Get/Set form values
$('#input').val()
$('#input').val('New value')
```

### Attribute Methods
```javascript
// Get/Set attributes
$('#element').attr('src')
$('#element').attr('src', 'image.jpg')
$('#element').attr({
    'src': 'image.jpg',
    'alt': 'Description'
})

// Remove attribute
$('#element').removeAttr('disabled')

// Properties (for form elements)
$('#checkbox').prop('checked')
$('#checkbox').prop('checked', true)

// Data attributes
$('#element').data('user-id')
$('#element').data('user-id', 123)
```

### CSS Methods
```javascript
// Get/Set CSS properties
$('#element').css('color')
$('#element').css('color', 'red')
$('#element').css({
    'color': 'red',
    'font-size': '16px'
})

// Classes
$('#element').addClass('highlight')
$('#element').removeClass('highlight')
$('#element').toggleClass('highlight')
$('#element').hasClass('highlight')

// Dimensions
$('#element').width()
$('#element').height()
$('#element').outerWidth()
$('#element').outerHeight()
```

### Element Creation and Insertion
```javascript
// Create elements
var $newElement = $('<div>Content</div>')
var $img = $('<img>', {
    src: 'image.jpg',
    alt: 'Description'
})

// Insert content
$('#container').append('<p>At end</p>')
$('#container').prepend('<p>At beginning</p>')
$('#element').before('<p>Before element</p>')
$('#element').after('<p>After element</p>')

// Wrap elements
$('#element').wrap('<div class="wrapper">')
$('#element').wrapInner('<span>')

// Remove elements
$('#element').remove()
$('#element').empty()

// Replace elements
$('#element').replaceWith('<div>New content</div>')
```

## Event Handling

### Basic Events
```javascript
// Click event
$('#button').click(function() {
    // Handle click
})

// Multiple events
$('#element').on('mouseenter mouseleave', function() {
    // Handle multiple events
})

// Event delegation (for dynamic content)
$(document).on('click', '.dynamic-button', function() {
    // Handle click on dynamically added buttons
})

// One-time event
$('#button').one('click', function() {
    // Fires only once
})

// Remove events
$('#element').off('click')
$('#element').off() // Remove all events
```

### Common Events
| Event | Description |
|-------|-------------|
| `click` | Mouse click |
| `dblclick` | Double click |
| `mouseenter` | Mouse enters element |
| `mouseleave` | Mouse leaves element |
| `mouseover` | Mouse over element |
| `mouseout` | Mouse moves out |
| `keydown` | Key pressed down |
| `keyup` | Key released |
| `keypress` | Key pressed |
| `focus` | Element gains focus |
| `blur` | Element loses focus |
| `change` | Form element changes |
| `submit` | Form submission |
| `resize` | Window resized |
| `scroll` | Page scrolled |

### Event Object Properties
```javascript
$('#element').on('click', function(e) {
    e.type          // Event type
    e.target        // Element that triggered event
    e.currentTarget // Element with event handler
    e.pageX         // Mouse X position
    e.pageY         // Mouse Y position
    e.which         // Key or button pressed
    e.preventDefault()    // Prevent default action
    e.stopPropagation()  // Stop event bubbling
})
```

## Effects and Animations

### Basic Effects
```javascript
// Show/Hide
$('#element').show()
$('#element').hide()
$('#element').toggle()

// Fade effects
$('#element').fadeIn()
$('#element').fadeOut()
$('#element').fadeToggle()
$('#element').fadeTo(500, 0.5) // To specific opacity

// Slide effects
$('#element').slideDown()
$('#element').slideUp()
$('#element').slideToggle()
```

### Custom Animations
```javascript
// Basic animation
$('#element').animate({
    left: '250px',
    opacity: 0.5,
    height: 'toggle'
}, 1000)

// With callback
$('#element').animate({
    width: '70%'
}, {
    duration: 1200,
    complete: function() {
        alert('Animation complete!')
    }
})

// Stop animations
$('#element').stop()
$('#element').stop(true, true) // Clear queue and jump to end
```

## AJAX Methods

### Basic AJAX
```javascript
// GET request
$.get('url', function(data) {
    console.log(data)
})

// POST request
$.post('url', {name: 'John'}, function(data) {
    console.log(data)
})

// JSON request
$.getJSON('url', function(data) {
    console.log(data)
})

// Full AJAX
$.ajax({
    url: 'api/endpoint',
    method: 'POST',
    data: {key: 'value'},
    dataType: 'json',
    success: function(data) {
        console.log('Success:', data)
    },
    error: function(xhr, status, error) {
        console.log('Error:', error)
    }
})
```

## Utility Methods

### Each Method
```javascript
// Iterate over jQuery object
$('.items').each(function(index, element) {
    console.log(index + ': ' + $(element).text())
})

// Iterate over array/object
$.each(array, function(index, value) {
    console.log(index + ': ' + value)
})
```

### Filtering and Traversal
```javascript
// Filtering
$('.items').filter('.active')
$('.items').not('.inactive')
$('.items').eq(2) // Element at index 2
$('.items').first()
$('.items').last()

// Traversal
$('#element').parent()
$('#element').parents()
$('#element').children()
$('#element').siblings()
$('#element').next()
$('#element').prev()
$('#element').find('.child')
$('#element').closest('.parent')
```

## Common Patterns

### Toggle Content
```javascript
$('#toggle-btn').click(function() {
    $('#content').slideToggle()
    $(this).text(function(i, text) {
        return text === 'Show' ? 'Hide' : 'Show'
    })
})
```

### Tab System
```javascript
$('.tab-btn').click(function() {
    var target = $(this).data('target')
    
    $('.tab-btn').removeClass('active')
    $(this).addClass('active')
    
    $('.tab-content').hide()
    $('#' + target).show()
})
```

### Modal Dialog
```javascript
// Open modal
$('[data-modal]').click(function() {
    var modal = $(this).data('modal')
    $('#' + modal).fadeIn()
})

// Close modal
$('.modal-close, .modal-overlay').click(function() {
    $('.modal').fadeOut()
})
```

### Form Validation
```javascript
$('#form').submit(function(e) {
    e.preventDefault()
    
    var isValid = true
    
    $(this).find('[required]').each(function() {
        if ($(this).val().trim() === '') {
            $(this).addClass('error')
            isValid = false
        } else {
            $(this).removeClass('error')
        }
    })
    
    if (isValid) {
        // Submit form
    }
})
```

## Performance Tips

1. **Cache selectors**: `var $element = $('#element')`
2. **Use ID selectors** when possible (fastest)
3. **Chain methods**: `$('#element').addClass('class').show()`
4. **Use event delegation** for dynamic content
5. **Minimize DOM queries** in loops
6. **Use specific selectors** instead of universal ones

## Best Practices

1. Always wrap code in `$(document).ready()`
2. Use meaningful variable names with `$` prefix for jQuery objects
3. Use event delegation for dynamically added elements
4. Cache frequently used selectors
5. Use method chaining when appropriate
6. Prefer `on()` over deprecated event methods
7. Always handle errors in AJAX requests
8. Use semantic HTML and progressive enhancement

## Common Mistakes to Avoid

1. Not waiting for document ready
2. Repeated DOM queries instead of caching
3. Not using event delegation for dynamic content
4. Forgetting to prevent default behavior
5. Using deprecated methods like `live()` or `bind()`
6. Not handling AJAX errors
7. Creating memory leaks by not removing event handlers