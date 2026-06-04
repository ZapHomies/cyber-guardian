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

        public string gameplaySceneName = "CyberGuardian_Level01";
        public Text selectedDifficultyText;
        public Button startButton;
        public Button easyButton;
        public Button normalButton;
        public Button hardButton;
        public Button quitButton;
        public Image[] difficultyHighlights;

        private readonly string[] difficultyNames = { "Easy", "Normal", "Hard" };
        private int selectedDifficulty = 1;

        private void Awake()
        {
            selectedDifficulty = Mathf.Clamp(PlayerPrefs.GetInt(DifficultyKey, 1), 0, 2);
            WireButtons();
            Refresh();
        }

        private void WireButtons()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(StartGame);
            }

            WireDifficultyButton(easyButton, 0);
            WireDifficultyButton(normalButton, 1);
            WireDifficultyButton(hardButton, 2);

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

        private void SelectDifficulty(int index)
        {
            selectedDifficulty = Mathf.Clamp(index, 0, 2);
            PlayerPrefs.SetInt(DifficultyKey, selectedDifficulty);
            PlayerPrefs.Save();
            Refresh();
        }

        private void StartGame()
        {
            PlayerPrefs.SetInt(DifficultyKey, selectedDifficulty);
            PlayerPrefs.Save();
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameplaySceneName);
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
                selectedDifficultyText.text = "DIFFICULTY: " + difficultyNames[selectedDifficulty].ToUpperInvariant();
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
    }
}
