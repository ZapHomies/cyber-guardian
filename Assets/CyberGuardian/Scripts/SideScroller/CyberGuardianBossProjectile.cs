using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianBossProjectile : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public Vector2 velocity = new Vector2(-8f, 0f);
        public int damage = 12;
        public float lifetime = 4f;
        public string damageSource = "Boss packet attack";
        public Sprite[] animationFrames;
        public float framesPerSecond = 14f;
        public Vector2 visualSize = new Vector2(0.34f, 0.34f);

        private SpriteRenderer spriteRenderer;
        private float animationElapsed;
#if UNITY_EDITOR
        private static Sprite[] runtimeSplatterFrames;
#endif

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            EnsureAnimationFramesLoaded();
            ApplyVisualSize();
            CyberGuardianSpriteFlipbookAnimator[] flipbooks = GetComponents<CyberGuardianSpriteFlipbookAnimator>();
            for (int i = 0; i < flipbooks.Length; i++)
            {
                if (flipbooks[i] != null)
                {
                    flipbooks[i].enabled = false;
                }
            }
        }

        private void Update()
        {
            transform.position += (Vector3)(velocity * Time.deltaTime);
            if (velocity.sqrMagnitude > 0.01f)
            {
                transform.right = velocity.normalized;
            }

            AnimateProjectile();
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<CyberGuardianBossShieldBlock>() != null)
            {
                Destroy(gameObject);
            }
            else if (game != null && other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                game.DamagePlayer(damage, damageSource);
                Destroy(gameObject);
            }
            else if (!other.isTrigger && other.GetComponent<CyberGuardianBossShieldBlock>() == null)
            {
                Destroy(gameObject);
            }
        }

        private void AnimateProjectile()
        {
            if (spriteRenderer == null || animationFrames == null || animationFrames.Length == 0)
            {
                return;
            }

            animationElapsed += Time.deltaTime;
            int index = Mathf.FloorToInt(animationElapsed * Mathf.Max(1f, framesPerSecond)) % animationFrames.Length;
            spriteRenderer.sprite = animationFrames[index];
            ApplyVisualSize();
        }

        private void EnsureAnimationFramesLoaded()
        {
            if (animationFrames != null && animationFrames.Length > 1)
            {
                return;
            }

#if UNITY_EDITOR
            if (runtimeSplatterFrames == null || runtimeSplatterFrames.Length == 0)
            {
                string folder = "Assets/CyberGuardian/assets/new/Super Pixel Effects Mini Pack 1/Super Pixel Effects Mini Pack 1/spritesheet/fx1_splatter_small_red";
                runtimeSplatterFrames = LoadRuntimeMetadataEffectSheet(folder, 48f);
            }

            Sprite[] sprites = runtimeSplatterFrames != null ? runtimeSplatterFrames : new Sprite[0];
            if (sprites.Length > 0)
            {
                animationFrames = sprites;
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = sprites[0];
                    spriteRenderer.color = new Color(1f, 0.18f, 0.34f, 0.96f);
                }
            }
#endif
        }

        private void ApplyVisualSize()
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null)
            {
                return;
            }

            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            {
                return;
            }

            transform.localScale = new Vector3(visualSize.x / spriteSize.x, visualSize.y / spriteSize.y, 1f);
        }

#if UNITY_EDITOR
        private static Sprite[] LoadRuntimeMetadataEffectSheet(string folderAssetPath, float pixelsPerUnit)
        {
            string absoluteFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "..", folderAssetPath));
            string imagePath = Path.Combine(absoluteFolder, "spritesheet.png");
            string metadataPath = Path.Combine(absoluteFolder, "spritesheet.txt");
            if (!File.Exists(imagePath) || !File.Exists(metadataPath))
            {
                return new Sprite[0];
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(imagePath)))
            {
                return new Sprite[0];
            }

            texture.filterMode = FilterMode.Point;
            List<Sprite> frames = new List<Sprite>();
            string[] lines = File.ReadAllLines(metadataPath);
            string sourceName = Path.GetFileName(folderAssetPath.TrimEnd('/', '\\'));
            for (int i = 0; i < lines.Length; i++)
            {
                if (!TryParseSpriteSheetMetadataLine(lines[i], out int x, out int y, out int width, out int height))
                {
                    continue;
                }

                int unityY = texture.height - y - height;
                if (!IsValidSpriteRect(texture, x, unityY, width, height))
                {
                    continue;
                }

                Rect rect = new Rect(x, unityY, width, height);
                Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.name = sourceName + "_runtime_frame_" + frames.Count.ToString("00");
                frames.Add(sprite);
            }

            if (frames.Count == 0)
            {
                AddInferredHorizontalFrames(texture, sourceName, lines.Length, pixelsPerUnit, frames);
            }

            return frames.ToArray();
        }

        private static bool IsValidSpriteRect(Texture2D texture, int x, int y, int width, int height)
        {
            return texture != null
                && x >= 0
                && y >= 0
                && width > 0
                && height > 0
                && x + width <= texture.width
                && y + height <= texture.height;
        }

        private static void AddInferredHorizontalFrames(Texture2D texture, string sourceName, int requestedCount, float pixelsPerUnit, List<Sprite> frames)
        {
            int frameCount = Mathf.Max(1, requestedCount);
            if (texture == null || frameCount <= 0)
            {
                return;
            }

            int frameWidth = Mathf.Max(1, texture.width / frameCount);
            int frameHeight = texture.height;
            for (int i = 0; i < frameCount; i++)
            {
                int x = i * frameWidth;
                if (!IsValidSpriteRect(texture, x, 0, frameWidth, frameHeight))
                {
                    continue;
                }

                Rect rect = new Rect(x, 0, frameWidth, frameHeight);
                Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.name = sourceName + "_runtime_inferred_frame_" + frames.Count.ToString("00");
                frames.Add(sprite);
            }
        }

        private static string ToProjectAssetPath(string path)
        {
            string normalized = path.Replace('\\', '/');
            int index = normalized.IndexOf("Assets/", System.StringComparison.OrdinalIgnoreCase);
            return index >= 0 ? normalized.Substring(index) : normalized;
        }

        private static bool TryParseSpriteSheetMetadataLine(string line, out int x, out int y, out int width, out int height)
        {
            x = 0;
            y = 0;
            width = 0;
            height = 0;
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            int equalsIndex = line.IndexOf('=');
            if (equalsIndex < 0 || equalsIndex >= line.Length - 1)
            {
                return false;
            }

            string[] parts = line.Substring(equalsIndex + 1).Trim().Split(' ');
            List<int> values = new List<int>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i], out int value))
                {
                    values.Add(value);
                }
            }

            if (values.Count < 4)
            {
                return false;
            }

            x = values[0];
            y = values[1];
            width = values[2];
            height = values[3];
            return width > 0 && height > 0;
        }
#endif
    }
}
