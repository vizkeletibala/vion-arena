using UnityEngine;
using Vitrial.Weapons;

namespace Vitrial.Enemies
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Health))]
    public sealed class EnemyChaseController : MonoBehaviour
    {
        [SerializeField] private EnemyDefinition definition;
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private float moveSpeed = 3.25f;
        [SerializeField] private float detectRange = 18f;
        [SerializeField] private float attackRange = 1.35f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private float attackCooldown = 0.75f;
        [SerializeField] private float gravity = -24f;

        private CharacterController characterController;
        private Health health;
        private Transform target;
        private float verticalVelocity;
        private float nextAttackTime;

        public string TargetTag => targetTag;
        public float MoveSpeed => moveSpeed;
        public float DetectRange => detectRange;
        public float AttackRange => attackRange;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            health = GetComponent<Health>();
            ApplyDefinition();
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.Died += HandleDeath;
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.Died -= HandleDeath;
            }
        }

        private void Update()
        {
            if (health != null && health.IsDead)
            {
                return;
            }

            AcquireTargetIfNeeded();
            ApplyGravity();

            if (target == null)
            {
                characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
                return;
            }

            Vector3 offset = target.position - transform.position;
            offset.y = 0f;
            float distance = offset.magnitude;

            if (distance > detectRange)
            {
                characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
                return;
            }

            if (distance > attackRange)
            {
                Vector3 direction = offset.normalized;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                Vector3 velocity = direction * moveSpeed;
                velocity.y = verticalVelocity;
                characterController.Move(velocity * Time.deltaTime);
            }
            else
            {
                TryContactDamage();
            }
        }

        public void ApplyDefinition()
        {
            if (definition == null)
            {
                return;
            }

            moveSpeed = definition.MoveSpeed;
            detectRange = definition.DetectRange;
            attackRange = definition.AttackRange;
            contactDamage = definition.ContactDamage;
            attackCooldown = definition.AttackCooldown;

            Health healthComponent = GetComponent<Health>();
            if (healthComponent != null)
            {
                healthComponent.SetMaxHealth(definition.MaxHealth);
            }
        }

        private void AcquireTargetIfNeeded()
        {
            if (target != null)
            {
                return;
            }

            GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
            target = targetObject != null ? targetObject.transform : null;
        }

        private void ApplyGravity()
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;
        }

        private void TryContactDamage()
        {
            if (Time.time < nextAttackTime || target == null)
            {
                return;
            }

            nextAttackTime = Time.time + attackCooldown;
            IDamageable damageable = target.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                return;
            }

            Vector3 hitPoint = target.position;
            Vector3 normal = (transform.position - target.position).normalized;
            damageable.ApplyDamage(new DamagePayload(contactDamage, hitPoint, normal, gameObject));
        }

        private void HandleDeath()
        {
            enabled = false;
            characterController.enabled = false;
        }
    }
}
