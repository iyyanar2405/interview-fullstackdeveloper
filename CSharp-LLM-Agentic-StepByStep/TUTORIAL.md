# C# LLM & Agent Tutorial — Beginner → Advanced → Expert

This tutorial walks you from first API call to a small agent with tools and RAG, then to Anthropic Claude and a minimal MCP server.

## Prereqs
- .NET 8 SDK
- Optional: Anthropic and/or OpenAI API keys

## 1) Environment
- Copy `.env.example` to `.env` and set LLM_PROVIDER: echo | openai | anthropic
- For provider keys, set ANTHROPIC_API_KEY and/or OPENAI_API_KEY

## 2) Run the playground
- Build and run `src/AgentPlayground`
- Try: `calc: 3 * 7` and `search: rag`
- Ask a question. If provider=openai/anthropic, it will call the API; else echo mode

## 3) Prompt engineering & schemas
- System prompt is defined in `SimpleAgent` — edit it and observe behavior
- Ask the model to respond with a JSON schema (fields: answer, sources)
- Add minimal validation to parse/verify JSON, then retry on failure

## 4) Tools
- The agent supports `calc:` and `search:`; extend with a `web:` tool using HttpClient
- Decide when to invoke a tool (keyword rule or LLM suggestion), then call tool and synthesize answer

## 5) RAG
- `SimpleRag` offers a tiny hashed TF vector store; add more docs
- Implement reranking (e.g., overlap score) or switch provider to use embeddings when keys are present

## 6) Anthropic Claude
- Set provider=anthropic and ANTHROPIC_API_KEY
- Test replies and adjust system prompt for explicit tool instructions

## 7) Evaluations & safety
- Create canned test prompts and expected patterns
- Measure pass/fail; add refusal criteria for unsafe requests

## 8) MCP server
- Build and run `mcp/MinimalMcpServer`; send `{"jsonrpc":"2.0","id":1,"method":"ping"}` on stdin
- Extend with a `summarize` method that accepts text and returns a short summary

## 9) VibeCoding workflow
- Short loops: Write prompt → Run → Inspect → Tweak → Log findings in Artifacts/
- Keep a prompt portfolio with before/after and metrics (tokens, latency, pass rate)

## 10) Capstone
- Small agent solving a real task (e.g., FAQ assistant over your docs) with RAG and one tool
- Deliver: README, run steps, metrics, and safety notes
