using UnityEngine;

namespace EcoVillage.Quest
{
    /// <summary>
    /// Loại yêu cầu của một bước nhiệm vụ.
    /// </summary>
    public enum QuestStepType
    {
        CollectResource,    // Thu thập tài nguyên (Gỗ, Đá...)
        InteractWithObject, // Tương tác với vật thể (Sửa giếng, sửa cầu...)
        TalkToNPC,          // Nói chuyện với NPC
        ReachReputation,    // Đạt mức Reputation nhất định
        BuildStructure,     // Xây dựng công trình
        EarnGold            // Kiếm đủ số vàng
    }

    /// <summary>
    /// Đại diện cho MỘT bước cụ thể trong một nhiệm vụ (Quest Step).
    /// Ví dụ: "Thu thập 20 Gỗ" là một QuestStep.
    /// </summary>
    [System.Serializable]
    public class QuestStep
    {
        [Header("Thông Tin Bước Nhiệm Vụ")]
        [Tooltip("Mô tả ngắn gọn hiển thị trên Quest Log. Ví dụ: 'Thu thập 20/20 Gỗ'")]
        public string stepDescription;

        [Tooltip("Loại yêu cầu của bước này.")]
        public QuestStepType stepType;

        [Header("Mục Tiêu Số Lượng")]
        [Tooltip("Số lượng cần đạt được. Ví dụ: 20 cho '20 Gỗ', 5 cho '5 đống Rác'.")]
        public int requiredAmount = 1;

        [Tooltip("ID định danh tài nguyên/vật thể/công trình. Ví dụ: 'Wood', 'Stone', 'OldWell'")]
        public string targetID;

        // ─── Runtime State (không serialize) ─────────────────────────────────────
        [System.NonSerialized]
        public int currentAmount = 0;

        [System.NonSerialized]
        public bool isCompleted = false;

        // ─────────────────────────────────────────────────────────────────────────
        // METHODS
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Tăng tiến độ của bước này lên. Gọi khi người chơi dọn rác, chặt cây, v.v.
        /// </summary>
        /// <param name="amount">Số lượng tăng thêm (mặc định 1)</param>
        public void AddProgress(int amount = 1)
        {
            if (isCompleted) return;

            currentAmount = Mathf.Clamp(currentAmount + amount, 0, requiredAmount);
            if (currentAmount >= requiredAmount)
            {
                isCompleted = true;
            }
        }

        /// <summary>
        /// Trả về chuỗi tiến độ dạng "3/5".
        /// </summary>
        public string GetProgressString() => $"{currentAmount}/{requiredAmount}";

        /// <summary>
        /// Trả về chuỗi mô tả đầy đủ: "Dọn rác: 3/5".
        /// </summary>
        public string GetFullDescription() => $"{stepDescription}: {GetProgressString()}";
    }
}
