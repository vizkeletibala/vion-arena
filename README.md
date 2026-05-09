# Vion Arena

Vion Arena is a small 2D browser game built to exercise a local DevOps platform. The frontend is a React + TypeScript + Vite app that renders the game on an HTML canvas. The backend is a FastAPI service with an in-memory leaderboard, Prometheus metrics, and structured JSON logs.

## What Is Included

- A minimal playable arena game where the player collects orbs before the timer expires
- Score submission and leaderboard retrieval through FastAPI
- Backend endpoints for `/health`, `/version`, `/scores`, and `/metrics`
- Frontend Vitest coverage for movement, collision, and scoring logic
- Backend pytest coverage for health, version, scores, and metrics
- Dockerfiles for both services
- `deploy/docker-compose.app.yml` for a platform that already runs Traefik, Jenkins, registry, Prometheus, Grafana, Loki, Promtail, cAdvisor, and node-exporter
- A Jenkins pipeline that tests, builds, pushes, deploys, and smoke-tests the app

## Repository Layout

```text
frontend/   React + TypeScript + Vite canvas client
backend/    FastAPI API, in-memory score store, tests, metrics, logs
deploy/     Application compose file for the shared platform
docs/       Platform notes copied from the original stack
scripts/    Smoke-test helper used by Jenkins
```

## Local Development

### Frontend

```bash
cd frontend
npm ci
npm run lint
npm run test
npm run build
npm run dev -- --host 0.0.0.0
```

The Vite app expects the backend at `http://localhost:8000` by default. Override it with `VITE_API_BASE_URL`.

### Backend

```bash
cd backend
python -m venv .venv
. .venv/bin/activate
pip install -r requirements-dev.txt
python -m ruff check app tests
pytest
uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
```

Useful backend environment variables:

- `APP_VERSION`: value returned by `/version`
- `APP_ALLOWED_ORIGINS`: comma-separated CORS allowlist for the frontend
- `APP_NAME`: service name returned by `/health`

## Docker

Build the backend image:

```bash
docker build -t registry.localhost/vion-arena-backend:dev -f backend/Dockerfile backend
```

Build the frontend image:

```bash
docker build \
  --build-arg VITE_API_BASE_URL=http://api.arena.vion.test \
  -t registry.localhost/vion-arena-frontend:dev \
  -f frontend/Dockerfile frontend
```

Push images to the local registry:

```bash
docker push registry.localhost/vion-arena-backend:dev
docker push registry.localhost/vion-arena-frontend:dev
```

## Deployment With The Shared Platform

`deploy/docker-compose.app.yml` assumes Traefik and the observability stack already exist. The compose file joins two external networks:

- `edge`
- `internal`

If your platform stack created differently named networks, override them:

```bash
EDGE_NETWORK=my-edge INTERNAL_NETWORK=my-internal docker compose -f deploy/docker-compose.app.yml up -d
```

Default routed hostnames:

- Frontend: `arena.vion.test`
- Backend: `api.arena.vion.test`

Deploy with explicit images:

```bash
BACKEND_IMAGE=registry.localhost/vion-arena-backend:dev \
FRONTEND_IMAGE=registry.localhost/vion-arena-frontend:dev \
APP_ALLOWED_ORIGINS=http://arena.vion.test \
docker compose -f deploy/docker-compose.app.yml up -d
```

## Jenkins Pipeline

`Jenkinsfile` is designed for the local Jenkins described in `docs/platform/jenkins.md`.

Pipeline stages:

1. Run frontend checks in a Node container
2. Run backend linting and pytest in a Python container
3. Build Docker images
4. Push images to `registry.localhost`
5. Deploy with `deploy/docker-compose.app.yml`
6. Run `scripts/smoke-test.sh`

Because Jenkins has Docker socket access, it can run the pipeline without installing Node or Python on the agent itself.

## Observability

Backend logs are emitted as structured JSON on stdout so Promtail and Loki can ingest them without extra parsing work. The backend also exposes Prometheus metrics at `/metrics`.

Application labels in `deploy/docker-compose.app.yml` follow the platform contracts from `docs/platform/observability.md`:

- `prometheus.scrape=true`
- `prometheus.port=8000`
- `prometheus.path=/metrics`
- `logging.enabled=true`
- `logging.stack=vion`
- `logging.service=vion-arena-backend`

Useful checks after deployment:

```bash
curl -H 'Host: api.arena.vion.test' http://localhost/health
curl -H 'Host: api.arena.vion.test' http://localhost/metrics
curl -H 'Host: arena.vion.test' http://localhost/
sh scripts/smoke-test.sh
```

## Troubleshooting

- If Traefik does not route the app, verify the service is attached to the same external network as Traefik and that the `Host(...)` labels match your local DNS or hosts file.
- If Prometheus does not scrape the backend, check the Docker labels and confirm `/metrics` is reachable inside the container on port `8000`.
- If leaderboard submissions fail from the browser, confirm `APP_ALLOWED_ORIGINS` includes the frontend origin.
- If Jenkins can build but cannot deploy, verify it can access the same Docker daemon and that the target networks already exist.
- If smoke tests fail on hostnames, use `curl -H 'Host: ...' http://localhost/...` or add local DNS entries for `arena.vion.test` and `api.arena.vion.test`.

## Scope Guardrails

This first version intentionally does not include PostgreSQL, Redis, authentication, or Kubernetes. Scores remain in memory so the CI/CD and observability flow stays easy to understand.
