using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThrownBonusCoin : MonoBehaviour
{
    public float startScale = 50;

    public float targetScale = 100;

    public Transform shadowTransform;

    public void Play(Vector3 startPos, Vector3 endPos)
    {
        //transform.position = startPos;
        //transform.DOMove(endPos, 0.75f).OnComplete(() => 
        //{ 
        //    GameMain.main.AddBonusCoin(10);
        //    Destroy(gameObject);
        //} );

        StartCoroutine(MoveCoroutine(startPos, endPos, 1f));
    }

    private IEnumerator MoveCoroutine(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        Vector3 controlPosition = new Vector3(startPosition.x + (endPosition.x - startPosition.x) * 0.75f, Mathf.Max(startPosition.y, endPosition.y) + 150f, startPosition.z);

        float time = -Time.deltaTime;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            if (t > 1f) t = 1f;

            float oneminust = 1f - t;
            Vector3 position = oneminust * oneminust * startPosition + 2f * oneminust * t * controlPosition + t * t * endPosition;
            transform.localPosition = position;

            float a = (2 * t - 1f);
            float st = -a * a + 1f;

            float scale = Mathf.Lerp(0.6f, 1.25f, st);
            transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        GameMain.main.AddBonusCoin(10);
        AudioManager.Instance.PlaySFX(AudioClipId.AddCoin);

        gameObject.SetActive(false);
    }
}
