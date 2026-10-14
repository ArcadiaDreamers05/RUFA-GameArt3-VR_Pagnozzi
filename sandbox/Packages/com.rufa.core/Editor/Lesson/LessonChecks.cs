using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Rufa.Editor
{
    /// <summary>
    /// What a tutorial page can ask for ("> verifica: id args") and the buttons it can show ("> azione: label | id | args").
    /// Prefab checks read the saved asset of the student's copy, never an instance or the clone in Play:
    /// a change made only during Play is lost when Play stops, and it does not count here either.
    /// </summary>
    public static class LessonChecks
    {
        static readonly Dictionary<string, Func<string[], bool>> Checks = new Dictionary<string, Func<string[], bool>>
        {
            ["sample-importato"] = a => ImportedFolder(a[0]) != null,
            ["scena-aperta"] = a => EditorSceneManager.GetActiveScene().name == a[0],
            ["simulatore-acceso"] = _ => EditorUserSettings.GetConfigValue(RigBootstrap.SimulatorSetting) == "1",
            ["in-play"] = _ => EditorApplication.isPlaying,
            ["fuori-play"] = _ => !EditorApplication.isPlaying,
            ["selezionato"] = a => Selection.activeGameObject != null && a.Contains(Selection.activeGameObject.name),
            ["prefab-aperto"] = a => PrefabStageUtility.GetCurrentPrefabStage() is var stage && stage != null && stage.prefabContentsRoot.name == a[0],
            ["prefab-numero"] = a => Property(a) is SerializedProperty p && Between(p.propertyType == SerializedPropertyType.Float ? p.floatValue : p.intValue, a[4], a[5]),
            ["prefab-vero"] = a => Property(a) is SerializedProperty p && p.boolValue == (a[4] == "sì"),
            ["prefab-attivo"] = a => Child(a) is Transform c && c.gameObject.activeSelf,
            ["prefab-componente"] = a => Child(a) is Transform c && TypeNamed(a[2]) is Type t && c.GetComponentInChildren(t, true) != null,
            ["componente-in-scena"] = a => TypeNamed(a[0]) is Type t && UnityEngine.Object.FindAnyObjectByType(t) != null,
            ["nessun-teleport-su"] = a => !UnityEngine.Object.FindObjectsByType<TeleportationArea>(FindObjectsSortMode.None).Any(t => a.Any(prefix => t.name.StartsWith(prefix))),
            ["anchor-verso"] = AnchorsFacing,
            ["volume-sguardo"] = a => GameObject.Find(a[0]) is GameObject o && o.TryGetComponent(out TeleportationMultiAnchorVolume v)
                                      && new SerializedObject(v).FindProperty(GazeFilterField).objectReferenceValue is GazeTeleportationAnchorFilter,
            ["su-anchor"] = _ => EditorApplication.isPlaying && Camera.main != null && UnityEngine.Object.FindObjectsByType<TeleportationAnchor>(FindObjectsSortMode.None)
                .Any(t => Vector3.ProjectOnPlane(Camera.main.transform.position - (t.teleportAnchorTransform != null ? t.teleportAnchorTransform : t.transform).position, Vector3.up).magnitude < 0.3f),
            ["sopra-oggetto"] = a => EditorApplication.isPlaying && CourseGoal.Feet() is Vector3 feet && Surface(a[0]) is Collider c
                                     && CourseGoal.Above(c, feet) && feet.y >= c.bounds.max.y - 0.1f,
            // On the surface of an area that does not accept its sides: aiming at the wall must not be a way up.
            ["sopra-area-filtrata"] = a => Checks["sopra-oggetto"](a) && Surface(a[0]).TryGetComponent(out TeleportationArea area) && area.filterSelectionByHitNormal,
            // On the named object or one of its children: an area on the disc of a pad counts as on the pad.
            ["scena-vero"] = a => GameObject.Find(a[0]) is GameObject o && o.GetComponentsInChildren<Component>(true).FirstOrDefault(x => x != null && x.GetType().Name == a[1]) is Component c
                                  && new SerializedObject(c).FindProperty(a[2]) is SerializedProperty p && p.boolValue == (a[3] == "sì"),
            ["componente-su"] = a => TypeNamed(a[0]) is Type t && Surface(a[1]) is Collider c
                                     && UnityEngine.Object.FindObjectsByType(t, FindObjectsSortMode.None).Any(x => CourseGoal.Above(c, ((Component)x).transform.position)),
            ["ambiente-intatto"] = a => GameObject.Find(a[0]) is GameObject o && o.TryGetComponent(out EnvironmentSnapshot s) && s.Changes().Count == 0,
            ["riuscito"] = a => GameObject.Find(a[0]) is GameObject o && o.TryGetComponent(out CourseGoal g) && g.reached,
        };

        /// <summary>Where a Multi-Anchor Volume keeps its filter, the Destination Filter Object of its Destination Evaluation Settings.</summary>
        public const string GazeFilterField = "m_DestinationEvaluationSettings.m_ConstantValue.m_DestinationFilterObject";

        /// <summary>The checks that can pass only in Play: in a list chapter they stay done once they pass.</summary>
        public static readonly HashSet<string> PlayOnly = new HashSet<string> { "in-play", "sopra-oggetto", "sopra-area-filtrata", "riuscito", "su-anchor" };

        static readonly Dictionary<string, Action<string[]>> Actions = new Dictionary<string, Action<string[]>>
        {
            ["importa-sample"] = a => ImportSample(a[0]),
            ["apri-scena"] = a => OpenSampleScene(a[0], a[1]),
            ["crea-progetto"] = _ => ProjectScene.Create(ProjectScene.Folder),
        };

        public static bool Exists(string id) => Checks.ContainsKey(id) || Actions.ContainsKey(id);

        public static bool Passes(string id, string[] args)
        {
            if (!Checks.TryGetValue(id, out var check)) { Debug.LogError($"RUFA: unknown tutorial check '{id}'."); return false; }
            try { return check(args); }
            catch (Exception e) { Debug.LogException(e); return false; }  // a broken page must not break the window
        }

        public static void Run(string id, string[] args)
        {
            if (Actions.TryGetValue(id, out var action)) action(args);
            else Debug.LogError($"RUFA: unknown tutorial action '{id}'.");
        }

        // The student's copy of a sample prefab: the one under Assets, never the original inside the package.
        static GameObject Prefab(string name) =>
            AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }).Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => Path.GetFileNameWithoutExtension(p) == name).Select(AssetDatabase.LoadAssetAtPath<GameObject>).FirstOrDefault();

        // An empty child path means the prefab root.
        static Transform Child(string[] a)
        {
            var root = Prefab(a[0]);
            if (root == null) return null;
            return a[1].Length == 0 ? root.transform : root.transform.Find(a[1]);
        }

        static SerializedProperty Property(string[] a)
        {
            var child = Child(a);
            var component = child == null ? null : child.GetComponent(a[2]);
            return component == null ? null : new SerializedObject(component).FindProperty(a[3]);
        }

        static Type TypeNamed(string name) => TypeCache.GetTypesDerivedFrom<Component>().FirstOrDefault(t => t.Name == name);
        // TryGetComponent: in the editor, GetComponent on a missing built-in component returns a stand-in that `is` accepts and that throws when used.
        static Collider Surface(string name) => GameObject.Find(name) is GameObject o && o.TryGetComponent(out Collider c) ? c : null;

        static bool Between(float value, string min, string max) => value >= Number(min) - 0.0001f && value <= Number(max) + 0.0001f;
        static float Number(string text) => float.Parse(text.Replace(',', '.'), CultureInfo.InvariantCulture);

        // Every named station has, within 3 m, a Teleportation Anchor on the Teleport layer that keeps its direction
        // on arrival (Match Orientation = Target Up And Forward), and that direction looks at the station within 45°.
        static bool AnchorsFacing(string[] stations)
        {
            var anchors = UnityEngine.Object.FindObjectsByType<TeleportationAnchor>(FindObjectsSortMode.None)
                .Where(t => t.matchOrientation == MatchOrientation.TargetUpAndForward && (t.interactionLayers.value & (1 << BuildingBlocks.TeleportLayer)) != 0).ToArray();
            return stations.All(name =>
            {
                var station = GameObject.Find(name);
                return station != null && anchors.Any(anchor =>
                {
                    var at = anchor.teleportAnchorTransform != null ? anchor.teleportAnchorTransform : anchor.transform;
                    var toStation = Vector3.ProjectOnPlane(station.transform.position - at.position, Vector3.up);
                    return toStation.magnitude < 3f && Vector3.Angle(Vector3.ProjectOnPlane(at.forward, Vector3.up), toStation) < 45f;
                });
            });
        }

        const string SamplesRoot = "Assets/Samples/RUFA Core";

        // The student's copy of a practice: one folder per lesson, without the version folder of Unity's own sample import
        // (Assets/Samples/RUFA Core/0.3.0/...), whose number next to a lesson's name reads like a lesson number.
        static string ImportedFolder(string displayName) =>
            AssetDatabase.IsValidFolder(SamplesRoot + "/" + displayName) ? SamplesRoot + "/" + displayName : null;

        // Copies the practice once (pressing again next week must never overwrite the student's work) and opens its scene.
        static void ImportSample(string displayName)
        {
            if (ImportedFolder(displayName) == null && !CopySample(displayName)) return;
            var folder = ImportedFolder(displayName);
            // FindAssets promises no order: the first scene by name opens, Percorso before Verifica.
            var scene = folder == null ? null : AssetDatabase.FindAssets("t:Scene", new[] { folder }).Select(AssetDatabase.GUIDToAssetPath).OrderBy(p => p).FirstOrDefault();
            if (scene != null && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(scene);
        }

        // A copy left by Unity's own import, like the one of the first lesson 2 (07/10), sits in a version folder:
        // it goes to the Trash, with the student's work on it, only if the student agrees.
        static bool CopySample(string displayName)
        {
            var sample = Sample.FindByPackage("com.rufa.core", null).FirstOrDefault(s => s.displayName == displayName);
            if (sample.displayName == null) { Debug.LogError($"RUFA: sample '{displayName}' not found in RUFA Core."); return false; }
            var old = AssetDatabase.IsValidFolder(SamplesRoot)
                ? AssetDatabase.GetSubFolders(SamplesRoot).Select(version => version + "/" + displayName).Where(AssetDatabase.IsValidFolder).ToArray()
                : new string[0];
            if (old.Length > 0)
            {
                if (!EditorUtility.DisplayDialog("Palestra vecchia",
                        $"Nel progetto c'è una copia vecchia di \"{displayName}\":\n{string.Join("\n", old)}\n\nSostituisci: la copia vecchia va nel Cestino, con le modifiche che ci hai fatto, e al suo posto arriva quella nuova.",
                        "Sostituisci", "Annulla"))
                    return false;
                if (old.Any(folder => EditorSceneManager.GetActiveScene().path.StartsWith(folder + "/")))
                    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);   // the open scene goes to the Trash too: no save dialog for it
                foreach (var folder in old)
                {
                    AssetDatabase.MoveAssetToTrash(folder);
                    var version = Path.GetDirectoryName(folder).Replace('\\', '/');
                    if (!Directory.EnumerateFileSystemEntries(version).Any()) AssetDatabase.MoveAssetToTrash(version);
                }
            }
            Directory.CreateDirectory(SamplesRoot);
            FileUtil.CopyFileOrDirectory(sample.resolvedPath, SamplesRoot + "/" + displayName);
            AssetDatabase.Refresh();
            return true;
        }

        // Opens a scene of the student's copy of a practice, copying the practice first if it is not there yet.
        static void OpenSampleScene(string displayName, string scene)
        {
            if (ImportedFolder(displayName) == null && !CopySample(displayName)) return;
            var path = ImportedFolder(displayName) + "/" + scene + ".unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
            {
                Debug.LogError($"RUFA: '{scene}' is not in your copy of '{displayName}'. Delete the folder {ImportedFolder(displayName)} and press the button again.");
                return;
            }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(path);
        }
    }
}
