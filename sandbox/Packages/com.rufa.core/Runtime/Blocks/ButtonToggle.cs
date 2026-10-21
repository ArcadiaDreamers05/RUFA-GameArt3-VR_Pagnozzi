using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace Rufa
{
    /// <summary>
    /// A switch the player carries, like a torch on a controller: every press of the button turns the target on or off.
    /// The button is read with an input reader, as XR Interaction Toolkit components read theirs. It starts with an
    /// action of its own (Input Action mode), to bind in the Inspector without touching the project's input actions.
    /// </summary>
    public sealed class ButtonToggle : MonoBehaviour
    {
        [Tooltip("The button that switches the target: give its action a binding, e.g. the secondary button of the right controller.")]
        public XRInputButtonReader button = new XRInputButtonReader("Accendi", inputSourceMode: XRInputButtonReader.InputSourceMode.InputAction);
        [Tooltip("What turns on and off: a light, or any object.")]
        public GameObject target;

        void OnEnable() => button.EnableDirectActionIfModeUsed();
        void OnDisable() => button.DisableDirectActionIfModeUsed();

        void Update()
        {
            if (target != null && button.ReadWasPerformedThisFrame()) target.SetActive(!target.activeSelf);
        }
    }
}
