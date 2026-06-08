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
        public Button quitButton;
        public Button settingsBackButton;
        public Button creditsBackButton;
        public GameObject settingsPanel;
        public GameObject creditsPanel;
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
            WireButtons();
            HidePanels();
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
                settingsBackButton,
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
            }

            if (continueButton != null)
            {
                continueButton.interactable = HasSavedProgress() && !startingGame;
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
    }
}
