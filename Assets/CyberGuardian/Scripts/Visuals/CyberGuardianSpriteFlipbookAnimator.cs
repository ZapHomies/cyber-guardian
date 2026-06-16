using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianSpriteFlipbookAnimator : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite[] frames;
        public float framesPerSecond = 8f;
        public bool randomStart = true;

        private float elapsed;

        private void OnEnable()
        {
            elapsed = randomStart && frames != null && frames.Length > 0
                ? Random.Range(0f, frames.Length / Mathf.Max(1f, framesPerSecond))
                : 0f;

            ApplyFrame();
        }

        private void Update()
        {
            if (frames == null || frames.Length == 0 || spriteRenderer == null)
            {
                return;
            }

            elapsed += Time.deltaTime;
            ApplyFrame();
        }

        private void ApplyFrame()
        {
            if (frames == null || frames.Length == 0 || spriteRenderer == null)
            {
                return;
            }

            int index = Mathf.FloorToInt(elapsed * Mathf.Max(1f, framesPerSecond)) % frames.Length;
            spriteRenderer.sprite = frames[index];
        }
    }
}
