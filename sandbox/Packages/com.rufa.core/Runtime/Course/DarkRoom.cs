using UnityEngine;
using UnityEngine.Rendering;

namespace Rufa
{
    /// <summary>
    /// A closed room in the dark. Walls stop the sun, through their shadows, but not the ambient light of the sky:
    /// without baked lighting Unity does not know the room is closed and lights it from every side. So the renderers
    /// of the room get no ambient light, and only real lights, like a torch, show what is inside.
    /// </summary>
    public sealed class DarkRoom : MonoBehaviour
    {
        [Tooltip("The ambient light left in the room: 0 is black, and only real lights show what is inside. On an LCD headset a little above 0 reads as dark instead of as a hole.")]
        [Range(0f, 0.5f)] public float ambient;

        void Awake()
        {
            var light = new SphericalHarmonicsL2();
            light.AddAmbientLight(new Color(ambient, ambient, ambient));
            var dark = new MaterialPropertyBlock();
            dark.CopySHCoefficientArraysFrom(new[] { light });
            foreach (var r in GetComponentsInChildren<Renderer>(true))
            {
                r.lightProbeUsage = LightProbeUsage.CustomProvided;
                r.SetPropertyBlock(dark);
            }
        }
    }
}
