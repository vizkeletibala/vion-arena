using UnityEngine;

namespace Vitrial.Player
{
    public sealed class PlayerRigAnchor : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform weaponSocket;

        public Transform CameraPivot => cameraPivot;
        public Transform WeaponSocket => weaponSocket;
    }
}
