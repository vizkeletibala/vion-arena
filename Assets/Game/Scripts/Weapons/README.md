# Assets/Game/Scripts/Weapons

Starter rifle combat lives here and is intentionally presentation-free so UI, loot, enemies, and networking can bind to clean data/events.

## Runtime pieces

- `WeaponDefinition` is the Unity `ScriptableObject` stat contract. Downstream systems should read `DisplayName`, `Damage`, `RoundsPerMinute`, `TimeBetweenShots`, `MagazineSize`, `ReserveAmmo`, `ReloadSeconds`, and `Range` instead of scraping prefabs.
- `StarterRifleController` consumes `PlayerInputReader` (`fire: Left Mouse Button`, `reload: R`), enforces cadence through `TimeBetweenShots`, spends ammo, starts reloads, raycasts, and sends `DamagePayload` to `IDamageable.ApplyDamage` on hit targets.
- `WeaponAmmoState` owns `CurrentMagazine`, `ReserveAmmo`, `CanFire`, `ConsumeRound`, and `ReloadFromReserve` so HUD/inventory code can observe ammo without duplicating math.
- `WeaponHitResult` reports `DidHit`, `Damageable`, `Point`, `Normal`, and `Collider` from each shot for hit markers, audio, VFX, or later networking reconciliation.

## Data/assets

- `Assets/Game/ScriptableObjects/Weapons/StarterRifle.asset` is the starter tuning asset.
- `Assets/Game/Prefabs/Weapons/StarterRifle.prefab` is a reusable weapon prefab.
- `Assets/Game/Prefabs/Player/VitrialPlayer.prefab` already mounts a starter rifle child under `WeaponSocket` and wires `StarterRifleController.inputReader` to the player input component.

## Downstream hooks

- HUD: subscribe to `StarterRifleController.AmmoChanged`, `ReloadStarted`, and `ReloadCompleted`; do not add UI code to weapon logic.
- Enemies/props: implement `IDamageable.ApplyDamage(DamagePayload payload)` to receive rifle damage.
- Loot/equipment: reference a `WeaponDefinition` asset for stats, then swap the active weapon prefab/controller when inventory work lands.

## Windows Unity editor notes

Unity may regenerate `.meta` local IDs when Andrew opens the project. If references break, assign `StarterRifle.asset` to the `weaponDefinition` field and the player `PlayerInputReader` to the `inputReader` field on the `StarterRifleController` mounted under `VitrialPlayer/WeaponSocket/StarterRifle`.
