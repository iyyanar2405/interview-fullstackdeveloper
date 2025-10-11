# Day 39: Bootstrap + React Integration ⚛️

Integrate Bootstrap with React using react-bootstrap library.

```jsx
// npm install react-bootstrap bootstrap
import { Container, Button, Card } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  return (
    <Container className="py-5">
      <Card>
        <Card.Body>
          <Card.Title>React Bootstrap</Card.Title>
          <Card.Text>Bootstrap components as React components</Card.Text>
          <Button variant="primary">Click Me</Button>
        </Card.Body>
      </Card>
    </Container>
  );
}

export default App;
```

**Next: Day 40 - Bootstrap + Vue Integration** 🖖
