import os
import sys
from fastapi import FastAPI
from pydantic import BaseModel
from dotenv import load_dotenv
from tenacity import retry, stop_after_attempt, wait_exponential
import httpx

load_dotenv()

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY", "")
OPENAI_API_BASE = os.getenv("OPENAI_API_BASE", "https://api.openai.com/v1")
OPENAI_CHAT_MODEL = os.getenv("OPENAI_CHAT_MODEL", "gpt-4o-mini")
TOP_K = int(os.getenv("TOP_K", "4"))

# ensure project root on sys.path for `rag` import
PROJECT_ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
if PROJECT_ROOT not in sys.path:
    sys.path.append(PROJECT_ROOT)

try:
    from rag.retrieve import top_k as rag_top_k  # type: ignore
except Exception:
    rag_top_k = None

app = FastAPI(title="AI Baseline API")

class ChatRequest(BaseModel):
    message: str

class ChatResponse(BaseModel):
    answer: str
    sources: list[str] = []

@retry(stop=stop_after_attempt(2), wait=wait_exponential(multiplier=0.5, min=0.5, max=2))
async def openai_chat(system: str, user: str) -> str:
    if not OPENAI_API_KEY:
        return f"[echo] {user}"
    headers = {"Authorization": f"Bearer {OPENAI_API_KEY}"}
    payload = {
        "model": OPENAI_CHAT_MODEL,
        "messages": [
            {"role": "system", "content": system},
            {"role": "user", "content": user},
        ],
    }
    async with httpx.AsyncClient(timeout=30) as client:
        r = await client.post(f"{OPENAI_API_BASE}/chat/completions", headers=headers, json=payload)
        r.raise_for_status()
        data = r.json()
        return data["choices"][0]["message"]["content"]

@app.post("/chat", response_model=ChatResponse)
async def chat(req: ChatRequest):
    system = "You are a concise assistant. Include sources if context is provided."
    sources: list[str] = []
    context = ""

    # Try RAG retrieval if available
    if rag_top_k is not None:
        try:
            hits = rag_top_k(req.message, TOP_K)
            if hits:
                sources = [doc_id for doc_id, _ in hits]
                context = "\n\n".join(text for _, text in hits)
        except Exception:
            # missing index or other issue → proceed without context
            pass

    user = req.message if not context else f"Context:\n{context}\n\nQuestion: {req.message}"
    answer = await openai_chat(system, user)
    return ChatResponse(answer=answer, sources=sources)

@app.get("/health")
async def health():
    return {"ok": True}
