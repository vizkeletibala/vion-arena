using Vitrial.Inventory;
using Vitrial.UI;
using UnityEngine;

namespace Vitrial.Loot
{
    [RequireComponent(typeof(Collider))]
    public sealed class LootPickup : MonoBehaviour
    {
        [SerializeField] private LootItemDefinition fallbackDefinition;
        [SerializeField] private bool autoPickupOnPlayerTouch = true;

        private LootItemInstance itemInstance;

        public LootItemInstance ItemInstance => itemInstance;

        public void Initialize(LootItemInstance instance)
        {
            itemInstance = instance;
            gameObject.name = instance == null ? "LootPickup" : $"LootPickup_{instance.DisplayName}";
        }

        private void Awake()
        {
            Collider pickupCollider = GetComponent<Collider>();
            pickupCollider.isTrigger = true;

            if (itemInstance == null && fallbackDefinition != null)
            {
                itemInstance = fallbackDefinition.RollInstance(new System.Random());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!autoPickupOnPlayerTouch || itemInstance == null)
            {
                return;
            }

            PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
            if (inventory != null)
            {
                PickupPromptView.Active?.SetPrompt(this);
            }

            if (inventory != null && inventory.AddItem(itemInstance))
            {
                PickupPromptView.Active?.SetPrompt($"Picked up {itemInstance.DisplayName}");
                Destroy(gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<PlayerInventory>() != null)
            {
                PickupPromptView.Active?.ClearPrompt();
            }
        }
    }
}
