using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelCompleteEffectController : MonoBehaviour
{
    [Header("Level Characters")]
    public Transform[] levelCharacters;

    [Header("Complete Characters")]
    public Transform[] completeCharacters;

    [Header("Animation Curve")]
    public AnimationCurve scaleUpCurve;

    public AnimationCurve birghtCurve;

    public AnimationCurve dropCurve;

    private Vector3[] levelCharactersSourcePositions;

    private Vector3[] levelCharactersTargetPositions;

    private Vector3[] completeCharactersSourcePositions;

    private Vector3[] completeCharactersTargetPositions;

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
        for (int i = 0; i < levelCharacters.Length; i++)
        {
            levelCharacters[i].DOKill();
            levelCharacters[i].localScale = Vector2.zero;
            levelCharacters[i].position += Vector3.up * 1.5f;
        }
            

        for (int i = 0; i < completeCharacters.Length; i++)
        {
            completeCharacters[i].DOKill();
            completeCharacters[i].localScale = Vector2.zero;
            completeCharacters[i].position += Vector3.up * 1.5f;
        }         

        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        string property = "_Brightness";
        int id = Shader.PropertyToID(property);
        

        for (int i = 0; i < levelCharacters.Length; i++)
        {
            levelCharacters[i].DOScale(1f, 0.25f).SetDelay(i * 0.075f).SetEase(scaleUpCurve);
            levelCharacters[i].DOMove(levelCharacters[i].position - Vector3.up * 1.5f, 0.65f).SetDelay(i * 0.075f).SetEase(dropCurve);

            SpriteRenderer spr = levelCharacters[i].GetChild(0).GetComponent<SpriteRenderer>();
            spr.material.SetFloat(id, 0f);
            spr.material.DOFloat(1f, property, 0.8f).SetDelay(i * 0.075f).SetEase(birghtCurve);
        }
            
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < completeCharacters.Length; i++)
        {
            completeCharacters[i].DOScale(1f, 0.25f).SetDelay(i * 0.075f).SetEase(scaleUpCurve);
            completeCharacters[i].DOMove(completeCharacters[i].position - Vector3.up * 1.5f, 0.65f).SetDelay(i * 0.075f).SetEase(dropCurve);

            SpriteRenderer spr = completeCharacters[i].GetChild(0).GetComponent<SpriteRenderer>();
            spr.material.SetFloat(id, 0f);
            spr.material.DOFloat(1f, property, 0.8f).SetDelay(i * 0.075f).SetEase(birghtCurve);
        }            
    }
}
