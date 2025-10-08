# Templates & Checklists — C# LLM & Agents

## Prompt Template
System: You are a helpful, concise assistant. Use tools when beneficial.
User: <task>
Output: JSON with { "answer": string, "sources": string[] }

## Tool Decision Checklist
- Is there a deterministic sub-task (calc, lookup, fetch)?
- Do I have enough context? If not, RAG search first
- Can I answer safely without tools? If yes, proceed
- Otherwise: call tool, then synthesize

## RAG Checklist
- Split docs into chunks (300–800 tokens)
- Store embeddings/hashes; include doc id and metadata
- Top-K retrieve; rerank; pass to prompt with source tags
- Log retrieved sources with the answer

## Safety/Eval Template
- Disallow: PII exfiltration, credentials, self-harm content
- Test set: 20 prompts with expected patterns (regex or JSON schema)
- Track: pass rate, latency, token cost

## Incident Comms Template
- Summary
- Impacted users/scope
- Timeline
- Root cause
- Remediation & follow-ups
