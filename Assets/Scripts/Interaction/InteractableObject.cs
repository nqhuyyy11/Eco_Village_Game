using UnityEngine;
using UnityEngine.InputSystem;
using EcoVillage.Quest;

namespace EcoVillage.Interaction
{
    /// <summary>
    /// Gắn script này lên các vật thể tương tác trong game:
    ///   - Đống rác (targetID = "Trash")
    ///   - Cây khô để chặt gỗ (targetID = "Wood")
    ///   - Đá cuội (targetID = "Stone")
    ///   - Giếng cổ (targetID = "OldWell")
    ///   - Cây cầu gỗ (targetID = "WoodenBridge")
    ///
    /// Khi người chơi đến gần và nhấn [E], object sẽ báo cáo tiến độ lên QuestManager.
    /// </summary>
    public class InteractableObject : MonoBehaviour
    {
        // ─── Inspector References ─────────────────────────────────────────────────
        [Header("Thông Tin Vật Thể")]
        [Tooltip("ID phải khớp với QuestStep.targetID trong QuestData. Ví dụ: 'Trash', 'Wood', 'OldWell'")]
        [SerializeField] private string targetID;

        [Tooltip("Loại bước nhiệm vụ tương ứng với hành động này.")]
        [SerializeField] private QuestStepType interactionType = QuestStepType.CollectResource;

        [Tooltip("Số lượng tài nguyên nhận được mỗi lần tương tác.")]
        [SerializeField] private int amountPerInteraction = 1;

        [Header("Tài Nguyên Thả Ra (nếu có)")]
        [Tooltip("Tên loại tài nguyên cộng vào kho đồ. Ví dụ: 'Wood', 'Stone'. Để trống nếu không cộng.")]
        [SerializeField] private string resourceDropType;

        [Tooltip("Số lượng tài nguyên thả ra.")]
        [SerializeField] private int resourceDropAmount = 1;

        [Header("Chi Phí Năng Lượng")]
        [Tooltip("Năng lượng tiêu hao mỗi lần tương tác. Kết nối với ProfileHUD.")]
#pragma warning disable 0414
        [SerializeField] private float energyCost = 10f;
#pragma warning restore 0414

        [Header("Sau Khi Tương Tác")]
        [Tooltip("Nếu true, object sẽ bị ẩn/xóa sau khi tương tác đủ số lần.")]
        [SerializeField] private bool destroyOnComplete = true;

        [Tooltip("Số lần tương tác cần thiết để hủy object này (ví dụ: cây cần 3 lần chặt).")]
        [SerializeField] private int interactionsRequired = 1;

        [Header("Giao Diện Chỉ Dẫn")]
        [Tooltip("Icon [E] hiện ra khi người chơi đứng gần.")]
        [SerializeField] private GameObject interactPromptUI;

        [Header("Hiệu Ứng")]
        [Tooltip("Particle hoặc Animation khi tương tác thành công. Kéo GameObject vào đây.")]
        [SerializeField] private GameObject interactEffect;

        // ─── Private State ────────────────────────────────────────────────────────
        private bool _playerInRange;
        private int _currentInteractions;
        private bool _isDepleted;

        // ─────────────────────────────────────────────────────────────────────────
        // UNITY LIFECYCLE
        // ─────────────────────────────────────────────────────────────────────────

        private void Start()
        {
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }

        private void Update()
        {
            if (_playerInRange && !_isDepleted)
            {
                // Không tương tác khi đang có hội thoại
                if (Dialogue.DialogueManager.Instance != null &&
                    Dialogue.DialogueManager.Instance.IsDialogueActive()) return;

                if (Keyboard.current != null && (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
                {
                    Interact();
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // TRIGGER 2D DETECTION
        // ─────────────────────────────────────────────────────────────────────────

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || _isDepleted) return;
            _playerInRange = true;
            if (interactPromptUI != null) interactPromptUI.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = false;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // INTERACT LOGIC
        // ─────────────────────────────────────────────────────────────────────────

        private void Interact()
        {
            // Kiểm tra năng lượng (kết nối ProfileHUD sau)
            // TODO: if (!PlayerEnergy.Instance.HasEnough(energyCost)) return;
            // TODO: PlayerEnergy.Instance.Spend(energyCost);

            _currentInteractions++;

            // Hiệu ứng tương tác
            if (interactEffect != null) interactEffect.SetActive(true);

            // Báo cáo tiến độ lên QuestManager
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.ReportProgress(interactionType, targetID, amountPerInteraction);
            }

            // TODO: Cộng tài nguyên vào Inventory (Thành viên 2 implement)
            if (!string.IsNullOrEmpty(resourceDropType))
            {
                Debug.Log($"[InteractableObject] Cộng {resourceDropAmount}x {resourceDropType} vào kho đồ.");
                // InventoryManager.Instance?.AddItem(resourceDropType, resourceDropAmount);
            }

            Debug.Log($"[InteractableObject] Đã tương tác với '{targetID}' ({_currentInteractions}/{interactionsRequired})");

            // Kiểm tra đã đủ số lần chưa
            if (_currentInteractions >= interactionsRequired)
            {
                OnDeplete();
            }
        }

        private void OnDeplete()
        {
            _isDepleted = true;

            if (interactPromptUI != null) interactPromptUI.SetActive(false);

            if (destroyOnComplete)
            {
                // Có thể dùng animation fade out trước khi destroy
                Destroy(gameObject, 0.3f);
            }
            else
            {
                // Hoặc đổi sang sprite "đã xử lý" (cây đã chặt, rác đã dọn)
                GetComponent<SpriteRenderer>()?.gameObject.SetActive(false);
            }
        }
    }
}
