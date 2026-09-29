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

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log("RUFA: mini examples added (narrative trigger; key -> lock -> door). Save the scene.");
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
