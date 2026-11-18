using UnityEngine;
using UnityEngine.Rendering;

namespace Rufa
{
    /// <summary>
    /// A rounded shape with as many triangles as asked, built when the object switches on or its number changes, in Edit
    /// mode too, and never saved: from a metre away two million triangles look like two thousand, and only the statistics
    /// tell them apart.
    /// </summary>
    [ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class DenseMesh : MonoBehaviour
    {
        [Tooltip("How many triangles the shape has, at least.")]
        [Min(8)] public int triangles = 2000;

        int Rings => Mathf.Max(2, Mathf.CeilToInt(Mathf.Sqrt(triangles / 2f)));   // n rings of n segments, two triangles each

        void OnEnable()
        {
            int n = Rings;
            var vertices = new Vector3[(n + 1) * (n + 1)];
            for (int ring = 0, v = 0; ring <= n; ring++)
                for (int segment = 0; segment <= n; segment++, v++)
                {
                    float lat = Mathf.PI * ring / n, lon = 2f * Mathf.PI * segment / n;
                    vertices[v] = 0.5f * new Vector3(Mathf.Sin(lat) * Mathf.Cos(lon), Mathf.Cos(lat), Mathf.Sin(lat) * Mathf.Sin(lon));
                }
            var indices = new int[n * n * 6];
            for (int ring = 0, i = 0; ring < n; ring++)
                for (int segment = 0; segment < n; segment++)
                {
                    int a = ring * (n + 1) + segment, b = a + n + 1;   // clockwise seen from outside: the faces look out
                    indices[i++] = a; indices[i++] = a + 1; indices[i++] = b;
                    indices[i++] = a + 1; indices[i++] = b + 1; indices[i++] = b;
                }
            var mesh = new Mesh { name = "Mesh densa", hideFlags = HideFlags.DontSave, indexFormat = IndexFormat.UInt32, vertices = vertices, triangles = indices };
            mesh.normals = System.Array.ConvertAll(vertices, v => v.normalized);   // a sphere's own normals: no seam where the rings close
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        void OnDisable()
        {
            var filter = GetComponent<MeshFilter>();
            if (filter.sharedMesh != null) DestroyImmediate(filter.sharedMesh);
            filter.sharedMesh = null;
        }

#if UNITY_EDITOR
        // A number typed in the Inspector rebuilds the shape at once; a moment later, because OnValidate must not swap meshes.
        void OnValidate() => UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null || !isActiveAndEnabled) return;
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            if (mesh != null && mesh.GetIndexCount(0) == 6L * Rings * Rings) return;   // already the shape asked for
            OnDisable();
            OnEnable();
        };
#endif
    }
}
