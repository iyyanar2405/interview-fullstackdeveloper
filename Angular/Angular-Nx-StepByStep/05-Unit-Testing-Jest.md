# 05 — Unit testing (Jest)

Set up and run Jest tests with Nx.

## Generate with Jest (default)
`@nx/angular` apps/libs default to Jest. If needed:
```powershell
npx nx g @nx/angular:application web --unit-test-runner=jest
npx nx g @nx/angular:library shared-utils --unit-test-runner=jest
```

## Run tests
```powershell
npx nx test web
npx nx test shared-utils --code-coverage
```

## Typical files
- jest.config.ts per project
- test setup: `src/test-setup.ts`
- specs: `*.spec.ts`

## Example spec
```ts
describe('math', () => {
  it('adds', () => {
    expect(2 + 3).toBe(5);
  });
});
```

See Nx docs for Jest plugin details and executors.
