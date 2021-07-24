using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectParticles : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem particleSystem;

    public Transform targetTransform;

    [Header("Properties")]
    public Vector3 targetPosition;

    public float spawnRate;

    public float duration;

    public float lifeTime;

    public float rotateAngle;

    private ParticleSystem.Particle[] particles;

    void Awake()
    {
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

        SetRotateAngle(rotateAngle);

        SetDuration(duration);

        SetSpawnRate(spawnRate);

        SetLifeTime(lifeTime);
    }

    public void SetRotateAngle(float angle)
    {
        var rotationByLifetime = particleSystem.rotationOverLifetime;
        rotationByLifetime.zMultiplier = angle * Mathf.Deg2Rad;
    }

    public void SetSpawnRate(float rate)
    {
        var emission = particleSystem.emission;
        emission.rateOverTime = rate;
    }

    public void SetDuration(float duration)
    {
        var main = particleSystem.main;
        main.duration = duration;
    }

    public void SetLifeTime(float lifeTime)
    {
        var main = particleSystem.main;
        main.startLifetime = lifeTime;
    }

    public float GetDuration()
    {
        return particleSystem.main.duration;
    }

    public float GetParticleMoveDuration()
    {
        return particleSystem.main.startLifetime.constant;
    }

    public void Play()
    {
        StartCoroutine(PlayCoroutine());
    }
    IEnumerator PlayCoroutine()
    {
        particleSystem.Play(false);

        while (true)
        {
            yield return null;

            int count = particleSystem.GetParticles(particles);

            if (targetTransform)
                targetPosition = targetTransform.position;

            for (int i = 0; i < count; i++)
            {
                ParticleSystem.Particle particle = particles[i];

                Vector3 particlePosition = particle.position;
                float t = 1f - particle.remainingLifetime / (particle.startLifetime);
                Vector3 delta = (targetPosition - particlePosition);
                t *= t;
                t *= t;
                t *= t;

                particle.position = particlePosition + delta * t;

                particles[i] = particle;
            }

            particleSystem.SetParticles(particles, count);
        }
    }
}
