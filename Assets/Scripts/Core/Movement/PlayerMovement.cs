using UnityEngine;
using UnityEngine.InputSystem;

namespace EcoVillage.Core.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Movement speed of the player.")]
        [SerializeField] private float moveSpeed = 5f;
        [Tooltip("Speed when sprinting.")]
        [SerializeField] private float sprintSpeed = 8f;
        [Tooltip("Smooth time for movement acceleration/deceleration.")]
        [SerializeField] private float speedSmoothTime = 0.1f;

        // References
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        // Movement variables
        private Vector2 movementInput;
        private Vector2 smoothVelocity;
        private float currentSpeed;
        private bool isSprinting;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Setup Rigidbody2D defaults for top-down game
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.freezeRotation = true;
        }

        private void Update()
        {
            ReadInput();
            HandleSpriteFlip();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void ReadInput()
        {
            // Using the new Input System's direct Keyboard access
            // This is 100% compatible with the Input System package and requires no inspector binding.
            float horizontal = 0f;
            float vertical = 0f;
            isSprinting = false;

            if (Keyboard.current != null)
            {
                // Read WASD
                if (Keyboard.current.wKey.isPressed) vertical += 1f;
                if (Keyboard.current.sKey.isPressed) vertical -= 1f;
                if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
                if (Keyboard.current.dKey.isPressed) horizontal += 1f;

                // Read Arrow Keys (Fallback)
                if (Keyboard.current.upArrowKey.isPressed) vertical += 1f;
                if (Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
                if (Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
                if (Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;

                // Read Sprint Key (Left Shift)
                isSprinting = Keyboard.current.leftShiftKey.isPressed;
            }

            movementInput = new Vector2(horizontal, vertical).normalized;
        }

        private void HandleSpriteFlip()
        {
            // Flip sprite horizontally based on move direction if SpriteRenderer exists
            if (spriteRenderer != null && movementInput.x != 0)
            {
                spriteRenderer.flipX = movementInput.x < 0;
            }
        }

        private void MovePlayer()
        {
            // Determine target speed (walking vs sprinting)
            float targetSpeed = 0f;
            if (movementInput.magnitude > 0.01f)
            {
                targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
            }

            // Smooth speed changes
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothVelocity.x, speedSmoothTime);

            // Set rigidbody velocity (using Unity 6 standard linearVelocity)
            rb.linearVelocity = movementInput * currentSpeed;
        }

        /// <summary>
        /// External API to get normalized speed for animator controller.
        /// </summary>
        public float GetSpeedPercentage()
        {
            if (moveSpeed == 0) return 0f;
            return rb.linearVelocity.magnitude / sprintSpeed;
        }
    }
}
