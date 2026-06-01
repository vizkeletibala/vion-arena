using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Vitrial.Inventory;
using Vitrial.Loot;
using Vitrial.Player;

namespace Vitrial.UI
{
    public sealed class InventoryComparisonView : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Text titleText;
        [SerializeField] private Text slotsText;
        [SerializeField] private Text comparisonText;
        [SerializeField] private Text controlsText;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerInputReader inputReader;

        private int selectedIndex;
        private bool isOpen;

        private void Awake()
        {
            EnsureFallbackUI();

            if (inventory == null)
            {
                inventory = FindObjectOfType<PlayerInventory>();
            }

            if (inputReader == null)
            {
                inputReader = FindObjectOfType<PlayerInputReader>();
            }

            SetOpen(false);
        }

        private void EnsureFallbackUI()
        {
            if (inventoryPanel == null)
            {
                inventoryPanel = new GameObject("InventoryPanel", typeof(RectTransform));
                inventoryPanel.transform.SetParent(transform, false);

                RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(1f, 0.5f);
                panelRect.anchorMax = new Vector2(1f, 0.5f);
                panelRect.pivot = new Vector2(1f, 0.5f);
                panelRect.anchoredPosition = new Vector2(-24f, 0f);
                panelRect.sizeDelta = new Vector2(520f, 520f);
            }

            EnsurePanelText(ref titleText, "InventoryTitle", new Vector2(0f, -10f), new Vector2(500f, 32f), 22, TextAnchor.UpperLeft);
            EnsurePanelText(ref slotsText, "InventorySlots", new Vector2(0f, -52f), new Vector2(245f, 380f), 17, TextAnchor.UpperLeft);
            EnsurePanelText(ref comparisonText, "InventoryComparison", new Vector2(255f, -52f), new Vector2(245f, 380f), 17, TextAnchor.UpperLeft);
            EnsurePanelText(ref controlsText, "InventoryControls", new Vector2(0f, -462f), new Vector2(500f, 42f), 16, TextAnchor.UpperLeft);
        }

        private void EnsurePanelText(ref Text field, string objectName, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
        {
            if (field != null)
            {
                return;
            }

            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(inventoryPanel.transform, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            field = textObject.GetComponent<Text>();
            field.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            field.fontSize = fontSize;
            field.alignment = alignment;
            field.color = Color.white;
        }

        private void OnEnable()
        {
            if (inventory != null)
            {
                inventory.InventoryChanged += OnInventoryChanged;
                inventory.EquipmentChanged += OnEquipmentChanged;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (inventory != null)
            {
                inventory.InventoryChanged -= OnInventoryChanged;
                inventory.EquipmentChanged -= OnEquipmentChanged;
            }
        }

        private void Update()
        {
            if ((inputReader != null && inputReader.InventoryPressed) || Input.GetKeyDown(KeyCode.Tab))
            {
                SetOpen(!isOpen);
            }

            if (!isOpen || inventory == null || inventory.Slots.Count == 0)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Select(selectedIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Select(selectedIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
            {
                EquipSelected();
            }
        }

        public void SetOpen(bool open)
        {
            isOpen = open;
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isOpen);
            }

            Refresh();
        }

        public void Select(int index)
        {
            if (inventory == null || inventory.Slots.Count == 0)
            {
                selectedIndex = 0;
                Refresh();
                return;
            }

            selectedIndex = Mathf.Clamp(index, 0, inventory.Slots.Count - 1);
            Refresh();
        }

        public bool EquipSelected()
        {
            LootItemInstance selected = GetSelectedItem();
            bool equipped = inventory != null && inventory.TryEquip(selected);
            Refresh();
            return equipped;
        }

        private void OnInventoryChanged(System.Collections.Generic.IReadOnlyList<InventorySlot> slots)
        {
            if (selectedIndex >= slots.Count)
            {
                selectedIndex = Mathf.Max(0, slots.Count - 1);
            }

            Refresh();
        }

        private void OnEquipmentChanged(EquipmentSlot slot, LootItemInstance item)
        {
            Refresh();
        }

        private LootItemInstance GetSelectedItem()
        {
            if (inventory == null || inventory.Slots.Count == 0 || selectedIndex < 0 || selectedIndex >= inventory.Slots.Count)
            {
                return null;
            }

            return inventory.Slots[selectedIndex].Item;
        }

        private void Refresh()
        {
            if (titleText != null)
            {
                titleText.text = "Inventory Comparison";
            }

            if (controlsText != null)
            {
                controlsText.text = "Tab Inventory | Up/Down Select | Enter/E Equip";
            }

            RefreshSlots();
            RefreshComparison();
        }

        private void RefreshSlots()
        {
            if (slotsText == null)
            {
                return;
            }

            if (inventory == null || inventory.Slots.Count == 0)
            {
                slotsText.text = "No loot collected";
                return;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                LootItemInstance item = inventory.Slots[i].Item;
                LootItemInstance equipped = item != null ? inventory.GetEquipped(item.EquipmentSlot) : null;
                string cursor = i == selectedIndex ? ">" : " ";
                string equippedMarker = ReferenceEquals(equipped, item) ? " [EQUIPPED]" : string.Empty;
                builder.AppendLine($"{cursor} {item?.DisplayName ?? "Empty"}{equippedMarker}");
            }

            slotsText.text = builder.ToString();
        }

        private void RefreshComparison()
        {
            if (comparisonText == null)
            {
                return;
            }

            LootItemInstance selected = GetSelectedItem();
            if (selected == null)
            {
                comparisonText.text = "Select collected loot to compare.";
                return;
            }

            LootItemInstance equipped = inventory.GetEquipped(selected.EquipmentSlot);
            float equippedScore = equipped != null ? equipped.CompareScore : 0f;
            float delta = selected.CompareScore - equippedScore;
            ItemStatBlock stats = selected.RolledStats;

            comparisonText.text =
                $"Selected: {selected.DisplayName}\n" +
                $"Slot: {selected.EquipmentSlot}\n" +
                $"Damage {stats.Damage:0.#} | FireRate {stats.FireRate:0.#} | Mag {stats.MagazineSize:0.#}\n" +
                $"Health {stats.MaxHealth:0.#} | Move {stats.MoveSpeed:0.#}\n" +
                $"Score {selected.CompareScore:0.#} ({delta:+0.#;-0.#;0} vs equipped)\n" +
                $"Equipped: {(equipped != null ? equipped.DisplayName : "None")}";
        }
    }
}
