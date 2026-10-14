using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Rufa
{
    /// <summary>
    /// A platform that holds only on its marked spots: a teleport that ends anywhere else on it sends the player back.
    /// It asks for a volume of anchors on the spots (a Teleportation Multi-Anchor Volume), where the volume, not the
    /// aim, picks the exact landing point.
    /// </summary>
    public sealed class MarkedSpots : CourseGoal
    {
        [Tooltip("The platform: a teleport that ends on its footprint must end on a spot.")]
        public Collider platform;
        [Tooltip("The marked spots.")]
        public Transform[] spots;
        [Tooltip("Where a wrong landing sends the player.")]
        public Transform back;
        [Tooltip("How close to a spot, seen from above, a landing must be, in meters.")]
        public float radius = 0.3f;

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

        void Landed(LocomotionProvider _)
        {
            var feet = Feet();
            if (feet == null || !Above(platform, feet.Value)) return;
            if (OnSpot(feet.Value, spots, radius)) { reached = true; return; }
            teleport.QueueTeleportRequest(new TeleportRequest
            {
                destinationPosition = back.position,
                destinationRotation = back.rotation,
                matchOrientation = MatchOrientation.TargetUpAndForward,
            });
        }

        /// <summary>True when the point is within radius of a spot, seen from above.</summary>
        public static bool OnSpot(Vector3 point, Transform[] spots, float radius) =>
            spots.Any(s => Vector3.ProjectOnPlane(point - s.position, Vector3.up).magnitude <= radius);
    }
}
