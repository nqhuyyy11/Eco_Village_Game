using UnityEngine;
using System.Collections.Generic;

namespace EcoVillage.Core.Building
{
    /// <summary>
    /// Script gắn vào Prefab của mỗi công trình thực tế trong game.
    /// Quản lý vị trí, va chạm và thông tin của công trình đó.
    /// </summary>
    public class Building : MonoBehaviour
    {
        [Header("Cấu hình công trình")]
        [Tooltip("Dữ liệu cấu hình được gán tự động hoặc thủ công từ ScriptableObject")]
        public BuildingData data;

        [Header("Trạng thái hiện tại")]
        [Tooltip("Công trình đã được đặt xuống đất chính thức chưa (hay đang ở chế độ xem trước/ghost)?")]
        private bool isPlaced = false;

        [Tooltip("Tọa độ gốc (ô dưới bên trái) của công trình trên lưới")]
        private Vector2Int originGridPosition;

        /// <summary>
        /// Khởi tạo các thông số cho công trình sau khi đặt thành công.
        /// </summary>
        public void Place(Vector2Int gridPos)
        {
            originGridPosition = gridPos;
            isPlaced = true;
            
            // Bật va chạm vật lý thực tế khi đã đặt xuống
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = false; // Chuyển từ cảm biến (Ghost) thành vật thể cứng (Solid) để nhân vật không đi xuyên qua
            }

            Debug.Log($"Đã đặt thành công công trình {data.displayName} tại tọa độ lưới {gridPos}");
        }

        /// <summary>
        /// Trả về danh sách tất cả các ô grid mà công trình này đang chiếm giữ.
        /// Rất hữu ích để kiểm tra xem ô grid có bị trùng lặp khi đặt nhà mới không.
        /// </summary>
        public List<Vector2Int> GetOccupiedCells()
        {
            var occupied = new List<Vector2Int>();
            if (data == null) return occupied;

            for (int x = 0; x < data.gridSize.x; x++)
            {
                for (int y = 0; y < data.gridSize.y; y++)
                {
                    occupied.Add(new Vector2Int(originGridPosition.x + x, originGridPosition.y + y));
                }
            }
            return occupied;
        }

        public bool IsPlaced()
        {
            return isPlaced;
        }

        public Vector2Int GetOriginPosition()
        {
            return originGridPosition;
        }
    }
}
