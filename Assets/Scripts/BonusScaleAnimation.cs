using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusScaleAnimation : MonoBehaviour
{ 
    public AnimationCurve curve;

    public float bonusScale = 0.25f;

    public float speed = 0.1f;

    public float superviseExpiredTime = 3f;

    private float animTime = 0;

    private float superviseTime = 0f;

    private Coroutine animCoroutine;

    private Coroutine superviseCoroutine;

    public void Trigger()
    {
        superviseTime = 0f;

        if (animCoroutine == null)
        {
            animCoroutine = StartCoroutine(AnimCoroutine());
            superviseCoroutine = StartCoroutine(SuperviseCoroutine());
        }
   
        if (animTime >= 0.5f) animTime = 1f - animTime;
    }

    private IEnumerator AnimCoroutine()
    {
        while (true)
        {
            animTime = Mathf.Min(1f, animTime + speed);

            float scale = curve.Evaluate(animTime) * bonusScale + 1f;
            transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }  
    }

    private IEnumerator SuperviseCoroutine()
    {
        while (true)
        {
            superviseTime += Time.deltaTime;

            if (superviseTime > superviseExpiredTime)
            {
                StopCoroutine(animCoroutine);

                animCoroutine = null;
                superviseCoroutine = null;

                break;
            }

            yield return null;
        }
    }
}
