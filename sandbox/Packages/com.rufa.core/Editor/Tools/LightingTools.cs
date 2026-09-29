using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rufa.Editor
{
    /// <summary>Baked lighting from the menu: a mobile preset, probe grids, reflection probes, bake and clear.</summary>
    public static class LightingTools
    {
        public const string SandboxSettingsPath = "Assets/RUFA/Settings/Lighting.lighting";

        [MenuItem("RUFA/Strumenti/Lighting: preset mobile")]
        public static void MobilePresetForScene()
        {
            if (!Lightmapping.TryGetLightingSettings(out var settings) || string.IsNullOrEmpty(AssetDatabase.GetAssetPath(settings)))
            {
                BuildingBlocks.EnsureFolder("Assets/RUFA/Settings");
                settings = new LightingSettings();
                AssetDatabase.CreateAsset(settings, SandboxSettingsPath);
                Lightmapping.lightingSettings = settings;
                Debug.Log($"RUFA: lighting settings asset created at {SandboxSettingsPath}.");
            }
            ApplyMobilePreset(settings);
            Debug.Log("RUFA: mobile lighting preset applied (baked GI, GPU lightmapper, 10 texels/unit, 2048, subtractive).");
        }

        public static void ApplyMobilePreset(LightingSettings settings)
        {
            settings.bakedGI = true;
            settings.realtimeGI = false;
            settings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            settings.lightmapResolution = 10f;
            settings.lightmapMaxSize = 2048;
            settings.directionalityMode = LightmapsMode.NonDirectional;
            settings.mixedBakeMode = MixedLightingMode.Subtractive;
            settings.lightmapCompression = LightmapCompression.NormalQuality;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("RUFA/Strumenti/Light Probe Group a griglia (selezione)")]
        public static void ProbeGridForSelection()
        {
            foreach (var go in Selection.gameObjects)
            {
                var group = new GameObject("Light Probe Group").AddComponent<LightProbeGroup>();
                group.transform.SetParent(go.transform, false);
                var positions = ProbeGrid(BoundsOf(go));
                for (int i = 0; i < positions.Length; i++) positions[i] = group.transform.InverseTransformPoint(positions[i]);
                group.probePositions = positions;
                EditorSceneManager.MarkSceneDirty(go.scene);
                Debug.Log($"RUFA: {positions.Length} light probes under {go.name}.");
            }
        }

        /// <summary>
        /// 3 x 2 x 3 probes inside the bounds, 0.5 m in from the sides, at 0.6 m and 1.8 m above the floor.
        /// A probe inside a piece of furniture records darkness and turns black whatever samples it, so those positions are skipped
        /// (objects that enclose the whole area, such as a house modelled as one mesh, do not count).
        /// </summary>
        public static Vector3[] ProbeGrid(Bounds bounds)
        {
            const float inset = 0.5f;
            float[] heights = { 0.6f, 1.8f };
            var positions = new List<Vector3>();
            for (int x = 0; x < 3; x++)
                for (int z = 0; z < 3; z++)
                    foreach (var h in heights)
                        positions.Add(new Vector3(
                            Mathf.Lerp(bounds.min.x + inset, bounds.max.x - inset, x / 2f),
                            Mathf.Min(bounds.min.y + h, bounds.max.y - 0.2f),
                            Mathf.Lerp(bounds.min.z + inset, bounds.max.z - inset, z / 2f)));
            var solids = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Select(r => r.bounds)
                .Where(b => !(b.Contains(bounds.min) && b.Contains(bounds.max))).ToArray();
            return positions.Where(p => !solids.Any(b => b.Contains(p))).ToArray();
        }

        [MenuItem("RUFA/Strumenti/Reflection Probe (selezione)")]
        public static void ReflectionProbeForSelection()
        {
            foreach (var go in Selection.gameObjects)
            {
                AddReflectionProbe(go.transform, $"Reflection Probe {go.name}", BoundsOf(go));
                EditorSceneManager.MarkSceneDirty(go.scene);
            }
        }

        public static ReflectionProbe AddReflectionProbe(Transform parent, string name, Bounds bounds)
        {
            var probe = new GameObject(name).AddComponent<ReflectionProbe>();
            probe.transform.SetParent(parent, false);
            probe.transform.position = bounds.center;
            probe.mode = ReflectionProbeMode.Baked;
            probe.size = bounds.size;
            probe.resolution = 128;
            probe.boxProjection = true;
            return probe;
        }

        [MenuItem("RUFA/Strumenti/Lighting: bake")]
        public static void Bake()
        {
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("RUFA: bake started (progress bar bottom right). Keep working on probes and settings while it cooks.");
            Lightmapping.BakeAsync();
        }

        [MenuItem("RUFA/Strumenti/Lighting: cancella")]
        public static void Clear()
        {
            Lightmapping.Clear();
            Lightmapping.ClearLightingDataAsset();
            Debug.Log("RUFA: baked lighting cleared.");
        }

        public static Bounds BoundsOf(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                var b = renderers[0].bounds;
                foreach (var r in renderers) b.Encapsulate(r.bounds);
                return b;
            }
            var collider = go.GetComponentInChildren<Collider>();
            if (collider != null) return collider.bounds;
            Debug.LogWarning($"RUFA: {go.name} has no renderer or collider; using a 1 m cube around it.");
            return new Bounds(go.transform.position, Vector3.one);
        }
    }
}
