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
        public Camera gameplayCamera;
        public Transform bossProjectileSpawn;
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
        public Text bossText;
        public Text scoreText;
        public Text modeText;
        public Text statusText;
        public Image playerHealthFill;
        public Image bossHealthFill;
        public Image boostEnergyFill;
        public GameObject bossHudGroup;
        public Button menuButton;
        public Button resetButton;
        public GameObject gameOverModal;
        public Text gameOverScoreText;
        public Button gameOverRetryButton;
        public Button gameOverMenuButton;

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

        public float bossArenaCenterX = 35.5f;
        public float bossArenaMinX = 28.0f;
        public float bossArenaMaxX = 37.2f;
        public float slingshotMaxPull = 2.35f;
        public float slingshotPower = 8.8f;
        public float projectileMaxFlightTime = 4.8f;
        public Vector2 cameraMin = new Vector2(-8f, -3.4f);
        public Vector2 cameraMax = new Vector2(78f, 5.4f);
        public Vector3 startingRecoveryPoint = new Vector3(-8f, 0.65f, 0f);

        private static readonly QuizQuestion[] FallbackQuestions =
        {
            new QuizQuestion(CyberQuestionCategory.Password, "PASSWORD GATE", "Password yang baik sebaiknya...", new[] { "Panjang, unik, dan sulit ditebak", "Sama untuk semua akun", "Berisi tanggal lahir", "Dibagikan ke teman" }, 0, "Benar. Password perlu panjang, unik, dan tidak dipakai ulang."),
            new QuizQuestion(CyberQuestionCategory.Malware, "MALWARE BLOCK", "Lampiran asing dari email tidak dikenal sebaiknya...", new[] { "Tidak dibuka sembarangan", "Langsung dijalankan", "Dibagikan ulang", "Diubah namanya saja" }, 0, "Benar. Lampiran asing bisa membawa malware."),
            new QuizQuestion(CyberQuestionCategory.Network, "NETWORK WALL", "Firewall membantu kita untuk...", new[] { "Menyaring koneksi berbahaya", "Membuka semua port", "Membuat virus", "Mematikan update" }, 0, "Benar. Firewall membantu mengontrol koneksi masuk dan keluar."),
            new QuizQuestion(CyberQuestionCategory.Privacy, "PRIVACY LOCK", "Data yang tidak boleh dibagikan sembarangan adalah...", new[] { "OTP, password, NIK", "Genre game favorit", "Warna kesukaan", "Nama panggilan" }, 0, "Benar. Data sensitif dapat dipakai untuk penipuan.")
        };

        private readonly Color[] categoryColors =
        {
            new Color(0.18f, 0.58f, 1f, 1f),
            new Color(0.40f, 0.86f, 0.28f, 1f),
            new Color(1.00f, 0.78f, 0.18f, 1f),
            new Color(0.78f, 0.35f, 1f, 1f)
        };

        private GameMode mode = GameMode.Adventure;
        private CyberGuardianBossShieldBlock activeQuizBlock;
        private int currentDifficultyIndex = 1;
        private int playerHealth;
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

        public bool PlayerInputEnabled => mode != GameMode.Victory && mode != GameMode.Defeat && !quizOpen;

        private void Awake()
        {
            currentDifficultyIndex = Mathf.Clamp(PlayerPrefs.GetInt(CyberGuardianMainMenu.DifficultyKey, 1), 0, 2);
            GetActiveDifficulty();
            playerHealth = 100;
            bossHealth = 100;
            score = 0;
            boostEnergy = 100f;
            currentRecoveryPoint = startingRecoveryPoint;

            if (player != null)
            {
                player.game = this;
                currentRecoveryPoint = player.transform.position + Vector3.up * 0.2f;
            }

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

            SetStatus("ADVENTURE MODE: A/D MOVE, SPACE JUMP, J MELEE, SHIFT BOOST");
            RefreshHud();
        }

        private void Update()
        {
            invulnerabilityTimer = Mathf.Max(0f, invulnerabilityTimer - Time.deltaTime);
            RegenerateBoost();
            UpdateCamera();

            if (mode == GameMode.Adventure && player != null && player.transform.position.x >= bossArenaMinX - 0.35f)
            {
                EnterBossMode();
            }

            if (mode == GameMode.BossSlingshot)
            {
                ClampPlayerToBossArena();
                HandleSlingshotInput();
                HandleBossAttack();
            }

            if (projectileInFlight)
            {
                MonitorSlingshotProjectile();
            }

            if (player != null && player.transform.position.y < -8f)
            {
                RecoverPlayerFromAbyss();
            }
        }

        public void EnterBossMode()
        {
            if (mode != GameMode.Adventure)
            {
                return;
            }

            mode = GameMode.BossSlingshot;
            if (player != null)
            {
                player.InBossMode = true;
            }

            bossFireTimer = 1.25f;
            ResetSlingshotProjectile();
            SetStatus("BOSS MODE: CLICK OR DRAG ANYWHERE TO PULL PATCH CORE");
            RefreshHud();
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
            score = Mathf.Min(999, score + 5);
            PlaySfx(hitSfx);
            SetStatus("Threat deleted. Keep moving.");
            RefreshHud();
        }

        public void DamagePlayer(int damage, string source)
        {
            if (mode == GameMode.Defeat || mode == GameMode.Victory || invulnerabilityTimer > 0f)
            {
                return;
            }

            invulnerabilityTimer = 0.75f;
            playerHealth = Mathf.Max(0, playerHealth - damage);
            PlaySfx(wrongSfx);
            SetStatus(playerHealth <= 0 ? "SYSTEM BREACH - GAME OVER" : "DAMAGE FROM " + source.ToUpperInvariant());
            RefreshHud();

            if (playerHealth <= 0)
            {
                BeginDefeatSequence(source);
            }
        }

        private void BeginDefeatSequence(string source)
        {
            if (defeatSequenceStarted)
            {
                return;
            }

            defeatSequenceStarted = true;
            mode = GameMode.Defeat;
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

            SetStatus("SYSTEM BREACH - GUARDIAN DESTROYED");
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
                gameOverScoreText.text = "SCORE " + score.ToString("0");
            }

            if (gameOverModal != null)
            {
                gameOverModal.SetActive(true);
            }
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

        public void ProjectileHitBoss()
        {
            if (!projectileInFlight || mode != GameMode.BossSlingshot)
            {
                return;
            }

            projectileInFlight = false;
            projectileFlightTimer = 0f;
            bossHealth = Mathf.Max(0, bossHealth - 18);
            score = Mathf.Min(999, score + 20);
            PlaySfx(hitSfx);
            SetStatus(bossHealth <= 0 ? "BOSS PURGED - LEVEL CLEAR" : "BOSS HIT. OPEN MORE ANGLES.");
            ResetSlingshotProjectile();
            RefreshHud();

            if (bossHealth <= 0)
            {
                mode = GameMode.Victory;
                if (player != null)
                {
                    player.InBossMode = false;
                }

                if (!string.IsNullOrEmpty(nextSceneName))
                {
                    SetStatus("LEVEL CLEAR - OPENING NEXT SECTOR");
                    Invoke(nameof(LoadNextScene), 1.35f);
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
            SetStatus("PATCH LOST. TARGET A QUIZ BLOCK OR OPEN BOSS GAP.");
            ResetSlingshotProjectile();
        }

        public Color GetCategoryColor(int category)
        {
            return categoryColors[Mathf.Abs(category) % categoryColors.Length];
        }

        public void SetRecoveryPoint(Vector3 point)
        {
            currentRecoveryPoint = point;
            SetStatus("RECOVERY NODE SYNCED");
        }

        public void RecoverPlayerFromAbyss()
        {
            if (player == null || mode == GameMode.Defeat || mode == GameMode.Victory)
            {
                return;
            }

            Rigidbody2D body = player.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }

            player.transform.position = currentRecoveryPoint;
            boostEnergy = Mathf.Min(100f, boostEnergy + 18f);
            SetStatus("RECOVERED FROM CODE ABYSS - FIND ANOTHER ROUTE");
            RefreshHud();
        }

        public bool TryUseBoost(float cost)
        {
            if (mode == GameMode.Defeat || mode == GameMode.Victory || quizOpen || boostEnergy < cost)
            {
                return false;
            }

            boostEnergy = Mathf.Max(0f, boostEnergy - cost);
            SetStatus("BOOST BURST: FAST MOVE CHARGED");
            RefreshHud();
            return true;
        }

        private void RegenerateBoost()
        {
            if (mode == GameMode.Defeat || mode == GameMode.Victory || quizOpen)
            {
                return;
            }

            float previous = boostEnergy;
            boostEnergy = Mathf.Min(100f, boostEnergy + 28f * Time.deltaTime);
            if (!Mathf.Approximately(previous, boostEnergy))
            {
                RefreshHud();
            }
        }

        private void WireUi()
        {
            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(ReturnToMenu);
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveAllListeners();
                resetButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
            }

            if (gameOverRetryButton != null)
            {
                gameOverRetryButton.onClick.RemoveAllListeners();
                gameOverRetryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
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
                SetStatus("AIMING PATCH CORE - RELEASE TO FIRE");
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

            float pull = Mathf.Clamp(aimDirection.magnitude * 0.35f, 0.75f, slingshotMaxPull);
            return -aimDirection.normalized * pull;
        }

        private void HandleBossAttack()
        {
            if (quizOpen || bossProjectilePrefab == null || bossProjectileSpawn == null || player == null)
            {
                return;
            }

            bossFireTimer -= Time.deltaTime;
            if (bossFireTimer > 0f)
            {
                return;
            }

            bossFireTimer = Mathf.Lerp(2.0f, 0.85f, 1f - bossHealth / 100f);
            float fireY = player.transform.position.y + 0.35f;
            if (IsBossLineBlocked(fireY))
            {
                Vector2 breachSpawn = new Vector2(
                    Mathf.Clamp(player.transform.position.x + Random.Range(-1.8f, 1.8f), bossArenaMinX + 0.65f, bossArenaMaxX - 0.65f),
                    cameraMax.y + 0.85f);
                Vector2 breachTarget = (Vector2)player.transform.position + new Vector2(Random.Range(-0.35f, 0.35f), 0.12f);
                SpawnBossProjectile(breachSpawn, breachTarget, 6.2f, 8);
                PlaySfx(bossShotSfx);
                SetStatus("BOSS BREACH PACKET FROM ABOVE - KEEP MOVING");
                return;
            }

            SpawnBossProjectile(bossProjectileSpawn.position, (Vector2)player.transform.position + new Vector2(0f, 0.35f), 7.0f, 12);
            PlaySfx(bossShotSfx);
            SetStatus("BOSS ATTACK THROUGH AN OPEN GAP - DODGE");
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
            QuizQuestion question = GetQuestion(block.category, score + block.category + Mathf.RoundToInt(block.transform.position.y * 10f));

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

            for (int i = 0; answerButtons != null && i < answerButtons.Length; i++)
            {
                bool hasAnswer = question.answers != null && i < question.answers.Length;
                if (answerButtons[i] != null)
                {
                    answerButtons[i].gameObject.SetActive(hasAnswer);
                    answerButtons[i].interactable = hasAnswer;
                }

                if (answerLabels != null && i < answerLabels.Length && answerLabels[i] != null)
                {
                    answerLabels[i].text = hasAnswer ? question.answers[i] : string.Empty;
                }
            }

            if (quizModal != null)
            {
                quizModal.SetActive(true);
            }

            SetStatus("ANSWER TO DESTROY THIS BOSS SHIELD BLOCK");
            RefreshHud();
        }

        private void ChooseAnswer(int choice)
        {
            if (!quizOpen || activeQuizBlock == null)
            {
                return;
            }

            QuizQuestion question = GetQuestion(activeQuizBlock.category, score + activeQuizBlock.category + Mathf.RoundToInt(activeQuizBlock.transform.position.y * 10f));
            for (int i = 0; answerButtons != null && i < answerButtons.Length; i++)
            {
                if (answerButtons[i] != null)
                {
                    answerButtons[i].interactable = false;
                }
            }

            if (choice == question.correctIndex)
            {
                activeQuizBlock.ClearBlock();
                playerHealth = Mathf.Min(100, playerHealth + 4);
                score = Mathf.Min(999, score + 8);
                PlaySfx(shieldSfx);
                if (feedbackText != null)
                {
                    feedbackText.text = question.feedback;
                }

                SetStatus("BLOCK DESTROYED. A NEW ATTACK GAP EXISTS.");
            }
            else
            {
                activeQuizBlock.PulseWrong();
                DamagePlayer(12, "wrong answer");
                bossFireTimer = Mathf.Min(bossFireTimer, 0.35f);
                if (feedbackText != null)
                {
                    feedbackText.text = "Jawaban salah. Shield melemah dan boss mendapat peluang menyerang.";
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
                DamagePlayer(6, "skipped quiz");
            }

            CloseQuiz();
            ResetSlingshotProjectile();
        }

        private void CloseQuiz()
        {
            quizOpen = false;
            activeQuizBlock = null;
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
                return quizQuestionBank.GetQuestion(category, seed, FallbackQuestions);
            }

            return FallbackQuestions[Mathf.Abs(category) % FallbackQuestions.Length];
        }

        private void FireSlingshot(Vector2 velocity)
        {
            projectileInFlight = true;
            projectileFlightTimer = 0f;
            HideSlingshotLines();
            if (slingshotProjectile != null && !slingshotProjectile.gameObject.activeSelf)
            {
                slingshotProjectile.gameObject.SetActive(true);
                slingshotProjectile.position = GetSlingshotRestPosition();
            }

            if (slingshotBody != null)
            {
                slingshotBody.simulated = true;
                slingshotBody.linearVelocity = velocity;
                slingshotBody.angularVelocity = -450f;
            }

            if (slingshotCollider != null)
            {
                slingshotCollider.enabled = true;
            }

            PlaySfx(bossShotSfx);
            SetStatus("PATCH CORE LAUNCHED");
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
            return (target - start - 0.5f * gravity * flightTime * flightTime) / flightTime;
        }

        private Vector2 GetLaunchVelocity(Vector2 pullOffset)
        {
            return -pullOffset * slingshotPower;
        }

        private Vector2 GetSlingshotRestPosition()
        {
            Vector2 basePosition = player != null ? player.transform.position : Vector3.zero;
            return basePosition + new Vector2(0.60f * (player != null ? player.FacingDirection : 1), 0.82f);
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
            Vector2 anchorA = rest + new Vector2(-0.25f, 0.24f);
            Vector2 anchorB = rest + new Vector2(-0.25f, -0.24f);
            SetLine(slingshotBandA, anchorA, projectilePosition);
            SetLine(slingshotBandB, anchorB, projectilePosition);

            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = true;
                trajectoryLine.positionCount = 18;
                for (int i = 0; i < trajectoryLine.positionCount; i++)
                {
                    float time = i * 0.105f;
                    Vector2 point = projectilePosition + velocity * time + 0.5f * Physics2D.gravity * time * time;
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
            player.transform.position = position;
        }

        private void UpdateCamera()
        {
            if (gameplayCamera == null || player == null)
            {
                return;
            }

            float targetSize = 5.4f;
            Vector3 target;
            if (mode == GameMode.Defeat)
            {
                targetSize = 2.65f;
                target = new Vector3(player.transform.position.x, player.transform.position.y + 0.45f, -10f);
            }
            else
            {
                bool bossView = mode == GameMode.BossSlingshot || mode == GameMode.Victory;
                float facingLead = player.FacingDirection >= 0 ? 3.1f : -1.6f;
                float targetX = bossView ? bossArenaCenterX : player.transform.position.x + facingLead;
                float targetY = bossView ? Mathf.Lerp(player.transform.position.y + 1.1f, 2.25f, 0.35f) : player.transform.position.y + 1.15f;
                target = new Vector3(Mathf.Clamp(targetX, cameraMin.x, cameraMax.x), Mathf.Clamp(targetY, cameraMin.y, cameraMax.y), -10f);
            }

            gameplayCamera.transform.position = Vector3.Lerp(gameplayCamera.transform.position, target, Time.deltaTime * 4.5f);
            gameplayCamera.orthographicSize = Mathf.Lerp(gameplayCamera.orthographicSize, targetSize, Time.deltaTime * 3.8f);
        }

        private void RefreshHud()
        {
            if (healthText != null)
            {
                healthText.text = "HP";
            }

            if (bossText != null)
            {
                bossText.text = "BOSS";
            }

            if (scoreText != null)
            {
                scoreText.text = score.ToString("0");
            }

            if (modeText != null)
            {
                modeText.text = "BOOST";
            }

            if (playerHealthFill != null)
            {
                playerHealthFill.fillAmount = playerHealth / 100f;
            }

            if (boostEnergyFill != null)
            {
                boostEnergyFill.fillAmount = boostEnergy / 100f;
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

        private DifficultyProfile GetActiveDifficulty()
        {
            if (difficultyProfiles != null && difficultyProfiles.Length > 0)
            {
                currentDifficultyIndex = Mathf.Clamp(currentDifficultyIndex, 0, difficultyProfiles.Length - 1);
                return difficultyProfiles[currentDifficultyIndex];
            }

            return null;
        }

        private void ReturnToMenu()
        {
            Time.timeScale = 1f;
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
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }
}
