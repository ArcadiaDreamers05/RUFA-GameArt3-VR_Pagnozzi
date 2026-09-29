using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Rufa.Demo
{
    /// <summary>
    /// The teacher's switchboard: a world-space panel on the left wrist (VR) or in front of the camera on F1 (PC).
    /// Defect scripts call Register in Awake; the panel is built once in Start.
    /// </summary>
    public sealed class TeacherMenu : MonoBehaviour
    {
        struct Entry { public string label; public Func<bool> get; public Action<bool> set; public TMP_Text text; }
        static readonly List<Entry> entries = new List<Entry>();

        public static void Register(string label, Func<bool> get, Action<bool> set) =>
            entries.Add(new Entry { label = label, get = get, set = set });

        public static TeacherMenu Instance { get; private set; }

        const string StudentsDemo = "it.rufa.gameart3.demo"; // the identifier of RUFA ▸ Build ▸ Demo (APK)

        Canvas canvas;

        void Start()
        {
            // The switchboard is the teacher's: it exists only where Assets/Demo/Docente/Resources/Docente.txt does,
            // and that folder never reaches the students' copy. The students' demo app never shows it, even when the teacher builds it.
            if (Resources.Load<TextAsset>("Docente") == null || Application.identifier == StudentsDemo) { gameObject.SetActive(false); return; }
            Instance = this;
            EnsureEventSystem();
            BuildPanel();
            Attach();
        }

        void Update()
        {
            if (RigBootstrap.XrIsActive || Keyboard.current == null) return;
            if (Keyboard.current.f1Key.wasPressedThisFrame) Show(!canvas.gameObject.activeSelf);
            else if (canvas.gameObject.activeSelf && Cursor.lockState == CursorLockMode.Locked) canvas.gameObject.SetActive(false);
        }

        void Show(bool show)
        {
            canvas.gameObject.SetActive(show);
            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = show;
        }

        static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));
            eventSystem.GetComponent<XRUIInputModule>().enableMouseInput = true;
        }

        void BuildPanel()
        {
            canvas = new GameObject("Teacher Menu Canvas", typeof(Canvas), typeof(TrackedDeviceGraphicRaycaster)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 40f * entries.Count + 20f);
            canvas.transform.localScale = Vector3.one * 0.001f;

            var background = new GameObject("Background", typeof(Image), typeof(VerticalLayoutGroup));
            background.transform.SetParent(canvas.transform, false);
            var rect = background.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            background.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
            var layout = background.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 4f;
            layout.childForceExpandHeight = false;

            for (int i = 0; i < entries.Count; i++) AddButton(background.transform, i);
        }

        void AddButton(Transform parent, int index)
        {
            var button = new GameObject(entries[index].label, typeof(Image), typeof(Button), typeof(LayoutElement));
            button.transform.SetParent(parent, false);
            button.GetComponent<LayoutElement>().preferredHeight = 36f;
            button.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 1f);

            var label = new GameObject("Label", typeof(TextMeshProUGUI));
            label.transform.SetParent(button.transform, false);
            var rect = label.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            var text = label.GetComponent<TextMeshProUGUI>();
            text.fontSize = 18f;
            text.alignment = TextAlignmentOptions.Center;

            var entry = entries[index];
            entry.text = text;
            entries[index] = entry;
            Refresh(index);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                var e = entries[index];
                e.set(!e.get());
                Refresh(index);
            });
        }

        void Refresh(int index) => entries[index].text.text = $"{entries[index].label}: {(entries[index].get() ? "ON" : "OFF")}";

        void Attach()
        {
            if (RigBootstrap.XrIsActive)
            {
                var wrist = GameObject.Find("Left Controller");
                if (wrist == null) { Debug.LogWarning("TeacherMenu: 'Left Controller' not found, panel left at the origin."); return; }
                canvas.transform.SetParent(wrist.transform, false);
                canvas.transform.localPosition = new Vector3(0f, 0.08f, -0.05f);
                canvas.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
                return;
            }
            canvas.gameObject.AddComponent<GraphicRaycaster>(); // the free mouse cursor clicks through this one; the VR ray uses the tracked-device one
            var camera = Camera.main;
            if (camera != null)
            {
                canvas.transform.SetParent(camera.transform, false);
                canvas.transform.localPosition = new Vector3(0f, 0f, 0.8f);
            }
            canvas.gameObject.SetActive(false); // F1 shows it
        }

        /// <summary>Lesson 12: false leaves the panel fixed in the world, 3 m ahead of the spawn point, above the head, at half scale
        /// (the "badly placed menu" defect); true puts it back on the wrist. Headset only: on PC it stays where F1 shows it.</summary>
        public void AttachToWrist(bool onWrist)
        {
            if (!RigBootstrap.XrIsActive) return;
            if (onWrist) { canvas.transform.localScale = Vector3.one * 0.001f; Attach(); return; }
            var spawn = GameObject.Find("Rig Bootstrap");
            canvas.transform.SetParent(null, false);
            // 1.95 m: awkward, yet under the ceiling of either house (2.2 m before lesson 4, 2.8 m after), so the teacher can still reach it
            canvas.transform.SetPositionAndRotation((spawn != null ? spawn.transform.position : Vector3.zero) + new Vector3(0f, 1.95f, 3f), Quaternion.identity);
            canvas.transform.localScale = Vector3.one * 0.0005f;
        }
    }
}
