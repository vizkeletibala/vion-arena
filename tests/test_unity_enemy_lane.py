from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ENEMY_SCRIPTS = ROOT / "Assets" / "Game" / "Scripts" / "Enemies"
ENEMY_PREFAB = ROOT / "Assets" / "Game" / "Prefabs" / "Enemies" / "GreyboxChaser.prefab"
ENEMY_DEFINITION = ROOT / "Assets" / "Game" / "ScriptableObjects" / "Enemies" / "GreyboxChaser.asset"
PROTOTYPE_SCENE = ROOT / "Assets" / "Game" / "Scenes" / "PrototypeArena.unity"
ENEMY_README = ENEMY_SCRIPTS / "README.md"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def test_enemy_runtime_scripts_define_chase_damage_death_and_spawn_hooks():
    expected = {
        "EnemyChaseController.cs": [
            "CharacterController",
            "TargetTag",
            "MoveSpeed",
            "DetectRange",
            "AttackRange",
            "IDamageable",
            "DamagePayload",
        ],
        "EnemyDeathHook.cs": ["UnityEvent", "Health.Died", "OnDeath", "Loot"],
        "EnemySpawnPoint.cs": ["enemyPrefab", "spawnOnStart", "Spawn", "spawnedEnemy"],
        "Health.cs": ["SetMaxHealth", "ResetHealth", "Died", "Damaged", "IDamageable"],
        "EnemyDefinition.cs": ["detectRange", "attackRange", "attackCooldown", "prefab"],
    }

    for filename, required_terms in expected.items():
        content = read(ENEMY_SCRIPTS / filename)
        for term in required_terms:
            assert term in content, f"{filename} should expose/use {term}"


def test_enemy_prefab_and_definition_wire_greybox_chaser_components():
    prefab = read(ENEMY_PREFAB)
    definition = read(ENEMY_DEFINITION)

    for phrase in [
        "m_Name: GreyboxChaser",
        "CharacterController",
        "EnemyChaseController",
        "Health",
        "EnemyDeathHook",
        "targetTag: Player",
        "contactDamage",
    ]:
        assert phrase in prefab

    for phrase in [
        "displayName: Greybox Chaser",
        "maxHealth: 50",
        "moveSpeed: 3.25",
        "detectRange: 18",
        "attackRange: 1.35",
        "attackCooldown: 0.75",
    ]:
        assert phrase in definition


def test_prototype_scene_contains_enemy_spawn_point_for_final_integration():
    scene = read(PROTOTYPE_SCENE)

    for phrase in [
        "m_Name: EnemySpawnPoint",
        "EnemySpawnPoint",
        "GreyboxChaser",
        "m_LocalPosition: {x: 4, y: 0.05, z: 4}",
    ]:
        assert phrase in scene


def test_enemy_readme_documents_damage_death_spawn_and_loot_contracts():
    readme = read(ENEMY_README)
    required = [
        "GreyboxChaser",
        "IDamageable.ApplyDamage",
        "Health.Died",
        "EnemyDeathHook.OnDeath",
        "EnemySpawnPoint.Spawn",
        "loot drops",
        "remaining scene hookup",
    ]

    for phrase in required:
        assert phrase in readme
