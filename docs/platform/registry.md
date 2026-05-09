# Registry

The stack includes a local Docker Registry service for storing built images close to the deployment environment.

## Current Service Definition

- image: `registry:2`
- routed hostname: `registry.localhost`
- direct Docker endpoint: `localhost:5000`
- internal app port: `5000`
- debug and metrics port: `5001`
- persistent volume: `registry-data`

Environment settings:

```text
REGISTRY_STORAGE_FILESYSTEM_ROOTDIRECTORY=/var/lib/registry
REGISTRY_HTTP_DEBUG_ADDR=0.0.0.0:5001
REGISTRY_HTTP_DEBUG_PROMETHEUS_ENABLED=true
REGISTRY_HTTP_DEBUG_PROMETHEUS_PATH=/metrics
```

## What This Enables

- local image pushes from developers or Jenkins
- deployment without depending on a third-party registry
- Prometheus scraping for registry metrics

## Current Access Pattern

The service is exposed through Traefik with:

```yaml
- traefik.http.routers.registry.rule=Host(`registry.localhost`)
- traefik.http.services.registry.loadbalancer.server.port=5000
```

It is also published directly on the host as:

```text
localhost:5000
```

That direct port is the reliable endpoint for `docker push` and `docker pull` on this machine.

The registry is also labeled for:

- logging through Promtail
- metrics scraping through Prometheus on port `5001`

## Example Use

Build and tag:

```bash
docker build -t localhost:5000/my-team/game-api:dev .
```

Push:

```bash
docker push localhost:5000/my-team/game-api:dev
```

Use in Compose:

```yaml
image: localhost:5000/my-team/game-api:dev
```

## Reusing This In A Game Repo

This is useful when:

- the game backend is deployed from Docker images
- CI should publish artifacts into a local environment
- staging or LAN environments should keep running without cloud dependencies

Suggested hostname normalization for a new repo:

- `registry.game.test`

That would better match the wildcard DNS pattern used by the other services.

If you do normalize the hostname in another repo, it is still worth keeping a direct host port for Docker CLI use unless you are also setting up a DNS name that Docker can reliably resolve on every machine.

## Important Security Note

This registry setup is local-development friendly, not production ready.

Current limitations:

- no TLS
- no authentication
- no image signing or policy enforcement

If the new repo will deploy outside a trusted LAN, add auth and TLS or use a managed registry.
