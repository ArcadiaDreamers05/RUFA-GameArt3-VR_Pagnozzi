using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rufa.Editor
{
    /// <summary>
    /// What the lesson 7 tutorial checks in the market: the SRP Batcher on, a stall's goods drawn with one shader variant,
    /// few materials, all static, the glass cut down to what the stall needs, and the mixed crate drawn with one atlas
    /// material. A stall counts as cured only if its goods are all on and in their place: deleting fruit is no cure.
    /// </summary>
    public static class DrawCallChecks
    {
        /// <summary>
        /// Every URP asset the project renders with, the default and each quality level's, has the SRP Batcher on. The
        /// demo's own asset, under Assets/Demo, is left out: the demo is sick on purpose.
        /// </summary>
        public static bool SrpBatcherOn()
        {
            var assets = Enumerable.Range(0, QualitySettings.names.Length).Select(QualitySettings.GetRenderPipelineAssetAt)
                .Append(GraphicsSettings.defaultRenderPipeline).OfType<UniversalRenderPipelineAsset>()
                .Where(a => !AssetDatabase.GetAssetPath(a).StartsWith("Assets/Demo/")).ToArray();
            return assets.Length > 0 && assets.All(a => a.useSRPBatcher);
        }

        /// <summary>
        /// Every material under the root uses the named shader with the same keywords and GPU Instancing off: one shader
        /// variant, which the SRP Batcher draws without changing state. The root is untouched.
        /// </summary>
        public static bool OneVariant(string root, string shader)
        {
            var materials = Materials(root, out var go);
            return materials.Length > 0 && materials.All(m => m.shader.name == shader && !m.enableInstancing && Keywords(m) == Keywords(materials[0]))
                && GeometryChecks.Untouched(go);
        }

        /// <summary>The renderers under the root use at most max different materials, and the root is untouched.</summary>
        public static bool MaterialsAtMost(string root, int max)
        {
            var materials = Materials(root, out var go);
            return materials.Length > 0 && materials.Length <= max && GeometryChecks.Untouched(go);
        }

        /// <summary>Every renderer under the root has Batching Static, and the root is untouched.</summary>
        public static bool AllStatic(string root)
        {
            var go = GameObject.Find(root);
            var renderers = go == null ? new Renderer[0] : go.GetComponentsInChildren<Renderer>(true);
            return renderers.Length > 0 && renderers.All(r => GameObjectUtility.GetStaticEditorFlags(r.gameObject).HasFlag(StaticEditorFlags.BatchingStatic))
                && GeometryChecks.Untouched(go);
        }

        /// <summary>
        /// The panes under the glass root that still draw (on, with a transparent material) are between min and max, and
        /// the goods are untouched: the second layer goes, the glass the stall needs stays.
        /// </summary>
        public static bool TransparentsBetween(string glass, int min, int max, string goods)
        {
            var panes = GameObject.Find(glass) is GameObject g
                ? g.GetComponentsInChildren<Renderer>().Count(r => r.enabled && r.sharedMaterials.Any(m => m != null && m.renderQueue > (int)RenderQueue.GeometryLast))
                : -1;
            return panes >= min && panes <= max && GameObject.Find(goods) is GameObject root && GeometryChecks.Untouched(root);
        }

        /// <summary>
        /// The model at the path is one mesh in one part, and every renderer of the open scene that draws it has one
        /// material whose base texture is new: none of the model's own textures, which sit in its .fbm folder.
        /// </summary>
        public static bool OneMaterial(string modelPath)
        {
            var meshes = AssetDatabase.LoadAllAssetsAtPath(modelPath).OfType<Mesh>().ToArray();
            if (meshes.Length != 1 || meshes[0].subMeshCount != 1) return false;
            var own = Path.ChangeExtension(modelPath, "fbm") + "/";
            var users = Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None).Where(f => f.sharedMesh == meshes[0]).Select(f => f.GetComponent<Renderer>()).ToArray();
            return users.Length > 0 && users.All(r => r != null && r.sharedMaterials.Length == 1 && r.sharedMaterial != null
                && r.sharedMaterial.GetTexture("_BaseMap") is Texture t && !AssetDatabase.GetAssetPath(t).StartsWith(own));
        }

        static string Keywords(Material m) => string.Join(" ", m.enabledKeywords.Select(k => k.name).OrderBy(k => k));

        // The different materials of every renderer under the root, switched off ones included.
        static Material[] Materials(string root, out GameObject go)
        {
            go = GameObject.Find(root);
            return go == null ? new Material[0] : go.GetComponentsInChildren<Renderer>(true).SelectMany(r => r.sharedMaterials).Where(m => m != null).Distinct().ToArray();
        }

        public const string MeasuresTemplate =
@"# Le misure della lezione 7

Al posto di ogni segnaposto tra « » scrivi il tuo numero o il tuo testo. Le foto stanno in questa cartella.

## Il mercato
La frutta, Batches e SetPass prima: «da scrivere»
La frutta con l'SRP Batcher spento, SetPass: «da scrivere»
La frutta, Batches e SetPass dopo: «da scrivere»
Le spezie, Batches e SetPass prima e dopo: «da scrivere»
La vetrina, Batches e SetPass prima e dopo: «da scrivere»
Il bar, Batches e SetPass prima e dopo: «da scrivere»
Il curatore ha indovinato? Dove, e cosa hai cambiato dopo: «da scrivere»

## Il laboratorio
Le cassette, Batches prima e dopo: «da scrivere»
Quanto è grande il tuo atlas, e quanto margine hai lasciato: «da scrivere»

## Il tuo progetto
Dove hai messo la camera, e perché: «da scrivere»
Batches e SetPass prima: «da scrivere»
Cosa hai fatto: «da scrivere»
Batches e SetPass dopo: «da scrivere»
";
    }
}
