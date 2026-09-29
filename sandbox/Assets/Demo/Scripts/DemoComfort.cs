using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rufa.Demo
{
    /// <summary>Lesson 12 defects as switches: 2D audio, no haptics, no vignette, badly placed menu.</summary>
    public sealed class DemoComfort : MonoBehaviour
    {
        public bool audio2d = true;
        public bool haptics = false;
        public bool vignette = false;
        public bool menuOnWrist = false;

        void Awake()
        {
            TeacherMenu.Register("Audio 2D", () => audio2d, v => { audio2d = v; ApplyAudio(); });
            TeacherMenu.Register("Haptics", () => haptics, v => { haptics = v; ApplyHaptics(); });
            TeacherMenu.Register("Vignetta", () => vignette, v => { vignette = v; ApplyVignette(); });
            TeacherMenu.Register("Menu sul polso", () => menuOnWrist, v => { menuOnWrist = v; ApplyMenu(); });
        }

        void Start()
        {
            // The crackles share one clip and start together: out of step they sound like six flames, not one loud one.
            foreach (var source in Crackles()) source.time = Random.Range(0f, source.clip != null ? source.clip.length : 0f);
            ApplyAudio();
            ApplyHaptics();
            ApplyVignette();
            Invoke(nameof(ApplyMenu), 0.1f); // after TeacherMenu.Start has built the panel
        }

        void ApplyAudio()
        {
            foreach (var source in Crackles()) source.spatialBlend = audio2d ? 0f : 1f;
        }

        static IEnumerable<AudioSource> Crackles() => FindObjectsByType<AudioSource>(FindObjectsSortMode.None).Where(s => s.name.StartsWith("Crepitio_"));

        void ApplyHaptics() { var h = FindFirstObjectByType<ComfortHaptics>(FindObjectsInactive.Include); if (h != null) h.enabled = haptics; }
        void ApplyVignette() { var v = FindFirstObjectByType<ComfortVignette>(FindObjectsInactive.Include); if (v != null) v.enabled = vignette; }
        void ApplyMenu() { if (TeacherMenu.Instance != null) TeacherMenu.Instance.AttachToWrist(menuOnWrist); }
    }
}
