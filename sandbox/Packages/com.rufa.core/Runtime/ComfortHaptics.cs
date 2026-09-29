using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Rufa
{
    /// <summary>
    /// A short controller vibration on every selection (grab, button, door). The XRI 3.6 rig has the haptic
    /// players on the controllers but no feedback component on the interactors: this adds them at runtime.
    /// </summary>
    public sealed class ComfortHaptics : MonoBehaviour
    {
        [Range(0f, 1f)] public float amplitude = 0.5f;
        public float duration = 0.1f;

        readonly List<SimpleHapticFeedback> feedbacks = new List<SimpleHapticFeedback>();

        void Start()
        {
            if (!RigBootstrap.XrIsActive) return;
            foreach (var interactor in FindObjectsByType<XRBaseInputInteractor>(FindObjectsSortMode.None))
            {
                if (interactor.GetComponentInParent<HapticImpulsePlayer>() == null) continue;
                var feedback = interactor.GetComponent<SimpleHapticFeedback>();
                if (feedback == null) feedback = interactor.gameObject.AddComponent<SimpleHapticFeedback>();
                feedback.playSelectEntered = true;
                feedback.selectEnteredData.amplitude = amplitude;
                feedback.selectEnteredData.duration = duration;
                feedback.enabled = enabled;
                feedbacks.Add(feedback);
            }
        }

        void OnEnable() { foreach (var f in feedbacks) if (f != null) f.enabled = true; }
        void OnDisable() { foreach (var f in feedbacks) if (f != null) f.enabled = false; }
    }
}
