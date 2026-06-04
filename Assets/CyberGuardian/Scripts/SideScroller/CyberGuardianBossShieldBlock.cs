using UnityEngine;

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianBossShieldBlock : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public int category;
        public bool cleared;

        private SpriteRenderer spriteRenderer;
        private Collider2D blockCollider;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            blockCollider = GetComponent<Collider2D>();
        }

        public void ClearBlock()
        {
            cleared = true;
            if (blockCollider != null)
            {
                blockCollider.enabled = false;
            }

            gameObject.SetActive(false);
        }

        public void PulseWrong()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 0.25f, 0.32f, 1f);
                CancelInvoke(nameof(RestoreColor));
                Invoke(nameof(RestoreColor), 0.22f);
            }
        }

        private void RestoreColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = game != null ? game.GetCategoryColor(category) : Color.white;
            }
        }
    }
}
