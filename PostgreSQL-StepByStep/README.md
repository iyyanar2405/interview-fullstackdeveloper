# PostgreSQL — Step by Step (00–13)

A practical path to learn PostgreSQL with runnable scripts and an e-commerce sample schema.

## Quickstart (Windows)
- Install: PostgreSQL (via EDB installer) and ensure `psql` is on PATH
- Connect with psql: `psql -h localhost -U postgres`
- Create DB then run scripts in order:
	1. `CREATE DATABASE ecom_training;`
	2. `\i .\\PostgreSQL-StepByStep\\scripts\\01-create-database-and-tables.sql`
	3. `\i .\\PostgreSQL-StepByStep\\scripts\\02-constraints-and-keys.sql`
	4. `\i .\\PostgreSQL-StepByStep\\scripts\\03-crud-basics.sql`
	5. Continue with remaining chapter scripts in sequence

## Contents
- 00 — Orientation & Tools
- 01 — Database & Tables
- 02 — Constraints & Keys
- 03 — CRUD Basics
- 04 — Joins & Set Ops
- 05 — Aggregations & Window Functions
- 06 — Views & Functions (PL/pgSQL)
- 07 — Transactions & Isolation
- 08 — Indexes & Performance Basics
- 09 — EXPLAIN/ANALYZE & Tuning
- 10 — Triggers
- 11 — Security & Roles
- 12 — Backup/Restore & Maintenance
- 13 — Capstone: Analytics & Reporting

Each chapter has a markdown doc and companion `.sql` scripts under `scripts/`.
