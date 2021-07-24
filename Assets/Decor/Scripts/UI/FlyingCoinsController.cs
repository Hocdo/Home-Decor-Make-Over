using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCoinsController : MonoBehaviour
{
    public Transform sourceTransform;

    public AnimationCurve curveEvaluator;

    public ParticleSystem coinParticleSystem;

    public ParticleSystem contactParticleSystem;

    private float time;

    private float duration;

    private Vector2 startPoint, endPoint, normal, previousPosition;
    private float outRange;

    private Action OnComplete;

    private void Awake()
    {

    }

    public void StartWithTarget(Vector2 aendPoint, out float calculatedDuration, Action aOnComplete)
    {
        time = 0f;

        startPoint = sourceTransform.position;
        endPoint = aendPoint;
        normal = new Vector2(-endPoint.y + startPoint.y, endPoint.x - startPoint.x).normalized;

        OnComplete = aOnComplete;

        Vector2 delta = endPoint - startPoint;
        float t = Mathf.Min(1f, delta.magnitude / 40f);
        duration = calculatedDuration = Mathf.Max(0.6f, t * 0.75f);

        float side = (endPoint.x < startPoint.x) ? 1 : -1;
        outRange = Mathf.Clamp((delta.magnitude / 40f - 0.3f) * 12, 2f, 8f) * side;

        transform.localPosition = startPoint;

        coinParticleSystem.Play(true);

        StartCoroutine(TweenCoroutine());
    }

    IEnumerator TweenCoroutine()
    {
        yield return null;

        previousPosition = startPoint;

        while (time < duration)
        {
            time += Time.deltaTime;
            SetPosition(time / duration);

            yield return null;
        }

        SetPosition(1f);

        yield return null;

        contactParticleSystem.Play();

        OnComplete?.Invoke();
    }

    private void SetPosition(float x)
    {
        float r = curveEvaluator.Evaluate(1 - x);
        float t = -(2 * (1 - r * r) - 1f);
        float a = -t * t + 1f;

        Vector3 controlDelta = outRange * a * normal;
        Vector2 currentPosition = Vector3.Lerp(startPoint, endPoint, 1f - r * r) + controlDelta;

        float angle = Mathf.Atan2(currentPosition.y - previousPosition.y, currentPosition.x - previousPosition.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0f, 0f, angle);
        transform.position = currentPosition;
    }
}
