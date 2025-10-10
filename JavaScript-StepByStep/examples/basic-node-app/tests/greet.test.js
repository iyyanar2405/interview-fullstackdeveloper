import { test } from 'node:test';
import assert from 'node:assert/strict';
import { greet } from '../src/index.js';

test('greet returns expected message', () => {
  assert.equal(greet('Ada'), 'Hello, Ada!');
});
