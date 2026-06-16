using UnityEngine;
using TMPro;
using System.Collections;

namespace EcoVillage.UI
{
    /// <summary>
    /// Hiển thị thông báo nổi (Pop-up Toast) trên màn hình.
    /// Dùng cho Quest, Nhặt đồ, Cảnh báo...
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        [Header("UI References")]
        [Tooltip("Kéo chữ TextMeshPro vào đây")]
        [SerializeField] private TextMeshProUGUI notificationText;

        [Header("Target Tracking")]
        [Tooltip("Kéo nhân vật chính (GameObject_MainPlayer) vào đây. Nếu để trống sẽ tự tìm bằng Tag.")]
        [SerializeField] private Transform playerTransform;

        [Header("Settings")]
        [Tooltip("Độ cao so với chân nhân vật (Mặc định: 1.5)")]
        [SerializeField] private float heightOffset = 1.5f;

        [Tooltip("Thời gian hiển thị rõ (giây)")]
        [SerializeField] private float displayTime = 1.5f;
        
        [Tooltip("Thời gian mờ dần (giây)")]
        [SerializeField] private float fadeTime = 0.5f;

        private Coroutine _fadeCoroutine;
        private Camera _mainCamera;
        private Vector3 _currentOffset;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _mainCamera = Camera.main;
            if (_mainCamera == null) _mainCamera = FindFirstObjectByType<Camera>();

            // Ẩn chữ lúc đầu bằng cách chỉnh Alpha (Độ trong suốt) = 0
            if (notificationText != null)
            {
                Color c = notificationText.color;
                c.a = 0f;
                notificationText.color = c;
            }
        }

        private void Start()
        {
            // Tự động tìm nhân vật nếu chưa được kéo vào Inspector
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }
        }

        private void Update()
        {
            // Nếu đang hiện chữ và tìm thấy Player, thì cho chữ bám theo trên đầu Player
            if (playerTransform != null && notificationText != null && notificationText.color.a > 0)
            {
                Vector3 worldPosition = playerTransform.position + _currentOffset;
                Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);
                
                notificationText.rectTransform.position = screenPosition;
            }
        }

        /// <summary>
        /// Gọi hàm này từ bất kỳ đâu: NotificationManager.Instance.ShowNotification("Nội dung");
        /// </summary>
        public void ShowNotification(string message)
        {
            if (notificationText == null) return;

            notificationText.text = message;

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeInOutRoutine());
        }

        private IEnumerator FadeInOutRoutine()
        {
            _currentOffset = new Vector3(0, heightOffset, 0);

            // 1. HIỆU ỨNG POP-UP (Phóng to nảy lên)
            Color c = notificationText.color;
            c.a = 1f;
            notificationText.color = c;

            float popTime = 0.2f;
            float t = 0;
            while (t < popTime)
            {
                t += Time.deltaTime;
                float scale = Mathf.Lerp(0.5f, 1.2f, t / popTime); // Nảy to hơn bình thường 1 chút
                notificationText.rectTransform.localScale = Vector3.one * scale;
                yield return null;
            }
            notificationText.rectTransform.localScale = Vector3.one; // Trả về cỡ chuẩn

            // 2. CHỜ VÀ BAY LÊN TỪ TỪ
            float timer = 0;
            while (timer < displayTime)
            {
                timer += Time.deltaTime;
                _currentOffset.y += Time.deltaTime * 0.7f; // Bay lên
                yield return null;
            }

            // 3. MỜ DẦN ĐI
            float fadeTimer = 0;
            while (fadeTimer < fadeTime)
            {
                fadeTimer += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, fadeTimer / fadeTime);
                notificationText.color = c;
                _currentOffset.y += Time.deltaTime * 0.7f; // Vẫn tiếp tục bay lên
                yield return null;
            }

            // Tắt hẳn
            c.a = 0f;
            notificationText.color = c;
        }
    }
}
