using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CyberGuardian
{
    public sealed class CyberGuardianMainMenu : MonoBehaviour
    {
        public const string DifficultyKey = "CyberGuardianDifficulty";
        public const string SaveExistsKey = "CyberGuardianSaveExists";
        public const string SaveSceneKey = "CyberGuardianSaveScene";
        public const string SaveXKey = "CyberGuardianSaveX";
        public const string SaveYKey = "CyberGuardianSaveY";
        public const string SaveZKey = "CyberGuardianSaveZ";
        public const string SaveHealthKey = "CyberGuardianSaveHealth";
        public const string SaveLivesKey = "CyberGuardianSaveLives";
        public const string SaveBoostKey = "CyberGuardianSaveBoost";
        public const string SaveScoreKey = "CyberGuardianSaveScore";
        public const string ResumeRequestedKey = "CyberGuardianResumeRequested";
        public const string DeveloperModeKey = "CyberGuardianDeveloperMode";
        public const string MusicEnabledKey = "CyberGuardianMusicEnabled";
        public const string SfxEnabledKey = "CyberGuardianSfxEnabled";
        public const string MusicVolumeKey = "CyberGuardianMusicVolume";
        public const string SfxVolumeKey = "CyberGuardianSfxVolume";
        public const string ControlSchemeKey = "CyberGuardianControlScheme";
        public const string KnowledgeUnlockPrefix = "CyberGuardianKnowledgeUnlocked_";
        public const string KnowledgeTitlePrefix = "CyberGuardianKnowledgeTitle_";
        public const string KnowledgeBodyPrefix = "CyberGuardianKnowledgeBody_";
        public const string CampaignCompleteKey = "CyberGuardianCampaignComplete";
        public const string DeveloperUsername = "developer";
        public const string DeveloperPassword = "cyberguardian2026";

        public string gameplaySceneName = "CyberGuardian_Level01";
        public string difficultySceneName = "CyberGuardian_PilihKesulitan";
        public Text selectedDifficultyText;
        public Button startButton;
        public Button continueButton;
        public Button easyButton;
        public Button normalButton;
        public Button hardButton;
        public Button settingsButton;
        public Button galleryButton;
        public Button creditsButton;
        public Button developerButton;
        public Button developerLoginButton;
        public Button developerDisableButton;
        public Button developerCancelButton;
        public Button quitButton;
        public Button settingsBackButton;
        public Button settingsMusicToggleButton;
        public Button settingsSfxToggleButton;
        public Button settingsControlSchemeButton;
        public Button galleryBackButton;
        public Button creditsBackButton;
        public GameObject settingsPanel;
        public GameObject galleryPanel;
        public GameObject creditsPanel;
        public GameObject developerLoginPanel;
        public InputField developerUserInput;
        public InputField developerPasswordInput;
        public Text developerStatusText;
        public Text settingsMusicText;
        public Text settingsSfxText;
        public Text settingsControlText;
        public Text galleryBodyText;
        public Image galleryIllustrationImage;
        public Sprite galleryWindowSprite;
        public Sprite galleryPaperSprite;
        public Sprite galleryChoiceSprite;
        public Sprite galleryBookButtonSprite;
        public Image[] difficultyHighlights;
        public GameObject startTransitionOverlay;
        public Image startTransitionFade;
        public Image startTransitionCircuit;
        public Text startTransitionText;
        public Image[] startTransitionFx;
        public float startTransitionDuration = 2.25f;

        private readonly string[] difficultyNames = { "Mudah", "Normal", "Sulit" };
        private int selectedDifficulty = 1;
        private bool startingGame;
        private string pendingSceneName = string.Empty;
        private string pendingTransitionIntro = "MENYIAPKAN CYBER GUARDIAN";
        private string pendingTransitionLoad = "MEMBUKA JALUR AMAN";
        private GameObject galleryRoot;
        private GameObject galleryChestView;
        private GameObject galleryBookView;
        private GameObject galleryReaderView;
        private RectTransform galleryChestContainer;
        private RectTransform galleryBookContainer;
        private Text galleryTitleText;
        private Text galleryHintText;
        private Text galleryReaderTitleText;
        private Text galleryReaderBodyText;
        private Text galleryReaderPageText;
        private Image galleryReaderPaperImage;
        private Image galleryReaderIllustrationImage;
        private Button galleryBookBackButton;
        private Button galleryReaderBackButton;
        private Button galleryPreviousPageButton;
        private Button galleryNextPageButton;
        private readonly List<Button> galleryDynamicButtons = new List<Button>();
        private int galleryViewMode;
        private int selectedGalleryChest;
        private int selectedGalleryBook;
        private int selectedGalleryPage;

        private const string FantasyGuiRoot = "Assets/CyberGuardian/assets/new/Fantasy_project/game/gui/";
        private const string FantasyGalleryPath = FantasyGuiRoot + "gallery.png";
        private const string FantasyTextboxPath = FantasyGuiRoot + "textbox.png";
        private const string FantasyChoicePath = FantasyGuiRoot + "button/choice_idle_background.png";
        private const string FantasyGalleryButtonPath = FantasyGuiRoot + "button/gallerybutton_idle_blank.png";

        private sealed class GalleryChest
        {
            public readonly int UnlockTier;
            public readonly string Title;
            public readonly string Subtitle;
            public readonly string LockedHint;
            public readonly bool AlwaysAvailable;
            public readonly GalleryBook[] Books;

            public GalleryChest(int unlockTier, string title, string subtitle, string lockedHint, bool alwaysAvailable, GalleryBook[] books)
            {
                UnlockTier = unlockTier;
                Title = title;
                Subtitle = subtitle;
                LockedHint = lockedHint;
                AlwaysAvailable = alwaysAvailable;
                Books = books;
            }
        }

        private sealed class GalleryBook
        {
            public readonly string Title;
            public readonly string Subtitle;
            public readonly Color AccentColor;
            public readonly GalleryPage[] Pages;

            public GalleryBook(string title, string subtitle, Color accentColor, GalleryPage[] pages)
            {
                Title = title;
                Subtitle = subtitle;
                AccentColor = accentColor;
                Pages = pages;
            }
        }

        private sealed class GalleryPage
        {
            public readonly string Title;
            public readonly string Body;

            public GalleryPage(string title, string body)
            {
                Title = title;
                Body = body;
            }
        }

        private void Awake()
        {
            selectedDifficulty = Mathf.Clamp(PlayerPrefs.GetInt(DifficultyKey, 1), 0, 2);
            EnsureDefaultPreferences();
            EnsureRuntimeGalleryUi();
            WireButtons();
            HidePanels();
            ApplyAudioPreferencesToScene();
            Refresh();
        }

        private void WireButtons()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(() => RunButtonAction(StartNewGame));
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() => RunButtonAction(ContinueGame));
            }

            WireDifficultyButton(easyButton, 0);
            WireDifficultyButton(normalButton, 1);
            WireDifficultyButton(hardButton, 2);

            WirePanelButton(settingsButton, settingsPanel);
            WireGalleryButton();
            WirePanelButton(creditsButton, creditsPanel);
            WireCloseButton(settingsBackButton, settingsPanel);
            WireCloseButton(galleryBackButton, galleryPanel);
            WireCloseButton(creditsBackButton, creditsPanel);
            WireSettingsButton(settingsMusicToggleButton, ToggleMusic);
            WireSettingsButton(settingsSfxToggleButton, ToggleSfx);
            WireSettingsButton(settingsControlSchemeButton, CycleControlScheme);

            if (developerButton != null)
            {
                developerButton.onClick.RemoveAllListeners();
                developerButton.onClick.AddListener(() => RunButtonAction(ShowDeveloperLogin));
            }

            if (developerLoginButton != null)
            {
                developerLoginButton.onClick.RemoveAllListeners();
                developerLoginButton.onClick.AddListener(() => RunButtonAction(SubmitDeveloperLogin));
            }

            if (developerDisableButton != null)
            {
                developerDisableButton.onClick.RemoveAllListeners();
                developerDisableButton.onClick.AddListener(() => RunButtonAction(DisableDeveloperMode));
            }

            if (developerCancelButton != null)
            {
                developerCancelButton.onClick.RemoveAllListeners();
                developerCancelButton.onClick.AddListener(() =>
                {
                    RunButtonAction(() =>
                    {
                        if (developerLoginPanel != null)
                        {
                            developerLoginPanel.SetActive(false);
                        }
                    });
                });
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(() => RunButtonAction(QuitGame));
            }
        }

        private void WireDifficultyButton(Button button, int index)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => RunButtonAction(() => SelectDifficulty(index)));
        }

        private void WirePanelButton(Button button, GameObject panel)
        {
            if (button == null || panel == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => RunButtonAction(() => ShowOnlyPanel(panel)));
        }

        private void WireCloseButton(Button button, GameObject panel)
        {
            if (button == null || panel == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => RunButtonAction(() => panel.SetActive(false)));
        }

        private void WireGalleryButton()
        {
            if (galleryButton == null || galleryPanel == null)
            {
                return;
            }

            galleryButton.onClick.RemoveAllListeners();
            galleryButton.onClick.AddListener(() => RunButtonAction(OpenGallery));
        }

        private void RunButtonAction(System.Action action)
        {
            CyberGuardianRuntimeAudio.PlayButtonClick();
            if (action != null)
            {
                action.Invoke();
            }
        }

        private void ShowOnlyPanel(GameObject panel)
        {
            HidePanels();
            panel.SetActive(true);
        }

        private void OpenGallery()
        {
            HidePanels();
            if (galleryPanel == null)
            {
                return;
            }

            galleryViewMode = 0;
            selectedGalleryChest = 0;
            selectedGalleryBook = 0;
            selectedGalleryPage = 0;
            galleryPanel.SetActive(true);
            RefreshGalleryView();
        }

        private void ShowDeveloperLogin()
        {
            HidePanels();
            if (developerLoginPanel != null)
            {
                developerLoginPanel.SetActive(true);
            }

            if (developerStatusText != null)
            {
                developerStatusText.text = IsDeveloperModeEnabled()
                    ? "Mode developer aktif. Tekan NONAKTIFKAN untuk kembali ke mode normal."
                    : "Masuk untuk membuka HP dan energi tanpa batas.";
            }

            if (developerPasswordInput != null)
            {
                developerPasswordInput.text = string.Empty;
            }

            Refresh();
        }

        private void SubmitDeveloperLogin()
        {
            string username = developerUserInput != null ? developerUserInput.text.Trim() : string.Empty;
            string password = developerPasswordInput != null ? developerPasswordInput.text : string.Empty;
            if (username == DeveloperUsername && password == DeveloperPassword)
            {
                PlayerPrefs.SetInt(DeveloperModeKey, 1);
                PlayerPrefs.Save();
                if (developerStatusText != null)
                {
                    developerStatusText.text = "Mode developer aktif. HP, energi, dan petunjuk jawaban terbuka.";
                }

                Refresh();
                return;
            }

            PlayerPrefs.SetInt(DeveloperModeKey, 0);
            PlayerPrefs.Save();
            if (developerStatusText != null)
            {
                developerStatusText.text = "Login gagal. Periksa username dan password.";
            }

            Refresh();
        }

        private void DisableDeveloperMode()
        {
            PlayerPrefs.SetInt(DeveloperModeKey, 0);
            PlayerPrefs.Save();
            if (developerStatusText != null)
            {
                developerStatusText.text = "Mode developer nonaktif. Game kembali memakai HP, energi, dan kuis normal.";
            }

            Refresh();
        }

        private void ToggleMusic()
        {
            CycleVolume(MusicVolumeKey, MusicEnabledKey);
            PlayerPrefs.Save();
            ApplyAudioPreferencesToScene();
            Refresh();
        }

        private void ToggleSfx()
        {
            CycleVolume(SfxVolumeKey, SfxEnabledKey);
            PlayerPrefs.Save();
            ApplyAudioPreferencesToScene();
            Refresh();
        }

        private void CycleControlScheme()
        {
            PlayerPrefs.SetInt(ControlSchemeKey, (GetControlScheme() + 1) % 4);
            PlayerPrefs.Save();
            Refresh();
        }

        private static void CycleVolume(string volumeKey, string enabledKey)
        {
            int volume = Mathf.Clamp(PlayerPrefs.GetInt(volumeKey, 100), 0, 100);
            int nextVolume;
            if (volume > 75)
            {
                nextVolume = 75;
            }
            else if (volume > 50)
            {
                nextVolume = 50;
            }
            else if (volume > 25)
            {
                nextVolume = 25;
            }
            else if (volume > 0)
            {
                nextVolume = 0;
            }
            else
            {
                nextVolume = 100;
            }

            PlayerPrefs.SetInt(volumeKey, nextVolume);
            PlayerPrefs.SetInt(enabledKey, nextVolume > 0 ? 1 : 0);
        }

        private void SelectDifficulty(int index)
        {
            selectedDifficulty = Mathf.Clamp(index, 0, 2);
            PlayerPrefs.SetInt(DifficultyKey, selectedDifficulty);
            PlayerPrefs.Save();
            Refresh();
        }

        private void StartNewGame()
        {
            if (startingGame)
            {
                return;
            }

            PlayerPrefs.SetInt(DifficultyKey, selectedDifficulty);
            PlayerPrefs.SetInt(ResumeRequestedKey, 0);
            PlayerPrefs.Save();
            Time.timeScale = 1f;
            BeginSceneLoad(difficultySceneName, "MEMBUKA PANEL MISI", "PILIH TINGKAT KESULITAN");
        }

        private void ContinueGame()
        {
            if (startingGame || !HasSavedProgress())
            {
                return;
            }

            string sceneName = PlayerPrefs.GetString(SaveSceneKey, gameplaySceneName);
            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = gameplaySceneName;
            }

            PlayerPrefs.SetInt(ResumeRequestedKey, 1);
            PlayerPrefs.Save();
            Time.timeScale = 1f;
            BeginSceneLoad(sceneName, "MEMULIHKAN CHECKPOINT", "MEMBUKA ULANG JALUR AMAN");
        }

        private void BeginSceneLoad(string sceneName, string intro, string load)
        {
            pendingSceneName = string.IsNullOrEmpty(sceneName) ? gameplaySceneName : sceneName;
            pendingTransitionIntro = intro;
            pendingTransitionLoad = load;
            StartCoroutine(StartGameTransition());
        }

        private IEnumerator StartGameTransition()
        {
            startingGame = true;
            SetButtonsInteractable(false);
            HidePanels();

            if (startTransitionOverlay == null)
            {
                SceneManager.LoadScene(pendingSceneName);
                yield break;
            }

            startTransitionOverlay.SetActive(true);
            float duration = Mathf.Max(0.35f, startTransitionDuration);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float smooth = Mathf.SmoothStep(0f, 1f, t);

                if (startTransitionFade != null)
                {
                    startTransitionFade.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.92f, smooth));
                }

                if (startTransitionCircuit != null)
                {
                    startTransitionCircuit.color = new Color(0.20f, 0.95f, 1f, Mathf.Lerp(0f, 0.24f, Mathf.Sin(t * Mathf.PI)));
                }

                if (startTransitionText != null)
                {
                    startTransitionText.text = t < 0.52f ? pendingTransitionIntro : pendingTransitionLoad;
                    startTransitionText.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t * 1.75f)));
                    startTransitionText.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.92f, 1.05f, Mathf.Sin(t * Mathf.PI));
                }

                AnimateTransitionEffects(t);
                yield return null;
            }

            SceneManager.LoadScene(pendingSceneName);
        }

        private void AnimateTransitionEffects(float t)
        {
            if (startTransitionFx == null)
            {
                return;
            }

            for (int i = 0; i < startTransitionFx.Length; i++)
            {
                Image effect = startTransitionFx[i];
                if (effect == null)
                {
                    continue;
                }

                float phase = Mathf.Repeat(t * 1.85f + i * 0.16f, 1f);
                RectTransform rect = effect.rectTransform;
                float direction = i % 2 == 0 ? 1f : -1f;
                rect.anchoredPosition = new Vector2(Mathf.Lerp(-1080f, 1080f, phase) * direction, rect.anchoredPosition.y);
                Color color = effect.color;
                color.a = Mathf.Sin(phase * Mathf.PI) * 0.75f;
                effect.color = color;
            }
        }

        private void SetButtonsInteractable(bool interactable)
        {
            Button[] buttons =
            {
                startButton,
                continueButton,
                easyButton,
                normalButton,
                hardButton,
                settingsButton,
                galleryButton,
                creditsButton,
                quitButton,
                developerButton,
                developerLoginButton,
                developerDisableButton,
                developerCancelButton,
                settingsBackButton,
                settingsMusicToggleButton,
                settingsSfxToggleButton,
                settingsControlSchemeButton,
                galleryBackButton,
                creditsBackButton
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].interactable = interactable;
                }
            }
        }

        private void HidePanels()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }

            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }

            if (galleryPanel != null)
            {
                galleryPanel.SetActive(false);
            }

            if (developerLoginPanel != null)
            {
                developerLoginPanel.SetActive(false);
            }
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void EnsureRuntimeGalleryUi()
        {
            if (galleryButton != null && galleryPanel != null)
            {
                EnsureInteractiveGalleryUi(ResolveRuntimeFont());
                return;
            }

            Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include);
            if (canvases == null || canvases.Length == 0)
            {
                return;
            }

            Transform parent = canvases[0].transform;
            Font font = ResolveRuntimeFont();
            if (galleryButton == null)
            {
                galleryButton = CreateRuntimeMenuButton(parent, "Gallery Button", new Vector2(-832f, 492f), new Vector2(224f, 46f), "GALERI", font);
            }

            if (galleryPanel == null)
            {
                galleryPanel = CreateRuntimeGalleryPanel(parent, font);
            }

            if (galleryPanel != null)
            {
                EnsureInteractiveGalleryUi(font);
            }
        }

        private Font ResolveRuntimeFont()
        {
            if (selectedDifficultyText != null && selectedDifficultyText.font != null)
            {
                return selectedDifficultyText.font;
            }

            Text[] texts = Object.FindObjectsByType<Text>(FindObjectsInactive.Include);
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] != null && texts[i].font != null)
                {
                    return texts[i].font;
                }
            }

            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private Button CreateRuntimeMenuButton(Transform parent, string objectName, Vector2 position, Vector2 size, string label, Font font)
        {
            GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.02f, 0.20f, 0.24f, 0.92f);

            Button button = buttonObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.72f, 1f, 1f, 1f);
            colors.pressedColor = new Color(1f, 0.30f, 0.58f, 1f);
            button.colors = colors;

            Text text = CreateRuntimeText("Label", buttonObject.transform, Vector2.zero, size, label, 18, Color.white, font, TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            return button;
        }

        private GameObject CreateRuntimeGalleryPanel(Transform parent, Font font)
        {
            GameObject overlay = new GameObject("Gallery Overlay", typeof(RectTransform));
            overlay.transform.SetParent(parent, false);
            RectTransform overlayRect = overlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image dim = overlay.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.62f);

            GameObject window = new GameObject("Gallery Window", typeof(RectTransform), typeof(Image));
            window.transform.SetParent(overlay.transform, false);
            RectTransform windowRect = window.GetComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.pivot = new Vector2(0.5f, 0.5f);
            windowRect.anchoredPosition = Vector2.zero;
            windowRect.sizeDelta = new Vector2(860f, 560f);
            Image windowImage = window.GetComponent<Image>();
            windowImage.color = new Color(0.01f, 0.07f, 0.09f, 0.96f);

            CreateRuntimeText("Gallery Title", window.transform, new Vector2(0f, 220f), new Vector2(740f, 52f), "GALERI PENGETAHUAN", 34, Color.white, font, TextAnchor.MiddleCenter).fontStyle = FontStyle.Bold;
            CreateRuntimeText("Gallery Hint", window.transform, new Vector2(0f, 178f), new Vector2(720f, 34f), "Materi belajar selalu tersedia. Chest boss membuka catatan bonus sektor berikutnya.", 15, new Color(0.42f, 0.95f, 1f, 1f), font, TextAnchor.MiddleCenter);
            galleryIllustrationImage = CreateRuntimeImage("Gallery Illustration", window.transform, new Vector2(-300f, 88f), new Vector2(188f, 118f), new Color(0.45f, 1f, 1f, 0.32f), CreateRuntimeGalleryIllustrationSprite());
            galleryBodyText = CreateRuntimeText("Gallery Body", window.transform, new Vector2(112f, -18f), new Vector2(560f, 350f), BuildGalleryBodyText(), 14, Color.white, font, TextAnchor.UpperLeft);
            galleryBackButton = CreateRuntimeMenuButton(window.transform, "Gallery Back Button", new Vector2(0f, -218f), new Vector2(240f, 54f), "KEMBALI", font);

            overlay.SetActive(false);
            return overlay;
        }

        private void EnsureInteractiveGalleryUi(Font font)
        {
            if (galleryPanel == null || galleryRoot != null)
            {
                return;
            }

            Transform window = FindGalleryWindow();
            if (window == null)
            {
                window = galleryPanel.transform;
            }

            HideLegacyGalleryContent(window);
            Sprite paperSprite = ResolveGallerySprite(galleryPaperSprite, FantasyTextboxPath, new Vector4(34f, 34f, 34f, 34f));
            Sprite windowSprite = ResolveGallerySprite(galleryWindowSprite, FantasyGalleryPath, new Vector4(36f, 36f, 36f, 36f));

            Image windowImage = window.GetComponent<Image>();
            if (windowImage != null && windowSprite != null)
            {
                windowImage.sprite = windowSprite;
                windowImage.type = Image.Type.Sliced;
                windowImage.color = Color.white;
            }

            galleryRoot = new GameObject("Interactive Gallery Root", typeof(RectTransform));
            galleryRoot.transform.SetParent(window, false);
            RectTransform rootRect = galleryRoot.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.anchoredPosition = Vector2.zero;
            rootRect.sizeDelta = new Vector2(860f, 560f);

            galleryTitleText = CreateRuntimeText("Interactive Gallery Title", galleryRoot.transform, new Vector2(0f, 234f), new Vector2(760f, 46f), "GALERI PENGETAHUAN", 32, Color.white, font, TextAnchor.MiddleCenter);
            galleryTitleText.fontStyle = FontStyle.Bold;
            galleryHintText = CreateRuntimeText("Interactive Gallery Hint", galleryRoot.transform, new Vector2(0f, 196f), new Vector2(760f, 34f), "Pilih chest, buka buku, lalu baca babnya sebelum menghadapi quiz.", 15, new Color(0.42f, 0.95f, 1f, 1f), font, TextAnchor.MiddleCenter);

            galleryChestView = CreateGalleryView("Gallery Chest View", galleryRoot.transform);
            galleryChestContainer = CreateGalleryContainer("Chest Cards", galleryChestView.transform, new Vector2(0f, 18f), new Vector2(800f, 320f));

            galleryBookView = CreateGalleryView("Gallery Book View", galleryRoot.transform);
            galleryBookContainer = CreateGalleryContainer("Book Cards", galleryBookView.transform, new Vector2(0f, 18f), new Vector2(800f, 320f));
            galleryBookBackButton = CreateRuntimeMenuButton(galleryBookView.transform, "Gallery Book Back Button", new Vector2(-282f, -210f), new Vector2(210f, 50f), "KE CHEST", font);
            galleryBookBackButton.onClick.AddListener(() => RunButtonAction(() =>
            {
                galleryViewMode = 0;
                RefreshGalleryView();
            }));

            galleryReaderView = CreateGalleryView("Gallery Reader View", galleryRoot.transform);
            galleryReaderPaperImage = CreateRuntimeImage("Gallery Book Paper", galleryReaderView.transform, new Vector2(0f, -8f), new Vector2(760f, 378f), Color.white, paperSprite != null ? paperSprite : CreateRuntimePaperSprite());
            galleryReaderPaperImage.type = Image.Type.Sliced;
            galleryReaderPaperImage.raycastTarget = false;
            galleryReaderIllustrationImage = CreateRuntimeImage("Gallery Reader Illustration", galleryReaderView.transform, new Vector2(-302f, 36f), new Vector2(126f, 126f), Color.white, CreateRuntimeBookSprite(new Color(0.40f, 0.95f, 1f, 1f), true));
            galleryReaderTitleText = CreateRuntimeText("Gallery Reader Title", galleryReaderView.transform, new Vector2(68f, 116f), new Vector2(540f, 48f), string.Empty, 22, new Color(0.08f, 0.10f, 0.12f, 1f), font, TextAnchor.MiddleLeft);
            galleryReaderTitleText.fontStyle = FontStyle.Bold;
            galleryReaderBodyText = CreateRuntimeText("Gallery Reader Body", galleryReaderView.transform, new Vector2(68f, -18f), new Vector2(540f, 210f), string.Empty, 16, new Color(0.06f, 0.07f, 0.08f, 1f), font, TextAnchor.UpperLeft);
            galleryReaderPageText = CreateRuntimeText("Gallery Reader Page", galleryReaderView.transform, new Vector2(0f, -170f), new Vector2(260f, 28f), string.Empty, 14, new Color(0.10f, 0.12f, 0.14f, 1f), font, TextAnchor.MiddleCenter);
            galleryReaderBackButton = CreateRuntimeMenuButton(galleryReaderView.transform, "Gallery Reader Back Button", new Vector2(-282f, -210f), new Vector2(210f, 50f), "KE BUKU", font);
            galleryPreviousPageButton = CreateRuntimeMenuButton(galleryReaderView.transform, "Gallery Previous Page Button", new Vector2(0f, -210f), new Vector2(190f, 50f), "SEBELUMNYA", font);
            galleryNextPageButton = CreateRuntimeMenuButton(galleryReaderView.transform, "Gallery Next Page Button", new Vector2(238f, -210f), new Vector2(190f, 50f), "BERIKUTNYA", font);

            galleryReaderBackButton.onClick.AddListener(() => RunButtonAction(() =>
            {
                galleryViewMode = 1;
                RefreshGalleryView();
            }));
            galleryPreviousPageButton.onClick.AddListener(() => RunButtonAction(() =>
            {
                selectedGalleryPage = Mathf.Max(0, selectedGalleryPage - 1);
                RefreshGalleryView();
            }));
            galleryNextPageButton.onClick.AddListener(() => RunButtonAction(() =>
            {
                selectedGalleryPage++;
                RefreshGalleryView();
            }));

            RefreshGalleryView();
        }

        private Transform FindGalleryWindow()
        {
            if (galleryPanel == null)
            {
                return null;
            }

            Transform found = galleryPanel.transform.Find("Gallery Window");
            if (found != null)
            {
                return found;
            }

            Image[] images = galleryPanel.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] != null && images[i].name == "Gallery Window")
                {
                    return images[i].transform;
                }
            }

            return galleryPanel.transform;
        }

        private void HideLegacyGalleryContent(Transform window)
        {
            if (galleryBodyText != null)
            {
                galleryBodyText.gameObject.SetActive(false);
            }

            if (galleryIllustrationImage != null)
            {
                galleryIllustrationImage.gameObject.SetActive(false);
            }

            HideDirectChild(window, "Gallery Title");
            HideDirectChild(window, "Gallery Hint");
        }

        private static void HideDirectChild(Transform parent, string childName)
        {
            if (parent == null)
            {
                return;
            }

            Transform child = parent.Find(childName);
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }
        }

        private GameObject CreateGalleryView(string objectName, Transform parent)
        {
            GameObject view = new GameObject(objectName, typeof(RectTransform));
            view.transform.SetParent(parent, false);
            RectTransform rect = view.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(820f, 452f);
            return view;
        }

        private RectTransform CreateGalleryContainer(string objectName, Transform parent, Vector2 position, Vector2 size)
        {
            GameObject container = new GameObject(objectName, typeof(RectTransform));
            container.transform.SetParent(parent, false);
            RectTransform rect = container.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private Image CreateRuntimeImage(string objectName, Transform parent, Vector2 position, Vector2 size, Color color, Sprite sprite)
        {
            GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
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
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private Text CreateRuntimeText(string objectName, Transform parent, Vector2 position, Vector2 size, string value, int fontSize, Color color, Font font, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.font = font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Sprite CreateRuntimeGalleryIllustrationSprite()
        {
            const int width = 160;
            const int height = 100;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color clear = new Color(0f, 0f, 0f, 0f);
            Color cyan = new Color(0.20f, 1f, 1f, 0.82f);
            Color magenta = new Color(1f, 0.18f, 0.50f, 0.78f);
            Color dark = new Color(0.02f, 0.10f, 0.13f, 0.92f);
            Color grid = new Color(0.12f, 0.55f, 0.62f, 0.34f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool border = x < 3 || x > width - 4 || y < 3 || y > height - 4;
                    bool gridLine = x % 16 == 0 || y % 14 == 0;
                    Color color = border ? cyan : (gridLine ? grid : dark);
                    texture.SetPixel(x, y, color);
                }
            }

            DrawRect(texture, 24, 24, 48, 34, new Color(0.05f, 0.22f, 0.27f, 1f));
            DrawRect(texture, 28, 28, 40, 26, new Color(0.08f, 0.42f, 0.48f, 1f));
            DrawLine(texture, 76, 41, 112, 66, cyan);
            DrawLine(texture, 76, 41, 112, 28, cyan);
            DrawRect(texture, 110, 58, 20, 16, magenta);
            DrawRect(texture, 112, 22, 22, 18, cyan);
            DrawRect(texture, 48, 62, 62, 5, magenta);
            DrawRect(texture, 48, 72, 78, 4, cyan);
            DrawRect(texture, 48, 82, 52, 4, new Color(1f, 0.86f, 0.25f, 0.9f));

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 64f);
        }

        private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    if (px >= 0 && py >= 0 && px < texture.width && py < texture.height)
                    {
                        texture.SetPixel(px, py, color);
                    }
                }
            }
        }

        private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Mathf.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;
            int x = x0;
            int y = y0;
            while (true)
            {
                DrawRect(texture, x - 1, y - 1, 3, 3, color);
                if (x == x1 && y == y1)
                {
                    break;
                }

                int e2 = 2 * error;
                if (e2 >= dy)
                {
                    error += dy;
                    x += sx;
                }

                if (e2 <= dx)
                {
                    error += dx;
                    y += sy;
                }
            }
        }

        private void RefreshGalleryView()
        {
            if (galleryPanel == null)
            {
                return;
            }

            if (galleryRoot == null)
            {
                EnsureInteractiveGalleryUi(ResolveRuntimeFont());
            }

            if (galleryRoot == null)
            {
                return;
            }

            Font font = ResolveRuntimeFont();
            GalleryChest[] chests = BuildGalleryChests();
            selectedGalleryChest = Mathf.Clamp(selectedGalleryChest, 0, chests.Length - 1);
            selectedGalleryBook = Mathf.Max(0, selectedGalleryBook);
            selectedGalleryPage = Mathf.Max(0, selectedGalleryPage);

            if (galleryViewMode <= 0)
            {
                ShowGalleryChestView(chests, font);
                return;
            }

            GalleryChest chest = chests[selectedGalleryChest];
            if (!IsGalleryChestUnlocked(chest))
            {
                galleryViewMode = 0;
                ShowGalleryChestView(chests, font);
                return;
            }

            selectedGalleryBook = Mathf.Clamp(selectedGalleryBook, 0, chest.Books.Length - 1);
            if (galleryViewMode == 1)
            {
                ShowGalleryBookView(chest, font);
                return;
            }

            GalleryBook book = chest.Books[selectedGalleryBook];
            selectedGalleryPage = Mathf.Clamp(selectedGalleryPage, 0, book.Pages.Length - 1);
            ShowGalleryReaderView(chest, book);
        }

        private void ShowGalleryChestView(GalleryChest[] chests, Font font)
        {
            galleryChestView.SetActive(true);
            galleryBookView.SetActive(false);
            galleryReaderView.SetActive(false);
            ClearGalleryContainer(galleryChestContainer);

            if (galleryTitleText != null)
            {
                galleryTitleText.text = "GALERI CHEST PENGETAHUAN";
            }

            if (galleryHintText != null)
            {
                galleryHintText.text = "Chest awal selalu tersedia. Chest boss membuka buku persiapan untuk sektor berikutnya.";
            }

            float[] xPositions = { -300f, -100f, 100f, 300f };
            for (int i = 0; i < chests.Length; i++)
            {
                GalleryChest chest = chests[i];
                bool unlocked = IsGalleryChestUnlocked(chest);
                int captured = i;
                string footer = unlocked ? "BUKA CHEST" : chest.LockedHint;
                Button button = CreateGalleryOptionButton(
                    galleryChestContainer,
                    "Knowledge Chest " + (i + 1).ToString("0"),
                    new Vector2(xPositions[Mathf.Clamp(i, 0, xPositions.Length - 1)], 4f),
                    new Vector2(184f, 250f),
                    chest.Title,
                    chest.Subtitle,
                    footer,
                    CreateRuntimeChestSprite(unlocked),
                    unlocked ? new Color(0.45f, 1f, 1f, 1f) : new Color(0.42f, 0.46f, 0.50f, 1f),
                    unlocked,
                    font,
                    () =>
                    {
                        selectedGalleryChest = captured;
                        selectedGalleryBook = 0;
                        selectedGalleryPage = 0;
                        galleryViewMode = 1;
                        RefreshGalleryView();
                    });
                galleryDynamicButtons.Add(button);
            }
        }

        private void ShowGalleryBookView(GalleryChest chest, Font font)
        {
            galleryChestView.SetActive(false);
            galleryBookView.SetActive(true);
            galleryReaderView.SetActive(false);
            ClearGalleryContainer(galleryBookContainer);

            if (galleryTitleText != null)
            {
                galleryTitleText.text = chest.Title.ToUpperInvariant();
            }

            if (galleryHintText != null)
            {
                galleryHintText.text = "Pilih buku, lalu balik halamannya sampai bab selesai.";
            }

            Vector2[] positions =
            {
                new Vector2(-204f, 82f),
                new Vector2(204f, 82f),
                new Vector2(-204f, -100f),
                new Vector2(204f, -100f)
            };

            for (int i = 0; i < chest.Books.Length; i++)
            {
                GalleryBook book = chest.Books[i];
                int captured = i;
                Button button = CreateGalleryOptionButton(
                    galleryBookContainer,
                    "Knowledge Book " + (i + 1).ToString("0"),
                    positions[Mathf.Clamp(i, 0, positions.Length - 1)],
                    new Vector2(348f, 150f),
                    book.Title,
                    book.Subtitle,
                    book.Pages.Length.ToString("0") + " BAB",
                    CreateRuntimeBookSprite(book.AccentColor, true),
                    book.AccentColor,
                    true,
                    font,
                    () =>
                    {
                        selectedGalleryBook = captured;
                        selectedGalleryPage = 0;
                        galleryViewMode = 2;
                        RefreshGalleryView();
                    });
                galleryDynamicButtons.Add(button);
            }
        }

        private void ShowGalleryReaderView(GalleryChest chest, GalleryBook book)
        {
            galleryChestView.SetActive(false);
            galleryBookView.SetActive(false);
            galleryReaderView.SetActive(true);

            GalleryPage page = book.Pages[selectedGalleryPage];
            if (galleryTitleText != null)
            {
                galleryTitleText.text = book.Title.ToUpperInvariant();
            }

            if (galleryHintText != null)
            {
                galleryHintText.text = chest.Subtitle;
            }

            if (galleryReaderTitleText != null)
            {
                galleryReaderTitleText.text = page.Title;
            }

            if (galleryReaderBodyText != null)
            {
                galleryReaderBodyText.text = page.Body;
            }

            if (galleryReaderPageText != null)
            {
                galleryReaderPageText.text = "HALAMAN " + (selectedGalleryPage + 1).ToString("0") + " / " + book.Pages.Length.ToString("0");
            }

            if (galleryReaderIllustrationImage != null)
            {
                galleryReaderIllustrationImage.sprite = CreateRuntimeBookSprite(book.AccentColor, true);
                galleryReaderIllustrationImage.color = Color.white;
            }

            if (galleryPreviousPageButton != null)
            {
                galleryPreviousPageButton.interactable = selectedGalleryPage > 0;
            }

            if (galleryNextPageButton != null)
            {
                galleryNextPageButton.interactable = selectedGalleryPage < book.Pages.Length - 1;
            }
        }

        private Button CreateGalleryOptionButton(Transform parent, string objectName, Vector2 position, Vector2 size, string title, string subtitle, string footer, Sprite icon, Color accent, bool interactable, Font font, UnityEngine.Events.UnityAction action)
        {
            GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = buttonObject.GetComponent<Image>();
            image.sprite = size.y <= 170f
                ? ResolveGallerySprite(galleryBookButtonSprite, FantasyGalleryButtonPath, new Vector4(24f, 24f, 24f, 24f))
                : ResolveGallerySprite(galleryChoiceSprite, FantasyChoicePath, new Vector4(24f, 24f, 24f, 24f));
            image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            image.color = interactable ? Color.white : new Color(0.35f, 0.36f, 0.38f, 0.82f);

            Button button = buttonObject.GetComponent<Button>();
            button.interactable = interactable;
            if (action != null)
            {
                button.onClick.AddListener(() => RunButtonAction(() => action.Invoke()));
            }

            Image iconImage = CreateRuntimeImage("Icon", buttonObject.transform, new Vector2(0f, size.y * 0.23f), new Vector2(Mathf.Min(92f, size.x * 0.45f), Mathf.Min(92f, size.y * 0.36f)), Color.white, icon);
            iconImage.raycastTarget = false;
            CreateRuntimeText("Title", buttonObject.transform, new Vector2(0f, size.y * -0.08f), new Vector2(size.x - 26f, 48f), title, 15, interactable ? Color.white : new Color(0.68f, 0.70f, 0.72f, 1f), font, TextAnchor.MiddleCenter).fontStyle = FontStyle.Bold;
            CreateRuntimeText("Subtitle", buttonObject.transform, new Vector2(0f, size.y * -0.29f), new Vector2(size.x - 26f, 50f), subtitle, 12, interactable ? new Color(0.78f, 0.96f, 1f, 1f) : new Color(0.56f, 0.58f, 0.60f, 1f), font, TextAnchor.MiddleCenter);
            CreateRuntimeText("Footer", buttonObject.transform, new Vector2(0f, size.y * -0.43f), new Vector2(size.x - 28f, 28f), footer, 11, interactable ? accent : new Color(0.72f, 0.44f, 0.52f, 1f), font, TextAnchor.MiddleCenter).fontStyle = FontStyle.Bold;
            return button;
        }

        private void ClearGalleryContainer(RectTransform container)
        {
            if (container == null)
            {
                return;
            }

            for (int i = container.childCount - 1; i >= 0; i--)
            {
                Destroy(container.GetChild(i).gameObject);
            }

            galleryDynamicButtons.Clear();
        }

        private static bool IsGalleryChestUnlocked(GalleryChest chest)
        {
            if (chest == null || chest.AlwaysAvailable)
            {
                return true;
            }

            return PlayerPrefs.GetInt(KnowledgeUnlockPrefix + chest.UnlockTier.ToString("0"), 0) == 1;
        }

        private static int GetAvailableGalleryChestCount()
        {
            GalleryChest[] chests = BuildGalleryChests();
            int count = 0;
            for (int i = 0; i < chests.Length; i++)
            {
                if (IsGalleryChestUnlocked(chests[i]))
                {
                    count++;
                }
            }

            return count;
        }

        private static GalleryChest[] BuildGalleryChests()
        {
            return new GalleryChest[]
            {
                new GalleryChest(0, "Chest Awal", "Persiapan Sektor 1: keamanan gadget pribadi", "BAWAAN", true, BuildSectorOneBooks()),
                new GalleryChest(1, "Chest Boss Sektor 1", "Persiapan Sektor 2: pertahanan berlapis", "KALAHKAN BOSS 1", false, BuildSectorTwoBooks()),
                new GalleryChest(2, "Chest Boss Sektor 2", "Persiapan Sektor 3: respons insiden", "KALAHKAN BOSS 2", false, BuildSectorThreeBooks()),
                new GalleryChest(3, "Chest Boss Final", "Arsip akhir: kebiasaan guardian setelah menang", "KALAHKAN BOSS 3", false, BuildFinalArchiveBooks())
            };
        }

        private static GalleryBook[] BuildSectorOneBooks()
        {
            return new GalleryBook[]
            {
                new GalleryBook("Kenapa Gadget Harus Aman", "Dasar ancaman pada ponsel, laptop, dan akun pribadi.", new Color(0.45f, 1f, 1f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Gadget Adalah Markas Digital", "Gadget menyimpan akun, foto, pesan, lokasi, catatan sekolah, dompet digital, dan akses ke media sosial. Jika gadget dikuasai penyerang, penyerang bisa membaca data, menipu kontak, mengambil akun, atau memasang aplikasi mata-mata. Karena itu gadget harus diperlakukan seperti markas utama Cyber Guardian: dikunci, diperbarui, dan dijaga dari aplikasi asing."),
                    Page("Bab 2 - Data Yang Perlu Dilindungi", "Data penting bukan hanya nomor kartu atau password. Nama lengkap, alamat, foto, nomor telepon, daftar kontak, riwayat chat, dan lokasi juga bisa dipakai untuk social engineering. Dalam quiz sektor 1, ingat prinsip ini: data pribadi kecil bisa menjadi potongan puzzle besar untuk menyerang akun."),
                    Page("Bab 3 - Kebiasaan Aman Harian", "Gunakan kunci layar, jangan meminjamkan akun, matikan instalasi dari sumber tidak dikenal, update aplikasi, dan periksa izin aplikasi. Jika aplikasi kalkulator meminta akses kontak dan lokasi tanpa alasan jelas, itu tanda bahaya. Kebiasaan kecil yang konsisten membuat shield Cyber Guardian semakin kuat.")
                }),
                new GalleryBook("Password, PIN, dan MFA", "Cara membuat kunci akun yang sulit ditembus.", new Color(1f, 0.36f, 0.62f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Password Unik", "Password yang sama di banyak akun berbahaya. Jika satu situs bocor, akun lain bisa ikut dibuka. Gunakan password unik untuk email, game, media sosial, dan belajar. Password kuat biasanya panjang, tidak memakai tanggal lahir, dan tidak mudah ditebak dari profil."),
                    Page("Bab 2 - Password Manager", "Password manager membantu menyimpan banyak password unik. Kamu cukup mengingat satu master password yang kuat. Jangan menyimpan password di chat, catatan publik, atau foto layar. Jika harus menulis cadangan, simpan di tempat fisik yang aman dan jangan dibagikan."),
                    Page("Bab 3 - MFA", "MFA atau autentikasi dua faktor menambah lapisan kedua, misalnya kode aplikasi authenticator. OTP tidak boleh diberikan kepada siapa pun, termasuk orang yang mengaku admin. Dalam game, jawaban benar tentang MFA berarti kamu membuat jalur aman yang lebih kuat.")
                }),
                new GalleryBook("Phishing dan Link Palsu", "Mengenali jebakan pesan, hadiah, dan halaman login palsu.", new Color(1f, 0.82f, 0.28f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Ciri Phishing", "Phishing sering memakai rasa panik atau hadiah: akun akan ditutup, paket tertahan, voucher besar, atau diminta login ulang segera. Periksa alamat pengirim, domain link, ejaan aneh, dan permintaan data rahasia. Jangan klik link hanya karena tampilannya mirip resmi."),
                    Page("Bab 2 - Cara Memeriksa Link", "Arahkan kursor atau tekan lama untuk melihat alamat asli. Domain resmi biasanya jelas dan konsisten. Contoh: keamanan-bank.example berbeda dari bank.example. Jika ragu, buka aplikasi atau situs resmi secara manual, bukan dari link pesan."),
                    Page("Bab 3 - Apa Yang Harus Dilakukan", "Jika terlanjur memasukkan password, segera ganti password dari perangkat aman, aktifkan MFA, logout semua sesi, dan laporkan. Jangan menunggu sampai akun dipakai penyerang. Dalam quiz, pilih tindakan yang mengurangi kerusakan paling cepat.")
                }),
                new GalleryBook("Virus dan Aplikasi Berbahaya", "Mencegah malware masuk dari file, aplikasi, dan lampiran.", new Color(0.62f, 1f, 0.42f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Jalan Masuk Malware", "Malware bisa masuk lewat aplikasi bajakan, cheat game, lampiran email, file installer palsu, atau ekstensi browser mencurigakan. Malware tidak selalu terlihat menyeramkan; kadang menyamar sebagai file biasa."),
                    Page("Bab 2 - Tanda Perangkat Terinfeksi", "Tanda umum: perangkat lambat mendadak, iklan muncul terus, baterai cepat habis, aplikasi asing muncul, browser membuka halaman sendiri, atau antivirus memberi peringatan. Satu tanda belum pasti, tetapi beberapa tanda sekaligus perlu diperiksa."),
                    Page("Bab 3 - Pencegahan", "Unduh dari sumber resmi, update sistem, pakai antivirus bawaan, dan jangan menjalankan file dari orang tidak dikenal. Hapus aplikasi yang tidak dipakai. Setiap aplikasi yang tidak perlu adalah pintu tambahan yang harus dijaga.")
                })
            };
        }

        private static GalleryBook[] BuildSectorTwoBooks()
        {
            return new GalleryBook[]
            {
                new GalleryBook("Pertahanan Berlapis", "Jika satu perlindungan gagal, lapisan lain masih menahan serangan.", new Color(0.45f, 1f, 1f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Konsep Layered Defense", "Pertahanan berlapis berarti tidak bergantung pada satu alat. Password kuat, MFA, update, firewall, backup, izin terbatas, dan edukasi pengguna bekerja bersama. Dalam sektor 2, pertanyaan quiz sering menilai urutan perlindungan yang paling aman."),
                    Page("Bab 2 - Least Privilege", "Least privilege berarti akun dan aplikasi hanya mendapat izin yang dibutuhkan. Jangan memakai akun admin untuk kegiatan harian. Jika malware masuk lewat akun biasa, dampaknya lebih kecil dibanding masuk lewat akun admin."),
                    Page("Bab 3 - Patch dan Update", "Update menutup celah keamanan yang sudah diketahui. Menunda update sama seperti membiarkan pintu rusak tetap terbuka. Prioritaskan update sistem operasi, browser, antivirus, dan aplikasi yang sering terhubung internet.")
                }),
                new GalleryBook("Backup dan Pemulihan", "Melindungi data saat perangkat rusak, hilang, atau terkena ransomware.", new Color(1f, 0.36f, 0.62f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Aturan 3-2-1", "Aturan 3-2-1: punya 3 salinan data, di 2 jenis media berbeda, dan 1 salinan berada di lokasi terpisah atau cloud tepercaya. Backup yang baik harus bisa dipulihkan, bukan hanya dibuat."),
                    Page("Bab 2 - Backup Offline", "Ransomware bisa ikut mengenkripsi drive yang selalu terhubung. Karena itu backup offline atau cloud dengan versioning penting. Setelah backup selesai, lepas drive eksternal dari perangkat."),
                    Page("Bab 3 - Uji Restore", "Backup yang belum pernah diuji bisa gagal saat dibutuhkan. Coba pulihkan beberapa file contoh secara berkala. Dalam quiz, jawaban terbaik biasanya mencakup membuat backup dan menguji pemulihan.")
                }),
                new GalleryBook("Jaringan Aman", "Wi-Fi, firewall, dan kebiasaan aman saat online.", new Color(1f, 0.82f, 0.28f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Wi-Fi Publik", "Wi-Fi publik bisa disadap atau dipalsukan. Hindari login akun penting di jaringan publik tanpa perlindungan. Jika harus memakai Wi-Fi publik, gunakan situs HTTPS dan jangan menyetujui peringatan sertifikat aneh."),
                    Page("Bab 2 - Firewall", "Firewall menyaring koneksi masuk dan keluar. Jika aplikasi asing meminta izin melewati firewall, periksa dulu fungsinya. Firewall bukan pengganti update atau antivirus, tetapi bagian dari pertahanan berlapis."),
                    Page("Bab 3 - Router Rumah", "Ganti password admin router, gunakan WPA2/WPA3, matikan WPS jika tidak perlu, dan update firmware router. Banyak serangan dimulai dari perangkat jaringan yang dibiarkan memakai password bawaan.")
                }),
                new GalleryBook("Alert dan Log", "Membaca tanda bahaya sebelum kerusakan membesar.", new Color(0.62f, 1f, 0.42f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Jangan Abaikan Alert", "Alert antivirus, browser, atau sistem adalah sinyal. Jangan langsung tekan allow hanya agar pekerjaan cepat selesai. Baca pesan, lihat nama file atau aplikasi, lalu putuskan berdasarkan sumbernya."),
                    Page("Bab 2 - Log Sederhana", "Log adalah catatan aktivitas sistem. Untuk pemain, log bisa dibayangkan sebagai jejak virus di dunia komputer. Waktu kejadian, nama aplikasi, dan alamat koneksi membantu mencari sumber masalah."),
                    Page("Bab 3 - Eskalasi", "Jika menemukan alert berulang, akun login dari lokasi asing, atau file penting berubah, segera laporkan ke orang yang bertanggung jawab. Kecepatan melapor bisa mencegah satu perangkat menular ke perangkat lain.")
                })
            };
        }

        private static GalleryBook[] BuildSectorThreeBooks()
        {
            return new GalleryBook[]
            {
                new GalleryBook("Respons Insiden", "Langkah darurat saat serangan sudah terjadi.", new Color(0.45f, 1f, 1f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Tenang dan Isolasi", "Saat perangkat diduga terinfeksi, jangan panik. Putuskan internet atau jaringan jika serangan masih aktif. Isolasi mencegah malware mengirim data atau menyebar ke perangkat lain."),
                    Page("Bab 2 - Simpan Bukti", "Jangan langsung menghapus semua bukti. Catat waktu kejadian, pesan error, nama file, akun yang terdampak, dan screenshot peringatan. Bukti membantu menentukan serangan berasal dari mana."),
                    Page("Bab 3 - Urutan Aman", "Urutan dasar: isolasi, identifikasi, bersihkan, pulihkan, lalu perkuat. Jangan memulihkan backup ke sistem yang masih terinfeksi. Pastikan sumber serangan ditutup sebelum kembali online.")
                }),
                new GalleryBook("Ransomware", "Menghadapi malware yang mengunci atau mengenkripsi data.", new Color(1f, 0.36f, 0.62f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Apa Itu Ransomware", "Ransomware mengenkripsi file lalu meminta tebusan. Membayar tidak menjamin data kembali. Pencegahan terbaik adalah backup bersih, update rutin, dan tidak membuka file mencurigakan."),
                    Page("Bab 2 - Saat Terkena", "Putuskan jaringan, jangan restart berkali-kali tanpa arahan, dokumentasikan pesan tebusan, dan cari bantuan. Periksa apakah ada decryptor resmi dari sumber keamanan tepercaya."),
                    Page("Bab 3 - Recovery", "Pulihkan dari backup yang dibuat sebelum infeksi. Ganti password dari perangkat aman, cek akun yang tersambung, dan patch celah yang dipakai penyerang. Setelah recovery, lakukan evaluasi agar tidak terulang.")
                }),
                new GalleryBook("Investigasi Jejak Virus", "Mencari sumber serangan dari gejala, log, dan perubahan sistem.", new Color(1f, 0.82f, 0.28f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Timeline", "Buat garis waktu: kapan file dibuka, kapan alert muncul, kapan akun login asing terlihat, dan aplikasi apa yang baru dipasang. Timeline membuat investigasi tidak menebak-nebak."),
                    Page("Bab 2 - Indikator Kompromi", "Indikator kompromi bisa berupa nama file asing, alamat server mencurigakan, proses berjalan aneh, atau perubahan pengaturan keamanan. Catat indikator ini untuk memblokir serangan serupa."),
                    Page("Bab 3 - Validasi", "Jangan menyimpulkan dari satu tanda saja. Gabungkan bukti dari alert, log, perilaku perangkat, dan sumber file. Dalam quiz tingkat lanjut, pilih jawaban yang mengumpulkan bukti sebelum mengambil tindakan besar.")
                }),
                new GalleryBook("Pemulihan dan Pencegahan Ulang", "Menutup celah setelah boss virus dikalahkan.", new Color(0.62f, 1f, 0.42f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Hardening", "Hardening berarti memperkuat sistem setelah insiden: update, matikan layanan tidak perlu, hapus aplikasi asing, batasi izin admin, dan aktifkan perlindungan tambahan."),
                    Page("Bab 2 - Rotasi Password", "Jika akun mungkin bocor, ganti password dari perangkat bersih. Mulai dari email utama, karena email sering dipakai untuk reset akun lain. Aktifkan MFA setelah password diganti."),
                    Page("Bab 3 - Pelajaran Setelah Insiden", "Setiap insiden harus menghasilkan pelajaran: apa pintu masuknya, alert apa yang terlewat, backup apakah berhasil, dan kebiasaan apa yang perlu diubah. Guardian yang kuat belajar dari serangan.")
                })
            };
        }

        private static GalleryBook[] BuildFinalArchiveBooks()
        {
            return new GalleryBook[]
            {
                new GalleryBook("Kode Etik Cyber Guardian", "Kemenangan bukan akhir, tetapi awal kebiasaan aman.", new Color(0.45f, 1f, 1f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Jaga Diri dan Orang Lain", "Keamanan digital bukan hanya untuk diri sendiri. Jangan menyebar link mencurigakan, jangan membagikan data teman, dan bantu orang lain mengenali penipuan dengan bahasa yang jelas."),
                    Page("Bab 2 - Belajar Berkala", "Ancaman berubah. Hari ini phishing, besok deepfake, lusa aplikasi palsu. Cyber Guardian harus terus memperbarui pengetahuan seperti sistem yang rutin patch."),
                    Page("Bab 3 - Checklist Akhir", "Checklist aman: password unik, MFA aktif, backup diuji, perangkat update, izin aplikasi dirapikan, dan kebiasaan cek link berjalan. Jika semua ini dilakukan, shield tetap kuat setelah game selesai.")
                }),
                new GalleryBook("Peta Belajar Lanjutan", "Materi berikutnya setelah versi pertama game.", new Color(1f, 0.36f, 0.62f, 1f), new GalleryPage[]
                {
                    Page("Bab 1 - Privasi Digital", "Pelajari pengaturan privasi, jejak digital, izin kamera/mikrofon, dan cara membatasi data yang dibagikan aplikasi. Privasi adalah bagian dari keamanan."),
                    Page("Bab 2 - Keamanan Jaringan", "Pelajari DNS aman, VPN yang benar, keamanan router, segmentasi jaringan, dan cara mengenali perangkat asing di jaringan rumah."),
                    Page("Bab 3 - Dasar Forensik", "Pelajari cara membaca log, hash file, sandbox dasar, dan cara menyimpan bukti tanpa merusaknya. Ini bisa menjadi fitur edukasi lanjutan di update game berikutnya.")
                })
            };
        }

        private static GalleryPage Page(string title, string body)
        {
            return new GalleryPage(title, body);
        }

        private static Sprite ResolveGallerySprite(Sprite assigned, string assetPath, Vector4 border)
        {
            if (assigned != null)
            {
                return assigned;
            }

#if UNITY_EDITOR
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                return sprite;
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture != null)
            {
                return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
            }
#endif

            return null;
        }

        private static Sprite CreateRuntimePaperSprite()
        {
            const int width = 192;
            const int height = 112;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color paper = new Color(0.93f, 0.86f, 0.70f, 1f);
            Color edge = new Color(0.48f, 0.35f, 0.20f, 1f);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool border = x < 5 || x >= width - 5 || y < 5 || y >= height - 5;
                    texture.SetPixel(x, y, border ? edge : paper);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                DrawRect(texture, 28, 22 + i * 9, 132, 2, new Color(0.36f, 0.25f, 0.16f, 0.22f));
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(8f, 8f, 8f, 8f));
        }

        private static Sprite CreateRuntimeChestSprite(bool open)
        {
            const int size = 96;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color clear = new Color(0f, 0f, 0f, 0f);
            Color wood = open ? new Color(0.74f, 0.46f, 0.18f, 1f) : new Color(0.40f, 0.28f, 0.18f, 1f);
            Color metal = open ? new Color(0.94f, 0.76f, 0.30f, 1f) : new Color(0.45f, 0.48f, 0.50f, 1f);
            Color glow = open ? new Color(0.26f, 1f, 1f, 0.90f) : new Color(0.10f, 0.16f, 0.18f, 0.75f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, clear);
                }
            }

            DrawRect(texture, 18, 34, 60, 36, wood);
            DrawRect(texture, 14, 30, 68, 8, metal);
            DrawRect(texture, 22, 22, 52, 14, open ? glow : wood);
            DrawRect(texture, 28, 44, 10, 24, metal);
            DrawRect(texture, 58, 44, 10, 24, metal);
            DrawRect(texture, 42, 48, 12, 12, glow);
            DrawLine(texture, 20, 70, 76, 70, metal);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 64f);
        }

        private static Sprite CreateRuntimeBookSprite(Color accent, bool open)
        {
            const int width = 112;
            const int height = 96;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color clear = new Color(0f, 0f, 0f, 0f);
            Color page = new Color(0.92f, 0.86f, 0.72f, 1f);
            Color dark = new Color(0.12f, 0.08f, 0.06f, 1f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, clear);
                }
            }

            if (open)
            {
                DrawRect(texture, 12, 20, 42, 58, page);
                DrawRect(texture, 58, 20, 42, 58, page);
                DrawRect(texture, 52, 18, 8, 62, dark);
                DrawRect(texture, 14, 18, 38, 6, accent);
                DrawRect(texture, 60, 18, 38, 6, accent);
                for (int i = 0; i < 5; i++)
                {
                    DrawRect(texture, 20, 34 + i * 8, 24, 2, new Color(0.16f, 0.12f, 0.09f, 0.32f));
                    DrawRect(texture, 68, 34 + i * 8, 24, 2, new Color(0.16f, 0.12f, 0.09f, 0.32f));
                }
            }
            else
            {
                DrawRect(texture, 26, 18, 58, 64, accent);
                DrawRect(texture, 32, 24, 46, 52, new Color(accent.r * 0.55f, accent.g * 0.55f, accent.b * 0.55f, 1f));
                DrawRect(texture, 34, 30, 40, 8, page);
                DrawRect(texture, 46, 46, 16, 16, page);
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 64f);
        }

        private void Refresh()
        {
            if (selectedDifficultyText != null)
            {
                selectedDifficultyText.text = "KESULITAN: " + difficultyNames[selectedDifficulty].ToUpperInvariant();
                if (IsDeveloperModeEnabled())
                {
                    selectedDifficultyText.text += "  |  MODE DEVELOPER AKTIF";
                }

                if (PlayerPrefs.GetInt(CampaignCompleteKey, 0) == 1)
                {
                    selectedDifficultyText.text += "  |  KAMPANYE SELESAI";
                }
            }

            if (continueButton != null)
            {
                continueButton.interactable = HasSavedProgress() && !startingGame;
            }

            SetButtonLabel(developerButton, IsDeveloperModeEnabled() ? "DEV: AKTIF" : "MODE DEV");
            SetButtonLabel(galleryButton, "GALERI (" + GetAvailableGalleryChestCount().ToString("0") + ")");
            NormalizeCornerUtilityButtons();
            SetButtonLabel(settingsMusicToggleButton, "MUSIK: " + GetMusicVolumePercent().ToString("0") + "%");
            SetButtonLabel(settingsSfxToggleButton, "SFX: " + GetSfxVolumePercent().ToString("0") + "%");
            SetButtonLabel(settingsControlSchemeButton, "KONTROL: " + GetControlSchemeName().ToUpperInvariant());

            if (settingsMusicText != null)
            {
                settingsMusicText.text = GetMusicVolumePercent() > 0
                    ? "Volume musik saat ini " + GetMusicVolumePercent().ToString("0") + "%. Tekan untuk mengubah."
                    : "Musik dimatikan. Tekan untuk mengaktifkan kembali.";
            }

            if (settingsSfxText != null)
            {
                settingsSfxText.text = GetSfxVolumePercent() > 0
                    ? "Volume efek suara saat ini " + GetSfxVolumePercent().ToString("0") + "%. Tekan untuk mengubah."
                    : "Efek suara dimatikan. Tekan untuk mengaktifkan kembali.";
            }

            if (settingsControlText != null)
            {
                settingsControlText.text = GetControlSchemeHint();
            }

            RefreshGalleryView();

            if (developerDisableButton != null)
            {
                developerDisableButton.gameObject.SetActive(IsDeveloperModeEnabled());
            }

            if (difficultyHighlights == null)
            {
                return;
            }

            for (int i = 0; i < difficultyHighlights.Length; i++)
            {
                if (difficultyHighlights[i] == null)
                {
                    continue;
                }

                difficultyHighlights[i].color = i == selectedDifficulty
                    ? new Color(0.12f, 0.92f, 1f, 1f)
                    : new Color(0.03f, 0.12f, 0.15f, 0.96f);
            }
        }

        public static bool HasSavedProgress()
        {
            return PlayerPrefs.GetInt(SaveExistsKey, 0) == 1 && !string.IsNullOrEmpty(PlayerPrefs.GetString(SaveSceneKey, string.Empty));
        }

        public static void UnlockKnowledgeItem(int tier, string title, string body)
        {
            int index = Mathf.Clamp(tier, 1, 3);
            PlayerPrefs.SetInt(KnowledgeUnlockPrefix + index.ToString("0"), 1);
            PlayerPrefs.SetString(KnowledgeTitlePrefix + index.ToString("0"), string.IsNullOrEmpty(title) ? GetDefaultKnowledgeTitle(index) : title);
            PlayerPrefs.SetString(KnowledgeBodyPrefix + index.ToString("0"), string.IsNullOrEmpty(body) ? GetDefaultKnowledgeBody(index) : body);
            PlayerPrefs.Save();
        }

        public static int GetUnlockedKnowledgeCount()
        {
            int count = 0;
            for (int i = 1; i <= 3; i++)
            {
                if (PlayerPrefs.GetInt(KnowledgeUnlockPrefix + i.ToString("0"), 0) == 1)
                {
                    count++;
                }
            }

            return count;
        }

        private static string BuildGalleryBodyText()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("MATERI SEKTOR 1 - DASAR KEAMANAN GADGET");
            builder.AppendLine("Password harus unik, panjang, dan tidak dipakai ulang. Aktifkan MFA agar pencuri password tetap sulit masuk. Update OS/aplikasi menutup celah yang sering dipakai malware.");
            builder.AppendLine("Hindari phishing: cek alamat pengirim, domain link, ejaan aneh, dan permintaan OTP/password. Jangan membuka lampiran asing sebelum sumbernya jelas.");
            builder.AppendLine();
            builder.AppendLine("MATERI SEKTOR 2 - PERTAHANAN BERLAPIS");
            builder.AppendLine("Chest sektor 1 menyiapkan sektor 2: backup, firewall, least privilege, patch rutin, dan pemantauan log. Jika satu lapisan gagal, lapisan lain masih menahan serangan.");
            builder.AppendLine("Data penting perlu backup offline/cloud tepercaya. Hak admin dipakai seperlunya. Alert antivirus/firewall harus dibaca, bukan diabaikan.");
            builder.AppendLine();
            builder.AppendLine("MATERI SEKTOR 3 - RESPONS INSIDEN");
            builder.AppendLine("Chest sektor 2 menyiapkan sektor 3: isolasi perangkat, putus koneksi berbahaya, simpan bukti log, pulihkan dari backup bersih, lalu ganti password dari perangkat aman.");
            builder.AppendLine("Untuk ransomware atau malware besar, jangan panik dan jangan asal membayar. Prioritaskan containment, identifikasi sumber, recovery, dan pencegahan ulang.");
            builder.AppendLine();
            builder.AppendLine("CATATAN CHEST");
            for (int i = 1; i <= 3; i++)
            {
                bool unlocked = PlayerPrefs.GetInt(KnowledgeUnlockPrefix + i.ToString("0"), 0) == 1;
                string title = PlayerPrefs.GetString(KnowledgeTitlePrefix + i.ToString("0"), GetDefaultKnowledgeTitle(i));
                string body = PlayerPrefs.GetString(KnowledgeBodyPrefix + i.ToString("0"), GetDefaultKnowledgeBody(i));
                builder.Append("Chest sektor ");
                builder.Append(i.ToString("0"));
                builder.Append(unlocked ? " terbuka: " : " belum ditemukan: ");
                builder.AppendLine(title);
                if (unlocked)
                {
                    builder.AppendLine(body);
                }
            }

            return builder.ToString();
        }

        private static string GetDefaultKnowledgeTitle(int tier)
        {
            switch (Mathf.Clamp(tier, 1, 3))
            {
                case 2:
                    return "Pertahanan Berlapis";
                case 3:
                    return "Respons Insiden";
                default:
                    return "Dasar Keamanan Gadget";
            }
        }

        private static string GetDefaultKnowledgeBody(int tier)
        {
            switch (Mathf.Clamp(tier, 1, 3))
            {
                case 2:
                    return "Gunakan backup, firewall, pembatasan izin, dan pemantauan alert agar malware tidak mudah menyebar.";
                case 3:
                    return "Saat serangan besar muncul, isolasi sistem, pulihkan dari backup, analisis log, lalu perkuat kontrol keamanan.";
                default:
                    return "Amankan akun dengan password unik, MFA, update rutin, dan kebiasaan tidak membuka file atau link mencurigakan.";
            }
        }

        public static bool IsDeveloperModeEnabled()
        {
            return PlayerPrefs.GetInt(DeveloperModeKey, 0) == 1;
        }

        public static bool IsMusicEnabled()
        {
            return PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1 && GetMusicVolumePercent() > 0;
        }

        public static bool IsSfxEnabled()
        {
            return PlayerPrefs.GetInt(SfxEnabledKey, 1) == 1 && GetSfxVolumePercent() > 0;
        }

        public static int GetMusicVolumePercent()
        {
            return Mathf.Clamp(PlayerPrefs.GetInt(MusicVolumeKey, 100), 0, 100);
        }

        public static int GetSfxVolumePercent()
        {
            return Mathf.Clamp(PlayerPrefs.GetInt(SfxVolumeKey, 100), 0, 100);
        }

        public static float GetMusicVolume()
        {
            return IsMusicEnabled() ? GetMusicVolumePercent() / 100f : 0f;
        }

        public static float GetSfxVolume()
        {
            return IsSfxEnabled() ? GetSfxVolumePercent() / 100f : 0f;
        }

        public static int GetControlScheme()
        {
            return Mathf.Clamp(PlayerPrefs.GetInt(ControlSchemeKey, 0), 0, 3);
        }

        public static string GetControlSchemeName()
        {
            switch (GetControlScheme())
            {
                case 1:
                    return "WASD";
                case 2:
                    return "Arrow";
                case 3:
                    return "IJKL";
                default:
                    return "Hybrid";
            }
        }

        public static string GetControlSchemeHint()
        {
            switch (GetControlScheme())
            {
                case 1:
                    return "GERAK: A/D  TUNDUK: S  LOMPAT: W/SPACE  BOOST: LEFT SHIFT  SERANG: J  TEMBAK: L/KLIK";
                case 2:
                    return "GERAK: PANAH KIRI/KANAN  TUNDUK: PANAH BAWAH  LOMPAT: PANAH ATAS/SPACE  BOOST: RIGHT SHIFT  SERANG: J  TEMBAK: L/KLIK";
                case 3:
                    return "GERAK: J/L  TUNDUK: K  LOMPAT: I/SPACE  BOOST: U  SERANG: H  TEMBAK: O/KLIK";
                default:
                    return "GERAK: A/D ATAU PANAH  TUNDUK: S/PANAH BAWAH  LOMPAT: SPACE/W/PANAH ATAS  BOOST: SHIFT/K  SERANG: J  TEMBAK: L/KLIK";
            }
        }

        public static KeyCode GetLeftKey()
        {
            switch (GetControlScheme())
            {
                case 2:
                    return KeyCode.LeftArrow;
                case 3:
                    return KeyCode.J;
                default:
                    return KeyCode.A;
            }
        }

        public static KeyCode GetRightKey()
        {
            switch (GetControlScheme())
            {
                case 2:
                    return KeyCode.RightArrow;
                case 3:
                    return KeyCode.L;
                default:
                    return KeyCode.D;
            }
        }

        public static KeyCode GetUpKey()
        {
            switch (GetControlScheme())
            {
                case 2:
                    return KeyCode.UpArrow;
                case 3:
                    return KeyCode.I;
                default:
                    return KeyCode.W;
            }
        }

        public static KeyCode GetDownKey()
        {
            switch (GetControlScheme())
            {
                case 2:
                    return KeyCode.DownArrow;
                case 3:
                    return KeyCode.K;
                default:
                    return KeyCode.S;
            }
        }

        public static KeyCode GetBoostKey()
        {
            switch (GetControlScheme())
            {
                case 2:
                    return KeyCode.RightShift;
                case 3:
                    return KeyCode.U;
                default:
                    return KeyCode.LeftShift;
            }
        }

        public static KeyCode GetMeleeKey()
        {
            return GetControlScheme() == 3 ? KeyCode.H : KeyCode.J;
        }

        public static KeyCode GetShootKey()
        {
            return GetControlScheme() == 3 ? KeyCode.O : KeyCode.L;
        }

        public static void ApplyAudioPreferencesToScene()
        {
            AudioSource[] sources = Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include);
            bool musicEnabled = IsMusicEnabled();
            bool sfxEnabled = IsSfxEnabled();
            float musicVolume = GetMusicVolume();
            float sfxVolume = GetSfxVolume();
            for (int i = 0; i < sources.Length; i++)
            {
                AudioSource source = sources[i];
                if (source == null)
                {
                    continue;
                }

                source.mute = source.loop ? !musicEnabled : !sfxEnabled;
                source.volume = source.loop ? musicVolume : sfxVolume;
            }
        }

        public static void ClearSavedProgress()
        {
            PlayerPrefs.SetInt(SaveExistsKey, 0);
            PlayerPrefs.SetInt(ResumeRequestedKey, 0);
            PlayerPrefs.DeleteKey(SaveSceneKey);
            PlayerPrefs.DeleteKey(SaveXKey);
            PlayerPrefs.DeleteKey(SaveYKey);
            PlayerPrefs.DeleteKey(SaveZKey);
            PlayerPrefs.DeleteKey(SaveHealthKey);
            PlayerPrefs.DeleteKey(SaveLivesKey);
            PlayerPrefs.DeleteKey(SaveBoostKey);
            PlayerPrefs.DeleteKey(SaveScoreKey);
        }

        private static void EnsureDefaultPreferences()
        {
            if (!PlayerPrefs.HasKey(MusicEnabledKey))
            {
                PlayerPrefs.SetInt(MusicEnabledKey, 1);
            }

            if (!PlayerPrefs.HasKey(SfxEnabledKey))
            {
                PlayerPrefs.SetInt(SfxEnabledKey, 1);
            }

            if (!PlayerPrefs.HasKey(ControlSchemeKey))
            {
                PlayerPrefs.SetInt(ControlSchemeKey, 0);
            }

            if (!PlayerPrefs.HasKey(MusicVolumeKey))
            {
                PlayerPrefs.SetInt(MusicVolumeKey, PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1 ? 100 : 0);
            }

            if (!PlayerPrefs.HasKey(SfxVolumeKey))
            {
                PlayerPrefs.SetInt(SfxVolumeKey, PlayerPrefs.GetInt(SfxEnabledKey, 1) == 1 ? 100 : 0);
            }
        }

        private void NormalizeCornerUtilityButtons()
        {
            MoveMenuButton(galleryButton, new Vector2(-832f, 492f), new Vector2(224f, 46f));
            MoveMenuButton(developerButton, new Vector2(-832f, 438f), new Vector2(224f, 46f));
        }

        private static void MoveMenuButton(Button button, Vector2 position, Vector2 size)
        {
            if (button == null)
            {
                return;
            }

            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private void WireSettingsButton(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null || action == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => RunButtonAction(() => action.Invoke()));
        }

        private static void SetButtonLabel(Button button, string text)
        {
            if (button == null)
            {
                return;
            }

            Text label = button.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                label.text = text;
            }
        }
    }
}
