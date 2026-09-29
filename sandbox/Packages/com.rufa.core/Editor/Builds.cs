using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>
    /// Builds from the editor menu or from batchmode:
    ///   Unity -batchmode -quit -projectPath . -buildTarget Android -executeMethod Rufa.Editor.Builds.BuildQuest
    ///   Unity -batchmode -quit -projectPath . -buildTarget Win64   -executeMethod Rufa.Editor.Builds.BuildPc
    /// </summary>
    public static class Builds
    {
        public const string DemoScene = "Assets/Demo/Scenes/Demo.unity";

        [MenuItem("RUFA/Build/Quest (APK)")]
        public static void BuildQuest() =>
            ExitIfBatch(Run(BuildTarget.Android, $"Builds/Quest/{FileName()}.apk"));

        [MenuItem("RUFA/Build/Quest (Development, Profiler)")]
        public static void BuildQuestProfiling() =>
            ExitIfBatch(Run(BuildTarget.Android, $"Builds/Quest/{FileName()}_dev.apk", BuildOptions.Development | BuildOptions.ConnectWithProfiler));

        [MenuItem("RUFA/Build/PC (Windows)")]
        public static void BuildPc() => ExitIfBatch(RunPc($"Builds/PC/{FileName()}/{FileName()}.exe", null));

        [MenuItem("RUFA/Build/Demo (APK)")]
        public static void BuildDemoQuest() => ExitIfBatch(BuildQuestScene("RUFA Demo", "it.rufa.gameart3.demo", DemoScene));

        /// <summary>One scene as its own app: its own name and package, so it installs next to the project's build.</summary>
        public static int BuildQuestScene(string product, string identifier, string scene, BuildOptions options = BuildOptions.None)
        {
            if (!System.IO.File.Exists(scene)) { Debug.LogError($"RUFA: {scene} not found."); return 1; }
            var suffix = (options & BuildOptions.Development) != 0 ? "_dev" : "";
            return WithNames(product, identifier, () => Run(BuildTarget.Android, $"Builds/Quest/{product.Replace(' ', '_')}{suffix}.apk", options, new[] { scene }));
        }

        public static int BuildPcScene(string product, string scene)
        {
            if (!System.IO.File.Exists(scene)) { Debug.LogError($"RUFA: {scene} not found."); return 1; }
            var name = product.Replace(' ', '_');
            return WithNames(product, PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android), () => RunPc($"Builds/PC/{name}/{name}.exe", new[] { scene }));
        }

        // Batchmode exit codes: 0 built, 1 build failed, 2 platform switched (run again).
        static void ExitIfBatch(int code)
        {
            if (code != 0 && Application.isBatchMode) EditorApplication.Exit(code);
        }

        static string FileName() => PlayerSettings.productName.Replace(' ', '_');

        // The names go back afterwards, even when the build fails: a build never leaves ProjectSettings changed.
        static int WithNames(string product, string identifier, System.Func<int> build)
        {
            var (oldProduct, oldIdentifier) = (PlayerSettings.productName, PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android));
            SetNames(product, identifier);
            try { return build(); }
            finally { SetNames(oldProduct, oldIdentifier); }
        }

        static void SetNames(string product, string identifier)
        {
            PlayerSettings.productName = product;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, identifier);
            AssetDatabase.SaveAssets();
        }

        static void SetInitOnStart(UnityEngine.XR.Management.XRGeneralSettings settings, bool value)
        {
            settings.InitManagerOnStart = value;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        static int RunPc(string outputPath, string[] scenes)
        {
            // The PC build always starts in desktop mode, whatever the Play Mode toggle says.
            var standalone = ProjectSetup.XrSettings(BuildTargetGroup.Standalone);
            bool previous = standalone.InitManagerOnStart;
            SetInitOnStart(standalone, false);
            try { return Run(BuildTarget.StandaloneWindows64, outputPath, BuildOptions.None, scenes); }
            finally { SetInitOnStart(standalone, previous); }
        }

        static int Run(BuildTarget target, string outputPath, BuildOptions options = BuildOptions.None, string[] scenes = null)
        {
            var group = BuildPipeline.GetBuildTargetGroup(target);
            // OpenXR's Meta Quest checks read the platform selected in Build Settings, not the active one: keep them in sync.
            EditorUserBuildSettings.selectedBuildTargetGroup = group;
            if (EditorUserBuildSettings.activeBuildTarget != target)
            {
                // Switching platform re-imports assets; building in the same call is not reliable.
                EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
                Debug.LogWarning($"RUFA: active platform switched to {target}. Run the same menu item again to build.");
                return 2;
            }

            scenes ??= EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                targetGroup = group,
                options = options,
            });

            var summary = report.summary;
            // For the Quest, the APK's own size: summary.totalSize counts much more than what reaches the headset.
            var size = summary.result == BuildResult.Succeeded && target == BuildTarget.Android ? new System.IO.FileInfo(outputPath).Length : (long)summary.totalSize;
            Debug.Log($"RUFA: build {target} -> {outputPath}: {summary.result}, {size / (1024 * 1024)} MB, {summary.totalErrors} errors");
            return summary.result == BuildResult.Succeeded ? 0 : 1;
        }
    }
}
