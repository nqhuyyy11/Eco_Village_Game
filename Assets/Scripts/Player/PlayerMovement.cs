using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Thiết lập cơ bản cho Rigidbody2D ở chế độ Top-Down
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Lấy thông tin nhấn phím WASD hoặc Mũi tên
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Chuẩn hóa vector để đi chéo không bị nhanh hơn
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Truyền tham số sang Animator để chạy Animation
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Lưu lại hướng nhìn cuối cùng để dùng cho Animation đứng im (Idle)
        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat("LastHorizontal", movement.x);
            animator.SetFloat("LastVertical", movement.y);
        }
    }

    void FixedUpdate()
    {
        // Di chuyển nhân vật (dùng FixedUpdate cho vật lý để không bị giật lag)
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
