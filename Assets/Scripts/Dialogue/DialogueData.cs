using UnityEngine;

namespace EcoVillage.Dialogue
{
    /// <summary>
    /// ScriptableObject chứa toàn bộ dữ liệu của MỘT cuộc hội thoại.
    /// Tạo file mới bằng cách: Right Click → Create → EcoVillage → Dialogue → Dialogue Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "EcoVillage/Dialogue/Dialogue Data", order = 1)]
    public class DialogueData : ScriptableObject
    {
        [Header("Thông Tin Cuộc Hội Thoại")]
        [Tooltip("Tên định danh cuộc hội thoại này (dùng trong code). Ví dụ: 'CuBa_FirstMeet'")]
        public string dialogueID;

        [Tooltip("Danh sách các dòng hội thoại theo thứ tự")]
        public DialogueLine[] lines;

        [Header("Phân Nhánh Lựa Chọn (Branching)")]
        [Tooltip("Nếu có lựa chọn phân nhánh, điền DialogueData cho từng nhánh vào đây.")]
        public DialogueData[] branchDialogues;

        [Header("Sau Khi Kết Thúc")]
        [Tooltip("Hội thoại tiếp theo sẽ bắt đầu ngay sau khi hội thoại này kết thúc (nếu có).")]
        public DialogueData nextDialogue;

        [Tooltip("Nhiệm vụ sẽ được kích hoạt sau khi hội thoại này kết thúc (nếu có).")]
        public EcoVillage.Quest.QuestData questToTrigger;
    }
}
