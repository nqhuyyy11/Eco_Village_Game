using UnityEngine;

namespace EcoVillage.Dialogue
{
    /// <summary>
    /// Đại diện cho MỘT dòng hội thoại đơn trong game.
    /// Đây là đơn vị nhỏ nhất của hệ thống hội thoại.
    /// </summary>
    [System.Serializable]
    public class DialogueLine
    {
        [Header("Người Nói")]
        [Tooltip("Tên hiển thị trên hộp thoại. Ví dụ: 'Cụ Bá', 'Người Chơi', 'Narrator'")]
        public string speakerName;

        [Tooltip("Ảnh chân dung (portrait) của người nói. Kéo sprite vào đây.")]
        public Sprite speakerPortrait;

        [Header("Nội Dung")]
        [Tooltip("Nội dung lời thoại. Hỗ trợ xuống dòng bằng \\n.")]
        [TextArea(2, 6)]
        public string dialogueText;

        [Header("Tùy Chọn Phân Nhánh")]
        [Tooltip("Bật lên nếu dòng này là lựa chọn của người chơi (branching).")]
        public bool isPlayerChoice = false;

        [Header("Sự Kiện Kích Hoạt")]
        [Tooltip("Tên sự kiện Unity gọi sau khi hiển thị dòng này. Để trống nếu không có.")]
        public string triggerEventName = "";
    }
}
