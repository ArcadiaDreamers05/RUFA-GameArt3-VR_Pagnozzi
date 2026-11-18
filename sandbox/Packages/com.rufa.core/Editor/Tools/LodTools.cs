using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>
    /// Builds a LODGroup from decimated variants exported next to the original model:
    /// for a mesh in "tavolo.fbx", the tool looks for "tavolo_LOD1" and "tavolo_LOD2" in the same folder. A variant of
    /// another size is refused, and a second run replaces the levels of the first.
    /// </summary>
    public static class LodTools
    {
        static readonly float[] Heights = { 0.5f, 0.15f, 0.03f }; // screen-height thresholds for LOD0, LOD1, LOD2

        [MenuItem("RUFA/Strumenti/LOD Group dalle varianti selezionate")]
        public static void BuildSelected()
        {
            int built = 0;
            foreach (var go in Selection.gameObjects) if (BuildFromVariants(go)) built++;
            Debug.Log($"RUFA: {built} LOD groups built.");
        }

        public static bool BuildFromVariants(GameObject go)
        {
            var filter = go.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) { Debug.LogWarning($"RUFA: {go.name} has no mesh."); return false; }
            var path = AssetDatabase.GetAssetPath(filter.sharedMesh);
            var folder = Path.GetDirectoryName(path).Replace('\\', '/');
            var baseName = Path.GetFileNameWithoutExtension(path);
            var renderer = go.GetComponent<MeshRenderer>();

            var variants = new List<Mesh>();
            for (int level = 1; level <= 2; level++)
            {
                var mesh = FindMesh(folder, $"{baseName}_LOD{level}");
                if (mesh == null) break;
                // Exported with Blender's default scaling, a variant arrives a hundred times smaller: the model would vanish
                // from afar, so the tool stops and says which setting to change.
                float ratio = mesh.bounds.size.magnitude / filter.sharedMesh.bounds.size.magnitude;
                if (ratio < 0.8f || ratio > 1.25f)
                {
                    Debug.LogWarning($"RUFA: {baseName}_LOD{level} is {ratio:0.###} times the size of {baseName}. Export it again from Blender with Apply Scalings: FBX Units Scale.");
                    return false;
                }
                variants.Add(mesh);
            }
            if (variants.Count == 0)
            {
                Debug.LogWarning($"RUFA: no mesh named {baseName}_LOD1 next to {path}. Export the decimated variants from Blender first.");
                return false;
            }

            // A second run, after a new export, starts again: the levels the tool made before make way for the new ones
            // (those that come with a prefab stay; the prefab is the place to change them).
            foreach (var old in go.transform.Cast<Transform>().Where(t => (t.name == "LOD1" || t.name == "LOD2") && !PrefabUtility.IsPartOfPrefabInstance(t)).ToArray())
                Object.DestroyImmediate(old.gameObject);
            var levels = new List<Renderer[]> { new Renderer[] { renderer } };
            for (int i = 0; i < variants.Count; i++)
            {
                var child = new GameObject($"LOD{i + 1}", typeof(MeshFilter), typeof(MeshRenderer));
                child.transform.SetParent(go.transform, false);
                child.GetComponent<MeshFilter>().sharedMesh = variants[i];
                child.GetComponent<MeshRenderer>().sharedMaterials = renderer.sharedMaterials;
                levels.Add(new Renderer[] { child.GetComponent<MeshRenderer>() });
            }

            var group = go.GetComponent<LODGroup>();
            if (group == null) group = go.AddComponent<LODGroup>();
            var lods = new LOD[levels.Count];
            for (int i = 0; i < lods.Length; i++) lods[i] = new LOD(Heights[i], levels[i]);
            group.SetLODs(lods);
            group.RecalculateBounds();
            EditorSceneManager.MarkSceneDirty(go.scene);
            return true;
        }

        // A variant exported from Blender keeps the name of the original mesh inside its file (tavolo inside
        // tavolo_LOD1.fbx), so the file's name counts first; a mesh with the variant's name also does.
        static Mesh FindMesh(string folder, string name)
        {
            foreach (var guid in AssetDatabase.FindAssets(name, new[] { folder }))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var meshes = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Mesh>();
                var mesh = Same(Path.GetFileNameWithoutExtension(assetPath), name) ? meshes.FirstOrDefault() : meshes.FirstOrDefault(m => Same(m.name, name));
                if (mesh != null) return mesh;
            }
            return null;
        }

        // Windows does not tell tavolo_lod1.fbx from tavolo_LOD1.fbx, and neither does the search.
        static bool Same(string a, string b) => string.Equals(a, b, System.StringComparison.OrdinalIgnoreCase);
    }
}
