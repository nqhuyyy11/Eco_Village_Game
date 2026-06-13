using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Di chuyển nhân vật 2D Top-Down bằng New Input System.
/// Hỗ trợ: WASD, Arrow Keys, Sprint (Shift), Animator, Flip sprite.
/// Gắn lên Player GameObject (cần Rigidbody2D).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Tốc độ di chuyển bình thường.")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Tốc độ khi chạy nhanh (giữ Shift).")]
    [SerializeField] private float sprintSpeed = 8f;

    [Tooltip("Thời gian tăng/giảm tốc mượt mà.")]
    [SerializeField] private float speedSmoothTime = 0.1f;

    // ─── References ──────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // ─── Movement State ──────────────────────────────────────────────────────
    private Vector2 movementInput;
    private Vector2 smoothVelocity;
    private float currentSpeed;
    private bool isSprinting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();           // null-safe nếu không có
        spriteRenderer = GetComponent<SpriteRenderer>(); // null-safe nếu không có

        // Setup Rigidbody2D cho top-down
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        ReadInput();
        HandleSpriteFlip();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // INPUT (New Input System - direct Keyboard access)
    // ─────────────────────────────────────────────────────────────────────────

    private void ReadInput()
    {
        float horizontal = 0f;
        float vertical = 0f;
        isSprinting = false;

        if (Keyboard.current != null)
        {
            // WASD
            if (Keyboard.current.wKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed) vertical -= 1f;
            if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.dKey.isPressed) horizontal += 1f;

            // Arrow Keys (fallback)
            if (Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
            if (Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;

            // Sprint (Left Shift)
            isSprinting = Keyboard.current.leftShiftKey.isPressed;
        }

        movementInput = new Vector2(horizontal, vertical).normalized;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MOVEMENT
    // ─────────────────────────────────────────────────────────────────────────

    private void MovePlayer()
    {
        float targetSpeed = 0f;
        if (movementInput.magnitude > 0.01f)
        {
            targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
        }

        // Tăng/giảm tốc mượt
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothVelocity.x, speedSmoothTime);

        rb.MovePosition(rb.position + movementInput * currentSpeed * Time.fixedDeltaTime);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SPRITE FLIP
    // ─────────────────────────────────────────────────────────────────────────

    private void HandleSpriteFlip()
    {
        if (movementInput.x == 0) return;

        // Ưu tiên dùng localScale flip (giữ nguyên logic cũ cho Player có child objects)
        Vector3 scale = transform.localScale;
        scale.x = movementInput.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ANIMATOR
    // ─────────────────────────────────────────────────────────────────────────

    private void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", movementInput.sqrMagnitude);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PUBLIC API
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Trả về tỷ lệ tốc độ hiện tại (0..1) để bên ngoài dùng (ví dụ: blend animation).
    /// </summary>
    public float GetSpeedPercentage()
    {
        if (sprintSpeed == 0) return 0f;
        return currentSpeed / sprintSpeed;
    }
}
