using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Rufa
{
    /// <summary>
    /// A door that opens only for whoever lands on its pad with a teleport, already facing it: the reward for a designed
    /// arrival (an anchor that sets the direction, or an area where the player picks it with the stick).
    /// Walking onto the pad and turning there does not open it.
    /// </summary>
    public sealed class ArrivalDoor : CourseGoal
    {
        [Tooltip("The pad in front of the door: the teleport must end on its footprint.")]
        public Collider pad;
        [Tooltip("Largest angle, in degrees, between the gaze and the direction of the door.")]
        public float maxAngle = 20f;
        [Tooltip("How far the door slides along its own X axis when it opens, in meters.")]
        public float slide = 1f;

        TeleportationProvider teleport;

        void Start()
        {
            teleport = FindAnyObjectByType<TeleportationProvider>();
            if (teleport != null) teleport.locomotionEnded += Landed;
        }

        void OnDestroy()
        {
            if (teleport != null) teleport.locomotionEnded -= Landed;
        }

        // XRI raises locomotionEnded once the rig has moved, so the feet and the camera are already at the destination.
        void Landed(LocomotionProvider _)
        {
            var feet = Feet();
            if (reached || feet == null || Camera.main == null || !Above(pad, feet.Value)) return;
            if (!Faces(Camera.main.transform.forward, feet.Value, transform.position, maxAngle)) return;
            reached = true;
            GetComponent<Collider>().enabled = false;
            transform.position += transform.right * slide;
        }

        /// <summary>True when the gaze, seen from above, points at the target within maxAngle degrees.</summary>
        public static bool Faces(Vector3 gaze, Vector3 from, Vector3 target, float maxAngle) =>
            Vector3.Angle(Vector3.ProjectOnPlane(gaze, Vector3.up), Vector3.ProjectOnPlane(target - from, Vector3.up)) <= maxAngle;
    }
}
