using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Rufa.Editor
{
    /// <summary>Two working examples in the open scene, one per genre, plus the teleport area students need from lesson 2.</summary>
    public static class MiniExamples
    {
        public const string RootName = "Mini-esempi";

        [MenuItem("RUFA/Setup/4 - Aggiungi mini-esempi alla scena")]
        public static void Add()
        {
            var floor = GameObject.Find("Floor");
            if (floor == null) { Debug.LogError("RUFA: no 'Floor' in the open scene. Open Assets/Scenes/Sandbox.unity first."); return; }
            if (BuildingBlocks.Load("Chiave") == null) { Debug.LogError("RUFA: prefabs not found. Run 'RUFA/Setup/3 - Crea prefab mattoncino' first."); return; }

            if (floor.GetComponent<TeleportationArea>() == null)
                floor.AddComponent<TeleportationArea>().interactionLayers = 1 << BuildingBlocks.TeleportLayer;

            var old = GameObject.Find(RootName);
            if (old != null) Object.DestroyImmediate(old);
            var root = new GameObject(RootName).transform;

            // Walking simulator: walk into the volume, read the panel.
            var panel = Place("PannelloUI", root, new Vector3(2f, 1.6f, 4.2f), Quaternion.identity);
            var audio = Place("AudioSpaziale", root, new Vector3(2f, 1.2f, 4.2f), Quaternion.identity);
            var trigger = Place("TriggerNarrativo", root, new Vector3(2f, 1f, 2f), Quaternion.identity).GetComponent<NarrativeTrigger>();
            trigger.panel = panel.GetComponentInChildren<TMP_Text>();
            trigger.audioSource = audio.GetComponent<AudioSource>();
            trigger.text = "Benvenuto. Sei entrato in un trigger narrativo: da qui può partire un audio o un testo.";

            // Escape room: key on the floor, a locked door with the keyhole under the knob.
            Place("Chiave", root, new Vector3(-1f, 0.01f, 1f), Quaternion.identity);
            var door = Place("Porta", root, new Vector3(-4f, 0f, 3f), Quaternion.identity).GetComponent<Door>();
            door.openOnSelect = false; // locked: only the key opens it
            var lockBlock = Place("Serratura", door.transform.Find("Anta"), Vector3.zero, Quaternion.identity).GetComponent<Lock>();
            lockBlock.transform.localPosition = new Vector3(0.835f, 0.9f, -0.025f); // under the knob, on the face towards the start
            lockBlock.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            UnityEventTools.AddPersistentListener(lockBlock.onUnlocked, new UnityAction(door.Open)); // visible in the Inspector

            Mechanisms(root);

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log("RUFA: mini examples added (narrative trigger; key -> lock -> door; a row of mechanisms). Save the scene.");
        }

        // The mechanisms in a row at z = 7, each wired in the Inspector to something one sees, with a label that says how
        // to use it: select a block to see how it is connected.
        static void Mechanisms(Transform root)
        {
            Transform Example(string block, float x, string how)
            {
                var example = new GameObject("Esempio " + block).transform;
                example.SetParent(root, false);
                example.position = new Vector3(x, 0f, 7f);
                Text(example, "Cartellino", $"<b>{block}</b>\n{how}", new Vector3(0f, 2.1f, 0f), 1.2f).rectTransform.sizeDelta = new Vector2(1.4f, 0.6f);
                return example;
            }
            // Blocks for a wall hang on a stand, at 1.25 m
            GameObject OnStand(string block, Transform example)
            {
                var stand = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stand.name = "Leggio";
                stand.transform.SetParent(example, false);
                stand.transform.localPosition = new Vector3(0f, 0.8f, 0.06f);
                stand.transform.localScale = new Vector3(0.5f, 1.6f, 0.1f);
                return Place(block, example, example.position + new Vector3(0f, 1.25f, 0f), Quaternion.identity);
            }
            // A lamp behind the block: a stand and a point light
            Light Lamp(Transform example, bool on)
            {
                var lamp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                lamp.name = "Lampada";
                lamp.transform.SetParent(example, false);
                lamp.transform.localPosition = new Vector3(0.45f, 0.15f, 0.8f);
                lamp.transform.localScale = Vector3.one * 0.15f;
                var bulb = new GameObject("Luce", typeof(Light)).GetComponent<Light>();
                bulb.transform.SetParent(lamp.transform, false);
                bulb.transform.localPosition = Vector3.up * 3f;
                bulb.type = LightType.Point;
                bulb.range = 3f;
                bulb.intensity = 2f;
                bulb.color = new Color(1f, 0.85f, 0.6f);
                bulb.gameObject.SetActive(on);
                return bulb;
            }
            // A property's setter, as the Inspector offers it in the list of an event
            UnityAction<T> Setter<T>(Object target, string property) =>
                (UnityAction<T>)System.Delegate.CreateDelegate(typeof(UnityAction<T>), target, target.GetType().GetProperty(property).GetSetMethod());

            var example = Example("Leva", -4.5f, "Tirala verso di te: accende la lampada");
            var lever = Place("Leva", example, example.position, Quaternion.identity).GetComponentInChildren<Lever>();
            UnityEventTools.AddBoolPersistentListener(lever.onPulled, Lamp(example, false).gameObject.SetActive, true);

            example = Example("Mobile", -3f, "Apri il cassetto fino in fondo");
            var note = Text(example, "Cartello del cassetto", "Chiuso", new Vector3(0f, 1.1f, 0f), 1.5f);
            var drawer = Place("Mobile", example, example.position, Quaternion.identity).GetComponentInChildren<Slide>();
            UnityEventTools.AddStringPersistentListener(drawer.onEnd, Setter<string>(note, "text"), "Aperto!");

            example = Example("Manovella", -1.5f, "Tre giri: si apre la porta");
            var gate = Place("Porta", example, example.position + new Vector3(0.25f, 0f, 0.8f), Quaternion.identity).GetComponent<Door>();
            gate.transform.localScale = Vector3.one * 0.6f;
            gate.openOnSelect = false;
            var crank = Place("Manovella", example, example.position, Quaternion.identity).GetComponentInChildren<Crank>();
            UnityEventTools.AddPersistentListener(crank.onCompleted, gate.Open);

            example = Example("Manopola", 0f, "Girala: cambia la luce");
            var knobLight = Lamp(example, true);
            UnityEventTools.AddPersistentListener(OnStand("Manopola", example).GetComponentInChildren<Knob>().onValueChanged, Setter<float>(knobLight, "intensity"));

            example = Example("Slider", 1.5f, "Fallo scorrere: cambia la luce");
            var slideLight = Lamp(example, true);
            UnityEventTools.AddPersistentListener(OnStand("Slider", example).GetComponentInChildren<Slide>().onValueChanged, Setter<float>(slideLight, "intensity"));

            example = Example("Interruttore", 3f, "Cliccalo: accende e spegne");
            var switchLamp = Lamp(example, false).gameObject;
            var toggle = OnStand("Interruttore", example).GetComponent<ToggleSwitch>();
            UnityEventTools.AddBoolPersistentListener(toggle.onSwitchedOn, switchLamp.SetActive, true);
            UnityEventTools.AddBoolPersistentListener(toggle.onSwitchedOff, switchLamp.SetActive, false);

            example = Example("Tastierino", 4.5f, "Il codice è 1234: accende la lampada");
            var pad = OnStand("Tastierino", example);
            pad.transform.position += Vector3.back * 0.01f;   // just in front of the stand
            UnityEventTools.AddBoolPersistentListener(pad.GetComponent<Keypad>().onUnlocked, Lamp(example, false).gameObject.SetActive, true);
        }

        // A text in the world, read from -Z, where the player starts
        static TextMeshPro Text(Transform parent, string name, string text, Vector3 position, float size)
        {
            var label = new GameObject(name, typeof(TextMeshPro)).GetComponent<TextMeshPro>();
            label.transform.SetParent(parent, false);
            label.transform.localPosition = position;
            label.rectTransform.sizeDelta = new Vector2(1f, 0.3f);
            label.text = text;
            label.fontSize = size;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(0.15f, 0.15f, 0.15f);
            return label;
        }

        public static GameObject Place(string prefabName, Transform parent, Vector3 position, Quaternion rotation)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(BuildingBlocks.Load(prefabName));
            instance.transform.SetParent(parent, false);
            instance.transform.SetPositionAndRotation(position, rotation);
            return instance;
        }
    }
}
