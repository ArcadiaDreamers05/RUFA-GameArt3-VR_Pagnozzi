using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Rufa
{
    /// <summary>A switch: each selection (ray, finger, desktop click) turns it on or off and tips its handle.</summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class ToggleSwitch : MonoBehaviour
    {
        public Transform handle;
        [Tooltip("How far the handle tips either way, in degrees.")]
        public float angle = 20f;
        public UnityEvent onSwitchedOn = new UnityEvent();
        public UnityEvent onSwitchedOff = new UnityEvent();
        public bool IsOn { get; private set; }

        void Awake()
        {
            GetComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => Toggle());
            Show();
        }

        public void Toggle()
        {
            IsOn = !IsOn;
            Show();
            (IsOn ? onSwitchedOn : onSwitchedOff).Invoke();
        }

        void Show()
        {
            if (handle != null) handle.localRotation = Quaternion.Euler(IsOn ? -angle : angle, 0f, 0f);
        }
    }
}
