using System.Collections.Generic;
using System.IO;
using CyberGuardian;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CyberGuardian.Editor
{
    public static class CyberGuardianGameBuilder
    {
        private const string MainMenuScenePath = "Assets/CyberGuardian/Scenes/CyberGuardian_MainMenu.unity";
        private const string LevelScenePath = "Assets/CyberGuardian/Scenes/CyberGuardian_Level01.unity";
        private const string GeneratedArtFolder = "Assets/CyberGuardian/Art/Generated";
        private const string StarterQuestionBankPath = "Assets/CyberGuardian/Data/Quiz/CyberSecurity_Starter_QuestionBank.asset";
        private const string EasyDifficultyPath = "Assets/CyberGuardian/Data/Difficulty/Easy.asset";
        private const string NormalDifficultyPath = "Assets/CyberGuardian/Data/Difficulty/Normal.asset";
        private const string HardDifficultyPath = "Assets/CyberGuardian/Data/Difficulty/Hard.asset";
        private const string BossProjectilePrefabPath = "Assets/CyberGuardian/Prefabs/Projectiles/BossPacketProjectile.prefab";

        private const string VirusSpritePath = "Assets/CyberGuardian/Art/Enemies/VirusBigPack/png1.png";
        private const string VirusAltSpritePath = "Assets/CyberGuardian/Art/Enemies/VirusBigPack/png18.png";
        private const string ProjectileSpritePath = "Assets/CyberGuardian/Art/VFX/KenneyParticles/PNG (Transparent)/muzzle_03.png";
        private const string SparkSpritePath = "Assets/CyberGuardian/Art/VFX/KenneyParticles/PNG (Transparent)/spark_02.png";
        private const string CrosshairSpritePath = "Assets/CyberGuardian/Art/UI/KenneySpaceUI/PNG/Blue/Default/crosshair_color_a.png";
        private const string PanelFrameSpritePath = "Assets/CyberGuardian/Art/UI/CyberpunkPixelUI/1 Frames/Frame_05.png";
        private const string ButtonSpritePath = "Assets/CyberGuardian/Art/UI/KenneySpaceUI/PNG/Blue/Default/button_square_header_large_rectangle.png";
        private const string CircuitSpritePath = "Assets/SourceFiles/Textures/Repeating Tiles/Circuit_Albedo.png";

        private const string MeleeSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/laserSmall_001.ogg";
        private const string HitSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/impactMetal_000.ogg";
        private const string BossShotSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/laserLarge_000.ogg";
        private const string ShieldSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/forceField_000.ogg";
        private const string WrongSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/slime_000.ogg";

        private const string GuardianGlbPath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_guardian_player.glb";
        private const string VirusGlbPath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_virus_grunt.glb";
        private const string BossGlbPath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_malware_boss.glb";
        private const string TrapGlbPath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_platform_traps.glb";
        private const string ProjectileGlbPath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_projectiles.glb";

        private const string GuardianGeneratedSpritePath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_guardian_player_sprite.png";
        private const string VirusGeneratedSpritePath = "Assets/CyberGuardian/GeneratedBlenderAssets/cg_virus_grunt_sprite.png";
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

        [MenuItem("Cyber Guardian/Build Main Menu And Level 01")]
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

            QuizQuestionBank questionBank = EnsureQuestionBank();
            DifficultyProfile[] difficulties = EnsureDifficultyProfiles();
            Font font = GetUiFont();
            GameObject bossProjectilePrefab = EnsureBossProjectilePrefab(circleSprite, sparkSprite);

            BuildMainMenuScene(panelSprite, circleSprite, rockTileSprite, metalCrateSprite, dataMossSprite, sawBladeSprite, dataBlobSprite, horrorSprites, virusSprite, frameSprite, buttonSprite, circuitSprite, font);
            BuildLevelScene(squareSprite, circleSprite, panelSprite, rockTileSprite, metalCrateSprite, dataMossSprite, sawBladeSprite, dataBlobSprite, horrorSprites, virusSprite, virusAltSprite, projectileSprite, sparkSprite, crosshairSprite, frameSprite, buttonSprite, circuitSprite, questionBank, difficulties, font, bossProjectilePrefab, meleeSfx, hitSfx, bossShotSfx, shieldSfx, wrongSfx);
            SetBuildScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Cyber Guardian side-scroller main menu and Level 01 built.");
        }

        private static void BuildMainMenuScene(Sprite panelSprite, Sprite circleSprite, Sprite rockTileSprite, Sprite metalCrateSprite, Sprite dataMossSprite, Sprite sawBladeSprite, Sprite dataBlobSprite, CyberHorrorAssetSprites horrorSprites, Sprite virusSprite, Sprite frameSprite, Sprite buttonSprite, Sprite circuitSprite, Font font)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CyberGuardian_MainMenu";

            CreateCamera("07171D", 5.4f, new Vector3(0f, 0.35f, -10f));
            EnsureEventSystem();

            GameObject canvasObject = CreateCanvas("Cyber Guardian Main Menu");
            AddStretchImage("Background", canvasObject.transform, Hex("0A2026"), panelSprite);
            if (circuitSprite != null)
            {
                Image circuit = AddStretchImage("Circuit Texture", canvasObject.transform, new Color(0.2f, 0.9f, 0.95f, 0.14f), circuitSprite);
                circuit.type = Image.Type.Tiled;
                circuit.pixelsPerUnitMultiplier = 0.38f;
            }

            Sprite panelFrame = horrorSprites != null && horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : panelSprite;
            Sprite menuHeader = horrorSprites != null && horrorSprites.UiMenuHeader != null ? horrorSprites.UiMenuHeader : panelFrame;
            Sprite skullMark = horrorSprites != null && horrorSprites.UiSkullMark != null ? horrorSprites.UiSkullMark : circleSprite;
            Sprite activeButton = horrorSprites != null && horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : (buttonSprite != null ? buttonSprite : panelSprite);
            Sprite dangerButton = horrorSprites != null && horrorSprites.UiButtonMagenta != null ? horrorSprites.UiButtonMagenta : activeButton;

            AddImage("Menu Data Cloud A", canvasObject.transform, new Vector2(-610f, 130f), new Vector2(790f, 510f), new Color(0.05f, 0.85f, 0.90f, 0.13f), dataBlobSprite);
            AddImage("Menu Data Cloud B", canvasObject.transform, new Vector2(508f, 112f), new Vector2(860f, 520f), new Color(0.85f, 0.08f, 0.55f, 0.12f), dataBlobSprite);
            AddPanel("Outer Cyber Horror Frame", canvasObject.transform, new Vector2(0f, 0f), new Vector2(1848f, 1002f), Color.black, panelFrame, 0.54f);

            AddImage("Cyber Horror Header", canvasObject.transform, new Vector2(-560f, 432f), new Vector2(760f, 136f), Color.white, menuHeader);
            AddText("Title", canvasObject.transform, new Vector2(-630f, 452f), new Vector2(560f, 72f), "CYBER GUARDIAN", 56, Color.white, font, TextAnchor.MiddleLeft, FontStyle.Bold);
            AddImage("Cyber Skull Mark", canvasObject.transform, new Vector2(-215f, 456f), new Vector2(70f, 70f), Color.white, skullMark);
            AddText("Subtitle", canvasObject.transform, new Vector2(-550f, 392f), new Vector2(590f, 34f), "2D SIDE-SCROLLER QUIZ DEFENSE", 22, Hex("69F7FF"), font, TextAnchor.MiddleLeft, FontStyle.Bold);

            CyberGuardianMainMenu menu = new GameObject("Cyber Guardian Main Menu Controller").AddComponent<CyberGuardianMainMenu>();
            menu.gameplaySceneName = "CyberGuardian_Level01";

            AddPanel("Main Menu Buttons Frame", canvasObject.transform, new Vector2(-584f, 104f), new Vector2(866f, 504f), Color.black, panelFrame, 0.64f);
            AddText("Main Menu Section Label", canvasObject.transform, new Vector2(-796f, 334f), new Vector2(350f, 34f), "1  MAIN MENU BUTTONS", 21, Hex("3FEFFF"), font, TextAnchor.MiddleLeft, FontStyle.Bold);
            AddText("Main Menu Default Label", canvasObject.transform, new Vector2(-572f, 286f), new Vector2(280f, 24f), "DEFAULT", 15, Hex("6EF7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            menu.startButton = AddButton("Start Button", canvasObject.transform, new Vector2(-572f, 230f), new Vector2(390f, 62f), "PLAY", 29, font, Hex("08181D"), Color.white, activeButton, out _);
            Button continueButton = AddButton("Continue Button", canvasObject.transform, new Vector2(-572f, 152f), new Vector2(390f, 58f), "CONTINUE", 23, font, Hex("08181D"), Color.white, activeButton, out _);
            Button settingsButton = AddButton("Settings Button", canvasObject.transform, new Vector2(-572f, 76f), new Vector2(390f, 58f), "SETTINGS", 23, font, Hex("08181D"), Color.white, activeButton, out _);
            Button creditsButton = AddButton("Credits Button", canvasObject.transform, new Vector2(-572f, 0f), new Vector2(390f, 58f), "CREDITS", 23, font, Hex("08181D"), Color.white, activeButton, out _);
            menu.quitButton = AddButton("Quit Button", canvasObject.transform, new Vector2(-572f, -76f), new Vector2(390f, 58f), "EXIT", 23, font, Hex("160810"), Color.white, dangerButton, out _);
            continueButton.interactable = false;
            settingsButton.interactable = false;
            creditsButton.interactable = false;

            AddText("Difficulty Header", canvasObject.transform, new Vector2(-572f, -150f), new Vector2(390f, 30f), "DIFFICULTY", 18, Hex("69F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            menu.selectedDifficultyText = AddText("Selected Difficulty", canvasObject.transform, new Vector2(-572f, -182f), new Vector2(420f, 32f), "DIFFICULTY: NORMAL", 17, Hex("D7EFEF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            menu.easyButton = AddButton("Easy Button", canvasObject.transform, new Vector2(-717f, -232f), new Vector2(118f, 42f), "EASY", 15, font, Hex("08181D"), Color.white, activeButton, out _);
            menu.normalButton = AddButton("Normal Button", canvasObject.transform, new Vector2(-572f, -232f), new Vector2(142f, 42f), "NORMAL", 15, font, Hex("08181D"), Color.white, activeButton, out _);
            menu.hardButton = AddButton("Hard Button", canvasObject.transform, new Vector2(-427f, -232f), new Vector2(118f, 42f), "HARD", 15, font, Hex("08181D"), Color.white, activeButton, out _);
            menu.difficultyHighlights = new[] { menu.easyButton.targetGraphic as Image, menu.normalButton.targetGraphic as Image, menu.hardButton.targetGraphic as Image };

            BuildMenuBlockPreview(canvasObject.transform, rockTileSprite, metalCrateSprite, dataMossSprite, sawBladeSprite, horrorSprites, circleSprite, virusSprite, panelSprite, font);

            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        }

        private static void BuildMenuBlockPreview(Transform parent, Sprite rockTileSprite, Sprite metalCrateSprite, Sprite dataMossSprite, Sprite sawBladeSprite, CyberHorrorAssetSprites horrorSprites, Sprite circleSprite, Sprite virusSprite, Sprite panelSprite, Font font)
        {
            Sprite frame = horrorSprites != null && horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : panelSprite;
            Sprite cyanButton = horrorSprites != null && horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : panelSprite;
            Sprite quizBlock = horrorSprites != null && horrorSprites.QuizBlock != null ? horrorSprites.QuizBlock : (horrorSprites != null && horrorSprites.CircuitBlock != null ? horrorSprites.CircuitBlock : metalCrateSprite);

            AddPanel("Mission Preview Frame", parent, new Vector2(340f, 142f), new Vector2(1050f, 438f), Color.black, frame, 0.60f);
            AddText("Mission Preview Label", parent, new Vector2(-112f, 326f), new Vector2(310f, 34f), "2  MISSION PREVIEW", 21, Hex("3FEFFF"), font, TextAnchor.MiddleLeft, FontStyle.Bold);
            AddText("Mission Preview Objective", parent, new Vector2(342f, 318f), new Vector2(780f, 34f), "BREAK QUIZ FIREWALLS  /  DEFEND THE DATA CORE", 20, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);

            AddPanel("Menu HP Cyber Back", parent, new Vector2(-14f, 262f), new Vector2(350f, 42f), Color.black, horrorSprites != null && horrorSprites.UiBarBack != null ? horrorSprites.UiBarBack : panelSprite, 0.94f);
            AddPanel("Menu HP Cyber Fill", parent, new Vector2(-14f, 262f), new Vector2(326f, 30f), Hex("FF2F83"), horrorSprites != null && horrorSprites.UiHpBarFill != null ? horrorSprites.UiHpBarFill : panelSprite, 1f);
            AddText("Menu HP Small Label", parent, new Vector2(-214f, 262f), new Vector2(70f, 36f), "HP", 22, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddPanel("Menu Boost Cyber Back", parent, new Vector2(656f, 262f), new Vector2(330f, 42f), Color.black, horrorSprites != null && horrorSprites.UiBarBack != null ? horrorSprites.UiBarBack : panelSprite, 0.94f);
            AddPanel("Menu Boost Cyber Fill", parent, new Vector2(656f, 262f), new Vector2(306f, 30f), Hex("16E8FF"), horrorSprites != null && horrorSprites.UiBoostBarFill != null ? horrorSprites.UiBoostBarFill : panelSprite, 1f);
            AddText("Menu Boost Small Label", parent, new Vector2(850f, 262f), new Vector2(140f, 36f), "BOOST", 21, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);

            for (int x = -80; x <= 745; x += 72)
            {
                AddImage("Preview Ground Tile " + x, parent, new Vector2(x, 10f), new Vector2(76f, 76f), Color.white, rockTileSprite);
                if ((x / 72) % 2 == 0)
                {
                    AddImage("Preview Data Moss " + x, parent, new Vector2(x, 58f), new Vector2(76f, 26f), Hex("5BFFFF"), dataMossSprite);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                AddImage("Menu Firewall Quiz Block " + i, parent, new Vector2(510f + i * 64f, 118f), new Vector2(64f, 64f), Color.white, quizBlock);
                AddImage("Menu Firewall Glow " + i, parent, new Vector2(510f + i * 64f, 118f), new Vector2(34f, 34f), GetCategoryColor(i), circleSprite);
            }

            AddImage("Menu Guardian Preview", parent, new Vector2(124f, 104f), new Vector2(116f, 128f), Hex("D8FBFF"), circleSprite);
            AddText("Menu Guardian CG", parent, new Vector2(124f, 104f), new Vector2(100f, 80f), "CG", 30, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddImage("Menu Virus Preview", parent, new Vector2(346f, 96f), new Vector2(92f, 92f), Color.white, virusSprite != null ? virusSprite : circleSprite);
            AddImage("Menu Saw Preview", parent, new Vector2(744f, 96f), new Vector2(124f, 124f), Color.white, sawBladeSprite != null ? sawBladeSprite : circleSprite);
            AddButton("Preview Locked Confirm", parent, new Vector2(344f, 204f), new Vector2(210f, 46f), "CONFIRM", 16, font, Hex("071116"), Color.white, cyanButton, out _).interactable = false;

            AddPanel("Tile Pack Frame", parent, new Vector2(218f, -312f), new Vector2(1512f, 250f), Color.black, frame, 0.62f);
            AddText("Tile Pack Label", parent, new Vector2(-490f, -210f), new Vector2(420f, 34f), "3  BLOCKS / QUIZ TILE PACK", 21, Hex("3FEFFF"), font, TextAnchor.MiddleLeft, FontStyle.Bold);

            Sprite[] tileSprites =
            {
                quizBlock,
                horrorSprites != null && horrorSprites.CircuitBlock != null ? horrorSprites.CircuitBlock : metalCrateSprite,
                horrorSprites != null && horrorSprites.DataStone != null ? horrorSprites.DataStone : rockTileSprite,
                horrorSprites != null && horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite,
                horrorSprites != null && horrorSprites.MetaPanel != null ? horrorSprites.MetaPanel : metalCrateSprite,
                horrorSprites != null && horrorSprites.CorruptedBlock != null ? horrorSprites.CorruptedBlock : metalCrateSprite,
                horrorSprites != null && horrorSprites.VirusBlock != null ? horrorSprites.VirusBlock : metalCrateSprite,
                horrorSprites != null && horrorSprites.SpikeBlock != null ? horrorSprites.SpikeBlock : metalCrateSprite
            };

            string[] tileLabels =
            {
                "QUIZ BLOCK",
                "CIRCUIT",
                "DATA STONE",
                "SERVER CORE",
                "META PANEL",
                "CORRUPTED",
                "VIRUS BLOCK",
                "SPIKE BLOCK"
            };

            for (int i = 0; i < tileSprites.Length; i++)
            {
                float x = -445f + i * 184f;
                AddImage("Menu Tile Pack Item " + i, parent, new Vector2(x, -306f), new Vector2(102f, 102f), Color.white, tileSprites[i]);
                AddText("Menu Tile Pack Label " + i, parent, new Vector2(x, -384f), new Vector2(160f, 26f), tileLabels[i], 13, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            }
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
            game.bossArenaCenterX = 84.4f;
            game.bossArenaMinX = 77.6f;
            game.bossArenaMaxX = 89.0f;
            game.slingshotMaxPull = 3.25f;
            game.slingshotPower = 11.6f;
            game.projectileMaxFlightTime = 5.2f;
            game.cameraMin = new Vector2(-8.8f, -3.8f);
            game.cameraMax = new Vector2(91.5f, 5.8f);
            game.bossProjectilePrefab = bossProjectilePrefab;
            game.sfxSource = game.gameObject.AddComponent<AudioSource>();
            game.sfxSource.playOnAwake = false;
            game.sfxSource.volume = 0.78f;
            game.meleeSfx = meleeSfx;
            game.hitSfx = hitSfx;
            game.bossShotSfx = bossShotSfx;
            game.shieldSfx = shieldSfx;
            game.wrongSfx = wrongSfx;

            Sprite generatedGuardianSprite = EnsureImportedSprite(GuardianGeneratedSpritePath);
            Sprite generatedVirusSprite = EnsureImportedSprite(VirusGeneratedSpritePath);
            Sprite generatedBossSprite = EnsureImportedSprite(BossGeneratedSpritePath);
            Sprite generatedProjectileSprite = EnsureImportedSprite(ProjectileGeneratedSpritePath);

            BuildBackground(world.transform, squareSprite, circuitSprite, dataBlobSprite, horrorSprites);
            BuildPlatforms(world.transform, squareSprite, panelSprite, rockTileSprite, metalCrateSprite, dataMossSprite, horrorSprites);
            BuildAdventureActors(world.transform, game, squareSprite, circleSprite, generatedGuardianSprite, virusSprite, generatedVirusSprite);
            BuildHazards(world.transform, game, squareSprite, sawBladeSprite, metalCrateSprite, horrorSprites);
            BuildBossArena(world.transform, game, squareSprite, circleSprite, metalCrateSprite, horrorSprites, virusSprite, virusAltSprite, generatedBossSprite, projectileSprite, generatedProjectileSprite, sparkSprite, crosshairSprite, font);
            BuildHud(game, panelSprite, buttonSprite != null ? buttonSprite : panelSprite, frameSprite, horrorSprites, font);

            EditorSceneManager.SaveScene(scene, LevelScenePath);
        }

        private static void BuildBackground(Transform parent, Sprite squareSprite, Sprite circuitSprite, Sprite dataBlobSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateWorldSprite("Back Wall", parent, new Vector2(40f, 1.2f), new Vector2(120f, 17.5f), Hex("02080B"), squareSprite, 0);
            CreateWorldSprite("Data Forest Backdrop", parent, new Vector2(0f, 2.3f), new Vector2(27f, 12f), new Color(0.42f, 0.82f, 0.84f, 0.46f), horrorSprites != null ? horrorSprites.DataForestBackground : dataBlobSprite, 1);
            CreateWorldSprite("Server Runs Backdrop", parent, new Vector2(31f, 2.3f), new Vector2(32f, 12f), new Color(0.36f, 0.72f, 0.9f, 0.42f), horrorSprites != null ? horrorSprites.ServerRunsBackground : dataBlobSprite, 1);
            CreateWorldSprite("Code Abyss Backdrop", parent, new Vector2(62f, 2.2f), new Vector2(31f, 12f), new Color(0.34f, 0.72f, 0.72f, 0.40f), horrorSprites != null ? horrorSprites.CodeAbyssBackground : dataBlobSprite, 1);
            CreateWorldSprite("Firewall Core Backdrop", parent, new Vector2(88f, 2.25f), new Vector2(31f, 12f), new Color(0.72f, 0.18f, 0.54f, 0.38f), horrorSprites != null ? horrorSprites.ServerRunsBackground : dataBlobSprite, 1);
            CreateWorldSprite("Aqua Data Sky", parent, new Vector2(12f, 2.8f), new Vector2(24f, 10.5f), new Color(0.05f, 0.78f, 0.88f, 0.12f), dataBlobSprite, 2);
            CreateWorldSprite("Magenta Memory Canopy", parent, new Vector2(37f, 3.2f), new Vector2(18f, 6.2f), new Color(0.95f, 0.05f, 0.54f, 0.10f), dataBlobSprite, 2);
            CreateWorldSprite("Violet Boss Data Cloud", parent, new Vector2(84f, 3.0f), new Vector2(22f, 7.4f), new Color(0.75f, 0.12f, 0.92f, 0.10f), dataBlobSprite, 2);
            if (circuitSprite != null)
            {
                for (int i = 0; i < 17; i++)
                {
                    CreateWorldSprite("Circuit Panel " + i, parent, new Vector2(-11f + i * 7.5f, 3.35f + (i % 2) * 0.8f), new Vector2(7.5f, 2.8f), new Color(0.13f, 0.9f, 0.96f, 0.06f), circuitSprite, 3);
                }
            }

            for (int i = 0; i < 21; i++)
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

            CreateHorrorPlatform("Lower Trap Route A", parent, new Vector2(12.3f, -3.65f), 7, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Trap Route B", parent, new Vector2(20.6f, -4.25f), 6, 1, horrorSprites.CorruptedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Recovery Route C", parent, new Vector2(29.0f, -3.52f), 7, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Trap Route D", parent, new Vector2(38.2f, -4.05f), 8, 1, horrorSprites.CrackedBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Exit Lift Base", parent, new Vector2(47.2f, -2.20f), 5, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));
            CreateHorrorPlatform("Lower Code Abyss Choice", parent, new Vector2(58.2f, -3.65f), 6, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
            CreateHorrorPlatform("Lower Upload Exit", parent, new Vector2(73.4f, -1.8f), 5, 1, horrorSprites.ServerCore, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("7BFFFF"));

            CreateHorrorPlatform("Boss Arena Floor", parent, new Vector2(86.0f, -1.02f), 24, 3, horrorSprites.MetaPanel, horrorSprites.NeonPlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Boss Dodge Platform A", parent, new Vector2(80.4f, 1.45f), 4, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Boss Dodge Platform B", parent, new Vector2(85.9f, 2.45f), 4, 1, horrorSprites.ServerCore, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Boss Dodge Platform C", parent, new Vector2(89.5f, 0.85f), 3, 1, horrorSprites.NeonPlatform, horrorSprites.GlowEdgePlatform, dataMossSprite, Color.white, Hex("69F7FF"));
            CreateHorrorPlatform("Boss Ceiling Firewall", parent, new Vector2(86.8f, 4.8f), 16, 1, horrorSprites.VirusBlock, horrorSprites.CorruptedPlatform, dataMossSprite, Color.white, Hex("FF3B88"));
        }

        private static void BuildAdventureActors(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite circleSprite, Sprite guardianGeneratedSprite, Sprite virusSprite, Sprite virusGeneratedSprite)
        {
            GameObject playerObject = new GameObject("Cyber Guardian Player", typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(CyberGuardianPlayerController));
            playerObject.transform.SetParent(parent, false);
            playerObject.transform.position = new Vector3(-8f, 0.4f, 0f);
            Transform playerVisualRoot = new GameObject("Cyber Guardian Visual Root").transform;
            playerVisualRoot.SetParent(playerObject.transform, false);
            playerVisualRoot.localPosition = Vector3.zero;
            CreateLocalSprite(
                "Cyber Guardian Visual",
                playerVisualRoot,
                Vector3.zero,
                guardianGeneratedSprite != null ? new Vector2(1.05f, 1.35f) : new Vector2(0.9f, 1.28f),
                guardianGeneratedSprite != null ? Color.white : Hex("2EBBEA"),
                guardianGeneratedSprite != null ? guardianGeneratedSprite : circleSprite,
                20);
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
            game.player = player;
            AttachGeneratedGlbVisual(GuardianGlbPath, playerVisualRoot, "Generated Guardian GLB Visual", new Vector3(0f, -0.58f, -0.24f), Vector3.one * 0.42f, new Vector3(0f, 180f, 0f));
            AddGuardianArmorOverlay(playerVisualRoot, squareSprite, circleSprite);

            GameObject meleeFlash = CreateWorldSprite("Melee Slash Flash", parent, new Vector2(-50f, -50f), new Vector2(1.15f, 0.36f), new Color(0.7f, 1f, 1f, 0.8f), circleSprite, 28).gameObject;
            meleeFlash.SetActive(false);
            game.meleeFlash = meleeFlash;

            Sprite enemySprite = virusGeneratedSprite != null ? virusGeneratedSprite : virusSprite;
            CreateEnemy("Virus Soldier A", parent, game, new Vector2(3.2f, 0.08f), enemySprite, squareSprite, circleSprite, 1.7f, 1.8f);
            CreateEnemy("Virus Soldier B", parent, game, new Vector2(14.3f, 0.2f), enemySprite, squareSprite, circleSprite, 1.9f, 1.5f);
            CreateEnemy("Virus Soldier C", parent, game, new Vector2(22.0f, 0.78f), enemySprite, squareSprite, circleSprite, 2.0f, 1.8f);
            CreateEnemy("Virus Soldier D", parent, game, new Vector2(26.0f, 4.0f), enemySprite, squareSprite, circleSprite, 2.2f, 1.4f);
            CreateEnemy("Virus Soldier E", parent, game, new Vector2(38.5f, 0.78f), enemySprite, squareSprite, circleSprite, 2.1f, 1.9f);
            CreateEnemy("Virus Soldier F", parent, game, new Vector2(46.0f, -1.55f), enemySprite, squareSprite, circleSprite, 2.3f, 1.5f);
            CreateEnemy("Virus Soldier G", parent, game, new Vector2(55.0f, 2.1f), enemySprite, squareSprite, circleSprite, 2.4f, 1.7f);
            CreateEnemy("Virus Soldier H", parent, game, new Vector2(62.2f, -0.48f), enemySprite, squareSprite, circleSprite, 2.3f, 1.4f);
            CreateEnemy("Virus Soldier I", parent, game, new Vector2(68.3f, 2.75f), enemySprite, squareSprite, circleSprite, 2.5f, 1.2f);
            CreateEnemy("Virus Soldier J", parent, game, new Vector2(73.6f, -1.1f), enemySprite, squareSprite, circleSprite, 2.4f, 1.5f);
        }

        private static void BuildHazards(Transform parent, CyberGuardianSideScrollerGame game, Sprite squareSprite, Sprite sawBladeSprite, Sprite metalCrateSprite, CyberHorrorAssetSprites horrorSprites)
        {
            CreateCheckpoint("Start Recovery Node", parent, game, new Vector2(-6.7f, 0.85f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Fork Recovery Node", parent, game, new Vector2(7.0f, 0.25f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Mid Route Recovery Node", parent, game, new Vector2(31.0f, 0.55f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Boss Approach Recovery Node", parent, game, new Vector2(56.3f, 1.92f), squareSprite, horrorSprites.ElectricNode);
            CreateCheckpoint("Upload Tower Recovery Node", parent, game, new Vector2(73.8f, 1.25f), squareSprite, horrorSprites.ElectricNode);
            CreateRecoveryZone("Global Code Abyss Recovery Field", parent, game, new Vector2(42f, -8.2f), new Vector2(118f, 1.4f));

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
            CreateSawTrap("Boss Gate Saw", parent, game, new Vector2(76.1f, 0.72f), 0.95f, 22, sawBladeSprite, squareSprite);

            CreateMovingPlatform("Corrupted Moving Platform Lower", parent, new Vector2(42.8f, -2.15f), new Vector3(0f, 1.2f, 0f), horrorSprites.CorruptedPlatform, squareSprite);
            CreateMovingPlatform("Upload Tower Moving Platform", parent, new Vector2(64.6f, -2.45f), new Vector3(2.2f, 1.45f, 0f), horrorSprites.CorruptedPlatform, squareSprite);
            CreateWorldSprite("Suspended Server Core A", parent, new Vector2(24.6f, 4.4f), new Vector2(1.42f, 0.78f), Color.white, horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite, 13);
            CreateWorldSprite("Suspended Server Core B", parent, new Vector2(50.2f, 2.6f), new Vector2(1.42f, 0.78f), Color.white, horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite, 13);
            CreateWorldSprite("Suspended Server Core C", parent, new Vector2(70.4f, 3.9f), new Vector2(1.42f, 0.78f), Color.white, horrorSprites.ServerCore != null ? horrorSprites.ServerCore : metalCrateSprite, 13);
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
            AddPanel("Top HUD Shade", canvasObject.transform, new Vector2(0f, 505f), new Vector2(1920f, 86f), Color.black, horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : panelSprite, 0.68f);

            AddImage("HP Icon Frame", canvasObject.transform, new Vector2(-936f, 500f), new Vector2(74f, 74f), Color.white, horrorSprites.UiAlertPanel != null ? horrorSprites.UiAlertPanel : panelSprite);
            AddText("HP Icon", canvasObject.transform, new Vector2(-936f, 500f), new Vector2(64f, 42f), "HP", 23, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.playerHealthFill = AddCyberBar(canvasObject.transform, new Vector2(-685f, 500f), new Vector2(420f, 46f), Hex("FF2F83"), horrorSprites.UiBarBack != null ? horrorSprites.UiBarBack : panelSprite, horrorSprites.UiHpBarFill != null ? horrorSprites.UiHpBarFill : panelSprite);

            AddImage("Score Cyber Card", canvasObject.transform, new Vector2(0f, 500f), new Vector2(370f, 76f), Color.white, horrorSprites.UiScorePanel != null ? horrorSprites.UiScorePanel : panelSprite);
            AddText("Score Label", canvasObject.transform, new Vector2(-112f, 516f), new Vector2(110f, 24f), "SCORE", 17, Hex("FF5B9B"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.scoreText = AddText("Score Text", canvasObject.transform, new Vector2(38f, 492f), new Vector2(250f, 54f), "0", 45, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);

            GameObject bossHud = new GameObject("Boss HUD Group", typeof(RectTransform));
            bossHud.transform.SetParent(canvasObject.transform, false);
            game.bossHudGroup = bossHud;
            AddImage("Boss Core Icon", bossHud.transform, new Vector2(-386f, 418f), new Vector2(86f, 86f), Color.white, horrorSprites.UiAlertPanel != null ? horrorSprites.UiAlertPanel : panelSprite);
            game.bossHealthFill = AddCyberBar(bossHud.transform, new Vector2(-14f, 418f), new Vector2(640f, 42f), Hex("FF2F83"), horrorSprites.UiBarBack != null ? horrorSprites.UiBarBack : panelSprite, horrorSprites.UiBossBarFill != null ? horrorSprites.UiBossBarFill : panelSprite);
            game.bossText = AddText("Boss Text", bossHud.transform, new Vector2(-14f, 462f), new Vector2(500f, 30f), "BOSS HP", 25, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            bossHud.SetActive(false);

            AddText("Boost Label", canvasObject.transform, new Vector2(765f, 534f), new Vector2(250f, 30f), "BOOST ENERGY", 22, Hex("61F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.boostEnergyFill = AddCyberBar(canvasObject.transform, new Vector2(760f, 500f), new Vector2(330f, 46f), Hex("16E8FF"), horrorSprites.UiBarBack != null ? horrorSprites.UiBarBack : panelSprite, horrorSprites.UiBoostBarFill != null ? horrorSprites.UiBoostBarFill : panelSprite);
            game.modeText = AddText("Mode Text", canvasObject.transform, new Vector2(915f, 500f), new Vector2(170f, 44f), "BOOST", 25, Color.white, font, TextAnchor.MiddleRight, FontStyle.Bold);
            AddPanel("Shift Key Back", canvasObject.transform, new Vector2(650f, 447f), new Vector2(88f, 48f), Color.black, horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : buttonSprite, 0.95f);
            AddPanel("K Key Back", canvasObject.transform, new Vector2(760f, 447f), new Vector2(54f, 48f), Color.black, horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : buttonSprite, 0.95f);
            AddText("Shift Hint", canvasObject.transform, new Vector2(650f, 447f), new Vector2(76f, 36f), "SHIFT", 15, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("K Hint", canvasObject.transform, new Vector2(760f, 447f), new Vector2(42f, 36f), "K", 23, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);

            game.menuButton = AddButton("Menu Button", canvasObject.transform, new Vector2(438f, 447f), new Vector2(96f, 42f), "MENU", 12, font, Hex("08181D"), Color.white, horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : buttonSprite, out _);
            game.resetButton = AddButton("Reset Button", canvasObject.transform, new Vector2(542f, 447f), new Vector2(96f, 42f), "RETRY", 12, font, Hex("160810"), Color.white, horrorSprites.UiButtonMagenta != null ? horrorSprites.UiButtonMagenta : buttonSprite, out _);
            AddImage("Status Warning Panel", canvasObject.transform, new Vector2(0f, 368f), new Vector2(620f, 58f), Color.white, horrorSprites.UiAlertPanel != null ? horrorSprites.UiAlertPanel : panelSprite);
            AddText("Status Alert Icon", canvasObject.transform, new Vector2(-282f, 368f), new Vector2(46f, 42f), "!", 26, Hex("FF4B88"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.statusText = AddText("Status Text", canvasObject.transform, new Vector2(28f, 368f), new Vector2(535f, 34f), "SYSTEM HOT", 20, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            Sprite modalPanel = horrorSprites.UiPanelFrame != null ? horrorSprites.UiPanelFrame : panelSprite;
            Sprite modalButton = horrorSprites.UiButtonCyan != null ? horrorSprites.UiButtonCyan : buttonSprite;
            BuildQuizModal(canvasObject.transform, game, modalPanel, modalButton, frameSprite, font);
            BuildGameOverModal(canvasObject.transform, game, modalPanel, modalButton, frameSprite, font);
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
            game.quizTitleText = AddText("Quiz Title", modal.transform, new Vector2(0f, 158f), new Vector2(590f, 44f), "QUIZ BLOCK", 22, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
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
                game.answerButtons[i] = AddButton("Answer " + i, modal.transform, positions[i], new Vector2(310f, 58f), "ANSWER", 14, font, Hex("2E6E75"), Color.white, buttonSprite, out Text label);
                game.answerLabels[i] = label;
            }

            game.closeQuizButton = AddButton("Close Quiz", modal.transform, new Vector2(315f, 178f), new Vector2(42f, 42f), "X", 18, font, Hex("A83C48"), Color.white, buttonSprite, out _);
            game.quizModal = modal;
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
            AddText("Game Over Title", modal.transform, new Vector2(0f, 102f), new Vector2(620f, 84f), "GAME OVER", 58, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Game Over Subtitle", modal.transform, new Vector2(0f, 24f), new Vector2(620f, 40f), "CYBER GUARDIAN DESTROYED", 22, Hex("FF6671"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.gameOverScoreText = AddText("Game Over Score", modal.transform, new Vector2(0f, -30f), new Vector2(620f, 46f), "SCORE 0", 26, Hex("69F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            game.gameOverRetryButton = AddButton("Game Over Retry Button", modal.transform, new Vector2(-118f, -116f), new Vector2(210f, 58f), "RETRY", 20, font, Hex("00AFC2"), Color.white, buttonSprite, out _);
            game.gameOverMenuButton = AddButton("Game Over Menu Button", modal.transform, new Vector2(118f, -116f), new Vector2(210f, 58f), "MENU", 20, font, Hex("263039"), Color.white, buttonSprite, out _);
            game.gameOverModal = modal;
            modal.SetActive(false);
        }

        private static void AddGuardianArmorOverlay(Transform parent, Sprite squareSprite, Sprite circleSprite)
        {
            CreateLocalSprite("Guardian Shield Aura", parent, new Vector3(0f, 0.18f, 0.08f), new Vector2(1.34f, 1.62f), new Color(0.0f, 0.95f, 1f, 0.22f), circleSprite, 19);
            CreateLocalSprite("Guardian Dark Cape", parent, new Vector3(-0.25f, -0.02f, 0.07f), new Vector2(0.42f, 1.18f), new Color(0.01f, 0.05f, 0.07f, 0.82f), squareSprite, 19);
            CreateLocalSprite("Guardian Helmet Visor", parent, new Vector3(0.09f, 0.39f, -0.05f), new Vector2(0.34f, 0.08f), Hex("7CFFFF"), squareSprite, 24);
            CreateLocalSprite("Guardian Security Core", parent, new Vector3(0.05f, 0.03f, -0.06f), new Vector2(0.24f, 0.24f), Hex("00F0FF"), circleSprite, 24);
            CreateLocalSprite("Guardian Data Blade", parent, new Vector3(0.66f, -0.12f, -0.07f), new Vector2(0.74f, 0.12f), Hex("8CFFFF"), squareSprite, 24);
            CreateLocalSprite("Guardian Shoulder Cyan", parent, new Vector3(-0.35f, 0.24f, -0.06f), new Vector2(0.24f, 0.18f), Hex("0CEAFF"), squareSprite, 24);
            CreateLocalSprite("Guardian Shoulder White", parent, new Vector3(0.34f, 0.24f, -0.06f), new Vector2(0.24f, 0.18f), Hex("DDFBFF"), squareSprite, 24);
        }

        private static void AddVirusOverlay(Transform parent, Sprite squareSprite, Sprite circleSprite)
        {
            CreateLocalSprite("Virus Magenta Core Glow", parent, new Vector3(0f, 0.08f, -0.06f), new Vector2(0.62f, 0.62f), new Color(1f, 0.05f, 0.48f, 0.58f), circleSprite, 21);
            CreateLocalSprite("Virus Red Eye", parent, new Vector3(0.12f, 0.18f, -0.08f), new Vector2(0.18f, 0.18f), Hex("FF2F83"), circleSprite, 23);
            CreateLocalSprite("Virus Eye Hotspot", parent, new Vector3(0.14f, 0.19f, -0.09f), new Vector2(0.06f, 0.06f), Hex("FFFFFF"), circleSprite, 24);
            CreateLocalSprite("Virus Claw Left", parent, new Vector3(-0.42f, -0.08f, -0.06f), new Vector2(0.28f, 0.08f), Hex("FF2F83"), squareSprite, 22);
            CreateLocalSprite("Virus Claw Right", parent, new Vector3(0.46f, -0.02f, -0.06f), new Vector2(0.30f, 0.08f), Hex("FF2F83"), squareSprite, 22);
            CreateLocalSprite("Virus Glitch Trail", parent, new Vector3(-0.26f, -0.42f, 0.05f), new Vector2(0.18f, 0.18f), new Color(1f, 0.05f, 0.48f, 0.45f), squareSprite, 20);
        }

        private static CyberGuardianEnemy CreateEnemy(string name, Transform parent, CyberGuardianSideScrollerGame game, Vector2 position, Sprite virusSprite, Sprite squareSprite, Sprite circleSprite, float speed, float patrol)
        {
            GameObject enemyObject = new GameObject(name, typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CyberGuardianEnemy));
            enemyObject.transform.SetParent(parent, false);
            enemyObject.transform.position = position;
            Transform visualRoot = new GameObject("Virus Visual Root").transform;
            visualRoot.SetParent(enemyObject.transform, false);
            visualRoot.localPosition = Vector3.zero;
            CreateLocalSprite("Virus Soldier Visual", visualRoot, Vector3.zero, new Vector2(0.92f, 0.92f), Color.white, virusSprite, 18);
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
            AttachGeneratedGlbVisual(VirusGlbPath, visualRoot, "Generated Virus GLB Visual", new Vector3(0f, -0.38f, -0.22f), Vector3.one * 0.42f, new Vector3(0f, 180f, 0f));
            AddVirusOverlay(visualRoot, squareSprite, circleSprite);
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
            collider.isTrigger = true;
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
            CircleCollider2D collider = saw.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = diameter * 0.46f;
            CyberGuardianDamageZone zone = saw.AddComponent<CyberGuardianDamageZone>();
            zone.game = game;
            zone.damage = damage;
            CreateWorldSprite(name + " Arm", parent, position + new Vector2(0f, diameter * 0.65f), new Vector2(0.16f, diameter * 1.2f), Hex("263039"), fallbackSprite, 12);
            CreateWorldSprite(name + " Core Glow", parent, position, new Vector2(diameter * 0.28f, diameter * 0.28f), new Color(1f, 0.82f, 0.18f, 0.8f), fallbackSprite, 23);
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
            CreateWorldSprite(name + " Glow", parent, position, new Vector2(1.2f, 1.2f), new Color(0.35f, 1f, 1f, 0.24f), fallbackSprite, 16);
            GameObject node = CreateWorldSprite(name, parent, position, new Vector2(0.72f, 0.92f), Color.white, nodeSprite != null ? nodeSprite : fallbackSprite, 24).gameObject;
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

        private static void EnsureEventSystem()
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static Button AddButton(string name, Transform parent, Vector2 position, Vector2 size, string text, int fontSize, Font font, Color background, Color textColor, Sprite sprite, out Text label)
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

            string icon = GetButtonIcon(text);
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
                case "PLAY":
                case "RESUME":
                case "START LEVEL 01":
                    return ">";
                case "CONTINUE":
                    return ">>";
                case "SETTINGS":
                    return "O";
                case "CREDITS":
                    return "@";
                case "CONFIRM":
                    return "V";
                case "EXIT":
                case "QUIT":
                case "CANCEL":
                    return "X";
                case "MAIN":
                case "MENU":
                    return "H";
                case "RETRY":
                case "RST":
                    return "R";
                case "EASY":
                case "NORMAL":
                case "HARD":
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
            textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            return textComponent;
        }

        private static void SetBuildScenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(LevelScenePath, true)
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
                    new QuizQuestion(CyberQuestionCategory.Password, "PASSWORD GATE", "Password yang baik sebaiknya...", new[] { "Panjang, unik, dan sulit ditebak", "Sama untuk semua akun", "Berisi tanggal lahir", "Dibagikan ke teman" }, 0, "Benar. Password perlu panjang, unik, dan tidak dipakai ulang."),
                    new QuizQuestion(CyberQuestionCategory.Password, "LOGIN SHIELD", "Autentikasi dua faktor berguna untuk...", new[] { "Menambah lapisan verifikasi", "Menghapus password", "Membuka semua akun", "Melemahkan akun" }, 0, "Benar. 2FA menambah bukti verifikasi selain password."),
                    new QuizQuestion(CyberQuestionCategory.Malware, "MALWARE BLOCK", "Lampiran asing dari email tidak dikenal sebaiknya...", new[] { "Tidak dibuka sembarangan", "Langsung dijalankan", "Dibagikan ulang", "Diubah namanya saja" }, 0, "Benar. Lampiran asing bisa membawa malware."),
                    new QuizQuestion(CyberQuestionCategory.Malware, "PHISHING FILTER", "Tanda umum phishing adalah...", new[] { "Alamat pengirim mencurigakan", "Bahasa selalu sempurna", "Tidak pernah ada link", "Selalu dari teman" }, 0, "Benar. Pengirim dan domain harus diperiksa."),
                    new QuizQuestion(CyberQuestionCategory.Network, "NETWORK WALL", "Firewall membantu kita untuk...", new[] { "Menyaring koneksi berbahaya", "Membuka semua port", "Membuat virus", "Mematikan update" }, 0, "Benar. Firewall membantu mengontrol koneksi masuk dan keluar."),
                    new QuizQuestion(CyberQuestionCategory.Network, "PATCH ROUTE", "Mengapa update sistem penting?", new[] { "Menutup celah keamanan", "Melepas proteksi", "Membagikan data", "Mematikan enkripsi" }, 0, "Benar. Update sering membawa patch keamanan."),
                    new QuizQuestion(CyberQuestionCategory.Privacy, "PRIVACY LOCK", "Data yang tidak boleh dibagikan sembarangan adalah...", new[] { "OTP, password, NIK", "Genre game favorit", "Warna kesukaan", "Nama panggilan" }, 0, "Benar. Data sensitif dapat dipakai untuk penipuan."),
                    new QuizQuestion(CyberQuestionCategory.Privacy, "DATA MINIMIZE", "Prinsip minimisasi data berarti...", new[] { "Hanya memakai data yang perlu", "Mengumpulkan semua data", "Menyimpan tanpa batas", "Membagikan cadangan" }, 0, "Benar. Data harus sesuai kebutuhan.")
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
                EnsureDifficultyProfile(EasyDifficultyPath, "Easy", 110f, 100, 10, 5, 8, 8, 8, 12, 25),
                EnsureDifficultyProfile(NormalDifficultyPath, "Normal", 87f, 100, 25, 7, 6, 12, 12, 250, 30),
                EnsureDifficultyProfile(HardDifficultyPath, "Hard", 70f, 85, 35, 10, 4, 18, 18, 320, 22)
            };
        }

        private static DifficultyProfile EnsureDifficultyProfile(string path, string displayName, float time, int shield, int virus, int routeOrbs, int shieldReward, int wrongDamage, int wrongVirusGain, int routeScore, int routeDamage)
        {
            DifficultyProfile profile = AssetDatabase.LoadAssetAtPath<DifficultyProfile>(path);
            if (profile != null)
            {
                profile.displayName = displayName;
                profile.startingTime = time;
                profile.startingScore = 100;
                profile.startingShield = Mathf.Clamp(shield, 1, 100);
                profile.startingVirusStrength = virus;
                profile.requiredRouteOrbs = routeOrbs;
                profile.correctScoreReward = 8;
                profile.correctShieldReward = shieldReward;
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
            profile.startingScore = 100;
            profile.startingTokens = 15;
            profile.startingShield = Mathf.Clamp(shield, 1, 100);
            profile.startingVirusStrength = virus;
            profile.requiredRouteOrbs = routeOrbs;
            profile.correctScoreReward = 8;
            profile.correctTokenReward = 1;
            profile.correctShieldReward = shieldReward;
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
                Texture2D texture = GenerateTexture(shape, 128);
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
                importer.filterMode = FilterMode.Point;
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
