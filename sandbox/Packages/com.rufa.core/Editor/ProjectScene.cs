using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Rufa.Editor
{
    /// <summary>
    /// The student's exam project scene, created once by the lesson 2 tutorial ("> azione: ... | crea-progetto") and then
    /// built on lesson after lesson. It lives in Assets/Progetto, which "git aggiorna" never touches.
    /// </summary>
    public static class ProjectScene
    {
        public const string Folder = "Assets/Progetto";

        /// <summary>
        /// Creates, only when missing, the project rig (a variant of the Starter Assets rig, with its default values) and
        /// the bare scene: Rig Bootstrap, light, default sky, a 20 x 20 m floor that takes the teleport. Then opens the
        /// scene and puts it first in the build. Existing files are never touched.
        /// </summary>
        public static void Create(string folder)
        {
            string scenePath = folder + "/Progetto.unity", rigPath = folder + "/XR Origin (progetto).prefab";
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder(Path.GetDirectoryName(folder).Replace('\\', '/'), Path.GetFileName(folder));
            var rig = AssetDatabase.LoadAssetAtPath<GameObject>(rigPath);
            if (rig == null)
            {
                var baseRig = SandboxScene.FindXrRigPrefab();
                if (baseRig == null) return; // error already logged
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(baseRig);
                rig = PrefabUtility.SaveAsPrefabAsset(instance, rigPath);
                Object.DestroyImmediate(instance);
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single); // light and sky
                Object.DestroyImmediate(GameObject.Find("Main Camera")); // the rig brings its own camera
                var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "Pavimento";
                floor.transform.localScale = new Vector3(2f, 1f, 2f); // a Plane is 10 m wide, so 20 x 20 m
                floor.AddComponent<TeleportationArea>().interactionLayers = 1 << BuildingBlocks.TeleportLayer;
                new GameObject("Rig Bootstrap").AddComponent<RigBootstrap>().xrRigPrefab = rig;
                EditorSceneManager.SaveScene(scene, scenePath);
            }
            else EditorSceneManager.OpenScene(scenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) }
                .Concat(EditorBuildSettings.scenes.Where(s => s.path != scenePath)).ToArray();
        }
    }
}
