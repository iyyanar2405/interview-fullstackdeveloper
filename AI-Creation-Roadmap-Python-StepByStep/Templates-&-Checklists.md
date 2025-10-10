# Templates & Checklists — AI Creation (Python)

## PR Template
- Context, change summary, risks, tests, rollback, links

## Prompt Template
System: You are a reliable assistant. Respond in strict JSON.
User: <task>
Output: { "answer": string, "sources": string[] }

## RAG Checklist
- Chunk size/overlap set; embedder/version pinned
- Metadata filters; top-k tuned; re-rank optional
- Citations attached to outputs

## Safety Checklist
- PII redaction; jailbreak testset; allowlist for fetch tools
- JSON schema validation; profanity/toxicity filter

## Runbook Template
- Trigger, steps, validation, rollback, contacts
