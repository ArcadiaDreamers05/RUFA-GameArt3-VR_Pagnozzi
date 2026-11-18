using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>
    /// What the lesson 6 tutorial checks and does in the museum: a room cured with LODs or with occlusion culling while
    /// everything in it stays on and in its place; in the lab, the back of a model cut away, Unity's own mesh LODs and a
    /// mesh with fewer vertices; the photo from a room's camera, and the file of the measures.
    /// </summary>
    public static class GeometryChecks
    {
        /// <summary>
        /// Every object under the room whose name starts with the prefix has a LOD Group of two levels or more, there is
        /// at least one, and the room is untouched.
        /// </summary>
        public static bool CuredWithLod(string room, string prefix)
        {
            var root = GameObject.Find(room);
            if (root == null) return false;
            var named = root.GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith(prefix)).ToArray();
            return named.Length > 0 && named.All(t => t.TryGetComponent(out LODGroup group) && group.lodCount >= 2) && Untouched(root);
        }

        /// <summary>
        /// The wall hides what is behind it: every renderer in it is Occluder Static, the open scene has occlusion data,
        /// and the room is untouched.
        /// </summary>
        public static bool CuredWithOcclusion(string room, string wall)
        {
            var root = GameObject.Find(room);
            var renderers = GameObject.Find(wall) is GameObject w ? w.GetComponentsInChildren<Renderer>(true) : new Renderer[0];
            return root != null && renderers.Length > 0 && StaticOcclusionCulling.umbraDataSize > 0
                && renderers.All(r => GameObjectUtility.GetStaticEditorFlags(r.gameObject).HasFlag(StaticEditorFlags.OccluderStatic))
                && Untouched(root);
        }

        /// <summary>
        /// The faces nobody sees are gone and the rest is still there. Every object under the room whose name starts with
        /// the prefix keeps at least minFront triangles in front of its pivot (on the side it faces, its local +Z), has at
        /// most maxTotal triangles in all and is still at least minHeight metres tall (a model exported a hundred times
        /// smaller is no cure); there is at least one, and the room is untouched.
        /// </summary>
        public static bool BackRemoved(string room, string prefix, int minFront, int maxTotal, float minHeight)
        {
            var root = GameObject.Find(room);
            if (root == null) return false;
            var named = root.GetComponentsInChildren<MeshFilter>(true).Where(f => f.name.StartsWith(prefix)).ToArray();
            return named.Length > 0 && named.All(f => FrontKept(f, minFront, maxTotal, minHeight)) && Untouched(root);
        }

        static bool FrontKept(MeshFilter filter, int minFront, int maxTotal, float minHeight)
        {
            var mesh = filter.sharedMesh;
            if (mesh == null || !filter.TryGetComponent(out Renderer renderer) || renderer.bounds.size.y < minHeight) return false;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            int front = 0;
            for (int i = 0; i < triangles.Length; i += 3)
                if (vertices[triangles[i]].z + vertices[triangles[i + 1]].z + vertices[triangles[i + 2]].z > 0f) front++;   // its centre is in front
            return front >= minFront && triangles.Length / 3 <= maxTotal;
        }

        /// <summary>The model at the path has Generate Mesh LODs on, and its mesh more than one level.</summary>
        public static bool HasMeshLods(string path) =>
            AssetImporter.GetAtPath(path) is ModelImporter importer && importer.generateMeshLods
            && AssetDatabase.LoadAllAssetsAtPath(path).OfType<Mesh>().Any(m => m.lodCount > 1);

        /// <summary>The named object under the room has a mesh of at most max vertices, and the room is untouched.</summary>
        public static bool FewVertices(string room, string name, int max)
        {
            var root = GameObject.Find(room);
            var filter = root == null ? null : root.GetComponentsInChildren<MeshFilter>(true).FirstOrDefault(f => f.name == name);
            return filter != null && filter.sharedMesh != null && filter.sharedMesh.vertexCount <= max && Untouched(root);
        }

        // Nothing switched off, moved or deleted: the cure is in the craft, not in hiding things.
        internal static bool Untouched(GameObject root) =>
            ProfilingChecks.AllOn(root.transform) && root.TryGetComponent(out EnvironmentSnapshot snapshot) && snapshot.Changes().Count == 0;

        /// <summary>
        /// Draws the named camera of the open scene into a 1920 x 1080 JPG at the path, from the repository: the photo of a
        /// room before and after its cure. The camera is left as it was.
        /// </summary>
        public static void Photo(string cameraName, string path)
        {
            var camera = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault(c => c.name == cameraName);
            if (camera == null) { Debug.LogError($"RUFA: no camera named '{cameraName}' in the open scene."); return; }
            var target = RenderTexture.GetTemporary(1920, 1080, 24);
            var previous = camera.targetTexture;
            camera.targetTexture = target;
            camera.Render();
            camera.targetTexture = previous;
            var active = RenderTexture.active;
            RenderTexture.active = target;
            var image = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(target);
            var full = Path.Combine(LevelChecks.Repository, path);   // an absolute path stays as it is
            Directory.CreateDirectory(Path.GetDirectoryName(full));
            File.WriteAllBytes(full, image.EncodeToJPG(90));
            Object.DestroyImmediate(image);
            Debug.Log($"RUFA: photo of {cameraName} saved in {path}.");
        }

        /// <summary>The file of a lesson's measures, from the template of its folder (L07, or else lesson 6's). Returns the full path.</summary>
        public static string CreateMeasures(string path) =>
            LevelChecks.FromTemplate(path, Path.GetFileName(Path.GetDirectoryName(path)) == "L07" ? DrawCallChecks.MeasuresTemplate : MeasuresTemplate);

        public const string MeasuresTemplate =
@"# Le misure della lezione 6

Al posto di ogni segnaposto tra « » scrivi il tuo numero o il tuo testo. Le foto stanno in questa cartella.

## Il museo
Il salone, triangoli prima e dopo: «da scrivere»
Il giardino, triangoli prima e dopo: «da scrivere»
Il deposito, triangoli prima e dopo: «da scrivere»
Le curiosità, triangoli prima e dopo: «da scrivere»
Cosa ha notato il curatore, e cosa hai cambiato dopo: «da scrivere»

## Il laboratorio
Le nicchie, triangoli prima e dopo: «da scrivere»
Il tuo LOD del vaso d'ottone, i Ratio che hai scelto e perché: «da scrivere»
Mesh LOD o il tuo LOD Group per il vaso: quale terresti, e perché: «da scrivere»
Il vaso sfaccettato, vertici prima e dopo, e lo Smoothing Angle che hai scelto: «da scrivere»

## Il tuo progetto
Dove hai messo la camera, e perché: «da scrivere»
Triangoli prima: «da scrivere»
Cosa hai fatto: «da scrivere»
Triangoli dopo: «da scrivere»
";
    }
}
