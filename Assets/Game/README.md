# Vitrial Unity Lane

Canonical Unity root: `Assets/Game/`

Milestone 1 lanes should target these paths:

- Canonical greybox scene: `Assets/Game/Scenes/PrototypeArena.unity` (create in the Unity Editor on Windows; this EC2 scaffold documents the path but does not hand-author a Unity scene file).
- Runtime scripts: `Assets/Game/Scripts/`
- Player: `Assets/Game/Scripts/Player/` and `Assets/Game/Prefabs/Player/`
- Weapons: `Assets/Game/Scripts/Weapons/`, `Assets/Game/ScriptableObjects/Weapons/`, `Assets/Game/Prefabs/Weapons/`
- Enemies: `Assets/Game/Scripts/Enemies/`, `Assets/Game/ScriptableObjects/Enemies/`, `Assets/Game/Prefabs/Enemies/`
- Loot and inventory: `Assets/Game/Scripts/Loot/`, `Assets/Game/Scripts/Inventory/`, `Assets/Game/ScriptableObjects/Loot/`, `Assets/Game/Prefabs/Loot/`
- HUD/UI: `Assets/Game/Scripts/UI/` and `Assets/Game/Prefabs/UI/`
- Match flow/state: `Assets/Game/Scripts/GameState/`
- Future server-authoritative seams: `Assets/Game/Scripts/Networking/`

Architecture guardrails:

- Keep MonoBehaviours small and scene/prefab focused.
- Put tunable combat, loot, enemy, and game-mode data in ScriptableObjects.
- Keep gameplay decisions isolated from presentation so future dedicated-server logic can reuse deterministic rules.
- Treat Networking scripts as seams/adapters for future server-authoritative work, not as transport-specific code yet.
