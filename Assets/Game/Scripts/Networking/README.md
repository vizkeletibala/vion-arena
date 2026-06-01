# Assets/Game/Scripts/Networking

Milestone 3 keeps networking as a seam, not a multiplayer implementation. The local greybox remains playable without a transport package, relay, account service, or backend dependency.

## Current contracts

- `AuthorityMode` declares the runtime authority choice:
  - `LocalPrototype` is the Milestone 1 default and keeps all gameplay in the Unity client.
  - `ServerAuthoritative` is a future mode for an EC2-hosted headless build.
- `AuthorityDecision` is the transport-agnostic accept/reject result that server validation should return.
- `IClientCommandAuthorizer` is the narrow command-validation port for future movement, combat, loot pickup, and inventory/equip checks.
- `LocalPrototypeAuthorizer` accepts valid local commands so current MonoBehaviours can keep running while preserving the future server call shape.
- `ClientConnectionConfig` holds host/port/TLS/player-alias values without selecting a networking stack.

## Future dedicated-server extension points

1. Replace or wrap `LocalPrototypeAuthorizer` with a server-backed implementation that owns authoritative validation.
2. Keep client-facing readers such as `PlayerInputReader` as input adapters; send named commands through the authorizer/transport seam instead of letting networking code read Unity input directly.
3. Keep gameplay data in ScriptableObjects (`GameModeDefinition`, weapon/enemy/loot definitions) so a Linux server build can load the same tuning as the Windows client.
4. Let `MatchBootstrap.ConnectionConfig` feed the first client connection attempt when `GameModeDefinition.AuthorityMode == ServerAuthoritative` and `AutoConnectOnClientLaunch` is enabled.
5. Build the Linux headless target from Unity as `LinuxServer`; package the resulting executable with `deploy/Dockerfile.vitrial-server` and `deploy/docker-compose.vitrial-server.yml` once real build artifacts exist.

## Intentionally deferred

- Transport/package selection (Unity Netcode, Mirror, Steam, or custom sockets).
- Authentication, matchmaking, persistence, MMO sharding, lobby services, and anti-cheat.
- Server tick simulation, state replication, prediction/reconciliation, and lag compensation.
- Real EC2 deployment gates beyond the existing placeholder Docker/Compose skeleton.
