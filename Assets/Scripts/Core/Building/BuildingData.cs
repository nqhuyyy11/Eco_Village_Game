using UnityEngine;

namespace EcoVillage.Core.Building
{
    [CreateAssetMenu(fileName = "NewBuildingData", menuName = "EcoVillage/Building Data")]
    public class BuildingData : ScriptableObject
    {
        [Header("Thông tin cơ bản (Basic Info)")]
        [Tooltip("ID duy nhất của công trình, viết liền không dấu, ví dụ: farm, market")]
        public string buildingID;
        
        [Tooltip("Tên hiển thị trên giao diện game")]
        public string displayName;
        
        [TextArea(2, 4)]
        [Tooltip("Mô tả công dụng của công trình")]
        public string description;

        [Header("Hình ảnh & Prefab (Visuals)")]
        [Tooltip("Prefab thực thể của công trình để xuất hiện trong thế giới game")]
        public GameObject prefab;
        
        [Tooltip("Ảnh hiển thị trong cửa hàng mua sắm")]
        public Sprite previewSprite;
        
        [Tooltip("Kích thước ô chiếm dụng trên lưới (Rộng x Cao), ví dụ: 2x2, 3x3")]
        public Vector2Int gridSize = new Vector2Int(2, 2);

        [Header("Chi phí xây dựng (Costs)")]
        [Tooltip("Số tiền vàng cần để xây")]
        public int goldCost;
        
        [Tooltip("Số gỗ cần để xây")]
        public int woodCost;
        
        [Tooltip("Số đá cần để xây")]
        public int stoneCost;

        [Header("Tiến trình game (Progression)")]
        [Tooltip("Số điểm Danh tiếng (Reputation) nhận được khi hoàn thành công trình này")]
        public int reputationPoints;
        
        [Tooltip("Có phải công trình bắt buộc phải xây dựng để chiến thắng không?")]
        public bool isMandatoryForVictory;
    }
}
