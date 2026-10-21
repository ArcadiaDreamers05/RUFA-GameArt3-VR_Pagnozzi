using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace Rufa
{
    /// <summary>
    /// A gesture of the player's hand, such as a thumbs up: a hand shape or a hand pose of XR Hands, held for a moment.
    /// The hand belongs to the rig, which appears only in Play: the gesture finds it by itself, so the component can
    /// stay in the scene, on what the gesture acts on.
    /// </summary>
    public sealed class HandGesture : MonoBehaviour
    {
        public Handedness hand = Handedness.Right;
        [Tooltip("An XR Hand Shape or an XR Hand Pose: the Gestures sample of XR Hands has some ready, such as Thumbs Up.")]
        public ScriptableObject shapeOrPose;
        [Tooltip("How long the hand must hold the gesture, in seconds.")]
        public float holdTime = 0.2f;
        public UnityEvent onPerformed = new UnityEvent();
        public bool Performed { get; private set; }

        XRHandTrackingEvents events;
        float nextSearch, heldSince = -1f;
        bool firedThisHold;

        // With the controllers in hand the hands are switched off, and a PC build has none: look again once a second.
        void Update()
        {
            if (events != null || Time.unscaledTime < nextSearch) return;
            nextSearch = Time.unscaledTime + 1f;
            foreach (var candidate in FindObjectsByType<XRHandTrackingEvents>(FindObjectsSortMode.None))
                if (candidate.handedness == hand) { events = candidate; break; }
            if (events == null) return;
            events.jointsUpdated.AddListener(Check);
            events.trackingChanged.AddListener(Restart);
            // A pose may want the hand to point at something, as Point At does: here, at this object
            if (shapeOrPose is XRHandPose pose && pose.relativeOrientation != null)
                pose.relativeOrientation.targetTransform = transform;
        }

        void OnDestroy()
        {
            if (events == null) return;
            events.jointsUpdated.RemoveListener(Check);
            events.trackingChanged.RemoveListener(Restart);
        }

        // A hand that comes back into view starts its gesture again
        void Restart(bool tracked) => Hold(false, 0f, holdTime, ref heldSince, ref firedThisHold);

        void Check(XRHandJointsUpdatedEventArgs joints)
        {
            var held = isActiveAndEnabled && events.handIsTracked
                       && (shapeOrPose is XRHandShape shape ? shape.CheckConditions(joints) : shapeOrPose is XRHandPose pose && pose.CheckConditions(joints));
            if (!Hold(held, Time.time, holdTime, ref heldSince, ref firedThisHold)) return;
            Performed = true;
            onPerformed.Invoke();
        }

        /// <summary>Whether a gesture fires now: once per hold, when it has been held for holdTime. Letting it go ends the hold.</summary>
        public static bool Hold(bool held, float now, float holdTime, ref float heldSince, ref bool fired)
        {
            if (!held)
            {
                heldSince = -1f;
                fired = false;
                return false;
            }
            if (heldSince < 0f) heldSince = now;
            if (fired || now - heldSince < holdTime) return false;
            return fired = true;
        }
    }
}
