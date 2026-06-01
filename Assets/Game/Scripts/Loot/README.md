# Loot runtime contract

Milestone 1 loot is intentionally small but complete enough for the UI and designer lanes.

## Terminology

- `LootItemDefinition`: ScriptableObject authoring data for a lootable item family such as `RustyBurstRifle`.
- `LootItemInstance`: runtime rolled item. This is the object inventory and compare UI should read; it carries `InstanceId`, `Rarity`, `RollPercent`, `RolledStats`, `DisplayName`, `ItemType`, and `EquipmentSlot`.
- `ItemStatBlock`: shared comparable stats: `Damage`, `FireRate`, `MagazineSize`, `MaxHealth`, `MoveSpeed`.
- `LootRarity`: `Common`, `Uncommon`, `Rare`, `Epic`, `Legendary`. Rarity applies a multiplier via `LootItemDefinition.GetRarityMultiplier`.
- `LootTableDefinition`: ScriptableObject containing `dropChance` and weighted `LootDropEntry` rows.

## Drop flow

`LootDropper` is mounted on an enemy with `EnemyDeathHook`. On `EnemyDeathHook.OnDeath`, it calls `LootTableDefinition.TryRollDrop`, instantiates a `LootPickup` prefab, and passes the randomized `LootItemInstance` to `LootPickup.Initialize`.

`LootPickup` uses a trigger collider. When the player touches it, the component finds `PlayerInventory` in the player hierarchy and calls `PlayerInventory.AddItem`; successful pickup destroys the world object.

## UI lane contract

Render inventory from `PlayerInventory.Slots` and listen to `PlayerInventory.InventoryChanged`. Render equipped items from `PlayerInventory.GetEquipped(EquipmentSlot)` or `GetEquippedItems()` and listen to `EquipmentChanged`. Do not scrape scene loot pickups for inventory state.
