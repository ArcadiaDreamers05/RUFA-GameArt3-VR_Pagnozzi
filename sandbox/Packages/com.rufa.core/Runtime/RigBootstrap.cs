using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features.Meta;

namespace Rufa
{
    /// <summary>
    /// The only component the scene needs. On Awake it spawns the player rig
    /// that matches how the app was started:
    /// - XR active (Quest build, or Play Mode with "VR con Quest Link" on): the XR rig prefab.
    /// - XR not active (PC build, or plain Play Mode): a first-person desktop rig built in code.
    ///   In the editor, "RUFA/Play mode/Simulatore VR" gives plain Play the XR rig instead.
    /// </summary>
    // Before the interactables of the scene (XRI runs them at -98): teleport targets and ladders look for the rig's providers
    // in Awake, and a Multi-Anchor Volume that finds none there throws as soon as the arc touches it.
    [DefaultExecutionOrder(-500)]
    public sealed class RigBootstrap : MonoBehaviour
    {
        [Tooltip("Prefab spawned when XR is active. Use 'XR Origin (XR Rig)' from the XRI Starter Assets sample.")]
        public GameObject xrRigPrefab;

        [Tooltip("Headset refresh rate requested at start-up, in Hz. Quest 3S supports 72, 90 and 120.")]
        public float targetRefreshRate = 90f;

        /// <summary>Per-project editor setting written by "RUFA/Play mode/Simulatore VR": "1" = on.</summary>
        public const string SimulatorSetting = "Rufa.SimulatoreVR";
        public const string SimulatorPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.6.1/XR Interaction Simulator/XR Interaction Simulator.prefab";

        /// <summary>True when XR Plug-in Management has an initialised loader, i.e. a headset is driving the app.</summary>
        public static bool XrIsActive =>
            XRGeneralSettings.Instance != null &&
            XRGeneralSettings.Instance.Manager != null &&
            XRGeneralSettings.Instance.Manager.activeLoader != null;

        void Awake()
        {
            // One interaction manager for both modes, created here so it is visible in the Hierarchy.
            // XRI would otherwise create one lazily from the first interactor.
            new GameObject("XR Interaction Manager", typeof(XRInteractionManager));

            if (!XrIsActive)
            {
                if (!SpawnSimulatedXrRig())
                    DesktopRig.Build(transform.position, transform.rotation);
                return;
            }

            if (xrRigPrefab == null)
            {
                Debug.LogError("RigBootstrap: xrRigPrefab is not assigned. Run 'RUFA/Setup/2 - Crea scena Sandbox'.");
                return;
            }

            Instantiate(xrRigPrefab, transform.position, transform.rotation);
            if (Application.platform == RuntimePlatform.Android) // Quest Link: the PC runtime owns the refresh rate
                StartCoroutine(RequestRefreshRate());
        }

        // Editor only: with "RUFA/Play mode/Simulatore VR" on, plain Play gets the XR rig prefab,
        // driven from keyboard and mouse by the XRI simulator, instead of the desktop rig.
        bool SpawnSimulatedXrRig()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUserSettings.GetConfigValue(SimulatorSetting) != "1" || xrRigPrefab == null) return false;
            var simulator = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(SimulatorPrefabPath);
            if (simulator == null)
            {
                Debug.LogWarning("RigBootstrap: simulator not found, using the desktop rig. Turn 'RUFA/Play mode/Simulatore VR' off and on again to import it.");
                return false;
            }
            var rig = Instantiate(xrRigPrefab, transform.position, transform.rotation);
            Instantiate(simulator);  // after the rig, so the simulator finds its controllers for point-and-click
            StartCoroutine(RaiseHead(rig.transform));
            return true;
#else
            return false;
#endif
        }

        // Without a headset nothing tracks the head, which would stay on the floor: put the eyes where the desktop rig
        // has them. One frame later, because the simulator resets the camera offset when the scene finishes loading.
        static IEnumerator RaiseHead(Transform rig)
        {
            yield return null;
            var cameraOffset = rig.Find("Camera Offset");
            if (cameraOffset != null) cameraOffset.localPosition = Vector3.up * DesktopRig.EyeHeight;
        }

        IEnumerator RequestRefreshRate()
        {
            // The display subsystem is up one frame after the loader starts.
            yield return null;

            var displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displays);
            if (displays.Count == 0)
            {
                Debug.LogWarning("RigBootstrap: no XR display subsystem, refresh rate not requested.");
                yield break;
            }

            bool accepted = displays[0].TryRequestDisplayRefreshRate(targetRefreshRate);
            Debug.Log($"RigBootstrap: refresh rate {targetRefreshRate} Hz requested, accepted = {accepted}");
        }
    }
}
