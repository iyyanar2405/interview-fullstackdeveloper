# MongoDB — Step by Step (00–13)

A practical path to learn MongoDB with runnable mongosh scripts and an e-commerce sample.

## Quickstart (Windows)
- Install: MongoDB Community Server + mongosh
- Connect: `mongosh "mongodb://localhost:27017"`
- Run scripts under `scripts/` to create the training DB and collections

### How to run the scripts
- Open PowerShell in the repo root
- Start shell: `mongosh "mongodb://localhost:27017"`
- Load a script, e.g. create DB/collections:
	- `load('MongoDB-StepByStep/scripts/01-create-db-and-collections.js')`
- Then continue chapter-by-chapter:
	- `load('MongoDB-StepByStep/scripts/02-documents-and-schema.js')`
	- `load('MongoDB-StepByStep/scripts/03-crud-basics.js')`
	- `load('MongoDB-StepByStep/scripts/04-indexes-and-query-patterns.js')`
	- `load('MongoDB-StepByStep/scripts/05-aggregation-framework.js')`
	- `load('MongoDB-StepByStep/scripts/06-transactions.js')` (requires replica set)
	- `load('MongoDB-StepByStep/scripts/07-data-modeling-patterns.js')`
	- `load('MongoDB-StepByStep/scripts/08-performance-and-profiling.js')`
	- `load('MongoDB-StepByStep/scripts/09-change-streams.js')` (requires replica set)
	- `load('MongoDB-StepByStep/scripts/10-validation-and-schema.js')`
	- `load('MongoDB-StepByStep/scripts/11-security-and-roles.js')`
	- `load('MongoDB-StepByStep/scripts/12-backup-restore-and-tools.js')`
	- `load('MongoDB-StepByStep/scripts/13-capstone-analytics-and-reporting.js')`

Notes
- Transactions and change streams require a replica set. For a quick local setup:
	1) Stop standalone mongod
	2) Start: `mongod --replSet rs0 --dbpath <path>`
	3) In mongosh: `rs.initiate()`
	4) Reconnect and run the scripts mentioned above

## Contents
- 00 — Orientation & Tools
- 01 — Database & Collections
- 02 — Documents & Schema Design (Schema-on-Read)
- 03 — CRUD Basics
- 04 — Indexes & Query Patterns
- 05 — Aggregation Framework
- 06 — Transactions (Replica Set)
- 07 — Data Modeling Patterns (Embedding vs Referencing)
- 08 — Performance & Profiling
- 09 — Change Streams
- 10 — Validation & Schema Governance
- 11 — Security & Roles
- 12 — Backup/Restore & Tools
- 13 — Capstone: Analytics & Reporting

Each chapter has a markdown doc and companion `.js` mongosh scripts under `scripts/`.
