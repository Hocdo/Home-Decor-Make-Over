using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TextEffectVictory : MonoBehaviour
{
    [Header("Level Characters")]
    public Transform[] characters;

    public float animInterval = 0.05f;

    [Header("Lift")]
    public float liftDuration = 0.5f;

    public float liftMagnitude = 1;

    public AnimationCurve liftCurve;

    [Header("Scale")]
    public float scaleDuration = 0.5f;

    public AnimationCurve scaleUpCurve;

    private float liftMagnitudeWorldSpace;

    class CharacterProperty
    {
        public Transform transform;
        public SpriteRenderer spriteRenderer;
        public Vector3 liftTargetPosition;
    }

    private CharacterProperty[] CharacterProps;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
    }

    private void Awake()
    {
        CharacterProps = new CharacterProperty[characters.Length];
        for (int i = 0; i < characters.Length; i++)
        {
            CharacterProps[i] = new CharacterProperty
            {
                transform = characters[i],
                spriteRenderer = characters[i].GetChild(0).GetComponent<SpriteRenderer>()
            };
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < CharacterProps.Length; i++)
        {
            CharacterProperty character = CharacterProps[i];
            character.transform.DOKill();
            character.spriteRenderer.DOKill();
        }
    }

    public void SetSortingLayer(string sortingLayerName, int order)
    {
        int sortingLayerId = SortingLayer.NameToID(sortingLayerName);

        for (int i = 0; i < characters.Length; i++)
        {
            SpriteRenderer spr = CharacterProps[i].spriteRenderer;
            spr.sortingLayerID = sortingLayerId;
            spr.sortingOrder = order;
        }
    }

    public void Play()
    {
        liftMagnitudeWorldSpace = transform.localScale.x * liftMagnitude;
        int count = CharacterProps.Length;

        for (int i = 0; i < count; i++)
        {
            CharacterProperty character = CharacterProps[i];

            character.transform.DOKill();
            character.transform.localScale = Vector3.zero;

            float factor = i - (count - 1) * 0.5f;
            float angle = (90f + factor * 5f) * Mathf.Deg2Rad;
            character.liftTargetPosition = character.transform.position + liftMagnitudeWorldSpace * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
        }

        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        for (int i = 0; i < CharacterProps.Length; i++)
        {
            CharacterProperty character = CharacterProps[i];

            character.transform.DOScale(1f, scaleDuration).SetDelay(i * animInterval).SetEase(scaleUpCurve);
            character.transform.DOMove(character.liftTargetPosition, liftDuration).SetDelay(i * animInterval).SetEase(liftCurve);
        }

        yield return null;
    }
}
