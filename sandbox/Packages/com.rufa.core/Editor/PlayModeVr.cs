using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>
    /// Menu toggle. Checked: pressing Play initialises OpenXR, so the editor drives the headset through
    /// Quest Link (the Meta Quest Link app must be running with OpenXR runtime = Meta).
    /// Unchecked (default): Play uses the desktop rig.
    /// </summary>
    public static class PlayModeVr
    {
        const string MenuPath = "RUFA/Play mode/VR con Quest Link";

        [MenuItem(MenuPath)]
        static void Toggle()
        {
            var standalone = ProjectSetup.XrSettings(BuildTargetGroup.Standalone);
            standalone.InitManagerOnStart = !standalone.InitManagerOnStart;
            EditorUtility.SetDirty(standalone);
            AssetDatabase.SaveAssets();
            Debug.Log(standalone.InitManagerOnStart
                ? "RUFA: Play will start OpenXR (VR via Quest Link)."
                : "RUFA: Play will use the desktop rig.");
        }

        // Read-only check mark; must not create assets, so it uses the static lookup that returns null when unset.
        [MenuItem(MenuPath, true)]
        static bool Validate()
        {
            var standalone = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
            Menu.SetChecked(MenuPath, standalone != null && standalone.InitManagerOnStart);
            return true;
        }
    }
}
