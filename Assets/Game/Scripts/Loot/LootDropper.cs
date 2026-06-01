using Vitrial.Enemies;
using UnityEngine;

namespace Vitrial.Loot
{
    [RequireComponent(typeof(EnemyDeathHook))]
    public sealed class LootDropper : MonoBehaviour
    {
        [SerializeField] private LootTableDefinition lootTable;
        [SerializeField] private LootPickup pickupPrefab;
        [SerializeField] private Transform dropOrigin;
        [SerializeField] private Vector3 dropOffset = new Vector3(0f, 0.25f, 0f);

        private EnemyDeathHook deathHook;
        private readonly System.Random random = new System.Random();

        public LootTableDefinition LootTable => lootTable;
        public LootPickup PickupPrefab => pickupPrefab;

        private void Awake()
        {
            deathHook = GetComponent<EnemyDeathHook>();
        }

        private void OnEnable()
        {
            if (deathHook != null)
            {
                deathHook.OnDeath.AddListener(SpawnDrop);
            }
        }

        private void OnDisable()
        {
            if (deathHook != null)
            {
                deathHook.OnDeath.RemoveListener(SpawnDrop);
            }
        }

        public bool TryGenerateDrop(out LootItemInstance itemInstance)
        {
            itemInstance = null;
            return lootTable != null && lootTable.TryRollDrop(random, out itemInstance);
        }

        public void SpawnDrop()
        {
            if (pickupPrefab == null || !TryGenerateDrop(out LootItemInstance itemInstance))
            {
                return;
            }

            Transform origin = dropOrigin != null ? dropOrigin : transform;
            LootPickup pickup = Instantiate(pickupPrefab, origin.position + dropOffset, Quaternion.identity);
            pickup.Initialize(itemInstance);
        }
    }
}
