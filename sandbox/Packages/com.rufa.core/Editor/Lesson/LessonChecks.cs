using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Rufa.Editor
{
    /// <summary>
    /// What a tutorial page can ask for ("> verifica: id args") and the buttons it can show ("> azione: label | id | args").
    /// Prefab checks read the saved asset of the student's copy, never an instance or the clone in Play, and scene checks
    /// read the scene only out of Play: a change made only during Play is lost when Play stops, and it does not count here either.
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
            ["scena-vero"] = a => SceneProperty(a) is SerializedProperty p && p.boolValue == (a[3] == "sì"),
            ["scena-numero"] = a => SceneProperty(a) is SerializedProperty p && Between(p.propertyType == SerializedPropertyType.Float ? p.floatValue : p.intValue, a[3], a[4]),
            ["scena-riferimento"] = a => SceneProperty(a) is SerializedProperty p && p.objectReferenceValue != null,
            ["scena-componente"] = a => !EditorApplication.isPlaying && SceneComponent(a[0], a[1]) != null,
            // A persistent listener, set in the Inspector: a call added by a script at runtime does not count.
            ["evento"] = a => SceneProperty(new[] { a[0], a[1], a[2] + ".m_PersistentCalls.m_Calls" }) is SerializedProperty calls
                              && Enumerable.Range(0, calls.arraySize).Select(calls.GetArrayElementAtIndex)
                                  .Any(c => c.FindPropertyRelative("m_MethodName").stringValue == a[3] && c.FindPropertyRelative("m_Target").objectReferenceValue != null),
            // What a component is doing in Play: a door open, a trigger fired, a speed set from a menu.
            ["stato"] = a => EditorApplication.isPlaying && SceneComponent(a[0], a[1]) is Component c && Member(c, a[2]) is object v
                             && (v is bool b ? b == (a[3] == "sì") : Between(Convert.ToSingle(v), a[3], a[4])),
            ["afferrato"] = a => EditorApplication.isPlaying && SceneComponent(a[0], nameof(XRGrabInteractable)) is XRGrabInteractable g && g.isSelected,
            ["finestra-aperta"] = a => Resources.FindObjectsOfTypeAll<EditorWindow>().Any(w => w.titleContent.text == a[0]),
            ["componente-su"] = a => TypeNamed(a[0]) is Type t && Surface(a[1]) is Collider c
                                     && UnityEngine.Object.FindObjectsByType(t, FindObjectsSortMode.None).Any(x => CourseGoal.Above(c, ((Component)x).transform.position)),
            ["ambiente-intatto"] = a => GameObject.Find(a[0]) is GameObject o && o.TryGetComponent(out EnvironmentSnapshot s) && s.Changes().Count == 0,
            ["riuscito"] = a => GameObject.Find(a[0]) is GameObject o && o.TryGetComponent(out CourseGoal g) && g.reached,
            // The measures of a blockout (LevelChecks), read on the colliders and only out of Play: a cure made in Play is lost when Play stops.
            ["varco"] = a => !EditorApplication.isPlaying && GameObject.Find(a[0]) is GameObject m && LevelChecks.Width(m.transform) is float w && LevelChecks.Clearance(m.transform) is float h
                             && Between(w, a[1], a[2]) && Between(h, a[3], a[4]),
            ["altezza-libera"] = a => !EditorApplication.isPlaying && GameObject.Find(a[0]) is GameObject m && LevelChecks.Clearance(m.transform) is float h && Between(h, a[1], a[2]),
            ["larghezza"] = a => !EditorApplication.isPlaying && GameObject.Find(a[0]) is GameObject m && LevelChecks.Width(m.transform) is float w && Between(w, a[1], a[2]),
            ["percorso-liscio"] = a => !EditorApplication.isPlaying && GameObject.Find(a[0]) is GameObject from && GameObject.Find(a[1]) is GameObject to
                                       && LevelChecks.Roughness(from.transform, to.transform) is float jump && jump <= Number(a[2]) + 0.0001f,
            ["teleport-sotto"] = a => !EditorApplication.isPlaying && a.All(name => GameObject.Find(name) is GameObject m && LevelChecks.TeleportUnder(m.transform)),
            ["rivolto-verso"] = a => !EditorApplication.isPlaying && GameObject.Find(a[0]) is GameObject o && GameObject.Find(a[1]) is GameObject t && LevelChecks.Facing(o.transform, t.transform) <= Number(a[2]),
            ["luce-vicino"] = a => !EditorApplication.isPlaying && GameObject.Find(a[0]) is GameObject o && LevelChecks.LightNear(o.transform, Number(a[1])),
            ["altezza"] = a => !EditorApplication.isPlaying && GameObject.Find(a[0]) is GameObject o && LevelChecks.Height(o.transform) is float y && Between(y, a[1], a[2]),
            ["dimensione"] = a => !EditorApplication.isPlaying && GameObject.Find(a[0]) is GameObject o && LevelChecks.Size(o.transform, a[1]) is float s && Between(s, a[2], a[3]),
            ["file-compilato"] = a => LevelChecks.FileFilled(Path.Combine(LevelChecks.Repository, a[0]), a[1]),
            // The sky and the fog of the Lighting window: a panorama takes its depth from both
            ["panorama"] = a => !EditorApplication.isPlaying && RenderSettings.skybox != null && RenderSettings.skybox.name == a[0]
                                && RenderSettings.fog && RenderSettings.fogMode == FogMode.Linear && RenderSettings.fogEndDistance >= Number(a[1]),
            ["mani-pronte"] = _ => HandsSetup.Ready(),
            ["modalita-mani"] = _ => EditorApplication.isPlaying && XRInputModalityManager.currentInputMode.Value == XRInputModalityManager.InputMode.TrackedHand,
        };

        /// <summary>Where a Multi-Anchor Volume keeps its filter, the Destination Filter Object of its Destination Evaluation Settings.</summary>
        public const string GazeFilterField = "m_DestinationEvaluationSettings.m_ConstantValue.m_DestinationFilterObject";

        /// <summary>The checks that can pass only in Play: in a list chapter they stay done once they pass.</summary>
        public static readonly HashSet<string> PlayOnly = new HashSet<string> { "in-play", "sopra-oggetto", "sopra-area-filtrata", "riuscito", "su-anchor", "stato", "afferrato", "modalita-mani" };

        static readonly Dictionary<string, Action<string[]>> Actions = new Dictionary<string, Action<string[]>>
        {
            ["importa-sample"] = a => ImportSample(a[0]),
            ["apri-scena"] = a => OpenSampleScene(a[0], a[1]),
            ["crea-progetto"] = _ => ProjectScene.Create(ProjectScene.Folder),
            ["prepara-mani"] = _ => HandsSetup.Prepare(),
            ["crea-concept"] = a => EditorUtility.OpenWithDefaultApp(LevelChecks.CreateConcept(a[0])),
            ["riferimenti-scala"] = _ => LevelChecks.AddScaleReferences(),
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

        // On the named object (a path works too) or one of its children, by type name.
        static Component SceneComponent(string name, string type) =>
            GameObject.Find(name) is GameObject o ? o.GetComponentsInChildren<Component>(true).FirstOrDefault(x => x != null && x.GetType().Name == type) : null;

        // Out of Play only: in Play the objects are copies, and what changes there is lost when Play stops.
        static SerializedProperty SceneProperty(string[] a) =>
            !EditorApplication.isPlaying && SceneComponent(a[0], a[1]) is Component c ? new SerializedObject(c).FindProperty(a[2]) : null;

        // A public property or field, read in Play.
        static object Member(Component c, string name) => c.GetType().GetProperty(name)?.GetValue(c) ?? c.GetType().GetField(name)?.GetValue(c);

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
            HandsSetup.PrepareIfNeeded();   // a practice built on the rig with hands, copied by someone who skipped "Prepara le mani"
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
