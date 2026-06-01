using System;
using UnityEngine;
using Vitrial.Weapons;

namespace Vitrial.Enemies
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 50f;

        private float currentHealth;

        public event Action<DamagePayload> Damaged;
        public event Action Died;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => currentHealth <= 0f;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void ApplyDamage(DamagePayload payload)
        {
            if (IsDead)
            {
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - payload.Amount);
            Damaged?.Invoke(payload);

            if (IsDead)
            {
                Died?.Invoke();
            }
        }
    }
}
