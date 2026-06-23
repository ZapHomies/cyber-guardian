using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianProjectileVisual2D : MonoBehaviour
    {
        private static Sprite glowSprite;
        private static Material trailMaterial;

        private SpriteRenderer mainRenderer;
        private SpriteRenderer glowRenderer;
        private TrailRenderer trailRenderer;
        private Color glowColor = new Color(0.28f, 0.94f, 1f, 0.72f);
        private Color trailEndColor = new Color(1f, 0.18f, 0.72f, 0f);
        private float glowWorldSize = 0.52f;
        private Vector3 previousPosition;

        public void Configure(SpriteRenderer renderer, float worldGlowSize, float trailWidth, Color coreColor, Color endColor)
        {
            mainRenderer = renderer;
            glowWorldSize = Mathf.Max(0.12f, worldGlowSize);
            glowColor = coreColor;
            trailEndColor = endColor;
            EnsureVisuals(trailWidth);
            previousPosition = transform.position;
            RefreshVisuals();
        }

        private void OnEnable()
        {
            previousPosition = transform.position;
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
                trailRenderer.emitting = false;
            }
        }

        private void OnDisable()
        {
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }
        }

        private void LateUpdate()
        {
            EnsureVisuals(trailRenderer != null ? trailRenderer.startWidth : 0.18f);
            RefreshVisuals();

            float moved = Vector3.Distance(previousPosition, transform.position);
            if (trailRenderer != null)
            {
                trailRenderer.emitting = moved > 0.008f;
            }

            previousPosition = transform.position;
        }

        private void EnsureVisuals(float trailWidth)
        {
            if (mainRenderer == null)
            {
                mainRenderer = GetComponent<SpriteRenderer>();
            }

            if (glowRenderer == null)
            {
                Transform existing = transform.Find("Runtime Projectile Energy Halo");
                GameObject glowObject;
                if (existing != null)
                {
                    glowObject = existing.gameObject;
                }
                else
                {
                    glowObject = new GameObject("Runtime Projectile Energy Halo", typeof(SpriteRenderer));
                    glowObject.transform.SetParent(transform, false);
                }

                glowRenderer = glowObject.GetComponent<SpriteRenderer>();
                glowRenderer.sprite = GetGlowSprite();
                glowRenderer.color = glowColor;
                glowRenderer.sortingLayerID = mainRenderer != null ? mainRenderer.sortingLayerID : 0;
                glowRenderer.sortingOrder = mainRenderer != null ? mainRenderer.sortingOrder - 1 : 0;
            }

            if (trailRenderer == null)
            {
                trailRenderer = GetComponent<TrailRenderer>();
                if (trailRenderer == null)
                {
                    trailRenderer = gameObject.AddComponent<TrailRenderer>();
                }

                trailRenderer.material = GetTrailMaterial();
                trailRenderer.time = 0.22f;
                trailRenderer.minVertexDistance = 0.025f;
                trailRenderer.numCapVertices = 4;
                trailRenderer.numCornerVertices = 3;
                trailRenderer.textureMode = LineTextureMode.Stretch;
                trailRenderer.alignment = LineAlignment.View;
                trailRenderer.sortingLayerID = mainRenderer != null ? mainRenderer.sortingLayerID : 0;
                trailRenderer.sortingOrder = mainRenderer != null ? mainRenderer.sortingOrder - 2 : 0;
            }

            trailRenderer.startWidth = Mathf.Max(0.08f, trailWidth);
            trailRenderer.endWidth = 0.015f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(glowColor, 0.24f),
                    new GradientColorKey(new Color(0.30f, 0.72f, 1f, 1f), 0.62f),
                    new GradientColorKey(trailEndColor, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0.96f, 0f),
                    new GradientAlphaKey(0.78f, 0.38f),
                    new GradientAlphaKey(0f, 1f)
                });
            trailRenderer.colorGradient = gradient;
        }

        private void RefreshVisuals()
        {
            if (glowRenderer == null || glowRenderer.sprite == null)
            {
                return;
            }

            float pulse = Mathf.Lerp(0.90f, 1.12f, 0.5f + Mathf.Sin(Time.unscaledTime * 14f) * 0.5f);
            Vector2 spriteSize = glowRenderer.sprite.bounds.size;
            Vector3 parentScale = transform.lossyScale;
            float scaleX = glowWorldSize * pulse / Mathf.Max(0.001f, spriteSize.x * Mathf.Abs(parentScale.x));
            float scaleY = glowWorldSize * pulse / Mathf.Max(0.001f, spriteSize.y * Mathf.Abs(parentScale.y));
            glowRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            glowRenderer.transform.localRotation = Quaternion.Inverse(transform.rotation);
            Color color = glowColor;
            color.a = Mathf.Lerp(0.30f, 0.62f, 0.5f + Mathf.Sin(Time.unscaledTime * 14f) * 0.5f);
            glowRenderer.color = color;
        }

        private static Sprite GetGlowSprite()
        {
            if (glowSprite != null)
            {
                return glowSprite;
            }

            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = Vector2.one * (size - 1) * 0.5f;
            float radius = size * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / radius;
                    float alpha = Mathf.Pow(Mathf.Clamp01(1f - distance), 2.2f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            glowSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 64f);
            glowSprite.name = "Runtime Projectile Energy Halo";
            return glowSprite;
        }

        private static Material GetTrailMaterial()
        {
            if (trailMaterial != null)
            {
                return trailMaterial;
            }

            Shader shader = Shader.Find("Sprites/Default");
            trailMaterial = new Material(shader != null ? shader : Shader.Find("UI/Default"));
            trailMaterial.name = "Runtime Cyber Projectile Trail";
            return trailMaterial;
        }
    }
}
