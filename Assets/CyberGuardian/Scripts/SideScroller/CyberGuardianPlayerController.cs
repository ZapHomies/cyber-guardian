using UnityEngine;

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
        public float meleeRange = 1.25f;
        public int meleeDamage = 1;
        public Transform visualRoot;

        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private float meleeCooldown;
        private float coyoteCounter;
        private float jumpBufferCounter;
        private float boostTimer;
        private float boostDirection = 1f;
        private Vector3 baseVisualScale;
        private bool facingRight = true;

        public bool InBossMode { get; set; }
        public int FacingDirection => facingRight ? 1 : -1;
        public float HorizontalInput { get; private set; }
        public bool IsGroundedForAnimation { get; private set; }
        public bool IsBoosting { get; private set; }
        public Vector2 Velocity => body != null ? body.linearVelocity : Vector2.zero;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
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
                    body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
                }

                jumpBufferCounter = 0f;
                boostTimer = 0f;
                HorizontalInput = 0f;
                IsBoosting = false;
                IsGroundedForAnimation = IsGrounded();
                return;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(horizontal) < 0.05f)
            {
                horizontal = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f)
                    + (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);
            }

            HorizontalInput = Mathf.Clamp(horizontal, -1f, 1f);
            if (Mathf.Abs(horizontal) > 0.05f)
            {
                facingRight = horizontal > 0f;
                ApplyFacing();
            }

            bool grounded = IsGrounded();
            IsGroundedForAnimation = grounded;
            coyoteCounter = grounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - Time.deltaTime);
            }

            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.K)) && game.TryUseBoost(boostCost))
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
            }

            meleeCooldown = Mathf.Max(0f, meleeCooldown - Time.deltaTime);
            if (!InBossMode && meleeCooldown <= 0f && Input.GetKeyDown(KeyCode.J))
            {
                meleeCooldown = 0.32f;
                Vector2 center = (Vector2)transform.position + new Vector2(FacingDirection * 0.92f, 0.12f);
                game.PlayerMelee(center, meleeRange, meleeDamage);
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(0.70f, 1f, 1f, 1f);
                    CancelInvoke(nameof(RestoreColor));
                    Invoke(nameof(RestoreColor), 0.10f);
                }
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

            float xScale = Mathf.Abs(baseVisualScale.x) * FacingDirection;
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
