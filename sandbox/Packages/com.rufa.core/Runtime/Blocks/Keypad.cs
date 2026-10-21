using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Rufa
{
    /// <summary>
    /// A code lock: the buttons of a panel call Press with their digit, and the right code fires onUnlocked once.
    /// Only the last digits count, as on a real keypad: typing 91234 opens a 1234 lock.
    /// </summary>
    public sealed class Keypad : MonoBehaviour
    {
        public string code = "1234";
        [Tooltip("Optional: shows the digits typed so far.")]
        public TMP_Text display;
        public UnityEvent onUnlocked = new UnityEvent();
        public bool IsUnlocked { get; private set; }
        string typed = "";

        public void Press(string digit)
        {
            if (IsUnlocked) return;
            typed += digit;
            if (typed.Length > code.Length) typed = typed.Substring(typed.Length - code.Length);
            if (display != null) display.text = typed;
            if (typed != code) return;
            IsUnlocked = true;
            onUnlocked.Invoke();
        }
    }
}
