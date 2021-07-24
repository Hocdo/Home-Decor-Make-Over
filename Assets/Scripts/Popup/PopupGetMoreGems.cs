using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;

public class PopupGetMoreGems : PopupBase
{
    public Transform mainTransform;

    public GemStorePack[] gemPack;

    public Text gemCountText;

    public Image gemImage;

    public CollectParticles collectParticles;

    private GemStorePack currentPack;

    private PlayerData playerData;

    private bool canPlayAddCoinSFX;

    public override void Show()
	{
        playerData = PlayerData.current;

        gemCountText.text = playerData.gemCount.ToString();
        mainTransform.GetComponent<CanvasGroup>().alpha = 1f;
        canClose = false;

		PopupAnimationUtility.AnimateScale(mainTransform, Ease.OutBack, 0.75f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
	}

	public override void Close(bool forceDestroying = true)
	{
		PopupAnimationUtility.AnimadeAlpha(mainTransform.GetComponent<CanvasGroup>(), Ease.Linear, 1f, 0f, 0.1f, 0f, false);
		PopupAnimationUtility.AnimateScale(mainTransform, Ease.OutQuart, 1f, 0.8f, 0.1f, 0f)
			.OnComplete(() =>
			{
				TerminateInternal(forceDestroying);
			});
	}

	public void Start()
	{
        for (int i = 0; i < gemPack.Length; i++)
        {
            Product product = IAPManager.Instance.GetProduct(gemPack[i].productId);

            if (product != null)
            {
                gemPack[i].SetPrice(product.metadata.localizedPriceString);
            }
            
            gemPack[i].BuyEvent = PurchaseButtonClick;
        }
    }
   
    public void PurchaseButtonClick(GemStorePack gemPack)
    {
        currentPack = gemPack;
        IAPManager.Instance.PurchaseButtonClick(currentPack.productId, PurchaseGemSucessful);
    }

    public void PurchaseGemSucessful(PurchaseEventArgs args)
    {
        //Product product = args.purchasedProduct;
        int currentGemCount = playerData.gemCount;
        playerData.AddGem(currentPack.rewardCount);

        AudioManager.Instance.PlaySFX(AudioClipId.DailyBonus);

        DOTween.To(() => currentGemCount, (value) => currentGemCount = value, playerData.gemCount, collectParticles.GetDuration())
            .SetDelay(collectParticles.GetParticleMoveDuration())
            .OnStart(() =>
            {
                StartCoroutine(PlayAddGemSFXCoroutine());
            })
            .OnUpdate(() => gemCountText.text = currentGemCount.ToString())
            .OnComplete(() => canPlayAddCoinSFX = false);

        collectParticles.targetTransform = gemImage.transform;
        Vector3 targetPosition = collectParticles.targetTransform.position;
        Vector3 sourcePosition = currentPack.GetIconImage().transform.position;
        collectParticles.transform.position = sourcePosition;     
        collectParticles.SetRotateAngle(Mathf.Atan2(targetPosition.y - sourcePosition.y, targetPosition.x - sourcePosition.x) * Mathf.Rad2Deg - 90f);
        collectParticles.SetSpawnRate(20);
        collectParticles.Play();

        EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, playerData.gemCount);

        playerData.tempData.spentIAP += currentPack.priceInUsd;
        if (playerData.tempData.spentIAP >= 5f && playerData.tempData.push_spentIAP_event == false)
        {
            playerData.tempData.push_spentIAP_event = true;
            //AppEventTracker.PushEventSpentIAP_Gequal_5Dollar();
        }

        //AppEventTracker.LogEventIap(currentPack.productId, currentPack.priceInUsd.ToString());    
    }

    public IEnumerator PlayAddGemSFXCoroutine()
    {
        var waitForInterval = new WaitForSeconds(0.15f);

        canPlayAddCoinSFX = true;
        while (canPlayAddCoinSFX)
        {
            AudioManager.Instance.PlaySFX(AudioClipId.AddCoin);

            yield return waitForInterval;
        }
    }
}

