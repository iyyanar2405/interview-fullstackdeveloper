# Tools & Stack (Python)

## Core
- API: [FastAPI](./tools/fastapi/README.md), [Uvicorn](./tools/uvicorn/README.md)
- Orchestration: [LangChain](./tools/langchain/README.md), [LangGraph](./tools/langgraph/README.md)
- Embeddings & Vector DB:
	- Encoders: [OpenAI](./tools/embeddings-openai/README.md) / [E5](./tools/embeddings-e5/README.md) / [Instructor](./tools/embeddings-instructor/README.md)
	- Stores: [pgvector](./tools/vectordb-pgvector/README.md) / [Redis](./tools/vectordb-redis/README.md) / [Pinecone](./tools/vectordb-pinecone/README.md)
- Caching: [Redis](./tools/cache-redis/README.md) (semantic cache optional)
- Eval: [Ragas](./tools/eval-ragas/README.md), [DeepEval](./tools/eval-deepeval/README.md)
- Obs: [OpenTelemetry](./tools/obs-opentelemetry/README.md), [Langfuse](./tools/obs-langfuse/README.md), [Weights & Biases](./tools/obs-wandb/README.md)
- Data: [DuckDB](./tools/data-duckdb/README.md) / [Parquet](./tools/data-parquet/README.md) / [Pandas](./tools/data-pandas/README.md) / [Polars](./tools/data-polars/README.md)
- CI/CD: [GitHub Actions](./tools/cicd-github-actions/README.md), [Docker](./tools/container-docker/README.md)

## Providers
- LLM: OpenAI, Anthropic, Azure OpenAI; fallback to local (Llama/Mistral) via vLLM
- Storage: S3/Azure Blob/GCS; Warehouse: BigQuery/Snowflake/SQL

## Security
- Secrets manager, vault, KMS
- PII redaction libs, allowlist for fetch, E2E logging with masking

## Utilities
- Pydantic for schema validation
- Tenacity for retries with backoff
- httpx/requests for tools
