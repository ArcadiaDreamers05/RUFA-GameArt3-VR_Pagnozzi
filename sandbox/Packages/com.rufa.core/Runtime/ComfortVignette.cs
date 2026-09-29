using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

namespace Rufa
{
    /// <summary>
    /// Tunnelling vignette during locomotion, attached to the headset camera at runtime and wired to every
    /// locomotion provider of the rig. Enable/disable the component to switch it on and off. VR only.
    /// </summary>
    public sealed class ComfortVignette : MonoBehaviour
    {
        [Tooltip("The 'TunnelingVignette' prefab from the XRI Starter Assets sample.")]
        public GameObject vignettePrefab;

        GameObject instance;
        bool started;

        void Start()
        {
            started = true;
            if (enabled) Build();
        }

        void OnEnable() { if (started) Build(); }

        void OnDisable()
        {
            if (instance != null) Destroy(instance);
            instance = null;
        }

        void Build()
        {
            if (instance != null || !RigBootstrap.XrIsActive || Camera.main == null) return;
            if (vignettePrefab == null) { Debug.LogWarning("ComfortVignette: no vignette prefab assigned. Run 'RUFA/Setup/6'."); return; }
            instance = Instantiate(vignettePrefab, Camera.main.transform, false);
            var controller = instance.GetComponent<TunnelingVignetteController>();
            if (controller == null) { Debug.LogWarning("ComfortVignette: the prefab has no TunnelingVignetteController."); return; }
            foreach (var provider in FindObjectsByType<LocomotionProvider>(FindObjectsSortMode.None))
                controller.locomotionVignetteProviders.Add(new LocomotionVignetteProvider { locomotionProvider = provider, enabled = true });
        }
    }
}
