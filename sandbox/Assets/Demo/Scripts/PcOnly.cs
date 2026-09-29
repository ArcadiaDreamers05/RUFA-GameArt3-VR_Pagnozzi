using UnityEngine;

namespace Rufa.Demo
{
    /// <summary>Switches its object off on the headset: the VFX Graph comparison of lesson 11 lives in the PC build and in the editor.</summary>
    public sealed class PcOnly : MonoBehaviour
    {
        // A runtime check, not #if UNITY_ANDROID: with Android as the active platform the editor would hide it too.
        void Awake()
        {
            if (Application.platform == RuntimePlatform.Android) gameObject.SetActive(false);
        }
    }
}
