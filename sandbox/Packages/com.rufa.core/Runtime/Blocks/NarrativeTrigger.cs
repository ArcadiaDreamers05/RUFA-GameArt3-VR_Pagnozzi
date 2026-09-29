using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Rufa
{
    /// <summary>
    /// A volume that reacts when the player walks in: plays a sound, shows a line of text on a panel, fires an event.
    /// Both rigs move with a CharacterController, so that is how the player is recognised.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class NarrativeTrigger : MonoBehaviour
    {
        [TextArea] public string text;
        public TMP_Text panel;
        public AudioSource audioSource;
        public float showSeconds = 5f;
        public bool once = true;
        public UnityEvent onEntered = new UnityEvent();

        bool fired;

        void OnTriggerEnter(Collider other)
        {
            if (once && fired) return;
            if (other.GetComponent<CharacterController>() == null) return;
            fired = true;
            if (audioSource != null && audioSource.clip != null) audioSource.Play();
            if (panel != null)
            {
                panel.text = text;
                StartCoroutine(Hide());
            }
            onEntered.Invoke();
        }

        IEnumerator Hide()
        {
            yield return new WaitForSeconds(showSeconds);
            if (panel != null) panel.text = string.Empty;
        }
    }
}
