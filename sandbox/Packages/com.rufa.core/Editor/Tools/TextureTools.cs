using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>Texture audit and mobile import preset (ASTC 6x6, mipmaps, capped size) from the menu.</summary>
    public static class TextureTools
    {
        [MenuItem("RUFA/Strumenti/Texture: audit (selezione)")]
        public static void Audit()
        {
            var table = new StringBuilder("RUFA texture audit\nname | size | android format | mipmaps | est. MB\n");
            long total = 0;
            int count = 0;
            foreach (var importer in Importers(Selection.objects))
            {
                long bytes = Estimate(importer);
                total += bytes;
                count++;
                table.AppendLine(Describe(importer));
            }
            table.AppendLine($"TOTAL: {count} textures, {total / (1024f * 1024f):F1} MB estimated on device");
            Debug.Log(table.ToString());
        }

        [MenuItem("RUFA/Strumenti/Texture: preset mobile 2048 (selezione)")]
        public static void Preset2048() => ApplyToSelection(2048);

        [MenuItem("RUFA/Strumenti/Texture: preset mobile 1024 (selezione)")]
        public static void Preset1024() => ApplyToSelection(1024);

        static void ApplyToSelection(int maxSize)
        {
            int changed = 0, total = 0;
            foreach (var importer in Importers(Selection.objects)) { total++; if (ApplyMobilePreset(importer, maxSize)) changed++; }
            Debug.Log($"RUFA: mobile preset ({maxSize}) applied to {changed} of {total} textures (the rest were already set).");
        }

        /// <summary>Textures in the selection, or every texture under selected folders.</summary>
        public static IEnumerable<TextureImporter> Importers(Object[] selection)
        {
            foreach (var obj in selection)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(path))
                {
                    foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { path }))
                        if (AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) is TextureImporter inFolder) yield return inFolder;
                }
                else if (AssetImporter.GetAtPath(path) is TextureImporter importer) yield return importer;
            }
        }

        public static bool ApplyMobilePreset(TextureImporter importer, int maxSize)
        {
            var android = importer.GetPlatformTextureSettings("Android");
            bool done = importer.mipmapEnabled && importer.textureCompression == TextureImporterCompression.Compressed && !importer.crunchedCompression
                        && importer.maxTextureSize == maxSize && android.overridden && android.format == TextureImporterFormat.ASTC_6x6 && android.maxTextureSize == maxSize;
            if (done) return false;
            importer.mipmapEnabled = true;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.crunchedCompression = false;
            importer.maxTextureSize = maxSize;
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
            {
                name = "Android", overridden = true, format = TextureImporterFormat.ASTC_6x6, maxTextureSize = maxSize, compressionQuality = 50,
            });
            importer.SaveAndReimport(); // textureType (Default / Normal map) is left as it is
            return true;
        }

        /// <summary>Bytes on the device, from the Android import settings (not from what the editor currently shows).</summary>
        public static long Estimate(TextureImporter importer)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
            if (texture == null) return 0;
            var android = importer.GetPlatformTextureSettings("Android");
            int cap = android.overridden ? android.maxTextureSize : importer.maxTextureSize;
            int width = Mathf.Min(texture.width, cap), height = Mathf.Min(texture.height, cap);
            float bytesPerPixel = android.overridden ? BytesPerPixel(android.format)
                : importer.textureCompression == TextureImporterCompression.Uncompressed ? 4f : 1f;
            double bytes = (double)width * height * bytesPerPixel;
            if (importer.mipmapEnabled) bytes *= 1.333;
            return (long)bytes;
        }

        public static string Describe(TextureImporter importer)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
            var android = importer.GetPlatformTextureSettings("Android");
            string format = android.overridden ? android.format.ToString() : $"(default: {(importer.textureCompression == TextureImporterCompression.Uncompressed ? "uncompressed" : "compressed")})";
            return $"{System.IO.Path.GetFileName(importer.assetPath)} | {texture.width}x{texture.height} | {format} | {(importer.mipmapEnabled ? "yes" : "no")} | {Estimate(importer) / (1024f * 1024f):F1}";
        }

        static float BytesPerPixel(TextureImporterFormat format)
        {
            switch (format)
            {
                case TextureImporterFormat.ASTC_4x4: return 1f;
                case TextureImporterFormat.ASTC_5x5: return 0.64f;
                case TextureImporterFormat.ASTC_6x6: return 0.444f;
                case TextureImporterFormat.ASTC_8x8: return 0.25f;
                case TextureImporterFormat.ASTC_10x10: return 0.16f;
                case TextureImporterFormat.ASTC_12x12: return 0.111f;
                case TextureImporterFormat.RGBA32: return 4f;
                case TextureImporterFormat.RGB24: return 3f;
                case TextureImporterFormat.ETC2_RGBA8: return 1f;
                case TextureImporterFormat.ETC2_RGB4:
                case TextureImporterFormat.ETC_RGB4: return 0.5f;
                default: return 1f;
            }
        }
    }
}
