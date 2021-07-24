using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Popup;

public class PopupPiggyBankCollect : PopupBase
{
    [Header("Text")]
    public Text gemText;

    public Text bonusText;

    public Image pigImage;

    [Header("Collect")]
    public GameObject collectGemPrefab;

    public Transform sourceTransform;

    public Transform targetTransform;

    public Transform controlTransform;

    private List<Transform> collectGemTransforms = new List<Transform>();

    private PiggyBankData piggyBankData;

    private int bonusCount;

    private int beforeStashGemCount;

    private int afterStashGemCount;

    private void Start()
    {
        piggyBankData = PlayerData.current.piggyBankData;
        bonusCount = PiggyBankUtility.GetBonusRange(piggyBankData.level).GetRandom();

        bonusText.text = bonusCount.ToString();
        gemText.text = piggyBankData.gemCount.ToString();

        beforeStashGemCount = piggyBankData.gemCount;

        PiggyBankUtility.OnStash(bonusCount);

        afterStashGemCount = piggyBankData.gemCount;

        for (int i = 0; i < 5; i++)
        {
            var collectGem = Instantiate(collectGemPrefab, collectGemPrefab.transform.parent);
            collectGemTransforms.Add(collectGem.transform);
        }
    }

    public override void Show()
    {
        canClose = false;

        PopupAnimationUtility.AnimadeAlpha(canvasGroup, Ease.InOutQuad, 0f, 1f, 0.5f, 0f);

        StartCoroutine(CollectCoroutine());
    }

    public override void Close(bool forceDestroying = true)
    {
        PopupAnimationUtility.AnimadeAlpha(canvasGroup, Ease.InOutQuad, 1f, 0f, 0.5f, 0f).
            OnComplete(() =>
            {
                TerminateInternal(forceDestroying);
                PostAnimateHideEvent?.Invoke();
            });
    }

    private IEnumerator CollectCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        int collectGemCount = collectGemTransforms.Count;

        float moveDuration = 0.5f;
        float moveInterval = 0.2f;

        Vector3 sourcePoint = sourceTransform.position;
        Vector3 targetPoint = targetTransform.position;
        Vector3 controlPoint = controlTransform.position;

        int tweenGemCount = beforeStashGemCount;
        bool isFull = beforeStashGemCount == afterStashGemCount;

        DOTween.To(() => tweenGemCount, (value) => tweenGemCount = value, beforeStashGemCount + bonusCount, moveDuration).
            OnUpdate(() =>
            {
                if (tweenGemCount <= afterStashGemCount)
                {
                    gemText.text = tweenGemCount.ToString();
                }
                else
                {
                    gemText.text = afterStashGemCount.ToString();
                    isFull = true;
                }
            }).
            SetEase(Ease.Linear).SetDelay(moveDuration);

        DOTween.To(() => bonusCount, (value) => bonusCount = value, 0, (collectGemCount - 1) * moveInterval).
            OnUpdate(() => bonusText.text = bonusCount.ToString()).
                SetEase(Ease.Linear);

        for (int i = 0; i < collectGemCount; i++)
        {
            Transform collectGemTransform = collectGemTransforms[i];
            collectGemTransform.gameObject.SetActive(true);

            float t = 0;
            DOTween.To(() => t, (value) => t = value, 1f, moveDuration).
                SetEase(Ease.Linear).
                OnUpdate(() => collectGemTransform.transform.position = (1 - t) * (1 - t) * sourcePoint + 2f * (1 - t) * t * controlPoint + t * t * targetPoint).
                OnComplete(() =>
                {
                    if (!isFull)
                    {
                        collectGemTransform.gameObject.SetActive(false);
                    }
                    else
                    {
                        float angle = UnityEngine.Random.Range(0f, 180f) * Mathf.Deg2Rad;
                        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                        Rigidbody2D rb = collectGemTransform.GetComponent<Rigidbody2D>();
                        rb.bodyType = RigidbodyType2D.Dynamic;
                        rb.AddForce(direction * UnityEngine.Random.Range(40f, 70f));
                        rb.angularVelocity = UnityEngine.Random.Range(400f, 1000f);
                    }

                    AudioManager.Instance.PlaySFX(AudioClipId.AddCoin);
                });

            if (i == collectGemCount - 1)
            {
                bonusText.transform.parent.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(moveInterval);
        }

        yield return new WaitForSeconds(moveDuration + (collectGemCount - 1) * moveInterval + 0.25f);

        canClose = true;
        CloseInternal();
    }
}
