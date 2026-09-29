using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Rufa.Demo
{
    /// <summary>
    /// Runtime side of the "sick" demo: settings that live on the player camera and
    /// therefore can only be applied after RigBootstrap has spawned the rig.
    /// Later lessons add more switches here and drive them from the teacher menu.
    /// </summary>
    public sealed class DemoDefects : MonoBehaviour
    {
        [Tooltip("Post-processing (bloom, vignette) on the player camera. Expensive on Quest; cured in lesson 10.")]
        public bool postProcessing = true;

        [Tooltip("Real-time shadows on the candles: six shadow maps per candle, every frame, the heaviest defect of all. Baked in lesson 8. " +
                 "In lessons 2-4 the teacher switches them off, so locomotion and interactions are judged at a usable frame rate.")]
        public bool candleShadows = true;

        void Awake() => TeacherMenu.Register("Ombre candele", () => candleShadows, on => { candleShadows = on; Apply(); });

        void Start() => Apply();

        public void Apply()
        {
            // Once lesson 8 has baked them, the candles' shadows are in the lightmaps and this changes nothing.
            foreach (var light in FindObjectsByType<Light>(FindObjectsSortMode.None))
                if (light.name == "Fiamma") light.shadows = candleShadows ? LightShadows.Soft : LightShadows.None;

            var camera = Camera.main;
            if (camera == null)
            {
                Debug.LogWarning("DemoDefects: no main camera yet, post-processing not applied.");
                return;
            }
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = postProcessing;
        }
    }
}
