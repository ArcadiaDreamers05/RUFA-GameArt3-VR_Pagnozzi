using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace Rufa
{
    /// <summary>
    /// A goal of a practice course. The lesson tutorial reads <see cref="reached"/> ("> verifica: riuscito Name");
    /// each subclass decides when it is reached.
    /// </summary>
    public abstract class CourseGoal : MonoBehaviour
    {
        [Tooltip("Set when the player achieves this goal. Read by the lesson tutorial.")]
        public bool reached;

        /// <summary>
        /// Where the player's body meets the ground: the bottom of the XR rig's Character Controller, which follows the
        /// head (walking in the room moves it too). Null when no XR rig is running, as with the desktop rig.
        /// </summary>
        public static Vector3? Feet()
        {
            var mediator = FindAnyObjectByType<LocomotionMediator>();
            var body = mediator == null ? null : mediator.transform.root.GetComponent<CharacterController>();
            if (body == null) return null;
            return body.transform.TransformPoint(body.center - Vector3.up * (body.height * 0.5f));
        }

        /// <summary>True when the point lies inside the horizontal footprint of the collider, at any height.</summary>
        public static bool Above(Collider surface, Vector3 point)
        {
            var b = surface.bounds;
            return point.x >= b.min.x && point.x <= b.max.x && point.z >= b.min.z && point.z <= b.max.z;
        }
    }
}
