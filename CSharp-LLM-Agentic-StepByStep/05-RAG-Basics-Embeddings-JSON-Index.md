# 05 — RAG Basics (Embeddings + JSON Index) (Day 16–18)

Goal: Persist a tiny document index and retrieve context.

Steps
1) Extend `SimpleRag` to save/load `_docs` as JSON (id→text)
2) Add CLI commands: `add: <id> <text>` and `save: <path>` / `load: <path>`
3) Query and confirm top-K returns expected docs

Artifacts
- Example index JSON file
- Before/after retrieval examples
