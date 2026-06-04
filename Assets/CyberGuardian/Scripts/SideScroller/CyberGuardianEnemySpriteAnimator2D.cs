using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianEnemySpriteAnimator2D : MonoBehaviour
    {
        public CyberGuardianEnemy enemy;
        public SpriteRenderer spriteRenderer;
        public Sprite[] walkEast;
        public Sprite[] walkWest;
        public Sprite[] attackEast;
        public Sprite[] attackWest;
        public float walkFps = 8f;
        public float attackFps = 12f;

        private string currentState;
        private int frameIndex;
        private float frameTimer;

        private void Awake()
        {
            if (enemy == null)
            {
                enemy = GetComponentInParent<CyberGuardianEnemy>();
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

            bool east = enemy == null || enemy.FacingDirection >= 0;
            bool attacking = enemy != null && enemy.IsAttackingForAnimation;
            string state = (attacking ? "attack_" : "walk_") + (east ? "east" : "west");
            Sprite[] frames = ResolveFrames(attacking, east);
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

            float fps = attacking ? attackFps : walkFps;
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

        private Sprite[] ResolveFrames(bool attacking, bool east)
        {
            Sprite[] preferred = attacking
                ? (east ? attackEast : attackWest)
                : (east ? walkEast : walkWest);
            if (preferred != null && preferred.Length > 0)
            {
                return preferred;
            }

            Sprite[] fallback = east ? walkEast : walkWest;
            if (fallback != null && fallback.Length > 0)
            {
                return fallback;
            }

            fallback = east ? walkWest : walkEast;
            if (fallback != null && fallback.Length > 0)
            {
                return fallback;
            }

            return walkEast;
        }
    }
}
