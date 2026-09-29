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
    /// </summary>
    public sealed class RigBootstrap : MonoBehaviour
    {
        [Tooltip("Prefab spawned when XR is active. Use 'XR Origin (XR Rig)' from the XRI Starter Assets sample.")]
        public GameObject xrRigPrefab;

        [Tooltip("Headset refresh rate requested at start-up, in Hz. Quest 3S supports 72, 90 and 120.")]
        public float targetRefreshRate = 90f;

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
