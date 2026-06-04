using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianPulseVisual : MonoBehaviour
    {
        public float scaleAmplitude = 0.04f;
        public float alphaAmplitude = 0.18f;
        public float speed = 2.4f;
        public float phase;

        private Vector3 baseScale;
        private SpriteRenderer spriteRenderer;
        private Color baseColor;

        private void Awake()
        {
            baseScale = transform.localScale;
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                baseColor = spriteRenderer.color;
            }
        }

        private void Update()
        {
            float wave = Mathf.Sin(Time.time * speed + phase);
            transform.localScale = baseScale * (1f + wave * scaleAmplitude);
            if (spriteRenderer != null)
            {
                Color color = baseColor;
                color.a = Mathf.Clamp01(baseColor.a + wave * alphaAmplitude);
                spriteRenderer.color = color;
            }
        }
    }
}
