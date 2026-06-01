using UnityEngine;

namespace Vitrial.Weapons
{
    [CreateAssetMenu(menuName = "Vitrial/Weapons/Weapon Definition", fileName = "WeaponDefinition")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName = "Starter Rifle";

        [Header("Ballistics")]
        [SerializeField] private float damage = 18f;
        [SerializeField] private float roundsPerMinute = 420f;
        [SerializeField] private float range = 85f;

        [Header("Ammo")]
        [SerializeField] private int magazineSize = 30;
        [SerializeField] private int reserveAmmo = 90;
        [SerializeField] private float reloadSeconds = 1.8f;

        public string DisplayName => displayName;
        public float Damage => damage;
        public float RoundsPerMinute => roundsPerMinute;
        public float FireRate => roundsPerMinute / 60f;
        public float TimeBetweenShots => roundsPerMinute <= 0f ? float.PositiveInfinity : 60f / roundsPerMinute;
        public int MagazineSize => magazineSize;
        public int ReserveAmmo => reserveAmmo;
        public float ReloadSeconds => reloadSeconds;
        public float Range => range;
    }
}
