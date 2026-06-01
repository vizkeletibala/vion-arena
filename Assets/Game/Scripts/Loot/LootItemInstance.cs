using System;
using Vitrial.Inventory;

namespace Vitrial.Loot
{
    [Serializable]
    public sealed class LootItemInstance
    {
        public LootItemInstance(LootItemDefinition definition, LootRarity rarity, ItemStatBlock rolledStats, float rollPercent, string instanceId)
        {
            Definition = definition;
            Rarity = rarity;
            RolledStats = rolledStats;
            RollPercent = rollPercent;
            InstanceId = instanceId;
        }

        public LootItemDefinition Definition { get; }
        public LootRarity Rarity { get; }
        public ItemStatBlock RolledStats { get; }
        public float RollPercent { get; }
        public string InstanceId { get; }
        public string DisplayName => Definition == null ? "Unknown Item" : $"{Rarity} {Definition.DisplayName}";
        public LootItemType ItemType => Definition == null ? LootItemType.Consumable : Definition.ItemType;
        public EquipmentSlot EquipmentSlot => Definition == null ? EquipmentSlot.Weapon : Definition.EquipmentSlot;
        public bool CanEquip => Definition != null && Definition.CanEquip;

        public float CompareScore => RolledStats.Damage + (RolledStats.FireRate * 0.1f) + RolledStats.MagazineSize + RolledStats.MaxHealth + RolledStats.MoveSpeed;
    }
}
