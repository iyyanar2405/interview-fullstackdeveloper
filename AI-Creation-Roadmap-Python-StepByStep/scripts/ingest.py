import os
import glob
import uuid
from typing import List, Tuple

from dotenv import load_dotenv
from rank_bm25 import BM25Okapi

load_dotenv()

DOCS_DIR = os.getenv("DOCS_DIR", os.path.join(os.path.dirname(__file__), "..", "data", "docs"))

# naive whitespace tokenizer
def _tokenize(text: str) -> List[str]:
    return [t.lower() for t in text.split() if t.strip()]


def load_docs() -> List[Tuple[str, str]]:
    files = glob.glob(os.path.join(DOCS_DIR, "**", "*.md"), recursive=True)
    files += glob.glob(os.path.join(DOCS_DIR, "**", "*.txt"), recursive=True)
    docs: List[Tuple[str, str]] = []
    for f in files:
        try:
            with open(f, "r", encoding="utf-8", errors="ignore") as fh:
                text = fh.read()
                docs.append((f, text))
        except Exception as e:
            print(f"Failed reading {f}: {e}")
    return docs


def build_bm25(corpus: List[Tuple[str, str]]):
    tokenized = [_tokenize(text) for _, text in corpus]
    return BM25Okapi(tokenized)


def main():
    corpus = load_docs()
    if not corpus:
        print("No docs found. Put .md or .txt files under data/docs.")
        return
    index = build_bm25(corpus)

    # Save index and docs to a simple pickle artefact for demo purposes
    import pickle
    artefact_path = os.path.join(os.path.dirname(__file__), "..", "rag", "bm25_index.pkl")
    with open(artefact_path, "wb") as fh:
        pickle.dump({"corpus": corpus, "index": index}, fh)
    print(f"Indexed {len(corpus)} docs → {artefact_path}")


if __name__ == "__main__":
    main()
