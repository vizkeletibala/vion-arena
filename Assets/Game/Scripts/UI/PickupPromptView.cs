using UnityEngine;
using UnityEngine.UI;
using Vitrial.Inventory;
using Vitrial.Loot;

namespace Vitrial.UI
{
    public sealed class PickupPromptView : MonoBehaviour
    {
        [SerializeField] private Text promptText;
        [SerializeField] private CanvasGroup promptGroup;
        [SerializeField] private string promptFormat = "Pick up {0}";
        [SerializeField, Min(0.25f)] private float autoClearSeconds = 1.75f;

        public static PickupPromptView Active { get; private set; }

        private LootPickup currentPickup;
        private float clearAtTime;

        private void Awake()
        {
            EnsureFallbackText();
            Active = this;
            ClearPrompt();
        }

        private void EnsureFallbackText()
        {
            if (promptText == null)
            {
                GameObject textObject = new GameObject("PickupPromptText", typeof(RectTransform), typeof(Text));
                textObject.transform.SetParent(transform, false);

                RectTransform rect = textObject.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(0f, -120f);
                rect.sizeDelta = new Vector2(520f, 36f);

                promptText = textObject.GetComponent<Text>();
                promptText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                promptText.fontSize = 22;
                promptText.alignment = TextAnchor.MiddleCenter;
                promptText.color = Color.white;
            }

            if (promptGroup == null)
            {
                promptGroup = GetComponent<CanvasGroup>();
                if (promptGroup == null)
                {
                    promptGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        private void OnDestroy()
        {
            if (Active == this)
            {
                Active = null;
            }
        }

        private void Update()
        {
            if (clearAtTime > 0f && Time.unscaledTime >= clearAtTime)
            {
                ClearPrompt();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            LootPickup pickup = other.GetComponentInParent<LootPickup>();
            PlayerInventory inventory = GetComponentInParent<PlayerInventory>();
            if (pickup != null && inventory != null)
            {
                SetPrompt(pickup);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            LootPickup pickup = other.GetComponentInParent<LootPickup>();
            if (pickup != null && pickup == currentPickup)
            {
                ClearPrompt();
            }
        }

        public void SetPrompt(LootPickup pickup)
        {
            currentPickup = pickup;
            string displayName = pickup?.ItemInstance?.DisplayName ?? "loot";
            SetPrompt(string.Format(promptFormat, displayName));
        }

        public void SetPrompt(string message)
        {
            if (promptText != null)
            {
                promptText.text = message;
            }

            bool hasMessage = !string.IsNullOrWhiteSpace(message);
            clearAtTime = hasMessage ? Time.unscaledTime + autoClearSeconds : 0f;

            if (promptGroup != null)
            {
                promptGroup.alpha = hasMessage ? 1f : 0f;
                promptGroup.blocksRaycasts = false;
                promptGroup.interactable = false;
            }
        }

        public void ClearPrompt()
        {
            currentPickup = null;
            clearAtTime = 0f;
            SetPrompt(string.Empty);
        }
    }
}
