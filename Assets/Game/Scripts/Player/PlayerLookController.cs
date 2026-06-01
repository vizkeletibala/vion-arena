using UnityEngine;

namespace Vitrial.Player
{
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerLookController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float mouseSensitivity = 2.5f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;
        [SerializeField] private bool lockCursorOnStart = true;

        private PlayerInputReader inputReader;
        private float yaw;
        private float pitch;

        public float Yaw => yaw;
        public float Pitch => pitch;
        public float MouseSensitivity => mouseSensitivity;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
            if (cameraPivot == null)
            {
                PlayerRigAnchor anchor = GetComponent<PlayerRigAnchor>();
                cameraPivot = anchor != null ? anchor.CameraPivot : null;
            }
        }

        private void Start()
        {
            yaw = transform.eulerAngles.y;
            if (lockCursorOnStart)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            Vector2 look = inputReader.Look * mouseSensitivity;
            yaw += look.x;
            pitch = Mathf.Clamp(pitch - look.y, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }
    }
}
