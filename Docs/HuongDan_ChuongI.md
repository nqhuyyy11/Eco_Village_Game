# HƯỚNG DẪN SETUP CHƯƠNG I TRONG UNITY
*Dành cho Eco_Village_Game — Thung Lũng Green Valley*

Tài liệu này hướng dẫn bạn **kéo-thả và cấu hình** trong Unity để chạy trọn vẹn Chương I.
Phần code + dữ liệu (.asset) + ảnh placeholder **đã được tạo sẵn**. Việc của bạn là gắn chúng vào scene **MainScene**.

> ⏱️ Thời gian dự kiến: 30–45 phút. Làm tuần tự từ Bước 0 → Bước 7.

---

## 0. TÓM TẮT NHỮNG GÌ ĐÃ TẠO SẴN

### Code đã sửa / thêm
| File | Thay đổi |
|---|---|
| `Assets/Scripts/Dialogue/DialogueManager.cs` | Fix bug: hội thoại rẽ nhánh không còn làm mất quest của hội thoại gốc |
| `Assets/Scripts/Quest/QuestManager.cs` | Phần thưởng quest giờ cộng **vàng + danh tiếng thật** qua ProfileHUD |
| `Assets/Scripts/ProfileHUD.cs` | Thêm `AddReputation()` + biến `currentReputation` |
| `Assets/Scripts/UI/ChapterCompleteUI.cs` | **MỚI** — màn "HOÀN THÀNH CHƯƠNG I" |

### Dữ liệu Chương I (Assets/ScriptableObjects)
| File | Vai trò |
|---|---|
| `Quest/Quest_CleanTrash_Ch1.asset` | Quest 1 (đã có, sửa lại số thưởng) — Dọn 5 rác |
| `Quest/Quest_CollectWood_Ch1.asset` | **MỚI** — Quest 2: 20 Gỗ + sửa Cầu |
| `Quest/Quest_RestoreWell_Ch1.asset` | **MỚI** — Quest 3: 10 Đá + khôi phục Giếng |
| `Dialogue/CuBa_GiveQuest2/3.asset` | **MỚI** — Cụ Bá giao quest 2 & 3 |
| `Dialogue/CuBa_Quest2/3_Complete.asset` | **MỚI** — hội thoại hoàn thành + kết chương |

### Ảnh placeholder (Assets/Sprites/Chapter1)
- `old_well.png` (giếng cổ), `wooden_bridge.png` (cầu gỗ) — *hình khối đơn giản, thay art đẹp sau*.
- Cây / Đá / Rác → **dùng art có sẵn** (xem Bước 3).

### Chuỗi nhiệm vụ đã nối sẵn (tự chạy nối tiếp)
```
Gặp Cụ Bá → [Quest 1] Dọn rác → [Quest 2] Gỗ + Cầu → [Quest 3] Đá + Giếng → HOÀN THÀNH CHƯƠNG I
```

---

## 1. MỞ UNITY & KIỂM TRA

1. Mở project bằng Unity **6000.3.16f1**.
2. Đợi Unity import xong (góc dưới phải hết quay).
3. Mở **Console** (menu `Window ▸ General ▸ Console`). **Phải KHÔNG có lỗi đỏ (error).**
   - Nếu có lỗi liên quan file `.asset`: báo lại, có thể YAML bị lệch.
4. Mở thư mục `Assets/ScriptableObjects/Quest`, bấm vào `Quest_CollectWood_Ch1`.
   Nhìn cửa sổ **Inspector** bên phải: phải thấy đúng "Nhịp Cầu Nối Lại", 2 steps (Wood 20, WoodenBridge 1).
   → Nếu hiện đúng nghĩa là file tôi tạo đã đọc được. ✅

---

## 2. ĐĂNG KÝ 2 QUEST MỚI VÀO QUESTMANAGER ⚠️ (QUAN TRỌNG NHẤT)

> Nếu bỏ qua bước này, quest 2 & 3 sẽ **không bao giờ hoàn thành** (vì QuestManager chỉ theo dõi các quest có trong danh sách của nó).

1. Trong cửa sổ **Project**, mở `Assets/Prefabs/SYSTEM/` và bấm đúp **`QuestManager.prefab`** để mở chế độ chỉnh prefab.
   *(Hoặc nếu trong MainScene đã có sẵn object `QuestManager`, chọn nó trong Hierarchy.)*
2. Ở Inspector, tìm component **Quest Manager (Script)** → mục **All Quests**.
3. Tăng **Size** của list lên cho đủ (hiện có 1 → đổi thành **3**).
4. Kéo 3 file này vào 3 ô (Element 0,1,2):
   - `Quest_CleanTrash_Ch1`
   - `Quest_CollectWood_Ch1`
   - `Quest_RestoreWell_Ch1`
5. Lưu lại (Ctrl+S). Nếu sửa trong prefab thì bấm mũi tên **‹** ở góc trên trái để thoát Prefab Mode.

---

## 3. ĐẶT VẬT THỂ TƯƠNG TÁC TRONG MAINSCENE

Mở scene `Assets/Scenes/MainScene.unity`.

### 3.1. Bảng tra cứu targetID (PHẢI gõ đúng từng chữ, phân biệt hoa/thường)

| Vật thể | Sprite dùng | `Target ID` | `Interaction Type` | Gợi ý số lượng |
|---|---|---|---|---|
| Đống rác | `Sprites/Generated/trash_pile.png` | `Trash` | CollectResource | đặt **5** đống |
| Cây khô | `Sprites/Sunnyside.../Elements/Plants/spr_deco_tree_01_strip4` | `Wood` | CollectResource | **4** cây × 5 gỗ = 20 |
| Cầu gỗ | `Sprites/Chapter1/wooden_bridge.png` | `WoodenBridge` | InteractWithObject | **1** cây cầu |
| Đá/Sỏi | `Sprites/Sunnyside.../Elements/Crops/rock.png` | `Stone` | CollectResource | **5** viên × 2 đá = 10 |
| Giếng cổ | `Sprites/Chapter1/old_well.png` | `OldWell` | InteractWithObject | **1** giếng |

### 3.2. Cấu hình import cho ảnh mới (chỉ làm 1 lần)

Với `old_well.png` và `wooden_bridge.png` (và bất kỳ ảnh nào trong Sunnyside chưa set):
1. Bấm vào file ảnh trong Project.
2. Ở Inspector đặt:
   - **Texture Type** = `Sprite (2D and UI)`
   - **Sprite Mode** = `Single`
   - **Pixels Per Unit** = `32` (chỉnh sao cho vật thể vừa với map)
   - **Filter Mode** = `Point (no filter)`  ← cho nét pixel-art
   - **Compression** = `None`
3. Bấm **Apply**.

> Ảnh cây/đá trong Sunnyside có dạng *strip* (nhiều khung). Tạm thời cứ để `Sprite Mode = Single` dùng cả dải cũng được; muốn cắt 1 khung thì dùng **Sprite Editor ▸ Slice ▸ Grid By Cell Count**.

### 3.3. Tạo 1 vật thể tương tác (làm mẫu cho "Cây khô")

1. Kéo sprite cây vào **Hierarchy** (hoặc chuột phải Hierarchy ▸ *2D Object ▸ Sprite*).
2. Đổi tên thành `DryTree_1`.
3. Bấm **Add Component** → gõ `Collider 2D` → chọn **Box Collider 2D**.
   - Tick ô **Is Trigger** ✅ (để người chơi tới gần kích hoạt được).
   - Chỉnh **Size** collider to hơn sprite một chút (vùng "tới gần").
4. Bấm **Add Component** → gõ `Interactable Object` → chọn script **InteractableObject**.
5. Điền vào component InteractableObject:
   | Trường | Giá trị |
   |---|---|
   | Target ID | `Wood` |
   | Interaction Type | `CollectResource` |
   | Amount Per Interaction | `5` |
   | Resource Drop Type | `Wood` (tuỳ chọn) |
   | Destroy On Complete | ✅ (chặt xong cây biến mất) |
   | Interactions Required | `1` |
6. **Player phải có Tag = `Player`** và có Collider2D + Rigidbody2D (thường đã có sẵn).
7. Nhân bản cây này (Ctrl+D) ra **4 cây**, rải quanh map.

### 3.4. Làm tương tự cho các vật thể còn lại

- **Đống rác** (×5): Target ID = `Trash`, Interaction Type = `CollectResource`, Amount = 1, Destroy On Complete ✅.
  → Lưu ý: rác đã có sẵn cơ chế tự hiện khi nhận Quest (xem `QuestObjectActivator` trên đống rác cũ — nếu có thì giữ nguyên).
- **Đá** (×5): Target ID = `Stone`, CollectResource, Amount = 2, Destroy ✅.
- **Cầu gỗ** (×1): Target ID = `WoodenBridge`, **InteractWithObject**, Interactions Required = 1. (xem mục 3.5 để cầu "sửa xong" đẹp hơn)
- **Giếng cổ** (×1): Target ID = `OldWell`, **InteractWithObject**, Interactions Required = 1.

### 3.5. (Tuỳ chọn — đẹp hơn) Hiệu ứng "hỏng → sửa xong" cho Cầu & Giếng

Mặc định khi `Destroy On Complete = false`, vật thể sẽ **bị ẩn** sau khi tương tác — không hợp cho cầu/giếng (ta muốn nó hiện ra phiên bản đã sửa). Cách làm mượt:

1. Tạo 2 object: `Bridge_Broken` (gắn InteractableObject, **Destroy On Complete = ✅**) và `Bridge_Fixed` (chỉ là Sprite, **để TẮT sẵn** — bỏ tick ô active ở góc trên Inspector).
2. Đặt `Bridge_Fixed` **KHÔNG phải con** của `Bridge_Broken` (để khỏi bị xoá theo).
3. Ở `Bridge_Broken` → InteractableObject → ô **Interact Effect**: kéo `Bridge_Fixed` vào.
   → Khi sửa: `Bridge_Fixed` bật lên, `Bridge_Broken` biến mất. Làm y hệt cho Giếng.

*(Nếu muốn nhanh, bỏ qua mục này, để Destroy On Complete = ✅ cho cầu/giếng — vật thể sẽ biến mất khi hoàn thành; chấp nhận được cho bản demo.)*

---

## 4. KIỂM TRA PROFILEHUD (để nhận thưởng vàng)

1. Trong MainScene, tìm object có script **ProfileHUD** (thường nằm trên Canvas HUD).
2. Đảm bảo các ô **Coins Text**, **Player Name Text**... đã được kéo Text vào (nếu trống thì gán).
3. Không cần làm gì thêm — khi hoàn thành quest, **QuestManager sẽ tự tìm ProfileHUD** và cộng vàng/danh tiếng.
   - Số vàng sẽ nhảy lên ở góc HUD; danh tiếng được cộng ngầm (chưa có thanh hiển thị — sẽ làm ở chương sau).

---

## 5. TẠO MÀN "HOÀN THÀNH CHƯƠNG I"

1. Trong Hierarchy, chuột phải vào **Canvas** ▸ *UI ▸ Panel*. Đổi tên `Panel_ChapterComplete`.
2. Cho Panel màu nền tối mờ (Image ▸ Color ▸ giảm Alpha ~200).
3. Chuột phải Panel ▸ *UI ▸ Text - TextMeshPro*. Đổi tên `Text_Title`.
   - Gõ chữ to giữa màn hình, cỡ chữ ~80, canh giữa.
4. **Tắt** `Panel_ChapterComplete` đi (bỏ tick active góc trên trái Inspector) — để nó ẩn lúc đầu.
5. Tạo 1 GameObject trống: chuột phải Hierarchy ▸ *Create Empty*, đặt tên `ChapterManager`.
6. Chọn `ChapterManager` → **Add Component** → `Chapter Complete UI`.
7. Điền:
   | Trường | Giá trị |
   |---|---|
   | Final Quest ID | `Quest_RestoreWell_Ch1` |
   | Complete Panel | kéo `Panel_ChapterComplete` vào |
   | Title Text | kéo `Text_Title` vào |
   | Title Message | `HOÀN THÀNH CHƯƠNG I` |
   | Victory Sound | (để trống, hoặc kéo 1 AudioSource) |
   | Auto Hide After | `5` |
   | Map Button To Highlight | (để trống — dành cho Chương II) |

→ Khi khôi phục giếng xong, panel này tự bật lên.

---

## 6. KIỂM TRA HỘI THOẠI CỤ BÁ (test cả 2 nhánh)

Cụ Bá (`Assets/Prefabs/NPC/NPC_CuBa.prefab`) đã có sẵn hội thoại gặp lần đầu + phân nhánh A/B.
Nhờ bản fix DialogueManager, dù người chơi chọn nhánh nào thì quest vẫn được giao.

**Hãy test riêng việc này trước:** vào Play, lại gần Cụ Bá nhấn **E**, chọn **lựa chọn A** → kiểm tra Quest 1 có hiện trên bảng nhiệm vụ không. Chơi lại, chọn **lựa chọn B** → cũng phải nhận được Quest 1.

---

## 7. BẬT UNITY SMART MERGE CHO GIT (chống hỏng scene khi merge)

File `.gitattributes` đã được thêm. Để nó hoạt động, **mỗi thành viên** chạy 1 lần trong terminal tại thư mục project:

```bash
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver '"C:/Program Files/Unity/Hub/Editor/6000.3.16f1/Editor/Data/Tools/UnityYAMLMerge.exe" merge -p %O %B %A %A'
```
*(Sửa lại đường dẫn cho đúng nơi cài Unity của bạn.)*

Từ giờ khi merge bị đụng scene/prefab, Git sẽ tự dùng công cụ của Unity để gộp thay vì làm hỏng file.

---

## ✅ CHECKLIST TEST TOÀN CHƯƠNG

Vào **Play** và đi hết mạch:

- [ ] Gặp Cụ Bá (E) → nhận **Quest 1: Dọn rác**.
- [ ] Dọn đủ 5 đống rác → hiện hội thoại cảm ơn → **tự chuyển sang Quest 2**.
- [ ] Chặt cây đủ **20 Gỗ** → sửa **Cầu** → hội thoại → **tự sang Quest 3**.
- [ ] Gom đủ **10 Đá** → khôi phục **Giếng** → hội thoại trao cuộn giấy + giới thiệu Lâm & Mai Vy.
- [ ] Hiện màn **"HOÀN THÀNH CHƯƠNG I"**.
- [ ] Số **vàng** ở HUD tăng sau mỗi quest (100 → 150 → 200).

---

## 📌 GHI CHÚ KHI MUỐN NÂNG CẤP SAU

- **Thay art đẹp:** đè file vào `Assets/Sprites/Chapter1/` (giữ nguyên tên) là tự cập nhật.
- **Đổi lời thoại:** mở file trong `Assets/ScriptableObjects/Dialogue/`, sửa ô `Dialogue Text` trong Inspector.
- **Đổi số lượng/ thưởng:** mở file trong `Assets/ScriptableObjects/Quest/`, sửa `Required Amount` / `Reward`.
- **Thêm chân dung Cụ Bá:** trong mỗi dòng thoại có ô `Speaker Portrait` — kéo ảnh portrait (có sẵn trong `Sprites/WebPrototype/portrait_village_chief_*`) vào.

---
*Mọi đường link tham chiếu giữa quest ↔ hội thoại đã được nối sẵn và kiểm tra khớp GUID. Bạn chỉ cần làm phần kéo-thả ở trên.*
