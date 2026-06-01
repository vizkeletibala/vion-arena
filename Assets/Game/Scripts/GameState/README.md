# Assets/Game/Scripts/GameState

Milestone 1 match state remains intentionally small and local to the Unity client.

- `MatchBootstrap` applies lightweight game-mode defaults such as target frame rate when a `GameModeDefinition` is assigned. It also exposes `ConnectionConfig`, `CurrentAuthorityMode`, `CommandAuthorizer`, and `ShouldAttemptClientConnection` so future client connection code has a single bootstrap surface to consume.
- `GameModeDefinition` is the runtime config holder for match duration, frame rate, authority mode, server host/port, secure transport flag, auto-connect opt-in, and the expected `LinuxServer` dedicated-server build target.
- `MatchLoopController` lives on the `PrototypeArena` scene root. It finds the Player-tagged `VitrialPlayer`, subscribes to its `Health.Died` event, and reloads the active scene after player death. Backspace is also wired as a manual restart key for repeated Windows playtest passes.

Future work should keep this layer thin until a server-authoritative match/session model exists under `Assets/Game/Scripts/Networking/`. Dedicated-server work should extend `MatchBootstrap` and `GameModeDefinition` rather than adding scene-specific connection constants.
