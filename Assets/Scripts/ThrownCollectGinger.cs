using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownCollectGinger : ThrowCollectItem
{
    private Vector3 targetPosition;

    private Vector3 beginPosition;

    private Vector3 controlPosition;

    public override void SetupAnimation(Vector3 beginPosition, Vector3 targetPosition, float duration, float delayTime)
    {
        this.beginPosition = beginPosition;
        this.targetPosition = targetPosition;

        controlPosition.x = (beginPosition.x - targetPosition.x) * 0.7f + targetPosition.x;
        controlPosition.y = (beginPosition.y - targetPosition.y) * 0.7f + targetPosition.y + 150f;
        controlPosition.z = beginPosition.z;

        StartCoroutine(FlyToTarget(duration, delayTime));
    }

    protected override IEnumerator FlyToTarget(float duration, float delayTime)
    {
        float currentScale = transform.localScale.x;
        float targetScale = 60f / transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.y * currentScale;

        yield return new WaitForSeconds(delayTime);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            if (t > 1f) t = 1f;

            float oneminust = 1f - t;
            Vector2 position = oneminust * oneminust * beginPosition + 2f * oneminust * t * controlPosition + t * t * targetPosition;
            transform.localPosition = position;

            Vector3 localPosition = transform.localPosition;
            localPosition.z = -localPosition.x * 0.01f - localPosition.y * 0.01f - 5f;
            transform.localPosition = localPosition;

            float a = 2 * t - 1;
            float scale;
            if (t < 0.5f)
            {
                scale = (-a * a + 1) * 0.25f * currentScale + currentScale;
            }
            else
            {
                scale = (-a * a + 1) * (1.25f * currentScale - targetScale) + targetScale;
            }
            
            transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }
    }
}
