using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>Visible measurement stations (a disc to stand on, an arrow to look along) and the Benchmark component that uses them.</summary>
    public static class BenchmarkStations
    {
        public const string RootName = "Benchmark";

        [MenuItem("RUFA/Setup/5 - Crea stazioni di benchmark")]
        public static void CreateSandbox() => Create(new[]
        {
            (new Vector3(0f, 0f, 0f), Vector3.forward),
            (new Vector3(3f, 0f, 3f), Vector3.left),
            (new Vector3(-3f, 0f, 3f), Vector3.right),
        });

        public static void Create((Vector3 position, Vector3 look)[] stations)
        {
            var old = GameObject.Find(RootName);
            if (old != null) Object.DestroyImmediate(old);
            var root = new GameObject(RootName);
            var benchmark = root.AddComponent<Benchmark>();
            benchmark.stations = new Transform[stations.Length];
            var paint = MarkerMaterial();

            for (int i = 0; i < stations.Length; i++)
            {
                var (position, look) = stations[i];
                var station = new GameObject($"Stazione {i + 1}").transform;
                station.SetParent(root.transform, false);
                station.SetPositionAndRotation(position, Quaternion.LookRotation(look, Vector3.up));
                benchmark.stations[i] = station;
                float ground = GroundAt(position) + 0.02f; // on a rug rather than under it, and clear of the floor's 1 cm of noise

                var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                disc.name = "Disco";
                disc.transform.SetParent(station, false);
                disc.transform.localPosition = new Vector3(0f, ground, 0f);
                disc.transform.localScale = new Vector3(0.5f, 0.005f, 0.5f);
                disc.GetComponent<MeshRenderer>().sharedMaterial = paint;
                Object.DestroyImmediate(disc.GetComponent<Collider>());

                var arrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                arrow.name = "Freccia";
                arrow.transform.SetParent(station, false);
                arrow.transform.localPosition = new Vector3(0f, ground + 0.02f, 0.45f);
                arrow.transform.localScale = new Vector3(0.05f, 0.05f, 0.4f);
                arrow.GetComponent<MeshRenderer>().sharedMaterial = paint;
                Object.DestroyImmediate(arrow.GetComponent<Collider>());

                // The station number lying on the disc, readable when you step on it facing the arrow.
                var number = new GameObject("Numero", typeof(TextMeshPro));
                number.transform.SetParent(station, false);
                number.transform.localPosition = new Vector3(0f, ground + 0.006f, 0f);
                number.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                var text = number.GetComponent<TextMeshPro>();
                text.text = (i + 1).ToString();
                text.fontSize = 3f;
                text.alignment = TextAlignmentOptions.Center;
                text.color = Color.black;
                number.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f, 0.4f);
            }
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log($"RUFA: {stations.Length} benchmark stations created (shown only while the benchmark runs). Save the scene.");
        }

        // The top of whatever lies on the floor at that spot (a rug, for instance), relative to the station.
        static float GroundAt(Vector3 spot)
        {
            float top = spot.y;
            foreach (var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
            {
                var b = r.bounds;
                if (b.max.y < spot.y + 0.3f && b.max.y > top && b.min.x < spot.x && b.max.x > spot.x && b.min.z < spot.z && b.max.z > spot.z) top = b.max.y;
            }
            return top - spot.y;
        }

        // Bright and unlit: it shows up in the darkest room, lights or no lights.
        static Material MarkerMaterial()
        {
            const string path = BuildingBlocks.MaterialsFolder + "/Stazione.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null) return material;
            BuildingBlocks.EnsureFolder(BuildingBlocks.MaterialsFolder);
            material = new Material(Shader.Find("Universal Render Pipeline/Unlit")) { color = new Color(1f, 0.75f, 0f) };
            AssetDatabase.CreateAsset(material, path);
            return material;
        }
    }
}
