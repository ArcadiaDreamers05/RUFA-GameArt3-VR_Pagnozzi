using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
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
            Save(Lever());
            Save(Chest());
            Save(Crank());
            Save(Knob());
            Save(Slider());
            Save(Switch());
            Save(Keypad());

            AssetDatabase.SaveAssets();
            Debug.Log($"RUFA: 15 prefabs written to {Folder}.");
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
                "Grigio" => (new Color(0.55f, 0.55f, 0.55f), 0f, 0.2f),
                "Rosso" => (new Color(0.8f, 0.2f, 0.15f), 0f, 0.4f),
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

        // A primitive inside parent at a local position and scale, painted; some are only to see, without a collider.
        static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, string paint, bool solid = true)
        {
            var part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            part.GetComponent<Renderer>().sharedMaterial = Paint(paint);
            if (!solid) Object.DestroyImmediate(part.GetComponent<Collider>());
            return part;
        }

        // A flat cylinder facing -Z. The capsule of a cylinder primitive this flat would be a ball as wide as the disc:
        // a convex mesh collider keeps its shape.
        static GameObject Disc(Transform parent, string name, Vector3 position, float diameter, float thickness, string paint)
        {
            var disc = Part(parent, name, PrimitiveType.Cylinder, position, new Vector3(diameter, thickness / 2f, diameter), paint, false);
            disc.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            disc.AddComponent<MeshCollider>().convex = true;
            return disc;
        }

        static Transform Point(Transform parent, string name, Vector3 position)
        {
            var point = new GameObject(name).transform;
            point.SetParent(parent, false);
            point.localPosition = position;
            return point;
        }

        // The hand moves the part with physics, so its joint keeps it on its way; the joint turns it, not the wrist. Let
        // go, it stays where it is.
        static void TakenByHand(GameObject part, Transform attach)
        {
            var grab = part.AddComponent<XRGrabInteractable>();
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.trackRotation = false;
            grab.throwOnDetach = false;
            grab.attachTransform = attach;
        }

        // Velocity Tracking moves the centre of mass of the body. A wheel has it on its axle, where the hinge holds it
        // still, and the hand would not turn it: on the handle, the pull of the hand turns the wheel round its hinge.
        static void DrivenBy(Rigidbody body, Vector3 handle)
        {
            body.automaticCenterOfMass = false;
            body.centerOfMass = handle;
        }

        // A lever on a post 1 m high: the arm turns on a hinge across X from -45° to 45°, and pulled with its top towards
        // -Z past 35° it fires On Pulled. No gravity: it stays where the hand leaves it. The limits act 10° before the
        // stop, or a hard pull would swing it past.
        static GameObject Lever()
        {
            var root = new GameObject("Leva").transform;
            Part(root, "Base", PrimitiveType.Cube, new Vector3(0f, 0.5f, 0f), new Vector3(0.3f, 1f, 0.3f), "Grigio");
            Part(root, "Cardine", PrimitiveType.Cylinder, new Vector3(0f, 1.03f, 0f), new Vector3(0.06f, 0.1f, 0.06f), "Ottone", false)
                .transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            var arm = new GameObject("Braccio", typeof(Lever)).transform;   // its hinge and body come with it
            arm.SetParent(root, false);
            arm.localPosition = new Vector3(0f, 1.03f, 0f);
            arm.GetComponent<Rigidbody>().useGravity = false;
            var hinge = arm.GetComponent<HingeJoint>();
            hinge.axis = Vector3.left;
            hinge.useLimits = true;
            hinge.limits = new JointLimits { min = -45f, max = 45f, contactDistance = 10f };
            Part(arm, "Asta", PrimitiveType.Cylinder, new Vector3(0f, 0.155f, 0f), new Vector3(0.05f, 0.125f, 0.05f), "Grigio");
            Part(arm, "Pomello", PrimitiveType.Sphere, new Vector3(0f, 0.28f, 0f), Vector3.one * 0.07f, "Rosso");
            TakenByHand(arm.gameObject, Point(arm, "Attach", new Vector3(0f, 0.28f, 0f)));
            return root.gameObject;
        }

        // A chest open at the front (-Z) with a drawer that runs 35 cm out on a Configurable Joint tied to the chest: X and
        // Y locked, Z limited to 17.5 cm either side of the middle of the run, no rotation, the limit acting 5 cm before the
        // stop. The chest is a kinematic body, so the joint's anchor lives in its space and the block can stand anywhere.
        static GameObject Chest()
        {
            var chest = new GameObject("Mobile");
            chest.AddComponent<Rigidbody>().isKinematic = true;
            foreach (var (board, position, size) in new[] { ("Fianco ovest", new Vector3(-0.385f, 0.4f, 0f), new Vector3(0.03f, 0.8f, 0.5f)),
                                                            ("Fianco est", new Vector3(0.385f, 0.4f, 0f), new Vector3(0.03f, 0.8f, 0.5f)),
                                                            ("Cielo", new Vector3(0f, 0.785f, 0f), new Vector3(0.8f, 0.03f, 0.5f)),
                                                            ("Fondo", new Vector3(0f, 0.535f, 0f), new Vector3(0.74f, 0.03f, 0.5f)),
                                                            ("Schiena", new Vector3(0f, 0.4f, 0.24f), new Vector3(0.74f, 0.8f, 0.02f)) })
                Part(chest.transform, board, PrimitiveType.Cube, position, size, "Legno");
            var tray = new GameObject("Cassetto").transform;
            tray.SetParent(chest.transform, false);
            tray.localPosition = new Vector3(0f, 0.66f, -0.025f);   // closed, its front flush with the chest
            foreach (var (board, position, size) in new[] { ("Cassetto - fondo", new Vector3(0f, -0.09f, 0f), new Vector3(0.7f, 0.02f, 0.45f)),
                                                            ("Cassetto - frontale", new Vector3(0f, 0f, -0.215f), new Vector3(0.7f, 0.2f, 0.02f)),
                                                            ("Cassetto - retro", new Vector3(0f, 0f, 0.215f), new Vector3(0.7f, 0.2f, 0.02f)),
                                                            ("Cassetto - fianco ovest", new Vector3(-0.34f, 0f, 0f), new Vector3(0.02f, 0.2f, 0.41f)),
                                                            ("Cassetto - fianco est", new Vector3(0.34f, 0f, 0f), new Vector3(0.02f, 0.2f, 0.41f)) })
                Part(tray, board, PrimitiveType.Cube, position, size, "Legno");
            Part(tray, "Maniglia del cassetto", PrimitiveType.Cube, new Vector3(0f, 0.02f, -0.24f), new Vector3(0.14f, 0.03f, 0.03f), "Ottone");
            tray.gameObject.AddComponent<Rigidbody>().linearDamping = 2f;
            var joint = tray.gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = chest.GetComponent<Rigidbody>();
            joint.xMotion = joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Limited;
            joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Locked;
            joint.linearLimit = new SoftJointLimit { limit = 0.175f, contactDistance = 0.05f };
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = tray.localPosition + Vector3.back * 0.175f;
            tray.gameObject.AddComponent<Slide>();
            TakenByHand(tray.gameObject, Point(tray, "Attach", new Vector3(0f, 0.02f, -0.24f)));
            return chest;
        }

        // A wheel 30 cm across on a post, facing -Z, with a handle 12 cm off its centre: a hinge on its axis with no
        // limits, so it turns round and round. Its axis points at whoever faces it: turned clockwise, the turns
        // count up.
        static GameObject Crank()
        {
            var root = new GameObject("Manovella").transform;
            Part(root, "Supporto", PrimitiveType.Cube, new Vector3(0f, 0.55f, 0f), new Vector3(0.1f, 1.1f, 0.1f), "Grigio");
            var wheel = new GameObject("Ruota", typeof(Crank)).transform;
            wheel.SetParent(root, false);
            wheel.localPosition = new Vector3(0f, 1f, -0.07f);   // a centimetre off the post: rubbing on it would brake the wheel
            var body = wheel.GetComponent<Rigidbody>();
            body.useGravity = false;
            body.angularDamping = 1f;   // let go, it slows down
            wheel.GetComponent<HingeJoint>().axis = Vector3.back;
            Disc(wheel, "Disco", Vector3.zero, 0.3f, 0.02f, "Legno");
            Part(wheel, "Maniglia", PrimitiveType.Cylinder, new Vector3(0.12f, 0f, -0.05f), new Vector3(0.03f, 0.04f, 0.03f), "Rosso")
                .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            TakenByHand(wheel.gameObject, Point(wheel, "Attach", new Vector3(0.12f, 0f, -0.05f)));
            DrivenBy(body, new Vector3(0.12f, 0f, -0.05f));
            return root.gameObject;
        }

        // A knob 10 cm across on a plate facing -Z: a hinge from 0° to 170°; the hand takes the notch on its rim and moves
        // it round, clockwise from 0 like the knob of a stove.
        static GameObject Knob()
        {
            var root = new GameObject("Manopola").transform;
            Part(root, "Placca", PrimitiveType.Cube, Vector3.zero, new Vector3(0.15f, 0.15f, 0.02f), "Grigio");
            var disc = new GameObject("Disco", typeof(Knob)).transform;
            disc.SetParent(root, false);
            disc.localPosition = new Vector3(0f, 0f, -0.03f);
            var body = disc.GetComponent<Rigidbody>();
            body.useGravity = false;
            body.angularDamping = 5f;
            var hinge = disc.GetComponent<HingeJoint>();
            hinge.axis = Vector3.back;
            hinge.useLimits = true;
            hinge.limits = new JointLimits { min = 0f, max = 170f, contactDistance = 10f };
            Disc(disc, "Corpo", Vector3.zero, 0.1f, 0.03f, "Legno");
            Part(disc, "Tacca", PrimitiveType.Cube, new Vector3(0f, 0.035f, -0.02f), new Vector3(0.015f, 0.03f, 0.015f), "Rosso");
            TakenByHand(disc.gameObject, Point(disc, "Attach", new Vector3(0f, 0.035f, -0.02f)));
            DrivenBy(body, new Vector3(0f, 0.035f, -0.02f));
            return root.gameObject;
        }

        // A track 50 cm long facing -Z, a kinematic body like the chest, and a cursor that runs along it on a Configurable
        // Joint: only X, limited to 22 cm either side of the middle, starting at the left end.
        static GameObject Slider()
        {
            var track = new GameObject("Slider");
            track.AddComponent<Rigidbody>().isKinematic = true;
            Part(track.transform, "Guida", PrimitiveType.Cube, Vector3.zero, new Vector3(0.5f, 0.03f, 0.05f), "Grigio");
            var cursor = Part(track.transform, "Cursore", PrimitiveType.Cube, new Vector3(-0.22f, 0f, -0.055f), Vector3.one * 0.06f, "Rosso");
            var body = cursor.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.linearDamping = 5f;
            var joint = cursor.AddComponent<ConfigurableJoint>();
            joint.anchor = Vector3.zero;   // on a cube, a new joint starts on the top face
            joint.connectedBody = track.GetComponent<Rigidbody>();
            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Locked;
            joint.linearLimit = new SoftJointLimit { limit = 0.22f, contactDistance = 0.05f };
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = new Vector3(0f, 0f, -0.055f);   // the middle of the track, in its space
            cursor.AddComponent<Slide>();
            TakenByHand(cursor, Point(cursor.transform, "Attach", Vector3.zero));
            return track;
        }

        // A switch plate facing -Z with a small handle that tips up and down at each selection.
        static GameObject Switch()
        {
            var root = new GameObject("Interruttore");
            root.AddComponent<BoxCollider>().size = new Vector3(0.08f, 0.12f, 0.03f);
            Part(root.transform, "Placca", PrimitiveType.Cube, Vector3.zero, new Vector3(0.08f, 0.12f, 0.02f), "Grigio", false);
            var handle = Point(root.transform, "Levetta", new Vector3(0f, 0f, -0.01f));
            Part(handle, "Asta", PrimitiveType.Cylinder, new Vector3(0f, 0f, -0.02f), new Vector3(0.015f, 0.02f, 0.015f), "Rosso", false)
                .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            root.AddComponent<ToggleSwitch>().handle = handle;   // its interactable comes with it
            return root;
        }

        // A keypad 36 x 52 cm in World Space: digits 0-9 and a display. The code is 1234. Two raycasters: the Tracked
        // Device one for the XR rays, the Graphic one for the mouse, in the PC version and in the simulator.
        static GameObject Keypad()
        {
            var go = new GameObject("Tastierino", typeof(Canvas), typeof(TrackedDeviceGraphicRaycaster), typeof(GraphicRaycaster));
            go.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 520f);
            go.transform.localScale = Vector3.one * 0.001f;
            var background = new GameObject("Sfondo", typeof(Image));
            background.transform.SetParent(go.transform, false);
            Stretch(background.GetComponent<RectTransform>());
            background.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.12f, 0.9f);
            var keypad = go.AddComponent<Keypad>();
            keypad.code = "1234";
            keypad.display = Label(go.transform, "Display", "", new Vector2(0f, 200f), new Vector2(320f, 80f), 60f);
            for (var n = 0; n <= 9; n++)
            {
                var key = new GameObject("Tasto " + n, typeof(Image), typeof(Button));
                key.transform.SetParent(go.transform, false);
                var rect = key.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(90f, 70f);
                rect.anchoredPosition = n == 0 ? new Vector2(0f, -165f) : new Vector2((n - 1) % 3 * 110f - 110f, 90f - (n - 1) / 3 * 85f);   // 1 2 3, 4 5 6, 7 8 9, 0
                var button = key.GetComponent<Button>();
                button.targetGraphic = key.GetComponent<Image>();
                Label(key.transform, "Etichetta", n.ToString(), Vector2.zero, new Vector2(90f, 70f), 40f).color = Color.black;
                UnityEventTools.AddStringPersistentListener(button.onClick, keypad.Press, n.ToString());
            }
            return go;
        }

        static TextMeshProUGUI Label(Transform parent, string name, string text, Vector2 position, Vector2 size, float fontSize)
        {
            var label = new GameObject(name, typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            label.transform.SetParent(parent, false);
            label.rectTransform.anchoredPosition = position;
            label.rectTransform.sizeDelta = size;
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            return label;
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
