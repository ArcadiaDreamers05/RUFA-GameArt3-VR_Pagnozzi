using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Rufa
{
    /// <summary>
    /// Dynamic resolution the simple way: watch the GPU frame time and move the URP render scale
    /// between minScale and maxScale. Enable/disable the component to switch it on and off.
    /// </summary>
    public sealed class AdaptiveRenderScale : MonoBehaviour
    {
        public float upperMs = 10f;    // above this the scale goes down
        public float lowerMs = 7.5f;   // below this the scale goes back up
        public float step = 0.05f;
        public float minScale = 0.7f;
        public float maxScale = 1f;
        public float interval = 0.5f;  // seconds between adjustments

        float nextCheck;
        bool warned;
        double sum;   // GPU milliseconds of the frames since the last adjustment
        int samples;

        UniversalRenderPipelineAsset asset;  // the one it adjusts, read at OnEnable: on Stop another may already be current

        void OnEnable()
        {
            asset = UniversalRenderPipeline.asset;
            nextCheck = Time.unscaledTime + interval;
        }

        void OnDisable()
        {
            if (asset != null) asset.renderScale = maxScale;
        }

        void Update()
        {
            double frame = GpuTime.LastFrameMs();
            if (frame > 0) { sum += frame; samples++; }
            if (Time.unscaledTime < nextCheck) return;
            nextCheck = Time.unscaledTime + interval;

            if (samples == 0)
            {
                if (!warned) Debug.LogWarning("AdaptiveRenderScale: no GPU timings on this platform; check 'Frame Timing Stats' in Player settings.");
                warned = true;
                return;
            }
            double gpu = sum / samples;
            sum = 0; samples = 0;

            if (asset == null) return;
            if (gpu > upperMs) asset.renderScale = Mathf.Max(minScale, asset.renderScale - step);
            else if (gpu < lowerMs) asset.renderScale = Mathf.Min(maxScale, asset.renderScale + step);
        }
    }
}
