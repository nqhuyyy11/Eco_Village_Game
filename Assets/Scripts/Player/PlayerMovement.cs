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

        // --- ĐOẠN MỚI: Lật cả nhân vật (Thân + Tóc) khi quay trái/phải ---
        if (movement.x != 0)
        {
            Vector3 scale = transform.localScale;
            // Giữ nguyên độ to nhỏ (Scale) ban đầu, chỉ lật âm/dương của trục X
            scale.x = movement.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // Báo cho Animator biết nhân vật đang di chuyển hay đứng im
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        // Di chuyển nhân vật (dùng FixedUpdate cho vật lý để không bị giật lag)
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
