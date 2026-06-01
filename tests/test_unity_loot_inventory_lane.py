from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
LOOT_SCRIPTS = ROOT / "Assets" / "Game" / "Scripts" / "Loot"
INVENTORY_SCRIPTS = ROOT / "Assets" / "Game" / "Scripts" / "Inventory"
ENEMY_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Enemies" / "GreyboxChaser.prefab"
PLAYER_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Player" / "VitrialPlayer.prefab"
LOOT_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Loot" / "LootPickup.prefab"
LOOT_ASSETS = ROOT / "Assets" / "Game" / "ScriptableObjects" / "Loot"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def assert_contains(path: Path, terms: list[str]) -> None:
    content = read(path)
    for term in terms:
        assert term in content, f"{path.name} should expose/use {term}"


def test_loot_runtime_scripts_define_rolls_dropper_pickup_and_instances():
    expected = {
        "LootItemDefinition.cs": [
            "CreateAssetMenu",
            "LootRarity",
            "ItemStatBlock",
            "RollInstance",
            "GetRarityMultiplier",
            "minimumRarity",
            "maximumRarity",
        ],
        "LootItemInstance.cs": ["InstanceId", "RolledStats", "RollPercent", "CompareScore", "EquipmentSlot"],
        "LootTableDefinition.cs": ["dropChance", "LootDropEntry", "TryRollDrop", "Weight", "itemLevel"],
        "LootDropper.cs": ["EnemyDeathHook", "OnDeath.AddListener", "TryGenerateDrop", "SpawnDrop", "LootPickup"],
        "LootPickup.cs": ["OnTriggerEnter", "PlayerInventory", "AddItem", "Initialize", "isTrigger"],
        "ItemStatBlock.cs": ["Damage", "FireRate", "MagazineSize", "MaxHealth", "MoveSpeed"],
    }

    for filename, required_terms in expected.items():
        assert_contains(LOOT_SCRIPTS / filename, required_terms)


def test_inventory_runtime_scripts_define_ui_contract_and_equip_state():
    expected = {
        "PlayerInventory.cs": [
            "IReadOnlyList<InventorySlot>",
            "InventoryChanged",
            "EquipmentChanged",
            "AddItem",
            "TryEquip",
            "GetEquipped",
            "IReadOnlyDictionary<EquipmentSlot, LootItemInstance>",
        ],
        "InventorySlot.cs": ["LootItemInstance", "Quantity", "IsEmpty"],
        "EquipmentSlot.cs": ["Weapon", "Armor", "Trinket"],
    }

    for filename, required_terms in expected.items():
        assert_contains(INVENTORY_SCRIPTS / filename, required_terms)


def test_prefabs_and_assets_wire_enemy_death_to_pickup_and_player_inventory():
    enemy_prefab = read(ENEMY_PREFAB)
    player_prefab = read(PLAYER_PREFAB)
    loot_prefab = read(LOOT_PREFAB)
    item_asset = read(LOOT_ASSETS / "RustyBurstRifle.asset")
    table_asset = read(LOOT_ASSETS / "GreyboxChaserLootTable.asset")

    for term in ["LootDropper", "lootTable:", "pickupPrefab:", "32e5522f71164d2c95e2dd17b3f81abc", "33e5522f71164d2c95e2dd17b3f81abc"]:
        assert term in enemy_prefab

    for term in ["PlayerInventory", "capacity: 24", "equipFirstWeaponOnPickup: 1"]:
        assert term in player_prefab

    for term in ["m_Name: LootPickup", "BoxCollider", "m_IsTrigger: 1", "LootPickup", "fallbackDefinition"]:
        assert term in loot_prefab

    for term in ["m_Name: RustyBurstRifle", "displayName: Rusty Burst Rifle", "baseStats:", "randomStatBonus:", "maximumRarity: 2"]:
        assert term in item_asset

    for term in ["m_Name: GreyboxChaserLootTable", "dropChance: 0.75", "entries:", "31e5522f71164d2c95e2dd17b3f81abc"]:
        assert term in table_asset


def test_readmes_document_designer_and_ui_contracts():
    loot_readme = read(LOOT_SCRIPTS / "README.md")
    inventory_readme = read(INVENTORY_SCRIPTS / "README.md")
    assets_readme = read(LOOT_ASSETS / "README.md")

    for term in ["LootItemInstance", "ItemStatBlock", "LootDropper", "EnemyDeathHook.OnDeath", "PlayerInventory.Slots"]:
        assert term in loot_readme

    for term in ["TryEquip", "GetEquipped", "InventoryChanged", "EquipmentChanged", "Run persistence"]:
        assert term in inventory_readme

    for term in ["RustyBurstRifle.asset", "GreyboxChaserLootTable.asset", "Vitrial/Loot/Loot Item"]:
        assert term in assets_readme
