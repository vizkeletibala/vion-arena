using Vitrial.Loot;

namespace Vitrial.Inventory
{
    public readonly struct InventorySlot
    {
        public InventorySlot(LootItemInstance item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public LootItemInstance Item { get; }
        public int Quantity { get; }
        public bool IsEmpty => Item == null || Quantity <= 0;
    }
}
