using UnityEngine;

namespace EcoVillage.Quest
{
    /// <summary>
    /// Phần thưởng khi hoàn thành nhiệm vụ.
    /// </summary>
    [System.Serializable]
    public class QuestReward
    {
        [Tooltip("Số vàng thưởng. Để 0 nếu không thưởng vàng.")]
        public int goldAmount;

        [Tooltip("Điểm Reputation thưởng. Để 0 nếu không thưởng Reputation.")]
        public int reputationAmount;

        [Tooltip("Mô tả phần thưởng hiển thị trên UI. Ví dụ: '100 Vàng + 5 Điểm Danh Tiếng'")]
        public string rewardDescription;
    }

    /// <summary>
    /// ScriptableObject chứa toàn bộ dữ liệu của MỘT nhiệm vụ (Quest).
    /// Tạo file mới bằng cách: Right Click → Create → EcoVillage → Quest → Quest Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuest", menuName = "EcoVillage/Quest/Quest Data", order = 1)]
    public class QuestData : ScriptableObject
    {
        [Header("Thông Tin Nhiệm Vụ")]
        [Tooltip("ID định danh duy nhất của nhiệm vụ (dùng trong code). Ví dụ: 'Quest_CleanTrash_Ch1'")]
        public string questID;

        [Tooltip("Tên hiển thị trên Quest Log. Ví dụ: 'Lối Về Trong Sạch'")]
        public string questName;

        [Tooltip("Mô tả nhiệm vụ hiển thị khi người chơi mở Quest Log.")]
        [TextArea(2, 5)]
        public string questDescription;

        [Tooltip("Tên Chương chứa nhiệm vụ này. Ví dụ: 'Chương I: Tiếng Gọi Từ Thiên Nhiên'")]
        public string chapterName;

        [Header("Các Bước Nhiệm Vụ")]
        [Tooltip("Danh sách các bước cần hoàn thành (thực hiện tuần tự).")]
        public QuestStep[] steps;

        [Header("Phần Thưởng")]
        public QuestReward reward;

        [Header("Hội Thoại Liên Kết")]
        [Tooltip("Hội thoại phát khi nhiệm vụ vừa được giao (Quest Start).")]
        public Dialogue.DialogueData onStartDialogue;

        [Tooltip("Hội thoại phát khi người chơi hoàn thành nhiệm vụ (Quest Complete).")]
        public Dialogue.DialogueData onCompleteDialogue;

        [Tooltip("Nhiệm vụ tiếp theo sẽ mở khóa sau khi hoàn thành nhiệm vụ này.")]
        public QuestData nextQuest;

        // ─────────────────────────────────────────────────────────────────────────
        // RUNTIME HELPERS
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Trả về true nếu tất cả các bước đều đã hoàn thành.
        /// </summary>
        public bool IsAllStepsCompleted()
        {
            foreach (QuestStep step in steps)
            {
                if (!step.isCompleted) return false;
            }
            return true;
        }

        /// <summary>
        /// Reset toàn bộ tiến độ về 0. Dùng khi New Game.
        /// </summary>
        public void ResetProgress()
        {
            foreach (QuestStep step in steps)
            {
                step.currentAmount = 0;
                step.isCompleted = false;
            }
        }

        /// <summary>
        /// Tìm bước nhiệm vụ đầu tiên chưa hoàn thành.
        /// </summary>
        public QuestStep GetCurrentActiveStep()
        {
            foreach (QuestStep step in steps)
            {
                if (!step.isCompleted) return step;
            }
            return null;
        }
    }
}
