using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CyberGuardian
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianPlayerController : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public Transform groundCheck;
        public LayerMask groundMask = ~0;
        public float runSpeed = 7.2f;
        public float jumpForce = 13.5f;
        public float groundCheckRadius = 0.40f;
        public float coyoteTime = 0.18f;
        public float jumpBufferTime = 0.18f;
        public float boostSpeed = 18f;
        public float boostLift = 1.4f;
        public float boostDuration = 0.16f;
        public float boostCost = 26f;
        public float flightSpeed = 8.2f;
        public float flightVerticalSpeed = 7.0f;
        public float flightBoostSpeed = 14.5f;
        public float flightSmoothing = 13.5f;
        public float meleeRange = 1.25f;
        public int meleeDamage = 1;
        public GameObject adventureProjectilePrefab;
        public Transform projectileSpawn;
        public float rangedProjectileSpeed = 12.5f;
        public float rangedAttackCooldown = 0.36f;
        public int rangedDamage = 1;
        public Transform visualRoot;
        public bool flipVisualRootWithFacing = true;
        public Sprite[] dashEffectFrames;
        public Sprite[] impactEffectFrames;
        public Sprite fallbackEffectSprite;
        public float crouchSpeedMultiplier = 0.42f;
        public float downDoubleTapWindow = 0.34f;
        public float smashDashDuration = 0.24f;
        public float smashDashSpeed = 15.5f;
        public float smashDownSpeed = 18f;
        public float smashBoostCost = 18f;
        public float smashImpactDamageRadius = 1.8f;
        public int smashImpactDamage = 2;

        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private float meleeCooldown;
        private float rangedCooldown;
        private float coyoteCounter;
        private float jumpBufferCounter;
        private float boostTimer;
        private float attackAnimationTimer;
        private float smashDashTimer;
        private float lastDownTapTime = -10f;
        private float boostDirection = 1f;
        private float smashDirection = 1f;
        private float smashLandingTimer;
        private float baseGravityScale = 1f;
        private Vector3 baseVisualScale;
        private bool facingRight = true;
        private bool crouching;
        private bool smashImpactTriggered;
        private bool smashLandingPending;

        public bool InBossMode { get; set; }
        public bool FlightMode { get; private set; }
        public int FacingDirection => facingRight ? 1 : -1;
        public float HorizontalInput { get; private set; }
        public bool IsGroundedForAnimation { get; private set; }
        public bool IsBoosting { get; private set; }
        public bool IsCrouchingForAnimation => crouching;
        public bool IsDashSmashing => smashDashTimer > 0f;
        public bool IsAttackingForAnimation => meleeCooldown > 0f || attackAnimationTimer > 0f;
        public Vector2 Velocity => body != null ? body.linearVelocity : Vector2.zero;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            baseGravityScale = body != null ? body.gravityScale : 1f;
            if (visualRoot == null)
            {
                visualRoot = transform;
            }

            spriteRenderer = visualRoot.GetComponentInChildren<SpriteRenderer>();
            baseVisualScale = visualRoot.localScale;
        }

        private void Update()
        {
            if (game == null || !game.PlayerInputEnabled)
            {
                if (body != null)
                {
                    body.linearVelocity = FlightMode ? Vector2.zero : new Vector2(0f, body.linearVelocity.y);
                }

                jumpBufferCounter = 0f;
                boostTimer = 0f;
                attackAnimationTimer = 0f;
                rangedCooldown = 0f;
                HorizontalInput = 0f;
                IsBoosting = false;
                crouching = false;
                smashDashTimer = 0f;
                smashLandingPending = false;
                smashLandingTimer = 0f;
                IsGroundedForAnimation = IsGrounded();
                return;
            }

            attackAnimationTimer = Mathf.Max(0f, attackAnimationTimer - Time.deltaTime);
            rangedCooldown = Mathf.Max(0f, rangedCooldown - Time.deltaTime);
            float horizontal = ReadHorizontalInput();

            HorizontalInput = Mathf.Clamp(horizontal, -1f, 1f);
            if (Mathf.Abs(horizontal) > 0.05f)
            {
                facingRight = horizontal > 0f;
                ApplyFacing();
            }

            float vertical = ReadVerticalInput();
            bool downHeld = IsDownHeld();
            bool downPressed = IsDownPressed();
            bool boostPressed = IsBoostPressed();
            if (FlightMode)
            {
                HandleFlightMovement(horizontal, vertical, boostPressed);
                return;
            }

            bool grounded = IsGrounded();
            IsGroundedForAnimation = grounded;
            if (smashLandingPending)
            {
                smashLandingTimer = Mathf.Max(0f, smashLandingTimer - Time.deltaTime);
                if (grounded)
                {
                    TriggerSmashImpact(transform.position + Vector3.down * 0.56f, smashDirection);
                }
                else if (smashLandingTimer <= 0f)
                {
                    smashLandingPending = false;
                }
            }

            coyoteCounter = grounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - Time.deltaTime);
            bool jumpPressed = IsJumpPressed();
            crouching = grounded && downHeld && smashDashTimer <= 0f && !jumpPressed;
            if (downPressed)
            {
                if (Time.time - lastDownTapTime <= downDoubleTapWindow)
                {
                    TryStartSmashDash(horizontal, grounded);
                    lastDownTapTime = -10f;
                }
                else
                {
                    lastDownTapTime = Time.time;
                }
            }

            if (jumpPressed)
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - Time.deltaTime);
            }

            if (boostPressed && game.TryUseBoost(boostCost))
            {
                boostTimer = boostDuration;
                boostDirection = Mathf.Abs(horizontal) > 0.05f ? Mathf.Sign(horizontal) : FacingDirection;
                facingRight = boostDirection > 0f;
                ApplyFacing();
                SpawnDashEffect(transform.position + new Vector3(-0.42f * boostDirection, -0.10f, 0f), -boostDirection);
            }

            if (smashDashTimer > 0f)
            {
                smashDashTimer = Mathf.Max(0f, smashDashTimer - Time.deltaTime);
                if (grounded)
                {
                    body.linearVelocity = new Vector2(smashDirection * smashDashSpeed, Mathf.Max(body.linearVelocity.y, 0.4f));
                }
                else
                {
                    body.linearVelocity = new Vector2(smashDirection * smashDashSpeed * 0.35f, -smashDownSpeed);
                }
            }
            else if (boostTimer > 0f)
            {
                boostTimer = Mathf.Max(0f, boostTimer - Time.deltaTime);
                body.linearVelocity = new Vector2(boostDirection * boostSpeed, Mathf.Max(body.linearVelocity.y, boostLift));
            }
            else
            {
                float speed = crouching ? runSpeed * crouchSpeedMultiplier : runSpeed;
                body.linearVelocity = new Vector2(horizontal * speed, body.linearVelocity.y);
            }

            IsBoosting = boostTimer > 0f || smashDashTimer > 0f;

            if (!crouching && jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                game.PlayPlayerJumpSfx();
            }

            meleeCooldown = Mathf.Max(0f, meleeCooldown - Time.deltaTime);
            if (!InBossMode && meleeCooldown <= 0f && IsMeleePressed())
            {
                meleeCooldown = 0.32f;
                TriggerFireAnimation(0.24f);
                Vector2 center = (Vector2)transform.position + new Vector2(FacingDirection * 0.92f, 0.12f);
                game.PlayerMelee(center, meleeRange, meleeDamage);
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(0.70f, 1f, 1f, 1f);
                    CancelInvoke(nameof(RestoreColor));
                    Invoke(nameof(RestoreColor), 0.10f);
                }
            }

            bool pointerOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (!InBossMode && rangedCooldown <= 0f && (IsShootPressed() || (Input.GetMouseButtonDown(0) && !pointerOverUi)))
            {
                FireAdventureProjectile();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision == null || (smashDashTimer <= 0f && !smashLandingPending))
            {
                return;
            }

            if (smashLandingPending && !IsGroundLikeCollision(collision))
            {
                return;
            }

            CyberGuardianBreakawayPlatform breakaway = collision.collider != null ? collision.collider.GetComponentInParent<CyberGuardianBreakawayPlatform>() : null;
            if (breakaway != null)
            {
                breakaway.ForceBreak(0.04f);
            }

            TriggerSmashImpact(transform.position + Vector3.down * 0.55f, Mathf.Sign(smashDirection));
        }

        private static bool IsGroundLikeCollision(Collision2D collision)
        {
            if (collision == null || collision.contactCount == 0)
            {
                return true;
            }

            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint2D contact = collision.GetContact(i);
                if (contact.normal.y > 0.35f)
                {
                    return true;
                }
            }

            return false;
        }

        private void TriggerSmashImpact(Vector3 position, float direction)
        {
            if (smashImpactTriggered)
            {
                return;
            }

            smashImpactTriggered = true;
            smashLandingPending = false;
            smashLandingTimer = 0f;
            SpawnImpactEffect(position, direction);
            if (game != null)
            {
                game.PlayerAreaImpact(position, smashImpactDamageRadius, smashImpactDamage, "GROUND SMASH: LEDAKAN DATA MENGHANTAM MUSUH");
            }
        }

        public void TriggerFireAnimation(float duration)
        {
            attackAnimationTimer = Mathf.Max(attackAnimationTimer, duration);
        }

        public void SetFlightMode(bool enabled)
        {
            if (FlightMode == enabled)
            {
                return;
            }

            FlightMode = enabled;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            boostTimer = 0f;
            if (body != null)
            {
                body.gravityScale = enabled ? 0f : baseGravityScale;
                body.linearVelocity = enabled ? Vector2.zero : body.linearVelocity;
            }

            IsGroundedForAnimation = enabled ? false : IsGrounded();
        }

        private void HandleFlightMovement(float horizontal, float vertical, bool boostPressed)
        {
            if (body == null)
            {
                return;
            }

            body.gravityScale = 0f;
            IsGroundedForAnimation = false;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;

            Vector2 input = new Vector2(Mathf.Clamp(horizontal, -1f, 1f), Mathf.Clamp(vertical, -1f, 1f));
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            if (boostPressed && game.TryUseBoost(boostCost))
            {
                boostTimer = boostDuration;
                boostDirection = Mathf.Abs(input.x) > 0.05f ? Mathf.Sign(input.x) : FacingDirection;
                if (Mathf.Abs(input.x) > 0.05f)
                {
                    facingRight = input.x > 0f;
                    ApplyFacing();
                }

                SpawnDashEffect(transform.position + new Vector3(-0.42f * boostDirection, -0.08f, 0f), -boostDirection);
            }

            boostTimer = Mathf.Max(0f, boostTimer - Time.deltaTime);
            IsBoosting = boostTimer > 0f;
            if (IsBoosting)
            {
                Vector2 boostVector = input.sqrMagnitude > 0.05f ? input.normalized : new Vector2(boostDirection, 0f);
                body.linearVelocity = boostVector * flightBoostSpeed;
            }
            else
            {
                Vector2 targetVelocity = new Vector2(input.x * flightSpeed, input.y * flightVerticalSpeed);
                body.linearVelocity = Vector2.Lerp(body.linearVelocity, targetVelocity, Time.deltaTime * flightSmoothing);
            }
        }

        private float ReadHorizontalInput()
        {
            int scheme = CyberGuardianMainMenu.GetControlScheme();
            float horizontal = 0f;
            horizontal += Input.GetKey(CyberGuardianMainMenu.GetLeftKey()) ? -1f : 0f;
            horizontal += Input.GetKey(CyberGuardianMainMenu.GetRightKey()) ? 1f : 0f;

            if (scheme == 0)
            {
                horizontal += Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;
                horizontal += Input.GetKey(KeyCode.RightArrow) ? 1f : 0f;
                if (Mathf.Abs(horizontal) < 0.05f)
                {
                    horizontal = Input.GetAxisRaw("Horizontal");
                }
            }

            return Mathf.Clamp(horizontal, -1f, 1f);
        }

        private float ReadVerticalInput()
        {
            int scheme = CyberGuardianMainMenu.GetControlScheme();
            float vertical = 0f;
            vertical += Input.GetKey(CyberGuardianMainMenu.GetUpKey()) ? 1f : 0f;
            vertical += Input.GetKey(CyberGuardianMainMenu.GetDownKey()) ? -1f : 0f;
            vertical += Input.GetKey(KeyCode.Space) ? 1f : 0f;

            if (scheme == 0)
            {
                vertical += Input.GetKey(KeyCode.UpArrow) ? 1f : 0f;
                vertical += Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;
                if (Mathf.Abs(vertical) < 0.05f)
                {
                    vertical = Input.GetAxisRaw("Vertical");
                }
            }

            return Mathf.Clamp(vertical, -1f, 1f);
        }

        private bool IsJumpPressed()
        {
            bool pressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(CyberGuardianMainMenu.GetUpKey());
            return CyberGuardianMainMenu.GetControlScheme() == 0
                ? pressed || Input.GetKeyDown(KeyCode.UpArrow)
                : pressed;
        }

        private bool IsDownHeld()
        {
            bool held = Input.GetKey(CyberGuardianMainMenu.GetDownKey());
            return CyberGuardianMainMenu.GetControlScheme() == 0
                ? held || Input.GetKey(KeyCode.DownArrow)
                : held;
        }

        private bool IsDownPressed()
        {
            bool pressed = Input.GetKeyDown(CyberGuardianMainMenu.GetDownKey());
            return CyberGuardianMainMenu.GetControlScheme() == 0
                ? pressed || Input.GetKeyDown(KeyCode.DownArrow)
                : pressed;
        }

        private bool IsBoostPressed()
        {
            bool pressed = Input.GetKeyDown(CyberGuardianMainMenu.GetBoostKey());
            return CyberGuardianMainMenu.GetControlScheme() == 0
                ? pressed || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.K)
                : pressed;
        }

        private bool IsMeleePressed()
        {
            return Input.GetKeyDown(CyberGuardianMainMenu.GetMeleeKey());
        }

        private bool IsShootPressed()
        {
            return Input.GetKeyDown(CyberGuardianMainMenu.GetShootKey());
        }

        private void TryStartSmashDash(float horizontal, bool grounded)
        {
            if (game == null || !game.TryUseBoost(smashBoostCost))
            {
                return;
            }

            smashDashTimer = smashDashDuration;
            smashDirection = Mathf.Abs(horizontal) > 0.05f ? Mathf.Sign(horizontal) : FacingDirection;
            smashImpactTriggered = false;
            smashLandingPending = !grounded;
            smashLandingTimer = grounded ? 0f : 1.6f;
            facingRight = smashDirection > 0f;
            crouching = false;
            ApplyFacing();
            SpawnDashEffect(transform.position + new Vector3(-0.42f * smashDirection, -0.22f, 0f), -smashDirection);
            if (grounded)
            {
                TriggerSmashImpact(transform.position + Vector3.down * 0.56f, smashDirection);
            }

            if (!grounded && body != null)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x * 0.25f, -smashDownSpeed);
            }
        }

        private void FireAdventureProjectile()
        {
            if (adventureProjectilePrefab == null || game == null)
            {
                return;
            }

            rangedCooldown = rangedAttackCooldown;
            TriggerFireAnimation(0.28f);
            float spawnYOffset = crouching ? 0.14f : 0.42f;
            Vector3 spawnPosition = projectileSpawn != null
                ? transform.position + new Vector3(Mathf.Abs(projectileSpawn.localPosition.x) * FacingDirection, spawnYOffset, 0f)
                : transform.position + new Vector3(0.62f * FacingDirection, spawnYOffset, 0f);
            GameObject shot = Instantiate(adventureProjectilePrefab, spawnPosition, Quaternion.identity);
            shot.SetActive(true);
            game.PlayPlayerShootSfx();
            CyberGuardianPlayerProjectile2D projectile = shot.GetComponent<CyberGuardianPlayerProjectile2D>();
            if (projectile != null)
            {
                projectile.game = game;
                projectile.damage = rangedDamage;
                projectile.velocity = new Vector2(FacingDirection * rangedProjectileSpeed, crouching ? 0f : 0.15f);
                projectile.lifetime = 0.92f;
                projectile.visualSize = new Vector2(0.34f, 0.34f);
                projectile.maxTravelDistance = 8.2f;
                projectile.destroyAfterOneAnimation = true;
            }
        }

        private bool IsGrounded()
        {
            Vector2 point = groundCheck != null ? groundCheck.position : transform.position + Vector3.down * 0.72f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(point, groundCheckRadius, groundMask);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hit = hits[i];
                if (hit != null && !hit.isTrigger && hit.attachedRigidbody != body && hit.gameObject != gameObject)
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyFacing()
        {
            if (visualRoot == null)
            {
                return;
            }

            float xScale = Mathf.Abs(baseVisualScale.x) * (flipVisualRootWithFacing ? FacingDirection : 1f);
            visualRoot.localScale = new Vector3(xScale, baseVisualScale.y, baseVisualScale.z);
        }

        private void RestoreColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        private void SpawnDashEffect(Vector3 position, float direction)
        {
            SpawnEffect("Guardian Dash Power Effect", position, direction, new Vector2(1.1f, 0.62f), dashEffectFrames);
        }

        private void SpawnImpactEffect(Vector3 position, float direction)
        {
            SpawnEffect("Guardian Platform Smash Effect", position, direction, new Vector2(1.42f, 0.50f), impactEffectFrames);
        }

        private void SpawnEffect(string objectName, Vector3 position, float direction, Vector2 size, Sprite[] frames)
        {
            EnsureEffectFramesLoaded();
            Sprite[] activeFrames = frames != null && frames.Length > 0 ? frames : dashEffectFrames;
            Sprite sprite = activeFrames != null && activeFrames.Length > 0 ? activeFrames[0] : GetFallbackEffectSprite();
            if (sprite == null)
            {
                return;
            }

            GameObject effect = new GameObject(objectName, typeof(SpriteRenderer));
            effect.transform.position = position;

            SpriteRenderer renderer = effect.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.52f, 1f, 1f, 0.88f);
            renderer.sortingOrder = 34;
            ScaleEffectToWorldSize(renderer, size, Mathf.Sign(direction == 0f ? FacingDirection : direction));

            if (activeFrames != null && activeFrames.Length > 1)
            {
                CyberGuardianSpriteFlipbookAnimator flipbook = effect.AddComponent<CyberGuardianSpriteFlipbookAnimator>();
                flipbook.spriteRenderer = renderer;
                flipbook.frames = activeFrames;
                flipbook.framesPerSecond = 18f;
                flipbook.randomStart = false;
            }

            float lifetime = activeFrames != null && activeFrames.Length > 1 ? Mathf.Max(0.36f, activeFrames.Length / 18f + 0.08f) : 0.44f;
            Destroy(effect, lifetime);
        }

        private static void ScaleEffectToWorldSize(SpriteRenderer renderer, Vector2 size, float direction)
        {
            if (renderer == null || renderer.sprite == null)
            {
                return;
            }

            Vector2 spriteSize = renderer.sprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            {
                return;
            }

            float xSign = Mathf.Sign(direction == 0f ? 1f : direction);
            renderer.transform.localScale = new Vector3(Mathf.Abs(size.x) / spriteSize.x * xSign, Mathf.Abs(size.y) / spriteSize.y, 1f);
        }

        private Sprite GetFallbackEffectSprite()
        {
            if (fallbackEffectSprite != null)
            {
                return fallbackEffectSprite;
            }

            const int size = 32;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f) / size;
                    float ny = (y + 0.5f) / size;
                    float distance = Mathf.Abs(ny - 0.5f) + Mathf.Abs(nx - 0.5f) * 0.34f;
                    float alpha = Mathf.Clamp01(1f - distance * 3.2f);
                    texture.SetPixel(x, y, new Color(0.35f, 1f, 1f, alpha));
                }
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            fallbackEffectSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 48f);
            return fallbackEffectSprite;
        }

        private void EnsureEffectFramesLoaded()
        {
#if UNITY_EDITOR
            string dashFolder = "Assets/CyberGuardian/assets/new/Super Package Retro Pixel Effects 32x32 pack 2 Free";
            Sprite[] dashSprites = LoadRuntimeEightFrameEffectSheet(dashFolder, "04.png", 0, 4, 2, 48f);
            if (dashSprites.Length > 0)
            {
                dashEffectFrames = dashSprites;
            }

            if (!HasUsableSlicedEffectFrames(impactEffectFrames))
            {
                string impactFolder = "Assets/CyberGuardian/assets/new/Super Pixel Effects Mini Pack 1/Super Pixel Effects Mini Pack 1/spritesheet/fx2_impact_shock_large_brown";
                Sprite[] sprites = LoadRuntimeMetadataEffectSheet(impactFolder, 48f);
                if (sprites.Length > 0)
                {
                    impactEffectFrames = sprites;
                }
            }

            Sprite[] fallbackFrames = HasUsableSlicedEffectFrames(impactEffectFrames) ? impactEffectFrames : dashEffectFrames;
            if (fallbackFrames != null && fallbackFrames.Length > 0)
            {
                fallbackEffectSprite = fallbackFrames[0];
            }
#endif
        }

        private static bool HasUsableSlicedEffectFrames(Sprite[] frames)
        {
            if (frames == null || frames.Length < 2 || frames[0] == null)
            {
                return false;
            }

            Rect rect = frames[0].rect;
            return rect.width < 180f && rect.height <= 96f;
        }

#if UNITY_EDITOR
        private static Sprite[] LoadRuntimeMetadataEffectSheet(string folderAssetPath, float pixelsPerUnit)
        {
            string absoluteFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "..", folderAssetPath));
            string imagePath = Path.Combine(absoluteFolder, "spritesheet.png");
            string metadataPath = Path.Combine(absoluteFolder, "spritesheet.txt");
            if (!File.Exists(imagePath) || !File.Exists(metadataPath))
            {
                return new Sprite[0];
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(imagePath)))
            {
                return new Sprite[0];
            }

            texture.filterMode = FilterMode.Point;
            List<Sprite> frames = new List<Sprite>();
            string[] lines = File.ReadAllLines(metadataPath);
            string sourceName = Path.GetFileName(folderAssetPath.TrimEnd('/', '\\'));
            for (int i = 0; i < lines.Length; i++)
            {
                if (!TryParseSpriteSheetMetadataLine(lines[i], out int x, out int y, out int width, out int height))
                {
                    continue;
                }

                int unityY = texture.height - y - height;
                if (!IsValidSpriteRect(texture, x, unityY, width, height))
                {
                    continue;
                }

                Rect rect = new Rect(x, unityY, width, height);
                Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.name = sourceName + "_runtime_frame_" + frames.Count.ToString("00");
                frames.Add(sprite);
            }

            if (frames.Count == 0)
            {
                AddInferredHorizontalFrames(texture, sourceName, lines.Length, pixelsPerUnit, frames);
            }

            return frames.ToArray();
        }

        private static bool IsValidSpriteRect(Texture2D texture, int x, int y, int width, int height)
        {
            return texture != null
                && x >= 0
                && y >= 0
                && width > 0
                && height > 0
                && x + width <= texture.width
                && y + height <= texture.height;
        }

        private static void AddInferredHorizontalFrames(Texture2D texture, string sourceName, int requestedCount, float pixelsPerUnit, List<Sprite> frames)
        {
            int frameCount = Mathf.Max(1, requestedCount);
            if (texture == null || frameCount <= 0)
            {
                return;
            }

            int frameWidth = Mathf.Max(1, texture.width / frameCount);
            int frameHeight = texture.height;
            for (int i = 0; i < frameCount; i++)
            {
                int x = i * frameWidth;
                if (!IsValidSpriteRect(texture, x, 0, frameWidth, frameHeight))
                {
                    continue;
                }

                Rect rect = new Rect(x, 0, frameWidth, frameHeight);
                Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.name = sourceName + "_runtime_inferred_frame_" + frames.Count.ToString("00");
                frames.Add(sprite);
            }
        }

        private static Sprite[] LoadRuntimeEightFrameEffectSheet(string folderAssetPath, string searchPattern, int sheetIndex, int columns, int rows, float pixelsPerUnit)
        {
            string absoluteFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "..", folderAssetPath));
            if (!Directory.Exists(absoluteFolder) || columns <= 0 || rows <= 0)
            {
                return new Sprite[0];
            }

            List<string> files = new List<string>(Directory.GetFiles(absoluteFolder, searchPattern, SearchOption.TopDirectoryOnly));
            files.Sort((a, b) =>
            {
                int numberA = ExtractTrailingNumber(a);
                int numberB = ExtractTrailingNumber(b);
                int numberCompare = numberA.CompareTo(numberB);
                return numberCompare != 0 ? numberCompare : string.Compare(a, b, System.StringComparison.OrdinalIgnoreCase);
            });

            if (files.Count == 0)
            {
                return new Sprite[0];
            }

            int clampedIndex = Mathf.Clamp(sheetIndex, 0, files.Count - 1);
            string assetPath = ToProjectAssetPath(files[clampedIndex]);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
            {
                return new Sprite[0];
            }

            int frameWidth = texture.width / columns;
            int frameHeight = texture.height / rows;
            int frameCount = columns * rows;
            if (frameWidth <= 0 || frameHeight <= 0)
            {
                return new Sprite[0];
            }

            List<Sprite> frames = new List<Sprite>(frameCount);
            string sourceName = Path.GetFileNameWithoutExtension(assetPath);
            for (int frame = 0; frame < frameCount; frame++)
            {
                int column = frame % columns;
                int row = frame / columns;
                Rect rect = new Rect(
                    column * frameWidth,
                    texture.height - ((row + 1) * frameHeight),
                    frameWidth,
                    frameHeight);
                Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.name = sourceName + "_runtime_frame_" + frame.ToString("00");
                frames.Add(sprite);
            }

            return frames.ToArray();
        }

        private static string ToProjectAssetPath(string path)
        {
            string normalized = path.Replace('\\', '/');
            int index = normalized.IndexOf("Assets/", System.StringComparison.OrdinalIgnoreCase);
            return index >= 0 ? normalized.Substring(index) : normalized;
        }

        private static bool TryParseSpriteSheetMetadataLine(string line, out int x, out int y, out int width, out int height)
        {
            x = 0;
            y = 0;
            width = 0;
            height = 0;
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            int equalsIndex = line.IndexOf('=');
            if (equalsIndex < 0 || equalsIndex >= line.Length - 1)
            {
                return false;
            }

            string[] parts = line.Substring(equalsIndex + 1).Trim().Split(' ');
            List<int> values = new List<int>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i], out int value))
                {
                    values.Add(value);
                }
            }

            if (values.Count < 4)
            {
                return false;
            }

            x = values[0];
            y = values[1];
            width = values[2];
            height = values[3];
            return width > 0 && height > 0;
        }

        private static int ExtractTrailingNumber(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            int end = -1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(name[i]))
                {
                    end = i;
                    break;
                }
            }

            if (end < 0)
            {
                return int.MaxValue;
            }

            int start = end;
            while (start > 0 && char.IsDigit(name[start - 1]))
            {
                start--;
            }

            return int.TryParse(name.Substring(start, end - start + 1), out int number) ? number : int.MaxValue;
        }
#endif
    }
}
