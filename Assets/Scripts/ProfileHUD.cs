using UnityEngine;
using UnityEngine.UI;
using TMPro; // Dùng cho TextMeshPro

public class ProfileHUD : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerLevelText;
    public TextMeshProUGUI coinsText;
    
    [Header("Energy Bar")]
    public Image energyBarFill; // Kéo thả ảnh thanh màu vàng vào đây
    public TextMeshProUGUI energyPercentageText;

    [Header("Quest Banner")]
    public GameObject questBanner; // Bật/tắt nguyên cụm thông báo nhiệm vụ
    public TextMeshProUGUI questNameText;

    [Header("Player Stats (Demo)")]
    public string playerName = "Elara";
    public int currentLevel = 12;
    public int currentCoins = 4250;
    public float maxEnergy = 100f;
    public float currentEnergy = 88f;

    [Header("Energy Regeneration")]
    public float energyRegenRate = 0.5f; // Hồi 0.5 năng lượng mỗi giây

    void Start()
    {
        // Cập nhật giao diện lần đầu khi mới chạy game
        UpdateProfileUI();
        UpdateEnergyUI();
    }

    void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
            
            UpdateEnergyUI();
        }
    }

    // Hàm cập nhật toàn bộ Profile
    public void UpdateProfileUI()
    {
        if (playerNameText != null) playerNameText.text = playerName;
        if (playerLevelText != null) playerLevelText.text = "Level " + currentLevel;
        if (coinsText != null) coinsText.text = currentCoins.ToString("N0") + "g";
    }

    // Hàm cập nhật riêng thanh Năng lượng
    public void UpdateEnergyUI()
    {
        if (energyBarFill != null)
        {
            // Tính phần trăm (0.0 đến 1.0) cho Fill Amount của Image
            float fillAmount = currentEnergy / maxEnergy;
            energyBarFill.fillAmount = fillAmount;
        }

        if (energyPercentageText != null)
        {
            // Hiển thị số %
            int percentage = Mathf.RoundToInt((currentEnergy / maxEnergy) * 100);
            energyPercentageText.text = percentage + "%";
        }
    }

    // ==========================================
    // CÁC HÀM TƯƠNG TÁC (GỌI TỪ NÚT BẤM HOẶC SỰ KIỆN)
    // ==========================================

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateProfileUI();
        Debug.Log("Đã cộng " + amount + " vàng!");
    }

    public void SpendEnergy(float amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0) currentEnergy = 0;
        
        UpdateEnergyUI();
        Debug.Log("Đã tiêu hao " + amount + " năng lượng!");
    }

    public void ShowNewQuest(string questName)
    {
        if (questBanner != null && questNameText != null)
        {
            questNameText.text = questName;
            questBanner.SetActive(true); // Bật popup nhiệm vụ lên
        }
    }

    public void HideQuestBanner()
    {
        if (questBanner != null)
        {
            questBanner.SetActive(false); // Tắt popup
        }
    }

    // Hàm test nhanh (Bạn có thể gắn hàm này vào các Nút tàng hình để bấm thử)
    public void Test_DoWork()
    {
        SpendEnergy(10f); // Tốn 10 năng lượng
        AddCoins(50);     // Kiếm được 50 vàng
    }
}
