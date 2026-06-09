#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using EcoVillage.Dialogue;
using EcoVillage.Quest;

namespace EcoVillage.Editor
{
    /// <summary>
    /// Editor Tool tự động thiết lập Chapter 1 Demo.
    /// Menu: Eco Village → Setup Chapter 1 Demo
    /// </summary>
    public static class EcoVillageSetupTool
    {
        // ═══════════════════════════════════════════════════════════════
        // PATHS
        // ═══════════════════════════════════════════════════════════════
        const string DIALOGUE_FOLDER   = "Assets/ScriptableObjects/Dialogue";
        const string QUEST_FOLDER      = "Assets/ScriptableObjects/Quest";
        const string GENERATED_SPRITES = "Assets/Sprites/Generated";
        const string SCENE_PATH        = "Assets/Scenes/Chapter1_Demo.unity";

        const string CUBA_SPRITE_PATH =
            "Assets/Sprites/Characters/spritesheet_village_chief_1780825568455_transparent-removebg-preview.png";
        const string PLAYER_SPRITE_PATH =
            "Assets/Sprites/Characters/spritesheet_main_player_handsome_1780826744798_transparent-removebg-preview.png";

        // ═══════════════════════════════════════════════════════════════
        // UI REFERENCES (truyền giữa các method)
        // ═══════════════════════════════════════════════════════════════
        struct UIRefs
        {
            public GameObject        dialoguePanel;
            public Image             portraitImage;
            public TextMeshProUGUI   speakerNameText;
            public TextMeshProUGUI   dialogueText;
            public GameObject        nextButtonGO;
            public Button            nextButton;
            public GameObject        choicePanelGO;
            public Button[]          choiceButtons;
            public TextMeshProUGUI[] choiceTexts;
        }

        // ═══════════════════════════════════════════════════════════════
        // MENU ITEM – ENTRY POINT
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("Eco Village/Setup Chapter 1 Demo")]
        public static void SetupChapter1()
        {
            bool ok = EditorUtility.DisplayDialog(
                "Eco Village – Setup Chapter 1",
                "Tool này sẽ tự động tạo:\n\n" +
                "• Scene mới: Chapter1_Demo\n" +
                "• Canvas UI hộp thoại đầy đủ\n" +
                "• DialogueManager & QuestManager\n" +
                "• 9 ScriptableObjects hội thoại Cụ Bá (Chương I)\n" +
                "• 1 QuestData \"Lối Về Trong Sạch\"\n" +
                "• Player, NPC Cụ Bá, 5 đống rác\n\n" +
                "Tiếp tục?",
                "Tiếp Tục", "Hủy");
            if (!ok) return;

            try
            {
                Progress("Tạo thư mục...", 0.05f);
                CreateFolders();

                Progress("Tạo Scene mới...", 0.10f);
                var scene = EditorSceneManager.NewScene(
                    NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

                Progress("Tạo sprite đống rác...", 0.15f);
                Sprite trashSprite = GetOrCreateTrashSprite();

                Progress("Tạo dữ liệu hội thoại...", 0.20f);
                var dlg = CreateDialogueAssets();

                Progress("Tạo dữ liệu nhiệm vụ...", 0.35f);
                var qst = CreateQuestAssets();

                Progress("Liên kết dữ liệu...", 0.45f);
                LinkAllAssets(dlg, qst);

                Progress("Tạo UI hộp thoại...", 0.55f);
                UIRefs ui = BuildDialogueUI();

                Progress("Tạo Managers...", 0.65f);
                SetupManagers(ui, qst);

                Progress("Tạo Player...", 0.72f);
                CreatePlayer();

                Progress("Tạo NPC Cụ Bá...", 0.80f);
                CreateNPC_CuBa(dlg);

                Progress("Tạo 5 đống rác...", 0.87f);
                CreateTrashPiles(trashSprite);

                Progress("Thiết lập Camera...", 0.93f);
                SetupCamera();

                Progress("Lưu Scene...", 0.97f);
                EditorSceneManager.SaveScene(scene, SCENE_PATH);
                EditorSceneManager.MarkSceneDirty(scene);

                EditorUtility.ClearProgressBar();
                Debug.Log("[EcoVillage] ✅ Setup Chapter 1 Demo hoàn tất!");

                EditorUtility.DisplayDialog(
                    "✅ Setup Hoàn Tất!",
                    "Chapter 1 Demo đã sẵn sàng!\n\n" +
                    "▶ Nhấn PLAY trong Unity để thử nghiệm:\n" +
                    "   • WASD / Mũi tên để di chuyển\n" +
                    "   • Đi đến gần Cụ Bá (bên phải)\n" +
                    "   • Nhấn [E] để nói chuyện\n" +
                    "   • Sau hội thoại, dọn 5 đống rác\n\n" +
                    "📂 Scene: Assets/Scenes/Chapter1_Demo\n" +
                    "📂 Data:  Assets/ScriptableObjects/",
                    "OK");
            }
            catch (System.Exception ex)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"[EcoVillage] ❌ Lỗi: {ex.Message}\n{ex.StackTrace}");
                EditorUtility.DisplayDialog("Lỗi",
                    $"Có lỗi xảy ra:\n{ex.Message}", "OK");
            }
        }

        public static void SetupChapter1Headless()
        {
            try
            {
                Debug.Log("[EcoVillage] Headless Setup: Creating folders...");
                CreateFolders();

                Debug.Log("[EcoVillage] Headless Setup: Creating new scene...");
                var scene = EditorSceneManager.NewScene(
                    NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

                Debug.Log("[EcoVillage] Headless Setup: Creating trash sprite...");
                Sprite trashSprite = GetOrCreateTrashSprite();

                Debug.Log("[EcoVillage] Headless Setup: Creating dialogue assets...");
                var dlg = CreateDialogueAssets();

                Debug.Log("[EcoVillage] Headless Setup: Creating quest assets...");
                var qst = CreateQuestAssets();

                Debug.Log("[EcoVillage] Headless Setup: Linking assets...");
                LinkAllAssets(dlg, qst);

                Debug.Log("[EcoVillage] Headless Setup: Building UI...");
                UIRefs ui = BuildDialogueUI();

                Debug.Log("[EcoVillage] Headless Setup: Setting up managers...");
                SetupManagers(ui, qst);

                Debug.Log("[EcoVillage] Headless Setup: Creating Player...");
                CreatePlayer();

                Debug.Log("[EcoVillage] Headless Setup: Creating NPC Cụ Bá...");
                CreateNPC_CuBa(dlg);

                Debug.Log("[EcoVillage] Headless Setup: Creating Trash Piles...");
                CreateTrashPiles(trashSprite);

                Debug.Log("[EcoVillage] Headless Setup: Setting up camera...");
                SetupCamera();

                Debug.Log("[EcoVillage] Headless Setup: Saving scene...");
                EditorSceneManager.SaveScene(scene, SCENE_PATH);
                EditorSceneManager.MarkSceneDirty(scene);

                Debug.Log("[EcoVillage] ✅ Headless Setup Chapter 1 Demo hoàn tất!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EcoVillage] ❌ Headless Setup Lỗi: {ex.Message}\n{ex.StackTrace}");
            }
        }

        static void Progress(string msg, float pct)
        {
            EditorUtility.DisplayProgressBar("Eco Village Setup", msg, pct);
        }

        // ═══════════════════════════════════════════════════════════════
        // 1) FOLDERS
        // ═══════════════════════════════════════════════════════════════

        static void CreateFolders()
        {
            EnsureFolder(DIALOGUE_FOLDER);
            EnsureFolder(QUEST_FOLDER);
            EnsureFolder(GENERATED_SPRITES);
            EnsureFolder("Assets/Scenes");
        }

        static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string cur = parts[0]; // "Assets"
            for (int i = 1; i < parts.Length; i++)
            {
                string next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // 2) TRASH SPRITE (sinh lập trình)
        // ═══════════════════════════════════════════════════════════════

        static Sprite GetOrCreateTrashSprite()
        {
            string assetPath = GENERATED_SPRITES + "/trash_pile.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (existing != null) return existing;

            // Tạo texture 32×32 hình đống rác pixel
            int sz = 32;
            Texture2D tex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
            Color brown     = new Color(0.50f, 0.35f, 0.18f);
            Color darkBrown = new Color(0.35f, 0.22f, 0.10f);
            Color leafGreen = new Color(0.30f, 0.50f, 0.22f);

            Color[] px = new Color[sz * sz];
            for (int y = 0; y < sz; y++)
            for (int x = 0; x < sz; x++)
            {
                float dx = x - 15.5f, dy = y - 13f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy * 0.55f);
                if (dist < 13f && y < 26)
                {
                    float n = Mathf.PerlinNoise(x * 0.35f, y * 0.35f);
                    px[y * sz + x] = n > 0.62f ? leafGreen
                                   : n > 0.30f ? brown
                                                : darkBrown;
                }
                else
                    px[y * sz + x] = Color.clear;
            }
            tex.SetPixels(px);
            tex.Apply();

            string diskPath = Path.Combine(Application.dataPath, "..",
                                           assetPath).Replace("\\", "/");
            File.WriteAllBytes(diskPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            var imp = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (imp != null)
            {
                imp.textureType        = TextureImporterType.Sprite;
                imp.spritePixelsPerUnit = 32;
                imp.filterMode          = FilterMode.Point;
                imp.textureCompression  = TextureImporterCompression.Uncompressed;
                imp.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        // ═══════════════════════════════════════════════════════════════
        // 3) DIALOGUE SCRIPTABLE OBJECTS
        // ═══════════════════════════════════════════════════════════════

        static Dictionary<string, DialogueData> CreateDialogueAssets()
        {
            var d = new Dictionary<string, DialogueData>();

            // Tạo 9 DialogueData (trống trước, điền sau)
            string[] ids =
            {
                "CuBa_FirstMeet",
                "CuBa_BranchA",
                "CuBa_BranchB",
                "CuBa_LoreDump",
                "CuBa_AcceptA",
                "CuBa_AcceptB",
                "CuBa_GiveQuest1",
                "CuBa_CleanTrash_Complete",
                "CuBa_Repeat"
            };

            foreach (string id in ids)
            {
                var asset = ScriptableObject.CreateInstance<DialogueData>();
                asset.dialogueID = id;
                asset.name = id;
                SaveAsset(asset, $"{DIALOGUE_FOLDER}/{id}.asset");
                d[id] = asset;
            }

            // ── Điền nội dung hội thoại ──────────────────────────

            // ▸ Gặp Cụ Bá lần đầu
            d["CuBa_FirstMeet"].lines = new DialogueLine[]
            {
                Line("Cụ Bá",
                    "Ủa... Trời ơi, có phải... là cháu không?!"),
                Line("Cụ Bá",
                    "Chà chà chà! Trông cháu bây giờ khác hẳn ngày bé rồi! " +
                    "Lớn thật rồi đó... mà trông vẫn giống cái hồi còn chạy " +
                    "tắm mưa trước sân đình lắm!"),
                Line("Cụ Bá",
                    "Cụ vui lắm khi cháu về... nhưng...\n" +
                    "Cụ cũng buồn lắm khi cháu phải thấy cảnh này."),
                ChoiceLine()
            };

            // ▸ Nhánh A – "Cháu về để thay đổi"
            d["CuBa_BranchA"].lines = new DialogueLine[]
            {
                Line("Người Chơi",
                    "Không sao đâu cụ, cháu về để thay đổi tất cả."),
                Line("Cụ Bá",
                    "Heh heh... Cụ biết cháu sẽ nói vậy! Giống y chang " +
                    "ông nội cháu ngày xưa – cái lưỡi thì mạnh, nhưng " +
                    "quan trọng là cái tay phải mạnh hơn! Hehe...")
            };

            // ▸ Nhánh B – "Sao ra nông nỗi này?"
            d["CuBa_BranchB"].lines = new DialogueLine[]
            {
                Line("Người Chơi",
                    "Cháu nhớ Green Valley mà cụ ơi... sao lại ra nông nỗi này?"),
                Line("Cụ Bá",
                    "Ôi cháu ơi... Chuyện dài lắm. Khách du lịch bỏ đi, " +
                    "người dân bỏ đi theo, không còn ai ngó ngàng. Rác nhiều, " +
                    "cầu hỏng, giếng cạn... Cụ già rồi, một mình không làm " +
                    "gì được.")
            };

            // ▸ Lore Dump – lịch sử làng
            d["CuBa_LoreDump"].lines = new DialogueLine[]
            {
                Line("Cụ Bá",
                    "Thôi, chuyện cũ để cũ. Cháu về là cụ mừng lắm rồi. " +
                    "Cụ có điều này muốn nói với cháu..."),
                Line("Cụ Bá",
                    "Ngồi xuống đây, cụ kể cháu nghe chuyện của Green Valley này."),
                Line("Cụ Bá",
                    "Ngày xưa, Green Valley này đẹp lắm cháu ơi. Khách từ " +
                    "khắp nơi đổ về. Người ta bảo đây là 'Viên ngọc xanh " +
                    "của núi rừng'."),
                Line("Cụ Bá",
                    "Nước giếng cổ đầu làng trong như pha lê, uống vào mát " +
                    "tận tim. Cây cầu gỗ bắc qua suối – do tổ tiên ta tự " +
                    "tay dựng lên từ trăm năm trước."),
                Line("Cụ Bá",
                    "Rồi người ta bắt đầu bỏ bê. Không ai quy hoạch, không " +
                    "ai chăm lo. Rác thải từ khách vãng lai chất đống. Cầu " +
                    "hỏng không ai sửa. Giếng cổ bị bùn lấp dần..."),
                Line("Cụ Bá",
                    "Người trẻ trong làng thấy không làm ăn gì được, kéo " +
                    "nhau lên thành phố hết. Giờ cụ nhìn quanh, chỉ còn " +
                    "mấy người già như cụ thôi."),
                Line("Cụ Bá",
                    "Nhưng hôm nay... cháu về. Và cụ tin – Green Valley " +
                    "chưa chết. Nó chỉ đang ngủ thôi."),
                Line("Cụ Bá",
                    "Cụ muốn trao cho cháu quyền quản lý khu đất hoang " +
                    "này. Đây là đất của dòng họ ta, của Green Valley. " +
                    "Cháu có dám nhận không?"),
                ChoiceLine()
            };

            // ▸ Đồng ý A – Nhiệt tình
            d["CuBa_AcceptA"].lines = new DialogueLine[]
            {
                Line("Người Chơi",
                    "Dạ, cháu nhận! Cháu sẽ không phụ lòng cụ."),
                Line("Cụ Bá",
                    "Hehe! Cụ biết ngay mà! Máu con cháu của cụ thì " +
                    "không biết sợ là gì! Được rồi, cụ sẽ chỉ cho cháu " +
                    "biết làm gì trước nhé.")
            };

            // ▸ Đồng ý B – Do dự
            d["CuBa_AcceptB"].lines = new DialogueLine[]
            {
                Line("Người Chơi",
                    "Cháu... cháu cũng không chắc mình làm được " +
                    "không cụ ơi."),
                Line("Cụ Bá",
                    "Hehe... Cháu biết không, ông nội cháu ngày xưa " +
                    "khi gặp khó cũng nói y chang vậy. Mà ổng có sao " +
                    "đâu – ổng là người xây nên cái làng này đó!"),
                Line("Người Chơi",
                    "... Vâng. Cháu sẽ cố."),
                Line("Cụ Bá",
                    "Tốt lắm! Vậy là đủ rồi. Thôi, cụ sẽ chỉ cho " +
                    "cháu biết làm gì trước nhé.")
            };

            // ▸ Giao nhiệm vụ dọn rác
            d["CuBa_GiveQuest1"].lines = new DialogueLine[]
            {
                Line("Cụ Bá",
                    "Cháu ơi, nhìn cái này mà thấy đau lòng lắm..."),
                Line("Cụ Bá",
                    "Khách du lịch vãng lai ngày trước để lại đấy. " +
                    "Không ai dọn, nó cứ chất lên mãi. Năm đống lớn " +
                    "thế kia, chặn cả lối đi vào làng."),
                Line("Cụ Bá",
                    "Cháu ơi, muốn làng đẹp thì trước tiên phải sạch " +
                    "đã. Cháu có thể giúp cụ dọn 5 đống rác lớn xung " +
                    "quanh khu vực lối vào không?"),
                Line("Cụ Bá",
                    "Cái túi này cụ dùng hồi còn trẻ. Cháu mang theo " +
                    "mà đựng rác.", "QuestStarted")
            };

            // ▸ Hoàn thành dọn rác
            d["CuBa_CleanTrash_Complete"].lines = new DialogueLine[]
            {
                Line("Cụ Bá",
                    "Cụ không ngờ... Chỉ mấy cái đống rác thôi, mà " +
                    "dọn xong rồi lại thấy con đường này quen lắm. " +
                    "Hồi nhỏ cụ chạy trên con đường này mỗi sáng đó..."),
                Line("Cụ Bá",
                    "Cảm ơn cháu. Thiệt lòng cảm ơn. Cháu không biết " +
                    "cụ vui đến mức nào đâu."),
                Line("Cụ Bá",
                    "Đây là 100 đồng vàng cụ dành dụm lâu nay. Không " +
                    "nhiều, nhưng là tất cả những gì cụ có. Cháu cầm " +
                    "lấy đi đường.")
            };

            // ▸ Hội thoại lặp lại
            d["CuBa_Repeat"].lines = new DialogueLine[]
            {
                Line("Cụ Bá",
                    "Đừng vội, cháu ơi. Làng này hỏng nhiều năm, không " +
                    "thể sửa trong một ngày. Nhưng mỗi ngày làm một " +
                    "chút, đất sẽ nhớ ơn mình.")
            };

            // Lưu tất cả
            foreach (var kv in d) EditorUtility.SetDirty(kv.Value);
            AssetDatabase.SaveAssets();
            return d;
        }

        // ═══════════════════════════════════════════════════════════════
        // 4) QUEST SCRIPTABLE OBJECTS
        // ═══════════════════════════════════════════════════════════════

        static Dictionary<string, QuestData> CreateQuestAssets()
        {
            var q = new Dictionary<string, QuestData>();

            var quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID          = "Quest_CleanTrash_Ch1";
            quest.questName        = "Lối Về Trong Sạch";
            quest.questDescription = "Dọn sạch 5 đống rác xung quanh khu vực " +
                                     "lối vào làng để mở đường cho du khách.";
            quest.chapterName      = "Chương I: Tiếng Gọi Từ Thiên Nhiên";
            quest.steps = new QuestStep[]
            {
                new QuestStep
                {
                    stepDescription = "Dọn đống rác",
                    stepType        = QuestStepType.CollectResource,
                    targetID        = "Trash",
                    requiredAmount  = 5
                }
            };
            quest.reward = new QuestReward
            {
                goldAmount        = 100,
                reputationAmount  = 5,
                rewardDescription = "+100 Vàng, +5 Danh Tiếng"
            };

            quest.name = "Quest_CleanTrash_Ch1";
            SaveAsset(quest, $"{QUEST_FOLDER}/Quest_CleanTrash_Ch1.asset");
            q["Quest_CleanTrash_Ch1"] = quest;

            EditorUtility.SetDirty(quest);
            AssetDatabase.SaveAssets();
            return q;
        }

        // ═══════════════════════════════════════════════════════════════
        // 5) LIÊN KẾT CROSS-REFERENCES
        // ═══════════════════════════════════════════════════════════════

        static void LinkAllAssets(
            Dictionary<string, DialogueData> d,
            Dictionary<string, QuestData>    q)
        {
            // FirstMeet → 2 nhánh lựa chọn
            d["CuBa_FirstMeet"].branchDialogues = new[]
                { d["CuBa_BranchA"], d["CuBa_BranchB"] };

            // Cả 2 nhánh → LoreDump
            d["CuBa_BranchA"].nextDialogue = d["CuBa_LoreDump"];
            d["CuBa_BranchB"].nextDialogue = d["CuBa_LoreDump"];

            // LoreDump → 2 lựa chọn đồng ý
            d["CuBa_LoreDump"].branchDialogues = new[]
                { d["CuBa_AcceptA"], d["CuBa_AcceptB"] };

            // Cả 2 đồng ý → Giao quest
            d["CuBa_AcceptA"].nextDialogue = d["CuBa_GiveQuest1"];
            d["CuBa_AcceptB"].nextDialogue = d["CuBa_GiveQuest1"];

            // Giao quest → kích hoạt Quest
            d["CuBa_GiveQuest1"].questToTrigger = q["Quest_CleanTrash_Ch1"];

            // Quest hoàn thành → hội thoại khen ngợi
            q["Quest_CleanTrash_Ch1"].onCompleteDialogue =
                d["CuBa_CleanTrash_Complete"];

            foreach (var kv in d) EditorUtility.SetDirty(kv.Value);
            foreach (var kv in q) EditorUtility.SetDirty(kv.Value);
            AssetDatabase.SaveAssets();
        }

        // ═══════════════════════════════════════════════════════════════
        // 6) CANVAS UI HỘP THOẠI
        // ═══════════════════════════════════════════════════════════════

        static UIRefs BuildDialogueUI()
        {
            UIRefs ui = new UIRefs();

            // ── Canvas ──────────────────────────────────────────────
            GameObject canvasGO = new GameObject("DialogueCanvas");
            Canvas canvas       = canvasGO.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler         = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode          = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution  = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight   = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // ── EventSystem (nếu chưa có) ───────────────────────────
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<EventSystem>();
                esGO.AddComponent<StandaloneInputModule>();
            }

            // ── Dialogue Panel ──────────────────────────────────────
            GameObject panelGO  = NewUI("DialoguePanel", canvasGO.transform);
            Image panelImg      = panelGO.AddComponent<Image>();
            panelImg.color      = new Color(0.08f, 0.08f, 0.14f, 0.93f);
            RectTransform panelRT = panelGO.GetComponent<RectTransform>();
            SetAnchors(panelRT, 0.04f, 0.02f, 0.96f, 0.30f);
            ui.dialoguePanel = panelGO;

            // Viền sáng nhẹ quanh panel
            Outline outline = panelGO.AddComponent<Outline>();
            outline.effectColor    = new Color(0.55f, 0.75f, 0.95f, 0.6f);
            outline.effectDistance = new Vector2(2, -2);

            // ── Portrait Image ──────────────────────────────────────
            GameObject portraitGO  = NewUI("PortraitImage", panelGO.transform);
            ui.portraitImage       = portraitGO.AddComponent<Image>();
            ui.portraitImage.color = Color.white;
            ui.portraitImage.preserveAspect = true;
            RectTransform pRT = portraitGO.GetComponent<RectTransform>();
            pRT.anchorMin        = new Vector2(0, 0);
            pRT.anchorMax        = new Vector2(0, 0);
            pRT.pivot            = new Vector2(0, 0);
            pRT.anchoredPosition = new Vector2(15, 15);
            pRT.sizeDelta        = new Vector2(150, 150);

            // Viền portrait
            Outline pOutline = portraitGO.AddComponent<Outline>();
            pOutline.effectColor    = new Color(1f, 0.85f, 0.3f, 0.7f);
            pOutline.effectDistance = new Vector2(2, -2);

            // ── Speaker Name ────────────────────────────────────────
            GameObject nameGO   = NewUI("SpeakerNameText", panelGO.transform);
            ui.speakerNameText  = nameGO.AddComponent<TextMeshProUGUI>();
            ui.speakerNameText.text      = "Cụ Bá";
            ui.speakerNameText.fontSize  = 30;
            ui.speakerNameText.fontStyle = FontStyles.Bold;
            ui.speakerNameText.color     = new Color(1f, 0.85f, 0.30f);
            RectTransform nRT = nameGO.GetComponent<RectTransform>();
            nRT.anchorMin        = new Vector2(0, 1);
            nRT.anchorMax        = new Vector2(1, 1);
            nRT.pivot            = new Vector2(0, 1);
            nRT.anchoredPosition = new Vector2(180, -12);
            nRT.sizeDelta        = new Vector2(-200, 38);

            // ── Dialogue Text ───────────────────────────────────────
            GameObject textGO  = NewUI("DialogueText", panelGO.transform);
            ui.dialogueText    = textGO.AddComponent<TextMeshProUGUI>();
            ui.dialogueText.text        = "Nhấn [E] gần NPC để bắt đầu hội thoại.";
            ui.dialogueText.fontSize    = 24;
            ui.dialogueText.color       = new Color(0.92f, 0.92f, 0.95f);
            ui.dialogueText.enableWordWrapping = true;
            ui.dialogueText.overflowMode       = TextOverflowModes.Ellipsis;
            RectTransform tRT = textGO.GetComponent<RectTransform>();
            tRT.anchorMin = Vector2.zero;
            tRT.anchorMax = Vector2.one;
            tRT.offsetMin = new Vector2(180, 15);
            tRT.offsetMax = new Vector2(-20, -55);

            // ── Next Button ─────────────────────────────────────────
            ui.nextButtonGO = NewUI("NextButton", panelGO.transform);
            Image btnImg    = ui.nextButtonGO.AddComponent<Image>();
            btnImg.color    = new Color(0.18f, 0.55f, 0.38f, 0.95f);
            ui.nextButton   = ui.nextButtonGO.AddComponent<Button>();

            // Hiệu ứng hover
            ColorBlock cb = ui.nextButton.colors;
            cb.highlightedColor = new Color(0.25f, 0.70f, 0.50f);
            cb.pressedColor     = new Color(0.12f, 0.40f, 0.28f);
            ui.nextButton.colors = cb;

            RectTransform bRT = ui.nextButtonGO.GetComponent<RectTransform>();
            bRT.anchorMin        = new Vector2(1, 0);
            bRT.anchorMax        = new Vector2(1, 0);
            bRT.pivot            = new Vector2(1, 0);
            bRT.anchoredPosition = new Vector2(-15, 12);
            bRT.sizeDelta        = new Vector2(130, 42);

            // Text bên trong nút
            GameObject btnTextGO = NewUI("Text", ui.nextButtonGO.transform);
            var btnTxt           = btnTextGO.AddComponent<TextMeshProUGUI>();
            btnTxt.text      = "Tiếp ▶";
            btnTxt.fontSize  = 22;
            btnTxt.color     = Color.white;
            btnTxt.alignment = TextAlignmentOptions.Center;
            StretchFull(btnTextGO.GetComponent<RectTransform>());

            // ── Choice Panel ────────────────────────────────────────
            ui.choicePanelGO = NewUI("ChoicePanel", panelGO.transform);
            var vlg = ui.choicePanelGO.AddComponent<VerticalLayoutGroup>();
            vlg.spacing             = 12;
            vlg.childAlignment      = TextAnchor.MiddleCenter;
            vlg.childControlWidth   = false;
            vlg.childControlHeight  = false;
            vlg.childForceExpandWidth  = false;
            vlg.childForceExpandHeight = false;
            RectTransform cRT = ui.choicePanelGO.GetComponent<RectTransform>();
            SetAnchors(cRT, 0.15f, 0.10f, 0.85f, 0.90f);

            // 2 nút lựa chọn
            ui.choiceButtons = new Button[2];
            ui.choiceTexts   = new TextMeshProUGUI[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject cbGO = NewUI($"ChoiceButton{i + 1}",
                                        ui.choicePanelGO.transform);
                Image cbImg     = cbGO.AddComponent<Image>();
                cbImg.color     = new Color(0.12f, 0.25f, 0.45f, 0.90f);
                ui.choiceButtons[i] = cbGO.AddComponent<Button>();

                ColorBlock ccb = ui.choiceButtons[i].colors;
                ccb.highlightedColor = new Color(0.20f, 0.40f, 0.65f);
                ccb.pressedColor     = new Color(0.08f, 0.18f, 0.35f);
                ui.choiceButtons[i].colors = ccb;

                var le = cbGO.AddComponent<LayoutElement>();
                le.preferredWidth  = 750;
                le.preferredHeight = 55;

                GameObject ctGO = NewUI("Text", cbGO.transform);
                ui.choiceTexts[i]           = ctGO.AddComponent<TextMeshProUGUI>();
                ui.choiceTexts[i].text      = $"Lựa chọn {i + 1}";
                ui.choiceTexts[i].fontSize  = 21;
                ui.choiceTexts[i].color     = Color.white;
                ui.choiceTexts[i].alignment = TextAlignmentOptions.Center;
                ui.choiceTexts[i].enableWordWrapping = true;
                RectTransform ctRT = ctGO.GetComponent<RectTransform>();
                ctRT.anchorMin = Vector2.zero;
                ctRT.anchorMax = Vector2.one;
                ctRT.offsetMin = new Vector2(12, 4);
                ctRT.offsetMax = new Vector2(-12, -4);
            }

            return ui;
        }

        // ═══════════════════════════════════════════════════════════════
        // 7) MANAGERS (DialogueManager + QuestManager)
        // ═══════════════════════════════════════════════════════════════

        static void SetupManagers(UIRefs ui, Dictionary<string, QuestData> quests)
        {
            // ── DialogueManager ─────────────────────────────────────
            GameObject dmGO = new GameObject("DialogueManager");
            var dm = dmGO.AddComponent<DialogueManager>();

            // Gán UI references qua SerializedObject (vì field là private)
            SerializedObject dmSO = new SerializedObject(dm);
            dmSO.FindProperty("dialoguePanel").objectReferenceValue    = ui.dialoguePanel;
            dmSO.FindProperty("portraitImage").objectReferenceValue     = ui.portraitImage;
            dmSO.FindProperty("speakerNameText").objectReferenceValue   = ui.speakerNameText;
            dmSO.FindProperty("dialogueText").objectReferenceValue      = ui.dialogueText;
            dmSO.FindProperty("nextButton").objectReferenceValue        = ui.nextButtonGO;
            dmSO.FindProperty("choicePanel").objectReferenceValue       = ui.choicePanelGO;
            dmSO.FindProperty("typingSpeed").floatValue                 = 0.035f;

            // choiceButtons array
            var cbProp = dmSO.FindProperty("choiceButtons");
            cbProp.arraySize = ui.choiceButtons.Length;
            for (int i = 0; i < ui.choiceButtons.Length; i++)
                cbProp.GetArrayElementAtIndex(i).objectReferenceValue =
                    ui.choiceButtons[i];

            // choiceTexts array
            var ctProp = dmSO.FindProperty("choiceTexts");
            ctProp.arraySize = ui.choiceTexts.Length;
            for (int i = 0; i < ui.choiceTexts.Length; i++)
                ctProp.GetArrayElementAtIndex(i).objectReferenceValue =
                    ui.choiceTexts[i];

            dmSO.ApplyModifiedPropertiesWithoutUndo();

            // Gắn sự kiện onClick cho nút Next
            UnityEventTools.AddPersistentListener(
                ui.nextButton.onClick,
                dm.OnNextButtonClicked);

            // ── QuestManager ────────────────────────────────────────
            GameObject qmGO = new GameObject("QuestManager");
            var qm = qmGO.AddComponent<QuestManager>();

            SerializedObject qmSO = new SerializedObject(qm);
            var questList = qmSO.FindProperty("allQuests");
            questList.arraySize = quests.Count;
            int idx = 0;
            foreach (var kv in quests)
            {
                questList.GetArrayElementAtIndex(idx).objectReferenceValue = kv.Value;
                idx++;
            }
            qmSO.ApplyModifiedPropertiesWithoutUndo();
        }

        // ═══════════════════════════════════════════════════════════════
        // 8) PLAYER
        // ═══════════════════════════════════════════════════════════════

        static void CreatePlayer()
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = new Vector3(0, 0, 0);

            // Sprite
            var sr    = player.AddComponent<SpriteRenderer>();
            sr.sprite = LoadAnySprite(PLAYER_SPRITE_PATH);
            sr.color  = sr.sprite != null ? Color.white
                                          : new Color(0.3f, 0.7f, 1f); // xanh fallback
            sr.sortingOrder = 5;

            // Nếu không có sprite → tạo hình tượng trưng
            if (sr.sprite == null)
            {
                sr.sprite = CreateColoredSprite("player_placeholder",
                    new Color(0.3f, 0.7f, 1f), 32);
            }

            // Physics – di chuyển top-down
            var rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints  = RigidbodyConstraints2D.FreezeRotation;

            // Collider – cần để trigger detection hoạt động
            var col = player.AddComponent<BoxCollider2D>();
            col.size   = new Vector2(0.5f, 0.7f);
            col.offset = new Vector2(0, -0.15f);

            // Animator (bắt buộc bởi PlayerMovement)
            player.AddComponent<Animator>();

            // Movement script
            player.AddComponent<PlayerMovement>();
        }

        // ═══════════════════════════════════════════════════════════════
        // 9) NPC CỤ BÁ
        // ═══════════════════════════════════════════════════════════════

        static void CreateNPC_CuBa(Dictionary<string, DialogueData> dlg)
        {
            GameObject npc = new GameObject("NPC_CuBa");
            npc.transform.position = new Vector3(4f, 0, 0);

            // Sprite
            var sr    = npc.AddComponent<SpriteRenderer>();
            sr.sprite = LoadAnySprite(CUBA_SPRITE_PATH);
            sr.sortingOrder = 5;
            if (sr.sprite == null)
            {
                sr.sprite = CreateColoredSprite("cuba_placeholder",
                    new Color(0.85f, 0.65f, 0.3f), 32);
            }

            // Trigger Collider (vùng phát hiện player)
            var col       = npc.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius    = 2.0f;

            // NPCInteract script
            var interact = npc.AddComponent<NPCInteract>();
            SerializedObject so = new SerializedObject(interact);
            so.FindProperty("firstMeetDialogue").objectReferenceValue = dlg["CuBa_FirstMeet"];
            so.FindProperty("repeatDialogue").objectReferenceValue    = dlg["CuBa_Repeat"];
            so.FindProperty("npcName").stringValue                    = "Cụ Bá";
            so.ApplyModifiedPropertiesWithoutUndo();

            // Label hiển thị trong Editor
            var labelGO = new GameObject("NameLabel");
            labelGO.transform.SetParent(npc.transform, false);
            labelGO.transform.localPosition = new Vector3(0, 1.2f, 0);
            // (TextMesh cho label trong scene view – không phải UI)
            var tm = labelGO.AddComponent<TextMesh>();
            tm.text      = "Cụ Bá";
            tm.fontSize  = 40;
            tm.characterSize = 0.12f;
            tm.anchor    = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color     = new Color(1f, 0.85f, 0.3f);
        }

        // ═══════════════════════════════════════════════════════════════
        // 10) 5 ĐỐNG RÁC
        // ═══════════════════════════════════════════════════════════════

        static void CreateTrashPiles(Sprite trashSprite)
        {
            Vector2[] positions = new Vector2[]
            {
                new Vector2(-3f,  2.5f),
                new Vector2(-2f, -1.5f),
                new Vector2( 1.5f, 3f),
                new Vector2( 5f,  -2f),
                new Vector2(-4.5f,-3f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject trash = new GameObject($"Trash_{i + 1}");
                trash.transform.position = new Vector3(
                    positions[i].x, positions[i].y, 0);

                // Sprite
                var sr         = trash.AddComponent<SpriteRenderer>();
                sr.sprite      = trashSprite;
                sr.sortingOrder = 3;
                // Xoay / scale nhẹ cho đa dạng
                float scaleVar = Random.Range(0.8f, 1.3f);
                trash.transform.localScale = Vector3.one * scaleVar;

                // Trigger Collider
                var col       = trash.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius    = 1.2f;

                // InteractableObject
                var io = trash.AddComponent<Interaction.InteractableObject>();
                SerializedObject so = new SerializedObject(io);
                so.FindProperty("targetID").stringValue        = "Trash";
                so.FindProperty("interactionType").enumValueIndex =
                    (int)QuestStepType.CollectResource;
                so.FindProperty("amountPerInteraction").intValue  = 1;
                so.FindProperty("destroyOnComplete").boolValue    = true;
                so.FindProperty("interactionsRequired").intValue  = 1;
                so.FindProperty("energyCost").floatValue          = 10f;
                so.ApplyModifiedPropertiesWithoutUndo();

                // Label
                var labelGO = new GameObject("Label");
                labelGO.transform.SetParent(trash.transform, false);
                labelGO.transform.localPosition = new Vector3(0, 0.8f, 0);
                var tm = labelGO.AddComponent<TextMesh>();
                tm.text      = $"Rác #{i + 1}";
                tm.fontSize  = 36;
                tm.characterSize = 0.1f;
                tm.anchor    = TextAnchor.MiddleCenter;
                tm.alignment = TextAlignment.Center;
                tm.color     = new Color(0.7f, 0.5f, 0.3f);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // 11) CAMERA
        // ═══════════════════════════════════════════════════════════════

        static void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camGO = new GameObject("Main Camera");
                camGO.tag = "MainCamera";
                cam = camGO.AddComponent<Camera>();
            }

            cam.orthographic     = true;
            cam.orthographicSize = 6f;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.backgroundColor  = new Color(0.18f, 0.30f, 0.15f); // xanh rừng

            // Tạo nền đất đơn giản
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
            ground.name = "Ground";
            ground.transform.position   = new Vector3(0, 0, 1);
            ground.transform.localScale = new Vector3(30, 20, 1);
            // Xóa collider 3D (không cần trong 2D)
            Object.DestroyImmediate(ground.GetComponent<Collider>());

            var groundRend = ground.GetComponent<MeshRenderer>();
            // Tạo material màu xanh cỏ
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = new Color(0.25f, 0.42f, 0.20f);
            groundRend.sharedMaterial = mat;
            groundRend.sortingOrder   = -10;
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Tạo UI GameObject với RectTransform, parented đúng cách</summary>
        static GameObject NewUI(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        /// <summary>Đặt anchor cho RectTransform, reset offsets</summary>
        static void SetAnchors(RectTransform rt,
            float xMin, float yMin, float xMax, float yMax)
        {
            rt.anchorMin = new Vector2(xMin, yMin);
            rt.anchorMax = new Vector2(xMax, yMax);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        /// <summary>Stretch full parent</summary>
        static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        /// <summary>Tạo một DialogueLine thường</summary>
        static DialogueLine Line(string speaker, string text,
            string triggerEvent = "")
        {
            return new DialogueLine
            {
                speakerName      = speaker,
                dialogueText     = text,
                isPlayerChoice   = false,
                triggerEventName = triggerEvent
            };
        }

        /// <summary>Tạo một DialogueLine phân nhánh</summary>
        static DialogueLine ChoiceLine()
        {
            return new DialogueLine
            {
                speakerName      = "",
                dialogueText     = "",
                isPlayerChoice   = true,
                triggerEventName = ""
            };
        }

        /// <summary>Lưu ScriptableObject, ghi đè nếu đã tồn tại</summary>
        static void SaveAsset(Object asset, string path)
        {
            Object existing = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (existing != null)
                AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(asset, path);
        }

        /// <summary>Load sprite từ asset path (hỗ trợ spritesheet)</summary>
        static Sprite LoadAnySprite(string assetPath)
        {
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (s != null) return s;

            // Thử tìm sub-sprite trong spritesheet
            Object[] all = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if (all != null)
            {
                foreach (Object obj in all)
                    if (obj is Sprite sprite) return sprite;
            }
            return null;
        }

        /// <summary>Tạo sprite màu đơn giản (placeholder)</summary>
        static Sprite CreateColoredSprite(string name, Color color, int size)
        {
            string path = $"{GENERATED_SPRITES}/{name}.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] px = new Color[size * size];

            // Hình tròn đơn giản
            float center = (size - 1) / 2f;
            float radius = center * 0.85f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(
                    new Vector2(x, y), new Vector2(center, center));
                px[y * size + x] = d <= radius ? color : Color.clear;
            }

            tex.SetPixels(px);
            tex.Apply();

            string diskPath = Path.Combine(Application.dataPath, "..",
                                           path).Replace("\\", "/");
            File.WriteAllBytes(diskPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp != null)
            {
                imp.textureType         = TextureImporterType.Sprite;
                imp.spritePixelsPerUnit = size;
                imp.filterMode          = FilterMode.Point;
                imp.textureCompression  = TextureImporterCompression.Uncompressed;
                imp.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
#endif
