using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>
    /// Menu toggle. Checked: plain Play, without a headset, spawns the real XR rig prefab and the XR Interaction Simulator,
    /// which drives head and controllers from keyboard and mouse: this is how a change to the XR rig is tried without a headset.
    /// Unchecked (default): plain Play spawns the desktop rig of the PC build, which does not use the XR rig prefab at all.
    /// With "VR con Quest Link" on and the headset connected, the headset wins.
    /// Stored per project and per user (EditorUserSettings), never in git.
    /// </summary>
    public static class PlayModeSimulator
    {
        const string MenuPath = "RUFA/Play mode/Simulatore VR";

        static bool On => EditorUserSettings.GetConfigValue(RigBootstrap.SimulatorSetting) == "1";

        [MenuItem(MenuPath)]
        static void Toggle()
        {
            bool on = !On;
            if (on && !ImportSimulator()) return;
            EditorUserSettings.SetConfigValue(RigBootstrap.SimulatorSetting, on ? "1" : null);
            Debug.Log(on ? "RUFA: Play will use the XR rig, driven by the simulator (no headset needed)."
                         : "RUFA: Play will use the desktop rig.");
        }

        [MenuItem(MenuPath, true)]
        static bool Validate()
        {
            Menu.SetChecked(MenuPath, On);
            return true;
        }

        // The XRI sample is imported into each copy on first use rather than kept in git: it weighs 34 MB.
        static bool ImportSimulator()
        {
            KeepOutOfGit(Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..")));
            if (AssetDatabase.LoadAssetAtPath<GameObject>(RigBootstrap.SimulatorPrefabPath) != null) return true;
            var sample = Sample.FindByPackage("com.unity.xr.interaction.toolkit", "3.6.1").FirstOrDefault(s => s.displayName == "XR Interaction Simulator");
            if (sample.displayName == null || !sample.Import(Sample.ImportOptions.HideImportWindow))
            {
                Debug.LogError("RUFA: could not import the 'XR Interaction Simulator' sample of XR Interaction Toolkit 3.6.1.");
                return false;
            }
            AssetDatabase.Refresh();
            return true;
        }

        static readonly string[] IgnoreRules =
        {
            "/sandbox/Assets/Samples/XR Interaction Toolkit/*/XR Interaction Simulator/",
            "/sandbox/Assets/Samples/XR Interaction Toolkit/*/XR Interaction Simulator.meta",
        };

        // The course's .gitignore lists the sample too, but "git aggiorna" does not bring a new .gitignore to copies cloned
        // before: the local exclude file of the repository keeps the sample out of "git salva" in those copies as well.
        public static void KeepOutOfGit(string repositoryRoot)
        {
            var info = Path.Combine(repositoryRoot, ".git", "info");
            if (!Directory.Exists(Path.Combine(repositoryRoot, ".git"))) return; // not the root of a git repository
            Directory.CreateDirectory(info);
            var exclude = Path.Combine(info, "exclude");
            var lines = File.Exists(exclude) ? File.ReadAllLines(exclude) : new string[0];
            foreach (var rule in IgnoreRules.Where(r => !lines.Contains(r)))
                File.AppendAllText(exclude, rule + "\n");
        }
    }
}
