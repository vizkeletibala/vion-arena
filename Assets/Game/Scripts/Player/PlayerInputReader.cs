using UnityEngine;

namespace Vitrial.Player
{
    /// <summary>
    /// Thin legacy-input adapter for the greybox controller.
    /// Keep gameplay systems coupled to these named properties instead of raw KeyCode checks.
    /// </summary>
    public sealed class PlayerInputReader : MonoBehaviour
    {
        public const string FireAction = "fire: Left Mouse Button";
        public const string ReloadAction = "reload: R";
        public const string InteractAction = "interact: E";
        public const string InventoryAction = "inventory: Tab";

        [Header("Look")]
        [SerializeField] private bool readMouse = true;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool SprintHeld { get; private set; }

        public bool FirePressed { get; private set; }
        public bool ReloadPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool InventoryPressed { get; private set; }

        private void Update()
        {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Look = readMouse ? new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) : Vector2.zero;

            JumpPressed = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space);
            SprintHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            FirePressed = Input.GetMouseButtonDown(0);
            ReloadPressed = Input.GetKeyDown(KeyCode.R);
            InteractPressed = Input.GetKeyDown(KeyCode.E);
            InventoryPressed = Input.GetKeyDown(KeyCode.Tab);
        }
    }
}
