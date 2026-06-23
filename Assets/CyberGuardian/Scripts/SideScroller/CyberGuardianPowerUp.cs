using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CyberGuardian
{
    public enum CyberGuardianPowerUpType
    {
        Health,
        Boost,
        Firewall,
        Overclock
    }

    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianPowerUp : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public CyberGuardianPowerUpType type = CyberGuardianPowerUpType.Boost;
        public int amount = 25;
        public Sprite[] animationFrames;
        public Vector2 visualSize = new Vector2(0.54f, 0.54f);
        public float framesPerSecond = 8f;

        private const float PickupWorldRadius = 0.56f;
        private static readonly Vector2 HaloVisualSize = new Vector2(1.22f, 1.22f);

        private bool collected;

        private void Awake()
        {
            ApplyPowerUpSkin();
        }

        private void Reset()
        {
            Collider2D powerCollider = GetComponent<Collider2D>();
            powerCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (collected || other == null || other.GetComponent<CyberGuardianPlayerController>() == null)
            {
                return;
            }

            collected = true;
            if (game != null)
            {
                game.ApplyPowerUp(this);
            }

            gameObject.SetActive(false);
        }

        private void ApplyPowerUpSkin()
        {
#if UNITY_EDITOR
            if (animationFrames == null || animationFrames.Length == 0)
            {
                animationFrames = LoadRuntimePowerUpFrames(type);
            }
#endif

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer == null || animationFrames == null || animationFrames.Length == 0 || animationFrames[0] == null)
            {
                return;
            }

            visualSize = GetVisualSize(type);
            renderer.sprite = animationFrames[0];
            renderer.color = Color.white;
            ScaleRenderer(renderer, visualSize);
            NormalizePickupCollider();
            ScaleChildSprite("Power Up Halo", HaloVisualSize);
            DisableLegacyCoreSprite();

            CyberGuardianSpriteFlipbookAnimator flipbook = GetComponent<CyberGuardianSpriteFlipbookAnimator>();
            if (animationFrames.Length > 1)
            {
                if (flipbook == null)
                {
                    flipbook = gameObject.AddComponent<CyberGuardianSpriteFlipbookAnimator>();
                }

                flipbook.spriteRenderer = renderer;
                flipbook.frames = animationFrames;
                flipbook.framesPerSecond = framesPerSecond;
                flipbook.randomStart = true;
            }
            else if (flipbook != null)
            {
                flipbook.enabled = false;
            }
        }

        private void DisableLegacyCoreSprite()
        {
            Transform core = transform.Find("Power Up Core");
            if (core == null)
            {
                return;
            }

            SpriteRenderer renderer = core.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        private void NormalizePickupCollider()
        {
            CircleCollider2D circle = GetComponent<CircleCollider2D>();
            if (circle != null)
            {
                circle.isTrigger = true;
                circle.offset = Vector2.zero;
                float scale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y), 0.01f);
                circle.radius = PickupWorldRadius / scale;
            }
        }

        private void ScaleChildSprite(string childName, Vector2 size)
        {
            Transform child = transform.Find(childName);
            if (child == null)
            {
                return;
            }

            SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                return;
            }

            ScaleRenderer(renderer, size, true);
        }

        private static Vector2 GetVisualSize(CyberGuardianPowerUpType type)
        {
            switch (type)
            {
                case CyberGuardianPowerUpType.Boost:
                    return new Vector2(0.82f, 1.16f);
                case CyberGuardianPowerUpType.Health:
                    return new Vector2(0.92f, 0.92f);
                default:
                    return new Vector2(0.96f, 0.96f);
            }
        }

        private static void ScaleRenderer(SpriteRenderer renderer, Vector2 size, bool compensateParentScale = false)
        {
            if (renderer == null || renderer.sprite == null)
            {
                return;
            }

            Vector2 spriteSize = renderer.sprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            {
                return;
            }

            float scaleX = size.x / spriteSize.x;
            float scaleY = size.y / spriteSize.y;
            if (compensateParentScale && renderer.transform.parent != null)
            {
                Vector3 parentScale = renderer.transform.parent.lossyScale;
                scaleX /= Mathf.Max(Mathf.Abs(parentScale.x), 0.01f);
                scaleY /= Mathf.Max(Mathf.Abs(parentScale.y), 0.01f);
            }

            renderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            CyberGuardianPulseVisual pulse = renderer.GetComponent<CyberGuardianPulseVisual>();
            if (pulse != null)
            {
                pulse.SetBaseScale(renderer.transform.localScale);
            }
        }

#if UNITY_EDITOR
        private static Sprite[] LoadRuntimePowerUpFrames(CyberGuardianPowerUpType type)
        {
            const string assetPath = "Assets/CyberGuardian/assets/new/coins-chests-etc-2-0-noborders.png";
            string absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
            if (!File.Exists(absolutePath))
            {
                return new Sprite[0];
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(absolutePath)))
            {
                return new Sprite[0];
            }

            texture.filterMode = FilterMode.Point;
            RectInt[] rects = GetPowerUpSheetRects(type);
            List<Sprite> frames = new List<Sprite>(rects.Length);
            for (int i = 0; i < rects.Length; i++)
            {
                RectInt topLeftRect = rects[i];
                int y = texture.height - topLeftRect.y - topLeftRect.height;
                if (topLeftRect.x < 0 || y < 0 || topLeftRect.x + topLeftRect.width > texture.width || y + topLeftRect.height > texture.height)
                {
                    continue;
                }

                Sprite sprite = Sprite.Create(texture, new Rect(topLeftRect.x, y, topLeftRect.width, topLeftRect.height), new Vector2(0.5f, 0.5f), 48f);
                sprite.name = "runtime_powerup_" + type.ToString().ToLowerInvariant() + "_" + frames.Count.ToString("00");
                frames.Add(sprite);
            }

            return frames.ToArray();
        }

        private static RectInt[] GetPowerUpSheetRects(CyberGuardianPowerUpType type)
        {
            switch (type)
            {
                case CyberGuardianPowerUpType.Health:
                    return new[]
                    {
                        new RectInt(322, 197, 14, 14),
                        new RectInt(338, 197, 14, 14),
                        new RectInt(354, 197, 14, 14),
                        new RectInt(322, 197, 14, 14)
                    };
                case CyberGuardianPowerUpType.Firewall:
                    return new[]
                    {
                        new RectInt(18, 820, 16, 16),
                        new RectInt(34, 820, 16, 16),
                        new RectInt(18, 820, 16, 16),
                        new RectInt(34, 820, 16, 16)
                    };
                case CyberGuardianPowerUpType.Overclock:
                    return new[]
                    {
                        new RectInt(18, 196, 18, 18),
                        new RectInt(82, 196, 18, 18),
                        new RectInt(100, 196, 18, 18),
                        new RectInt(118, 196, 18, 18)
                    };
                default:
                    return new[]
                    {
                        new RectInt(674, 208, 20, 32),
                        new RectInt(674, 246, 20, 32),
                        new RectInt(674, 208, 20, 32),
                        new RectInt(674, 246, 20, 32)
                    };
            }
        }
#endif
    }
}
