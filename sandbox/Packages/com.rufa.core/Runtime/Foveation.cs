using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Rufa
{
    /// <summary>Fixed foveated rendering level of the headset: 0 = off, 0.5 = medium, 1 = high. No headset: no-op.</summary>
    public static class Foveation
    {
        public static float GetLevel()
        {
            var display = Display();
            return display != null ? display.foveatedRenderingLevel : 0f;
        }

        public static void SetLevel(float level)
        {
            var display = Display();
            if (display != null) display.foveatedRenderingLevel = Mathf.Clamp01(level);
        }

        static XRDisplaySubsystem Display()
        {
            var displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displays);
            return displays.Count > 0 ? displays[0] : null;
        }
    }

    /// <summary>Drop this on any object to set the foveation level at start-up (students' projects).</summary>
    public sealed class FoveationLevel : MonoBehaviour
    {
        [Range(0f, 1f)] public float level = 1f;
        void Start() => Foveation.SetLevel(level);
    }
}
