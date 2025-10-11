# Day 02: TypeScript Fundamentals for Angular 📘

Master TypeScript essentials required for Angular development.

## 🎯 Learning Objectives

- Understand TypeScript basics and benefits
- Learn TypeScript types and type annotations
- Master interfaces and classes
- Understand decorators and metadata
- Work with ES6+ features
- Apply TypeScript in Angular context

## 📋 What is TypeScript?

TypeScript is a typed superset of JavaScript that compiles to plain JavaScript. Angular is built with TypeScript and uses it as its primary language.

### Benefits of TypeScript
- ✅ Static type checking
- ✅ Better IDE support (IntelliSense)
- ✅ Early error detection
- ✅ Better code documentation
- ✅ Advanced OOP features
- ✅ Latest JavaScript features

## 💻 Basic Types

### Primitive Types

```typescript
// String
let name: string = 'John Doe';
let template: string = `Hello, ${name}!`;

// Number
let age: number = 25;
let price: number = 99.99;
let hex: number = 0xf00d;

// Boolean
let isActive: boolean = true;
let hasPermission: boolean = false;

// Null and Undefined
let nothing: null = null;
let notDefined: undefined = undefined;

// Any (avoid when possible)
let dynamic: any = 'can be anything';
dynamic = 42;
dynamic = true;

// Unknown (safer than any)
let userInput: unknown;
userInput = 'text';
userInput = 42;

// Void (for functions that don't return)
function logMessage(message: string): void {
  console.log(message);
}

// Never (for functions that never return)
function throwError(message: string): never {
  throw new Error(message);
}
```

### Arrays

```typescript
// Array of numbers
let numbers: number[] = [1, 2, 3, 4, 5];
let scores: Array<number> = [90, 85, 95];

// Array of strings
let names: string[] = ['Alice', 'Bob', 'Charlie'];

// Mixed array (union type)
let mixed: (string | number)[] = ['text', 42, 'hello', 100];

// Array methods
numbers.push(6);
numbers.pop();
numbers.forEach(num => console.log(num));
let doubled = numbers.map(num => num * 2);
```

### Tuples

```typescript
// Fixed-length array with known types
let person: [string, number, boolean];
person = ['John', 30, true];

// Accessing tuple elements
let name = person[0];  // string
let age = person[1];   // number

// Tuple with optional elements
let coordinate: [number, number, number?];
coordinate = [10, 20];
coordinate = [10, 20, 30];
```

### Enums

```typescript
// Numeric enum
enum UserRole {
  Admin,      // 0
  Editor,     // 1
  Viewer      // 2
}

let role: UserRole = UserRole.Admin;

// String enum
enum Status {
  Active = 'ACTIVE',
  Inactive = 'INACTIVE',
  Pending = 'PENDING'
}

let currentStatus: Status = Status.Active;

// Enum with custom values
enum HttpStatus {
  OK = 200,
  BadRequest = 400,
  Unauthorized = 401,
  NotFound = 404,
  ServerError = 500
}
```

## 🎯 Interfaces

Interfaces define the structure of objects.

```typescript
// Basic interface
interface User {
  id: number;
  name: string;
  email: string;
  age?: number;  // Optional property
}

let user: User = {
  id: 1,
  name: 'John Doe',
  email: 'john@example.com'
};

// Interface with methods
interface Product {
  id: number;
  name: string;
  price: number;
  calculateDiscount(percentage: number): number;
}

let product: Product = {
  id: 1,
  name: 'Laptop',
  price: 1000,
  calculateDiscount(percentage: number): number {
    return this.price * (percentage / 100);
  }
};

// Extending interfaces
interface Employee extends User {
  employeeId: string;
  department: string;
  salary: number;
}

let employee: Employee = {
  id: 1,
  name: 'Jane Smith',
  email: 'jane@example.com',
  employeeId: 'EMP001',
  department: 'IT',
  salary: 75000
};

// Interface for function types
interface MathOperation {
  (a: number, b: number): number;
}

let add: MathOperation = (x, y) => x + y;
let multiply: MathOperation = (x, y) => x * y;
```

## 🏗️ Classes

```typescript
// Basic class
class Person {
  // Properties
  name: string;
  age: number;
  
  // Constructor
  constructor(name: string, age: number) {
    this.name = name;
    this.age = age;
  }
  
  // Method
  greet(): string {
    return `Hello, my name is ${this.name}`;
  }
}

let person = new Person('John', 30);
console.log(person.greet());

// Class with access modifiers
class BankAccount {
  public accountNumber: string;
  private balance: number;
  protected accountType: string;
  
  constructor(accountNumber: string, initialBalance: number) {
    this.accountNumber = accountNumber;
    this.balance = initialBalance;
    this.accountType = 'Savings';
  }
  
  public deposit(amount: number): void {
    this.balance += amount;
  }
  
  public withdraw(amount: number): boolean {
    if (this.balance >= amount) {
      this.balance -= amount;
      return true;
    }
    return false;
  }
  
  public getBalance(): number {
    return this.balance;
  }
}

// Shorthand constructor parameters
class Student {
  constructor(
    public id: number,
    public name: string,
    private grades: number[]
  ) {}
  
  getAverage(): number {
    return this.grades.reduce((a, b) => a + b) / this.grades.length;
  }
}

// Inheritance
class Animal {
  constructor(public name: string) {}
  
  makeSound(): void {
    console.log('Some sound');
  }
}

class Dog extends Animal {
  constructor(name: string, public breed: string) {
    super(name);
  }
  
  makeSound(): void {
    console.log('Woof! Woof!');
  }
  
  fetch(): void {
    console.log(`${this.name} is fetching!`);
  }
}

// Abstract classes
abstract class Shape {
  constructor(public color: string) {}
  
  abstract calculateArea(): number;
  
  describe(): void {
    console.log(`This is a ${this.color} shape`);
  }
}

class Circle extends Shape {
  constructor(color: string, public radius: number) {
    super(color);
  }
  
  calculateArea(): number {
    return Math.PI * this.radius * this.radius;
  }
}
```

## 🎨 Decorators (Important for Angular)

Decorators are special declarations that can be attached to classes, methods, properties, or parameters.

```typescript
// Class decorator
function Component(config: any) {
  return function(target: Function) {
    // Add metadata to class
    console.log('Component decorator called');
  };
}

@Component({
  selector: 'app-root',
  template: '<h1>Hello</h1>'
})
class AppComponent {}

// Property decorator
function Required(target: any, propertyKey: string) {
  let value: any;
  
  const getter = () => value;
  const setter = (newValue: any) => {
    if (!newValue) {
      throw new Error(`${propertyKey} is required`);
    }
    value = newValue;
  };
  
  Object.defineProperty(target, propertyKey, {
    get: getter,
    set: setter
  });
}

class User {
  @Required
  username: string;
}

// Method decorator
function Log(target: any, propertyKey: string, descriptor: PropertyDescriptor) {
  const originalMethod = descriptor.value;
  
  descriptor.value = function(...args: any[]) {
    console.log(`Calling ${propertyKey} with`, args);
    const result = originalMethod.apply(this, args);
    console.log(`Result:`, result);
    return result;
  };
}

class Calculator {
  @Log
  add(a: number, b: number): number {
    return a + b;
  }
}
```

## 🚀 Generics

```typescript
// Generic function
function identity<T>(arg: T): T {
  return arg;
}

let output1 = identity<string>("hello");
let output2 = identity<number>(42);

// Generic interface
interface Repository<T> {
  getById(id: number): T;
  getAll(): T[];
  save(item: T): void;
  delete(id: number): void;
}

// Generic class
class DataStore<T> {
  private data: T[] = [];
  
  add(item: T): void {
    this.data.push(item);
  }
  
  getAll(): T[] {
    return this.data;
  }
  
  getById(index: number): T {
    return this.data[index];
  }
}

let numberStore = new DataStore<number>();
numberStore.add(1);
numberStore.add(2);

let stringStore = new DataStore<string>();
stringStore.add('hello');
stringStore.add('world');

// Generic constraints
interface HasLength {
  length: number;
}

function logLength<T extends HasLength>(arg: T): void {
  console.log(arg.length);
}

logLength('hello');  // OK
logLength([1, 2, 3]);  // OK
// logLength(42);  // Error: number doesn't have length
```

## 🎯 Advanced Types

### Union Types

```typescript
// Variable can be multiple types
let id: string | number;
id = '123';
id = 123;

function formatValue(value: string | number): string {
  if (typeof value === 'string') {
    return value.toUpperCase();
  }
  return value.toFixed(2);
}
```

### Intersection Types

```typescript
interface Serializable {
  serialize(): string;
}

interface Loggable {
  log(): void;
}

type SerializableLoggable = Serializable & Loggable;

class MyClass implements SerializableLoggable {
  serialize(): string {
    return JSON.stringify(this);
  }
  
  log(): void {
    console.log(this.serialize());
  }
}
```

### Type Aliases

```typescript
type StringOrNumber = string | number;
type Point = { x: number; y: number };
type Callback = (data: string) => void;

let value: StringOrNumber = 'hello';
let point: Point = { x: 10, y: 20 };
let handler: Callback = (data) => console.log(data);
```

### Literal Types

```typescript
type Direction = 'North' | 'South' | 'East' | 'West';
type Status = 'success' | 'error' | 'pending';

let direction: Direction = 'North';
let status: Status = 'success';

// direction = 'Up';  // Error: not in union
```

## 🔧 Utility Types

```typescript
// Partial - makes all properties optional
interface User {
  id: number;
  name: string;
  email: string;
}

type PartialUser = Partial<User>;
let user: PartialUser = { name: 'John' };

// Required - makes all properties required
type RequiredUser = Required<PartialUser>;

// Readonly - makes all properties readonly
type ReadonlyUser = Readonly<User>;
let readonlyUser: ReadonlyUser = { id: 1, name: 'John', email: 'john@example.com' };
// readonlyUser.name = 'Jane';  // Error: readonly

// Pick - selects specific properties
type UserPreview = Pick<User, 'id' | 'name'>;

// Omit - excludes specific properties
type UserWithoutEmail = Omit<User, 'email'>;

// Record - creates object type with specific keys and values
type UserRoles = Record<string, boolean>;
let roles: UserRoles = {
  admin: true,
  editor: false,
  viewer: true
};
```

## 🏃 Hands-On Practice

### Exercise 1: Create User Management System

```typescript
// Define enums
enum UserRole {
  Admin = 'ADMIN',
  User = 'USER',
  Guest = 'GUEST'
}

enum UserStatus {
  Active = 'ACTIVE',
  Inactive = 'INACTIVE',
  Suspended = 'SUSPENDED'
}

// Define interfaces
interface Address {
  street: string;
  city: string;
  country: string;
  zipCode: string;
}

interface User {
  id: number;
  username: string;
  email: string;
  role: UserRole;
  status: UserStatus;
  address?: Address;
  createdAt: Date;
}

// User management class
class UserManager {
  private users: User[] = [];
  
  addUser(user: User): void {
    this.users.push(user);
  }
  
  getUserById(id: number): User | undefined {
    return this.users.find(user => user.id === id);
  }
  
  getUsersByRole(role: UserRole): User[] {
    return this.users.filter(user => user.role === role);
  }
  
  updateUserStatus(id: number, status: UserStatus): boolean {
    const user = this.getUserById(id);
    if (user) {
      user.status = status;
      return true;
    }
    return false;
  }
  
  getAllUsers(): User[] {
    return this.users;
  }
}

// Usage
const userManager = new UserManager();

userManager.addUser({
  id: 1,
  username: 'john_doe',
  email: 'john@example.com',
  role: UserRole.Admin,
  status: UserStatus.Active,
  createdAt: new Date()
});

userManager.addUser({
  id: 2,
  username: 'jane_smith',
  email: 'jane@example.com',
  role: UserRole.User,
  status: UserStatus.Active,
  address: {
    street: '123 Main St',
    city: 'New York',
    country: 'USA',
    zipCode: '10001'
  },
  createdAt: new Date()
});

console.log(userManager.getAllUsers());
```

### Exercise 2: Generic Data Service

```typescript
interface Entity {
  id: number;
}

class GenericDataService<T extends Entity> {
  private items: T[] = [];
  
  create(item: T): T {
    this.items.push(item);
    return item;
  }
  
  getAll(): T[] {
    return this.items;
  }
  
  getById(id: number): T | undefined {
    return this.items.find(item => item.id === id);
  }
  
  update(id: number, updates: Partial<T>): T | undefined {
    const item = this.getById(id);
    if (item) {
      Object.assign(item, updates);
      return item;
    }
    return undefined;
  }
  
  delete(id: number): boolean {
    const index = this.items.findIndex(item => item.id === id);
    if (index !== -1) {
      this.items.splice(index, 1);
      return true;
    }
    return false;
  }
}

// Usage
interface Product extends Entity {
  name: string;
  price: number;
  category: string;
}

const productService = new GenericDataService<Product>();

productService.create({
  id: 1,
  name: 'Laptop',
  price: 999.99,
  category: 'Electronics'
});

console.log(productService.getAll());
```

## ✅ Day 2 Checklist

- [ ] Understand TypeScript basic types
- [ ] Master interfaces and classes
- [ ] Learn about decorators
- [ ] Practice with generics
- [ ] Complete user management exercise
- [ ] Complete generic service exercise
- [ ] Understand TypeScript in Angular context

## 🎯 Key Takeaways

1. **TypeScript** adds static typing to JavaScript
2. **Interfaces** define object structures
3. **Classes** support OOP in TypeScript
4. **Decorators** are crucial for Angular
5. **Generics** enable reusable code
6. **Type safety** prevents runtime errors

## 📚 Additional Resources

- [TypeScript Official Docs](https://www.typescriptlang.org/docs/)
- [TypeScript Deep Dive](https://basarat.gitbook.io/typescript/)
- [TypeScript Playground](https://www.typescriptlang.org/play)

## 🚀 Next Steps

Tomorrow in **Day 03: Components & Templates**, you'll learn:
- Creating Angular components
- Component lifecycle hooks
- Template syntax
- Component communication

---

**Great Progress!** 🎉 You now understand TypeScript fundamentals for Angular development!
