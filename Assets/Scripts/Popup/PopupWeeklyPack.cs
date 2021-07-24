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

public class PopupWeeklyPack : PopupBase
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

		playerData.noAds = true;
		playerData.weeklyPackPurchaseNo = WeeklyPackUtility.GetWeekOfYear();

		playerData.AddGem(500);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, 3);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, 3);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.HBomb, 3);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.VBomb, 3);

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
		popupReward.Add(RewardType.CurrencyGem, 500)
			.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, playerData.gemCount);

		popupReward.Add(RewardType.IngameBoosterCandyPack, 3);
		popupReward.Add(RewardType.IngameBoosterHammer, 3);
		popupReward.Add(RewardType.IngameBoosterHRocket, 3);
		popupReward.Add(RewardType.IngameBoosterVRocket, 3);

		popupReward.Align(2f, 3f);

		AcceptEvent?.Invoke(null);
	}
}

public static class WeeklyPackUtility
{
	public static int match3LevelToShow = 15;

	public static float GetRemainingTimeInSeconds()
	{
		var playerData = PlayerData.current;

		DateTime now = DateTimeUtility.GetUtcNow();

		var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
		int remainingDay = 7 - ((now.DayOfWeek - DayOfWeek.Monday) + 7) % 7;

		TimeSpan timeSpan = TimeSpan.FromDays(remainingDay) - now.TimeOfDay;

		return (float)timeSpan.TotalSeconds;
	}

	public static void ResetExpireTime()
	{
		PlayerData.current.starterPackExpiredTime = "";
	}

	public static bool Available()
	{
		var playerData = PlayerData.current;
		var now = DateTimeUtility.GetUtcNow();
		var dayOfWeek = now.DayOfWeek;

		return (/*dayOfWeek == DayOfWeek.Thursday || */dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday) 
			&& playerData.weeklyPackPurchaseNo != GetWeekOfYear() && playerData.match3Data.level >= match3LevelToShow;
	}

	public static int GetWeekOfYear()
    {
		var now = DateTimeUtility.GetUtcNow();
		CultureInfo myCI = new CultureInfo("en-US");
		Calendar myCal = myCI.Calendar;

		return myCal.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
	}
}

