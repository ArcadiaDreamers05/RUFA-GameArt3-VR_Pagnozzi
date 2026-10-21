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
        void Awake()
        {
            var dark = new MaterialPropertyBlock();
            dark.CopySHCoefficientArraysFrom(new[] { new SphericalHarmonicsL2() });
            foreach (var r in GetComponentsInChildren<Renderer>(true))
            {
                r.lightProbeUsage = LightProbeUsage.CustomProvided;
                r.SetPropertyBlock(dark);
            }
        }
    }
}
