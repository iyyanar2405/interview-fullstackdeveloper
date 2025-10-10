import { readFile } from 'node:fs/promises';
import path from 'node:path';
import process from 'node:process';

function greet(name) {
  return `Hello, ${name}!`;
}

async function main() {
  const name = process.argv[2] ?? 'World';
  console.log(greet(name));

  // demo async fs
  const pkgPath = path.resolve(process.cwd(), 'package.json');
  try {
    const pkg = JSON.parse(await readFile(pkgPath, 'utf8'));
    console.log('Package name:', pkg.name);
  } catch (err) {
    console.error('Could not read package.json:', err.message);
  }
}

main();

export { greet };
