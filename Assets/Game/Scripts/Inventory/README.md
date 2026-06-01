# Inventory and equip runtime contract

`PlayerInventory` is the single runtime owner for picked up loot during a run. It keeps an in-memory list of `InventorySlot` values and a dictionary of equipped `LootItemInstance` values keyed by `EquipmentSlot`.

## Public surface for UI

- `IReadOnlyList<InventorySlot> PlayerInventory.Slots`
- `event InventoryChanged(IReadOnlyList<InventorySlot> slots)`
- `bool AddItem(LootItemInstance itemInstance)`
- `bool TryEquip(LootItemInstance itemInstance)`
- `LootItemInstance GetEquipped(EquipmentSlot slot)`
- `IReadOnlyDictionary<EquipmentSlot, LootItemInstance> GetEquippedItems()`
- `event EquipmentChanged(EquipmentSlot slot, LootItemInstance itemInstance)`

`InventorySlot.Item` is a `LootItemInstance`, not the authoring `LootItemDefinition`; compare UI should use `Item.DisplayName`, `Item.Rarity`, `Item.RolledStats`, and `Item.CompareScore`.

## Run persistence

Equip state is persisted for the current match by the `PlayerInventory` component on the player prefab. This milestone does not write save files; later save/account lanes can serialize `LootItemInstance.InstanceId`, `Definition`, `Rarity`, `RollPercent`, and `RolledStats` if needed.
