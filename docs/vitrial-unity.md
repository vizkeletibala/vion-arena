# Vitrial Unity Scaffold

This repository now has a Unity lane alongside the existing browser-game stack.

## Coexistence with the current platform code

- `frontend/` and `backend/` remain the current React/FastAPI browser-game and DevOps demo stack.
- Unity source assets live under `Assets/Game/` at the repo root so Unity can open the repository root as a project.
- `Packages/` and `ProjectSettings/` contain the minimal Unity project metadata needed for Unity Hub to recognize the repo.
- `Library/`, `Temp/`, `Obj/`, `Build/`, user settings, and other generated Unity caches are intentionally ignored.

## Canonical paths for downstream lanes

- Scene: `Assets/Game/Scenes/PrototypeArena.unity`
- Assembly definition: `Assets/Game/Vitrial.Game.asmdef`
- Player scripts/prefabs: `Assets/Game/Scripts/Player/`, `Assets/Game/Prefabs/Player/`
- Weapons scripts/data/prefabs: `Assets/Game/Scripts/Weapons/`, `Assets/Game/ScriptableObjects/Weapons/`, `Assets/Game/Prefabs/Weapons/`
- Enemies scripts/data/prefabs: `Assets/Game/Scripts/Enemies/`, `Assets/Game/ScriptableObjects/Enemies/`, `Assets/Game/Prefabs/Enemies/`
- Loot and inventory: `Assets/Game/Scripts/Loot/`, `Assets/Game/Scripts/Inventory/`, `Assets/Game/ScriptableObjects/Loot/`, `Assets/Game/Prefabs/Loot/`
- HUD/UI: `Assets/Game/Scripts/UI/`, `Assets/Game/Prefabs/UI/`
- Match state: `Assets/Game/Scripts/GameState/`
- Future server-authoritative seams and runtime connection config: `Assets/Game/Scripts/Networking/`

## EC2/Jenkins automation boundary

The EC2/Vion stack lane may run source-level checks, Docker packaging, Jenkins orchestration, Docker Compose deployment, and Loki/Grafana/Prometheus integration. It must not become a Milestone 1 gate for Unity Editor import, C# compilation, scene validation, or Linux dedicated-server build generation until Unity licensing/tooling is explicitly available there.

Current headless validation entrypoint:

```bash
sh scripts/validate-vitrial-headless.sh
```

Future Linux server packaging and deployment skeletons live in:

- `deploy/Dockerfile.vitrial-server`
- `deploy/docker-compose.vitrial-server.yml`
- `docs/vitrial-automation.md`

For the CI/CD recommendation that separates current EC2/Jenkins static checks from real Unity-capable compile/playmode/build lanes, see `docs/vitrial-ci-cd-strategy.md`.

## Windows/Unity Editor follow-up

This Milestone 1 slice was assembled on a headless EC2 host and still requires Unity Editor validation on Windows. On Windows:

1. Install/open with Unity 2022.3 LTS. If Andrew standardizes on a different 2022.3 patch, let Unity update `ProjectSettings/ProjectVersion.txt`.
2. Open the repository root as the Unity project.
3. Open the canonical scene at `Assets/Game/Scenes/PrototypeArena.unity` and press Play.
4. Verify movement/look/sprint/jump, starter rifle fire/reload, enemy death, loot pickup, inventory/equip UI, and the death/manual restart loop documented in the top-level README.
5. Let Unity generate or normalize `.meta` files for imported assets and commit only intentional scene/prefab/data/settings changes.
6. Keep `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, and user-local settings out of git.

## Architecture notes

The initial scripts intentionally establish seams rather than full gameplay:

- ScriptableObjects define game mode, weapons, enemies, and loot data.
- MonoBehaviours are small anchors/components for scene and prefab composition.
- `DamagePayload` and `IDamageable` provide a narrow combat contract for weapons/enemies.
- `Networking/AuthorityMode`, `AuthorityDecision`, `ClientConnectionConfig`, and `IClientCommandAuthorizer` are transport-agnostic placeholders for later server-authoritative validation.

## Milestone 3 client/server seam prep

The path from the local greybox to an EC2-hosted headless server should stay incremental:

1. Keep the Windows client playable in `AuthorityMode.LocalPrototype` until the first real transport package is chosen.
2. Store runtime knobs on `GameModeDefinition`: authority mode, server host, server port, secure transport flag, auto-connect opt-in, and dedicated server build target (`LinuxServer`).
3. Let `MatchBootstrap` be the scene-level extension point for future connection startup. It already exposes `ConnectionConfig`, `CurrentAuthorityMode`, `CommandAuthorizer`, and `ShouldAttemptClientConnection` so later work does not need hardcoded connection constants in gameplay scripts.
4. Route future movement, combat, loot pickup, and inventory/equip requests through an `IClientCommandAuthorizer` implementation. The current `LocalPrototypeAuthorizer` accepts local commands; a server-backed authorizer should own validation in the headless build.
5. Generate Unity Linux dedicated-server artifacts outside EC2 until Unity licensing/tooling is installed there, then package the build with `deploy/Dockerfile.vitrial-server` and `deploy/docker-compose.vitrial-server.yml`.

Initial client connection flow design:

- Client launches `PrototypeArena` and `MatchBootstrap` reads its assigned `GameModeDefinition`.
- If `AuthorityMode` remains `LocalPrototype`, no network connection is attempted and Milestone 1 behavior is unchanged.
- If `AuthorityMode` is `ServerAuthoritative` and `AutoConnectOnClientLaunch` is enabled, a future transport adapter reads `MatchBootstrap.ConnectionConfig`, connects to the configured host/port, registers the local player alias, then sends named client commands through the authorizer/transport seam.
- The headless server should load the same ScriptableObject tuning where possible and become authoritative over player state, enemy state, loot rolls/pickups, and equipment changes.

Intentionally deferred: transport selection, authentication, matchmaking, persistence, MMO sharding, state replication, client prediction/reconciliation, lag compensation, anti-cheat, and real production observability endpoints for the Unity server.
