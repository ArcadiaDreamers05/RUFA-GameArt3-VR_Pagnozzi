using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// The pose of every object of a course environment, recorded when the course is generated. The lesson tutorial
    /// ("> verifica: ambiente-intatto Name") compares it with the current one: moving, turning, scaling or deleting an
    /// object shows up, while the components and objects a student adds do not.
    /// </summary>
    public sealed class EnvironmentSnapshot : MonoBehaviour
    {
        [Serializable]
        public struct Pose
        {
            public string path;
            public Vector3 position, scale;
            public Quaternion rotation;
        }

        [SerializeField] List<Pose> poses = new List<Pose>();

        /// <summary>Records the current poses: this object's in world space, every descendant's relative to its parent.</summary>
        public void Record()
        {
            poses.Clear();
            foreach (var t in GetComponentsInChildren<Transform>(true))
                poses.Add(new Pose { path = PathOf(t), position = Position(t), rotation = Rotation(t), scale = t.localScale });
            if (poses.Select(p => p.path).Distinct().Count() != poses.Count)
                Debug.LogError($"EnvironmentSnapshot on {name}: two objects share a path. Give them different names.", this);
        }

        /// <summary>The objects moved, turned, scaled or deleted since Record; empty when the environment is intact.</summary>
        public List<string> Changes()
        {
            var changes = new List<string>();
            foreach (var p in poses)
            {
                var t = p.path.Length == 0 ? transform : transform.Find(p.path);
                if (Application.isPlaying && t != null && t.GetComponent<CourseGoal>() != null) continue; // a goal may move itself in Play: a door slides open
                if (t == null || Vector3.Distance(Position(t), p.position) > 0.001f || Quaternion.Angle(Rotation(t), p.rotation) > 0.1f
                    || Vector3.Distance(t.localScale, p.scale) > 0.001f)
                    changes.Add(p.path.Length == 0 ? name : p.path);
            }
            return changes;
        }

        Vector3 Position(Transform t) => t == transform ? t.position : t.localPosition;
        Quaternion Rotation(Transform t) => t == transform ? t.rotation : t.localRotation;
        string PathOf(Transform t) => t == transform ? "" : t.parent == transform ? t.name : PathOf(t.parent) + "/" + t.name;
    }
}
