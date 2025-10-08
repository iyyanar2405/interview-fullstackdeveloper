# 08 — Multi-turn Orchestration & Memory (Day 25–27)

Goal: Keep short-term memory across turns.

Steps
1) Maintain last N user Q&A pairs in memory
2) Prepend memory to the next prompt (bounded to ~1–2k tokens)
3) Test follow-ups: “and what about X?”

Artifacts
- Memory design (queue/window size)
- Example of successful follow-up understanding
