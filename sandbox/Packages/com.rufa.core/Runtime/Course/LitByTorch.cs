using System.Linq;
using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// Something hidden in the dark: reached when a spot light carried by the player, a torch on the rig, shines on it.
    /// The rig is recognised by the CharacterController above the light, as the narrative trigger recognises the player.
    /// </summary>
    public sealed class LitByTorch : CourseGoal
    {
        void Update()
        {
            if (reached) return;
            var target = GetComponent<Collider>();
            reached = FindObjectsByType<Light>(FindObjectsSortMode.None)
                .Any(l => l.GetComponentInParent<CharacterController>() != null && Shines(l, transform.position, target));
        }

        /// <summary>True when a switched-on spot light reaches the point inside its cone, with nothing in between but the target.</summary>
        public static bool Shines(Light light, Vector3 point, Collider target)
        {
            if (light.type != LightType.Spot || !light.isActiveAndEnabled) return false;
            var from = light.transform.position;
            var to = point - from;
            if (to.magnitude > light.range || Vector3.Angle(light.transform.forward, to) > light.spotAngle / 2f) return false;
            return !Physics.Linecast(from, point, out var hit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) || hit.collider == target;
        }
    }
}
