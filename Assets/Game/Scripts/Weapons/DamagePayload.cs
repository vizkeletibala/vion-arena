using UnityEngine;

namespace Vitrial.Weapons
{
    public readonly struct DamagePayload
    {
        public DamagePayload(float amount, Vector3 point, Vector3 normal, GameObject instigator)
        {
            Amount = amount;
            Point = point;
            Normal = normal;
            Instigator = instigator;
        }

        public float Amount { get; }
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public GameObject Instigator { get; }
    }
}
