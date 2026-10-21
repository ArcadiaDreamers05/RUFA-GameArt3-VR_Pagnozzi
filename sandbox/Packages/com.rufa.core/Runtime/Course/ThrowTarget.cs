using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Rufa
{
    /// <summary>A target to hit with something thrown: a body nobody is holding that strikes it fast enough.</summary>
    public sealed class ThrowTarget : CourseGoal
    {
        [Tooltip("Slowest hit that counts, in metres per second: pushing the ball against the target does not.")]
        public float minSpeed = 1.5f;
        public UnityEvent onHit = new UnityEvent();

        void OnCollisionEnter(Collision collision)
        {
            if (reached || !Counts(collision.rigidbody, collision.relativeVelocity, minSpeed)) return;
            reached = true;
            onHit.Invoke();
        }

        /// <summary>
        /// True for a free body (let go, not held in a hand) hitting at least at the given speed across the ground:
        /// a ball dropped from above falls fast too, but it was not thrown.
        /// </summary>
        public static bool Counts(Rigidbody body, Vector3 velocity, float minSpeed) =>
            body != null && Vector3.ProjectOnPlane(velocity, Vector3.up).magnitude >= minSpeed
            && !(body.TryGetComponent(out XRGrabInteractable grab) && grab.isSelected);
    }
}
