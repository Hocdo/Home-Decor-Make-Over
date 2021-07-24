using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Popup;
using Spine.Unity;
using UnityEngine.Purchasing;

public class PopupGameOutOfMoves : PopupBase
{
	public Transform main;

    public CollectBlockPlayView collectViewSample;

	public Button buttonContinueAds;

	public Button buttonContinueGemReplacement;

	public RectTransform characterTransform;

	public SkeletonGraphic characterGraphic;

	public Text textPrice;

	private UIEdgeSnapPosition characterEdgeSnap;

	private bool invokeDenyEvent = true;

	private static string productId = "rescure_pack";

	private static float priceInUsd = 4.99f;

    private void Start()
    {
		Product product = IAPManager.Instance.GetProduct(productId);

		if (product != null)
		{
			string localizedPriceString = product.metadata.localizedPriceString;
			textPrice.text = localizedPriceString;
		}
		else
		{
			textPrice.text = "$" + priceInUsd.ToString();
		}

		InitRemainingGoals();

		bool allowWatchAdsReward3Moves = !(AppTempData.watch_ads_reward_3moves_count == AppTempData.watch_ads_reward_3moves_limit /*&& RemoteConfig.index_ads_move == 1*/);
			
		buttonContinueAds.gameObject.SetActive(allowWatchAdsReward3Moves);		
		buttonContinueGemReplacement.gameObject.SetActive(!allowWatchAdsReward3Moves);
	}

	private void InitRemainingGoals()
    {
		var collectBlocks = MapData.main.collectBlocks;
		var collectViews = new List<CollectBlockView>();

		for (int j = 0; j < collectBlocks.Length; j++)
		{
			if (string.IsNullOrEmpty(collectBlocks[j].blockType) || collectBlocks[j].count <= 0 || !(collectViewSample != null))
			{
				continue;
			}

			CollectBlockPlayView component = (j == 0) ? collectViewSample
				: Instantiate(collectViewSample).GetComponent<CollectBlockPlayView>();
			collectViews.Add(component);

			if (!component)
			{
				continue;
			}

			if (!string.IsNullOrEmpty(collectBlocks[j].blockType) && collectBlocks[j].count > 0)
			{
				component.gameObject.transform.SetParent(collectViewSample.transform.parent, worldPositionStays: false);
				CollectBlockType collectBlockType = collectBlocks[j].GetCollectBlockType();

				Sprite sprite;
				switch (collectBlockType)
				{
					case CollectBlockType.NormalRed:
					case CollectBlockType.NormalOrange:
					case CollectBlockType.NormalYellow:
					case CollectBlockType.NormalGreen:
					case CollectBlockType.NormalBlue:
					case CollectBlockType.NormalPurple:
						sprite = CollectIconTable.Instance.GetSprite(MapData.main.collectBlocks[j].blockType);
						break;
					default:
						sprite = CollectIconTable.Instance.GetSprite(MapData.main.collectBlocks[j].blockType);
						break;
					case CollectBlockType.Null:
						continue;
				}

				if (sprite != null)
				{
					int num = GameMain.main.countOfEachTargetCount[(int)collectBlockType];

					component.SetData(collectBlockType, collectBlocks[j].count, sprite);
					if (num > 0)
						component.targetCountText.text = num.ToString();
					else
					{
						component.check.gameObject.SetActive(true);
						component.targetCountText.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				DestroyImmediate(component);
			}
		}

		for (int i = 0; i < collectViews.Count; i++)
		{
			collectViews[i].UpdateSize();
		}
	}

    public override void Show()
    {
		invokeDenyEvent = true;
		GetComponent<CanvasGroup>().alpha = 1f;
		canClose = false;

		characterEdgeSnap = new UIEdgeSnapPosition(characterTransform, new Vector2(0f, -1.2f));
		characterEdgeSnap.SetPositionVisibility(false);
		characterEdgeSnap.Show(0.5f).SetEase(Ease.OutBack).OnComplete(() => canClose = true);

		PopupAnimationUtility.AnimateScale(main, Ease.OutBack, 0.25f, 1f, 0.25f, 0f);

		characterGraphic.AnimationState.AddAnimation(0, "lose", true, 0f);
	}

    public override void Close(bool forceDestroying = true)
    {
		characterGraphic.DOFade(0f, 0.075f);

		PopupAnimationUtility.AnimadeAlpha(GetComponent<CanvasGroup>(), Ease.Linear, 1f, 0f, 0.1f, 0f, false);
		PopupAnimationUtility.AnimateScale(main, Ease.OutQuart, 1f, 0.8f, 0.1f, 0f)
            .OnComplete(() =>
			{
				if (invokeDenyEvent && forceDestroying)
					DenyEvent?.Invoke(null);
				
				TerminateInternal(forceDestroying);
			});
    }

    public void ContinueWithGem()
    {
		if (PlayerData.current.gemCount >= 50)
        {
			PlayerData.current.AddGem(-50);

			AcceptEvent?.Invoke(5);
			invokeDenyEvent = false;

			CloseInternal();
		}
        else
        {
			PopupSystem.Instance.ShowPopup(PopupType.PopupGetMoreGems, CurrentPopupBehaviour.HideTemporary, true, true);
			invokeDenyEvent = false;
		}	
    }

	public void ContinueWithGem_Replacement()
	{
		if (PlayerData.current.gemCount >= 30)
		{
			PlayerData.current.AddGem(-30);

			AcceptEvent?.Invoke(3);
			invokeDenyEvent = false;

			CloseInternal();
		}
		else
		{
			PopupSystem.Instance.ShowPopup(PopupType.PopupGetMoreGems, CurrentPopupBehaviour.HideTemporary, true, true);
			invokeDenyEvent = false;
		}
	}

	public void ContinueWithAds()
    {
		Action RewardedVideoReward = () =>
		{
			AppTempData.watch_ads_reward_3moves_count++;

			AcceptEvent?.Invoke(3);
			invokeDenyEvent = false;

			CloseInternal();

			//AppEventTracker.LogEventRewardAd("outofmove_+3", true);

			var playerData = PlayerData.current;
			playerData.tempData.extra3MovesRewardCount++;
			//if (playerData.tempData.extra3MovesRewardCount == 5)
			//{
			//	AppEventTracker.PushEvent5RewardVideos3Move();
			//}
		};

        Action RewardedVideoFailed = () =>
        {
            //AppEventTracker.LogEventRewardAd("outofmove_+3", false);
        };

        AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);
	}

	public void PurchaseButtonClick()
	{
		IAPManager.Instance.PurchaseButtonClick(productId, PurchaseGemSucessful);
	}

	public void PurchaseGemSucessful(PurchaseEventArgs args)
	{
		AudioManager.Instance.PlaySFX(AudioClipId.Purchased);

		PlayerData playerData = PlayerData.current;

        playerData.tempData.spentIAP += priceInUsd;
        if (playerData.tempData.spentIAP >= 5f && playerData.tempData.push_spentIAP_event == false)
        {
            playerData.tempData.push_spentIAP_event = true;
            //AppEventTracker.PushEventSpentIAP_Gequal_5Dollar();
        }

        //AppEventTracker.LogEventIap(productId, priceInUsd.ToString());

        var popupReward = PopupSystem.GetOpenBuilder().SetType(PopupType.PopupReward).SetCurrentPopupBehaviour(CurrentPopupBehaviour.KeepShowing).Open<PopupReward>();

		playerData.AddGem(100);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, 5);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, 5);

		popupReward.Add(RewardType.CurrencyGem, 100);

		popupReward.Add(RewardType.IngameBoosterHammer, 5).CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.HammerChange,
			playerData.match3Data.GetIngameBoosterCount(Match3.BoosterType.Hammer));
		popupReward.Add(RewardType.IngameBoosterCandyPack, 5).CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CandyPackChange,
			playerData.match3Data.GetIngameBoosterCount(Match3.BoosterType.CandyPack)); ;

		popupReward.Align(2f, 3f);
		popupReward.playCollectAnimation = false;
		popupReward.PostAnimateHideEvent = () =>
		{
			AcceptEvent?.Invoke(5);
			invokeDenyEvent = false;

			CloseInternal();
		};
    }
}
