using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// A door that opens once the player stands on its platform: the way out of a shaft reached only by falling through a
    /// trapdoor. Until then it stays shut, so the teleport arc cannot leave the shaft either.
    /// </summary>
    public sealed class FallDoor : CourseGoal
    {
        [Tooltip("The floor of the shaft: the door opens when the feet are on it.")]
        public Collider platform;
        [Tooltip("How far the door slides along its own X axis when it opens, in meters.")]
        public float slide = 1f;

        void Update()
        {
            if (reached || !(Feet() is Vector3 feet) || !Above(platform, feet) || feet.y > platform.bounds.max.y + 0.2f) return;
            reached = true;
            GetComponent<Collider>().enabled = false;
            transform.position += transform.right * slide;
        }
    }
}
