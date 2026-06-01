using UnityEngine;

namespace Vitrial.Loot
{
    public enum LootRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    [CreateAssetMenu(menuName = "Vitrial/Loot/Loot Item", fileName = "LootItemDefinition")]
    public sealed class LootItemDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Prototype Drop";
        [SerializeField] private LootRarity rarity = LootRarity.Common;
        [SerializeField] private Sprite icon;

        public string DisplayName => displayName;
        public LootRarity Rarity => rarity;
        public Sprite Icon => icon;
    }
}
