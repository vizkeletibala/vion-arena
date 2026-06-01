from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
UI_SCRIPTS = ROOT / "Assets" / "Game" / "Scripts" / "UI"
UI_PREFABS = ROOT / "Assets" / "Game" / "Prefabs" / "UI"
PLAYER_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Player" / "VitrialPlayer.prefab"
SCENE = ROOT / "Assets" / "Game" / "Scenes" / "PrototypeArena.unity"
LOOT_PICKUP = ROOT / "Assets" / "Game" / "Scripts" / "Loot" / "LootPickup.cs"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def assert_contains(path: Path, terms: list[str]) -> None:
    content = read(path)
    for term in terms:
        assert term in content, f"{path.name} should expose/use {term}"


def test_ui_runtime_scripts_bind_hud_pickup_prompt_and_inventory_to_gameplay_contracts():
    expected = {
        "HudView.cs": [
            "StarterRifleController",
            "AmmoChanged",
            "ReloadStarted",
            "ReloadCompleted",
            "Health",
            "Damaged",
            "Died",
            "CurrentMagazine",
            "ReserveAmmo",
            "CurrentHealth",
            "MaxHealth",
        ],
        "PickupPromptView.cs": [
            "LootPickup",
            "ItemInstance",
            "DisplayName",
            "OnTriggerEnter",
            "OnTriggerExit",
            "PlayerInventory",
            "SetPrompt",
        ],
        "InventoryComparisonView.cs": [
            "PlayerInventory",
            "InventoryChanged",
            "EquipmentChanged",
            "Slots",
            "GetEquipped",
            "TryEquip",
            "CompareScore",
            "InventoryPressed",
            "KeyCode.UpArrow",
            "KeyCode.DownArrow",
            "KeyCode.Return",
        ],
    }

    for filename, terms in expected.items():
        assert_contains(UI_SCRIPTS / filename, terms)


def test_ui_prefab_exposes_canvas_text_views_and_control_hints():
    prefab = read(UI_PREFABS / "GreyboxHud.prefab")

    for term in [
        "m_Name: GreyboxHud",
        "Canvas",
        "CanvasScaler",
        "GraphicRaycaster",
        "HudView",
        "PickupPromptView",
        "InventoryComparisonView",
        "healthText:",
        "ammoText:",
        "promptText:",
        "inventoryPanel:",
        "slotsText:",
        "comparisonText:",
        "Tab Inventory",
        "Up/Down Select",
        "Enter/E Equip",
    ]:
        assert term in prefab


def test_scene_instantiates_hud_and_loot_pickups_notify_prompt_view():
    scene = read(SCENE)
    pickup = read(LOOT_PICKUP)

    for term in ["GreyboxHud", "44e5522f71164d2c95e2dd17b3f81abc", "m_SourcePrefab"]:
        assert term in scene

    for term in ["PickupPromptView.Active", "Picked up", "SetPrompt", "ClearPrompt"]:
        assert term in pickup


def test_player_prefab_has_player_health_for_hud_binding():
    prefab = read(PLAYER_PREFAB)

    for term in [
        "Health",
        "m_EditorClassIdentifier: Vitrial.Game::Vitrial.Enemies.Health",
        "maxHealth: 100",
    ]:
        assert term in prefab


def test_ui_readme_documents_runtime_data_sources_and_assumptions():
    readme = read(UI_SCRIPTS / "README.md")

    for term in [
        "StarterRifleController.AmmoChanged",
        "Health.CurrentHealth",
        "LootPickup.ItemInstance",
        "PlayerInventory.Slots",
        "PlayerInventory.TryEquip",
        "Assumption",
        "Tab",
        "Enter/E",
    ]:
        assert term in readme
