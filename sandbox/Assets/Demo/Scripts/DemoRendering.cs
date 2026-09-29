using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rufa.Demo
{
    /// <summary>Lesson 10: the rendering features as live switches. All start in the healthy position; the teacher flips them one by one.</summary>
    public sealed class DemoRendering : MonoBehaviour
    {
        static UniversalRenderPipelineAsset Asset => UniversalRenderPipeline.asset;

        void Awake()
        {
            Remember();
            TeacherMenu.Register("MSAA 4x", () => Asset != null && Asset.msaaSampleCount == 4, on => { if (Asset != null) Asset.msaaSampleCount = on ? 4 : 1; });
            TeacherMenu.Register("Render scale 1.5", () => Asset != null && Asset.renderScale > 1.25f, on =>
            {
                var adaptive = FindFirstObjectByType<AdaptiveRenderScale>(FindObjectsInactive.Include);
                if (on && adaptive != null) adaptive.enabled = false;   // the two would fight each other
                if (Asset != null) Asset.renderScale = on ? 1.5f : 1f;
            });
            TeacherMenu.Register("FFR alto", () => Foveation.GetLevel() > 0.75f, on => Foveation.SetLevel(on ? 1f : 0f));
            TeacherMenu.Register("Post-processing", () => Defects() != null && Defects().postProcessing, on =>
            {
                var defects = Defects();
                if (defects != null) { defects.postProcessing = on; defects.Apply(); }
                var volume = GameObject.Find("Post-processing (malato)");
                if (volume != null && volume.TryGetComponent<Volume>(out var v)) v.enabled = on;
            });
            TeacherMenu.Register("HDR", () => Asset != null && Asset.supportsHDR, on => { if (Asset != null) Asset.supportsHDR = on; });
            TeacherMenu.Register("Risoluzione dinamica",
                () => { var a = FindFirstObjectByType<AdaptiveRenderScale>(FindObjectsInactive.Include); return a != null && a.enabled; },
                on => { var a = FindFirstObjectByType<AdaptiveRenderScale>(FindObjectsInactive.Include); if (a != null) a.enabled = on; });
        }

        void Start() => Foveation.SetLevel(1f); // healthy default on the headset

        static DemoDefects Defects() => FindFirstObjectByType<DemoDefects>();

        // In the editor the switches write into the URP asset, and an asset changed during Play stays changed:
        // put the values back when Play ends. In a build this changes nothing.
        int msaa; float scale; bool hdr;
        void Remember() { if (Asset != null) (msaa, scale, hdr) = (Asset.msaaSampleCount, Asset.renderScale, Asset.supportsHDR); }
        void OnDestroy()
        {
            if (Application.isEditor && Asset != null) (Asset.msaaSampleCount, Asset.renderScale, Asset.supportsHDR) = (msaa, scale, hdr);
        }
    }
}
