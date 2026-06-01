using UnityEngine;

namespace Vitrial.Player
{
    public sealed class PlayerRigAnchor : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private PlayerMotor motor;
        [SerializeField] private PlayerLookController lookController;

        public Transform CameraPivot => cameraPivot;
        public Transform WeaponSocket => weaponSocket;
        public PlayerInputReader InputReader => inputReader;
        public PlayerMotor Motor => motor;
        public PlayerLookController LookController => lookController;

        private void Reset()
        {
            inputReader = GetComponent<PlayerInputReader>();
            motor = GetComponent<PlayerMotor>();
            lookController = GetComponent<PlayerLookController>();
        }
    }
}
