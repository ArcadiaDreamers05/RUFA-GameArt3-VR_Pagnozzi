using UnityEngine;
using UnityEngine.Events;

namespace Rufa
{
    /// <summary>
    /// A part that slides on a Configurable Joint, like a drawer or the knob of a slider: its value goes from 0, where it
    /// starts, to 1, at the other end of its run. The joint is connected to the body that carries it, so the block works
    /// wherever it stands.
    /// </summary>
    [RequireComponent(typeof(ConfigurableJoint))]
    public sealed class Slide : MonoBehaviour
    {
        public UnityEvent<float> onValueChanged = new UnityEvent<float>();
        [Tooltip("Fired when the part reaches the end of its run; again only after it has gone back a little.")]
        public UnityEvent onEnd = new UnityEvent();
        [Tooltip("The fastest the part runs, in m/s: as fast as a hand. Velocity Tracking, with the hand far off, would slam it to its stop in one step and leave behind what lies in a drawer.")]
        public float maxSpeed = 1.5f;
        public float Value { get; private set; }

        ConfigurableJoint joint;
        Vector3 start;
        float reported = -1f;   // the first FixedUpdate tells the value it starts with
        bool atEnd;

        void Awake()
        {
            joint = GetComponent<ConfigurableJoint>();
            GetComponent<Rigidbody>().maxLinearVelocity = maxSpeed;   // the body does not save it: set when the scene starts
            start = Local();
        }

        void FixedUpdate()
        {
            Value = Fraction(Vector3.Distance(Local(), start), 2f * joint.linearLimit.limit);
            if (Mathf.Abs(Value - reported) >= 0.01f) { reported = Value; onValueChanged.Invoke(Value); }
            if (!atEnd && Value > 0.98f) { atEnd = true; onEnd.Invoke(); }
            else if (atEnd && Value < 0.9f) atEnd = false;
        }

        // Where the part is, seen from the body it slides on
        Vector3 Local() => joint.connectedBody != null ? joint.connectedBody.transform.InverseTransformPoint(transform.position) : transform.position;

        public static float Fraction(float travelled, float run) => run <= 0f ? 0f : Mathf.Clamp01(travelled / run);
    }
}
