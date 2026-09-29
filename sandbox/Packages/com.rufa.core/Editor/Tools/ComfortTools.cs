using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>Comfort and audio helpers for the sandbox: vignette + haptics objects, spatial audio settings, reverb zones.</summary>
    public static class ComfortTools
    {
        const string VignettePrefabName = "TunnelingVignette";

        [MenuItem("RUFA/Setup/6 - Aggiungi comfort (vignetta e haptics)")]
        public static void AddComfort()
        {
            var vignette = GameObject.Find("Comfort Vignette") ?? new GameObject("Comfort Vignette");
            var component = vignette.TryGetComponent<ComfortVignette>(out var found) ? found : vignette.AddComponent<ComfortVignette>(); // also an object left without its component
            component.vignettePrefab = FindVignettePrefab();
            EditorUtility.SetDirty(component);
            var haptics = GameObject.Find("Comfort Haptics") ?? new GameObject("Comfort Haptics");
            if (!haptics.TryGetComponent<ComfortHaptics>(out _)) haptics.AddComponent<ComfortHaptics>();
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log(component.vignettePrefab != null
                ? "RUFA: comfort vignette and haptics added. Save the scene."
                : "RUFA: comfort objects added but the TunnelingVignette prefab was not found: run 'RUFA/Setup/1' to import the XRI Starter Assets.");
        }

        public static GameObject FindVignettePrefab()
        {
            foreach (var guid in AssetDatabase.FindAssets($"{VignettePrefabName} t:Prefab"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == VignettePrefabName) return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            return null;
        }

        [MenuItem("RUFA/Strumenti/Audio: rendi spaziali (selezione)")]
        public static void MakeSelectionSpatial()
        {
            int count = 0;
            foreach (var go in Selection.gameObjects)
                foreach (var source in go.GetComponentsInChildren<AudioSource>(true)) { MakeSpatial(source, 1f, 15f); count++; }
            Debug.Log($"RUFA: {count} audio sources made spatial (3D, logarithmic rolloff, 1-15 m).");
        }

        public static void MakeSpatial(AudioSource source, float minDistance, float maxDistance)
        {
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.dopplerLevel = 0f;
            EditorUtility.SetDirty(source);
        }

        [MenuItem("RUFA/Strumenti/Reverb zone (selezione)")]
        public static void ReverbZoneForSelection()
        {
            foreach (var go in Selection.gameObjects)
            {
                AddReverbZone(go.transform, LightingTools.BoundsOf(go));
                EditorSceneManager.MarkSceneDirty(go.scene);
            }
        }

        public static AudioReverbZone AddReverbZone(Transform parent, Bounds bounds)
        {
            var zone = new GameObject("Reverb Zone").AddComponent<AudioReverbZone>();
            zone.transform.SetParent(parent, false);
            zone.transform.position = bounds.center;
            zone.reverbPreset = AudioReverbPreset.Stoneroom;
            zone.minDistance = Mathf.Min(bounds.extents.x, bounds.extents.z);
            zone.maxDistance = Mathf.Max(bounds.size.x, bounds.size.z);
            return zone;
        }
    }
}
