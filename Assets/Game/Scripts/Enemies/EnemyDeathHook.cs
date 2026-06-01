using UnityEngine;
using UnityEngine.Events;

namespace Vitrial.Enemies
{
    [RequireComponent(typeof(Health))]
    public sealed class EnemyDeathHook : MonoBehaviour
    {
        [SerializeField] private UnityEvent onDeath = new UnityEvent();
        [SerializeField] private bool destroyOnDeath = true;
        [SerializeField] private float destroyDelay = 0.15f;

        private Health health;
        private bool invoked;

        public UnityEvent OnDeath => onDeath;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.Died += HandleDied; // Health.Died is the death signal this hook adapts for loot/VFX.
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.Died -= HandleDied;
            }
        }

        private void HandleDied()
        {
            if (invoked)
            {
                return;
            }

            invoked = true;
            onDeath.Invoke(); // Loot drops can subscribe to EnemyDeathHook.OnDeath without coupling to AI.

            if (destroyOnDeath)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
