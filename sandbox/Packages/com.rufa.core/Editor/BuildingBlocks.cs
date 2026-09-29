using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Rufa.Editor
{
    /// <summary>Generates the ready-to-use prefabs students drag into their scenes, and names the interaction layers they rely on.</summary>
    public static class BuildingBlocks
    {
        public const string Folder = "Assets/RUFA/Prefabs";
        public const string MaterialsFolder = "Assets/RUFA/Materials";
        const string ModelsFolder = "Packages/com.rufa.core/Models/"; // CC0 models, see CREDITS.md there
        public const int KeyLayer = 1;
        public const int TeleportLayer = 31;

        [MenuItem("RUFA/Setup/3 - Crea prefab mattoncino")]
        public static void Create()
        {
            NameInteractionLayers();
            EnsureFolder(Folder);
            EnsureFolder(MaterialsFolder);

            Save(Grabbable("Afferrabile", new Vector3(0.1f, 0.1f, 0.1f), Vector3.zero, 1 << 0));
            Save(Key());
            Save(Socket());
            Save(Door());
            Save(Button());
            Save(Trigger());
            Save(Panel());
            Save(SpatialAudio());

            AssetDatabase.SaveAssets();
            Debug.Log($"RUFA: 8 prefabs written to {Folder}.");
        }

        public static GameObject Load(string prefabName) =>
            AssetDatabase.LoadAssetAtPath<GameObject>($"{Folder}/{prefabName}.prefab");

        static void Save(GameObject temp)
        {
            PrefabUtility.SaveAsPrefabAsset(temp, $"{Folder}/{temp.name}.prefab");
            Object.DestroyImmediate(temp);
        }

        static GameObject Grabbable(string name, Vector3 size, Vector3 attachOffset, int layers)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.localScale = size;
            go.AddComponent<Rigidbody>().mass = 0.5f;
            var attach = new GameObject("Attach").transform;
            attach.SetParent(go.transform, false);
            attach.localPosition = new Vector3(attachOffset.x / size.x, attachOffset.y / size.y, attachOffset.z / size.z);
            var grab = go.AddComponent<XRGrabInteractable>();
            grab.attachTransform = attach;
            grab.useDynamicAttach = false;
            grab.interactionLayers = layers;
            return go;
        }

        // An old key, 15 cm long, lying flat, blade along +Z, held by the bow.
        static GameObject Key()
        {
            var go = new GameObject("Chiave");
            var model = Model("key_01.fbx", go.transform, "Ottone").transform;
            model.localRotation = Quaternion.Euler(90f, 0f, 0f); // the model stands along Y
            model.localScale = Vector3.one * 1.5f;               // the model is 10 cm: an antique key is bigger, and a small one is lost on the floor
            go.AddComponent<BoxCollider>().size = new Vector3(0.06f, 0.03f, 0.165f); // a bit larger than the key: easier to pick up
            var body = go.AddComponent<Rigidbody>();
            body.mass = 0.1f;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; // thin: would fall through the floor when dropped
            var attach = new GameObject("Attach").transform;
            attach.SetParent(go.transform, false);
            attach.localPosition = new Vector3(0f, 0f, -0.057f); // the bow
            var grab = go.AddComponent<XRGrabInteractable>();
            grab.attachTransform = attach;
            grab.useDynamicAttach = false;
            grab.interactionLayers = (1 << 0) | (1 << KeyLayer);
            return go;
        }

        // A keyhole plate for the face of a door: local +Z points out of the door. The key goes in straight, bit down.
        static GameObject Socket()
        {
            var go = new GameObject("Serratura");
            Model("drawer_key_hole.fbx", go.transform, "Ottone").transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // the model lies flat
            var zone = go.AddComponent<BoxCollider>();   // the trigger volume the socket senses with
            zone.isTrigger = true;
            zone.center = new Vector3(0f, 0f, 0.06f);
            zone.size = new Vector3(0.2f, 0.2f, 0.15f);
            var attach = new GameObject("Attach").transform;
            attach.SetParent(go.transform, false);
            attach.localPosition = new Vector3(0f, 0f, 0.09f); // the bow stays out, the blade ends 4 cm into the 5 cm door
            attach.localRotation = Quaternion.LookRotation(Vector3.back, Vector3.left); // blade into the door, bit down
            var socket = go.AddComponent<XRSocketInteractor>();
            socket.attachTransform = attach;
            socket.interactionLayers = 1 << KeyLayer;
            socket.showInteractableHoverMeshes = true;
            go.AddComponent<Lock>();
            return go;
        }

        // A 90 x 210 x 5 cm panelled door with a knob on each face. The leaf's pivot is on the hinge.
        static GameObject Door()
        {
            var root = new GameObject("Porta");
            var leaf = new GameObject("Anta");
            leaf.transform.SetParent(root.transform, false);
            var panel = Model("door_04.fbx", leaf.transform, "Legno"); // pivot at the bottom centre
            panel.name = "Pannello";
            var size = panel.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            panel.transform.localScale = new Vector3(0.9f / size.x, 2.1f / size.y, 0.05f / size.z);
            panel.transform.localPosition = new Vector3(0.45f, 0f, 0f);
            var panelCollider = panel.AddComponent<BoxCollider>(); // fits the mesh
            foreach (var side in new[] { 1f, -1f })
            {
                var knob = Model("doorknob.fbx", leaf.transform, "Ottone");
                knob.name = side > 0f ? "Pomello_A" : "Pomello_B";
                knob.transform.localPosition = new Vector3(0.835f, 1f, 0.025f * side);
                knob.transform.localRotation = Quaternion.Euler(0f, side > 0f ? 0f : 180f, 0f);
            }
            leaf.AddComponent<XRSimpleInteractable>().colliders.Add(panelCollider); // only the panel: a lock added to the leaf keeps its own
            var door = root.AddComponent<Door>();
            door.leaf = leaf.transform;
            return root;
        }

        static GameObject Model(string file, Transform parent, string material)
        {
            var model = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(ModelsFolder + file));
            model.transform.SetParent(parent, false);
            foreach (var r in model.GetComponentsInChildren<Renderer>())
                r.sharedMaterials = Enumerable.Repeat(Paint(material), r.sharedMaterials.Length).ToArray();
            return model;
        }

        // Plain URP materials for the models, created once in the project.
        static Material Paint(string name)
        {
            var path = $"{MaterialsFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null) return material;
            var (color, metallic, smoothness) = name switch
            {
                "Ottone" => (new Color(0.78f, 0.6f, 0.28f), 0.9f, 0.6f),
                _ => (new Color(0.42f, 0.27f, 0.16f), 0f, 0.3f), // Legno
            };
            material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = color };
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Smoothness", smoothness);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        static GameObject Button()
        {
            var root = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            root.name = "Pulsante";
            root.transform.localScale = new Vector3(0.1f, 0.01f, 0.1f);
            var cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cap.name = "Cappello";
            cap.transform.SetParent(root.transform, false);
            cap.transform.localPosition = new Vector3(0f, 2f, 0f);          // 2 cm above the base, in the base's scaled space
            cap.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
            root.AddComponent<XRSimpleInteractable>();
            var button = root.AddComponent<PushButton>();
            button.cap = cap.transform;
            return root;
        }

        static GameObject Trigger()
        {
            var go = new GameObject("TriggerNarrativo");
            var box = go.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(2f, 2f, 2f);
            go.AddComponent<NarrativeTrigger>();
            return go;
        }

        static GameObject Panel()
        {
            var go = new GameObject("PannelloUI", typeof(Canvas), typeof(TrackedDeviceGraphicRaycaster));
            go.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 400f);
            go.transform.localScale = Vector3.one * 0.001f;               // 0.6 x 0.4 m

            var background = new GameObject("Sfondo", typeof(Image));
            background.transform.SetParent(go.transform, false);
            Stretch(background.GetComponent<RectTransform>());
            background.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);

            var text = new GameObject("Testo", typeof(TextMeshProUGUI));
            text.transform.SetParent(go.transform, false);
            Stretch(text.GetComponent<RectTransform>(), 20f);
            var tmp = text.GetComponent<TextMeshProUGUI>();
            tmp.fontSize = 32f;
            tmp.color = Color.white;
            tmp.text = "Testo";
            return go;
        }

        static GameObject SpatialAudio()
        {
            var go = new GameObject("AudioSpaziale");
            var source = go.AddComponent<AudioSource>();
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 1f;
            source.maxDistance = 15f;
            source.loop = true;
            source.playOnAwake = true;
            source.dopplerLevel = 0f;
            return go;
        }

        static void Stretch(RectTransform rect, float margin = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(margin, margin);
            rect.offsetMax = new Vector2(-margin, -margin);
        }

        // InteractionLayerSettings is an internal XRI type: its Instance, reached by reflection, loads the settings asset
        // or creates it on a new project; the layer names are then edited through its serialized fields.
        static void NameInteractionLayers()
        {
            var type = typeof(UnityEngine.XR.Interaction.Toolkit.InteractionLayerMask).Assembly.GetType("UnityEngine.XR.Interaction.Toolkit.InteractionLayerSettings");
            var instance = type?.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)?.GetValue(null) as Object;
            if (instance == null)
            {
                Debug.LogWarning("RUFA: XRI interaction layer settings not found: name layer 1 'Chiave' and layer 31 'Teleport' in Project Settings > XR Interaction Toolkit.");
                return;
            }
            var settings = new SerializedObject(instance);
            var names = settings.FindProperty("m_LayerNames");
            if (names.arraySize < 32) names.arraySize = 32;
            names.GetArrayElementAtIndex(KeyLayer).stringValue = "Chiave";
            names.GetArrayElementAtIndex(TeleportLayer).stringValue = "Teleport";
            settings.ApplyModifiedProperties();
        }

        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, System.IO.Path.GetFileName(path));
        }
    }
}
