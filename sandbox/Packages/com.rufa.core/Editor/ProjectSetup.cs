using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager.UI;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.Meta;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

namespace Rufa.Editor
{
    /// <summary>
    /// One-shot project configuration for the dual target (Meta Quest 3S + Windows).
    /// Idempotent: running it again re-applies the same values.
    /// </summary>
    public static class ProjectSetup
    {
        public const string AndroidIdentifier = "it.rufa.gameart3.sandbox";
        public const string ProductName = "RUFA Sandbox";
        public const string CompanyName = "RUFA";

        const string OpenXRLoaderType = "UnityEngine.XR.OpenXR.OpenXRLoader";
        const string XrSettingsFolder = "Assets/XR";
        const string XrSettingsAssetPath = "Assets/XR/XRGeneralSettingsPerBuildTarget.asset";
        const string XriPackage = "com.unity.xr.interaction.toolkit";
        const string XriSample = "Starter Assets";

        [MenuItem("RUFA/Setup/1 - Configura progetto (Player, XR, URP)")]
        public static void Configure()
        {
            ConfigurePlayer();
            ConfigureXr(BuildTargetGroup.Android);
            ConfigureXr(BuildTargetGroup.Standalone);
            ConfigureUrpMobileAsset();
            AssetDatabase.SaveAssets();
            Debug.Log("RUFA: project configured.");

            // Last on purpose: importing the sample compiles its scripts and reloads the domain.
            ImportXriStarterAssets();
        }

        static void ConfigurePlayer()
        {
            PlayerSettings.productName = ProductName;
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft; // Meta Quest validation rule
            PlayerSettings.enableFrameTimingStats = true;                             // CPU/GPU times for the benchmark

            var android = NamedBuildTarget.Android;
            PlayerSettings.SetApplicationIdentifier(android, AndroidIdentifier);
            PlayerSettings.SetScriptingBackend(android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel32;    // Meta minimum
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34; // Meta requirement for new apps
            PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.GameActivity;  // Meta Quest validation rule on Unity 6
            PlayerSettings.Android.textureCompressionFormats = new[] { TextureCompressionFormat.ASTC };
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });

            EnsureInputSystemActive();
        }

        // XRI reads input through the Input System package. 0 = Input Manager (old), 1 = Input System, 2 = both.
        static void EnsureInputSystemActive()
        {
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var handler = settings.FindProperty("activeInputHandler");
            if (handler.intValue != 0) return;

            handler.intValue = 2;
            settings.ApplyModifiedProperties();
            Debug.LogWarning("RUFA: Active Input Handling was 'Input Manager (Old)', now 'Both'. Restart the editor when Unity asks for it.");
        }

        /// <summary>XR Plug-in Management settings for one build target group, created on first use.</summary>
        internal static XRGeneralSettings XrSettings(BuildTargetGroup group)
        {
            if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.settingsKey, out XRGeneralSettingsPerBuildTarget perTarget) || perTarget == null)
            {
                perTarget = AssetDatabase.LoadAssetAtPath<XRGeneralSettingsPerBuildTarget>(XrSettingsAssetPath);
                if (perTarget == null)
                {
                    if (!AssetDatabase.IsValidFolder(XrSettingsFolder))
                        AssetDatabase.CreateFolder("Assets", "XR");
                    perTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                    AssetDatabase.CreateAsset(perTarget, XrSettingsAssetPath);
                }
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.settingsKey, perTarget, true);
            }

            if (!perTarget.HasManagerSettingsForBuildTarget(group))
                perTarget.CreateDefaultManagerSettingsForBuildTarget(group);

            return perTarget.SettingsForBuildTarget(group);
        }

        static void ConfigureXr(BuildTargetGroup group)
        {
            var settings = XrSettings(group);
            settings.InitManagerOnStart = group == BuildTargetGroup.Android; // Quest starts in VR; PC and Play Mode start desktop
            if (!XRPackageMetadataStore.AssignLoader(settings.Manager, OpenXRLoaderType, group))
                Debug.LogError($"RUFA: could not assign the OpenXR loader for {group}. Is the OpenXR Plugin installed?");
            EditorUtility.SetDirty(settings);

            // RefreshFeatures also creates the OpenXR package settings asset when it does not exist yet.
            FeatureHelpers.RefreshFeatures(group);
            var openXr = OpenXRSettings.GetSettingsForBuildTargetGroup(group);
            if (openXr == null)
            {
                Debug.LogError($"RUFA: OpenXR settings for {group} not found. Open Project Settings > XR Plug-in Management > OpenXR once, then run this again.");
                return;
            }

            Enable<OculusTouchControllerProfile>(openXr);
            if (group == BuildTargetGroup.Android)
            {
                Enable<MetaQuestFeature>(openXr);       // Quest manifest, target devices, Quest validation rules
                Enable<DisplayUtilitiesFeature>(openXr); // needed by TryRequestDisplayRefreshRate
            }
            EditorUtility.SetDirty(openXr);
        }

        static void Enable<T>(OpenXRSettings settings) where T : OpenXRFeature
        {
            var feature = settings.GetFeature<T>();
            if (feature == null)
            {
                Debug.LogError($"RUFA: OpenXR feature {typeof(T).Name} not found.");
                return;
            }
            feature.enabled = true;
            EditorUtility.SetDirty(feature);
        }

        // The Universal 3D template ships Mobile_RPAsset (Android default) and PC_RPAsset. Only the mobile one is tuned for Quest.
        static void ConfigureUrpMobileAsset()
        {
            var asset = FindUrpAsset("Mobile");
            if (asset == null)
            {
                Debug.LogError("RUFA: no URP asset with 'Mobile' in its name (expected Assets/Settings/Mobile_RPAsset.asset from the Universal 3D template).");
                return;
            }
            asset.msaaSampleCount = 4;                  // Meta recommends 4x MSAA on Quest
            asset.supportsHDR = false;
            asset.renderScale = 1f;
            asset.supportsCameraDepthTexture = false;
            asset.supportsCameraOpaqueTexture = false;
            EditorUtility.SetDirty(asset);
        }

        static UniversalRenderPipelineAsset FindUrpAsset(string nameContains)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path).Contains(nameContains))
                    return AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            }
            return null;
        }

        static void ImportXriStarterAssets()
        {
            var package = UnityEditor.PackageManager.PackageInfo.FindForPackageName(XriPackage);
            if (package == null)
            {
                Debug.LogError("RUFA: XR Interaction Toolkit is not installed.");
                return;
            }

            foreach (var sample in Sample.FindByPackage(XriPackage, package.version))
            {
                if (sample.displayName != XriSample) continue;
                if (sample.isImported)
                {
                    Debug.Log("RUFA: XRI Starter Assets already imported.");
                    return;
                }
                Debug.Log(sample.Import(Sample.ImportOptions.OverridePreviousImports)
                    ? "RUFA: XRI Starter Assets imported."
                    : "RUFA: XRI Starter Assets import FAILED.");
                return;
            }
            Debug.LogError($"RUFA: sample '{XriSample}' not found in {XriPackage} {package.version}.");
        }
    }
}
