using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace EcoVillage.Quest
{
    public class QuestLogUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Text hiển thị danh sách nhiệm vụ trên màn hình")]
        [SerializeField] private TextMeshProUGUI questLogText;

        private void Start()
        {
            // Đăng ký lắng nghe các sự kiện từ QuestManager
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.onQuestStarted.AddListener(UpdateQuestLogUI);
                QuestManager.Instance.onQuestCompleted.AddListener(UpdateQuestLogUI);
                QuestManager.Instance.onQuestProgressUpdated.AddListener(UpdateQuestLogUIString);
            }
            
            UpdateQuestLogUIString();
        }

        private void OnDestroy()
        {
            // Hủy đăng ký để tránh lỗi khi chuyển scene
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.onQuestStarted.RemoveListener(UpdateQuestLogUI);
                QuestManager.Instance.onQuestCompleted.RemoveListener(UpdateQuestLogUI);
                QuestManager.Instance.onQuestProgressUpdated.RemoveListener(UpdateQuestLogUIString);
            }
        }

        private void UpdateQuestLogUI(string questName)
        {
            UpdateQuestLogUIString();
        }

        public void UpdateQuestLogUIString()
        {
            if (questLogText == null || QuestManager.Instance == null) return;

            List<QuestData> activeQuests = QuestManager.Instance.GetActiveQuests();
            
            if (activeQuests.Count == 0)
            {
                questLogText.text = "Không có nhiệm vụ nào.";
                return;
            }

            string displayText = "<color=yellow><b>NHIỆM VỤ ĐANG LÀM:</b></color>\n";

            foreach (QuestData quest in activeQuests)
            {
                displayText += $"\n<b>{quest.questName}</b>\n";
                
                // Tìm bước nhiệm vụ đang làm hiện tại
                QuestStep currentStep = quest.GetCurrentActiveStep();
                if (currentStep != null)
                {
                    displayText += $"- {currentStep.GetFullDescription()}";
                }
            }

            questLogText.text = displayText;
        }
    }
}
