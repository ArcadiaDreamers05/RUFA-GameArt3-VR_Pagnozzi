using UnityEngine;

namespace Rufa.Demo
{
    /// <summary>Teacher menu entry that starts the benchmark; it reads OFF again when the run is over.</summary>
    public sealed class DemoBenchmarkSwitch : MonoBehaviour
    {
        void Awake() => TeacherMenu.Register("Benchmark",
            () => { var b = FindFirstObjectByType<Benchmark>(); return b != null && b.IsRunning; },
            on => { var b = FindFirstObjectByType<Benchmark>(); if (on && b != null) b.Run(); });
    }
}
