using UnityEngine;
using UnityEngine.Rendering;

namespace Rufa
{
    /// <summary>
    /// A flat square of 1 x 1 m cut into cells x cells squares, two triangles each, built when the object switches on or
    /// its number changes, in Edit mode too, and never saved: the floor exported with all its subdivisions. Flat stays
    /// flat with two triangles or two million; only the statistics tell them apart.
    /// </summary>
    [ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class DenseGrid : MonoBehaviour
    {
        [Tooltip("Squares on each side: the triangles are 2 x cells x cells.")]
        [Min(1)] public int cells = 100;

        void OnEnable()
        {
            int n = cells, row = n + 1;
            var vertices = new Vector3[row * row];
            var uv = new Vector2[vertices.Length];
            for (int z = 0, v = 0; z <= n; z++)
                for (int x = 0; x <= n; x++, v++)
                {
                    uv[v] = new Vector2((float)x / n, (float)z / n);
                    vertices[v] = new Vector3(uv[v].x - 0.5f, 0f, uv[v].y - 0.5f);
                }
            var indices = new int[n * n * 6];
            for (int z = 0, i = 0; z < n; z++)
                for (int x = 0; x < n; x++)
                {
                    int a = z * row + x, b = a + row;   // clockwise seen from above: the faces look up
                    indices[i++] = a; indices[i++] = b; indices[i++] = a + 1;
                    indices[i++] = a + 1; indices[i++] = b; indices[i++] = b + 1;
                }
            var mesh = new Mesh { name = "Griglia densa", hideFlags = HideFlags.DontSave, indexFormat = IndexFormat.UInt32, vertices = vertices, uv = uv, triangles = indices };
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();   // the floor's normal map needs them
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        void OnDisable()
        {
            var filter = GetComponent<MeshFilter>();
            if (filter.sharedMesh != null) DestroyImmediate(filter.sharedMesh);
            filter.sharedMesh = null;
        }

#if UNITY_EDITOR
        // A number typed in the Inspector rebuilds the floor at once; a moment later, because OnValidate must not swap meshes.
        void OnValidate() => UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null || !isActiveAndEnabled) return;
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            if (mesh != null && mesh.GetIndexCount(0) == 6L * cells * cells) return;   // already the floor asked for
            OnDisable();
            OnEnable();
        };
#endif
    }
}
