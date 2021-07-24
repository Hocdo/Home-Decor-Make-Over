using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownCollectLiteChip : ThrowCollectItem
{
    private Vector3 targetPosition;

    private Vector3 beginPosition;

    private Vector3 controlPosition;

    public override void SetupAnimation(Vector3 beginPosition, Vector3 targetPosition, float duration, float delayTime)
    {
        this.beginPosition = beginPosition;
        this.targetPosition = targetPosition;

        //controlPosition.x = (beginPosition.x - targetPosition.x) * 0.35f + targetPosition.x;
        //controlPosition.y = (beginPosition.y - targetPosition.y) * 0.35f + targetPosition.y + 275f;
        controlPosition.x = beginPosition.x;
        controlPosition.y = beginPosition.y + 225f;
        controlPosition.z = beginPosition.z;

        //transform.DOMove(new Vector3(beginPosition.x, beginPosition.y + 17.5f, beginPosition.z), 0.25f)
        //    .SetEase(Ease.InOutQuad)
        //    .OnComplete(() => StartCoroutine(FlyToTarget(duration, delayTime)));

        StartCoroutine(FlyToTarget(duration, delayTime));
    }

    protected override IEnumerator FlyToTarget(float duration, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        float time = 0f;
        float t = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            t = time / duration;

            if (t > 1f) t = 1f;

            float oneminust = 1f - t;
            Vector2 position = oneminust * oneminust * beginPosition + 2f * oneminust * t * controlPosition + t * t * targetPosition;
            transform.localPosition = position;

            Vector3 localPosition = transform.localPosition;
            localPosition.z = -localPosition.x * 0.01f - localPosition.y * 0.01f - 5f;
            transform.localPosition = localPosition;

            float a = 2 * t - 1;
            float scale = (-a * a + 1) * 0.4f + 1f;

            transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }
    }
}
