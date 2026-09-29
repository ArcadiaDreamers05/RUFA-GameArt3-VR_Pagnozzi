using UnityEngine;
using UnityEngine.Rendering;

namespace Rufa.Demo
{
    /// <summary>
    /// Teacher menu entry: SRP Batcher on/off at runtime, visible immediately as SetPass calls in the Profiler.
    /// The URP asset's checkbox is read only when the pipeline starts; while the app runs the switch is in GraphicsSettings.
    /// </summary>
    public sealed class DemoBatchingSwitch : MonoBehaviour
    {
        void Awake() => TeacherMenu.Register("SRP Batcher",
            () => GraphicsSettings.useScriptableRenderPipelineBatching,
            on => GraphicsSettings.useScriptableRenderPipelineBatching = on);
    }
}
