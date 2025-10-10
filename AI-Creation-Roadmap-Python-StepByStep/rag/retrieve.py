import os
import pickle
from typing import List, Tuple

from rank_bm25 import BM25Okapi

ARTEFACT = os.path.join(os.path.dirname(__file__), "bm25_index.pkl")


def load_index():
    if not os.path.exists(ARTEFACT):
        raise FileNotFoundError("Index not found. Run scripts/ingest.py first.")
    with open(ARTEFACT, "rb") as fh:
        data = pickle.load(fh)
    return data["corpus"], data["index"]


def top_k(query: str, k: int = 4) -> List[Tuple[str, str]]:
    corpus, index: Tuple[List[Tuple[str, str]], BM25Okapi] = load_index()
    # tokenization mirrors ingest
    tokens = [t.lower() for t in query.split() if t.strip()]
    scores = index.get_scores(tokens)
    ranked = sorted(zip(corpus, scores), key=lambda x: x[1], reverse=True)[:k]
    return [(doc_id, text) for (doc_id, text), _ in ranked]


if __name__ == "__main__":
    import sys
    q = " ".join(sys.argv[1:]) or "what is this project about?"
    hits = top_k(q, 4)
    for i, (doc_id, text) in enumerate(hits, 1):
        print(f"[{i}] {doc_id}")
        print(text[:400], "...\n")
