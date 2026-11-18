using System.Diagnostics;
using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// A script that wastes CPU time every frame, the same on every PC: it keeps busy for a fixed number of milliseconds.
    /// In the Profiler it shows as CpuBurner.Update; the Hierarchy search t:CpuBurner finds the object that carries it.
    /// </summary>
    public sealed class CpuBurner : MonoBehaviour
    {
        [Tooltip("How long it keeps the CPU busy every frame, in milliseconds.")]
        [Range(0f, 20f)] public float milliseconds = 4f;

        double sink;   // the result of the busy loop: kept, so that the loop cannot be thrown away

        void Update() => Burn();

        public void Burn()
        {
            var clock = Stopwatch.StartNew();
            while (clock.Elapsed.TotalMilliseconds < milliseconds) sink += System.Math.Sqrt(sink + clock.ElapsedTicks);
        }
    }
}
