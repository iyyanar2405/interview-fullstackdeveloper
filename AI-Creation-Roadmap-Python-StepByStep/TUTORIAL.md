# Tutorial — Python Baseline & RAG Quickstart

Goal: Stand up a FastAPI endpoint that calls an LLM and optionally retrieves context from a lightweight retriever.

## Quickstart (Windows PowerShell)

Prereqs:
- Python 3.10+; PowerShell 5.1+

Setup:
1) Optional venv
	- python -m venv .venv; .\.venv\Scripts\Activate.ps1
2) Install requirements
	- python -m pip install --upgrade pip; pip install -r requirements.txt
3) Configure env
	- Copy `.env.example` to `.env` and set keys. If `OPENAI_API_KEY` is blank, the API will echo responses for smoke tests.

Prepare docs and build index:
1) Add a few `.md` or `.txt` files under `data/docs/` (see the example README.txt)
2) Build the BM25 index
	- python .\scripts\ingest.py

Run the API:
1) Start server
	- python -m uvicorn app.main:app --reload --port 8000
2) Health check (new terminal)
	- Invoke-WebRequest http://localhost:8000/health | Select -ExpandProperty Content
3) Chat test
	- Invoke-RestMethod -Method Post -Uri http://localhost:8000/chat -ContentType 'application/json' -Body '{"message":"Summarize the AI roadmap"}'

Notes:
- If you skip the ingest step, the API still works but without sources/context.
- The ingest script creates `rag/bm25_index.pkl`. Delete it to force a rebuild.
- You can switch to embedding + pgvector later; this demo keeps it minimal to get you productive fast.
