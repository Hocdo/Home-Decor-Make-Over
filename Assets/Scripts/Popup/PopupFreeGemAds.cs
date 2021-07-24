using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Popup;

public class PopupFreeGemAds : PopupBase
{
	public Transform mainTransform;

	public Transform gemPacks;

	public Text gemCountText;

	public Image gemImage;

	public Transform lightTransform;

	public CollectParticles collectParticles;

	private UIFreeGemAdReward[] freeGemAdRewards;

	private PlayerData playerData;

    private bool autoReward = false;

    private bool canWatchAds = true;

	private bool canPlayAddCoinSFX;

	public void AutoReward()
    {
        autoReward = true;
    }

    public void Start()
    {
		playerData = PlayerData.current;
		gemCountText.text = playerData.gemCount.ToString();

		freeGemAdRewards = new UIFreeGemAdReward[gemPacks.childCount];
		for (int i = 0; i < freeGemAdRewards.Length; i++)
        {
			freeGemAdRewards[i] = gemPacks.GetChild(i).GetComponent<UIFreeGemAdReward>();
		}

		UpdateRewardPack();
	}

    public override void Show()
	{
		StartCoroutine(LightRotate());

		mainTransform.GetComponent<CanvasGroup>().alpha = 1f;
		canClose = false;

		PopupAnimationUtility.AnimateScale(mainTransform, Ease.OutBack, 0.75f, 1f, 0.25f, 0f)
			.OnComplete(() => { if (autoReward == false) { canClose = true; } });

        this.ExecuteNextFrame(() =>
        {
            if (autoReward)
                OnRewardSucessful();
        });
        
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

	private IEnumerator LightRotate()
    {
		while (true)
        {
			lightTransform.Rotate(new Vector3(0f, 0f, 30 * Time.deltaTime), Space.Self);

			yield return null;
        }
	}

	private void UpdateRewardPack()
    {
		int rewardedCounter = FreeGemAdsUtility.GetRewardCounter();

		for (int i = 0; i < freeGemAdRewards.Length; i++)
		{
			if (i < rewardedCounter) freeGemAdRewards[i].SetAsClaimed();
			else if (i == rewardedCounter) freeGemAdRewards[i].SetAsCurrent(FreeGemAdsUtility.rewardCounts[i]);
			else freeGemAdRewards[i].SetAsNext(FreeGemAdsUtility.rewardCounts[i]);
		}
	}

	public void WatchAds()
    {
        if (canWatchAds == false) return;

		canClose = false;

		Action RewardedVideoFailed = () =>
		{
			canClose = true;
			canWatchAds = true;
			canPlayAddCoinSFX = false;
			//AppEventTracker.LogEventRewardAd("free_gem_ad_" + FreeGemAdsUtility.GetRewardCounter().ToString(), false);	
			Firebase.Analytics.FirebaseAnalytics.LogEvent("Free_gem_ads", new Firebase.Analytics.Parameter("Rewarded_video_status", "false")) ;
		};
		
		AdvertisementManager.Instance.ShowRewardedVideo(OnRewardSucessful, RewardedVideoFailed);
	}

    private void OnRewardSucessful()
    {
        canWatchAds = false;
        int currentGemCount = playerData.gemCount;
		int rewardCounter = FreeGemAdsUtility.GetRewardCounter();
		int day = rewardCounter + 1;

		UIFreeGemAdReward uiFreeGemAd = freeGemAdRewards[rewardCounter];

        if (rewardCounter == 6 && playerData.tempData.push_freeGems7th_event == false)
        {
            playerData.tempData.push_freeGems7th_event = true;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("feature_selected_predict_ad_gem_end");
        }

        int rewardCount = FreeGemAdsUtility.GetCurrentRewardCount();

		FreeGemAdsUtility.RewardCurrent();
        FreeGemAdsUtility.IncreaseFreeGemAdCounter();

		AudioManager.Instance.PlaySFX(AudioClipId.DailyBonus);

		DOTween.To(() => currentGemCount, (value) => currentGemCount = value, playerData.gemCount, collectParticles.GetDuration())
			.SetDelay(collectParticles.GetParticleMoveDuration())
			.OnStart(() =>
			{
				StartCoroutine(PlayAddGemSFXCoroutine(rewardCount));
			})
			.OnUpdate(() => gemCountText.text = currentGemCount.ToString())
			.OnComplete(() =>
			{
				canWatchAds = true;
				canPlayAddCoinSFX = false;
				canClose = true;

				UpdateRewardPack();			
			});

		Vector3 targetPosition = collectParticles.targetTransform.position;
		Vector3 sourcePosition = uiFreeGemAd.GetIconImage().transform.position;
		collectParticles.transform.position = sourcePosition;
		collectParticles.SetRotateAngle(Mathf.Atan2(targetPosition.y - sourcePosition.y, targetPosition.x - sourcePosition.x) * Mathf.Rad2Deg - 90f);
		collectParticles.SetSpawnRate(rewardCount * 3f);
		collectParticles.Play();

		EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, playerData.gemCount);

		//AppEventTracker.LogEventRewardAd("free_gem_ad", true);
		Firebase.Analytics.FirebaseAnalytics.LogEvent("Free_gem_ads", new Firebase.Analytics.Parameter("Rewarded_video_status", "false"));
	}

	public IEnumerator PlayAddGemSFXCoroutine(int maxCount)
	{
		int count = 0;
		var waitForInterval = new WaitForSeconds(0.15f);

		canPlayAddCoinSFX = true;
		while (canPlayAddCoinSFX)
		{
			AudioManager.Instance.PlaySFX(AudioClipId.AddCoin);
			count++;

			if (count == maxCount)
				break;

			yield return waitForInterval;
		}
	}
}

public static class FreeGemAdsUtility
{
	public static int GetCurrentRewardCount()
	{
		return rewardCounts[PlayerData.current.freeGemAdCounter];
	}

	public static int GetRewardCounter()
	{
		return PlayerData.current.freeGemAdCounter;
	}

	public static void RewardCurrent()
	{
		PlayerData.current.AddGem(GetCurrentRewardCount());
	}

	public static void IncreaseFreeGemAdCounter()
	{
		var playerData = PlayerData.current;
		playerData.freeGemAdCounter = (playerData.freeGemAdCounter + 1) % 7;

	}

	public static int[] rewardCounts = new int[] { 2, 1, 4, 3, 6, 4, 10 };
}