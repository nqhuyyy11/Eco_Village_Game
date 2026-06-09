using UnityEngine;
using UnityEngine.InputSystem;

namespace EcoVillage.Dialogue
{
    /// <summary>
    /// Gắn script này lên GameObject của NPC trong Scene.
    /// Khi người chơi đứng gần và nhấn [E], hội thoại sẽ bắt đầu.
    /// </summary>
    public class NPCInteract : MonoBehaviour
    {
        // ─── Inspector References ─────────────────────────────────────────────────
        [Header("Hội Thoại Của NPC")]
        [Tooltip("Hội thoại sẽ phát khi người chơi gặp NPC lần đầu tiên.")]
        [SerializeField] private DialogueData firstMeetDialogue;

        [Tooltip("Hội thoại sẽ phát khi người chơi tương tác lại (sau khi đã gặp).")]
        [SerializeField] private DialogueData repeatDialogue;

        [Header("Giao Diện Chỉ Dẫn")]
        [Tooltip("GameObject icon [E] hiển thị khi người chơi đứng gần. Kéo prefab từ Hierarchy.")]
        [SerializeField] private GameObject interactPromptUI;

        [Header("Tên NPC")]
        [Tooltip("Tên NPC hiển thị khi debug. Ví dụ: 'Cụ Bá'")]
        [SerializeField] private string npcName = "NPC";

        // ─── Private State ────────────────────────────────────────────────────────
        private bool _playerInRange;
        private bool _hasMetBefore;

        // ─────────────────────────────────────────────────────────────────────────
        // UNITY LIFECYCLE
        // ─────────────────────────────────────────────────────────────────────────

        private void Start()
        {
            // Ẩn UI [E] khi chưa có người chơi gần
            if (interactPromptUI != null)
                interactPromptUI.SetActive(false);
        }

        private void Update()
        {
            // Phát hiện người chơi nhấn [E] khi đứng trong vùng
            if (_playerInRange && !DialogueManager.Instance.IsDialogueActive())
            {
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    TriggerDialogue();
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // TRIGGER 2D DETECTION
        // ─────────────────────────────────────────────────────────────────────────

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

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
        // PRIVATE METHODS
        // ─────────────────────────────────────────────────────────────────────────

        private void TriggerDialogue()
        {
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("[NPCInteract] Không tìm thấy DialogueManager trong Scene!");
                return;
            }

            // Chọn hội thoại phù hợp: lần đầu vs. lặp lại
            DialogueData dialogueToPlay = (!_hasMetBefore && firstMeetDialogue != null)
                ? firstMeetDialogue
                : repeatDialogue;

            if (dialogueToPlay == null)
            {
                Debug.LogWarning($"[NPCInteract] NPC '{npcName}' chưa được gán DialogueData!");
                return;
            }

            _hasMetBefore = true;
            DialogueManager.Instance.StartDialogue(dialogueToPlay);

            // Ẩn UI prompt khi đang nói chuyện
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // PUBLIC API
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reset trạng thái đã gặp. Hữu ích khi cần NPC giao quest lại từ đầu (ví dụ: new game).
        /// </summary>
        public void ResetMeetState()
        {
            _hasMetBefore = false;
        }

        /// <summary>
        /// Thay thế hội thoại lặp lại của NPC (ví dụ: khi mở khóa quest mới).
        /// </summary>
        public void SetRepeatDialogue(DialogueData newDialogue)
        {
            repeatDialogue = newDialogue;
        }
    }
}
