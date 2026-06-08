using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CyberGuardian.Editor
{
    public static class CyberGuardianSpriteImportFixer
    {
        private const string CentralCityRoot = "Assets/CyberGuardian/assets/Sidescroller Shooter - Central City";
        private const string AutoFixEditorPref = "CyberGuardian.SpriteImportFixer.CentralCity.v1";

        [InitializeOnLoadMethod]
        private static void AutoFixAfterCompile()
        {
            if (EditorPrefs.GetBool(AutoFixEditorPref, false))
            {
                return;
            }

            EditorApplication.delayCall += () =>
            {
                if (EditorPrefs.GetBool(AutoFixEditorPref, false))
                {
                    return;
                }

                FixImportedSpritePacks();
                EditorPrefs.SetBool(AutoFixEditorPref, true);
            };
        }

        [MenuItem("Cyber Guardian/Perbaiki Import Sprite Asset Pack")]
        public static void FixImportedSpritePacks()
        {
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { CentralCityRoot });
            int fixedCount = 0;

            for (int i = 0; i < textureGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(textureGuids[i]);
                if (string.IsNullOrEmpty(path) || !path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (ConfigureTexture(path))
                {
                    fixedCount++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("Import sprite asset pack diperbaiki: " + fixedCount + " texture PNG diproses.");
        }

        private static bool ConfigureTexture(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                return false;
            }

            bool shouldSlice = ShouldSlice(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = shouldSlice ? SpriteImportMode.Multiple : SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 16f;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.isReadable = shouldSlice;
            importer.SaveAndReimport();

            if (!shouldSlice)
            {
                return true;
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null)
            {
                return true;
            }

            Rect[] rects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles(texture, 4, 0);
            Array.Sort(rects, CompareSpriteRects);

            if (rects.Length == 0)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.isReadable = false;
                importer.SaveAndReimport();
                return true;
            }

#pragma warning disable 0618
            SpriteMetaData[] sprites = new SpriteMetaData[rects.Length];
            string baseName = Path.GetFileNameWithoutExtension(path).Replace(' ', '_');
            for (int i = 0; i < rects.Length; i++)
            {
                sprites[i] = new SpriteMetaData
                {
                    name = baseName + "_" + i.ToString("00"),
                    rect = rects[i],
                    alignment = (int)SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f),
                    border = Vector4.zero
                };
            }

            importer.spritesheet = sprites;
#pragma warning restore 0618
            importer.isReadable = false;
            importer.SaveAndReimport();
            return true;
        }

        private static bool ShouldSlice(string path)
        {
            string normalized = path.Replace('\\', '/');
            if (normalized.Contains("/Background/"))
            {
                return normalized.EndsWith("Background Props.png", StringComparison.OrdinalIgnoreCase);
            }

            if (normalized.Contains("/Assets/"))
            {
                return true;
            }

            if (normalized.Contains("/Social/"))
            {
                return !normalized.EndsWith("MockUp-01.png", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static int CompareSpriteRects(Rect a, Rect b)
        {
            int y = b.y.CompareTo(a.y);
            return y != 0 ? y : a.x.CompareTo(b.x);
        }
    }
}
