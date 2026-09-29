using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Rufa
{
    /// <summary>
    /// The GPU time of the last frame, in milliseconds, from whichever source the platform has: FrameTimingManager
    /// in the editor, on PC and on plain Android; the XR display on Quest with OpenXR, where FrameTimingManager
    /// reports 0. Returns 0 when neither is available.
    /// </summary>
    public static class GpuTime
    {
        static readonly FrameTiming[] timing = new FrameTiming[1];
        static readonly List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();

        /// <summary>Where the last value came from, for logs and reports: "FrameTimingManager", "XR display" or "none".</summary>
        public static string Source { get; private set; } = "none";

        public static double LastFrameMs()
        {
            FrameTimingManager.CaptureFrameTimings();
            if (FrameTimingManager.GetLatestTimings(1, timing) > 0 && timing[0].gpuFrameTime > 0)
            {
                Source = "FrameTimingManager";
                return timing[0].gpuFrameTime;
            }
            SubsystemManager.GetSubsystems(displays);
            if (displays.Count > 0 && displays[0].running && displays[0].TryGetAppGPUTimeLastFrame(out float value) && value > 0f)
            {
                Source = "XR display";
                return value < 1f ? value * 1000.0 : value; // documented in seconds; a runtime that reports milliseconds is caught too
            }
            Source = "none";
            return 0.0;
        }
    }
}
