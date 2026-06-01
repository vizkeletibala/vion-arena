using System;
using System.Collections;
using UnityEngine;
using Vitrial.Player;

namespace Vitrial.Weapons
{
    public sealed class StarterRifleController : MonoBehaviour
    {
        [SerializeField] private WeaponDefinition weaponDefinition;
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private Transform fireOrigin;
        [SerializeField] private LayerMask hitMask = ~0;

        private WeaponAmmoState ammoState;
        private float nextAllowedFireTime;
        private bool isReloading;

        public event Action<WeaponAmmoState> AmmoChanged;
        public event Action ReloadStarted;
        public event Action ReloadCompleted;
        public event Action<WeaponHitResult> Fired;

        public WeaponDefinition WeaponDefinition => weaponDefinition;
        public WeaponAmmoState AmmoState => ammoState;
        public bool IsReloading => isReloading;
        public float TimeBetweenShots => weaponDefinition != null ? weaponDefinition.TimeBetweenShots : float.PositiveInfinity;

        private void Awake()
        {
            if (inputReader == null)
            {
                inputReader = GetComponentInParent<PlayerInputReader>();
            }

            if (fireOrigin == null)
            {
                fireOrigin = transform;
            }

            if (weaponDefinition != null)
            {
                ammoState = WeaponAmmoState.FullFromDefinition(weaponDefinition);
                AmmoChanged?.Invoke(ammoState);
            }
        }

        private void Update()
        {
            if (weaponDefinition == null || inputReader == null || ammoState == null)
            {
                return;
            }

            if (inputReader.ReloadPressed)
            {
                TryStartReload();
            }

            if (inputReader.FirePressed)
            {
                TryFire();
            }
        }

        public bool TryFire()
        {
            if (weaponDefinition == null || ammoState == null || isReloading || Time.time < nextAllowedFireTime)
            {
                return false;
            }

            if (!ammoState.ConsumeRound())
            {
                TryStartReload();
                return false;
            }

            nextAllowedFireTime = Time.time + TimeBetweenShots;
            AmmoChanged?.Invoke(ammoState);

            WeaponHitResult hit = TraceShot();
            if (hit.Damageable != null)
            {
                hit.Damageable.ApplyDamage(new DamagePayload(weaponDefinition.Damage, hit.Point, hit.Normal, gameObject));
            }

            Fired?.Invoke(hit);
            return true;
        }

        public bool TryStartReload()
        {
            if (weaponDefinition == null || ammoState == null || isReloading || !ammoState.HasReserve)
            {
                return false;
            }

            if (ammoState.CurrentMagazine >= weaponDefinition.MagazineSize)
            {
                return false;
            }

            StartCoroutine(ReloadRoutine());
            return true;
        }

        private IEnumerator ReloadRoutine()
        {
            isReloading = true;
            ReloadStarted?.Invoke();

            yield return new WaitForSeconds(weaponDefinition.ReloadSeconds);

            ammoState.ReloadFromReserve(weaponDefinition.MagazineSize);
            isReloading = false;
            AmmoChanged?.Invoke(ammoState);
            ReloadCompleted?.Invoke();
        }

        private WeaponHitResult TraceShot()
        {
            Transform origin = fireOrigin != null ? fireOrigin : transform;
            Ray ray = new Ray(origin.position, origin.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, weaponDefinition.Range, hitMask, QueryTriggerInteraction.Ignore))
            {
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                return new WeaponHitResult(true, damageable, hit.point, hit.normal, hit.collider);
            }

            Vector3 missPoint = origin.position + origin.forward * weaponDefinition.Range;
            return new WeaponHitResult(false, null, missPoint, -origin.forward, null);
        }
    }
}
