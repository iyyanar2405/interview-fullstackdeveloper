# 03 — Response Parsing & Validation (Day 10–12)

Goal: Parse JSON responses and add retry on failure.

Steps
1) In `Program.cs`, after `ChatAsync`, attempt `JsonDocument.Parse` on reply
2) If parse fails, prepend: "Reply strictly as JSON" and retry once
3) Extract fields: `answer`, `sources` (string[])

Artifacts
- Snippet or notes of parsing approach
- 2 failing cases and the improved prompt/retry result
