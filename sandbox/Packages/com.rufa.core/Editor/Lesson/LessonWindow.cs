using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>
    /// The lesson tutorial inside the editor (RUFA > Tutorial). Chapters done together go one page at a time, with the
    /// field being explained highlighted in its window, and pages with something to do let you go on only when the
    /// project is really in the right state: the teacher uses it on the projector, the students at their desks.
    /// "Da soli" chapters show all their tasks as one list, to do in any order, with the score; the tasks tick themselves off as the student works.
    /// </summary>
    public sealed class LessonWindow : EditorWindow
    {
        const string Folder = "Packages/com.rufa.core/Tutorials";

        // Serialized, so entering and leaving Play (a domain reload) keeps the place, the pages done and the last check.
        [SerializeField] string lessonPath;
        [SerializeField] int pageCount;  // of the lesson the saved progress belongs to: a rewritten lesson starts over
        [SerializeField] int index;
        [SerializeField] List<int> done = new List<int>();      // pages done, and list tasks done in Play: for good
        [SerializeField] List<int> verified = new List<int>();  // list tasks that do not need Play and passed at the last check of their chapter
        [SerializeField] int fontSize = 16;

        List<LessonPage> pages;
        Vector2 scroll;
        double nextCheck, nextVerify;
        int highlightTries; // the field may show up a moment after the page or the selection changes; Highlighter warns at each miss
        UnityEngine.Object lastSelection;

        [MenuItem("RUFA/Tutorial")]
        static void OpenWindow() => GetWindow<LessonWindow>("Tutorial");

        public static void Open(string path)
        {
            var window = GetWindow<LessonWindow>("Tutorial");
            window.lessonPath = path;
            window.done.Clear();
            window.verified.Clear();
            window.pages = Load(path);
            window.pageCount = window.pages == null ? 0 : window.pages.Count;
            if (window.pages != null && window.pages.Count > 0) window.GoTo(0);
        }

        void OnEnable()
        {
            pages = Load(lessonPath);
            // The lesson changed since the window saved its progress: the pages would not match.
            if (pages != null && pages.Count != pageCount) { done.Clear(); verified.Clear(); index = 0; pageCount = pages.Count; }
            EditorApplication.update += Tick;
        }

        void OnDisable()
        {
            EditorApplication.update -= Tick;
            Highlighter.Stop();
        }

        static List<LessonPage> Load(string path)
        {
            var asset = string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            return asset == null ? null : Lesson.Parse(asset.text);
        }

        void OnGUI()
        {
            var lessons = AssetDatabase.FindAssets("t:TextAsset", new[] { Folder }).Select(AssetDatabase.GUIDToAssetPath).Where(p => p.EndsWith(".md")).OrderBy(p => p).ToArray();
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                int chosen = EditorGUILayout.Popup(Array.IndexOf(lessons, lessonPath), lessons.Select(Path.GetFileNameWithoutExtension).ToArray(), EditorStyles.toolbarPopup, GUILayout.Width(260f));
                if (chosen >= 0 && lessons[chosen] != lessonPath) Open(lessons[chosen]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("A−", EditorStyles.toolbarButton)) fontSize = Mathf.Max(11, fontSize - 2);
                if (GUILayout.Button("A+", EditorStyles.toolbarButton)) fontSize = Mathf.Min(40, fontSize + 2);
            }
            if (pages == null || pages.Count == 0) { EditorGUILayout.HelpBox("Scegli la lezione dal menu in alto a sinistra.", MessageType.Info); return; }

            index = Mathf.Clamp(index, 0, pages.Count - 1);
            var page = pages[index];
            var chapters = pages.Select(p => p.chapter).Distinct().ToArray();
            int chapter = EditorGUILayout.Popup(Array.IndexOf(chapters, page.chapter), chapters);
            if (chapters[chapter] != page.chapter) GoTo(pages.FindIndex(p => p.chapter == chapters[chapter]));

            var text = new GUIStyle(EditorStyles.label) { wordWrap = true, richText = true, fontSize = fontSize };
            if (page.alone) { DrawList(page.chapter, text); return; }

            GUILayout.Label(page.title, new GUIStyle(text) { fontSize = fontSize + 6, fontStyle = FontStyle.Bold });
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUILayout.Label(Rich(page.body), text);
            EditorGUILayout.EndScrollView();

            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                if (page.action != null && GUILayout.Button(page.actionLabel, GUILayout.Height(fontSize * 2f)))
                    LessonChecks.Run(page.action, page.actionArgs);
            bool ok = page.check == null || done.Contains(index);
            if (page.check != null)
                GUILayout.Label(ok ? "<color=#4caf50><b>Fatto</b></color>" : "<color=#ffb300><b>Da fare</b></color>", text);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(index == 0))
                    if (GUILayout.Button("‹ Indietro", GUILayout.Height(fontSize * 2f))) GoTo(index - 1);
                GUILayout.Label($"{index + 1} / {pages.Count}", new GUIStyle(text) { alignment = TextAnchor.MiddleCenter });
                using (new EditorGUI.DisabledScope(!ok || index == pages.Count - 1))
                    if (GUILayout.Button("Avanti ›", GUILayout.Height(fontSize * 2f))) GoTo(index + 1);
            }
        }

        // A "Da soli" chapter: every task at once, in any order, with the score of the last check.
        void DrawList(string chapter, GUIStyle text)
        {
            var tasks = Enumerable.Range(0, pages.Count).Where(i => pages[i].chapter == chapter).ToList();
            // A task whose title starts with "Extra" is a bonus: out of the total, shown in the score once done.
            var scored = tasks.Where(i => pages[i].check != null && !pages[i].title.StartsWith("Extra")).ToList();
            var extra = tasks.Count(i => pages[i].check != null && pages[i].title.StartsWith("Extra") && Passed(i));
            if (scored.Count > 0)
                GUILayout.Label($"<b>{scored.Count(Passed)} / {scored.Count}</b>" + (extra > 0 ? $"  <b>+{extra} extra</b>" : ""),
                    new GUIStyle(text) { fontSize = fontSize + 4 });
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var i in tasks)
            {
                var page = pages[i];
                GUILayout.Label(page.title, new GUIStyle(text) { fontSize = fontSize + 4, fontStyle = FontStyle.Bold });
                if (page.body.Length > 0) GUILayout.Label(Rich(page.body), text);
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                    if (page.action != null && GUILayout.Button(page.actionLabel, GUILayout.Height(fontSize * 2f)))
                        LessonChecks.Run(page.action, page.actionArgs);
                if (page.check != null) GUILayout.Label(Status(i), text);
                GUILayout.Space(fontSize);
            }
            EditorGUILayout.EndScrollView();
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(tasks[0] == 0))
                    if (GUILayout.Button("‹ Capitolo prima", GUILayout.Height(fontSize * 2f))) GoTo(tasks[0] - 1);
                using (new EditorGUI.DisabledScope(tasks[tasks.Count - 1] == pages.Count - 1))
                    if (GUILayout.Button("Capitolo dopo ›", GUILayout.Height(fontSize * 2f))) GoTo(tasks[tasks.Count - 1] + 1);
            }
        }

        bool Passed(int i) => LessonChecks.PlayOnly.Contains(pages[i].check) ? done.Contains(i) : verified.Contains(i);

        string Status(int i) =>
            Passed(i) ? "<color=#4caf50><b>✓ Fatto</b></color>"
            : LessonChecks.PlayOnly.Contains(pages[i].check) ? "<color=#ffb300><b>Da fare in Play</b></color>"
            : "<color=#e57373><b>✗ Non ancora</b></color>";

        /// <summary>Checks the tasks of the current list that do not need Play, on the project as it is now. The other lists keep their last result.</summary>
        public void Verify()
        {
            var chapter = pages[Mathf.Clamp(index, 0, pages.Count - 1)].chapter;
            verified.RemoveAll(i => i >= pages.Count || pages[i].chapter == chapter);
            verified.AddRange(Enumerable.Range(0, pages.Count)
                .Where(i => pages[i].chapter == chapter && pages[i].check != null && !LessonChecks.PlayOnly.Contains(pages[i].check)
                            && LessonChecks.Passes(pages[i].check, pages[i].checkArgs)));
            Repaint();
        }

        // Twice a second, from EditorApplication.update: it runs even when the tab is hidden behind another one.
        // Pages done together: a few attempts at the highlight after the page or the selection changes,
        // and the check of the current page. Lists: out of Play, every 2 seconds, the tasks that do not need Play (a pass
        // costs some 20 ms, because the prefab checks search the project); in Play, the tasks only Play can show, whose
        // first pass is for good.
        void Tick()
        {
            if (pages == null || pages.Count == 0 || EditorApplication.timeSinceStartup < nextCheck) return;
            nextCheck = EditorApplication.timeSinceStartup + 0.5;
            var page = pages[Mathf.Clamp(index, 0, pages.Count - 1)];
            if (page.alone)
            {
                if (!EditorApplication.isPlaying)
                {
                    if (EditorApplication.timeSinceStartup >= nextVerify) { nextVerify = EditorApplication.timeSinceStartup + 2; Verify(); }
                    return;
                }
                foreach (var i in Enumerable.Range(0, pages.Count).Where(i => pages[i].chapter == page.chapter && !done.Contains(i)
                                                                             && LessonChecks.PlayOnly.Contains(pages[i].check ?? "")))
                    if (LessonChecks.Passes(pages[i].check, pages[i].checkArgs)) { done.Add(i); Repaint(); }
                return;
            }
            if (Selection.activeObject != lastSelection) { lastSelection = Selection.activeObject; highlightTries = 3; Highlighter.Stop(); } // the field shows up once its object is selected
            if (highlightTries > 0 && !Highlight(page)) highlightTries--;
            else highlightTries = 0;
            if (page.check == null || done.Contains(index) || !LessonChecks.Passes(page.check, page.checkArgs)) return;
            done.Add(index);
            Repaint();
        }

        void GoTo(int i)
        {
            index = Mathf.Clamp(i, 0, pages.Count - 1);
            scroll = Vector2.zero;
            Highlighter.Stop();
            highlightTries = 3;
        }

        // Highlighter finds the field in IMGUI and UI Toolkit windows alike. True when there is nothing more to try.
        // Highlighter knows labels, not objects: when the page names the field's object, the field lights up only on that object,
        // and until it is selected the first name of its path lights up in the Hierarchy instead.
        static bool Highlight(LessonPage page) =>
            string.IsNullOrEmpty(page.highlightWindow) || Highlighter.active
            || (page.highlightObject == null || Selected(page.highlightObject)
                ? Highlighter.Highlight(page.highlightWindow, page.highlightLabel, HighlightSearchMode.Auto)
                : Highlighter.Highlight("Hierarchy", page.highlightObject.Split('/')[0], HighlightSearchMode.Auto));

        // The path of the selected object ends with the given one: in the scene, in Play (on the "(Clone)") and in Prefab Mode alike.
        static bool Selected(string path)
        {
            var full = "";
            for (var t = Selection.activeTransform; t != null; t = t.parent) full = "/" + t.name + full;
            return full.EndsWith("/" + path);
        }

        // Markdown kept to what a page needs: **bold**, `names` in bold, "- " lists.
        static string Rich(string markdown) =>
            Regex.Replace(Regex.Replace(Regex.Replace(markdown, @"\*\*(.+?)\*\*", "<b>$1</b>"), "`(.+?)`", "<b>$1</b>"), @"(?m)^- ", "• ");
    }
}
