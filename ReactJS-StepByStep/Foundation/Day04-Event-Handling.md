# Day 04: Event Handling - Making Components Interactive

## Learning Objectives
By the end of this lesson, you will:
- Master event handling in React
- Understand synthetic events and their benefits
- Learn form handling and controlled components
- Build interactive user interfaces
- Handle keyboard, mouse, and form events effectively
- Prevent common event handling pitfalls

## 1. Introduction to Event Handling

### React Events vs DOM Events
React uses **SyntheticEvents** - a cross-browser wrapper around native events that provides consistent behavior across different browsers.

```jsx
import React, { useState } from 'react'

function EventBasics() {
  const [message, setMessage] = useState('')
  
  // Basic event handler
  const handleClick = (event) => {
    console.log('Event object:', event)
    console.log('Event type:', event.type)
    console.log('Target element:', event.target)
    setMessage('Button was clicked!')
  }
  
  // Inline event handler
  const handleInlineClick = () => {
    setMessage('Inline handler executed!')
  }
  
  return (
    <div>
      <h2>Event Handling Basics</h2>
      <button onClick={handleClick}>Click Me</button>
      <button onClick={handleInlineClick}>Inline Handler</button>
      <button onClick={() => setMessage('Arrow function handler!')}>
        Arrow Function
      </button>
      <p>{message}</p>
    </div>
  )
}

export default EventBasics
```

### Event Handler with Parameters

```jsx
function EventWithParams() {
  const [selectedItem, setSelectedItem] = useState('')
  
  // Handler with parameters
  const handleItemClick = (itemName, event) => {
    event.preventDefault()
    setSelectedItem(itemName)
    console.log('Clicked item:', itemName)
  }
  
  // Using arrow function to pass parameters
  const items = ['Apple', 'Banana', 'Orange', 'Grape']
  
  return (
    <div>
      <h3>Selected Item: {selectedItem}</h3>
      <ul>
        {items.map((item, index) => (
          <li key={index}>
            <button onClick={(e) => handleItemClick(item, e)}>
              {item}
            </button>
          </li>
        ))}
      </ul>
    </div>
  )
}
```

## 2. Common Event Types

### Mouse Events

```jsx
import React, { useState } from 'react'

function MouseEvents() {
  const [mouseInfo, setMouseInfo] = useState({
    clicked: false,
    position: { x: 0, y: 0 },
    isHovering: false
  })
  
  const handleMouseMove = (event) => {
    setMouseInfo(prev => ({
      ...prev,
      position: { x: event.clientX, y: event.clientY }
    }))
  }
  
  const handleMouseEnter = () => {
    setMouseInfo(prev => ({ ...prev, isHovering: true }))
  }
  
  const handleMouseLeave = () => {
    setMouseInfo(prev => ({ ...prev, isHovering: false }))
  }
  
  const handleClick = () => {
    setMouseInfo(prev => ({ ...prev, clicked: !prev.clicked }))
  }
  
  const handleDoubleClick = () => {
    alert('Double clicked!')
  }
  
  const handleRightClick = (event) => {
    event.preventDefault()
    alert('Right clicked!')
  }
  
  return (
    <div 
      onMouseMove={handleMouseMove}
      style={{ padding: '20px', border: '1px solid #ccc', minHeight: '200px' }}
    >
      <h3>Mouse Events</h3>
      <p>Mouse Position: X: {mouseInfo.position.x}, Y: {mouseInfo.position.y}</p>
      
      <button
        onClick={handleClick}
        onDoubleClick={handleDoubleClick}
        onContextMenu={handleRightClick}
        onMouseEnter={handleMouseEnter}
        onMouseLeave={handleMouseLeave}
        style={{
          backgroundColor: mouseInfo.isHovering ? '#007bff' : '#6c757d',
          color: 'white',
          border: mouseInfo.clicked ? '3px solid red' : '1px solid gray',
          padding: '10px 20px',
          margin: '10px'
        }}
      >
        Interactive Button
      </button>
      
      <p>Status: {mouseInfo.isHovering ? 'Hovering' : 'Not hovering'}</p>
      <p>Clicked: {mouseInfo.clicked ? 'Yes' : 'No'}</p>
    </div>
  )
}

export default MouseEvents
```

### Keyboard Events

```jsx
import React, { useState } from 'react'

function KeyboardEvents() {
  const [keyInfo, setKeyInfo] = useState({
    lastKey: '',
    keyHistory: [],
    inputValue: ''
  })
  
  const handleKeyDown = (event) => {
    console.log('Key down:', event.key, 'Code:', event.code)
    
    // Handle special keys
    if (event.key === 'Enter') {
      alert('Enter key pressed!')
    } else if (event.key === 'Escape') {
      setKeyInfo(prev => ({ ...prev, inputValue: '' }))
    }
    
    setKeyInfo(prev => ({
      ...prev,
      lastKey: event.key,
      keyHistory: [...prev.keyHistory.slice(-9), event.key]
    }))
  }
  
  const handleKeyPress = (event) => {
    // Only fires for printable characters
    console.log('Key press:', event.key)
  }
  
  const handleInputChange = (event) => {
    setKeyInfo(prev => ({
      ...prev,
      inputValue: event.target.value
    }))
  }
  
  const handleKeyUp = (event) => {
    console.log('Key up:', event.key)
  }
  
  return (
    <div>
      <h3>Keyboard Events</h3>
      <input
        type="text"
        value={keyInfo.inputValue}
        onChange={handleInputChange}
        onKeyDown={handleKeyDown}
        onKeyPress={handleKeyPress}
        onKeyUp={handleKeyUp}
        placeholder="Type something... Try Enter or Escape"
        style={{ padding: '10px', fontSize: '16px', width: '300px' }}
      />
      
      <div style={{ marginTop: '20px' }}>
        <p><strong>Last Key:</strong> {keyInfo.lastKey}</p>
        <p><strong>Current Value:</strong> {keyInfo.inputValue}</p>
        <p><strong>Key History:</strong> {keyInfo.keyHistory.join(' → ')}</p>
      </div>
      
      <div style={{ marginTop: '20px' }}>
        <h4>Keyboard Shortcuts:</h4>
        <ul>
          <li>Enter - Show alert</li>
          <li>Escape - Clear input</li>
        </ul>
      </div>
    </div>
  )
}

export default KeyboardEvents
```

## 3. Form Handling

### Controlled Components

```jsx
import React, { useState } from 'react'

function ControlledForm() {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    age: '',
    gender: '',
    country: '',
    skills: [],
    bio: '',
    subscribe: false,
    terms: false
  })
  
  const [errors, setErrors] = useState({})
  
  // Generic input handler
  const handleInputChange = (event) => {
    const { name, value, type, checked } = event.target
    
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }))
    
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }))
    }
  }
  
  // Handle checkbox array (skills)
  const handleSkillChange = (event) => {
    const { value, checked } = event.target
    
    setFormData(prev => ({
      ...prev,
      skills: checked
        ? [...prev.skills, value]
        : prev.skills.filter(skill => skill !== value)
    }))
  }
  
  // Form validation
  const validateForm = () => {
    const newErrors = {}
    
    if (!formData.firstName.trim()) {
      newErrors.firstName = 'First name is required'
    }
    
    if (!formData.lastName.trim()) {
      newErrors.lastName = 'Last name is required'
    }
    
    if (!formData.email.trim()) {
      newErrors.email = 'Email is required'
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email is invalid'
    }
    
    if (!formData.password) {
      newErrors.password = 'Password is required'
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters'
    }
    
    if (!formData.terms) {
      newErrors.terms = 'You must accept the terms and conditions'
    }
    
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }
  
  const handleSubmit = (event) => {
    event.preventDefault()
    
    if (validateForm()) {
      console.log('Form submitted:', formData)
      alert('Form submitted successfully!')
    } else {
      console.log('Form has errors:', errors)
    }
  }
  
  const handleReset = () => {
    setFormData({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      age: '',
      gender: '',
      country: '',
      skills: [],
      bio: '',
      subscribe: false,
      terms: false
    })
    setErrors({})
  }
  
  return (
    <div style={{ maxWidth: '600px', margin: '0 auto', padding: '20px' }}>
      <h2>User Registration Form</h2>
      
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
        {/* Text Inputs */}
        <div style={{ display: 'flex', gap: '10px' }}>
          <div style={{ flex: 1 }}>
            <label>First Name *</label>
            <input
              type="text"
              name="firstName"
              value={formData.firstName}
              onChange={handleInputChange}
              style={{ 
                width: '100%', 
                padding: '8px', 
                border: errors.firstName ? '2px solid red' : '1px solid #ccc' 
              }}
            />
            {errors.firstName && <span style={{ color: 'red', fontSize: '12px' }}>{errors.firstName}</span>}
          </div>
          
          <div style={{ flex: 1 }}>
            <label>Last Name *</label>
            <input
              type="text"
              name="lastName"
              value={formData.lastName}
              onChange={handleInputChange}
              style={{ 
                width: '100%', 
                padding: '8px',
                border: errors.lastName ? '2px solid red' : '1px solid #ccc'
              }}
            />
            {errors.lastName && <span style={{ color: 'red', fontSize: '12px' }}>{errors.lastName}</span>}
          </div>
        </div>
        
        {/* Email */}
        <div>
          <label>Email *</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleInputChange}
            style={{ 
              width: '100%', 
              padding: '8px',
              border: errors.email ? '2px solid red' : '1px solid #ccc'
            }}
          />
          {errors.email && <span style={{ color: 'red', fontSize: '12px' }}>{errors.email}</span>}
        </div>
        
        {/* Password */}
        <div>
          <label>Password *</label>
          <input
            type="password"
            name="password"
            value={formData.password}
            onChange={handleInputChange}
            style={{ 
              width: '100%', 
              padding: '8px',
              border: errors.password ? '2px solid red' : '1px solid #ccc'
            }}
          />
          {errors.password && <span style={{ color: 'red', fontSize: '12px' }}>{errors.password}</span>}
        </div>
        
        {/* Number Input */}
        <div>
          <label>Age</label>
          <input
            type="number"
            name="age"
            value={formData.age}
            onChange={handleInputChange}
            min="1"
            max="120"
            style={{ width: '100%', padding: '8px' }}
          />
        </div>
        
        {/* Radio Buttons */}
        <div>
          <label>Gender</label>
          <div style={{ display: 'flex', gap: '15px', marginTop: '5px' }}>
            <label>
              <input
                type="radio"
                name="gender"
                value="male"
                checked={formData.gender === 'male'}
                onChange={handleInputChange}
              />
              Male
            </label>
            <label>
              <input
                type="radio"
                name="gender"
                value="female"
                checked={formData.gender === 'female'}
                onChange={handleInputChange}
              />
              Female
            </label>
            <label>
              <input
                type="radio"
                name="gender"
                value="other"
                checked={formData.gender === 'other'}
                onChange={handleInputChange}
              />
              Other
            </label>
          </div>
        </div>
        
        {/* Select Dropdown */}
        <div>
          <label>Country</label>
          <select
            name="country"
            value={formData.country}
            onChange={handleInputChange}
            style={{ width: '100%', padding: '8px' }}
          >
            <option value="">Select a country</option>
            <option value="us">United States</option>
            <option value="uk">United Kingdom</option>
            <option value="ca">Canada</option>
            <option value="au">Australia</option>
            <option value="de">Germany</option>
          </select>
        </div>
        
        {/* Multiple Checkboxes */}
        <div>
          <label>Skills</label>
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: '10px', marginTop: '5px' }}>
            {['JavaScript', 'React', 'Node.js', 'Python', 'Java', 'C++'].map(skill => (
              <label key={skill}>
                <input
                  type="checkbox"
                  value={skill}
                  checked={formData.skills.includes(skill)}
                  onChange={handleSkillChange}
                />
                {skill}
              </label>
            ))}
          </div>
        </div>
        
        {/* Textarea */}
        <div>
          <label>Bio</label>
          <textarea
            name="bio"
            value={formData.bio}
            onChange={handleInputChange}
            rows="4"
            placeholder="Tell us about yourself..."
            style={{ width: '100%', padding: '8px', resize: 'vertical' }}
          />
        </div>
        
        {/* Single Checkbox */}
        <div>
          <label>
            <input
              type="checkbox"
              name="subscribe"
              checked={formData.subscribe}
              onChange={handleInputChange}
            />
            Subscribe to newsletter
          </label>
        </div>
        
        {/* Required Checkbox */}
        <div>
          <label style={{ color: errors.terms ? 'red' : 'black' }}>
            <input
              type="checkbox"
              name="terms"
              checked={formData.terms}
              onChange={handleInputChange}
            />
            I accept the terms and conditions *
          </label>
          {errors.terms && <div style={{ color: 'red', fontSize: '12px' }}>{errors.terms}</div>}
        </div>
        
        {/* Buttons */}
        <div style={{ display: 'flex', gap: '10px', marginTop: '20px' }}>
          <button 
            type="submit"
            style={{ 
              backgroundColor: '#007bff', 
              color: 'white', 
              padding: '10px 20px', 
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            Submit
          </button>
          <button 
            type="button"
            onClick={handleReset}
            style={{ 
              backgroundColor: '#6c757d', 
              color: 'white', 
              padding: '10px 20px', 
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            Reset
          </button>
        </div>
      </form>
      
      {/* Form Data Preview */}
      <div style={{ marginTop: '30px', padding: '20px', backgroundColor: '#f8f9fa', borderRadius: '4px' }}>
        <h3>Form Data Preview:</h3>
        <pre style={{ fontSize: '12px', overflow: 'auto' }}>
          {JSON.stringify(formData, null, 2)}
        </pre>
      </div>
    </div>
  )
}

export default ControlledForm
```

## 4. Advanced Event Handling

### Event Delegation and Performance

```jsx
import React, { useState, useCallback } from 'react'

function EventDelegation() {
  const [items, setItems] = useState([
    { id: 1, name: 'Item 1', active: false },
    { id: 2, name: 'Item 2', active: false },
    { id: 3, name: 'Item 3', active: false }
  ])
  
  // Using event delegation
  const handleListClick = useCallback((event) => {
    const itemId = event.target.dataset.itemId
    const action = event.target.dataset.action
    
    if (!itemId || !action) return
    
    const id = parseInt(itemId)
    
    switch (action) {
      case 'toggle':
        setItems(prev => prev.map(item =>
          item.id === id ? { ...item, active: !item.active } : item
        ))
        break
      case 'delete':
        setItems(prev => prev.filter(item => item.id !== id))
        break
      default:
        break
    }
  }, [])
  
  const addItem = () => {
    const newId = Math.max(...items.map(item => item.id)) + 1
    setItems(prev => [...prev, { id: newId, name: `Item ${newId}`, active: false }])
  }
  
  return (
    <div>
      <h3>Event Delegation Example</h3>
      <button onClick={addItem}>Add Item</button>
      
      {/* Single event listener for all items */}
      <ul onClick={handleListClick} style={{ listStyle: 'none', padding: 0 }}>
        {items.map(item => (
          <li
            key={item.id}
            style={{
              padding: '10px',
              margin: '5px 0',
              backgroundColor: item.active ? '#e3f2fd' : '#f5f5f5',
              border: '1px solid #ddd',
              borderRadius: '4px',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>{item.name} {item.active ? '(Active)' : ''}</span>
            <div>
              <button
                data-item-id={item.id}
                data-action="toggle"
                style={{ marginRight: '10px' }}
              >
                {item.active ? 'Deactivate' : 'Activate'}
              </button>
              <button
                data-item-id={item.id}
                data-action="delete"
                style={{ backgroundColor: '#dc3545', color: 'white' }}
              >
                Delete
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  )
}

export default EventDelegation
```

### Preventing Default and Stopping Propagation

```jsx
import React, { useState } from 'react'

function EventPrevention() {
  const [logs, setLogs] = useState([])
  
  const addLog = (message) => {
    setLogs(prev => [...prev, `${new Date().toLocaleTimeString()}: ${message}`])
  }
  
  const handleParentClick = () => {
    addLog('Parent div clicked')
  }
  
  const handleChildClick = (event) => {
    addLog('Child button clicked')
    // Uncomment to stop event bubbling
    // event.stopPropagation()
  }
  
  const handleLinkClick = (event) => {
    event.preventDefault() // Prevent navigation
    addLog('Link clicked but navigation prevented')
  }
  
  const handleFormSubmit = (event) => {
    event.preventDefault() // Prevent form submission
    addLog('Form submit prevented')
  }
  
  const clearLogs = () => {
    setLogs([])
  }
  
  return (
    <div style={{ padding: '20px' }}>
      <h3>Event Prevention Examples</h3>
      
      {/* Event Bubbling Example */}
      <div
        onClick={handleParentClick}
        style={{
          padding: '20px',
          backgroundColor: '#e3f2fd',
          border: '2px solid #2196f3',
          margin: '20px 0'
        }}
      >
        <p>Parent Div (click me)</p>
        <button onClick={handleChildClick}>
          Child Button (click me too)
        </button>
        <p><small>Notice how clicking the button also triggers the parent's click event</small></p>
      </div>
      
      {/* Prevent Default Examples */}
      <div style={{ margin: '20px 0' }}>
        <a href="https://example.com" onClick={handleLinkClick}>
          Click this link (navigation prevented)
        </a>
      </div>
      
      <form onSubmit={handleFormSubmit} style={{ margin: '20px 0' }}>
        <input type="text" placeholder="Type something" />
        <button type="submit">Submit (prevented)</button>
      </form>
      
      {/* Event Logs */}
      <div style={{ marginTop: '30px' }}>
        <h4>Event Logs:</h4>
        <button onClick={clearLogs} style={{ marginBottom: '10px' }}>
          Clear Logs
        </button>
        <div
          style={{
            backgroundColor: '#f8f9fa',
            padding: '10px',
            maxHeight: '200px',
            overflowY: 'auto',
            border: '1px solid #ddd',
            borderRadius: '4px'
          }}
        >
          {logs.length === 0 ? (
            <p>No events logged yet</p>
          ) : (
            logs.map((log, index) => (
              <div key={index} style={{ fontSize: '12px', marginBottom: '2px' }}>
                {log}
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  )
}

export default EventPrevention
```

## 5. Real-World Examples

### Interactive Calculator

```jsx
import React, { useState } from 'react'

function Calculator() {
  const [display, setDisplay] = useState('0')
  const [previousValue, setPreviousValue] = useState(null)
  const [operation, setOperation] = useState(null)
  const [waitingForNewValue, setWaitingForNewValue] = useState(false)
  
  const inputNumber = (num) => {
    if (waitingForNewValue) {
      setDisplay(String(num))
      setWaitingForNewValue(false)
    } else {
      setDisplay(display === '0' ? String(num) : display + num)
    }
  }
  
  const inputDecimal = () => {
    if (waitingForNewValue) {
      setDisplay('0.')
      setWaitingForNewValue(false)
    } else if (display.indexOf('.') === -1) {
      setDisplay(display + '.')
    }
  }
  
  const clear = () => {
    setDisplay('0')
    setPreviousValue(null)
    setOperation(null)
    setWaitingForNewValue(false)
  }
  
  const performOperation = (nextOperation) => {
    const inputValue = parseFloat(display)
    
    if (previousValue === null) {
      setPreviousValue(inputValue)
    } else if (operation) {
      const currentValue = previousValue || 0
      const newValue = calculate(currentValue, inputValue, operation)
      
      setDisplay(String(newValue))
      setPreviousValue(newValue)
    }
    
    setWaitingForNewValue(true)
    setOperation(nextOperation)
  }
  
  const calculate = (firstValue, secondValue, operation) => {
    switch (operation) {
      case '+':
        return firstValue + secondValue
      case '-':
        return firstValue - secondValue
      case '*':
        return firstValue * secondValue
      case '/':
        return firstValue / secondValue
      case '=':
        return secondValue
      default:
        return secondValue
    }
  }
  
  const handleKeyPress = (event) => {
    const { key } = event
    
    if (key >= '0' && key <= '9') {
      inputNumber(parseInt(key))
    } else if (key === '.') {
      inputDecimal()
    } else if (key === '+' || key === '-' || key === '*' || key === '/') {
      performOperation(key)
    } else if (key === 'Enter' || key === '=') {
      performOperation('=')
    } else if (key === 'Escape' || key === 'c' || key === 'C') {
      clear()
    }
  }
  
  React.useEffect(() => {
    document.addEventListener('keydown', handleKeyPress)
    return () => document.removeEventListener('keydown', handleKeyPress)
  }, [display, previousValue, operation, waitingForNewValue])
  
  const buttonStyle = {
    padding: '20px',
    fontSize: '18px',
    border: '1px solid #ccc',
    backgroundColor: '#f8f9fa',
    cursor: 'pointer',
    transition: 'background-color 0.2s'
  }
  
  const operatorStyle = {
    ...buttonStyle,
    backgroundColor: '#007bff',
    color: 'white'
  }
  
  return (
    <div style={{ maxWidth: '300px', margin: '0 auto', fontFamily: 'Arial, sans-serif' }}>
      <h3>Interactive Calculator</h3>
      <p><small>You can use keyboard for input!</small></p>
      
      {/* Display */}
      <div
        style={{
          padding: '20px',
          fontSize: '24px',
          textAlign: 'right',
          backgroundColor: '#000',
          color: '#0f0',
          fontFamily: 'monospace',
          border: '2px solid #333',
          marginBottom: '10px',
          minHeight: '40px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'flex-end'
        }}
      >
        {display}
      </div>
      
      {/* Buttons Grid */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '2px' }}>
        <button style={buttonStyle} onClick={clear}>C</button>
        <button style={buttonStyle} onClick={() => {}}>±</button>
        <button style={buttonStyle} onClick={() => {}}>%</button>
        <button style={operatorStyle} onClick={() => performOperation('/')}>÷</button>
        
        <button style={buttonStyle} onClick={() => inputNumber(7)}>7</button>
        <button style={buttonStyle} onClick={() => inputNumber(8)}>8</button>
        <button style={buttonStyle} onClick={() => inputNumber(9)}>9</button>
        <button style={operatorStyle} onClick={() => performOperation('*')}>×</button>
        
        <button style={buttonStyle} onClick={() => inputNumber(4)}>4</button>
        <button style={buttonStyle} onClick={() => inputNumber(5)}>5</button>
        <button style={buttonStyle} onClick={() => inputNumber(6)}>6</button>
        <button style={operatorStyle} onClick={() => performOperation('-')}>-</button>
        
        <button style={buttonStyle} onClick={() => inputNumber(1)}>1</button>
        <button style={buttonStyle} onClick={() => inputNumber(2)}>2</button>
        <button style={buttonStyle} onClick={() => inputNumber(3)}>3</button>
        <button style={operatorStyle} onClick={() => performOperation('+')}>+</button>
        
        <button style={{...buttonStyle, gridColumn: '1 / 3'}} onClick={() => inputNumber(0)}>0</button>
        <button style={buttonStyle} onClick={inputDecimal}>.</button>
        <button style={operatorStyle} onClick={() => performOperation('=')}>=</button>
      </div>
    </div>
  )
}

export default Calculator
```

## 6. Best Practices and Performance

### Event Handler Optimization

```jsx
import React, { useState, useCallback, useMemo } from 'react'

function OptimizedEventHandling() {
  const [items, setItems] = useState([
    { id: 1, name: 'Item 1', count: 0 },
    { id: 2, name: 'Item 2', count: 0 },
    { id: 3, name: 'Item 3', count: 0 }
  ])
  
  // ✅ Optimized: Using useCallback to memoize handlers
  const handleIncrement = useCallback((id) => {
    setItems(prev => prev.map(item =>
      item.id === id ? { ...item, count: item.count + 1 } : item
    ))
  }, [])
  
  const handleDecrement = useCallback((id) => {
    setItems(prev => prev.map(item =>
      item.id === id ? { ...item, count: Math.max(0, item.count - 1) } : item
    ))
  }, [])
  
  // ✅ Memoize expensive calculations
  const totalCount = useMemo(() => {
    return items.reduce((sum, item) => sum + item.count, 0)
  }, [items])
  
  return (
    <div>
      <h3>Optimized Event Handling</h3>
      <p>Total Count: {totalCount}</p>
      
      {items.map(item => (
        <OptimizedItem
          key={item.id}
          item={item}
          onIncrement={() => handleIncrement(item.id)}
          onDecrement={() => handleDecrement(item.id)}
        />
      ))}
    </div>
  )
}

// ✅ Memoized child component
const OptimizedItem = React.memo(({ item, onIncrement, onDecrement }) => {
  console.log(`Rendering item ${item.id}`) // This should only log when the specific item changes
  
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '10px', margin: '10px 0' }}>
      <span>{item.name}</span>
      <button onClick={onDecrement}>-</button>
      <span>{item.count}</span>
      <button onClick={onIncrement}>+</button>
    </div>
  )
})

export default OptimizedEventHandling
```

## 7. Common Mistakes and Solutions

### 1. Binding Issues (Class Components)
```jsx
// ❌ Wrong - 'this' will be undefined
onClick={this.handleClick}

// ✅ Correct - Bind in constructor or use arrow functions
onClick={this.handleClick.bind(this)}
onClick={() => this.handleClick()}
```

### 2. Inline Arrow Functions (Performance Impact)
```jsx
// ❌ Creates new function on every render
onClick={() => console.log('clicked')}

// ✅ Better - Use useCallback for handlers with dependencies
const handleClick = useCallback(() => {
  console.log('clicked')
}, [])
```

### 3. Event Object Usage
```jsx
// ❌ Event object might be null in async operations
const handleClick = (event) => {
  setTimeout(() => {
    console.log(event.target) // Might be null
  }, 1000)
}

// ✅ Persist event or extract values immediately
const handleClick = (event) => {
  const target = event.target
  setTimeout(() => {
    console.log(target) // Safe to use
  }, 1000)
}
```

## 8. Exercises

### Exercise 1: Image Carousel
Create an image carousel with previous/next buttons and keyboard navigation.

### Exercise 2: Drag and Drop List
Build a todo list where items can be reordered by dragging.

### Exercise 3: Advanced Form
Create a multi-step form with validation and progress indicator.

### Exercise 4: Game Implementation
Build a simple game (like Tic-tac-toe) using event handling.

## Summary

Today you learned:
- ✅ React event system and SyntheticEvents
- ✅ Mouse, keyboard, and form event handling
- ✅ Controlled components and form management
- ✅ Event delegation and performance optimization
- ✅ Preventing default behavior and event propagation
- ✅ Real-world interactive component examples

## Next Lesson Preview

In Day 05, we'll explore:
- Advanced React Hooks (useEffect, useContext, useReducer)
- Component lifecycle and side effects
- Custom hooks creation
- Performance optimization techniques

Keep practicing with interactive components and you'll master React event handling!