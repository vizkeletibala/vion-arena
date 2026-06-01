using UnityEngine;

namespace Vitrial.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5.5f;
        [SerializeField] private float sprintSpeed = 8.5f;
        [SerializeField] private float acceleration = 18f;

        [Header("Jumping")]
        [SerializeField] private float jumpHeight = 1.35f;
        [SerializeField] private float gravity = -24f;
        [SerializeField] private float groundedStickForce = -2f;

        private CharacterController characterController;
        private PlayerInputReader inputReader;
        private Vector3 planarVelocity;
        private float verticalVelocity;

        public CharacterController CharacterController => characterController;
        public float WalkSpeed => walkSpeed;
        public float SprintSpeed => sprintSpeed;
        public float JumpHeight => jumpHeight;
        public float Gravity => gravity;
        public Vector3 Velocity => planarVelocity + (Vector3.up * verticalVelocity);

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputReader = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            Vector2 move = Vector2.ClampMagnitude(inputReader.Move, 1f);
            float targetSpeed = inputReader.SprintHeld ? sprintSpeed : walkSpeed;
            Vector3 targetVelocity = (transform.right * move.x + transform.forward * move.y) * targetSpeed;
            planarVelocity = Vector3.MoveTowards(planarVelocity, targetVelocity, acceleration * Time.deltaTime);

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedStickForce;
            }

            if (characterController.isGrounded && inputReader.JumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 motion = (planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime;
            characterController.Move(motion);
        }
    }
}
