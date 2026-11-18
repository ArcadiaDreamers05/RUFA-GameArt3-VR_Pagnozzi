using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// A heap of pieces, each with its own material, built when the object switches on, in Edit mode too, and never saved:
    /// every piece is one more draw call. Twenty big pieces and two thousand small ones make the same heap.
    /// </summary>
    [ExecuteAlways]
    public sealed class MaterialPile : MonoBehaviour
    {
        [Tooltip("How many pieces the heap has.")]
        [Min(1)] public int pieces = 20;
        [Tooltip("How wide the heap is at its foot, in metres.")]
        [Min(0.1f)] public float size = 1.2f;

        void OnEnable()
        {
            if (transform.childCount > 0) return;   // switched off and on again: the pieces are still there
            var random = new System.Random(pieces);   // the same heap every time
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            float side = 0.6f * size / Mathf.Pow(pieces, 1f / 3f);
            for (int i = 0; i < pieces; i++)
            {
                var piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                piece.name = "Pezzo " + (i + 1);
                piece.hideFlags = HideFlags.DontSave;
                DestroyImmediate(piece.GetComponent<Collider>());
                piece.transform.SetParent(transform, false);
                // A cone of pieces: wide at the foot, narrow at the top
                float height = (float)random.NextDouble(), radius = (1f - height) * size / 2f * (float)random.NextDouble(), angle = (float)random.NextDouble() * 2f * Mathf.PI;
                piece.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, side / 2f + height * size * 0.6f, Mathf.Sin(angle) * radius);
                piece.transform.localRotation = Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f);
                piece.transform.localScale = Vector3.one * side;
                piece.GetComponent<MeshRenderer>().sharedMaterial = new Material(shader)
                {
                    name = piece.name, hideFlags = HideFlags.DontSave, color = Color.HSVToRGB(0.08f, 0.35f, 0.35f + 0.4f * (float)random.NextDouble()),
                };
            }
        }

        // The materials made for the pieces go with the heap; the pieces go with the scene.
        void OnDestroy()
        {
            foreach (var r in GetComponentsInChildren<MeshRenderer>(true))
                if (r.sharedMaterial != null && (r.sharedMaterial.hideFlags & HideFlags.DontSave) != 0) DestroyImmediate(r.sharedMaterial);
        }
    }
}
