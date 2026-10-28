using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// No game can stop a player who leans: the headset goes where the body goes, walls included. Meta asks never to
    /// move or freeze the camera for it, and to show nothing while the head is in a wall: the player sees black and steps
    /// back. So the view goes black when the head comes within <see cref="radius"/> of something that does not move, and
    /// it stays black while a wall stands between the head and the last place where the head was free: a head pushed
    /// right through a thin wall does not peek into the next room either. Objects with a Rigidbody, like what is held,
    /// and the rig itself never count. Goes on the rig root, next to the XR Origin; on the head it works the same.
    /// </summary>
    public sealed class HeadBlackout : MonoBehaviour
    {
        [Tooltip("How close to a wall, in metres, the view goes black.")]
        public float radius = 0.1f;

        readonly Collider[] near = new Collider[16];
        readonly RaycastHit[] between = new RaycastHit[16];
        Transform rig;   // where the rig's capsule is, the root: the component may sit lower down, on the head
        Camera head;
        Vector3 free;   // the last place where the head was free, in the rig's space: a teleport carries it with the rig
        bool dark;
        int mask;
        CameraClearFlags clear;
        Color background;

        void Start()
        {
            var body = GetComponentInParent<CharacterController>();
            rig = body != null ? body.transform : transform;
            head = rig.GetComponentInChildren<Camera>();
            if (head != null) free = rig.InverseTransformPoint(head.transform.position);
        }

        void LateUpdate()
        {
            if (head == null) return;
            var position = head.transform.position;
            var last = rig.TransformPoint(free);
            var blocked = false;
            for (var i = Physics.OverlapSphereNonAlloc(position, radius, near, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) - 1; i >= 0; i--)
                blocked |= Wall(near[i]);
            var distance = Vector3.Distance(last, position);
            if (distance > 0.001f)
                for (var i = Physics.RaycastNonAlloc(last, (position - last) / distance, between, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) - 1; i >= 0; i--)
                    blocked |= Wall(between[i].collider);
            if (!blocked) free = rig.InverseTransformPoint(position);
            Black(blocked);
        }

        void OnDisable() => Black(false);

        bool Wall(Collider c) => c.attachedRigidbody == null && !c.transform.IsChildOf(rig);

        void Black(bool on)
        {
            if (on == dark || head == null) return;
            dark = on;
            if (on)
            {
                (mask, clear, background) = (head.cullingMask, head.clearFlags, head.backgroundColor);
                (head.cullingMask, head.clearFlags, head.backgroundColor) = (0, CameraClearFlags.SolidColor, Color.black);
            }
            else (head.cullingMask, head.clearFlags, head.backgroundColor) = (mask, clear, background);
        }
    }
}
