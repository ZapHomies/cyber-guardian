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
            EmphasizeCategoryVisuals();
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

        private void EmphasizeCategoryVisuals()
        {
            Color categoryColor = game != null ? game.GetCategoryColor(category) : Color.white;
            ResizeChildSprite("Quiz Block Category Aura", new Vector2(0.86f, 0.74f), new Color(categoryColor.r, categoryColor.g, categoryColor.b, 0.36f));
            ResizeChildSprite("Category Charge Fill", new Vector2(0.54f, 0.42f), new Color(categoryColor.r, categoryColor.g, categoryColor.b, 0.88f));
            ResizeChildSprite("Quiz Block Scanline Top", new Vector2(0.64f, 0.055f), new Color(categoryColor.r, categoryColor.g, categoryColor.b, 0.96f));
            ResizeChildSprite("Quiz Block Scanline Bottom", new Vector2(0.64f, 0.055f), new Color(0.05f, 0.95f, 1f, 0.84f));
        }

        private void ResizeChildSprite(string childName, Vector2 targetSize, Color color)
        {
            Transform child = transform.Find(childName);
            if (child == null)
            {
                return;
            }

            SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null)
            {
                return;
            }

            renderer.color = color;
            Vector2 spriteSize = renderer.sprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            {
                return;
            }

            child.localScale = new Vector3(targetSize.x / spriteSize.x, targetSize.y / spriteSize.y, 1f);
        }
    }
}
