using UnityEngine;
using EcoVillage.Quest;

namespace EcoVillage.Interaction
{
    /// <summary>
    /// Script này dùng để ẩn các vật thể (ví dụ: rác) lúc mới vào game, 
    /// và tự động bật chúng lên khi người chơi nhận được một Quest cụ thể.
    /// </summary>
    public class QuestObjectActivator : MonoBehaviour
    {
        [Header("Cài đặt")]
        [Tooltip("ID của Quest sẽ kích hoạt vật thể này (VD: Quest_CleanTrash_Ch1)")]
        [SerializeField] private string requiredQuestID;

        [Tooltip("Kéo các đống rác (GameObject) vào mảng này")]
        [SerializeField] private GameObject[] objectsToActivate;

        private void Start()
        {
            // Ban đầu, tắt tất cả các đống rác đi
            SetObjectsActive(false);

            // Kiểm tra xem Quest có đang được làm không (ví dụ trường hợp Load Game)
            if (QuestManager.Instance != null)
            {
                if (QuestManager.Instance.GetQuestStatus(requiredQuestID) == QuestStatus.Active)
                {
                    SetObjectsActive(true);
                    this.enabled = false; // Tắt Update
                }
            }
        }

        private void Update()
        {
            // Liên tục kiểm tra xem Quest đã được chuyển sang Active chưa
            if (QuestManager.Instance != null)
            {
                if (QuestManager.Instance.GetQuestStatus(requiredQuestID) == QuestStatus.Active)
                {
                    // Bật rác lên
                    SetObjectsActive(true);
                    
                    // Sau khi bật xong thì tắt luôn script này đi để khỏi chạy hàm Update nữa cho nhẹ máy
                    this.enabled = false; 
                }
            }
        }

        private void SetObjectsActive(bool isActive)
        {
            foreach (var obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(isActive);
                }
            }
        }
    }
}
