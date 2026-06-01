# Assets/Game/Scripts/Enemies

Milestone 1 enemy loop assets live here.

## GreyboxChaser runtime contract

- `GreyboxChaser` prefab (`Assets/Game/Prefabs/Enemies/GreyboxChaser.prefab`) is a capsule-shaped greybox enemy with `CharacterController`, `Health`, `EnemyChaseController`, `EnemyDeathHook`, and `LootDropper` components.
- `EnemyChaseController` finds the player by the `Player` tag, chases inside `detectRange`, and applies contact-threat damage when inside `attackRange`.
- Weapons should damage enemies through `IDamageable.ApplyDamage(DamagePayload)`. The enemy `Health` component implements that interface and raises `Damaged` and `Health.Died` events.
- `EnemyDeathHook.OnDeath` is a UnityEvent intended as the clean integration point for loot drops, VFX, score, audio, and future encounter bookkeeping. `LootDropper` now subscribes to it and rolls `GreyboxChaserLootTable` when the chaser dies.
- `EnemySpawnPoint.Spawn` instantiates the configured enemy prefab and keeps a `SpawnedEnemy` reference so a scene can spawn one chaser without a monolithic enemy manager.

## Data

`Assets/Game/ScriptableObjects/Enemies/GreyboxChaser.asset` stores inspector-tunable health, movement, detection, attack range, contact damage, and cooldown values.

## PrototypeArena hookup

`PrototypeArena.unity` includes an `EnemySpawnPoint` configured to spawn the `GreyboxChaser` prefab at scene start.

remaining scene hookup for the final integrator: confirm Unity imports the hand-authored YAML on Andrew's Windows Unity workstation, including the `LootDropper` references to `GreyboxChaserLootTable` and `LootPickup`.
