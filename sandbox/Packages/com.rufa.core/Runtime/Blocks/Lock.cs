using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Rufa
{
    /// <summary>A socket that accepts the key (by interaction layer) and fires onUnlocked once. Wire onUnlocked to Door.Open.</summary>
    [RequireComponent(typeof(XRSocketInteractor))]
    public sealed class Lock : MonoBehaviour
    {
        public UnityEvent onUnlocked = new UnityEvent();
        public bool IsUnlocked { get; private set; }

        void Awake() => GetComponent<XRSocketInteractor>().selectEntered.AddListener(_ => Unlock());

        void Unlock()
        {
            if (IsUnlocked) return;
            IsUnlocked = true;
            onUnlocked.Invoke();
        }
    }
}
