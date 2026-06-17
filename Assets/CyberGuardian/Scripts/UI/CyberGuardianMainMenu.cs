using System.Collections;
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
        public Button creditsBackButton;
        public GameObject settingsPanel;
        public GameObject creditsPanel;
        public GameObject developerLoginPanel;
        public InputField developerUserInput;
        public InputField developerPasswordInput;
        public Text developerStatusText;
        public Text settingsMusicText;
        public Text settingsSfxText;
        public Text settingsControlText;
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

        private void Awake()
        {
            selectedDifficulty = Mathf.Clamp(PlayerPrefs.GetInt(DifficultyKey, 1), 0, 2);
            EnsureDefaultPreferences();
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
                startButton.onClick.AddListener(StartNewGame);
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(ContinueGame);
            }

            WireDifficultyButton(easyButton, 0);
            WireDifficultyButton(normalButton, 1);
            WireDifficultyButton(hardButton, 2);

            WirePanelButton(settingsButton, settingsPanel);
            WirePanelButton(creditsButton, creditsPanel);
            WireCloseButton(settingsBackButton, settingsPanel);
            WireCloseButton(creditsBackButton, creditsPanel);
            WireSettingsButton(settingsMusicToggleButton, ToggleMusic);
            WireSettingsButton(settingsSfxToggleButton, ToggleSfx);
            WireSettingsButton(settingsControlSchemeButton, CycleControlScheme);

            if (developerButton != null)
            {
                developerButton.onClick.RemoveAllListeners();
                developerButton.onClick.AddListener(ShowDeveloperLogin);
            }

            if (developerLoginButton != null)
            {
                developerLoginButton.onClick.RemoveAllListeners();
                developerLoginButton.onClick.AddListener(SubmitDeveloperLogin);
            }

            if (developerDisableButton != null)
            {
                developerDisableButton.onClick.RemoveAllListeners();
                developerDisableButton.onClick.AddListener(DisableDeveloperMode);
            }

            if (developerCancelButton != null)
            {
                developerCancelButton.onClick.RemoveAllListeners();
                developerCancelButton.onClick.AddListener(() =>
                {
                    if (developerLoginPanel != null)
                    {
                        developerLoginPanel.SetActive(false);
                    }
                });
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(QuitGame);
            }
        }

        private void WireDifficultyButton(Button button, int index)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectDifficulty(index));
        }

        private void WirePanelButton(Button button, GameObject panel)
        {
            if (button == null || panel == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ShowOnlyPanel(panel));
        }

        private void WireCloseButton(Button button, GameObject panel)
        {
            if (button == null || panel == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => panel.SetActive(false));
        }

        private void ShowOnlyPanel(GameObject panel)
        {
            HidePanels();
            panel.SetActive(true);
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

        private void Refresh()
        {
            if (selectedDifficultyText != null)
            {
                selectedDifficultyText.text = "KESULITAN: " + difficultyNames[selectedDifficulty].ToUpperInvariant();
                if (IsDeveloperModeEnabled())
                {
                    selectedDifficultyText.text += "  |  MODE DEVELOPER AKTIF";
                }
            }

            if (continueButton != null)
            {
                continueButton.interactable = HasSavedProgress() && !startingGame;
            }

            SetButtonLabel(developerButton, IsDeveloperModeEnabled() ? "DEV: AKTIF" : "MODE DEV");
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

        private static void WireSettingsButton(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null || action == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
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
