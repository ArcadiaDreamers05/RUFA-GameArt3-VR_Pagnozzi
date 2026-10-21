using UnityEngine;
using UnityEngine.Events;

namespace Rufa
{
    /// <summary>
    /// A lever on a hinge: pulled past its angle, it fires onPulled once. The hand moves it through an XR Grab
    /// Interactable in Velocity Tracking, which moves the body with physics, so the Hinge Joint keeps it on its pivot;
    /// Instantaneous would carry it off the hinge.
    /// </summary>
    [RequireComponent(typeof(HingeJoint))]
    public sealed class Lever : MonoBehaviour
    {
        [Tooltip("Angle of the hinge, in degrees, past which the lever counts as pulled.")]
        public float pullAngle = 35f;
        public UnityEvent onPulled = new UnityEvent();
        public bool IsPulled { get; private set; }

        void FixedUpdate()
        {
            if (IsPulled || !Pulled(GetComponent<HingeJoint>().angle, pullAngle)) return;
            IsPulled = true;
            onPulled.Invoke();
        }

        public static bool Pulled(float angle, float pullAngle) => angle >= pullAngle;
    }
}
