using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesFollowTarget : MonoBehaviour
{
    public Transform targetTransform;

    private ParticleSystem system;

    private ParticleSystem.Particle[] particles;

    private List<Vector4> customData = new List<Vector4>();

    private Vector3 sourcePosition;

    private Vector3 targetPosition;

    private Vector3 delta;

    private Vector3 normal;

    public float GetDuration()
    {
        return system.main.duration;
    }

    public float GetParticleMoveDuration()
    {
        return system.main.startLifetime.constant;
    }

    void Awake()
    {
        system = GetComponent<ParticleSystem>();

        particles = new ParticleSystem.Particle[system.main.maxParticles];

        sourcePosition = transform.position;
        targetPosition = targetTransform.position;
        delta = targetPosition - sourcePosition;
        normal = new Vector3(-delta.y, delta.x, 0f).normalized;     
    }

    public void Play()
    {
        StartCoroutine(PlayCoroutine());
    }
    IEnumerator PlayCoroutine()
    {
        system.Play(false);

        while (true)
        {
            yield return null;

            float dt = Time.deltaTime;
            int count = system.GetParticles(particles);

            customData.Clear();
            system.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

            for (int i = 0; i < count; i++)
            {
                ParticleSystem.Particle particle = particles[i];

                float r = particle.remainingLifetime / particle.startLifetime;
                float t = -(2 * r - 1f);
                float a = -t * t + 1f;
                float range = customData[i].x;
                Vector3 controlDelta = normal * range * a;
                particle.position = Vector3.Lerp(sourcePosition, targetPosition, 1 - r) + controlDelta;

                particles[i] = particle;
            }

            system.SetParticles(particles, count);  
        }  
    }
}
