using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Rufa
{
    /// <summary>
    /// A hinged door. The leaf is a child whose pivot sits on the hinge; Open/Close rotate it over a short time.
    /// With openOnSelect, selecting the leaf (VR hand or desktop click) toggles the door.
    /// </summary>
    public sealed class Door : MonoBehaviour
    {
        [Tooltip("Child with its pivot on the hinge. Rotated around its local Y axis.")]
        public Transform leaf;
        public float openAngle = 90f;
        public float duration = 0.6f;
        public bool openOnSelect = true;

        public bool IsOpen { get; private set; }
        Coroutine motion;

        void Awake()
        {
            if (openOnSelect && leaf != null && leaf.TryGetComponent<XRSimpleInteractable>(out var interactable))
                interactable.selectEntered.AddListener(_ => Toggle());
        }

        public void Open() { if (!IsOpen) RotateTo(openAngle); }
        public void Close() { if (IsOpen) RotateTo(0f); }
        public void Toggle() { if (IsOpen) Close(); else Open(); }

        void RotateTo(float angle)
        {
            IsOpen = angle != 0f;
            if (motion != null) StopCoroutine(motion);
            motion = StartCoroutine(Swing(Quaternion.Euler(0f, angle, 0f)));
        }

        IEnumerator Swing(Quaternion target)
        {
            var from = leaf.localRotation;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                leaf.localRotation = Quaternion.Slerp(from, target, t);
                yield return null;
            }
            leaf.localRotation = target;
        }
    }
}
