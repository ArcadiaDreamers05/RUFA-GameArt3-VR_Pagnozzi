using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace Rufa
{
    /// <summary>
    /// Frame-time measurements at fixed stations, written to a CSV, so "before" and "after" numbers are comparable.
    /// VR: guided, the player walks onto each disc and looks along the arrow (no forced movement).
    /// PC: automatic flythrough of the desktop rig. Start with B, with -benchmark on the command line, or Run().
    /// </summary>
    public sealed class Benchmark : MonoBehaviour
    {
        public Transform[] stations;
        public float sampleSeconds = 10f;
        public float settleSeconds = 2f;
        public float reachRadius = 0.6f;
        public bool startOnLaunch = false;

        public bool IsRunning { get; private set; }
        public event Action Finished;

        const string Header = "station,samples,mean_ms,p1_low_ms,max_ms,fps_mean,cpu_mean_ms,gpu_mean_ms";
        readonly List<float> frames = new List<float>();
        readonly List<double> cpu = new List<double>();
        readonly List<double> gpu = new List<double>();
        readonly FrameTiming[] timing = new FrameTiming[1];
        TMP_Text prompt;

        static bool FromCommandLine => Environment.GetCommandLineArgs().Contains("-benchmark");

        void Awake() => ShowMarkers(false); // discs, arrows and numbers belong to the run, not to the scene a lesson shows

        void Start()
        {
            if (startOnLaunch || FromCommandLine) Run();
        }

        void ShowMarkers(bool show)
        {
            if (stations == null) return;
            foreach (var station in stations)
                if (station != null) foreach (Transform marker in station) marker.gameObject.SetActive(show);
        }

        void Update()
        {
            if (!IsRunning && !RigBootstrap.XrIsActive && Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
                Run();
        }

        public void Run()
        {
            if (IsRunning) return;
            if (stations == null || stations.Length == 0)
            {
                Debug.LogError("Benchmark: no stations assigned. Run 'RUFA/Setup/5 - Crea stazioni di benchmark'.");
                return;
            }
            StartCoroutine(RunAll());
        }

        IEnumerator RunAll()
        {
            IsRunning = true;
            ShowMarkers(true);
            var head = Camera.main.transform;
            var mover = FindFirstObjectByType<DesktopMover>();
            if (mover != null) mover.enabled = false; // PC: the flythrough drives the rig
            var rows = new List<string>();

            for (int i = 0; i < stations.Length; i++)
            {
                if (RigBootstrap.XrIsActive) yield return WaitAtStation(head, stations[i], i);
                else Teleport(stations[i]);
                Say($"Stazione {i + 1}/{stations.Length}: fermo, sto misurando");
                yield return new WaitForSeconds(settleSeconds);
                yield return Sample();
                rows.Add(Summarise(i));
            }

            var path = Write(rows);
            Say($"Fatto. CSV: {path}");
            ShowMarkers(false);
            if (mover != null) mover.enabled = true;
            IsRunning = false;
            Finished?.Invoke();
            if (FromCommandLine) Application.Quit();
        }

        IEnumerator WaitAtStation(Transform head, Transform station, int index)
        {
            while (true)
            {
                var offset = head.position - station.position; offset.y = 0f;
                var look = head.forward; look.y = 0f;
                if (offset.magnitude <= reachRadius && Vector3.Dot(look.normalized, station.forward) > 0.9f) yield break;
                Say($"Stazione {index + 1}/{stations.Length}: vai sul disco e guarda la freccia");
                yield return null;
            }
        }

        static void Teleport(Transform station)
        {
            var rig = GameObject.Find("Desktop Rig");
            if (rig == null) return;
            var controller = rig.GetComponent<CharacterController>();
            controller.enabled = false; // otherwise the controller fights the teleport
            rig.transform.SetPositionAndRotation(station.position, Quaternion.LookRotation(station.forward, Vector3.up));
            var head = rig.GetComponentInChildren<Camera>();
            if (head != null) head.transform.localRotation = Quaternion.identity;
            controller.enabled = true;
        }

        IEnumerator Sample()
        {
            frames.Clear(); cpu.Clear(); gpu.Clear();
            float end = Time.unscaledTime + sampleSeconds;
            while (Time.unscaledTime < end)
            {
                yield return null;
                frames.Add(Time.unscaledDeltaTime * 1000f);
                double gpuMs = GpuTime.LastFrameMs(); // also captures this frame's timings for the CPU value below
                if (FrameTimingManager.GetLatestTimings(1, timing) > 0 && timing[0].cpuFrameTime > 0) cpu.Add(timing[0].cpuFrameTime);
                if (gpuMs > 0) gpu.Add(gpuMs);
            }
        }

        string Summarise(int station)
        {
            var worstFirst = frames.OrderByDescending(f => f).ToList();
            int worstCount = Mathf.Max(1, worstFirst.Count / 100);      // the slowest 1 % of frames
            float mean = frames.Average(), p1Low = worstFirst.Take(worstCount).Average(), max = worstFirst[0];
            double cpuMean = cpu.Count > 0 ? cpu.Average() : 0.0, gpuMean = gpu.Count > 0 ? gpu.Average() : 0.0;
            var row = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2:F2},{3:F2},{4:F2},{5:F1},{6:F2},{7:F2}",
                station + 1, frames.Count, mean, p1Low, max, 1000f / mean, cpuMean, gpuMean);
            Debug.Log($"Benchmark: {Header} = {row}");
            return row;
        }

        string Write(List<string> rows)
        {
            var path = Path.Combine(Application.persistentDataPath, $"benchmark-{DateTime.Now:yyyyMMdd-HHmmss}.csv");
            var asset = UniversalRenderPipeline.asset;
            var meta = string.Format(CultureInfo.InvariantCulture, "# mode={0} development={1} renderScale={2:F2} msaa={3} refreshRate={4:F0} gpuTime={5} product={6}",
                RigBootstrap.XrIsActive ? "VR" : "PC", Debug.isDebugBuild, asset != null ? asset.renderScale : 1f,
                asset != null ? asset.msaaSampleCount : 0, RefreshRate(), GpuTime.Source, Application.productName);
            File.WriteAllLines(path, new[] { meta, Header }.Concat(rows));
            Debug.Log($"Benchmark: written {path}");
            return path;
        }

        static float RefreshRate()
        {
            var displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displays);
            return displays.Count > 0 && displays[0].TryGetDisplayRefreshRate(out float hz) ? hz : 0f;
        }

        // A small text panel 0.7 m in front of the eyes, like the crosshair: world-space UI, built once.
        void Say(string text)
        {
            if (prompt == null)
            {
                var canvas = new GameObject("Benchmark Prompt", typeof(Canvas)).GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.transform.SetParent(Camera.main.transform, false);
                canvas.transform.localPosition = new Vector3(0f, -0.12f, 0.7f);
                canvas.transform.localScale = Vector3.one * 0.001f;
                canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 120f);
                var label = new GameObject("Text", typeof(TextMeshProUGUI));
                label.transform.SetParent(canvas.transform, false);
                var rect = label.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
                prompt = label.GetComponent<TextMeshProUGUI>();
                prompt.fontSize = 24f;
                prompt.alignment = TextAlignmentOptions.Center;
            }
            prompt.text = text;
        }
    }
}
