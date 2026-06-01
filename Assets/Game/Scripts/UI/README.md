# Assets/Game/Scripts/UI

Greybox UI for Vitrial Milestone 1. The implementation intentionally stays lightweight UGUI + MonoBehaviour so Windows playtesters can read state without a polished art pass.

## Runtime data consumed

- `HudView` reads `StarterRifleController.AmmoChanged`, `StarterRifleController.ReloadStarted`, `StarterRifleController.ReloadCompleted`, and `WeaponAmmoState.CurrentMagazine/ReserveAmmo` for ammo display.
- `HudView` reads `Health.CurrentHealth` / `Health.MaxHealth` and refreshes on `Health.Damaged` / `Health.Died` for the health readout. The player prefab now carries the same generic `Health` component used by combat targets.
- `PickupPromptView` displays `LootPickup.ItemInstance.DisplayName`; `LootPickup` notifies the active prompt view when the player inventory enters/exits the trigger.
- `InventoryComparisonView` reads `PlayerInventory.Slots`, listens to `PlayerInventory.InventoryChanged` and `PlayerInventory.EquipmentChanged`, compares selected loot against `PlayerInventory.GetEquipped(slot)`, and equips via `PlayerInventory.TryEquip`.

## Controls

- `Tab` toggles the inventory/comparison panel (also consumes `PlayerInputReader.InventoryPressed`).
- `Up/Down` selects collected loot.
- `Enter/E` equips the selected item when it is equipable.

## Assumption / review notes

- Assumption: player health can use the existing generic `Vitrial.Enemies.Health` component until a dedicated player damage model lands.
- Assumption: pickup remains auto-collect-on-touch for milestone 1; the prompt confirms what was collected rather than gating collection behind Interact.
- `GreyboxHud.prefab` has fallback text creation in code, so missing hand-wired Text references still produce readable runtime UI after Unity imports the prefab.
