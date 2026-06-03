using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CyberGuardian
{
    public sealed class CyberGuardianLevelController : MonoBehaviour
    {
        [System.Serializable]
        public sealed class QuizOrbNode
        {
            public Button button;
            public Image fill;
            public Text label;
            public RectTransform rectTransform;
            public int category;
            public bool startsCleared;
            [HideInInspector] public bool cleared;
        }

        private enum LaunchImpactType
        {
            None,
            Orb,
            Virus
        }

        private struct LaunchImpact
        {
            public LaunchImpactType type;
            public int orbIndex;
            public Vector2 position;
        }

        public string menuSceneName = "CyberGuardian_MainMenu";
        public QuizQuestionBank quizQuestionBank;
        public DifficultyProfile defaultDifficulty;
        public DifficultyProfile[] difficultyProfiles;
        public List<QuizOrbNode> quizOrbs = new List<QuizOrbNode>();

        public RectTransform guardianAnchor;
        public RectTransform virusAnchor;
        public RectTransform attackProjectile;
        public RectTransform attackBeam;
        public RectTransform slingshotLeftAnchor;
        public RectTransform slingshotRightAnchor;
        public RectTransform slingshotBandLeft;
        public RectTransform slingshotBandRight;
        public RectTransform[] trajectoryDots;
        public Image guardianShieldGlow;
        public Image virusGlow;
        public Image shieldFill;
        public Image virusFill;
        public Image launchPowerFill;
        public Text shieldText;
        public Text virusText;
        public Text scoreText;
        public Text difficultyText;
        public Text routeText;
        public Text statusText;
        public Text attackButtonLabel;
        public Text launchHintText;

        public Button attackButton;
        public Button resetButton;
        public Button menuButton;
        public Button pauseButton;
        public Text pauseButtonLabel;

        public GameObject quizModal;
        public Text quizTitleText;
        public Text quizPromptText;
        public Text feedbackText;
        public Button[] answerButtons;
        public Text[] answerLabels;
        public Button closeQuizButton;

        public AudioSource sfxSource;
        public AudioClip launchSfx;
        public AudioClip orbHitSfx;
        public AudioClip virusHitSfx;
        public AudioClip shieldSfx;
        public AudioClip wrongAnswerSfx;

        public float maxPullDistance = 135f;
        public float launchVelocityScale = 8.4f;
        public Vector2 launchGravity = new Vector2(0f, -880f);
        public float trajectoryTimeStep = 0.12f;
        public float maxFlightTime = 1.85f;

        private static readonly QuizQuestion[] FallbackQuestions =
        {
            new QuizQuestion(CyberQuestionCategory.Password, "PASSWORD NODE", "Apa ciri password yang aman?", new[] { "Panjang dan unik", "Nama sendiri", "123456", "Tanggal lahir" }, 0, "Benar. Password aman panjang, unik, dan tidak dipakai ulang."),
            new QuizQuestion(CyberQuestionCategory.Malware, "PHISHING NODE", "Email meminta OTP lewat link mencurigakan. Apa tindakan aman?", new[] { "Jangan klik dan laporkan", "Kirim OTP", "Login cepat", "Bagikan link" }, 0, "Benar. OTP tidak boleh dibagikan dan link harus diverifikasi."),
            new QuizQuestion(CyberQuestionCategory.Network, "NETWORK NODE", "Saat memakai Wi-Fi publik, apa langkah paling aman?", new[] { "Hindari akses sensitif", "Matikan firewall", "Pakai password sama", "Abaikan alamat situs" }, 0, "Benar. Wi-Fi publik harus dipakai dengan hati-hati."),
            new QuizQuestion(CyberQuestionCategory.Privacy, "PRIVACY NODE", "Data apa yang tidak boleh dibagikan sembarangan?", new[] { "NIK, OTP, password", "Warna favorit", "Genre game", "Nama panggilan" }, 0, "Benar. Data sensitif bisa dipakai untuk penipuan.")
        };

        private readonly Color[] categoryColors =
        {
            new Color(0.18f, 0.58f, 1f, 1f),
            new Color(0.40f, 0.86f, 0.28f, 1f),
            new Color(1.00f, 0.78f, 0.18f, 1f),
            new Color(0.78f, 0.35f, 1f, 1f)
        };

        private const float BlockingDistance = 58f;
        private const float OrbHitRadius = 47f;
        private const float VirusHitRadius = 76f;
        private const float MinLaunchPullDistance = 18f;
        private const float PlayfieldTopLimit = 286f;
        private const float PlayfieldBottomLimit = -286f;
        private int currentDifficultyIndex = 1;
        private int currentOrbIndex = -1;
        private int shieldIntegrity;
        private int virusStrength;
        private int score;
        private int launchedAttacks;
        private bool attackInFlight;
        private bool paused;
        private bool levelEnded;
        private bool wired;
        private bool isDraggingSlingshot;
        private float guardianBoostPulse;
        private float virusBoostPulse;
        private Vector2 currentPullOffset;

        private void Awake()
        {
            currentDifficultyIndex = Mathf.Clamp(PlayerPrefs.GetInt(CyberGuardianMainMenu.DifficultyKey, 1), 0, 2);
            WireControls();
            ResetLevel();
        }

        private void Update()
        {
            PulseVisuals();
        }

        private void WireControls()
        {
            if (wired)
            {
                return;
            }

            for (int i = 0; i < quizOrbs.Count; i++)
            {
                int index = i;
                if (quizOrbs[i] != null && quizOrbs[i].button != null)
                {
                    quizOrbs[i].button.onClick.RemoveAllListeners();
                    quizOrbs[i].button.onClick.AddListener(() => OpenQuiz(index));
                }
            }

            if (answerButtons != null)
            {
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    int choice = i;
                    if (answerButtons[i] != null)
                    {
                        answerButtons[i].onClick.RemoveAllListeners();
                        answerButtons[i].onClick.AddListener(() => ChooseAnswer(choice));
                    }
                }
            }

            if (attackButton != null)
            {
                attackButton.onClick.RemoveAllListeners();
                attackButton.onClick.AddListener(ShowSlingshotHint);
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveAllListeners();
                resetButton.onClick.AddListener(ResetLevel);
            }

            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(ReturnToMenu);
            }

            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(TogglePause);
            }

            if (closeQuizButton != null)
            {
                closeQuizButton.onClick.RemoveAllListeners();
                closeQuizButton.onClick.AddListener(CloseQuiz);
            }

            wired = true;
        }

        public void ResetLevel()
        {
            DifficultyProfile difficulty = GetActiveDifficulty();
            shieldIntegrity = difficulty != null ? difficulty.startingShield : 100;
            virusStrength = difficulty != null ? difficulty.startingVirusStrength : 25;
            score = difficulty != null ? difficulty.startingScore : 1200;
            launchedAttacks = 0;
            currentOrbIndex = -1;
            attackInFlight = false;
            isDraggingSlingshot = false;
            levelEnded = false;
            paused = false;
            guardianBoostPulse = 0f;
            virusBoostPulse = 0f;
            Time.timeScale = 1f;

            for (int i = 0; i < quizOrbs.Count; i++)
            {
                QuizOrbNode orb = quizOrbs[i];
                if (orb == null)
                {
                    continue;
                }

                orb.cleared = orb.startsCleared;
                if (orb.button != null)
                {
                    orb.button.gameObject.SetActive(!orb.cleared);
                    orb.button.interactable = !orb.cleared;
                }
            }

            if (quizModal != null)
            {
                quizModal.SetActive(false);
            }

            ResetProjectile();
            SetStatus("HIT QUIZ FIREWALL ORBS, ANSWER CORRECTLY, THEN OPEN A PATH");
            RefreshOrbVisuals();
            RefreshRouteVisual();
            RefreshHud();
        }

        public void BeginSlingshotDrag(PointerEventData eventData)
        {
            if (!CanUseSlingshot())
            {
                ShowSlingshotHint();
                return;
            }

            isDraggingSlingshot = true;
            if (attackProjectile != null)
            {
                attackProjectile.gameObject.SetActive(true);
            }

            SetSlingshotBandsActive(true);
            UpdateDragPosition(eventData);
        }

        public void DragSlingshot(PointerEventData eventData)
        {
            if (!isDraggingSlingshot || !CanUseSlingshot())
            {
                return;
            }

            UpdateDragPosition(eventData);
        }

        public void ReleaseSlingshot(PointerEventData eventData)
        {
            if (!isDraggingSlingshot)
            {
                return;
            }

            isDraggingSlingshot = false;
            SetTrajectoryDotsActive(false);

            float pullDistance = currentPullOffset.magnitude;
            if (pullDistance < MinLaunchPullDistance)
            {
                ResetProjectile();
                SetStatus("PULL FARTHER TO CHARGE THE SECURITY PATCH");
                return;
            }

            Vector2 start = attackProjectile != null ? attackProjectile.anchoredPosition : GetSlingshotRestPosition();
            Vector2 velocity = GetLaunchVelocity(currentPullOffset);
            StartCoroutine(LaunchSlingshotRoutine(start, velocity));
        }

        private bool CanUseSlingshot()
        {
            bool quizOpen = quizModal != null && quizModal.activeSelf;
            return !attackInFlight && !paused && !levelEnded && !quizOpen && attackProjectile != null;
        }

        private void UpdateDragPosition(PointerEventData eventData)
        {
            if (attackProjectile == null || eventData == null)
            {
                return;
            }

            RectTransform parentRect = attackProjectile.parent as RectTransform;
            if (parentRect == null)
            {
                return;
            }

            Camera eventCamera = eventData.pressEventCamera != null ? eventData.pressEventCamera : eventData.enterEventCamera;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventCamera, out Vector2 localPoint))
            {
                return;
            }

            Vector2 rest = GetSlingshotRestPosition();
            Vector2 offset = localPoint - rest;
            if (offset.magnitude > maxPullDistance)
            {
                offset = offset.normalized * maxPullDistance;
            }

            currentPullOffset = offset;
            Vector2 projectilePosition = rest + offset;
            attackProjectile.anchoredPosition = projectilePosition;
            UpdateSlingshotBands(projectilePosition);
            UpdatePowerVisual(offset.magnitude / maxPullDistance);

            if (offset.magnitude >= MinLaunchPullDistance)
            {
                UpdateTrajectoryPreview(projectilePosition, GetLaunchVelocity(offset));
                SetStatus("AIMING PATCH CORE - RELEASE TO LAUNCH");
            }
            else
            {
                SetTrajectoryDotsActive(false);
                SetStatus("PULL THE PATCH CORE BACK TO AIM");
            }
        }

        private IEnumerator LaunchSlingshotRoutine(Vector2 start, Vector2 velocity)
        {
            attackInFlight = true;
            launchedAttacks++;
            RefreshHud();
            SetSlingshotBandsActive(false);
            SetTrajectoryDotsActive(false);
            PlaySfx(launchSfx);
            SetStatus("SECURITY PATCH LAUNCHED");

            Vector2 previous = start;
            LaunchImpact impact = new LaunchImpact
            {
                type = LaunchImpactType.None,
                orbIndex = -1,
                position = start
            };

            float elapsed = 0f;
            while (elapsed < maxFlightTime)
            {
                elapsed += Time.unscaledDeltaTime;
                Vector2 next = GetTrajectoryPosition(start, velocity, elapsed);
                if (attackProjectile != null)
                {
                    attackProjectile.anchoredPosition = next;
                    Vector2 direction = next - previous;
                    if (direction.sqrMagnitude > 0.01f)
                    {
                        attackProjectile.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                    }
                }

                int orbIndex = FindOrbHit(previous, next);
                if (orbIndex >= 0)
                {
                    impact.type = LaunchImpactType.Orb;
                    impact.orbIndex = orbIndex;
                    impact.position = quizOrbs[orbIndex].rectTransform.anchoredPosition;
                    break;
                }

                if (IsVirusHit(previous, next))
                {
                    impact.type = LaunchImpactType.Virus;
                    impact.position = virusAnchor != null ? virusAnchor.anchoredPosition : next;
                    break;
                }

                if (IsOutsidePlayfield(next))
                {
                    break;
                }

                previous = next;
                yield return null;
            }

            if (attackProjectile != null)
            {
                attackProjectile.anchoredPosition = impact.type == LaunchImpactType.None ? attackProjectile.anchoredPosition : impact.position;
            }

            if (impact.type == LaunchImpactType.Orb)
            {
                yield return ResolveOrbImpact(impact.orbIndex);
            }
            else if (impact.type == LaunchImpactType.Virus)
            {
                yield return ResolveVirusImpact();
            }
            else
            {
                yield return ResolveMiss();
            }
        }

        private IEnumerator ResolveOrbImpact(int orbIndex)
        {
            PlaySfx(orbHitSfx);
            FlashOrb(orbIndex);
            SetStatus("PATCH HIT A QUIZ NODE - ANSWER TO DELETE IT");
            yield return new WaitForSecondsRealtime(0.22f);

            attackInFlight = false;
            ResetProjectile();
            RefreshHud();
            OpenQuiz(orbIndex, true);
        }

        private IEnumerator ResolveVirusImpact()
        {
            PlaySfx(virusHitSfx);
            DifficultyProfile difficulty = GetActiveDifficulty();
            virusStrength = Mathf.Max(0, virusStrength - (difficulty != null ? difficulty.routeVirusDamage : 30));
            score += difficulty != null ? difficulty.routeScoreReward : 250;
            bool victory = virusStrength <= 0;

            if (virusGlow != null)
            {
                virusBoostPulse = 0.55f;
            }

            SetStatus(victory ? "LEVEL CLEAR - VIRUS PURGED" : "VIRUS HIT - KEEP PATCHING");
            RefreshHud();
            yield return new WaitForSecondsRealtime(0.28f);

            attackInFlight = false;
            ResetProjectile();
            if (victory)
            {
                EndLevel(true);
            }
            else
            {
                RefreshRouteVisual();
                RefreshHud();
            }
        }

        private IEnumerator ResolveMiss()
        {
            SetStatus("PATCH MISSED - ADJUST YOUR SLINGSHOT ANGLE");
            yield return new WaitForSecondsRealtime(0.22f);
            attackInFlight = false;
            ResetProjectile();
            RefreshRouteVisual();
            RefreshHud();
        }

        private void OpenQuiz(int index)
        {
            OpenQuiz(index, false);
        }

        private void OpenQuiz(int index, bool fromProjectileImpact)
        {
            if (levelEnded || paused || (!fromProjectileImpact && attackInFlight) || index < 0 || index >= quizOrbs.Count)
            {
                return;
            }

            QuizOrbNode orb = quizOrbs[index];
            if (orb == null || orb.cleared)
            {
                return;
            }

            QuizQuestion question = GetQuestionForOrb(index);
            if (question == null)
            {
                SetStatus("QUIZ DATA NOT FOUND");
                return;
            }

            currentOrbIndex = index;
            if (quizTitleText != null)
            {
                quizTitleText.text = question.title;
            }

            if (quizPromptText != null)
            {
                quizPromptText.text = question.prompt;
            }

            if (feedbackText != null)
            {
                feedbackText.text = string.Empty;
            }

            if (answerButtons != null && answerLabels != null)
            {
                for (int i = 0; i < answerButtons.Length && i < answerLabels.Length; i++)
                {
                    bool hasAnswer = question.answers != null && i < question.answers.Length;
                    if (answerButtons[i] != null)
                    {
                        answerButtons[i].gameObject.SetActive(hasAnswer);
                        answerButtons[i].interactable = hasAnswer;
                    }

                    if (answerLabels[i] != null)
                    {
                        answerLabels[i].text = hasAnswer ? question.answers[i] : string.Empty;
                    }
                }
            }

            if (quizModal != null)
            {
                quizModal.SetActive(true);
            }

            SetStatus("ANSWER THE QUIZ TO DELETE THIS BLOCKER");
            RefreshHud();
        }

        private void ChooseAnswer(int choice)
        {
            if (currentOrbIndex < 0 || currentOrbIndex >= quizOrbs.Count)
            {
                return;
            }

            QuizOrbNode orb = quizOrbs[currentOrbIndex];
            QuizQuestion question = GetQuestionForOrb(currentOrbIndex);
            if (orb == null || question == null)
            {
                return;
            }

            if (answerButtons != null)
            {
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    if (answerButtons[i] != null)
                    {
                        answerButtons[i].interactable = false;
                    }
                }
            }

            DifficultyProfile difficulty = GetActiveDifficulty();
            if (choice == question.correctIndex)
            {
                orb.cleared = true;
                if (orb.button != null)
                {
                    orb.button.gameObject.SetActive(false);
                }

                score += difficulty != null ? difficulty.correctScoreReward : 75;
                shieldIntegrity = Mathf.Min(100, shieldIntegrity + (difficulty != null ? difficulty.correctShieldReward : 6));
                if (feedbackText != null)
                {
                    feedbackText.text = question.feedback;
                }

                if (guardianShieldGlow != null)
                {
                    guardianBoostPulse = 1f;
                }

                PlaySfx(shieldSfx);
                SetStatus("NODE DELETED - SHIELD AND ROUTE IMPROVED");
            }
            else
            {
                shieldIntegrity = Mathf.Max(0, shieldIntegrity - (difficulty != null ? difficulty.wrongShieldDamage : 15));
                virusStrength = Mathf.Min(100, virusStrength + (difficulty != null ? difficulty.wrongVirusGain : 12));
                if (feedbackText != null)
                {
                    feedbackText.text = "Jawaban salah. Shield melemah dan virus semakin kuat.";
                }

                if (virusGlow != null)
                {
                    virusBoostPulse = 1f;
                }

                PlaySfx(wrongAnswerSfx);
                SetStatus(shieldIntegrity <= 0 ? "GAME OVER - GUARDIAN SHIELD DOWN" : "WRONG ANSWER - SHIELD DAMAGED");
            }

            RefreshOrbVisuals();
            RefreshRouteVisual();
            RefreshHud();

            if (shieldIntegrity <= 0)
            {
                EndLevel(false);
                return;
            }

            StopAllCoroutines();
            StartCoroutine(CloseQuizSoon());
        }

        private IEnumerator CloseQuizSoon()
        {
            yield return new WaitForSecondsRealtime(1.0f);
            CloseQuiz();
            SetStatus("HIT QUIZ FIREWALL ORBS, ANSWER CORRECTLY, THEN OPEN A PATH");
        }

        private void CloseQuiz()
        {
            if (quizModal != null)
            {
                quizModal.SetActive(false);
            }

            currentOrbIndex = -1;
            RefreshHud();
        }

        private void ShowSlingshotHint()
        {
            if (levelEnded)
            {
                return;
            }

            if (paused)
            {
                SetStatus("RESUME FIRST, THEN DRAG THE PATCH CORE");
                return;
            }

            if (quizModal != null && quizModal.activeSelf)
            {
                SetStatus("ANSWER THE QUIZ BEFORE LAUNCHING AGAIN");
                return;
            }

            SetStatus("DRAG THE GLOWING PATCH CORE BESIDE CYBER GUARDIAN");
        }

        private int FindFirstBlockingOrb()
        {
            Vector2 start = guardianAnchor != null ? guardianAnchor.anchoredPosition : new Vector2(-470f, -98f);
            Vector2 end = virusAnchor != null ? virusAnchor.anchoredPosition : new Vector2(500f, 20f);
            int blockerIndex = -1;
            float bestT = float.PositiveInfinity;

            for (int i = 0; i < quizOrbs.Count; i++)
            {
                QuizOrbNode orb = quizOrbs[i];
                if (orb == null || orb.cleared || orb.rectTransform == null)
                {
                    continue;
                }

                float t;
                float distance = DistancePointToSegment(orb.rectTransform.anchoredPosition, start, end, out t);
                if (distance <= BlockingDistance && t > 0.08f && t < 0.92f && t < bestT)
                {
                    bestT = t;
                    blockerIndex = i;
                }
            }

            return blockerIndex;
        }

        private int FindOrbHit(Vector2 start, Vector2 end)
        {
            int hitIndex = -1;
            float bestT = float.PositiveInfinity;

            for (int i = 0; i < quizOrbs.Count; i++)
            {
                QuizOrbNode orb = quizOrbs[i];
                if (orb == null || orb.cleared || orb.rectTransform == null)
                {
                    continue;
                }

                float t;
                float distance = DistancePointToSegment(orb.rectTransform.anchoredPosition, start, end, out t);
                if (distance <= OrbHitRadius && t < bestT)
                {
                    bestT = t;
                    hitIndex = i;
                }
            }

            return hitIndex;
        }

        private bool IsVirusHit(Vector2 start, Vector2 end)
        {
            if (virusAnchor == null)
            {
                return false;
            }

            float t;
            return DistancePointToSegment(virusAnchor.anchoredPosition, start, end, out t) <= VirusHitRadius;
        }

        private bool IsOutsidePlayfield(Vector2 position)
        {
            if (attackProjectile == null)
            {
                return false;
            }

            RectTransform parentRect = attackProjectile.parent as RectTransform;
            if (parentRect == null)
            {
                return false;
            }

            Rect rect = parentRect.rect;
            return position.x < rect.xMin - 120f
                || position.x > rect.xMax + 120f
                || position.y < PlayfieldBottomLimit
                || position.y > PlayfieldTopLimit;
        }

        private float DistancePointToSegment(Vector2 point, Vector2 a, Vector2 b, out float t)
        {
            Vector2 ab = b - a;
            float lengthSq = ab.sqrMagnitude;
            if (lengthSq <= Mathf.Epsilon)
            {
                t = 0f;
                return Vector2.Distance(point, a);
            }

            t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / lengthSq);
            Vector2 projection = a + ab * t;
            return Vector2.Distance(point, projection);
        }

        private void FlashOrb(int index)
        {
            if (index < 0 || index >= quizOrbs.Count || quizOrbs[index] == null || quizOrbs[index].fill == null)
            {
                return;
            }

            quizOrbs[index].fill.color = new Color(1f, 0.20f, 0.24f, 1f);
        }

        private void ResetProjectile()
        {
            currentPullOffset = Vector2.zero;
            Vector2 rest = GetSlingshotRestPosition();
            if (attackProjectile != null)
            {
                attackProjectile.anchoredPosition = rest;
                attackProjectile.localRotation = Quaternion.identity;
                attackProjectile.gameObject.SetActive(!levelEnded);
            }

            UpdatePowerVisual(0f);
            UpdateSlingshotBands(rest);
            SetSlingshotBandsActive(!levelEnded && !attackInFlight);
            SetTrajectoryDotsActive(false);
        }

        private Vector2 GetSlingshotRestPosition()
        {
            return guardianAnchor != null ? guardianAnchor.anchoredPosition : new Vector2(-470f, -98f);
        }

        private Vector2 GetLaunchVelocity(Vector2 pullOffset)
        {
            return -pullOffset * launchVelocityScale;
        }

        private Vector2 GetTrajectoryPosition(Vector2 start, Vector2 velocity, float time)
        {
            return start + velocity * time + 0.5f * launchGravity * time * time;
        }

        private void UpdateTrajectoryPreview(Vector2 start, Vector2 velocity)
        {
            if (trajectoryDots == null || trajectoryDots.Length == 0)
            {
                return;
            }

            for (int i = 0; i < trajectoryDots.Length; i++)
            {
                RectTransform dot = trajectoryDots[i];
                if (dot == null)
                {
                    continue;
                }

                float time = (i + 1) * trajectoryTimeStep;
                dot.anchoredPosition = GetTrajectoryPosition(start, velocity, time);
                dot.localScale = Vector3.one * Mathf.Lerp(1f, 0.55f, i / Mathf.Max(1f, trajectoryDots.Length - 1f));
                dot.gameObject.SetActive(true);

                Image image = dot.GetComponent<Image>();
                if (image != null)
                {
                    Color color = image.color;
                    color.a = Mathf.Lerp(0.85f, 0.25f, i / Mathf.Max(1f, trajectoryDots.Length - 1f));
                    image.color = color;
                }
            }
        }

        private void SetTrajectoryDotsActive(bool active)
        {
            if (trajectoryDots == null)
            {
                return;
            }

            for (int i = 0; i < trajectoryDots.Length; i++)
            {
                if (trajectoryDots[i] != null)
                {
                    trajectoryDots[i].gameObject.SetActive(active);
                }
            }
        }

        private void UpdateSlingshotBands(Vector2 projectilePosition)
        {
            if (slingshotBandLeft != null && slingshotLeftAnchor != null)
            {
                PlaceLine(slingshotBandLeft, slingshotLeftAnchor.anchoredPosition, projectilePosition, 6f);
            }

            if (slingshotBandRight != null && slingshotRightAnchor != null)
            {
                PlaceLine(slingshotBandRight, slingshotRightAnchor.anchoredPosition, projectilePosition, 6f);
            }
        }

        private void SetSlingshotBandsActive(bool active)
        {
            if (slingshotBandLeft != null)
            {
                slingshotBandLeft.gameObject.SetActive(active);
            }

            if (slingshotBandRight != null)
            {
                slingshotBandRight.gameObject.SetActive(active);
            }
        }

        private void UpdatePowerVisual(float normalizedPower)
        {
            float power = Mathf.Clamp01(normalizedPower);
            if (launchPowerFill != null)
            {
                launchPowerFill.fillAmount = power;
            }

            if (launchHintText != null)
            {
                launchHintText.text = power > 0.03f ? "POWER " + Mathf.RoundToInt(power * 100f) + "%" : "DRAG PATCH CORE";
            }
        }

        private QuizQuestion GetQuestionForOrb(int orbIndex)
        {
            if (orbIndex < 0 || orbIndex >= quizOrbs.Count || quizOrbs[orbIndex] == null)
            {
                return null;
            }

            int seed = orbIndex + score + launchedAttacks;
            if (quizQuestionBank != null)
            {
                return quizQuestionBank.GetQuestion(quizOrbs[orbIndex].category, seed, FallbackQuestions);
            }

            return FallbackQuestions[Mathf.Abs(quizOrbs[orbIndex].category) % FallbackQuestions.Length];
        }

        private DifficultyProfile GetActiveDifficulty()
        {
            if (difficultyProfiles != null && difficultyProfiles.Length > 0)
            {
                currentDifficultyIndex = Mathf.Clamp(currentDifficultyIndex, 0, difficultyProfiles.Length - 1);
                if (difficultyProfiles[currentDifficultyIndex] != null)
                {
                    return difficultyProfiles[currentDifficultyIndex];
                }
            }

            return defaultDifficulty;
        }

        private string GetDifficultyName()
        {
            DifficultyProfile difficulty = GetActiveDifficulty();
            if (difficulty != null && !string.IsNullOrWhiteSpace(difficulty.displayName))
            {
                return difficulty.displayName;
            }

            return "Normal";
        }

        private void RefreshOrbVisuals()
        {
            for (int i = 0; i < quizOrbs.Count; i++)
            {
                QuizOrbNode orb = quizOrbs[i];
                if (orb == null)
                {
                    continue;
                }

                if (orb.fill != null)
                {
                    orb.fill.color = categoryColors[Mathf.Abs(orb.category) % categoryColors.Length];
                }

                if (orb.label != null)
                {
                    orb.label.text = GetCategoryCode(orb.category);
                }

                if (orb.button != null)
                {
                    bool quizOpen = quizModal != null && quizModal.activeSelf;
                    orb.button.interactable = !levelEnded && !paused && !attackInFlight && !isDraggingSlingshot && !quizOpen && !orb.cleared;
                }
            }
        }

        private string GetCategoryCode(int category)
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

        private void RefreshRouteVisual()
        {
            int blocker = FindFirstBlockingOrb();
            bool routeClear = blocker < 0;
            Vector2 start = guardianAnchor != null ? guardianAnchor.anchoredPosition : new Vector2(-470f, -98f);
            Vector2 end = routeClear
                ? (virusAnchor != null ? virusAnchor.anchoredPosition : new Vector2(500f, 20f))
                : quizOrbs[blocker].rectTransform.anchoredPosition;

            if (attackBeam != null)
            {
                PlaceLine(attackBeam, start, end, routeClear ? 7f : 4f);
                Image lineImage = attackBeam.GetComponent<Image>();
                if (lineImage != null)
                {
                    lineImage.color = routeClear ? new Color(0.23f, 0.95f, 0.62f, 0.72f) : new Color(1f, 0.67f, 0.22f, 0.55f);
                }
            }

            if (routeText != null)
            {
                routeText.text = routeClear ? "ROUTE CLEAR" : "ROUTE BLOCKED";
            }

            if (attackButtonLabel != null)
            {
                attackButtonLabel.text = "DRAG PATCH CORE";
            }
        }

        private void PlaceLine(RectTransform line, Vector2 start, Vector2 end, float thickness)
        {
            Vector2 delta = end - start;
            line.anchoredPosition = start + delta * 0.5f;
            line.sizeDelta = new Vector2(delta.magnitude, thickness);
            line.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }

        private void RefreshHud()
        {
            bool quizOpen = quizModal != null && quizModal.activeSelf;
            if (shieldText != null)
            {
                shieldText.text = "SHIELD " + shieldIntegrity + "%";
            }

            if (virusText != null)
            {
                virusText.text = "VIRUS " + virusStrength + "%";
            }

            if (scoreText != null)
            {
                scoreText.text = "SCORE " + score.ToString("0000");
            }

            if (difficultyText != null)
            {
                difficultyText.text = GetDifficultyName().ToUpperInvariant();
            }

            if (shieldFill != null)
            {
                shieldFill.fillAmount = shieldIntegrity / 100f;
            }

            if (virusFill != null)
            {
                virusFill.fillAmount = virusStrength / 100f;
            }

            if (pauseButtonLabel != null)
            {
                pauseButtonLabel.text = paused ? "RESUME" : "PAUSE";
            }

            if (attackButton != null)
            {
                attackButton.interactable = !levelEnded && !paused && !attackInFlight && !quizOpen;
            }

            RefreshOrbVisuals();
        }

        private void PulseVisuals()
        {
            float pulse = 0.55f + Mathf.PingPong(Time.unscaledTime * 1.7f, 0.45f);
            guardianBoostPulse = Mathf.MoveTowards(guardianBoostPulse, 0f, Time.unscaledDeltaTime * 1.45f);
            virusBoostPulse = Mathf.MoveTowards(virusBoostPulse, 0f, Time.unscaledDeltaTime * 1.15f);

            if (guardianShieldGlow != null)
            {
                Color color = guardianShieldGlow.color;
                color.a = Mathf.Clamp01(Mathf.Lerp(0.20f, 0.52f, pulse) * Mathf.Clamp01(shieldIntegrity / 100f) + guardianBoostPulse * 0.34f);
                guardianShieldGlow.color = color;
                guardianShieldGlow.rectTransform.localScale = Vector3.one * (Mathf.Lerp(0.96f, 1.08f, pulse) + guardianBoostPulse * 0.20f);
            }

            if (virusGlow != null)
            {
                Color color = virusGlow.color;
                color.a = Mathf.Clamp01(Mathf.Lerp(0.20f, 0.68f, pulse) * Mathf.Clamp01(virusStrength / 100f) + virusBoostPulse * 0.38f);
                virusGlow.color = color;
                virusGlow.rectTransform.localScale = Vector3.one * (Mathf.Lerp(0.94f, 1.16f, pulse) + virusBoostPulse * 0.25f);
            }
        }

        private void TogglePause()
        {
            if (levelEnded)
            {
                return;
            }

            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;
            SetStatus(paused ? "PAUSED" : "HIT QUIZ FIREWALL ORBS, ANSWER CORRECTLY, THEN OPEN A PATH");
            RefreshOrbVisuals();
            RefreshHud();
        }

        private void EndLevel(bool victory)
        {
            levelEnded = true;
            CloseQuiz();
            Time.timeScale = 1f;
            SetStatus(victory ? "LEVEL CLEAR - VIRUS PURGED" : "GAME OVER - VIRUS BREACHED THE SHIELD");
            SetSlingshotBandsActive(false);
            SetTrajectoryDotsActive(false);
            if (attackProjectile != null)
            {
                attackProjectile.gameObject.SetActive(false);
            }

            RefreshOrbVisuals();
            RefreshRouteVisual();
            RefreshHud();
        }

        private void ReturnToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        private void PlaySfx(AudioClip clip)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }
}
