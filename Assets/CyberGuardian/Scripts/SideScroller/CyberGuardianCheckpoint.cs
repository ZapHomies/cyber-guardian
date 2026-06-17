using System.IO;
using UnityEngine;

namespace CyberGuardian
{
    public sealed class CyberGuardianCheckpoint : MonoBehaviour
    {
        public CyberGuardianSideScrollerGame game;
        public Transform recoveryPoint;

        private static Sprite cachedCheckpointSprite;

        private void Awake()
        {
            ApplyPixelCheckpointSkin();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (game != null && other.GetComponent<CyberGuardianPlayerController>() != null)
            {
                Vector3 point = recoveryPoint != null ? recoveryPoint.position : transform.position;
                game.SetRecoveryPoint(point);
            }
        }

        private void ApplyPixelCheckpointSkin()
        {
#if UNITY_EDITOR
            Sprite sprite = GetSlicedCheckpointButtonSprite();
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (sprite == null || renderer == null)
            {
                return;
            }

            renderer.sprite = sprite;
            renderer.color = Color.white;
            Vector2 spriteSize = sprite.bounds.size;
            if (spriteSize.x > 0f && spriteSize.y > 0f)
            {
                transform.localScale = new Vector3(1.36f / spriteSize.x, 0.62f / spriteSize.y, 1f);
            }
#endif
        }

#if UNITY_EDITOR
        private static Sprite GetSlicedCheckpointButtonSprite()
        {
            if (cachedCheckpointSprite != null)
            {
                return cachedCheckpointSprite;
            }

            const string assetPath = "Assets/CyberGuardian/assets/new/Pixel UI pack 3/00.png";
            string absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
            if (!File.Exists(absolutePath))
            {
                return null;
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(absolutePath)))
            {
                Destroy(texture);
                return null;
            }

            texture.filterMode = FilterMode.Point;
            const int sliceX = 0;
            const int sliceYFromTop = 85;
            const int sliceWidth = 48;
            const int sliceHeight = 22;
            int unityY = texture.height - sliceYFromTop - sliceHeight;
            if (unityY < 0 || sliceX + sliceWidth > texture.width || unityY + sliceHeight > texture.height)
            {
                Destroy(texture);
                return null;
            }

            cachedCheckpointSprite = Sprite.Create(
                texture,
                new Rect(sliceX, unityY, sliceWidth, sliceHeight),
                new Vector2(0.5f, 0.5f),
                100f);
            cachedCheckpointSprite.name = "runtime_checkpoint_blue_button";
            return cachedCheckpointSprite;
        }
#endif
    }
}
