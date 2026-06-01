using Vitrial.Loot;

namespace Vitrial.Inventory
{
    public readonly struct InventorySlot
    {
        public InventorySlot(LootItemDefinition item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public LootItemDefinition Item { get; }
        public int Quantity { get; }
        public bool IsEmpty => Item == null || Quantity <= 0;
    }
}
