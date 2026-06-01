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
- Future server-authoritative seams: `Assets/Game/Scripts/Networking/`

## Windows/Unity Editor follow-up

This scaffold was created on a headless EC2 host and does not require the Unity Editor here. On Windows:

1. Install/open with Unity 2022.3 LTS. If Andrew standardizes on a different 2022.3 patch, let Unity update `ProjectSettings/ProjectVersion.txt`.
2. Open the repository root as the Unity project.
3. Create and save the canonical scene at `Assets/Game/Scenes/PrototypeArena.unity`.
4. Let Unity generate `.meta` files for imported assets and commit them with the scene/prefab/data assets they belong to.
5. Keep `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, and user-local settings out of git.

## Architecture notes

The initial scripts intentionally establish seams rather than full gameplay:

- ScriptableObjects define game mode, weapons, enemies, and loot data.
- MonoBehaviours are small anchors/components for scene and prefab composition.
- `DamagePayload` and `IDamageable` provide a narrow combat contract for weapons/enemies.
- `Networking/AuthorityMode` and `AuthorityDecision` are transport-agnostic placeholders for later server-authoritative validation.
