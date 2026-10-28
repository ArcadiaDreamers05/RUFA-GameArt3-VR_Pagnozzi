using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Rufa.Editor
{
    /// <summary>
    /// The measures of a blockout, taken the way the body meets them in the headset: rays from the markers of a scene
    /// ("Misura ...", empty objects 1 m above the floor) against the colliders. Cubes and imported models count alike,
    /// and a model without colliders is not there for them, as it is not there for the player. Triggers are ignored.
    /// </summary>
    public static class LevelChecks
    {
        const float Reach = 3f;   // how far a ray looks for a wall or a floor

        // The distance to the first collider along the ray; null when there is none within reach.
        static float? Ray(Vector3 from, Vector3 direction, float reach = Reach) =>
            Physics.Raycast(from, direction, out var hit, reach, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ? hit.distance : (float?)null;

        /// <summary>From wall to wall, along the marker's X axis.</summary>
        public static float? Width(Transform marker)
        {
            Physics.SyncTransforms();   // out of Play, what moved reaches the physics only now
            return Ray(marker.position, marker.right) is float right && Ray(marker.position, -marker.right) is float left ? right + left : (float?)null;
        }

        /// <summary>From the floor under the marker to what stands above it, up to 10 m.</summary>
        public static float? Clearance(Transform marker)
        {
            Physics.SyncTransforms();
            return Ray(marker.position, Vector3.down) is float down && Ray(marker.position, Vector3.up, 10f) is float up ? down + up : (float?)null;
        }

        /// <summary>The largest change of height between two points 5 cm apart on the line between the markers, as a foot
        /// would follow it; null where the floor is missing.</summary>
        public static float? Roughness(Transform from, Transform to)
        {
            Physics.SyncTransforms();
            var steps = Mathf.Max(1, Mathf.CeilToInt(Vector3.Distance(from.position, to.position) / 0.05f));
            float worst = 0f, last = 0f;
            for (var i = 0; i <= steps; i++)
            {
                var point = Vector3.Lerp(from.position, to.position, i / (float)steps);
                if (!(Ray(point, Vector3.down) is float down)) return null;
                var height = point.y - down;
                if (i > 0) worst = Mathf.Max(worst, Mathf.Abs(height - last));
                last = height;
            }
            return worst;
        }

        /// <summary>The floor under the marker takes the teleport: a Teleportation Area or Anchor, on it or on a parent, on the Teleport layer.</summary>
        public static bool TeleportUnder(Transform marker)
        {
            Physics.SyncTransforms();
            if (!Physics.Raycast(marker.position, Vector3.down, out var hit, Reach, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) return false;
            var target = hit.collider.GetComponentInParent<BaseTeleportationInteractable>();
            return target != null && target.enabled && (target.interactionLayers.value & (1 << BuildingBlocks.TeleportLayer)) != 0;
        }

        /// <summary>The angle, seen from above, between where the object looks and where the target is.</summary>
        public static float Facing(Transform from, Transform to) =>
            Vector3.Angle(Vector3.ProjectOnPlane(from.forward, Vector3.up), Vector3.ProjectOnPlane(to.position - from.position, Vector3.up));

        /// <summary>A light switched on within the distance.</summary>
        public static bool LightNear(Transform place, float metres) =>
            Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Any(l => l.isActiveAndEnabled && l.intensity > 0f && Vector3.Distance(l.transform.position, place.position) <= metres);

        /// <summary>How high the middle of what is seen of the object stands.</summary>
        public static float? Height(Transform target) => Union(target.GetComponentsInChildren<Renderer>().Select(r => r.bounds).ToArray()) is Bounds b ? b.center.y : (float?)null;

        /// <summary>The size of the object's colliders along x, y or z.</summary>
        public static float? Size(Transform target, string axis)
        {
            Physics.SyncTransforms();
            return Union(target.GetComponentsInChildren<Collider>().Where(c => c.enabled && !c.isTrigger).Select(c => c.bounds).ToArray()) is Bounds b ? b.size["xyz".IndexOf(axis)] : (float?)null;
        }

        static Bounds? Union(Bounds[] all)
        {
            if (all.Length == 0) return null;
            var total = all[0];
            foreach (var b in all) total.Encapsulate(b);
            return total;
        }

        /// <summary>The student's repository: the folder that holds sandbox, where Consegne is.</summary>
        public static string Repository => Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

        public static bool FileFilled(string path, string placeholder) => File.Exists(path) && !File.ReadAllText(path).Contains(placeholder);

        public const string Placeholder = "«da scrivere»";

        /// <summary>Writes the concept template where there is none; the student's text is never overwritten. Returns the full path.</summary>
        public static string CreateConcept(string path)
        {
            var full = Path.Combine(Repository, path);   // an absolute path stays as it is
            if (File.Exists(full)) return full;
            Directory.CreateDirectory(Path.GetDirectoryName(full));
            File.WriteAllText(full, ConceptTemplate);
            return full;
        }

        /// <summary>The course's mannequin and door in front of the start of the open scene, once.</summary>
        public static void AddScaleReferences()
        {
            if (GameObject.Find(SandboxScene.MannequinName) != null) return;
            var start = GameObject.Find("Rig Bootstrap");
            SandboxScene.ScaleReferences(start != null ? start.transform.position : Vector3.zero,
                                         start != null ? Quaternion.Euler(0f, start.transform.eulerAngles.y, 0f) : Quaternion.identity);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        const string ConceptTemplate =
@"# Concept

Al posto di ogni segnaposto tra « » scrivi il tuo testo. La pianta disegnala a mano o in qualunque programma, e salvala come pianta.png in questa cartella.

## Titolo e genere
Titolo: «da scrivere»
Genere (walking simulator o escape room): «da scrivere»

## Ambientazione e tono
«da scrivere»

## Struttura
Walking simulator: i 3-5 momenti in ordine, ognuno con cosa si vede, cosa si sente e quale trigger lo avvia.
Escape room: la catena degli enigmi, 3-5 passaggi ""trovo X → uso X su Y → si apre Z"", coi mattoncini che usi.
«da scrivere»

## Pianta
![Pianta](pianta.png)

## Perimetro
Stanze e misure: «da scrivere»
Asset, solo CC0, e da dove: «da scrivere»
Se manca il tempo taglio: «da scrivere»
";
    }
}
