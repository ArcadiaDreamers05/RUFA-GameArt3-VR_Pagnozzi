using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// A script that makes garbage every frame, as code does when it builds strings or lists in Update: memory the garbage
    /// collector frees later, in a frame that then takes much longer. In the Profiler: the GC Alloc column of
    /// GarbageMaker.Update, and the GC.Collect spikes.
    /// </summary>
    public sealed class GarbageMaker : MonoBehaviour
    {
        [Tooltip("How much memory it throws away every frame, in kilobytes.")]
        [Range(0, 2048)] public int kilobytes = 200;

        byte[] last;

        void Update() => Make();

        public void Make() => last = new byte[kilobytes * 1024];
    }
}
