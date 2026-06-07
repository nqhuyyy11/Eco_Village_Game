using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace EcoVillage.Dialogue
{
    /// <summary>
    /// Singleton quản lý toàn bộ quá trình hiển thị hội thoại trên UI.
    /// Gắn script này lên một GameObject tên là "DialogueManager" trong Scene.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────────
        public static DialogueManager Instance { get; private set; }

        // ─── Inspector References ─────────────────────────────────────────────────
        [Header("UI Hộp Thoại (Kéo từ Hierarchy vào)")]
        [Tooltip("Panel GameObject bao ngoài toàn bộ hộp thoại.")]
        [SerializeField] private GameObject dialoguePanel;

        [Tooltip("Ảnh chân dung người nói.")]
        [SerializeField] private Image portraitImage;

        [Tooltip("Text hiển thị tên người nói.")]
        [SerializeField] private TextMeshProUGUI speakerNameText;

        [Tooltip("Text hiển thị nội dung lời thoại.")]
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Tooltip("Nút [Tiếp theo] để qua dòng hội thoại.")]
        [SerializeField] private GameObject nextButton;

        [Header("Panel Lựa Chọn (Branching)")]
        [Tooltip("Panel chứa các nút lựa chọn phân nhánh.")]
        [SerializeField] private GameObject choicePanel;

        [Tooltip("Mảng các nút lựa chọn (cần ít nhất 2 nút trong scene).")]
        [SerializeField] private Button[] choiceButtons;

        [Tooltip("Mảng text label cho các nút lựa chọn.")]
        [SerializeField] private TextMeshProUGUI[] choiceTexts;

        [Header("Cài Đặt Hiệu Ứng")]
        [Tooltip("Tốc độ gõ từng ký tự (giây/ký tự). 0.03 = khá nhanh, 0.06 = trung bình.")]
        [SerializeField] private float typingSpeed = 0.04f;

        [Tooltip("Sprite mặc định khi không có ảnh chân dung.")]
        [SerializeField] private Sprite defaultPortrait;

        // ─── Events ───────────────────────────────────────────────────────────────
        [Header("Sự Kiện")]
        public UnityEvent onDialogueStart;
        public UnityEvent onDialogueEnd;

        // ─── Private State ────────────────────────────────────────────────────────
        private DialogueData _currentDialogue;
        private int _currentLineIndex;
        private bool _isTyping;
        private bool _isActive;
        private Coroutine _typingCoroutine;

        // ─────────────────────────────────────────────────────────────────────────
        // UNITY LIFECYCLE
        // ─────────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Đảm bảo panel tắt khi bắt đầu game
            HideDialoguePanel();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // PUBLIC API – Gọi từ bên ngoài
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Bắt đầu hiển thị một cuộc hội thoại.
        /// Gọi hàm này từ NPCInteract.cs khi người chơi nhấn E.
        /// </summary>
        public void StartDialogue(DialogueData data)
        {
            if (data == null || data.lines == null || data.lines.Length == 0)
            {
                Debug.LogWarning("[DialogueManager] DialogueData rỗng hoặc null!");
                return;
            }

            _currentDialogue = data;
            _currentLineIndex = 0;
            _isActive = true;

            ShowDialoguePanel();
            onDialogueStart?.Invoke();

            DisplayCurrentLine();
        }

        /// <summary>
        /// Người chơi bấm nút "Tiếp theo" → qua dòng tiếp theo.
        /// Gắn hàm này vào sự kiện onClick của nút Next trong Unity Inspector.
        /// </summary>
        public void OnNextButtonClicked()
        {
            // Nếu đang gõ chữ → bấm skip, hiện toàn bộ text ngay
            if (_isTyping)
            {
                SkipTyping();
                return;
            }

            // Qua dòng tiếp theo
            _currentLineIndex++;

            if (_currentLineIndex < _currentDialogue.lines.Length)
            {
                DisplayCurrentLine();
            }
            else
            {
                // Hết tất cả dòng thoại
                EndDialogue();
            }
        }

        /// <summary>
        /// Người chơi chọn một nhánh hội thoại (choice button).
        /// Index tương ứng với DialogueData.branchDialogues[index].
        /// </summary>
        public void OnChoiceSelected(int choiceIndex)
        {
            if (_currentDialogue.branchDialogues == null ||
                choiceIndex >= _currentDialogue.branchDialogues.Length)
            {
                Debug.LogWarning("[DialogueManager] Không tìm thấy nhánh thoại!");
                EndDialogue();
                return;
            }

            HideChoicePanel();
            StartDialogue(_currentDialogue.branchDialogues[choiceIndex]);
        }

        /// <summary>
        /// Trả về true nếu hộp thoại đang mở.
        /// Dùng để chặn các input khác khi đang hội thoại.
        /// </summary>
        public bool IsDialogueActive() => _isActive;

        // ─────────────────────────────────────────────────────────────────────────
        // PRIVATE METHODS
        // ─────────────────────────────────────────────────────────────────────────

        private void DisplayCurrentLine()
        {
            DialogueLine line = _currentDialogue.lines[_currentLineIndex];

            // Cập nhật tên người nói
            speakerNameText.text = line.speakerName;

            // Cập nhật ảnh chân dung
            if (portraitImage != null)
            {
                portraitImage.sprite = line.speakerPortrait != null
                    ? line.speakerPortrait
                    : defaultPortrait;

                // Ẩn khung portrait nếu không có ảnh (Narrator)
                portraitImage.gameObject.SetActive(line.speakerPortrait != null);
            }

            // Kiểm tra có phải lựa chọn phân nhánh không
            if (line.isPlayerChoice && _currentDialogue.branchDialogues != null
                && _currentDialogue.branchDialogues.Length > 0)
            {
                ShowChoicePanel(_currentDialogue.branchDialogues);
                return;
            }

            // Hiển thị text với hiệu ứng gõ từng chữ
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeText(line.dialogueText));

            // Kích hoạt sự kiện nếu có
            if (!string.IsNullOrEmpty(line.triggerEventName))
            {
                TriggerLineEvent(line.triggerEventName);
            }
        }

        private IEnumerator TypeText(string text)
        {
            _isTyping = true;
            dialogueText.text = "";
            nextButton.SetActive(false);

            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            _isTyping = false;
            nextButton.SetActive(true);
        }

        private void SkipTyping()
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            dialogueText.text = _currentDialogue.lines[_currentLineIndex].dialogueText;
            _isTyping = false;
            nextButton.SetActive(true);
        }

        private void EndDialogue()
        {
            _isActive = false;
            HideDialoguePanel();
            onDialogueEnd?.Invoke();

            // Kích hoạt nhiệm vụ nếu DialogueData có gắn QuestData
            if (_currentDialogue.questToTrigger != null)
            {
                Quest.QuestManager.Instance?.StartQuest(_currentDialogue.questToTrigger);
            }

            // Chuyển sang hội thoại tiếp theo nếu có
            if (_currentDialogue.nextDialogue != null)
            {
                StartDialogue(_currentDialogue.nextDialogue);
            }
        }

        private void ShowChoicePanel(DialogueData[] branches)
        {
            if (choicePanel == null) return;
            choicePanel.SetActive(true);
            nextButton.SetActive(false);

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < branches.Length)
                {
                    choiceButtons[i].gameObject.SetActive(true);

                    // Hiển thị dòng đầu tiên của nhánh làm label nút
                    string label = (branches[i].lines != null && branches[i].lines.Length > 0)
                        ? branches[i].lines[0].dialogueText
                        : $"Lựa chọn {i + 1}";

                    if (i < choiceTexts.Length) choiceTexts[i].text = label;

                    int capturedIndex = i; // tránh closure bug
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(capturedIndex));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }

        private void HideChoicePanel()
        {
            if (choicePanel != null) choicePanel.SetActive(false);
        }

        private void ShowDialoguePanel()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(true);
        }

        private void HideDialoguePanel()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            HideChoicePanel();
        }

        private void TriggerLineEvent(string eventName)
        {
            // Có thể mở rộng bằng cách dùng Dictionary hoặc UnityEvent có tên
            Debug.Log($"[DialogueManager] Kích hoạt sự kiện: {eventName}");
        }
    }
}
