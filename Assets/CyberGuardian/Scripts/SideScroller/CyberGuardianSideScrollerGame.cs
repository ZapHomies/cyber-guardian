using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CyberGuardian
{
    public sealed class CyberGuardianSideScrollerGame : MonoBehaviour
    {
        private enum GameMode
        {
            Adventure,
            BossSlingshot,
            Victory,
            Defeat
        }

        public string menuSceneName = "CyberGuardian_MainMenu";
        public QuizQuestionBank quizQuestionBank;
        public DifficultyProfile[] difficultyProfiles;
        public CyberGuardianPlayerController player;
        public CyberGuardianBossCore bossCore;
        public List<CyberGuardianEnemy> enemies = new List<CyberGuardianEnemy>();
        public List<CyberGuardianBossShieldBlock> bossBlocks = new List<CyberGuardianBossShieldBlock>();
        public List<GameObject> aerialBossBreakawayBlocks = new List<GameObject>();
        public Camera gameplayCamera;
        public Transform bossProjectileSpawn;
        public Transform[] bossProjectileSpawns;
        public Transform slingshotProjectile;
        public Rigidbody2D slingshotBody;
        public Collider2D slingshotCollider;
        public LineRenderer slingshotBandA;
        public LineRenderer slingshotBandB;
        public LineRenderer trajectoryLine;
        public GameObject bossProjectilePrefab;
        public GameObject meleeFlash;
        public Sprite deathShardSprite;
        public string nextSceneName = string.Empty;

        public Text healthText;
        public Text livesText;
        public Image[] lifeIconImages;
        public Text bossText;
        public Text scoreText;
        public Text modeText;
        public Text statusText;
        public GameObject storyPanel;
        public Text storyTitleText;
        public Text storyBodyText;
        public GameObject educationPanel;
        public Text educationTitleText;
        public Text educationBodyText;
        public Image educationIconImage;
        public Button educationContinueButton;
        public GameObject bossDialoguePanel;
        public Text bossDialogueTitleText;
        public Text bossDialogueBodyText;
        public Image bossDialoguePortraitImage;
        public Button bossDialogueContinueButton;
        public RectTransform bossDialoguePanelRect;
        public RectTransform bossDialoguePortraitRect;
        public CanvasGroup bossDialogueCanvasGroup;
        public Sprite bossDialogueIdlePortraitSprite;
        public Sprite bossDialogueIntroPortraitSprite;
        public Image playerHealthFill;
        public Image bossHealthFill;
        public Image boostEnergyFill;
        public GameObject bossHudGroup;
        public Button pauseButton;
        public Button menuButton;
        public Button resetButton;
        public GameObject pauseModal;
        public Button pauseResumeButton;
        public Button pauseRetryButton;
        public Button pauseMenuButton;
        public GameObject gameOverModal;
        public Text gameOverScoreText;
        public Button gameOverRetryButton;
        public Button gameOverMenuButton;
        public GameObject readyPanel;
        public Image readyDimImage;
        public Text readyTitleText;
        public Text readyCountdownText;
        public Image[] readyPulseImages;

        public GameObject quizModal;
        public Text quizTitleText;
        public Text quizPromptText;
        public Text feedbackText;
        public Button[] answerButtons;
        public Text[] answerLabels;
        public Button closeQuizButton;

        public AudioSource sfxSource;
        public AudioClip meleeSfx;
        public AudioClip hitSfx;
        public AudioClip jumpSfx;
        public AudioClip bossShotSfx;
        public AudioClip shieldSfx;
        public AudioClip wrongSfx;
        public AudioSource musicSource;
        public AudioClip adventureMusic;
        public AudioClip bossMusic;
        public AudioClip playerHitSfx;
        public AudioClip playerDeathSfx;
        public AudioClip playerBoostSfx;
        public AudioClip playerRecoverySfx;
        public AudioClip playerShootSfx;
        public AudioClip checkpointSfx;
        public AudioClip quizOpenSfx;
        public AudioClip quizCorrectSfx;
        public AudioClip quizWrongSfx;
        public AudioClip bossShieldBreakSfx;
        public AudioClip bossDamageSfx;
        public AudioClip bossDefeatSfx;
        public AudioClip bossWarningSfx;
        public AudioClip bossSlingshotPullSfx;
        public AudioClip bossSlingshotLaunchSfx;
        public AudioClip enemyDeathSfx;
        public AudioClip powerupEnergySfx;
        public AudioClip powerupHealthSfx;
        public AudioClip countdownBeepSfx;
        public AudioClip countdownStartSfx;
        public AudioClip gameOverSfx;

        public float bossArenaCenterX = 35.5f;
        public float bossArenaMinX = 28.0f;
        public float bossArenaMaxX = 37.2f;
        public Vector2 slingshotRestOffset = new Vector2(0f, 0.54f);
        public float slingshotMaxPull = 2.35f;
        public float slingshotMinPull = 0.36f;
        public float slingshotPullScreenScale = 0.35f;
        public float slingshotPower = 8.8f;
        public float slingshotMinLaunchSpeed = 5.0f;
        public float slingshotMaxLaunchSpeed = 24.0f;
        public float slingshotDirectShotMaxSpeed = 25.0f;
        public float projectileMaxFlightTime = 4.8f;
        public bool aerialBossEncounter;
        public float bossAttackIntervalScale = 1f;
        public int bossVolleyCount = 1;
        public float bossProjectileSpeedBonus;
        public float bossCameraSize = 5.4f;
        public float aerialBossCameraSize = 7.6f;
        public float aerialBossCameraXOffset = 3.8f;
        public float aerialBossCameraYOffset = 1.35f;
        public float electricDeathCameraSize = 2.05f;
        public Vector2 cameraMin = new Vector2(-8f, -3.4f);
        public Vector2 cameraMax = new Vector2(78f, 5.4f);
        public Vector3 startingRecoveryPoint = new Vector3(-8f, 0.65f, 0f);
        public Transform bossCinematicTarget;
        public Transform bossVisualRoot;
        public string sceneIntroTitle = "SEKTOR BARU";
        [TextArea(2, 4)] public string sceneIntroBody = "Pelajari pola ancaman, lewati rintangan, lalu gunakan jawaban kuis untuk membuka celah serangan.";
        public string bossThreatTitle = "BOS MALWARE";
        [TextArea(2, 4)] public string bossThreatBody = "Aku mengunci jalur ini. Jawab blok kuis, buka celah, dan tembak inti patch jika berani.";
        public string bossInfoTitle = "KARTU ANCAMAN";
        [TextArea(2, 5)] public string bossInfoBody = "Malware mencoba mencuri data, merusak sistem, dan membuka pintu bagi serangan berikutnya.";
        public string bossDefeatTitle = "ANCAMAN DINETRALISIR";
        [TextArea(2, 4)] public string bossDefeatBody = "Inti malware pecah. Sistem kembali stabil karena kamu memahami lapisan pertahanan yang tepat.";
        public string bossLessonTitle = "PELAJARAN SEKTOR";
        [TextArea(2, 5)] public string bossLessonBody = "Gunakan kebiasaan aman: cek sumber, perbarui sistem, aktifkan perlindungan akun, dan jangan menjalankan file mencurigakan.";
        public int maxLives = 3;
        [Range(1, 3)] public int quizLessonTier = 1;

        private static readonly QuizQuestion[] FallbackQuestions =
        {
            new QuizQuestion(CyberQuestionCategory.Password, "GERBANG PASSWORD", "Password yang baik sebaiknya...", new[] { "Panjang, unik, dan sulit ditebak", "Sama untuk semua akun", "Berisi tanggal lahir", "Dibagikan ke teman" }, 0, "Benar. Password perlu panjang, unik, dan tidak dipakai ulang."),
            new QuizQuestion(CyberQuestionCategory.Malware, "BLOK MALWARE", "Lampiran asing dari email tidak dikenal sebaiknya...", new[] { "Tidak dibuka sembarangan", "Langsung dijalankan", "Dibagikan ulang", "Diubah namanya saja" }, 0, "Benar. Lampiran asing bisa membawa malware."),
            new QuizQuestion(CyberQuestionCategory.Network, "DINDING JARINGAN", "Firewall membantu kita untuk...", new[] { "Menyaring koneksi berbahaya", "Membuka semua port", "Membuat virus", "Mematikan update" }, 0, "Benar. Firewall membantu mengontrol koneksi masuk dan keluar."),
            new QuizQuestion(CyberQuestionCategory.Privacy, "KUNCI PRIVASI", "Data yang tidak boleh dibagikan sembarangan adalah...", new[] { "OTP, password, NIK", "Genre game favorit", "Warna kesukaan", "Nama panggilan" }, 0, "Benar. Data sensitif dapat dipakai untuk penipuan.")
        };

        private readonly Color[] categoryColors =
        {
            new Color(0.18f, 0.58f, 1f, 1f),
            new Color(0.40f, 0.86f, 0.28f, 1f),
            new Color(1.00f, 0.78f, 0.18f, 1f),
            new Color(0.78f, 0.35f, 1f, 1f)
        };

        private const int MaxScore = 999999;

        private GameMode mode = GameMode.Adventure;
        private CyberGuardianBossShieldBlock activeQuizBlock;
        private QuizQuestion activeQuizQuestion;
        private int[] displayedAnswerIndices = new int[0];
        private DifficultyProfile activeDifficulty;
        private int currentDifficultyIndex = 1;
        private int playerHealth;
        private int playerLives;
        private int bossHealth;
        private int score;
        private float boostEnergy = 100f;
        private float invulnerabilityTimer;
        private float bossFireTimer = 1.0f;
        private Vector3 currentRecoveryPoint;
        private bool quizOpen;
        private bool draggingSlingshot;
        private bool projectileInFlight;
        private float projectileFlightTimer;
        private bool defeatSequenceStarted;
        private bool paused;
        private bool introCountdownActive = true;
        private bool resumedFromCheckpoint;
        private int bossAttackPatternIndex;
        private Coroutine bossDialogueRoutine;
        private float bossDialoguePreviousTimeScale = 1f;
        private float bossDialoguePreviousFixedDeltaTime = 0.02f;
        private float bossDialogueHoldSeconds = 3.45f;
        private bool bossDialogueRequiresContinue;
        private bool bossDialogueSlowActive;
        private bool developerMode;
        private bool electricShockSequenceActive;
        private bool cinematicCameraActive;
        private Vector3 cinematicCameraTarget;
        private float cinematicCameraSize = 5.4f;
        private bool bossDefeatSequenceActive;
        private bool narrativeOpen;
        private bool narrativeContinueRequested;
        private bool aerialBossTransitionStarted;

        public bool PlayerInputEnabled => mode != GameMode.Victory && mode != GameMode.Defeat && !quizOpen && !paused && !introCountdownActive && !electricShockSequenceActive && !narrativeOpen;

        private void Awake()
        {
            currentDifficultyIndex = Mathf.Clamp(PlayerPrefs.GetInt(CyberGuardianMainMenu.DifficultyKey, 1), 0, 2);
            developerMode = CyberGuardianMainMenu.IsDeveloperModeEnabled();
            CyberGuardianMainMenu.ApplyAudioPreferencesToScene();
            if (developerMode)
            {
                maxLives = Mathf.Max(maxLives, 99);
            }

            activeDifficulty = GetActiveDifficulty();
            playerHealth = activeDifficulty != null ? activeDifficulty.startingShield : 100;
            playerLives = Mathf.Max(1, maxLives);
            bossHealth = 100;
            score = 0;
            boostEnergy = 100f;
            currentRecoveryPoint = startingRecoveryPoint;

            if (player != null)
            {
                player.game = this;
                currentRecoveryPoint = player.transform.position + Vector3.up * 0.2f;
            }

            resumedFromCheckpoint = TryApplySavedContinue();
            ApplyDeveloperModeVitals(false);

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] != null)
                {
                    enemies[i].game = this;
                }
            }

            for (int i = 0; i < bossBlocks.Count; i++)
            {
                if (bossBlocks[i] != null)
                {
                    bossBlocks[i].game = this;
                }
            }

            WireUi();
            ResetSlingshotProjectile();
            if (quizModal != null)
            {
                quizModal.SetActive(false);
            }

            if (gameOverModal != null)
            {
                gameOverModal.SetActive(false);
            }

            if (pauseModal != null)
            {
                pauseModal.SetActive(false);
            }

            if (storyPanel != null)
            {
                storyPanel.SetActive(false);
            }

            if (educationPanel != null)
            {
                educationPanel.SetActive(false);
            }

            if (bossDialoguePanel != null)
            {
                bossDialoguePanel.SetActive(false);
            }

            if (readyPanel != null)
            {
                readyPanel.SetActive(false);
            }

            StartLoopingMusic(adventureMusic);
            SetStatus("MODE PETUALANGAN: A/D GERAK, SPACE LOMPAT, J SERANG, SHIFT ENERGI");
            RefreshHud();
            StartCoroutine(SceneOpeningSequence());
        }

        private void Update()
        {
            if (!introCountdownActive && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)))
            {
                TogglePause();
            }

            if (paused)
            {
                return;
            }

            invulnerabilityTimer = Mathf.Max(0f, invulnerabilityTimer - Time.deltaTime);
            RegenerateBoost();
            ApplyDeveloperModeVitals(true);
            UpdateCamera();

            if (mode == GameMode.Adventure && player != null && player.transform.position.x >= bossArenaMinX - 0.35f)
            {
                EnterBossMode();
            }

            if (mode == GameMode.BossSlingshot)
            {
                ClampPlayerToBossArena();
                if (!narrativeOpen)
                {
                    HandleSlingshotInput();
                    HandleBossAttack();
                }
            }

            if (projectileInFlight)
            {
                MonitorSlingshotProjectile();
            }

            if (player != null && player.transform.position.y < -8f)
            {
                FallIntoElectricRiver();
            }
        }

        private IEnumerator SceneOpeningSequence()
        {
            introCountdownActive = true;
            Time.timeScale = 1f;
            RestoreBossDialogueTimeScale();
            HideStoryPanel();
            if (educationPanel != null)
            {
                educationPanel.SetActive(false);
            }

            Vector3 playerFocus = player != null ? player.transform.position + new Vector3(2.2f, 1.0f, -10f) : new Vector3(startingRecoveryPoint.x, startingRecoveryPoint.y + 1.0f, -10f);
            Vector3 bossFocus = ResolveBossCinematicPosition();
            cinematicCameraActive = true;
            cinematicCameraSize = aerialBossEncounter ? Mathf.Max(bossCameraSize, aerialBossCameraSize) : Mathf.Max(4.8f, bossCameraSize);
            cinematicCameraTarget = bossFocus;
            if (gameplayCamera != null)
            {
                gameplayCamera.transform.position = bossFocus;
                gameplayCamera.orthographicSize = cinematicCameraSize;
            }

            ShowBossDialogue(bossThreatTitle, bossThreatBody, 1.45f, true);
            while (bossDialogueRoutine != null)
            {
                yield return null;
            }

            yield return ShowEducationCardRoutine(bossInfoTitle, bossInfoBody, 3.1f);
            ShowStory(sceneIntroTitle, sceneIntroBody, 3.0f);
            yield return new WaitForSecondsRealtime(1.35f);

            float flyDuration = 2.45f;
            float elapsed = 0f;
            Vector3 start = gameplayCamera != null ? gameplayCamera.transform.position : bossFocus;
            float startSize = gameplayCamera != null ? gameplayCamera.orthographicSize : cinematicCameraSize;
            while (elapsed < flyDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / flyDuration);
                float eased = t * t * (3f - 2f * t);
                cinematicCameraTarget = Vector3.Lerp(start, playerFocus, eased);
                cinematicCameraSize = Mathf.Lerp(startSize, 5.4f, eased);
                yield return null;
            }

            cinematicCameraActive = false;
            HideStoryPanel();
            yield return ReadyCountdownSequence();
        }

        private Vector3 ResolveBossCinematicPosition()
        {
            Transform target = bossCinematicTarget != null ? bossCinematicTarget : (bossCore != null ? bossCore.transform : null);
            if (target != null)
            {
                return new Vector3(target.position.x, target.position.y + 0.35f, -10f);
            }

            float y = aerialBossEncounter ? 5.4f : 2.1f;
            return new Vector3(bossArenaCenterX + (aerialBossEncounter ? aerialBossCameraXOffset : 0f), y, -10f);
        }

        private IEnumerator ReadyCountdownSequence()
        {
            introCountdownActive = true;
            Time.timeScale = 1f;
            if (readyPanel != null)
            {
                readyPanel.SetActive(true);
            }

            SetStatus("URUTAN BOOT MISI");
            string title = resumedFromCheckpoint ? "CHECKPOINT DIPULIHKAN" : "SIAP?";
            yield return AnimateReadyStep(title, string.Empty, 0.85f);
            yield return AnimateReadyStep(title, "3", 0.58f);
            yield return AnimateReadyStep(title, "2", 0.58f);
            yield return AnimateReadyStep(title, "1", 0.58f);
            yield return AnimateReadyStep("MULAI!", "LARI", 0.48f);

            introCountdownActive = false;
            if (readyPanel != null)
            {
                readyPanel.SetActive(false);
            }

            SetStatus("MODE PETUALANGAN: A/D GERAK, SPACE LOMPAT, J SERANG, SHIFT ENERGI");
        }

        private IEnumerator AnimateReadyStep(string title, string countdown, float duration)
        {
            if (!string.IsNullOrEmpty(countdown))
            {
                PlaySfx(title == "MULAI!" ? countdownStartSfx : countdownBeepSfx);
            }

            if (readyTitleText != null)
            {
                readyTitleText.text = title;
            }

            if (readyCountdownText != null)
            {
                readyCountdownText.text = countdown;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, duration));
                float pulse = Mathf.Sin(t * Mathf.PI);

                if (readyDimImage != null)
                {
                    readyDimImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(0.42f, 0.66f, pulse));
                }

                if (readyTitleText != null)
                {
                    readyTitleText.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.72f, 1f, pulse));
                    readyTitleText.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.94f, 1.03f, pulse);
                }

                if (readyCountdownText != null)
                {
                    readyCountdownText.color = new Color(0.38f, 0.97f, 1f, Mathf.Lerp(0.70f, 1f, pulse));
                    readyCountdownText.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.82f, 1.22f, pulse);
                }

                if (readyPulseImages != null)
                {
                    for (int i = 0; i < readyPulseImages.Length; i++)
                    {
                        Image image = readyPulseImages[i];
                        if (image == null)
                        {
                            continue;
                        }

                        float phase = Mathf.Repeat(t + i * 0.18f, 1f);
                        Color color = image.color;
                        color.a = Mathf.Sin(phase * Mathf.PI) * 0.72f;
                        image.color = color;
                    }
                }

                yield return null;
            }
        }

        public void EnterBossMode()
        {
            if (mode != GameMode.Adventure)
            {
                return;
            }

            mode = GameMode.BossSlingshot;
            SyncPlayerBossMovementMode();
            if (aerialBossEncounter && player != null)
            {
                currentRecoveryPoint = new Vector3(bossArenaMinX + 1.25f, Mathf.Clamp(player.transform.position.y + 1.15f, 3.35f, 5.65f), player.transform.position.z);
                player.transform.position = currentRecoveryPoint;
                SaveProgress(currentRecoveryPoint);
                StartAerialBossTransition();
            }

            bossFireTimer = 1.25f;
            ResetSlingshotProjectile();
            PlaySfx(bossWarningSfx);
            StartLoopingMusic(bossMusic);
            ShowBossDialogue();
            SetStatus(aerialBossEncounter ? "MODE BOS UDARA: TERBANG DENGAN WASD/PANAH, TARIK INTI PATCH UNTUK MENEMBUS PERISAI" : "MODE BOS: KLIK ATAU TARIK DI MANA SAJA UNTUK MENARIK INTI PATCH");
            RefreshHud();
        }

        private void SyncPlayerBossMovementMode()
        {
            if (player == null)
            {
                return;
            }

            bool bossMode = mode == GameMode.BossSlingshot;
            player.InBossMode = bossMode;
            player.SetFlightMode(bossMode && aerialBossEncounter);
        }

        private void StartAerialBossTransition()
        {
            if (!aerialBossEncounter || aerialBossTransitionStarted)
            {
                return;
            }

            EnsureAerialBossBreakawayBlocks();
            aerialBossTransitionStarted = true;
            StartCoroutine(AerialBossEntryBreakawaySequence());
        }

        private void EnsureAerialBossBreakawayBlocks()
        {
            if (aerialBossBreakawayBlocks == null)
            {
                aerialBossBreakawayBlocks = new List<GameObject>();
            }

            aerialBossBreakawayBlocks.RemoveAll(block => block == null);
            if (aerialBossBreakawayBlocks.Count > 0)
            {
                return;
            }

            string[] breakawayNames =
            {
                "L03 Aerial Boss Lower Dodge Bridge",
                "L03 Aerial Boss Left Floating Shelf",
                "L03 Aerial Boss Right Floating Shelf"
            };

            for (int i = 0; i < breakawayNames.Length; i++)
            {
                GameObject block = GameObject.Find(breakawayNames[i]);
                if (block != null)
                {
                    aerialBossBreakawayBlocks.Add(block);
                }
            }
        }

        private IEnumerator AerialBossEntryBreakawaySequence()
        {
            SetStatus("ARENA DARAT RUNTUH - MODE TERBANG AKTIF");
            PlaySfx(bossShieldBreakSfx != null ? bossShieldBreakSfx : shieldSfx);
            if (aerialBossBreakawayBlocks == null)
            {
                yield break;
            }

            for (int i = 0; i < aerialBossBreakawayBlocks.Count; i++)
            {
                GameObject block = aerialBossBreakawayBlocks[i];
                if (block == null || !block.activeInHierarchy)
                {
                    continue;
                }

                StartCoroutine(CollapseAerialBossBlock(block, i));
                yield return new WaitForSecondsRealtime(0.07f);
            }
        }

        private IEnumerator CollapseAerialBossBlock(GameObject block, int index)
        {
            Collider2D[] colliders = block.GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                {
                    colliders[i].enabled = false;
                }
            }

            SpriteRenderer[] renderers = block.GetComponentsInChildren<SpriteRenderer>();
            int fragmentBudget = 18;
            for (int i = 0; i < renderers.Length && fragmentBudget > 0; i++)
            {
                SpriteRenderer renderer = renderers[i];
                if (renderer == null || renderer.sprite == null || renderer.color.a < 0.05f)
                {
                    continue;
                }

                SpawnAerialBreakawayFragment(renderer, i + index * 7);
                fragmentBudget--;
            }

            float elapsed = 0f;
            const float duration = 0.42f;
            Vector3 baseScale = block.transform.localScale;
            while (elapsed < duration && block != null)
            {
                float deltaTime = Time.unscaledDeltaTime;
                elapsed += deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float shake = Mathf.Sin(t * Mathf.PI * 8f) * (1f - t) * 0.05f;
                block.transform.localScale = baseScale * Mathf.Lerp(1f, 0.72f, t);
                block.transform.position += new Vector3(shake, -deltaTime * 1.2f, 0f);
                for (int i = 0; i < renderers.Length; i++)
                {
                    SpriteRenderer renderer = renderers[i];
                    if (renderer == null)
                    {
                        continue;
                    }

                    Color color = renderer.color;
                    color.a = Mathf.Lerp(color.a, 0f, t);
                    renderer.color = color;
                }

                yield return null;
            }

            if (block != null)
            {
                block.SetActive(false);
            }
        }

        private void SpawnAerialBreakawayFragment(SpriteRenderer source, int seed)
        {
            if (source == null || source.sprite == null)
            {
                return;
            }

            GameObject fragment = new GameObject("Aerial Boss Arena Data Fragment", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D));
            fragment.transform.position = source.transform.position + new Vector3(Random.Range(-0.18f, 0.18f), Random.Range(-0.14f, 0.14f), 0f);
            fragment.transform.localScale = Vector3.one * Random.Range(0.06f, 0.18f);
            fragment.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            SpriteRenderer renderer = fragment.GetComponent<SpriteRenderer>();
            renderer.sprite = source.sprite;
            renderer.color = Color.Lerp(source.color, seed % 2 == 0 ? new Color(0.35f, 1f, 1f, 1f) : new Color(1f, 0.24f, 0.55f, 1f), 0.45f);
            renderer.sortingOrder = Mathf.Max(30, source.sortingOrder + 6);

            Rigidbody2D body = fragment.GetComponent<Rigidbody2D>();
            body.gravityScale = Random.Range(0.18f, 0.48f);
            body.linearVelocity = new Vector2(Random.Range(-3.2f, 3.2f), Random.Range(1.2f, 5.8f));
            body.angularVelocity = Random.Range(-520f, 520f);

            CircleCollider2D collider = fragment.GetComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.08f;
            Destroy(fragment, 1.35f);
        }

        public void PlayerMelee(Vector2 center, float radius, int damage)
        {
            PlaySfx(meleeSfx);
            if (meleeFlash != null)
            {
                meleeFlash.transform.position = center;
                meleeFlash.SetActive(true);
                CancelInvoke(nameof(HideMeleeFlash));
                Invoke(nameof(HideMeleeFlash), 0.08f);
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                CyberGuardianEnemy enemy = enemies[i];
                if (enemy == null)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                if (Vector2.Distance(center, enemy.transform.position) <= radius)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        public void EnemyDefeated(CyberGuardianEnemy enemy)
        {
            enemies.Remove(enemy);
            AddScore(50);
            PlaySfx(enemyDeathSfx != null ? enemyDeathSfx : hitSfx);
            SetStatus("Ancaman dihapus. Terus bergerak.");
            RefreshHud();
        }

        private static string TranslateDamageSource(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "ancaman";
            }

            string normalized = source.ToLowerInvariant();
            if (normalized.Contains("boss packet"))
            {
                return "serangan paket bos";
            }

            if (normalized.Contains("boss electric") || normalized.Contains("listrik bos"))
            {
                return "serangan listrik bos";
            }

            if (normalized.Contains("sungai") || normalized.Contains("electric river") || normalized.Contains("abyss") || normalized.Contains("jurang"))
            {
                return "sungai listrik";
            }

            if (normalized.Contains("enemy"))
            {
                return "kontak musuh";
            }

            if (normalized.Contains("wrong") || normalized.Contains("jawaban salah"))
            {
                return "jawaban salah";
            }

            if (normalized.Contains("skipped") || normalized.Contains("dilewati"))
            {
                return "kuis dilewati";
            }

            if (normalized.Contains("saw"))
            {
                return "gergaji jebakan";
            }

            if (normalized.Contains("laser"))
            {
                return "laser jebakan";
            }

            if (normalized.Contains("spike"))
            {
                return "duri jebakan";
            }

            if (normalized.Contains("glitch"))
            {
                return "ranjau glitch";
            }

            if (normalized.Contains("crusher") || normalized.Contains("crushing"))
            {
                return "blok penghancur";
            }

            return "jebakan";
        }

        public void DamagePlayer(int damage, string source)
        {
            if (mode == GameMode.Defeat || mode == GameMode.Victory || invulnerabilityTimer > 0f)
            {
                return;
            }

            if (developerMode)
            {
                invulnerabilityTimer = 0.15f;
                ApplyDeveloperModeVitals(false);
                SetStatus("MODE DEVELOPER: DAMAGE DARI " + TranslateDamageSource(source).ToUpperInvariant() + " DIABAIKAN");
                RefreshHud();
                return;
            }

            invulnerabilityTimer = 0.75f;
            playerHealth = Mathf.Max(0, playerHealth - damage);
            PlaySfx(GetDamageSfx(source));
            SetStatus(playerHealth <= 0 ? "NYAWA GUARDIAN TERKENA KRITIS" : "TERKENA " + TranslateDamageSource(source).ToUpperInvariant());
            RefreshHud();

            if (playerHealth <= 0)
            {
                if (!TrySpendLifeAndRespawn(source))
                {
                    SetStatus("SISTEM JEBOL - PERMAINAN BERAKHIR");
                    BeginDefeatSequence(source);
                }
            }
        }

        private bool TrySpendLifeAndRespawn(string source)
        {
            if (playerLives <= 1)
            {
                playerLives = 0;
                RefreshHud();
                return false;
            }

            playerLives--;
            playerHealth = GetRespawnHealth();
            boostEnergy = 100f;
            invulnerabilityTimer = 2.0f;
            quizOpen = false;
            draggingSlingshot = false;
            projectileInFlight = false;
            electricShockSequenceActive = false;
            projectileFlightTimer = 0f;
            HideSlingshotLines();

            if (quizModal != null)
            {
                quizModal.SetActive(false);
            }

            if (player != null)
            {
                Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
                if (playerBody != null)
                {
                    playerBody.simulated = true;
                    playerBody.linearVelocity = Vector2.zero;
                    playerBody.angularVelocity = 0f;
                }

                Collider2D playerCollider = player.GetComponent<Collider2D>();
                if (playerCollider != null)
                {
                    playerCollider.enabled = true;
                }

                SetPlayerRenderersEnabled(true);
                player.transform.position = currentRecoveryPoint;
                SyncPlayerBossMovementMode();
            }

            ResetSlingshotProjectile();
            SaveProgress(currentRecoveryPoint);
            PlaySfx(playerRecoverySfx);
            SetStatus("NYAWA BERKURANG: " + playerLives + " TERSISA - KEMBALI KE CHECKPOINT");
            RefreshHud();
            return true;
        }

        private void BeginDefeatSequence(string source)
        {
            if (defeatSequenceStarted)
            {
                return;
            }

            defeatSequenceStarted = true;
            SetPaused(false);
            mode = GameMode.Defeat;
            electricShockSequenceActive = false;
            introCountdownActive = false;
            PlaySfx(playerDeathSfx);
            quizOpen = false;
            draggingSlingshot = false;
            projectileInFlight = false;
            HideSlingshotLines();
            if (quizModal != null)
            {
                quizModal.SetActive(false);
            }

            if (player != null)
            {
                player.InBossMode = false;
                player.SetFlightMode(false);
                Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
                if (playerBody != null)
                {
                    playerBody.linearVelocity = Vector2.zero;
                    playerBody.angularVelocity = 0f;
                    playerBody.simulated = false;
                }

                Collider2D playerCollider = player.GetComponent<Collider2D>();
                if (playerCollider != null)
                {
                    playerCollider.enabled = false;
                }
            }

            SetStatus("SISTEM JEBOL - GUARDIAN HANCUR");
            StartCoroutine(DefeatSequence(source));
        }

        private IEnumerator DefeatSequence(string source)
        {
            SpawnDeathFragments();
            SetPlayerRenderersEnabled(false);
            yield return new WaitForSeconds(1.05f);
            ShowGameOverModal(source);
        }

        private void SpawnDeathFragments()
        {
            if (player == null)
            {
                return;
            }

            SpriteRenderer playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
            Sprite shardSprite = deathShardSprite != null ? deathShardSprite : (playerRenderer != null ? playerRenderer.sprite : null);
            Vector3 origin = player.transform.position + new Vector3(0f, 0.25f, 0f);
            for (int i = 0; i < 22; i++)
            {
                GameObject shard = new GameObject("Guardian Data Shard", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D));
                shard.transform.position = origin + new Vector3(Random.Range(-0.22f, 0.22f), Random.Range(-0.16f, 0.24f), 0f);
                shard.transform.localScale = Vector3.one * Random.Range(0.055f, 0.14f);

                SpriteRenderer renderer = shard.GetComponent<SpriteRenderer>();
                renderer.sprite = shardSprite;
                renderer.color = Color.Lerp(new Color(0.35f, 1f, 1f, 1f), new Color(1f, 0.22f, 0.36f, 1f), Random.value);
                renderer.sortingOrder = 42;

                Rigidbody2D body = shard.GetComponent<Rigidbody2D>();
                Vector2 direction = Random.insideUnitCircle.normalized;
                if (direction.sqrMagnitude < 0.1f)
                {
                    direction = Vector2.up;
                }

                body.gravityScale = 0.85f;
                body.linearVelocity = direction * Random.Range(2.8f, 7.3f) + Vector2.up * Random.Range(0.6f, 2.4f);
                body.angularVelocity = Random.Range(-520f, 520f);

                CircleCollider2D collider = shard.GetComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = 0.10f;
                Destroy(shard, 1.8f);
            }
        }

        private void SpawnElectricShockBurst(Vector3 origin)
        {
            SpriteRenderer playerRenderer = player != null ? player.GetComponentInChildren<SpriteRenderer>() : null;
            Sprite sparkSprite = deathShardSprite != null ? deathShardSprite : (playerRenderer != null ? playerRenderer.sprite : null);
            for (int i = 0; i < 8; i++)
            {
                GameObject spark = new GameObject("Electric River Shock Spark", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D));
                spark.transform.position = origin + new Vector3(Random.Range(-0.18f, 0.18f), Random.Range(-0.12f, 0.18f), 0f);
                spark.transform.localScale = new Vector3(Random.Range(0.055f, 0.13f), Random.Range(0.018f, 0.05f), 1f);
                spark.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

                SpriteRenderer renderer = spark.GetComponent<SpriteRenderer>();
                renderer.sprite = sparkSprite;
                renderer.color = i % 2 == 0 ? new Color(0.34f, 1f, 1f, 1f) : new Color(1f, 0.18f, 0.55f, 0.9f);
                renderer.sortingOrder = 44;

                Rigidbody2D body = spark.GetComponent<Rigidbody2D>();
                Vector2 direction = Random.insideUnitCircle.normalized;
                if (direction.sqrMagnitude < 0.1f)
                {
                    direction = Vector2.up;
                }

                body.gravityScale = 0.15f;
                body.linearVelocity = direction * Random.Range(2.6f, 6.4f);
                body.angularVelocity = Random.Range(-720f, 720f);

                CircleCollider2D collider = spark.GetComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = 0.06f;
                Destroy(spark, 0.72f);
            }
        }

        private void SetPlayerRenderersEnabled(bool enabled)
        {
            if (player == null)
            {
                return;
            }

            Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].enabled = enabled;
                }
            }
        }

        private void ShowGameOverModal(string source)
        {
            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = "SKOR " + score.ToString("0");
            }

            if (gameOverModal != null)
            {
                gameOverModal.SetActive(true);
            }

            PlaySfx(gameOverSfx);
        }

        public void ProjectileHitShieldBlock(CyberGuardianBossShieldBlock block)
        {
            if (!projectileInFlight || block == null || block.cleared)
            {
                return;
            }

            projectileInFlight = false;
            projectileFlightTimer = 0f;
            FreezeSlingshotProjectile();
            OpenBlockQuiz(block);
        }

        public void PlayerProjectileHitShieldBlock(CyberGuardianBossShieldBlock block)
        {
            if (block == null || block.cleared)
            {
                return;
            }

            if (mode != GameMode.BossSlingshot)
            {
                SetStatus("KUIS FIREWALL HARUS DIBUKA DI MODE BOS");
                return;
            }

            OpenBlockQuiz(block);
        }

        public void ProjectileHitBoss()
        {
            if (!projectileInFlight || mode != GameMode.BossSlingshot)
            {
                return;
            }

            projectileInFlight = false;
            projectileFlightTimer = 0f;
            bossHealth = Mathf.Max(0, bossHealth - GetBossHitDamage());
            AddScore(GetBossHitScoreReward());
            PlaySfx(bossHealth <= 0 ? (bossDefeatSfx != null ? bossDefeatSfx : bossDamageSfx) : (bossDamageSfx != null ? bossDamageSfx : hitSfx));
            SetStatus(bossHealth <= 0 ? "BOS DIBERSIHKAN - LEVEL SELESAI" : "BOS TERKENA. BUKA SUDUT SERANGAN LAIN.");
            ResetSlingshotProjectile();
            RefreshHud();

            if (bossHealth <= 0)
            {
                StartCoroutine(BossDefeatSequence());
            }
        }

        private IEnumerator BossDefeatSequence()
        {
            if (bossDefeatSequenceActive)
            {
                yield break;
            }

            bossDefeatSequenceActive = true;
            mode = GameMode.Victory;
            introCountdownActive = true;
            draggingSlingshot = false;
            projectileInFlight = false;
            HideSlingshotLines();
            if (player != null)
            {
                player.InBossMode = false;
                player.SetFlightMode(false);
            }

            SetStatus("INTI MALWARE RETAK - ANALISIS HASIL");
            cinematicCameraActive = true;
            cinematicCameraTarget = ResolveBossCinematicPosition();
            cinematicCameraSize = aerialBossEncounter ? Mathf.Max(6.6f, aerialBossCameraSize - 0.5f) : Mathf.Max(3.8f, bossCameraSize - 1.2f);
            yield return new WaitForSecondsRealtime(0.85f);
            SpawnBossDestructionFragments();
            yield return new WaitForSecondsRealtime(0.55f);
            ShowStory(bossDefeatTitle, bossDefeatBody, 2.6f);
            yield return new WaitForSecondsRealtime(2.7f);
            yield return ShowEducationCardRoutine(bossLessonTitle, bossLessonBody, 4.25f);

            cinematicCameraActive = false;
            introCountdownActive = false;
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SetStatus("LEVEL SELESAI - MEMBUKA SEKTOR BERIKUTNYA");
                SaveProgressForScene(nextSceneName, GetNextSceneStartPoint(), true);
                LoadNextScene();
            }
            else
            {
                SetStatus("SEMUA SEKTOR DIBERSIHKAN - KOMPUTER AMAN");
            }
        }

        private void SpawnBossDestructionFragments()
        {
            Vector3 origin = bossVisualRoot != null ? bossVisualRoot.position : (bossCore != null ? bossCore.transform.position : new Vector3(bossArenaCenterX, 2f, 0f));
            SpriteRenderer bossRenderer = bossVisualRoot != null ? bossVisualRoot.GetComponentInChildren<SpriteRenderer>() : (bossCore != null ? bossCore.GetComponentInChildren<SpriteRenderer>() : null);
            Sprite shardSprite = deathShardSprite != null ? deathShardSprite : (bossRenderer != null ? bossRenderer.sprite : null);
            for (int i = 0; i < 42; i++)
            {
                GameObject shard = new GameObject("Boss Malware Data Fragment", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D));
                shard.transform.position = origin + new Vector3(Random.Range(-0.85f, 0.85f), Random.Range(-0.65f, 0.85f), 0f);
                shard.transform.localScale = new Vector3(Random.Range(0.06f, 0.22f), Random.Range(0.035f, 0.16f), 1f);
                shard.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

                SpriteRenderer renderer = shard.GetComponent<SpriteRenderer>();
                renderer.sprite = shardSprite;
                renderer.color = i % 3 == 0 ? new Color(0.25f, 1f, 1f, 0.96f) : (i % 3 == 1 ? new Color(1f, 0.12f, 0.52f, 0.95f) : new Color(0.95f, 1f, 1f, 0.9f));
                renderer.sortingOrder = 45;

                Rigidbody2D body = shard.GetComponent<Rigidbody2D>();
                Vector2 direction = Random.insideUnitCircle.normalized;
                if (direction.sqrMagnitude < 0.1f)
                {
                    direction = Vector2.up;
                }

                body.gravityScale = Random.Range(0.18f, 0.55f);
                body.linearVelocity = direction * Random.Range(3.4f, 8.6f) + Vector2.up * Random.Range(0.2f, 2.2f);
                body.angularVelocity = Random.Range(-740f, 740f);

                CircleCollider2D collider = shard.GetComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = 0.10f;
                Destroy(shard, 2.25f);
            }

            Transform visualRoot = bossVisualRoot != null ? bossVisualRoot : (bossCore != null ? bossCore.transform : null);
            if (visualRoot != null)
            {
                Renderer[] renderers = visualRoot.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] != null)
                    {
                        renderers[i].enabled = false;
                    }
                }
            }
        }

        public void ProjectileHitSolid()
        {
            if (!projectileInFlight)
            {
                return;
            }

            projectileInFlight = false;
            projectileFlightTimer = 0f;
            SetStatus("PATCH HILANG. BIDIK BLOK KUIS ATAU CELAH BOS.");
            ResetSlingshotProjectile();
        }

        public Color GetCategoryColor(int category)
        {
            return categoryColors[Mathf.Abs(category) % categoryColors.Length];
        }

        public void SetRecoveryPoint(Vector3 point)
        {
            currentRecoveryPoint = point;
            SaveProgress(currentRecoveryPoint);
            PlaySfx(checkpointSfx);
            SetStatus("NODE PEMULIHAN TERSINKRON");
        }

        public void RecoverPlayerFromAbyss()
        {
            FallIntoElectricRiver();
        }

        public void FallIntoElectricRiver()
        {
            if (player == null || mode == GameMode.Defeat || mode == GameMode.Victory)
            {
                return;
            }

            if (developerMode)
            {
                player.transform.position = currentRecoveryPoint;
                Rigidbody2D body = player.GetComponent<Rigidbody2D>();
                if (body != null)
                {
                    body.linearVelocity = Vector2.zero;
                    body.angularVelocity = 0f;
                }

                ApplyDeveloperModeVitals(false);
                SetStatus("MODE DEVELOPER: SUNGAI LISTRIK DIABAIKAN");
                RefreshHud();
                return;
            }

            if (!electricShockSequenceActive)
            {
                StartCoroutine(ElectricRiverShockSequence());
            }
        }

        private IEnumerator ElectricRiverShockSequence()
        {
            electricShockSequenceActive = true;
            introCountdownActive = true;
            invulnerabilityTimer = 999f;
            PlaySfx(GetDamageSfx("sungai listrik"));
            SetStatus("TERSENGAT SUNGAI LISTRIK - SISTEM TERBAKAR");

            Rigidbody2D playerBody = player != null ? player.GetComponent<Rigidbody2D>() : null;
            Collider2D playerCollider = player != null ? player.GetComponent<Collider2D>() : null;
            if (playerBody != null)
            {
                playerBody.linearVelocity = Vector2.zero;
                playerBody.angularVelocity = 0f;
                playerBody.simulated = false;
            }

            if (playerCollider != null)
            {
                playerCollider.enabled = false;
            }

            float elapsed = 0f;
            while (elapsed < 0.92f)
            {
                elapsed += Time.unscaledDeltaTime;
                bool visible = Mathf.FloorToInt(elapsed * 18f) % 2 == 0;
                SetPlayerRenderersEnabled(visible);
                if (Time.frameCount % 5 == 0 && player != null)
                {
                    SpawnElectricShockBurst(player.transform.position + new Vector3(0f, 0.24f, 0f));
                }

                yield return null;
            }

            SetPlayerRenderersEnabled(false);
            SpawnDeathFragments();
            yield return new WaitForSecondsRealtime(0.24f);

            introCountdownActive = false;
            electricShockSequenceActive = false;
            invulnerabilityTimer = 0f;

            if (!TrySpendLifeAndRespawn("sungai listrik"))
            {
                SetStatus("SISTEM JEBOL - TERSENGAT SUNGAI LISTRIK");
                BeginDefeatSequence("sungai listrik");
                yield break;
            }

            SetStatus("TERSENGAT SUNGAI LISTRIK - NYAWA BERKURANG");
            RefreshHud();
        }

        public bool TryUseBoost(float cost)
        {
            if (mode == GameMode.Defeat || mode == GameMode.Victory || quizOpen)
            {
                return false;
            }

            if (developerMode)
            {
                ApplyDeveloperModeVitals(false);
                PlaySfx(playerBoostSfx);
                SetStatus("MODE DEVELOPER: ENERGI BOOST TANPA BATAS");
                RefreshHud();
                return true;
            }

            if (boostEnergy < cost)
            {
                return false;
            }

            boostEnergy = Mathf.Max(0f, boostEnergy - cost);
            PlaySfx(playerBoostSfx);
            SetStatus("ENERGI AKTIF: GERAK CEPAT TERPAKAI");
            RefreshHud();
            return true;
        }

        public void ApplyPowerUp(CyberGuardianPowerUp powerUp)
        {
            if (powerUp == null || mode == GameMode.Defeat || mode == GameMode.Victory)
            {
                return;
            }

            switch (powerUp.type)
            {
                case CyberGuardianPowerUpType.Health:
                    playerHealth = Mathf.Min(100, playerHealth + Mathf.Max(1, powerUp.amount));
                    AddScore(75);
                    PlaySfx(powerupHealthSfx != null ? powerupHealthSfx : shieldSfx);
                    SetStatus("PATCH KESEHATAN TERPASANG");
                    break;
                case CyberGuardianPowerUpType.Boost:
                    boostEnergy = Mathf.Min(100f, boostEnergy + Mathf.Max(1, powerUp.amount));
                    AddScore(60);
                    PlaySfx(powerupEnergySfx != null ? powerupEnergySfx : shieldSfx);
                    SetStatus("CACHE ENERGI DIPULIHKAN");
                    break;
                case CyberGuardianPowerUpType.Firewall:
                    invulnerabilityTimer = Mathf.Max(invulnerabilityTimer, Mathf.Max(2f, powerUp.amount * 0.1f));
                    playerHealth = Mathf.Min(100, playerHealth + 8);
                    AddScore(100);
                    PlaySfx(shieldSfx);
                    SetStatus("FIREWALL SEMENTARA AKTIF");
                    break;
                case CyberGuardianPowerUpType.Overclock:
                    boostEnergy = 100f;
                    invulnerabilityTimer = Mathf.Max(invulnerabilityTimer, 1.25f);
                    AddScore(125);
                    PlaySfx(shieldSfx);
                    SetStatus("KEMAMPUAN OVERCLOCK SIAP");
                    break;
            }

            RefreshHud();
        }

        public void ShowStory(string title, string body, float duration)
        {
            if (storyPanel == null)
            {
                SetStatus(title + ": " + body);
                return;
            }

            if (storyTitleText != null)
            {
                storyTitleText.text = title;
            }

            if (storyBodyText != null)
            {
                storyBodyText.text = body;
            }

            storyPanel.SetActive(true);
            CancelInvoke(nameof(HideStoryPanel));
            Invoke(nameof(HideStoryPanel), Mathf.Max(1f, duration));
        }

        private IEnumerator ShowEducationCardRoutine(string title, string body, float duration)
        {
            if (educationPanel == null)
            {
                ShowStory(title, body, duration);
                yield return new WaitForSecondsRealtime(Mathf.Max(1f, duration));
                yield break;
            }

            narrativeOpen = true;
            narrativeContinueRequested = false;
            if (educationTitleText != null)
            {
                educationTitleText.text = title;
            }

            if (educationBodyText != null)
            {
                educationBodyText.text = body;
            }

            educationPanel.SetActive(true);
            if (educationContinueButton != null)
            {
                educationContinueButton.gameObject.SetActive(false);
            }

            RectTransform rect = educationPanel.GetComponent<RectTransform>();
            Vector3 baseScale = rect != null ? rect.localScale : Vector3.one;
            float fadeDuration = 0.28f;
            float elapsed = 0f;
            CanvasGroup group = educationPanel.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 0f;
            }

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                float eased = t * t * (3f - 2f * t);
                if (group != null)
                {
                    group.alpha = eased;
                }

                if (rect != null)
                {
                    rect.localScale = Vector3.one * Mathf.Lerp(0.94f, 1f, eased);
                }

                yield return null;
            }

            if (group != null)
            {
                group.alpha = 1f;
            }

            if (educationContinueButton != null)
            {
                educationContinueButton.gameObject.SetActive(true);
            }

            SetStatus("BACA KARTU EDUKASI - KLIK LANJUT ATAU TEKAN ENTER");
            yield return WaitForNarrativeContinue(Mathf.Max(0.65f, duration));
            if (rect != null)
            {
                rect.localScale = baseScale;
            }

            educationPanel.SetActive(false);
            narrativeOpen = false;
        }

        private void RequestNarrativeContinue()
        {
            narrativeContinueRequested = true;
        }

        private IEnumerator WaitForNarrativeContinue(float minimumReadSeconds)
        {
            float elapsed = 0f;
            float minimum = Mathf.Max(0.25f, minimumReadSeconds);
            while (elapsed < minimum || !narrativeContinueRequested)
            {
                elapsed += Time.unscaledDeltaTime;
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
                {
                    narrativeContinueRequested = true;
                }

                yield return null;
            }
        }

        private void ShowBossDialogue()
        {
            ShowBossDialogue(bossThreatTitle, bossThreatBody, 3.45f, true);
        }

        private void ShowBossDialogue(string title, string body, float holdSeconds)
        {
            ShowBossDialogue(title, body, holdSeconds, false);
        }

        private void ShowBossDialogue(string title, string body, float holdSeconds, bool requiresContinue)
        {
            if (bossDialoguePanel == null)
            {
                ShowStory(string.IsNullOrEmpty(title) ? "BOS MALWARE" : title, string.IsNullOrEmpty(body) ? "Akhirnya kamu sampai di firewall inti. Jawab blok kuis, buka celah, lalu tembak inti patch sebelum sistem direbut." : body, holdSeconds + 0.8f);
                return;
            }

            if (bossDialogueRoutine != null)
            {
                StopCoroutine(bossDialogueRoutine);
                bossDialogueRoutine = null;
                RestoreBossDialogueTimeScale();
            }

            if (bossDialogueTitleText != null)
            {
                bossDialogueTitleText.text = string.IsNullOrEmpty(title) ? "BOS MALWARE" : title;
            }

            if (bossDialogueBodyText != null)
            {
                bossDialogueBodyText.text = string.IsNullOrEmpty(body) ? "Kamu masuk ke domainku, Guardian. Setiap blok kuis adalah firewall palsu. Salah menjawab, seranganku makin kuat. Buka celahnya kalau berani." : body;
            }

            if (bossDialoguePortraitImage != null)
            {
                bossDialoguePortraitImage.sprite = bossDialogueIntroPortraitSprite != null ? bossDialogueIntroPortraitSprite : bossDialogueIdlePortraitSprite;
                bossDialoguePortraitImage.preserveAspect = true;
            }

            bossDialoguePanel.SetActive(true);
            bossDialogueHoldSeconds = Mathf.Max(0.8f, holdSeconds);
            bossDialogueRequiresContinue = requiresContinue;
            bossDialogueRoutine = StartCoroutine(BossDialogueRevealRoutine());
        }

        private void HideBossDialoguePanel()
        {
            if (bossDialogueRoutine != null)
            {
                StopCoroutine(bossDialogueRoutine);
                bossDialogueRoutine = null;
            }

            RestoreBossDialogueTimeScale();
            if (bossDialoguePanel != null)
            {
                bossDialoguePanel.SetActive(false);
            }

            narrativeOpen = false;
        }

        private IEnumerator BossDialogueRevealRoutine()
        {
            narrativeOpen = true;
            narrativeContinueRequested = false;
            if (bossDialogueContinueButton != null)
            {
                bossDialogueContinueButton.gameObject.SetActive(false);
            }

            bossDialoguePreviousTimeScale = Time.timeScale <= 0f ? 1f : Time.timeScale;
            bossDialoguePreviousFixedDeltaTime = Time.fixedDeltaTime <= 0f ? 0.02f : Time.fixedDeltaTime;
            Time.timeScale = Mathf.Min(bossDialoguePreviousTimeScale, 0.38f);
            Time.fixedDeltaTime = bossDialoguePreviousFixedDeltaTime * (Time.timeScale / Mathf.Max(0.01f, bossDialoguePreviousTimeScale));
            bossDialogueSlowActive = true;

            if (bossDialogueCanvasGroup != null)
            {
                bossDialogueCanvasGroup.alpha = 0f;
            }

            if (bossDialoguePanelRect != null)
            {
                bossDialoguePanelRect.localScale = Vector3.one * 0.92f;
            }

            if (bossDialoguePortraitRect != null)
            {
                bossDialoguePortraitRect.localScale = Vector3.one * 0.52f;
                bossDialoguePortraitRect.localRotation = Quaternion.Euler(-360f, 0f, 0f);
            }

            const float duration = 1.15f;
            float elapsed = 0f;
            bool switchedToIdle = false;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = t * t * (3f - 2f * t);
                float punch = Mathf.Sin(t * Mathf.PI);

                if (bossDialogueCanvasGroup != null)
                {
                    bossDialogueCanvasGroup.alpha = eased;
                }

                if (bossDialoguePanelRect != null)
                {
                    bossDialoguePanelRect.localScale = Vector3.one * Mathf.Lerp(0.92f, 1f, eased);
                }

                if (bossDialoguePortraitRect != null)
                {
                    float scale = Mathf.Lerp(0.52f, 1.10f, eased) + punch * 0.16f;
                    bossDialoguePortraitRect.localScale = Vector3.one * scale;
                    bossDialoguePortraitRect.localRotation = Quaternion.Euler(Mathf.Lerp(-360f, 0f, eased), 0f, Mathf.Sin(t * Mathf.PI) * -8f);
                }

                if (!switchedToIdle && t >= 0.74f && bossDialoguePortraitImage != null && bossDialogueIdlePortraitSprite != null)
                {
                    bossDialoguePortraitImage.sprite = bossDialogueIdlePortraitSprite;
                    switchedToIdle = true;
                }

                yield return null;
            }

            if (bossDialogueCanvasGroup != null)
            {
                bossDialogueCanvasGroup.alpha = 1f;
            }

            if (bossDialoguePanelRect != null)
            {
                bossDialoguePanelRect.localScale = Vector3.one;
            }

            if (bossDialoguePortraitRect != null)
            {
                bossDialoguePortraitRect.localScale = Vector3.one;
                bossDialoguePortraitRect.localRotation = Quaternion.identity;
            }

            if (bossDialoguePortraitImage != null && bossDialogueIdlePortraitSprite != null)
            {
                bossDialoguePortraitImage.sprite = bossDialogueIdlePortraitSprite;
            }

            RestoreBossDialogueTimeScale();
            if (bossDialogueContinueButton != null)
            {
                bossDialogueContinueButton.gameObject.SetActive(true);
            }

            if (bossDialogueRequiresContinue)
            {
                SetStatus("DIALOG BOS - KLIK LANJUT ATAU TEKAN ENTER");
                yield return WaitForNarrativeContinue(bossDialogueHoldSeconds);
            }
            else
            {
                yield return new WaitForSecondsRealtime(bossDialogueHoldSeconds);
            }

            if (bossDialoguePanel != null)
            {
                bossDialoguePanel.SetActive(false);
            }

            narrativeOpen = false;
            bossDialogueRoutine = null;
        }

        private void RestoreBossDialogueTimeScale()
        {
            if (!bossDialogueSlowActive)
            {
                return;
            }

            Time.timeScale = bossDialoguePreviousTimeScale;
            Time.fixedDeltaTime = bossDialoguePreviousFixedDeltaTime;
            bossDialogueSlowActive = false;
        }

        private void RegenerateBoost()
        {
            if (developerMode)
            {
                ApplyDeveloperModeVitals(true);
                return;
            }

            if (mode == GameMode.Defeat || mode == GameMode.Victory || quizOpen)
            {
                return;
            }

            float previous = boostEnergy;
            boostEnergy = Mathf.Min(100f, boostEnergy + GetBoostRegenPerSecond() * Time.deltaTime);
            if (!Mathf.Approximately(previous, boostEnergy))
            {
                RefreshHud();
            }
        }

        private void WireUi()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(TogglePause);
            }

            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(ReturnToMenu);
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveAllListeners();
                resetButton.onClick.AddListener(ReloadCurrentScene);
            }

            if (pauseResumeButton != null)
            {
                pauseResumeButton.onClick.RemoveAllListeners();
                pauseResumeButton.onClick.AddListener(() => SetPaused(false));
            }

            if (pauseRetryButton != null)
            {
                pauseRetryButton.onClick.RemoveAllListeners();
                pauseRetryButton.onClick.AddListener(ReloadCurrentScene);
            }

            if (pauseMenuButton != null)
            {
                pauseMenuButton.onClick.RemoveAllListeners();
                pauseMenuButton.onClick.AddListener(ReturnToMenu);
            }

            if (gameOverRetryButton != null)
            {
                gameOverRetryButton.onClick.RemoveAllListeners();
                gameOverRetryButton.onClick.AddListener(ReloadCurrentScene);
            }

            if (gameOverMenuButton != null)
            {
                gameOverMenuButton.onClick.RemoveAllListeners();
                gameOverMenuButton.onClick.AddListener(ReturnToMenu);
            }

            if (closeQuizButton != null)
            {
                closeQuizButton.onClick.RemoveAllListeners();
                closeQuizButton.onClick.AddListener(CloseQuizWithoutAnswer);
            }

            if (bossDialogueContinueButton != null)
            {
                bossDialogueContinueButton.onClick.RemoveAllListeners();
                bossDialogueContinueButton.onClick.AddListener(RequestNarrativeContinue);
            }

            if (educationContinueButton != null)
            {
                educationContinueButton.onClick.RemoveAllListeners();
                educationContinueButton.onClick.AddListener(RequestNarrativeContinue);
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
        }

        private void HandleSlingshotInput()
        {
            if (quizOpen || slingshotProjectile == null || player == null || projectileInFlight)
            {
                return;
            }

            Vector2 rest = GetSlingshotRestPosition();
            if (!slingshotProjectile.gameObject.activeSelf)
            {
                slingshotProjectile.gameObject.SetActive(true);
            }

            if (slingshotCollider != null && !projectileInFlight)
            {
                slingshotCollider.enabled = false;
            }

            if (!draggingSlingshot)
            {
                slingshotProjectile.position = rest;
            }

            Vector3 mouse = Input.mousePosition;
            Camera activeCamera = gameplayCamera != null ? gameplayCamera : Camera.main;
            if (activeCamera == null)
            {
                return;
            }

            mouse.z = Mathf.Abs(activeCamera.transform.position.z);
            Vector2 mouseWorld = activeCamera.ScreenToWorldPoint(mouse);

            if (Input.GetMouseButtonDown(0))
            {
                CyberGuardianBossShieldBlock targetedBlock = GetClosestActiveQuizBlock(mouseWorld, 0.92f);
                if (targetedBlock != null)
                {
                    FirePatchAt(targetedBlock.transform.position);
                    return;
                }

                draggingSlingshot = true;
                UpdateScreenWideSlingshotPull(rest, mouseWorld);
                PlaySfx(bossSlingshotPullSfx);
                SetStatus("MEMBIDIK INTI PATCH - LEPAS UNTUK MENEMBAK");
            }

            if (draggingSlingshot && Input.GetMouseButton(0))
            {
                UpdateScreenWideSlingshotPull(rest, mouseWorld);
            }

            if (draggingSlingshot && Input.GetMouseButtonUp(0))
            {
                draggingSlingshot = false;
                Vector2 offset = (Vector2)slingshotProjectile.position - rest;
                if (offset.magnitude < 0.25f)
                {
                    offset = GetScreenWidePullOffset(rest, mouseWorld);
                    slingshotProjectile.position = rest + offset;
                }

                FireSlingshot(GetLaunchVelocity(offset));
            }
        }

        private CyberGuardianBossShieldBlock GetClosestActiveQuizBlock(Vector2 point, float maxDistance)
        {
            CyberGuardianBossShieldBlock closest = null;
            float bestDistance = maxDistance * maxDistance;
            for (int i = 0; i < bossBlocks.Count; i++)
            {
                CyberGuardianBossShieldBlock block = bossBlocks[i];
                if (block == null || block.cleared || !block.gameObject.activeInHierarchy)
                {
                    continue;
                }

                float distance = ((Vector2)block.transform.position - point).sqrMagnitude;
                if (distance <= bestDistance)
                {
                    bestDistance = distance;
                    closest = block;
                }
            }

            return closest;
        }

        private void UpdateScreenWideSlingshotPull(Vector2 rest, Vector2 mouseWorld)
        {
            Vector2 offset = GetScreenWidePullOffset(rest, mouseWorld);
            Vector2 projectilePosition = rest + offset;
            slingshotProjectile.position = projectilePosition;
            UpdateSlingshotLines(rest, projectilePosition, GetLaunchVelocity(offset));
        }

        private Vector2 GetScreenWidePullOffset(Vector2 rest, Vector2 mouseWorld)
        {
            Vector2 aimDirection = mouseWorld - rest;
            if (aimDirection.sqrMagnitude < 0.04f)
            {
                aimDirection = Vector2.right * (player != null ? player.FacingDirection : 1);
            }

            float minPull = Mathf.Clamp(slingshotMinPull, 0.05f, slingshotMaxPull);
            float pull = Mathf.Clamp(aimDirection.magnitude * slingshotPullScreenScale, minPull, slingshotMaxPull);
            return -aimDirection.normalized * pull;
        }

        private void HandleBossAttack()
        {
            if (aerialBossEncounter)
            {
                HandleAerialBossAttack();
                return;
            }

            if (quizOpen || bossProjectilePrefab == null || bossProjectileSpawn == null || player == null)
            {
                return;
            }

            bossFireTimer -= Time.deltaTime;
            if (bossFireTimer > 0f)
            {
                return;
            }

            float pressure = GetDifficultyPressure();
            float lowHealthPressure = 1f - bossHealth / 100f;
            bossFireTimer = Mathf.Lerp(Mathf.Lerp(2.15f, 1.55f, pressure), Mathf.Lerp(1.05f, 0.62f, pressure), lowHealthPressure);
            float fireY = player.transform.position.y + 0.35f;
            int projectileDamage = GetBossProjectileDamage();
            float projectileSpeed = Mathf.Lerp(6.2f, 8.4f, pressure);
            if (IsBossLineBlocked(fireY))
            {
                Vector2 breachSpawn = new Vector2(
                    Mathf.Clamp(player.transform.position.x + Random.Range(-2.0f, 2.0f), bossArenaMinX + 0.65f, bossArenaMaxX - 0.65f),
                    cameraMax.y + 0.85f);
                Vector2 breachTarget = (Vector2)player.transform.position + new Vector2(Random.Range(-0.35f, 0.35f), 0.12f);
                SpawnBossProjectile(breachSpawn, breachTarget, projectileSpeed, Mathf.Max(6, projectileDamage - 3));
                if (pressure > 0.72f && bossHealth < 70)
                {
                    SpawnBossProjectile(breachSpawn + new Vector2(1.2f, 0.15f), breachTarget + new Vector2(-0.7f, 0f), projectileSpeed * 0.92f, Mathf.Max(5, projectileDamage - 5));
                }

                PlaySfx(bossShotSfx);
                SetStatus("PAKET SERANGAN BOS DARI ATAS - TERUS BERGERAK");
                return;
            }

            Vector2 directTarget = (Vector2)player.transform.position + new Vector2(0f, 0.35f);
            SpawnBossProjectile(bossProjectileSpawn.position, directTarget, projectileSpeed + 0.55f, projectileDamage);
            if (pressure > 0.58f && bossHealth < 55)
            {
                SpawnBossProjectile((Vector2)bossProjectileSpawn.position + new Vector2(0f, -0.32f), directTarget + new Vector2(0f, -0.48f), projectileSpeed, Mathf.Max(5, projectileDamage - 4));
            }

            PlaySfx(bossShotSfx);
            SetStatus("BOS MENYERANG LEWAT CELAH - HINDARI");
        }

        private void HandleAerialBossAttack()
        {
            if (quizOpen || bossProjectilePrefab == null || player == null)
            {
                return;
            }

            bossFireTimer -= Time.deltaTime;
            if (bossFireTimer > 0f)
            {
                return;
            }

            float pressure = GetDifficultyPressure();
            float lowHealthPressure = 1f - bossHealth / 100f;
            float interval = Mathf.Lerp(1.35f, 0.68f, Mathf.Clamp01(pressure + lowHealthPressure * 0.45f));
            bossFireTimer = Mathf.Max(0.38f, interval * Mathf.Max(0.35f, bossAttackIntervalScale));

            int volley = Mathf.Max(2, bossVolleyCount + Mathf.RoundToInt(lowHealthPressure * 2f));
            float speed = Mathf.Lerp(7.4f, 10.6f, pressure) + bossProjectileSpeedBonus + lowHealthPressure * 1.15f;
            int damage = GetBossProjectileDamage() + 2;
            int pattern = bossAttackPatternIndex++ % 4;

            if (pattern == 0)
            {
                Transform port = GetBossSpawnPort(0);
                Vector2 spawn = port != null ? (Vector2)port.position : (Vector2)bossProjectileSpawn.position;
                for (int i = 0; i < volley; i++)
                {
                    float yOffset = (i - (volley - 1) * 0.5f) * 0.46f;
                    Vector2 target = (Vector2)player.transform.position + new Vector2(Random.Range(-0.25f, 0.25f), 0.22f + yOffset);
                    SpawnBossProjectile(spawn + new Vector2(0f, yOffset * 0.18f), target, speed, damage);
                }

                SetStatus("SAMBARAN LISTRIK BOS UDARA: HINDARI JALUR PETIR");
            }
            else if (pattern == 1)
            {
                int rainCount = volley + 3;
                for (int i = 0; i < rainCount; i++)
                {
                    float x = Mathf.Lerp(bossArenaMinX + 0.9f, bossArenaMaxX - 0.9f, rainCount <= 1 ? 0.5f : i / (float)(rainCount - 1));
                    Vector2 spawn = new Vector2(x + Random.Range(-0.45f, 0.45f), cameraMax.y + 1.35f);
                    Vector2 target = new Vector2(x + Random.Range(-0.8f, 0.8f), player.transform.position.y - 0.4f);
                    SpawnBossProjectile(spawn, target, speed * 0.92f, Mathf.Max(5, damage - 2));
                }

                SetStatus("HUJAN LISTRIK BOS UDARA: AWASI ATAS");
            }
            else if (pattern == 2)
            {
                for (int i = 0; i < Mathf.Max(3, volley); i++)
                {
                    Transform port = GetBossSpawnPort(i);
                    Vector2 spawn = port != null ? (Vector2)port.position : new Vector2(bossArenaMaxX + 2f, 4.4f + i * 0.35f);
                    Vector2 target = (Vector2)player.transform.position + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-0.4f, 1.1f));
                    SpawnBossProjectile(spawn, target, speed * (0.85f + i * 0.05f), damage);
                }

                SetStatus("SAPUAN PETIR BOS UDARA: SERANGAN MULTI-PORT");
            }
            else
            {
                Vector2 leftSpawn = new Vector2(bossArenaMinX + 0.4f, cameraMax.y + 0.8f);
                Vector2 rightSpawn = new Vector2(bossArenaMaxX - 0.4f, cameraMax.y + 0.8f);
                for (int i = 0; i < volley + 1; i++)
                {
                    float t = i / (float)Mathf.Max(1, volley);
                    Vector2 targetLeft = new Vector2(Mathf.Lerp(bossArenaMaxX - 0.9f, bossArenaMinX + 0.8f, t), player.transform.position.y + Random.Range(-0.45f, 0.85f));
                    Vector2 targetRight = new Vector2(Mathf.Lerp(bossArenaMinX + 0.9f, bossArenaMaxX - 0.8f, t), player.transform.position.y + Random.Range(-0.45f, 0.85f));
                    SpawnBossProjectile(leftSpawn, targetLeft, speed * 0.78f, Mathf.Max(5, damage - 3));
                    SpawnBossProjectile(rightSpawn, targetRight, speed * 0.78f, Mathf.Max(5, damage - 3));
                }

                SetStatus("BADAI LISTRIK SILANG: CARI CELAH AMAN");
            }

            PlaySfx(bossShotSfx);
        }

        private Transform GetBossSpawnPort(int index)
        {
            if (bossProjectileSpawns != null && bossProjectileSpawns.Length > 0)
            {
                return bossProjectileSpawns[Mathf.Abs(index) % bossProjectileSpawns.Length];
            }

            return bossProjectileSpawn;
        }

        private void SpawnBossProjectile(Vector2 spawnPosition, Vector2 targetPosition, float speed, int damage)
        {
            GameObject shot = Instantiate(bossProjectilePrefab, spawnPosition, Quaternion.identity);
            CyberGuardianBossProjectile projectile = shot.GetComponent<CyberGuardianBossProjectile>();
            if (projectile != null)
            {
                projectile.game = this;
                Vector2 direction = (targetPosition - spawnPosition).normalized;
                projectile.velocity = direction * speed;
                projectile.damage = damage;
                projectile.lifetime = 4.2f;
            }
        }

        private bool IsBossLineBlocked(float y)
        {
            for (int i = 0; i < bossBlocks.Count; i++)
            {
                CyberGuardianBossShieldBlock block = bossBlocks[i];
                if (block != null && block.gameObject.activeSelf && Mathf.Abs(block.transform.position.y - y) < 0.55f)
                {
                    return true;
                }
            }

            return false;
        }

        private void OpenBlockQuiz(CyberGuardianBossShieldBlock block)
        {
            activeQuizBlock = block;
            quizOpen = true;
            int seed = score + block.category * 37 + Mathf.RoundToInt(block.transform.position.x * 13f) + Mathf.RoundToInt(block.transform.position.y * 17f) + Time.frameCount;
            QuizQuestion question = GetQuestion(block.category, seed);
            activeQuizQuestion = question;
            displayedAnswerIndices = BuildAnswerIndexMap(question, seed);

            if (quizTitleText != null)
            {
                quizTitleText.text = question != null ? question.title : "BLOK KUIS";
            }

            if (quizPromptText != null)
            {
                quizPromptText.text = question != null ? question.prompt : "Pertanyaan keamanan tidak tersedia.";
            }

            if (feedbackText != null)
            {
                feedbackText.text = string.Empty;
            }

            for (int i = 0; answerButtons != null && i < answerButtons.Length; i++)
            {
                int answerIndex = GetOriginalAnswerIndex(i);
                bool hasAnswer = question != null && question.answers != null && answerIndex >= 0 && answerIndex < question.answers.Length;
                if (answerButtons[i] != null)
                {
                    answerButtons[i].gameObject.SetActive(hasAnswer);
                    answerButtons[i].interactable = hasAnswer;
                }

                if (answerLabels != null && i < answerLabels.Length && answerLabels[i] != null)
                {
                    string answerText = hasAnswer ? question.answers[answerIndex] : string.Empty;
                    if (developerMode && hasAnswer && answerIndex == question.correctIndex)
                    {
                        answerText = "[BENAR] " + answerText;
                    }

                    answerLabels[i].text = answerText;
                }
            }

            if (quizModal != null)
            {
                quizModal.SetActive(true);
            }

            PlaySfx(quizOpenSfx);
            SetStatus("JAWAB UNTUK MENGHANCURKAN BLOK PERISAI BOS");
            RefreshHud();
        }

        private void ChooseAnswer(int choice)
        {
            if (!quizOpen || activeQuizBlock == null)
            {
                return;
            }

            QuizQuestion question = activeQuizQuestion ?? GetQuestion(activeQuizBlock.category, score + activeQuizBlock.category + Mathf.RoundToInt(activeQuizBlock.transform.position.y * 10f));
            int originalChoice = GetOriginalAnswerIndex(choice);
            for (int i = 0; answerButtons != null && i < answerButtons.Length; i++)
            {
                if (answerButtons[i] != null)
                {
                    answerButtons[i].interactable = false;
                }
            }

            if (question != null && originalChoice == question.correctIndex)
            {
                activeQuizBlock.ClearBlock();
                playerHealth = Mathf.Min(100, playerHealth + GetCorrectHealthReward());
                AddScore(GetCorrectScoreReward());
                PlaySfx(quizCorrectSfx != null ? quizCorrectSfx : (bossShieldBreakSfx != null ? bossShieldBreakSfx : shieldSfx));
                if (feedbackText != null)
                {
                    feedbackText.text = question.feedback;
                }

                SetStatus("BLOK HANCUR. CELAH SERANGAN BARU TERBUKA.");
            }
            else
            {
                activeQuizBlock.PulseWrong();
                DamagePlayer(GetWrongAnswerDamage(), "jawaban salah");
                bossFireTimer = Mathf.Min(bossFireTimer, 0.35f);
                if (feedbackText != null)
                {
                    string correctAnswer = question != null && question.answers != null && question.correctIndex >= 0 && question.correctIndex < question.answers.Length
                        ? question.answers[question.correctIndex]
                        : "tidak tersedia";
                    feedbackText.text = developerMode
                        ? "Jawaban benar: " + correctAnswer
                        : "Jawaban salah. Perisai melemah dan bos mendapat peluang menyerang.";
                }
            }

            Invoke(nameof(CloseQuizAfterAnswer), 0.95f);
            RefreshHud();
        }

        private void CloseQuizAfterAnswer()
        {
            CloseQuiz();
            ResetSlingshotProjectile();
        }

        private void CloseQuizWithoutAnswer()
        {
            if (quizOpen)
            {
                DamagePlayer(Mathf.Max(4, GetWrongAnswerDamage() / 2), "quiz dilewati");
            }

            CloseQuiz();
            ResetSlingshotProjectile();
        }

        private void CloseQuiz()
        {
            quizOpen = false;
            activeQuizBlock = null;
            activeQuizQuestion = null;
            displayedAnswerIndices = new int[0];
            if (quizModal != null)
            {
                quizModal.SetActive(false);
            }

            RefreshHud();
        }

        private QuizQuestion GetQuestion(int category, int seed)
        {
            if (quizQuestionBank != null)
            {
                return quizQuestionBank.GetQuestion(category, quizLessonTier, seed, FallbackQuestions);
            }

            return FallbackQuestions[(category & 0x7fffffff) % FallbackQuestions.Length];
        }

        private int[] BuildAnswerIndexMap(QuizQuestion question, int seed)
        {
            int buttonCount = answerButtons != null ? answerButtons.Length : 4;
            int answerCount = question != null && question.answers != null ? question.answers.Length : 0;
            int count = Mathf.Min(buttonCount, answerCount);
            int[] map = new int[count];
            for (int i = 0; i < count; i++)
            {
                map[i] = i;
            }

            unchecked
            {
                int state = seed == int.MinValue ? 1 : Mathf.Abs(seed) + 97;
                for (int i = count - 1; i > 0; i--)
                {
                    state = state * 1103515245 + 12345;
                    int j = (state & 0x7fffffff) % (i + 1);
                    int tmp = map[i];
                    map[i] = map[j];
                    map[j] = tmp;
                }
            }

            return map;
        }

        private int GetOriginalAnswerIndex(int displayIndex)
        {
            if (displayedAnswerIndices == null || displayIndex < 0 || displayIndex >= displayedAnswerIndices.Length)
            {
                return displayIndex;
            }

            return displayedAnswerIndices[displayIndex];
        }

        private void FireSlingshot(Vector2 velocity)
        {
            projectileInFlight = true;
            projectileFlightTimer = 0f;
            HideSlingshotLines();
            Vector2 launchStart = GetSlingshotRestPosition();
            if (slingshotProjectile != null && !slingshotProjectile.gameObject.activeSelf)
            {
                slingshotProjectile.gameObject.SetActive(true);
            }

            if (slingshotProjectile != null)
            {
                slingshotProjectile.position = launchStart;
            }

            if (slingshotCollider != null)
            {
                slingshotCollider.enabled = false;
            }

            if (slingshotBody != null)
            {
                slingshotBody.position = launchStart;
                slingshotBody.simulated = true;
                slingshotBody.linearVelocity = velocity;
                slingshotBody.angularVelocity = -450f;
            }

            if (player != null)
            {
                player.TriggerFireAnimation(0.42f);
            }

            if (slingshotCollider != null)
            {
                StartCoroutine(EnableSlingshotColliderAfterLaunch());
            }

            PlaySfx(bossSlingshotLaunchSfx != null ? bossSlingshotLaunchSfx : bossShotSfx);
            SetStatus("INTI PATCH DITEMBAKKAN");
        }

        private IEnumerator EnableSlingshotColliderAfterLaunch()
        {
            yield return new WaitForFixedUpdate();
            if (projectileInFlight && slingshotCollider != null)
            {
                slingshotCollider.enabled = true;
            }
        }

        private void FirePatchAt(Vector2 target)
        {
            if (slingshotProjectile == null)
            {
                return;
            }

            Vector2 rest = GetSlingshotRestPosition();
            slingshotProjectile.position = rest;
            FireSlingshot(GetBallisticVelocity(rest, target));
        }

        private Vector2 GetBallisticVelocity(Vector2 start, Vector2 target)
        {
            float distance = Vector2.Distance(start, target);
            float flightTime = Mathf.Clamp(distance / 12f, 0.55f, 1.15f);
            Vector2 gravity = Physics2D.gravity * (slingshotBody != null ? slingshotBody.gravityScale : 1f);
            Vector2 velocity = (target - start - 0.5f * gravity * flightTime * flightTime) / flightTime;
            float speed = velocity.magnitude;
            if (speed > slingshotDirectShotMaxSpeed && speed > 0.01f)
            {
                velocity = velocity.normalized * slingshotDirectShotMaxSpeed;
            }

            return velocity;
        }

        private Vector2 GetLaunchVelocity(Vector2 pullOffset)
        {
            Vector2 velocity = -pullOffset * slingshotPower;
            float speed = velocity.magnitude;
            if (speed <= 0.01f)
            {
                return velocity;
            }

            float clampedSpeed = Mathf.Clamp(speed, slingshotMinLaunchSpeed, slingshotMaxLaunchSpeed);
            return velocity.normalized * clampedSpeed;
        }

        private Vector2 GetSlingshotRestPosition()
        {
            Vector2 basePosition = player != null ? player.transform.position : Vector3.zero;
            return basePosition + slingshotRestOffset;
        }

        private void ResetSlingshotProjectile()
        {
            draggingSlingshot = false;
            projectileInFlight = false;
            projectileFlightTimer = 0f;
            HideSlingshotLines();
            if (slingshotProjectile != null)
            {
                slingshotProjectile.position = GetSlingshotRestPosition();
                slingshotProjectile.gameObject.SetActive(mode == GameMode.BossSlingshot);
            }

            if (slingshotBody != null)
            {
                slingshotBody.simulated = false;
                slingshotBody.linearVelocity = Vector2.zero;
                slingshotBody.angularVelocity = 0f;
            }

            if (slingshotCollider != null)
            {
                slingshotCollider.enabled = false;
            }
        }

        private void FreezeSlingshotProjectile()
        {
            HideSlingshotLines();
            if (slingshotBody != null)
            {
                slingshotBody.simulated = false;
                slingshotBody.linearVelocity = Vector2.zero;
            }

            if (slingshotCollider != null)
            {
                slingshotCollider.enabled = false;
            }
        }

        private void UpdateSlingshotLines(Vector2 rest, Vector2 projectilePosition, Vector2 velocity)
        {
            float facing = player != null ? player.FacingDirection : 1f;
            Vector2 anchorA = rest + new Vector2(0.18f * facing, 0.25f);
            Vector2 anchorB = rest + new Vector2(0.18f * facing, -0.22f);
            SetLine(slingshotBandA, anchorA, projectilePosition);
            SetLine(slingshotBandB, anchorB, projectilePosition);

            if (trajectoryLine != null)
            {
                float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 12f);
                Color start = Color.Lerp(new Color(0.25f, 1f, 1f, 0.42f), new Color(1f, 0.22f, 0.62f, 0.78f), pulse);
                Color end = new Color(0.25f, 1f, 1f, 0.05f);
                trajectoryLine.enabled = true;
                trajectoryLine.positionCount = 28;
                trajectoryLine.startColor = start;
                trajectoryLine.endColor = end;
                trajectoryLine.startWidth = Mathf.Lerp(0.035f, 0.075f, pulse);
                trajectoryLine.endWidth = 0.018f;
                for (int i = 0; i < trajectoryLine.positionCount; i++)
                {
                    float time = i * 0.078f;
                    Vector2 point = projectilePosition + velocity * time + 0.5f * Physics2D.gravity * time * time;
                    point += Vector2.up * (Mathf.Sin(Time.unscaledTime * 10f + i * 0.9f) * 0.018f);
                    trajectoryLine.SetPosition(i, point);
                }
            }
        }

        private void SetLine(LineRenderer line, Vector2 a, Vector2 b)
        {
            if (line == null)
            {
                return;
            }

            line.enabled = true;
            line.positionCount = 2;
            line.SetPosition(0, a);
            line.SetPosition(1, b);
        }

        private void HideSlingshotLines()
        {
            if (slingshotBandA != null)
            {
                slingshotBandA.enabled = false;
            }

            if (slingshotBandB != null)
            {
                slingshotBandB.enabled = false;
            }

            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = false;
            }
        }

        private void MonitorSlingshotProjectile()
        {
            projectileFlightTimer += Time.deltaTime;
            if (slingshotProjectile == null)
            {
                projectileInFlight = false;
                return;
            }

            Vector3 position = slingshotProjectile.position;
            bool outOfArena = position.x < bossArenaMinX - 5.0f || position.x > bossArenaMaxX + 9.5f || position.y < -7.4f || position.y > 8.2f;
            if (projectileFlightTimer >= projectileMaxFlightTime || outOfArena)
            {
                ProjectileHitSolid();
            }
        }

        private void ClampPlayerToBossArena()
        {
            if (player == null)
            {
                return;
            }

            Vector3 position = player.transform.position;
            position.x = Mathf.Clamp(position.x, bossArenaMinX, bossArenaMaxX);
            if (aerialBossEncounter)
            {
                position.y = Mathf.Clamp(position.y, 1.35f, 7.15f);
            }
            player.transform.position = position;
        }

        private void UpdateCamera()
        {
            if (gameplayCamera == null || player == null)
            {
                return;
            }

            if (cinematicCameraActive)
            {
                gameplayCamera.transform.position = Vector3.Lerp(gameplayCamera.transform.position, cinematicCameraTarget, Time.unscaledDeltaTime * 5.0f);
                gameplayCamera.orthographicSize = Mathf.Lerp(gameplayCamera.orthographicSize, Mathf.Max(1.0f, cinematicCameraSize), Time.unscaledDeltaTime * 4.2f);
                return;
            }

            float targetSize = Mathf.Max(1.0f, bossCameraSize);
            Vector3 target;
            if (mode == GameMode.Defeat || electricShockSequenceActive)
            {
                targetSize = mode == GameMode.Defeat ? 2.65f : Mathf.Max(1.55f, electricDeathCameraSize);
                target = new Vector3(player.transform.position.x, player.transform.position.y + 0.45f, -10f);
            }
            else
            {
                bool bossView = mode == GameMode.BossSlingshot || mode == GameMode.Victory;
                targetSize = bossView ? (aerialBossEncounter ? Mathf.Max(bossCameraSize, aerialBossCameraSize) : bossCameraSize) : 5.4f;
                float facingLead = player.FacingDirection >= 0 ? 3.25f : -1.85f;
                float verticalLead = Mathf.Clamp(player.Velocity.y * 0.16f, -0.62f, 1.18f);
                float targetX = bossView ? bossArenaCenterX + (aerialBossEncounter ? aerialBossCameraXOffset : 0f) : player.transform.position.x + facingLead;
                float targetY = bossView
                    ? (aerialBossEncounter ? player.transform.position.y + aerialBossCameraYOffset : Mathf.Lerp(player.transform.position.y + 1.0f + verticalLead * 0.45f, 2.25f, 0.25f))
                    : player.transform.position.y + 1.05f + verticalLead;
                target = new Vector3(Mathf.Clamp(targetX, cameraMin.x, cameraMax.x), Mathf.Clamp(targetY, cameraMin.y, cameraMax.y), -10f);
            }

            float followSpeed = player.IsBoosting ? 7.2f : 5.2f;
            float delta = electricShockSequenceActive ? Time.unscaledDeltaTime : Time.deltaTime;
            gameplayCamera.transform.position = Vector3.Lerp(gameplayCamera.transform.position, target, delta * followSpeed);
            gameplayCamera.orthographicSize = Mathf.Lerp(gameplayCamera.orthographicSize, targetSize, delta * 4.1f);
        }

        private void RefreshHud()
        {
            if (healthText != null)
            {
                healthText.text = "HP";
            }

            if (livesText != null)
            {
                livesText.gameObject.SetActive(false);
            }

            if (lifeIconImages != null)
            {
                int displayLives = developerMode ? lifeIconImages.Length : Mathf.Clamp(playerLives, 0, lifeIconImages.Length);
                for (int i = 0; i < lifeIconImages.Length; i++)
                {
                    Image icon = lifeIconImages[i];
                    if (icon == null)
                    {
                        continue;
                    }

                    icon.gameObject.SetActive(true);
                    bool filled = i < displayLives;
                    icon.color = filled ? Color.white : new Color(0.42f, 0.50f, 0.56f, 0.36f);
                }
            }

            if (bossText != null)
            {
                bossText.text = "BOS";
            }

            if (scoreText != null)
            {
                scoreText.text = score.ToString("0");
            }

            if (modeText != null)
            {
                modeText.text = developerMode ? "DEV" : "ENERGI";
            }

            if (playerHealthFill != null)
            {
                playerHealthFill.fillAmount = developerMode ? 1f : playerHealth / 100f;
            }

            if (boostEnergyFill != null)
            {
                boostEnergyFill.fillAmount = developerMode ? 1f : boostEnergy / 100f;
            }

            bool showBossHud = mode == GameMode.BossSlingshot || mode == GameMode.Victory;
            if (bossHudGroup != null)
            {
                bossHudGroup.SetActive(showBossHud);
            }

            if (bossHealthFill != null)
            {
                bossHealthFill.fillAmount = bossHealth / 100f;
                if (bossHudGroup == null)
                {
                    bossHealthFill.gameObject.SetActive(showBossHud);
                }
            }

            if (bossText != null && bossHudGroup == null)
            {
                bossText.gameObject.SetActive(showBossHud);
            }
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        private void HideMeleeFlash()
        {
            if (meleeFlash != null)
            {
                meleeFlash.SetActive(false);
            }
        }

        private void HideStoryPanel()
        {
            if (storyPanel != null)
            {
                storyPanel.SetActive(false);
            }
        }

        private void AddScore(int amount)
        {
            score = Mathf.Clamp(score + Mathf.Max(0, amount), 0, MaxScore);
        }

        private void ApplyDeveloperModeVitals(bool refreshIfChanged)
        {
            if (!developerMode)
            {
                return;
            }

            int targetLives = Mathf.Max(maxLives, 99);
            bool changed = playerHealth != 100 || playerLives < targetLives || boostEnergy < 99.9f;
            playerHealth = 100;
            playerLives = Mathf.Max(playerLives, targetLives);
            boostEnergy = 100f;

            if (refreshIfChanged && changed)
            {
                RefreshHud();
            }
        }

        private bool TryApplySavedContinue()
        {
            if (PlayerPrefs.GetInt(CyberGuardianMainMenu.ResumeRequestedKey, 0) != 1)
            {
                return false;
            }

            PlayerPrefs.SetInt(CyberGuardianMainMenu.ResumeRequestedKey, 0);
            if (!CyberGuardianMainMenu.HasSavedProgress())
            {
                PlayerPrefs.Save();
                return false;
            }

            string savedScene = PlayerPrefs.GetString(CyberGuardianMainMenu.SaveSceneKey, string.Empty);
            if (savedScene != SceneManager.GetActiveScene().name)
            {
                PlayerPrefs.Save();
                return false;
            }

            currentRecoveryPoint = new Vector3(
                PlayerPrefs.GetFloat(CyberGuardianMainMenu.SaveXKey, currentRecoveryPoint.x),
                PlayerPrefs.GetFloat(CyberGuardianMainMenu.SaveYKey, currentRecoveryPoint.y),
                PlayerPrefs.GetFloat(CyberGuardianMainMenu.SaveZKey, currentRecoveryPoint.z));

            playerHealth = Mathf.Clamp(PlayerPrefs.GetInt(CyberGuardianMainMenu.SaveHealthKey, playerHealth), 1, 100);
            playerLives = Mathf.Clamp(PlayerPrefs.GetInt(CyberGuardianMainMenu.SaveLivesKey, playerLives), 1, Mathf.Max(1, maxLives));
            boostEnergy = Mathf.Clamp(PlayerPrefs.GetFloat(CyberGuardianMainMenu.SaveBoostKey, boostEnergy), 0f, 100f);
            score = Mathf.Clamp(PlayerPrefs.GetInt(CyberGuardianMainMenu.SaveScoreKey, score), 0, MaxScore);
            ApplyDeveloperModeVitals(false);

            if (player != null)
            {
                player.transform.position = currentRecoveryPoint;
                Rigidbody2D body = player.GetComponent<Rigidbody2D>();
                if (body != null)
                {
                    body.linearVelocity = Vector2.zero;
                    body.angularVelocity = 0f;
                }
            }

            PlayerPrefs.Save();
            return true;
        }

        private void SaveProgress(Vector3 recoveryPoint)
        {
            SaveProgressForScene(SceneManager.GetActiveScene().name, recoveryPoint, false);
        }

        private void SaveProgressForScene(string sceneName, Vector3 recoveryPoint, bool requestResume)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            PlayerPrefs.SetInt(CyberGuardianMainMenu.SaveExistsKey, 1);
            PlayerPrefs.SetString(CyberGuardianMainMenu.SaveSceneKey, sceneName);
            PlayerPrefs.SetFloat(CyberGuardianMainMenu.SaveXKey, recoveryPoint.x);
            PlayerPrefs.SetFloat(CyberGuardianMainMenu.SaveYKey, recoveryPoint.y);
            PlayerPrefs.SetFloat(CyberGuardianMainMenu.SaveZKey, recoveryPoint.z);
            PlayerPrefs.SetInt(CyberGuardianMainMenu.SaveHealthKey, Mathf.Clamp(playerHealth, 1, 100));
            PlayerPrefs.SetInt(CyberGuardianMainMenu.SaveLivesKey, Mathf.Clamp(playerLives, 1, Mathf.Max(1, maxLives)));
            PlayerPrefs.SetFloat(CyberGuardianMainMenu.SaveBoostKey, Mathf.Clamp(boostEnergy, 0f, 100f));
            PlayerPrefs.SetInt(CyberGuardianMainMenu.SaveScoreKey, Mathf.Clamp(score, 0, MaxScore));
            PlayerPrefs.SetInt(CyberGuardianMainMenu.DifficultyKey, currentDifficultyIndex);
            PlayerPrefs.SetInt(CyberGuardianMainMenu.ResumeRequestedKey, requestResume ? 1 : 0);
            PlayerPrefs.Save();
        }

        private Vector3 GetNextSceneStartPoint()
        {
            return new Vector3(-8f, 0.95f, 0f);
        }

        private int GetCorrectScoreReward()
        {
            return activeDifficulty != null ? Mathf.Max(10, activeDifficulty.correctScoreReward) : 100;
        }

        private int GetCorrectHealthReward()
        {
            return activeDifficulty != null ? Mathf.Max(2, activeDifficulty.correctShieldReward) : 6;
        }

        private int GetRespawnHealth()
        {
            return Mathf.Clamp(activeDifficulty != null ? activeDifficulty.startingShield : 100, 1, 100);
        }

        private int GetWrongAnswerDamage()
        {
            return activeDifficulty != null ? Mathf.Max(4, activeDifficulty.wrongShieldDamage) : 12;
        }

        private int GetBossHitDamage()
        {
            return activeDifficulty != null ? Mathf.Clamp(activeDifficulty.routeVirusDamage, 16, 32) : 22;
        }

        private int GetBossHitScoreReward()
        {
            return activeDifficulty != null ? Mathf.Max(120, activeDifficulty.routeScoreReward) : 600;
        }

        private int GetBossProjectileDamage()
        {
            return Mathf.RoundToInt(Mathf.Lerp(9f, 16f, GetDifficultyPressure()));
        }

        private float GetBoostRegenPerSecond()
        {
            return Mathf.Lerp(34f, 22f, GetDifficultyPressure());
        }

        private float GetDifficultyPressure()
        {
            if (activeDifficulty == null)
            {
                return 0.45f;
            }

            return Mathf.Clamp01(Mathf.InverseLerp(10f, 35f, activeDifficulty.startingVirusStrength));
        }

        private DifficultyProfile GetActiveDifficulty()
        {
            if (difficultyProfiles != null && difficultyProfiles.Length > 0)
            {
                currentDifficultyIndex = Mathf.Clamp(currentDifficultyIndex, 0, difficultyProfiles.Length - 1);
                return difficultyProfiles[currentDifficultyIndex];
            }

            return null;
        }

        private void TogglePause()
        {
            if (introCountdownActive || mode == GameMode.Defeat || mode == GameMode.Victory || quizOpen)
            {
                return;
            }

            SetPaused(!paused);
        }

        private void SetPaused(bool value)
        {
            paused = value && mode != GameMode.Defeat && mode != GameMode.Victory;
            Time.timeScale = paused ? 0f : 1f;
            if (pauseModal != null)
            {
                pauseModal.SetActive(paused);
            }

            SetStatus(paused ? "JEDA" : (mode == GameMode.BossSlingshot ? "MODE BOS: KLIK ATAU TARIK DI MANA SAJA UNTUK MENARIK INTI PATCH" : "MODE PETUALANGAN"));
            RefreshHud();
        }

        private void ReloadCurrentScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ReturnToMenu()
        {
            Time.timeScale = 1f;
            if (mode != GameMode.Defeat)
            {
                SaveProgress(currentRecoveryPoint);
            }

            SceneManager.LoadScene(menuSceneName);
        }

        private void LoadNextScene()
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }

        private void PlaySfx(AudioClip clip)
        {
            if (sfxSource != null && clip != null && CyberGuardianMainMenu.IsSfxEnabled())
            {
                sfxSource.mute = false;
                sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayPlayerJumpSfx()
        {
            PlaySfx(jumpSfx);
        }

        public void PlayPlayerShootSfx()
        {
            PlaySfx(playerShootSfx != null ? playerShootSfx : bossShotSfx);
        }

        private AudioClip GetDamageSfx(string source)
        {
            string normalized = string.IsNullOrEmpty(source) ? string.Empty : source.ToLowerInvariant();
            if (normalized.Contains("wrong") || normalized.Contains("jawaban salah") || normalized.Contains("quiz"))
            {
                return quizWrongSfx != null ? quizWrongSfx : wrongSfx;
            }

            return playerHitSfx != null ? playerHitSfx : wrongSfx;
        }

        private void StartLoopingMusic(AudioClip clip)
        {
            if (musicSource == null || clip == null || musicSource.clip == clip)
            {
                return;
            }

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.mute = !CyberGuardianMainMenu.IsMusicEnabled();
            musicSource.Play();
        }
    }
}
