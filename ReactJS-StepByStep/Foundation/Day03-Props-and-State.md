# Day 03: Props and State - Building Dynamic Components

## Learning Objectives
By the end of this lesson, you will:
- Understand the difference between props and state
- Learn how to pass data between components using props
- Master state management with useState hook
- Build interactive components with state
- Understand the component lifecycle and re-rendering

## 1. Understanding Props

### What are Props?
Props (properties) are read-only data passed from parent to child components. They allow components to be dynamic and reusable.

### Basic Props Example

```jsx
// ParentComponent.jsx
import React from 'react'
import ChildComponent from './ChildComponent'

function ParentComponent() {
  const userName = "John Doe"
  const userAge = 25
  
  return (
    <div>
      <h1>Parent Component</h1>
      <ChildComponent name={userName} age={userAge} />
    </div>
  )
}

export default ParentComponent
```

```jsx
// ChildComponent.jsx
import React from 'react'

function ChildComponent(props) {
  return (
    <div>
      <h2>Child Component</h2>
      <p>Name: {props.name}</p>
      <p>Age: {props.age}</p>
    </div>
  )
}

export default ChildComponent
```

### Props Destructuring

```jsx
// More elegant way using destructuring
function ChildComponent({ name, age }) {
  return (
    <div>
      <h2>Child Component</h2>
      <p>Name: {name}</p>
      <p>Age: {age}</p>
    </div>
  )
}
```

### Props with TypeScript

```tsx
// ChildComponent.tsx
import React from 'react'

interface ChildProps {
  name: string
  age: number
  isActive?: boolean // Optional prop
}

const ChildComponent: React.FC<ChildProps> = ({ name, age, isActive = true }) => {
  return (
    <div>
      <h2>Child Component</h2>
      <p>Name: {name}</p>
      <p>Age: {age}</p>
      <p>Status: {isActive ? 'Active' : 'Inactive'}</p>
    </div>
  )
}

export default ChildComponent
```

## 2. Understanding State

### What is State?
State is mutable data that belongs to a component. When state changes, the component re-renders to reflect the new data.

### useState Hook Basics

```jsx
import React, { useState } from 'react'

function Counter() {
  // useState returns [currentValue, setterFunction]
  const [count, setCount] = useState(0)
  
  const increment = () => {
    setCount(count + 1)
  }
  
  const decrement = () => {
    setCount(count - 1)
  }
  
  return (
    <div>
      <h2>Counter: {count}</h2>
      <button onClick={increment}>+</button>
      <button onClick={decrement}>-</button>
    </div>
  )
}

export default Counter
```

### Different Types of State

```jsx
import React, { useState } from 'react'

function StateExamples() {
  // String state
  const [name, setName] = useState('')
  
  // Boolean state
  const [isVisible, setIsVisible] = useState(true)
  
  // Array state
  const [items, setItems] = useState([])
  
  // Object state
  const [user, setUser] = useState({
    name: '',
    email: '',
    age: 0
  })
  
  // Adding item to array
  const addItem = () => {
    setItems([...items, `Item ${items.length + 1}`])
  }
  
  // Updating object state
  const updateUser = () => {
    setUser({
      ...user,
      name: 'John Doe'
    })
  }
  
  return (
    <div>
      <div>
        <input 
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Enter name"
        />
        <p>Hello, {name}!</p>
      </div>
      
      <div>
        <button onClick={() => setIsVisible(!isVisible)}>
          Toggle Visibility
        </button>
        {isVisible && <p>I'm visible!</p>}
      </div>
      
      <div>
        <button onClick={addItem}>Add Item</button>
        <ul>
          {items.map((item, index) => (
            <li key={index}>{item}</li>
          ))}
        </ul>
      </div>
      
      <div>
        <button onClick={updateUser}>Update User</button>
        <p>User: {user.name}</p>
      </div>
    </div>
  )
}

export default StateExamples
```

## 3. Practical Examples

### Example 1: User Profile Card

```jsx
// UserProfile.jsx
import React, { useState } from 'react'
import './UserProfile.css'

function UserProfile({ initialUser }) {
  const [user, setUser] = useState(initialUser || {
    name: '',
    email: '',
    bio: '',
    avatar: ''
  })
  
  const [isEditing, setIsEditing] = useState(false)
  
  const handleInputChange = (field, value) => {
    setUser({
      ...user,
      [field]: value
    })
  }
  
  const toggleEdit = () => {
    setIsEditing(!isEditing)
  }
  
  return (
    <div className="user-profile">
      <div className="profile-header">
        <img 
          src={user.avatar || '/default-avatar.png'} 
          alt="Profile" 
          className="avatar"
        />
        <button onClick={toggleEdit} className="edit-btn">
          {isEditing ? 'Save' : 'Edit'}
        </button>
      </div>
      
      <div className="profile-content">
        {isEditing ? (
          <div className="edit-form">
            <input
              type="text"
              value={user.name}
              onChange={(e) => handleInputChange('name', e.target.value)}
              placeholder="Name"
            />
            <input
              type="email"
              value={user.email}
              onChange={(e) => handleInputChange('email', e.target.value)}
              placeholder="Email"
            />
            <textarea
              value={user.bio}
              onChange={(e) => handleInputChange('bio', e.target.value)}
              placeholder="Bio"
            />
          </div>
        ) : (
          <div className="profile-display">
            <h2>{user.name || 'No name provided'}</h2>
            <p className="email">{user.email || 'No email provided'}</p>
            <p className="bio">{user.bio || 'No bio provided'}</p>
          </div>
        )}
      </div>
    </div>
  )
}

export default UserProfile
```

```css
/* UserProfile.css */
.user-profile {
  max-width: 400px;
  margin: 20px auto;
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background: white;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.profile-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  object-fit: cover;
}

.edit-btn {
  padding: 8px 16px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.edit-btn:hover {
  background: #0056b3;
}

.edit-form input,
.edit-form textarea {
  width: 100%;
  padding: 8px;
  margin: 5px 0;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.edit-form textarea {
  height: 60px;
  resize: vertical;
}

.profile-display h2 {
  margin: 0 0 10px 0;
  color: #333;
}

.email {
  color: #666;
  font-style: italic;
}

.bio {
  margin-top: 15px;
  line-height: 1.5;
}
```

### Example 2: Shopping Cart

```jsx
// ShoppingCart.jsx
import React, { useState } from 'react'
import './ShoppingCart.css'

function ShoppingCart() {
  const [cartItems, setCartItems] = useState([])
  const [products] = useState([
    { id: 1, name: 'Laptop', price: 999.99 },
    { id: 2, name: 'Phone', price: 599.99 },
    { id: 3, name: 'Headphones', price: 199.99 },
    { id: 4, name: 'Keyboard', price: 89.99 }
  ])
  
  const addToCart = (product) => {
    const existingItem = cartItems.find(item => item.id === product.id)
    
    if (existingItem) {
      setCartItems(cartItems.map(item =>
        item.id === product.id
          ? { ...item, quantity: item.quantity + 1 }
          : item
      ))
    } else {
      setCartItems([...cartItems, { ...product, quantity: 1 }])
    }
  }
  
  const removeFromCart = (productId) => {
    setCartItems(cartItems.filter(item => item.id !== productId))
  }
  
  const updateQuantity = (productId, newQuantity) => {
    if (newQuantity === 0) {
      removeFromCart(productId)
    } else {
      setCartItems(cartItems.map(item =>
        item.id === productId
          ? { ...item, quantity: newQuantity }
          : item
      ))
    }
  }
  
  const getTotalPrice = () => {
    return cartItems.reduce((total, item) => total + (item.price * item.quantity), 0)
  }
  
  return (
    <div className="shopping-cart">
      <div className="products-section">
        <h2>Products</h2>
        <div className="products-grid">
          {products.map(product => (
            <div key={product.id} className="product-card">
              <h3>{product.name}</h3>
              <p className="price">${product.price}</p>
              <button onClick={() => addToCart(product)}>
                Add to Cart
              </button>
            </div>
          ))}
        </div>
      </div>
      
      <div className="cart-section">
        <h2>Shopping Cart ({cartItems.length} items)</h2>
        {cartItems.length === 0 ? (
          <p>Your cart is empty</p>
        ) : (
          <>
            <div className="cart-items">
              {cartItems.map(item => (
                <div key={item.id} className="cart-item">
                  <div className="item-info">
                    <h4>{item.name}</h4>
                    <p>${item.price}</p>
                  </div>
                  <div className="quantity-controls">
                    <button onClick={() => updateQuantity(item.id, item.quantity - 1)}>
                      -
                    </button>
                    <span>{item.quantity}</span>
                    <button onClick={() => updateQuantity(item.id, item.quantity + 1)}>
                      +
                    </button>
                  </div>
                  <div className="item-total">
                    ${(item.price * item.quantity).toFixed(2)}
                  </div>
                  <button 
                    className="remove-btn"
                    onClick={() => removeFromCart(item.id)}
                  >
                    Remove
                  </button>
                </div>
              ))}
            </div>
            <div className="cart-total">
              <h3>Total: ${getTotalPrice().toFixed(2)}</h3>
              <button className="checkout-btn">Checkout</button>
            </div>
          </>
        )}
      </div>
    </div>
  )
}

export default ShoppingCart
```

```css
/* ShoppingCart.css */
.shopping-cart {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 40px;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.product-card {
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
  text-align: center;
  background: white;
}

.product-card h3 {
  margin: 0 0 10px 0;
}

.price {
  font-size: 18px;
  font-weight: bold;
  color: #007bff;
  margin: 10px 0;
}

.product-card button {
  background: #28a745;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}

.cart-section {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
}

.cart-item {
  display: flex;
  align-items: center;
  padding: 15px;
  border-bottom: 1px solid #ddd;
  gap: 15px;
}

.item-info {
  flex: 1;
}

.item-info h4 {
  margin: 0 0 5px 0;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 10px;
}

.quantity-controls button {
  width: 30px;
  height: 30px;
  border: 1px solid #ddd;
  background: white;
  cursor: pointer;
}

.item-total {
  font-weight: bold;
  min-width: 80px;
  text-align: right;
}

.remove-btn {
  background: #dc3545;
  color: white;
  border: none;
  padding: 5px 10px;
  border-radius: 4px;
  cursor: pointer;
}

.cart-total {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 2px solid #007bff;
  text-align: right;
}

.checkout-btn {
  background: #007bff;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  margin-top: 10px;
}

@media (max-width: 768px) {
  .shopping-cart {
    grid-template-columns: 1fr;
  }
  
  .cart-item {
    flex-direction: column;
    align-items: stretch;
    gap: 10px;
  }
  
  .item-total {
    text-align: left;
  }
}
```

## 4. Best Practices

### 1. State Management Best Practices

```jsx
// ❌ Don't mutate state directly
const [items, setItems] = useState([1, 2, 3])
items.push(4) // Wrong!

// ✅ Always create new state
setItems([...items, 4]) // Correct!

// ❌ Don't mutate objects directly
const [user, setUser] = useState({ name: 'John', age: 25 })
user.age = 26 // Wrong!

// ✅ Create new object
setUser({ ...user, age: 26 }) // Correct!
```

### 2. Props Validation with TypeScript

```tsx
interface ComponentProps {
  title: string
  count: number
  items?: string[] // Optional
  onItemClick: (item: string) => void // Function prop
}

const MyComponent: React.FC<ComponentProps> = ({ 
  title, 
  count, 
  items = [], 
  onItemClick 
}) => {
  return (
    <div>
      <h1>{title}</h1>
      <p>Count: {count}</p>
      <ul>
        {items.map((item, index) => (
          <li key={index} onClick={() => onItemClick(item)}>
            {item}
          </li>
        ))}
      </ul>
    </div>
  )
}
```

### 3. Conditional Rendering

```jsx
function ConditionalExample({ user, isLoading, error }) {
  // Loading state
  if (isLoading) {
    return <div>Loading...</div>
  }
  
  // Error state
  if (error) {
    return <div>Error: {error.message}</div>
  }
  
  // Conditional rendering with logical AND
  return (
    <div>
      {user && <h1>Welcome, {user.name}!</h1>}
      {user?.isAdmin && <button>Admin Panel</button>}
      
      {/* Ternary operator */}
      {user ? (
        <UserProfile user={user} />
      ) : (
        <LoginForm />
      )}
    </div>
  )
}
```

## 5. Exercises

### Exercise 1: Temperature Converter
Create a component that converts between Celsius and Fahrenheit.

### Exercise 2: Todo List
Build a simple todo list with add, remove, and toggle complete functionality.

### Exercise 3: Form with Validation
Create a registration form with real-time validation.

### Exercise 4: Image Gallery
Build an image gallery with modal view and navigation.

## 6. Common Mistakes and Solutions

1. **Directly mutating state**
   - Always use setState function
   - Create new objects/arrays

2. **Not providing keys for lists**
   - Always provide unique keys for list items
   - Avoid using array index as key when possible

3. **Props drilling**
   - Consider using Context API for deeply nested props
   - Keep component hierarchy shallow when possible

## Next Lesson Preview

In Day 04, we'll explore:
- Event handling in depth
- Form management
- Controlled vs uncontrolled components
- Event delegation and performance

## Summary

Today you learned:
- ✅ How to pass data with props
- ✅ Managing component state with useState
- ✅ Building interactive components
- ✅ Best practices for state management
- ✅ Practical examples with real-world applications

Keep practicing with the exercises and you'll master React state and props!