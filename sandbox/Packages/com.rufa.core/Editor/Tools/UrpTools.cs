using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace Rufa.Editor
{
    /// <summary>The Quest-friendly pipeline and OpenXR settings, from the menu.</summary>
    public static class UrpTools
    {
        [MenuItem("RUFA/Strumenti/URP: preset Quest (asset selezionato)")]
        public static void QuestPresetForSelection()
        {
            var assets = Selection.GetFiltered<UniversalRenderPipelineAsset>(SelectionMode.Assets);
            if (assets.Length == 0) { Debug.LogError("RUFA: select a URP asset in the Project window (for example Mobile_RPAsset)."); return; }
            foreach (var asset in assets) ApplyQuestPreset(asset);
        }

        public static void ApplyQuestPreset(UniversalRenderPipelineAsset asset)
        {
            asset.msaaSampleCount = 4;                   // nearly free on a tile-based GPU, essential in VR
            asset.supportsHDR = false;
            asset.renderScale = 1f;
            asset.supportsCameraDepthTexture = false;
            asset.supportsCameraOpaqueTexture = false;
            asset.shadowCascadeCount = 1;
            asset.shadowDistance = 20f;
            asset.mainLightShadowmapResolution = 1024;
            asset.additionalLightsShadowmapResolution = 512;
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            Debug.Log($"RUFA: Quest preset applied to {asset.name} (MSAA 4x, no HDR, scale 1, no depth/opaque, 1 cascade, 20 m shadows).");
        }

        [MenuItem("RUFA/Strumenti/OpenXR: single pass e foveated rendering")]
        public static void ConfigureOpenXr()
        {
            FeatureHelpers.RefreshFeatures(BuildTargetGroup.Android);
            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            if (settings == null) { Debug.LogError("RUFA: OpenXR settings for Android not found. Run 'RUFA/Setup/1' first."); return; }
            settings.renderMode = OpenXRSettings.RenderMode.SinglePassInstanced;
            var foveation = settings.GetFeature<FoveatedRenderingFeature>();
            if (foveation != null) { foveation.enabled = true; EditorUtility.SetDirty(foveation); }
            else Debug.LogWarning("RUFA: Foveated Rendering feature not found in this OpenXR version.");
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            Debug.Log("RUFA: OpenXR Android set to Single Pass Instanced with Foveated Rendering enabled.");
        }
    }
}
