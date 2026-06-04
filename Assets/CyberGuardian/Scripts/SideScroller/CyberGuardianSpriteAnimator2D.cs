using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianSpriteAnimator2D : MonoBehaviour
    {
        public CyberGuardianPlayerController player;
        public SpriteRenderer spriteRenderer;
        public Sprite[] idleEast;
        public Sprite[] idleWest;
        public Sprite[] runEast;
        public Sprite[] runWest;
        public Sprite[] jumpEast;
        public Sprite[] jumpWest;
        public Sprite[] fireEast;
        public Sprite[] fireWest;
        public float idleFps = 5f;
        public float runFps = 10f;
        public float jumpFps = 9f;
        public float fireFps = 12f;

        private string currentState;
        private int frameIndex;
        private float frameTimer;

        private void Awake()
        {
            if (player == null)
            {
                player = GetComponentInParent<CyberGuardianPlayerController>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void LateUpdate()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            bool facingEast = player == null || player.FacingDirection >= 0;
            Sprite[] frames;
            float fps;
            string state;

            if (player != null && player.IsAttackingForAnimation)
            {
                state = facingEast ? "fire_east" : "fire_west";
                frames = facingEast ? fireEast : fireWest;
                fps = fireFps;
            }
            else if (player != null && (!player.IsGroundedForAnimation || Mathf.Abs(player.Velocity.y) > 0.18f))
            {
                state = facingEast ? "jump_east" : "jump_west";
                frames = facingEast ? jumpEast : jumpWest;
                fps = jumpFps;
            }
            else if (player != null && Mathf.Abs(player.HorizontalInput) > 0.05f)
            {
                state = facingEast ? "run_east" : "run_west";
                frames = facingEast ? runEast : runWest;
                fps = runFps;
            }
            else
            {
                state = facingEast ? "idle_east" : "idle_west";
                frames = facingEast ? idleEast : idleWest;
                fps = idleFps;
            }

            frames = ResolveFrames(frames, facingEast);
            if (frames == null || frames.Length == 0)
            {
                return;
            }

            if (currentState != state)
            {
                currentState = state;
                frameIndex = 0;
                frameTimer = 0f;
            }

            frameTimer += Time.deltaTime;
            float frameDuration = 1f / Mathf.Max(1f, fps);
            while (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                frameIndex = (frameIndex + 1) % frames.Length;
            }

            spriteRenderer.sprite = frames[Mathf.Abs(frameIndex) % frames.Length];
            spriteRenderer.flipX = false;
        }

        private Sprite[] ResolveFrames(Sprite[] preferred, bool facingEast)
        {
            if (preferred != null && preferred.Length > 0)
            {
                return preferred;
            }

            if (facingEast)
            {
                if (runEast != null && runEast.Length > 0)
                {
                    return runEast;
                }

                if (idleEast != null && idleEast.Length > 0)
                {
                    return idleEast;
                }
            }
            else
            {
                if (runWest != null && runWest.Length > 0)
                {
                    return runWest;
                }

                if (idleWest != null && idleWest.Length > 0)
                {
                    return idleWest;
                }
            }

            return idleEast != null && idleEast.Length > 0 ? idleEast : idleWest;
        }
    }
}
