using UnityEngine;

namespace Vitrial.Enemies
{
    [CreateAssetMenu(menuName = "Vitrial/Enemies/Enemy Definition", fileName = "EnemyDefinition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Greybox Drone";
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float contactDamage = 10f;

        public string DisplayName => displayName;
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float ContactDamage => contactDamage;
    }
}
