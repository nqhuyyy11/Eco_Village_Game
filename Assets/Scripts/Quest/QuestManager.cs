using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EcoVillage.Quest
{
    /// <summary>
    /// Trạng thái của một nhiệm vụ tại runtime.
    /// </summary>
    public enum QuestStatus
    {
        NotStarted, // Chưa nhận
        Active,     // Đang làm
        Completed   // Đã hoàn thành
    }

    /// <summary>
    /// Singleton quản lý toàn bộ trạng thái Quest trong game.
    /// Gắn script này lên một GameObject tên là "QuestManager" trong Scene.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────────
        public static QuestManager Instance { get; private set; }

        // ─── Inspector References ─────────────────────────────────────────────────
        [Header("Danh Sách Quest Chương I")]
        [Tooltip("Kéo tất cả QuestData của Chương I vào đây để đăng ký theo dõi.")]
        [SerializeField] private List<QuestData> allQuests = new List<QuestData>();

        // ─── Events ───────────────────────────────────────────────────────────────
        [Header("Sự Kiện Quest")]
        [Tooltip("Gọi khi một quest mới bắt đầu. Truyền tên quest vào.")]
        public UnityEvent<string> onQuestStarted;

        [Tooltip("Gọi khi một quest hoàn thành. Truyền tên quest vào.")]
        public UnityEvent<string> onQuestCompleted;

        [Tooltip("Gọi khi tiến độ một bước quest thay đổi.")]
        public UnityEvent onQuestProgressUpdated;

        // ─── Private State ────────────────────────────────────────────────────────
        private Dictionary<string, QuestStatus> _questStatuses = new Dictionary<string, QuestStatus>();

        // ─────────────────────────────────────────────────────────────────────────
        // UNITY LIFECYCLE
        // ─────────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            transform.SetParent(null); // Đảm bảo object là Root GameObject trước khi DontDestroyOnLoad
            DontDestroyOnLoad(gameObject);

            InitializeQuestStatuses();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // PUBLIC API – Gọi từ bên ngoài (NPC, Collectible, Building...)
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Bắt đầu một nhiệm vụ. Gọi từ DialogueManager sau khi kết thúc hội thoại giao quest.
        /// </summary>
        public void StartQuest(QuestData questData)
        {
            if (questData == null) return;

            if (!_questStatuses.ContainsKey(questData.questID))
            {
                _questStatuses[questData.questID] = QuestStatus.NotStarted;
            }

            if (_questStatuses[questData.questID] != QuestStatus.NotStarted)
            {
                Debug.Log($"[QuestManager] Quest '{questData.questName}' đã được bắt đầu hoặc hoàn thành.");
                return;
            }

            questData.ResetProgress();
            _questStatuses[questData.questID] = QuestStatus.Active;

            Debug.Log($"[QuestManager] ✅ Quest bắt đầu: '{questData.questName}'");
            onQuestStarted?.Invoke(questData.questName);

            // Phát hội thoại bắt đầu quest nếu có
            if (questData.onStartDialogue != null)
            {
                Dialogue.DialogueManager.Instance?.StartDialogue(questData.onStartDialogue);
            }
        }

        /// <summary>
        /// Báo cáo tiến độ cho tất cả quest đang Active có StepType và TargetID tương ứng.
        /// Gọi hàm này từ bất kỳ đâu trong game khi người chơi làm gì đó.
        ///
        /// Ví dụ sử dụng:
        ///   QuestManager.Instance.ReportProgress(QuestStepType.CollectResource, "Wood", 1);
        ///   QuestManager.Instance.ReportProgress(QuestStepType.InteractWithObject, "OldWell", 1);
        /// </summary>
        /// <param name="stepType">Loại hành động vừa xảy ra</param>
        /// <param name="targetID">ID mục tiêu (phải khớp với QuestStep.targetID)</param>
        /// <param name="amount">Số lượng tăng thêm</param>
        public void ReportProgress(QuestStepType stepType, string targetID, int amount = 1)
        {
            bool anyUpdated = false;

            foreach (QuestData quest in allQuests)
            {
                if (GetQuestStatus(quest.questID) != QuestStatus.Active) continue;

                foreach (QuestStep step in quest.steps)
                {
                    if (step.isCompleted) continue;
                    if (step.stepType != stepType) continue;
                    if (step.targetID != targetID) continue;

                    step.AddProgress(amount);
                    anyUpdated = true;

                    Debug.Log($"[QuestManager] 📋 '{quest.questName}' → {step.GetFullDescription()}");

                    // Kiểm tra toàn bộ quest hoàn thành không
                    if (quest.IsAllStepsCompleted())
                    {
                        CompleteQuest(quest);
                    }

                    break; // Mỗi quest chỉ cập nhật một step tại một thời điểm
                }
            }

            if (anyUpdated) onQuestProgressUpdated?.Invoke();
        }

        /// <summary>
        /// Lấy trạng thái của một quest theo ID.
        /// </summary>
        public QuestStatus GetQuestStatus(string questID)
        {
            if (_questStatuses.TryGetValue(questID, out QuestStatus status))
                return status;
            return QuestStatus.NotStarted;
        }

        /// <summary>
        /// Lấy danh sách tất cả quest đang Active.
        /// Dùng để hiển thị trên Quest Log UI.
        /// </summary>
        public List<QuestData> GetActiveQuests()
        {
            List<QuestData> activeList = new List<QuestData>();
            foreach (QuestData quest in allQuests)
            {
                if (GetQuestStatus(quest.questID) == QuestStatus.Active)
                    activeList.Add(quest);
            }
            return activeList;
        }

        /// <summary>
        /// Kiểm tra một quest cụ thể đã hoàn thành chưa.
        /// </summary>
        public bool IsQuestCompleted(string questID)
        {
            return GetQuestStatus(questID) == QuestStatus.Completed;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // PRIVATE METHODS
        // ─────────────────────────────────────────────────────────────────────────

        private void InitializeQuestStatuses()
        {
            foreach (QuestData quest in allQuests)
            {
                if (!_questStatuses.ContainsKey(quest.questID))
                {
                    _questStatuses[quest.questID] = QuestStatus.NotStarted;
                    quest.ResetProgress();
                }
            }
        }

        private void CompleteQuest(QuestData questData)
        {
            _questStatuses[questData.questID] = QuestStatus.Completed;

            Debug.Log($"[QuestManager] 🏆 Quest hoàn thành: '{questData.questName}'!");

            // Trao phần thưởng
            GiveReward(questData.reward);

            // Phát hội thoại hoàn thành quest
            if (questData.onCompleteDialogue != null)
            {
                Dialogue.DialogueManager.Instance?.StartDialogue(questData.onCompleteDialogue);
            }

            onQuestCompleted?.Invoke(questData.questName);

            // Tự động bắt đầu quest tiếp theo nếu có
            if (questData.nextQuest != null)
            {
                StartQuest(questData.nextQuest);
            }
        }

        private void GiveReward(QuestReward reward)
        {
            if (reward == null) return;

            // Kết nối với ProfileHUD / PlayerData để cộng vàng và Reputation
            // Thành viên 2 (Inventory) sẽ cung cấp PlayerData.Instance
            // Thành viên 4 (Reputation) sẽ cung cấp ReputationManager.Instance
            // Tạm thời dùng log để test:
            Debug.Log($"[QuestManager] 💰 Thưởng: +{reward.goldAmount} Vàng, +{reward.reputationAmount} Reputation");

            // TODO: Uncomment khi Thành viên 2 và 4 hoàn thành phần của họ:
            // PlayerData.Instance?.AddGold(reward.goldAmount);
            // ReputationManager.Instance?.AddReputation(reward.reputationAmount);
        }
    }
}
