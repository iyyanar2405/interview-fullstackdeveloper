# Day 2: JSX and Components Deep Dive

## Understanding JSX in Detail

JSX (JavaScript XML) is a syntax extension for JavaScript that allows you to write HTML-like code in your JavaScript files. It's not HTML, but it looks very similar!

### JSX Fundamentals

#### 1. JSX is Just JavaScript
Behind the scenes, JSX gets compiled to JavaScript function calls:

```jsx
// JSX
const element = <h1>Hello, World!</h1>;

// Compiles to JavaScript
const element = React.createElement('h1', null, 'Hello, World!');
```

#### 2. Embedding Expressions in JSX
Use curly braces `{}` to embed any JavaScript expression:

```jsx
function Greeting() {
  const user = {
    firstName: 'John',
    lastName: 'Doe'
  };
  
  const formatName = (user) => {
    return user.firstName + ' ' + user.lastName;
  };
  
  return (
    <div>
      <h1>Hello, {formatName(user)}!</h1>
      <p>Today is {new Date().toLocaleDateString()}</p>
      <p>Random number: {Math.floor(Math.random() * 100)}</p>
      <p>2 + 2 = {2 + 2}</p>
    </div>
  );
}
```

#### 3. JSX Attributes
JSX attributes use camelCase and some have different names:

```jsx
function MyComponent() {
  const imageUrl = 'https://via.placeholder.com/300';
  const isDisabled = true;
  
  return (
    <div>
      {/* HTML class becomes className */}
      <div className="container">
        
        {/* HTML for becomes htmlFor */}
        <label htmlFor="email">Email:</label>
        <input 
          type="email" 
          id="email"
          disabled={isDisabled}
          style={{
            backgroundColor: 'lightblue',
            border: '1px solid blue',
            padding: '10px'
          }}
        />
        
        {/* Self-closing tags must end with /> */}
        <img src={imageUrl} alt="Placeholder" />
        <br />
        <hr />
      </div>
    </div>
  );
}
```

#### 4. Conditional Rendering in JSX

```jsx
function WelcomeMessage({ isLoggedIn, username }) {
  return (
    <div>
      {/* Conditional rendering with && */}
      {isLoggedIn && <h1>Welcome back, {username}!</h1>}
      
      {/* Conditional rendering with ternary operator */}
      {isLoggedIn ? (
        <button>Logout</button>
      ) : (
        <button>Login</button>
      )}
      
      {/* Complex conditional logic */}
      {isLoggedIn ? (
        <div>
          <p>You have access to premium features</p>
          <button>Go to Dashboard</button>
        </div>
      ) : (
        <div>
          <p>Please log in to access all features</p>
          <button>Sign Up</button>
          <button>Login</button>
        </div>
      )}
    </div>
  );
}
```

#### 5. Lists and Keys in JSX

```jsx
function TodoList() {
  const todos = [
    { id: 1, text: 'Learn React', completed: false },
    { id: 2, text: 'Build a project', completed: false },
    { id: 3, text: 'Get hired', completed: false }
  ];
  
  return (
    <ul>
      {todos.map(todo => (
        <li 
          key={todo.id}
          style={{
            textDecoration: todo.completed ? 'line-through' : 'none',
            color: todo.completed ? 'gray' : 'black'
          }}
        >
          {todo.text}
        </li>
      ))}
    </ul>
  );
}
```

## React Components Deep Dive

### Component Types

#### 1. Functional Components (Recommended)

```jsx
// Basic functional component
function Welcome() {
  return <h1>Hello, World!</h1>;
}

// Arrow function component
const Welcome = () => {
  return <h1>Hello, World!</h1>;
};

// Arrow function with implicit return
const Welcome = () => <h1>Hello, World!</h1>;
```

#### 2. Component with Logic

```jsx
function UserCard() {
  // Component logic goes here
  const currentTime = new Date().toLocaleTimeString();
  const isBusinessHours = new Date().getHours() >= 9 && new Date().getHours() <= 17;
  
  const getUserStatus = () => {
    return isBusinessHours ? 'Available' : 'Away';
  };
  
  return (
    <div className="user-card">
      <h2>John Doe</h2>
      <p>Status: {getUserStatus()}</p>
      <p>Last seen: {currentTime}</p>
      <div className={`status-indicator ${isBusinessHours ? 'online' : 'offline'}`}>
        {isBusinessHours ? '🟢' : '🔴'}
      </div>
    </div>
  );
}
```

### Component Organization and File Structure

#### Recommended File Structure
```
src/
├── components/
│   ├── Header/
│   │   ├── Header.js
│   │   ├── Header.css
│   │   └── index.js
│   ├── UserCard/
│   │   ├── UserCard.js
│   │   ├── UserCard.css
│   │   └── index.js
│   └── common/
│       ├── Button/
│       ├── Input/
│       └── Modal/
├── pages/
├── utils/
├── App.js
└── index.js
```

#### Component File Example

```jsx
// components/UserCard/UserCard.js
import React from 'react';
import './UserCard.css';

function UserCard({ name, email, avatar, isOnline }) {
  return (
    <div className="user-card">
      <div className="user-avatar">
        <img src={avatar} alt={`${name}'s avatar`} />
        <span className={`status-dot ${isOnline ? 'online' : 'offline'}`}></span>
      </div>
      <div className="user-info">
        <h3 className="user-name">{name}</h3>
        <p className="user-email">{email}</p>
        <p className="user-status">
          {isOnline ? 'Online' : 'Offline'}
        </p>
      </div>
    </div>
  );
}

export default UserCard;
```

```css
/* components/UserCard/UserCard.css */
.user-card {
  display: flex;
  align-items: center;
  padding: 16px;
  border: 1px solid #ddd;
  border-radius: 8px;
  margin: 8px 0;
  background-color: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.user-avatar {
  position: relative;
  margin-right: 16px;
}

.user-avatar img {
  width: 50px;
  height: 50px;
  border-radius: 50%;
}

.status-dot {
  position: absolute;
  width: 12px;
  height: 12px;
  border-radius: 50%;
  bottom: 0;
  right: 0;
  border: 2px solid white;
}

.status-dot.online {
  background-color: #4caf50;
}

.status-dot.offline {
  background-color: #f44336;
}

.user-info {
  flex: 1;
}

.user-name {
  margin: 0 0 4px 0;
  font-size: 18px;
  font-weight: 600;
}

.user-email {
  margin: 0 0 4px 0;
  color: #666;
  font-size: 14px;
}

.user-status {
  margin: 0;
  font-size: 12px;
  color: #888;
}
```

```jsx
// components/UserCard/index.js
export { default } from './UserCard';
```

### Advanced JSX Patterns

#### 1. Fragment to Avoid Extra Divs

```jsx
import React, { Fragment } from 'react';

function UserInfo() {
  return (
    // Using React.Fragment
    <Fragment>
      <h1>User Information</h1>
      <p>Details about the user</p>
    </Fragment>
    
    // Or using shorthand syntax
    <>
      <h1>User Information</h1>
      <p>Details about the user</p>
    </>
  );
}
```

#### 2. Dynamic Class Names

```jsx
function Button({ type, size, disabled, children }) {
  const baseClass = 'btn';
  const typeClass = `btn-${type}`;
  const sizeClass = `btn-${size}`;
  
  // Method 1: Template literals
  const className = `${baseClass} ${typeClass} ${sizeClass} ${disabled ? 'disabled' : ''}`;
  
  // Method 2: Array join
  const classNameArray = [
    baseClass,
    typeClass,
    sizeClass,
    disabled && 'disabled'
  ].filter(Boolean).join(' ');
  
  return (
    <button 
      className={className}
      disabled={disabled}
    >
      {children}
    </button>
  );
}
```

#### 3. Inline Styles with Dynamic Values

```jsx
function ProgressBar({ percentage, color = 'blue' }) {
  const barStyle = {
    width: '100%',
    height: '20px',
    backgroundColor: '#f0f0f0',
    borderRadius: '10px',
    overflow: 'hidden'
  };
  
  const fillStyle = {
    width: `${percentage}%`,
    height: '100%',
    backgroundColor: color,
    transition: 'width 0.3s ease'
  };
  
  return (
    <div style={barStyle}>
      <div style={fillStyle}></div>
      <div style={{ textAlign: 'center', marginTop: '5px' }}>
        {percentage}%
      </div>
    </div>
  );
}
```

## Practical Examples

### Example 1: Product Card Component

```jsx
function ProductCard({ product }) {
  const { id, name, price, image, inStock, rating, reviews } = product;
  
  const formatPrice = (price) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };
  
  const renderStars = (rating) => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push(
        <span key={i} className={i <= rating ? 'star filled' : 'star'}>
          ⭐
        </span>
      );
    }
    return stars;
  };
  
  return (
    <div className="product-card">
      <div className="product-image">
        <img src={image} alt={name} />
        {!inStock && <div className="out-of-stock">Out of Stock</div>}
      </div>
      
      <div className="product-info">
        <h3 className="product-name">{name}</h3>
        <div className="product-rating">
          {renderStars(rating)}
          <span className="review-count">({reviews} reviews)</span>
        </div>
        <div className="product-price">{formatPrice(price)}</div>
        
        <button 
          className={`add-to-cart ${!inStock ? 'disabled' : ''}`}
          disabled={!inStock}
        >
          {inStock ? 'Add to Cart' : 'Out of Stock'}
        </button>
      </div>
    </div>
  );
}
```

### Example 2: Navigation Component

```jsx
function Navigation({ currentPage }) {
  const navItems = [
    { id: 'home', label: 'Home', path: '/' },
    { id: 'products', label: 'Products', path: '/products' },
    { id: 'about', label: 'About', path: '/about' },
    { id: 'contact', label: 'Contact', path: '/contact' }
  ];
  
  return (
    <nav className="navigation">
      <div className="nav-brand">
        <h2>My Store</h2>
      </div>
      
      <ul className="nav-menu">
        {navItems.map(item => (
          <li key={item.id} className="nav-item">
            <a 
              href={item.path}
              className={`nav-link ${currentPage === item.id ? 'active' : ''}`}
            >
              {item.label}
            </a>
          </li>
        ))}
      </ul>
      
      <div className="nav-actions">
        <button className="btn-search">🔍</button>
        <button className="btn-cart">🛒</button>
      </div>
    </nav>
  );
}
```

### Example 3: Weather Widget

```jsx
function WeatherWidget() {
  const weatherData = {
    city: 'New York',
    temperature: 72,
    condition: 'Sunny',
    humidity: 45,
    windSpeed: 8,
    forecast: [
      { day: 'Mon', high: 75, low: 65, condition: 'sunny' },
      { day: 'Tue', high: 73, low: 63, condition: 'cloudy' },
      { day: 'Wed', high: 68, low: 58, condition: 'rainy' }
    ]
  };
  
  const getWeatherIcon = (condition) => {
    const icons = {
      sunny: '☀️',
      cloudy: '☁️',
      rainy: '🌧️',
      snowy: '❄️'
    };
    return icons[condition.toLowerCase()] || '🌤️';
  };
  
  return (
    <div className="weather-widget">
      <div className="current-weather">
        <h2>{weatherData.city}</h2>
        <div className="temperature">{weatherData.temperature}°F</div>
        <div className="condition">
          {getWeatherIcon(weatherData.condition)} {weatherData.condition}
        </div>
        
        <div className="weather-details">
          <div className="detail">
            <span>Humidity: {weatherData.humidity}%</span>
          </div>
          <div className="detail">
            <span>Wind: {weatherData.windSpeed} mph</span>
          </div>
        </div>
      </div>
      
      <div className="forecast">
        <h3>3-Day Forecast</h3>
        <div className="forecast-list">
          {weatherData.forecast.map((day, index) => (
            <div key={index} className="forecast-item">
              <div className="day">{day.day}</div>
              <div className="icon">{getWeatherIcon(day.condition)}</div>
              <div className="temps">
                <span className="high">{day.high}°</span>
                <span className="low">{day.low}°</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
```

## Best Practices for JSX and Components

### 1. Component Naming
- Use PascalCase for component names
- Use descriptive names that indicate the component's purpose
- Avoid generic names like `Component` or `Item`

```jsx
// Good
function UserProfileCard() { }
function ShoppingCartItem() { }
function NavigationMenu() { }

// Avoid
function Component() { }
function Item() { }
function Thing() { }
```

### 2. JSX Formatting
- Use proper indentation
- Break long attribute lists into multiple lines
- Use meaningful variable names

```jsx
// Good
function LoginForm() {
  const handleSubmit = (event) => {
    event.preventDefault();
    // Handle form submission
  };
  
  return (
    <form 
      className="login-form"
      onSubmit={handleSubmit}
    >
      <input
        type="email"
        placeholder="Enter your email"
        required
      />
      <input
        type="password"
        placeholder="Enter your password"
        required
      />
      <button type="submit">
        Login
      </button>
    </form>
  );
}
```

### 3. Conditional Rendering Best Practices

```jsx
function UserGreeting({ user, isLoggedIn }) {
  // Early return for better readability
  if (!isLoggedIn) {
    return <div>Please log in to continue</div>;
  }
  
  return (
    <div>
      <h1>Welcome back, {user.name}!</h1>
      {user.isAdmin && (
        <div className="admin-panel">
          <button>Admin Dashboard</button>
        </div>
      )}
    </div>
  );
}
```

## Common Mistakes to Avoid

### 1. Missing Keys in Lists
```jsx
// Wrong
{items.map(item => <li>{item}</li>)}

// Correct
{items.map((item, index) => <li key={item.id || index}>{item}</li>)}
```

### 2. Mutating Props
```jsx
// Wrong
function BadComponent({ items }) {
  items.push('new item'); // Don't mutate props!
  return <div>{items.length}</div>;
}

// Correct
function GoodComponent({ items }) {
  const itemCount = items.length;
  return <div>{itemCount}</div>;
}
```

### 3. Incorrect Event Handling
```jsx
// Wrong
<button onClick={handleClick()}>Click me</button>

// Correct
<button onClick={handleClick}>Click me</button>
// or
<button onClick={() => handleClick()}>Click me</button>
```

## Assignment

### Task 1: Build a Blog Post Component
Create a `BlogPost` component that displays:
- Title
- Author name and avatar
- Publication date
- Content preview
- Read time estimate
- Tags
- Like and comment counts

### Task 2: Create a Dashboard Widget
Build a `StatsWidget` component that shows:
- A title
- A large number (metric value)
- Percentage change from last period
- A trend indicator (up/down arrow)
- Color coding based on positive/negative change

### Task 3: Build a Contact Card
Create a `ContactCard` component featuring:
- Person's photo
- Name and job title
- Contact information (phone, email)
- Social media links
- Online/offline status

## Quiz

1. What is JSX and how is it different from HTML?
2. How do you embed JavaScript expressions in JSX?
3. Why do we use `className` instead of `class` in JSX?
4. What is the purpose of the `key` prop in lists?
5. How do you conditionally render content in JSX?
6. What are React Fragments and when should you use them?

## Resources

- [JSX In Depth](https://react.dev/learn/writing-markup-with-jsx)
- [Components and Props](https://react.dev/learn/passing-props-to-a-component)
- [Conditional Rendering](https://react.dev/learn/conditional-rendering)
- [Rendering Lists](https://react.dev/learn/rendering-lists)

## Next Steps

Tomorrow we'll learn about:
- Props: Passing data between components
- PropTypes for type checking
- Default props
- Component composition patterns
- Children prop

## Summary

Today you mastered:
- ✅ Advanced JSX syntax and patterns
- ✅ Component creation and organization
- ✅ Conditional rendering techniques
- ✅ List rendering with keys
- ✅ Dynamic styling and class names
- ✅ Best practices for JSX and components

Great progress! You're building a solid foundation in React development. 🚀