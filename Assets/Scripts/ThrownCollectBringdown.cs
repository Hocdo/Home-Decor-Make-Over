using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownCollectBringdown : ThrowCollectItem
{
    private Vector3 targetPosition;

    private Vector3 beginPosition;

    public override void SetupAnimation(Vector3 beginPosition, Vector3 targetPosition, float duration, float delayTime)
    {
        this.beginPosition = beginPosition;
        this.targetPosition = targetPosition;

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

            Vector2 position = Vector3.Lerp(beginPosition, targetPosition, 1f - (1f - t) * (1f - t));
            transform.localPosition = position;

            Vector3 localPosition = transform.localPosition;
            localPosition.z = -localPosition.x * 0.01f - localPosition.y * 0.01f - 5f;
            transform.localPosition = localPosition;

            yield return null;
        }
    }
}
