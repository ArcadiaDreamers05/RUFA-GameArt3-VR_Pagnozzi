using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.XR.Hands.OpenXR;
using UnityEngine.XR.OpenXR;

namespace Rufa.Editor
{
    /// <summary>
    /// The hands of the Quest, through XR Hands and OpenXR. The tutorial of lesson 3 prepares them: the samples with the
    /// hand models, the gestures and the rig with hands, imported into each copy like the simulator, and the two OpenXR
    /// features that read the hands, for the Quest and for the PC, which Quest Link uses. With them on, XR Hands adds
    /// the hand tracking permission to the Quest build.
    /// The Gestures sample comes without its Editor folder: there it checks, at every reload, that the samples it needs
    /// are imported, reading every folder of Assets/Samples as package/version/sample. The practices of the course have
    /// no version folder, so the check failed: an error in the console and Project Validation opening each time. The
    /// samples it looks for are the ones imported here.
    /// </summary>
    public static class HandsSetup
    {
        static readonly (string package, string version, string sample)[] Samples =
        {
            ("com.unity.xr.hands", "1.8.1", "HandVisualizer"),   // first: the hands of the XRI sample are built on its models
            ("com.unity.xr.hands", "1.8.1", "Gestures"),
            ("com.unity.xr.interaction.toolkit", "3.6.1", "Hands Interaction Demo"),
        };

        static readonly BuildTargetGroup[] Targets = { BuildTargetGroup.Android, BuildTargetGroup.Standalone };

        // The practice built on the rig with hands. Git keeps it and not the samples: in a fresh clone, on another
        // computer, it would open without its rig, so the hands come back by themselves, as the simulator does.
        const string HandsPractice = "Assets/Samples/RUFA Core/Lezione 03 - Interazioni";

        [InitializeOnLoadMethod]
        static void PrepareOnLoad() => EditorApplication.delayCall += PrepareIfNeeded;

        public static void PrepareIfNeeded()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && AssetDatabase.IsValidFolder(HandsPractice) && !Ready()) Prepare();
        }

        public static void Prepare()
        {
            PlayModeSimulator.KeepOutOfGit(Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..")));
            foreach (var sample in Samples.Select(Find).Where(s => !s.isImported))
                if (!SampleImport.Copy(sample))
                    Debug.LogError("RUFA: a sample of the hands is missing from its package.");
            if (Find(Samples[1]).isImported) AssetDatabase.DeleteAsset(GesturesCheck);
            foreach (var group in Targets)
            {
                var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(group);
                ProjectSetup.Enable<HandTracking>(settings);
                ProjectSetup.Enable<MetaHandTrackingAim>(settings);
            }
            AssetDatabase.SaveAssets();
        }

        public static bool Ready() =>
            Samples.All(s => Find(s).isImported) && !AssetDatabase.IsValidFolder(GesturesCheck)
            && Targets.Select(OpenXRSettings.GetSettingsForBuildTargetGroup)
                      .All(s => s != null && s.GetFeature<HandTracking>()?.enabled == true && s.GetFeature<MetaHandTrackingAim>()?.enabled == true);

        static string GesturesCheck => FileUtil.GetProjectRelativePath(Find(Samples[1]).importPath.Replace('\\', '/')) + "/Editor";

        static Sample Find((string package, string version, string sample) s) =>
            Sample.FindByPackage(s.package, s.version).FirstOrDefault(x => x.displayName == s.sample);
    }
}
