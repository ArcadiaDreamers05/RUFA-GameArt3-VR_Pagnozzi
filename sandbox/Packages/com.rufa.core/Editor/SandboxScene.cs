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

            ScaleReferences(Vector3.zero, Quaternion.identity);

            var bootstrap = new GameObject("Rig Bootstrap").AddComponent<RigBootstrap>();
            bootstrap.xrRigPrefab = xrRigPrefab;

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Debug.Log($"RUFA: scene saved to {ScenePath} and set as the only scene in Build Settings.");
        }

        public const string MannequinName = "Scale Reference - Mannequin 1.75 m";
        public const string DoorName = "Scale Reference - Door 80 x 210 cm";
        public const string PlayAreaName = "Scale Reference - Play Area 2 x 2 m";
        public const string PlayAreaMaterial = "Packages/com.rufa.core/Materials/Play Area.mat";   // unlit yellow: a grey line on a grey floor would not show

        /// <summary>The course's scale references, 3 m in front of a place looking along `facing`: a 1.75 m mannequin on the right, an 80 x 210 cm door on the left.</summary>
        public static void ScaleReferences(Vector3 origin, Quaternion facing)
        {
            var mannequin = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            mannequin.name = MannequinName;
            mannequin.transform.localScale = new Vector3(0.4f, 0.875f, 0.4f); // a Capsule is 2 m tall
            mannequin.transform.SetPositionAndRotation(origin + facing * new Vector3(2f, 0.875f, 3f), facing);

            var door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = DoorName;
            door.transform.localScale = new Vector3(0.8f, 2.1f, 0.05f);
            door.transform.SetPositionAndRotation(origin + facing * new Vector3(-2f, 1.05f, 3f), facing);

            var area = new GameObject(PlayAreaName).transform;   // the play area Meta asks to design for, standing: 2 x 2 m around the start
            area.SetPositionAndRotation(origin, facing);
            var paint = AssetDatabase.LoadAssetAtPath<Material>(PlayAreaMaterial);
            foreach (var (center, size) in new[]
            {
                (new Vector3(0f, 0.003f, 1f), new Vector3(2.04f, 0.005f, 0.04f)), (new Vector3(0f, 0.003f, -1f), new Vector3(2.04f, 0.005f, 0.04f)),
                (new Vector3(1f, 0.003f, 0f), new Vector3(0.04f, 0.005f, 2f)), (new Vector3(-1f, 0.003f, 0f), new Vector3(0.04f, 0.005f, 2f)),
            })
            {
                var side = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.DestroyImmediate(side.GetComponent<Collider>());   // a line on the floor, not a step
                side.GetComponent<Renderer>().sharedMaterial = paint;
                side.transform.SetParent(area, false);
                side.transform.localPosition = center;
                side.transform.localScale = size;
            }
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
