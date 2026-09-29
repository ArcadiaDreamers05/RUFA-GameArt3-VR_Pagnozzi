using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rufa.Demo
{
    /// <summary>
    /// Heavy geometry generated at start-up from parameters: dense floor grids and fluted columns.
    /// Generated at runtime on purpose, so the repository holds numbers instead of 50 MB meshes and the
    /// cure of lesson 6 is a change of parameters plus LOD groups.
    /// </summary>
    public sealed class ProceduralGeometry : MonoBehaviour
    {
        [Header("Floors: one grid mesh per Floor_<room> object, replacing the flat cube mesh")]
        public int floorSubdivisions = 300;   // 300 x 300 cells x 2 = 180 000 triangles per floor
        public float floorNoise = 0.01f;      // metres of vertical noise, a hint of old stone tiles

        [Header("Fluted columns in the living room")]
        public int columnSegments = 128;      // around
        public int columnRings = 256;         // along the height
        public int columnCount = 6;

        [Header("Cures (lesson 6 and 7)")]
        [Range(1, 3)] public int lodLevels = 1;   // 1 = no LOD group
        public bool shareMaterials = false;       // false = one material instance per column (draw call defect)
        public Material columnMaterial;           // null = the walls' material; lesson 7 sets it, because the columns' UVs do not fit the atlas

        // Pairs of pillars framing the living room's three doorways (south, north, east), clear of the walkways.
        static readonly Vector3[] ColumnSpots =
        {
            new Vector3(-4.05f, 0f, 0.45f), new Vector3(-1.95f, 0f, 0.45f),
            new Vector3(-4.05f, 0f, 5.55f), new Vector3(-1.95f, 0f, 5.55f),
            new Vector3(-0.45f, 0f, 1.95f), new Vector3(-0.45f, 0f, 4.05f),
        };

        void Start()
        {
            var house = GameObject.Find("Casa");
            if (house == null) { Debug.LogWarning("ProceduralGeometry: no 'Casa' root, nothing generated."); return; }
            foreach (var filter in house.GetComponentsInChildren<MeshFilter>())
                if (filter.name.StartsWith("Floor_")) BuildFloor(filter);
            BuildColumns(house.transform);
        }

        void BuildFloor(MeshFilter floor)
        {
            var renderer = floor.GetComponent<MeshRenderer>();
            float scaleY = floor.transform.localScale.y;
            if (lodLevels <= 1)
            {
                floor.sharedMesh = Grid(floorSubdivisions, floorNoise / scaleY);
                return;
            }

            // LOD0 on the object itself, coarser copies as children, all sharing the material.
            var renderers = new List<Renderer[]>();
            floor.sharedMesh = Grid(floorSubdivisions, floorNoise / scaleY);
            renderers.Add(new Renderer[] { renderer });
            for (int level = 1; level < lodLevels; level++)
            {
                int cells = Mathf.Max(4, floorSubdivisions >> (2 * level)); // 300 -> 75 -> 18
                var child = new GameObject($"LOD{level}", typeof(MeshFilter), typeof(MeshRenderer));
                child.transform.SetParent(floor.transform, false);
                child.GetComponent<MeshFilter>().sharedMesh = Grid(cells, floorNoise / scaleY);
                child.GetComponent<MeshRenderer>().sharedMaterial = renderer.sharedMaterial;
                renderers.Add(new Renderer[] { child.GetComponent<MeshRenderer>() });
            }
            var group = floor.gameObject.AddComponent<LODGroup>();
            var lods = new LOD[renderers.Count];
            float[] heights = { 0.5f, 0.15f, 0.03f };
            for (int i = 0; i < lods.Length; i++) lods[i] = new LOD(heights[i], renderers[i]);
            group.SetLODs(lods);
            group.RecalculateBounds();
        }

        void BuildColumns(Transform house)
        {
            var ceiling = house.Find("Salotto/Ceiling_Salotto");
            float height = ceiling != null ? ceiling.position.y - 0.05f : 2.2f;
            Material wall = columnMaterial;
            if (wall == null)
                foreach (var r in house.GetComponentsInChildren<MeshRenderer>())
                    if (r.name.StartsWith("Wall_")) { wall = r.sharedMaterial; break; }
            var mesh = Lathe(columnSegments, columnRings, height);

            for (int i = 0; i < Mathf.Min(columnCount, ColumnSpots.Length); i++)
            {
                var column = new GameObject($"Colonna_{i}", typeof(MeshFilter), typeof(MeshRenderer), typeof(CapsuleCollider));
                column.transform.SetParent(transform, false);
                column.transform.position = ColumnSpots[i];
                column.GetComponent<MeshFilter>().sharedMesh = mesh;
                var renderer = column.GetComponent<MeshRenderer>();
                if (shareMaterials) renderer.sharedMaterial = wall;
                else { renderer.sharedMaterial = wall; renderer.material.color = wall.color; } // .material = one instance per object
                var collider = column.GetComponent<CapsuleCollider>();
                collider.center = new Vector3(0f, height * 0.5f, 0f);
                collider.height = height;
                collider.radius = 0.25f;
            }
        }

        // Unit square on XZ (-0.5..0.5), top face of the floor cube (y = +0.5 in the cube's local space).
        static Mesh Grid(int cells, float noise)
        {
            int n = cells + 1;
            var vertices = new Vector3[n * n];
            var uv = new Vector2[n * n];
            var triangles = new int[cells * cells * 6];
            for (int z = 0; z < n; z++)
                for (int x = 0; x < n; x++)
                {
                    float u = (float)x / cells, v = (float)z / cells;
                    float y = 0.5f + noise * Mathf.PerlinNoise(u * 40f, v * 40f);
                    vertices[z * n + x] = new Vector3(u - 0.5f, y, v - 0.5f);
                    uv[z * n + x] = new Vector2(u * 4f, v * 4f);
                }
            int t = 0;
            for (int z = 0; z < cells; z++)
                for (int x = 0; x < cells; x++)
                {
                    int i = z * n + x;
                    triangles[t++] = i; triangles[t++] = i + n; triangles[t++] = i + 1;
                    triangles[t++] = i + 1; triangles[t++] = i + n; triangles[t++] = i + n + 1;
                }
            return Finish(vertices, uv, triangles);
        }

        // Fluted column: radius varies around the circumference and slightly along the height.
        static Mesh Lathe(int segments, int rings, float height)
        {
            var vertices = new Vector3[(segments + 1) * (rings + 1)];
            var uv = new Vector2[vertices.Length];
            var triangles = new int[segments * rings * 6];
            for (int r = 0; r <= rings; r++)
            {
                float v = (float)r / rings;
                for (int s = 0; s <= segments; s++)
                {
                    float u = (float)s / segments, angle = u * Mathf.PI * 2f;
                    float radius = 0.2f + 0.03f * Mathf.Sin(angle * 12f) + 0.02f * Mathf.Sin(v * Mathf.PI * 3f);
                    vertices[r * (segments + 1) + s] = new Vector3(Mathf.Cos(angle) * radius, v * height, Mathf.Sin(angle) * radius);
                    uv[r * (segments + 1) + s] = new Vector2(u, v * 3f);
                }
            }
            int t = 0;
            for (int r = 0; r < rings; r++)
                for (int s = 0; s < segments; s++)
                {
                    int i = r * (segments + 1) + s, j = i + segments + 1;
                    triangles[t++] = i; triangles[t++] = j; triangles[t++] = i + 1;
                    triangles[t++] = i + 1; triangles[t++] = j; triangles[t++] = j + 1;
                }
            return Finish(vertices, uv, triangles);
        }

        static Mesh Finish(Vector3[] vertices, Vector2[] uv, int[] triangles)
        {
            var mesh = new Mesh { indexFormat = IndexFormat.UInt32, vertices = vertices, uv = uv, triangles = triangles };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
