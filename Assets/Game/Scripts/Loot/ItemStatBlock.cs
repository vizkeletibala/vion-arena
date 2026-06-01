using System;
using UnityEngine;

namespace Vitrial.Loot
{
    [Serializable]
    public struct ItemStatBlock
    {
        [SerializeField] private float damage;
        [SerializeField] private float fireRate;
        [SerializeField] private int magazineSize;
        [SerializeField] private float maxHealth;
        [SerializeField] private float moveSpeed;

        public ItemStatBlock(float damage, float fireRate, int magazineSize, float maxHealth, float moveSpeed)
        {
            this.damage = damage;
            this.fireRate = fireRate;
            this.magazineSize = magazineSize;
            this.maxHealth = maxHealth;
            this.moveSpeed = moveSpeed;
        }

        public float Damage => damage;
        public float FireRate => fireRate;
        public int MagazineSize => magazineSize;
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;

        public static ItemStatBlock operator +(ItemStatBlock left, ItemStatBlock right)
        {
            return new ItemStatBlock(
                left.damage + right.damage,
                left.fireRate + right.fireRate,
                left.magazineSize + right.magazineSize,
                left.maxHealth + right.maxHealth,
                left.moveSpeed + right.moveSpeed);
        }

        public static ItemStatBlock operator *(ItemStatBlock block, float multiplier)
        {
            return new ItemStatBlock(
                block.damage * multiplier,
                block.fireRate * multiplier,
                Mathf.RoundToInt(block.magazineSize * multiplier),
                block.maxHealth * multiplier,
                block.moveSpeed * multiplier);
        }
    }
}
