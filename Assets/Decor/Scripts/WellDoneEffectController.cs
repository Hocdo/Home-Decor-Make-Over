using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WellDoneEffectController : MonoBehaviour
{
    [Header("Characters")]
    public Transform[] characters;

    [Header("Animation Curve")]
    public AnimationCurve scaleUpCurve;

    public AnimationCurve birghtCurve;

    public AnimationCurve dropCurve;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
    }

    void Play()
    {
        int count = characters.Length;
        for (int i = 0; i < characters.Length; i++)
        {
            float mul = Mathf.Abs(i - count / 2) * 0.1f;

            characters[i].DOKill();
            characters[i].localScale = Vector2.zero;
            characters[i].position -= Vector3.up * (3f + 0.4f * mul);
        }

        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        string property = "_Brightness";
        int id = Shader.PropertyToID(property);

        int count = characters.Length;
        for (int i = 0; i < count; i++)
        {
            float mul = Mathf.Abs(i - count / 2) * 0.1f;
            characters[i].DOScale(1f, 0.25f).SetDelay(mul * 0.2f).SetEase(scaleUpCurve);
            characters[i].DOMove(characters[i].position + Vector3.up * (3f + 0.4f * mul), 1f).SetDelay(mul * 0.2f).SetEase(dropCurve);

            SpriteRenderer spr = characters[i].GetChild(0).GetComponent<SpriteRenderer>();
            spr.material.SetFloat(id, 0f);
            spr.material.DOFloat(1f, property, 0.8f).SetDelay(mul * 0.8f).SetEase(birghtCurve);
        }

        yield return null;
    }
}
