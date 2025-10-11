# Day 42: Testing Bootstrap Components 🧪

Test Bootstrap components with Jest, Cypress, and accessibility testing tools.

```javascript
// Jest + Testing Library Example
import { render, screen, fireEvent } from '@testing-library/react';
import { Button, Modal } from 'react-bootstrap';

test('modal opens when button clicked', () => {
  const { getByRole } = render(
    <div>
      <Button data-bs-toggle="modal" data-bs-target="#testModal">Open</Button>
      <Modal id="testModal">
        <Modal.Body>Test Content</Modal.Body>
      </Modal>
    </div>
  );
  
  const button = getByRole('button');
  fireEvent.click(button);
  expect(screen.getByText('Test Content')).toBeInTheDocument();
});

// Cypress E2E Test
describe('Bootstrap Navigation', () => {
  it('navigates correctly', () => {
    cy.visit('/');
    cy.get('.navbar-nav .nav-link').first().click();
    cy.url().should('include', '/about');
  });
});
```

**Next: Day 43 - E-Commerce Landing Page** 🛒
