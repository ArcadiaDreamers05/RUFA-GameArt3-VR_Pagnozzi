using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>Caps the expensive knobs of the selected Particle Systems to values that are safe on Quest.</summary>
    public static class ParticleTools
    {
        [MenuItem("RUFA/Strumenti/Particle System: preset mobile (selezione)")]
        public static void MobilePresetForSelection()
        {
            int changed = 0;
            foreach (var go in Selection.gameObjects)
                foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
                {
                    bool touched = false;
                    var main = ps.main;
                    if (main.maxParticles > 100) { main.maxParticles = 100; touched = true; }
                    if (main.startSize.constantMax > 0.3f) { main.startSize = new ParticleSystem.MinMaxCurve(Mathf.Min(0.3f, main.startSize.constantMin), 0.3f); touched = true; }
                    if (main.cullingMode != ParticleSystemCullingMode.Automatic) { main.cullingMode = ParticleSystemCullingMode.Automatic; touched = true; }
                    var lights = ps.lights;
                    if (lights.enabled) { lights.enabled = false; touched = true; }
                    var trails = ps.trails;
                    if (trails.enabled) { trails.enabled = false; touched = true; }
                    var renderer = ps.GetComponent<ParticleSystemRenderer>();
                    if (renderer.renderMode == ParticleSystemRenderMode.Mesh && !renderer.enableGPUInstancing) { renderer.enableGPUInstancing = true; touched = true; }
                    if (touched) { changed++; EditorUtility.SetDirty(ps); EditorSceneManager.MarkSceneDirty(ps.gameObject.scene); }
                }
            Debug.Log(changed > 0 ? $"RUFA: mobile preset applied to {changed} particle systems (max 100 particles, size <= 0.3 m, automatic culling, no lights, no trails)." : "RUFA: nothing to change.");
        }
    }
}
