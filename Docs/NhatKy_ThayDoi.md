# NHẬT KÝ THAY ĐỔI — Hoàn thiện Chương I + Sửa rủi ro
*Dự án: Eco_Village_Game (Thung Lũng Green Valley) — Nhánh: `ĐứcAnh`*

Tài liệu này tóm tắt **toàn bộ những gì đã được làm/sửa**. Phần hướng dẫn ráp vào Unity nằm ở [HuongDan_ChuongI.md](HuongDan_ChuongI.md).

| Commit | Nội dung |
|---|---|
| `a65aa764` | Hoàn thiện Chương I + sửa rủi ro |
| `4e33b6a6` | Merge nhánh David (collider chặn sông/núi) |

---

## 1. PHÂN TÍCH RỦI RO (đã rà soát toàn dự án)

Những rủi ro lớn được phát hiện và **đã xử lý** trong phạm vi Chương I:
- Bug hội thoại phân nhánh làm **mất nhiệm vụ**.
- Phần thưởng quest chỉ là `Debug.Log`, **không cộng vàng thật**.
- Thiếu `.gitattributes` → **dễ hỏng scene khi nhóm merge**.
- Build Settings có **GUID giả** cho MenuScene.
- File `.meta` mồ côi gây cảnh báo.

Rủi ro **để lại cho giai đoạn sau** (không thuộc Chương I): Inventory thật, Save/Load, hệ thống Building/Bản đồ quy hoạch (Chương II+), dọn asset thừa (~38 MB), license asset pack.

---

## 2. LỖI ĐÃ SỬA (CODE)

| File | Thay đổi | Vì sao |
|---|---|---|
| `Assets/Scripts/Dialogue/DialogueManager.cs` | Thêm cơ chế nhớ hội thoại gốc (`_pendingRootDialogue`); khi rẽ nhánh xong vẫn chạy `questToTrigger`/`nextDialogue` của hội thoại gốc | Trước đây chọn nhánh A/B làm **mất quest** vì `_currentDialogue` bị ghi đè |
| `Assets/Scripts/Quest/QuestManager.cs` | `GiveReward()` giờ tự tìm `ProfileHUD` và cộng **vàng + danh tiếng thật** | Trước đây phần thưởng chỉ ghi log, không có tác dụng |
| `Assets/Scripts/ProfileHUD.cs` | Thêm biến `currentReputation` + hàm `AddReputation()` | Để nhận điểm Danh Tiếng từ quest |
| `Assets/Scripts/UI/ChapterCompleteUI.cs` | **File MỚI** — hiện màn "HOÀN THÀNH CHƯƠNG I" khi quest cuối xong | Chương I cần màn kết chương |

---

## 3. NỘI DUNG CHƯƠNG I ĐÃ THÊM (DỮ LIỆU)

### Quest (Assets/ScriptableObjects/Quest)
| File | Nội dung |
|---|---|
| `Quest_CleanTrash_Ch1.asset` | *(sửa)* Quest 1 — Dọn 5 rác. Sửa mô tả thưởng `+50` → `+100 Vàng` cho khớp |
| `Quest_CollectWood_Ch1.asset` | **MỚI** Quest 2 — Thu 20 Gỗ + sửa Cầu (2 bước). Thưởng 150 vàng |
| `Quest_RestoreWell_Ch1.asset` | **MỚI** Quest 3 — Thu 10 Đá + khôi phục Giếng (2 bước). Thưởng 200 vàng |

### Hội thoại Cụ Bá (Assets/ScriptableObjects/Dialogue)
| File | Nội dung |
|---|---|
| `CuBa_GiveQuest2.asset` | **MỚI** Cụ Bá giao nhiệm vụ Gỗ/Cầu |
| `CuBa_Quest2_Complete.asset` | **MỚI** Hoàn thành Cầu → dẫn sang Quest 3 |
| `CuBa_GiveQuest3.asset` | **MỚI** Cụ Bá giao nhiệm vụ Đá/Giếng |
| `CuBa_Quest3_Complete.asset` | **MỚI** Kết chương: trao cuộn giấy + giới thiệu Lâm & Mai Vy |
| `CuBa_CleanTrash_Complete.asset` | *(sửa)* Nối tiếp sang `CuBa_GiveQuest2` |

### Chuỗi đã nối sẵn (đã kiểm tra khớp GUID 100%)
```
Gặp Cụ Bá → [Q1] Dọn rác → [Q2] Gỗ + Cầu → [Q3] Đá + Giếng → HOÀN THÀNH CHƯƠNG I
```
Cơ chế: hội thoại hoàn thành quest → `nextDialogue` → hội thoại giao quest kế → `questToTrigger` → quest kế. Không dùng auto `nextQuest` để tránh hội thoại bị chèn đè nhau.

---

## 4. ẢNH (dùng tối đa art có sẵn)

| Vật thể | Nguồn |
|---|---|
| Rác | `Sprites/Generated/trash_pile.png` *(có sẵn)* |
| Cây khô | Sunnyside pack — `Elements/Plants/spr_deco_tree_*` *(có sẵn)* |
| Đá | Sunnyside pack — `Elements/Crops/rock.png` *(có sẵn)* |
| Giếng cổ | `Sprites/Chapter1/old_well.png` **(MỚI — placeholder)** |
| Cầu gỗ | `Sprites/Chapter1/wooden_bridge.png` **(MỚI — placeholder)** |

Placeholder là hình khối đơn giản; thay art đẹp sau bằng cách đè file cùng tên.

---

## 5. SỬA RỦI RO REPO / CẤU HÌNH

| Thay đổi | Chi tiết |
|---|---|
| Thêm `.gitattributes` | Ép các file Unity (.unity/.prefab/.asset/.meta) ở dạng text + khai báo Unity Smart Merge; đánh dấu ảnh/âm thanh là binary |
| Sửa `ProjectSettings/EditorBuildSettings.asset` | Thay GUID giả `000…0001` của MenuScene bằng GUID thật `4e91c8bc…` |
| Xoá `Assets/Scripts/Core/Movement.meta` | File `.meta` mồ côi (thư mục gốc đã bị xoá) |

---

## 6. MERGE NHÁNH DAVID (commit `4e33b6a6`)

- **Lấy về 4 vùng va chạm** chặn địa hình: `Vung_can_song`, `Vung_can_song2`, `Vung_can_song3` (EdgeCollider2D) + `Vung_Cản_Nui_1` (PolygonCollider2D).
- **Lưu ý:** `MenuScene` (bảng PLAY/SETTING/EXIT) **giống hệt** trên cả hai nhánh — không có gì mới từ David.
- **Xung đột đã xử lý:** 7 vùng trong `MainScene` + cache font TMP, đều là vị trí/kích thước UI & sprite mà cả hai nhánh cùng chỉnh. → **Giữ bố cục của `ĐứcAnh`**, vẫn giữ nguyên collider của David.
- ⚠️ **Cảnh báo quan trọng:** UnityYAMLMerge khi **thiếu tool fallback** đã âm thầm làm **mất collider của David** ở lần thử đầu. Đã phát hiện và merge lại an toàn bằng text-merge + giải quyết tay. Xem mục cập nhật ở [HuongDan_ChuongI.md §7](HuongDan_ChuongI.md).

---

## 7. TRẠNG THÁI GIT

- Đã commit & **push lên `origin/ĐứcAnh`** thành công (2 commit ở trên).
- ⚠️ Khi mở Unity lần đầu, nó sẽ tự sinh `.meta` cho `ChapterCompleteUI.cs` và 2 ảnh trong `Sprites/Chapter1/` → **nhớ commit + push các `.meta` này** để cả nhóm dùng chung GUID.

---

## 8. VIỆC CÒN LẠI CỦA BẠN (bên Unity)

Toàn bộ phần kéo-thả/cấu hình trong Unity → làm theo **[HuongDan_ChuongI.md](HuongDan_ChuongI.md)**.
Tóm tắt nhanh:
1. Mở Unity, kiểm tra Console sạch lỗi.
2. **Đăng ký 3 quest vào QuestManager** (quan trọng nhất).
3. Đặt vật thể tương tác (rác/cây/đá/cầu/giếng) vào MainScene.
4. Tạo màn "HOÀN THÀNH CHƯƠNG I".
5. Kiểm tra 4 collider chặn sông/núi (từ David) nằm đúng chỗ.
6. Chạy thử toàn chương theo checklist.
