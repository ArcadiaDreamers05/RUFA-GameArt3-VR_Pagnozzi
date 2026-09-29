using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Rufa
{
    /// <summary>
    /// Builds the first-person desktop rig at runtime, entirely in code:
    /// CharacterController + camera + XRI ray interactor + a world-space crosshair.
    /// No prefab, so there is nothing to keep in sync with the VR rig.
    /// </summary>
    public static class DesktopRig
    {
        public const float Height = 1.75f;     // same as the mannequin in the scene
        public const float EyeHeight = 1.65f;
        public const float Radius = 0.3f;
        public const float ReachMeters = 3f;   // how far the desktop "hand" ray reaches

        const int IgnoreRaycastLayer = 2;      // built-in Unity layer: keeps the ray off our own collider

        public static GameObject Build(Vector3 position, Quaternion rotation)
        {
            var root = new GameObject("Desktop Rig");
            root.layer = IgnoreRaycastLayer;
            root.transform.SetPositionAndRotation(position, rotation);

            var controller = root.AddComponent<CharacterController>();
            controller.height = Height;
            controller.radius = Radius;
            controller.center = new Vector3(0f, Height * 0.5f, 0f);

            var head = new GameObject("Main Camera");
            head.tag = "MainCamera";
            head.layer = IgnoreRaycastLayer;
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0f, EyeHeight, 0f);
            var camera = head.AddComponent<Camera>();
            camera.nearClipPlane = 0.05f;
            head.AddComponent<AudioListener>();

            // Same interactor family as the VR controllers, so the same interactables work in both modes.
            var ray = head.AddComponent<XRRayInteractor>();
            ray.rayOriginTransform = head.transform;
            ray.maxRaycastDistance = ReachMeters;
            ray.raycastMask = ~(1 << IgnoreRaycastLayer);
            ray.enableUIInteraction = false;
            ray.interactionLayers = 1;           // Default only, like the Starter Assets hands: never the teleport layer
            ray.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            ray.activateInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;

            // The desktop "hand": what you pick up comes here, low on the right and in view, like in a first-person game.
            var hand = new GameObject("Mano").transform;
            hand.SetParent(head.transform, false);
            hand.localPosition = new Vector3(0.12f, -0.12f, 0.4f);
            hand.localRotation = Quaternion.Euler(-20f, -25f, 0f);  // a long object, like a key, shows its length instead of pointing away
            ray.attachTransform = hand;
            ray.useForceGrab = true;

            root.AddComponent<DesktopMover>().Init(controller, head.transform, ray, BuildCrosshair(head.transform));
            return root;
        }

        // A 6 mm white dot half a metre in front of the eyes. World-space UI, like everything else in the course.
        static Image BuildCrosshair(Transform head)
        {
            var canvasGo = new GameObject("Crosshair", typeof(Canvas));
            canvasGo.transform.SetParent(head, false);
            canvasGo.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            canvasGo.transform.localScale = Vector3.one * 0.001f;  // canvas units become millimetres
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

            var dot = new GameObject("Dot", typeof(Image));
            dot.transform.SetParent(canvasGo.transform, false);
            dot.GetComponent<RectTransform>().sizeDelta = new Vector2(6f, 6f);
            var image = dot.GetComponent<Image>();
            image.color = Color.white;
            return image;
        }
    }
}
