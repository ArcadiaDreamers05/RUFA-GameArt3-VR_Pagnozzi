using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Rufa.Editor
{
    /// <summary>Generates the starting scene of the sandbox from code, so it is reproducible and needs no hand-made YAML.</summary>
    public static class SandboxScene
    {
        public const string ScenePath = "Assets/Scenes/Sandbox.unity";

        const string XrRigPrefabName = "XR Origin (XR Rig)";
        static readonly string[] TemplateLeftovers =
        {
            "Assets/TutorialInfo",
            "Assets/Readme.asset",
            "Assets/Scenes/SampleScene.unity",
        };

        [MenuItem("RUFA/Setup/2 - Crea scena Sandbox")]
        public static void Create()
        {
            var xrRigPrefab = FindXrRigPrefab();
            if (xrRigPrefab == null) return; // error already logged; nothing is written
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            foreach (var path in TemplateLeftovers)
                AssetDatabase.DeleteAsset(path); // no-op when the asset is already gone

            var templateCamera = GameObject.Find("Main Camera");
            if (templateCamera != null)
                Object.DestroyImmediate(templateCamera); // each rig brings its own camera

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(2f, 1f, 2f); // a Plane is 10 m wide, so 20 x 20 m

            var mannequin = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            mannequin.name = "Scale Reference - Mannequin 1.75 m";
            mannequin.transform.localScale = new Vector3(0.4f, 0.875f, 0.4f); // a Capsule is 2 m tall
            mannequin.transform.position = new Vector3(2f, 0.875f, 3f);

            var door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "Scale Reference - Door 80 x 210 cm";
            door.transform.localScale = new Vector3(0.8f, 2.1f, 0.05f);
            door.transform.position = new Vector3(-2f, 1.05f, 3f);

            var bootstrap = new GameObject("Rig Bootstrap").AddComponent<RigBootstrap>();
            bootstrap.xrRigPrefab = xrRigPrefab;

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Debug.Log($"RUFA: scene saved to {ScenePath} and set as the only scene in Build Settings.");
        }

        public static GameObject FindXrRigPrefab()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab XR Origin"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == XrRigPrefabName)
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            Debug.LogError($"RUFA: prefab '{XrRigPrefabName}' not found. Run 'RUFA/Setup/1' first and check that the XRI 'Starter Assets' sample is under Assets/Samples.");
            return null;
        }
    }
}
