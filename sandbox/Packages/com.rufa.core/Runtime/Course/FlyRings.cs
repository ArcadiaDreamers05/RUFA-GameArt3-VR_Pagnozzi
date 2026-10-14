using System.Linq;
using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// Rings hanging in the air, to fly through: each one disappears when the head goes through it, and the goal is reached
    /// when none is left.
    /// </summary>
    public sealed class FlyRings : CourseGoal
    {
        [Tooltip("The rings: the hole of each one lies in its local XY plane, around its origin.")]
        public Transform[] rings;
        [Tooltip("Radius of the hole, in meters.")]
        public float radius = 0.9f;

        void Update()
        {
            if (reached || Camera.main == null) return;
            foreach (var ring in rings.Where(r => r.gameObject.activeSelf && Through(r, Camera.main.transform.position, radius)))
                ring.gameObject.SetActive(false);
            reached = rings.All(r => !r.gameObject.activeSelf);
        }

        /// <summary>True when the point is in the hole of the ring: close to its plane and within the radius.</summary>
        public static bool Through(Transform ring, Vector3 point, float radius)
        {
            var local = ring.InverseTransformPoint(point);
            return Mathf.Abs(local.z) < 0.3f && new Vector2(local.x, local.y).magnitude < radius;
        }
    }
}
