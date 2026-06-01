from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SCENE = ROOT / "Assets" / "Game" / "Scenes" / "PrototypeArena.unity"
README = ROOT / "README.md"
GAMESTATE = ROOT / "Assets" / "Game" / "Scripts" / "GameState"
PLAYER_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Player" / "VitrialPlayer.prefab"
ENEMY_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Enemies" / "GreyboxChaser.prefab"
HUD_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "UI" / "GreyboxHud.prefab"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def assert_contains(path: Path, terms: list[str]) -> None:
    content = read(path)
    for term in terms:
        assert term in content, f"{path.name} should contain {term}"


def test_prototype_arena_scene_wires_milestone1_playable_slice():
    scene = read(SCENE)

    for term in [
        "m_Name: PrototypeArena",
        "VitrialPlayer",
        "e5f5522f71164d2c95e2dd17b3f81abc",
        "EnemySpawnPoint",
        "GreyboxChaser",
        "16e5522f71164d2c95e2dd17b3f81abc",
        "GreyboxHud",
        "44e5522f71164d2c95e2dd17b3f81abc",
        "MatchLoopController",
        "66e5522f71164d2c95e2dd17b3f81abc",
        "manualRestartKey: 8",
    ]:
        assert term in scene


def test_match_loop_controller_restarts_on_player_death_and_manual_key():
    assert_contains(
        GAMESTATE / "MatchLoopController.cs",
        [
            "using UnityEngine.SceneManagement;",
            "GameObject.FindGameObjectWithTag(\"Player\")",
            "GetComponentInChildren<Health>()",
            "playerHealth.Died += HandlePlayerDied",
            "Invoke(nameof(RestartScene), restartDelaySeconds)",
            "Input.GetKeyDown(manualRestartKey)",
            "SceneManager.LoadScene",
            "KeyCode.Backspace",
        ],
    )


def test_prefabs_expose_cross_lane_contracts_for_integrated_loop():
    assert_contains(
        PLAYER_PREFAB,
        [
            "m_TagString: Player",
            "PlayerInputReader",
            "PlayerMotor",
            "PlayerLookController",
            "StarterRifleController",
            "PlayerInventory",
            "Health",
            "weaponDefinition:",
        ],
    )
    assert_contains(
        ENEMY_PREFAB,
        [
            "EnemyChaseController",
            "EnemyDeathHook",
            "LootDropper",
            "lootTable:",
        ],
    )
    assert_contains(
        HUD_PREFAB,
        [
            "HudView",
            "PickupPromptView",
            "InventoryComparisonView",
        ],
    )


def test_readme_documents_windows_playtest_and_repo_coexistence():
    readme = read(README)

    for term in [
        "Windows Milestone 1 playtest",
        "Unity Editor `2022.3.55f1`",
        "Assets/Game/Scenes/PrototypeArena.unity",
        "WASD moves",
        "Left Mouse fires",
        "GreyboxChaser spawns",
        "dies, and drops loot",
        "Tab opens inventory comparison",
        "Backspace manually restarts",
        "frontend/` or `backend/`",
        "pre-existing browser-game stack",
        "Windows-only validation is intentionally left to Andrew",
        "sh scripts/validate-vitrial-headless.sh",
    ]:
        assert term in readme


def test_gamestate_readme_names_current_loop_and_future_work_boundary():
    assert_contains(
        GAMESTATE / "README.md",
        [
            "MatchLoopController",
            "Health.Died",
            "Backspace",
            "Future work",
            "server-authoritative",
        ],
    )
