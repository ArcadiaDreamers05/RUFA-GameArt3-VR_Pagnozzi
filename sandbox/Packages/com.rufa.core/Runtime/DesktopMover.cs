using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Rufa
{
    /// <summary>
    /// Keyboard + mouse control for the desktop rig: WASD or arrows to walk, mouse to look,
    /// left button = select (hold it to carry), right button = activate (use).
    /// Mouse buttons are forwarded to the XRI ray interactor, so interactables
    /// need no desktop-specific code.
    /// </summary>
    public sealed class DesktopMover : MonoBehaviour
    {
        public float moveSpeed = 3f;          // m/s, a comfortable walking pace
        public float lookSensitivity = 0.1f;  // degrees per mouse pixel
        public float gravity = -9.81f;        // m/s^2

        CharacterController controller;
        Transform head;
        XRRayInteractor ray;
        Image dot;
        float pitch;
        float verticalSpeed;
        bool armed;   // false until the button is released after the cursor is captured
        static readonly RaycastHit[] hits = new RaycastHit[8];

        public void Init(CharacterController controller, Transform head, XRRayInteractor ray, Image dot)
        {
            this.controller = controller;
            this.head = head;
            this.ray = ray;
            this.dot = dot;
        }

        void Start() => SetCursorLocked(true);

        // Whoever turned the head while this was off (the benchmark does) has the last word: no snap back on the first mouse move.
        void OnEnable() { if (head != null) pitch = Mathf.DeltaAngle(0f, head.localEulerAngles.x); }

        void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null || ray == null) return;

            // Esc frees the cursor; a click takes it back. While the cursor is free the rig is frozen.
            if (keyboard.escapeKey.wasPressedThisFrame) SetCursorLocked(false);
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                // A click takes the cursor back, unless it lands on a UI element (the teacher menu, for instance).
                bool overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                if (mouse.leftButton.wasPressedThisFrame && !overUi) SetCursorLocked(true);
                ray.selectInput.QueueManualState(false, 0f);
                ray.activateInput.QueueManualState(false, 0f);
                return;
            }

            Look(mouse.delta.ReadValue());
            Move(keyboard);
            Carry(mouse.leftButton.wasReleasedThisFrame);

            // Effective next frame; the reader works out pressed/released edges itself.
            if (!mouse.leftButton.isPressed) armed = true;
            bool select = armed && mouse.leftButton.isPressed;
            ray.selectInput.QueueManualState(select, select ? 1f : 0f);
            ray.activateInput.QueueManualState(mouse.rightButton.isPressed, mouse.rightButton.isPressed ? 1f : 0f);
        }

        // The dot is yellow on something to grab or press, and green, while you carry something, on a socket that takes it.
        // On PC the hand cannot reach down to a lock, so letting go on green hands the object over directly
        // (waiting for the socket's trigger to notice a falling key would depend on the frame rate).
        void Carry(bool released)
        {
            // A door leaf or a button stays selected while the button is down, but only a grabbable is carried.
            var carried = ray.hasSelection ? ray.interactablesSelected[0] as XRGrabInteractable : null;
            var socket = carried != null ? SocketFor(carried) : null;
            dot.color = socket != null ? Color.green : !ray.hasSelection && ray.hasHover ? Color.yellow : Color.white;
            if (!released || socket == null) return;
            var manager = ray.interactionManager;
            manager.SelectExit(ray, (IXRSelectInteractable)carried);  // the cast picks the current overload, not the obsolete one
            if (manager.CanSelect(socket, carried)) manager.SelectEnter(socket, (IXRSelectInteractable)carried);
        }

        // The free, switched-on socket under the dot that accepts the carried object's interaction layer, if no wall stands in front of it.
        // Sockets are trigger zones, so the ray has to collect triggers.
        XRSocketInteractor SocketFor(XRGrabInteractable carried)
        {
            float reach = Physics.Raycast(head.position, head.forward, out var wall, DesktopRig.ReachMeters, ray.raycastMask, QueryTriggerInteraction.Ignore)
                ? wall.distance : DesktopRig.ReachMeters;
            int count = Physics.RaycastNonAlloc(head.position, head.forward, hits, reach, ray.raycastMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < count; i++)
            {
                var socket = hits[i].collider.GetComponentInParent<XRSocketInteractor>();
                if (socket != null && socket.isSelectActive && !socket.hasSelection && (socket.interactionLayers.value & carried.interactionLayers.value) != 0) return socket;
            }
            return null;
        }

        void Look(Vector2 mouseDelta)
        {
            transform.Rotate(0f, mouseDelta.x * lookSensitivity, 0f);
            pitch = Mathf.Clamp(pitch - mouseDelta.y * lookSensitivity, -85f, 85f);
            head.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void Move(Keyboard keyboard)
        {
            float x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                    - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
            float z = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f)
                    - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
            var planar = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1f) * moveSpeed;

            // A small downward push while grounded keeps isGrounded reliable.
            verticalSpeed = controller.isGrounded ? -1f : verticalSpeed + gravity * Time.deltaTime;
            controller.Move((planar + Vector3.up * verticalSpeed) * Time.deltaTime);
        }

        void SetCursorLocked(bool locked)
        {
            armed = false; // the click that captures the cursor must not also grab something
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
