using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CyberGuardian
{
    public sealed class CyberGuardianDifficultySelect : MonoBehaviour
    {
        public string gameplaySceneName = "CyberGuardian_Level01";
        public string menuSceneName = "CyberGuardian_MainMenu";
        public Button easyButton;
        public Button normalButton;
        public Button hardButton;
        public Button backButton;
        public Text detailText;
        public GameObject startTransitionOverlay;
        public Image startTransitionFade;
        public Image startTransitionCircuit;
        public Text startTransitionText;
        public Image[] startTransitionFx;
        public float startTransitionDuration = 1.85f;

        private bool loading;

        private void Awake()
        {
            WireButton(easyButton, 0);
            WireButton(normalButton, 1);
            WireButton(hardButton, 2);

            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(BackToMenu);
            }

            if (startTransitionOverlay != null)
            {
                startTransitionOverlay.SetActive(false);
            }

            SetDetail("Pilih tingkat kesulitan sebelum Cyber Guardian memasuki Hutan Data.");
        }

        private void WireButton(Button button, int difficultyIndex)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectDifficultyAndStart(difficultyIndex));
        }

        private void SelectDifficultyAndStart(int difficultyIndex)
        {
            if (loading)
            {
                return;
            }

            PlayerPrefs.SetInt(CyberGuardianMainMenu.DifficultyKey, Mathf.Clamp(difficultyIndex, 0, 2));
            CyberGuardianMainMenu.ClearSavedProgress();
            PlayerPrefs.SetInt(CyberGuardianMainMenu.ResumeRequestedKey, 0);
            PlayerPrefs.Save();
            Time.timeScale = 1f;
            StartCoroutine(LoadGameplaySequence());
        }

        private void BackToMenu()
        {
            if (loading)
            {
                return;
            }

            SceneManager.LoadScene(menuSceneName);
        }

        private IEnumerator LoadGameplaySequence()
        {
            loading = true;
            SetButtonsInteractable(false);

            if (startTransitionOverlay == null)
            {
                SceneManager.LoadScene(gameplaySceneName);
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
                    startTransitionCircuit.color = new Color(0.20f, 0.95f, 1f, Mathf.Lerp(0f, 0.26f, Mathf.Sin(t * Mathf.PI)));
                }

                if (startTransitionText != null)
                {
                    startTransitionText.text = t < 0.54f ? "MENYIAPKAN MISI" : "MASUK KE HUTAN DATA";
                    startTransitionText.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t * 1.8f)));
                    startTransitionText.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.92f, 1.06f, Mathf.Sin(t * Mathf.PI));
                }

                AnimateTransitionEffects(t);
                yield return null;
            }

            SceneManager.LoadScene(gameplaySceneName);
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

                float phase = Mathf.Repeat(t * 1.9f + i * 0.15f, 1f);
                RectTransform rect = effect.rectTransform;
                float direction = i % 2 == 0 ? 1f : -1f;
                rect.anchoredPosition = new Vector2(Mathf.Lerp(-1080f, 1080f, phase) * direction, rect.anchoredPosition.y);
                Color color = effect.color;
                color.a = Mathf.Sin(phase * Mathf.PI) * 0.72f;
                effect.color = color;
            }
        }

        private void SetButtonsInteractable(bool interactable)
        {
            Button[] buttons =
            {
                easyButton,
                normalButton,
                hardButton,
                backButton
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].interactable = interactable;
                }
            }
        }

        private void SetDetail(string message)
        {
            if (detailText != null)
            {
                detailText.text = message;
            }
        }
    }
}
