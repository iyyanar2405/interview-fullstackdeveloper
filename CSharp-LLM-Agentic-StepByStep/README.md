# C# LLM & Agent Development — Step by Step (Beginner → Advanced → Expert)

Hands-on path to build LLM apps and agents in C#/.NET: prompt engineering, RAG, tool use, Anthropic/OpenAI clients, MCP servers, and vibeCoding workflows.

## Outcomes
- Beginner: Call LLMs from .NET, craft prompts, structure responses with schemas
- Advanced: Build a small agent with tools (HTTP, file, math), add RAG index
- Expert: Multi-step agent with evaluations, Anthropic Claude integration, MCP server

## Quickstart
- Install .NET 8 SDK
- Copy `.env.example` to `.env` and set keys if available (Anthropic, OpenAI)
- Build and run the sample console in `src/AgentPlayground`

## Structure
- src/AgentPlayground — .NET console app with LLM client abstraction and tools
- rag/ — simple embedding index and retriever (JSON-backed)
- mcp/ — minimal MCP server skeleton (stdio JSON-RPC)
- TUTORIAL.md — guided path from prompts → agents → RAG → MCP
- Templates-&-Checklists.md — prompts, evals, safety, incident comms
- Tracking-Workbook.md — weekly progress
- Artifacts/ — store your outputs

## Table of Contents
- [00 — Foundations & Environment](./00-Foundations-&-Environment.md)
- [01 — Prompt Engineering & Schemas](./01-Prompt-Engineering-&-Schemas.md)
- [02 — LLM Clients (OpenAI/Anthropic)](./02-LLM-Clients-OpenAI-Anthropic.md)
- [03 — Response Parsing & Validation](./03-Response-Parsing-&-Validation.md)
- [04 — Tool Use & Function Calling](./04-Tool-Use-&-Function-Calling.md)
- [05 — RAG Basics (Embeddings + JSON index)](./05-RAG-Basics-Embeddings-JSON-Index.md)
- [06 — RAG Retrieval + Re-ranking](./06-RAG-Retrieval-&-Re-ranking.md)
- [07 — Mini-Agent: Planner + Tools](./07-Mini-Agent-Planner-and-Tools.md)
- [08 — Multi-turn Orchestration & Memory](./08-Multi-turn-Orchestration-&-Memory.md)
- [09 — Evaluations & Safety](./09-Evaluations-&-Safety.md)
- [10 — Anthropic Claude Integration](./10-Anthropic-Claude-Integration.md)
- [11 — MCP Server (Model Context Protocol)](./11-MCP-Server-Model-Context-Protocol.md)
- [12 — Observability & Cost](./12-Observability-&-Cost.md)
- [13 — Capstone & Portfolio](./13-Capstone-&-Portfolio.md)

Links will be added as chapters are created.
