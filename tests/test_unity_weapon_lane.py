from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
WEAPON_SCRIPTS = ROOT / "Assets" / "Game" / "Scripts" / "Weapons"
PLAYER_SCRIPTS = ROOT / "Assets" / "Game" / "Scripts" / "Player"
PLAYER_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Player" / "VitrialPlayer.prefab"
WEAPON_ASSETS = ROOT / "Assets" / "Game" / "ScriptableObjects" / "Weapons"
WEAPON_PREFABS = ROOT / "Assets" / "Game" / "Prefabs" / "Weapons"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def assert_contains(path: Path, terms: list[str]) -> None:
    content = read(path)
    for term in terms:
        assert term in content, f"{path.name} should expose/use {term}"


def test_weapon_runtime_scripts_define_data_ammo_reload_and_damage_contracts():
    expected = {
        "WeaponDefinition.cs": [
            "CreateAssetMenu",
            "MagazineSize",
            "ReserveAmmo",
            "Damage",
            "ReloadSeconds",
            "RoundsPerMinute",
            "Range",
        ],
        "WeaponAmmoState.cs": [
            "CurrentMagazine",
            "ReserveAmmo",
            "CanFire",
            "ConsumeRound",
            "ReloadFromReserve",
        ],
        "StarterRifleController.cs": [
            "PlayerInputReader",
            "FirePressed",
            "ReloadPressed",
            "Physics.Raycast",
            "IDamageable",
            "DamagePayload",
            "AmmoChanged",
            "ReloadStarted",
            "ReloadCompleted",
            "TimeBetweenShots",
        ],
        "WeaponHitResult.cs": ["DidHit", "Damageable", "Point", "Normal"],
    }

    for filename, required_terms in expected.items():
        assert_contains(WEAPON_SCRIPTS / filename, required_terms)


def test_starter_rifle_asset_and_prefab_expose_tunable_combat_values():
    asset = read(WEAPON_ASSETS / "StarterRifle.asset")
    prefab = read(WEAPON_PREFABS / "StarterRifle.prefab")

    for term in [
        "m_Name: StarterRifle",
        "displayName: Starter Rifle",
        "damage:",
        "roundsPerMinute:",
        "magazineSize:",
        "reserveAmmo:",
        "reloadSeconds:",
        "range:",
    ]:
        assert term in asset

    for term in [
        "m_Name: StarterRifle",
        "StarterRifleController",
        "weaponDefinition:",
        "m_EditorClassIdentifier: Vitrial.Game::Vitrial.Weapons.StarterRifleController",
    ]:
        assert term in prefab


def test_player_prefab_mounts_starter_rifle_on_weapon_socket_and_documents_hooks():
    prefab = read(PLAYER_PREFAB)
    weapons_readme = read(WEAPON_SCRIPTS / "README.md")
    player_readme = read(PLAYER_SCRIPTS / "README.md")

    for term in ["StarterRifle", "StarterRifleController", "WeaponSocket", "PlayerInputReader"]:
        assert term in prefab

    for term in [
        "AmmoChanged",
        "ReloadStarted",
        "ReloadCompleted",
        "IDamageable.ApplyDamage",
        "WeaponDefinition",
        "StarterRifle.asset",
    ]:
        assert term in weapons_readme

    for term in ["Starter Rifle", "Left Mouse Button", "reload: R"]:
        assert term in player_readme
