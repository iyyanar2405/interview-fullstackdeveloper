# Day 5: Event Handling with jQuery

## Understanding Events

Events are actions that can be detected by JavaScript. jQuery provides powerful and simplified methods for handling events, making your web pages interactive and responsive.

## Basic Event Binding

### Common Event Methods
```javascript
// Click event
$('#button').click(function() {
    alert('Button clicked!');
});

// Mouse events
$('#element').mouseover(function() {
    $(this).addClass('highlight');
});

$('#element').mouseout(function() {
    $(this).removeClass('highlight');
});

// Keyboard events
$('#input').keyup(function() {
    console.log('Key pressed: ' + $(this).val());
});

// Form events
$('#form').submit(function(e) {
    e.preventDefault(); // Prevent default form submission
    console.log('Form submitted');
});
```

### The .on() Method (Recommended)
```javascript
// Basic syntax
$('#button').on('click', function() {
    console.log('Button clicked');
});

// Multiple events
$('#element').on('mouseenter mouseleave', function(e) {
    console.log('Event type: ' + e.type);
});

// Multiple events with different handlers
$('#element').on({
    mouseenter: function() {
        $(this).addClass('hover');
    },
    mouseleave: function() {
        $(this).removeClass('hover');
    },
    click: function() {
        alert('Clicked!');
    }
});
```

## Event Delegation

Event delegation allows you to handle events for elements that don't exist yet or are added dynamically.

### Why Event Delegation?
```javascript
// This won't work for dynamically added elements
$('.dynamic-button').click(function() {
    alert('Clicked');
});

// This WILL work for dynamically added elements
$(document).on('click', '.dynamic-button', function() {
    alert('Clicked');
});
```

### Event Delegation Examples
```javascript
// Delegate to parent container
$('#container').on('click', '.item', function() {
    console.log('Item clicked: ' + $(this).text());
});

// Delegate to document (use sparingly)
$(document).on('click', '.global-button', function() {
    console.log('Global button clicked');
});

// Multiple events with delegation
$('#list').on('click mouseover', '.list-item', function(e) {
    console.log('Event: ' + e.type + ' on item: ' + $(this).text());
});
```

## Event Object

The event object contains information about the triggered event.

### Event Object Properties
```javascript
$('#element').on('click', function(event) {
    console.log('Event type: ' + event.type);
    console.log('Target element: ' + event.target.tagName);
    console.log('Current element: ' + event.currentTarget.tagName);
    console.log('Mouse position: ' + event.pageX + ', ' + event.pageY);
    console.log('Timestamp: ' + event.timeStamp);
});
```

### Event Methods
```javascript
$('#link').on('click', function(e) {
    e.preventDefault();        // Prevent default action
    e.stopPropagation();      // Stop event bubbling
    e.stopImmediatePropagation(); // Stop other handlers on same element
    
    console.log('Link clicked but default prevented');
});
```

## Mouse Events

### Basic Mouse Events
```javascript
$('#element').on({
    click: function() { console.log('Clicked'); },
    dblclick: function() { console.log('Double clicked'); },
    mousedown: function() { console.log('Mouse down'); },
    mouseup: function() { console.log('Mouse up'); },
    mouseenter: function() { console.log('Mouse entered'); },
    mouseleave: function() { console.log('Mouse left'); },
    mouseover: function() { console.log('Mouse over'); },
    mouseout: function() { console.log('Mouse out'); },
    mousemove: function(e) { 
        console.log('Mouse at: ' + e.pageX + ', ' + e.pageY); 
    }
});
```

### Mouse Event Examples
```javascript
// Hover effect (mouseenter + mouseleave)
$('#button').hover(
    function() {
        $(this).addClass('hovered');
    },
    function() {
        $(this).removeClass('hovered');
    }
);

// Right-click context menu
$('#element').on('contextmenu', function(e) {
    e.preventDefault();
    $('#contextMenu').css({
        top: e.pageY + 'px',
        left: e.pageX + 'px'
    }).show();
});
```

## Keyboard Events

### Basic Keyboard Events
```javascript
$('#input').on({
    keydown: function(e) {
        console.log('Key down: ' + e.which);
    },
    keyup: function(e) {
        console.log('Key up: ' + e.which);
    },
    keypress: function(e) {
        console.log('Key pressed: ' + String.fromCharCode(e.which));
    }
});
```

### Keyboard Event Examples
```javascript
// Enter key detection
$('#input').on('keypress', function(e) {
    if (e.which === 13) { // Enter key
        console.log('Enter pressed');
        $(this).blur(); // Remove focus
    }
});

// Escape key to close modal
$(document).on('keyup', function(e) {
    if (e.which === 27) { // Escape key
        $('.modal').hide();
    }
});

// Modifier keys
$('#input').on('keydown', function(e) {
    if (e.ctrlKey && e.which === 83) { // Ctrl+S
        e.preventDefault();
        console.log('Save shortcut pressed');
    }
});
```

## Form Events

### Form-Specific Events
```javascript
// Form submission
$('#myForm').on('submit', function(e) {
    e.preventDefault();
    
    // Validate form
    if (validateForm()) {
        // Submit via AJAX
        submitForm();
    }
});

// Input changes
$('#input').on('change', function() {
    console.log('Input changed to: ' + $(this).val());
});

// Input focus and blur
$('#input').on({
    focus: function() {
        $(this).addClass('focused');
    },
    blur: function() {
        $(this).removeClass('focused');
        validateField(this);
    }
});

// Select element changes
$('#dropdown').on('change', function() {
    var selectedValue = $(this).val();
    console.log('Selected: ' + selectedValue);
});
```

### Form Validation Events
```javascript
// Real-time validation
$('#email').on('input', function() {
    var email = $(this).val();
    var isValid = validateEmail(email);
    
    $(this).toggleClass('valid', isValid)
           .toggleClass('invalid', !isValid);
});

// Form field validation on blur
$('.required').on('blur', function() {
    var $field = $(this);
    var value = $field.val().trim();
    
    if (value === '') {
        $field.addClass('error');
        $field.next('.error-message').text('This field is required');
    } else {
        $field.removeClass('error');
        $field.next('.error-message').text('');
    }
});
```

## Custom Events

### Triggering Custom Events
```javascript
// Define custom event handler
$('#element').on('customEvent', function(e, data) {
    console.log('Custom event triggered with data:', data);
});

// Trigger custom event
$('#element').trigger('customEvent', { message: 'Hello World' });

// Trigger with multiple parameters
$('#element').trigger('customEvent', ['param1', 'param2', 'param3']);
```

### Custom Event Examples
```javascript
// Shopping cart example
$('#cart').on('itemAdded', function(e, item) {
    console.log('Item added to cart:', item);
    updateCartDisplay();
});

// Add item to cart
function addToCart(item) {
    // Add item logic here
    $('#cart').trigger('itemAdded', item);
}

// User authentication event
$(document).on('userLoggedIn', function(e, user) {
    console.log('User logged in:', user.name);
    updateUIForLoggedInUser(user);
});
```

## Event Namespacing

Event namespacing helps organize and manage events, especially when removing specific event handlers.

### Basic Namespacing
```javascript
// Add namespaced events
$('#element').on('click.myNamespace', function() {
    console.log('Namespaced click');
});

$('#element').on('mouseover.myNamespace', function() {
    console.log('Namespaced mouseover');
});

// Remove all events in namespace
$('#element').off('.myNamespace');

// Remove specific namespaced event
$('#element').off('click.myNamespace');
```

### Multiple Namespaces
```javascript
// Multiple namespaces
$('#element').on('click.nav.main', function() {
    console.log('Navigation main click');
});

// Remove by specific namespace combination
$('#element').off('.nav.main');

// Remove by single namespace (removes all events with that namespace)
$('#element').off('.nav');
```

## Event Management

### Removing Event Handlers
```javascript
// Remove all event handlers
$('#element').off();

// Remove specific event type
$('#element').off('click');

// Remove specific handler
function myHandler() {
    console.log('My handler');
}
$('#element').on('click', myHandler);
$('#element').off('click', myHandler);
```

### One-Time Events
```javascript
// Event fires only once
$('#button').one('click', function() {
    console.log('This will only fire once');
});

// One-time event with delegation
$(document).one('click', '.one-time-button', function() {
    console.log('One-time delegated event');
});
```

## Practical Examples

### Interactive Navigation Menu
```javascript
$(document).ready(function() {
    // Mobile menu toggle
    $('.menu-toggle').on('click', function() {
        $('nav ul').slideToggle();
    });
    
    // Dropdown menu
    $('.dropdown').on('mouseenter', function() {
        $(this).find('.dropdown-menu').fadeIn(200);
    }).on('mouseleave', function() {
        $(this).find('.dropdown-menu').fadeOut(200);
    });
    
    // Active menu item
    $('nav a').on('click', function() {
        $('nav a').removeClass('active');
        $(this).addClass('active');
    });
});
```

### Modal Dialog System
```javascript
// Modal functions
function openModal(modalId) {
    $('#' + modalId).fadeIn();
    $('body').addClass('modal-open');
}

function closeModal() {
    $('.modal').fadeOut();
    $('body').removeClass('modal-open');
}

// Modal event handlers
$(document).ready(function() {
    // Open modal
    $('[data-modal]').on('click', function() {
        var modalId = $(this).data('modal');
        openModal(modalId);
    });
    
    // Close modal
    $('.modal-close, .modal-overlay').on('click', closeModal);
    
    // Prevent modal content click from closing
    $('.modal-content').on('click', function(e) {
        e.stopPropagation();
    });
    
    // Close on Escape key
    $(document).on('keyup.modal', function(e) {
        if (e.which === 27) {
            closeModal();
        }
    });
});
```

### Form Validation System
```javascript
$(document).ready(function() {
    // Form validation
    $('.validate-form').on('submit', function(e) {
        e.preventDefault();
        
        var isValid = true;
        var $form = $(this);
        
        // Validate required fields
        $form.find('[required]').each(function() {
            var $field = $(this);
            var value = $field.val().trim();
            
            if (value === '') {
                showFieldError($field, 'This field is required');
                isValid = false;
            } else {
                clearFieldError($field);
            }
        });
        
        // Validate email fields
        $form.find('[type="email"]').each(function() {
            var $field = $(this);
            var email = $field.val().trim();
            
            if (email && !isValidEmail(email)) {
                showFieldError($field, 'Please enter a valid email');
                isValid = false;
            }
        });
        
        if (isValid) {
            // Submit form
            submitForm($form);
        }
    });
    
    // Real-time validation
    $('.validate-form input').on('blur', function() {
        validateField($(this));
    });
});

function showFieldError($field, message) {
    $field.addClass('error');
    $field.siblings('.error-message').text(message);
}

function clearFieldError($field) {
    $field.removeClass('error');
    $field.siblings('.error-message').text('');
}
```

## Event Performance Tips

### Efficient Event Handling
```javascript
// Good: Use event delegation for dynamic content
$('#container').on('click', '.item', handler);

// Avoid: Binding events to many elements
$('.item').on('click', handler); // If many .item elements exist

// Good: Cache selectors
var $elements = $('.my-elements');
$elements.on('click', handler);

// Good: Use namespaces for organized event management
$('#element').on('click.myModule', handler);
```

### Throttling and Debouncing
```javascript
// Throttle scroll events
var throttleTimer;
$(window).on('scroll', function() {
    if (throttleTimer) return;
    
    throttleTimer = setTimeout(function() {
        // Handle scroll
        handleScroll();
        throttleTimer = null;
    }, 16); // ~60fps
});

// Debounce search input
var searchTimer;
$('#search').on('input', function() {
    clearTimeout(searchTimer);
    var query = $(this).val();
    
    searchTimer = setTimeout(function() {
        performSearch(query);
    }, 300);
});
```

## Common Event Patterns

### Toggle Pattern
```javascript
$('#toggle-button').on('click', function() {
    var $target = $('#toggle-target');
    var isVisible = $target.is(':visible');
    
    $target.toggle();
    $(this).text(isVisible ? 'Show' : 'Hide');
});
```

### Tab System
```javascript
$('.tab-button').on('click', function() {
    var $button = $(this);
    var targetTab = $button.data('tab');
    
    // Update active button
    $('.tab-button').removeClass('active');
    $button.addClass('active');
    
    // Update active content
    $('.tab-content').hide();
    $('#' + targetTab).show();
});
```

## Best Practices

1. **Use event delegation** for dynamic content
2. **Prevent memory leaks** by removing event handlers when elements are removed
3. **Use namespaces** for organized event management
4. **Cache selectors** to improve performance
5. **Use `preventDefault()`** and `stopPropagation()` appropriately
6. **Throttle or debounce** high-frequency events
7. **Keep event handlers lightweight**

## Common Mistakes

1. **Not using event delegation** for dynamic content
2. **Forgetting to prevent default** behavior when needed
3. **Creating memory leaks** by not removing event handlers
4. **Binding events inside loops** without delegation
5. **Not stopping event propagation** when necessary

## Summary

jQuery event handling provides:
- **Simple event binding** with intuitive methods
- **Event delegation** for dynamic content
- **Rich event object** with useful properties and methods
- **Custom events** for application architecture
- **Event namespacing** for organized code
- **Performance optimizations** for better user experience

Tomorrow we'll explore jQuery animations and effects to make your interactions visually appealing!