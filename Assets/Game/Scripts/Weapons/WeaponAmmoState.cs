using System;
using UnityEngine;

namespace Vitrial.Weapons
{
    [Serializable]
    public sealed class WeaponAmmoState
    {
        [SerializeField] private int currentMagazine;
        [SerializeField] private int reserveAmmo;

        public WeaponAmmoState(int currentMagazine, int reserveAmmo)
        {
            CurrentMagazine = currentMagazine;
            ReserveAmmo = reserveAmmo;
        }

        public int CurrentMagazine
        {
            get => currentMagazine;
            private set => currentMagazine = Mathf.Max(0, value);
        }

        public int ReserveAmmo
        {
            get => reserveAmmo;
            private set => reserveAmmo = Mathf.Max(0, value);
        }

        public bool CanFire => CurrentMagazine > 0;
        public bool HasReserve => ReserveAmmo > 0;

        public static WeaponAmmoState FullFromDefinition(WeaponDefinition definition)
        {
            return new WeaponAmmoState(definition.MagazineSize, definition.ReserveAmmo);
        }

        public bool ConsumeRound()
        {
            if (!CanFire)
            {
                return false;
            }

            CurrentMagazine--;
            return true;
        }

        public int ReloadFromReserve(int magazineSize)
        {
            int needed = Mathf.Max(0, magazineSize - CurrentMagazine);
            int loaded = Mathf.Min(needed, ReserveAmmo);
            CurrentMagazine += loaded;
            ReserveAmmo -= loaded;
            return loaded;
        }
    }
}
