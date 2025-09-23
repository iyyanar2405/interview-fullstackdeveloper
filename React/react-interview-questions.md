# Comprehensive React.js & React Native Interview Questions

## Table of Contents

1. [React.js Core Concepts](#reactjs-core-concepts)
2. [React Hooks](#react-hooks)
3. [State Management](#state-management)
4. [Routing & Navigation](#routing--navigation)
5. [Performance Optimization](#performance-optimization)
6. [Testing](#testing)
7. [React Native Core](#react-native-core)
8. [React Native Development](#react-native-development)
9. [Architecture & Design Patterns](#architecture--design-patterns)
10. [Advanced Topics](#advanced-topics)

---

## React.js Core Concepts

### Beginner Level

#### Q1: What is React and what are its key features?
**Answer:**
React is a JavaScript library for building user interfaces, particularly web applications. It was developed by Facebook and focuses on creating reusable UI components.

**Key Features:**
- **Virtual DOM**: Efficient updates and rendering
- **Component-Based**: Reusable and modular UI components
- **JSX**: JavaScript XML syntax for writing components
- **One-Way Data Flow**: Predictable data flow
- **Declarative**: Describe what the UI should look like

**Example:**
```jsx
// Simple React Component
import React, { useState } from 'react';

function Welcome({ name }) {
  const [count, setCount] = useState(0);

  return (
    <div className="welcome-container">
      <h1>Hello, {name}!</h1>
      <p>You clicked {count} times</p>
      <button onClick={() => setCount(count + 1)}>
        Click me
      </button>
    </div>
  );
}

// Usage
function App() {
  return (
    <div>
      <Welcome name="John" />
      <Welcome name="Jane" />
    </div>
  );
}

export default App;
```

#### Q2: What is JSX and how does it work?
**Answer:**
JSX (JavaScript XML) is a syntax extension for JavaScript that allows you to write HTML-like code within JavaScript. It gets transpiled to regular JavaScript function calls.

**Example:**
```jsx
// JSX syntax
const element = (
  <div className="container">
    <h1>Hello, World!</h1>
    <p>This is JSX</p>
  </div>
);

// Transpiled JavaScript (what JSX becomes)
const element = React.createElement(
  'div',
  { className: 'container' },
  React.createElement('h1', null, 'Hello, World!'),
  React.createElement('p', null, 'This is JSX')
);

// JSX with expressions
function Greeting({ user, isLoggedIn }) {
  return (
    <div>
      {isLoggedIn ? (
        <h1>Welcome back, {user.name}!</h1>
      ) : (
        <h1>Please sign in</h1>
      )}
      
      {/* Conditional rendering */}
      {user.messages.length > 0 && (
        <div>
          You have {user.messages.length} unread messages
        </div>
      )}
      
      {/* List rendering */}
      <ul>
        {user.hobbies.map((hobby, index) => (
          <li key={index}>{hobby}</li>
        ))}
      </ul>
    </div>
  );
}

// JSX Rules and Best Practices
function JSXExample() {
  const users = ['Alice', 'Bob', 'Charlie'];
  const isVisible = true;
  
  return (
    <div>
      {/* Must have one parent element or use Fragment */}
      <React.Fragment>
        <h1>Users List</h1>
        <div>Content here</div>
      </React.Fragment>
      
      {/* Or use short syntax */}
      <>
        <h2>Another Section</h2>
        <p>Fragment shorthand</p>
      </>
      
      {/* Class becomes className */}
      <div className="my-class" />
      
      {/* Style as object */}
      <div style={{ color: 'red', fontSize: '16px' }} />
      
      {/* Self-closing tags must end with /> */}
      <img src="image.jpg" alt="Example" />
      <input type="text" />
      
      {/* Event handlers use camelCase */}
      <button onClick={handleClick}>Click me</button>
    </div>
  );
}
```

#### Q3: What are props and how do you use them?
**Answer:**
Props (properties) are read-only inputs passed from parent components to child components. They allow data to flow down the component tree.

**Example:**
```jsx
// Child Component receiving props
function UserCard({ user, showEmail = false, onEdit }) {
  const handleEdit = () => {
    onEdit(user.id);
  };

  return (
    <div className="user-card">
      <img src={user.avatar} alt={user.name} />
      <h3>{user.name}</h3>
      <p>Age: {user.age}</p>
      
      {/* Conditional rendering based on prop */}
      {showEmail && <p>Email: {user.email}</p>}
      
      <button onClick={handleEdit}>Edit User</button>
    </div>
  );
}

// Parent Component passing props
function UserList() {
  const users = [
    { id: 1, name: 'John Doe', age: 30, email: 'john@example.com', avatar: 'john.jpg' },
    { id: 2, name: 'Jane Smith', age: 25, email: 'jane@example.com', avatar: 'jane.jpg' }
  ];

  const handleUserEdit = (userId) => {
    console.log(`Editing user ${userId}`);
  };

  return (
    <div className="user-list">
      <h2>Our Users</h2>
      {users.map(user => (
        <UserCard
          key={user.id}
          user={user}
          showEmail={true}
          onEdit={handleUserEdit}
        />
      ))}
    </div>
  );
}

// Props validation with PropTypes
import PropTypes from 'prop-types';

UserCard.propTypes = {
  user: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    age: PropTypes.number,
    email: PropTypes.string,
    avatar: PropTypes.string
  }).isRequired,
  showEmail: PropTypes.bool,
  onEdit: PropTypes.func.isRequired
};

UserCard.defaultProps = {
  showEmail: false
};

// Destructuring props
function Profile({ name, age, location, skills = [] }) {
  return (
    <div>
      <h1>{name}</h1>
      <p>Age: {age}</p>
      <p>Location: {location}</p>
      <div>
        <h3>Skills:</h3>
        <ul>
          {skills.map((skill, index) => (
            <li key={index}>{skill}</li>
          ))}
        </ul>
      </div>
    </div>
  );
}

// Spreading props
function Button({ children, ...otherProps }) {
  return (
    <button className="custom-button" {...otherProps}>
      {children}
    </button>
  );
}

// Usage: <Button onClick={handleClick} disabled={true}>Submit</Button>
```

### Intermediate Level

#### Q4: What are React lifecycle methods and their modern equivalents?
**Answer:**
Lifecycle methods are special methods in class components that get called at different stages of a component's life. With functional components and hooks, these are handled differently.

**Example:**
```jsx
// Class Component with Lifecycle Methods
import React, { Component } from 'react';

class UserProfile extends Component {
  constructor(props) {
    super(props);
    this.state = {
      user: null,
      loading: true,
      error: null
    };
  }

  // Mounting phase
  componentDidMount() {
    console.log('Component mounted');
    this.fetchUserData();
    
    // Set up subscriptions, timers, etc.
    this.timer = setInterval(() => {
      this.checkUserStatus();
    }, 30000);
  }

  // Updating phase
  componentDidUpdate(prevProps, prevState) {
    console.log('Component updated');
    
    // Fetch new data if userId changed
    if (prevProps.userId !== this.props.userId) {
      this.fetchUserData();
    }
    
    // Log state changes
    if (prevState.user !== this.state.user) {
      console.log('User data changed:', this.state.user);
    }
  }

  // Unmounting phase
  componentWillUnmount() {
    console.log('Component will unmount');
    
    // Cleanup: remove timers, cancel requests, unsubscribe
    if (this.timer) {
      clearInterval(this.timer);
    }
    
    if (this.abortController) {
      this.abortController.abort();
    }
  }

  // Error handling
  componentDidCatch(error, errorInfo) {
    console.error('Component caught error:', error, errorInfo);
    this.setState({ error: error.message });
  }

  fetchUserData = async () => {
    try {
      this.setState({ loading: true, error: null });
      
      this.abortController = new AbortController();
      const response = await fetch(`/api/users/${this.props.userId}`, {
        signal: this.abortController.signal
      });
      
      const user = await response.json();
      this.setState({ user, loading: false });
    } catch (error) {
      if (error.name !== 'AbortError') {
        this.setState({ error: error.message, loading: false });
      }
    }
  };

  checkUserStatus = () => {
    // Check user status periodically
    console.log('Checking user status...');
  };

  render() {
    const { user, loading, error } = this.state;

    if (loading) return <div>Loading...</div>;
    if (error) return <div>Error: {error}</div>;
    if (!user) return <div>No user found</div>;

    return (
      <div className="user-profile">
        <h1>{user.name}</h1>
        <p>{user.email}</p>
      </div>
    );
  }
}

// Modern Functional Component with Hooks (Equivalent)
import React, { useState, useEffect, useCallback } from 'react';

function UserProfileHooks({ userId }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Equivalent to componentDidMount and componentDidUpdate
  const fetchUserData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      const abortController = new AbortController();
      const response = await fetch(`/api/users/${userId}`, {
        signal: abortController.signal
      });
      
      const userData = await response.json();
      setUser(userData);
    } catch (err) {
      if (err.name !== 'AbortError') {
        setError(err.message);
      }
    } finally {
      setLoading(false);
    }
  }, [userId]);

  // Equivalent to componentDidMount
  useEffect(() => {
    console.log('Component mounted');
    fetchUserData();
  }, [fetchUserData]);

  // Equivalent to componentDidMount + componentWillUnmount for timer
  useEffect(() => {
    const timer = setInterval(() => {
      console.log('Checking user status...');
    }, 30000);

    // Cleanup function (equivalent to componentWillUnmount)
    return () => {
      console.log('Cleaning up timer');
      clearInterval(timer);
    };
  }, []);

  // Equivalent to componentDidUpdate for userId changes
  useEffect(() => {
    fetchUserData();
  }, [userId, fetchUserData]);

  // Equivalent to componentDidUpdate for user changes
  useEffect(() => {
    if (user) {
      console.log('User data changed:', user);
    }
  }, [user]);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;
  if (!user) return <div>No user found</div>;

  return (
    <div className="user-profile">
      <h1>{user.name}</h1>
      <p>{user.email}</p>
    </div>
  );
}

// Error Boundary (still requires class component)
class ErrorBoundary extends Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error };
  }

  componentDidCatch(error, errorInfo) {
    console.error('Error caught by boundary:', error, errorInfo);
    // Log to error reporting service
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="error-boundary">
          <h2>Something went wrong!</h2>
          <p>{this.state.error?.message}</p>
          <button onClick={() => this.setState({ hasError: false, error: null })}>
            Try again
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}

// Usage with Error Boundary
function App() {
  return (
    <ErrorBoundary>
      <UserProfileHooks userId={1} />
    </ErrorBoundary>
  );
}
```

#### Q5: What is the difference between controlled and uncontrolled components?
**Answer:**
Controlled components have their state managed by React, while uncontrolled components manage their own state internally using refs.

**Example:**
```jsx
import React, { useState, useRef, useEffect } from 'react';

// Controlled Components
function ControlledForm() {
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    country: 'US',
    newsletter: false,
    gender: '',
    bio: ''
  });

  const [errors, setErrors] = useState({});

  // Handle input changes
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));

    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  // Validation
  const validateForm = () => {
    const newErrors = {};

    if (!formData.username.trim()) {
      newErrors.username = 'Username is required';
    } else if (formData.username.length < 3) {
      newErrors.username = 'Username must be at least 3 characters';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
    }

    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (validateForm()) {
      console.log('Form submitted:', formData);
      // Submit to API
    }
  };

  return (
    <form onSubmit={handleSubmit} className="controlled-form">
      <h2>Controlled Form</h2>
      
      {/* Text Input */}
      <div className="form-group">
        <label>Username:</label>
        <input
          type="text"
          name="username"
          value={formData.username}
          onChange={handleChange}
          className={errors.username ? 'error' : ''}
        />
        {errors.username && <span className="error-message">{errors.username}</span>}
      </div>

      {/* Email Input */}
      <div className="form-group">
        <label>Email:</label>
        <input
          type="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          className={errors.email ? 'error' : ''}
        />
        {errors.email && <span className="error-message">{errors.email}</span>}
      </div>

      {/* Password Input */}
      <div className="form-group">
        <label>Password:</label>
        <input
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          className={errors.password ? 'error' : ''}
        />
        {errors.password && <span className="error-message">{errors.password}</span>}
      </div>

      {/* Select */}
      <div className="form-group">
        <label>Country:</label>
        <select name="country" value={formData.country} onChange={handleChange}>
          <option value="US">United States</option>
          <option value="CA">Canada</option>
          <option value="UK">United Kingdom</option>
          <option value="AU">Australia</option>
        </select>
      </div>

      {/* Checkbox */}
      <div className="form-group">
        <label>
          <input
            type="checkbox"
            name="newsletter"
            checked={formData.newsletter}
            onChange={handleChange}
          />
          Subscribe to newsletter
        </label>
      </div>

      {/* Radio buttons */}
      <div className="form-group">
        <label>Gender:</label>
        <label>
          <input
            type="radio"
            name="gender"
            value="male"
            checked={formData.gender === 'male'}
            onChange={handleChange}
          />
          Male
        </label>
        <label>
          <input
            type="radio"
            name="gender"
            value="female"
            checked={formData.gender === 'female'}
            onChange={handleChange}
          />
          Female
        </label>
      </div>

      {/* Textarea */}
      <div className="form-group">
        <label>Bio:</label>
        <textarea
          name="bio"
          value={formData.bio}
          onChange={handleChange}
          rows={4}
        />
      </div>

      <button type="submit">Submit</button>
      
      {/* Display current form state */}
      <div className="form-state">
        <h3>Current Form State:</h3>
        <pre>{JSON.stringify(formData, null, 2)}</pre>
      </div>
    </form>
  );
}

// Uncontrolled Components
function UncontrolledForm() {
  const usernameRef = useRef();
  const emailRef = useRef();
  const passwordRef = useRef();
  const countryRef = useRef();
  const newsletterRef = useRef();
  const bioRef = useRef();
  const formRef = useRef();

  useEffect(() => {
    // Focus on first input when component mounts
    usernameRef.current.focus();
  }, []);

  const handleSubmit = (e) => {
    e.preventDefault();
    
    // Get values from refs
    const formData = {
      username: usernameRef.current.value,
      email: emailRef.current.value,
      password: passwordRef.current.value,
      country: countryRef.current.value,
      newsletter: newsletterRef.current.checked,
      bio: bioRef.current.value
    };

    // Basic validation
    if (!formData.username.trim()) {
      alert('Username is required');
      usernameRef.current.focus();
      return;
    }

    if (!formData.email.trim()) {
      alert('Email is required');
      emailRef.current.focus();
      return;
    }

    console.log('Uncontrolled form submitted:', formData);
    
    // Reset form
    formRef.current.reset();
  };

  const handleReset = () => {
    formRef.current.reset();
    usernameRef.current.focus();
  };

  return (
    <form ref={formRef} onSubmit={handleSubmit} className="uncontrolled-form">
      <h2>Uncontrolled Form</h2>
      
      <div className="form-group">
        <label>Username:</label>
        <input
          ref={usernameRef}
          type="text"
          name="username"
          defaultValue=""
        />
      </div>

      <div className="form-group">
        <label>Email:</label>
        <input
          ref={emailRef}
          type="email"
          name="email"
          defaultValue=""
        />
      </div>

      <div className="form-group">
        <label>Password:</label>
        <input
          ref={passwordRef}
          type="password"
          name="password"
          defaultValue=""
        />
      </div>

      <div className="form-group">
        <label>Country:</label>
        <select ref={countryRef} name="country" defaultValue="US">
          <option value="US">United States</option>
          <option value="CA">Canada</option>
          <option value="UK">United Kingdom</option>
          <option value="AU">Australia</option>
        </select>
      </div>

      <div className="form-group">
        <label>
          <input
            ref={newsletterRef}
            type="checkbox"
            name="newsletter"
            defaultChecked={false}
          />
          Subscribe to newsletter
        </label>
      </div>

      <div className="form-group">
        <label>Bio:</label>
        <textarea
          ref={bioRef}
          name="bio"
          defaultValue=""
          rows={4}
        />
      </div>

      <button type="submit">Submit</button>
      <button type="button" onClick={handleReset}>Reset</button>
    </form>
  );
}

// Custom Hook for Form Handling (Best of both worlds)
function useForm(initialValues, validationRules) {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});

  const setValue = (name, value) => {
    setValues(prev => ({ ...prev, [name]: value }));
    
    // Clear error when value changes
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const setFieldTouched = (name) => {
    setTouched(prev => ({ ...prev, [name]: true }));
  };

  const validate = () => {
    const newErrors = {};
    
    Object.keys(validationRules).forEach(field => {
      const rule = validationRules[field];
      const value = values[field];
      
      if (rule.required && !value?.toString().trim()) {
        newErrors[field] = `${field} is required`;
      } else if (rule.minLength && value?.length < rule.minLength) {
        newErrors[field] = `${field} must be at least ${rule.minLength} characters`;
      } else if (rule.pattern && !rule.pattern.test(value)) {
        newErrors[field] = rule.message || `${field} is invalid`;
      }
    });
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
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
    setValue,
    setFieldTouched,
    validate,
    reset
  };
}

// Using Custom Hook
function FormWithCustomHook() {
  const {
    values,
    errors,
    touched,
    setValue,
    setFieldTouched,
    validate,
    reset
  } = useForm(
    {
      username: '',
      email: '',
      password: ''
    },
    {
      username: { required: true, minLength: 3 },
      email: { 
        required: true, 
        pattern: /\S+@\S+\.\S+/,
        message: 'Please enter a valid email'
      },
      password: { required: true, minLength: 6 }
    }
  );

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (validate()) {
      console.log('Form submitted:', values);
      reset();
    }
  };

  const handleChange = (e) => {
    setValue(e.target.name, e.target.value);
  };

  const handleBlur = (e) => {
    setFieldTouched(e.target.name);
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Form with Custom Hook</h2>
      
      <input
        name="username"
        value={values.username}
        onChange={handleChange}
        onBlur={handleBlur}
        placeholder="Username"
      />
      {touched.username && errors.username && (
        <span className="error">{errors.username}</span>
      )}
      
      <input
        name="email"
        type="email"
        value={values.email}
        onChange={handleChange}
        onBlur={handleBlur}
        placeholder="Email"
      />
      {touched.email && errors.email && (
        <span className="error">{errors.email}</span>
      )}
      
      <input
        name="password"
        type="password"
        value={values.password}
        onChange={handleChange}
        onBlur={handleBlur}
        placeholder="Password"
      />
      {touched.password && errors.password && (
        <span className="error">{errors.password}</span>
      )}
      
      <button type="submit">Submit</button>
      <button type="button" onClick={reset}>Reset</button>
    </form>
  );
}

// Main App Component
function App() {
  return (
    <div className="app">
      <ControlledForm />
      <hr />
      <UncontrolledForm />
      <hr />
      <FormWithCustomHook />
    </div>
  );
}

export default App;
```

### Expert Level

#### Q6: How does React's reconciliation algorithm work?
**Answer:**
React's reconciliation algorithm is the process by which React updates the DOM efficiently by comparing the new virtual DOM tree with the previous one and making minimal changes to the actual DOM.

**Example:**
```jsx
import React, { useState, useMemo, useCallback, memo } from 'react';

// Understanding Reconciliation with Keys
function ReconciliationExample() {
  const [items, setItems] = useState([
    { id: 1, name: 'Apple', price: 1.50 },
    { id: 2, name: 'Banana', price: 0.75 },
    { id: 3, name: 'Cherry', price: 2.00 }
  ]);
  const [sortOrder, setSortOrder] = useState('asc');

  // BAD: Using array index as key
  const BadListComponent = () => (
    <div>
      <h3>❌ Bad: Using Index as Key</h3>
      {items.map((item, index) => (
        <div key={index} className="item">
          <span>{item.name}</span>
          <input type="text" defaultValue={item.name} />
          <span>${item.price}</span>
        </div>
      ))}
    </div>
  );

  // GOOD: Using stable unique identifier as key
  const GoodListComponent = () => (
    <div>
      <h3>✅ Good: Using Stable ID as Key</h3>
      {items.map(item => (
        <div key={item.id} className="item">
          <span>{item.name}</span>
          <input type="text" defaultValue={item.name} />
          <span>${item.price}</span>
        </div>
      ))}
    </div>
  );

  const sortItems = () => {
    const sorted = [...items].sort((a, b) => {
      if (sortOrder === 'asc') {
        return a.name.localeCompare(b.name);
      } else {
        return b.name.localeCompare(a.name);
      }
    });
    setItems(sorted);
    setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
  };

  const addItem = () => {
    const newItem = {
      id: Date.now(),
      name: `Item ${items.length + 1}`,
      price: Math.random() * 5
    };
    setItems([...items, newItem]);
  };

  const removeItem = (id) => {
    setItems(items.filter(item => item.id !== id));
  };

  return (
    <div className="reconciliation-demo">
      <h2>React Reconciliation Demo</h2>
      
      <div className="controls">
        <button onClick={sortItems}>
          Sort {sortOrder === 'asc' ? 'Descending' : 'Ascending'}
        </button>
        <button onClick={addItem}>Add Item</button>
      </div>

      <div className="comparison">
        <BadListComponent />
        <GoodListComponent />
      </div>
    </div>
  );
}

// Memoization for Performance Optimization
const ExpensiveListItem = memo(({ item, onUpdate, onDelete }) => {
  console.log(`Rendering item: ${item.name}`);
  
  // Simulate expensive calculation
  const expensiveValue = useMemo(() => {
    console.log(`Calculating expensive value for ${item.name}`);
    let result = 0;
    for (let i = 0; i < 1000000; i++) {
      result += Math.random();
    }
    return result.toFixed(2);
  }, [item.id]); // Only recalculate if item.id changes

  return (
    <div className="expensive-item">
      <h4>{item.name}</h4>
      <p>Price: ${item.price}</p>
      <p>Expensive calculation: {expensiveValue}</p>
      <button onClick={() => onUpdate(item.id)}>Update</button>
      <button onClick={() => onDelete(item.id)}>Delete</button>
    </div>
  );
});

// Parent component with optimized callbacks
function OptimizedParent() {
  const [items, setItems] = useState([
    { id: 1, name: 'Laptop', price: 999.99 },
    { id: 2, name: 'Phone', price: 599.99 },
    { id: 3, name: 'Tablet', price: 399.99 }
  ]);
  const [searchTerm, setSearchTerm] = useState('');

  // Memoized filtered items
  const filteredItems = useMemo(() => {
    console.log('Filtering items...');
    return items.filter(item =>
      item.name.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [items, searchTerm]);

  // Memoized callbacks to prevent unnecessary re-renders
  const handleUpdate = useCallback((id) => {
    setItems(prevItems =>
      prevItems.map(item =>
        item.id === id
          ? { ...item, price: item.price * 1.1 } // Increase price by 10%
          : item
      )
    );
  }, []);

  const handleDelete = useCallback((id) => {
    setItems(prevItems => prevItems.filter(item => item.id !== id));
  }, []);

  return (
    <div className="optimized-parent">
      <h2>Optimized Component Tree</h2>
      
      <input
        type="text"
        placeholder="Search items..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      
      <div className="items-list">
        {filteredItems.map(item => (
          <ExpensiveListItem
            key={item.id}
            item={item}
            onUpdate={handleUpdate}
            onDelete={handleDelete}
          />
        ))}
      </div>
    </div>
  );
}

// Demonstrating React Fiber and Time Slicing
function FiberDemo() {
  const [count, setCount] = useState(0);
  const [isPaused, setIsPaused] = useState(false);

  // Generate large list to demonstrate time slicing
  const generateLargeList = useMemo(() => {
    console.log('Generating large list...');
    return Array.from({ length: 5000 }, (_, i) => ({
      id: i,
      value: Math.random() * 1000
    }));
  }, [count]);

  const handleIncrement = () => {
    setCount(prev => prev + 1);
  };

  return (
    <div className="fiber-demo">
      <h2>React Fiber Demo</h2>
      
      <div className="controls">
        <button onClick={handleIncrement}>
          Increment Count ({count})
        </button>
        <button onClick={() => setIsPaused(!isPaused)}>
          {isPaused ? 'Resume' : 'Pause'} Rendering
        </button>
      </div>

      {!isPaused && (
        <div className="large-list">
          {generateLargeList.map(item => (
            <div key={item.id} className="list-item">
              Item {item.id}: {item.value.toFixed(2)}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

// Advanced Reconciliation Patterns
function AdvancedReconciliation() {
  const [showComponent, setShowComponent] = useState(true);
  const [componentType, setComponentType] = useState('A');

  // Different component types for same position
  const ComponentA = () => (
    <div className="component-a">
      <h3>Component A</h3>
      <input type="text" defaultValue="Component A input" />
    </div>
  );

  const ComponentB = () => (
    <div className="component-b">
      <h3>Component B</h3>
      <input type="text" defaultValue="Component B input" />
    </div>
  );

  return (
    <div className="advanced-reconciliation">
      <h2>Advanced Reconciliation Patterns</h2>
      
      <div className="controls">
        <button onClick={() => setShowComponent(!showComponent)}>
          {showComponent ? 'Hide' : 'Show'} Component
        </button>
        <button onClick={() => setComponentType(componentType === 'A' ? 'B' : 'A')}>
          Switch to Component {componentType === 'A' ? 'B' : 'A'}
        </button>
      </div>

      {/* Conditional rendering affects reconciliation */}
      {showComponent && (
        <div>
          {componentType === 'A' ? <ComponentA /> : <ComponentB />}
        </div>
      )}

      {/* Using key to force re-mounting */}
      <div>
        <h3>Force Re-mount with Key</h3>
        {componentType === 'A' ? (
          <ComponentA key="component-a" />
        ) : (
          <ComponentB key="component-b" />
        )}
      </div>
    </div>
  );
}

// React DevTools Profiler Integration
function ProfilerDemo() {
  const [updates, setUpdates] = useState(0);

  const onRenderCallback = (id, phase, actualDuration, baseDuration, startTime, commitTime) => {
    console.log('Profiler data:', {
      id,
      phase,
      actualDuration,
      baseDuration,
      startTime,
      commitTime
    });
  };

  return (
    <React.Profiler id="ProfilerDemo" onRender={onRenderCallback}>
      <div className="profiler-demo">
        <h2>React Profiler Demo</h2>
        <button onClick={() => setUpdates(prev => prev + 1)}>
          Trigger Update ({updates})
        </button>
        <ExpensiveComponent updates={updates} />
      </div>
    </React.Profiler>
  );
}

const ExpensiveComponent = memo(({ updates }) => {
  const expensiveCalculation = useMemo(() => {
    // Simulate expensive operation
    let result = 0;
    for (let i = 0; i < 100000; i++) {
      result += Math.random();
    }
    return result;
  }, [updates]);

  return (
    <div>
      <p>Updates: {updates}</p>
      <p>Expensive calculation result: {expensiveCalculation.toFixed(2)}</p>
    </div>
  );
});

// Main App
function App() {
  return (
    <div className="app">
      <ReconciliationExample />
      <hr />
      <OptimizedParent />
      <hr />
      <FiberDemo />
      <hr />
      <AdvancedReconciliation />
      <hr />
      <ProfilerDemo />
    </div>
  );
}

export default App;
```

---

## React Hooks

### Beginner Level

#### Q7: What are React Hooks and why were they introduced?
**Answer:**
React Hooks are functions that allow you to use state and other React features in functional components. They were introduced in React 16.8 to solve several problems with class components.

**Problems Hooks Solve:**
- **Complex component logic**: Hard to reuse stateful logic between components
- **Wrapper hell**: Higher-order components and render props created deeply nested components
- **Class confusion**: `this` binding and lifecycle method complexity
- **Bundle size**: Classes don't minify as well as functions

**Example:**
```jsx
import React, { useState, useEffect } from 'react';

// Before Hooks (Class Component)
class CounterClass extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      count: 0,
      name: ''
    };
    this.handleIncrement = this.handleIncrement.bind(this);
  }

  componentDidMount() {
    document.title = `Count: ${this.state.count}`;
  }

  componentDidUpdate() {
    document.title = `Count: ${this.state.count}`;
  }

  handleIncrement() {
    this.setState(prevState => ({
      count: prevState.count + 1
    }));
  }

  render() {
    return (
      <div>
        <p>Count: {this.state.count}</p>
        <button onClick={this.handleIncrement}>+</button>
        <input
          value={this.state.name}
          onChange={(e) => this.setState({ name: e.target.value })}
        />
      </div>
    );
  }
}

// After Hooks (Functional Component)
function CounterHooks() {
  const [count, setCount] = useState(0);
  const [name, setName] = useState('');

  useEffect(() => {
    document.title = `Count: ${count}`;
  }, [count]); // Only run when count changes

  const handleIncrement = () => {
    setCount(prevCount => prevCount + 1);
  };

  return (
    <div>
      <p>Count: {count}</p>
      <button onClick={handleIncrement}>+</button>
      <input
        value={name}
        onChange={(e) => setName(e.target.value)}
      />
    </div>
  );
}

// Rules of Hooks
function HooksRulesDemo() {
  // ✅ Good: Always call hooks at the top level
  const [count, setCount] = useState(0);
  const [isVisible, setIsVisible] = useState(true);

  // ✅ Good: useEffect for side effects
  useEffect(() => {
    console.log('Component updated');
  });

  // ❌ Bad: Don't call hooks inside conditions
  // if (count > 5) {
  //   const [error, setError] = useState('');
  // }

  // ❌ Bad: Don't call hooks inside loops
  // for (let i = 0; i < count; i++) {
  //   useEffect(() => {}, []);
  // }

  // ❌ Bad: Don't call hooks inside functions
  // const handleClick = () => {
  //   const [temp, setTemp] = useState(0);
  // };

  // ✅ Good: Conditional logic inside hooks
  useEffect(() => {
    if (count > 5) {
      console.log('Count is greater than 5');
    }
  }, [count]);

  return (
    <div>
      <h3>Hooks Rules Demo</h3>
      <p>Count: {count}</p>
      <button onClick={() => setCount(count + 1)}>Increment</button>
      <button onClick={() => setIsVisible(!isVisible)}>
        Toggle Visibility
      </button>
      {isVisible && <p>I'm visible!</p>}
    </div>
  );
}
```

#### Q8: How do useState and useEffect work?
**Answer:**
`useState` manages local component state, while `useEffect` handles side effects like API calls, subscriptions, and DOM manipulation.

**Example:**
```jsx
import React, { useState, useEffect } from 'react';

function UserProfile({ userId }) {
  // useState for managing component state
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [posts, setPosts] = useState([]);
  const [followersCount, setFollowersCount] = useState(0);

  // useEffect patterns

  // 1. Effect that runs on every render (no dependency array)
  useEffect(() => {
    console.log('Component rendered');
  });

  // 2. Effect that runs only once (empty dependency array)
  useEffect(() => {
    console.log('Component mounted');
    
    // Cleanup function (equivalent to componentWillUnmount)
    return () => {
      console.log('Component will unmount');
    };
  }, []);

  // 3. Effect that runs when specific values change
  useEffect(() => {
    if (!userId) return;

    setLoading(true);
    setError(null);

    // Simulate API call
    const fetchUser = async () => {
      try {
        const response = await fetch(`/api/users/${userId}`);
        if (!response.ok) {
          throw new Error('Failed to fetch user');
        }
        const userData = await response.json();
        setUser(userData);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, [userId]); // Only run when userId changes

  // 4. Effect with cleanup for subscriptions
  useEffect(() => {
    if (!userId) return;

    const eventSource = new EventSource(`/api/users/${userId}/live-updates`);
    
    eventSource.onmessage = (event) => {
      const data = JSON.parse(event.data);
      if (data.type === 'follower_count') {
        setFollowersCount(data.count);
      }
    };

    // Cleanup function to close connection
    return () => {
      eventSource.close();
    };
  }, [userId]);

  // 5. Effect for fetching related data
  useEffect(() => {
    if (!user) return;

    const fetchUserPosts = async () => {
      try {
        const response = await fetch(`/api/users/${user.id}/posts`);
        const postsData = await response.json();
        setPosts(postsData);
      } catch (err) {
        console.error('Failed to fetch posts:', err);
      }
    };

    fetchUserPosts();
  }, [user]); // Run when user changes

  // useState with function for complex state updates
  const [formData, setFormData] = useState(() => {
    // Lazy initial state - only runs on first render
    const savedData = localStorage.getItem('userFormData');
    return savedData ? JSON.parse(savedData) : {
      name: '',
      email: '',
      bio: ''
    };
  });

  const updateFormData = (field, value) => {
    setFormData(prevData => ({
      ...prevData,
      [field]: value
    }));
  };

  // useState with reducer-like pattern
  const [counters, setCounters] = useState({ likes: 0, shares: 0, comments: 0 });

  const incrementCounter = (type) => {
    setCounters(prev => ({
      ...prev,
      [type]: prev[type] + 1
    }));
  };

  // useEffect for localStorage synchronization
  useEffect(() => {
    localStorage.setItem('userFormData', JSON.stringify(formData));
  }, [formData]);

  // useEffect with conditional execution
  useEffect(() => {
    // Only run effect if certain conditions are met
    if (user && user.isActive && posts.length > 0) {
      console.log('User is active and has posts');
      
      // Track user engagement
      const trackEngagement = () => {
        // Analytics tracking
        console.log('Tracking user engagement');
      };

      trackEngagement();
    }
  }, [user, posts]);

  // Render loading state
  if (loading) {
    return <div className="loading">Loading user profile...</div>;
  }

  // Render error state
  if (error) {
    return (
      <div className="error">
        <p>Error: {error}</p>
        <button onClick={() => window.location.reload()}>
          Retry
        </button>
      </div>
    );
  }

  // Render user profile
  if (!user) {
    return <div>No user found</div>;
  }

  return (
    <div className="user-profile">
      <div className="user-header">
        <img src={user.avatar} alt={user.name} />
        <h1>{user.name}</h1>
        <p>{user.email}</p>
        <p>Followers: {followersCount}</p>
      </div>

      <div className="user-stats">
        <div>Likes: {counters.likes}</div>
        <div>Shares: {counters.shares}</div>
        <div>Comments: {counters.comments}</div>
        <button onClick={() => incrementCounter('likes')}>Like</button>
        <button onClick={() => incrementCounter('shares')}>Share</button>
        <button onClick={() => incrementCounter('comments')}>Comment</button>
      </div>

      <div className="user-posts">
        <h2>Posts ({posts.length})</h2>
        {posts.map(post => (
          <div key={post.id} className="post">
            <h3>{post.title}</h3>
            <p>{post.content}</p>
          </div>
        ))}
      </div>

      <div className="user-form">
        <h3>Update Profile</h3>
        <input
          value={formData.name}
          onChange={(e) => updateFormData('name', e.target.value)}
          placeholder="Name"
        />
        <input
          value={formData.email}
          onChange={(e) => updateFormData('email', e.target.value)}
          placeholder="Email"
        />
        <textarea
          value={formData.bio}
          onChange={(e) => updateFormData('bio', e.target.value)}
          placeholder="Bio"
        />
      </div>
    </div>
  );
}

// Custom hook demonstrating useState and useEffect patterns
function useLocalStorage(key, initialValue) {
  // useState with lazy initialization
  const [storedValue, setStoredValue] = useState(() => {
    try {
      const item = window.localStorage.getItem(key);
      return item ? JSON.parse(item) : initialValue;
    } catch (error) {
      console.error(`Error reading localStorage key "${key}":`, error);
      return initialValue;
    }
  });

  // Return a wrapped version of useState's setter function that persists to localStorage
  const setValue = (value) => {
    try {
      // Allow value to be a function so we have the same API as useState
      const valueToStore = value instanceof Function ? value(storedValue) : value;
      setStoredValue(valueToStore);
      window.localStorage.setItem(key, JSON.stringify(valueToStore));
    } catch (error) {
      console.error(`Error setting localStorage key "${key}":`, error);
    }
  };

  // Listen for changes to this localStorage key from other tabs/windows
  useEffect(() => {
    const handleStorageChange = (e) => {
      if (e.key === key && e.newValue !== null) {
        try {
          setStoredValue(JSON.parse(e.newValue));
        } catch (error) {
          console.error(`Error parsing localStorage key "${key}":`, error);
        }
      }
    };

    window.addEventListener('storage', handleStorageChange);
    
    return () => {
      window.removeEventListener('storage', handleStorageChange);
    };
  }, [key]);

  return [storedValue, setValue];
}

// Component using custom hook
function Settings() {
  const [theme, setTheme] = useLocalStorage('theme', 'light');
  const [notifications, setNotifications] = useLocalStorage('notifications', true);

  return (
    <div className={`settings ${theme}`}>
      <h2>Settings</h2>
      
      <label>
        Theme:
        <select value={theme} onChange={(e) => setTheme(e.target.value)}>
          <option value="light">Light</option>
          <option value="dark">Dark</option>
        </select>
      </label>
      
      <label>
        <input
          type="checkbox"
          checked={notifications}
          onChange={(e) => setNotifications(e.target.checked)}
        />
        Enable notifications
      </label>
    </div>
  );
}

export default UserProfile;
```

### Intermediate Level

#### Q9: What are useContext, useReducer, and how do you create custom hooks?
**Answer:**
`useContext` provides a way to consume context values, `useReducer` manages complex state logic, and custom hooks allow you to extract and reuse stateful logic.

**Example:**
```jsx
import React, { useContext, useReducer, createContext, useMemo, useCallback, useState, useEffect } from 'react';

// 1. useContext for Global State Management
const ThemeContext = createContext();
const UserContext = createContext();

// Theme Provider
function ThemeProvider({ children }) {
  const [theme, setTheme] = useState('light');
  const [fontSize, setFontSize] = useState('medium');

  const toggleTheme = () => {
    setTheme(prev => prev === 'light' ? 'dark' : 'light');
  };

  const changeFontSize = (size) => {
    setFontSize(size);
  };

  const value = {
    theme,
    fontSize,
    toggleTheme,
    changeFontSize
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

// 2. useReducer for Complex State Management
const initialState = {
  user: null,
  loading: false,
  error: null,
  posts: [],
  notifications: [],
  preferences: {
    language: 'en',
    timezone: 'UTC'
  }
};

function userReducer(state, action) {
  switch (action.type) {
    case 'FETCH_USER_START':
      return {
        ...state,
        loading: true,
        error: null
      };

    case 'FETCH_USER_SUCCESS':
      return {
        ...state,
        loading: false,
        user: action.payload,
        error: null
      };

    case 'FETCH_USER_ERROR':
      return {
        ...state,
        loading: false,
        error: action.payload
      };

    case 'ADD_POST':
      return {
        ...state,
        posts: [...state.posts, action.payload]
      };

    case 'UPDATE_POST':
      return {
        ...state,
        posts: state.posts.map(post =>
          post.id === action.payload.id ? action.payload : post
        )
      };

    case 'DELETE_POST':
      return {
        ...state,
        posts: state.posts.filter(post => post.id !== action.payload)
      };

    case 'ADD_NOTIFICATION':
      return {
        ...state,
        notifications: [...state.notifications, {
          id: Date.now(),
          message: action.payload,
          timestamp: new Date().toISOString()
        }]
      };

    case 'REMOVE_NOTIFICATION':
      return {
        ...state,
        notifications: state.notifications.filter(
          notification => notification.id !== action.payload
        )
      };

    case 'UPDATE_PREFERENCES':
      return {
        ...state,
        preferences: {
          ...state.preferences,
          ...action.payload
        }
      };

    case 'RESET_STATE':
      return initialState;

    default:
      return state;
  }
}

// User Provider with useReducer
function UserProvider({ children }) {
  const [state, dispatch] = useReducer(userReducer, initialState);

  // Action creators
  const fetchUser = useCallback(async (userId) => {
    dispatch({ type: 'FETCH_USER_START' });
    
    try {
      const response = await fetch(`/api/users/${userId}`);
      if (!response.ok) {
        throw new Error('Failed to fetch user');
      }
      const user = await response.json();
      dispatch({ type: 'FETCH_USER_SUCCESS', payload: user });
    } catch (error) {
      dispatch({ type: 'FETCH_USER_ERROR', payload: error.message });
    }
  }, []);

  const addPost = useCallback((post) => {
    const newPost = {
      ...post,
      id: Date.now(),
      createdAt: new Date().toISOString()
    };
    dispatch({ type: 'ADD_POST', payload: newPost });
    dispatch({ type: 'ADD_NOTIFICATION', payload: 'Post added successfully!' });
  }, []);

  const updatePost = useCallback((updatedPost) => {
    dispatch({ type: 'UPDATE_POST', payload: updatedPost });
    dispatch({ type: 'ADD_NOTIFICATION', payload: 'Post updated successfully!' });
  }, []);

  const deletePost = useCallback((postId) => {
    dispatch({ type: 'DELETE_POST', payload: postId });
    dispatch({ type: 'ADD_NOTIFICATION', payload: 'Post deleted successfully!' });
  }, []);

  const removeNotification = useCallback((notificationId) => {
    dispatch({ type: 'REMOVE_NOTIFICATION', payload: notificationId });
  }, []);

  const updatePreferences = useCallback((newPreferences) => {
    dispatch({ type: 'UPDATE_PREFERENCES', payload: newPreferences });
  }, []);

  const value = {
    ...state,
    fetchUser,
    addPost,
    updatePost,
    deletePost,
    removeNotification,
    updatePreferences
  };

  return (
    <UserContext.Provider value={value}>
      {children}
    </UserContext.Provider>
  );
}

// Custom hook to use user context
function useUser() {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error('useUser must be used within a UserProvider');
  }
  return context;
}

// 3. Custom Hooks Examples

// Custom hook for API calls
function useApi(url, options = {}) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await fetch(url, options);
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
  }, [url, options]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return { data, loading, error, refetch: fetchData };
}

// Custom hook for debounced value
function useDebounce(value, delay) {
  const [debouncedValue, setDebouncedValue] = useState(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
}

// Custom hook for form handling
function useForm(initialValues, validationRules = {}) {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const setValue = useCallback((name, value) => {
    setValues(prev => ({ ...prev, [name]: value }));
    
    // Clear error when value changes
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  }, [errors]);

  const setFieldTouched = useCallback((name) => {
    setTouched(prev => ({ ...prev, [name]: true }));
  }, []);

  const validate = useCallback(() => {
    const newErrors = {};

    Object.keys(validationRules).forEach(field => {
      const rules = validationRules[field];
      const value = values[field];

      if (rules.required && !value?.toString().trim()) {
        newErrors[field] = `${field} is required`;
      } else if (rules.minLength && value?.length < rules.minLength) {
        newErrors[field] = `${field} must be at least ${rules.minLength} characters`;
      } else if (rules.pattern && !rules.pattern.test(value)) {
        newErrors[field] = rules.message || `${field} is invalid`;
      } else if (rules.custom && !rules.custom(value, values)) {
        newErrors[field] = rules.customMessage || `${field} is invalid`;
      }
    });

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }, [values, validationRules]);

  const handleSubmit = useCallback(async (onSubmit) => {
    setIsSubmitting(true);
    
    // Mark all fields as touched
    const allTouched = {};
    Object.keys(values).forEach(key => {
      allTouched[key] = true;
    });
    setTouched(allTouched);

    if (validate()) {
      try {
        await onSubmit(values);
      } catch (error) {
        console.error('Form submission error:', error);
      }
    }
    
    setIsSubmitting(false);
  }, [values, validate]);

  const reset = useCallback(() => {
    setValues(initialValues);
    setErrors({});
    setTouched({});
    setIsSubmitting(false);
  }, [initialValues]);

  return {
    values,
    errors,
    touched,
    isSubmitting,
    setValue,
    setFieldTouched,
    validate,
    handleSubmit,
    reset
  };
}

// Custom hook for online status
function useOnlineStatus() {
  const [isOnline, setIsOnline] = useState(navigator.onLine);

  useEffect(() => {
    const handleOnline = () => setIsOnline(true);
    const handleOffline = () => setIsOnline(false);

    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, []);

  return isOnline;
}

// Custom hook for window size
function useWindowSize() {
  const [windowSize, setWindowSize] = useState({
    width: window.innerWidth,
    height: window.innerHeight
  });

  useEffect(() => {
    const handleResize = () => {
      setWindowSize({
        width: window.innerWidth,
        height: window.innerHeight
      });
    };

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  return windowSize;
}

// Component using multiple hooks
function Dashboard() {
  const { theme, toggleTheme } = useTheme();
  const { user, posts, notifications, addPost, removeNotification } = useUser();
  const isOnline = useOnlineStatus();
  const windowSize = useWindowSize();
  
  const [searchTerm, setSearchTerm] = useState('');
  const debouncedSearchTerm = useDebounce(searchTerm, 300);
  
  const { data: searchResults, loading: searchLoading } = useApi(
    debouncedSearchTerm ? `/api/search?q=${debouncedSearchTerm}` : null
  );

  const {
    values,
    errors,
    touched,
    isSubmitting,
    setValue,
    setFieldTouched,
    handleSubmit
  } = useForm(
    { title: '', content: '' },
    {
      title: { required: true, minLength: 3 },
      content: { required: true, minLength: 10 }
    }
  );

  const onSubmitPost = async (formValues) => {
    addPost(formValues);
    // Reset form after successful submission
    setTimeout(() => {
      setValue('title', '');
      setValue('content', '');
    }, 100);
  };

  // Filter posts based on search
  const filteredPosts = useMemo(() => {
    if (!debouncedSearchTerm) return posts;
    
    return posts.filter(post =>
      post.title.toLowerCase().includes(debouncedSearchTerm.toLowerCase()) ||
      post.content.toLowerCase().includes(debouncedSearchTerm.toLowerCase())
    );
  }, [posts, debouncedSearchTerm]);

  return (
    <div className={`dashboard ${theme}`}>
      <header>
        <h1>Dashboard</h1>
        <div className="status">
          <span>Status: {isOnline ? '🟢 Online' : '🔴 Offline'}</span>
          <span>Size: {windowSize.width}x{windowSize.height}</span>
          <button onClick={toggleTheme}>
            Switch to {theme === 'light' ? 'dark' : 'light'} theme
          </button>
        </div>
      </header>

      {/* Notifications */}
      <div className="notifications">
        {notifications.map(notification => (
          <div key={notification.id} className="notification">
            <span>{notification.message}</span>
            <button onClick={() => removeNotification(notification.id)}>
              ×
            </button>
          </div>
        ))}
      </div>

      {/* Search */}
      <div className="search">
        <input
          type="text"
          placeholder="Search posts..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
        {searchLoading && <span>Searching...</span>}
      </div>

      {/* Add Post Form */}
      <form
        onSubmit={(e) => {
          e.preventDefault();
          handleSubmit(onSubmitPost);
        }}
        className="add-post-form"
      >
        <h3>Add New Post</h3>
        
        <input
          type="text"
          placeholder="Post title"
          value={values.title}
          onChange={(e) => setValue('title', e.target.value)}
          onBlur={() => setFieldTouched('title')}
        />
        {touched.title && errors.title && (
          <span className="error">{errors.title}</span>
        )}
        
        <textarea
          placeholder="Post content"
          value={values.content}
          onChange={(e) => setValue('content', e.target.value)}
          onBlur={() => setFieldTouched('content')}
          rows={4}
        />
        {touched.content && errors.content && (
          <span className="error">{errors.content}</span>
        )}
        
        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Adding...' : 'Add Post'}
        </button>
      </form>

      {/* Posts List */}
      <div className="posts">
        <h3>Posts ({filteredPosts.length})</h3>
        {filteredPosts.map(post => (
          <div key={post.id} className="post">
            <h4>{post.title}</h4>
            <p>{post.content}</p>
            <small>{new Date(post.createdAt).toLocaleDateString()}</small>
          </div>
        ))}
      </div>

      {/* Search Results */}
      {searchResults && (
        <div className="search-results">
          <h3>Search Results</h3>
          {searchResults.map(result => (
            <div key={result.id} className="search-result">
              {result.title}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

// App with providers
function App() {
  return (
    <ThemeProvider>
      <UserProvider>
        <Dashboard />
      </UserProvider>
    </ThemeProvider>
  );
}

export default App;
```

### Expert Level

#### Q10: How do you optimize performance with useMemo, useCallback, and create advanced custom hooks?
**Answer:**
Performance optimization involves preventing unnecessary re-renders and computations using memoization techniques and creating sophisticated custom hooks for complex logic.

**Example:**
```jsx
import React, { useState, useMemo, useCallback, useRef, useEffect, memo, forwardRef, useImperativeHandle } from 'react';

// 1. Performance Optimization with useMemo and useCallback

// Expensive calculation function
const expensiveCalculation = (data, filters) => {
  console.log('Running expensive calculation...');
  
  // Simulate expensive operation
  let result = data;
  
  // Apply filters
  if (filters.category) {
    result = result.filter(item => item.category === filters.category);
  }
  
  if (filters.minPrice) {
    result = result.filter(item => item.price >= filters.minPrice);
  }
  
  if (filters.searchTerm) {
    result = result.filter(item => 
      item.name.toLowerCase().includes(filters.searchTerm.toLowerCase())
    );
  }
  
  // Sort by relevance score (expensive operation)
  result = result.map(item => ({
    ...item,
    relevanceScore: calculateRelevanceScore(item, filters)
  })).sort((a, b) => b.relevanceScore - a.relevanceScore);
  
  return result;
};

const calculateRelevanceScore = (item, filters) => {
  // Simulate expensive scoring algorithm
  let score = 0;
  for (let i = 0; i < 10000; i++) {
    score += Math.random();
  }
  return score;
};

// Memoized expensive list item component
const ExpensiveListItem = memo(({ item, onSelect, onUpdate, selectedId }) => {
  console.log(`Rendering item: ${item.name}`);
  
  // Expensive rendering calculation
  const renderingData = useMemo(() => {
    console.log(`Computing rendering data for ${item.name}`);
    
    // Simulate expensive formatting
    return {
      formattedPrice: new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(item.price),
      formattedDate: new Date(item.createdAt).toLocaleDateString(),
      isExpensive: item.price > 100,
      discountedPrice: item.price * 0.9
    };
  }, [item.price, item.createdAt]);
  
  const isSelected = selectedId === item.id;
  
  return (
    <div className={`list-item ${isSelected ? 'selected' : ''}`}>
      <h3>{item.name}</h3>
      <p>Price: {renderingData.formattedPrice}</p>
      <p>Created: {renderingData.formattedDate}</p>
      {renderingData.isExpensive && <span className="expensive-badge">Premium</span>}
      <p>Discounted: {renderingData.discountedPrice}</p>
      <button onClick={() => onSelect(item.id)}>
        {isSelected ? 'Deselect' : 'Select'}
      </button>
      <button onClick={() => onUpdate(item.id)}>Update</button>
    </div>
  );
});

// Main optimized component
function OptimizedProductList() {
  const [products] = useState(() => {
    // Generate initial data
    return Array.from({ length: 1000 }, (_, i) => ({
      id: i,
      name: `Product ${i}`,
      price: Math.random() * 200,
      category: ['electronics', 'clothing', 'books'][Math.floor(Math.random() * 3)],
      createdAt: new Date(Date.now() - Math.random() * 10000000000).toISOString()
    }));
  });
  
  const [filters, setFilters] = useState({
    category: '',
    minPrice: 0,
    searchTerm: ''
  });
  
  const [selectedId, setSelectedId] = useState(null);
  const [sortOrder, setSortOrder] = useState('asc');
  
  // Memoized filtered and sorted products
  const filteredProducts = useMemo(() => {
    return expensiveCalculation(products, filters);
  }, [products, filters]);
  
  // Memoized sorted products
  const sortedProducts = useMemo(() => {
    console.log('Sorting products...');
    return [...filteredProducts].sort((a, b) => {
      if (sortOrder === 'asc') {
        return a.price - b.price;
      } else {
        return b.price - a.price;
      }
    });
  }, [filteredProducts, sortOrder]);
  
  // Memoized statistics
  const statistics = useMemo(() => {
    console.log('Calculating statistics...');
    
    if (filteredProducts.length === 0) {
      return { average: 0, min: 0, max: 0, total: 0 };
    }
    
    const prices = filteredProducts.map(p => p.price);
    return {
      average: prices.reduce((sum, price) => sum + price, 0) / prices.length,
      min: Math.min(...prices),
      max: Math.max(...prices),
      total: filteredProducts.length
    };
  }, [filteredProducts]);
  
  // Memoized callbacks to prevent child re-renders
  const handleSelectProduct = useCallback((productId) => {
    setSelectedId(prev => prev === productId ? null : productId);
  }, []);
  
  const handleUpdateProduct = useCallback((productId) => {
    console.log(`Updating product ${productId}`);
    // Update logic here
  }, []);
  
  const handleFilterChange = useCallback((filterType, value) => {
    setFilters(prev => ({
      ...prev,
      [filterType]: value
    }));
  }, []);
  
  const handleSortToggle = useCallback(() => {
    setSortOrder(prev => prev === 'asc' ? 'desc' : 'asc');
  }, []);
  
  // Debounced search to avoid excessive filtering
  const [searchInput, setSearchInput] = useState('');
  const searchTimeoutRef = useRef();
  
  useEffect(() => {
    clearTimeout(searchTimeoutRef.current);
    searchTimeoutRef.current = setTimeout(() => {
      handleFilterChange('searchTerm', searchInput);
    }, 300);
    
    return () => clearTimeout(searchTimeoutRef.current);
  }, [searchInput, handleFilterChange]);
  
  return (
    <div className="optimized-product-list">
      <div className="filters">
        <input
          type="text"
          placeholder="Search products..."
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
        />
        
        <select
          value={filters.category}
          onChange={(e) => handleFilterChange('category', e.target.value)}
        >
          <option value="">All Categories</option>
          <option value="electronics">Electronics</option>
          <option value="clothing">Clothing</option>
          <option value="books">Books</option>
        </select>
        
        <input
          type="number"
          placeholder="Min Price"
          value={filters.minPrice}
          onChange={(e) => handleFilterChange('minPrice', Number(e.target.value))}
        />
        
        <button onClick={handleSortToggle}>
          Sort by Price ({sortOrder})
        </button>
      </div>
      
      <div className="statistics">
        <h3>Statistics</h3>
        <p>Total Products: {statistics.total}</p>
        <p>Average Price: ${statistics.average.toFixed(2)}</p>
        <p>Price Range: ${statistics.min.toFixed(2)} - ${statistics.max.toFixed(2)}</p>
      </div>
      
      <div className="products-grid">
        {sortedProducts.map(product => (
          <ExpensiveListItem
            key={product.id}
            item={product}
            onSelect={handleSelectProduct}
            onUpdate={handleUpdateProduct}
            selectedId={selectedId}
          />
        ))}
      </div>
    </div>
  );
}

// 2. Advanced Custom Hooks

// Advanced data fetching hook with caching
function useAdvancedFetch(url, options = {}) {
  const [state, setState] = useState({
    data: null,
    loading: false,
    error: null
  });
  
  const cacheRef = useRef(new Map());
  const abortControllerRef = useRef();
  
  const fetchData = useCallback(async (fetchUrl = url, fetchOptions = options) => {
    // Check cache first
    const cacheKey = JSON.stringify({ url: fetchUrl, options: fetchOptions });
    if (cacheRef.current.has(cacheKey)) {
      setState(prev => ({
        ...prev,
        data: cacheRef.current.get(cacheKey),
        loading: false
      }));
      return cacheRef.current.get(cacheKey);
    }
    
    // Cancel previous request
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    
    abortControllerRef.current = new AbortController();
    
    setState(prev => ({ ...prev, loading: true, error: null }));
    
    try {
      const response = await fetch(fetchUrl, {
        ...fetchOptions,
        signal: abortControllerRef.current.signal
      });
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      
      // Cache the result
      cacheRef.current.set(cacheKey, data);
      
      setState({ data, loading: false, error: null });
      return data;
    } catch (error) {
      if (error.name !== 'AbortError') {
        setState(prev => ({ ...prev, loading: false, error: error.message }));
        throw error;
      }
    }
  }, [url, options]);
  
  useEffect(() => {
    fetchData();
    
    return () => {
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
    };
  }, [fetchData]);
  
  const clearCache = useCallback(() => {
    cacheRef.current.clear();
  }, []);
  
  return {
    ...state,
    refetch: fetchData,
    clearCache
  };
}

// Advanced form hook with validation and submission
function useAdvancedForm({
  initialValues,
  validationSchema,
  onSubmit,
  submitOnChange = false
}) {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isDirty, setIsDirty] = useState(false);
  
  const validateField = useCallback((fieldName, value) => {
    const fieldValidation = validationSchema[fieldName];
    if (!fieldValidation) return '';
    
    if (typeof fieldValidation === 'function') {
      return fieldValidation(value, values) || '';
    }
    
    // Schema-based validation
    let error = '';
    
    if (fieldValidation.required && !value?.toString().trim()) {
      error = fieldValidation.requiredMessage || `${fieldName} is required`;
    } else if (fieldValidation.min && value < fieldValidation.min) {
      error = `${fieldName} must be at least ${fieldValidation.min}`;
    } else if (fieldValidation.max && value > fieldValidation.max) {
      error = `${fieldName} must be no more than ${fieldValidation.max}`;
    } else if (fieldValidation.pattern && !fieldValidation.pattern.test(value)) {
      error = fieldValidation.message || `${fieldName} is invalid`;
    }
    
    return error;
  }, [validationSchema, values]);
  
  const validateAllFields = useCallback(() => {
    const newErrors = {};
    Object.keys(validationSchema).forEach(fieldName => {
      const error = validateField(fieldName, values[fieldName]);
      if (error) {
        newErrors[fieldName] = error;
      }
    });
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }, [validationSchema, values, validateField]);
  
  const setValue = useCallback((fieldName, value) => {
    setValues(prev => {
      const newValues = { ...prev, [fieldName]: value };
      setIsDirty(JSON.stringify(newValues) !== JSON.stringify(initialValues));
      return newValues;
    });
    
    // Validate field on change
    const error = validateField(fieldName, value);
    setErrors(prev => ({ ...prev, [fieldName]: error }));
    
    // Submit on change if enabled
    if (submitOnChange && !error) {
      handleSubmit();
    }
  }, [initialValues, validateField, submitOnChange]);
  
  const setFieldTouched = useCallback((fieldName, isTouched = true) => {
    setTouched(prev => ({ ...prev, [fieldName]: isTouched }));
  }, []);
  
  const handleSubmit = useCallback(async (e) => {
    if (e) e.preventDefault();
    
    setIsSubmitting(true);
    
    // Mark all fields as touched
    const allTouched = {};
    Object.keys(values).forEach(key => {
      allTouched[key] = true;
    });
    setTouched(allTouched);
    
    if (validateAllFields()) {
      try {
        await onSubmit(values);
        setIsDirty(false);
      } catch (error) {
        console.error('Form submission error:', error);
      }
    }
    
    setIsSubmitting(false);
  }, [values, validateAllFields, onSubmit]);
  
  const reset = useCallback(() => {
    setValues(initialValues);
    setErrors({});
    setTouched({});
    setIsSubmitting(false);
    setIsDirty(false);
  }, [initialValues]);
  
  const setFieldError = useCallback((fieldName, error) => {
    setErrors(prev => ({ ...prev, [fieldName]: error }));
  }, []);
  
  return {
    values,
    errors,
    touched,
    isSubmitting,
    isDirty,
    setValue,
    setFieldTouched,
    setFieldError,
    handleSubmit,
    reset,
    validateField,
    validateAllFields
  };
}

// Advanced pagination hook
function usePagination(data, itemsPerPage = 10) {
  const [currentPage, setCurrentPage] = useState(1);
  
  const totalPages = Math.ceil(data.length / itemsPerPage);
  
  const paginatedData = useMemo(() => {
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return data.slice(startIndex, endIndex);
  }, [data, currentPage, itemsPerPage]);
  
  const goToPage = useCallback((page) => {
    setCurrentPage(Math.max(1, Math.min(page, totalPages)));
  }, [totalPages]);
  
  const nextPage = useCallback(() => {
    goToPage(currentPage + 1);
  }, [currentPage, goToPage]);
  
  const prevPage = useCallback(() => {
    goToPage(currentPage - 1);
  }, [currentPage, goToPage]);
  
  const goToFirstPage = useCallback(() => {
    goToPage(1);
  }, [goToPage]);
  
  const goToLastPage = useCallback(() => {
    goToPage(totalPages);
  }, [goToPage, totalPages]);
  
  return {
    currentPage,
    totalPages,
    paginatedData,
    goToPage,
    nextPage,
    prevPage,
    goToFirstPage,
    goToLastPage,
    hasNextPage: currentPage < totalPages,
    hasPrevPage: currentPage > 1
  };
}

// Component using advanced hooks
function AdvancedFormExample() {
  const {
    values,
    errors,
    touched,
    isSubmitting,
    isDirty,
    setValue,
    setFieldTouched,
    handleSubmit
  } = useAdvancedForm({
    initialValues: {
      name: '',
      email: '',
      age: '',
      password: '',
      confirmPassword: ''
    },
    validationSchema: {
      name: {
        required: true,
        requiredMessage: 'Name is required'
      },
      email: {
        required: true,
        pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
        message: 'Please enter a valid email address'
      },
      age: {
        required: true,
        min: 18,
        max: 120
      },
      password: {
        required: true,
        pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/,
        message: 'Password must contain at least 8 characters, one uppercase, one lowercase, and one number'
      },
      confirmPassword: (value, allValues) => {
        if (value !== allValues.password) {
          return 'Passwords do not match';
        }
        return '';
      }
    },
    onSubmit: async (formData) => {
      console.log('Submitting form:', formData);
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 2000));
    }
  });
  
  return (
    <form onSubmit={handleSubmit} className="advanced-form">
      <h2>Advanced Form Example</h2>
      
      <div className="form-field">
        <input
          type="text"
          placeholder="Name"
          value={values.name}
          onChange={(e) => setValue('name', e.target.value)}
          onBlur={() => setFieldTouched('name')}
        />
        {touched.name && errors.name && (
          <span className="error">{errors.name}</span>
        )}
      </div>
      
      <div className="form-field">
        <input
          type="email"
          placeholder="Email"
          value={values.email}
          onChange={(e) => setValue('email', e.target.value)}
          onBlur={() => setFieldTouched('email')}
        />
        {touched.email && errors.email && (
          <span className="error">{errors.email}</span>
        )}
      </div>
      
      <div className="form-field">
        <input
          type="number"
          placeholder="Age"
          value={values.age}
          onChange={(e) => setValue('age', e.target.value)}
          onBlur={() => setFieldTouched('age')}
        />
        {touched.age && errors.age && (
          <span className="error">{errors.age}</span>
        )}
      </div>
      
      <div className="form-field">
        <input
          type="password"
          placeholder="Password"
          value={values.password}
          onChange={(e) => setValue('password', e.target.value)}
          onBlur={() => setFieldTouched('password')}
        />
        {touched.password && errors.password && (
          <span className="error">{errors.password}</span>
        )}
      </div>
      
      <div className="form-field">
        <input
          type="password"
          placeholder="Confirm Password"
          value={values.confirmPassword}
          onChange={(e) => setValue('confirmPassword', e.target.value)}
          onBlur={() => setFieldTouched('confirmPassword')}
        />
        {touched.confirmPassword && errors.confirmPassword && (
          <span className="error">{errors.confirmPassword}</span>
        )}
      </div>
      
      <button type="submit" disabled={isSubmitting || !isDirty}>
        {isSubmitting ? 'Submitting...' : 'Submit'}
      </button>
    </form>
  );
}

export default OptimizedProductList;
```

---

## State Management

### Beginner Level

#### Q11: What are different approaches to state management in React?
**Answer:**
React offers several approaches to manage state, from local component state to global application state management solutions.

**State Management Approaches:**

**Example:**
```jsx
import React, { useState, useReducer, useContext, createContext } from 'react';

// 1. Local Component State (useState)
function LocalStateExample() {
  const [count, setCount] = useState(0);
  const [user, setUser] = useState({ name: '', email: '' });
  const [todos, setTodos] = useState([]);

  const addTodo = (text) => {
    const newTodo = {
      id: Date.now(),
      text,
      completed: false
    };
    setTodos(prev => [...prev, newTodo]);
  };

  const toggleTodo = (id) => {
    setTodos(prev => 
      prev.map(todo => 
        todo.id === id ? { ...todo, completed: !todo.completed } : todo
      )
    );
  };

  return (
    <div>
      <h3>Local State Management</h3>
      
      {/* Simple state */}
      <div>
        <p>Count: {count}</p>
        <button onClick={() => setCount(count + 1)}>+</button>
        <button onClick={() => setCount(count - 1)}>-</button>
      </div>
      
      {/* Object state */}
      <div>
        <input
          placeholder="Name"
          value={user.name}
          onChange={(e) => setUser({...user, name: e.target.value})}
        />
        <input
          placeholder="Email"
          value={user.email}
          onChange={(e) => setUser({...user, email: e.target.value})}
        />
      </div>
      
      {/* Array state */}
      <div>
        <h4>Todos</h4>
        <button onClick={() => addTodo(`Todo ${todos.length + 1}`)}>
          Add Todo
        </button>
        {todos.map(todo => (
          <div key={todo.id}>
            <span 
              style={{ textDecoration: todo.completed ? 'line-through' : 'none' }}
              onClick={() => toggleTodo(todo.id)}
            >
              {todo.text}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}

// 2. useReducer for Complex State Logic
const todoReducer = (state, action) => {
  switch (action.type) {
    case 'ADD_TODO':
      return {
        ...state,
        todos: [...state.todos, {
          id: Date.now(),
          text: action.payload,
          completed: false
        }]
      };
    
    case 'TOGGLE_TODO':
      return {
        ...state,
        todos: state.todos.map(todo =>
          todo.id === action.payload
            ? { ...todo, completed: !todo.completed }
            : todo
        )
      };
    
    case 'DELETE_TODO':
      return {
        ...state,
        todos: state.todos.filter(todo => todo.id !== action.payload)
      };
    
    case 'SET_FILTER':
      return {
        ...state,
        filter: action.payload
      };
    
    default:
      return state;
  }
};

function ReducerStateExample() {
  const [state, dispatch] = useReducer(todoReducer, {
    todos: [],
    filter: 'all' // all, active, completed
  });

  const addTodo = (text) => {
    dispatch({ type: 'ADD_TODO', payload: text });
  };

  const toggleTodo = (id) => {
    dispatch({ type: 'TOGGLE_TODO', payload: id });
  };

  const deleteTodo = (id) => {
    dispatch({ type: 'DELETE_TODO', payload: id });
  };

  const setFilter = (filter) => {
    dispatch({ type: 'SET_FILTER', payload: filter });
  };

  const filteredTodos = state.todos.filter(todo => {
    if (state.filter === 'active') return !todo.completed;
    if (state.filter === 'completed') return todo.completed;
    return true;
  });

  return (
    <div>
      <h3>useReducer State Management</h3>
      
      <div>
        <button onClick={() => addTodo(`Todo ${state.todos.length + 1}`)}>
          Add Todo
        </button>
      </div>
      
      <div>
        <button 
          onClick={() => setFilter('all')}
          style={{ fontWeight: state.filter === 'all' ? 'bold' : 'normal' }}
        >
          All
        </button>
        <button 
          onClick={() => setFilter('active')}
          style={{ fontWeight: state.filter === 'active' ? 'bold' : 'normal' }}
        >
          Active
        </button>
        <button 
          onClick={() => setFilter('completed')}
          style={{ fontWeight: state.filter === 'completed' ? 'bold' : 'normal' }}
        >
          Completed
        </button>
      </div>
      
      <div>
        {filteredTodos.map(todo => (
          <div key={todo.id} style={{ display: 'flex', alignItems: 'center' }}>
            <span 
              style={{ 
                textDecoration: todo.completed ? 'line-through' : 'none',
                cursor: 'pointer',
                flex: 1
              }}
              onClick={() => toggleTodo(todo.id)}
            >
              {todo.text}
            </span>
            <button onClick={() => deleteTodo(todo.id)}>Delete</button>
          </div>
        ))}
      </div>
    </div>
  );
}

// 3. Context API for Global State
const AppContext = createContext();

function AppProvider({ children }) {
  const [state, setState] = useState({
    user: null,
    theme: 'light',
    notifications: [],
    settings: {
      language: 'en',
      timezone: 'UTC'
    }
  });

  const login = (userData) => {
    setState(prev => ({ ...prev, user: userData }));
  };

  const logout = () => {
    setState(prev => ({ ...prev, user: null }));
  };

  const toggleTheme = () => {
    setState(prev => ({
      ...prev,
      theme: prev.theme === 'light' ? 'dark' : 'light'
    }));
  };

  const addNotification = (message, type = 'info') => {
    const notification = {
      id: Date.now(),
      message,
      type,
      timestamp: new Date().toISOString()
    };
    setState(prev => ({
      ...prev,
      notifications: [...prev.notifications, notification]
    }));
  };

  const removeNotification = (id) => {
    setState(prev => ({
      ...prev,
      notifications: prev.notifications.filter(n => n.id !== id)
    }));
  };

  const updateSettings = (newSettings) => {
    setState(prev => ({
      ...prev,
      settings: { ...prev.settings, ...newSettings }
    }));
  };

  const contextValue = {
    ...state,
    login,
    logout,
    toggleTheme,
    addNotification,
    removeNotification,
    updateSettings
  };

  return (
    <AppContext.Provider value={contextValue}>
      {children}
    </AppContext.Provider>
  );
}

// Custom hook to use app context
function useApp() {
  const context = useContext(AppContext);
  if (!context) {
    throw new Error('useApp must be used within an AppProvider');
  }
  return context;
}

// Components using Context
function Header() {
  const { user, theme, toggleTheme, logout } = useApp();

  return (
    <header style={{ 
      background: theme === 'light' ? '#fff' : '#333',
      color: theme === 'light' ? '#333' : '#fff',
      padding: '1rem'
    }}>
      <h1>My App</h1>
      <div>
        {user ? (
          <div>
            <span>Welcome, {user.name}!</span>
            <button onClick={logout}>Logout</button>
          </div>
        ) : (
          <span>Please log in</span>
        )}
        <button onClick={toggleTheme}>
          Switch to {theme === 'light' ? 'dark' : 'light'} theme
        </button>
      </div>
    </header>
  );
}

function LoginForm() {
  const { login, addNotification } = useApp();
  const [credentials, setCredentials] = useState({ name: '', email: '' });

  const handleSubmit = (e) => {
    e.preventDefault();
    if (credentials.name && credentials.email) {
      login(credentials);
      addNotification('Login successful!', 'success');
      setCredentials({ name: '', email: '' });
    } else {
      addNotification('Please fill in all fields', 'error');
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h3>Login</h3>
      <input
        type="text"
        placeholder="Name"
        value={credentials.name}
        onChange={(e) => setCredentials({...credentials, name: e.target.value})}
      />
      <input
        type="email"
        placeholder="Email"
        value={credentials.email}
        onChange={(e) => setCredentials({...credentials, email: e.target.value})}
      />
      <button type="submit">Login</button>
    </form>
  );
}

function Notifications() {
  const { notifications, removeNotification } = useApp();

  return (
    <div className="notifications">
      {notifications.map(notification => (
        <div
          key={notification.id}
          style={{
            padding: '0.5rem',
            margin: '0.5rem 0',
            backgroundColor: notification.type === 'error' ? '#ffebee' : 
                            notification.type === 'success' ? '#e8f5e8' : '#e3f2fd',
            border: '1px solid',
            borderColor: notification.type === 'error' ? '#f44336' : 
                        notification.type === 'success' ? '#4caf50' : '#2196f3'
          }}
        >
          <span>{notification.message}</span>
          <button 
            onClick={() => removeNotification(notification.id)}
            style={{ marginLeft: '1rem' }}
          >
            ×
          </button>
        </div>
      ))}
    </div>
  );
}

// 4. Lifting State Up Pattern
function ShoppingCart() {
  const [cart, setCart] = useState([]);
  const [products] = useState([
    { id: 1, name: 'Laptop', price: 999 },
    { id: 2, name: 'Phone', price: 599 },
    { id: 3, name: 'Tablet', price: 399 }
  ]);

  const addToCart = (product) => {
    setCart(prev => {
      const existingItem = prev.find(item => item.id === product.id);
      if (existingItem) {
        return prev.map(item =>
          item.id === product.id
            ? { ...item, quantity: item.quantity + 1 }
            : item
        );
      } else {
        return [...prev, { ...product, quantity: 1 }];
      }
    });
  };

  const removeFromCart = (productId) => {
    setCart(prev => prev.filter(item => item.id !== productId));
  };

  const updateQuantity = (productId, newQuantity) => {
    if (newQuantity === 0) {
      removeFromCart(productId);
    } else {
      setCart(prev =>
        prev.map(item =>
          item.id === productId
            ? { ...item, quantity: newQuantity }
            : item
        )
      );
    }
  };

  return (
    <div>
      <h3>Shopping Cart Example</h3>
      <div style={{ display: 'flex', gap: '2rem' }}>
        <ProductList products={products} onAddToCart={addToCart} />
        <Cart 
          cart={cart} 
          onRemoveFromCart={removeFromCart}
          onUpdateQuantity={updateQuantity}
        />
      </div>
    </div>
  );
}

function ProductList({ products, onAddToCart }) {
  return (
    <div>
      <h4>Products</h4>
      {products.map(product => (
        <div key={product.id} style={{ marginBottom: '1rem', padding: '1rem', border: '1px solid #ccc' }}>
          <h5>{product.name}</h5>
          <p>${product.price}</p>
          <button onClick={() => onAddToCart(product)}>
            Add to Cart
          </button>
        </div>
      ))}
    </div>
  );
}

function Cart({ cart, onRemoveFromCart, onUpdateQuantity }) {
  const total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);

  return (
    <div>
      <h4>Cart ({cart.length} items)</h4>
      {cart.length === 0 ? (
        <p>Cart is empty</p>
      ) : (
        <>
          {cart.map(item => (
            <div key={item.id} style={{ marginBottom: '1rem', padding: '1rem', border: '1px solid #ccc' }}>
              <h5>{item.name}</h5>
              <p>${item.price} x {item.quantity}</p>
              <div>
                <button onClick={() => onUpdateQuantity(item.id, item.quantity - 1)}>-</button>
                <span style={{ margin: '0 1rem' }}>{item.quantity}</span>
                <button onClick={() => onUpdateQuantity(item.id, item.quantity + 1)}>+</button>
                <button 
                  onClick={() => onRemoveFromCart(item.id)}
                  style={{ marginLeft: '1rem' }}
                >
                  Remove
                </button>
              </div>
            </div>
          ))}
          <div style={{ marginTop: '1rem', fontWeight: 'bold' }}>
            Total: ${total.toFixed(2)}
          </div>
        </>
      )}
    </div>
  );
}

// Main App Component
function StateManagementDemo() {
  return (
    <AppProvider>
      <div>
        <Header />
        <Notifications />
        
        <div style={{ padding: '2rem' }}>
          <LocalStateExample />
          <hr />
          <ReducerStateExample />
          <hr />
          <LoginForm />
          <hr />
          <ShoppingCart />
        </div>
      </div>
    </AppProvider>
  );
}

export default StateManagementDemo;
```

#### Q12: How do you implement Redux for state management?
**Answer:**
Redux is a predictable state container that provides a centralized store for managing application state with a unidirectional data flow.

**Example:**
```jsx
// Redux Setup
import { createStore, combineReducers, applyMiddleware } from 'redux';
import { Provider, useSelector, useDispatch } from 'react-redux';
import { composeWithDevTools } from 'redux-devtools-extension';
import thunk from 'redux-thunk';

// 1. Action Types
const ActionTypes = {
  // User actions
  LOGIN_START: 'LOGIN_START',
  LOGIN_SUCCESS: 'LOGIN_SUCCESS',
  LOGIN_FAILURE: 'LOGIN_FAILURE',
  LOGOUT: 'LOGOUT',
  
  // Todo actions
  ADD_TODO: 'ADD_TODO',
  TOGGLE_TODO: 'TOGGLE_TODO',
  DELETE_TODO: 'DELETE_TODO',
  SET_FILTER: 'SET_FILTER',
  FETCH_TODOS_START: 'FETCH_TODOS_START',
  FETCH_TODOS_SUCCESS: 'FETCH_TODOS_SUCCESS',
  FETCH_TODOS_FAILURE: 'FETCH_TODOS_FAILURE',
  
  // UI actions
  SET_LOADING: 'SET_LOADING',
  SET_ERROR: 'SET_ERROR',
  CLEAR_ERROR: 'CLEAR_ERROR'
};

// 2. Action Creators
const userActions = {
  loginStart: () => ({ type: ActionTypes.LOGIN_START }),
  
  loginSuccess: (user) => ({
    type: ActionTypes.LOGIN_SUCCESS,
    payload: user
  }),
  
  loginFailure: (error) => ({
    type: ActionTypes.LOGIN_FAILURE,
    payload: error
  }),
  
  logout: () => ({ type: ActionTypes.LOGOUT }),
  
  // Async action creator (thunk)
  login: (credentials) => {
    return async (dispatch) => {
      dispatch(userActions.loginStart());
      
      try {
        // Simulate API call
        const response = await fetch('/api/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(credentials)
        });
        
        if (!response.ok) {
          throw new Error('Login failed');
        }
        
        const user = await response.json();
        dispatch(userActions.loginSuccess(user));
        
        // Store token in localStorage
        localStorage.setItem('token', user.token);
        
        return user;
      } catch (error) {
        dispatch(userActions.loginFailure(error.message));
        throw error;
      }
    };
  }
};

const todoActions = {
  addTodo: (text) => ({
    type: ActionTypes.ADD_TODO,
    payload: {
      id: Date.now(),
      text,
      completed: false,
      createdAt: new Date().toISOString()
    }
  }),
  
  toggleTodo: (id) => ({
    type: ActionTypes.TOGGLE_TODO,
    payload: id
  }),
  
  deleteTodo: (id) => ({
    type: ActionTypes.DELETE_TODO,
    payload: id
  }),
  
  setFilter: (filter) => ({
    type: ActionTypes.SET_FILTER,
    payload: filter
  }),
  
  fetchTodosStart: () => ({ type: ActionTypes.FETCH_TODOS_START }),
  
  fetchTodosSuccess: (todos) => ({
    type: ActionTypes.FETCH_TODOS_SUCCESS,
    payload: todos
  }),
  
  fetchTodosFailure: (error) => ({
    type: ActionTypes.FETCH_TODOS_FAILURE,
    payload: error
  }),
  
  // Async action to fetch todos
  fetchTodos: () => {
    return async (dispatch, getState) => {
      const { user } = getState();
      
      if (!user.currentUser) {
        dispatch(todoActions.fetchTodosFailure('User not authenticated'));
        return;
      }
      
      dispatch(todoActions.fetchTodosStart());
      
      try {
        const response = await fetch('/api/todos', {
          headers: {
            'Authorization': `Bearer ${user.currentUser.token}`
          }
        });
        
        if (!response.ok) {
          throw new Error('Failed to fetch todos');
        }
        
        const todos = await response.json();
        dispatch(todoActions.fetchTodosSuccess(todos));
      } catch (error) {
        dispatch(todoActions.fetchTodosFailure(error.message));
      }
    };
  }
};

const uiActions = {
  setLoading: (isLoading) => ({
    type: ActionTypes.SET_LOADING,
    payload: isLoading
  }),
  
  setError: (error) => ({
    type: ActionTypes.SET_ERROR,
    payload: error
  }),
  
  clearError: () => ({ type: ActionTypes.CLEAR_ERROR })
};

// 3. Reducers
const initialUserState = {
  currentUser: null,
  isAuthenticated: false,
  loading: false,
  error: null
};

const userReducer = (state = initialUserState, action) => {
  switch (action.type) {
    case ActionTypes.LOGIN_START:
      return {
        ...state,
        loading: true,
        error: null
      };
    
    case ActionTypes.LOGIN_SUCCESS:
      return {
        ...state,
        currentUser: action.payload,
        isAuthenticated: true,
        loading: false,
        error: null
      };
    
    case ActionTypes.LOGIN_FAILURE:
      return {
        ...state,
        currentUser: null,
        isAuthenticated: false,
        loading: false,
        error: action.payload
      };
    
    case ActionTypes.LOGOUT:
      return {
        ...state,
        currentUser: null,
        isAuthenticated: false,
        loading: false,
        error: null
      };
    
    default:
      return state;
  }
};

const initialTodoState = {
  items: [],
  filter: 'all', // all, active, completed
  loading: false,
  error: null
};

const todoReducer = (state = initialTodoState, action) => {
  switch (action.type) {
    case ActionTypes.ADD_TODO:
      return {
        ...state,
        items: [...state.items, action.payload]
      };
    
    case ActionTypes.TOGGLE_TODO:
      return {
        ...state,
        items: state.items.map(todo =>
          todo.id === action.payload
            ? { ...todo, completed: !todo.completed }
            : todo
        )
      };
    
    case ActionTypes.DELETE_TODO:
      return {
        ...state,
        items: state.items.filter(todo => todo.id !== action.payload)
      };
    
    case ActionTypes.SET_FILTER:
      return {
        ...state,
        filter: action.payload
      };
    
    case ActionTypes.FETCH_TODOS_START:
      return {
        ...state,
        loading: true,
        error: null
      };
    
    case ActionTypes.FETCH_TODOS_SUCCESS:
      return {
        ...state,
        items: action.payload,
        loading: false,
        error: null
      };
    
    case ActionTypes.FETCH_TODOS_FAILURE:
      return {
        ...state,
        loading: false,
        error: action.payload
      };
    
    default:
      return state;
  }
};

const initialUIState = {
  globalLoading: false,
  globalError: null,
  theme: 'light',
  sidebarOpen: false
};

const uiReducer = (state = initialUIState, action) => {
  switch (action.type) {
    case ActionTypes.SET_LOADING:
      return {
        ...state,
        globalLoading: action.payload
      };
    
    case ActionTypes.SET_ERROR:
      return {
        ...state,
        globalError: action.payload
      };
    
    case ActionTypes.CLEAR_ERROR:
      return {
        ...state,
        globalError: null
      };
    
    default:
      return state;
  }
};

// 4. Root Reducer
const rootReducer = combineReducers({
  user: userReducer,
  todos: todoReducer,
  ui: uiReducer
});

// 5. Store Configuration
const store = createStore(
  rootReducer,
  composeWithDevTools(
    applyMiddleware(thunk)
  )
);

// 6. Selectors (for complex state derivations)
const selectors = {
  // User selectors
  getCurrentUser: (state) => state.user.currentUser,
  isAuthenticated: (state) => state.user.isAuthenticated,
  getUserLoading: (state) => state.user.loading,
  getUserError: (state) => state.user.error,
  
  // Todo selectors
  getAllTodos: (state) => state.todos.items,
  getTodoFilter: (state) => state.todos.filter,
  getTodosLoading: (state) => state.todos.loading,
  getTodosError: (state) => state.todos.error,
  
  // Filtered todos selector
  getFilteredTodos: (state) => {
    const todos = state.todos.items;
    const filter = state.todos.filter;
    
    switch (filter) {
      case 'active':
        return todos.filter(todo => !todo.completed);
      case 'completed':
        return todos.filter(todo => todo.completed);
      default:
        return todos;
    }
  },
  
  // Todo statistics
  getTodoStats: (state) => {
    const todos = state.todos.items;
    return {
      total: todos.length,
      completed: todos.filter(todo => todo.completed).length,
      active: todos.filter(todo => !todo.completed).length
    };
  },
  
  // UI selectors
  getGlobalLoading: (state) => state.ui.globalLoading,
  getGlobalError: (state) => state.ui.globalError,
  getTheme: (state) => state.ui.theme
};

// 7. React Components using Redux
function LoginComponent() {
  const dispatch = useDispatch();
  const { loading, error } = useSelector(state => ({
    loading: selectors.getUserLoading(state),
    error: selectors.getUserError(state)
  }));
  
  const [credentials, setCredentials] = useState({
    email: '',
    password: ''
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await dispatch(userActions.login(credentials));
      setCredentials({ email: '', password: '' });
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h3>Login</h3>
      
      {error && (
        <div style={{ color: 'red', marginBottom: '1rem' }}>
          Error: {error}
        </div>
      )}
      
      <div>
        <input
          type="email"
          placeholder="Email"
          value={credentials.email}
          onChange={(e) => setCredentials({
            ...credentials,
            email: e.target.value
          })}
        />
      </div>
      
      <div>
        <input
          type="password"
          placeholder="Password"
          value={credentials.password}
          onChange={(e) => setCredentials({
            ...credentials,
            password: e.target.value
          })}
        />
      </div>
      
      <button type="submit" disabled={loading}>
        {loading ? 'Logging in...' : 'Login'}
      </button>
    </form>
  );
}

function TodoApp() {
  const dispatch = useDispatch();
  const {
    todos,
    filter,
    loading,
    error,
    stats
  } = useSelector(state => ({
    todos: selectors.getFilteredTodos(state),
    filter: selectors.getTodoFilter(state),
    loading: selectors.getTodosLoading(state),
    error: selectors.getTodosError(state),
    stats: selectors.getTodoStats(state)
  }));
  
  const [newTodoText, setNewTodoText] = useState('');

  useEffect(() => {
    dispatch(todoActions.fetchTodos());
  }, [dispatch]);

  const handleAddTodo = (e) => {
    e.preventDefault();
    if (newTodoText.trim()) {
      dispatch(todoActions.addTodo(newTodoText.trim()));
      setNewTodoText('');
    }
  };

  const handleToggleTodo = (id) => {
    dispatch(todoActions.toggleTodo(id));
  };

  const handleDeleteTodo = (id) => {
    dispatch(todoActions.deleteTodo(id));
  };

  const handleFilterChange = (newFilter) => {
    dispatch(todoActions.setFilter(newFilter));
  };

  if (loading) {
    return <div>Loading todos...</div>;
  }

  return (
    <div>
      <h3>Todo App with Redux</h3>
      
      {error && (
        <div style={{ color: 'red', marginBottom: '1rem' }}>
          Error: {error}
        </div>
      )}
      
      {/* Statistics */}
      <div style={{ marginBottom: '1rem' }}>
        <p>
          Total: {stats.total} | 
          Active: {stats.active} | 
          Completed: {stats.completed}
        </p>
      </div>
      
      {/* Add new todo */}
      <form onSubmit={handleAddTodo} style={{ marginBottom: '1rem' }}>
        <input
          type="text"
          placeholder="Add new todo..."
          value={newTodoText}
          onChange={(e) => setNewTodoText(e.target.value)}
        />
        <button type="submit">Add</button>
      </form>
      
      {/* Filter buttons */}
      <div style={{ marginBottom: '1rem' }}>
        {['all', 'active', 'completed'].map(filterType => (
          <button
            key={filterType}
            onClick={() => handleFilterChange(filterType)}
            style={{
              marginRight: '0.5rem',
              fontWeight: filter === filterType ? 'bold' : 'normal'
            }}
          >
            {filterType.charAt(0).toUpperCase() + filterType.slice(1)}
          </button>
        ))}
      </div>
      
      {/* Todo list */}
      <div>
        {todos.length === 0 ? (
          <p>No todos found</p>
        ) : (
          todos.map(todo => (
            <div
              key={todo.id}
              style={{
                display: 'flex',
                alignItems: 'center',
                marginBottom: '0.5rem',
                padding: '0.5rem',
                border: '1px solid #ccc'
              }}
            >
              <span
                style={{
                  flex: 1,
                  textDecoration: todo.completed ? 'line-through' : 'none',
                  cursor: 'pointer'
                }}
                onClick={() => handleToggleTodo(todo.id)}
              >
                {todo.text}
              </span>
              <button onClick={() => handleDeleteTodo(todo.id)}>
                Delete
              </button>
            </div>
          ))
        )}
      </div>
    </div>
  );
}

function UserProfile() {
  const dispatch = useDispatch();
  const { user, isAuthenticated } = useSelector(state => ({
    user: selectors.getCurrentUser(state),
    isAuthenticated: selectors.isAuthenticated(state)
  }));

  const handleLogout = () => {
    dispatch(userActions.logout());
    localStorage.removeItem('token');
  };

  if (!isAuthenticated) {
    return <LoginComponent />;
  }

  return (
    <div>
      <h3>Welcome, {user.name}!</h3>
      <p>Email: {user.email}</p>
      <button onClick={handleLogout}>Logout</button>
    </div>
  );
}

// 8. Main App with Redux Provider
function ReduxApp() {
  return (
    <Provider store={store}>
      <div style={{ padding: '2rem' }}>
        <h1>Redux State Management Demo</h1>
        <UserProfile />
        <hr />
        <TodoApp />
      </div>
    </Provider>
  );
}

export default ReduxApp;
```