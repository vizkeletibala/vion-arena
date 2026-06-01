using UnityEngine;

namespace Vitrial.Weapons
{
    [CreateAssetMenu(menuName = "Vitrial/Weapons/Weapon Definition", fileName = "WeaponDefinition")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Starter Rifle";
        [SerializeField] private float damage = 10f;
        [SerializeField] private float fireRate = 8f;
        [SerializeField] private int magazineSize = 30;
        [SerializeField] private float reloadSeconds = 1.8f;
        [SerializeField] private float range = 100f;

        public string DisplayName => displayName;
        public float Damage => damage;
        public float FireRate => fireRate;
        public int MagazineSize => magazineSize;
        public float ReloadSeconds => reloadSeconds;
        public float Range => range;
    }
}
