using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace Rufa
{
    /// <summary>A room to cross by pulling the world with the hands: reached when the player grab-moves inside it.</summary>
    public sealed class GrabMoveRoom : CourseGoal
    {
        [Tooltip("The floor of the room: grab moving above its footprint counts.")]
        public Collider room;

        void Update()
        {
            if (reached) return;
            var feet = Feet();
            if (feet == null || !Above(room, feet.Value)) return;
            // Grab Move Provider and Two-Handed Grab Move Provider are both constrained move providers.
            reached = FindObjectsByType<ConstrainedMoveProvider>(FindObjectsSortMode.None).Any(p => p.locomotionState == LocomotionState.Moving);
        }
    }
}
