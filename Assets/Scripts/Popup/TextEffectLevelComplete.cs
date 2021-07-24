using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TextEffectLevelComplete : MonoBehaviour
{
    [Header("Level Characters")]
    public Transform[] levelCharacters;

    [Header("Complete Characters")]
    public Transform[] completeCharacters;

    public float interval = 0.05f;

    [Header("Drop")]
    public float dropHeight = 2f;

    public float dropDuration = 0.5f;

    public AnimationCurve dropCurve;

    [Header("Lift")]
    public float liftDelay = 2f;

    public float liftDuration = 0.5f;

    public AnimationCurve liftCurve;

    [Header("Scale")]
    public float scaleDelay = 1f;

    public float scaleDuration = 0.5f;

    public AnimationCurve scaleUpCurve;

    private float dropHeightWorldSpace;

    private bool initialized = false;

    struct CharacterProperty
    {
        public Vector3 startPosition;
        public Transform transform;
        public SpriteRenderer spriteRenderer;
    }

    private CharacterProperty[] levelCharacterProps;

    private CharacterProperty[] completeCharacterProps;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
    }
#endif

    private void Initialized()
    {
        levelCharacterProps = new CharacterProperty[levelCharacters.Length];
        for (int i = 0; i < levelCharacters.Length; i++)
        {
            levelCharacterProps[i] = new CharacterProperty
            {
                startPosition = levelCharacters[i].position,
                transform = levelCharacters[i],
                spriteRenderer = levelCharacters[i].GetChild(0).GetComponent<SpriteRenderer>()
            };
        }

        completeCharacterProps = new CharacterProperty[completeCharacters.Length];
        for (int i = 0; i < completeCharacters.Length; i++)
        {
            completeCharacterProps[i] = new CharacterProperty
            {
                startPosition = completeCharacters[i].position,
                transform = completeCharacters[i],
                spriteRenderer = completeCharacters[i].GetChild(0).GetComponent<SpriteRenderer>()
            };
        }

        initialized = true;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < levelCharacterProps.Length; i++)
        {
            CharacterProperty character = levelCharacterProps[i];
            character.transform.DOKill();
            character.spriteRenderer.DOKill();
        }

        for (int i = 0; i < completeCharacterProps.Length; i++)
        {
            CharacterProperty character = completeCharacterProps[i];
            character.transform.DOKill();
            character.spriteRenderer.DOKill();
        }
    }

    public void SetSortingLayer(string sortingLayerName, int order)
    {
        int sortingLayerId = SortingLayer.NameToID(sortingLayerName);

        for (int i = 0; i < levelCharacters.Length; i++)
        {
            SpriteRenderer spr = levelCharacterProps[i].spriteRenderer;
            spr.sortingLayerID = sortingLayerId;
            spr.sortingOrder = order;
        }

        for (int i = 0; i < completeCharacters.Length; i++)
        {
            SpriteRenderer spr = completeCharacterProps[i].spriteRenderer;
            spr.color = new Color(1f, 1f, 1f, 0f);
            spr.sortingLayerID = sortingLayerId;
            spr.sortingOrder = order;
        }
    }

    public void Play()
    {
        if (initialized == false) Initialized();

        dropHeightWorldSpace = transform.localScale.x * dropHeight;

        for (int i = 0; i < levelCharacterProps.Length; i++)
        {
            CharacterProperty character = levelCharacterProps[i];

            character.transform.DOKill();
            character.transform.position = character.startPosition + Vector3.up * dropHeightWorldSpace;
            character.spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }

        for (int i = 0; i < completeCharacterProps.Length; i++)
        {
            CharacterProperty character = completeCharacterProps[i];

            character.transform.DOKill();
            character.transform.position = character.startPosition + Vector3.up * dropHeightWorldSpace;
            character.spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }

        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        Color white = Color.white;
        Color invisible = new Color(1f, 1f, 1f, 0f);

        for (int i = 0; i < levelCharacterProps.Length; i++)
        {
            CharacterProperty character = levelCharacterProps[i];

            character.transform.DOMove(character.startPosition, dropDuration).SetDelay(i * interval).SetEase(dropCurve);

            character.spriteRenderer.DOColor(white, dropDuration).SetDelay(i * interval);

            character.transform.DOScale(1.25f, 0.45f).SetDelay(i * interval + scaleDelay).SetEase(scaleUpCurve);

            character.transform.DOMove(character.startPosition + Vector3.up * dropHeightWorldSpace, liftDuration).SetDelay(i * interval + liftDelay).SetEase(liftCurve);

            character.spriteRenderer.DOColor(invisible, liftDuration).SetDelay(i * interval + liftDelay);
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < completeCharacterProps.Length; i++)
        {
            CharacterProperty character = completeCharacterProps[i];

            character.transform.DOMove(character.startPosition, dropDuration).SetDelay(i * interval).SetEase(dropCurve);

            character.spriteRenderer.DOColor(white, dropDuration).SetDelay(i * interval);

            character.transform.DOScale(1.35f, 0.45f).SetDelay(i * interval + scaleDelay).SetEase(scaleUpCurve);

            character.transform.DOMove(character.startPosition + Vector3.up * dropHeightWorldSpace, liftDuration).SetDelay(i * interval + liftDelay).SetEase(liftCurve);

            character.spriteRenderer.DOColor(invisible, liftDuration).SetDelay(i * interval + liftDelay);
        }
    }
}
