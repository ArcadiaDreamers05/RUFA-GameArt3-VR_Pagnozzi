using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>
    /// Builds a LODGroup from decimated variants exported next to the original model:
    /// for a mesh in "tavolo.fbx", the tool looks for "tavolo_LOD1" and "tavolo_LOD2" in the same folder.
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

            var levels = new List<Renderer[]> { new Renderer[] { renderer } };
            for (int level = 1; level <= 2; level++)
            {
                var mesh = FindMesh(folder, $"{baseName}_LOD{level}");
                if (mesh == null) break;
                var child = new GameObject($"LOD{level}", typeof(MeshFilter), typeof(MeshRenderer));
                child.transform.SetParent(go.transform, false);
                child.GetComponent<MeshFilter>().sharedMesh = mesh;
                child.GetComponent<MeshRenderer>().sharedMaterials = renderer.sharedMaterials;
                levels.Add(new Renderer[] { child.GetComponent<MeshRenderer>() });
            }
            if (levels.Count == 1)
            {
                Debug.LogWarning($"RUFA: no mesh named {baseName}_LOD1 next to {path}. Export the decimated variants from Blender first.");
                return false;
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

        static Mesh FindMesh(string folder, string name)
        {
            foreach (var guid in AssetDatabase.FindAssets($"{name} t:Mesh", new[] { folder }))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(assetPath))
                    if (asset is Mesh mesh && (mesh.name == name || Path.GetFileNameWithoutExtension(assetPath) == name))
                        return mesh;
            }
            return null;
        }
    }
}
