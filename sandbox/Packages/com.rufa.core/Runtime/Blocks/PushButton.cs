using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Rufa
{
    /// <summary>A button: selecting it (poke, grab or desktop click) fires onPressed and dips the cap for a moment.</summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class PushButton : MonoBehaviour
    {
        public Transform cap;
        public float travel = 0.01f;
        public UnityEvent onPressed = new UnityEvent();

        Vector3 rest; // taken once: a second press during the dip must not make the dipped position the new rest

        void Awake()
        {
            GetComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => Press());
            if (cap != null) rest = cap.localPosition;
        }

        void Press()
        {
            onPressed.Invoke();
            if (cap != null) StartCoroutine(Dip());
        }

        // In world metres: the base is flattened by its scale, so a local offset would move the cap a tenth of a millimetre.
        IEnumerator Dip()
        {
            cap.localPosition = rest;
            cap.position -= transform.up * travel;
            yield return new WaitForSeconds(0.15f);
            cap.localPosition = rest;
        }
    }
}
