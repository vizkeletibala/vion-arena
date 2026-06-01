using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vitrial.Loot
{
    [Serializable]
    public sealed class LootDropEntry
    {
        [SerializeField] private LootItemDefinition item;
        [SerializeField, Min(0f)] private float weight = 1f;
        [SerializeField, Min(1)] private int itemLevel = 1;

        public LootItemDefinition Item => item;
        public float Weight => weight;
        public int ItemLevel => itemLevel;
    }

    [CreateAssetMenu(menuName = "Vitrial/Loot/Loot Table", fileName = "LootTableDefinition")]
    public sealed class LootTableDefinition : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float dropChance = 0.65f;
        [SerializeField] private LootDropEntry[] entries = Array.Empty<LootDropEntry>();

        public float DropChance => dropChance;
        public IReadOnlyList<LootDropEntry> Entries => entries;

        public bool TryRollDrop(System.Random random, out LootItemInstance itemInstance)
        {
            if (random == null)
            {
                random = new System.Random();
            }

            itemInstance = null;

            if (entries == null || entries.Length == 0 || random.NextDouble() > dropChance)
            {
                return false;
            }

            float totalWeight = 0f;
            foreach (LootDropEntry entry in entries)
            {
                if (entry?.Item != null)
                {
                    totalWeight += Mathf.Max(0f, entry.Weight);
                }
            }

            if (totalWeight <= 0f)
            {
                return false;
            }

            double pick = random.NextDouble() * totalWeight;
            float cursor = 0f;
            foreach (LootDropEntry entry in entries)
            {
                if (entry?.Item == null)
                {
                    continue;
                }

                cursor += Mathf.Max(0f, entry.Weight);
                if (pick <= cursor)
                {
                    itemInstance = entry.Item.RollInstance(random, entry.ItemLevel);
                    return true;
                }
            }

            return false;
        }
    }
}
