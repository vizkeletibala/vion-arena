using UnityEngine;

namespace Vitrial.Enemies
{
    [CreateAssetMenu(menuName = "Vitrial/Enemies/Enemy Definition", fileName = "EnemyDefinition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Greybox Chaser";
        [SerializeField] private GameObject prefab;
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float moveSpeed = 3.25f;
        [SerializeField] private float detectRange = 18f;
        [SerializeField] private float attackRange = 1.35f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private float attackCooldown = 0.75f;

        public string DisplayName => displayName;
        public GameObject Prefab => prefab;
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float DetectRange => detectRange;
        public float AttackRange => attackRange;
        public float ContactDamage => contactDamage;
        public float AttackCooldown => attackCooldown;
    }
}
