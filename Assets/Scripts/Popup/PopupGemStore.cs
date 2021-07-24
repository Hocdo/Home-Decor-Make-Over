using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;

public class PopupGemStore : PopupBase
{
    [Header("Refs")]
	public BundlePack[] bundlePacks;

    public Image closeButton;

    [Header("Open Animation")]
    public AnimationCurve scaleUpCurve;

    public float scaleUpDuration = 0.5f;

    public float fadeInDuration = 0.5f;

    public float openDelayInterval = 0.1f;

    [Header("Close Animation")]
    public AnimationCurve scaleDownCurve;

    public AnimationCurve fadeOutCurve;

    public float closeDuration = 0.35f;

    public float closeDelayInterval = 0.1f;

    private BundlePack currentPack;

    private float openTime;

    private void Start()
    {
        for (int i = 0; i < bundlePacks.Length; i++)
        {
            Product product = IAPManager.Instance.GetProduct(bundlePacks[i].productId);

            if (product != null)
            {
                bundlePacks[i].SetPrice(product.metadata.localizedPriceString);
            }

            bundlePacks[i].BuyEvent = PurchaseButtonClick;
            bundlePacks[i].index = i;  
        }

        openTime = Time.realtimeSinceStartup;

        //AppEventTracker.LogEventShop(Analytics.Feature_SHOP.ACTION_TYPE._start);
    }

    public override void Show()
	{
        canClose = false;

		for (int i = 0; i < bundlePacks.Length; i++)
        {
            var transform = bundlePacks[i].transform;
            var canvasGroup = transform.GetComponent<CanvasGroup>();
            float delay = (i != 4) ? i * openDelayInterval : (i - 1) * openDelayInterval;

            float scale = transform.localScale.x;
            transform.localScale = Vector3.zero;
            var tween = transform.DOScale(scale, scaleUpDuration).SetDelay(delay).SetEase(scaleUpCurve);

            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeInDuration).SetDelay(delay).SetEase(Ease.Linear);

            if (i == 4)
            {
                tween.OnComplete(() => canClose = true);
            }
        }

        canClose = true;
    }

	public override void Close(bool forceDestroying = true)
	{
        TerminateInternal();

        float openDuration = Time.realtimeSinceStartup - openTime;
        //AppEventTracker.LogEventShop(Analytics.Feature_SHOP.ACTION_TYPE._end,
        //    Analytics.Feature_SHOP.ACTION_NAME.NONE,
        //    Analytics.Feature_SHOP.TYPE_ITEM.NONE,
        //    openDuration);

        //float maxDelayDuration = (bundlePacks.Length - 1) * closeDelayInterval;

        //for (int i = 0; i < bundlePacks.Length; i++)
        //{
        //    var transform = bundlePacks[i].transform;
        //    var canvasGroup = transform.GetComponent<CanvasGroup>();
        //    float delay = maxDelayDuration - ((i != 4) ? i * openDelayInterval : (i - 1) * openDelayInterval);

        //    var tween = transform.DOScale(0f, closeDuration).SetDelay(delay).SetEase(scaleDownCurve);
        //    canvasGroup.DOFade(0f, closeDuration - 1f / 30f).SetDelay(delay).SetEase(fadeOutCurve);

        //    if (i == 0)
        //    {
        //        tween.OnComplete(() => TerminateInternal(forceDestroying));
        //    }
        //}
    }

    public void PurchaseButtonClick(BundlePack gemPack)
    {
        currentPack = gemPack;
        IAPManager.Instance.PurchaseButtonClick(currentPack.productId, PurchaseGemSucessful);
    }

    public void PurchaseGemSucessful(PurchaseEventArgs args)
    {
        AudioManager.Instance.PlaySFX(AudioClipId.Purchased);

        PlayerData playerData = PlayerData.current;

        playerData.tempData.spentIAP += currentPack.priceInUsd;
        if (playerData.tempData.spentIAP >= 5f && playerData.tempData.push_spentIAP_event == false)
        {
            playerData.tempData.push_spentIAP_event = true;
            //AppEventTracker.PushEventSpentIAP_Gequal_5Dollar();
        }

        //AppEventTracker.LogEventIap(currentPack.productId, currentPack.priceInUsd.ToString());

        //Analytics.Feature_SHOP.TYPE_ITEM itemType = Analytics.Feature_SHOP.TYPE_ITEM.NONE;
        //if (currentPack.productId.Contains("bundle"))
        //    itemType = Analytics.Feature_SHOP.TYPE_ITEM._pack;
        //else if (currentPack.productId.Contains("gemshop"))
        //    itemType = Analytics.Feature_SHOP.TYPE_ITEM._gem;

        //AppEventTracker.LogEventShop(Analytics.Feature_SHOP.ACTION_TYPE._action,
        //    Analytics.Feature_SHOP.ACTION_NAME._buy_item,
        //    itemType, 0f, currentPack.productId);

        var popupReward = PopupSystem.GetOpenBuilder().SetType(PopupType.PopupReward).SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).Open<PopupReward>();

        if (currentPack.index == 0)
        {
            playerData.AddGem(1500);

            playerData.match3Data.AddHeadStartBooster(Match3.HeadStartBoosterType.Move, 12);
            playerData.match3Data.AddHeadStartBooster(Match3.HeadStartBoosterType.Bomb, 12);

            playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, 12);
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, 12);
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.HBomb, 12);
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.VBomb, 12);

            popupReward.Add(RewardType.CurrencyGem, 1500)
                .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });

            popupReward.Add(RewardType.StartBoosterMove, 12);
            popupReward.Add(RewardType.StartBoosterBomb, 12);
            popupReward.Add(RewardType.IngameBoosterHammer, 12);
            popupReward.Add(RewardType.IngameBoosterHRocket, 12);
            popupReward.Add(RewardType.IngameBoosterVRocket, 12);
            popupReward.Add(RewardType.IngameBoosterCandyPack, 12);     
        }
        else if (currentPack.index == 1)
        {
            playerData.AddGem(300);

            playerData.match3Data.AddHeadStartBooster(Match3.HeadStartBoosterType.Move, 6);
            playerData.match3Data.AddHeadStartBooster(Match3.HeadStartBoosterType.Bomb, 6);

            playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, 6);
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, 6);
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.HBomb, 6);
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.VBomb, 6);

            popupReward.Add(RewardType.CurrencyGem, 300)
               .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });

            popupReward.Add(RewardType.StartBoosterMove, 6);
            popupReward.Add(RewardType.StartBoosterBomb, 6);
            popupReward.Add(RewardType.IngameBoosterHammer, 6);
            popupReward.Add(RewardType.IngameBoosterHRocket, 6);
            popupReward.Add(RewardType.IngameBoosterVRocket, 6);
            popupReward.Add(RewardType.IngameBoosterCandyPack, 6);
        }
        else if (currentPack.index == 2)
        {
            playerData.AddGem(60);

            playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, 1);
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, 1);
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.VBomb, 1);

            popupReward.Add(RewardType.CurrencyGem, 60)
               .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });

            popupReward.Add(RewardType.IngameBoosterHammer, 1);
            popupReward.Add(RewardType.IngameBoosterVRocket, 1);
            popupReward.Add(RewardType.IngameBoosterCandyPack, 1);
        }
        else if (currentPack.index == 3)
        {
            playerData.AddGem(1450);

            popupReward.Add(RewardType.CurrencyGem, 1450)
              .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });
        }
        else if (currentPack.index == 4)
        {
            playerData.AddGem(500);

            popupReward.Add(RewardType.CurrencyGem, 500)
              .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });
        }

        popupReward.Align(2f, 3f);
    }
}
