using System.Collections;
using UnityEngine;
using TMPro;

namespace EcoVillage.UI
{
    /// <summary>
    /// Hiển thị màn "HOÀN THÀNH CHƯƠNG I" khi nhiệm vụ cuối của chương được hoàn thành.
    ///
    /// Cách hoạt động:
    ///   - Lắng nghe sự kiện onQuestCompleted của QuestManager.
    ///   - Khi quest có ID = finalQuestID hoàn thành -> bật panel + (tùy chọn) phát âm thanh.
    ///
    /// Cách gắn trong Unity:
    ///   1. Tạo 1 Panel full màn hình (mặc định TẮT) chứa dòng chữ lớn "HOÀN THÀNH CHƯƠNG I".
    ///   2. Gắn script này lên panel đó (hoặc 1 GameObject quản lý UI).
    ///   3. Kéo panel vào ô "Complete Panel", kéo Text vào "Title Text".
    ///   4. Điền finalQuestID = "Quest_RestoreWell_Ch1" (ID nhiệm vụ khôi phục giếng).
    /// </summary>
    public class ChapterCompleteUI : MonoBehaviour
    {
        [Header("Điều Kiện Kích Hoạt")]
        [Tooltip("ID của nhiệm vụ CUỐI chương. Khi quest này xong thì hiện màn hoàn thành chương.")]
        [SerializeField] private string finalQuestID = "Quest_RestoreWell_Ch1";

        [Header("UI References")]
        [Tooltip("Panel full màn hình hiện ra khi hoàn thành chương (để TẮT sẵn trong Hierarchy).")]
        [SerializeField] private GameObject completePanel;

        [Tooltip("Dòng chữ tiêu đề lớn (TextMeshPro).")]
        [SerializeField] private TextMeshProUGUI titleText;

        [Tooltip("Nội dung chữ tiêu đề.")]
        [SerializeField] private string titleMessage = "HOÀN THÀNH CHƯƠNG I";

        [Header("Tùy Chọn")]
        [Tooltip("Âm thanh chiến thắng (kéo 1 AudioSource vào, để trống nếu chưa có).")]
        [SerializeField] private AudioSource victorySound;

        [Tooltip("Tự ẩn panel sau bao nhiêu giây (0 = không tự ẩn).")]
        [SerializeField] private float autoHideAfter = 5f;

        [Tooltip("Nút MAP (Bản đồ Quy hoạch) sẽ nhấp nháy ở Chương II. Để trống nếu chưa có.")]
        [SerializeField] private GameObject mapButtonToHighlight;

        private bool _shown;

        private void Start()
        {
            if (completePanel != null) completePanel.SetActive(false);

            if (Quest.QuestManager.Instance != null)
            {
                Quest.QuestManager.Instance.onQuestCompleted.AddListener(OnAnyQuestCompleted);
            }
            else
            {
                Debug.LogWarning("[ChapterCompleteUI] Chưa tìm thấy QuestManager khi Start.");
            }
        }

        private void OnDestroy()
        {
            if (Quest.QuestManager.Instance != null)
            {
                Quest.QuestManager.Instance.onQuestCompleted.RemoveListener(OnAnyQuestCompleted);
            }
        }

        private void OnAnyQuestCompleted(string questName)
        {
            if (_shown) return;
            if (Quest.QuestManager.Instance == null) return;

            // Dùng ID (ổn định) thay vì tên hiển thị để kiểm tra
            if (Quest.QuestManager.Instance.IsQuestCompleted(finalQuestID))
            {
                ShowChapterComplete();
            }
        }

        public void ShowChapterComplete()
        {
            _shown = true;

            if (titleText != null) titleText.text = titleMessage;
            if (completePanel != null) completePanel.SetActive(true);
            if (victorySound != null) victorySound.Play();
            if (mapButtonToHighlight != null) mapButtonToHighlight.SetActive(true);

            Debug.Log("[ChapterCompleteUI] 🏆 HOÀN THÀNH CHƯƠNG I!");

            if (autoHideAfter > 0f)
            {
                StartCoroutine(AutoHideRoutine());
            }
        }

        private IEnumerator AutoHideRoutine()
        {
            yield return new WaitForSeconds(autoHideAfter);
            if (completePanel != null) completePanel.SetActive(false);
        }
    }
}
