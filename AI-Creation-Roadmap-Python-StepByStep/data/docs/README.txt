This is a tiny sample corpus to test the RAG flow.

Add a few .md or .txt files here. For example:
- ai_roadmap_intro.md: A short summary of the AI Creation Roadmap.
- fastapi_notes.txt: Some tips about building APIs with FastAPI.
- vector_search.md: Notes about BM25 versus embedding + pgvector.

The ingest script will index these files with a lightweight BM25 index (no DB required).
Then the API can load top-k snippets as "context" for answers.
