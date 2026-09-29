using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Rufa.Demo
{
    /// <summary>
    /// Lesson 11: dust particles done wrong (twelve systems of huge, faint, soft quads) or done right (small, few, culled).
    /// Systems are built at start-up; the switch reconfigures them in place.
    /// </summary>
    public sealed class DemoParticles : MonoBehaviour
    {
        public bool sick = true;
        public Material sickMaterial;     // Polvere_malato: soft particles, 2048 texture
        public Material healthyMaterial;  // Polvere_sano: alpha blend, 128 texture

        static readonly Vector3[] Spots =
        {
            new Vector3(-4f, 1.2f, -1.5f), new Vector3(-3f, 1.2f, -1.5f), new Vector3(-2f, 1.2f, -1.5f),   // Ingresso
            new Vector3(-4.5f, 1.2f, 3f), new Vector3(-3f, 1.2f, 3f), new Vector3(-1.5f, 1.2f, 3f),         // Salotto
            new Vector3(1.5f, 1.2f, 3f), new Vector3(3f, 1.2f, 3f), new Vector3(4.5f, 1.2f, 3f),            // Cucina
            new Vector3(-4.5f, 1.2f, 8.5f), new Vector3(-3f, 1.2f, 8.5f), new Vector3(-1.5f, 1.2f, 8.5f),   // Studio
        };

        readonly List<ParticleSystem> systems = new List<ParticleSystem>();

        bool depthAtStart;

        void Awake()
        {
            depthAtStart = UniversalRenderPipeline.asset != null && UniversalRenderPipeline.asset.supportsCameraDepthTexture;
            TeacherMenu.Register("Particellari malati", () => sick, value => { sick = value; Apply(); });
        }

        // In the editor Apply writes into the URP asset, and an asset changed during Play stays changed: put it back.
        void OnDestroy()
        {
            if (Application.isEditor && UniversalRenderPipeline.asset != null) UniversalRenderPipeline.asset.supportsCameraDepthTexture = depthAtStart;
        }

        void Start()
        {
            if (sickMaterial == null || healthyMaterial == null)
            {
                Debug.LogWarning("DemoParticles: materials not assigned.");
                return;
            }
            var root = new GameObject("Particellari").transform;
            root.SetParent(transform, false);
            foreach (var spot in Spots)
            {
                var go = new GameObject("Polvere", typeof(ParticleSystem));
                go.transform.SetParent(root, false);
                go.transform.position = spot;
                systems.Add(go.GetComponent<ParticleSystem>());
            }
            Apply();
        }

        public void Apply()
        {
            for (int i = 0; i < systems.Count; i++)
            {
                var ps = systems[i];
                bool on = sick || i % 3 == 1;   // healthy: one system per room, on the middle spot
                ps.gameObject.SetActive(on);
                if (!on) continue;

                var main = ps.main;
                main.maxParticles = sick ? 500 : 60;
                main.startSize = sick ? 3f : 0.15f;
                main.startLifetime = sick ? 6f : 10f;
                main.startSpeed = sick ? 0.3f : 0.05f;
                main.startColor = new Color(1f, 0.9f, 0.7f, sick ? 0.02f : 0.35f); // sick: hundreds of faint layers on every pixel, a haze that still costs them all
                main.cullingMode = sick ? ParticleSystemCullingMode.AlwaysSimulate : ParticleSystemCullingMode.Automatic;

                var emission = ps.emission;
                emission.rateOverTime = sick ? 200f : 8f;

                var shape = ps.shape;
                if (sick) { shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 2f; shape.scale = Vector3.one; } // undo the healthy box's scale
                else { shape.shapeType = ParticleSystemShapeType.Box; shape.scale = new Vector3(3f, 2f, 3f); }

                ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = sick ? sickMaterial : healthyMaterial;
                ps.Clear();
                ps.Play();
            }
            // Soft particles read the depth texture: the sick preset pays for it, the healthy one does not.
            if (UniversalRenderPipeline.asset != null) UniversalRenderPipeline.asset.supportsCameraDepthTexture = sick;
        }
    }
}
