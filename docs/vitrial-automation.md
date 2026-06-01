# Vitrial EC2 Automation And Dedicated Server Plan

This document is the Milestone 2 CI/server foundation for the Vitrial Unity lane. It deliberately reuses the existing Vion Project stack instead of introducing a second deployment island.

## Responsibility split

| Area | Runs now on EC2/Jenkins | Deferred to Windows Unity / licensed Unity build lane |
| --- | --- | --- |
| Repo checkout | clone/pull this repo in Jenkins workspace | none |
| Static scaffold validation | JSON syntax for `Packages/manifest.json` and `Assets/Game/Vitrial.Game.asmdef`; required Unity folder/source paths; C# brace-balance sanity check via `scripts/validate-vitrial-headless.sh` | Unity import, compilation, playmode tests, scene validation |
| Existing web stack | frontend lint/test/build; backend ruff/pytest; Docker build/push/deploy/smoke test | none |
| Unity dedicated server | placeholder Dockerfile/compose/pipeline stages are present but disabled | produce `Builds/LinuxServer/VitrialServer.x86_64` with Unity 2022.3 LTS Linux dedicated-server tooling and valid licensing |
| Observability | Docker stdout logs, labels for Promtail/Loki, future Prometheus scrape labels | in-process game metrics endpoint and structured server events |

Milestone 1 gameplay remains validated in the Windows Unity Editor. EC2 automation should never fail Milestone 1 because Unity CLI/editor licensing is unavailable on the host.

## Jenkins pipeline skeleton

`Jenkinsfile` keeps the existing Vion stack flow:

1. `Vitrial Headless Scaffold Checks`: runs `scripts/validate-vitrial-headless.sh` inside `python:3.12-slim` using the same `--volumes-from "$HOSTNAME"` Jenkins workspace-sharing pattern already required by this Docker-socket Jenkins setup.
2. Existing frontend checks.
3. Existing backend checks.
4. Existing frontend/backend Docker image build and push to `localhost:5000`.
5. Future Unity server image build, gated by `ENABLE_UNITY_SERVER_BUILD=true`.
6. Future Unity server compose deploy, also gated by `ENABLE_UNITY_SERVER_BUILD=true`.
7. Existing app deploy and smoke tests.

The Unity server stages are intentionally disabled by default. Enable them only after a licensed build lane writes the expected Linux server artifact:

```text
Builds/LinuxServer/VitrialServer.x86_64
```

When enabled, Jenkins builds `deploy/Dockerfile.vitrial-server`, pushes `localhost:5000/vitrial-server:${BUILD_NUMBER}`, and starts `deploy/docker-compose.vitrial-server.yml` with the `vitrial-server` profile.

## Dedicated server Docker direction

`deploy/Dockerfile.vitrial-server` is a runtime image skeleton, not a Unity builder image. It expects a prebuilt Linux dedicated-server directory and fails loudly if `VitrialServer.x86_64` is missing. This keeps the current EC2 lane honest: it can package and deploy server output later, but it does not pretend to run Unity builds without Unity tooling/licensing.

Future licensed build options:

- Windows workstation creates Linux dedicated-server build and commits/uploads an artifact for Jenkins.
- A separate Unity Builder agent with proper license produces `Builds/LinuxServer/` and archives it for this repo pipeline.
- A future Jenkins node with Unity installed runs build scripts, after credentials and license handling are explicitly documented.

Do not check `Builds/` into git unless the team deliberately changes the artifact policy; it is ignored today as generated output.

## Docker Compose service design

`deploy/docker-compose.vitrial-server.yml` defines a future `vitrial-server` service with:

- image from the local Vion registry (`localhost:5000/vitrial-server:dev` by default);
- `profiles: [vitrial-server]` so it never starts accidentally with normal app deploys;
- internal Vion network attachment only (`vion-project_internal` by default);
- no Traefik route and no public port publishing by default;
- Promtail/Loki logging labels (`logging.enabled=true`, `logging.stack=vion`, `logging.service=vitrial-server`);
- Prometheus label placeholders for a future `/metrics` endpoint on port `9100`.

Public gameplay traffic, if needed later, should be opened deliberately with firewall/TLS/auth decisions documented. Do not expose dashboards or game admin endpoints through Traefik without auth/TLS.

## Logging and monitoring plan

Initial server process requirements:

- write structured JSON logs to stdout;
- include fields such as `match_id`, `server_id`, `map`, `player_count`, `event`, `level`, and `duration_ms` where relevant;
- keep logs free of tokens, credentials, or raw player PII;
- emit startup/shutdown and match lifecycle events.

Vion stack integration:

- Promtail discovers the container through Docker labels and ships stdout to Loki.
- Grafana queries should start with `{service="vitrial-server"}`.
- Prometheus scraping stays disabled until the server exposes real metrics; then set `VITRIAL_PROMETHEUS_SCRAPE=true` and publish `/metrics` on `VITRIAL_METRICS_PORT`.

Recommended future metrics:

- server process up/time since start;
- active matches and connected players;
- tick duration and frame budget overruns;
- matchmaking/session allocation counts;
- disconnect/error counts;
- memory and CPU from cAdvisor for the container.

## EC2 checkout/basic validation story

Headless checks available today from a clean EC2 checkout:

```bash
sh scripts/validate-vitrial-headless.sh
python3 -m json.tool Packages/manifest.json >/dev/null
python3 -m json.tool Assets/Game/Vitrial.Game.asmdef >/dev/null
```

When Docker Compose v2 is available, also validate the future server compose file:

```bash
docker compose -f deploy/docker-compose.vitrial-server.yml config >/dev/null
```

Optional existing app checks remain separate from Unity validation:

```bash
cd frontend && npm ci && npm run lint && npm run test && npm run build
cd backend && python -m venv .venv && . .venv/bin/activate && pip install -r requirements-dev.txt && python -m ruff check app tests && pytest
```

Deferred until Unity tooling/licensing exists:

- Unity Editor project import on EC2;
- C# compile through Unity assemblies;
- editmode/playmode tests;
- Linux dedicated-server build generation;
- scene/prefab validation against `Assets/Game/Scenes/PrototypeArena.unity`.

## Rollback notes

The future server deploy is a Docker Compose service using a tagged image from the local registry. Rollback should be either:

```bash
VITRIAL_SERVER_IMAGE=localhost:5000/vitrial-server:<known-good-build> \
INTERNAL_NETWORK=vion-project_internal \
docker compose -f deploy/docker-compose.vitrial-server.yml --profile vitrial-server up -d
```

or, for immediate shutdown:

```bash
docker compose -f deploy/docker-compose.vitrial-server.yml --profile vitrial-server down
```
