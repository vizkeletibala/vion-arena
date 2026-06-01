from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PLAYER_SCRIPTS = ROOT / "Assets" / "Game" / "Scripts" / "Player"
PLAYER_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Player" / "VitrialPlayer.prefab"
PROTOTYPE_SCENE = ROOT / "Assets" / "Game" / "Scenes" / "PrototypeArena.unity"
PLAYER_README = PLAYER_SCRIPTS / "README.md"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def test_player_controller_scripts_define_separate_movement_input_and_camera_components():
    expected = {
        "PlayerInputReader.cs": ["Move", "Look", "JumpPressed", "SprintHeld"],
        "PlayerMotor.cs": ["CharacterController", "WalkSpeed", "SprintSpeed", "JumpHeight", "Gravity"],
        "PlayerLookController.cs": ["Yaw", "Pitch", "MouseSensitivity", "CursorLockMode.Locked"],
        "PlayerRigAnchor.cs": ["CameraPivot", "WeaponSocket", "InputReader", "Motor", "LookController"],
    }

    for filename, required_terms in expected.items():
        content = read(PLAYER_SCRIPTS / filename)
        for term in required_terms:
            assert term in content, f"{filename} should expose/use {term}"


def test_player_prefab_and_prototype_scene_hook_up_greybox_controller():
    prefab = read(PLAYER_PREFAB)
    scene = read(PROTOTYPE_SCENE)

    assert "m_Name: VitrialPlayer" in prefab
    assert "CharacterController" in prefab or "m_Script" in prefab
    assert "CameraPivot" in prefab
    assert "WeaponSocket" in prefab
    assert "m_Name: PrototypeArena" in scene
    assert "VitrialPlayer" in scene
    assert "Directional Light" in scene
    assert "Greybox Floor" in scene


def test_player_readme_documents_windows_controls_and_downstream_input_contract():
    readme = read(PLAYER_README)
    required = [
        "WASD",
        "Mouse",
        "Left Shift",
        "Space",
        "fire: Left Mouse Button",
        "reload: R",
        "interact: E",
        "inventory: Tab",
    ]

    for phrase in required:
        assert phrase in readme
