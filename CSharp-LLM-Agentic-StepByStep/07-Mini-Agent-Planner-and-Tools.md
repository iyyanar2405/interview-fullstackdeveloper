# 07 — Mini-Agent: Planner + Tools (Day 22–24)

Goal: Add a simple planner that decides tool calls, then synthesizes.

Steps
1) Add a `Decide(input)` method: returns `none|calc|search|web`
2) If not `none`, call tool and append result to the prompt
3) Ask multi-step questions; confirm tool usage and final answer

Artifacts
- Planner rules
- 3 transcripts showing plan → tool → synthesis
