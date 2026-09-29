using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rufa.Editor
{
    /// <summary>Static flags, occlusion culling and triangle counting from the menu: the same steps for the demo and for the students' scenes.</summary>
    public static class StaticTools
    {
        public const StaticEditorFlags Flags =
            StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.BatchingStatic |
            StaticEditorFlags.ContributeGI | StaticEditorFlags.ReflectionProbeStatic;

        [MenuItem("RUFA/Strumenti/Statici: imposta flag (selezione)")]
        public static void MarkSelection() => Debug.Log($"RUFA: {MarkStatic(Selection.gameObjects)} renderers marked static.");

        public static int MarkStatic(GameObject[] roots)
        {
            int count = 0;
            foreach (var root in roots)
                foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>(true))
                {
                    // Glass and foliage do not hide what is behind them: they can be culled, but never cull anything.
                    bool seeThrough = renderer.sharedMaterials.Any(m => m != null && m.renderQueue >= (int)RenderQueue.Transparent);
                    GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, seeThrough ? Flags & ~StaticEditorFlags.OccluderStatic : Flags);
                    count++;
                }
            if (count > 0) EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            return count;
        }

        [MenuItem("RUFA/Strumenti/Occlusion culling: bake")]
        public static void BakeOcclusion()
        {
            StaticOcclusionCulling.smallestOccluder = 0.5f;
            StaticOcclusionCulling.smallestHole = 0.25f;
            StaticOcclusionCulling.backfaceThreshold = 100f;
            Debug.Log("RUFA: baking occlusion culling, this can take a minute on a full house...");
            bool ok = StaticOcclusionCulling.Compute();
            Debug.Log(ok
                ? "RUFA: occlusion data baked. Check it in Scene view: Occlusion Culling window, Visualize."
                : "RUFA: occlusion bake produced no data. Are there renderers marked Occluder Static in the scene?");
        }

        [MenuItem("RUFA/Strumenti/Occlusion culling: cancella")]
        public static void ClearOcclusion()
        {
            StaticOcclusionCulling.Clear();
            Debug.Log("RUFA: occlusion data cleared.");
        }

        [MenuItem("RUFA/Strumenti/Conta triangoli (selezione)")]
        public static void CountTriangles()
        {
            long triangles = 0;
            int meshes = 0;
            foreach (var root in Selection.gameObjects)
                foreach (var filter in root.GetComponentsInChildren<MeshFilter>(true))
                    if (filter.sharedMesh != null) { triangles += filter.sharedMesh.triangles.Length / 3; meshes++; }
            Debug.Log($"RUFA: {meshes} meshes, {triangles:N0} triangles in the selection (every LOD level counted; this is asset size, not what is drawn on screen).");
        }
    }
}
