using System;
using Vitrial.Inventory;
using UnityEngine;

namespace Vitrial.Loot
{
    [CreateAssetMenu(menuName = "Vitrial/Loot/Loot Item", fileName = "LootItemDefinition")]
    public sealed class LootItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName = "Prototype Drop";
        [SerializeField] private LootItemType itemType = LootItemType.Weapon;
        [SerializeField] private EquipmentSlot equipmentSlot = EquipmentSlot.Weapon;
        [SerializeField] private Sprite icon;

        [Header("Rolls")]
        [SerializeField] private LootRarity minimumRarity = LootRarity.Common;
        [SerializeField] private LootRarity maximumRarity = LootRarity.Rare;
        [SerializeField] private ItemStatBlock baseStats = new ItemStatBlock(12f, 420f, 24, 0f, 0f);
        [SerializeField] private ItemStatBlock randomStatBonus = new ItemStatBlock(6f, 90f, 8, 15f, 0.4f);

        public string DisplayName => displayName;
        public LootItemType ItemType => itemType;
        public EquipmentSlot EquipmentSlot => equipmentSlot;
        public Sprite Icon => icon;
        public LootRarity Rarity => minimumRarity;
        public LootRarity MinimumRarity => minimumRarity;
        public LootRarity MaximumRarity => maximumRarity;
        public ItemStatBlock BaseStats => baseStats;
        public ItemStatBlock RandomStatBonus => randomStatBonus;
        public bool CanEquip => itemType == LootItemType.Weapon || itemType == LootItemType.Armor || itemType == LootItemType.Trinket;

        public LootItemInstance RollInstance(System.Random random, int itemLevel = 1)
        {
            if (random == null)
            {
                random = new System.Random();
            }

            float rollPercent = (float)random.NextDouble();
            LootRarity rolledRarity = RollRarity(random);
            float rarityMultiplier = GetRarityMultiplier(rolledRarity);
            float itemLevelMultiplier = Mathf.Max(1, itemLevel) / 1f;
            ItemStatBlock rolledStats = (baseStats + (randomStatBonus * rollPercent)) * rarityMultiplier * itemLevelMultiplier;
            string instanceId = $"{name}-{Guid.NewGuid():N}";
            return new LootItemInstance(this, rolledRarity, rolledStats, rollPercent, instanceId);
        }

        private LootRarity RollRarity(System.Random random)
        {
            int min = (int)minimumRarity;
            int max = Mathf.Max(min, (int)maximumRarity);
            int span = max - min + 1;
            double value = random.NextDouble();
            int offset = 0;

            if (value >= 0.98 && span >= 5)
            {
                offset = 4;
            }
            else if (value >= 0.9 && span >= 4)
            {
                offset = 3;
            }
            else if (value >= 0.72 && span >= 3)
            {
                offset = 2;
            }
            else if (value >= 0.42 && span >= 2)
            {
                offset = 1;
            }

            return (LootRarity)Mathf.Clamp(min + offset, min, max);
        }

        public static float GetRarityMultiplier(LootRarity rarity)
        {
            switch (rarity)
            {
                case LootRarity.Uncommon:
                    return 1.15f;
                case LootRarity.Rare:
                    return 1.35f;
                case LootRarity.Epic:
                    return 1.65f;
                case LootRarity.Legendary:
                    return 2f;
                default:
                    return 1f;
            }
        }
    }
}
