using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CyberGuardian
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CyberGuardianSlingshotProjectile2D : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public Sprite[] animationFrames;
        public float framesPerSecond = 18f;
        public Vector2 visualSize = new Vector2(0.34f, 0.34f);

        private SpriteRenderer spriteRenderer;
        private Collider2D ownCollider;
        private CyberGuardianProjectileVisual2D projectileVisual;
        private float animationElapsed;
        private Vector3 previousPosition;
        private bool hitResolved;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            ownCollider = GetComponent<Collider2D>();
            visualSize = new Vector2(Mathf.Max(0.34f, visualSize.x), Mathf.Max(0.34f, visualSize.y));
            EnsureAnimationFramesLoaded();
            DisableOversizedChildVfx();
            ApplyVisualSize();
            EnsureProjectileVisual();
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
            AnimateProjectile();
        }

        private void FixedUpdate()
        {
            if (game == null || !game.SlingshotProjectileInFlight || hitResolved)
            {
                previousPosition = transform.position;
                return;
            }

            ScanProjectilePath(previousPosition, transform.position);
            previousPosition = transform.position;
        }

        private void OnEnable()
        {
            animationElapsed = 0f;
            previousPosition = transform.position;
            hitResolved = false;
            ApplyVisualSize();
            DisableOversizedChildVfx();
            EnsureProjectileVisual();
        }

        public void ArmForLaunch(Vector2 launchPosition)
        {
            previousPosition = launchPosition;
            hitResolved = false;
            animationElapsed = 0f;
            if (projectileVisual != null)
            {
                projectileVisual.enabled = false;
                projectileVisual.enabled = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryHitTarget(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryHitTarget(other);
        }

        private void TryHitTarget(Collider2D other)
        {
            if (hitResolved || game == null || !game.SlingshotProjectileInFlight || other == null || other == ownCollider || other.GetComponentInParent<CyberGuardianPlayerController>() != null)
            {
                return;
            }

            CyberGuardianBossShieldBlock block = other.GetComponentInParent<CyberGuardianBossShieldBlock>();
            if (block != null)
            {
                hitResolved = true;
                game.ProjectileHitShieldBlock(block);
                return;
            }

            CyberGuardianBossCore boss = other.GetComponentInParent<CyberGuardianBossCore>();
            if (boss != null)
            {
                CyberGuardianBossShieldBlock blockingQuizBlock = FindBlockingQuizBlock(previousPosition, transform.position);
                if (blockingQuizBlock != null)
                {
                    hitResolved = true;
                    game.ProjectileHitShieldBlock(blockingQuizBlock);
                    return;
                }

                hitResolved = true;
                game.ProjectileHitBoss();
                return;
            }

            if (!other.isTrigger)
            {
                hitResolved = true;
                game.ProjectileHitSolid();
            }
        }

        private CyberGuardianBossShieldBlock FindBlockingQuizBlock(Vector3 from, Vector3 to)
        {
            Vector2 delta = to - from;
            float distance = delta.magnitude;
            if (distance <= 0.001f)
            {
                return null;
            }

            RaycastHit2D[] hits = Physics2D.CircleCastAll(from, 0.09f, delta / distance, distance);
            System.Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D candidate = hits[i].collider;
                if (candidate == null || candidate == ownCollider)
                {
                    continue;
                }

                CyberGuardianBossShieldBlock block = candidate.GetComponentInParent<CyberGuardianBossShieldBlock>();
                if (block != null && !block.cleared && block.gameObject.activeInHierarchy)
                {
                    return block;
                }
            }

            return null;
        }

        private void ScanProjectilePath(Vector3 from, Vector3 to)
        {
            Vector2 delta = to - from;
            float distance = delta.magnitude;
            if (distance <= 0.001f)
            {
                return;
            }

            RaycastHit2D[] hits = Physics2D.CircleCastAll(from, 0.09f, delta / distance, distance);
            System.Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D candidate = hits[i].collider;
                if (candidate == null || candidate == ownCollider)
                {
                    continue;
                }

                TryHitTarget(candidate);
                if (hitResolved)
                {
                    return;
                }
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
            string folder = "Assets/CyberGuardian/assets/new/Super Pixel Effects Mini Pack 1/Super Pixel Effects Mini Pack 1/spritesheet/fx2_electric_burst_large_violet";
            Sprite[] sprites = LoadRuntimeMetadataEffectSheet(folder, 72f);
            if (sprites.Length > 0)
            {
                animationFrames = sprites;
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = sprites[0];
                    spriteRenderer.color = new Color(0.92f, 0.58f, 1f, 0.96f);
                    ApplyVisualSize();
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

        private void DisableOversizedChildVfx()
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null
                    && renderers[i] != spriteRenderer
                    && renderers[i].name != "Runtime Projectile Energy Halo")
                {
                    renderers[i].enabled = false;
                }
            }
        }

        private void EnsureProjectileVisual()
        {
            projectileVisual = GetComponent<CyberGuardianProjectileVisual2D>();
            if (projectileVisual == null)
            {
                projectileVisual = gameObject.AddComponent<CyberGuardianProjectileVisual2D>();
            }

            projectileVisual.Configure(
                spriteRenderer,
                0.58f,
                0.24f,
                new Color(0.34f, 1f, 1f, 0.78f),
                new Color(1f, 0.16f, 0.72f, 0f));
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
