using System;
using System.Collections.Generic;
using Vitrial.Loot;
using UnityEngine;

namespace Vitrial.Inventory
{
    public sealed class PlayerInventory : MonoBehaviour
    {
        [SerializeField, Min(1)] private int capacity = 24;
        [SerializeField] private bool equipFirstWeaponOnPickup = true;

        private readonly List<InventorySlot> slots = new List<InventorySlot>();
        private readonly Dictionary<EquipmentSlot, LootItemInstance> equippedItems = new Dictionary<EquipmentSlot, LootItemInstance>();

        public event Action<IReadOnlyList<InventorySlot>> InventoryChanged;
        public event Action<EquipmentSlot, LootItemInstance> EquipmentChanged;

        public int Capacity => capacity;
        public IReadOnlyList<InventorySlot> Slots => slots;

        public bool AddItem(LootItemInstance itemInstance)
        {
            if (itemInstance == null || slots.Count >= capacity)
            {
                return false;
            }

            slots.Add(new InventorySlot(itemInstance, 1));
            InventoryChanged?.Invoke(slots);

            if (equipFirstWeaponOnPickup && itemInstance.CanEquip && !equippedItems.ContainsKey(itemInstance.EquipmentSlot))
            {
                TryEquip(itemInstance);
            }

            return true;
        }

        public bool TryEquip(LootItemInstance itemInstance)
        {
            if (itemInstance == null || !itemInstance.CanEquip || !Contains(itemInstance))
            {
                return false;
            }

            equippedItems[itemInstance.EquipmentSlot] = itemInstance;
            EquipmentChanged?.Invoke(itemInstance.EquipmentSlot, itemInstance);
            return true;
        }

        public LootItemInstance GetEquipped(EquipmentSlot slot)
        {
            equippedItems.TryGetValue(slot, out LootItemInstance itemInstance);
            return itemInstance;
        }

        public IReadOnlyDictionary<EquipmentSlot, LootItemInstance> GetEquippedItems()
        {
            return equippedItems;
        }

        private bool Contains(LootItemInstance itemInstance)
        {
            foreach (InventorySlot slot in slots)
            {
                if (ReferenceEquals(slot.Item, itemInstance) || slot.Item?.InstanceId == itemInstance.InstanceId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
