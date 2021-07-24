using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Popup;
using UnityEngine.Purchasing;
using System.Globalization;
using System.Text.RegularExpressions;

public class PopupStarterPack : PopupBase
{
	public float priceInUsd;

    public float discountPercent;

	public string productId;

	public Text priceText;

	public Text fakePriceText;

	public Transform lightTransform;

    public override void Show()
	{
		canClose = false;
		PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);

		if (lightTransform)
			StartCoroutine(LightRotate());

		Product product = IAPManager.Instance.GetProduct(productId);

		if (product != null)
        {
            string localizedPriceString = product.metadata.localizedPriceString;
			priceText.text = localizedPriceString;

            var match = Regex.Match(localizedPriceString, @"([-+]?[0-9]*\.?[0-9]+)");
            if (match.Success)
            {
                string s = match.Groups[1].Value;
                float value = Convert.ToSingle(s);

                float fakePrice = (value) / (100f - discountPercent) * 100f;
                fakePriceText.text = localizedPriceString.Replace(value.ToString(), fakePrice.ToString());
            }           
        }		
    }

	public override void Close(bool forceDestroying = true)
	{
		TerminateInternal(forceDestroying);
	}

	public void BuyButtonPress()
    {
		IAPManager.Instance.PurchaseButtonClick(productId, PurchaseGemSucessful);
	}

	private IEnumerator LightRotate()
    {
		while (true)
        {
			lightTransform.Rotate(new Vector3(0f, 0f, -30f * Time.deltaTime), Space.Self);

			yield return null;
		}		
    }

	public void PurchaseGemSucessful(PurchaseEventArgs args)
	{
        AudioManager.Instance.PlaySFX(AudioClipId.Purchased);

        PlayerData playerData = PlayerData.current;
		playerData.purchasedStartPack = true;

		playerData.AddGem(60);
		playerData.stamina.AddInfinity(3600);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, 1);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, 1);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.HBomb, 1);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.VBomb, 1);

		playerData.tempData.spentIAP += priceInUsd;
		if (playerData.tempData.spentIAP >= 5f && playerData.tempData.push_spentIAP_event == false)
		{
			playerData.tempData.push_spentIAP_event = true;
			//AppEventTracker.PushEventSpentIAP_Gequal_5Dollar();
		}
		//AppEventTracker.LogEventIap(productId, priceInUsd.ToString());

		var popupReward = PopupSystem.GetOpenBuilder().
			SetType(PopupType.PopupReward).
			SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).
			Open<PopupReward>();
        popupReward.Add(RewardType.CurrencyGem, 60)
			.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, playerData.gemCount);
        popupReward.Add(RewardType.CurrencyStaminaInf, 3600)
			.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.StaminaChange);

        popupReward.Add(RewardType.IngameBoosterCandyPack, 1);
        popupReward.Add(RewardType.IngameBoosterHammer, 1);
        popupReward.Add(RewardType.IngameBoosterHRocket, 1);
        popupReward.Add(RewardType.IngameBoosterVRocket, 1);

        popupReward.Align(2f, 3f);

		AcceptEvent?.Invoke(null);
	}
}

public static class StarterPackUtility
{
	public static int match3LevelToShow = 10;

	public static int expiredDuration = 24 * 3600;

	public static int GetRemainingTimeInSeconds()
    {
		var playerData = PlayerData.current;
		
		if (string.IsNullOrEmpty(playerData.starterPackExpiredTime))
        {
			SaveExpireTime();
			return expiredDuration;
		}
        else 
        {
			DateTime now = DateTimeUtility.GetUtcNow();
			DateTime expiredTime = DateTimeUtility.Get(playerData.starterPackExpiredTime);
			if (expiredTime > now)
            {
				TimeSpan timeSpan = expiredTime - now;

				return (int)timeSpan.TotalSeconds;
            }
            else
            {
				SaveExpireTime();
				return expiredDuration;
			}			
        }
	}

	public static void SaveExpireTime()
    {
		DateTime now = DateTimeUtility.GetUtcNow();

		PlayerData.current.starterPackExpiredTime = now.AddSeconds(expiredDuration).ToString();
	}

	public static void ResetExpireTime()
    {
		PlayerData.current.starterPackExpiredTime = "";
	}

	public static bool Available()
    {
		var playerData = PlayerData.current;
		return playerData.match3Data.level >= match3LevelToShow && playerData.purchasedStartPack == false;
	}
}
