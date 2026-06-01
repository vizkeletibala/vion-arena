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
- `deploy/Dockerfile.vitrial-server` and `deploy/docker-compose.vitrial-server.yml` as gated placeholders for a future Unity Linux dedicated server
- A Jenkins pipeline that tests, builds, pushes, deploys, and smoke-tests the current app while keeping future Unity server packaging disabled until Unity build artifacts exist

## Repository Layout

```text
frontend/        React + TypeScript + Vite canvas client
backend/         FastAPI API, in-memory score store, tests, metrics, logs
Assets/Game/     Unity Vitrial greybox prototype source lane
Packages/        Minimal Unity package manifest for opening the repo as a project
ProjectSettings/ Minimal Unity project metadata; Unity fills this out on Windows
deploy/          Application compose file plus future Vitrial server Docker/Compose skeleton
docs/            Platform notes plus Unity scaffold and EC2 automation notes
scripts/         Smoke-test and headless scaffold validation helpers
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

### Unity / Vitrial

Unity greybox prototype work lives under `Assets/Game/`. Open the repository root in Unity 2022.3 LTS on Windows; the EC2 host is only used for source validation, Jenkins orchestration, Docker packaging, and future dedicated-server deployment, not to run the Unity Editor.

Canonical Milestone 1 scene path: `Assets/Game/Scenes/PrototypeArena.unity`. The scene is a hand-authored greybox slice that wires together `VitrialPlayer`, `StarterRifle`, `GreyboxChaser`, loot drops, pickup/inventory UI, and a scene restart loop. See `docs/vitrial-unity.md` for folder conventions and handoff notes, and `docs/vitrial-automation.md` for the EC2/Jenkins/server automation boundary.

#### Windows Milestone 1 playtest

1. Install Unity Hub and Unity Editor `2022.3.55f1` or another 2022.3 LTS patch. If Unity Hub updates the patch version, let it update `ProjectSettings/ProjectVersion.txt` and review the diff before committing.
2. Open this repository root as the Unity project. Do not open `frontend/` or `backend/` as the Unity project; those remain the pre-existing browser-game stack.
3. In the Project window open `Assets/Game/Scenes/PrototypeArena.unity`.
4. Press Play. If Unity asks to import/normalize assets, allow it, then save intentional `.unity`, `.prefab`, `.asset`, `.meta`, and ProjectSettings changes only.
5. Validate the loop:
   - WASD moves, mouse looks, Left Shift sprints, and Space jumps.
   - Left Mouse fires the starter rifle, R reloads, and the HUD updates ammo/reload state.
   - The GreyboxChaser spawns, chases the Player-tag object, takes rifle damage, dies, and drops loot.
   - Walking through loot shows the pickup prompt, adds the item to inventory, and equips the first weapon pickup.
   - Tab opens inventory comparison, Up/Down changes selection, and Enter or E equips the selected item.
   - Let the enemy kill the player to verify the scene restarts after the death delay; Backspace manually restarts the scene for repeated playtest passes.
6. Stop Play Mode before committing; never commit `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, or user-local Unity settings.

Windows-only validation is intentionally left to Andrew because the EC2 host used by agents does not have the Unity Editor or graphics stack. Headless checks available on EC2 are source/structure checks such as `sh scripts/validate-vitrial-headless.sh`, `python3 -m pytest tests/test_unity_player_lane.py tests/test_unity_weapon_lane.py tests/test_unity_enemy_lane.py tests/test_unity_loot_inventory_lane.py tests/test_unity_ui_lane.py tests/test_unity_milestone1_integration.py -q`, JSON validation for `Packages/manifest.json` and `Assets/Game/Vitrial.Game.asmdef`, and `git diff --check`.

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
docker build -t localhost:5000/vion-arena-backend:dev -f backend/Dockerfile backend
```

Build the frontend image:

```bash
docker build \
  --build-arg VITE_API_BASE_URL=http://api.arena.vion.test \
  -t localhost:5000/vion-arena-frontend:dev \
  -f frontend/Dockerfile frontend
```

Push images to the local registry:

```bash
docker push localhost:5000/vion-arena-backend:dev
docker push localhost:5000/vion-arena-frontend:dev
```

The shared platform still routes the registry UI and HTTP API at `registry.localhost`, but Docker pushes and pulls on this machine should use the directly published endpoint `localhost:5000`.

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
BACKEND_IMAGE=localhost:5000/vion-arena-backend:dev \
FRONTEND_IMAGE=localhost:5000/vion-arena-frontend:dev \
APP_ALLOWED_ORIGINS=http://arena.vion.test \
docker compose -f deploy/docker-compose.app.yml up -d
```

## Jenkins Pipeline

`Jenkinsfile` is designed for the local Jenkins described in `docs/platform/jenkins.md`. It now has an explicit split between checks that can run headlessly on EC2 today and Unity/server work that must wait for licensed Unity build output. See `docs/vitrial-automation.md` for the full Vitrial automation and dedicated-server plan.

Pipeline stages:

1. Run Vitrial headless scaffold checks in a Python container
2. Run frontend checks in a Node container
3. Run backend linting and pytest in a Python container
4. Build Docker images for the current app
5. Push images to `localhost:5000`
6. Optionally build/push/deploy a future Unity Linux dedicated-server image when `ENABLE_UNITY_SERVER_BUILD=true` and `Builds/LinuxServer/VitrialServer.x86_64` exists
7. Deploy the current app with `deploy/docker-compose.app.yml`
8. Run `scripts/smoke-test.sh`

Because Jenkins has Docker socket access, it can run the pipeline without installing Node or Python on the agent itself.

The pipeline uses `--volumes-from "$HOSTNAME"` for the temporary test containers instead of bind-mounting `$PWD`. That matters because Jenkins talks to the host Docker daemon through `/var/run/docker.sock`, so container-internal paths cannot be mounted reliably with `-v "$PWD":...`.

## Observability

Backend logs are emitted as structured JSON on stdout so Promtail and Loki can ingest them without extra parsing work. The backend also exposes Prometheus metrics at `/metrics`.

The future Vitrial dedicated server should follow the same Vion stack contracts: structured JSON logs on stdout for Loki/Promtail, `logging.*` Docker labels in `deploy/docker-compose.vitrial-server.yml`, and a real `/metrics` endpoint before `VITRIAL_PROMETHEUS_SCRAPE=true` is enabled. The server compose profile is internal-only by default and has `traefik.enable=false` so dashboards/admin surfaces are not accidentally exposed without auth/TLS.

Application labels in `deploy/docker-compose.app.yml` follow the platform contracts from `docs/platform/observability.md`:

- `prometheus.scrape=true`
- `prometheus.port=8000`
- `prometheus.path=/metrics`
- `logging.enabled=true`
- `logging.stack=vion`
- `logging.service=vion-arena-backend`

If the shared platform provisioning from `docs/platform/observability.md` is present, Grafana also includes persistent dashboards for this app:

- `Vion Arena App Health`
- `Vion Arena Containers & Platform Health`

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
- If `npm ci` fails inside the Jenkins frontend check even though `package-lock.json` exists in the repo, verify the pipeline still uses `--volumes-from "$HOSTNAME"` for its helper containers.
- If smoke tests fail on hostnames, use `curl -H 'Host: ...' http://localhost/...` or add local DNS entries for `arena.vion.test` and `api.arena.vion.test`.

## Scope Guardrails

This first version intentionally does not include PostgreSQL, Redis, authentication, or Kubernetes. Scores remain in memory so the CI/CD and observability flow stays easy to understand.
