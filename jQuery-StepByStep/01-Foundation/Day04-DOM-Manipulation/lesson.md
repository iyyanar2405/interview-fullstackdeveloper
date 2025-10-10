# Day 4: DOM Manipulation with jQuery

## Understanding DOM Manipulation

DOM (Document Object Model) manipulation is the process of dynamically changing the structure, content, and styling of web pages. jQuery makes DOM manipulation simple and intuitive.

## Content Manipulation

### Text Content
```javascript
// Get text content
var text = $('#myDiv').text();

// Set text content (escapes HTML)
$('#myDiv').text('Hello World');

// Set text for multiple elements
$('.items').text('Same text for all');
```

### HTML Content
```javascript
// Get HTML content
var html = $('#myDiv').html();

// Set HTML content (renders HTML)
$('#myDiv').html('<b>Bold Text</b>');

// Append HTML content
$('#myDiv').html($('#myDiv').html() + '<p>Additional content</p>');
```

### Form Values
```javascript
// Get form input value
var value = $('#input').val();

// Set form input value
$('#input').val('New value');

// Get/Set for different input types
$('#textInput').val('text value');
$('#selectDropdown').val('option2');
$('#checkbox').val(['option1', 'option3']); // For checkboxes
```

## Attribute Manipulation

### Getting and Setting Attributes
```javascript
// Get attribute value
var src = $('img').attr('src');
var id = $('#myDiv').attr('id');

// Set single attribute
$('img').attr('src', 'new-image.jpg');

// Set multiple attributes
$('img').attr({
    'src': 'image.jpg',
    'alt': 'Description',
    'width': '200'
});

// Remove attribute
$('img').removeAttr('width');
```

### Properties vs Attributes
```javascript
// Properties (current state)
var isChecked = $('#checkbox').prop('checked');
$('#checkbox').prop('checked', true);

// Attributes (initial HTML state)
var checkedAttr = $('#checkbox').attr('checked');
$('#checkbox').attr('checked', 'checked');
```

### Data Attributes
```javascript
// HTML: <div data-user-id="123" data-role="admin">
var userId = $('#myDiv').data('user-id');    // Gets 123
var role = $('#myDiv').data('role');         // Gets "admin"

// Set data attributes
$('#myDiv').data('user-id', 456);
$('#myDiv').data('status', 'active');
```

## CSS Manipulation

### CSS Properties
```javascript
// Get CSS property
var color = $('#myDiv').css('color');

// Set single CSS property
$('#myDiv').css('color', 'red');

// Set multiple CSS properties
$('#myDiv').css({
    'color': 'red',
    'font-size': '16px',
    'background-color': 'yellow',
    'border': '1px solid black'
});
```

### Classes
```javascript
// Add class
$('#myDiv').addClass('highlight');
$('#myDiv').addClass('highlight active'); // Multiple classes

// Remove class
$('#myDiv').removeClass('highlight');
$('#myDiv').removeClass(); // Remove all classes

// Toggle class
$('#myDiv').toggleClass('highlight');

// Check if has class
if ($('#myDiv').hasClass('highlight')) {
    console.log('Element has highlight class');
}
```

### Computed Styles
```javascript
// Get computed width/height
var width = $('#myDiv').width();      // Content width
var height = $('#myDiv').height();    // Content height
var outerWidth = $('#myDiv').outerWidth();   // Including padding/border
var outerHeight = $('#myDiv').outerHeight(); // Including padding/border

// Set dimensions
$('#myDiv').width(200);
$('#myDiv').height(100);
```

## Element Creation and Insertion

### Creating Elements
```javascript
// Create elements
var $newDiv = $('<div>');
var $newParagraph = $('<p>Hello World</p>');
var $complexElement = $('<div class="item" id="item1">Content</div>');

// Create with attributes
var $img = $('<img>', {
    src: 'image.jpg',
    alt: 'Description',
    class: 'thumbnail'
});
```

### Insertion Methods

#### Append and Prepend (Inside)
```javascript
// Append to end (inside)
$('#container').append('<p>Last paragraph</p>');
$('#container').append($newParagraph);

// Prepend to beginning (inside)
$('#container').prepend('<p>First paragraph</p>');

// AppendTo and PrependTo (reverse syntax)
$('<p>New content</p>').appendTo('#container');
$('<p>New content</p>').prependTo('#container');
```

#### Before and After (Outside)
```javascript
// Insert before element (outside)
$('#target').before('<p>Before target</p>');

// Insert after element (outside)
$('#target').after('<p>After target</p>');

// InsertBefore and InsertAfter (reverse syntax)
$('<p>Before target</p>').insertBefore('#target');
$('<p>After target</p>').insertAfter('#target');
```

### Wrapping Elements
```javascript
// Wrap element with another element
$('#content').wrap('<div class="wrapper">');

// Wrap inner content
$('#content').wrapInner('<div class="inner">');

// Wrap all selected elements together
$('.items').wrapAll('<div class="items-container">');

// Unwrap (remove parent wrapper)
$('#content').unwrap();
```

## Element Removal and Replacement

### Removing Elements
```javascript
// Remove element and its data/events
$('#myDiv').remove();

// Remove element but keep data/events
$('#myDiv').detach();

// Empty element (remove all children)
$('#container').empty();
```

### Replacing Elements
```javascript
// Replace element with new content
$('#oldElement').replaceWith('<div id="newElement">New content</div>');

// Replace all matched elements
$('.oldClass').replaceWith('<span class="newClass">Replaced</span>');

// ReplaceAll (reverse syntax)
$('<div>New content</div>').replaceAll('.oldClass');
```

## Cloning Elements

### Basic Cloning
```javascript
// Clone element (structure only)
var $clone = $('#original').clone();

// Clone with events and data
var $cloneWithEvents = $('#original').clone(true);

// Clone and append
$('#original').clone().appendTo('#container');
```

### Deep Cloning
```javascript
// Clone with all descendants' events and data
var $deepClone = $('#original').clone(true, true);
```

## Practical Examples

### Dynamic List Management
```javascript
function addListItem(text) {
    var $newItem = $('<li>').text(text);
    $newItem.append('<button class="delete-btn">Delete</button>');
    $('#myList').append($newItem);
}

function removeListItem($item) {
    $item.fadeOut(function() {
        $(this).remove();
    });
}

// Event delegation for dynamically added elements
$('#myList').on('click', '.delete-btn', function() {
    removeListItem($(this).parent());
});
```

### Form Field Generation
```javascript
function createFormField(type, name, label) {
    var $field = $('<div class="form-group">');
    var $label = $('<label>').attr('for', name).text(label);
    var $input = $('<input>').attr({
        type: type,
        name: name,
        id: name
    });
    
    $field.append($label).append($input);
    return $field;
}

// Usage
var $emailField = createFormField('email', 'email', 'Email Address');
$('#form-container').append($emailField);
```

### Table Row Manipulation
```javascript
function addTableRow(data) {
    var $row = $('<tr>');
    data.forEach(function(cellData) {
        $row.append($('<td>').text(cellData));
    });
    
    // Add action buttons
    var $actions = $('<td>');
    $actions.append('<button class="edit-btn">Edit</button>');
    $actions.append('<button class="delete-btn">Delete</button>');
    $row.append($actions);
    
    $('#dataTable tbody').append($row);
}
```

## Performance Considerations

### Efficient DOM Manipulation
```javascript
// Inefficient - multiple DOM updates
$('#container').append('<div>Item 1</div>');
$('#container').append('<div>Item 2</div>');
$('#container').append('<div>Item 3</div>');

// Efficient - single DOM update
var html = '<div>Item 1</div><div>Item 2</div><div>Item 3</div>';
$('#container').append(html);

// Or build as jQuery object
var $fragment = $('<div>');
$fragment.append('<div>Item 1</div>');
$fragment.append('<div>Item 2</div>');
$fragment.append('<div>Item 3</div>');
$('#container').append($fragment.html());
```

### Caching Elements
```javascript
// Cache frequently used elements
var $container = $('#container');
var $items = $('.items');

// Use cached references
$container.addClass('active');
$items.show();
```

## Common Patterns

### Conditional Content
```javascript
function updateStatus(isActive) {
    var $status = $('#status');
    if (isActive) {
        $status.removeClass('inactive').addClass('active').text('Online');
    } else {
        $status.removeClass('active').addClass('inactive').text('Offline');
    }
}
```

### Template-based Generation
```javascript
function createCard(data) {
    var template = `
        <div class="card">
            <h3 class="card-title">${data.title}</h3>
            <p class="card-content">${data.content}</p>
            <div class="card-actions">
                <button class="btn-primary">Edit</button>
                <button class="btn-danger">Delete</button>
            </div>
        </div>
    `;
    return $(template);
}
```

## Method Chaining in DOM Manipulation

### Chaining Creation and Insertion
```javascript
$('<div>')
    .addClass('highlight')
    .attr('id', 'newElement')
    .text('Hello World')
    .appendTo('#container')
    .fadeIn();
```

### Chaining Modifications
```javascript
$('#myElement')
    .empty()
    .append('<h2>New Title</h2>')
    .append('<p>New content</p>')
    .addClass('updated')
    .show();
```

## Best Practices

1. **Use appropriate methods**:
   - Use `text()` for plain text (security)
   - Use `html()` only when you need HTML rendering
   - Use `attr()` for HTML attributes
   - Use `prop()` for DOM properties

2. **Minimize DOM manipulations**:
   - Build HTML strings or fragments
   - Use `DocumentFragment` for multiple insertions
   - Cache jQuery objects

3. **Event delegation for dynamic content**:
   ```javascript
   // For dynamically added elements
   $(document).on('click', '.dynamic-button', handler);
   ```

4. **Clean up when removing elements**:
   ```javascript
   // Remove event listeners and data
   $('#element').off().removeData().remove();
   ```

## Common Mistakes

1. **Forgetting to return jQuery objects in custom methods**
2. **Not using event delegation for dynamically added elements**
3. **Excessive DOM queries instead of caching**
4. **Using `html()` when `text()` would be safer**
5. **Not considering performance with large DOM manipulations**

## Practice Exercises

### Exercise 1: Todo List
Create a dynamic todo list with add, edit, delete, and toggle functionality.

### Exercise 2: Image Gallery
Build an image gallery with dynamic thumbnail generation and modal viewing.

### Exercise 3: Data Table
Create a sortable, filterable data table with CRUD operations.

### Exercise 4: Form Builder
Build a form builder that allows adding/removing different field types dynamically.

## Summary

jQuery DOM manipulation provides powerful methods for:
- **Content**: `text()`, `html()`, `val()`
- **Attributes**: `attr()`, `prop()`, `data()`
- **CSS**: `css()`, `addClass()`, `removeClass()`, `toggleClass()`
- **Creation**: `$('<element>')`, element creation with attributes
- **Insertion**: `append()`, `prepend()`, `before()`, `after()`
- **Removal**: `remove()`, `detach()`, `empty()`
- **Cloning**: `clone()` with optional event/data preservation

Tomorrow we'll explore jQuery event handling to make our DOM manipulations interactive!