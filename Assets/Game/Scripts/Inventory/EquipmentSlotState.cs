using System;
using Vitrial.Loot;

namespace Vitrial.Inventory
{
    [Serializable]
    public sealed class EquipmentSlotState
    {
        public EquipmentSlotState(EquipmentSlot slot)
        {
            Slot = slot;
        }

        public EquipmentSlot Slot { get; }
        public LootItemInstance EquippedItem { get; private set; }
        public bool IsEmpty => EquippedItem == null;

        public void Equip(LootItemInstance item)
        {
            EquippedItem = item;
        }
    }
}
