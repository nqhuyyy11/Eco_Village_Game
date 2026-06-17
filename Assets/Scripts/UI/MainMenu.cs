using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoVillageGame.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Panels")]
        [Tooltip("Kéo Panel Settings vào đây")]
        public GameObject settingsPanel;

        [Header("Scene Settings")]
        [Tooltip("Tên Scene của Game chính để chuyển sang khi bấm Play")]
        public string gameSceneName = "MainScene";

        private void Start()
        {
            // Đảm bảo ẩn màn hình Cài đặt khi mới bắt đầu
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
            
            // Vì đây là Scene Menu riêng, thời gian chạy bình thường để UI Animation hoạt động
            Time.timeScale = 1f; 
        }

        /// <summary>
        /// Gọi hàm này khi nhấn nút Play
        /// </summary>
        public void PlayGame()
        {
            // Chuyển cảnh sang Game chính
            SceneManager.LoadScene(gameSceneName);
        }

        /// <summary>
        /// Gọi hàm này khi nhấn nút Settings
        /// </summary>
        public void OpenSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Gọi hàm này khi nhấn nút Close trong màn hình Settings
        /// </summary>
        public void CloseSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Gọi hàm này khi nhấn nút Quit
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Đang thoát game...");
            Application.Quit();
        }
    }
}
