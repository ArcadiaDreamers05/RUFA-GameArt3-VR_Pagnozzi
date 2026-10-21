using UnityEngine;
using UnityEngine.Events;

namespace Rufa
{
    /// <summary>A knob on a hinge with limits: its value goes from 0 at one limit to 1 at the other.</summary>
    [RequireComponent(typeof(HingeJoint))]
    public sealed class Knob : MonoBehaviour
    {
        public UnityEvent<float> onValueChanged = new UnityEvent<float>();
        public float Value { get; private set; }

        HingeJoint hinge;
        float reported = -1f;   // the first FixedUpdate tells the value it starts with

        void Awake() => hinge = GetComponent<HingeJoint>();

        void FixedUpdate()
        {
            var angle = float.IsNaN(hinge.angle) ? 0f : hinge.angle;   // right where it started, the hinge may say NaN instead of 0
            Value = Fraction(angle, hinge.limits.min, hinge.limits.max);
            if (Mathf.Abs(Value - reported) < 0.01f) return;
            reported = Value;
            onValueChanged.Invoke(Value);
        }

        public static float Fraction(float angle, float min, float max) => Mathf.InverseLerp(min, max, angle);
    }
}
