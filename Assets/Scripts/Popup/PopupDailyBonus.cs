using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;
using System.Text;

public class PopupDailyBonus : PopupBase
{
	public Transform lightTransform;

	public Transform packTransform;

	public Text nextRewardTime;

	public Button closeButton;

	private UIDailyBonusPack[] dailyBonusUIPack;

	private Coroutine timeCoroutine;

	private bool available;

    public override void Show()
	{
		available = DailyBonusUtility.Available();

		if (available)
        {
			nextRewardTime.gameObject.SetActive(false);
			closeButton.onClick.AddListener(ClaimButtonPressed);
		}
        else
        {
			closeButton.onClick.AddListener(CloseInternal);
			nextRewardTime.gameObject.SetActive(true);
			timeCoroutine = StartCoroutine(TimeUpdate());
		}

        dailyBonusUIPack = new UIDailyBonusPack[packTransform.childCount];
		for (int i = 0; i < dailyBonusUIPack.Length; i++)
		{
			dailyBonusUIPack[i] = packTransform.GetChild(i).GetComponent<UIDailyBonusPack>();
		}

		for (int i = 0; i < dailyBonusUIPack.Length; i++)
		{
			dailyBonusUIPack[i].SetCallback(ClaimButtonPressed, X2BonusPressed);
			dailyBonusUIPack[i].SetDay(i + 1);
		}

		UpdateDailyBonusUIPack();

		StartCoroutine(LightRotate());

		canClose = false;
		PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
	}

	public override void Close(bool forceDestroying = true)
	{
		TerminateInternal(forceDestroying);
	}

	void UpdateDailyBonusUIPack()
    {
		int currentDay = DailyBonusUtility.GetCurrentDay();

		for (int i = 0; i < dailyBonusUIPack.Length; i++)
        {
			if (i < currentDay)
				dailyBonusUIPack[i].SetAsClaimed();
			else if (i == currentDay)
			{
				dailyBonusUIPack[i].SetAsCurrent();
				if (available == false)
                {
					dailyBonusUIPack[i].x2RewardButton.gameObject.SetActive(false);
				}
			}	
			else 
				dailyBonusUIPack[i].SetAsNext();
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
		var stringBuilder = new StringBuilder();

		float remainingTime = 24 * 3600f - (float)DateTime.Now.TimeOfDay.TotalSeconds;
		float bias = remainingTime - (int)remainingTime;
		remainingTime -= bias;

		DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
		nextRewardTime.text = stringBuilder.ToString();

		yield return new WaitForSeconds(bias);

		while (true)
		{
			DateTimeUtility.ToHourMinuteSecond(stringBuilder, (int)remainingTime);
			nextRewardTime.text = stringBuilder.ToString();

			remainingTime -= 1f;

			yield return waitFor1s;

			if (remainingTime <= 0f)
			{
				CloseInternal();
			}
		}
	}

	private IEnumerator LightRotate()
	{
		while (true)
		{
			lightTransform.Rotate(new Vector3(0f, 0f, 30 * Time.deltaTime), Space.Self);

			yield return null;
		}
	}

	public void ClaimButtonPressed()
    {
		if (DailyBonusUtility.Available() == false)
			return;

		ReceiveDailyBonus(1);

		DailyBonusUtility.SaveLastReceiveTimeAsPresent();
		DailyBonusUtility.IncreaseCurrentDay();

		UpdateDailyBonusUIPack();

		//AppEventTracker.LogEventDailyBonus(Analytics.Feature_DAILY_BONUS.ACTION_NAME._claim_reward);
		Firebase.Analytics.FirebaseAnalytics.LogEvent("daily_bonus", new Firebase.Analytics.Parameter("claim_reward", "true"));
	}

	public void X2BonusPressed()
    {
		if (DailyBonusUtility.Available() == false)
			return;

		Action RewardedVideoReward = () =>
		{
			ReceiveDailyBonus(2);

			DailyBonusUtility.SaveLastReceiveTimeAsPresent();
			DailyBonusUtility.IncreaseCurrentDay();

			UpdateDailyBonusUIPack();

			//AppEventTracker.LogEventRewardAd("daily_bonus", true);
			//AppEventTracker.LogEventDailyBonus(Analytics.Feature_DAILY_BONUS.ACTION_NAME._watch_ads);

			Firebase.Analytics.FirebaseAnalytics.LogEvent("daily_bonus", new Firebase.Analytics.Parameter("watch_ads", "true"));
		};

        Action RewardedVideoFailed = () =>
        {
			//AppEventTracker.LogEventRewardAd("daily_bonus", false);
			Firebase.Analytics.FirebaseAnalytics.LogEvent("daily_bonus", new Firebase.Analytics.Parameter("watch_ads", "false"));
		};

        AdvertisementManager.Instance.ShowRewardedVideo(RewardedVideoReward, RewardedVideoFailed);
    }

	private void ReceiveDailyBonus(int multiplier)
    {
		AudioManager.Instance.PlaySFX(AudioClipId.DailyBonus);

		int currentDay = DailyBonusUtility.GetCurrentDay();

		PlayerData playerData = PlayerData.current;

        var popupReward = PopupSystem.GetOpenBuilder().SetType(PopupType.PopupReward).SetCurrentPopupBehaviour(CurrentPopupBehaviour.Close).Open<PopupReward>();

        if (currentDay == 0) // day 1
        {
            int bonusCount = 300 * multiplier;
            playerData.AddCoin(bonusCount);

            popupReward.Add(RewardType.CurrencyCoin, bonusCount)
                .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinAnimChange, new int[] { playerData.cointCount, 1000 });
        }
        else if (currentDay == 1) // day 2
        {
            int bonusCount = 10 * multiplier;
            playerData.AddGem(bonusCount);

            popupReward.Add(RewardType.CurrencyGem, bonusCount)
                .CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });
        }
        else if (currentDay == 2) // day 3
        {
            int bonusCount = 2 * multiplier;
            playerData.match3Data.AddHeadStartBooster(Match3.HeadStartBoosterType.Move, bonusCount);

            popupReward.Add(RewardType.StartBoosterMove, bonusCount);
        }
        else if (currentDay == 3) // day 4
        {
            int bonusCount = 2 * multiplier;
            playerData.match3Data.AddIngameBooster(Match3.BoosterType.CandyPack, bonusCount);

            popupReward.Add(RewardType.IngameBoosterCandyPack, bonusCount);
        }
        else if (currentDay == 4) // day 5
        {
			int bonusCount = 1000 * multiplier;
			playerData.AddCoin(bonusCount);

			popupReward.Add(RewardType.CurrencyCoin, bonusCount)
				.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinAnimChange, new int[] { playerData.cointCount, 1000 });
		}
		else if (currentDay == 5) // day 6
		{
			int bonusCount = 2 * multiplier;
			playerData.match3Data.AddIngameBooster(Match3.BoosterType.Hammer, bonusCount);
			playerData.stamina.AddInfinity(1800 * multiplier);

			popupReward.Add(RewardType.IngameBoosterHammer, bonusCount)
				.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinAnimChange, new int[] { playerData.cointCount, 1000 });
			popupReward.Add(RewardType.CurrencyStaminaInf, 1800 * multiplier)
			.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.StaminaChange);
		}
		else if (currentDay == 6) // day 7
		{
			int bonusCount = 50 * multiplier;
			playerData.AddGem(bonusCount);
           
			popupReward.Add(RewardType.CurrencyGem, bonusCount)
				.CompleteAction = () => EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemAnimChange, new int[] { playerData.gemCount, 1000 });

			if (!playerData.tempData.push_retention_day7)
			{
				playerData.tempData.push_retention_day7 = true;
				Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_retention_day7");
			}
		}

		popupReward.Align(2f, 2f);
    }
}

public static class DailyBonusUtility
{
	public static int GetCurrentDay()
    {
		return PlayerData.current.currentDailyBonusDay;
    }

	public static void IncreaseCurrentDay()
    {
		var playerData = PlayerData.current;
		playerData.currentDailyBonusDay = (playerData.currentDailyBonusDay + 1) % 7;
	}

	public static void SaveLastReceiveTimeAsPresent()
    {
        PlayerData.current.lastReceiveDailyBonusTime = DateTime.Now.ToString();
	}

	public static bool Available()
    {
//#if UNITY_EDITOR
//        return true;
//#endif

        if (AdvertisementManager.Instance.IsRewardedVideoAvailable)
        {
			var playerData = PlayerData.current;

			if (string.IsNullOrEmpty(playerData.lastReceiveDailyBonusTime))
			{
				return true;
			}
			else
			{
				var lastReceiveDateTime = DateTimeUtility.Get(playerData.lastReceiveDailyBonusTime);
				var currentDateTime = DateTime.Now;

				if (lastReceiveDateTime >= currentDateTime
					|| (lastReceiveDateTime.Year == currentDateTime.Year
					&& lastReceiveDateTime.Month == currentDateTime.Month
					&& lastReceiveDateTime.Day == currentDateTime.Day))
				{
					return false;
				}

				return true;
			}
		}

		return false;
    }
}
