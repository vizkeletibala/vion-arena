using UnityEngine;

namespace Vitrial.Weapons
{
    public readonly struct WeaponHitResult
    {
        public WeaponHitResult(bool didHit, IDamageable damageable, Vector3 point, Vector3 normal, Collider collider)
        {
            DidHit = didHit;
            Damageable = damageable;
            Point = point;
            Normal = normal;
            Collider = collider;
        }

        public bool DidHit { get; }
        public IDamageable Damageable { get; }
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public Collider Collider { get; }
    }
}
