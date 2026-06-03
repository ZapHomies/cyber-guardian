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
        private const string VirusSpritePath = "Assets/CyberGuardian/Art/Enemies/VirusBigPack/png1.png";
        private const string VirusAltSpritePath = "Assets/CyberGuardian/Art/Enemies/VirusBigPack/png18.png";
        private const string ProjectileSpritePath = "Assets/CyberGuardian/Art/VFX/KenneyParticles/PNG (Transparent)/muzzle_03.png";
        private const string TrajectoryDotSpritePath = "Assets/CyberGuardian/Art/VFX/KenneyParticles/PNG (Transparent)/spark_02.png";
        private const string OrbShellSpritePath = "Assets/CyberGuardian/Art/VFX/KenneyParticles/PNG (Transparent)/circle_05.png";
        private const string CrosshairSpritePath = "Assets/CyberGuardian/Art/UI/KenneySpaceUI/PNG/Blue/Default/crosshair_color_a.png";
        private const string PanelFrameSpritePath = "Assets/CyberGuardian/Art/UI/CyberpunkPixelUI/1 Frames/Frame_05.png";
        private const string ButtonSpritePath = "Assets/CyberGuardian/Art/UI/KenneySpaceUI/PNG/Blue/Default/button_square_header_large_rectangle.png";
        private const string HealthBarSpritePath = "Assets/CyberGuardian/Art/UI/CyberpunkPixelUI/2 Bars/HealthBar3.png";
        private const string EnergyBarSpritePath = "Assets/CyberGuardian/Art/UI/CyberpunkPixelUI/2 Bars/EnergyBar4.png";
        private const string CircuitSpritePath = "Assets/SourceFiles/Textures/Repeating Tiles/Circuit_Albedo.png";
        private const string LaunchSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/laserSmall_000.ogg";
        private const string OrbHitSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/impactMetal_000.ogg";
        private const string VirusHitSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/explosionCrunch_000.ogg";
        private const string ShieldSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/forceField_000.ogg";
        private const string WrongSfxPath = "Assets/CyberGuardian/Audio/SFX/KenneySciFi/Audio/slime_000.ogg";

        [MenuItem("Cyber Guardian/Build Main Menu And Level 01")]
        public static void BuildGameScenes()
        {
            Directory.CreateDirectory(ToAbsolutePath("Assets/CyberGuardian/Scenes"));
            Directory.CreateDirectory(ToAbsolutePath(GeneratedArtFolder));

            Sprite panelSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_rounded_panel.png", TextureShape.RoundedRect);
            Sprite circleSprite = EnsureGeneratedSprite(GeneratedArtFolder + "/cg_circle.png", TextureShape.Circle);
            Sprite virusSprite = EnsureImportedSprite(VirusSpritePath);
            Sprite virusAltSprite = EnsureImportedSprite(VirusAltSpritePath);
            Sprite projectileSprite = EnsureImportedSprite(ProjectileSpritePath);
            Sprite trajectoryDotSprite = EnsureImportedSprite(TrajectoryDotSpritePath);
            Sprite orbShellSprite = EnsureImportedSprite(OrbShellSpritePath);
            Sprite crosshairSprite = EnsureImportedSprite(CrosshairSpritePath);
            Sprite panelFrameSprite = EnsureImportedSprite(PanelFrameSpritePath, new Vector4(28f, 28f, 28f, 28f));
            Sprite buttonSprite = EnsureImportedSprite(ButtonSpritePath, new Vector4(18f, 18f, 18f, 18f));
            Sprite healthBarSprite = EnsureImportedSprite(HealthBarSpritePath);
            Sprite energyBarSprite = EnsureImportedSprite(EnergyBarSpritePath);
            Sprite circuitSprite = EnsureImportedSprite(CircuitSpritePath);
            AudioClip launchSfx = EnsureImportedAudioClip(LaunchSfxPath);
            AudioClip orbHitSfx = EnsureImportedAudioClip(OrbHitSfxPath);
            AudioClip virusHitSfx = EnsureImportedAudioClip(VirusHitSfxPath);
            AudioClip shieldSfx = EnsureImportedAudioClip(ShieldSfxPath);
            AudioClip wrongSfx = EnsureImportedAudioClip(WrongSfxPath);
            QuizQuestionBank questionBank = EnsureQuestionBank();
            DifficultyProfile[] difficulties = EnsureDifficultyProfiles();
            Font font = GetUiFont();

            BuildMainMenuScene(panelSprite, circleSprite, virusSprite, panelFrameSprite, buttonSprite, circuitSprite, font);
            BuildLevelScene(panelSprite, circleSprite, virusSprite, virusAltSprite, projectileSprite, trajectoryDotSprite, orbShellSprite, crosshairSprite, panelFrameSprite, buttonSprite, healthBarSprite, energyBarSprite, circuitSprite, questionBank, difficulties, font, launchSfx, orbHitSfx, virusHitSfx, shieldSfx, wrongSfx);
            SetBuildScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Cyber Guardian main menu and Level 01 built.");
        }

        private static void BuildMainMenuScene(Sprite panelSprite, Sprite circleSprite, Sprite virusSprite, Sprite panelFrameSprite, Sprite buttonSprite, Sprite circuitSprite, Font font)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CyberGuardian_MainMenu";

            Camera camera = CreateCamera("07171D");
            camera.orthographicSize = 5f;
            EnsureEventSystem();

            GameObject canvasObject = CreateCanvas("Cyber Guardian Main Menu");
            AddStretchImage("Background", canvasObject.transform, Hex("07171D"), panelSprite);
            if (circuitSprite != null)
            {
                Image circuitOverlay = AddStretchImage("Circuit Board Texture Overlay", canvasObject.transform, new Color(0.25f, 0.9f, 0.95f, 0.12f), circuitSprite);
                circuitOverlay.type = Image.Type.Tiled;
                circuitOverlay.pixelsPerUnitMultiplier = 0.42f;
                circuitOverlay.raycastTarget = false;
            }

            AddStretchImage("Top Circuit Band", canvasObject.transform, new Color(0.09f, 0.40f, 0.46f, 0.62f), panelSprite, new Vector2(0f, 0.82f), Vector2.one, Vector2.zero, Vector2.zero);
            AddStretchImage("Bottom Circuit Band", canvasObject.transform, new Color(0.12f, 0.34f, 0.39f, 0.70f), panelSprite, Vector2.zero, new Vector2(1f, 0.18f), Vector2.zero, Vector2.zero);

            AddPanel("Menu Frame", canvasObject.transform, new Vector2(-330f, -15f), new Vector2(510f, 560f), Hex("102E35"), panelSprite, 0.92f);
            AddPanel("Menu Frame Inner", canvasObject.transform, new Vector2(-330f, -15f), new Vector2(460f, 510f), Hex("173E47"), panelSprite, 0.70f);
            if (panelFrameSprite != null)
            {
                Image menuFrameAsset = AddImage("Cyberpunk Menu Frame Asset", canvasObject.transform, new Vector2(-330f, -15f), new Vector2(520f, 570f), Color.white, panelFrameSprite);
                menuFrameAsset.type = Image.Type.Sliced;
                menuFrameAsset.raycastTarget = false;
            }

            AddText("Title", canvasObject.transform, new Vector2(-330f, 240f), new Vector2(460f, 76f), "CYBER GUARDIAN", 42, Hex("E6FBFF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Subtitle", canvasObject.transform, new Vector2(-330f, 188f), new Vector2(420f, 42f), "Quiz Tactical Slingshot", 22, Hex("7EE6F1"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            CyberGuardianMainMenu menu = new GameObject("Cyber Guardian Main Menu Controller").AddComponent<CyberGuardianMainMenu>();
            menu.gameplaySceneName = "CyberGuardian_Level01";

            menu.selectedDifficultyText = AddText("Selected Difficulty", canvasObject.transform, new Vector2(-330f, 135f), new Vector2(350f, 34f), "DIFFICULTY: NORMAL", 18, Hex("D7EFEF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            Sprite activeButtonSprite = buttonSprite != null ? buttonSprite : panelSprite;
            menu.startButton = AddButton("Start Button", canvasObject.transform, new Vector2(-330f, 82f), new Vector2(320f, 56f), "START LEVEL 01", 22, font, Hex("55D264"), Color.white, activeButtonSprite, out _);

            menu.easyButton = AddButton("Easy Button", canvasObject.transform, new Vector2(-447f, 22f), new Vector2(96f, 42f), "EASY", 15, font, Hex("2D626B"), Color.white, activeButtonSprite, out _);
            menu.normalButton = AddButton("Normal Button", canvasObject.transform, new Vector2(-330f, 22f), new Vector2(110f, 42f), "NORMAL", 15, font, Hex("2D626B"), Color.white, activeButtonSprite, out _);
            menu.hardButton = AddButton("Hard Button", canvasObject.transform, new Vector2(-209f, 22f), new Vector2(96f, 42f), "HARD", 15, font, Hex("2D626B"), Color.white, activeButtonSprite, out _);
            menu.difficultyHighlights = new[]
            {
                menu.easyButton.targetGraphic as Image,
                menu.normalButton.targetGraphic as Image,
                menu.hardButton.targetGraphic as Image
            };

            menu.quitButton = AddButton("Quit Button", canvasObject.transform, new Vector2(-330f, -48f), new Vector2(320f, 48f), "QUIT", 16, font, Hex("A83C48"), Color.white, activeButtonSprite, out _);

            AddPanel("Right Preview Bay", canvasObject.transform, new Vector2(310f, -20f), new Vector2(600f, 520f), Hex("0D252C"), panelSprite, 0.86f);
            AddPanel("Right Preview Inner", canvasObject.transform, new Vector2(310f, -20f), new Vector2(548f, 468f), Hex("143640"), panelSprite, 0.58f);
            AddText("Preview Label", canvasObject.transform, new Vector2(310f, 208f), new Vector2(470f, 34f), "MISSION PREVIEW", 20, Hex("7EE6F1"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            AddImage("Guardian Menu Glow", canvasObject.transform, new Vector2(85f, -60f), new Vector2(155f, 155f), new Color(0.1f, 0.75f, 1f, 0.22f), circleSprite);
            AddImage("Guardian Menu Core", canvasObject.transform, new Vector2(85f, -60f), new Vector2(96f, 96f), Hex("2EBBEA"), circleSprite);
            AddText("Guardian Menu Text", canvasObject.transform, new Vector2(85f, -60f), new Vector2(82f, 82f), "CG", 27, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddImage("Virus Menu Glow", canvasObject.transform, new Vector2(530f, 10f), new Vector2(170f, 170f), new Color(1f, 0.06f, 0.13f, 0.30f), circleSprite);
            if (virusSprite != null)
            {
                AddImage("Virus Menu Art", canvasObject.transform, new Vector2(530f, 10f), new Vector2(140f, 140f), Color.white, virusSprite);
            }
            else
            {
                AddText("Virus Menu Text", canvasObject.transform, new Vector2(530f, 10f), new Vector2(120f, 120f), "VX", 36, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            }

            RectTransform beam = AddImage("Preview Attack Route", canvasObject.transform, Vector2.zero, new Vector2(1f, 8f), new Color(0.29f, 0.95f, 0.66f, 0.55f), panelSprite).rectTransform;
            PlaceLine(beam, new Vector2(145f, -60f), new Vector2(470f, 8f), 8f);

            Vector2[] menuOrbs =
            {
                new Vector2(245f, -18f),
                new Vector2(308f, -4f),
                new Vector2(370f, 8f)
            };

            for (int i = 0; i < menuOrbs.Length; i++)
            {
                Image orb = AddImage("Preview Quiz Orb " + i, canvasObject.transform, menuOrbs[i], new Vector2(54f, 54f), GetCategoryColor(i), circleSprite);
                AddText("Preview Quiz Orb Label " + i, orb.transform, Vector2.zero, new Vector2(46f, 46f), i == 0 ? "PW" : i == 1 ? "MW" : "NT", 13, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            }

            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        }

        private static void BuildLevelScene(
            Sprite panelSprite,
            Sprite circleSprite,
            Sprite virusSprite,
            Sprite virusAltSprite,
            Sprite projectileSprite,
            Sprite trajectoryDotSprite,
            Sprite orbShellSprite,
            Sprite crosshairSprite,
            Sprite panelFrameSprite,
            Sprite buttonSprite,
            Sprite healthBarSprite,
            Sprite energyBarSprite,
            Sprite circuitSprite,
            QuizQuestionBank questionBank,
            DifficultyProfile[] difficulties,
            Font font,
            AudioClip launchSfx,
            AudioClip orbHitSfx,
            AudioClip virusHitSfx,
            AudioClip shieldSfx,
            AudioClip wrongSfx)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CyberGuardian_Level01";

            Camera camera = CreateCamera("0B1D24");
            camera.orthographicSize = 5f;
            EnsureEventSystem();

            GameObject canvasObject = CreateCanvas("Cyber Guardian Level 01");
            AddStretchImage("Background - Computer Core", canvasObject.transform, Hex("0B1D24"), panelSprite);
            if (circuitSprite != null)
            {
                Image circuitOverlay = AddStretchImage("Circuit Board Texture Overlay", canvasObject.transform, new Color(0.19f, 0.85f, 0.90f, 0.13f), circuitSprite);
                circuitOverlay.type = Image.Type.Tiled;
                circuitOverlay.pixelsPerUnitMultiplier = 0.36f;
                circuitOverlay.raycastTarget = false;
            }

            AddStretchImage("Top Hud Band", canvasObject.transform, new Color(0.11f, 0.39f, 0.44f, 0.74f), panelSprite, new Vector2(0f, 0.86f), Vector2.one, Vector2.zero, Vector2.zero);
            AddStretchImage("Bottom Command Band", canvasObject.transform, new Color(0.10f, 0.31f, 0.36f, 0.78f), panelSprite, Vector2.zero, new Vector2(1f, 0.15f), Vector2.zero, Vector2.zero);
            AddPanel("Playfield", canvasObject.transform, new Vector2(0f, 12f), new Vector2(1180f, 575f), Hex("102D35"), panelSprite, 0.82f);
            AddPanel("Playfield Inner", canvasObject.transform, new Vector2(0f, 12f), new Vector2(1125f, 520f), Hex("143943"), panelSprite, 0.48f);
            if (panelFrameSprite != null)
            {
                Image playfieldFrame = AddImage("Cyberpunk Playfield Frame Asset", canvasObject.transform, new Vector2(0f, 12f), new Vector2(1192f, 586f), new Color(1f, 1f, 1f, 0.95f), panelFrameSprite);
                playfieldFrame.type = Image.Type.Sliced;
                playfieldFrame.raycastTarget = false;
            }

            CyberGuardianLevelController level = new GameObject("Cyber Guardian Level Controller").AddComponent<CyberGuardianLevelController>();
            level.quizQuestionBank = questionBank;
            level.difficultyProfiles = difficulties;
            level.defaultDifficulty = difficulties.Length > 1 ? difficulties[1] : null;
            level.sfxSource = level.gameObject.AddComponent<AudioSource>();
            level.sfxSource.playOnAwake = false;
            level.sfxSource.volume = 0.78f;
            level.launchSfx = launchSfx;
            level.orbHitSfx = orbHitSfx;
            level.virusHitSfx = virusHitSfx;
            level.shieldSfx = shieldSfx;
            level.wrongAnswerSfx = wrongSfx;

            Sprite activeButtonSprite = buttonSprite != null ? buttonSprite : panelSprite;
            BuildLevelHud(canvasObject.transform, level, font, panelSprite, activeButtonSprite, healthBarSprite, energyBarSprite);
            BuildLevelPlayfield(canvasObject.transform, level, font, panelSprite, circleSprite, virusSprite, virusAltSprite, projectileSprite, trajectoryDotSprite, orbShellSprite, crosshairSprite);
            BuildQuizModal(canvasObject.transform, level, font, panelSprite, activeButtonSprite, panelFrameSprite);

            EditorSceneManager.SaveScene(scene, LevelScenePath);
        }

        private static void BuildLevelHud(Transform parent, CyberGuardianLevelController level, Font font, Sprite panelSprite, Sprite buttonSprite, Sprite healthBarSprite, Sprite energyBarSprite)
        {
            AddPanel("Score Box", parent, new Vector2(-540f, 332f), new Vector2(235f, 56f), Hex("C7973D"), panelSprite, 1f);
            level.scoreText = AddText("Score Text", parent, new Vector2(-540f, 332f), new Vector2(210f, 42f), "SCORE 1200", 20, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);

            AddPanel("Difficulty Box", parent, new Vector2(-280f, 332f), new Vector2(178f, 56f), Hex("295D68"), panelSprite, 1f);
            level.difficultyText = AddText("Difficulty Text", parent, new Vector2(-280f, 332f), new Vector2(150f, 42f), "NORMAL", 18, Hex("D7EFEF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            AddPanel("Route Box", parent, new Vector2(0f, 332f), new Vector2(260f, 56f), Hex("C7973D"), panelSprite, 1f);
            level.routeText = AddText("Route Text", parent, new Vector2(0f, 332f), new Vector2(230f, 42f), "ROUTE BLOCKED", 20, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);

            BuildStatusBar(parent, level, font, panelSprite, healthBarSprite, new Vector2(300f, 332f), "Shield", Hex("44D26A"), true);
            BuildStatusBar(parent, level, font, panelSprite, healthBarSprite, new Vector2(540f, 332f), "Virus", Hex("E84B56"), false);

            AddPanel("Status Box", parent, new Vector2(0f, -326f), new Vector2(615f, 52f), Hex("C7973D"), panelSprite, 1f);
            level.statusText = AddText("Status Text", parent, new Vector2(0f, -326f), new Vector2(580f, 38f), "HIT QUIZ FIREWALL ORBS, ANSWER CORRECTLY, THEN OPEN A PATH", 15, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);

            if (energyBarSprite != null)
            {
                Image energyBarAsset = AddImage("Launch Energy Bar Asset", parent, new Vector2(-500f, -270f), new Vector2(230f, 48f), new Color(1f, 1f, 1f, 0.70f), energyBarSprite);
                energyBarAsset.raycastTarget = false;
            }

            AddPanel("Launch Power Back", parent, new Vector2(-500f, -270f), new Vector2(210f, 36f), Hex("17282D"), panelSprite, 0.92f);
            Image launchPower = AddImage("Launch Power Fill", parent, new Vector2(-500f, -278f), new Vector2(176f, 13f), Hex("7CF8FF"), panelSprite);
            launchPower.type = Image.Type.Filled;
            launchPower.fillMethod = Image.FillMethod.Horizontal;
            launchPower.fillAmount = 0f;
            level.launchPowerFill = launchPower;
            level.launchHintText = AddText("Launch Power Text", parent, new Vector2(-500f, -263f), new Vector2(185f, 20f), "DRAG PATCH CORE", 12, Hex("D7EFEF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            level.attackButton = AddButton("Attack Button", parent, new Vector2(-500f, -326f), new Vector2(210f, 58f), "DRAG PATCH CORE", 16, font, Hex("55D264"), Color.white, buttonSprite, out Text attackLabel);
            level.attackButtonLabel = attackLabel;
            level.pauseButton = AddButton("Pause Button", parent, new Vector2(455f, -326f), new Vector2(118f, 52f), "PAUSE", 16, font, Hex("C7973D"), Color.white, buttonSprite, out Text pauseLabel);
            level.pauseButtonLabel = pauseLabel;
            level.resetButton = AddButton("Reset Button", parent, new Vector2(588f, -326f), new Vector2(118f, 52f), "RESET", 16, font, Hex("295D68"), Color.white, buttonSprite, out _);
            level.menuButton = AddButton("Menu Button", parent, new Vector2(587f, 250f), new Vector2(116f, 46f), "MENU", 16, font, Hex("A83C48"), Color.white, buttonSprite, out _);
        }

        private static void BuildStatusBar(Transform parent, CyberGuardianLevelController level, Font font, Sprite panelSprite, Sprite barAssetSprite, Vector2 position, string label, Color fillColor, bool shield)
        {
            AddPanel(label + " Bar Back", parent, position, new Vector2(205f, 48f), Hex("17282D"), panelSprite, 0.92f);
            if (barAssetSprite != null)
            {
                Image barAsset = AddImage(label + " Cyberpunk Bar Asset", parent, position, new Vector2(222f, 52f), new Color(1f, 1f, 1f, 0.58f), barAssetSprite);
                barAsset.raycastTarget = false;
            }

            Image fill = AddImage(label + " Bar Fill", parent, position + new Vector2(0f, -9f), new Vector2(178f, 17f), fillColor, panelSprite);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = shield ? 1f : 0.25f;
            Text text = AddText(label + " Bar Text", parent, position + new Vector2(0f, 8f), new Vector2(185f, 22f), label.ToUpperInvariant() + (shield ? " 100%" : " 25%"), 14, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);

            if (shield)
            {
                level.shieldFill = fill;
                level.shieldText = text;
            }
            else
            {
                level.virusFill = fill;
                level.virusText = text;
            }
        }

        private static void BuildLevelPlayfield(Transform parent, CyberGuardianLevelController level, Font font, Sprite panelSprite, Sprite circleSprite, Sprite virusSprite, Sprite virusAltSprite, Sprite projectileSprite, Sprite trajectoryDotSprite, Sprite orbShellSprite, Sprite crosshairSprite)
        {
            Vector2 guardianPosition = new Vector2(-505f, -75f);
            Vector2 virusPosition = new Vector2(505f, 18f);

            level.guardianShieldGlow = AddImage("Guardian Shield Glow", parent, guardianPosition, new Vector2(170f, 170f), new Color(0.13f, 0.76f, 1f, 0.24f), circleSprite);
            level.guardianShieldGlow.raycastTarget = false;
            if (crosshairSprite != null)
            {
                Image crosshair = AddImage("Slingshot Aim Crosshair", parent, guardianPosition, new Vector2(124f, 124f), new Color(0.55f, 1f, 1f, 0.72f), crosshairSprite);
                crosshair.raycastTarget = false;
            }

            Image guardianCore = AddImage("Guardian Core", parent, guardianPosition, new Vector2(102f, 102f), Hex("2EBBEA"), circleSprite);
            guardianCore.raycastTarget = false;
            AddText("Guardian Label", parent, guardianPosition, new Vector2(88f, 88f), "CG", 28, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            level.guardianAnchor = AddEmptyRect("Guardian Anchor", parent, guardianPosition, new Vector2(20f, 20f));

            level.virusGlow = AddImage("Virus Glow", parent, virusPosition, new Vector2(180f, 180f), new Color(1f, 0.06f, 0.13f, 0.30f), circleSprite);
            level.virusGlow.raycastTarget = false;
            if (virusSprite != null)
            {
                Image virusArt = AddImage("Virus Art", parent, virusPosition, new Vector2(140f, 140f), Color.white, virusSprite);
                virusArt.raycastTarget = false;
                if (virusAltSprite != null)
                {
                    Image virusOverlay = AddImage("Virus Mutation Overlay", parent, virusPosition + new Vector2(18f, 12f), new Vector2(86f, 86f), new Color(1f, 0.35f, 0.42f, 0.62f), virusAltSprite);
                    virusOverlay.raycastTarget = false;
                }
            }
            else
            {
                Image virusPlaceholder = AddImage("Virus Placeholder", parent, virusPosition, new Vector2(115f, 115f), Hex("D83A55"), circleSprite);
                virusPlaceholder.raycastTarget = false;
                AddText("Virus Placeholder Text", parent, virusPosition, new Vector2(88f, 88f), "VX", 28, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            }

            level.virusAnchor = AddEmptyRect("Virus Anchor", parent, virusPosition, new Vector2(20f, 20f));
            Image attackRouteImage = AddImage("Attack Route Preview", parent, Vector2.zero, new Vector2(1f, 7f), new Color(1f, 0.72f, 0.20f, 0.55f), panelSprite);
            attackRouteImage.raycastTarget = false;
            level.attackBeam = attackRouteImage.rectTransform;

            Vector2 leftBandAnchor = guardianPosition + new Vector2(-34f, 36f);
            Vector2 rightBandAnchor = guardianPosition + new Vector2(-34f, -36f);
            level.slingshotLeftAnchor = AddEmptyRect("Slingshot Upper Anchor", parent, leftBandAnchor, new Vector2(12f, 12f));
            level.slingshotRightAnchor = AddEmptyRect("Slingshot Lower Anchor", parent, rightBandAnchor, new Vector2(12f, 12f));
            Image leftAnchorDot = AddImage("Slingshot Upper Anchor Dot", parent, leftBandAnchor, new Vector2(18f, 18f), Hex("7CF8FF"), circleSprite);
            Image rightAnchorDot = AddImage("Slingshot Lower Anchor Dot", parent, rightBandAnchor, new Vector2(18f, 18f), Hex("7CF8FF"), circleSprite);
            leftAnchorDot.raycastTarget = false;
            rightAnchorDot.raycastTarget = false;

            Image leftBand = AddImage("Slingshot Energy Band Upper", parent, Vector2.zero, new Vector2(1f, 6f), new Color(0.35f, 1f, 1f, 0.82f), panelSprite);
            Image rightBand = AddImage("Slingshot Energy Band Lower", parent, Vector2.zero, new Vector2(1f, 6f), new Color(0.35f, 1f, 1f, 0.82f), panelSprite);
            leftBand.raycastTarget = false;
            rightBand.raycastTarget = false;
            level.slingshotBandLeft = leftBand.rectTransform;
            level.slingshotBandRight = rightBand.rectTransform;
            PlaceLine(level.slingshotBandLeft, leftBandAnchor, guardianPosition, 6f);
            PlaceLine(level.slingshotBandRight, rightBandAnchor, guardianPosition, 6f);

            Sprite dotSprite = trajectoryDotSprite != null ? trajectoryDotSprite : circleSprite;
            level.trajectoryDots = new RectTransform[16];
            for (int i = 0; i < level.trajectoryDots.Length; i++)
            {
                float size = Mathf.Lerp(20f, 10f, i / Mathf.Max(1f, level.trajectoryDots.Length - 1f));
                Image dot = AddImage("Trajectory Dot " + i.ToString("00"), parent, guardianPosition, new Vector2(size, size), new Color(0.64f, 1f, 1f, 0.80f), dotSprite);
                dot.raycastTarget = false;
                dot.gameObject.SetActive(false);
                level.trajectoryDots[i] = dot.rectTransform;
            }

            Sprite activeProjectileSprite = projectileSprite != null ? projectileSprite : circleSprite;
            Image projectileImage = AddImage("Attack Projectile", parent, guardianPosition, new Vector2(52f, 52f), projectileSprite != null ? new Color(0.72f, 1f, 1f, 0.96f) : Hex("84F6FF"), activeProjectileSprite);
            projectileImage.raycastTarget = true;
            level.attackProjectile = projectileImage.rectTransform;
            CyberGuardianSlingshotHandle slingshotHandle = projectileImage.gameObject.AddComponent<CyberGuardianSlingshotHandle>();
            slingshotHandle.controller = level;

            AddText("Guardian Name", parent, guardianPosition + new Vector2(0f, -84f), new Vector2(190f, 28f), "CYBER GUARDIAN", 16, Hex("D7EFEF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);
            AddText("Virus Name", parent, virusPosition + new Vector2(0f, -94f), new Vector2(170f, 28f), "MALWARE CORE", 16, Hex("FFD5DA"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            AddText("Firewall Label", parent, new Vector2(8f, 247f), new Vector2(390f, 28f), "QUIZ FIREWALL", 15, Hex("7EE6F1"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            List<Vector2> orbPositions = new List<Vector2>();
            const int rows = 9;
            const int columns = 8;
            const float startX = -238f;
            const float startY = -218f;
            const float xSpacing = 58f;
            const float ySpacing = 56f;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    float x = startX + column * xSpacing + (row % 2 == 0 ? 0f : xSpacing * 0.5f);
                    float y = startY + row * ySpacing;
                    orbPositions.Add(new Vector2(x, y));
                }
            }

            for (int i = 0; i < orbPositions.Count; i++)
            {
                int category = Mathf.Abs((i * 3) + Mathf.FloorToInt(orbPositions[i].y)) % 4;
                CyberGuardianLevelController.QuizOrbNode node = AddQuizOrb("Quiz Orb " + i.ToString("00"), parent, orbPositions[i], category, font, circleSprite, orbShellSprite);
                level.quizOrbs.Add(node);
            }
        }

        private static CyberGuardianLevelController.QuizOrbNode AddQuizOrb(string name, Transform parent, Vector2 position, int category, Font font, Sprite circleSprite, Sprite orbShellSprite)
        {
            GameObject outerObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            outerObject.transform.SetParent(parent, false);
            RectTransform rect = outerObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(68f, 68f);

            Image outer = outerObject.GetComponent<Image>();
            outer.sprite = orbShellSprite != null ? orbShellSprite : circleSprite;
            outer.color = new Color(1f, 1f, 1f, 0.92f);

            Image fill = AddImage(name + " Fill", outerObject.transform, Vector2.zero, new Vector2(52f, 52f), GetCategoryColor(category), circleSprite);
            Text text = AddText(name + " Label", outerObject.transform, Vector2.zero, new Vector2(48f, 48f), GetCategoryCode(category), 14, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            text.raycastTarget = false;

            Button button = outerObject.GetComponent<Button>();
            button.targetGraphic = fill;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
            colors.pressedColor = new Color(0.84f, 0.94f, 1f, 1f);
            colors.disabledColor = Color.white;
            button.colors = colors;

            return new CyberGuardianLevelController.QuizOrbNode
            {
                button = button,
                fill = fill,
                label = text,
                rectTransform = rect,
                category = category,
                startsCleared = false,
                cleared = false
            };
        }

        private static void BuildQuizModal(Transform parent, CyberGuardianLevelController level, Font font, Sprite panelSprite, Sprite buttonSprite, Sprite panelFrameSprite)
        {
            GameObject modal = new GameObject("Quiz Modal", typeof(RectTransform));
            modal.transform.SetParent(parent, false);
            RectTransform modalRect = modal.GetComponent<RectTransform>();
            modalRect.anchorMin = Vector2.zero;
            modalRect.anchorMax = Vector2.one;
            modalRect.offsetMin = Vector2.zero;
            modalRect.offsetMax = Vector2.zero;

            AddStretchImage("Modal Dim", modal.transform, new Color(0f, 0f, 0f, 0.52f), panelSprite);
            AddPanel("Quiz Window", modal.transform, Vector2.zero, new Vector2(690f, 395f), Hex("142F38"), panelSprite, 0.98f);
            if (panelFrameSprite != null)
            {
                Image frameAsset = AddImage("Quiz Modal Cyberpunk Frame Asset", modal.transform, Vector2.zero, new Vector2(708f, 413f), new Color(1f, 1f, 1f, 0.95f), panelFrameSprite);
                frameAsset.type = Image.Type.Sliced;
                frameAsset.raycastTarget = false;
            }

            AddPanel("Quiz Header", modal.transform, new Vector2(0f, 152f), new Vector2(600f, 58f), Hex("C7973D"), panelSprite, 1f);
            level.quizTitleText = AddText("Quiz Title", modal.transform, new Vector2(0f, 152f), new Vector2(570f, 44f), "QUIZ NODE", 22, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            level.quizPromptText = AddText("Quiz Prompt", modal.transform, new Vector2(0f, 80f), new Vector2(590f, 75f), "Pertanyaan keamanan siber", 20, Color.white, font, TextAnchor.MiddleCenter, FontStyle.Normal);
            level.feedbackText = AddText("Quiz Feedback", modal.transform, new Vector2(0f, -146f), new Vector2(590f, 42f), string.Empty, 16, Hex("B7F7FF"), font, TextAnchor.MiddleCenter, FontStyle.Bold);

            level.answerButtons = new Button[4];
            level.answerLabels = new Text[4];
            Vector2[] answerPositions =
            {
                new Vector2(-165f, 10f),
                new Vector2(165f, 10f),
                new Vector2(-165f, -70f),
                new Vector2(165f, -70f)
            };

            for (int i = 0; i < answerPositions.Length; i++)
            {
                Button answer = AddButton("Answer " + (i + 1), modal.transform, answerPositions[i], new Vector2(300f, 58f), "ANSWER", 15, font, Hex("2E6E75"), Color.white, buttonSprite, out Text label);
                level.answerButtons[i] = answer;
                level.answerLabels[i] = label;
            }

            level.closeQuizButton = AddButton("Close Quiz", modal.transform, new Vector2(300f, 170f), new Vector2(42f, 42f), "X", 20, font, Hex("A83C48"), Color.white, buttonSprite, out _);
            level.quizModal = modal;
            modal.SetActive(false);
        }

        private static GameObject CreateCanvas(string name)
        {
            GameObject canvasObject = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1366f, 768f);
            scaler.matchWidthOrHeight = 0.5f;
            return canvasObject;
        }

        private static Camera CreateCamera(string backgroundHex)
        {
            Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Hex(backgroundHex);
            camera.orthographic = true;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            return camera;
        }

        private static RectTransform AddEmptyRect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
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
            colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
            colors.pressedColor = new Color(0.82f, 0.90f, 0.94f, 1f);
            button.colors = colors;

            label = AddText(name + " Label", buttonObject.transform, Vector2.zero, size - new Vector2(12f, 10f), text, fontSize, textColor, font, TextAnchor.MiddleCenter, FontStyle.Bold);
            label.raycastTarget = false;
            return button;
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
            return AddStretchImage(name, parent, color, sprite, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        }

        private static Image AddStretchImage(string name, Transform parent, Color color, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            Image image = imageObject.GetComponent<Image>();
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
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

        private static void EnsureEventSystem()
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static void SetBuildScenes()
        {
            string[] requiredScenes =
            {
                MainMenuScenePath,
                LevelScenePath
            };

            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
            for (int i = 0; i < requiredScenes.Length; i++)
            {
                if (File.Exists(ToAbsolutePath(requiredScenes[i])))
                {
                    scenes.Add(new EditorBuildSettingsScene(requiredScenes[i], true));
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
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
                    new QuizQuestion(CyberQuestionCategory.Password, "PASSWORD NODE", "Apa ciri password yang aman?", new[] { "Panjang dan unik", "Nama sendiri", "123456", "Tanggal lahir" }, 0, "Benar. Password aman sebaiknya panjang, unik, dan tidak dipakai ulang."),
                    new QuizQuestion(CyberQuestionCategory.Password, "LOGIN SHIELD", "Apa fungsi utama autentikasi dua faktor?", new[] { "Menambah lapisan verifikasi", "Menghapus password", "Membuka semua akun", "Melemahkan akun" }, 0, "Benar. 2FA menambah bukti verifikasi selain password."),
                    new QuizQuestion(CyberQuestionCategory.Malware, "PHISHING FILTER", "Tanda umum phishing adalah...", new[] { "Alamat pengirim mencurigakan", "Bahasa selalu sempurna", "Tidak pernah ada link", "Selalu dari teman" }, 0, "Benar. Pengirim, domain, dan permintaan mendesak perlu diperiksa."),
                    new QuizQuestion(CyberQuestionCategory.Malware, "FILE SCAN", "File lampiran dari sumber tidak dikenal sebaiknya...", new[] { "Tidak dibuka sembarangan", "Langsung dijalankan", "Dibagikan ke grup", "Diubah namanya saja" }, 0, "Benar. Lampiran asing dapat membawa malware."),
                    new QuizQuestion(CyberQuestionCategory.Network, "FIREWALL NODE", "Firewall digunakan untuk...", new[] { "Membantu menyaring koneksi", "Membuat virus", "Menghapus backup", "Membuka semua port" }, 0, "Benar. Firewall membantu mengontrol koneksi masuk dan keluar."),
                    new QuizQuestion(CyberQuestionCategory.Network, "UPDATE PATCH", "Mengapa update sistem penting?", new[] { "Menutup celah keamanan", "Melepas proteksi", "Membagikan data", "Mematikan enkripsi" }, 0, "Benar. Update sering membawa patch untuk celah keamanan."),
                    new QuizQuestion(CyberQuestionCategory.Privacy, "PRIVACY CORE", "Data apa yang tidak boleh dibagikan sembarangan?", new[] { "NIK, OTP, password", "Warna favorit", "Genre game", "Nama panggilan" }, 0, "Benar. Data sensitif bisa dipakai untuk penipuan atau pengambilalihan akun."),
                    new QuizQuestion(CyberQuestionCategory.Privacy, "DATA MINIMIZE", "Prinsip minimisasi data berarti...", new[] { "Hanya minta data yang perlu", "Kumpulkan semua data", "Sebar data cadangan", "Simpan tanpa batas" }, 0, "Benar. Data yang dikumpulkan sebaiknya sesuai kebutuhan.")
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
                EnsureDifficultyProfile(EasyDifficultyPath, "Easy", 110f, 100, 10, 5, 8, 10, 8, 200, 25),
                EnsureDifficultyProfile(NormalDifficultyPath, "Normal", 87f, 100, 20, 7, 6, 15, 12, 250, 30),
                EnsureDifficultyProfile(HardDifficultyPath, "Hard", 70f, 85, 35, 10, 4, 22, 18, 320, 22)
            };
        }

        private static DifficultyProfile EnsureDifficultyProfile(string path, string displayName, float time, int shield, int virus, int routeOrbs, int shieldReward, int wrongDamage, int wrongVirusGain, int routeScore, int routeDamage)
        {
            DifficultyProfile profile = AssetDatabase.LoadAssetAtPath<DifficultyProfile>(path);
            if (profile != null)
            {
                return profile;
            }

            profile = ScriptableObject.CreateInstance<DifficultyProfile>();
            profile.displayName = displayName;
            profile.startingTime = time;
            profile.startingScore = 1200;
            profile.startingTokens = 15;
            profile.startingShield = shield;
            profile.startingVirusStrength = virus;
            profile.requiredRouteOrbs = routeOrbs;
            profile.correctScoreReward = 75;
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

        private static Sprite EnsureImportedSprite(string assetPath, Vector4 spriteBorder)
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
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.spriteBorder = spriteBorder;
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
            Circle,
            RoundedRect
        }

        private static Sprite EnsureGeneratedSprite(string assetPath, TextureShape shape)
        {
            if (!File.Exists(ToAbsolutePath(assetPath)))
            {
                Texture2D texture = shape == TextureShape.Circle ? GenerateCircleTexture(128) : GenerateRoundedRectTexture(128, 28);
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
                importer.spriteBorder = shape == TextureShape.RoundedRect ? new Vector4(28f, 28f, 28f, 28f) : Vector4.zero;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
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

        private static void PlaceLine(RectTransform line, Vector2 start, Vector2 end, float thickness)
        {
            Vector2 delta = end - start;
            line.anchoredPosition = start + delta * 0.5f;
            line.sizeDelta = new Vector2(delta.magnitude, thickness);
            line.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
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
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
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
