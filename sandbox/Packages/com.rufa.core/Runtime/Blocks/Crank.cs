using UnityEngine;
using UnityEngine.Events;

namespace Rufa
{
    /// <summary>
    /// A crank on a hinge with no limits: it counts the turns, and its value goes from 0 to 1 over the turns asked.
    /// Turned back, it unwinds; it fires onCompleted once, the first time it gets to 1.
    /// </summary>
    [RequireComponent(typeof(HingeJoint))]
    public sealed class Crank : MonoBehaviour
    {
        [Tooltip("Full turns that complete the crank.")]
        public float turns = 3f;
        public UnityEvent<float> onValueChanged = new UnityEvent<float>();
        public UnityEvent onCompleted = new UnityEvent();
        public float Value { get; private set; }
        public bool IsCompleted { get; private set; }

        HingeJoint hinge;
        float previous, degrees, reported = -1f;

        void Awake()
        {
            hinge = GetComponent<HingeJoint>();
        }

        void FixedUpdate()
        {
            var angle = float.IsNaN(hinge.angle) ? previous : hinge.angle;   // right where it started, the hinge may say NaN instead of 0
            degrees = Accumulate(degrees, previous, angle, 360f * turns);
            previous = angle;
            Value = Fraction(degrees, turns);
            if (Mathf.Abs(Value - reported) >= 0.01f) { reported = Value; onValueChanged.Invoke(Value); }
            if (IsCompleted || Value < 1f) return;
            IsCompleted = true;
            onCompleted.Invoke();
        }

        // The hinge gives its angle between -180 and 180: the turns are the sum of the steps, kept within 0 and max
        public static float Accumulate(float degrees, float previous, float angle, float max) =>
            Mathf.Clamp(degrees + Mathf.DeltaAngle(previous, angle), 0f, max);

        public static float Fraction(float degrees, float turns) => turns <= 0f ? 1f : Mathf.Clamp01(degrees / (360f * turns));
    }
}
