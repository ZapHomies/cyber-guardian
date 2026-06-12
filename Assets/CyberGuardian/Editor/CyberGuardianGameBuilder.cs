using System.Collections.Generic;
using System.IO;
using CyberGuardian;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace CyberGuardian.Editor
{
    public static class CyberGuardianGameBuilder
    {
        private const string MainMenuScenePath = "Assets/CyberGuardian/Scenes/CyberGuardian_MainMenu.unity";
        private const string DifficultyScenePath = "Assets/CyberGuardian/Scenes/CyberGuardian_PilihKesulitan.unity";
        private const string LevelScenePath = "Assets/CyberGuardian/Scenes/CyberGuardian_Level01.unity";
        private const string Level02ScenePath = "Assets/CyberGuardian/Scenes/CyberGuardian_Level02.unity";
        private const string Level03ScenePath = "Assets/CyberGuardian/Scenes/CyberGuardian_Level03.unity";
        private const string GeneratedArtFolder = "Assets/CyberGuardian/Art/Generated";
        private const string StarterQuestionBankPath = "Assets/CyberGuardian/Data/Quiz/CyberSecurity_Starter_QuestionBank.asset";
        private const string EasyDifficultyPath = "Assets/CyberGuardian/Data/Difficulty/Easy.asset";
        private const string NormalDifficultyPath = "Assets/CyberGuardian/Data/Difficulty/Normal.asset";
        private const string HardDifficultyPath = "Assets/CyberGuardian/Data/Difficulty/Hard.asset";
        private const string BossProjectilePrefabPath = "Assets/CyberGuardian/Prefabs/Projectiles/BossPacketProjectile.prefab";
        private const string PlayerSpriteFolder = "Assets/CyberGuardian/Art/Player/CyberGuardianSprites";
        private const string EnemySpriteFolder = "Assets/CyberGuardian/Art/Enemies/BasicEnemySprites";

        private const string VirusSpritePath = "Assets/CyberGuardian/Art/Enemies/VirusBigPack/png1.png";
        private const string VirusAltSpritePath = "Assets/CyberGuardian/Art/Enemies/VirusBigPack/png18.png";
        private const string ProjectileSpritePath = "Assets/CyberGuardian/Art/VFX/KenneyParticles/PNG (Transparent)/muzzle_03.png";
        private const string SparkSpritePath = "Assets/CyberGuardian/Art/VFX/KenneyParticles/PNG (Transparent)/spark_02.png";
        private const string CrosshairSpritePath = "Assets/CyberGuardian/Art/UI/KenneySpaceUI/PNG/Blue/Default/crosshair_color_a.png";
        private const string PanelFrameSpritePath = "Assets/CyberGuardian/Art/UI/CyberpunkPixelUI/1 Frames/Frame_05.png";
        private const string ButtonSpritePath = "Assets/CyberGuardian/Art/UI/KenneySpaceUI/PNG/Blue/Default/button_square_header_large_rectangle.png";
        private const string CircuitSpritePath = "Assets/SourceFiles/Textures/Repeating Tiles/Circuit_Albedo.png";
        private const string MainMenuVideoPath = "Assets/CyberGuardian/Art/Menu/kling_20260604_VIDEO_Image1berf_5479_0.mp4";
        private const string MainMenuRenderTexturePath = "Assets/CyberGuardian/Art/Menu/MainMenuVideoBackground.renderTexture";

        private const string MainMenuMusicPath = "Assets/CyberGuardian/Audio/BGM/BGM_Main_Menu.wav";
        private const string Level01MusicPath = "Assets/CyberGuardian/Audio/BGM/BGM_Level_01_Data_Forest.wav";
        private const string Level02MusicPath = "Assets/CyberGuardian/Audio/BGM/BGM_Level_02_Server_Ruins.wav";
        private const string Level03MusicPath = "Assets/CyberGuardian/Audio/BGM/BGM_Level_03_Code_Abyss.wav";
        private const string BossMusicPath = "Assets/CyberGuardian/Audio/BGM/BGM_Boss_Encounter.wav";
        private const string NamedSfxFolder = "Assets/CyberGuardian/Audio/SFX/Named/";
        private const string MeleeSfxPath = NamedSfxFolder + "SFX_Player_Melee.wav";
        private const string HitSfxPath = NamedSfxFolder + "SFX_Player_Hit.wav";
        private const string BossShotSfxPath = NamedSfxFolder + "SFX_Boss_Shoot.wav";
        private const string ShieldSfxPath = NamedSfxFolder + "SFX_Boss_Shield_Hit.wav";
        private const string WrongSfxPath = NamedSfxFolder + "SFX_Quiz_Wrong.wav";

        private const string BossGlbPath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_malware_boss.glb";
        private const string TrapGlbPath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_platform_traps.glb";
        private const string ProjectileGlbPath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_projectiles.glb";

        private const string BossGeneratedSpritePath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_malware_boss_sprite.png";
        private const string ProjectileGeneratedSpritePath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_patch_core_sprite.png";
        private const bool RegenerateGeneratedSprites = true;

        private sealed class CyberHorrorAssetSprites
        {
            public Sprite CircuitBlock;
            public Sprite DataStone;
            public Sprite ServerCore;
            public Sprite NeonPlatform;
            public Sprite MetaPanel;
            public Sprite CrackedBlock;
            public Sprite CorruptedBlock;
            public Sprite VirusBlock;
            public Sprite GlowEdgePlatform;
            public Sprite SpikeBlock;
            public Sprite ElectricNode;
            public Sprite GlitchMine;
            public Sprite CrushingBlock;
            public Sprite VirusTurret;
            public Sprite CorruptedPlatform;
            public Sprite DataForestBackground;
            public Sprite ServerRunsBackground;
            public Sprite CodeAbyssBackground;
            public Sprite UiButtonCyan;
            public Sprite UiButtonMagenta;
            public Sprite UiPanelFrame;
            public Sprite UiBarBack;
            public Sprite UiHpBarFill;
            public Sprite UiBoostBarFill;
            public Sprite UiBossBarFill;
            public Sprite UiScorePanel;
            public Sprite UiAlertPanel;
            public Sprite UiMenuHeader;
            public Sprite UiSkullMark;
            public Sprite QuizBlock;
        }

        [MenuItem("Cyber Guardian/Bangun Semua Scene Game")]
        public static void BuildGameScenes()
        {
            Directory.CreateDirectory(ToAbsolutePath("Assets/CyberGuardian/Scenes"));
            Directory.CreateDirectory(ToAbsolutePath(GeneratedArtFolder));
            Directory.CreateDirectory(ToAbsolutePath("Assets/CyberGuardian/Prefabs/Projectiles"));

            Sprite squareSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_square.png", TextureShape.Square);
            Sprite circleSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_circle.png", TextureShape.Circle);
            Sprite panelSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_panel.png", TextureShape.RoundedRect);
            Sprite rockTileSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_cyber_rock_tile.png", TextureShape.CyberRock);
            Sprite metalCrateSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_metal_crate_tile.png", TextureShape.MetalCrate);
            Sprite dataMossSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_data_moss.png", TextureShape.DataMoss);
            Sprite sawBladeSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_saw_blade.png", TextureShape.SawBlade);
            Sprite dataBlobSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_data_cloud.png", TextureShape.DataCloud);
            CyberHorrorAssetSprites horrorSprites = EnsureCyberHorrorSprites();
            Sprite virusSprite = EnsureImportedSprite(VirusSpritePath);
            Sprite virusAltSprite = EnsureImportedSprite(VirusAltSpritePath);
            Sprite projectileSprite = EnsureImportedSprite(ProjectileSpritePath);
            Sprite sparkSprite = EnsureImportedSprite(SparkSpritePath);
            Sprite crosshairSprite = EnsureImportedSprite(CrosshairSpritePath);
            Sprite frameSprite = EnsureImportedSprite(PanelFrameSpritePath, new Vector4(28f, 28f, 28f, 28f));
            Sprite buttonSprite = EnsureImportedSprite(ButtonSpritePath, new Vector4(18f, 18f, 18f, 18f));
            Sprite circuitSprite = EnsureImportedSprite(CircuitSpritePath);

            AudioClip meleeSfx = EnsureImportedAudioClip(MeleeSfxPath);
            AudioClip hitSfx = EnsureImportedAudioClip(HitSfxPath);
            AudioClip bossShotSfx = EnsureImportedAudioClip(BossShotSfxPath);
            AudioClip shieldSfx = EnsureImportedAudioClip(ShieldSfxPath);
            AudioClip wrongSfx = EnsureImportedAudioClip(WrongSfxPath);
            AudioClip menuMusic = EnsureImportedAudioClip(MainMenuMusicPath);

            QuizQuestionBank questionBank = EnsureQuestionBank();
            DifficultyProfile[] difficulties = EnsureDifficultyProfiles();
            Font font = GetUiFont();
            GameObject bossProjectilePrefab = EnsureBossProjectilePrefab(circleSprite, sparkSprite);

            BuildMainMenuScene(panelSprite, circleSprite, rockTileSprite, metalCrateSprite, dataMossSprite, sawBladeSprite, dataBlobSprite, horrorSprites, virusSprite, frameSprite, buttonSprite, circuitSprite, font, menuMusic);
            BuildDifficultySelectScene(panelSprite, horrorSprites, frameSprite, buttonSprite, circuitSprite, font);
            BuildLevelScene(squareSprite, circleSprite, panelSprite, rockTileSprite, metalCrateSprite, dataMossSprite, sawBladeSprite, dataBlobSprite, horrorSprites, virusSprite, virusAltSprite, projectileSprite, sparkSprite, crosshairSprite, frameSprite, buttonSprite, circuitSprite, questionBank, difficulties, font, bossProjectilePrefab, meleeSfx, hitSfx, bossShotSfx, shieldSfx, wrongSfx);
            BuildLevel02Scene(squareSprite, circleSprite, panelSprite, rockTileSprite, metalCrateSprite, dataMossSprite, sawBladeSprite, dataBlobSprite, horrorSprites, virusSprite, virusAltSprite, projectileSprite, sparkSprite, crosshairSprite, frameSprite, buttonSprite, circuitSprite, questionBank, difficulties, font, bossProjectilePrefab, meleeSfx, hitSfx, bossShotSfx, shieldSfx, wrongSfx);
            BuildLevel03Scene(squareSprite, circleSprite, panelSprite, rockTileSprite, metalCrateSprite, dataMossSprite, sawBladeSprite, dataBlobSprite, horrorSprites, virusSprite, virusAltSprite, projectileSprite, sparkSprite, crosshairSprite, frameSprite, buttonSprite, circuitSprite, questionBank, difficulties, font, bossProjectilePrefab, meleeSfx, hitSfx, bossShotSfx, shieldSfx, wrongSfx);
            SetBuildScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Cyber Guardian side-scroller main menu, difficulty select, and Levels 01-03 built.");
        }

        private static void BuildMainMenuScene(Sprite panelSprite, Sprite circleSprite, Sprite rockTileSprite, Sprite metalCrateSprite, Sprite dataMossSprite, Sprite sawBladeSprite, Sprite dataBlobSprite, CyberHorrorAssetSprites horrorSprites, Sprite virusSprite, Sprite frameSprite, Sprite buttonSprite, Sprite circuitSprite, Font font, AudioClip menuMusic)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CyberGuardian_MainMenu";

            CreateCamera("07171D", 5.4f, new Vector3(0f, 0.35f, -10f));
            EnsureEventSystem();
            AddLoopingMusic("Main Menu BGM", menuMusic, 0.52f);

            GameObject canvasObject = CreateCanvas("Cyber Guardian Main Menu");
            AddStretchImage("Background", canvasObject.transform, Hex("061317"), panelSprite);
            CreateMainMenuVideoBackground(canvasObject.transform, panelSprite);
            if (circuitSprite != null)
            {
                Image circuit = AddStretchImage("Circuit Texture", canvasObject.transform, new Color(0.2f, 0.95f, 1f, 0.18f), circuitSprite);
                circuit.type = Image.Type.Tiled;
                circuit.pixelsPerUnitMultiplier = 0.32f;
            }

            Sprite panelFrame = horrorSprites != null && horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : panelSprite;
            Sprite activeButton = horrorSprites != null && horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : (buttonSprite != null ? buttonSprite : panelSprite);
            Sprite dangerButton = horrorSprites != null && horrorSprites.UiButtonMagenta != null ? horrorSprites.UiButtonMagenta : activeButton;

            AddStretchImage("Circuit Dark Overlay", canvasObject.transform, new Color(0f, 0f, 0f, 0.50f), panelSprite);
            AddText("Title", canvasObject.transform, new Vector2(0f, -238f), new Vector2(900f, 88f), "CYBER GUARDIAN", 66, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Subtitle", canvasObject.transform, new Vector2(0f, -298f), new Vector2(720f, 34f), "AKSI CYBER DAN KUIS KEAMANAN", 23, Hex("69F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            CyberGuardianMainMenu menu = new GameObject("Cyber Guardian Main Menu Controller").AddComponent<CyberGuardianMainMenu>();
            menu.gameplaySceneName = "CyberGuardian_Level01";
            menu.difficultySceneName = "CyberGuardian_PilihKesulitan";

            menu.creditsButton = AddButton("Credits Button", canvasObject.transform, new Vector2(-760f, -382f), new Vector2(290f, 54f), "KREDIT", 18, font, Hex("08181D"), Color.white, activeButton, out _, false);
            menu.continueButton = AddButton("Continue Button", canvasObject.transform, new Vector2(-380f, -430f), new Vector2(300f, 58f), "LANJUTKAN", 19, font, Hex("08181D"), Color.white, activeButton, out _, false);
            menu.startButton = AddButton("Start Button", canvasObject.transform, new Vector2(0f, -468f), new Vector2(318f, 64f), "MULAI", 27, font, Hex("08181D"), Color.white, activeButton, out _, false);
            menu.settingsButton = AddButton("Settings Button", canvasObject.transform, new Vector2(380f, -430f), new Vector2(300f, 58f), "PENGATURAN", 18, font, Hex("08181D"), Color.white, activeButton, out _, false);
            menu.quitButton = AddButton("Quit Button", canvasObject.transform, new Vector2(760f, -382f), new Vector2(290f, 54f), "KELUAR", 19, font, Hex("160810"), Color.white, dangerButton, out _, false);

            BuildMenuInfoPanels(canvasObject.transform, menu, panelFrame, activeButton, font);
            BuildMenuStartTransition(canvasObject.transform, menu, panelFrame, circuitSprite, font);

            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        }

        private static void BuildDifficultySelectScene(Sprite panelSprite, CyberHorrorAssetSprites horrorSprites, Sprite frameSprite, Sprite buttonSprite, Sprite circuitSprite, Font font)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CyberGuardian_PilihKesulitan";

            CreateCamera("07171D", 5.4f, new Vector3(0f, 0.35f, -10f));
            EnsureEventSystem();

            GameObject canvasObject = CreateCanvas("Cyber Guardian Pilih Kesulitan");
            AddStretchImage("Background", canvasObject.transform, Hex("061317"), panelSprite);
            if (circuitSprite != null)
            {
                Image circuit = AddStretchImage("Circuit Texture", canvasObject.transform, new Color(0.2f, 0.95f, 1f, 0.20f), circuitSprite);
                circuit.type = Image.Type.Tiled;
                circuit.pixelsPerUnitMultiplier = 0.28f;
            }

            AddStretchImage("Dark Overlay", canvasObject.transform, new Color(0f, 0f, 0f, 0.46f), panelSprite);

            Sprite panelFrame = horrorSprites != null && horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : (frameSprite != null ? frameSprite : panelSprite);
            Sprite activeButton = horrorSprites != null && horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : (buttonSprite != null ? buttonSprite : panelSprite);
            Sprite dangerButton = horrorSprites != null && horrorSprites.UiButtonMagenta != null ? horrorSprites.UiButtonMagenta : activeButton;

            CyberGuardianDifficultySelect select = new GameObject("Cyber Guardian Difficulty Select Controller").AddComponent<CyberGuardianDifficultySelect>();
            select.gameplaySceneName = "CyberGuardian_Level01";
            select.menuSceneName = "CyberGuardian_MainMenu";

            AddText("Title", canvasObject.transform, new Vector2(0f, 244f), new Vector2(920f, 72f), "PILIH KESULITAN", 50, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Subtitle", canvasObject.transform, new Vector2(0f, 190f), new Vector2(900f, 42f), "Setiap pilihan mengubah perisai awal, kerusakan jebakan, dan tekanan bos.", 20, Hex("69F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            AddDifficultyCard(canvasObject.transform, panelFrame, activeButton, font, new Vector2(-430f, -12f), "MUDAH", "Perisai lebih tebal. Cocok untuk belajar kontrol, kuis, dan pola bos.", Hex("61F7FF"), out select.easyButton);
            AddDifficultyCard(canvasObject.transform, panelFrame, activeButton, font, new Vector2(0f, -40f), "NORMAL", "Tantangan seimbang untuk petualangan utama Cyber Guardian.", Hex("7DFF9B"), out select.normalButton);
            AddDifficultyCard(canvasObject.transform, panelFrame, dangerButton, font, new Vector2(430f, -12f), "SULIT", "Musuh lebih menekan, jawaban salah lebih berbahaya, bos lebih agresif.", Hex("FF3B88"), out select.hardButton);

            select.detailText = AddText("Detail", canvasObject.transform, new Vector2(0f, -244f), new Vector2(1040f, 48f), "Pilih tingkat kesulitan sebelum memasuki Hutan Data.", 19, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            select.backButton = AddButton("Back Button", canvasObject.transform, new Vector2(0f, -336f), new Vector2(250f, 56f), "KEMBALI", 18, font, Hex("08181D"), Color.white, activeButton, out _, false);

            BuildDifficultyStartTransition(canvasObject.transform, select, panelFrame, circuitSprite, font);

            EditorSceneManager.SaveScene(scene, DifficultyScenePath);
        }

        private static void AddDifficultyCard(Transform parent, Sprite panelSprite, Sprite buttonSprite, Font font, Vector2 position, string title, string body, Color accent, out Button button)
        {
            GameObject card = AddPanel(title + " Card", parent, position, new Vector2(348f, 256f), new Color(0f, 0f, 0f, 0.78f), panelSprite, 0.92f).gameObject;
            AddText(title + " Title", card.transform, new Vector2(0f, 78f), new Vector2(280f, 42f), title, 30, accent, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText(title + " Body", card.transform, new Vector2(0f, 6f), new Vector2(286f, 82f), body, 16, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            button = AddButton(title + " Button", card.transform, new Vector2(0f, -84f), new Vector2(218f, 54f), "PILIH", 17, font, Hex("08181D"), Color.white, buttonSprite, out _, false);
        }

        private static void CreateMainMenuVideoBackground(Transform parent, Sprite fallbackSprite)
        {
            AssetDatabase.ImportAsset(MainMenuVideoPath, ImportAssetOptions.ForceUpdate);
            VideoClip clip = AssetDatabase.LoadAssetAtPath<VideoClip>(MainMenuVideoPath);
            if (clip == null)
            {
                return;
            }

            RenderTexture renderTexture = AssetDatabase.LoadAssetAtPath<RenderTexture>(MainMenuRenderTexturePath);
            if (renderTexture == null)
            {
                Directory.CreateDirectory(ToAbsolutePath("Assets/CyberGuardian/Art/Menu"));
                renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32)
                {
                    name = "MainMenuVideoBackground",
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear,
                    useMipMap = false
                };
                AssetDatabase.CreateAsset(renderTexture, MainMenuRenderTexturePath);
            }

            GameObject rawObject = new GameObject("Menu Video Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            rawObject.transform.SetParent(parent, false);
            RectTransform rawRect = rawObject.GetComponent<RectTransform>();
            rawRect.anchorMin = Vector2.zero;
            rawRect.anchorMax = Vector2.one;
            rawRect.offsetMin = Vector2.zero;
            rawRect.offsetMax = Vector2.zero;
            RawImage rawImage = rawObject.GetComponent<RawImage>();
            rawImage.texture = renderTexture;
            rawImage.color = Color.white;

            GameObject playerObject = new GameObject("Menu Background Video Player", typeof(VideoPlayer), typeof(AudioSource));
            VideoPlayer videoPlayer = playerObject.GetComponent<VideoPlayer>();
            AudioSource audioSource = playerObject.GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.20f;
            audioSource.loop = true;
            videoPlayer.clip = clip;
            videoPlayer.playOnAwake = true;
            videoPlayer.isLooping = true;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);

            if (fallbackSprite != null)
            {
                RawImage vignette = AddStretchRawImage("Menu Video Vignette", parent, new Color(0f, 0f, 0f, 0.16f));
                vignette.raycastTarget = false;
            }
        }

        private static void BuildMenuInfoPanels(Transform parent, CyberGuardianMainMenu menu, Sprite panelSprite, Sprite buttonSprite, Font font)
        {
            GameObject settingsPanel = CreateOverlayPanel("Settings Overlay", parent);
            AddStretchImage("Settings Dim", settingsPanel.transform, new Color(0f, 0f, 0f, 0.58f), panelSprite);
            AddPanel("Settings Window", settingsPanel.transform, new Vector2(0f, 0f), new Vector2(680f, 420f), Color.black, panelSprite, 0.92f);
            AddText("Settings Title", settingsPanel.transform, new Vector2(0f, 146f), new Vector2(520f, 48f), "PENGATURAN", 34, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Settings Audio", settingsPanel.transform, new Vector2(0f, 74f), new Vector2(500f, 34f), "AUDIO: AKTIF", 20, Hex("69F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Settings Control", settingsPanel.transform, new Vector2(0f, 22f), new Vector2(600f, 34f), "GERAK: A/D   LOMPAT: SPACE   SERANG: J   ENERGI: SHIFT", 17, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Settings Ranged", settingsPanel.transform, new Vector2(0f, -30f), new Vector2(600f, 34f), "TEMBAK: L ATAU KLIK KIRI   PAUSE: P / ESC", 17, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Settings Note", settingsPanel.transform, new Vector2(0f, -88f), new Vector2(580f, 46f), "Blok kuis membuka jalur serangan saat melawan bos.", 16, Hex("FF6AA7"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Settings Difficulty Note", settingsPanel.transform, new Vector2(0f, -138f), new Vector2(580f, 34f), "Kesulitan dipilih setelah tombol MULAI.", 16, Hex("69F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            menu.settingsBackButton = AddButton("Settings Back Button", settingsPanel.transform, new Vector2(0f, -184f), new Vector2(220f, 52f), "KEMBALI", 18, font, Hex("08181D"), Color.white, buttonSprite, out _, false);
            menu.settingsPanel = settingsPanel;
            settingsPanel.SetActive(false);

            GameObject creditsPanel = CreateOverlayPanel("Credits Overlay", parent);
            AddStretchImage("Credits Dim", creditsPanel.transform, new Color(0f, 0f, 0f, 0.58f), panelSprite);
            AddPanel("Credits Window", creditsPanel.transform, new Vector2(0f, 0f), new Vector2(680f, 350f), Color.black, panelSprite, 0.92f);
            AddText("Credits Title", creditsPanel.transform, new Vector2(0f, 112f), new Vector2(540f, 48f), "KREDIT", 34, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Credits Line A", creditsPanel.transform, new Vector2(0f, 44f), new Vector2(560f, 34f), "PROTOTYPE CYBER GUARDIAN", 20, Hex("69F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Credits Line B", creditsPanel.transform, new Vector2(0f, -4f), new Vector2(560f, 34f), "PEMBANGUN ADEGAN UNITY DAN ASSET UI CYBER", 17, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Credits Line C", creditsPanel.transform, new Vector2(0f, -52f), new Vector2(560f, 34f), "TEMA: PETUALANGAN SAMPING DAN KUIS KEAMANAN CYBER", 17, Hex("FF6AA7"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            menu.creditsBackButton = AddButton("Credits Back Button", creditsPanel.transform, new Vector2(0f, -130f), new Vector2(220f, 52f), "KEMBALI", 18, font, Hex("08181D"), Color.white, buttonSprite, out _, false);
            menu.creditsPanel = creditsPanel;
            creditsPanel.SetActive(false);
        }

        private static void BuildMenuStartTransition(Transform parent, CyberGuardianMainMenu menu, Sprite panelSprite, Sprite circuitSprite, Font font)
        {
            GameObject transitionPanel = CreateOverlayPanel("Start Transition Overlay", parent);
            Image fade = AddStretchImage("Start Transition Fade", transitionPanel.transform, new Color(0f, 0f, 0f, 0f), panelSprite);
            fade.raycastTarget = true;
            if (circuitSprite != null)
            {
                Image circuit = AddStretchImage("Start Transition Circuit Sweep", transitionPanel.transform, new Color(0.20f, 0.95f, 1f, 0.0f), circuitSprite);
                circuit.type = Image.Type.Tiled;
                circuit.pixelsPerUnitMultiplier = 0.26f;
                menu.startTransitionCircuit = circuit;
            }

            menu.startTransitionText = AddText("Start Transition Text", transitionPanel.transform, new Vector2(0f, 28f), new Vector2(920f, 88f), "MEMBUKA PILIHAN KESULITAN", 36, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            menu.startTransitionText.color = new Color(1f, 1f, 1f, 0f);

            Image[] effects = new Image[8];
            for (int i = 0; i < effects.Length; i++)
            {
                float y = -210f + i * 60f;
                float x = i % 2 == 0 ? -1080f : 1080f;
                Image scanline = AddImage("Start Transition Scanline " + i, transitionPanel.transform, new Vector2(x, y), new Vector2(520f, 7f), i % 2 == 0 ? Hex("61F7FF") : Hex("FF3B88"), panelSprite);
                scanline.raycastTarget = false;
                scanline.color = new Color(scanline.color.r, scanline.color.g, scanline.color.b, 0f);
                effects[i] = scanline;
            }

            menu.startTransitionOverlay = transitionPanel;
            menu.startTransitionFade = fade;
            menu.startTransitionFx = effects;
            transitionPanel.SetActive(false);
        }

        private static void BuildDifficultyStartTransition(Transform parent, CyberGuardianDifficultySelect select, Sprite panelSprite, Sprite circuitSprite, Font font)
        {
            GameObject transitionPanel = CreateOverlayPanel("Difficulty Start Transition Overlay", parent);
            Image fade = AddStretchImage("Difficulty Start Transition Fade", transitionPanel.transform, new Color(0f, 0f, 0f, 0f), panelSprite);
            fade.raycastTarget = true;
            if (circuitSprite != null)
            {
                Image circuit = AddStretchImage("Difficulty Start Circuit Sweep", transitionPanel.transform, new Color(0.20f, 0.95f, 1f, 0.0f), circuitSprite);
                circuit.type = Image.Type.Tiled;
                circuit.pixelsPerUnitMultiplier = 0.26f;
                select.startTransitionCircuit = circuit;
            }

            select.startTransitionText = AddText("Difficulty Start Transition Text", transitionPanel.transform, new Vector2(0f, 28f), new Vector2(920f, 88f), "MASUK KE HUTAN DATA", 36, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            select.startTransitionText.color = new Color(1f, 1f, 1f, 0f);

            Image[] effects = new Image[8];
            for (int i = 0; i < effects.Length; i++)
            {
                float y = -210f + i * 60f;
                float x = i % 2 == 0 ? -1080f : 1080f;
                Image scanline = AddImage("Difficulty Start Scanline " + i, transitionPanel.transform, new Vector2(x, y), new Vector2(520f, 7f), i % 2 == 0 ? Hex("61F7FF") : Hex("FF3B88"), panelSprite);
                scanline.raycastTarget = false;
                scanline.color = new Color(scanline.color.r, scanline.color.g, scanline.color.b, 0f);
                effects[i] = scanline;
            }

            select.startTransitionOverlay = transitionPanel;
            select.startTransitionFade = fade;
            select.startTransitionFx = effects;
            transitionPanel.SetActive(false);
        }

        private static GameObject CreateOverlayPanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return panel;
        }

        private static void BuildLevelScene(
            Sprite squareSprite,
            Sprite circleSprite,
            Sprite panelSprite,
            Sprite rockTileSprite,
            Sprite metalCrateSprite,
            Sprite dataMossSprite,
            Sprite sawBladeSprite,
            Sprite dataBlobSprite,
            CyberHorrorAssetSprites horrorSprites,
            Sprite virusSprite,
            Sprite virusAltSprite,
            Sprite projectileSprite,
            Sprite sparkSprite,
            Sprite crosshairSprite,
            Sprite frameSprite,
            Sprite buttonSprite,
            Sprite circuitSprite,
            QuizQuestionBank questionBank,
            DifficultyProfile[] difficulties,
            Font font,
            GameObject bossProjectilePrefab,
            AudioClip meleeSfx,
            AudioClip hitSfx,
            AudioClip bossShotSfx,
            AudioClip shieldSfx,
            AudioClip wrongSfx)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CyberGuardian_Level01";

            Camera camera = CreateCamera("0A2026", 5.4f, new Vector3(0f, 2.15f, -10f));
            EnsureEventSystem();

            GameObject world = new GameObject("World");
            CyberGuardianSideScrollerGame game = new GameObject("Cyber Guardian Side Scroller Game").AddComponent<CyberGuardianSideScrollerGame>();
            game.quizQuestionBank = questionBank;
            game.difficultyProfiles = difficulties;
            game.gameplayCamera = camera;
            game.deathShardSprite = squareSprite;
            game.startingRecoveryPoint = new Vector3(-8f, 0.65f, 0f);
            game.bossArenaCenterX = 222.0f;
            game.bossArenaMinX = 215.0f;
            game.bossArenaMaxX = 226.5f;
            game.slingshotMaxPull = 3.25f;
            game.slingshotPower = 11.6f;
            game.projectileMaxFlightTime = 5.2f;
            game.cameraMin = new Vector2(-8.8f, -3.8f);
            game.cameraMax = new Vector2(233.0f, 7.4f);
            game.bossProjectilePrefab = bossProjectilePrefab;
            game.sfxSource = game.gameObject.AddComponent<AudioSource>();
            game.sfxSource.playOnAwake = false;
            game.sfxSource.volume = 0.78f;
            game.meleeSfx = meleeSfx;
            game.hitSfx = hitSfx;
            game.bossShotSfx = bossShotSfx;
            game.shieldSfx = shieldSfx;
            game.wrongSfx = wrongSfx;
            ConfigureGameplayAudio(game, Level01MusicPath);
            game.nextSceneName = "CyberGuardian_Level02";

            Sprite generatedBossSprite = EnsureImportedSprite(BossGeneratedSpritePath);
            Sprite generatedProjectileSprite = EnsureImportedSprite(ProjectileGeneratedSpritePath);

            BuildBackground(world.transform, squareSprite, circuitSprite, dataBlobSprite, horrorSprites);
            BuildCyberAmbientEffects(world.transform, squareSprite, dataBlobSprite, false);
            BuildPlatforms(world.transform, squareSprite, panelSprite, rockTileSprite, metalCrateSprite, dataMossSprite, horrorSprites);
            BuildAdventureActors(world.transform, game, squareSprite, circleSprite);
            BuildHazards(world.transform, game, squareSprite, circleSprite, sawBladeSprite, metalCrateSprite, horrorSprites);
            BuildStoryZones(world.transform, game, false);
            BuildPowerUps(world.transform, game, squareSprite, circleSprite, false);
            BuildUnfairChallengeLayer(world.transform, game, squareSprite, circleSprite, metalCrateSprite, dataMossSprite, horrorSprites, false);
            BuildBossArena(world.transform, game, squareSprite, circleSprite, metalCrateSprite, horrorSprites, virusSprite, virusAltSprite, generatedBossSprite, projectileSprite, generatedProjectileSprite, sparkSprite, crosshairSprite, font);
            BuildHud(game, panelSprite, buttonSprite != null ? buttonSprite : panelSprite, frameSprite, horrorSprites, font);

            EditorSceneManager.SaveScene(scene, LevelScenePath);
        }

        private static void BuildLevel02Scene(
            Sprite squareSprite,
            Sprite circleSprite,
            Sprite panelSprite,
            Sprite rockTileSprite,
            Sprite metalCrateSprite,
            Sprite dataMossSprite,
            Sprite sawBladeSprite,
            Sprite dataBlobSprite,
            CyberHorrorAssetSprites horrorSprites,
            Sprite virusSprite,
            Sprite virusAltSprite,
            Sprite projectileSprite,
            Sprite sparkSprite,
            Sprite crosshairSprite,
            Sprite frameSprite,
            Sprite buttonSprite,
            Sprite circuitSprite,
            QuizQuestionBank questionBank,
            DifficultyProfile[] difficulties,
            Font font,
            GameObject bossProjectilePrefab,
            AudioClip meleeSfx,
            AudioClip hitSfx,
            AudioClip bossShotSfx,
            AudioClip shieldSfx,
            AudioClip wrongSfx)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CyberGuardian_Level02";

            Camera camera = CreateCamera("041018", 5.25f, new Vector3(0f, 2.2f, -10f));
            EnsureEventSystem();

            GameObject world = new GameObject("World HD 2.5D");
            CyberGuardianSideScrollerGame game = new GameObject("Cyber Guardian Side Scroller Game").AddComponent<CyberGuardianSideScrollerGame>();
            game.quizQuestionBank = questionBank;
            game.difficultyProfiles = difficulties;
            game.gameplayCamera = camera;
            game.deathShardSprite = squareSprite;
            game.startingRecoveryPoint = new Vector3(-8f, 0.75f, 0f);
            game.bossArenaCenterX = 274.0f;
            game.bossArenaMinX = 266.0f;
            game.bossArenaMaxX = 279.0f;
            game.slingshotMaxPull = 3.35f;
            game.slingshotPower = 11.8f;
            game.projectileMaxFlightTime = 5.4f;
            game.cameraMin = new Vector2(-8.8f, -3.9f);
            game.cameraMax = new Vector2(288.0f, 7.8f);
            game.bossProjectilePrefab = bossProjectilePrefab;
            game.sfxSource = game.gameObject.AddComponent<AudioSource>();
            game.sfxSource.playOnAwake = false;
            game.sfxSource.volume = 0.78f;
            game.meleeSfx = meleeSfx;
            game.hitSfx = hitSfx;
            game.bossShotSfx = bossShotSfx;
            game.shieldSfx = shieldSfx;
            game.wrongSfx = wrongSfx;
            ConfigureGameplayAudio(game, Level02MusicPath);
            game.nextSceneName = "CyberGuardian_Level03";

            Sprite generatedBossSprite = EnsureImportedSprite(BossGeneratedSpritePath);
            Sprite generatedProjectileSprite = EnsureImportedSprite(ProjectileGeneratedSpritePath);

            BuildLevel02Background(world.transform, camera, squareSprite, circuitSprite, dataBlobSprite, horrorSprites);
            BuildCyberAmbientEffects(world.transform, squareSprite, dataBlobSprite, true);
            BuildLevel02Platforms(world.transform, squareSprite, rockTileSprite, metalCrateSprite, dataMossSprite, horrorSprites);
            BuildAdventureActors(world.transform, game, squareSprite, circleSprite);
            BuildLevel02Hazards(world.transform, game, squareSprite, circleSprite, sawBladeSprite, horrorSprites);
            BuildStoryZones(world.transform, game, true);
            BuildPowerUps(world.transform, game, squareSprite, circleSprite, true);
            BuildUnfairChallengeLayer(world.transform, game, squareSprite, circleSprite, metalCrateSprite, dataMossSprite, horrorSprites, true);
            BuildBossArena(world.transform, game, squareSprite, circleSprite, metalCrateSprite, horrorSprites, virusSprite, virusAltSprite, generatedBossSprite, projectileSprite, generatedProjectileSprite, sparkSprite, crosshairSprite, font);
            BuildHud(game, panelSprite, buttonSprite != null ? buttonSprite : panelSprite, frameSprite, horrorSprites, font);

            EditorSceneManager.SaveScene(scene, Level02ScenePath);
        }

        private static void BuildLevel03Scene(
            Sprite squareSprite,
            Sprite circleSprite,
            Sprite panelSprite,
            Sprite rockTileSprite,
            Sprite metalCrateSprite,
            Sprite dataMossSprite,
            Sprite sawBladeSprite,
            Sprite dataBlobSprite,
            CyberHorrorAssetSprites horrorSprites,
            Sprite virusSprite,
            Sprite virusAltSprite,
            Sprite projectileSprite,
            Sprite sparkSprite,
            Sprite crosshairSprite,
            Sprite frameSprite,
            Sprite buttonSprite,
            Sprite circuitSprite,
            QuizQuestionBank questionBank,
            DifficultyProfile[] difficulties,
            Font font,
            GameObject bossProjectilePrefab,
            AudioClip meleeSfx,
            AudioClip hitSfx,
            AudioClip bossShotSfx,
            AudioClip shieldSfx,
            AudioClip wrongSfx)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CyberGuardian_Level03";

            Camera camera = CreateCamera("02070D", 5.35f, new Vector3(0f, 2.35f, -10f));
            EnsureEventSystem();

            GameObject world = new GameObject("World Cloud Core Uplink");
            CyberGuardianSideScrollerGame game = new GameObject("Cyber Guardian Side Scroller Game").AddComponent<CyberGuardianSideScrollerGame>();
            game.quizQuestionBank = questionBank;
            game.difficultyProfiles = difficulties;
            game.gameplayCamera = camera;
            game.deathShardSprite = squareSprite;
            game.startingRecoveryPoint = new Vector3(-8f, 0.90f, 0f);
            game.bossArenaCenterX = 348.0f;
            game.bossArenaMinX = 337.0f;
            game.bossArenaMaxX = 357.0f;
            game.slingshotMaxPull = 3.85f;
            game.slingshotPower = 13.2f;
            game.projectileMaxFlightTime = 6.3f;
            game.cameraMin = new Vector2(-8.8f, -4.3f);
            game.cameraMax = new Vector2(366.0f, 10.2f);
            game.aerialBossEncounter = true;
            game.bossAttackIntervalScale = 0.72f;
            game.bossVolleyCount = 4;
            game.bossProjectileSpeedBonus = 1.25f;
            game.bossProjectilePrefab = bossProjectilePrefab;
            game.sfxSource = game.gameObject.AddComponent<AudioSource>();
            game.sfxSource.playOnAwake = false;
            game.sfxSource.volume = 0.82f;
            game.meleeSfx = meleeSfx;
            game.hitSfx = hitSfx;
            game.bossShotSfx = bossShotSfx;
            game.shieldSfx = shieldSfx;
            game.wrongSfx = wrongSfx;
            ConfigureGameplayAudio(game, Level03MusicPath);

            Sprite generatedBossSprite = EnsureImportedSprite(BossGeneratedSpritePath);
            Sprite generatedProjectileSprite = EnsureImportedSprite(ProjectileGeneratedSpritePath);

            BuildLevel03Background(world.transform, squareSprite, circuitSprite, dataBlobSprite, horrorSprites);
            BuildCyberAmbientEffects(world.transform, squareSprite, dataBlobSprite, true);
            BuildLevel03Platforms(world.transform, squareSprite, rockTileSprite, metalCrateSprite, dataMossSprite, horrorSprites);
            BuildAdventureActors(world.transform, game, squareSprite, circleSprite);
            BuildLevel03Hazards(world.transform, game, squareSprite, circleSprite, sawBladeSprite, horrorSprites);
            BuildLevel03StoryZones(world.transform, game);
            BuildLevel03PowerUps(world.transform, game, circleSprite);
            BuildLevel03UnfairChallengeLayer(world.transform, game, squareSprite, circleSprite, metalCrateSprite, horrorSprites);
            BuildAerialBossArena(world.transform, game, squareSprite, circleSprite, metalCrateSprite, horrorSprites, virusSprite, virusAltSprite, generatedBossSprite, projectileSprite, generatedProjectileSprite, sparkSprite, crosshairSprite, font);
            BuildHud(game, panelSprite, buttonSprite != null ? buttonSprite : panelSprite, frameSprite, horrorSprites, font);

            EditorSceneManager.SaveScene(scene, Level03ScenePath);
        }

        private static void BuildLevel03Background(Transform parent, Sprite squareSprite, Sprite circuitSprite, Sprite dataBlobSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateWorldSprite("L03 Cloud Core Back Wall", parent, new Vector2(178f, 2.2f), new Vector2(392f, 22f), Hex("01060A"), squareSprite, 0);

            for (int i = 0; i < 13; i++)
            {
                float x = -8f + i * 31.5f;
                Sprite backdrop = i % 3 == 0 ? horrorSprites.CodeAbyssBackground : (i % 3 == 1 ? horrorSprites.ServerRunsBackground : horrorSprites.DataForestBackground);
                Color tint = i % 3 == 0
                    ? new Color(0.12f, 0.72f, 0.94f, 0.28f)
                    : (i % 3 == 1 ? new Color(0.86f, 0.08f, 0.58f, 0.22f) : new Color(0.18f, 0.95f, 0.72f, 0.22f));
                CreateWorldSprite("L03 Parallax Cloud Core Panel " + i, parent, new Vector2(x, 2.8f + (i % 2) * 0.45f), new Vector2(34f, 14f), tint, backdrop != null ? backdrop : dataBlobSprite, 1);
            }

            if (circuitSprite != null)
            {
                for (int i = 0; i < 58; i++)
                {
                    float x = -13f + i * 6.6f;
                    float y = 5.1f + Mathf.Sin(i * 0.75f) * 1.1f;
                    CreateWorldSprite("L03 Data Circuit Vein " + i, parent, new Vector2(x, y), new Vector2(6.9f, 2.45f), new Color(0.16f, 0.95f, 1f, 0.065f), circuitSprite, 3);
                }
            }

            for (int i = 0; i < 72; i++)
            {
                float x = -13f + i * 5.3f;
                float y = i % 2 == 0 ? 6.35f : -2.85f;
                CreateWorldSprite("L03 Background Packet Rail " + i, parent, new Vector2(x, y), new Vector2(3.8f, 0.12f), new Color(0.2f, 0.92f, 0.95f, 0.17f), squareSprite, 4);
                CreateWorldSprite("L03 Background Packet Node " + i, parent, new Vector2(x + 1.9f, y), new Vector2(0.32f, 0.32f), new Color(0.72f, 1f, 1f, 0.20f), squareSprite, 5);
            }

            for (int i = 0; i < 18; i++)
            {
                float x = 10f + i * 18.8f;
                float y = -2.7f + (i % 4) * 2.25f;
                SpriteRenderer portal = CreateWorldSprite("L03 Rotating Cloud Portal " + i, parent, new Vector2(x, y), new Vector2(1.35f, 1.35f), new Color(0.22f, 0.95f, 1f, 0.13f), dataBlobSprite, 6);
                portal.gameObject.AddComponent<CyberGuardianRotator>().degreesPerSecond = i % 2 == 0 ? 28f : -22f;
                AddPulse(portal, 0.08f, 0.12f, 2.3f, i * 0.31f);
            }
        }

        private static void BuildLevel03Platforms(Transform parent, Sprite squareSprite, Sprite rockTileSprite, Sprite metalCrateSprite, Sprite dataMossSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateCyberPlatform("L03 Entry Cloud Cache Floor", parent, new Vector2(-3.8f, -0.92f), 18, 3, rockTileSprite, metalCrateSprite, dataMossSprite, false);

            Vector2[] mainRoute =
            {
                new Vector2(8.6f, -0.42f), new Vector2(18.5f, 0.92f), new Vector2(29.4f, 2.36f), new Vector2(41.4f, 1.52f),
                new Vector2(55.0f, 0.42f), new Vector2(69.2f, 1.92f), new Vector2(84.2f, 3.38f), new Vector2(99.4f, 2.28f),
                new Vector2(116.4f, 0.52f), new Vector2(134.5f, 2.68f), new Vector2(153.2f, 4.12f), new Vector2(173.2f, 2.05f),
                new Vector2(194.4f, 0.78f), new Vector2(216.8f, 2.88f), new Vector2(240.2f, 4.38f), new Vector2(263.8f, 2.00f),
                new Vector2(287.5f, 0.82f), new Vector2(311.8f, 2.42f), new Vector2(329.5f, 1.16f)
            };
            int[] widths = { 6, 5, 6, 7, 8, 5, 6, 7, 8, 5, 6, 7, 8, 5, 6, 7, 8, 6, 6 };

            for (int i = 0; i < mainRoute.Length; i++)
            {
                Sprite tile = i % 4 == 0 ? horrorSprites.ServerCore : (i % 4 == 1 ? horrorSprites.CircuitBlock : (i % 4 == 2 ? horrorSprites.MetaPanel : horrorSprites.DataStone));
                Color edge = i % 3 == 0 ? Hex("61F7FF") : (i % 3 == 1 ? Hex("FF3B88") : Hex("7DFF9B"));
                CreateHorrorPlatform("L03 Main Cloud Route Segment " + i, parent, mainRoute[i], widths[i], 1, tile, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, edge);
            }

            CreateHorrorPlatform("L03 Low Decoy Malware Rail A", parent, new Vector2(20.4f, -3.55f), 9, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L03 Low Decoy Malware Rail B", parent, new Vector2(43.8f, -3.85f), 8, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L03 Low Decoy Malware Rail C", parent, new Vector2(91.6f, -3.42f), 9, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L03 Low Recovery Rail D", parent, new Vector2(126.0f, -2.92f), 8, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("61F7FF"));
            CreateHorrorPlatform("L03 Low Trap Rail E", parent, new Vector2(185.2f, -3.55f), 10, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L03 Low Correct Rejoin Rail F", parent, new Vector2(293.0f, -1.90f), 9, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("61F7FF"));

            CreateMovingPlatform("L03 Async Lift A", parent, new Vector2(62.2f, -0.45f), new Vector3(0f, 2.35f, 0f), horrorSprites.GlowEdgePlatform, metalCrateSprite);
            CreateMovingPlatform("L03 Packet Elevator B", parent, new Vector2(107.0f, 0.35f), new Vector3(0f, 2.75f, 0f), horrorSprites.NeonPlatform, metalCrateSprite);
            CreateMovingPlatform("L03 Cloud Shard Lift C", parent, new Vector2(143.8f, 1.42f), new Vector3(0f, 2.40f, 0f), horrorSprites.GlowEdgePlatform, metalCrateSprite);
            CreateMovingPlatform("L03 Zero Trust Elevator D", parent, new Vector2(205.6f, 0.72f), new Vector3(0f, 2.95f, 0f), horrorSprites.NeonPlatform, metalCrateSprite);
            CreateMovingPlatform("L03 Boss Approach Drift Platform", parent, new Vector2(321.2f, 2.02f), new Vector3(0f, 1.95f, 0f), horrorSprites.GlowEdgePlatform, metalCrateSprite);

            CreateHorrorPlatform("L03 High MFA Cloud Span", parent, new Vector2(76.2f, 5.15f), 7, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("61F7FF"));
            CreateHorrorPlatform("L03 High Audit Trail Span", parent, new Vector2(151.5f, 5.85f), 7, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("7DFF9B"));
            CreateHorrorPlatform("L03 High Backup Mirror Span", parent, new Vector2(229.5f, 6.15f), 8, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("61F7FF"));
            CreateHorrorPlatform("L03 High Boss Key Span", parent, new Vector2(306.2f, 5.20f), 6, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("FFD85E"));
        }

        private static void BuildLevel03Hazards(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, Sprite sawBladeSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateCheckpoint("L03 Entry Recovery Node", parent, game, new Vector2(-6.8f, 0.95f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L03 Hash Bridge Recovery Node", parent, game, new Vector2(29.4f, 2.80f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L03 Token Vault Recovery Node", parent, game, new Vector2(76.2f, 5.45f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L03 Incident Response Recovery Node", parent, game, new Vector2(126.0f, -2.38f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L03 SIEM Spine Recovery Node", parent, game, new Vector2(153.2f, 4.62f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L03 Backup Mirror Recovery Node", parent, game, new Vector2(229.5f, 6.52f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L03 Encryption Stair Recovery Node", parent, game, new Vector2(263.8f, 2.46f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L03 Boss Approach Recovery Node", parent, game, new Vector2(329.5f, 1.60f), squareSprite, horrorSprites.ElectricNode);
            CreateRecoveryZone("L03 Global Cloud Abyss Recovery Field", parent, game, new Vector2(178f, -8.9f), new Vector2(392f, 1.6f));

            CreateSawTrap("L03 Entry Upload Saw", parent, game, new Vector2(9.4f, 0.42f), 0.92f, 22, sawBladeSprite, squareSprite);
            CreateLaserBarrier("L03 Hash Bridge Laser Stack", parent, game, new Vector2(29.4f, 2.95f), 2.25f, 4, 22, squareSprite);
            CreateSpikeTrap("L03 Low Decoy Spike Fan A", parent, game, new Vector2(20.4f, -3.10f), 3.2f, 22, horrorSprites.SpikeBlock, squareSprite);
            CreateGlitchMine("L03 Low Decoy Glitch Mine A", parent, game, new Vector2(43.8f, -3.30f), 24, horrorSprites.GlitchMine, squareSprite);
            CreateSwingingSawTrap("L03 Token Vault Hanging Saw A", parent, game, new Vector2(69.2f, 5.45f), 1.18f, 2.75f, 31, sawBladeSprite, squareSprite, 0.2f);
            CreateVirusTurret("L03 Token Vault Virus Turret", parent, game, new Vector2(84.2f, 4.10f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateCrushingBlock("L03 Incident Crusher A", parent, game, new Vector2(99.4f, 3.70f), new Vector3(0f, -1.75f, 0f), 28, horrorSprites.CrushingBlock, squareSprite);
            CreateSwingingSawTrap("L03 SIEM Pendulum Saw B", parent, game, new Vector2(134.5f, 5.88f), 1.16f, 2.50f, 32, sawBladeSprite, squareSprite, 0.85f);
            CreateLaserBarrier("L03 SIEM Spine Laser", parent, game, new Vector2(153.2f, 4.78f), 2.65f, 4, 25, squareSprite);
            CreateGlitchMine("L03 Low Response Glitch Mine", parent, game, new Vector2(185.2f, -3.00f), 25, horrorSprites.GlitchMine, squareSprite);
            CreateCrushingBlock("L03 Backup Mirror Crusher", parent, game, new Vector2(216.8f, 4.30f), new Vector3(0f, -1.92f, 0f), 30, horrorSprites.CrushingBlock, squareSprite);
            CreateSwingingSawTrap("L03 Backup Mirror Hanging Saw C", parent, game, new Vector2(240.2f, 7.25f), 1.22f, 2.85f, 33, sawBladeSprite, squareSprite, 1.7f);
            CreateLaserBarrier("L03 Encryption Stair Laser", parent, game, new Vector2(263.8f, 2.82f), 2.55f, 4, 27, squareSprite);
            CreateVirusTurret("L03 Boss Gate High Virus Turret", parent, game, new Vector2(306.2f, 5.80f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateSwingingSawTrap("L03 Boss Gate Final Pendulum", parent, game, new Vector2(321.8f, 5.45f), 1.26f, 2.55f, 35, sawBladeSprite, squareSprite, 2.4f);
            CreateSawTrap("L03 Boss Gate Floor Saw", parent, game, new Vector2(331.5f, 0.98f), 1.0f, 32, sawBladeSprite, squareSprite);

            CreateEnemy("L03 Virus Soldier Hash A", parent, game, new Vector2(29.4f, 3.18f), squareSprite, circleSprite, 2.95f, 1.8f);
            CreateEnemy("L03 Virus Soldier Token B", parent, game, new Vector2(84.2f, 4.16f), squareSprite, circleSprite, 3.15f, 1.7f);
            CreateEnemy("L03 Virus Soldier Response C", parent, game, new Vector2(126.0f, -2.24f), squareSprite, circleSprite, 3.20f, 2.1f);
            CreateEnemy("L03 Virus Soldier SIEM D", parent, game, new Vector2(153.2f, 4.82f), squareSprite, circleSprite, 3.35f, 1.8f);
            CreateEnemy("L03 Virus Soldier Backup E", parent, game, new Vector2(229.5f, 6.62f), squareSprite, circleSprite, 3.45f, 1.6f);
            CreateEnemy("L03 Virus Soldier Encryption F", parent, game, new Vector2(263.8f, 2.58f), squareSprite, circleSprite, 3.55f, 1.7f);
            CreateEnemy("L03 Virus Soldier Boss Gate G", parent, game, new Vector2(329.5f, 1.68f), squareSprite, circleSprite, 3.65f, 1.4f);
        }

        private static void BuildLevel03StoryZones(Transform parent, CyberGuardianSideScrollerGame game)
        {
            CreateStoryZone("L03 Story Start", parent, game, new Vector2(-6.2f, 1.1f), new Vector2(2.0f, 4.4f), "ADEGAN 03: UPLINK INTI CLOUD", "Cyber Guardian mencapai lapisan cloud tempat session curian, API terbuka, dan token akses lemah digabung menjadi satu pikiran malware raksasa.");
            CreateStoryZone("L03 Story Tokens", parent, game, new Vector2(29.4f, 2.7f), new Vector2(2.0f, 6.8f), "TOKEN SESSION", "Token session seperti kunci sementara. Jika dicuri, penyerang bisa melewati password, jadi aplikasi aman harus melindungi token, memberi masa berlaku, dan tidak membocorkannya di log.");
            CreateStoryZone("L03 Story API", parent, game, new Vector2(76.2f, 4.6f), new Vector2(2.0f, 7.0f), "GERBANG API", "API perlu autentikasi, batas akses, validasi, dan monitoring. Satu endpoint terbuka bisa menjadi terowongan menuju seluruh sistem.");
            CreateStoryZone("L03 Story Incident", parent, game, new Vector2(126.0f, -1.4f), new Vector2(2.0f, 6.6f), "RESPONS INSIDEN", "Saat serangan terdeteksi, defender mengisolasi sistem terdampak, menyimpan bukti, mengganti secret, menambal celah masuk, dan berkomunikasi jelas.");
            CreateStoryZone("L03 Story SIEM", parent, game, new Vector2(153.2f, 3.8f), new Vector2(2.0f, 7.2f), "TULANG PUNGGUNG SIEM", "SIEM menghubungkan alert dari endpoint, server, dan jaringan. Korelasi membantu defender melihat pola serangan, bukan sekadar noise yang terpisah.");
            CreateStoryZone("L03 Story Phishing", parent, game, new Vector2(194.4f, 1.4f), new Vector2(2.0f, 7.0f), "REKAYASA SOSIAL", "Tidak semua serangan dimulai dari kode. Phishing memanfaatkan rasa percaya, panik, dan tergesa-gesa, jadi perlambat langkah dan verifikasi sebelum memasukkan secret.");
            CreateStoryZone("L03 Story Backups", parent, game, new Vector2(229.5f, 5.4f), new Vector2(2.0f, 7.2f), "CERMIN BACKUP", "Backup harus diuji, dipisahkan, dan dilindungi. Backup yang bisa dihapus malware bukan rencana pemulihan yang aman.");
            CreateStoryZone("L03 Story Encryption", parent, game, new Vector2(263.8f, 1.6f), new Vector2(2.0f, 7.0f), "TANGGA ENKRIPSI", "Enkripsi melindungi data saat dikirim dan disimpan, tetapi kuncinya harus dijaga. Kunci yang bocor membuat enkripsi kuat terasa seperti pintu terbuka.");
            CreateStoryZone("L03 Story Boss", parent, game, new Vector2(329.0f, 1.5f), new Vector2(2.0f, 6.2f), "BOS: DIGITAL OVERLORD", "Bos udara ini dilindungi blok kuis yang mengorbit. Setiap jawaban benar menghapus bagian perisai dan membuka jalur tembakan singkat.");
        }

        private static void BuildLevel03PowerUps(Transform parent, CyberGuardianSideScrollerGame game, Sprite circleSprite)
        {
            CreatePowerUp("L03 Entry Boost Cache", parent, game, new Vector2(18.5f, 1.48f), CyberGuardianPowerUpType.Boost, 38, circleSprite, Hex("16E8FF"));
            CreatePowerUp("L03 Token Health Patch", parent, game, new Vector2(76.2f, 5.52f), CyberGuardianPowerUpType.Health, 30, circleSprite, Hex("7DFF9B"));
            CreatePowerUp("L03 Incident Firewall Cache", parent, game, new Vector2(126.0f, -2.22f), CyberGuardianPowerUpType.Firewall, 34, circleSprite, Hex("FF3B88"));
            CreatePowerUp("L03 SIEM Overclock Cache", parent, game, new Vector2(153.2f, 4.78f), CyberGuardianPowerUpType.Overclock, 44, circleSprite, Hex("FFD85E"));
            CreatePowerUp("L03 Backup Boost Cache", parent, game, new Vector2(229.5f, 6.58f), CyberGuardianPowerUpType.Boost, 40, circleSprite, Hex("16E8FF"));
            CreatePowerUp("L03 Encryption Health Cache", parent, game, new Vector2(287.5f, 1.40f), CyberGuardianPowerUpType.Health, 32, circleSprite, Hex("7DFF9B"));
            CreatePowerUp("L03 Boss Gate Overclock Cache", parent, game, new Vector2(329.5f, 1.72f), CyberGuardianPowerUpType.Overclock, 48, circleSprite, Hex("FFD85E"));
        }

        private static void BuildLevel03UnfairChallengeLayer(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, Sprite metalCrateSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateBreakawayPlatform("L03 Token Collapse Tile A", parent, new Vector2(41.4f, 2.18f), new Vector2(1.36f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("L03 API Collapse Tile B", parent, new Vector2(69.2f, 2.64f), new Vector2(1.36f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("L03 SIEM Collapse Tile C", parent, new Vector2(134.5f, 3.28f), new Vector2(1.36f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("L03 Backup Collapse Tile D", parent, new Vector2(216.8f, 3.58f), new Vector2(1.36f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("L03 Boss Key Collapse Tile E", parent, new Vector2(311.8f, 3.10f), new Vector2(1.40f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);

            CreateVirusBeastEnemy("L03 Quad Malware Token A", parent, game, new Vector2(20.4f, -3.06f), squareSprite, circleSprite, 2.45f, 2.3f);
            CreateVirusBeastEnemy("L03 Quad Malware API B", parent, game, new Vector2(55.0f, 1.18f), squareSprite, circleSprite, 2.65f, 1.9f);
            CreateVirusBeastEnemy("L03 Quad Malware Response C", parent, game, new Vector2(91.6f, -2.98f), squareSprite, circleSprite, 2.85f, 2.2f);
            CreateVirusBeastEnemy("L03 Quad Malware SIEM D", parent, game, new Vector2(173.2f, 2.78f), squareSprite, circleSprite, 3.05f, 1.8f);
            CreateVirusBeastEnemy("L03 Quad Malware Backup E", parent, game, new Vector2(240.2f, 5.20f), squareSprite, circleSprite, 3.25f, 1.6f);
            CreateVirusBeastEnemy("L03 Quad Malware Boss Gate F", parent, game, new Vector2(311.8f, 2.95f), squareSprite, circleSprite, 3.45f, 1.6f);

            CreatePacketLaser("L03 Hidden Token Packet Beam", parent, game, new Vector2(69.2f, 3.05f), new Vector2(2.0f, 0.08f), 24, squareSprite);
            CreatePacketLaser("L03 SIEM Hidden Packet Beam", parent, game, new Vector2(153.2f, 5.18f), new Vector2(2.2f, 0.08f), 26, squareSprite);
            CreatePacketLaser("L03 Backup Hidden Packet Beam", parent, game, new Vector2(229.5f, 6.88f), new Vector2(2.4f, 0.08f), 28, squareSprite);
            CreatePacketLaser("L03 Boss Gate Hidden Packet Beam", parent, game, new Vector2(329.5f, 2.35f), new Vector2(2.2f, 0.08f), 30, squareSprite);
        }

        private static void BuildAerialBossArena(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, Sprite metalCrateSprite, CyberHorrorAssetSprites horrorSprites, Sprite virusSprite, Sprite virusAltSprite, Sprite bossGeneratedSprite, Sprite projectileSprite, Sprite projectileGeneratedSprite, Sprite sparkSprite, Sprite crosshairSprite, Font font)
        {
            float triggerX = game.bossArenaMinX - 1.3f;
            float bossX = game.bossArenaMaxX + 4.8f;
            Vector2 bossCenter = new Vector2(bossX, 5.15f);

            GameObject trigger = new GameObject("L03 Aerial Boss Mode Trigger", typeof(BoxCollider2D), typeof(CyberGuardianBossArenaTrigger));
            trigger.transform.SetParent(parent, false);
            trigger.transform.position = new Vector3(triggerX, 2.6f, 0f);
            BoxCollider2D triggerCollider = trigger.GetComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector2(0.65f, 6.4f);
            trigger.GetComponent<CyberGuardianBossArenaTrigger>().game = game;

            CreatePlatform("L03 Boss Left Firewall Gate", parent, new Vector2(game.bossArenaMinX - 0.75f, 2.8f), new Vector2(0.36f, 7.0f), Hex("172B35"), squareSprite);
            CreatePlatform("L03 Boss Right Cloud Wall", parent, new Vector2(game.bossArenaMaxX + 9.0f, 3.0f), new Vector2(0.36f, 7.2f), Hex("172B35"), squareSprite);
            CreateHorrorPlatform("L03 Aerial Boss Lower Dodge Bridge", parent, new Vector2(game.bossArenaCenterX - 1.8f, 0.82f), 14, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, null, Color.white, Hex("61F7FF"));
            CreateHorrorPlatform("L03 Aerial Boss Left Floating Shelf", parent, new Vector2(game.bossArenaMinX + 2.4f, 2.95f), 4, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, null, Color.white, Hex("7DFF9B"));
            CreateHorrorPlatform("L03 Aerial Boss Right Floating Shelf", parent, new Vector2(game.bossArenaMaxX - 2.4f, 3.65f), 4, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, null, Color.white, Hex("FF3B88"));

            SpriteRenderer bossGlow = CreateWorldSprite("L03 Digital Overlord Hit Core", parent, bossCenter, new Vector2(3.2f, 3.2f), new Color(1f, 0.05f, 0.38f, 0.30f), circleSprite, 18);
            bossGlow.gameObject.AddComponent<CyberGuardianBossCore>().game = game;
            CircleCollider2D bossCollider = bossGlow.gameObject.AddComponent<CircleCollider2D>();
            bossCollider.isTrigger = true;
            bossCollider.radius = 1.08f;
            game.bossCore = bossGlow.GetComponent<CyberGuardianBossCore>();
            AddPulse(bossGlow, 0.08f, 0.12f, 3.0f, 0f);

            Sprite bossSprite = bossGeneratedSprite != null ? bossGeneratedSprite : (virusSprite != null ? virusSprite : circleSprite);
            SpriteRenderer bossArt = CreateWorldSprite("L03 Super Giant Airborne Digital Overlord", parent, bossCenter, new Vector2(5.8f, 5.8f), Color.white, bossSprite, 20);
            AddPulse(bossArt, 0.025f, 0.04f, 2.0f, 0.35f);
            AttachGeneratedGlbVisual(BossGlbPath, bossArt.transform, "Generated Airborne Malware Boss GLB Visual", new Vector3(0f, -1.36f, -0.34f), Vector3.one * 1.18f, new Vector3(0f, 180f, 0f));

            CreateLocalSprite("Overlord Left Data Wing", bossArt.transform, new Vector3(-0.70f, 0.10f, 0.04f), new Vector2(1.65f, 0.42f), new Color(1f, 0.06f, 0.44f, 0.72f), squareSprite, 21);
            CreateLocalSprite("Overlord Right Data Wing", bossArt.transform, new Vector3(0.70f, 0.10f, 0.04f), new Vector2(1.65f, 0.42f), new Color(0.18f, 0.95f, 1f, 0.64f), squareSprite, 21);
            CreateLocalSprite("Overlord Crown Node", bossArt.transform, new Vector3(0f, 0.58f, -0.03f), new Vector2(0.56f, 0.56f), Hex("FF3B88"), circleSprite, 23);
            CreateLocalSprite("Overlord Core Eye", bossArt.transform, new Vector3(0f, 0f, -0.05f), new Vector2(0.72f, 0.72f), Hex("FFFFFF"), circleSprite, 24);
            if (virusAltSprite != null)
            {
                CreateWorldSprite("L03 Overlord Mutation Shell", parent, bossCenter + new Vector2(0.18f, 0.34f), new Vector2(2.4f, 2.4f), new Color(1f, 0.18f, 0.46f, 0.58f), virusAltSprite, 22);
            }

            List<Transform> ports = new List<Transform>();
            Vector3[] portOffsets =
            {
                new Vector3(-2.1f, 0.80f, 0f),
                new Vector3(-2.35f, -0.45f, 0f),
                new Vector3(-1.28f, 1.65f, 0f),
                new Vector3(-1.16f, -1.52f, 0f),
                new Vector3(0.15f, 1.95f, 0f)
            };
            for (int i = 0; i < portOffsets.Length; i++)
            {
                Transform port = new GameObject("L03 Aerial Boss Attack Port " + i).transform;
                port.SetParent(bossGlow.transform, false);
                port.localPosition = portOffsets[i];
                ports.Add(port);
                CreateLocalSprite("Attack Port Glow " + i, port, Vector3.zero, new Vector2(0.38f, 0.38f), i % 2 == 0 ? Hex("FF3B88") : Hex("61F7FF"), circleSprite, 24);
            }

            game.bossProjectileSpawn = ports[0];
            game.bossProjectileSpawns = ports.ToArray();

            if (crosshairSprite != null)
            {
                CreateWorldSprite("L03 Boss Arena Crosshair Hint", parent, new Vector2(game.bossArenaMinX + 1.8f, 3.15f), new Vector2(1.2f, 1.2f), new Color(0.6f, 1f, 1f, 0.55f), crosshairSprite, 6);
            }

            Sprite quizBlockSprite = horrorSprites != null && horrorSprites.QuizBlock != null ? horrorSprites.QuizBlock : (horrorSprites != null && horrorSprites.CircuitBlock != null ? horrorSprites.CircuitBlock : (metalCrateSprite != null ? metalCrateSprite : squareSprite));
            CreateOrbitingShieldRing("L03 Inner Orbit Quiz Block ", parent, game, bossGlow.transform, 12, 2.95f, 34f, 0.68f, quizBlockSprite, squareSprite, font, 0);
            CreateOrbitingShieldRing("L03 Outer Orbit Quiz Block ", parent, game, bossGlow.transform, 16, 4.25f, -22f, 0.74f, quizBlockSprite, squareSprite, font, 2);

            Sprite projectileActive = projectileGeneratedSprite != null ? projectileGeneratedSprite : (projectileSprite != null ? projectileSprite : circleSprite);
            GameObject projectile = CreateWorldSprite("L03 Patch Core Slingshot Projectile", parent, new Vector2(game.bossArenaMinX + 1.0f, 1.25f), new Vector2(0.62f, 0.62f), new Color(0.72f, 1f, 1f, 1f), projectileActive, 30).gameObject;
            Rigidbody2D projectileBody = projectile.AddComponent<Rigidbody2D>();
            projectileBody.gravityScale = 0.92f;
            projectileBody.simulated = false;
            projectileBody.interpolation = RigidbodyInterpolation2D.Interpolate;
            projectileBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            CircleCollider2D projectileCollider = projectile.AddComponent<CircleCollider2D>();
            projectileCollider.isTrigger = true;
            projectileCollider.enabled = false;
            projectile.AddComponent<CyberGuardianSlingshotProjectile2D>().game = game;
            game.slingshotProjectile = projectile.transform;
            game.slingshotBody = projectileBody;
            game.slingshotCollider = projectileCollider;
            AttachGeneratedGlbVisual(ProjectileGlbPath, projectile.transform, "Generated Patch Core GLB Visual", new Vector3(0f, -0.42f, -0.18f), Vector3.one * 0.25f, new Vector3(0f, 180f, 0f));
            projectile.SetActive(false);

            game.slingshotBandA = CreateLine("L03 Slingshot Band A", parent, new Color(0.55f, 1f, 1f, 0.88f), 0.08f);
            game.slingshotBandB = CreateLine("L03 Slingshot Band B", parent, new Color(0.55f, 1f, 1f, 0.88f), 0.08f);
            game.trajectoryLine = CreateLine("L03 Patch Trajectory", parent, new Color(0.75f, 1f, 1f, 0.60f), 0.05f);
        }

        private static void CreateOrbitingShieldRing(string prefix, Transform parent, CyberGuardianSideScrollerGame game, Transform center, int count, float radius, float speed, float verticalSquash, Sprite blockSprite, Sprite squareSprite, Font font, int categoryOffset)
        {
            for (int i = 0; i < count; i++)
            {
                float phase = i * (360f / count);
                float angle = phase * Mathf.Deg2Rad;
                Vector2 position = (Vector2)center.position + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius * verticalSquash);
                int category = (i + categoryOffset) % 4;
                CyberGuardianBossShieldBlock block = CreateShieldBlock(prefix + i.ToString("00"), parent, game, position, category, blockSprite, squareSprite, font);
                CyberGuardianOrbitingShieldBlock orbit = block.gameObject.AddComponent<CyberGuardianOrbitingShieldBlock>();
                orbit.center = center;
                orbit.radius = radius;
                orbit.angularSpeed = speed;
                orbit.phaseDegrees = phase;
                orbit.verticalSquash = verticalSquash;
                orbit.pulseAmplitude = 0.055f;
                orbit.pulseSpeed = 2.4f + count * 0.05f;
                game.bossBlocks.Add(block);
            }
        }

        private static void BuildBackground(Transform parent, Sprite squareSprite, Sprite circuitSprite, Sprite dataBlobSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateWorldSprite("Back Wall", parent, new Vector2(112f, 1.2f), new Vector2(270f, 18.8f), Hex("02080B"), squareSprite, 0);
            CreateWorldSprite("Data Forest Backdrop", parent, new Vector2(0f, 2.3f), new Vector2(27f, 12f), new Color(0.42f, 0.82f, 0.84f, 0.46f), horrorSprites != null ? horrorSprites.DataForestBackground : dataBlobSprite, 1);
            CreateWorldSprite("Server Runs Backdrop", parent, new Vector2(31f, 2.3f), new Vector2(32f, 12f), new Color(0.36f, 0.72f, 0.9f, 0.42f), horrorSprites != null ? horrorSprites.ServerRunsBackground : dataBlobSprite, 1);
            CreateWorldSprite("Code Abyss Backdrop", parent, new Vector2(62f, 2.2f), new Vector2(31f, 12f), new Color(0.34f, 0.72f, 0.72f, 0.40f), horrorSprites != null ? horrorSprites.CodeAbyssBackground : dataBlobSprite, 1);
            CreateWorldSprite("Firewall Core Backdrop", parent, new Vector2(88f, 2.25f), new Vector2(31f, 12f), new Color(0.72f, 0.18f, 0.54f, 0.38f), horrorSprites != null ? horrorSprites.ServerRunsBackground : dataBlobSprite, 1);
            CreateWorldSprite("Checksum Vault Backdrop", parent, new Vector2(112f, 2.35f), new Vector2(33f, 12f), new Color(0.22f, 0.86f, 0.92f, 0.34f), horrorSprites != null ? horrorSprites.CodeAbyssBackground : dataBlobSprite, 1);
            CreateWorldSprite("Integrity Kernel Backdrop", parent, new Vector2(146f, 2.32f), new Vector2(42f, 12.5f), new Color(0.10f, 0.70f, 0.95f, 0.32f), horrorSprites != null ? horrorSprites.ServerRunsBackground : dataBlobSprite, 1);
            CreateWorldSprite("Zero Trust Core Backdrop", parent, new Vector2(184f, 2.32f), new Vector2(42f, 12.5f), new Color(0.86f, 0.12f, 0.62f, 0.29f), horrorSprites != null ? horrorSprites.CodeAbyssBackground : dataBlobSprite, 1);
            CreateWorldSprite("Boss Firewall Backdrop", parent, new Vector2(222f, 2.42f), new Vector2(42f, 12.5f), new Color(0.30f, 0.95f, 1f, 0.27f), horrorSprites != null ? horrorSprites.DataForestBackground : dataBlobSprite, 1);
            CreateWorldSprite("Aqua Data Sky", parent, new Vector2(12f, 2.8f), new Vector2(24f, 10.5f), new Color(0.05f, 0.78f, 0.88f, 0.12f), dataBlobSprite, 2);
            CreateWorldSprite("Magenta Memory Canopy", parent, new Vector2(37f, 3.2f), new Vector2(18f, 6.2f), new Color(0.95f, 0.05f, 0.54f, 0.10f), dataBlobSprite, 2);
            CreateWorldSprite("Violet Boss Data Cloud", parent, new Vector2(84f, 3.0f), new Vector2(22f, 7.4f), new Color(0.75f, 0.12f, 0.92f, 0.10f), dataBlobSprite, 2);
            if (circuitSprite != null)
            {
                for (int i = 0; i < 38; i++)
                {
                    CreateWorldSprite("Circuit Panel " + i, parent, new Vector2(-11f + i * 7.5f, 3.35f + (i % 2) * 0.8f), new Vector2(7.5f, 2.8f), new Color(0.13f, 0.9f, 0.96f, 0.06f), circuitSprite, 3);
                }
            }

            for (int i = 0; i < 46; i++)
            {
                CreateWorldSprite("Back Data Pipe " + i, parent, new Vector2(-10f + i * 5.8f, 5.35f), new Vector2(4.4f, 0.15f), new Color(0.2f, 0.92f, 0.95f, 0.16f), squareSprite, 4);
                CreateWorldSprite("Back Data Node " + i, parent, new Vector2(-7.8f + i * 5.8f, 5.35f), new Vector2(0.34f, 0.34f), new Color(0.72f, 1f, 1f, 0.18f), squareSprite, 5);
            }
        }

        private static void BuildPlatforms(Transform parent, Sprite squareSprite, Sprite panelSprite, Sprite rockTileSprite, Sprite metalCrateSprite, Sprite dataMossSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateCyberPlatform("Start Data Forest Floor", parent, new Vector2(-3.35f, -1.02f), 17, 3, rockTileSprite, metalCrateSprite, dataMossSprite, false);
            CreateHorrorPlatform("Fork Circuit Bridge", parent, new Vector2(6.1f, -0.72f), 7, 2, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Hex("D9FFFF"), Hex("69F7FF"));
            CreateHorrorPlatform("Upper Route Step A", parent, new Vector2(11.7f, 0.92f), 4, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Upper Route Step B", parent, new Vector2(17.4f, 2.28f), 5, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Upper Server Core Walkway", parent, new Vector2(25.6f, 3.38f), 8, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Upper Correct Route", parent, new Vector2(36.0f, 2.52f), 8, 1, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Upper Descent Route", parent, new Vector2(46.1f, 1.48f), 7, 1, horrorSprites.DataStone, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));

            CreateHorrorPlatform("Middle Safe Route A", parent, new Vector2(14.3f, -0.55f), 5, 1, horrorSprites.DataStone, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Middle Safe Route B", parent, new Vector2(22.0f, 0.10f), 7, 1, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Middle Safe Route C", parent, new Vector2(31.0f, -0.35f), 7, 1, horrorSprites.MetaPanel, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Middle Choice Exit", parent, new Vector2(41.0f, 0.08f), 7, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Boss Approach Ramp A", parent, new Vector2(51.0f, 0.75f), 8, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Boss Approach Ramp B", parent, new Vector2(57.7f, 1.38f), 5, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Data Tower Lower Fork", parent, new Vector2(62.0f, -1.18f), 6, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Upload Step A", parent, new Vector2(64.8f, 0.55f), 4, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Upload Step B", parent, new Vector2(68.3f, 2.05f), 4, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Risk Upper Server Span", parent, new Vector2(72.5f, 3.05f), 5, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("False Drop Recovery Catch", parent, new Vector2(69.0f, -3.38f), 7, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Correct Route To Boss Gate", parent, new Vector2(75.2f, 0.75f), 5, 1, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Deep Packet Vault Bridge", parent, new Vector2(82.0f, 0.42f), 8, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateMovingPlatform("Compression Lift To Hash Tower", parent, new Vector2(88.2f, 0.70f), new Vector3(0f, 1.95f, 0f), horrorSprites.GlowEdgePlatform, metalCrateSprite);
            CreateHorrorPlatform("Hash Tower Checksum Step A", parent, new Vector2(93.2f, 2.32f), 5, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Hash Tower Checksum Step B", parent, new Vector2(99.0f, 3.42f), 5, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Zero Trust Corridor", parent, new Vector2(103.0f, 1.05f), 7, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Boss Gate Decrypt Ramp", parent, new Vector2(106.0f, 0.35f), 4, 1, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Extended Lower Bait Route A", parent, new Vector2(86.0f, -3.25f), 7, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Extended Lower Bait Route B", parent, new Vector2(98.2f, -2.65f), 8, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));

            CreateHorrorPlatform("Lower Trap Route A", parent, new Vector2(12.3f, -3.65f), 7, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Trap Route B", parent, new Vector2(20.6f, -4.25f), 6, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Recovery Route C", parent, new Vector2(29.0f, -3.52f), 7, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Trap Route D", parent, new Vector2(38.2f, -4.05f), 8, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Exit Lift Base", parent, new Vector2(47.2f, -2.20f), 5, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Lower Code Abyss Choice", parent, new Vector2(58.2f, -3.65f), 6, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Upload Exit", parent, new Vector2(73.4f, -1.8f), 5, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));

            CreateHorrorPlatform("Integrity Bridge A", parent, new Vector2(112.6f, 0.62f), 8, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateMovingPlatform("Binary Lift To Integrity Roof", parent, new Vector2(119.4f, 0.82f), new Vector3(0f, 2.05f, 0f), horrorSprites.GlowEdgePlatform, metalCrateSprite);
            CreateHorrorPlatform("Packet Stair Integrity A", parent, new Vector2(126.0f, 2.18f), 5, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Packet Stair Integrity B", parent, new Vector2(132.1f, 3.20f), 4, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Checksum High Span", parent, new Vector2(140.0f, 3.05f), 9, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Secure Descent Cache", parent, new Vector2(149.0f, 1.55f), 7, 1, horrorSprites.DataStone, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Kernel Jump Pillar A", parent, new Vector2(156.0f, 0.10f), 3, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Kernel Jump Pillar B", parent, new Vector2(162.2f, 1.52f), 3, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Kernel Jump Pillar C", parent, new Vector2(169.2f, 2.82f), 4, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Firewall Tunnel Floor", parent, new Vector2(178.2f, 0.42f), 10, 1, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateMovingPlatform("Root Access Lift", parent, new Vector2(186.2f, 0.78f), new Vector3(0f, 2.25f, 0f), horrorSprites.NeonPlatform, metalCrateSprite);
            CreateHorrorPlatform("MFA Upper Span", parent, new Vector2(194.4f, 3.32f), 9, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Last Packet Bridge", parent, new Vector2(205.4f, 1.38f), 7, 1, horrorSprites.MetaPanel, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Final Boss Gate Ramp", parent, new Vector2(212.4f, 0.40f), 5, 1, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Integrity Lower Decoy A", parent, new Vector2(122.8f, -3.22f), 8, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Kernel Panic Lower Decoy", parent, new Vector2(160.8f, -3.36f), 9, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("MFA Lower Trap Bridge", parent, new Vector2(191.4f, -2.70f), 8, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));

            CreateHorrorPlatform("Boss Arena Floor", parent, new Vector2(223.5f, -1.02f), 28, 3, horrorSprites.MetaPanel, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Boss Dodge Platform A", parent, new Vector2(217.2f, 1.45f), 4, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Boss Dodge Platform B", parent, new Vector2(222.7f, 2.55f), 4, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Boss Dodge Platform C", parent, new Vector2(226.6f, 0.95f), 3, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Boss Ceiling Firewall", parent, new Vector2(224.2f, 5.45f), 20, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
        }

        private static void BuildLevel02Background(Transform parent, Camera camera, Sprite squareSprite, Sprite circuitSprite, Sprite dataBlobSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateWorldSprite("L02 Deep Back Wall", parent, new Vector2(138f, 1.35f), new Vector2(330f, 19.4f), Hex("01070B"), squareSprite, 0);

            GameObject farLayer = new GameObject("L02 Far Parallax Data Skyline");
            farLayer.transform.SetParent(parent, false);
            CyberGuardianParallaxLayer farParallax = farLayer.AddComponent<CyberGuardianParallaxLayer>();
            farParallax.targetCamera = camera;
            farParallax.factor = new Vector2(0.13f, 0.04f);

            GameObject midLayer = new GameObject("L02 Mid Parallax Server Glass");
            midLayer.transform.SetParent(parent, false);
            CyberGuardianParallaxLayer midParallax = midLayer.AddComponent<CyberGuardianParallaxLayer>();
            midParallax.targetCamera = camera;
            midParallax.factor = new Vector2(0.28f, 0.08f);

            for (int i = 0; i < 16; i++)
            {
                float x = -14f + i * 20f;
                Sprite bg = i % 3 == 0 ? horrorSprites.DataForestBackground : (i % 3 == 1 ? horrorSprites.ServerRunsBackground : horrorSprites.CodeAbyssBackground);
                SpriteRenderer skyline = CreateWorldSprite("L02 HD Skyline Panel " + i, farLayer.transform, new Vector2(x, 2.45f), new Vector2(24f, 12.8f), new Color(0.32f, 0.95f, 1f, 0.28f), bg != null ? bg : dataBlobSprite, 1);
                AddPulse(skyline, 0.015f, 0.035f, 1.4f, i * 0.45f);
            }

            for (int i = 0; i < 44; i++)
            {
                float x = -12f + i * 7.2f;
                CreateWorldSprite("L02 Parallax Server Rib " + i, midLayer.transform, new Vector2(x, 3.25f + (i % 4) * 0.25f), new Vector2(0.22f, 7.4f), new Color(0.10f, 0.95f, 1f, 0.18f), squareSprite, 3);
                CreateWorldSprite("L02 Parallax Neon Rail " + i, midLayer.transform, new Vector2(x + 2.3f, 5.85f - (i % 2) * 0.45f), new Vector2(4.8f, 0.12f), new Color(0.90f, 0.12f, 0.58f, 0.18f), squareSprite, 4);
            }

            if (circuitSprite != null)
            {
                for (int i = 0; i < 48; i++)
                {
                    CreateWorldSprite("L02 Soft Circuit Glass " + i, parent, new Vector2(-10f + i * 6.5f, -2.7f + (i % 3) * 2.7f), new Vector2(5.8f, 2.2f), new Color(0.20f, 0.95f, 1f, 0.045f), circuitSprite, 5);
                }
            }

            for (int i = 0; i < 24; i++)
            {
                float x = -2f + i * 12.5f;
                SpriteRenderer portal = CreateWorldSprite("L02 Animated Data Portal " + i, parent, new Vector2(x, 3.55f + (i % 2) * 0.5f), new Vector2(2.2f, 2.2f), new Color(0.45f, 1f, 1f, 0.18f), dataBlobSprite, 6);
                AddPulse(portal, 0.055f, 0.08f, 2.2f, i * 0.7f);
            }

            for (int i = 0; i < 24; i++)
            {
                SpriteRenderer foreground = CreateWorldSprite("L02 Foreground Glass Blade " + i, parent, new Vector2(-8f + i * 9.2f, -3.55f), new Vector2(0.42f, 4.4f), new Color(0.05f, 0.95f, 1f, 0.10f), squareSprite, 31);
                foreground.transform.localRotation = Quaternion.Euler(0f, 0f, i % 2 == 0 ? -8f : 8f);
                AddPulse(foreground, 0.02f, 0.04f, 1.8f, i * 0.33f);
            }
        }

        private static void BuildLevel02Platforms(Transform parent, Sprite squareSprite, Sprite rockTileSprite, Sprite metalCrateSprite, Sprite dataMossSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateCyberPlatform("L02 Start HD Data Floor", parent, new Vector2(-3.35f, -1.02f), 18, 3, rockTileSprite, metalCrateSprite, dataMossSprite, true);
            CreateHorrorPlatform("L02 Glass Bridge A", parent, new Vector2(6.7f, -0.72f), 8, 2, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Upper Fork Step A", parent, new Vector2(12.6f, 0.92f), 4, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Upper Fork Step B", parent, new Vector2(18.4f, 2.35f), 5, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateMovingPlatform("L02 Vertical Memory Lift A", parent, new Vector2(24.2f, 1.62f), new Vector3(0f, 1.75f, 0f), horrorSprites.NeonPlatform, squareSprite);
            CreateHorrorPlatform("L02 Overclock Roof Route", parent, new Vector2(31.0f, 3.72f), 8, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 High Decoder Bridge", parent, new Vector2(41.0f, 2.85f), 9, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateMovingPlatform("L02 Diagonal Sync Platform", parent, new Vector2(50.0f, 1.1f), new Vector3(3.2f, 1.15f, 0f), horrorSprites.GlowEdgePlatform, squareSprite);
            CreateHorrorPlatform("L02 Split Route Mid A", parent, new Vector2(58.4f, 0.42f), 8, 1, horrorSprites.DataStone, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Split Route Upper B", parent, new Vector2(65.5f, 2.32f), 5, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Narrow Timing Step", parent, new Vector2(73.2f, 1.20f), 3, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateMovingPlatform("L02 Risk Hover Bridge", parent, new Vector2(79.5f, -0.15f), new Vector3(0f, 2.15f, 0f), horrorSprites.NeonPlatform, squareSprite);
            CreateHorrorPlatform("L02 Upload Gate Platform", parent, new Vector2(88.5f, 1.02f), 8, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Boss Approach Glass Run", parent, new Vector2(96.0f, 0.72f), 7, 1, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Encrypted Cache Bridge", parent, new Vector2(104.0f, 1.48f), 8, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateMovingPlatform("L02 Packet Compression Elevator", parent, new Vector2(111.0f, 0.18f), new Vector3(0f, 2.05f, 0f), horrorSprites.NeonPlatform, squareSprite);
            CreateHorrorPlatform("L02 Audit Trail Upper A", parent, new Vector2(116.4f, 2.92f), 6, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Audit Trail Upper B", parent, new Vector2(124.0f, 3.62f), 7, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Zero Day Firewall Walk", parent, new Vector2(128.2f, 1.15f), 6, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));

            CreateHorrorPlatform("L02 Lower Decoy Route A", parent, new Vector2(13.4f, -3.62f), 8, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L02 Lower Decoy Route B", parent, new Vector2(25.8f, -4.25f), 8, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L02 Lower Recovery Bridge", parent, new Vector2(39.2f, -3.45f), 9, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L02 Lower Choice Elevator Base", parent, new Vector2(54.0f, -2.62f), 6, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Lower Firewall Run", parent, new Vector2(69.0f, -3.15f), 10, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L02 Lower Exit Catch", parent, new Vector2(88.2f, -2.55f), 8, 1, horrorSprites.MetaPanel, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Deep Phishing Decoy A", parent, new Vector2(106.6f, -3.40f), 8, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L02 Deep Phishing Decoy B", parent, new Vector2(120.2f, -2.80f), 9, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));

            CreateHorrorPlatform("L02 Auth Vault Bridge", parent, new Vector2(136.5f, 1.52f), 8, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateMovingPlatform("L02 Privilege Escalation Lift", parent, new Vector2(144.2f, 0.24f), new Vector3(0f, 2.32f, 0f), horrorSprites.NeonPlatform, squareSprite);
            CreateHorrorPlatform("L02 Sandboxed High Route", parent, new Vector2(152.0f, 3.45f), 8, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Memory Leak Step A", parent, new Vector2(161.0f, 2.18f), 4, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Memory Leak Step B", parent, new Vector2(168.2f, 1.08f), 4, 1, horrorSprites.DataStone, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Twin Path Gate", parent, new Vector2(176.8f, 0.82f), 9, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Telemetry Stair A", parent, new Vector2(185.8f, 2.35f), 5, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Security Log Bridge", parent, new Vector2(195.0f, 3.62f), 9, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Quantum Cache Drop", parent, new Vector2(205.0f, 1.48f), 7, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Kernel Panic Pillar A", parent, new Vector2(214.0f, 0.32f), 3, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Kernel Panic Pillar B", parent, new Vector2(221.2f, 1.82f), 3, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Kernel Panic Pillar C", parent, new Vector2(228.8f, 3.10f), 4, 1, horrorSprites.CircuitBlock, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateMovingPlatform("L02 Zero Trust Elevator", parent, new Vector2(237.0f, 0.52f), new Vector3(0f, 2.52f, 0f), horrorSprites.NeonPlatform, squareSprite);
            CreateHorrorPlatform("L02 Enclave Run", parent, new Vector2(246.0f, 3.20f), 9, 1, horrorSprites.MetaPanel, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Final Boss Approach A", parent, new Vector2(256.6f, 1.35f), 8, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Last Firewall Gate", parent, new Vector2(263.0f, 0.52f), 5, 1, horrorSprites.CircuitBlock, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("72FFFF"));
            CreateHorrorPlatform("L02 Data Graveyard Decoy", parent, new Vector2(146.0f, -3.35f), 8, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L02 Dark Web Decoy", parent, new Vector2(204.0f, -3.12f), 10, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("L02 Exploit Dump Trap Bridge", parent, new Vector2(238.0f, -2.72f), 9, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));

            CreateHorrorPlatform("L02 Boss Arena HD Floor", parent, new Vector2(274.0f, -1.02f), 32, 3, horrorSprites.MetaPanel, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Boss Dodge Shelf A", parent, new Vector2(268.2f, 1.45f), 4, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Boss Dodge Shelf B", parent, new Vector2(273.8f, 2.70f), 4, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Boss Dodge Shelf C", parent, new Vector2(278.0f, 1.02f), 3, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("8CFFFF"));
            CreateHorrorPlatform("L02 Boss Ceiling Data Lock", parent, new Vector2(274.6f, 5.55f), 22, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
        }

        private static void BuildCyberAmbientEffects(Transform parent, Sprite squareSprite, Sprite dataBlobSprite, bool level2)
        {
            float startX = level2 ? -8f : -8f;
            float length = level2 ? 314f : 250f;
            int columns = level2 ? 72 : 58;
            for (int i = 0; i < columns; i++)
            {
                float x = startX + i * (length / columns);
                float y = 4.7f - (i % 4) * 0.72f;
                SpriteRenderer rain = CreateWorldSprite((level2 ? "L02 " : string.Empty) + "Falling Data Rain " + i, parent, new Vector2(x, y), new Vector2(0.08f, 2.8f), new Color(0.18f, 1f, 1f, level2 ? 0.20f : 0.15f), squareSprite, 7);
                CyberGuardianMover mover = rain.gameObject.AddComponent<CyberGuardianMover>();
                mover.localOffset = new Vector3(0f, -1.45f - (i % 3) * 0.35f, 0f);
                mover.speed = 0.35f + (i % 5) * 0.06f;
                AddPulse(rain, 0.015f, 0.07f, 1.2f + i * 0.06f, i * 0.23f);
            }

            int portals = level2 ? 17 : 12;
            for (int i = 0; i < portals; i++)
            {
                float x = (level2 ? 6f : 4f) + i * (level2 ? 12f : 13f);
                SpriteRenderer portal = CreateWorldSprite((level2 ? "L02 " : string.Empty) + "Cyber Depth Portal " + i, parent, new Vector2(x, -2.8f + (i % 2) * 5.3f), new Vector2(1.45f, 1.45f), new Color(0.15f, 0.95f, 1f, 0.12f), dataBlobSprite, 6);
                portal.gameObject.AddComponent<CyberGuardianRotator>().degreesPerSecond = i % 2 == 0 ? 24f : -18f;
                AddPulse(portal, 0.08f, 0.10f, 1.9f, i * 0.44f);
            }
        }

        private static void BuildStoryZones(Transform parent, CyberGuardianSideScrollerGame game, bool level2)
        {
            if (level2)
            {
                CreateStoryZone("L02 Story Start", parent, game, new Vector2(-6.0f, 0.8f), new Vector2(2.0f, 4.0f), "ADEGAN 02: RERUNTUHAN SERVER", "Malware sudah mempelajari rute kamu. Dalam keamanan nyata, penyerang juga beradaptasi, jadi defender perlu mengganti password, menambal sistem, dan tidak bergantung pada satu lapis proteksi.");
                CreateStoryZone("L02 Story Split Route", parent, game, new Vector2(54.0f, 0.6f), new Vector2(2.0f, 6.6f), "PERINGATAN RUTE", "Beberapa platform runtuh setelah disentuh. Perlakukan link mencurigakan dengan cara yang sama: periksa dulu, karena satu klik ceroboh bisa menjatuhkanmu ke halaman login palsu.");
                CreateStoryZone("L02 Story Audit Trail", parent, game, new Vector2(108.0f, 1.2f), new Vector2(2.0f, 6.2f), "JEJAK AUDIT", "Sistem yang baik menyimpan log. Jika ada kejadian aneh, log membantu menemukan sumber, waktu kejadian, dan akun terdampak sebelum kerusakan menyebar.");
                CreateStoryZone("L02 Story Zero Day", parent, game, new Vector2(123.0f, 2.1f), new Vector2(2.0f, 6.8f), "FIREWALL ZERO-DAY", "Zero-day adalah celah yang belum dikenal. Pertahanan terbaik adalah keamanan berlapis: update, hak akses minimal, backup, dan perilaku hati-hati.");
                CreateStoryZone("L02 Story Privilege", parent, game, new Vector2(151.5f, 2.2f), new Vector2(2.0f, 6.8f), "HAK AKSES MINIMAL", "Jangan memberi akses administrator ke semua aplikasi. Least privilege membatasi kerusakan saat satu akun atau program disusupi.");
                CreateStoryZone("L02 Story Telemetry", parent, game, new Vector2(195.0f, 2.4f), new Vector2(2.0f, 6.8f), "LOG KEAMANAN", "Telemetry mengubah kekacauan menjadi bukti. Saat alert tersambung ke log, defender bisa menemukan titik masuk dan menutupnya lebih cepat.");
                CreateStoryZone("L02 Story Backup", parent, game, new Vector2(238.0f, 1.1f), new Vector2(2.0f, 7.0f), "RUTE BACKUP", "Backup adalah armor pemulihan. Jika malware mengenkripsi data, backup bersih menjaga misi tetap hidup tanpa membayar penyerang.");
                CreateStoryZone("L02 Story Boss Gate", parent, game, new Vector2(263.0f, 1.1f), new Vector2(2.0f, 5.4f), "BOS: DATA REAPER", "Grid perisai di sini lebih rapat. Hancurkan blok kuis, hindari paket serangan, dan pakai cache overclock sebelum celah terakhir tertutup.");
                return;
            }

            CreateStoryZone("L01 Story Start", parent, game, new Vector2(-6.5f, 0.9f), new Vector2(2.0f, 4.0f), "ADEGAN 01: HUTAN DATA", "Cyber Guardian manusia memasuki memori yang terinfeksi. Setiap malware beast mewakili unduhan tidak aman, lampiran asing, dan file yang harus dipindai sebelum dibuka.");
            CreateStoryZone("L01 Story Fork", parent, game, new Vector2(8.0f, -0.2f), new Vector2(2.0f, 6.0f), "PILIHAN RUTE PERTAMA", "Jalur bawah menyimpan power, tetapi platformnya tidak stabil. Dalam keamanan cyber, jalan pintas sering tampak menguntungkan tetapi bisa membawa phishing, malware, atau kredensial curian.");
            CreateStoryZone("L01 Story Password", parent, game, new Vector2(34.0f, 0.2f), new Vector2(2.0f, 6.0f), "PELAJARAN PASSWORD", "Password kuat harus panjang, unik, dan dilindungi autentikasi dua faktor. Memakai ulang satu password membuat satu kebocoran berubah menjadi banyak kebocoran.");
            CreateStoryZone("L01 Story Patch Vault", parent, game, new Vector2(86.0f, 1.0f), new Vector2(2.0f, 6.0f), "RUANG PATCH", "Update bukan hanya fitur baru. Update menutup celah keamanan yang sudah diketahui, jadi menunda patch memberi malware lebih banyak waktu untuk menyerang sistem.");
            CreateStoryZone("L01 Story Integrity", parent, game, new Vector2(126.0f, 1.4f), new Vector2(2.0f, 6.4f), "INTEGRITAS FILE", "Checksum membantu memastikan file tidak berubah. Jika nilainya salah, file bisa rusak atau berbahaya.");
            CreateStoryZone("L01 Story MFA", parent, game, new Vector2(194.4f, 2.3f), new Vector2(2.0f, 6.8f), "AKSES MULTI-FAKTOR", "Password adalah satu kunci. Autentikasi multi-faktor menambah kunci lain, sehingga password curian saja tidak cukup.");
            CreateStoryZone("L01 Story Boss Gate", parent, game, new Vector2(212.4f, 1.0f), new Vector2(2.0f, 5.4f), "BOS: FIREWALL VIRUS", "Perisai bos adalah dinding kuis. Jawab dengan benar untuk menghapus blok, membuat jalur serangan bersih, dan membuktikan sistem aman.");
        }

        private static void CreateStoryZone(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Vector2 size, string title, string body)
        {
            GameObject zone = new GameObject(name, typeof(BoxCollider2D), typeof(CyberGuardianStoryZone));
            zone.transform.SetParent(parent, false);
            zone.transform.position = position;
            BoxCollider2D collider = zone.GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = size;
            CyberGuardianStoryZone story = zone.GetComponent<CyberGuardianStoryZone>();
            story.game = game;
            story.storyTitle = title;
            story.storyBody = body;
            story.duration = 5.6f;
        }

        private static void BuildPowerUps(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, bool level2)
        {
            if (level2)
            {
                CreatePowerUp("L02 Boost Cache Upper", parent, game, new Vector2(31.0f, 4.34f), CyberGuardianPowerUpType.Boost, 34, circleSprite, Hex("16E8FF"));
                CreatePowerUp("L02 Firewall Cache Lower", parent, game, new Vector2(39.2f, -2.78f), CyberGuardianPowerUpType.Firewall, 28, circleSprite, Hex("FF3B88"));
                CreatePowerUp("L02 Health Patch Before Gate", parent, game, new Vector2(88.2f, -1.82f), CyberGuardianPowerUpType.Health, 24, circleSprite, Hex("7DFF9B"));
                CreatePowerUp("L02 Audit Boost Cache", parent, game, new Vector2(116.4f, 3.52f), CyberGuardianPowerUpType.Boost, 34, circleSprite, Hex("16E8FF"));
                CreatePowerUp("L02 Privilege Boost Cache", parent, game, new Vector2(152.0f, 4.04f), CyberGuardianPowerUpType.Boost, 34, circleSprite, Hex("16E8FF"));
                CreatePowerUp("L02 Telemetry Health Patch", parent, game, new Vector2(205.0f, 2.04f), CyberGuardianPowerUpType.Health, 26, circleSprite, Hex("7DFF9B"));
                CreatePowerUp("L02 Backup Firewall Cache", parent, game, new Vector2(246.0f, 3.82f), CyberGuardianPowerUpType.Firewall, 28, circleSprite, Hex("FF3B88"));
                CreatePowerUp("L02 Overclock Cache Boss Approach", parent, game, new Vector2(262.6f, 1.16f), CyberGuardianPowerUpType.Overclock, 40, circleSprite, Hex("FFD85E"));
                return;
            }

            CreatePowerUp("Boost Cache Upper Route", parent, game, new Vector2(17.4f, 2.86f), CyberGuardianPowerUpType.Boost, 32, circleSprite, Hex("16E8FF"));
            CreatePowerUp("Health Patch Lower Trap Route", parent, game, new Vector2(29.0f, -2.88f), CyberGuardianPowerUpType.Health, 22, circleSprite, Hex("7DFF9B"));
            CreatePowerUp("Firewall Cache Data Tower", parent, game, new Vector2(64.8f, 1.1f), CyberGuardianPowerUpType.Firewall, 26, circleSprite, Hex("FF3B88"));
            CreatePowerUp("Checksum Vault Boost Cache", parent, game, new Vector2(93.2f, 2.92f), CyberGuardianPowerUpType.Boost, 30, circleSprite, Hex("16E8FF"));
            CreatePowerUp("Integrity Boost Cache", parent, game, new Vector2(132.1f, 3.82f), CyberGuardianPowerUpType.Boost, 30, circleSprite, Hex("16E8FF"));
            CreatePowerUp("Kernel Health Patch", parent, game, new Vector2(169.2f, 3.42f), CyberGuardianPowerUpType.Health, 24, circleSprite, Hex("7DFF9B"));
            CreatePowerUp("MFA Firewall Cache", parent, game, new Vector2(194.4f, 3.92f), CyberGuardianPowerUpType.Firewall, 26, circleSprite, Hex("FF3B88"));
            CreatePowerUp("Overclock Cache Boss Gate", parent, game, new Vector2(212.0f, 1.05f), CyberGuardianPowerUpType.Overclock, 36, circleSprite, Hex("FFD85E"));
        }

        private static void CreatePowerUp(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, CyberGuardianPowerUpType type, int amount, Sprite circleSprite, Color color)
        {
            GameObject power = CreateWorldSprite(name, parent, position, new Vector2(0.56f, 0.56f), color, circleSprite, 29).gameObject;
            CircleCollider2D collider = power.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.32f;
            CyberGuardianPowerUp powerUp = power.AddComponent<CyberGuardianPowerUp>();
            powerUp.game = game;
            powerUp.type = type;
            powerUp.amount = amount;
            SpriteRenderer renderer = power.GetComponent<SpriteRenderer>();
            AddPulse(renderer, 0.10f, 0.16f, 4.8f, amount * 0.03f);
            CreateLocalSprite("Power Up Core", power.transform, Vector3.zero, new Vector2(0.24f, 0.24f), Color.white, circleSprite, 30);
            CreateLocalSprite("Power Up Halo", power.transform, Vector3.zero, new Vector2(0.84f, 0.84f), new Color(color.r, color.g, color.b, 0.22f), circleSprite, 28);
        }

        private static void BuildUnfairChallengeLayer(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, Sprite metalCrateSprite, Sprite dataMossSprite, CyberHorrorAssetSprites horrorSprites, bool level2)
        {
            if (level2)
            {
                CreateBreakawayPlatform("L02 Breakaway Glass Step A", parent, new Vector2(73.2f, 1.78f), new Vector2(1.22f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
                CreateBreakawayPlatform("L02 Breakaway Glass Step B", parent, new Vector2(79.5f, 2.54f), new Vector2(1.22f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
                CreateBreakawayPlatform("L02 Lower Decoy Collapse A", parent, new Vector2(68.0f, -2.54f), new Vector2(1.45f, 0.32f), horrorSprites.CorruptedBlock != null ? horrorSprites.CorruptedBlock : metalCrateSprite, squareSprite);
                CreateBreakawayPlatform("L02 Audit Trail Collapse A", parent, new Vector2(111.0f, 2.22f), new Vector2(1.32f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
                CreateBreakawayPlatform("L02 Zero Day Collapse B", parent, new Vector2(126.0f, 1.80f), new Vector2(1.32f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
                CreateBreakawayPlatform("L02 Privilege Collapse Tile", parent, new Vector2(161.0f, 2.88f), new Vector2(1.32f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
                CreateBreakawayPlatform("L02 Telemetry Collapse Tile", parent, new Vector2(185.8f, 3.05f), new Vector2(1.32f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
                CreateBreakawayPlatform("L02 Final Gate Collapse Tile", parent, new Vector2(256.6f, 2.05f), new Vector2(1.32f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
                CreateVirusBeastEnemy("L02 Quad Malware Beast A", parent, game, new Vector2(15.8f, -3.05f), squareSprite, circleSprite, 1.85f, 2.5f);
                CreateVirusBeastEnemy("L02 Quad Malware Beast B", parent, game, new Vector2(58.2f, 1.15f), squareSprite, circleSprite, 2.15f, 2.2f);
                CreateVirusBeastEnemy("L02 Quad Malware Beast C", parent, game, new Vector2(87.8f, 1.75f), squareSprite, circleSprite, 2.45f, 2.0f);
                CreateVirusBeastEnemy("L02 Quad Malware Beast D", parent, game, new Vector2(121.0f, 4.35f), squareSprite, circleSprite, 2.55f, 1.9f);
                CreateVirusBeastEnemy("L02 Quad Malware Beast E", parent, game, new Vector2(168.2f, 1.82f), squareSprite, circleSprite, 2.62f, 1.7f);
                CreateVirusBeastEnemy("L02 Quad Malware Beast F", parent, game, new Vector2(228.8f, 3.82f), squareSprite, circleSprite, 2.76f, 1.6f);
                CreatePacketLaser("L02 Hidden Ceiling Packet", parent, game, new Vector2(73.2f, 2.58f), new Vector2(1.65f, 0.08f), 18, squareSprite);
                CreatePacketLaser("L02 Audit Ceiling Packet", parent, game, new Vector2(116.4f, 4.18f), new Vector2(2.0f, 0.08f), 20, squareSprite);
                CreatePacketLaser("L02 Enclave Ceiling Packet", parent, game, new Vector2(246.0f, 4.10f), new Vector2(2.2f, 0.08f), 22, squareSprite);
                return;
            }

            CreateBreakawayPlatform("Breakaway Memory Tile A", parent, new Vector2(11.6f, -0.02f), new Vector2(1.24f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("Breakaway Memory Tile B", parent, new Vector2(42.4f, 0.72f), new Vector2(1.24f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("False Lower Memory Tile", parent, new Vector2(58.2f, -3.05f), new Vector2(1.45f, 0.32f), horrorSprites.CorruptedBlock != null ? horrorSprites.CorruptedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("Checksum Vault Collapse Tile", parent, new Vector2(88.2f, 2.15f), new Vector2(1.32f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("Zero Trust Collapse Tile", parent, new Vector2(101.0f, 1.58f), new Vector2(1.32f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("Integrity Collapse Tile", parent, new Vector2(126.0f, 2.85f), new Vector2(1.30f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("Kernel Collapse Tile", parent, new Vector2(162.2f, 2.22f), new Vector2(1.30f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateBreakawayPlatform("MFA Collapse Tile", parent, new Vector2(205.4f, 2.05f), new Vector2(1.30f, 0.32f), horrorSprites.CrackedBlock != null ? horrorSprites.CrackedBlock : metalCrateSprite, squareSprite);
            CreateVirusBeastEnemy("Quad Malware Beast A", parent, game, new Vector2(12.3f, -3.08f), squareSprite, circleSprite, 1.65f, 2.2f);
            CreateVirusBeastEnemy("Quad Malware Beast B", parent, game, new Vector2(31.0f, 0.38f), squareSprite, circleSprite, 1.95f, 2.0f);
            CreateVirusBeastEnemy("Quad Malware Beast C", parent, game, new Vector2(67.6f, 2.78f), squareSprite, circleSprite, 2.15f, 1.8f);
            CreateVirusBeastEnemy("Quad Malware Beast D", parent, game, new Vector2(98.8f, 4.15f), squareSprite, circleSprite, 2.35f, 1.8f);
            CreateVirusBeastEnemy("Quad Malware Beast E", parent, game, new Vector2(149.0f, 2.28f), squareSprite, circleSprite, 2.48f, 1.8f);
            CreateVirusBeastEnemy("Quad Malware Beast F", parent, game, new Vector2(191.4f, -2.00f), squareSprite, circleSprite, 2.58f, 2.1f);
            CreatePacketLaser("Surprise Packet Beam Fork", parent, game, new Vector2(11.8f, 1.42f), new Vector2(1.58f, 0.08f), 16, squareSprite);
            CreatePacketLaser("Checksum Ceiling Packet Beam", parent, game, new Vector2(93.2f, 3.15f), new Vector2(1.9f, 0.08f), 18, squareSprite);
            CreatePacketLaser("MFA Ceiling Packet Beam", parent, game, new Vector2(194.4f, 4.42f), new Vector2(2.1f, 0.08f), 21, squareSprite);
        }

        private static void CreateBreakawayPlatform(string name, Transform parent, Vector2 position, Vector2 size, Sprite tileSprite, Sprite fallbackSprite)
        {
            GameObject platform = new GameObject(name);
            platform.transform.SetParent(parent, false);
            platform.transform.position = position;
            BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
            collider.size = size;
            CyberGuardianBreakawayPlatform breakaway = platform.AddComponent<CyberGuardianBreakawayPlatform>();
            breakaway.breakDelay = 0.32f;
            breakaway.respawnDelay = 2.4f;
            CreateLocalSprite("Breakaway Tile", platform.transform, Vector3.zero, size, Color.white, tileSprite != null ? tileSprite : fallbackSprite, 17);
            CreateLocalSprite("Breakaway Warning Core", platform.transform, new Vector3(0f, 0f, -0.03f), size * 0.62f, new Color(1f, 0.18f, 0.42f, 0.28f), fallbackSprite, 18);
        }

        private static void BuildAdventureActors(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite)
        {
            GameObject playerObject = new GameObject("Cyber Guardian Player", typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(CyberGuardianPlayerController));
            playerObject.transform.SetParent(parent, false);
            playerObject.transform.position = new Vector3(-8f, 0.4f, 0f);
            Transform playerVisualRoot = new GameObject("Cyber Guardian Visual Root").transform;
            playerVisualRoot.SetParent(playerObject.transform, false);
            playerVisualRoot.localPosition = Vector3.zero;
            Rigidbody2D body = playerObject.GetComponent<Rigidbody2D>();
            body.freezeRotation = true;
            body.gravityScale = 3.3f;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            CapsuleCollider2D collider = playerObject.GetComponent<CapsuleCollider2D>();
            collider.size = new Vector2(0.72f, 1.22f);
            Transform groundCheck = new GameObject("Ground Check").transform;
            groundCheck.SetParent(playerObject.transform, false);
            groundCheck.localPosition = new Vector3(0f, -0.70f, 0f);
            CyberGuardianPlayerController player = playerObject.GetComponent<CyberGuardianPlayerController>();
            player.groundCheck = groundCheck;
            player.groundCheckRadius = 0.42f;
            player.coyoteTime = 0.18f;
            player.jumpBufferTime = 0.18f;
            player.visualRoot = playerVisualRoot;
            player.flipVisualRootWithFacing = false;
            game.player = player;

            Sprite[] runEast = LoadPlayerSpriteSequence("running_east");
            Sprite[] runWest = LoadPlayerSpriteSequence("running_west");
            Sprite[] jumpEast = LoadPlayerSpriteSequence("jump_east");
            Sprite[] jumpWest = LoadPlayerSpriteSequence("jump_west");
            Sprite[] fireEast = LoadPlayerSpriteSequence("fire_east");
            Sprite[] fireWest = LoadPlayerSpriteSequence("fire_west");
            Sprite idleSprite = EnsurePlayerSprite(PlayerSpriteFolder + "/idle_00.png");
            Sprite idleEast = runEast.Length > 0 ? runEast[0] : idleSprite;
            Sprite idleWest = runWest.Length > 0 ? runWest[0] : idleSprite;
            SpriteRenderer playerSprite = CreateLocalSprite("Cyber Guardian Imported Sprite", playerVisualRoot, new Vector3(0f, -0.05f, 0f), new Vector2(1.82f, 1.82f), Color.white, idleEast != null ? idleEast : circleSprite, 26);
            CyberGuardianSpriteAnimator2D spriteAnimator = playerObject.AddComponent<CyberGuardianSpriteAnimator2D>();
            spriteAnimator.player = player;
            spriteAnimator.spriteRenderer = playerSprite;
            spriteAnimator.idleEast = idleEast != null ? new[] { idleEast } : runEast;
            spriteAnimator.idleWest = idleWest != null ? new[] { idleWest } : runWest;
            spriteAnimator.runEast = runEast;
            spriteAnimator.runWest = runWest;
            spriteAnimator.jumpEast = jumpEast;
            spriteAnimator.jumpWest = jumpWest;
            spriteAnimator.fireEast = fireEast;
            spriteAnimator.fireWest = fireWest;

            Transform projectileSpawn = new GameObject("Adventure Fireball Spawn").transform;
            projectileSpawn.SetParent(playerObject.transform, false);
            projectileSpawn.localPosition = new Vector3(0.62f, 0.42f, 0f);
            player.projectileSpawn = projectileSpawn;
            player.adventureProjectilePrefab = CreatePlayerFireballPrefab(parent, game, circleSprite);

            GameObject meleeFlash = CreateWorldSprite("Melee Slash Flash", parent, new Vector2(-50f, -50f), new Vector2(1.15f, 0.36f), new Color(0.7f, 1f, 1f, 0.8f), circleSprite, 28).gameObject;
            meleeFlash.SetActive(false);
            game.meleeFlash = meleeFlash;

            CreateEnemy("Virus Soldier A", parent, game, new Vector2(3.2f, 0.62f), squareSprite, circleSprite, 1.7f, 1.8f);
            CreateEnemy("Virus Soldier B", parent, game, new Vector2(14.3f, 0.38f), squareSprite, circleSprite, 1.9f, 1.5f);
            CreateEnemy("Virus Soldier C", parent, game, new Vector2(22.0f, 1.02f), squareSprite, circleSprite, 2.0f, 1.8f);
            CreateEnemy("Virus Soldier D", parent, game, new Vector2(26.0f, 4.30f), squareSprite, circleSprite, 2.2f, 1.4f);
            CreateEnemy("Virus Soldier E", parent, game, new Vector2(38.5f, 1.00f), squareSprite, circleSprite, 2.1f, 1.9f);
            CreateEnemy("Virus Soldier F", parent, game, new Vector2(46.0f, -1.28f), squareSprite, circleSprite, 2.3f, 1.5f);
            CreateEnemy("Virus Soldier G", parent, game, new Vector2(55.0f, 2.30f), squareSprite, circleSprite, 2.4f, 1.7f);
            CreateEnemy("Virus Soldier H", parent, game, new Vector2(62.2f, -0.26f), squareSprite, circleSprite, 2.3f, 1.4f);
            CreateEnemy("Virus Soldier I", parent, game, new Vector2(68.3f, 2.98f), squareSprite, circleSprite, 2.5f, 1.2f);
            CreateEnemy("Virus Soldier J", parent, game, new Vector2(73.6f, -0.88f), squareSprite, circleSprite, 2.4f, 1.5f);
            CreateEnemy("Virus Soldier Extension K", parent, game, new Vector2(86.2f, 1.34f), squareSprite, circleSprite, 2.55f, 1.6f);
            CreateEnemy("Virus Soldier Extension L", parent, game, new Vector2(99.0f, 4.34f), squareSprite, circleSprite, 2.75f, 1.5f);
        }

        private static GameObject CreatePlayerFireballPrefab(Transform parent, CyberGuardianSideScrollerGame game, Sprite circleSprite)
        {
            GameObject projectile = new GameObject("Cyber Guardian Fireball Projectile Prefab", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(CyberGuardianPlayerProjectile2D));
            projectile.transform.SetParent(parent, false);
            projectile.transform.position = new Vector3(-80f, -80f, 0f);
            projectile.transform.localScale = Vector3.one;

            SpriteRenderer renderer = projectile.GetComponent<SpriteRenderer>();
            renderer.sprite = circleSprite;
            renderer.color = new Color(0.42f, 1f, 1f, 0.92f);
            renderer.sortingOrder = 31;
            ScaleSprite(renderer, new Vector2(0.38f, 0.38f));

            Rigidbody2D body = projectile.GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D collider = projectile.GetComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.18f;

            CyberGuardianPlayerProjectile2D projectileLogic = projectile.GetComponent<CyberGuardianPlayerProjectile2D>();
            projectileLogic.game = game;
            projectileLogic.damage = 1;
            projectileLogic.lifetime = 1.55f;
            AddPulse(renderer, 0.10f, 0.12f, 9.2f, 0f);

            CreateLocalSprite("Fireball Hot Core", projectile.transform, Vector3.zero, new Vector2(0.17f, 0.17f), Color.white, circleSprite, 32);
            projectile.SetActive(false);
            return projectile;
        }

        private static void BuildHazards(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, Sprite sawBladeSprite, Sprite metalCrateSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateCheckpoint("Start Recovery Node", parent, game, new Vector2(-6.7f, 0.85f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Fork Recovery Node", parent, game, new Vector2(7.0f, 0.25f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Mid Route Recovery Node", parent, game, new Vector2(31.0f, 0.55f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Boss Approach Recovery Node", parent, game, new Vector2(56.3f, 1.92f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Upload Tower Recovery Node", parent, game, new Vector2(73.8f, 1.25f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Checksum Vault Recovery Node", parent, game, new Vector2(96.4f, 3.95f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Extended Boss Gate Recovery Node", parent, game, new Vector2(104.4f, 0.95f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Integrity Recovery Node", parent, game, new Vector2(126.0f, 3.02f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Kernel Recovery Node", parent, game, new Vector2(178.2f, 1.18f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("MFA Recovery Node", parent, game, new Vector2(194.4f, 4.10f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Final Boss Gate Recovery Node", parent, game, new Vector2(212.4f, 1.08f), squareSprite, horrorSprites.ElectricNode);
            CreateRecoveryZone("Global Code Abyss Recovery Field", parent, game, new Vector2(112f, -8.2f), new Vector2(264f, 1.4f));

            CreateSawTrap("Spinning Saw Fork", parent, game, new Vector2(6.85f, 1.0f), 0.9f, 16, sawBladeSprite, squareSprite);
            CreateLaserBarrier("Laser Barrier Upper A", parent, game, new Vector2(18.2f, 3.02f), 2.2f, 3, 15, squareSprite);
            CreateSpikeTrap("Spike Trap Middle A", parent, game, new Vector2(16.8f, -0.08f), 2.3f, 14, horrorSprites.SpikeBlock, squareSprite);
            CreateElectricNode("Electric Node Fork", parent, game, new Vector2(11.0f, -2.65f), 15, horrorSprites.ElectricNode, squareSprite);
            CreateGlitchMine("Glitch Mine Lower A", parent, game, new Vector2(18.9f, -3.54f), 18, horrorSprites.GlitchMine, squareSprite);
            CreateCrushingBlock("Crushing Block Upper", parent, game, new Vector2(25.7f, 4.55f), new Vector3(0f, -1.55f, 0f), 20, horrorSprites.CrushingBlock, squareSprite);
            CreateVirusTurret("Virus Turret Lower", parent, game, new Vector2(30.2f, -2.82f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateSawTrap("Spinning Saw Lower Exit", parent, game, new Vector2(39.5f, -3.1f), 0.85f, 18, sawBladeSprite, squareSprite);
            CreateLaserBarrier("Laser Barrier Middle B", parent, game, new Vector2(43.2f, 0.85f), 2.45f, 2, 16, squareSprite);
            CreateGlitchMine("Glitch Mine Route Choice", parent, game, new Vector2(47.0f, -1.45f), 18, horrorSprites.GlitchMine, squareSprite);
            CreateCrushingBlock("Crushing Block Boss Approach", parent, game, new Vector2(53.6f, 2.72f), new Vector3(0f, -1.35f, 0f), 20, horrorSprites.CrushingBlock, squareSprite);
            CreateVirusTurret("Virus Turret Boss Gate", parent, game, new Vector2(58.6f, 2.12f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateSawTrap("Upload Tower Saw", parent, game, new Vector2(63.8f, -0.35f), 0.86f, 20, sawBladeSprite, squareSprite);
            CreateElectricNode("Upload Tower Electric Node", parent, game, new Vector2(67.0f, 1.10f), 16, horrorSprites.ElectricNode, squareSprite);
            CreateLaserBarrier("Risk Span Laser Wall", parent, game, new Vector2(71.1f, 2.42f), 1.8f, 2, 17, squareSprite);
            CreateGlitchMine("False Drop Glitch Mine", parent, game, new Vector2(69.2f, -2.74f), 18, horrorSprites.GlitchMine, squareSprite);
            CreateVirusTurret("Virus Turret Upload Exit", parent, game, new Vector2(75.2f, 1.32f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateSawTrap("Old Gate Training Saw", parent, game, new Vector2(76.1f, 0.72f), 0.95f, 18, sawBladeSprite, squareSprite);
            CreateLaserBarrier("Checksum Vault Laser Stack", parent, game, new Vector2(86.2f, 0.95f), 2.3f, 3, 18, squareSprite);
            CreateGlitchMine("Checksum Vault Glitch Mine A", parent, game, new Vector2(89.0f, 2.08f), 20, horrorSprites.GlitchMine, squareSprite);
            CreateCrushingBlock("Hash Tower Crusher", parent, game, new Vector2(96.0f, 4.62f), new Vector3(0f, -1.55f, 0f), 22, horrorSprites.CrushingBlock, squareSprite);
            CreateVirusTurret("Zero Trust Corridor Turret", parent, game, new Vector2(101.0f, 1.62f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateSawTrap("Boss Gate Saw", parent, game, new Vector2(104.2f, 0.62f), 0.95f, 24, sawBladeSprite, squareSprite);
            CreateSwingingSawTrap("Integrity Hanging Saw A", parent, game, new Vector2(119.5f, 4.48f), 1.0f, 2.25f, 25, sawBladeSprite, squareSprite, 0f);
            CreateLaserBarrier("Integrity Checksum Laser", parent, game, new Vector2(132.0f, 3.65f), 2.1f, 2, 19, squareSprite);
            CreateSwingingSawTrap("Checksum Hanging Saw B", parent, game, new Vector2(140.4f, 5.55f), 1.08f, 2.05f, 26, sawBladeSprite, squareSprite, 0.65f);
            CreateGlitchMine("Secure Descent Glitch Mine", parent, game, new Vector2(149.4f, 2.12f), 20, horrorSprites.GlitchMine, squareSprite);
            CreateSwingingSawTrap("Kernel Pendulum Saw", parent, game, new Vector2(160.8f, 4.35f), 1.02f, 2.40f, 27, sawBladeSprite, squareSprite, 1.15f);
            CreateCrushingBlock("Firewall Tunnel Crusher", parent, game, new Vector2(178.2f, 2.20f), new Vector3(0f, -1.28f, 0f), 24, horrorSprites.CrushingBlock, squareSprite);
            CreateVirusTurret("MFA Upper Span Turret", parent, game, new Vector2(194.4f, 4.02f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateSwingingSawTrap("MFA Hanging Saw C", parent, game, new Vector2(205.0f, 4.60f), 1.02f, 2.20f, 28, sawBladeSprite, squareSprite, 1.85f);
            CreateLaserBarrier("Final Gate Packet Laser", parent, game, new Vector2(212.1f, 1.06f), 2.2f, 3, 22, squareSprite);
            CreateSawTrap("Final Boss Gate Floor Saw", parent, game, new Vector2(213.8f, 0.98f), 0.92f, 25, sawBladeSprite, squareSprite);

            CreateEnemy("Virus Soldier Integrity M", parent, game, new Vector2(112.6f, 1.54f), squareSprite, circleSprite, 2.55f, 1.7f);
            CreateEnemy("Virus Soldier Checksum N", parent, game, new Vector2(140.0f, 3.96f), squareSprite, circleSprite, 2.80f, 2.0f);
            CreateEnemy("Virus Soldier Kernel O", parent, game, new Vector2(178.2f, 1.34f), squareSprite, circleSprite, 2.90f, 2.2f);
            CreateEnemy("Virus Soldier MFA P", parent, game, new Vector2(194.4f, 4.24f), squareSprite, circleSprite, 3.00f, 1.8f);
            CreateEnemy("Virus Soldier Final Gate Q", parent, game, new Vector2(212.4f, 1.32f), squareSprite, circleSprite, 3.10f, 1.4f);

            CreateMovingPlatform("Corrupted Moving Platform Lower", parent, new Vector2(42.8f, -2.15f), new Vector3(0f, 1.2f, 0f), horrorSprites.CorruptedPlatform, squareSprite);
            CreateMovingPlatform("Upload Tower Moving Platform", parent, new Vector2(64.6f, -2.45f), new Vector3(2.2f, 1.45f, 0f), horrorSprites.CorruptedPlatform, squareSprite);
            CreateWorldSprite("Suspended Server Core A", parent, new Vector2(24.6f, 4.4f), new Vector2(1.42f, 0.78f), Color.white, horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite, 13);
            CreateWorldSprite("Suspended Server Core B", parent, new Vector2(50.2f, 2.6f), new Vector2(1.42f, 0.78f), Color.white, horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite, 13);
            CreateWorldSprite("Suspended Server Core C", parent, new Vector2(70.4f, 3.9f), new Vector2(1.42f, 0.78f), Color.white, horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite, 13);
            CreateWorldSprite("Suspended Server Core D", parent, new Vector2(92.4f, 4.55f), new Vector2(1.42f, 0.78f), Color.white, horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite, 13);
            CreateWorldSprite("Suspended Server Core E", parent, new Vector2(103.2f, 2.3f), new Vector2(1.42f, 0.78f), Color.white, horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite, 13);
        }

        private static void BuildLevel02Hazards(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, Sprite sawBladeSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateCheckpoint("L02 Start Recovery Node", parent, game, new Vector2(-6.9f, 0.95f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Upper Fork Recovery Node", parent, game, new Vector2(18.4f, 3.0f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Split Route Recovery Node", parent, game, new Vector2(54.0f, -1.8f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Upload Gate Recovery Node", parent, game, new Vector2(88.2f, -1.85f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Audit Trail Recovery Node", parent, game, new Vector2(116.4f, 3.48f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Boss Gate Recovery Node", parent, game, new Vector2(128.3f, 1.72f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Privilege Recovery Node", parent, game, new Vector2(152.0f, 4.02f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Telemetry Recovery Node", parent, game, new Vector2(195.0f, 4.18f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Backup Recovery Node", parent, game, new Vector2(237.0f, 1.10f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("L02 Final Gate Recovery Node", parent, game, new Vector2(263.0f, 1.15f), squareSprite, horrorSprites.ElectricNode);
            CreateRecoveryZone("L02 Global Code Abyss Recovery Field", parent, game, new Vector2(138f, -8.3f), new Vector2(324f, 1.4f));

            CreateSawTrap("L02 Start Fork Saw", parent, game, new Vector2(7.2f, 0.75f), 0.95f, 18, sawBladeSprite, squareSprite);
            CreateLaserBarrier("L02 Upper Memory Laser", parent, game, new Vector2(18.4f, 2.95f), 2.0f, 3, 16, squareSprite);
            CreateSpikeTrap("L02 Lower Decoy Spike A", parent, game, new Vector2(14.2f, -3.15f), 2.5f, 16, horrorSprites.SpikeBlock, squareSprite);
            CreateGlitchMine("L02 Lower Corrupt Mine A", parent, game, new Vector2(25.4f, -3.78f), 20, horrorSprites.GlitchMine, squareSprite);
            CreateCrushingBlock("L02 Roof Crush Gate", parent, game, new Vector2(33.4f, 4.92f), new Vector3(0f, -1.7f, 0f), 22, horrorSprites.CrushingBlock, squareSprite);
            CreateVirusTurret("L02 High Route Virus Turret", parent, game, new Vector2(42.6f, 3.45f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateElectricNode("L02 Lower Elevator Electric Node", parent, game, new Vector2(54.0f, -1.88f), 18, horrorSprites.ElectricNode, squareSprite);
            CreateLaserBarrier("L02 Split Route Laser Fence", parent, game, new Vector2(63.8f, 1.35f), 2.5f, 2, 18, squareSprite);
            CreateSawTrap("L02 Risk Hover Saw", parent, game, new Vector2(78.7f, 1.05f), 0.92f, 20, sawBladeSprite, squareSprite);
            CreateVirusTurret("L02 Upload Gate Virus Turret", parent, game, new Vector2(89.2f, 1.64f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateGlitchMine("L02 Boss Approach Mine", parent, game, new Vector2(95.2f, 1.38f), 20, horrorSprites.GlitchMine, squareSprite);
            CreateLaserBarrier("L02 Encrypted Cache Laser Grid", parent, game, new Vector2(104.0f, 2.05f), 2.4f, 3, 20, squareSprite);
            CreateCrushingBlock("L02 Audit Trail Crusher", parent, game, new Vector2(117.2f, 4.55f), new Vector3(0f, -1.65f, 0f), 24, horrorSprites.CrushingBlock, squareSprite);
            CreateVirusTurret("L02 Zero Day Firewall Turret", parent, game, new Vector2(124.4f, 3.98f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateSawTrap("L02 Boss Gate Twin Saw A", parent, game, new Vector2(129.2f, 1.05f), 0.85f, 24, sawBladeSprite, squareSprite);
            CreateSawTrap("L02 Boss Gate Twin Saw B", parent, game, new Vector2(132.2f, 2.15f), 0.85f, 24, sawBladeSprite, squareSprite);
            CreateSwingingSawTrap("L02 Auth Vault Hanging Saw", parent, game, new Vector2(138.8f, 4.65f), 1.06f, 2.35f, 28, sawBladeSprite, squareSprite, 0.15f);
            CreateLaserBarrier("L02 Privilege Escalation Laser", parent, game, new Vector2(152.0f, 3.82f), 2.4f, 3, 22, squareSprite);
            CreateSwingingSawTrap("L02 Memory Leak Pendulum Saw", parent, game, new Vector2(164.5f, 4.58f), 1.10f, 2.50f, 29, sawBladeSprite, squareSprite, 0.9f);
            CreateCrushingBlock("L02 Twin Path Crusher", parent, game, new Vector2(176.8f, 2.16f), new Vector3(0f, -1.52f, 0f), 25, horrorSprites.CrushingBlock, squareSprite);
            CreateSwingingSawTrap("L02 Telemetry Hanging Saw", parent, game, new Vector2(195.0f, 5.72f), 1.06f, 2.20f, 30, sawBladeSprite, squareSprite, 1.5f);
            CreateGlitchMine("L02 Quantum Cache Glitch Mine", parent, game, new Vector2(205.0f, 2.05f), 22, horrorSprites.GlitchMine, squareSprite);
            CreateSwingingSawTrap("L02 Kernel Panic Saw", parent, game, new Vector2(221.2f, 4.86f), 1.08f, 2.65f, 30, sawBladeSprite, squareSprite, 2.1f);
            CreateVirusTurret("L02 Enclave Run Virus Turret", parent, game, new Vector2(246.0f, 3.88f), Vector2.left, horrorSprites.VirusTurret, squareSprite);
            CreateSwingingSawTrap("L02 Final Gate Hanging Saw", parent, game, new Vector2(257.0f, 4.42f), 1.06f, 2.28f, 31, sawBladeSprite, squareSprite, 2.7f);
            CreateLaserBarrier("L02 Last Firewall Laser", parent, game, new Vector2(263.0f, 1.18f), 2.15f, 3, 24, squareSprite);

            CreateEnemy("Virus Soldier K", parent, game, new Vector2(88.2f, -1.60f), squareSprite, circleSprite, 2.55f, 2.2f);
            CreateEnemy("Virus Soldier L", parent, game, new Vector2(96.0f, 1.28f), squareSprite, circleSprite, 2.70f, 1.8f);
            CreateEnemy("Virus Soldier M", parent, game, new Vector2(104.0f, 2.40f), squareSprite, circleSprite, 2.80f, 1.5f);
            CreateEnemy("Virus Soldier N", parent, game, new Vector2(116.4f, 3.86f), squareSprite, circleSprite, 2.90f, 1.8f);
            CreateEnemy("Virus Soldier O", parent, game, new Vector2(124.0f, 4.56f), squareSprite, circleSprite, 3.00f, 1.6f);
            CreateEnemy("Virus Soldier Privilege P", parent, game, new Vector2(152.0f, 4.02f), squareSprite, circleSprite, 3.05f, 1.8f);
            CreateEnemy("Virus Soldier Twin Path Q", parent, game, new Vector2(176.8f, 1.38f), squareSprite, circleSprite, 3.15f, 2.1f);
            CreateEnemy("Virus Soldier Telemetry R", parent, game, new Vector2(195.0f, 4.18f), squareSprite, circleSprite, 3.20f, 2.0f);
            CreateEnemy("Virus Soldier Quantum S", parent, game, new Vector2(205.0f, 2.04f), squareSprite, circleSprite, 3.25f, 1.7f);
            CreateEnemy("Virus Soldier Enclave T", parent, game, new Vector2(246.0f, 3.76f), squareSprite, circleSprite, 3.35f, 1.9f);
            CreateEnemy("Virus Soldier Final Gate U", parent, game, new Vector2(263.0f, 1.42f), squareSprite, circleSprite, 3.45f, 1.4f);
        }

        private static void BuildBossArena(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, Sprite metalCrateSprite, CyberHorrorAssetSprites horrorSprites, Sprite virusSprite, Sprite virusAltSprite, Sprite bossGeneratedSprite, Sprite projectileSprite, Sprite projectileGeneratedSprite, Sprite sparkSprite, Sprite crosshairSprite, Font font)
        {
            float triggerX = game.bossArenaMinX - 1.2f;
            float leftGateX = game.bossArenaMinX - 0.7f;
            float rightWallX = game.bossArenaMaxX + 5.1f;
            float bossX = game.bossArenaMaxX + 3.0f;
            float shieldStartX = game.bossArenaMaxX - 3.3f;
            float projectileX = game.bossArenaMinX + 0.9f;

            GameObject trigger = new GameObject("Boss Mode Trigger", typeof(BoxCollider2D), typeof(CyberGuardianBossArenaTrigger));
            trigger.transform.SetParent(parent, false);
            trigger.transform.position = new Vector3(triggerX, 1.6f, 0f);
            BoxCollider2D triggerCollider = trigger.GetComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector2(0.65f, 4.2f);
            trigger.GetComponent<CyberGuardianBossArenaTrigger>().game = game;

            CreatePlatform("Boss Left Gate", parent, new Vector2(leftGateX, 2.15f), new Vector2(0.35f, 4.7f), Hex("1C343C"), squareSprite);
            CreatePlatform("Boss Right Wall", parent, new Vector2(rightWallX, 2.15f), new Vector2(0.35f, 4.7f), Hex("1C343C"), squareSprite);

            SpriteRenderer bossGlow = CreateWorldSprite("Boss Glow", parent, new Vector2(bossX, 2.15f), new Vector2(2.8f, 2.8f), new Color(1f, 0.07f, 0.14f, 0.26f), circleSprite, 15);
            bossGlow.gameObject.AddComponent<CyberGuardianBossCore>().game = game;
            CircleCollider2D bossCollider = bossGlow.gameObject.AddComponent<CircleCollider2D>();
            bossCollider.isTrigger = true;
            bossCollider.radius = 0.72f;
            game.bossCore = bossGlow.GetComponent<CyberGuardianBossCore>();
            Sprite bossSprite = bossGeneratedSprite != null ? bossGeneratedSprite : (virusSprite != null ? virusSprite : circleSprite);
            Vector2 bossSize = bossGeneratedSprite != null ? new Vector2(2.55f, 2.55f) : new Vector2(1.95f, 1.95f);
            SpriteRenderer bossArt = CreateWorldSprite("Boss Malware Core Art", parent, new Vector2(bossX, 2.15f), bossSize, Color.white, bossSprite, 18);
            AttachGeneratedGlbVisual(BossGlbPath, bossArt.transform, "Generated Malware Boss GLB Visual", new Vector3(0f, -0.88f, -0.28f), Vector3.one * 0.55f, new Vector3(0f, 180f, 0f));
            if (virusAltSprite != null)
            {
                CreateWorldSprite("Boss Mutation Overlay", parent, new Vector2(bossX + 0.28f, 2.35f), new Vector2(1.1f, 1.1f), new Color(1f, 0.35f, 0.45f, 0.68f), virusAltSprite, 19);
            }

            Transform bossSpawn = new GameObject("Boss Projectile Spawn").transform;
            bossSpawn.SetParent(parent, false);
            bossSpawn.position = new Vector3(bossX - 1.2f, 2.25f, 0f);
            game.bossProjectileSpawn = bossSpawn;

            if (crosshairSprite != null)
            {
                CreateWorldSprite("Boss Arena Crosshair Hint", parent, new Vector2(projectileX + 0.2f, 2.5f), new Vector2(1.1f, 1.1f), new Color(0.6f, 1f, 1f, 0.52f), crosshairSprite, 6);
            }

            int index = 0;
            for (int column = 0; column < 3; column++)
            {
                for (int row = 0; row < 7; row++)
                {
                    Vector2 position = new Vector2(shieldStartX + column * 0.66f, 0.65f + row * 0.58f);
                    int category = (index + row + column * 2) % 4;
                    Sprite quizBlockSprite = horrorSprites != null && horrorSprites.QuizBlock != null ? horrorSprites.QuizBlock : (horrorSprites != null && horrorSprites.CircuitBlock != null ? horrorSprites.CircuitBlock : (metalCrateSprite != null ? metalCrateSprite : squareSprite));
                    CyberGuardianBossShieldBlock block = CreateShieldBlock("Boss Quiz Shield Block " + index.ToString("00"), parent, game, position, category, quizBlockSprite, squareSprite, font);
                    game.bossBlocks.Add(block);
                    index++;
                }
            }

            Sprite projectileActive = projectileGeneratedSprite != null ? projectileGeneratedSprite : (projectileSprite != null ? projectileSprite : circleSprite);
            GameObject projectile = CreateWorldSprite("Patch Core Slingshot Projectile", parent, new Vector2(projectileX, 1.1f), new Vector2(0.58f, 0.58f), new Color(0.72f, 1f, 1f, 1f), projectileActive, 30).gameObject;
            Rigidbody2D projectileBody = projectile.AddComponent<Rigidbody2D>();
            projectileBody.gravityScale = 1f;
            projectileBody.simulated = false;
            projectileBody.interpolation = RigidbodyInterpolation2D.Interpolate;
            projectileBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            CircleCollider2D projectileCollider = projectile.AddComponent<CircleCollider2D>();
            projectileCollider.isTrigger = true;
            projectileCollider.enabled = false;
            projectile.AddComponent<CyberGuardianSlingshotProjectile2D>().game = game;
            game.slingshotProjectile = projectile.transform;
            game.slingshotBody = projectileBody;
            game.slingshotCollider = projectileCollider;
            AttachGeneratedGlbVisual(ProjectileGlbPath, projectile.transform, "Generated Patch Core GLB Visual", new Vector3(0f, -0.42f, -0.18f), Vector3.one * 0.24f, new Vector3(0f, 180f, 0f));
            projectile.SetActive(false);

            game.slingshotBandA = CreateLine("Slingshot Band A", parent, new Color(0.55f, 1f, 1f, 0.88f), 0.08f);
            game.slingshotBandB = CreateLine("Slingshot Band B", parent, new Color(0.55f, 1f, 1f, 0.88f), 0.08f);
            game.trajectoryLine = CreateLine("Patch Trajectory", parent, new Color(0.75f, 1f, 1f, 0.60f), 0.05f);
        }

        private static void BuildHud(CyberGuardianSideScrollerGame game, Sprite panelSprite, Sprite buttonSprite, Sprite frameSprite, CyberHorrorAssetSprites horrorSprites, Font font)
        {
            GameObject canvasObject = CreateCanvas("Cyber Guardian HUD");
            Sprite barBack = horrorSprites.UiBarBack != null ? horrorSprites.UiBarBack : panelSprite;
            Sprite panelFrame = horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : panelSprite;

            AddPanel("Player Combat Bars Back", canvasObject.transform, new Vector2(-700f, 498f), new Vector2(630f, 112f), Color.black, panelFrame, 0.58f);
            AddImage("HP Icon Frame", canvasObject.transform, new Vector2(-960f, 522f), new Vector2(68f, 58f), Color.white, horrorSprites.UiAlertPanel != null ? horrorSprites.UiAlertPanel : panelSprite);
            game.healthText = AddText("HP Icon", canvasObject.transform, new Vector2(-960f, 522f), new Vector2(62f, 36f), "HP", 22, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.playerHealthFill = AddCyberBar(canvasObject.transform, new Vector2(-690f, 522f), new Vector2(440f, 40f), Hex("FF2F83"), barBack, horrorSprites.UiHpBarFill != null ? horrorSprites.UiHpBarFill : panelSprite);
            game.livesText = AddText("Lives Text", canvasObject.transform, new Vector2(-420f, 522f), new Vector2(120f, 30f), "NYAWA 3", 14, Hex("FFD85E"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            AddText("Boost Label", canvasObject.transform, new Vector2(-960f, 470f), new Vector2(86f, 26f), "ENERGI", 13, Hex("61F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.boostEnergyFill = AddCyberBar(canvasObject.transform, new Vector2(-690f, 470f), new Vector2(440f, 34f), Hex("16E8FF"), barBack, horrorSprites.UiBoostBarFill != null ? horrorSprites.UiBoostBarFill : panelSprite);
            game.modeText = AddText("Mode Text", canvasObject.transform, new Vector2(-466f, 470f), new Vector2(74f, 24f), "ENERGI", 11, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.statusText = AddText("Status Text", canvasObject.transform, new Vector2(-700f, 426f), new Vector2(560f, 28f), "MODE PETUALANGAN", 13, Hex("B7F7FF"), font, TextAnchor.MiddleLeft, FontStyle.Bold);

            AddImage("Score Cyber Card", canvasObject.transform, new Vector2(720f, 518f), new Vector2(320f, 64f), Color.white, horrorSprites.UiScorePanel != null ? horrorSprites.UiScorePanel : panelSprite);
            AddText("Score Label", canvasObject.transform, new Vector2(612f, 532f), new Vector2(92f, 22f), "SKOR", 14, Hex("FF5B9B"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.scoreText = AddText("Score Text", canvasObject.transform, new Vector2(756f, 504f), new Vector2(206f, 42f), "0", 34, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.pauseButton = AddButton("Menu Button", canvasObject.transform, new Vector2(915f, 518f), new Vector2(112f, 50f), "MENU", 14, font, Hex("08181D"), Color.white, horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : buttonSprite, out _);

            GameObject bossHud = new GameObject("Boss HUD Group", typeof(RectTransform));
            bossHud.transform.SetParent(canvasObject.transform, false);
            game.bossHudGroup = bossHud;
            AddImage("Boss Core Icon", bossHud.transform, new Vector2(-376f, 424f), new Vector2(76f, 76f), Color.white, horrorSprites.UiAlertPanel != null ? horrorSprites.UiAlertPanel : panelSprite);
            game.bossHealthFill = AddCyberBar(bossHud.transform, new Vector2(-8f, 424f), new Vector2(630f, 38f), Hex("FF2F83"), barBack, horrorSprites.UiBossBarFill != null ? horrorSprites.UiBossBarFill : panelSprite);
            game.bossText = AddText("Boss Text", bossHud.transform, new Vector2(-8f, 464f), new Vector2(500f, 28f), "HP BOS", 21, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            bossHud.SetActive(false);
            BuildStoryPanel(canvasObject.transform, game, horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : panelSprite, font);
            Sprite modalPanel = horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : panelSprite;
            Sprite modalButton = horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : buttonSprite;
            BuildQuizModal(canvasObject.transform, game, modalPanel, modalButton, frameSprite, font);
            BuildPauseModal(canvasObject.transform, game, modalPanel, modalButton, frameSprite, horrorSprites.UiButtonMagenta != null ? horrorSprites.UiButtonMagenta : buttonSprite, font);
            BuildGameOverModal(canvasObject.transform, game, modalPanel, modalButton, frameSprite, font);
            BuildReadyCountdownModal(canvasObject.transform, game, modalPanel, font);
        }

        private static void BuildStoryPanel(Transform parent, CyberGuardianSideScrollerGame game, Sprite panelSprite, Font font)
        {
            GameObject panel = new GameObject("Story Overlay Text", typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -365f);
            rect.sizeDelta = new Vector2(1260f, 150f);

            game.storyTitleText = AddText("Story Title", panel.transform, new Vector2(0f, 40f), new Vector2(1160f, 40f), "SEKTOR", 28, Hex("61F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            Shadow titleShadow = game.storyTitleText.gameObject.AddComponent<Shadow>();
            titleShadow.effectColor = new Color(0f, 0f, 0f, 0.96f);
            titleShadow.effectDistance = new Vector2(3f, -3f);
            game.storyBodyText = AddText("Story Body", panel.transform, new Vector2(0f, -24f), new Vector2(1180f, 86f), "Pembaruan misi", 19, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            Shadow bodyShadow = game.storyBodyText.gameObject.AddComponent<Shadow>();
            bodyShadow.effectColor = new Color(0f, 0f, 0f, 0.96f);
            bodyShadow.effectDistance = new Vector2(3f, -3f);
            game.storyPanel = panel;
            panel.SetActive(false);
        }

        private static void BuildQuizModal(Transform parent, CyberGuardianSideScrollerGame game, Sprite panelSprite, Sprite buttonSprite, Sprite frameSprite, Font font)
        {
            GameObject modal = new GameObject("Quiz Modal", typeof(RectTransform));
            modal.transform.SetParent(parent, false);
            RectTransform modalRect = modal.GetComponent<RectTransform>();
            modalRect.anchorMin = Vector2.zero;
            modalRect.anchorMax = Vector2.one;
            modalRect.offsetMin = Vector2.zero;
            modalRect.offsetMax = Vector2.zero;

            AddStretchImage("Quiz Dim", modal.transform, new Color(0f, 0f, 0f, 0.54f), panelSprite);
            AddPanel("Quiz Window", modal.transform, Vector2.zero, new Vector2(720f, 420f), Hex("142F38"), panelSprite, 0.97f);
            if (frameSprite != null)
            {
                Image frame = AddImage("Quiz Cyberpunk Frame", modal.transform, Vector2.zero, new Vector2(736f, 436f), Color.white, frameSprite);
                frame.type = Image.Type.Sliced;
                frame.raycastTarget = false;
            }

            AddPanel("Quiz Header", modal.transform, new Vector2(0f, 158f), new Vector2(620f, 58f), Hex("C7973D"), panelSprite, 1f);
            game.quizTitleText = AddText("Quiz Title", modal.transform, new Vector2(0f, 158f), new Vector2(590f, 44f), "BLOK KUIS", 22, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.quizPromptText = AddText("Quiz Prompt", modal.transform, new Vector2(0f, 84f), new Vector2(600f, 78f), "Pertanyaan keamanan siber", 19, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Normal);
            game.feedbackText = AddText("Quiz Feedback", modal.transform, new Vector2(0f, -150f), new Vector2(600f, 40f), string.Empty, 15, Hex("B7F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            game.answerButtons = new Button[4];
            game.answerLabels = new Text[4];
            Vector2[] positions =
            {
                new Vector2(-170f, 8f),
                new Vector2(170f, 8f),
                new Vector2(-170f, -72f),
                new Vector2(170f, -72f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                game.answerButtons[i] = AddButton("Answer " + i, modal.transform, positions[i], new Vector2(310f, 58f), "JAWAB", 14, font, Hex("2E6E75"), Color.white, buttonSprite, out Text label);
                game.answerLabels[i] = label;
            }

            game.closeQuizButton = AddButton("Close Quiz", modal.transform, new Vector2(315f, 178f), new Vector2(42f, 42f), "X", 18, font, Hex("A83C48"), Color.white, buttonSprite, out _);
            game.quizModal = modal;
            modal.SetActive(false);
        }

        private static void BuildPauseModal(Transform parent, CyberGuardianSideScrollerGame game, Sprite panelSprite, Sprite buttonSprite, Sprite frameSprite, Sprite dangerButtonSprite, Font font)
        {
            GameObject modal = new GameObject("Pause Modal", typeof(RectTransform));
            modal.transform.SetParent(parent, false);
            RectTransform modalRect = modal.GetComponent<RectTransform>();
            modalRect.anchorMin = Vector2.zero;
            modalRect.anchorMax = Vector2.one;
            modalRect.offsetMin = Vector2.zero;
            modalRect.offsetMax = Vector2.zero;

            AddStretchImage("Pause Dim", modal.transform, new Color(0f, 0f, 0f, 0.48f), panelSprite);
            AddPanel("Pause Window", modal.transform, Vector2.zero, new Vector2(560f, 330f), Hex("071820"), panelSprite, 0.96f);
            if (frameSprite != null)
            {
                Image frame = AddImage("Pause Cyberpunk Frame", modal.transform, Vector2.zero, new Vector2(582f, 352f), Color.white, frameSprite);
                frame.type = Image.Type.Sliced;
                frame.raycastTarget = false;
            }

            AddText("Pause Title", modal.transform, new Vector2(0f, 104f), new Vector2(470f, 62f), "JEDA", 42, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.pauseResumeButton = AddButton("Pause Resume Button", modal.transform, new Vector2(0f, 34f), new Vector2(310f, 56f), "LANJUT", 19, font, Hex("071B21"), Color.white, buttonSprite, out _);
            game.pauseRetryButton = AddButton("Pause Retry Button", modal.transform, new Vector2(0f, -38f), new Vector2(310f, 56f), "ULANGI", 19, font, Hex("160810"), Color.white, dangerButtonSprite, out _);
            game.pauseMenuButton = AddButton("Pause Main Menu Button", modal.transform, new Vector2(0f, -110f), new Vector2(310f, 56f), "MENU", 19, font, Hex("071B21"), Color.white, buttonSprite, out _);
            game.pauseModal = modal;
            modal.SetActive(false);
        }

        private static void BuildGameOverModal(Transform parent, CyberGuardianSideScrollerGame game, Sprite panelSprite, Sprite buttonSprite, Sprite frameSprite, Font font)
        {
            GameObject modal = new GameObject("Game Over Modal", typeof(RectTransform));
            modal.transform.SetParent(parent, false);
            RectTransform modalRect = modal.GetComponent<RectTransform>();
            modalRect.anchorMin = Vector2.zero;
            modalRect.anchorMax = Vector2.one;
            modalRect.offsetMin = Vector2.zero;
            modalRect.offsetMax = Vector2.zero;

            AddStretchImage("Game Over Dim", modal.transform, new Color(0f, 0f, 0f, 0.62f), panelSprite);
            AddPanel("Game Over Window", modal.transform, Vector2.zero, new Vector2(780f, 360f), Color.black, panelSprite, 0.88f);
            if (frameSprite != null)
            {
                Image frame = AddImage("Game Over Cyberpunk Frame", modal.transform, Vector2.zero, new Vector2(800f, 380f), Color.white, frameSprite);
                frame.type = Image.Type.Sliced;
                frame.raycastTarget = false;
            }

            AddPanel("Game Over Header", modal.transform, new Vector2(0f, 92f), new Vector2(640f, 92f), Hex("11171B"), panelSprite, 1f);
            AddText("Game Over Title", modal.transform, new Vector2(0f, 102f), new Vector2(620f, 84f), "SISTEM JEBOL", 58, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Game Over Subtitle", modal.transform, new Vector2(0f, 24f), new Vector2(620f, 40f), "CYBER GUARDIAN HANCUR", 22, Hex("FF6671"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.gameOverScoreText = AddText("Game Over Score", modal.transform, new Vector2(0f, -30f), new Vector2(620f, 46f), "SKOR 0", 26, Hex("69F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.gameOverRetryButton = AddButton("Game Over Retry Button", modal.transform, new Vector2(-118f, -116f), new Vector2(210f, 58f), "ULANGI", 20, font, Hex("00AFC2"), Color.white, buttonSprite, out _);
            game.gameOverMenuButton = AddButton("Game Over Menu Button", modal.transform, new Vector2(118f, -116f), new Vector2(210f, 58f), "MENU", 20, font, Hex("263039"), Color.white, buttonSprite, out _);
            game.gameOverModal = modal;
            modal.SetActive(false);
        }

        private static void BuildReadyCountdownModal(Transform parent, CyberGuardianSideScrollerGame game, Sprite panelSprite, Font font)
        {
            GameObject modal = new GameObject("Ready Countdown Modal", typeof(RectTransform));
            modal.transform.SetParent(parent, false);
            RectTransform modalRect = modal.GetComponent<RectTransform>();
            modalRect.anchorMin = Vector2.zero;
            modalRect.anchorMax = Vector2.one;
            modalRect.offsetMin = Vector2.zero;
            modalRect.offsetMax = Vector2.zero;

            game.readyDimImage = AddStretchImage("Ready Countdown Dim", modal.transform, new Color(0f, 0f, 0f, 0.48f), panelSprite);
            game.readyTitleText = AddText("Ready Countdown Title", modal.transform, new Vector2(0f, 94f), new Vector2(860f, 82f), "SIAP?", 56, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.readyCountdownText = AddText("Ready Countdown Number", modal.transform, new Vector2(0f, -24f), new Vector2(560f, 128f), "3", 92, Hex("61F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            Image[] pulses = new Image[6];
            for (int i = 0; i < pulses.Length; i++)
            {
                float side = i % 2 == 0 ? -1f : 1f;
                float y = -154f + (i / 2) * 74f;
                Image pulse = AddImage("Ready Pulse Line " + i, modal.transform, new Vector2(side * 460f, y), new Vector2(260f, 6f), i % 3 == 0 ? Hex("FF3B88") : Hex("61F7FF"), panelSprite);
                pulse.raycastTarget = false;
                pulse.color = new Color(pulse.color.r, pulse.color.g, pulse.color.b, 0f);
                pulses[i] = pulse;
            }

            game.readyPulseImages = pulses;
            game.readyPanel = modal;
            modal.SetActive(false);
        }

        private static void CreateGuardianRig(Transform parent, Sprite squareSprite, Sprite circleSprite, CyberGuardianCharacterAnimator2D animator)
        {
            CreateLocalSprite("Guardian Shield Aura", parent, new Vector3(0f, 0.12f, 0.10f), new Vector2(1.16f, 1.46f), new Color(0.0f, 0.95f, 1f, 0.18f), circleSprite, 18);
            SpriteRenderer torso = CreateLocalSprite("Guardian Torso Armor", parent, new Vector3(0f, 0.07f, 0f), new Vector2(0.46f, 0.68f), Hex("101B23"), squareSprite, 22);
            CreateLocalSprite("Guardian Chest Plate", parent, new Vector3(0.03f, 0.13f, -0.02f), new Vector2(0.34f, 0.42f), Hex("253A45"), squareSprite, 23);
            SpriteRenderer core = CreateLocalSprite("Guardian Security Core", parent, new Vector3(0.07f, 0.13f, -0.05f), new Vector2(0.19f, 0.19f), Hex("00F0FF"), circleSprite, 26);
            SpriteRenderer head = CreateLocalSprite("Guardian Helmet", parent, new Vector3(0.03f, 0.57f, -0.02f), new Vector2(0.46f, 0.38f), Hex("121F29"), circleSprite, 24);
            CreateLocalSprite("Guardian Helmet Crest", parent, new Vector3(0.22f, 0.68f, -0.04f), new Vector2(0.18f, 0.12f), Hex("69F7FF"), squareSprite, 25);
            SpriteRenderer visor = CreateLocalSprite("Guardian Helmet Visor", parent, new Vector3(0.11f, 0.58f, -0.05f), new Vector2(0.30f, 0.07f), Hex("7CFFFF"), squareSprite, 27);
            CreateLocalSprite("Guardian Left Shoulder", parent, new Vector3(-0.32f, 0.28f, -0.01f), new Vector2(0.24f, 0.18f), Hex("122734"), squareSprite, 25);
            CreateLocalSprite("Guardian Right Shoulder", parent, new Vector3(0.34f, 0.28f, -0.01f), new Vector2(0.25f, 0.18f), Hex("DDFBFF"), squareSprite, 25);
            SpriteRenderer leftArm = CreateLocalSprite("Guardian Shield Arm", parent, new Vector3(-0.40f, -0.05f, -0.01f), new Vector2(0.18f, 0.58f), Hex("1A303A"), squareSprite, 23);
            leftArm.transform.localRotation = Quaternion.Euler(0f, 0f, 10f);
            SpriteRenderer rightArm = CreateLocalSprite("Guardian Blade Arm", parent, new Vector3(0.42f, -0.05f, -0.01f), new Vector2(0.17f, 0.54f), Hex("E9FDFF"), squareSprite, 23);
            rightArm.transform.localRotation = Quaternion.Euler(0f, 0f, -10f);
            SpriteRenderer blade = CreateLocalSprite("Guardian Data Blade", parent, new Vector3(0.79f, -0.15f, -0.04f), new Vector2(0.72f, 0.10f), Hex("8CFFFF"), squareSprite, 26);
            blade.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);
            SpriteRenderer leftLeg = CreateLocalSprite("Guardian Left Leg", parent, new Vector3(-0.15f, -0.48f, 0.01f), new Vector2(0.18f, 0.48f), Hex("0E1B24"), squareSprite, 21);
            leftLeg.transform.localRotation = Quaternion.Euler(0f, 0f, 5f);
            SpriteRenderer rightLeg = CreateLocalSprite("Guardian Right Leg", parent, new Vector3(0.18f, -0.48f, 0.01f), new Vector2(0.18f, 0.48f), Hex("182C36"), squareSprite, 21);
            rightLeg.transform.localRotation = Quaternion.Euler(0f, 0f, -5f);
            SpriteRenderer cape = CreateLocalSprite("Guardian Fragment Cape", parent, new Vector3(-0.34f, -0.08f, 0.06f), new Vector2(0.36f, 1.10f), new Color(0.01f, 0.05f, 0.07f, 0.85f), squareSprite, 19);
            cape.transform.localRotation = Quaternion.Euler(0f, 0f, -7f);

            if (animator != null)
            {
                animator.torso = torso.transform;
                animator.head = head.transform;
                animator.cape = cape.transform;
                animator.blade = blade.transform;
                animator.leftArm = leftArm.transform;
                animator.rightArm = rightArm.transform;
                animator.leftLeg = leftLeg.transform;
                animator.rightLeg = rightLeg.transform;
                animator.coreRenderer = core;
                animator.visorRenderer = visor;
            }
        }

        private static void CreateVirusSoldierRig(Transform parent, Sprite squareSprite, Sprite circleSprite, CyberGuardianEnemyAnimator2D animator)
        {
            CreateLocalSprite("Virus Shadow Halo", parent, new Vector3(0f, 0.04f, 0.06f), new Vector2(0.84f, 0.88f), new Color(1f, 0.03f, 0.42f, 0.20f), circleSprite, 17);
            SpriteRenderer torso = CreateLocalSprite("Virus Soldier Torso", parent, new Vector3(0f, -0.02f, 0f), new Vector2(0.48f, 0.55f), Hex("17151D"), squareSprite, 19);
            SpriteRenderer core = CreateLocalSprite("Virus Magenta Core Glow", parent, new Vector3(0.06f, 0.04f, -0.04f), new Vector2(0.25f, 0.25f), new Color(1f, 0.05f, 0.48f, 0.78f), circleSprite, 23);
            SpriteRenderer head = CreateLocalSprite("Virus Soldier Head", parent, new Vector3(0.04f, 0.38f, -0.02f), new Vector2(0.42f, 0.34f), Hex("201A27"), circleSprite, 21);
            SpriteRenderer eye = CreateLocalSprite("Virus Red Eye", parent, new Vector3(0.15f, 0.40f, -0.06f), new Vector2(0.18f, 0.16f), Hex("FF2F83"), circleSprite, 24);
            CreateLocalSprite("Virus Eye Hotspot", parent, new Vector3(0.18f, 0.42f, -0.08f), new Vector2(0.055f, 0.055f), Hex("FFFFFF"), circleSprite, 25);
            SpriteRenderer leftClaw = CreateLocalSprite("Virus Claw Left", parent, new Vector3(-0.43f, 0.02f, -0.03f), new Vector2(0.35f, 0.11f), Hex("FF2F83"), squareSprite, 22);
            leftClaw.transform.localRotation = Quaternion.Euler(0f, 0f, -16f);
            SpriteRenderer rightClaw = CreateLocalSprite("Virus Claw Right", parent, new Vector3(0.48f, 0.03f, -0.03f), new Vector2(0.36f, 0.11f), Hex("FF2F83"), squareSprite, 22);
            rightClaw.transform.localRotation = Quaternion.Euler(0f, 0f, 14f);
            CreateLocalSprite("Virus Leg Left", parent, new Vector3(-0.15f, -0.44f, 0.01f), new Vector2(0.16f, 0.30f), Hex("151923"), squareSprite, 18);
            CreateLocalSprite("Virus Leg Right", parent, new Vector3(0.17f, -0.44f, 0.01f), new Vector2(0.16f, 0.30f), Hex("22202B"), squareSprite, 18);
            SpriteRenderer glitchTrail = CreateLocalSprite("Virus Glitch Trail", parent, new Vector3(-0.28f, -0.42f, 0.05f), new Vector2(0.20f, 0.20f), new Color(1f, 0.05f, 0.48f, 0.46f), squareSprite, 17);

            if (animator != null)
            {
                animator.visualRoot = parent;
                animator.torso = torso.transform;
                animator.head = head.transform;
                animator.leftClaw = leftClaw.transform;
                animator.rightClaw = rightClaw.transform;
                animator.glitchTrail = glitchTrail.transform;
                animator.coreRenderer = core;
                animator.eyeRenderer = eye;
            }
        }

        private static void CreateQuadMalwareBeastRig(Transform parent, Sprite squareSprite, Sprite circleSprite, CyberGuardianEnemyAnimator2D animator)
        {
            CreateLocalSprite("Quad Beast Shadow Halo", parent, new Vector3(0f, -0.18f, 0.08f), new Vector2(1.34f, 0.58f), new Color(1f, 0.03f, 0.46f, 0.22f), circleSprite, 17);
            SpriteRenderer torso = CreateLocalSprite("Quad Malware Armored Body", parent, new Vector3(0f, 0.02f, 0f), new Vector2(0.92f, 0.48f), Hex("111820"), squareSprite, 20);
            SpriteRenderer head = CreateLocalSprite("Quad Malware Head", parent, new Vector3(0.52f, 0.18f, -0.03f), new Vector2(0.42f, 0.32f), Hex("1B1724"), circleSprite, 22);
            SpriteRenderer eye = CreateLocalSprite("Quad Malware Eye", parent, new Vector3(0.66f, 0.20f, -0.06f), new Vector2(0.16f, 0.13f), Hex("FF2F83"), circleSprite, 25);
            SpriteRenderer core = CreateLocalSprite("Quad Malware Core", parent, new Vector3(0.02f, 0.06f, -0.06f), new Vector2(0.28f, 0.28f), new Color(1f, 0.03f, 0.48f, 0.92f), circleSprite, 26);
            SpriteRenderer leftClaw = CreateLocalSprite("Quad Malware Front Claw", parent, new Vector3(0.84f, -0.02f, -0.04f), new Vector2(0.38f, 0.12f), Hex("FF2F83"), squareSprite, 24);
            leftClaw.transform.localRotation = Quaternion.Euler(0f, 0f, -14f);
            SpriteRenderer rightClaw = CreateLocalSprite("Quad Malware Rear Spike", parent, new Vector3(-0.72f, 0.10f, -0.04f), new Vector2(0.30f, 0.12f), Hex("61F7FF"), squareSprite, 24);
            rightClaw.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);

            Vector3[] legPositions =
            {
                new Vector3(-0.36f, -0.34f, 0.01f),
                new Vector3(-0.10f, -0.38f, 0.01f),
                new Vector3(0.22f, -0.38f, 0.01f),
                new Vector3(0.48f, -0.34f, 0.01f)
            };

            for (int i = 0; i < legPositions.Length; i++)
            {
                SpriteRenderer leg = CreateLocalSprite("Quad Malware Leg " + i, parent, legPositions[i], new Vector2(0.15f, 0.42f), i % 2 == 0 ? Hex("111820") : Hex("24202E"), squareSprite, 19);
                leg.transform.localRotation = Quaternion.Euler(0f, 0f, i % 2 == 0 ? -14f : 14f);
                CreateLocalSprite("Quad Malware Foot " + i, parent, legPositions[i] + new Vector3(i % 2 == 0 ? -0.08f : 0.08f, -0.23f, -0.02f), new Vector2(0.28f, 0.10f), Hex("FF2F83"), squareSprite, 21);
            }

            SpriteRenderer glitchTrail = CreateLocalSprite("Quad Malware Glitch Tail", parent, new Vector3(-0.68f, -0.10f, 0.06f), new Vector2(0.40f, 0.24f), new Color(1f, 0.05f, 0.48f, 0.38f), squareSprite, 18);
            CreateLocalSprite("Quad Malware Cyan Data Spike", parent, new Vector3(-0.12f, 0.34f, -0.02f), new Vector2(0.15f, 0.30f), Hex("61F7FF"), squareSprite, 24);
            CreateLocalSprite("Quad Malware Pink Data Spike", parent, new Vector3(0.22f, 0.34f, -0.02f), new Vector2(0.15f, 0.30f), Hex("FF2F83"), squareSprite, 24);

            if (animator != null)
            {
                animator.visualRoot = parent;
                animator.torso = torso.transform;
                animator.head = head.transform;
                animator.leftClaw = leftClaw.transform;
                animator.rightClaw = rightClaw.transform;
                animator.glitchTrail = glitchTrail.transform;
                animator.coreRenderer = core;
                animator.eyeRenderer = eye;
            }
        }

        private static CyberGuardianEnemy CreateEnemy(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Sprite squareSprite, Sprite circleSprite, float speed, float patrol)
        {
            GameObject enemyObject = new GameObject(name, typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CyberGuardianEnemy));
            enemyObject.transform.SetParent(parent, false);
            enemyObject.transform.position = position;
            Transform visualRoot = new GameObject("Virus Visual Root").transform;
            visualRoot.SetParent(enemyObject.transform, false);
            visualRoot.localPosition = Vector3.zero;
            Rigidbody2D body = enemyObject.GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.freezeRotation = true;
            BoxCollider2D collider = enemyObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(0.74f, 0.74f);
            CyberGuardianEnemy enemy = enemyObject.GetComponent<CyberGuardianEnemy>();
            enemy.game = game;
            enemy.speed = speed;
            enemy.patrolDistance = patrol;
            enemy.visualRoot = visualRoot;
            game.enemies.Add(enemy);

            int visualType = Mathf.Abs(game.enemies.Count - 1) % 2;
            if (TryAttachImportedBasicEnemyVisual(enemyObject, enemy, visualRoot, visualType, circleSprite))
            {
                enemy.flipVisualRootWithDirection = false;
            }
            else
            {
                CyberGuardianEnemyAnimator2D animator = enemyObject.AddComponent<CyberGuardianEnemyAnimator2D>();
                animator.visualRoot = visualRoot;
                CreateVirusSoldierRig(visualRoot, squareSprite, circleSprite, animator);
            }

            return enemy;
        }

        private static bool TryAttachImportedBasicEnemyVisual(GameObject enemyObject, CyberGuardianEnemy enemy, Transform visualRoot, int visualType, Sprite fallbackSprite)
        {
            bool beastType = visualType == 0;
            Sprite[] walkEast = LoadEnemySpriteSequence(beastType ? "malware_beast_walk_east" : "cyber_hunter_walk_east");
            Sprite[] walkWest = LoadEnemySpriteSequence(beastType ? "malware_beast_walk_west" : "cyber_hunter_walk_west");
            if ((walkEast == null || walkEast.Length == 0) && (walkWest == null || walkWest.Length == 0))
            {
                return false;
            }

            Sprite[] attackEast = beastType ? LoadEnemySpriteSequence("malware_beast_attack_east") : walkEast;
            Sprite[] attackWest = beastType ? LoadEnemySpriteSequence("malware_beast_attack_west") : walkWest;
            Vector2 targetSize = beastType ? new Vector2(1.34f, 1.34f) : new Vector2(1.48f, 1.48f);
            Vector2 colliderSize = beastType ? new Vector2(1.14f, 0.72f) : new Vector2(0.78f, 1.18f);
            Vector3 spriteOffset = beastType ? new Vector3(0f, -0.10f, 0f) : new Vector3(0f, 0.05f, 0f);

            Sprite startSprite = walkEast != null && walkEast.Length > 0 ? walkEast[0] : (walkWest != null && walkWest.Length > 0 ? walkWest[0] : fallbackSprite);
            SpriteRenderer renderer = CreateLocalSprite(beastType ? "Imported Malware Beast Sprite" : "Imported Cyber Hunter Sprite", visualRoot, spriteOffset, targetSize, Color.white, startSprite, 23);
            CyberGuardianEnemySpriteAnimator2D animator = enemyObject.AddComponent<CyberGuardianEnemySpriteAnimator2D>();
            animator.enemy = enemy;
            animator.spriteRenderer = renderer;
            animator.walkEast = walkEast;
            animator.walkWest = walkWest;
            animator.attackEast = attackEast;
            animator.attackWest = attackWest;
            animator.walkFps = beastType ? 8f : 10f;
            animator.attackFps = beastType ? 13f : 10f;

            BoxCollider2D collider = enemyObject.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = colliderSize;
                collider.offset = beastType ? new Vector2(0f, -0.12f) : new Vector2(0f, 0.02f);
            }

            enemy.health = beastType ? 3 : 2;
            enemy.touchDamage = beastType ? 13 : 10;
            return true;
        }

        private static CyberGuardianEnemy CreateVirusBeastEnemy(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Sprite squareSprite, Sprite circleSprite, float speed, float patrol)
        {
            GameObject enemyObject = new GameObject(name, typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CyberGuardianEnemy));
            enemyObject.transform.SetParent(parent, false);
            enemyObject.transform.position = position;
            Transform visualRoot = new GameObject("Quad Malware Beast Visual Root").transform;
            visualRoot.SetParent(enemyObject.transform, false);
            visualRoot.localPosition = Vector3.zero;

            Rigidbody2D body = enemyObject.GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.freezeRotation = true;

            BoxCollider2D collider = enemyObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(1.22f, 0.74f);

            CyberGuardianEnemy enemy = enemyObject.GetComponent<CyberGuardianEnemy>();
            enemy.game = game;
            enemy.health = 3;
            enemy.touchDamage = 14;
            enemy.speed = speed;
            enemy.patrolDistance = patrol;
            enemy.visualRoot = visualRoot;
            game.enemies.Add(enemy);

            CyberGuardianEnemyAnimator2D animator = enemyObject.AddComponent<CyberGuardianEnemyAnimator2D>();
            animator.visualRoot = visualRoot;
            CreateQuadMalwareBeastRig(visualRoot, squareSprite, circleSprite, animator);
            return enemy;
        }

        private static CyberGuardianBossShieldBlock CreateShieldBlock(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, int category, Sprite blockSprite, Sprite squareSprite, Font font)
        {
            GameObject blockObject = new GameObject(name, typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(CyberGuardianBossShieldBlock));
            blockObject.transform.SetParent(parent, false);
            blockObject.transform.position = position;
            SpriteRenderer renderer = blockObject.GetComponent<SpriteRenderer>();
            renderer.sprite = blockSprite;
            renderer.color = Color.white;
            renderer.sortingOrder = 16;
            ScaleSprite(renderer, new Vector2(0.72f, 0.62f));
            BoxCollider2D collider = blockObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(0.72f, 0.62f);
            collider.isTrigger = false;
            CyberGuardianBossShieldBlock block = blockObject.GetComponent<CyberGuardianBossShieldBlock>();
            block.game = game;
            block.category = category;

            Color categoryColor = GetCategoryColor(category);
            CreateLocalSprite("Quiz Block Category Aura", blockObject.transform, new Vector3(0f, 0f, 0.04f), new Vector2(0.76f, 0.66f), new Color(categoryColor.r, categoryColor.g, categoryColor.b, 0.28f), squareSprite, 15);
            CreateLocalSprite("Category Charge Fill", blockObject.transform, new Vector3(0f, -0.01f, -0.02f), new Vector2(0.34f, 0.28f), new Color(categoryColor.r, categoryColor.g, categoryColor.b, 0.78f), squareSprite, 17);
            CreateLocalSprite("Quiz Block Scanline Top", blockObject.transform, new Vector3(0f, 0.22f, -0.03f), new Vector2(0.52f, 0.035f), new Color(categoryColor.r, categoryColor.g, categoryColor.b, 0.92f), squareSprite, 18);
            CreateLocalSprite("Quiz Block Scanline Bottom", blockObject.transform, new Vector3(0f, -0.23f, -0.03f), new Vector2(0.52f, 0.035f), new Color(0.05f, 0.95f, 1f, 0.78f), squareSprite, 18);

            GameObject labelObject = new GameObject("Label", typeof(TextMesh));
            labelObject.transform.SetParent(blockObject.transform, false);
            labelObject.transform.localPosition = new Vector3(-0.13f, -0.09f, -0.05f);
            TextMesh label = labelObject.GetComponent<TextMesh>();
            label.text = GetCategoryCode(category);
            label.font = font;
            label.fontSize = 28;
            label.characterSize = 0.055f;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = Color.white;
            MeshRenderer labelRenderer = labelObject.GetComponent<MeshRenderer>();
            if (labelRenderer != null)
            {
                labelRenderer.sortingOrder = 19;
            }
            return block;
        }

        private static void CreateTrap(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Vector2 size, int damage, Color color, Sprite squareSprite)
        {
            GameObject trap = CreateWorldSprite(name, parent, position, size, color, squareSprite, 14).gameObject;
            BoxCollider2D collider = trap.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            CyberGuardianDamageZone zone = trap.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;
            AttachGeneratedGlbVisual(TrapGlbPath, trap.transform, "Generated Trap GLB Visual", new Vector3(0f, -0.28f, -0.18f), Vector3.one * 0.25f, new Vector3(0f, 180f, 0f));
        }

        private static void CreateSawTrap(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, float diameter, int damage, Sprite sawBladeSprite, Sprite fallbackSprite)
        {
            GameObject saw = CreateWorldSprite(name, parent, position, new Vector2(diameter, diameter), Color.white, sawBladeSprite != null ? sawBladeSprite : fallbackSprite, 22).gameObject;
            saw.AddComponent<CyberGuardianRotator>().degreesPerSecond = -260f;
            CircleCollider2D collider = saw.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = diameter * 0.46f;
            CyberGuardianDamageZone zone = saw.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;
            CreateWorldSprite(name + " Arm", parent, position + new Vector2(0f, diameter * 0.65f), new Vector2(0.16f, diameter * 1.2f), Hex("263039"), fallbackSprite, 12);
            SpriteRenderer core = CreateWorldSprite(name + " Core Glow", parent, position, new Vector2(diameter * 0.28f, diameter * 0.28f), new Color(1f, 0.82f, 0.18f, 0.8f), fallbackSprite, 23);
            AddPulse(core, 0.08f, 0.12f, 4.2f, 0f);
        }

        private static void CreateSwingingSawTrap(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 pivotPosition, float diameter, float armLength, int damage, Sprite sawBladeSprite, Sprite fallbackSprite, float phase)
        {
            GameObject pivot = new GameObject(name + " Pivot", typeof(CyberGuardianSwingingTrap));
            pivot.transform.SetParent(parent, false);
            pivot.transform.position = pivotPosition;
            CyberGuardianSwingingTrap swing = pivot.GetComponent<CyberGuardianSwingingTrap>();
            swing.angle = 34f;
            swing.speed = 1.15f;
            swing.phase = phase;

            CreateLocalSprite(name + " Anchor", pivot.transform, Vector3.zero, new Vector2(0.46f, 0.46f), Hex("263039"), fallbackSprite, 21);
            CreateLocalSprite(name + " Hanging Arm", pivot.transform, new Vector3(0f, -armLength * 0.5f, 0.02f), new Vector2(0.14f, armLength), Hex("303942"), fallbackSprite, 20);

            GameObject saw = CreateLocalSprite(name + " Blade", pivot.transform, new Vector3(0f, -armLength, -0.04f), new Vector2(diameter, diameter), Color.white, sawBladeSprite != null ? sawBladeSprite : fallbackSprite, 26).gameObject;
            saw.AddComponent<CyberGuardianRotator>().degreesPerSecond = -330f;
            CircleCollider2D collider = saw.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = diameter * 0.46f;
            CyberGuardianDamageZone zone = saw.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;

            SpriteRenderer glow = CreateLocalSprite(name + " Core Glow", saw.transform, Vector3.zero, new Vector2(diameter * 0.30f, diameter * 0.30f), new Color(1f, 0.82f, 0.18f, 0.78f), fallbackSprite, 27);
            AddPulse(glow, 0.08f, 0.12f, 4.2f, phase);
        }

        private static void CreatePacketLaser(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Vector2 size, int damage, Sprite squareSprite)
        {
            CreateWorldSprite(name + " Warning Glow", parent, position, size + new Vector2(0.3f, 0.28f), new Color(1f, 0.82f, 0.05f, 0.30f), squareSprite, 13);
            GameObject laser = CreateWorldSprite(name, parent, position, size, new Color(1f, 0.9f, 0.05f, 0.95f), squareSprite, 24).gameObject;
            BoxCollider2D collider = laser.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            CyberGuardianDamageZone zone = laser.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;
        }

        private static void CreateLaserBarrier(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 center, float width, int beams, int damage, Sprite squareSprite)
        {
            CreateWorldSprite(name + " Left Pylon", parent, center + new Vector2(-width * 0.5f, 0f), new Vector2(0.25f, 1.25f), Hex("303942"), squareSprite, 18);
            CreateWorldSprite(name + " Right Pylon", parent, center + new Vector2(width * 0.5f, 0f), new Vector2(0.25f, 1.25f), Hex("303942"), squareSprite, 18);
            for (int i = 0; i < beams; i++)
            {
                float y = center.y + (i - (beams - 1) * 0.5f) * 0.33f;
                CreatePacketLaser(name + " Beam " + i, parent, game, new Vector2(center.x, y), new Vector2(width, 0.08f), damage, squareSprite);
            }
        }

        private static void CreateSpikeTrap(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, float width, int damage, Sprite spikeSprite, Sprite fallbackSprite)
        {
            GameObject trap = CreateWorldSprite(name, parent, position, new Vector2(width, 0.46f), Color.white, spikeSprite != null ? spikeSprite : fallbackSprite, 23).gameObject;
            BoxCollider2D collider = trap.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(width, 0.32f);
            CyberGuardianDamageZone zone = trap.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;
        }

        private static void CreateElectricNode(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, int damage, Sprite nodeSprite, Sprite fallbackSprite)
        {
            SpriteRenderer glow = CreateWorldSprite(name + " Glow", parent, position, new Vector2(1.2f, 1.2f), new Color(0.35f, 1f, 1f, 0.24f), fallbackSprite, 16);
            AddPulse(glow, 0.10f, 0.12f, 3.6f, 0.4f);
            GameObject node = CreateWorldSprite(name, parent, position, new Vector2(0.72f, 0.92f), Color.white, nodeSprite != null ? nodeSprite : fallbackSprite, 24).gameObject;
            AddPulse(node.GetComponent<SpriteRenderer>(), 0.025f, 0.035f, 4.4f, 1.0f);
            CircleCollider2D collider = node.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.38f;
            CyberGuardianDamageZone zone = node.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;
        }

        private static void CreateGlitchMine(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, int damage, Sprite mineSprite, Sprite fallbackSprite)
        {
            GameObject mine = CreateWorldSprite(name, parent, position, new Vector2(0.72f, 0.72f), Color.white, mineSprite != null ? mineSprite : fallbackSprite, 24).gameObject;
            AddPulse(mine.GetComponent<SpriteRenderer>(), 0.075f, 0.10f, 5.2f, 0.25f);
            CircleCollider2D collider = mine.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.34f;
            CyberGuardianDamageZone zone = mine.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;
        }

        private static void CreateCrushingBlock(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Vector3 offset, int damage, Sprite crusherSprite, Sprite fallbackSprite)
        {
            GameObject crusher = CreateWorldSprite(name, parent, position, new Vector2(1.1f, 1.1f), Color.white, crusherSprite != null ? crusherSprite : fallbackSprite, 25).gameObject;
            BoxCollider2D collider = crusher.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.94f, 0.94f);
            CyberGuardianDamageZone zone = crusher.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;
            CyberGuardianMover mover = crusher.AddComponent<CyberGuardianMover>();
            mover.localOffset = offset;
            mover.speed = 0.92f;
            mover.pauseTime = 0.18f;
        }

        private static void CreateVirusTurret(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Vector2 direction, Sprite turretSprite, Sprite fallbackSprite)
        {
            GameObject turretObject = CreateWorldSprite(name, parent, position, new Vector2(0.9f, 0.78f), Color.white, turretSprite != null ? turretSprite : fallbackSprite, 24).gameObject;
            BoxCollider2D collider = turretObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.78f, 0.62f);
            CyberGuardianTurret turret = turretObject.AddComponent<CyberGuardianTurret>();
            turret.game = game;
            turret.projectilePrefab = game.bossProjectilePrefab;
            turret.direction = direction;
            turret.fireInterval = 1.55f;
            turret.projectileSpeed = 7.4f;

            Transform muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(turretObject.transform, false);
            muzzle.localPosition = new Vector3(direction.normalized.x * 0.46f, 0.06f, 0f);
            turret.muzzle = muzzle;
        }

        private static void CreateMovingPlatform(string name, Transform parent, Vector2 position, Vector3 offset, Sprite platformSprite, Sprite fallbackSprite)
        {
            GameObject platform = CreateWorldSprite(name, parent, position, new Vector2(2.2f, 0.42f), Color.white, platformSprite != null ? platformSprite : fallbackSprite, 16).gameObject;
            BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(2.1f, 0.36f);
            CyberGuardianMover mover = platform.AddComponent<CyberGuardianMover>();
            mover.localOffset = offset;
            mover.speed = 0.65f;
            mover.pauseTime = 0.35f;
        }

        private static void CreateCheckpoint(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Sprite fallbackSprite, Sprite nodeSprite)
        {
            GameObject checkpoint = CreateWorldSprite(name, parent, position, new Vector2(0.7f, 0.9f), Color.white, nodeSprite != null ? nodeSprite : fallbackSprite, 21).gameObject;
            CircleCollider2D collider = checkpoint.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.48f;
            Transform recovery = new GameObject("Recovery Point").transform;
            recovery.SetParent(checkpoint.transform, false);
            recovery.localPosition = new Vector3(0f, 0.42f, 0f);
            CyberGuardianCheckpoint trigger = checkpoint.AddComponent<CyberGuardianCheckpoint>();
            trigger.game = game;
            trigger.recoveryPoint = recovery;
        }

        private static void CreateRecoveryZone(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Vector2 size)
        {
            GameObject zone = new GameObject(name, typeof(BoxCollider2D), typeof(CyberGuardianRecoveryZone));
            zone.transform.SetParent(parent, false);
            zone.transform.position = position;
            BoxCollider2D collider = zone.GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = size;
            zone.GetComponent<CyberGuardianRecoveryZone>().game = game;
        }

        private static void CreatePlatform(string name, Transform parent, Vector2 position, Vector2 size, Color color, Sprite sprite)
        {
            GameObject platform = CreateWorldSprite(name, parent, position, size, color, sprite, 10).gameObject;
            platform.AddComponent<BoxCollider2D>();
        }

        private static GameObject CreateCyberPlatform(string name, Transform parent, Vector2 center, int columns, int rows, Sprite rockTileSprite, Sprite metalCrateSprite, Sprite dataMossSprite, bool metal)
        {
            const float tile = 0.72f;
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = center;
            BoxCollider2D collider = root.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(columns * tile, rows * tile);

            Sprite tileSprite = metal && metalCrateSprite != null ? metalCrateSprite : rockTileSprite;
            CreateLocalSprite("Platform Contrast Backplate", root.transform, new Vector3(0.04f, -0.05f, 0.08f), new Vector2(columns * tile + 0.24f, rows * tile + 0.22f), new Color(0f, 0f, 0f, 0.74f), tileSprite, 9);
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    float localX = (x - (columns - 1) * 0.5f) * tile;
                    float localY = (y - (rows - 1) * 0.5f) * tile;
                    Color color = metal ? Color.white : (y == rows - 1 ? Hex("707681") : Hex("555963"));
                    CreateLocalSprite("Tile " + x + "-" + y, root.transform, new Vector3(localX, localY, 0f), new Vector2(tile * 1.02f, tile * 1.02f), color, tileSprite, 10);
                }
            }

            for (int x = 0; x < columns; x++)
            {
                float localX = (x - (columns - 1) * 0.5f) * tile;
                CreateLocalSprite("Data Moss " + x, root.transform, new Vector3(localX, rows * tile * 0.5f + 0.07f, -0.02f), new Vector2(tile * 1.05f, 0.23f), metal ? Hex("6EF7FF") : Hex("5BE85D"), dataMossSprite, 12);
            }

            return root;
        }

        private static GameObject CreateHorrorPlatform(string name, Transform parent, Vector2 center, int columns, int rows, Sprite tileSprite, Sprite edgeSprite, Sprite dataMossSprite, Color tileTint, Color edgeTint)
        {
            const float tile = 0.72f;
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = center;
            BoxCollider2D collider = root.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(columns * tile, rows * tile);

            Sprite activeTile = tileSprite != null ? tileSprite : edgeSprite;
            CreateLocalSprite("Horror Platform Contrast Backplate", root.transform, new Vector3(0.04f, -0.05f, 0.08f), new Vector2(columns * tile + 0.28f, rows * tile + 0.24f), new Color(0f, 0f, 0f, 0.78f), activeTile, 9);
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    float localX = (x - (columns - 1) * 0.5f) * tile;
                    float localY = (y - (rows - 1) * 0.5f) * tile;
                    CreateLocalSprite("Horror Tile " + x + "-" + y, root.transform, new Vector3(localX, localY, 0f), new Vector2(tile * 1.02f, tile * 1.02f), tileTint, activeTile, 10);
                }
            }

            Sprite topSprite = edgeSprite != null ? edgeSprite : activeTile;
            for (int x = 0; x < columns; x++)
            {
                float localX = (x - (columns - 1) * 0.5f) * tile;
                CreateLocalSprite("Glow Edge " + x, root.transform, new Vector3(localX, rows * tile * 0.5f + 0.06f, -0.02f), new Vector2(tile * 1.05f, 0.22f), edgeTint, topSprite, 12);
            }

            return root;
        }

        private static SpriteRenderer CreateWorldSprite(string name, Transform parent, Vector2 position, Vector2 size, Color color, Sprite sprite, int sortingOrder)
        {
            GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.position = new Vector3(position.x, position.y, 0f);
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            ScaleSprite(renderer, size);
            return renderer;
        }

        private static void AddPulse(SpriteRenderer renderer, float scaleAmplitude, float alphaAmplitude, float speed, float phase)
        {
            if (renderer == null)
            {
                return;
            }

            CyberGuardianPulseVisual pulse = renderer.gameObject.AddComponent<CyberGuardianPulseVisual>();
            pulse.scaleAmplitude = scaleAmplitude;
            pulse.alphaAmplitude = alphaAmplitude;
            pulse.speed = speed;
            pulse.phase = phase;
        }

        private static SpriteRenderer CreateLocalSprite(string name, Transform parent, Vector3 localPosition, Vector2 size, Color color, Sprite sprite, int sortingOrder)
        {
            GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = localPosition;
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            ScaleSprite(renderer, size);
            return renderer;
        }

        private static void ScaleSprite(SpriteRenderer renderer, Vector2 size)
        {
            if (renderer == null || renderer.sprite == null)
            {
                return;
            }

            Vector2 spriteSize = renderer.sprite.bounds.size;
            renderer.transform.localScale = new Vector3(size.x / spriteSize.x, size.y / spriteSize.y, 1f);
        }

        private static GameObject AttachGeneratedGlbVisual(string assetPath, Transform parent, string name, Vector3 localPosition, Vector3 localScale, Vector3 localEulerAngles)
        {
            if (!File.Exists(ToAbsolutePath(assetPath)))
            {
                return null;
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (source == null)
            {
                return null;
            }

            GameObject visual = PrefabUtility.InstantiatePrefab(source) as GameObject;
            if (visual == null)
            {
                visual = Object.Instantiate(source);
            }

            visual.name = name;
            visual.transform.SetParent(parent, false);
            visual.transform.localPosition = localPosition;
            visual.transform.localRotation = Quaternion.Euler(localEulerAngles);
            visual.transform.localScale = localScale;
            return visual;
        }

        private static LineRenderer CreateLine(string name, Transform parent, Color color, float width)
        {
            GameObject lineObject = new GameObject(name, typeof(LineRenderer));
            lineObject.transform.SetParent(parent, false);
            LineRenderer line = lineObject.GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = color;
            line.endColor = color;
            line.startWidth = width;
            line.endWidth = width;
            line.positionCount = 0;
            line.enabled = false;
            return line;
        }

        private static Image AddBar(Transform parent, Vector2 position, Vector2 size, Color fillColor, Sprite panelSprite)
        {
            AddPanel("Bar Back", parent, position, size, Color.black, panelSprite, 0.66f);
            Image fill = AddImage("Bar Fill", parent, position - new Vector2(0f, 0f), size - new Vector2(14f, 12f), fillColor, panelSprite);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;
            return fill;
        }

        private static Image AddCyberBar(Transform parent, Vector2 position, Vector2 size, Color fillColor, Sprite backSprite, Sprite fillSprite)
        {
            AddPanel("Cyber Bar Back", parent, position, size, Color.white, backSprite, 1f);
            Image fill = AddImage("Cyber Bar Fill", parent, position, size - new Vector2(24f, 18f), fillColor, fillSprite);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;

            int segments = 10;
            float usableWidth = size.x - 42f;
            for (int i = 1; i < segments; i++)
            {
                float x = position.x - usableWidth * 0.5f + usableWidth * i / segments;
                AddPanel("Cyber Bar Segment " + i, parent, new Vector2(x, position.y), new Vector2(4f, size.y - 22f), Color.black, backSprite, 0.58f);
            }

            return fill;
        }

        private static GameObject EnsureBossProjectilePrefab(Sprite circleSprite, Sprite sparkSprite)
        {
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(BossProjectilePrefabPath);
            if (existing != null)
            {
                return existing;
            }

            GameObject prefabObject = new GameObject("BossPacketProjectile", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(CyberGuardianBossProjectile));
            SpriteRenderer renderer = prefabObject.GetComponent<SpriteRenderer>();
            renderer.sprite = sparkSprite != null ? sparkSprite : circleSprite;
            renderer.color = new Color(1f, 0.22f, 0.30f, 1f);
            renderer.sortingOrder = 26;
            ScaleSprite(renderer, new Vector2(0.42f, 0.42f));
            CircleCollider2D collider = prefabObject.GetComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.22f;
            PrefabUtility.SaveAsPrefabAsset(prefabObject, BossProjectilePrefabPath);
            Object.DestroyImmediate(prefabObject);
            return AssetDatabase.LoadAssetAtPath<GameObject>(BossProjectilePrefabPath);
        }

        private static GameObject CreateCanvas(string name)
        {
            GameObject canvasObject = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return canvasObject;
        }

        private static Camera CreateCamera(string backgroundHex, float orthographicSize, Vector3 position)
        {
            Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Hex(backgroundHex);
            camera.orthographic = true;
            camera.orthographicSize = orthographicSize;
            camera.transform.position = position;
            return camera;
        }

        private static AudioSource AddLoopingMusic(string name, AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return null;
            }

            GameObject musicObject = new GameObject(name, typeof(AudioSource));
            AudioSource source = musicObject.GetComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.playOnAwake = true;
            source.volume = volume;
            return source;
        }

        private static void ConfigureGameplayAudio(CyberGuardianSideScrollerGame game, string levelMusicPath)
        {
            if (game == null)
            {
                return;
            }

            game.musicSource = game.gameObject.AddComponent<AudioSource>();
            game.musicSource.playOnAwake = false;
            game.musicSource.loop = true;
            game.musicSource.volume = 0.46f;
            game.adventureMusic = EnsureImportedAudioClip(levelMusicPath);
            game.bossMusic = EnsureImportedAudioClip(BossMusicPath);
            game.jumpSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Player_Jump.wav");
            game.playerHitSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Player_Hit.wav");
            game.playerDeathSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Player_Death.wav");
            game.playerBoostSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Player_Boost.wav");
            game.playerRecoverySfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Player_Recovery.wav");
            game.playerShootSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Player_Shoot.wav");
            game.checkpointSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Player_Checkpoint.wav");
            game.quizOpenSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Quiz_Open.wav");
            game.quizCorrectSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Quiz_Correct.wav");
            game.quizWrongSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Quiz_Wrong.wav");
            game.bossShieldBreakSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Boss_Shield_Break.wav");
            game.bossDamageSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Boss_Damage.wav");
            game.bossDefeatSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Boss_Defeat.wav");
            game.bossWarningSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Boss_Warning.wav");
            game.bossSlingshotPullSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Boss_Slingshot_Pull.wav");
            game.bossSlingshotLaunchSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Boss_Slingshot_Launch.wav");
            game.enemyDeathSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Enemy_Death.wav");
            game.powerupEnergySfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Powerup_Energy.wav");
            game.powerupHealthSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_Powerup_Health.wav");
            game.countdownBeepSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_UI_Countdown_Beep.wav");
            game.countdownStartSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_UI_Countdown_Start.wav");
            game.gameOverSfx = EnsureImportedAudioClip(NamedSfxFolder + "SFX_UI_Game_Over.wav");
        }

        private static void EnsureEventSystem()
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static Button AddButton(string name, Transform parent, Vector2 position, Vector2 size, string text, int fontSize, Font font, Color background, Color textColor, Sprite sprite, out Text label, bool showIcon = true)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Image image = buttonObject.GetComponent<Image>();
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.color = background;
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Hex("61F7FF");
            colors.pressedColor = Hex("FF3B88");
            colors.selectedColor = Hex("61F7FF");
            colors.disabledColor = new Color(0.12f, 0.16f, 0.18f, 0.45f);
            colors.colorMultiplier = 1f;
            button.colors = colors;

            string icon = showIcon ? GetButtonIcon(text) : string.Empty;
            if (!string.IsNullOrEmpty(icon))
            {
                Text iconLabel = AddText(name + " Icon", buttonObject.transform, new Vector2(-size.x * 0.38f, 0f), new Vector2(48f, size.y - 10f), icon, Mathf.Max(16, fontSize - 1), Hex("61F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
                iconLabel.raycastTarget = false;
            }

            Vector2 labelOffset = string.IsNullOrEmpty(icon) ? Vector2.zero : new Vector2(20f, 0f);
            Vector2 labelSize = string.IsNullOrEmpty(icon) ? size - new Vector2(12f, 10f) : size - new Vector2(82f, 10f);
            label = AddText(name + " Label", buttonObject.transform, labelOffset, labelSize, text, fontSize, textColor, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            label.raycastTarget = false;
            return button;
        }

        private static string GetButtonIcon(string text)
        {
            switch (text)
            {
                case "PAUSE":
                case "JEDA":
                    return "||";
                case "PLAY":
                case "MULAI":
                case "LANJUT":
                case "RESUME":
                case "START LEVEL 01":
                    return ">";
                case "CONTINUE":
                case "LANJUTKAN":
                    return ">>";
                case "SETTINGS":
                case "PENGATURAN":
                    return "O";
                case "CREDITS":
                case "KREDIT":
                    return "@";
                case "CONFIRM":
                    return "V";
                case "EXIT":
                case "QUIT":
                case "CANCEL":
                case "KELUAR":
                case "BATAL":
                    return "X";
                case "MAIN":
                case "MENU":
                    return "H";
                case "BACK":
                case "KEMBALI":
                    return "<";
                case "RETRY":
                case "RST":
                case "ULANGI":
                    return "R";
                case "EASY":
                case "NORMAL":
                case "HARD":
                case "MUDAH":
                case "SULIT":
                    return "/";
                default:
                    return string.Empty;
            }
        }

        private static Image AddPanel(string name, Transform parent, Vector2 position, Vector2 size, Color color, Sprite sprite, float alpha)
        {
            color.a = alpha;
            Image image = AddImage(name, parent, position, size, color, sprite);
            image.type = Image.Type.Sliced;
            return image;
        }

        private static Image AddStretchImage(string name, Transform parent, Color color, Sprite sprite)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image image = imageObject.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            return image;
        }

        private static RawImage AddStretchRawImage(string name, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            imageObject.transform.SetParent(parent, false);
            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            RawImage image = imageObject.GetComponent<RawImage>();
            image.color = color;
            return image;
        }

        private static Image AddImage(string name, Transform parent, Vector2 position, Vector2 size, Color color, Sprite sprite)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Image image = imageObject.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            return image;
        }

        private static Text AddText(string name, Transform parent, Vector2 position, Vector2 size, string text, int fontSize, Color color, Font font, TextAnchor anchor, FontStyle style)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Text textComponent = textObject.GetComponent<Text>();
            textComponent.text = text;
            textComponent.font = font;
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = style;
            textComponent.color = color;
            textComponent.alignment = anchor;
            textComponent.resizeTextForBestFit = true;
            textComponent.resizeTextMinSize = Mathf.Max(8, fontSize - 8);
            textComponent.resizeTextMaxSize = fontSize;
            textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            return textComponent;
        }

        private static void SetBuildScenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(DifficultyScenePath, true),
                new EditorBuildSettingsScene(LevelScenePath, true),
                new EditorBuildSettingsScene(Level02ScenePath, true),
                new EditorBuildSettingsScene(Level03ScenePath, true)
            };
        }

        private static QuizQuestionBank EnsureQuestionBank()
        {
            Directory.CreateDirectory(ToAbsolutePath("Assets/CyberGuardian/Data/Quiz"));
            QuizQuestionBank bank = AssetDatabase.LoadAssetAtPath<QuizQuestionBank>(StarterQuestionBankPath);
            if (bank == null)
            {
                bank = ScriptableObject.CreateInstance<QuizQuestionBank>();
                AssetDatabase.CreateAsset(bank, StarterQuestionBankPath);
            }

            if (bank.questions.Count == 0)
            {
                bank.questions.AddRange(new[]
                {
                    new QuizQuestion(CyberQuestionCategory.Password, "GERBANG PASSWORD", "Password yang baik sebaiknya...", new[] { "Panjang, unik, dan sulit ditebak", "Sama untuk semua akun", "Berisi tanggal lahir", "Dibagikan ke teman" }, 0, "Benar. Password perlu panjang, unik, dan tidak dipakai ulang."),
                    new QuizQuestion(CyberQuestionCategory.Password, "PERISAI LOGIN", "Autentikasi dua faktor berguna untuk...", new[] { "Menambah lapisan verifikasi", "Menghapus password", "Membuka semua akun", "Melemahkan akun" }, 0, "Benar. 2FA menambah bukti verifikasi selain password."),
                    new QuizQuestion(CyberQuestionCategory.Malware, "BLOK MALWARE", "Lampiran asing dari email tidak dikenal sebaiknya...", new[] { "Tidak dibuka sembarangan", "Langsung dijalankan", "Dibagikan ulang", "Diubah namanya saja" }, 0, "Benar. Lampiran asing bisa membawa malware."),
                    new QuizQuestion(CyberQuestionCategory.Malware, "FILTER PHISHING", "Tanda umum phishing adalah...", new[] { "Alamat pengirim mencurigakan", "Bahasa selalu sempurna", "Tidak pernah ada link", "Selalu dari teman" }, 0, "Benar. Pengirim dan domain harus diperiksa."),
                    new QuizQuestion(CyberQuestionCategory.Network, "DINDING JARINGAN", "Firewall membantu kita untuk...", new[] { "Menyaring koneksi berbahaya", "Membuka semua port", "Membuat virus", "Mematikan update" }, 0, "Benar. Firewall membantu mengontrol koneksi masuk dan keluar."),
                    new QuizQuestion(CyberQuestionCategory.Network, "RUTE PATCH", "Mengapa update sistem penting?", new[] { "Menutup celah keamanan", "Melepas proteksi", "Membagikan data", "Mematikan enkripsi" }, 0, "Benar. Update sering membawa patch keamanan."),
                    new QuizQuestion(CyberQuestionCategory.Privacy, "KUNCI PRIVASI", "Data yang tidak boleh dibagikan sembarangan adalah...", new[] { "OTP, password, NIK", "Genre game favorit", "Warna kesukaan", "Nama panggilan" }, 0, "Benar. Data sensitif dapat dipakai untuk penipuan."),
                    new QuizQuestion(CyberQuestionCategory.Privacy, "MINIMISASI DATA", "Prinsip minimisasi data berarti...", new[] { "Hanya memakai data yang perlu", "Mengumpulkan semua data", "Menyimpan tanpa batas", "Membagikan cadangan" }, 0, "Benar. Data harus sesuai kebutuhan.")
                });
                EditorUtility.SetDirty(bank);
            }

            return bank;
        }

        private static DifficultyProfile[] EnsureDifficultyProfiles()
        {
            Directory.CreateDirectory(ToAbsolutePath("Assets/CyberGuardian/Data/Difficulty"));
            return new[]
            {
                EnsureDifficultyProfile(EasyDifficultyPath, "Mudah", 110f, 100, 10, 5, 80, 8, 8, 420, 25),
                EnsureDifficultyProfile(NormalDifficultyPath, "Normal", 87f, 100, 25, 7, 100, 12, 12, 600, 30),
                EnsureDifficultyProfile(HardDifficultyPath, "Sulit", 70f, 85, 35, 10, 120, 18, 18, 800, 22)
            };
        }

        private static DifficultyProfile EnsureDifficultyProfile(string path, string displayName, float time, int shield, int virus, int routeOrbs, int correctScoreReward, int wrongDamage, int wrongVirusGain, int routeScore, int routeDamage)
        {
            DifficultyProfile profile = AssetDatabase.LoadAssetAtPath<DifficultyProfile>(path);
            if (profile != null)
            {
                profile.displayName = displayName;
                profile.startingTime = time;
                profile.startingScore = 0;
                profile.startingShield = Mathf.Clamp(shield, 1, 100);
                profile.startingVirusStrength = virus;
                profile.requiredRouteOrbs = routeOrbs;
                profile.correctScoreReward = correctScoreReward;
                profile.correctShieldReward = displayName == "Sulit" ? 4 : (displayName == "Normal" ? 6 : 8);
                profile.wrongShieldDamage = wrongDamage;
                profile.wrongVirusGain = wrongVirusGain;
                profile.routeScoreReward = routeScore;
                profile.routeVirusDamage = routeDamage;
                EditorUtility.SetDirty(profile);
                return profile;
            }

            profile = ScriptableObject.CreateInstance<DifficultyProfile>();
            profile.displayName = displayName;
            profile.startingTime = time;
            profile.startingScore = 0;
            profile.startingTokens = 15;
            profile.startingShield = Mathf.Clamp(shield, 1, 100);
            profile.startingVirusStrength = virus;
            profile.requiredRouteOrbs = routeOrbs;
            profile.correctScoreReward = correctScoreReward;
            profile.correctTokenReward = 1;
            profile.correctShieldReward = displayName == "Sulit" ? 4 : (displayName == "Normal" ? 6 : 8);
            profile.wrongShieldDamage = wrongDamage;
            profile.wrongVirusGain = wrongVirusGain;
            profile.routeScoreReward = routeScore;
            profile.routeVirusDamage = routeDamage;
            AssetDatabase.CreateAsset(profile, path);
            return profile;
        }

        private static Sprite EnsureImportedSprite(string assetPath)
        {
            return EnsureImportedSprite(assetPath, Vector4.zero);
        }

        private static Sprite[] LoadPlayerSpriteSequence(string prefix)
        {
            List<Sprite> sprites = new List<Sprite>();
            for (int i = 0; i < 64; i++)
            {
                string path = PlayerSpriteFolder + "/" + prefix + "_" + i.ToString("00") + ".png";
                if (!File.Exists(ToAbsolutePath(path)))
                {
                    break;
                }

                Sprite sprite = EnsurePlayerSprite(path);
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }

            return sprites.ToArray();
        }

        private static Sprite[] LoadEnemySpriteSequence(string prefix)
        {
            List<Sprite> sprites = new List<Sprite>();
            for (int i = 0; i < 64; i++)
            {
                string path = EnemySpriteFolder + "/" + prefix + "_" + i.ToString("00") + ".png";
                if (!File.Exists(ToAbsolutePath(path)))
                {
                    break;
                }

                Sprite sprite = EnsureEnemySprite(path);
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }

            return sprites.ToArray();
        }

        private static Sprite EnsurePlayerSprite(string assetPath)
        {
            if (!File.Exists(ToAbsolutePath(assetPath)))
            {
                return null;
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 48;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static Sprite EnsureEnemySprite(string assetPath)
        {
            if (!File.Exists(ToAbsolutePath(assetPath)))
            {
                return null;
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 48;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static Sprite EnsureImportedSprite(string assetPath, Vector4 border)
        {
            if (!File.Exists(ToAbsolutePath(assetPath)))
            {
                return null;
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.spriteBorder = border;
                importer.textureCompression = TextureImporterCompression.CompressedHQ;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static AudioClip EnsureImportedAudioClip(string assetPath)
        {
            if (!File.Exists(ToAbsolutePath(assetPath)))
            {
                return null;
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
        }

        private enum TextureShape
        {
            Square,
            Circle,
            RoundedRect,
            CyberRock,
            MetalCrate,
            DataMoss,
            SawBlade,
            DataCloud,
            CircuitBlock,
            DataStone,
            ServerCore,
            NeonPlatform,
            MetaPanel,
            CrackedBlock,
            CorruptedBlock,
            VirusBlock,
            GlowEdgePlatform,
            SpikeBlock,
            ElectricNode,
            GlitchMine,
            CrushingBlock,
            VirusTurret,
            CorruptedPlatform,
            DataForestBackground,
            ServerRunsBackground,
            CodeAbyssBackground,
            UiButtonCyan,
            UiButtonMagenta,
            UiPanelFrame,
            UiBarBack,
            UiHpBarFill,
            UiBoostBarFill,
            UiBossBarFill,
            UiScorePanel,
            UiAlertPanel,
            UiMenuHeader,
            UiSkullMark,
            QuizBlock
        }

        private static CyberHorrorAssetSprites EnsureCyberHorrorSprites()
        {
            return new CyberHorrorAssetSprites
            {
                CircuitBlock = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_circuit_block.png", TextureShape.CircuitBlock),
                DataStone = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_data_stone.png", TextureShape.DataStone),
                ServerCore = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_server_core.png", TextureShape.ServerCore),
                NeonPlatform = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_neon_platform.png", TextureShape.NeonPlatform),
                MetaPanel = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_meta_panel.png", TextureShape.MetaPanel),
                CrackedBlock = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_cracked_block.png", TextureShape.CrackedBlock),
                CorruptedBlock = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_corrupted_block.png", TextureShape.CorruptedBlock),
                VirusBlock = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_virus_block.png", TextureShape.VirusBlock),
                GlowEdgePlatform = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_glow_edge_platform.png", TextureShape.GlowEdgePlatform),
                SpikeBlock = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_spike_block.png", TextureShape.SpikeBlock),
                ElectricNode = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_electric_node.png", TextureShape.ElectricNode),
                GlitchMine = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_glitch_mine.png", TextureShape.GlitchMine),
                CrushingBlock = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_crushing_block.png", TextureShape.CrushingBlock),
                VirusTurret = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_virus_turret.png", TextureShape.VirusTurret),
                CorruptedPlatform = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_corrupted_platform.png", TextureShape.CorruptedPlatform),
                DataForestBackground = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_bg_data_forest.png", TextureShape.DataForestBackground),
                ServerRunsBackground = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_bg_server_runs.png", TextureShape.ServerRunsBackground),
                CodeAbyssBackground = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_bg_code_abyss.png", TextureShape.CodeAbyssBackground),
                UiButtonCyan = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_button_cyan.png", TextureShape.UiButtonCyan),
                UiButtonMagenta = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_button_magenta.png", TextureShape.UiButtonMagenta),
                UiPanelFrame = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_panel_frame.png", TextureShape.UiPanelFrame),
                UiBarBack = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_bar_back.png", TextureShape.UiBarBack),
                UiHpBarFill = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_bar_hp_fill.png", TextureShape.UiHpBarFill),
                UiBoostBarFill = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_bar_boost_fill.png", TextureShape.UiBoostBarFill),
                UiBossBarFill = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_bar_boss_fill.png", TextureShape.UiBossBarFill),
                UiScorePanel = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_score_panel.png", TextureShape.UiScorePanel),
                UiAlertPanel = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_alert_panel.png", TextureShape.UiAlertPanel),
                UiMenuHeader = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_menu_header.png", TextureShape.UiMenuHeader),
                UiSkullMark = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_ui_skull_mark.png", TextureShape.UiSkullMark),
                QuizBlock = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_quiz_block.png", TextureShape.QuizBlock)
            };
        }

        private static Sprite EnsureGeneratedSprite(string assetPath, TextureShape shape)
        {
            if (RegenerateGeneratedSprites || !File.Exists(ToAbsolutePath(assetPath)))
            {
                Texture2D texture = GenerateTexture(shape, 256);
                File.WriteAllBytes(ToAbsolutePath(assetPath), texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.spriteBorder = IsSlicedGeneratedSprite(shape) ? new Vector4(22f, 22f, 22f, 22f) : Vector4.zero;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static Texture2D GenerateTexture(TextureShape shape, int size)
        {
            switch (shape)
            {
                case TextureShape.Circle:
                    return GenerateCircleTexture(size);
                case TextureShape.RoundedRect:
                    return GenerateRoundedRectTexture(size, 22);
                case TextureShape.CyberRock:
                    return GenerateCyberRockTexture(size);
                case TextureShape.MetalCrate:
                    return GenerateMetalCrateTexture(size);
                case TextureShape.DataMoss:
                    return GenerateDataMossTexture(size);
                case TextureShape.SawBlade:
                    return GenerateSawBladeTexture(size);
                case TextureShape.DataCloud:
                    return GenerateDataCloudTexture(size);
                case TextureShape.CircuitBlock:
                    return GenerateCyberTileTexture(size, Hex("26323A"), Hex("9BFFFF"), 0);
                case TextureShape.DataStone:
                    return GenerateCyberTileTexture(size, Hex("555E68"), Hex("D7FFFF"), 1);
                case TextureShape.ServerCore:
                    return GenerateCyberTileTexture(size, Hex("101820"), Hex("35F0FF"), 2);
                case TextureShape.NeonPlatform:
                    return GeneratePlatformTexture(size, Hex("1E3038"), Hex("79FFFF"), false);
                case TextureShape.MetaPanel:
                    return GenerateCyberTileTexture(size, Hex("303A44"), Hex("86FFFF"), 3);
                case TextureShape.CrackedBlock:
                    return GenerateCyberTileTexture(size, Hex("5A5E66"), Hex("F0F6FF"), 4);
                case TextureShape.CorruptedBlock:
                    return GenerateCyberTileTexture(size, Hex("300918"), Hex("FF46A1"), 5);
                case TextureShape.VirusBlock:
                    return GenerateCyberTileTexture(size, Hex("14040C"), Hex("FF2F83"), 6);
                case TextureShape.GlowEdgePlatform:
                    return GeneratePlatformTexture(size, Hex("17242B"), Hex("82FFFF"), false);
                case TextureShape.SpikeBlock:
                    return GenerateSpikeBlockTexture(size);
                case TextureShape.ElectricNode:
                    return GenerateElectricNodeTexture(size);
                case TextureShape.GlitchMine:
                    return GenerateGlitchMineTexture(size);
                case TextureShape.CrushingBlock:
                    return GenerateCrushingBlockTexture(size);
                case TextureShape.VirusTurret:
                    return GenerateVirusTurretTexture(size);
                case TextureShape.CorruptedPlatform:
                    return GeneratePlatformTexture(size, Hex("260812"), Hex("FF2F83"), true);
                case TextureShape.DataForestBackground:
                    return GenerateCyberBackgroundTexture(size, Hex("02090A"), Hex("066F69"), 0);
                case TextureShape.ServerRunsBackground:
                    return GenerateCyberBackgroundTexture(size, Hex("02090F"), Hex("0A5B78"), 1);
                case TextureShape.CodeAbyssBackground:
                    return GenerateCyberBackgroundTexture(size, Hex("02080A"), Hex("0A625F"), 2);
                case TextureShape.UiButtonCyan:
                    return GenerateCyberUiButtonTexture(size, Hex("061116"), Hex("19F2FF"));
                case TextureShape.UiButtonMagenta:
                    return GenerateCyberUiButtonTexture(size, Hex("12070E"), Hex("FF2F83"));
                case TextureShape.UiPanelFrame:
                    return GenerateCyberPanelFrameTexture(size, Hex("050B0F"), Hex("16E8FF"));
                case TextureShape.UiBarBack:
                    return GenerateCyberBarTexture(size, Hex("04070A"), Hex("234A52"), false);
                case TextureShape.UiHpBarFill:
                    return GenerateCyberBarTexture(size, Hex("FF2F83"), Hex("FF8ABB"), true);
                case TextureShape.UiBoostBarFill:
                    return GenerateCyberBarTexture(size, Hex("16E8FF"), Hex("9AFFFF"), true);
                case TextureShape.UiBossBarFill:
                    return GenerateCyberBarTexture(size, Hex("FF2F83"), Hex("FF4B96"), true);
                case TextureShape.UiScorePanel:
                    return GenerateCyberScorePanelTexture(size);
                case TextureShape.UiAlertPanel:
                    return GenerateCyberAlertPanelTexture(size);
                case TextureShape.UiMenuHeader:
                    return GenerateCyberMenuHeaderTexture(size);
                case TextureShape.UiSkullMark:
                    return GenerateCyberSkullMarkTexture(size);
                case TextureShape.QuizBlock:
                    return GenerateQuizBlockTexture(size);
                default:
                    return GenerateSquareTexture(size);
            }
        }

        private static bool IsSlicedGeneratedSprite(TextureShape shape)
        {
            switch (shape)
            {
                case TextureShape.RoundedRect:
                case TextureShape.UiButtonCyan:
                case TextureShape.UiButtonMagenta:
                case TextureShape.UiPanelFrame:
                case TextureShape.UiBarBack:
                case TextureShape.UiHpBarFill:
                case TextureShape.UiBoostBarFill:
                case TextureShape.UiBossBarFill:
                case TextureShape.UiScorePanel:
                case TextureShape.UiAlertPanel:
                case TextureShape.UiMenuHeader:
                case TextureShape.QuizBlock:
                    return true;
                default:
                    return false;
            }
        }

        private static Texture2D GenerateSquareTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCircleTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            float radius = size * 0.48f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float alpha = Mathf.Clamp01(radius + 1.2f - distance);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateRoundedRectTexture(int size, int radius)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(Mathf.Abs(x + 0.5f - size * 0.5f) - (size * 0.5f - radius), 0f);
                    float dy = Mathf.Max(Mathf.Abs(y + 0.5f - size * 0.5f) - (size * 0.5f - radius), 0f);
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(radius + 1.2f - distance);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberRockTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = Mathf.Abs(x - center.x) / center.x;
                    float ny = Mathf.Abs(y - center.y) / center.y;
                    float edge = Mathf.Max(nx, ny);
                    float chip = Mathf.PerlinNoise(x * 0.085f + 2.1f, y * 0.085f + 4.6f);
                    float facet = Mathf.PerlinNoise(x * 0.035f + 8.4f, y * 0.035f + 1.3f);
                    Color baseColor = Color.Lerp(Hex("45464D"), Hex("747781"), facet);
                    if (edge > 0.88f)
                    {
                        baseColor = Color.Lerp(baseColor, Hex("22262C"), 0.45f);
                    }

                    if (chip > 0.72f)
                    {
                        baseColor = Color.Lerp(baseColor, Hex("A7ABB3"), 0.26f);
                    }

                    texture.SetPixel(x, y, baseColor);
                }
            }

            DrawRect(texture, 5, 5, size - 10, size - 10, Hex("2A2C32"));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateMetalCrateTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float centerShade = Vector2.Distance(new Vector2(x, y), new Vector2(size * 0.5f, size * 0.5f)) / (size * 0.72f);
                    Color color = Color.Lerp(Hex("5D6770"), Hex("252B31"), Mathf.Clamp01(centerShade));
                    texture.SetPixel(x, y, color);
                }
            }

            FillRect(texture, 0, 0, size, 10, Hex("9EA7B0"));
            FillRect(texture, 0, size - 10, size, 10, Hex("9EA7B0"));
            FillRect(texture, 0, 0, 10, size, Hex("7F8992"));
            FillRect(texture, size - 10, 0, 10, size, Hex("7F8992"));
            DrawRect(texture, 16, 16, size - 32, size - 32, Hex("2C3339"));
            FillRect(texture, 18, 18, size - 36, size - 36, new Color(0.1f, 0.12f, 0.14f, 0.32f));
            for (int i = 18; i < size - 18; i += 24)
            {
                FillCircle(texture, i, 8, 3, Hex("2E3338"));
                FillCircle(texture, i, size - 9, 3, Hex("2E3338"));
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateDataMossTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float wave = 0.58f + Mathf.PerlinNoise(x * 0.11f, 3.4f) * 0.30f;
                    float height = size * wave;
                    float alpha = y < height ? 1f : 0f;
                    Color color = Color.Lerp(Hex("24A63C"), Hex("78F45C"), Mathf.PerlinNoise(x * 0.05f, y * 0.05f));
                    if (y < size * 0.18f)
                    {
                        color = Hex("0E6129");
                    }

                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateSawBladeTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 delta = new Vector2(x + 0.5f, y + 0.5f) - center;
                    float distance = delta.magnitude / (size * 0.5f);
                    float angle = Mathf.Atan2(delta.y, delta.x);
                    float tooth = Mathf.Sin(angle * 18f) * 0.045f;
                    float outer = 0.84f + tooth;
                    float innerHole = 0.17f;
                    if (distance > outer || distance < innerHole)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    Color color = Color.Lerp(Hex("1D2025"), Hex("A2A9B0"), Mathf.Clamp01(1f - distance));
                    if (distance > 0.72f)
                    {
                        color = Hex("DDE5EA");
                    }

                    texture.SetPixel(x, y, color);
                }
            }

            FillCircle(texture, size / 2, size / 2, 10, Hex("F7D44A"));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateDataCloudTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 delta = new Vector2(x + 0.5f, y + 0.5f) - center;
                    float ellipse = Mathf.Sqrt((delta.x * delta.x) / (size * size * 0.22f) + (delta.y * delta.y) / (size * size * 0.12f));
                    float noise = Mathf.PerlinNoise(x * 0.045f + 1.7f, y * 0.045f + 6.3f);
                    float alpha = Mathf.Clamp01((1.0f - ellipse) * 1.35f + (noise - 0.5f) * 0.42f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberTileTexture(int size, Color baseColor, Color accentColor, int variant)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float edge = Mathf.Max(Mathf.Abs(x - center.x), Mathf.Abs(y - center.y)) / center.x;
                    float noise = Mathf.PerlinNoise(x * 0.055f + variant * 3.1f, y * 0.055f + variant * 7.2f);
                    Color color = Color.Lerp(baseColor, Hex("56616B"), noise * 0.24f);
                    if (edge > 0.86f)
                    {
                        color = Color.Lerp(color, Color.black, 0.42f);
                    }

                    texture.SetPixel(x, y, color);
                }
            }

            DrawRect(texture, 1, 1, size - 2, size - 2, Color.black);
            DrawRect(texture, 6, 6, size - 12, size - 12, Color.Lerp(accentColor, Color.black, 0.08f));
            FillRect(texture, 12, 12, size - 24, size - 24, Color.Lerp(baseColor, Color.black, 0.16f));
            FillRect(texture, 14, size - 20, size - 28, 5, new Color(accentColor.r, accentColor.g, accentColor.b, 0.54f));
            if (variant == 2)
            {
                for (int i = 18; i < size - 18; i += 14)
                {
                    FillRect(texture, size / 2 - 12, i, 24, 5, accentColor);
                }
            }
            else if (variant == 4 || variant == 5)
            {
                Color crack = variant == 5 ? Hex("FF2F83") : Hex("BFD0D9");
                DrawLine(texture, 22, 96, 58, 58, crack);
                DrawLine(texture, 58, 58, 86, 76, crack);
                DrawLine(texture, 70, 18, 55, 56, crack);
                DrawLine(texture, 88, 112, 86, 76, crack);
            }
            else if (variant == 6)
            {
                FillCircle(texture, size / 2, size / 2, 22, new Color(accentColor.r, accentColor.g, accentColor.b, 0.75f));
                DrawLine(texture, 30, 30, 98, 98, accentColor);
                DrawLine(texture, 98, 30, 30, 98, accentColor);
            }
            else
            {
                DrawLine(texture, 18, size / 2, 52, size / 2, accentColor);
                DrawLine(texture, size - 18, size / 2, size - 52, size / 2, accentColor);
                DrawLine(texture, size / 2, 18, size / 2, 52, accentColor);
                DrawLine(texture, size / 2, size - 18, size / 2, size - 52, accentColor);
                FillCircle(texture, size / 2, size / 2, 5, accentColor);
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GeneratePlatformTexture(int size, Color baseColor, Color accentColor, bool corrupted)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float alpha = y > size * 0.70f || y < size * 0.30f ? 0f : 1f;
                    Color color = Color.Lerp(baseColor, Hex("3D464D"), Mathf.PerlinNoise(x * 0.08f, y * 0.08f) * 0.24f);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
            }

            FillRect(texture, 6, 52, size - 12, 9, accentColor);
            FillRect(texture, 14, 66, size - 28, 5, Color.Lerp(accentColor, Color.white, 0.25f));
            DrawRect(texture, 8, 42, size - 16, 42, Color.Lerp(accentColor, Color.black, 0.35f));
            if (corrupted)
            {
                for (int i = 0; i < 8; i++)
                {
                    int x = 18 + i * 13;
                    DrawLine(texture, x, 48, x + 8, 34, accentColor);
                    FillRect(texture, x + 3, 28, 4, 12, accentColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateSpikeBlockTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }

            FillRect(texture, 2, 12, size - 4, 22, Hex("20262D"));
            DrawRect(texture, 2, 12, size - 4, 22, Hex("3F4750"));
            for (int i = 0; i < 7; i++)
            {
                int centerX = 12 + i * 18;
                for (int y = 34; y < 104; y++)
                {
                    int halfWidth = Mathf.Max(1, (104 - y) / 3);
                    for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
                    {
                        if (x >= 0 && x < size)
                        {
                            float t = (y - 34f) / 70f;
                            texture.SetPixel(x, y, Color.Lerp(Hex("FF2F83"), Hex("F3F8FF"), t));
                        }
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateElectricNodeTexture(int size)
        {
            Texture2D texture = GenerateSquareTexture(size);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }

            FillRect(texture, size / 2 - 12, 8, 24, 58, Hex("252D35"));
            DrawRect(texture, size / 2 - 16, 8, 32, 62, Hex("6EF7FF"));
            FillCircle(texture, size / 2, 82, 21, new Color(0.38f, 1f, 1f, 0.75f));
            FillCircle(texture, size / 2, 82, 9, Hex("E8FFFF"));
            DrawLine(texture, 16, 82, 44, 82, Hex("6EF7FF"));
            DrawLine(texture, 84, 82, 112, 82, Hex("6EF7FF"));
            DrawLine(texture, 34, 106, 50, 90, Hex("6EF7FF"));
            DrawLine(texture, 78, 90, 94, 106, Hex("6EF7FF"));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateGlitchMineTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / (size * 0.48f);
                    if (distance > 1f)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    Color color = Color.Lerp(Hex("11151B"), Hex("FF2F83"), Mathf.Clamp01(1f - distance));
                    texture.SetPixel(x, y, color);
                }
            }

            FillCircle(texture, size / 2, size / 2, 10, Hex("FFD0EA"));
            DrawLine(texture, 18, 64, 110, 64, Hex("FF2F83"));
            DrawLine(texture, 64, 18, 64, 110, Hex("FF2F83"));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCrushingBlockTexture(int size)
        {
            Texture2D texture = GenerateCyberTileTexture(size, Hex("2A2E35"), Hex("FF3D89"), 5);
            FillRect(texture, size / 2 - 8, 30, 16, 46, Hex("FF3D89"));
            for (int y = 18; y < 42; y++)
            {
                int half = (42 - y) / 2;
                FillRect(texture, size / 2 - half, y, half * 2 + 1, 2, Hex("FF3D89"));
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateVirusTurretTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }

            FillRect(texture, 28, 22, 72, 20, Hex("252D35"));
            DrawRect(texture, 28, 22, 72, 20, Hex("59636D"));
            FillCircle(texture, 64, 70, 28, Hex("1C222A"));
            FillCircle(texture, 64, 70, 16, Hex("FF2F83"));
            FillCircle(texture, 64, 70, 7, Hex("FFE2F2"));
            FillRect(texture, 14, 62, 34, 14, Hex("303942"));
            FillRect(texture, 6, 65, 18, 8, Hex("FF2F83"));
            FillRect(texture, 54, 6, 20, 18, Hex("1A2027"));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberBackgroundTexture(int size, Color baseColor, Color accentColor, int variant)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float vertical = y / (float)(size - 1);
                    float noise = Mathf.PerlinNoise(x * 0.035f + variant * 2.7f, y * 0.035f + variant * 4.1f);
                    Color color = Color.Lerp(baseColor, Color.Lerp(accentColor, Color.black, 0.45f), vertical * 0.28f + noise * 0.10f);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, 0.88f));
                }
            }

            for (int i = 0; i < 10; i++)
            {
                int x = 8 + i * 13;
                int height = 28 + (i * 17 + variant * 11) % 72;
                FillRect(texture, x, 0, 7, height, Color.Lerp(Hex("071116"), accentColor, 0.22f));
                FillRect(texture, x + 2, 6, 2, Mathf.Max(4, height - 12), new Color(accentColor.r, accentColor.g, accentColor.b, 0.55f));
            }

            FillCircle(texture, 96, 42 + variant * 14, 15, new Color(accentColor.r, accentColor.g, accentColor.b, 0.42f));
            DrawRect(texture, 82, 28 + variant * 14, 28, 28, new Color(accentColor.r, accentColor.g, accentColor.b, 0.55f));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberUiButtonTexture(int size, Color baseColor, Color accentColor)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.12f + 4.4f, y * 0.12f + 9.1f);
                    Color color = Color.Lerp(baseColor, Hex("162028"), noise * 0.28f);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, 0.92f));
                }
            }

            DrawRect(texture, 7, 10, size - 14, size - 20, Color.Lerp(accentColor, Color.black, 0.15f));
            DrawRect(texture, 12, 15, size - 24, size - 30, Hex("27333B"));
            FillRect(texture, 18, 18, size - 36, 7, new Color(accentColor.r, accentColor.g, accentColor.b, 0.72f));
            FillRect(texture, 18, size - 25, size - 36, 6, new Color(accentColor.r, accentColor.g, accentColor.b, 0.48f));
            FillRect(texture, 10, 18, 8, 24, accentColor);
            FillRect(texture, size - 18, size - 42, 8, 24, accentColor);
            DrawLine(texture, 28, 84, 62, 84, new Color(accentColor.r, accentColor.g, accentColor.b, 0.52f));
            DrawLine(texture, 78, 44, 108, 44, new Color(accentColor.r, accentColor.g, accentColor.b, 0.42f));
            FillCircle(texture, 18, 18, 3, Hex("9FFFFF"));
            FillCircle(texture, size - 18, size - 18, 3, Color.Lerp(accentColor, Color.white, 0.35f));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberPanelFrameTexture(int size, Color baseColor, Color accentColor)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float edge = Mathf.Max(Mathf.Abs(x - size * 0.5f), Mathf.Abs(y - size * 0.5f)) / (size * 0.5f);
                    Color color = Color.Lerp(baseColor, Hex("101820"), Mathf.Clamp01(edge) * 0.18f);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, 0.88f));
                }
            }

            DrawRect(texture, 4, 4, size - 8, size - 8, new Color(accentColor.r, accentColor.g, accentColor.b, 0.62f));
            DrawRect(texture, 12, 12, size - 24, size - 24, Hex("1E3038"));
            FillRect(texture, 0, 0, 26, 5, accentColor);
            FillRect(texture, 0, 0, 5, 26, accentColor);
            FillRect(texture, size - 26, size - 5, 26, 5, accentColor);
            FillRect(texture, size - 5, size - 26, 5, 26, accentColor);
            DrawLine(texture, 32, 18, 78, 18, new Color(accentColor.r, accentColor.g, accentColor.b, 0.38f));
            DrawLine(texture, 54, 110, 112, 110, new Color(accentColor.r, accentColor.g, accentColor.b, 0.32f));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberBarTexture(int size, Color baseColor, Color accentColor, bool filled)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float vertical = y / (float)(size - 1);
                    Color color = filled ? Color.Lerp(baseColor, accentColor, vertical * 0.34f) : Color.Lerp(baseColor, Hex("10171B"), vertical * 0.18f);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, filled ? 0.96f : 0.88f));
                }
            }

            DrawRect(texture, 5, 28, size - 10, 72, filled ? Color.Lerp(accentColor, Color.white, 0.25f) : Hex("24545C"));
            FillRect(texture, 12, 38, size - 24, 14, filled ? Color.Lerp(accentColor, Color.white, 0.18f) : Hex("071015"));
            for (int i = 1; i < 8; i++)
            {
                int x = 12 + i * 13;
                DrawLine(texture, x, 31, x - 8, 97, filled ? Color.Lerp(baseColor, Color.black, 0.18f) : Hex("1A242A"));
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberScorePanelTexture(int size)
        {
            Texture2D texture = GenerateCyberUiButtonTexture(size, Hex("130711"), Hex("FF2F83"));
            FillRect(texture, 18, 18, size - 36, size - 36, new Color(0.02f, 0.0f, 0.02f, 0.78f));
            DrawRect(texture, 20, 20, size - 40, size - 40, Hex("FF2F83"));
            FillRect(texture, size - 31, 34, 5, 34, Hex("FF2F83"));
            FillRect(texture, size - 23, 25, 5, 43, Hex("FF6BAA"));
            FillRect(texture, size - 15, 46, 5, 22, Hex("FF2F83"));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberAlertPanelTexture(int size)
        {
            Texture2D texture = GenerateCyberUiButtonTexture(size, Hex("12060A"), Hex("FF2F83"));
            FillRect(texture, 17, 22, 34, 84, new Color(1f, 0.18f, 0.42f, 0.28f));
            DrawLine(texture, 34, 88, 22, 40, Hex("FF5B8D"));
            DrawLine(texture, 22, 40, 46, 40, Hex("FF5B8D"));
            DrawLine(texture, 46, 40, 34, 88, Hex("FF5B8D"));
            FillRect(texture, 32, 50, 4, 22, Hex("FF5B8D"));
            FillRect(texture, 32, 78, 4, 5, Hex("FFFFFF"));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberMenuHeaderTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.10f + 1.6f, y * 0.10f + 8.8f);
                    Color color = Color.Lerp(Hex("010608"), Hex("0B171C"), noise * 0.38f);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, 0.94f));
                }
            }

            DrawRect(texture, 3, 7, size - 6, size - 14, new Color(0.06f, 0.86f, 0.92f, 0.74f));
            DrawRect(texture, 10, 14, size - 20, size - 28, Hex("13272E"));
            FillRect(texture, 0, size - 12, 42, 5, Hex("15DCE8"));
            FillRect(texture, 0, size - 24, 25, 4, Hex("15DCE8"));
            FillRect(texture, size - 44, 7, 44, 5, Hex("FF2F83"));
            FillRect(texture, size - 18, 12, 8, 30, Hex("FF2F83"));
            DrawLine(texture, 20, 23, 56, 23, new Color(0.08f, 0.95f, 1f, 0.42f));
            DrawLine(texture, 68, 94, 116, 94, new Color(0.08f, 0.95f, 1f, 0.32f));
            DrawLine(texture, 18, 112, 42, 100, new Color(0.08f, 0.95f, 1f, 0.35f));
            FillRect(texture, 94, 22, 8, 8, new Color(1f, 0.05f, 0.45f, 0.75f));
            FillRect(texture, 108, 22, 4, 20, new Color(1f, 0.05f, 0.45f, 0.45f));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCyberSkullMarkTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }

            FillCircle(texture, size / 2, 78, 34, new Color(1f, 0.02f, 0.38f, 0.92f));
            FillRect(texture, 39, 35, 50, 38, new Color(1f, 0.02f, 0.38f, 0.92f));
            FillCircle(texture, 48, 78, 10, Hex("05070A"));
            FillCircle(texture, 80, 78, 10, Hex("05070A"));
            FillRect(texture, 59, 55, 10, 13, Hex("05070A"));
            FillRect(texture, 43, 28, 8, 17, Hex("05070A"));
            FillRect(texture, 56, 26, 7, 20, Hex("05070A"));
            FillRect(texture, 68, 26, 7, 20, Hex("05070A"));
            FillRect(texture, 81, 28, 8, 17, Hex("05070A"));
            DrawLine(texture, 28, 99, 14, 112, new Color(1f, 0.02f, 0.38f, 0.70f));
            DrawLine(texture, 100, 99, 114, 112, new Color(1f, 0.02f, 0.38f, 0.70f));
            FillRect(texture, 22, 46, 12, 5, Hex("FF2F83"));
            FillRect(texture, 96, 46, 12, 5, Hex("FF2F83"));
            FillRect(texture, 36, 110, 10, 7, new Color(0.08f, 0.95f, 1f, 0.65f));
            FillRect(texture, 86, 18, 14, 6, new Color(0.08f, 0.95f, 1f, 0.55f));
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateQuizBlockTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float centerShade = Vector2.Distance(new Vector2(x, y), new Vector2(size * 0.5f, size * 0.5f)) / (size * 0.70f);
                    float noise = Mathf.PerlinNoise(x * 0.095f + 3.3f, y * 0.095f + 6.2f);
                    Color baseColor = Color.Lerp(Hex("0B1117"), Hex("303942"), noise * 0.32f);
                    baseColor = Color.Lerp(baseColor, Hex("030607"), Mathf.Clamp01(centerShade) * 0.30f);
                    texture.SetPixel(x, y, baseColor);
                }
            }

            DrawRect(texture, 4, 4, size - 8, size - 8, Hex("5C6670"));
            DrawRect(texture, 10, 10, size - 20, size - 20, Hex("0EF4FF"));
            FillRect(texture, 16, 16, size - 32, size - 32, new Color(0.02f, 0.04f, 0.05f, 0.72f));
            DrawRect(texture, 23, 23, size - 46, size - 46, Hex("263942"));
            DrawLine(texture, 20, 32, 46, 32, Hex("0EF4FF"));
            DrawLine(texture, 32, 20, 32, 46, Hex("0EF4FF"));
            DrawLine(texture, size - 20, size - 32, size - 46, size - 32, Hex("0EF4FF"));
            DrawLine(texture, size - 32, size - 20, size - 32, size - 46, Hex("0EF4FF"));
            DrawLine(texture, 34, size - 30, 64, 78, new Color(1f, 0.08f, 0.50f, 0.82f));
            DrawLine(texture, 64, 78, 92, 92, new Color(1f, 0.08f, 0.50f, 0.70f));
            FillCircle(texture, size / 2, size / 2, 17, new Color(1f, 0.08f, 0.50f, 0.55f));
            FillCircle(texture, size / 2, size / 2, 7, Hex("EAFDFF"));
            FillRect(texture, 18, size - 28, 32, 5, new Color(0.08f, 0.95f, 1f, 0.75f));
            FillRect(texture, size - 50, 23, 32, 5, new Color(1f, 0.08f, 0.50f, 0.78f));
            FillCircle(texture, 16, 16, 4, Hex("B7FFFF"));
            FillCircle(texture, size - 16, 16, 4, Hex("FF87BD"));
            FillCircle(texture, 16, size - 16, 4, Hex("FF87BD"));
            FillCircle(texture, size - 16, size - 16, 4, Hex("B7FFFF"));
            texture.Apply();
            return texture;
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, color);
                    }
                }
            }
        }

        private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color color)
        {
            FillRect(texture, x, y, width, 3, color);
            FillRect(texture, x, y + height - 3, width, 3, color);
            FillRect(texture, x, y, 3, height, color);
            FillRect(texture, x + width - 3, y, 3, height, color);
        }

        private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Mathf.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;
            while (true)
            {
                FillRect(texture, x0 - 1, y0 - 1, 3, 3, color);
                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                int e2 = 2 * error;
                if (e2 >= dy)
                {
                    error += dy;
                    x0 += sx;
                }

                if (e2 <= dx)
                {
                    error += dx;
                    y0 += sy;
                }
            }
        }

        private static void FillCircle(Texture2D texture, int cx, int cy, int radius, Color color)
        {
            int radiusSqr = radius * radius;
            for (int y = cy - radius; y <= cy + radius; y++)
            {
                for (int x = cx - radius; x <= cx + radius; x++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    if (dx * dx + dy * dy <= radiusSqr && x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
        }

        private static string GetCategoryCode(int category)
        {
            switch (Mathf.Abs(category) % 4)
            {
                case 0:
                    return "PW";
                case 1:
                    return "MW";
                case 2:
                    return "NT";
                default:
                    return "PR";
            }
        }

        private static Color GetCategoryColor(int category)
        {
            switch (Mathf.Abs(category) % 4)
            {
                case 0:
                    return new Color(0.18f, 0.58f, 1f, 1f);
                case 1:
                    return new Color(0.40f, 0.86f, 0.28f, 1f);
                case 2:
                    return new Color(1.00f, 0.78f, 0.18f, 1f);
                default:
                    return new Color(0.78f, 0.35f, 1f, 1f);
            }
        }

        private static Font GetUiFont()
        {
            Font cyberpunkFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/CyberGuardian/Art/UI/CyberpunkPixelUI/10 Font/CyberpunkCraftpixPixel.otf");
            if (cyberpunkFont != null)
            {
                return cyberpunkFont;
            }

            Font kenneyFuture = AssetDatabase.LoadAssetAtPath<Font>("Assets/CyberGuardian/Art/UI/KenneySpaceUI/Font/Kenney Future.ttf");
            if (kenneyFuture != null)
            {
                return kenneyFuture;
            }

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out Color color);
            return color;
        }

        private static string ToAbsolutePath(string assetPath)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
