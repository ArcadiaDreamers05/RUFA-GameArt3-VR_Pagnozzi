using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>
    /// Packs the base colour maps of several materials into one atlas texture and one Simple Lit material.
    /// Sources are copied to RGBA32 through the GPU, so their import format does not matter.
    /// </summary>
    public static class AtlasTools
    {
        const int AtlasSize = 4096;
        const int Padding = 4;

        [MenuItem("RUFA/Strumenti/Atlas dai materiali selezionati")]
        public static void BuildSelected()
        {
            var materials = Selection.GetFiltered<Material>(SelectionMode.Assets);
            if (materials.Length < 2) { Debug.LogError("RUFA: select at least two materials in the Project window."); return; }
            var (atlas, rects) = Build(materials, "Assets/RUFA/Atlas", "Atlas");
            for (int i = 0; i < materials.Length; i++)
                Debug.Log($"RUFA: {materials[i].name} -> x {rects[i].x:F3} y {rects[i].y:F3} w {rects[i].width:F3} h {rects[i].height:F3}");
            Debug.Log($"RUFA: atlas material {AssetDatabase.GetAssetPath(atlas)}. Remap your meshes' UVs into these rectangles.");
        }

        public static (Material atlas, Rect[] rects) Build(Material[] sources, string folder, string name)
        {
            BuildingBlocks.EnsureFolder(folder);
            var copies = new Texture2D[sources.Length];
            for (int i = 0; i < sources.Length; i++)
            {
                var map = sources[i].HasProperty("_BaseMap") ? sources[i].GetTexture("_BaseMap") as Texture2D : null;
                copies[i] = map != null ? Uncompressed(map) : Solid(sources[i].HasProperty("_BaseColor") ? sources[i].GetColor("_BaseColor") : Color.gray);
            }

            var packed = new Texture2D(AtlasSize, AtlasSize, TextureFormat.RGBA32, false);
            var rects = packed.PackTextures(copies, Padding, AtlasSize);
            for (int i = 0; i < sources.Length; i++)
                if (rects[i].width * AtlasSize < copies[i].width - 1)
                    Debug.LogWarning($"RUFA: {sources[i].name} was shrunk to fit the atlas ({copies[i].width} -> {rects[i].width * AtlasSize:F0} px).");

            var pngPath = $"{folder}/{name}.png";
            File.WriteAllBytes(pngPath, packed.EncodeToPNG());
            AssetDatabase.ImportAsset(pngPath);

            var material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit")) { enableInstancing = true };
            material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(pngPath));
            AssetDatabase.CreateAsset(material, $"{folder}/{name}.mat");
            AssetDatabase.SaveAssets();
            return (material, rects);
        }

        static Texture2D Uncompressed(Texture2D source)
        {
            var rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Graphics.Blit(source, rt);
            var previous = RenderTexture.active;
            RenderTexture.active = rt;
            var copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            copy.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            copy.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
            return copy;
        }

        static Texture2D Solid(Color color)
        {
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            var pixels = new Color[64 * 64];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
