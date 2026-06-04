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

        private Collider2D platformCollider;
        private SpriteRenderer[] renderers;
        private Color[] baseColors;
        private bool breaking;

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
            if (!breaking && collision.collider != null && collision.collider.GetComponent<CyberGuardianPlayerController>() != null)
            {
                StartCoroutine(BreakAndRespawn());
            }
        }

        private IEnumerator BreakAndRespawn()
        {
            breaking = true;
            float elapsed = 0f;
            while (elapsed < breakDelay)
            {
                elapsed += Time.deltaTime;
                float blink = Mathf.PingPong(Time.time * 11f, 1f);
                SetRendererColors(Color.Lerp(warningColor, Color.white, blink));
                yield return null;
            }

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
