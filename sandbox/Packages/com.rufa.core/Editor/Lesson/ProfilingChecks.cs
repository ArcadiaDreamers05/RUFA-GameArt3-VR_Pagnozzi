using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Rufa.Editor
{
    /// <summary>
    /// What the lesson 5 tutorial checks and does around measuring: lights without shadows, the culprit of a room switched
    /// off and nothing else, the files a measurement leaves, the measurements pulled from the headset with the adb that
    /// comes with Unity's Android module, and the report.
    /// </summary>
    public static class ProfilingChecks
    {
        /// <summary>At least one light with that name in the scene, and none of them casts shadows.</summary>
        public static bool ShadowlessLights(string name)
        {
            var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Where(l => l.name == name).ToArray();
            return lights.Length > 0 && lights.All(l => l.shadows == LightShadows.None);
        }

        /// <summary>
        /// Every open Game view has its own VSync off (the toggle in its aspect ratio menu): with it on, the editor waits for
        /// the monitor every frame, and on a 60 Hz screen no frame gets under 16.7 ms. Unity keeps the Game view internal,
        /// so its flag is read by reflection; the course stays on Unity 6000.3.
        /// </summary>
        public static bool GameViewVsyncOff() => GameViewVsyncOff(Resources.FindObjectsOfTypeAll(GameView));

        /// <summary>The same check on the given Game views: with none, nothing is measured, so it does not pass.</summary>
        public static bool GameViewVsyncOff(Object[] views) => views.Length > 0 && views.All(v => !(bool)GameView.GetProperty("vSyncEnabled").GetValue(v));

        static readonly System.Type GameView = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");

        /// <summary>
        /// The culprit is switched off and everything else in the room is on, components included: switching the whole
        /// room off does not find the culprit. It is looked for among the room's children, where a switched-off object is
        /// still found (GameObject.Find sees only active objects).
        /// </summary>
        public static bool CulpritOff(string culprit, string room)
        {
            var root = GameObject.Find(room);
            var found = root == null ? null : root.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == culprit);
            if (found == null || found.gameObject.activeSelf) return false;
            return AllOn(root.transform, found);
        }

        /// <summary>Everything under the root is on, components included, except the given branch.</summary>
        public static bool AllOn(Transform root, Transform except = null) =>
            root.GetComponentsInChildren<Transform>(true).Where(t => except == null || !t.IsChildOf(except))
                .All(t => t.gameObject.activeSelf && t.GetComponents<Component>().All(On));

        static bool On(Component c) => c is Behaviour b ? b.enabled : c is Renderer r ? r.enabled : !(c is Collider collider) || collider.enabled;

        /// <summary>Every pattern matches at least one file. A pattern is a path from the repository, with * allowed in the file name.</summary>
        public static bool FilesExist(string[] patterns) => patterns.All(pattern =>
        {
            var full = Path.Combine(LevelChecks.Repository, pattern);   // an absolute path stays as it is
            var folder = Path.GetDirectoryName(full);
            return Directory.Exists(folder) && Directory.GetFiles(folder, Path.GetFileName(full)).Length > 0;
        });

        /// <summary>Where the measurements are on the headset: the app's files (the benchmark's CSVs) and OVR Metrics' recordings.</summary>
        public static string[] HeadsetFolders(string identifier) => new[]
        {
            $"/sdcard/Android/data/{identifier}/files/",
            "/sdcard/Android/data/com.oculus.ovrmonitormetricsservice/files/CapturedMetrics/",
        };

        /// <summary>The adb of Unity's Android module, or the one on the PATH when the module keeps none.</summary>
        public static string Adb
        {
            get
            {
                var bundled = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines", "AndroidPlayer", "SDK", "platform-tools",
                                           Application.platform == RuntimePlatform.WindowsEditor ? "adb.exe" : "adb");
                return File.Exists(bundled) ? bundled : "adb";
            }
        }

        /// <summary>
        /// Copies the benchmark's CSVs from the headset into the folder (a path from the repository) and OVR Metrics' into
        /// its "OVR Metrics" subfolder: only those written after the student's Quest build, because a headset shared in
        /// turns keeps everybody's recordings. The Console says what came, or why nothing did. True when something was copied.
        /// </summary>
        public static bool PullMeasures(string folder, string adb)
        {
            var target = Path.Combine(LevelChecks.Repository, folder);
            var apk = Path.GetFullPath($"Builds/Quest/{PlayerSettings.productName.Replace(' ', '_')}.apk");
            var since = File.Exists(apk) ? File.GetLastWriteTime(apk) : System.DateTime.MinValue;
            if (!File.Exists(apk)) Debug.LogWarning($"RUFA: no build at {apk}: every measurement on the headset is copied, other turns' too.");
            var sources = HeadsetFolders(PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android));
            int benchmark = Pull(adb, sources[0], target, "benchmark-*.csv", since), metrics = Pull(adb, sources[1], Path.Combine(target, "OVR Metrics"), "*.csv", since);
            int copied = System.Math.Max(0, benchmark) + System.Math.Max(0, metrics);   // -1: that pull failed, and said why
            if (copied > 0) Debug.Log($"RUFA: {System.Math.Max(0, benchmark)} benchmark CSV and {System.Math.Max(0, metrics)} OVR Metrics CSV copied into {target}; the newest is the run that counts.");
            else if (benchmark == 0 && metrics == 0) Debug.LogWarning($"RUFA: nothing on the headset is newer than your build of {since:HH:mm}: run the app again and measure.");
            return copied > 0;
        }

        /// <summary>The files of a folder and its subfolders that match and were written at that moment or later, copied into the target.</summary>
        public static int CopyNewer(string from, string target, string pattern, System.DateTime since)
        {
            var files = Directory.GetFiles(from, pattern, SearchOption.AllDirectories).Where(f => File.GetLastWriteTime(f) >= since).ToArray();
            if (files.Length > 0) Directory.CreateDirectory(target);
            foreach (var file in files) File.Copy(file, Path.Combine(target, Path.GetFileName(file)), true);
            return files.Length;
        }

        // adb pull of a folder of the headset into a temporary folder, with the headset's dates (-a), then the newer matching
        // files into the target; -1 when adb failed.
        static int Pull(string adb, string remote, string target, string pattern, System.DateTime since)
        {
            var temp = Path.Combine(Path.GetTempPath(), "rufa-misure-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temp);
            try
            {
                var error = Run(adb, $"pull -a \"{remote}\" \"{temp}\"");
                if (error != null) { Debug.LogWarning($"RUFA: could not pull {remote}: {error}"); return -1; }
                return CopyNewer(temp, target, pattern, since);
            }
            finally { Directory.Delete(temp, true); }
        }

        // Runs adb, waiting at most a minute: null when it worked, otherwise what went wrong.
        static string Run(string adb, string arguments)
        {
            try
            {
                using (var process = Process.Start(new ProcessStartInfo(adb, arguments) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true }))
                {
                    var output = process.StandardOutput.ReadToEndAsync();
                    var error = process.StandardError.ReadToEndAsync();
                    if (!process.WaitForExit(60000)) { process.Kill(); return "adb did not answer within a minute"; }
                    return process.ExitCode == 0 ? null : (error.Result + output.Result).Trim();
                }
            }
            catch (System.ComponentModel.Win32Exception) { return "adb not found: install the Android module of Unity"; }
        }

        /// <summary>The profiling report from its template, like the concept of lesson 4: written once, never over the student's text.</summary>
        public static string CreateReport(string path) => LevelChecks.FromTemplate(path, ReportTemplate);

        const string ReportTemplate =
@"# Report di profiling

Al posto di ogni segnaposto tra « » scrivi il tuo testo. I CSV del benchmark stanno in questa cartella, quelli di OVR Metrics nella sottocartella OVR Metrics.

## Piattaforma e build
Visore, build (normale o di sviluppo) e frequenza dello schermo: «da scrivere»
Il file CSV del benchmark sul visore: «da scrivere»

## Le condizioni della misura in editor
Game view davanti, VSync (Game view only) spento, Record del Profiler spento: «da scrivere»

## Il benchmark, stazione per stazione
Copia dal CSV mean_ms, p1_low_ms, fps_mean, cpu_mean_ms e gpu_mean_ms, del PC e del visore.
Stazione 1, dove: «da scrivere»
Stazione 2, dove: «da scrivere»
Stazione 3, dove: «da scrivere»

## OVR Metrics, alla stazione più pesante
Il file CSV di OVR Metrics: «da scrivere»
Average FPS: «da scrivere»
App GPU Time: «da scrivere»
CPU Utilization e GPU Utilization: «da scrivere»
CPU level e GPU level: «da scrivere»

## Chi sfora e come lo sai
I tempi di Display Stats (Frame Time, CPU Main Thread Frame, CPU Render Thread Frame, GPU Frame): «da scrivere»
Chi aspetta chi, nella Timeline del Profiler: «da scrivere»
Sul visore, App GPU Time contro gli 11 111 µs del frame: «da scrivere»
Quindi sfora: «da scrivere»

## I due test di Meta sul tuo blockout
Frame Time con la camera accesa e con la camera spenta: «da scrivere»
Frame Time e GPU Frame con la Game view normale e con Pochi pixel (160x90): «da scrivere»
Cosa ne deduci: «da scrivere»

## Project Auditor: le tre segnalazioni più gravi del tuo progetto
Segnalazione 1: «da scrivere»
Segnalazione 2: «da scrivere»
Segnalazione 3: «da scrivere»

## Memory Profiler: i tre oggetti più pesanti del tuo progetto
Oggetto 1, col suo tipo: «da scrivere»
Oggetto 2, col suo tipo: «da scrivere»
Oggetto 3, col suo tipo: «da scrivere»

## La diagnosi
Causa 1, con lo strumento che la mostra: «da scrivere»
Causa 2, con lo strumento che la mostra: «da scrivere»
Causa 3, con lo strumento che la mostra: «da scrivere»
Cosa curerai, e in quale lezione: «da scrivere»
";
    }
}
