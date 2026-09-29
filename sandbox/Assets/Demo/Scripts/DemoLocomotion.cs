using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace Rufa.Demo
{
    /// <summary>Lesson 2 defect: locomotion that makes people sick, switchable live from the teacher menu.</summary>
    public sealed class DemoLocomotion : MonoBehaviour
    {
        public bool sick = true;

        void Awake() => TeacherMenu.Register("Locomozione malata", () => sick, value => { sick = value; Apply(); });

        void Start() => Apply(); // the rig exists by now (RigBootstrap.Awake ran first)

        public void Apply()
        {
            var move = FindFirstObjectByType<ContinuousMoveProvider>();   // the Starter Assets rig uses DynamicMoveProvider, a subclass
            if (move == null) return;                                      // desktop mode: nothing to do
            move.moveSpeed = sick ? 8f : 2f;
            move.enableStrafe = true;

            var smooth = FindFirstObjectByType<ContinuousTurnProvider>();
            if (smooth != null) smooth.turnSpeed = sick ? 180f : 60f;

            var snap = FindFirstObjectByType<SnapTurnProvider>();
            if (snap != null) { snap.turnAmount = 45f; snap.debounceTime = 0.5f; }

            // Each hand decides between smooth and snap turn; sick = smooth and fast.
            foreach (var hand in FindObjectsByType<ControllerInputActionManager>(FindObjectsSortMode.None))
                hand.smoothTurnEnabled = sick;
        }
    }
}
