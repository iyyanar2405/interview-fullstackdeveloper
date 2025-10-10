# Uvicorn — Step by Step

- What it is: ASGI server to run FastAPI.
- Install: pip install uvicorn
- Quickstart: `python -m uvicorn app.main:app --reload --port 8000`
- Exercises: Run behind reverse proxy, set workers with gunicorn+uvicorn workers.
- Troubleshooting: Address already in use → change port, stop prior process.
