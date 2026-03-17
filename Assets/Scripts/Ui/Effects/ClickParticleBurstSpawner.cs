using System.Collections.Generic;
using UnityEngine;

namespace FunClicker.UI.Effects
{
    public class ClickParticleBurstSpawner : MonoBehaviour
    {
        [Header("Spawn")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float spawnDistance = 10f;
        [SerializeField] private int poolSize = 6;

        [Header("Burst")]
        [SerializeField] private int burstCount = 10;
        [SerializeField] private float lifetime = 0.35f;
        [SerializeField] private float speed = 5.5f;
        [SerializeField] private float size = 0.12f;
        [SerializeField] private float sizeVariation = 0.04f;
        [SerializeField] private float spread = 35f;
        [SerializeField] private Color startColor = new(1f, 0.95f, 0.45f, 1f);
        [SerializeField] private Color endColor = new(1f, 0.55f, 0.1f, 0f);

        private readonly Queue<ParticleSystem> pool = new();
        private static Material sharedParticleMaterial;

        private void Awake()
        {
            for (int i = 0; i < Mathf.Max(1, poolSize); i++)
            {
                pool.Enqueue(CreateParticleSystem(i));
            }
        }

        public void SpawnAtScreenPosition(Vector2 screenPosition, Camera eventCamera)
        {
            if (FunClicker.UI.ExclusivePanelCoordinator.HasOpenPanel)
                return;

            Camera cameraToUse = ResolveCamera(eventCamera);
            if (cameraToUse == null)
            {
                Debug.LogWarning("ClickParticleBurstSpawner requires a camera.");
                return;
            }

            ParticleSystem system = GetNextSystem();
            Vector3 spawnPosition = cameraToUse.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, spawnDistance));
            system.transform.position = spawnPosition;

            var shape = system.shape;
            shape.rotation = new Vector3(0f, 0f, Random.Range(0f, 360f));

            system.Clear(true);
            system.Emit(burstCount);
            system.Play(true);
        }

        private Camera ResolveCamera(Camera eventCamera)
        {
            if (eventCamera != null)
            {
                return eventCamera;
            }

            if (targetCamera != null)
            {
                return targetCamera;
            }

            if (Camera.main != null)
            {
                return Camera.main;
            }

            return FindFirstObjectByType<Camera>();
        }

        private ParticleSystem GetNextSystem()
        {
            ParticleSystem system = pool.Dequeue();
            pool.Enqueue(system);
            return system;
        }

        private ParticleSystem CreateParticleSystem(int index)
        {
            GameObject go = new($"ClickBurstParticle_{index}");
            go.transform.SetParent(transform, false);

            ParticleSystem system = go.AddComponent<ParticleSystem>();
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = system.main;
            main.playOnAwake = false;
            main.loop = false;
            main.duration = lifetime;
            main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.75f, lifetime);
            main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.65f, speed);
            main.startSize = new ParticleSystem.MinMaxCurve(Mathf.Max(0.01f, size - sizeVariation), size + sizeVariation);
            main.startColor = new ParticleSystem.MinMaxGradient(startColor);
            main.maxParticles = burstCount;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.stopAction = ParticleSystemStopAction.None;

            var emission = system.emission;
            emission.enabled = false;

            var shape = system.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = spread;
            shape.radius = 0.01f;
            shape.arc = 360f;

            var colorOverLifetime = system.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(startColor, 0f),
                    new GradientColorKey(new Color((startColor.r + endColor.r) * 0.5f, (startColor.g + endColor.g) * 0.5f, (startColor.b + endColor.b) * 0.5f), 0.5f),
                    new GradientColorKey(endColor, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(startColor.a, 0f),
                    new GradientAlphaKey(startColor.a, 0.15f),
                    new GradientAlphaKey(endColor.a, 1f)
                });
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            var sizeOverLifetime = system.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.65f, 0.9f);
            sizeCurve.AddKey(1f, 0f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            var noise = system.noise;
            noise.enabled = true;
            noise.strength = 0.15f;
            noise.frequency = 0.45f;

            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.alignment = ParticleSystemRenderSpace.View;
            renderer.sortMode = ParticleSystemSortMode.Distance;
            renderer.sharedMaterial = GetParticleMaterial();

            return system;
        }

        private static Material GetParticleMaterial()
        {
            if (sharedParticleMaterial != null)
            {
                return sharedParticleMaterial;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Particles/Standard Unlit");
            }

            if (shader == null)
            {
                Debug.LogWarning("No compatible particle shader found for ClickParticleBurstSpawner.");
                return null;
            }

            sharedParticleMaterial = new Material(shader)
            {
                name = "ClickBurstParticleMaterial"
            };

            return sharedParticleMaterial;
        }
    }
}
