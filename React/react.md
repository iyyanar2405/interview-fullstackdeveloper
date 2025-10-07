# 🚀 React Complete Tutorial: Beginner to Advanced

*A comprehensive step-by-step guide to mastering React from scratch*

---

## 📚 Table of Contents

1. [Introduction & Setup](#1-introduction--setup)
2. [React Fundamentals](#2-react-fundamentals)
3. [Components & JSX](#3-components--jsx)
4. [Props & State](#4-props--state)
5. [Event Handling](#5-event-handling)
6. [Conditional Rendering](#6-conditional-rendering)
7. [Lists & Keys](#7-lists--keys)
8. [Forms & Controlled Components](#8-forms--controlled-components)
9. [Component Lifecycle](#9-component-lifecycle)
10. [React Hooks](#10-react-hooks)
11. [Context API](#11-context-api)
12. [Routing](#12-routing)
13. [State Management](#13-state-management)
14. [Performance Optimization](#14-performance-optimization)
15. [Testing](#15-testing)
16. [Advanced Patterns](#16-advanced-patterns)
17. [Real-World Projects](#17-real-world-projects)

---

## 1. Introduction & Setup

### What is React?
React is a JavaScript library for building user interfaces, particularly web applications. It's component-based and uses a virtual DOM for efficient updates.

### Prerequisites
- Basic HTML, CSS, and JavaScript knowledge
- Understanding of ES6+ features (arrow functions, destructuring, modules)
- Node.js and npm installed

### Development Environment Setup

#### Step 1: Install Node.js
```bash
# Check if Node.js is installed
node --version
npm --version
```

#### Step 2: Create React App
```bash
# Create a new React application
npx create-react-app my-react-app
cd my-react-app

# Start development server
npm start
```

#### Step 3: Project Structure
```
my-react-app/
├── public/
│   ├── index.html
│   └── favicon.ico
├── src/
│   ├── App.js
│   ├── App.css
│   ├── index.js
│   └── index.css
├── package.json
└── README.md
```

#### Step 4: Essential VS Code Extensions
```json
{
  "recommendations": [
    "ms-vscode.vscode-typescript-next",
    "bradlc.vscode-tailwindcss",
    "esbenp.prettier-vscode",
    "ms-vscode.vscode-json"
  ]
}
```

---

## 2. React Fundamentals

### Virtual DOM Concept
```javascript
// Traditional DOM manipulation (slow)
document.getElementById('myDiv').innerHTML = 'Hello World';

// React Virtual DOM (fast)
// React creates a virtual representation and efficiently updates only what changed
```

### React Element vs Component
```javascript
// React Element (what JSX creates)
const element = <h1>Hello, world!</h1>;

// React Component (reusable piece of UI)
function Welcome(props) {
  return <h1>Hello, {props.name}!</h1>;
}
```

### Your First React App
```javascript
// src/index.js
import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(<App />);
```

```javascript
// src/App.js
function App() {
  return (
    <div className="App">
      <h1>Welcome to React!</h1>
      <p>Your journey starts here.</p>
    </div>
  );
}

export default App;
```

---

## 3. Components & JSX

### JSX Syntax Rules
```javascript
// ✅ Correct JSX
function MyComponent() {
  return (
    <div className="container">
      <h1>Title</h1>
      <p>Description</p>
    </div>
  );
}

// ❌ Incorrect JSX - Multiple elements without wrapper
function BadComponent() {
  return (
    <h1>Title</h1>
    <p>Description</p>
  );
}
```

### JavaScript Expressions in JSX
```javascript
function ExpressionComponent() {
  const name = "John";
  const age = 25;
  const isActive = true;

  return (
    <div>
      <h1>Hello, {name}!</h1>
      <p>Age: {age}</p>
      <p>Status: {isActive ? 'Active' : 'Inactive'}</p>
      <p>Next year: {age + 1}</p>
    </div>
  );
}
```

### Component Types

#### Functional Components (Recommended)
```javascript
// Simple functional component
function Greeting() {
  return <h1>Hello, World!</h1>;
}

// Arrow function component
const Greeting = () => {
  return <h1>Hello, World!</h1>;
};

// Implicit return for simple JSX
const Greeting = () => <h1>Hello, World!</h1>;
```

#### Class Components (Legacy)
```javascript
import React, { Component } from 'react';

class Greeting extends Component {
  render() {
    return <h1>Hello, World!</h1>;
  }
}
```

### Component Composition
```javascript
// Header component
function Header() {
  return (
    <header>
      <h1>My Website</h1>
      <nav>
        <a href="/">Home</a>
        <a href="/about">About</a>
      </nav>
    </header>
  );
}

// Main content component
function MainContent() {
  return (
    <main>
      <h2>Welcome to our website!</h2>
      <p>This is the main content area.</p>
    </main>
  );
}

// Footer component
function Footer() {
  return (
    <footer>
      <p>&copy; 2024 My Website. All rights reserved.</p>
    </footer>
  );
}

// App component using composition
function App() {
  return (
    <div className="App">
      <Header />
      <MainContent />
      <Footer />
    </div>
  );
}
```

---

## 4. Props & State

### Understanding Props
Props are read-only properties passed from parent to child components.

```javascript
// Child component receiving props
function UserCard(props) {
  return (
    <div className="user-card">
      <img src={props.avatar} alt={props.name} />
      <h3>{props.name}</h3>
      <p>{props.email}</p>
      <p>Age: {props.age}</p>
    </div>
  );
}

// Parent component passing props
function App() {
  return (
    <div>
      <UserCard 
        name="John Doe" 
        email="john@example.com" 
        age={30}
        avatar="https://example.com/avatar.jpg"
      />
      <UserCard 
        name="Jane Smith" 
        email="jane@example.com" 
        age={25}
        avatar="https://example.com/avatar2.jpg"
      />
    </div>
  );
}
```

### Props Destructuring
```javascript
// Without destructuring
function UserCard(props) {
  return (
    <div>
      <h3>{props.name}</h3>
      <p>{props.email}</p>
    </div>
  );
}

// With destructuring (cleaner)
function UserCard({ name, email, age, avatar }) {
  return (
    <div className="user-card">
      <img src={avatar} alt={name} />
      <h3>{name}</h3>
      <p>{email}</p>
      <p>Age: {age}</p>
    </div>
  );
}
```

### Default Props
```javascript
function Button({ text, color, size }) {
  return (
    <button className={`btn btn-${color} btn-${size}`}>
      {text}
    </button>
  );
}

// Setting default props
Button.defaultProps = {
  text: 'Click me',
  color: 'primary',
  size: 'medium'
};

// Or using default parameters
function Button({ 
  text = 'Click me', 
  color = 'primary', 
  size = 'medium' 
}) {
  return (
    <button className={`btn btn-${color} btn-${size}`}>
      {text}
    </button>
  );
}
```

### Props Validation with PropTypes
```javascript
import PropTypes from 'prop-types';

function UserCard({ name, email, age, isActive }) {
  return (
    <div className="user-card">
      <h3>{name}</h3>
      <p>{email}</p>
      <p>Age: {age}</p>
      <span>{isActive ? 'Active' : 'Inactive'}</span>
    </div>
  );
}

UserCard.propTypes = {
  name: PropTypes.string.isRequired,
  email: PropTypes.string.isRequired,
  age: PropTypes.number,
  isActive: PropTypes.bool
};
```

### State with useState Hook
```javascript
import React, { useState } from 'react';

function Counter() {
  // State declaration
  const [count, setCount] = useState(0);

  const increment = () => {
    setCount(count + 1);
  };

  const decrement = () => {
    setCount(count - 1);
  };

  const reset = () => {
    setCount(0);
  };

  return (
    <div className="counter">
      <h2>Counter: {count}</h2>
      <button onClick={increment}>+</button>
      <button onClick={decrement}>-</button>
      <button onClick={reset}>Reset</button>
    </div>
  );
}
```

### Multiple State Variables
```javascript
function UserProfile() {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [age, setAge] = useState(0);
  const [isSubscribed, setIsSubscribed] = useState(false);

  return (
    <div>
      <input 
        type="text" 
        placeholder="Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
      />
      <input 
        type="email" 
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />
      <input 
        type="number" 
        placeholder="Age"
        value={age}
        onChange={(e) => setAge(parseInt(e.target.value))}
      />
      <label>
        <input 
          type="checkbox" 
          checked={isSubscribed}
          onChange={(e) => setIsSubscribed(e.target.checked)}
        />
        Subscribe to newsletter
      </label>
      
      <div>
        <h3>Preview:</h3>
        <p>Name: {name}</p>
        <p>Email: {email}</p>
        <p>Age: {age}</p>
        <p>Subscribed: {isSubscribed ? 'Yes' : 'No'}</p>
      </div>
    </div>
  );
}
```

### State with Objects
```javascript
function UserForm() {
  const [user, setUser] = useState({
    name: '',
    email: '',
    age: 0,
    address: {
      street: '',
      city: '',
      zipCode: ''
    }
  });

  const updateUser = (field, value) => {
    setUser(prevUser => ({
      ...prevUser,
      [field]: value
    }));
  };

  const updateAddress = (field, value) => {
    setUser(prevUser => ({
      ...prevUser,
      address: {
        ...prevUser.address,
        [field]: value
      }
    }));
  };

  return (
    <form>
      <input 
        type="text"
        placeholder="Name"
        value={user.name}
        onChange={(e) => updateUser('name', e.target.value)}
      />
      <input 
        type="email"
        placeholder="Email"
        value={user.email}
        onChange={(e) => updateUser('email', e.target.value)}
      />
      <input 
        type="text"
        placeholder="Street"
        value={user.address.street}
        onChange={(e) => updateAddress('street', e.target.value)}
      />
      <input 
        type="text"
        placeholder="City"
        value={user.address.city}
        onChange={(e) => updateAddress('city', e.target.value)}
      />
    </form>
  );
}
```

---

## 5. Event Handling

### Basic Event Handling
```javascript
function EventHandlingDemo() {
  const handleClick = () => {
    alert('Button clicked!');
  };

  const handleMouseEnter = () => {
    console.log('Mouse entered!');
  };

  const handleInputChange = (event) => {
    console.log('Input value:', event.target.value);
  };

  return (
    <div>
      <button onClick={handleClick}>
        Click Me
      </button>
      
      <div onMouseEnter={handleMouseEnter}>
        Hover over me
      </div>
      
      <input 
        type="text" 
        onChange={handleInputChange}
        placeholder="Type something..."
      />
    </div>
  );
}
```

### Event Object and Parameters
```javascript
function EventObjectDemo() {
  const handleSubmit = (event) => {
    event.preventDefault(); // Prevent default form submission
    console.log('Form submitted!');
  };

  const handleButtonClick = (event, customMessage) => {
    event.stopPropagation(); // Stop event bubbling
    alert(customMessage);
  };

  const handleKeyPress = (event) => {
    if (event.key === 'Enter') {
      console.log('Enter key pressed!');
    }
  };

  return (
    <div onClick={() => console.log('Div clicked!')}>
      <form onSubmit={handleSubmit}>
        <input 
          type="text" 
          onKeyPress={handleKeyPress}
          placeholder="Press Enter..."
        />
        <button type="submit">Submit</button>
      </form>
      
      <button 
        onClick={(e) => handleButtonClick(e, 'Custom message!')}
      >
        Click with custom message
      </button>
    </div>
  );
}
```

### Dynamic Event Handlers
```javascript
function TodoList() {
  const [todos, setTodos] = useState([
    { id: 1, text: 'Learn React', completed: false },
    { id: 2, text: 'Build a project', completed: false }
  ]);

  const toggleTodo = (id) => {
    setTodos(prevTodos =>
      prevTodos.map(todo =>
        todo.id === id 
          ? { ...todo, completed: !todo.completed }
          : todo
      )
    );
  };

  const deleteTodo = (id) => {
    setTodos(prevTodos =>
      prevTodos.filter(todo => todo.id !== id)
    );
  };

  return (
    <ul>
      {todos.map(todo => (
        <li key={todo.id}>
          <span 
            style={{ 
              textDecoration: todo.completed ? 'line-through' : 'none' 
            }}
            onClick={() => toggleTodo(todo.id)}
          >
            {todo.text}
          </span>
          <button onClick={() => deleteTodo(todo.id)}>
            Delete
          </button>
        </li>
      ))}
    </ul>
  );
}
```

---

## 6. Conditional Rendering

### If-Else with Ternary Operator
```javascript
function Welcome({ user }) {
  return (
    <div>
      {user ? (
        <h1>Welcome back, {user.name}!</h1>
      ) : (
        <h1>Please sign in.</h1>
      )}
    </div>
  );
}
```

### Logical AND Operator
```javascript
function Notifications({ messages }) {
  return (
    <div>
      {messages.length > 0 && (
        <div>
          <h2>You have {messages.length} unread messages.</h2>
          <ul>
            {messages.map((message, index) => (
              <li key={index}>{message}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
```

### Complex Conditional Rendering
```javascript
function UserDashboard({ user, isLoading, error }) {
  // Loading state
  if (isLoading) {
    return <div>Loading...</div>;
  }

  // Error state
  if (error) {
    return <div>Error: {error.message}</div>;
  }

  // No user state
  if (!user) {
    return <div>No user found.</div>;
  }

  // Success state with nested conditions
  return (
    <div>
      <h1>Welcome, {user.name}!</h1>
      
      {user.isAdmin && (
        <div>
          <h2>Admin Panel</h2>
          <button>Manage Users</button>
        </div>
      )}
      
      {user.notifications && user.notifications.length > 0 ? (
        <div>
          <h3>Notifications</h3>
          {user.notifications.map(notification => (
            <div key={notification.id}>
              {notification.message}
            </div>
          ))}
        </div>
      ) : (
        <p>No new notifications.</p>
      )}
      
      {user.subscription === 'premium' ? (
        <div>
          <h3>Premium Features</h3>
          <button>Access Premium Content</button>
        </div>
      ) : (
        <div>
          <h3>Upgrade to Premium</h3>
          <button>Upgrade Now</button>
        </div>
      )}
    </div>
  );
}
```

### Switch-Case Alternative
```javascript
function StatusIndicator({ status }) {
  const renderStatus = () => {
    switch (status) {
      case 'loading':
        return <div className="spinner">Loading...</div>;
      case 'success':
        return <div className="success">✓ Success!</div>;
      case 'error':
        return <div className="error">✗ Error occurred</div>;
      case 'warning':
        return <div className="warning">⚠ Warning</div>;
      default:
        return <div>Unknown status</div>;
    }
  };

  return (
    <div className="status-indicator">
      {renderStatus()}
    </div>
  );
}
```

---

## 7. Lists & Keys

### Basic List Rendering
```javascript
function SimpleList() {
  const fruits = ['Apple', 'Banana', 'Orange', 'Mango'];

  return (
    <ul>
      {fruits.map((fruit, index) => (
        <li key={index}>{fruit}</li>
      ))}
    </ul>
  );
}
```

### List with Objects
```javascript
function UserList() {
  const users = [
    { id: 1, name: 'John Doe', email: 'john@example.com' },
    { id: 2, name: 'Jane Smith', email: 'jane@example.com' },
    { id: 3, name: 'Bob Johnson', email: 'bob@example.com' }
  ];

  return (
    <div>
      {users.map(user => (
        <div key={user.id} className="user-card">
          <h3>{user.name}</h3>
          <p>{user.email}</p>
        </div>
      ))}
    </div>
  );
}
```

### Why Keys Matter
```javascript
// ❌ Bad: Using array index as key (can cause issues)
function BadList({ items }) {
  return (
    <ul>
      {items.map((item, index) => (
        <li key={index}>{item.name}</li>
      ))}
    </ul>
  );
}

// ✅ Good: Using unique, stable identifiers
function GoodList({ items }) {
  return (
    <ul>
      {items.map(item => (
        <li key={item.id}>{item.name}</li>
      ))}
    </ul>
  );
}
```

### Dynamic List with CRUD Operations
```javascript
function TaskManager() {
  const [tasks, setTasks] = useState([
    { id: 1, text: 'Learn React', completed: false },
    { id: 2, text: 'Build a project', completed: true }
  ]);
  const [newTask, setNewTask] = useState('');

  const addTask = () => {
    if (newTask.trim()) {
      setTasks(prevTasks => [
        ...prevTasks,
        {
          id: Date.now(), // Simple ID generation
          text: newTask,
          completed: false
        }
      ]);
      setNewTask('');
    }
  };

  const toggleTask = (id) => {
    setTasks(prevTasks =>
      prevTasks.map(task =>
        task.id === id ? { ...task, completed: !task.completed } : task
      )
    );
  };

  const deleteTask = (id) => {
    setTasks(prevTasks => prevTasks.filter(task => task.id !== id));
  };

  const editTask = (id, newText) => {
    setTasks(prevTasks =>
      prevTasks.map(task =>
        task.id === id ? { ...task, text: newText } : task
      )
    );
  };

  return (
    <div>
      <div>
        <input
          type="text"
          value={newTask}
          onChange={(e) => setNewTask(e.target.value)}
          placeholder="Add new task..."
          onKeyPress={(e) => e.key === 'Enter' && addTask()}
        />
        <button onClick={addTask}>Add Task</button>
      </div>

      <ul>
        {tasks.map(task => (
          <TaskItem
            key={task.id}
            task={task}
            onToggle={() => toggleTask(task.id)}
            onDelete={() => deleteTask(task.id)}
            onEdit={(newText) => editTask(task.id, newText)}
          />
        ))}
      </ul>
    </div>
  );
}

function TaskItem({ task, onToggle, onDelete, onEdit }) {
  const [isEditing, setIsEditing] = useState(false);
  const [editText, setEditText] = useState(task.text);

  const handleSave = () => {
    onEdit(editText);
    setIsEditing(false);
  };

  const handleCancel = () => {
    setEditText(task.text);
    setIsEditing(false);
  };

  return (
    <li className={`task-item ${task.completed ? 'completed' : ''}`}>
      {isEditing ? (
        <div>
          <input
            type="text"
            value={editText}
            onChange={(e) => setEditText(e.target.value)}
          />
          <button onClick={handleSave}>Save</button>
          <button onClick={handleCancel}>Cancel</button>
        </div>
      ) : (
        <div>
          <input
            type="checkbox"
            checked={task.completed}
            onChange={onToggle}
          />
          <span onClick={onToggle}>{task.text}</span>
          <button onClick={() => setIsEditing(true)}>Edit</button>
          <button onClick={onDelete}>Delete</button>
        </div>
      )}
    </li>
  );
}
```

### Filtering and Sorting Lists
```javascript
function ProductList() {
  const [products] = useState([
    { id: 1, name: 'Laptop', category: 'Electronics', price: 999 },
    { id: 2, name: 'Shirt', category: 'Clothing', price: 29 },
    { id: 3, name: 'Phone', category: 'Electronics', price: 699 },
    { id: 4, name: 'Jeans', category: 'Clothing', price: 59 }
  ]);

  const [filter, setFilter] = useState('all');
  const [sortBy, setSortBy] = useState('name');

  const filteredProducts = products.filter(product => {
    if (filter === 'all') return true;
    return product.category.toLowerCase() === filter.toLowerCase();
  });

  const sortedProducts = [...filteredProducts].sort((a, b) => {
    if (sortBy === 'name') {
      return a.name.localeCompare(b.name);
    }
    if (sortBy === 'price') {
      return a.price - b.price;
    }
    return 0;
  });

  return (
    <div>
      <div className="controls">
        <select value={filter} onChange={(e) => setFilter(e.target.value)}>
          <option value="all">All Categories</option>
          <option value="electronics">Electronics</option>
          <option value="clothing">Clothing</option>
        </select>

        <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
          <option value="name">Sort by Name</option>
          <option value="price">Sort by Price</option>
        </select>
      </div>

      <div className="product-grid">
        {sortedProducts.map(product => (
          <div key={product.id} className="product-card">
            <h3>{product.name}</h3>
            <p>Category: {product.category}</p>
            <p>Price: ${product.price}</p>
          </div>
        ))}
      </div>
    </div>
  );
}
```

---

## 8. Forms & Controlled Components

### Basic Controlled Input
```javascript
function SimpleForm() {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');

  const handleSubmit = (event) => {
    event.preventDefault();
    console.log('Form submitted:', { name, email });
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>
          Name:
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </label>
      </div>
      <div>
        <label>
          Email:
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </label>
      </div>
      <button type="submit">Submit</button>
    </form>
  );
}
```

### Complex Form with Validation
```javascript
function RegistrationForm() {
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    age: '',
    gender: '',
    interests: [],
    newsletter: false
  });

  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleInputChange = (event) => {
    const { name, value, type, checked } = event.target;
    
    setFormData(prevData => ({
      ...prevData,
      [name]: type === 'checkbox' ? checked : value
    }));

    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prevErrors => ({
        ...prevErrors,
        [name]: ''
      }));
    }
  };

  const handleInterestChange = (event) => {
    const { value, checked } = event.target;
    
    setFormData(prevData => ({
      ...prevData,
      interests: checked
        ? [...prevData.interests, value]
        : prevData.interests.filter(interest => interest !== value)
    }));
  };

  const validateForm = () => {
    const newErrors = {};

    // Username validation
    if (!formData.username.trim()) {
      newErrors.username = 'Username is required';
    } else if (formData.username.length < 3) {
      newErrors.username = 'Username must be at least 3 characters';
    }

    // Email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!formData.email) {
      newErrors.email = 'Email is required';
    } else if (!emailRegex.test(formData.email)) {
      newErrors.email = 'Please enter a valid email';
    }

    // Password validation
    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }

    // Confirm password validation
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }

    // Age validation
    if (!formData.age) {
      newErrors.age = 'Age is required';
    } else if (formData.age < 13) {
      newErrors.age = 'Must be at least 13 years old';
    }

    return newErrors;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    
    const formErrors = validateForm();
    
    if (Object.keys(formErrors).length > 0) {
      setErrors(formErrors);
      return;
    }

    setIsSubmitting(true);
    
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 2000));
      console.log('Form submitted successfully:', formData);
      alert('Registration successful!');
      
      // Reset form
      setFormData({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        age: '',
        gender: '',
        interests: [],
        newsletter: false
      });
    } catch (error) {
      console.error('Submission error:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="registration-form">
      <div className="form-group">
        <label>
          Username:
          <input
            type="text"
            name="username"
            value={formData.username}
            onChange={handleInputChange}
            className={errors.username ? 'error' : ''}
          />
          {errors.username && <span className="error-message">{errors.username}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Email:
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleInputChange}
            className={errors.email ? 'error' : ''}
          />
          {errors.email && <span className="error-message">{errors.email}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Password:
          <input
            type="password"
            name="password"
            value={formData.password}
            onChange={handleInputChange}
            className={errors.password ? 'error' : ''}
          />
          {errors.password && <span className="error-message">{errors.password}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Confirm Password:
          <input
            type="password"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleInputChange}
            className={errors.confirmPassword ? 'error' : ''}
          />
          {errors.confirmPassword && <span className="error-message">{errors.confirmPassword}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Age:
          <input
            type="number"
            name="age"
            value={formData.age}
            onChange={handleInputChange}
            className={errors.age ? 'error' : ''}
          />
          {errors.age && <span className="error-message">{errors.age}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Gender:
          <select
            name="gender"
            value={formData.gender}
            onChange={handleInputChange}
          >
            <option value="">Select Gender</option>
            <option value="male">Male</option>
            <option value="female">Female</option>
            <option value="other">Other</option>
          </select>
        </label>
      </div>

      <div className="form-group">
        <fieldset>
          <legend>Interests:</legend>
          {['Technology', 'Sports', 'Music', 'Travel', 'Reading'].map(interest => (
            <label key={interest} className="checkbox-label">
              <input
                type="checkbox"
                value={interest}
                checked={formData.interests.includes(interest)}
                onChange={handleInterestChange}
              />
              {interest}
            </label>
          ))}
        </fieldset>
      </div>

      <div className="form-group">
        <label className="checkbox-label">
          <input
            type="checkbox"
            name="newsletter"
            checked={formData.newsletter}
            onChange={handleInputChange}
          />
          Subscribe to newsletter
        </label>
      </div>

      <button 
        type="submit" 
        disabled={isSubmitting}
        className={isSubmitting ? 'loading' : ''}
      >
        {isSubmitting ? 'Registering...' : 'Register'}
      </button>
    </form>
  );
}
```

### File Upload Component
```javascript
function FileUpload() {
  const [selectedFiles, setSelectedFiles] = useState([]);
  const [uploadProgress, setUploadProgress] = useState({});
  const [uploadedFiles, setUploadedFiles] = useState([]);

  const handleFileSelect = (event) => {
    const files = Array.from(event.target.files);
    setSelectedFiles(files);
  };

  const handleUpload = async () => {
    for (const file of selectedFiles) {
      try {
        // Simulate file upload with progress
        setUploadProgress(prev => ({ ...prev, [file.name]: 0 }));
        
        for (let progress = 0; progress <= 100; progress += 10) {
          await new Promise(resolve => setTimeout(resolve, 100));
          setUploadProgress(prev => ({ ...prev, [file.name]: progress }));
        }

        setUploadedFiles(prev => [...prev, {
          name: file.name,
          size: file.size,
          type: file.type,
          url: URL.createObjectURL(file)
        }]);

      } catch (error) {
        console.error('Upload failed:', error);
      }
    }
    
    setSelectedFiles([]);
    setUploadProgress({});
  };

  const handleRemoveFile = (fileName) => {
    setUploadedFiles(prev => prev.filter(file => file.name !== fileName));
  };

  return (
    <div className="file-upload">
      <div className="upload-area">
        <input
          type="file"
          multiple
          onChange={handleFileSelect}
          accept="image/*,.pdf,.doc,.docx"
        />
        
        {selectedFiles.length > 0 && (
          <div>
            <h4>Selected Files:</h4>
            <ul>
              {selectedFiles.map((file, index) => (
                <li key={index}>
                  {file.name} ({(file.size / 1024 / 1024).toFixed(2)} MB)
                  {uploadProgress[file.name] !== undefined && (
                    <div className="progress-bar">
                      <div 
                        className="progress-fill"
                        style={{ width: `${uploadProgress[file.name]}%` }}
                      />
                    </div>
                  )}
                </li>
              ))}
            </ul>
            <button onClick={handleUpload}>Upload Files</button>
          </div>
        )}
      </div>

      {uploadedFiles.length > 0 && (
        <div className="uploaded-files">
          <h4>Uploaded Files:</h4>
          <div className="file-grid">
            {uploadedFiles.map((file, index) => (
              <div key={index} className="file-card">
                {file.type.startsWith('image/') && (
                  <img src={file.url} alt={file.name} className="file-preview" />
                )}
                <div className="file-info">
                  <p>{file.name}</p>
                  <p>{(file.size / 1024 / 1024).toFixed(2)} MB</p>
                  <button onClick={() => handleRemoveFile(file.name)}>
                    Remove
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
```

---

## 9. Component Lifecycle

### Lifecycle with useEffect Hook
```javascript
import React, { useState, useEffect } from 'react';

function LifecycleDemo() {
  const [count, setCount] = useState(0);
  const [data, setData] = useState(null);
  const [windowWidth, setWindowWidth] = useState(window.innerWidth);

  // Mounting (component did mount)
  useEffect(() => {
    console.log('Component mounted');
    
    // Fetch initial data
    fetchData();
    
    // Cleanup function (component will unmount)
    return () => {
      console.log('Component will unmount');
    };
  }, []); // Empty dependency array = runs once on mount

  // Updating (component did update)
  useEffect(() => {
    console.log('Count updated:', count);
    
    // Update document title
    document.title = `Count: ${count}`;
  }, [count]); // Runs when count changes

  // Window resize listener
  useEffect(() => {
    const handleResize = () => {
      setWindowWidth(window.innerWidth);
    };

    window.addEventListener('resize', handleResize);

    // Cleanup
    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  const fetchData = async () => {
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      setData({ message: 'Data loaded successfully!' });
    } catch (error) {
      console.error('Failed to fetch data:', error);
    }
  };

  return (
    <div>
      <h2>Lifecycle Demo</h2>
      <p>Count: {count}</p>
      <p>Window Width: {windowWidth}px</p>
      
      <button onClick={() => setCount(count + 1)}>
        Increment Count
      </button>
      
      {data ? (
        <p>{data.message}</p>
      ) : (
        <p>Loading data...</p>
      )}
    </div>
  );
}
```

### Data Fetching with useEffect
```javascript
function UserProfile({ userId }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let isCancelled = false; // Flag to prevent state updates if component unmounts

    const fetchUser = async () => {
      try {
        setLoading(true);
        setError(null);
        
        const response = await fetch(`/api/users/${userId}`);
        
        if (!response.ok) {
          throw new Error('Failed to fetch user');
        }
        
        const userData = await response.json();
        
        // Only update state if component is still mounted
        if (!isCancelled) {
          setUser(userData);
        }
      } catch (err) {
        if (!isCancelled) {
          setError(err.message);
        }
      } finally {
        if (!isCancelled) {
          setLoading(false);
        }
      }
    };

    if (userId) {
      fetchUser();
    }

    // Cleanup function
    return () => {
      isCancelled = true;
    };
  }, [userId]); // Re-run when userId changes

  if (loading) return <div>Loading user...</div>;
  if (error) return <div>Error: {error}</div>;
  if (!user) return <div>No user found</div>;

  return (
    <div className="user-profile">
      <h2>{user.name}</h2>
      <p>Email: {user.email}</p>
      <p>Joined: {new Date(user.joinDate).toLocaleDateString()}</p>
    </div>
  );
}
```

### Timer and Interval Example
```javascript
function Timer() {
  const [seconds, setSeconds] = useState(0);
  const [isRunning, setIsRunning] = useState(false);

  useEffect(() => {
    let interval = null;

    if (isRunning) {
      interval = setInterval(() => {
        setSeconds(prevSeconds => prevSeconds + 1);
      }, 1000);
    }

    return () => {
      if (interval) {
        clearInterval(interval);
      }
    };
  }, [isRunning]);

  const startTimer = () => setIsRunning(true);
  const stopTimer = () => setIsRunning(false);
  const resetTimer = () => {
    setIsRunning(false);
    setSeconds(0);
  };

  const formatTime = (totalSeconds) => {
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const secs = totalSeconds % 60;
    
    return `${hours.toString().padStart(2, '0')}:${minutes
      .toString()
      .padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  };

  return (
    <div className="timer">
      <h2>Timer</h2>
      <div className="time-display">{formatTime(seconds)}</div>
      
      <div className="timer-controls">
        <button onClick={startTimer} disabled={isRunning}>
          Start
        </button>
        <button onClick={stopTimer} disabled={!isRunning}>
          Stop
        </button>
        <button onClick={resetTimer}>Reset</button>
      </div>
    </div>
  );
}
```

---

## 10. React Hooks

### useState Hook Deep Dive
```javascript
function StateExamples() {
  // Basic state
  const [count, setCount] = useState(0);
  
  // State with object
  const [user, setUser] = useState({
    name: '',
    email: '',
    preferences: {
      theme: 'light',
      notifications: true
    }
  });

  // State with array
  const [items, setItems] = useState([]);

  // Lazy initial state (expensive calculation)
  const [expensiveValue, setExpensiveValue] = useState(() => {
    console.log('Computing expensive initial value...');
    return Array.from({ length: 1000 }, (_, i) => i).reduce((sum, i) => sum + i, 0);
  });

  // Functional updates
  const incrementCount = () => {
    setCount(prevCount => prevCount + 1);
  };

  const updateUser = (field, value) => {
    setUser(prevUser => ({
      ...prevUser,
      [field]: value
    }));
  };

  const updatePreferences = (key, value) => {
    setUser(prevUser => ({
      ...prevUser,
      preferences: {
        ...prevUser.preferences,
        [key]: value
      }
    }));
  };

  const addItem = (item) => {
    setItems(prevItems => [...prevItems, item]);
  };

  const removeItem = (index) => {
    setItems(prevItems => prevItems.filter((_, i) => i !== index));
  };

  return (
    <div>
      <h3>State Examples</h3>
      
      <div>
        <h4>Counter: {count}</h4>
        <button onClick={incrementCount}>Increment</button>
      </div>

      <div>
        <h4>User Info</h4>
        <input
          placeholder="Name"
          value={user.name}
          onChange={(e) => updateUser('name', e.target.value)}
        />
        <input
          placeholder="Email"
          value={user.email}
          onChange={(e) => updateUser('email', e.target.value)}
        />
        
        <label>
          <input
            type="checkbox"
            checked={user.preferences.notifications}
            onChange={(e) => updatePreferences('notifications', e.target.checked)}
          />
          Enable Notifications
        </label>
      </div>

      <div>
        <h4>Expensive Value: {expensiveValue}</h4>
      </div>
    </div>
  );
}
```

### useEffect Hook Advanced Patterns
```javascript
function AdvancedEffects() {
  const [data, setData] = useState(null);
  const [query, setQuery] = useState('');
  const [debouncedQuery, setDebouncedQuery] = useState('');

  // Debounced search effect
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedQuery(query);
    }, 500);

    return () => clearTimeout(timer);
  }, [query]);

  // Search effect that runs when debounced query changes
  useEffect(() => {
    if (!debouncedQuery) return;

    const searchData = async () => {
      try {
        const response = await fetch(`/api/search?q=${debouncedQuery}`);
        const results = await response.json();
        setData(results);
      } catch (error) {
        console.error('Search failed:', error);
      }
    };

    searchData();
  }, [debouncedQuery]);

  // Cleanup effect for subscriptions
  useEffect(() => {
    const subscription = subscribeToUpdates((update) => {
      console.log('Received update:', update);
    });

    return () => subscription.unsubscribe();
  }, []);

  return (
    <div>
      <input
        type="text"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="Search..."
      />
      
      {data && (
        <div>
          <h4>Search Results:</h4>
          <pre>{JSON.stringify(data, null, 2)}</pre>
        </div>
      )}
    </div>
  );
}

// Mock subscription function
function subscribeToUpdates(callback) {
  const interval = setInterval(() => {
    callback({ timestamp: Date.now(), data: 'Update data' });
  }, 5000);

  return {
    unsubscribe: () => clearInterval(interval)
  };
}
```

### Custom Hooks
```javascript
// Custom hook for localStorage
function useLocalStorage(key, initialValue) {
  const [storedValue, setStoredValue] = useState(() => {
    try {
      const item = window.localStorage.getItem(key);
      return item ? JSON.parse(item) : initialValue;
    } catch (error) {
      console.error(`Error reading localStorage key "${key}":`, error);
      return initialValue;
    }
  });

  const setValue = (value) => {
    try {
      const valueToStore = value instanceof Function ? value(storedValue) : value;
      setStoredValue(valueToStore);
      window.localStorage.setItem(key, JSON.stringify(valueToStore));
    } catch (error) {
      console.error(`Error setting localStorage key "${key}":`, error);
    }
  };

  return [storedValue, setValue];
}

// Custom hook for API calls
function useApi(url) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const refetch = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch(url);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const result = await response.json();
      setData(result);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [url]);

  useEffect(() => {
    refetch();
  }, [refetch]);

  return { data, loading, error, refetch };
}

// Custom hook for form handling
function useForm(initialValues, validationSchema) {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});

  const handleChange = (event) => {
    const { name, value, type, checked } = event.target;
    
    setValues(prevValues => ({
      ...prevValues,
      [name]: type === 'checkbox' ? checked : value
    }));

    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prevErrors => ({
        ...prevErrors,
        [name]: ''
      }));
    }
  };

  const handleBlur = (event) => {
    const { name } = event.target;
    setTouched(prevTouched => ({
      ...prevTouched,
      [name]: true
    }));

    // Validate field on blur if validation schema exists
    if (validationSchema && validationSchema[name]) {
      const error = validationSchema[name](values[name], values);
      if (error) {
        setErrors(prevErrors => ({
          ...prevErrors,
          [name]: error
        }));
      }
    }
  };

  const validate = () => {
    if (!validationSchema) return true;

    const newErrors = {};
    let isValid = true;

    Object.keys(validationSchema).forEach(field => {
      const error = validationSchema[field](values[field], values);
      if (error) {
        newErrors[field] = error;
        isValid = false;
      }
    });

    setErrors(newErrors);
    return isValid;
  };

  const reset = () => {
    setValues(initialValues);
    setErrors({});
    setTouched({});
  };

  return {
    values,
    errors,
    touched,
    handleChange,
    handleBlur,
    validate,
    reset
  };
}

// Usage examples
function HookExamples() {
  // Using localStorage hook
  const [name, setName] = useLocalStorage('userName', '');
  
  // Using API hook
  const { data: posts, loading, error, refetch } = useApi('/api/posts');
  
  // Using form hook
  const form = useForm(
    { email: '', password: '' },
    {
      email: (value) => {
        if (!value) return 'Email is required';
        if (!/\S+@\S+\.\S+/.test(value)) return 'Email is invalid';
        return '';
      },
      password: (value) => {
        if (!value) return 'Password is required';
        if (value.length < 6) return 'Password must be at least 6 characters';
        return '';
      }
    }
  );

  const handleSubmit = (event) => {
    event.preventDefault();
    if (form.validate()) {
      console.log('Form is valid:', form.values);
    }
  };

  return (
    <div>
      <h3>Custom Hook Examples</h3>
      
      {/* localStorage example */}
      <div>
        <h4>Name (stored in localStorage):</h4>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Enter your name"
        />
        <p>Hello, {name}!</p>
      </div>

      {/* API hook example */}
      <div>
        <h4>Posts from API:</h4>
        {loading && <p>Loading...</p>}
        {error && <p>Error: {error}</p>}
        {posts && (
          <div>
            <button onClick={refetch}>Refresh</button>
            <ul>
              {posts.map(post => (
                <li key={post.id}>{post.title}</li>
              ))}
            </ul>
          </div>
        )}
      </div>

      {/* Form hook example */}
      <div>
        <h4>Login Form:</h4>
        <form onSubmit={handleSubmit}>
          <div>
            <input
              type="email"
              name="email"
              placeholder="Email"
              value={form.values.email}
              onChange={form.handleChange}
              onBlur={form.handleBlur}
            />
            {form.touched.email && form.errors.email && (
              <span className="error">{form.errors.email}</span>
            )}
          </div>
          
          <div>
            <input
              type="password"
              name="password"
              placeholder="Password"
              value={form.values.password}
              onChange={form.handleChange}
              onBlur={form.handleBlur}
            />
            {form.touched.password && form.errors.password && (
              <span className="error">{form.errors.password}</span>
            )}
          </div>
          
          <button type="submit">Login</button>
          <button type="button" onClick={form.reset}>Reset</button>
        </form>
      </div>
    </div>
  );
}
```

### useCallback and useMemo
```javascript
function PerformanceOptimization() {
  const [count, setCount] = useState(0);
  const [items, setItems] = useState([]);
  const [filter, setFilter] = useState('');

  // Expensive calculation - only recalculate when items change
  const expensiveValue = useMemo(() => {
    console.log('Calculating expensive value...');
    return items.reduce((sum, item) => sum + item.value, 0);
  }, [items]);

  // Filtered items - only recalculate when items or filter change
  const filteredItems = useMemo(() => {
    console.log('Filtering items...');
    return items.filter(item =>
      item.name.toLowerCase().includes(filter.toLowerCase())
    );
  }, [items, filter]);

  // Memoized callback - prevent unnecessary re-renders of child components
  const handleAddItem = useCallback(() => {
    const newItem = {
      id: Date.now(),
      name: `Item ${items.length + 1}`,
      value: Math.floor(Math.random() * 100)
    };
    setItems(prevItems => [...prevItems, newItem]);
  }, [items.length]);

  // Another memoized callback
  const handleRemoveItem = useCallback((id) => {
    setItems(prevItems => prevItems.filter(item => item.id !== id));
  }, []);

  return (
    <div>
      <h3>Performance Optimization</h3>
      
      <div>
        <p>Count: {count}</p>
        <button onClick={() => setCount(count + 1)}>Increment Count</button>
      </div>

      <div>
        <p>Expensive Value: {expensiveValue}</p>
        <input
          type="text"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          placeholder="Filter items..."
        />
        <button onClick={handleAddItem}>Add Item</button>
      </div>

      <div>
        <h4>Filtered Items ({filteredItems.length}):</h4>
        <ItemList items={filteredItems} onRemoveItem={handleRemoveItem} />
      </div>
    </div>
  );
}

// Memoized child component
const ItemList = React.memo(({ items, onRemoveItem }) => {
  console.log('ItemList rendered');
  
  return (
    <ul>
      {items.map(item => (
        <ItemRow
          key={item.id}
          item={item}
          onRemove={() => onRemoveItem(item.id)}
        />
      ))}
    </ul>
  );
});

const ItemRow = React.memo(({ item, onRemove }) => {
  console.log(`ItemRow ${item.id} rendered`);
  
  return (
    <li>
      {item.name} (Value: {item.value})
      <button onClick={onRemove}>Remove</button>
    </li>
  );
});
```

---

## 11. Context API

### Basic Context Setup
```javascript
// Create context
const ThemeContext = React.createContext();

// Theme provider component
function ThemeProvider({ children }) {
  const [theme, setTheme] = useState('light');

  const toggleTheme = () => {
    setTheme(prevTheme => prevTheme === 'light' ? 'dark' : 'light');
  };

  const value = {
    theme,
    toggleTheme
  };

  return (
    <ThemeContext.Provider value={value}>
      {children}
    </ThemeContext.Provider>
  );
}

// Custom hook to use theme context
function useTheme() {
  const context = useContext(ThemeContext);
  
  if (!context) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  
  return context;
}

// Component using the context
function ThemedButton() {
  const { theme, toggleTheme } = useTheme();

  return (
    <button
      onClick={toggleTheme}
      style={{
        backgroundColor: theme === 'light' ? '#fff' : '#333',
        color: theme === 'light' ? '#333' : '#fff',
        border: `1px solid ${theme === 'light' ? '#333' : '#fff'}`
      }}
    >
      Current theme: {theme}
    </button>
  );
}

// App component
function App() {
  return (
    <ThemeProvider>
      <div>
        <h1>Context API Example</h1>
        <ThemedButton />
      </div>
    </ThemeProvider>
  );
}
```

### Complex State Management with Context
```javascript
// User context for authentication
const UserContext = React.createContext();

function UserProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Check for existing user session
    checkAuthStatus();
  }, []);

  const checkAuthStatus = async () => {
    try {
      const token = localStorage.getItem('token');
      if (token) {
        const response = await fetch('/api/auth/me', {
          headers: { Authorization: `Bearer ${token}` }
        });
        
        if (response.ok) {
          const userData = await response.json();
          setUser(userData);
        } else {
          localStorage.removeItem('token');
        }
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const login = async (credentials) => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials)
      });

      if (!response.ok) {
        throw new Error('Login failed');
      }

      const { user: userData, token } = await response.json();
      
      localStorage.setItem('token', token);
      setUser(userData);
      
      return { success: true };
    } catch (err) {
      setError(err.message);
      return { success: false, error: err.message };
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  const updateProfile = async (profileData) => {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch('/api/auth/profile', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(profileData)
      });

      if (!response.ok) {
        throw new Error('Profile update failed');
      }

      const updatedUser = await response.json();
      setUser(updatedUser);
      
      return { success: true };
    } catch (err) {
      setError(err.message);
      return { success: false, error: err.message };
    }
  };

  const value = {
    user,
    loading,
    error,
    login,
    logout,
    updateProfile,
    isAuthenticated: !!user
  };

  return (
    <UserContext.Provider value={value}>
      {children}
    </UserContext.Provider>
  );
}

// Hook to use user context
function useUser() {
  const context = useContext(UserContext);
  
  if (!context) {
    throw new Error('useUser must be used within a UserProvider');// filepath: c:\POC\Full-stack-developer\interview-fullstackdeveloper\React-Complete-Tutorial-Beginner-to-Advanced.md
# 🚀 React Complete Tutorial: Beginner to Advanced

*A comprehensive step-by-step guide to mastering React from scratch*

---

## 📚 Table of Contents

1. [Introduction & Setup](#1-introduction--setup)
2. [React Fundamentals](#2-react-fundamentals)
3. [Components & JSX](#3-components--jsx)
4. [Props & State](#4-props--state)
5. [Event Handling](#5-event-handling)
6. [Conditional Rendering](#6-conditional-rendering)
7. [Lists & Keys](#7-lists--keys)
8. [Forms & Controlled Components](#8-forms--controlled-components)
9. [Component Lifecycle](#9-component-lifecycle)
10. [React Hooks](#10-react-hooks)
11. [Context API](#11-context-api)
12. [Routing](#12-routing)
13. [State Management](#13-state-management)
14. [Performance Optimization](#14-performance-optimization)
15. [Testing](#15-testing)
16. [Advanced Patterns](#16-advanced-patterns)
17. [Real-World Projects](#17-real-world-projects)

---

## 1. Introduction & Setup

### What is React?
React is a JavaScript library for building user interfaces, particularly web applications. It's component-based and uses a virtual DOM for efficient updates.

### Prerequisites
- Basic HTML, CSS, and JavaScript knowledge
- Understanding of ES6+ features (arrow functions, destructuring, modules)
- Node.js and npm installed

### Development Environment Setup

#### Step 1: Install Node.js
```bash
# Check if Node.js is installed
node --version
npm --version
```

#### Step 2: Create React App
```bash
# Create a new React application
npx create-react-app my-react-app
cd my-react-app

# Start development server
npm start
```

#### Step 3: Project Structure
```
my-react-app/
├── public/
│   ├── index.html
│   └── favicon.ico
├── src/
│   ├── App.js
│   ├── App.css
│   ├── index.js
│   └── index.css
├── package.json
└── README.md
```

#### Step 4: Essential VS Code Extensions
```json
{
  "recommendations": [
    "ms-vscode.vscode-typescript-next",
    "bradlc.vscode-tailwindcss",
    "esbenp.prettier-vscode",
    "ms-vscode.vscode-json"
  ]
}
```

---

## 2. React Fundamentals

### Virtual DOM Concept
```javascript
// Traditional DOM manipulation (slow)
document.getElementById('myDiv').innerHTML = 'Hello World';

// React Virtual DOM (fast)
// React creates a virtual representation and efficiently updates only what changed
```

### React Element vs Component
```javascript
// React Element (what JSX creates)
const element = <h1>Hello, world!</h1>;

// React Component (reusable piece of UI)
function Welcome(props) {
  return <h1>Hello, {props.name}!</h1>;
}
```

### Your First React App
```javascript
// src/index.js
import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(<App />);
```

```javascript
// src/App.js
function App() {
  return (
    <div className="App">
      <h1>Welcome to React!</h1>
      <p>Your journey starts here.</p>
    </div>
  );
}

export default App;
```

---

## 3. Components & JSX

### JSX Syntax Rules
```javascript
// ✅ Correct JSX
function MyComponent() {
  return (
    <div className="container">
      <h1>Title</h1>
      <p>Description</p>
    </div>
  );
}

// ❌ Incorrect JSX - Multiple elements without wrapper
function BadComponent() {
  return (
    <h1>Title</h1>
    <p>Description</p>
  );
}
```

### JavaScript Expressions in JSX
```javascript
function ExpressionComponent() {
  const name = "John";
  const age = 25;
  const isActive = true;

  return (
    <div>
      <h1>Hello, {name}!</h1>
      <p>Age: {age}</p>
      <p>Status: {isActive ? 'Active' : 'Inactive'}</p>
      <p>Next year: {age + 1}</p>
    </div>
  );
}
```

### Component Types

#### Functional Components (Recommended)
```javascript
// Simple functional component
function Greeting() {
  return <h1>Hello, World!</h1>;
}

// Arrow function component
const Greeting = () => {
  return <h1>Hello, World!</h1>;
};

// Implicit return for simple JSX
const Greeting = () => <h1>Hello, World!</h1>;
```

#### Class Components (Legacy)
```javascript
import React, { Component } from 'react';

class Greeting extends Component {
  render() {
    return <h1>Hello, World!</h1>;
  }
}
```

### Component Composition
```javascript
// Header component
function Header() {
  return (
    <header>
      <h1>My Website</h1>
      <nav>
        <a href="/">Home</a>
        <a href="/about">About</a>
      </nav>
    </header>
  );
}

// Main content component
function MainContent() {
  return (
    <main>
      <h2>Welcome to our website!</h2>
      <p>This is the main content area.</p>
    </main>
  );
}

// Footer component
function Footer() {
  return (
    <footer>
      <p>&copy; 2024 My Website. All rights reserved.</p>
    </footer>
  );
}

// App component using composition
function App() {
  return (
    <div className="App">
      <Header />
      <MainContent />
      <Footer />
    </div>
  );
}
```

---

## 4. Props & State

### Understanding Props
Props are read-only properties passed from parent to child components.

```javascript
// Child component receiving props
function UserCard(props) {
  return (
    <div className="user-card">
      <img src={props.avatar} alt={props.name} />
      <h3>{props.name}</h3>
      <p>{props.email}</p>
      <p>Age: {props.age}</p>
    </div>
  );
}

// Parent component passing props
function App() {
  return (
    <div>
      <UserCard 
        name="John Doe" 
        email="john@example.com" 
        age={30}
        avatar="https://example.com/avatar.jpg"
      />
      <UserCard 
        name="Jane Smith" 
        email="jane@example.com" 
        age={25}
        avatar="https://example.com/avatar2.jpg"
      />
    </div>
  );
}
```

### Props Destructuring
```javascript
// Without destructuring
function UserCard(props) {
  return (
    <div>
      <h3>{props.name}</h3>
      <p>{props.email}</p>
    </div>
  );
}

// With destructuring (cleaner)
function UserCard({ name, email, age, avatar }) {
  return (
    <div className="user-card">
      <img src={avatar} alt={name} />
      <h3>{name}</h3>
      <p>{email}</p>
      <p>Age: {age}</p>
    </div>
  );
}
```

### Default Props
```javascript
function Button({ text, color, size }) {
  return (
    <button className={`btn btn-${color} btn-${size}`}>
      {text}
    </button>
  );
}

// Setting default props
Button.defaultProps = {
  text: 'Click me',
  color: 'primary',
  size: 'medium'
};

// Or using default parameters
function Button({ 
  text = 'Click me', 
  color = 'primary', 
  size = 'medium' 
}) {
  return (
    <button className={`btn btn-${color} btn-${size}`}>
      {text}
    </button>
  );
}
```

### Props Validation with PropTypes
```javascript
import PropTypes from 'prop-types';

function UserCard({ name, email, age, isActive }) {
  return (
    <div className="user-card">
      <h3>{name}</h3>
      <p>{email}</p>
      <p>Age: {age}</p>
      <span>{isActive ? 'Active' : 'Inactive'}</span>
    </div>
  );
}

UserCard.propTypes = {
  name: PropTypes.string.isRequired,
  email: PropTypes.string.isRequired,
  age: PropTypes.number,
  isActive: PropTypes.bool
};
```

### State with useState Hook
```javascript
import React, { useState } from 'react';

function Counter() {
  // State declaration
  const [count, setCount] = useState(0);

  const increment = () => {
    setCount(count + 1);
  };

  const decrement = () => {
    setCount(count - 1);
  };

  const reset = () => {
    setCount(0);
  };

  return (
    <div className="counter">
      <h2>Counter: {count}</h2>
      <button onClick={increment}>+</button>
      <button onClick={decrement}>-</button>
      <button onClick={reset}>Reset</button>
    </div>
  );
}
```

### Multiple State Variables
```javascript
function UserProfile() {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [age, setAge] = useState(0);
  const [isSubscribed, setIsSubscribed] = useState(false);

  return (
    <div>
      <input 
        type="text" 
        placeholder="Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
      />
      <input 
        type="email" 
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />
      <input 
        type="number" 
        placeholder="Age"
        value={age}
        onChange={(e) => setAge(parseInt(e.target.value))}
      />
      <label>
        <input 
          type="checkbox" 
          checked={isSubscribed}
          onChange={(e) => setIsSubscribed(e.target.checked)}
        />
        Subscribe to newsletter
      </label>
      
      <div>
        <h3>Preview:</h3>
        <p>Name: {name}</p>
        <p>Email: {email}</p>
        <p>Age: {age}</p>
        <p>Subscribed: {isSubscribed ? 'Yes' : 'No'}</p>
      </div>
    </div>
  );
}
```

### State with Objects
```javascript
function UserForm() {
  const [user, setUser] = useState({
    name: '',
    email: '',
    age: 0,
    address: {
      street: '',
      city: '',
      zipCode: ''
    }
  });

  const updateUser = (field, value) => {
    setUser(prevUser => ({
      ...prevUser,
      [field]: value
    }));
  };

  const updateAddress = (field, value) => {
    setUser(prevUser => ({
      ...prevUser,
      address: {
        ...prevUser.address,
        [field]: value
      }
    }));
  };

  return (
    <form>
      <input 
        type="text"
        placeholder="Name"
        value={user.name}
        onChange={(e) => updateUser('name', e.target.value)}
      />
      <input 
        type="email"
        placeholder="Email"
        value={user.email}
        onChange={(e) => updateUser('email', e.target.value)}
      />
      <input 
        type="text"
        placeholder="Street"
        value={user.address.street}
        onChange={(e) => updateAddress('street', e.target.value)}
      />
      <input 
        type="text"
        placeholder="City"
        value={user.address.city}
        onChange={(e) => updateAddress('city', e.target.value)}
      />
    </form>
  );
}
```

---

## 5. Event Handling

### Basic Event Handling
```javascript
function EventHandlingDemo() {
  const handleClick = () => {
    alert('Button clicked!');
  };

  const handleMouseEnter = () => {
    console.log('Mouse entered!');
  };

  const handleInputChange = (event) => {
    console.log('Input value:', event.target.value);
  };

  return (
    <div>
      <button onClick={handleClick}>
        Click Me
      </button>
      
      <div onMouseEnter={handleMouseEnter}>
        Hover over me
      </div>
      
      <input 
        type="text" 
        onChange={handleInputChange}
        placeholder="Type something..."
      />
    </div>
  );
}
```

### Event Object and Parameters
```javascript
function EventObjectDemo() {
  const handleSubmit = (event) => {
    event.preventDefault(); // Prevent default form submission
    console.log('Form submitted!');
  };

  const handleButtonClick = (event, customMessage) => {
    event.stopPropagation(); // Stop event bubbling
    alert(customMessage);
  };

  const handleKeyPress = (event) => {
    if (event.key === 'Enter') {
      console.log('Enter key pressed!');
    }
  };

  return (
    <div onClick={() => console.log('Div clicked!')}>
      <form onSubmit={handleSubmit}>
        <input 
          type="text" 
          onKeyPress={handleKeyPress}
          placeholder="Press Enter..."
        />
        <button type="submit">Submit</button>
      </form>
      
      <button 
        onClick={(e) => handleButtonClick(e, 'Custom message!')}
      >
        Click with custom message
      </button>
    </div>
  );
}
```

### Dynamic Event Handlers
```javascript
function TodoList() {
  const [todos, setTodos] = useState([
    { id: 1, text: 'Learn React', completed: false },
    { id: 2, text: 'Build a project', completed: false }
  ]);

  const toggleTodo = (id) => {
    setTodos(prevTodos =>
      prevTodos.map(todo =>
        todo.id === id 
          ? { ...todo, completed: !todo.completed }
          : todo
      )
    );
  };

  const deleteTodo = (id) => {
    setTodos(prevTodos =>
      prevTodos.filter(todo => todo.id !== id)
    );
  };

  return (
    <ul>
      {todos.map(todo => (
        <li key={todo.id}>
          <span 
            style={{ 
              textDecoration: todo.completed ? 'line-through' : 'none' 
            }}
            onClick={() => toggleTodo(todo.id)}
          >
            {todo.text}
          </span>
          <button onClick={() => deleteTodo(todo.id)}>
            Delete
          </button>
        </li>
      ))}
    </ul>
  );
}
```

---

## 6. Conditional Rendering

### If-Else with Ternary Operator
```javascript
function Welcome({ user }) {
  return (
    <div>
      {user ? (
        <h1>Welcome back, {user.name}!</h1>
      ) : (
        <h1>Please sign in.</h1>
      )}
    </div>
  );
}
```

### Logical AND Operator
```javascript
function Notifications({ messages }) {
  return (
    <div>
      {messages.length > 0 && (
        <div>
          <h2>You have {messages.length} unread messages.</h2>
          <ul>
            {messages.map((message, index) => (
              <li key={index}>{message}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
```

### Complex Conditional Rendering
```javascript
function UserDashboard({ user, isLoading, error }) {
  // Loading state
  if (isLoading) {
    return <div>Loading...</div>;
  }

  // Error state
  if (error) {
    return <div>Error: {error.message}</div>;
  }

  // No user state
  if (!user) {
    return <div>No user found.</div>;
  }

  // Success state with nested conditions
  return (
    <div>
      <h1>Welcome, {user.name}!</h1>
      
      {user.isAdmin && (
        <div>
          <h2>Admin Panel</h2>
          <button>Manage Users</button>
        </div>
      )}
      
      {user.notifications && user.notifications.length > 0 ? (
        <div>
          <h3>Notifications</h3>
          {user.notifications.map(notification => (
            <div key={notification.id}>
              {notification.message}
            </div>
          ))}
        </div>
      ) : (
        <p>No new notifications.</p>
      )}
      
      {user.subscription === 'premium' ? (
        <div>
          <h3>Premium Features</h3>
          <button>Access Premium Content</button>
        </div>
      ) : (
        <div>
          <h3>Upgrade to Premium</h3>
          <button>Upgrade Now</button>
        </div>
      )}
    </div>
  );
}
```

### Switch-Case Alternative
```javascript
function StatusIndicator({ status }) {
  const renderStatus = () => {
    switch (status) {
      case 'loading':
        return <div className="spinner">Loading...</div>;
      case 'success':
        return <div className="success">✓ Success!</div>;
      case 'error':
        return <div className="error">✗ Error occurred</div>;
      case 'warning':
        return <div className="warning">⚠ Warning</div>;
      default:
        return <div>Unknown status</div>;
    }
  };

  return (
    <div className="status-indicator">
      {renderStatus()}
    </div>
  );
}
```

---

## 7. Lists & Keys

### Basic List Rendering
```javascript
function SimpleList() {
  const fruits = ['Apple', 'Banana', 'Orange', 'Mango'];

  return (
    <ul>
      {fruits.map((fruit, index) => (
        <li key={index}>{fruit}</li>
      ))}
    </ul>
  );
}
```

### List with Objects
```javascript
function UserList() {
  const users = [
    { id: 1, name: 'John Doe', email: 'john@example.com' },
    { id: 2, name: 'Jane Smith', email: 'jane@example.com' },
    { id: 3, name: 'Bob Johnson', email: 'bob@example.com' }
  ];

  return (
    <div>
      {users.map(user => (
        <div key={user.id} className="user-card">
          <h3>{user.name}</h3>
          <p>{user.email}</p>
        </div>
      ))}
    </div>
  );
}
```

### Why Keys Matter
```javascript
// ❌ Bad: Using array index as key (can cause issues)
function BadList({ items }) {
  return (
    <ul>
      {items.map((item, index) => (
        <li key={index}>{item.name}</li>
      ))}
    </ul>
  );
}

// ✅ Good: Using unique, stable identifiers
function GoodList({ items }) {
  return (
    <ul>
      {items.map(item => (
        <li key={item.id}>{item.name}</li>
      ))}
    </ul>
  );
}
```

### Dynamic List with CRUD Operations
```javascript
function TaskManager() {
  const [tasks, setTasks] = useState([
    { id: 1, text: 'Learn React', completed: false },
    { id: 2, text: 'Build a project', completed: true }
  ]);
  const [newTask, setNewTask] = useState('');

  const addTask = () => {
    if (newTask.trim()) {
      setTasks(prevTasks => [
        ...prevTasks,
        {
          id: Date.now(), // Simple ID generation
          text: newTask,
          completed: false
        }
      ]);
      setNewTask('');
    }
  };

  const toggleTask = (id) => {
    setTasks(prevTasks =>
      prevTasks.map(task =>
        task.id === id ? { ...task, completed: !task.completed } : task
      )
    );
  };

  const deleteTask = (id) => {
    setTasks(prevTasks => prevTasks.filter(task => task.id !== id));
  };

  const editTask = (id, newText) => {
    setTasks(prevTasks =>
      prevTasks.map(task =>
        task.id === id ? { ...task, text: newText } : task
      )
    );
  };

  return (
    <div>
      <div>
        <input
          type="text"
          value={newTask}
          onChange={(e) => setNewTask(e.target.value)}
          placeholder="Add new task..."
          onKeyPress={(e) => e.key === 'Enter' && addTask()}
        />
        <button onClick={addTask}>Add Task</button>
      </div>

      <ul>
        {tasks.map(task => (
          <TaskItem
            key={task.id}
            task={task}
            onToggle={() => toggleTask(task.id)}
            onDelete={() => deleteTask(task.id)}
            onEdit={(newText) => editTask(task.id, newText)}
          />
        ))}
      </ul>
    </div>
  );
}

function TaskItem({ task, onToggle, onDelete, onEdit }) {
  const [isEditing, setIsEditing] = useState(false);
  const [editText, setEditText] = useState(task.text);

  const handleSave = () => {
    onEdit(editText);
    setIsEditing(false);
  };

  const handleCancel = () => {
    setEditText(task.text);
    setIsEditing(false);
  };

  return (
    <li className={`task-item ${task.completed ? 'completed' : ''}`}>
      {isEditing ? (
        <div>
          <input
            type="text"
            value={editText}
            onChange={(e) => setEditText(e.target.value)}
          />
          <button onClick={handleSave}>Save</button>
          <button onClick={handleCancel}>Cancel</button>
        </div>
      ) : (
        <div>
          <input
            type="checkbox"
            checked={task.completed}
            onChange={onToggle}
          />
          <span onClick={onToggle}>{task.text}</span>
          <button onClick={() => setIsEditing(true)}>Edit</button>
          <button onClick={onDelete}>Delete</button>
        </div>
      )}
    </li>
  );
}
```

### Filtering and Sorting Lists
```javascript
function ProductList() {
  const [products] = useState([
    { id: 1, name: 'Laptop', category: 'Electronics', price: 999 },
    { id: 2, name: 'Shirt', category: 'Clothing', price: 29 },
    { id: 3, name: 'Phone', category: 'Electronics', price: 699 },
    { id: 4, name: 'Jeans', category: 'Clothing', price: 59 }
  ]);

  const [filter, setFilter] = useState('all');
  const [sortBy, setSortBy] = useState('name');

  const filteredProducts = products.filter(product => {
    if (filter === 'all') return true;
    return product.category.toLowerCase() === filter.toLowerCase();
  });

  const sortedProducts = [...filteredProducts].sort((a, b) => {
    if (sortBy === 'name') {
      return a.name.localeCompare(b.name);
    }
    if (sortBy === 'price') {
      return a.price - b.price;
    }
    return 0;
  });

  return (
    <div>
      <div className="controls">
        <select value={filter} onChange={(e) => setFilter(e.target.value)}>
          <option value="all">All Categories</option>
          <option value="electronics">Electronics</option>
          <option value="clothing">Clothing</option>
        </select>

        <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
          <option value="name">Sort by Name</option>
          <option value="price">Sort by Price</option>
        </select>
      </div>

      <div className="product-grid">
        {sortedProducts.map(product => (
          <div key={product.id} className="product-card">
            <h3>{product.name}</h3>
            <p>Category: {product.category}</p>
            <p>Price: ${product.price}</p>
          </div>
        ))}
      </div>
    </div>
  );
}
```

---

## 8. Forms & Controlled Components

### Basic Controlled Input
```javascript
function SimpleForm() {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');

  const handleSubmit = (event) => {
    event.preventDefault();
    console.log('Form submitted:', { name, email });
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>
          Name:
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </label>
      </div>
      <div>
        <label>
          Email:
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </label>
      </div>
      <button type="submit">Submit</button>
    </form>
  );
}
```

### Complex Form with Validation
```javascript
function RegistrationForm() {
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    age: '',
    gender: '',
    interests: [],
    newsletter: false
  });

  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleInputChange = (event) => {
    const { name, value, type, checked } = event.target;
    
    setFormData(prevData => ({
      ...prevData,
      [name]: type === 'checkbox' ? checked : value
    }));

    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prevErrors => ({
        ...prevErrors,
        [name]: ''
      }));
    }
  };

  const handleInterestChange = (event) => {
    const { value, checked } = event.target;
    
    setFormData(prevData => ({
      ...prevData,
      interests: checked
        ? [...prevData.interests, value]
        : prevData.interests.filter(interest => interest !== value)
    }));
  };

  const validateForm = () => {
    const newErrors = {};

    // Username validation
    if (!formData.username.trim()) {
      newErrors.username = 'Username is required';
    } else if (formData.username.length < 3) {
      newErrors.username = 'Username must be at least 3 characters';
    }

    // Email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!formData.email) {
      newErrors.email = 'Email is required';
    } else if (!emailRegex.test(formData.email)) {
      newErrors.email = 'Please enter a valid email';
    }

    // Password validation
    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }

    // Confirm password validation
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }

    // Age validation
    if (!formData.age) {
      newErrors.age = 'Age is required';
    } else if (formData.age < 13) {
      newErrors.age = 'Must be at least 13 years old';
    }

    return newErrors;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    
    const formErrors = validateForm();
    
    if (Object.keys(formErrors).length > 0) {
      setErrors(formErrors);
      return;
    }

    setIsSubmitting(true);
    
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 2000));
      console.log('Form submitted successfully:', formData);
      alert('Registration successful!');
      
      // Reset form
      setFormData({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        age: '',
        gender: '',
        interests: [],
        newsletter: false
      });
    } catch (error) {
      console.error('Submission error:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="registration-form">
      <div className="form-group">
        <label>
          Username:
          <input
            type="text"
            name="username"
            value={formData.username}
            onChange={handleInputChange}
            className={errors.username ? 'error' : ''}
          />
          {errors.username && <span className="error-message">{errors.username}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Email:
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleInputChange}
            className={errors.email ? 'error' : ''}
          />
          {errors.email && <span className="error-message">{errors.email}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Password:
          <input
            type="password"
            name="password"
            value={formData.password}
            onChange={handleInputChange}
            className={errors.password ? 'error' : ''}
          />
          {errors.password && <span className="error-message">{errors.password}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Confirm Password:
          <input
            type="password"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleInputChange}
            className={errors.confirmPassword ? 'error' : ''}
          />
          {errors.confirmPassword && <span className="error-message">{errors.confirmPassword}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Age:
          <input
            type="number"
            name="age"
            value={formData.age}
            onChange={handleInputChange}
            className={errors.age ? 'error' : ''}
          />
          {errors.age && <span className="error-message">{errors.age}</span>}
        </label>
      </div>

      <div className="form-group">
        <label>
          Gender:
          <select
            name="gender"
            value={formData.gender}
            onChange={handleInputChange}
          >
            <option value="">Select Gender</option>
            <option value="male">Male</option>
            <option value="female">Female</option>
            <option value="other">Other</option>
          </select>
        </label>
      </div>

      <div className="form-group">
        <fieldset>
          <legend>Interests:</legend>
          {['Technology', 'Sports', 'Music', 'Travel', 'Reading'].map(interest => (
            <label key={interest} className="checkbox-label">
              <input
                type="checkbox"
                value={interest}
                checked={formData.interests.includes(interest)}
                onChange={handleInterestChange}
              />
              {interest}
            </label>
          ))}
        </fieldset>
      </div>

      <div className="form-group">
        <label className="checkbox-label">
          <input
            type="checkbox"
            name="newsletter"
            checked={formData.newsletter}
            onChange={handleInputChange}
          />
          Subscribe to newsletter
        </label>
      </div>

      <button 
        type="submit" 
        disabled={isSubmitting}
        className={isSubmitting ? 'loading' : ''}
      >
        {isSubmitting ? 'Registering...' : 'Register'}
      </button>
    </form>
  );
}
```

### File Upload Component
```javascript
function FileUpload() {
  const [selectedFiles, setSelectedFiles] = useState([]);
  const [uploadProgress, setUploadProgress] = useState({});
  const [uploadedFiles, setUploadedFiles] = useState([]);

  const handleFileSelect = (event) => {
    const files = Array.from(event.target.files);
    setSelectedFiles(files);
  };

  const handleUpload = async () => {
    for (const file of selectedFiles) {
      try {
        // Simulate file upload with progress
        setUploadProgress(prev => ({ ...prev, [file.name]: 0 }));
        
        for (let progress = 0; progress <= 100; progress += 10) {
          await new Promise(resolve => setTimeout(resolve, 100));
          setUploadProgress(prev => ({ ...prev, [file.name]: progress }));
        }

        setUploadedFiles(prev => [...prev, {
          name: file.name,
          size: file.size,
          type: file.type,
          url: URL.createObjectURL(file)
        }]);

      } catch (error) {
        console.error('Upload failed:', error);
      }
    }
    
    setSelectedFiles([]);
    setUploadProgress({});
  };

  const handleRemoveFile = (fileName) => {
    setUploadedFiles(prev => prev.filter(file => file.name !== fileName));
  };

  return (
    <div className="file-upload">
      <div className="upload-area">
        <input
          type="file"
          multiple
          onChange={handleFileSelect}
          accept="image/*,.pdf,.doc,.docx"
        />
        
        {selectedFiles.length > 0 && (
          <div>
            <h4>Selected Files:</h4>
            <ul>
              {selectedFiles.map((file, index) => (
                <li key={index}>
                  {file.name} ({(file.size / 1024 / 1024).toFixed(2)} MB)
                  {uploadProgress[file.name] !== undefined && (
                    <div className="progress-bar">
                      <div 
                        className="progress-fill"
                        style={{ width: `${uploadProgress[file.name]}%` }}
                      />
                    </div>
                  )}
                </li>
              ))}
            </ul>
            <button onClick={handleUpload}>Upload Files</button>
          </div>
        )}
      </div>

      {uploadedFiles.length > 0 && (
        <div className="uploaded-files">
          <h4>Uploaded Files:</h4>
          <div className="file-grid">
            {uploadedFiles.map((file, index) => (
              <div key={index} className="file-card">
                {file.type.startsWith('image/') && (
                  <img src={file.url} alt={file.name} className="file-preview" />
                )}
                <div className="file-info">
                  <p>{file.name}</p>
                  <p>{(file.size / 1024 / 1024).toFixed(2)} MB</p>
                  <button onClick={() => handleRemoveFile(file.name)}>
                    Remove
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
```

---

## 9. Component Lifecycle

### Lifecycle with useEffect Hook
```javascript
import React, { useState, useEffect } from 'react';

function LifecycleDemo() {
  const [count, setCount] = useState(0);
  const [data, setData] = useState(null);
  const [windowWidth, setWindowWidth] = useState(window.innerWidth);

  // Mounting (component did mount)
  useEffect(() => {
    console.log('Component mounted');
    
    // Fetch initial data
    fetchData();
    
    // Cleanup function (component will unmount)
    return () => {
      console.log('Component will unmount');
    };
  }, []); // Empty dependency array = runs once on mount

  // Updating (component did update)
  useEffect(() => {
    console.log('Count updated:', count);
    
    // Update document title
    document.title = `Count: ${count}`;
  }, [count]); // Runs when count changes

  // Window resize listener
  useEffect(() => {
    const handleResize = () => {
      setWindowWidth(window.innerWidth);
    };

    window.addEventListener('resize', handleResize);

    // Cleanup
    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  const fetchData = async () => {
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      setData({ message: 'Data loaded successfully!' });
    } catch (error) {
      console.error('Failed to fetch data:', error);
    }
  };

  return (
    <div>
      <h2>Lifecycle Demo</h2>
      <p>Count: {count}</p>
      <p>Window Width: {windowWidth}px</p>
      
      <button onClick={() => setCount(count + 1)}>
        Increment Count
      </button>
      
      {data ? (
        <p>{data.message}</p>
      ) : (
        <p>Loading data...</p>
      )}
    </div>
  );
}
```

### Data Fetching with useEffect
```javascript
function UserProfile({ userId }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let isCancelled = false; // Flag to prevent state updates if component unmounts

    const fetchUser = async () => {
      try {
        setLoading(true);
        setError(null);
        
        const response = await fetch(`/api/users/${userId}`);
        
        if (!response.ok) {
          throw new Error('Failed to fetch user');
        }
        
        const userData = await response.json();
        
        // Only update state if component is still mounted
        if (!isCancelled) {
          setUser(userData);
        }
      } catch (err) {
        if (!isCancelled) {
          setError(err.message);
        }
      } finally {
        if (!isCancelled) {
          setLoading(false);
        }
      }
    };

    if (userId) {
      fetchUser();
    }

    // Cleanup function
    return () => {
      isCancelled = true;
    };
  }, [userId]); // Re-run when userId changes

  if (loading) return <div>Loading user...</div>;
  if (error) return <div>Error: {error}</div>;
  if (!user) return <div>No user found</div>;

  return (
    <div className="user-profile">
      <h2>{user.name}</h2>
      <p>Email: {user.email}</p>
      <p>Joined: {new Date(user.joinDate).toLocaleDateString()}</p>
    </div>
  );
}
```

### Timer and Interval Example
```javascript
function Timer() {
  const [seconds, setSeconds] = useState(0);
  const [isRunning, setIsRunning] = useState(false);

  useEffect(() => {
    let interval = null;

    if (isRunning) {
      interval = setInterval(() => {
        setSeconds(prevSeconds => prevSeconds + 1);
      }, 1000);
    }

    return () => {
      if (interval) {
        clearInterval(interval);
      }
    };
  }, [isRunning]);

  const startTimer = () => setIsRunning(true);
  const stopTimer = () => setIsRunning(false);
  const resetTimer = () => {
    setIsRunning(false);
    setSeconds(0);
  };

  const formatTime = (totalSeconds) => {
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const secs = totalSeconds % 60;
    
    return `${hours.toString().padStart(2, '0')}:${minutes
      .toString()
      .padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  };

  return (
    <div className="timer">
      <h2>Timer</h2>
      <div className="time-display">{formatTime(seconds)}</div>
      
      <div className="timer-controls">
        <button onClick={startTimer} disabled={isRunning}>
          Start
        </button>
        <button onClick={stopTimer} disabled={!isRunning}>
          Stop
        </button>
        <button onClick={resetTimer}>Reset</button>
      </div>
    </div>
  );
}
```

---

## 10. React Hooks

### useState Hook Deep Dive
```javascript
function StateExamples() {
  // Basic state
  const [count, setCount] = useState(0);
  
  // State with object
  const [user, setUser] = useState({
    name: '',
    email: '',
    preferences: {
      theme: 'light',
      notifications: true
    }
  });

  // State with array
  const [items, setItems] = useState([]);

  // Lazy initial state (expensive calculation)
  const [expensiveValue, setExpensiveValue] = useState(() => {
    console.log('Computing expensive initial value...');
    return Array.from({ length: 1000 }, (_, i) => i).reduce((sum, i) => sum + i, 0);
  });

  // Functional updates
  const incrementCount = () => {
    setCount(prevCount => prevCount + 1);
  };

  const updateUser = (field, value) => {
    setUser(prevUser => ({
      ...prevUser,
      [field]: value
    }));
  };

  const updatePreferences = (key, value) => {
    setUser(prevUser => ({
      ...prevUser,
      preferences: {
        ...prevUser.preferences,
        [key]: value
      }
    }));
  };

  const addItem = (item) => {
    setItems(prevItems => [...prevItems, item]);
  };

  const removeItem = (index) => {
    setItems(prevItems => prevItems.filter((_, i) => i !== index));
  };

  return (
    <div>
      <h3>State Examples</h3>
      
      <div>
        <h4>Counter: {count}</h4>
        <button onClick={incrementCount}>Increment</button>
      </div>

      <div>
        <h4>User Info</h4>
        <input
          placeholder="Name"
          value={user.name}
          onChange={(e) => updateUser('name', e.target.value)}
        />
        <input
          placeholder="Email"
          value={user.email}
          onChange={(e) => updateUser('email', e.target.value)}
        />
        
        <label>
          <input
            type="checkbox"
            checked={user.preferences.notifications}
            onChange={(e) => updatePreferences('notifications', e.target.checked)}
          />
          Enable Notifications
        </label>
      </div>

      <div>
        <h4>Expensive Value: {expensiveValue}</h4>
      </div>
    </div>
  );
}
```

### useEffect Hook Advanced Patterns
```javascript
function AdvancedEffects() {
  const [data, setData] = useState(null);
  const [query, setQuery] = useState('');
  const [debouncedQuery, setDebouncedQuery] = useState('');

  // Debounced search effect
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedQuery(query);
    }, 500);

    return () => clearTimeout(timer);
  }, [query]);

  // Search effect that runs when debounced query changes
  useEffect(() => {
    if (!debouncedQuery) return;

    const searchData = async () => {
      try {
        const response = await fetch(`/api/search?q=${debouncedQuery}`);
        const results = await response.json();
        setData(results);
      } catch (error) {
        console.error('Search failed:', error);
      }
    };

    searchData();
  }, [debouncedQuery]);

  // Cleanup effect for subscriptions
  useEffect(() => {
    const subscription = subscribeToUpdates((update) => {
      console.log('Received update:', update);
    });

    return () => subscription.unsubscribe();
  }, []);

  return (
    <div>
      <input
        type="text"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="Search..."
      />
      
      {data && (
        <div>
          <h4>Search Results:</h4>
          <pre>{JSON.stringify(data, null, 2)}</pre>
        </div>
      )}
    </div>
  );
}

// Mock subscription function
function subscribeToUpdates(callback) {
  const interval = setInterval(() => {
    callback({ timestamp: Date.now(), data: 'Update data' });
  }, 5000);

  return {
    unsubscribe: () => clearInterval(interval)
  };
}
```

### Custom Hooks
```javascript
// Custom hook for localStorage
function useLocalStorage(key, initialValue) {
  const [storedValue, setStoredValue] = useState(() => {
    try {
      const item = window.localStorage.getItem(key);
      return item ? JSON.parse(item) : initialValue;
    } catch (error) {
      console.error(`Error reading localStorage key "${key}":`, error);
      return initialValue;
    }
  });

  const setValue = (value) => {
    try {
      const valueToStore = value instanceof Function ? value(storedValue) : value;
      setStoredValue(valueToStore);
      window.localStorage.setItem(key, JSON.stringify(valueToStore));
    } catch (error) {
      console.error(`Error setting localStorage key "${key}":`, error);
    }
  };

  return [storedValue, setValue];
}

// Custom hook for API calls
function useApi(url) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const refetch = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch(url);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const result = await response.json();
      setData(result);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [url]);

  useEffect(() => {
    refetch();
  }, [refetch]);

  return { data, loading, error, refetch };
}

// Custom hook for form handling
function useForm(initialValues, validationSchema) {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});

  const handleChange = (event) => {
    const { name, value, type, checked } = event.target;
    
    setValues(prevValues => ({
      ...prevValues,
      [name]: type === 'checkbox' ? checked : value
    }));

    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prevErrors => ({
        ...prevErrors,
        [name]: ''
      }));
    }
  };

  const handleBlur = (event) => {
    const { name } = event.target;
    setTouched(prevTouched => ({
      ...prevTouched,
      [name]: true
    }));

    // Validate field on blur if validation schema exists
    if (validationSchema && validationSchema[name]) {
      const error = validationSchema[name](values[name], values);
      if (error) {
        setErrors(prevErrors => ({
          ...prevErrors,
          [name]: error
        }));
      }
    }
  };

  const validate = () => {
    if (!validationSchema) return true;

    const newErrors = {};
    let isValid = true;

    Object.keys(validationSchema).forEach(field => {
      const error = validationSchema[field](values[field], values);
      if (error) {
        newErrors[field] = error;
        isValid = false;
      }
    });

    setErrors(newErrors);
    return isValid;
  };

  const reset = () => {
    setValues(initialValues);
    setErrors({});
    setTouched({});
  };

  return {
    values,
    errors,
    touched,
    handleChange,
    handleBlur,
    validate,
    reset
  };
}

// Usage examples
function HookExamples() {
  // Using localStorage hook
  const [name, setName] = useLocalStorage('userName', '');
  
  // Using API hook
  const { data: posts, loading, error, refetch } = useApi('/api/posts');
  
  // Using form hook
  const form = useForm(
    { email: '', password: '' },
    {
      email: (value) => {
        if (!value) return 'Email is required';
        if (!/\S+@\S+\.\S+/.test(value)) return 'Email is invalid';
        return '';
      },
      password: (value) => {
        if (!value) return 'Password is required';
        if (value.length < 6) return 'Password must be at least 6 characters';
        return '';
      }
    }
  );

  const handleSubmit = (event) => {
    event.preventDefault();
    if (form.validate()) {
      console.log('Form is valid:', form.values);
    }
  };

  return (
    <div>
      <h3>Custom Hook Examples</h3>
      
      {/* localStorage example */}
      <div>
        <h4>Name (stored in localStorage):</h4>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Enter your name"
        />
        <p>Hello, {name}!</p>
      </div>

      {/* API hook example */}
      <div>
        <h4>Posts from API:</h4>
        {loading && <p>Loading...</p>}
        {error && <p>Error: {error}</p>}
        {posts && (
          <div>
            <button onClick={refetch}>Refresh</button>
            <ul>
              {posts.map(post => (
                <li key={post.id}>{post.title}</li>
              ))}
            </ul>
          </div>
        )}
      </div>

      {/* Form hook example */}
      <div>
        <h4>Login Form:</h4>
        <form onSubmit={handleSubmit}>
          <div>
            <input
              type="email"
              name="email"
              placeholder="Email"
              value={form.values.email}
              onChange={form.handleChange}
              onBlur={form.handleBlur}
            />
            {form.touched.email && form.errors.email && (
              <span className="error">{form.errors.email}</span>
            )}
          </div>
          
          <div>
            <input
              type="password"
              name="password"
              placeholder="Password"
              value={form.values.password}
              onChange={form.handleChange}
              onBlur={form.handleBlur}
            />
            {form.touched.password && form.errors.password && (
              <span className="error">{form.errors.password}</span>
            )}
          </div>
          
          <button type="submit">Login</button>
          <button type="button" onClick={form.reset}>Reset</button>
        </form>
      </div>
    </div>
  );
}
```

### useCallback and useMemo
```javascript
function PerformanceOptimization() {
  const [count, setCount] = useState(0);
  const [items, setItems] = useState([]);
  const [filter, setFilter] = useState('');

  // Expensive calculation - only recalculate when items change
  const expensiveValue = useMemo(() => {
    console.log('Calculating expensive value...');
    return items.reduce((sum, item) => sum + item.value, 0);
  }, [items]);

  // Filtered items - only recalculate when items or filter change
  const filteredItems = useMemo(() => {
    console.log('Filtering items...');
    return items.filter(item =>
      item.name.toLowerCase().includes(filter.toLowerCase())
    );
  }, [items, filter]);

  // Memoized callback - prevent unnecessary re-renders of child components
  const handleAddItem = useCallback(() => {
    const newItem = {
      id: Date.now(),
      name: `Item ${items.length + 1}`,
      value: Math.floor(Math.random() * 100)
    };
    setItems(prevItems => [...prevItems, newItem]);
  }, [items.length]);

  // Another memoized callback
  const handleRemoveItem = useCallback((id) => {
    setItems(prevItems => prevItems.filter(item => item.id !…