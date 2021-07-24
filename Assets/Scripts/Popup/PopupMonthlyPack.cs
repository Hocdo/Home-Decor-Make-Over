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
using System.Text;

public class PopupMonthlyPack : PopupBase
{
	public float priceInUsd;

	public float discountPercent;

	public string productId;

	public Text priceText;

	public Text fakePriceText;

	public Transform lightTransform;

	public Text timeText;

	private Coroutine timeCoroutine;

	public override void Show()
	{
		timeCoroutine = StartCoroutine(TimeUpdate());

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

	public void OnApplicationFocus(bool focus)
	{
		if (timeCoroutine != null) StopCoroutine(timeCoroutine);
		timeCoroutine = StartCoroutine(TimeUpdate());
	}

	public void OnApplicationPause(bool pause)
	{
		if (timeCoroutine != null) StopCoroutine(timeCoroutine);
		timeCoroutine = StartCoroutine(TimeUpdate());
	}

	public IEnumerator TimeUpdate()
	{
		var waitFor1s = new WaitForSeconds(1f);
		var waitFor1h = new WaitForSeconds(3600f);
		var stringBuilder = new StringBuilder();

		float remainingTime = MonthlyPackUtility.GetRemainingTimeInSeconds();
		float bias = 0f;

		if (remainingTime <= 86400f)
		{
			DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
			bias = remainingTime - (int)remainingTime;
			remainingTime -= bias;
		}
		else
		{
			DateTimeUtility.ToDayHour(stringBuilder, remainingTime);
			bias = remainingTime - ((int)(remainingTime / 3600f)) * 3600f;
			remainingTime -= bias;
		}

		timeText.text = stringBuilder.ToString();

		yield return new WaitForSeconds(bias);

		while (true)
		{
			DateTimeUtility.ToDayHour(stringBuilder, remainingTime);
			timeText.text = stringBuilder.ToString();

			if (remainingTime <= 86400f)
			{
				remainingTime -= 1f;

				yield return waitFor1s;
			}
			else
			{
				remainingTime -= 3600f;

				yield return waitFor1h;
			}

			if (remainingTime <= 0f)
			{
				timeText.text = "00:00:00";
			}
		}
	}

	public void PurchaseGemSucessful(PurchaseEventArgs args)
	{
		AudioManager.Instance.PlaySFX(AudioClipId.Purchased);

		PlayerData playerData = PlayerData.current;
		
		playerData.monthlyPackPurchaseNo = MonthlyPackUtility.GetMonthOfYear();
		playerData.noAds = true;

		playerData.AddGem(1000);
		playerData.stamina.AddInfinity(3600);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, 5);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, 5);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.HBomb, 5);
		playerData.match3Data.AddIngameBooster(Match3.BoosterType.VBomb, 5);

		playerData.tempData.spentIAP += priceInUsd;

		if (playerData.tempData.spentIAP >= 5f && playerData.tempData.push_spentIAP_event == false)
		{
			playerData.tempData.push_spentIAP_event = true;
			Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_IAPspend_5usd");
		}

		//AppEventTracker.LogEventIap(productId, priceInUsd.ToString());
		

		var popupReward = PopupSystem.GetOpenBuilder().
			SetType(PopupType.PopupReward).
			SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).
			Open<PopupReward>();

		popupReward.Add(RewardType.CurrencyGem, 1000)
			.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, playerData.gemCount);
		popupReward.Add(RewardType.CurrencyStaminaInf, 3600)
			.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.StaminaChange);

		popupReward.Add(RewardType.IngameBoosterCandyPack, 5);
		popupReward.Add(RewardType.IngameBoosterHammer, 5);
		popupReward.Add(RewardType.IngameBoosterHRocket, 5);
		popupReward.Add(RewardType.IngameBoosterVRocket, 5);

		popupReward.Align(2f, 3f);

		AcceptEvent?.Invoke(null);
	}
}

public static class MonthlyPackUtility
{
	public static int match3LevelToShow = 15;

	public static float GetRemainingTimeInSeconds()
	{
		DateTime now = DateTimeUtility.GetUtcNow();
		int dayCountOfMonth = DateTime.DaysInMonth(now.Year, now.Month);
		TimeSpan timeSpan = TimeSpan.FromDays(dayCountOfMonth - now.Day) + TimeSpan.FromHours(24) - now.TimeOfDay;

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

		return now.Day >= 20
			&& playerData.monthlyPackPurchaseNo != GetMonthOfYear() && playerData.match3Data.level >= match3LevelToShow;
	}

	public static int GetMonthOfYear()
	{
		return DateTimeUtility.GetUtcNow().Month;
	}
}

