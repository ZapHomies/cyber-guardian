using System.Collections;
using UnityEngine;

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianBreakawayPlatform : MonoBehaviour
    {
        public float breakDelay = 0.34f;
        public float respawnDelay = 2.3f;
        public Color warningColor = new Color(1f, 0.24f, 0.52f, 1f);
        public Sprite[] breakEffectFrames;
        public Sprite fallbackEffectSprite;

        private Collider2D platformCollider;
        private SpriteRenderer[] renderers;
        private Color[] baseColors;
        private bool breaking;
        private float forcedBreakDelay = -1f;

        private void Awake()
        {
            platformCollider = GetComponent<Collider2D>();
            renderers = GetComponentsInChildren<SpriteRenderer>();
            baseColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                baseColors[i] = renderers[i] != null ? renderers[i].color : Color.white;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!breaking && collision.collider != null && collision.collider.GetComponentInParent<CyberGuardianPlayerController>() != null)
            {
                StartCoroutine(BreakAndRespawn());
            }
        }

        public void ForceBreak(float delayOverride)
        {
            if (breaking)
            {
                return;
            }

            forcedBreakDelay = Mathf.Max(0f, delayOverride);
            StartCoroutine(BreakAndRespawn());
        }

        private IEnumerator BreakAndRespawn()
        {
            breaking = true;
            float activeBreakDelay = forcedBreakDelay >= 0f ? forcedBreakDelay : breakDelay;
            forcedBreakDelay = -1f;
            float elapsed = 0f;
            while (elapsed < activeBreakDelay)
            {
                elapsed += Time.deltaTime;
                float blink = Mathf.PingPong(Time.time * 11f, 1f);
                SetRendererColors(Color.Lerp(warningColor, Color.white, blink));
                yield return null;
            }

            SpawnBreakEffect();
            if (platformCollider != null)
            {
                platformCollider.enabled = false;
            }

            SetRenderersEnabled(false);
            yield return new WaitForSeconds(respawnDelay);

            if (platformCollider != null)
            {
                platformCollider.enabled = true;
            }

            SetRenderersEnabled(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].color = baseColors[i];
                }
            }

            breaking = false;
        }

        private void SpawnBreakEffect()
        {
            Sprite sprite = breakEffectFrames != null && breakEffectFrames.Length > 0 ? breakEffectFrames[0] : fallbackEffectSprite;
            if (sprite == null)
            {
                return;
            }

            GameObject effect = new GameObject("Breakaway Data Shatter Effect", typeof(SpriteRenderer));
            effect.transform.position = transform.position + Vector3.up * 0.08f;
            effect.transform.localScale = new Vector3(1.25f, 1.25f, 1f);
            SpriteRenderer renderer = effect.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.58f, 1f, 1f, 0.92f);
            renderer.sortingOrder = 35;

            if (breakEffectFrames != null && breakEffectFrames.Length > 1)
            {
                CyberGuardianSpriteFlipbookAnimator flipbook = effect.AddComponent<CyberGuardianSpriteFlipbookAnimator>();
                flipbook.spriteRenderer = renderer;
                flipbook.frames = breakEffectFrames;
                flipbook.framesPerSecond = 18f;
                flipbook.randomStart = false;
            }

            Destroy(effect, 0.46f);
        }

        private void SetRendererColors(Color color)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].color = color;
                }
            }
        }

        private void SetRenderersEnabled(bool enabled)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].enabled = enabled;
                }
            }
        }
    }
}
