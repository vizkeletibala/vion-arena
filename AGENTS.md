# AGENTS.md

## Project

This repository contains Vion Arena, a small 2D browser game used to exercise a local DevOps platform. Keep the project easy to understand and biased toward learning-friendly infrastructure integration over feature breadth.

## Stack

Frontend:
- React
- TypeScript
- Vite
- HTML Canvas
- Vitest

Backend:
- Python
- FastAPI
- pytest
- ruff
- prometheus-client

Deployment:
- Docker
- Docker Compose
- Jenkins
- Traefik
- Prometheus
- Grafana
- Loki
- Promtail
- local Docker registry

## Architecture Notes

- The frontend is a static Vite build served by Nginx.
- The game is intentionally minimal: one player, one arena, one orb stream, one timer.
- The backend stores scores in memory only. Do not introduce a database unless explicitly requested.
- The backend must keep `/health`, `/version`, `/scores`, and `/metrics` stable.
- The backend should log structured JSON to stdout.
- The backend should expose Prometheus metrics through `prometheus-client`.
- Deployments should keep matching the label conventions documented in `docs/platform`.

## Working Rules

- Prefer simple code over abstractions.
- Add tests for gameplay logic and backend endpoints when behavior changes.
- Keep Docker images small and readable.
- Preserve the CI/CD learning goal with each feature.
- Avoid PostgreSQL, Redis, authentication, and Kubernetes unless the user explicitly asks.
- Keep the app compatible with the shared platform contracts:
  - Traefik labels for routing
  - `prometheus.scrape=true` labels for metrics
  - `logging.enabled=true` labels for Loki ingestion

## Commands

Frontend:

```bash
cd frontend
npm ci
npm run lint
npm run test
npm run build
```

Backend:

```bash
cd backend
python -m venv .venv
. .venv/bin/activate
pip install -r requirements-dev.txt
python -m ruff check app tests
pytest
```

Docker:

```bash
docker build -t registry.localhost/vion-arena-backend:dev -f backend/Dockerfile backend
docker build --build-arg VITE_API_BASE_URL=http://api.arena.vion.test -t registry.localhost/vion-arena-frontend:dev -f frontend/Dockerfile frontend
docker compose -f deploy/docker-compose.app.yml up -d
```

Smoke test:

```bash
sh scripts/smoke-test.sh
```

## Files Future Agents Should Check First

- `README.md`
- `docs/platform/`
- `deploy/docker-compose.app.yml`
- `Jenkinsfile`
- `backend/app/main.py`
- `frontend/src/App.tsx`
