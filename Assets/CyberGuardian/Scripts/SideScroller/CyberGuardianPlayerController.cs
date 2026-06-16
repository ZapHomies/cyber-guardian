using UnityEngine;
using UnityEngine.EventSystems;

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

        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private float meleeCooldown;
        private float rangedCooldown;
        private float coyoteCounter;
        private float jumpBufferCounter;
        private float boostTimer;
        private float attackAnimationTimer;
        private float boostDirection = 1f;
        private float baseGravityScale = 1f;
        private Vector3 baseVisualScale;
        private bool facingRight = true;

        public bool InBossMode { get; set; }
        public bool FlightMode { get; private set; }
        public int FacingDirection => facingRight ? 1 : -1;
        public float HorizontalInput { get; private set; }
        public bool IsGroundedForAnimation { get; private set; }
        public bool IsBoosting { get; private set; }
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
                IsGroundedForAnimation = IsGrounded();
                return;
            }

            attackAnimationTimer = Mathf.Max(0f, attackAnimationTimer - Time.deltaTime);
            rangedCooldown = Mathf.Max(0f, rangedCooldown - Time.deltaTime);
            int controlScheme = CyberGuardianMainMenu.GetControlScheme();
            bool useWasd = controlScheme != 2;
            bool useArrow = controlScheme != 1;
            float horizontal = 0f;
            if (useWasd)
            {
                horizontal += Input.GetKey(KeyCode.A) ? -1f : 0f;
                horizontal += Input.GetKey(KeyCode.D) ? 1f : 0f;
            }

            if (useArrow)
            {
                horizontal += Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;
                horizontal += Input.GetKey(KeyCode.RightArrow) ? 1f : 0f;
            }

            if (controlScheme == 0 && Mathf.Abs(horizontal) < 0.05f)
            {
                horizontal = Input.GetAxisRaw("Horizontal");
            }

            HorizontalInput = Mathf.Clamp(horizontal, -1f, 1f);
            if (Mathf.Abs(horizontal) > 0.05f)
            {
                facingRight = horizontal > 0f;
                ApplyFacing();
            }

            float vertical = 0f;
            if (useWasd)
            {
                vertical += Input.GetKey(KeyCode.W) ? 1f : 0f;
                vertical += Input.GetKey(KeyCode.S) ? -1f : 0f;
            }

            if (useArrow)
            {
                vertical += Input.GetKey(KeyCode.UpArrow) ? 1f : 0f;
                vertical += Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                vertical += 1f;
            }

            if (controlScheme == 0 && Mathf.Abs(vertical) < 0.05f)
            {
                vertical = Input.GetAxisRaw("Vertical");
            }

            bool boostPressed = (controlScheme == 1 && Input.GetKeyDown(KeyCode.LeftShift))
                || (controlScheme == 2 && Input.GetKeyDown(KeyCode.RightShift))
                || (controlScheme == 0 && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.K)));
            if (FlightMode)
            {
                HandleFlightMovement(horizontal, vertical, boostPressed);
                return;
            }

            bool grounded = IsGrounded();
            IsGroundedForAnimation = grounded;
            coyoteCounter = grounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - Time.deltaTime);
            bool jumpPressed = Input.GetKeyDown(KeyCode.Space)
                || (useWasd && Input.GetKeyDown(KeyCode.W))
                || (useArrow && Input.GetKeyDown(KeyCode.UpArrow));
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
            }

            if (boostTimer > 0f)
            {
                boostTimer = Mathf.Max(0f, boostTimer - Time.deltaTime);
                body.linearVelocity = new Vector2(boostDirection * boostSpeed, Mathf.Max(body.linearVelocity.y, boostLift));
            }
            else
            {
                body.linearVelocity = new Vector2(horizontal * runSpeed, body.linearVelocity.y);
            }

            IsBoosting = boostTimer > 0f;

            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                game.PlayPlayerJumpSfx();
            }

            meleeCooldown = Mathf.Max(0f, meleeCooldown - Time.deltaTime);
            if (!InBossMode && meleeCooldown <= 0f && Input.GetKeyDown(KeyCode.J))
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
            if (!InBossMode && rangedCooldown <= 0f && (Input.GetKeyDown(KeyCode.L) || (Input.GetMouseButtonDown(0) && !pointerOverUi)))
            {
                FireAdventureProjectile();
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

        private void FireAdventureProjectile()
        {
            if (adventureProjectilePrefab == null || game == null)
            {
                return;
            }

            rangedCooldown = rangedAttackCooldown;
            TriggerFireAnimation(0.28f);
            Vector3 spawnPosition = projectileSpawn != null
                ? transform.position + new Vector3(Mathf.Abs(projectileSpawn.localPosition.x) * FacingDirection, projectileSpawn.localPosition.y, 0f)
                : transform.position + new Vector3(0.62f * FacingDirection, 0.42f, 0f);
            GameObject shot = Instantiate(adventureProjectilePrefab, spawnPosition, Quaternion.identity);
            shot.SetActive(true);
            game.PlayPlayerShootSfx();
            CyberGuardianPlayerProjectile2D projectile = shot.GetComponent<CyberGuardianPlayerProjectile2D>();
            if (projectile != null)
            {
                projectile.game = game;
                projectile.damage = rangedDamage;
                projectile.velocity = new Vector2(FacingDirection * rangedProjectileSpeed, 0.15f);
                projectile.lifetime = 1.55f;
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
    }
}
